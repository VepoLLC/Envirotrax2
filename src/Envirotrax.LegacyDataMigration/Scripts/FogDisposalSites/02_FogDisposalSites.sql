BEGIN TRAN

BEGIN TRY

    -- The V1 record id becomes the V2 row id, the way sites, water suppliers and backflow tests are
    -- migrated (EN-241). Transporter registrations (FogDisposalSiteRegistrations.DisposalSiteID) and
    -- trip tickets (FogTripTickets.ReceiverSiteID) point at a disposal site by its V1 id, so they line
    -- up by themselves once they are migrated.
    SET IDENTITY_INSERT FogDisposalSites ON;

    INSERT INTO FogDisposalSites
        (Id, LegacyRecordId, Name, Address, City, StateId, ZipCode, PhoneNumber, EmailAddress,
         County, TceqRegion, RegistrationNumber, PermitNumber, PhysicalType, LocationDescription,
         Latitude, Longitude, CreatedById, CreatedTime, DeletedById, DeletedTime)
    SELECT
        legacyDisposalSites.ID,
        legacyDisposalSites.ID,
        -- V2 requires the name, county, TCEQ region, registration number and permit number; V1 does
        -- not. Empty keeps the site, which trip tickets and registrations point at, rather than dropping
        -- it over a blank field.
        LEFT(ISNULL(legacyDisposalSites.Name, ''), 255),
        LEFT(legacyDisposalSites.Address, 255),
        LEFT(legacyDisposalSites.City, 50),
        states.Id,
        LEFT(legacyDisposalSites.ZIP, 50),
        LEFT(legacyDisposalSites.PhoneNumber, 50),
        LEFT(legacyDisposalSites.EmailAddress, 255),
        LEFT(ISNULL(legacyDisposalSites.County, ''), 50),
        LEFT(ISNULL(legacyDisposalSites.TceqRegion, ''), 50),
        LEFT(ISNULL(legacyDisposalSites.RegistrationNumber, ''), 50),
        LEFT(ISNULL(legacyDisposalSites.PermitNumber, ''), 50),
        -- V1 keeps the physical type as free text, V2 as an enum. V1 web only ever displays it
        -- (transporters/disposal_site_registration.aspx.vb, transporters/main.aspx.vb); the list is
        -- maintained in the Management Studio, whose source is not available, so both spellings of each
        -- V2 name are accepted and anything else becomes Other.
        CASE LTRIM(legacyDisposalSites.PhysicalType)
            WHEN 'Landfill' THEN 1
            WHEN 'Transfer Station' THEN 2 WHEN 'TransferStation' THEN 2
            WHEN 'Processing Facility' THEN 3 WHEN 'ProcessingFacility' THEN 3
            ELSE 0
        END,
        LEFT(legacyDisposalSites.LocationDescription, 255),
        -- V1 keeps the coordinates as text and V2 as numbers; NULLIF stops a blank one from converting
        -- to 0, which would pin the site to latitude/longitude 0,0.
        TRY_CAST(NULLIF(legacyDisposalSites.Latitude, '') AS FLOAT),
        TRY_CAST(NULLIF(legacyDisposalSites.Longitude, '') AS FLOAT),
        NULL,
        -- Deliberately not defaulted: there is no honest stand-in for when a site was added, so a NULL
        -- there should stop the run.
        legacyDisposalSites.CreationDate,
        NULL,
        -- V2 has no Active flag; retiring a disposal site is a soft delete there. V1 only offers active
        -- sites for registration (transporters/disposal_site_registration.aspx.vb: WHERE Active = 1), so
        -- an inactive one is migrated as deleted. V1 records no date for switching a site off, so the time
        -- of the migration stands in. The row itself is kept, because registrations and trip tickets
        -- still point at it.
        CASE WHEN ISNULL(legacyDisposalSites.Active, 0) = 0 THEN GETUTCDATE() END
    FROM Vepo.dbo.FogDisposalSites AS legacyDisposalSites
    -- The address is shown as "City, State  ZIP" in V1, so this is normally a state code, but it is typed
    -- into the Management Studio, so a full state name is matched as well.
    LEFT JOIN States AS states
        ON states.Code = legacyDisposalSites.State
        OR states.Name = legacyDisposalSites.State
    WHERE NOT EXISTS
    (
        SELECT 1
        FROM FogDisposalSites AS alreadyInserted
        WHERE alreadyInserted.LegacyRecordId = legacyDisposalSites.ID
    )

    SET IDENTITY_INSERT FogDisposalSites OFF;

    COMMIT TRAN

END TRY
BEGIN CATCH
    ROLLBACK TRAN;
    THROW;
END CATCH
