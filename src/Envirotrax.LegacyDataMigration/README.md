# Legacy Data Migration

> ⚠️ **LOCAL DATABASES ONLY.** This app writes data with no undo. Never point it at a shared or production connection string — doing so will corrupt or overwrite real data. Always run it against databases on your own machine. See [Prerequisites](#prerequisites) before running.

Moves data from the old (V1) system into the new Envirotrax2 (V2) database.

## How it works

The app migrates one entity at a time — users, water suppliers, supplier users, professionals, GIS areas, sites, renewal email verifications, site logs, backflow gauges, backflow tests, FOG inspections, FOG disposal sites, FOG vehicles and their permits, FOG trip tickets, notification settings, then notifications — in that order, because later entities depend on earlier ones. Each entity's migration has two steps:

1. **Run SQL scripts.** Files under `Scripts/<Entity>/`, named with a numeric prefix (`01_`, `02_`, ...), run in that order. They copy rows from the legacy database into the V2 tables.
2. **Run C# cleanup code.** After the raw data is in place, C# fixes up anything SQL can't do well — legacy passwords come in as plain text and get hashed with ASP.NET Identity's password hasher, and site log file attachments get downloaded from the legacy file server and uploaded to Azure Storage.

`Program.cs` wires up the database connections, the file server address, the storage account and the services, then calls each service's `MigrateAsync()` in order.

**Re-running is safe.** Every insert into a real table is guarded, so running the app again creates no duplicate records and re-does no upload that already succeeded. That is also how you retry: if some attachments failed because the network was down, just run the whole app again — it walks past everything that is already migrated and picks up only what is still pending.

One caveat, on the audit tables rather than the data: `Scripts/Users/02_`–`06_` re-insert into `MigrationSkippedUsers` on every run, and their "UserID already exists in AspNetUsers" reason matches every user the previous run migrated successfully. So after a second run that table describes the state it found, not users it rejected. `MigrationSkippedSites` and `MigrationSkippedSiteLogs` are guarded and stay accurate.

## Project layout

- `Scripts/` — SQL scripts, one subfolder per entity, run in filename order.
- `Services/` — one service per entity (`UserService`, `WaterSupplierService`, `WaterSupplierUserService`, `ProfessionalService`, `GisAreaService`, `SiteService`, `SiteLogService`, `BackflowGaugeService`, `BackflowTestService`, `FogInspectionService`, `FogDisposalSiteService`, `FogVehicleService`, `FogTripTicketService`, `NotificationSettingService`, `NotificationService`, and the other professional services). Each has a `MigrateAsync()` that runs its scripts, then does any C#-side cleanup. `LegacyFileServerService` and `BlobStorageService` are helpers rather than entity migrations: they read files off the legacy file server and write them to Azure Storage.
- `Data/` — EF Core `DbContext`s and entity models for the V2 database.
- `Logs/` — one log file per service, written while the migration runs.

## Prerequisites

> ⚠️ **Do this before running the app.** Skipping this, or pointing a connection string at the shared V2 database, means you will migrate legacy data on top of everyone else's data.

Both databases below must be **local** — on your own machine, not the shared dev/staging V2 database.

1. **Legacy V1 database.** You should already have the old Vepo database running locally.
2. **Empty V2 database.** This doesn't exist until you create it yourself. Launch the `Envirotrax.Auth` and `Envirotrax.App` projects with their connection strings pointed at your local SQL Server. Running them applies EF Core migrations and creates an empty, correctly-shaped V2 database for you.

Only once both databases exist **locally** should you move on to running this project.

3. **Azure sign-in, plus access to the legacy file server.** The site log migration downloads every V1 file attachment from `https://iofiles.envirotrax.com` and uploads it to Azure Blob Storage, so the machine needs outbound access to that server and you need to be signed in to Azure (`az login`, or a signed-in Visual Studio) as an account holding the **Storage Blob Data Contributor** role on the storage account named in `Program.cs`. The app checks this with a probe upload before it starts; if the check fails it logs an error, moves no files, and leaves every attachment pending for a later run.

> ⚠️ **The storage account is shared, and there is no local substitute for it.** Unlike the two databases, blob storage has no `(localdb)` equivalent, so a local run still writes into the real `envirotrax2dev` storage account. Re-running against the **same** V2 database is bounded: the blob name is fixed by the site's row id plus the V1 record id, so a retry overwrites the same file instead of piling up a new one. Rebuilding the V2 database from scratch is **not** bounded — `Sites.Id` is reassigned, so the next run writes the same files under different folders and leaves the previous run's blobs behind. Know what you are pointing at before you run.

## How site log attachments are migrated

V1 kept no path for an attachment; it rebuilt the location from the record id every time it rendered a link. The SQL script therefore writes that rebuilt location into `SiteLogs.LegacyFilePath` (for example `site_log/17390000/17394521.pdf`), leaves `FileAttachmentPath` NULL, and sets `SkipFile`. `SiteLogService` then downloads each file, uploads it to Azure, and in one save writes `FileAttachmentPath` and clears `SkipFile`.

This ordering is what keeps the UI honest: while an attachment is still pending or has permanently failed, V2 shows its file name as plain text rather than a link that goes nowhere; it becomes a link only once the file is provably in Azure.

Two queries tell you where things stand:

- **Still pending:** `SELECT COUNT(*) FROM SiteLogs WHERE LegacyFilePath IS NOT NULL AND FileAttachmentPath IS NULL`
- **Permanently unmigratable:** `SELECT * FROM MigrationSkippedSiteLogs WHERE SourceTable = 'CsiBackflowSiteLog.FileAttachment'` — V1 rows whose file type is missing or unusable, so there is no legacy file name to fetch. Everything else is logged to `Logs/SiteLogService.log` with per-run counts.

## How renewal opt-in is migrated

Site owners switch their renewal letters to email with the one-time code printed on a letter, then
confirm each address through a link sent to it. Both have to survive the move to V2.

- **Opt-in codes and choices** travel with the site in `Scripts/Sites/02_Sites.sql`. V1 prints a new
  code on every letter run, so a re-run refreshes the code and the opt-in choice of sites migrated by an
  earlier run, whichever side changed last. A code already used in V2 is never brought back.
- **Pending email verifications** are copied by `Scripts/RenewalEmailVerifications`. Only rows that are
  neither verified nor unsubscribed move; expired ones move too, so their link reports "expired" rather
  than "invalid". Each row keeps its V1 id, because the link in the email carries it, and its V1 BCrypt
  token hash, which V2 still accepts.

Verifications that could not be migrated are queryable:

`SELECT * FROM MigrationSkippedRenewalEmailVerifications` — either the site never migrated, or its V1 id
was already taken by a verification created in V2.

## How notification settings are migrated

V1 keeps a backflow notification rule per recipient in `WaterSupplierNotificationSettings`, naming that
recipient by login rather than by id. V2 requires the recipient (`NotificationSettings.UserId` is NOT
NULL), so the script resolves the login through `AspNetUsers.Email` to the supplier user of that same
supplier, and records any rule it cannot place instead of dropping it silently.

Three V1 shapes change on the way across:

- **Colour** is an index in V1 (0-11) that only became a hex string when a page rendered it. V2 stores
  the hex string itself, so the script applies the same table `ColorString` used, and the same
  clamp-to-white fallback the editor applied before saving.
- **Backflow test type** is its own V1 enum sitting one below `BackflowReasonForTest`. Its `Any` member
  has no V2 counterpart: V2 leaves `ReasonForTest` NULL to mean the same thing.
- **Two filter columns** are misspelled `...Exeeded` in V1 and corrected in V2.

Rules that could not be migrated are queryable:

`SELECT * FROM MigrationSkippedNotificationSettings` — either the water supplier never migrated, or no
migrated supplier user matches the recipient login the rule was addressed to.

## How notifications are migrated

A notification is a raised alert rather than a rule, so it carries the matched rule flattened onto it
plus a pointer to the record that triggered it. The same three shape changes as the settings apply —
colour index to hex, `Any` test type to a NULL `ReasonForTest`, and the `...Exeeded` spelling fix —
and the recipient login is resolved the same way, with unplaceable rows recorded in
`MigrationSkippedNotifications`.

Four things are specific to notifications:

- **`RecordId` is carried over unresolved, and only backflow tests need to catch up.** It is a
  pointer read through `ModuleType`, and V2 puts no foreign key on it. In V1 it is always a backflow
  test id: `ValidateBackflowTest` is the only factory that produces a notification and it hardcodes
  the module type, while the CSI, FOG and transporter checkouts that appear to raise one each iterate
  a collection whose only `Add` is commented out, so no CSI notification can exist. EN-241 made a
  migrated row keep its V1 id, so these pointers line up by themselves once the **backflow test**
  migration lands — provided it inserts with `IDENTITY_INSERT` the way sites and water suppliers do.
  Nothing needs rewriting afterwards, and no CSI inspection migration is needed for this to resolve.
- **`MasterWaterSupplierID` is dropped.** V1 shows a notification under either that or
  `WaterSupplierID`; V2 removed the master column (EN-184), so the owning `WaterSupplierID` is the
  one carried over.
- **`HazardTypeFountainsGardenPondsWaterFeatures` is always false.** The V1 notifications table has
  no such column — neither `WaterSupplierNotification.Save` nor its `FromReader` touches one — even
  though the settings table does, so V1 could never record that hazard against a notification.
- **`SentTime` stays NULL.** V1 records no send time; its scheduled jobs select by `CreationDate`
  and interval instead of stamping a row once it has gone out.

## How backflow tests are migrated

This is the largest entity in the system: 176 columns, 135 of which carry the same name in both
databases. Everything it points at — water suppliers, sites, professionals, professional users,
supplier users and the seeded states — is migrated before it, so nothing here has to be backfilled.

Like sites and water suppliers, a test keeps its V1 id: the insert runs under `IDENTITY_INSERT`, so
`BackflowTests.Id` equals `SaveBackflowDeviceTests.ID`. That is what makes two earlier migrations
resolve without any rewriting — `Notifications.RecordId` and `SiteLogs.LegacyAssemblyRecordId` both
reference a test by its V1 id. `LegacyRecordId` is written as well, because `Id` alone cannot say
whether a row came from V1 or was created in V2, and every guard in this script reads it.

Four things are worth knowing:

- **A test carries a BPAT or a CSI inspector, never both.** V2 gives them one shared
  `ProfessionalId`, so the BPAT login is preferred and the inspector is left NULL when one resolves.
  Mixing them would point the composite foreign key at a person from another company.
  A test whose tester login matches no migrated professional user is recorded as skipped, because
  `ProfessionalId` is NOT NULL.
- **V2 made 55 value columns NOT NULL that V1 leaves nullable.** Each falls back to `0`, so a single
  legacy NULL cannot roll back the whole run. `CreatedTime` is deliberately not defaulted: there is
  no honest stand-in for when a test was recorded, so a NULL there should stop the run.
- **Text is copied without `LEFT()`.** V1 declares eight text columns wider than V2 does (the BPAT
  company, contact, address, city and ZIP, the property and mailing ZIPs, and the gauge serial
  number), but in the real V1 data only `MailingZIP` ever holds a longer value: seven tests from one
  misaligned import carry a 23-character phone number there. `MailingZip` on tests was widened to 50
  to match sites and FOG inspections, so nothing is cut, and a value that did not fit would stop the
  run instead of being shortened.
- **Some V1 columns have no V2 counterpart** and are deliberately dropped: the master water supplier
  and master BPAT ids, the per-component `Repair*Cleaned` / `Repair*Replaced` flags (V2 keeps only
  the `Repair*` booleans and their details), the rain/freeze sensor make and model, `TestType`,
  `PriorInstall` and `ManualUserID`.

Tests that could not be migrated are queryable:

`SELECT * FROM MigrationSkippedBackflowTests` — either the water supplier never migrated, or no
migrated professional user matches the tester login the test was recorded under.

### How backflow test images are migrated

A test has up to five images: the assembly and its serial number, the bypass assembly and its serial
number (detectors only: DCD, DCD2, RPPD, RPPD2), and the air gap (air gaps only, which have no other).
They are files on the legacy file server. V1 stores no path for them: every page rebuilds it from the
record id (`backflow/<id rounded down to 10000>/<id>-ext.jpg`, or `-sn`, `-bext`, `-bsn`, `-ag`) and
shows the image when that file exists. Which images exist is only partly recorded:

- **A tester's upload** is also written to `SaveBackflowDeviceImages`, where an image type that is not
  NULL means the file was written. About 132,000 images are known this way.
- **A water supplier's upload** writes only the file. The one trace it leaves is the save around it,
  which stamps the test's `LastModifiedBy` (or `ManualUserID`, on a test the supplier entered). For
  such a test every image its device type can have is queued as a guess, about 181,000 more requests,
  and most of them are expected to find nothing.

`03_BackflowTestImages.sql` writes those paths into `LegacyAssemblyImagePath`,
`LegacySerialNumberImagePath`, `LegacyBypassAssemblyImagePath`, `LegacyBypassSerialNumberImagePath` and
`LegacyAirGapImagePath`, sets `LegacyImagesUnrecorded` on a test a supplier saved, and marks every test it
looked at with `LegacyImagesQueued` so it never queues the same test twice.

`BackflowTestService` then moves the images eight at a time, uploading each to
`backflow-tests/<test id>/<assembly|serial-number|bypass-assembly|bypass-serial-number|air-gap>/legacy-<V1 id>.jpg`,
the folders V2 uses when a supplier replaces a test image, and writes `AssemblyImagePath` and the
others once a batch is up, in one save. A migrated test keeps its V1 id, so unlike the other image
migrations these names survive a rebuild of the V2 database, and a re-run overwrites them in place.
A guess the file server does not have loses its legacy path, so no later run asks for it again; a
recorded image it does not have stays queued and is tried again, the way the other image migrations
treat a missing file. As with site log attachments, an image V2 has no path for yet shows as missing
rather than as a broken link.

- **Still pending:** `SELECT COUNT(*) FROM BackflowTests WHERE (LegacyAssemblyImagePath IS NOT NULL AND AssemblyImagePath IS NULL) OR (LegacySerialNumberImagePath IS NOT NULL AND SerialNumberImagePath IS NULL) OR (LegacyBypassAssemblyImagePath IS NOT NULL AND BypassAssemblyImagePath IS NULL) OR (LegacyBypassSerialNumberImagePath IS NOT NULL AND BypassSerialNumberImagePath IS NULL) OR (LegacyAirGapImagePath IS NOT NULL AND AirGapImagePath IS NULL)`
- **Not found in V1:** each recorded image the legacy file server no longer has is logged to
  `Logs/BackflowTestService.log`; guesses that found nothing are only counted, per run.

## How FOG inspections are migrated

V2 gives each inspection a new `Id`; the V1 `FogInspections.ID` goes into `LegacyRecordId`, which is
what links the row back to V1 and what every guard in the script reads. (Only backflow tests, water
suppliers and sites keep their V1 ids, because the customer-facing legacy API exposes them.) Water
suppliers, sites, professionals and professional users are all migrated before it, so nothing here has
to be backfilled.

Five things are worth knowing:

- **The inspector is matched within the company the inspection was filed under.** V1 names the
  inspector by login (`InspectorID`) and their company by `MasterInspectorID`, which is empty when the
  inspector is the company. One login can belong to more than one company in V2, so the script prefers
  the pairing in that company, then any pairing flagged as a FOG inspector. `ProfessionalId` and
  `InspectorId` are NOT NULL, so an inspection whose login matches no migrated professional user is
  recorded as skipped.
- **The site is required.** `SiteId` is NOT NULL in V2, so an inspection whose site did not migrate is
  recorded as skipped too. If a later run migrates it, its entry in the skipped table is cleared.
- **An unpaid inspection has a NULL `TransactionId`.** V1 marks one with an empty string, and some V2
  checks, such as the one that blocks downloading the report before payment, only test for NULL.
- **Facility type and coordinates change shape.** `FacilityType` is the text of a dropdown in V1 and an
  enum in V2; it goes through the same table as sites. The interceptor's latitude and longitude are
  text in V1 and numbers in V2, and a blank one becomes NULL rather than 0.
- **Some columns have no counterpart on the other side.** The master water supplier ids,
  `MasterInspectorID` (used only for the match above) and `InterceptorPhotograph` (read by V1 but never
  written) are dropped. `InspectorJobTitle`, `LastTripTicketDate`, `TripTicketInterval` and `EmailPdf`
  stay empty: V1 stores no job title or "email PDF" flag on a FOG inspection, and keeps the trip
  ticket schedule on the site, which the sites migration already carries over.

Inspections that could not be migrated are queryable:

`SELECT * FROM MigrationSkippedFogInspections` — the water supplier or the site never migrated, or no
migrated professional user matches the inspector login.

### How FOG inspection images are migrated

An inspection has up to three images, and V1 keeps them in two different places:

- **The exterior and interior photos** are files on the legacy file server. V1 stores no path for
  them: every page rebuilds it from the record id (`fog_inspections/<id rounded down to 10000>/<id>-ext.jpg`,
  or `-int.jpg`) and shows the photo whenever `OutsideImageType` (or `InsideImageType`) is not NULL.
  The script applies the same rule and writes that path into `LegacyExteriorImagePath` /
  `LegacyInteriorImagePath`.
- **The signature** is a PNG stored in the V1 `FogInspections.Signature` column. The script sets
  `HasLegacySignature`, and the service reads the bytes back out of the legacy database.

`FogInspectionService` then uploads each image to
`professionals/<ProfessionalId>/fog-inspections/<exterior|interior|signature>/legacy-<V1 id>.<jpg|png>`,
the same folders V2 uses, and only after that writes `ExteriorImagePath`, `InteriorImagePath` or
`SignatureImagePath`. As with site log attachments, an image V2 has no path for yet shows as missing
rather than as a broken link, and a re-run picks up only what is still pending. The same storage
caveat applies too: the folder is the V2 `ProfessionalId`, which a rebuild of the V2 database reassigns.

- **Still pending:** `SELECT COUNT(*) FROM FogInspections WHERE (LegacyExteriorImagePath IS NOT NULL AND ExteriorImagePath IS NULL) OR (LegacyInteriorImagePath IS NOT NULL AND InteriorImagePath IS NULL) OR (HasLegacySignature = 1 AND SignatureImagePath IS NULL)`
- **Not found in V1:** logged to `Logs/FogInspectionService.log` with per-run counts, for example a
  photo the legacy file server no longer has.

## How FOG disposal sites are migrated

Disposal sites are one global master list, not owned by a water supplier, and a site points at nothing
V2 requires, so every V1 row migrates and there is no skipped table. V2 gives each site a new `Id`; the
V1 id goes into `LegacyRecordId`. That matters later: transporter registrations and trip tickets both
point at a disposal site by its V1 id, so their migrations look the site up through `LegacyRecordId`.

Three V1 shapes change on the way across:

- **`Active` becomes a soft delete.** V2 has no active flag; retiring a site there is a delete that
  only sets `DeletedTime`. V1 only offers active sites for registration, so an inactive (or NULL) one
  arrives deleted, with the migration time as `DeletedTime` because V1 records no date for switching a
  site off. The row stays, so registrations and trip tickets that point at it still resolve. Unlike
  V1, a deleted site is also left out of a transporter's own list, so it can no longer be picked on a
  new trip ticket.
- **`PhysicalType`** is free text in V1 and an enum in V2. "Landfill", "Transfer Station" and
  "Processing Facility" (with or without the space) map to their V2 values, and anything else becomes
  Other. V1 web only ever displays this text; the list itself is maintained in the Management Studio,
  whose source is not available.
- **Coordinates and state.** Latitude and longitude are text in V1 and numbers in V2, and a blank one
  becomes NULL rather than 0. The state is matched by code or by full name, since it is typed in by hand.

The five columns V2 requires (name, county, TCEQ region, registration number, permit number) fall back
to an empty string when V1 leaves them NULL, so one blank field cannot drop a site that other records
point at.

## How FOG vehicles and permits are migrated

`Scripts/FogVehicles` moves both: `02_FogVehicles.sql` the vehicles, then `03_FogVehiclePermits.sql` the
permits water suppliers issue for them. V2 gives each vehicle a new `Id`; the V1 id goes into
`LegacyRecordId`, which is how the permit script, and later the trip ticket migration, find a vehicle.
A V2 permit has no id of its own (it is keyed by water supplier and vehicle), so `LegacyRecordId` on it
is the only way back to V1.

Four things are worth knowing:

- **A vehicle belongs to its transporter's company.** V1 names the owner by login (`TransporterID`),
  resolved through `MigrationLegacyProfessionalAccounts` exactly as gauges are, which already applies
  the master/sub-account rule. The vehicle's own `MasterTransporterID` is not used: a vehicle a water
  supplier adds never sets it, and a login rename only rewrites `TransporterID`.
- **One permit per vehicle.** V2 enforces it with a unique index, and V1 does the same in practice:
  issuing a permit first deletes every permit the vehicle already has, whichever supplier issued it.
  Older V1 rows can still hold several, so the newest one is kept (ID breaks a tie) and the rest are
  recorded as superseded. If V1 issues a newer permit after a vehicle's first permit was migrated, a
  re-run records it instead of replacing the one already in V2.
- **Deleted vehicles leave permits behind.** V1 deletes a vehicle outright and leaves its permits in
  place, so a permit whose vehicle is gone is recorded as skipped. Every vehicle that does migrate is a
  live one, so `DeletedTime` stays NULL.
- **Dropped:** the permit's `MasterWaterSupplierID`, since V2 removed master suppliers (EN-184).

Vehicles and permits that could not be migrated are queryable, told apart by `SourceTable`:

`SELECT * FROM MigrationSkippedFogVehicles` — a vehicle whose transporter login has no migrated company,
or a permit whose water supplier or vehicle did not migrate, or that lost to a newer permit for the same
vehicle.

## How FOG trip tickets are migrated

V2 gives each ticket a new `Id`; the V1 `FogTripTickets.ID` goes into `LegacyRecordId`. A ticket points at
a site, a transporter, a vehicle, a disposal site and sometimes the supplier user who approved it. All of
them are migrated before it, and each is found through its own `LegacyRecordId`, or by login for people.

Five things are worth knowing:

- **The transporter is matched within the company the ticket was filed under**, as FOG inspections are.
  V2 requires the company (`ProfessionalId`) but not the user (`TransporterId`), so a ticket whose
  transporter login did not migrate still goes in under the company it was filed under
  (`MasterTransporterID`, or the transporter's own login when they are the company). Only a ticket with
  neither is skipped. A ticket whose site did not migrate is skipped too, because `SiteId` is required.
- **Vehicles and disposal sites are optional links.** V1 deletes a vehicle outright, so a ticket can name
  a vehicle that no longer exists; `VehicleId` is then NULL, and the vehicle columns on the ticket still
  say which vehicle it was. `ReceiverDisposalSiteId` is set the same way. `ReceiverSiteId` stays NULL, as
  it does on the tickets V2 creates, because V2 never reads it.
- **Text is copied without `LEFT()`.** V1 declares every text column no wider than its V2 column, so
  nothing can be cut, and a value that did not fit would stop the run instead of being shortened.
- **Values that change shape:** an unpaid ticket's empty `TransactionID` becomes NULL, as for FOG
  inspections. A capacity type of `-1` (V1's "not selected", shown with no unit) is kept as it is rather
  than turned into a unit. `UpdatedBy` and `UpdatedTime` come from `LastModifiedBy` and `LastModifiedDate`,
  which V1 writes only when a supplier user approves or disapproves a ticket; `ApprovedBy` is resolved the
  same way, to that supplier's user.
- **Dropped:** the master water supplier ids, since V2 removed master suppliers (EN-184).

Tickets that could not be migrated are queryable:

`SELECT * FROM MigrationSkippedFogTripTickets` — the water supplier or the site never migrated, or
neither the transporter nor their company did.

### How FOG trip ticket signatures are migrated

V1 stores three signatures on each ticket as PNG bytes: the generator's and the receiver's, captured on
the ticket, and a copy of the transporter's own signature taken when the ticket is submitted. The script
sets `HasLegacyTransporterSignature`, `HasLegacyGeneratorSignature` and `HasLegacyReceiverSignature`.
`FogTripTicketService` then reads the bytes out of the legacy database a batch of tickets at a time,
uploads each to `professionals/<ProfessionalId>/fog-trip-tickets/<transporter|generator|receiver>/legacy-<V1 id>.png`,
and only after that writes the matching `…SignaturePath`.

V2 already uses the generator and receiver folders. It keeps no per-ticket transporter signature (a
ticket it creates points at the transporter's own signature), so the copy V1 kept on each ticket gets a
`transporter` folder of its own. V1 has no transporter signature date; `TransporterSignatureDate` is the
ticket's creation date, the moment V1 copied the signature and V2 would stamp it.

- **Still pending:** `SELECT COUNT(*) FROM FogTripTickets WHERE (HasLegacyTransporterSignature = 1 AND TransporterSignaturePath IS NULL) OR (HasLegacyGeneratorSignature = 1 AND GeneratorSignaturePath IS NULL) OR (HasLegacyReceiverSignature = 1 AND ReceiverSignaturePath IS NULL)`
- **Not found in V1:** logged to `Logs/FogTripTicketService.log` with per-run counts.

## How backflow renewal requirements are migrated

**Note:** Re-running the migration after renewal requirements have been deleted or copied in V2 may restore
original V1 records.

## Running it

Before running, double-check every connection string in `Program.cs` points to `localhost` / `(localdb)` — never to a shared server — and check the storage account name right below them. Then run the project. Logs print to the console and to `Logs/<ServiceName>.log`.
