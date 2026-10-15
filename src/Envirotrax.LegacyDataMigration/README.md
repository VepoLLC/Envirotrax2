# Legacy Data Migration

> ⚠️ **LOCAL DATABASES ONLY.** This app writes data with no undo. Never point it at a shared or production connection string — doing so will corrupt or overwrite real data. Always run it against databases on your own machine. See [Prerequisites](#prerequisites) before running.

Moves data from the old (V1) system into the new Envirotrax2 (V2) database.

## How it works

The app migrates one entity at a time — users, water suppliers, supplier users, professionals, GIS areas, sites, renewal email verifications, site logs, backflow gauges, backflow tests, FOG inspections, FOG disposal sites, notification settings, then notifications — in that order, because later entities depend on earlier ones. Each entity's migration has two steps:

1. **Run SQL scripts.** Files under `Scripts/<Entity>/`, named with a numeric prefix (`01_`, `02_`, ...), run in that order. They copy rows from the legacy database into the V2 tables.
2. **Run C# cleanup code.** After the raw data is in place, C# fixes up anything SQL can't do well — legacy passwords come in as plain text and get hashed with ASP.NET Identity's password hasher, and site log file attachments get downloaded from the legacy file server and uploaded to Azure Storage.

`Program.cs` wires up the database connections, the file server address, the storage account and the services, then calls each service's `MigrateAsync()` in order.

**Re-running is safe.** Every insert into a real table is guarded, so running the app again creates no duplicate records and re-does no upload that already succeeded. That is also how you retry: if some attachments failed because the network was down, just run the whole app again — it walks past everything that is already migrated and picks up only what is still pending.

One caveat, on the audit tables rather than the data: `Scripts/Users/02_`–`06_` re-insert into `MigrationSkippedUsers` on every run, and their "UserID already exists in AspNetUsers" reason matches every user the previous run migrated successfully. So after a second run that table describes the state it found, not users it rejected. `MigrationSkippedSites` and `MigrationSkippedSiteLogs` are guarded and stay accurate.

## Project layout

- `Scripts/` — SQL scripts, one subfolder per entity, run in filename order.
- `Services/` — one service per entity (`UserService`, `WaterSupplierService`, `WaterSupplierUserService`, `ProfessionalService`, `GisAreaService`, `SiteService`, `SiteLogService`, `BackflowGaugeService`, `BackflowTestService`, `FogInspectionService`, `FogDisposalSiteService`, `NotificationSettingService`, `NotificationService`, and the other professional services). Each has a `MigrateAsync()` that runs its scripts, then does any C#-side cleanup. `LegacyFileServerService` and `BlobStorageService` are helpers rather than entity migrations: they read files off the legacy file server and write them to Azure Storage.
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
- **The five test images are not migrated yet.** V1 stores no image columns at all: it rebuilds each
  path from the record id (`backflow/<id rounded down to 10000>/<id>-ext|sn|bext|bsn|ag.jpg`) and
  decides whether to show it with `File.Exists`, so which images exist is not knowable from the
  database. Moving them needs a probe-and-upload pass of its own and is tracked separately; the five
  `*ImagePath` columns stay NULL until then.
- **Some V1 columns have no V2 counterpart** and are deliberately dropped: the master water supplier
  and master BPAT ids, the per-component `Repair*Cleaned` / `Repair*Replaced` flags (V2 keeps only
  the `Repair*` booleans and their details), the rain/freeze sensor make and model, `TestType`,
  `PriorInstall` and `ManualUserID`.

Tests that could not be migrated are queryable:

`SELECT * FROM MigrationSkippedBackflowTests` — either the water supplier never migrated, or no
migrated professional user matches the tester login the test was recorded under.

## How FOG inspections are migrated

Like backflow tests, an inspection keeps its V1 id: the insert runs under `IDENTITY_INSERT`, so
`FogInspections.Id` equals the V1 `FogInspections.ID`, and `LegacyRecordId` is written as well because
every guard in the script reads it. Water suppliers, sites, professionals and professional users are
all migrated before it, so nothing here has to be backfilled.

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
V2 requires, so every V1 row migrates and there is no skipped table. A site keeps its V1 id
(`IDENTITY_INSERT`), and `LegacyRecordId` is written as well. That matters later: transporter
registrations and trip tickets both point at a disposal site by its V1 id, so they line up without any
rewriting once they are migrated.

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

## Running it

Before running, double-check every connection string in `Program.cs` points to `localhost` / `(localdb)` — never to a shared server — and check the storage account name right below them. Then run the project. Logs print to the console and to `Logs/<ServiceName>.log`.
