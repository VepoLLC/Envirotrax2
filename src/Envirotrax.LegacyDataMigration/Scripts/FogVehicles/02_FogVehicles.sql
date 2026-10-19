BEGIN TRAN

BEGIN TRY

    -- A vehicle names its owner by login, and V2 hangs vehicles off the company, so the login is
    -- resolved through the staging table that 02_LegacyAccounts.sql fills in, the same way
    -- 02_BackflowGauges.sql resolves a gauge. That table already holds the rule that the company is
    -- MasterTransporterID when there is one and the account's own login otherwise.
    -- The vehicle's own MasterTransporterID is deliberately not used: a vehicle a water supplier adds
    -- for a transporter never sets it (water_suppliers/account_management_transporter.aspx.vb), and a
    -- login rename only rewrites TransporterID (transporters/edit_account.aspx.vb).
    -- Ranking guards against a login appearing twice in FogTransporters. The skip check uses the same
    -- first account as the insert below, so a vehicle is always either migrated or recorded here.
    ;WITH AccountsWithoutDuplicates AS
    (
        SELECT accounts.*,
            ROW_NUMBER() OVER (PARTITION BY accounts.LegacyUserId ORDER BY accounts.LegacyRecordId) AS AccountRank
        FROM MigrationLegacyProfessionalAccounts AS accounts
        WHERE accounts.SourceTable = 'FogTransporters'
    )
    INSERT INTO MigrationSkippedFogVehicles (LegacyRecordId, LegacyUserId, SourceTable, Reason)
    SELECT legacyVehicles.ID, legacyVehicles.TransporterID, 'FogVehicles', 'No migrated transporter company for the vehicle''s TransporterID'
    FROM Vepo.dbo.FogVehicles AS legacyVehicles
    LEFT JOIN AccountsWithoutDuplicates AS accounts
        ON accounts.LegacyUserId = legacyVehicles.TransporterID
        AND accounts.AccountRank = 1
    LEFT JOIN Professionals AS professionals
        ON professionals.LegacyUserId = accounts.LegacyCompanyUserId
    LEFT JOIN MigrationSkippedFogVehicles AS alreadySkipped
        ON alreadySkipped.LegacyRecordId = legacyVehicles.ID
        AND alreadySkipped.SourceTable = 'FogVehicles'
    WHERE professionals.Id IS NULL
        AND alreadySkipped.LegacyRecordId IS NULL

    -- V2 assigns the vehicle a new Id. LegacyRecordId keeps the V1 record id and is what links the row
    -- back to V1: 03_FogVehiclePermits.sql finds a permit's vehicle through it, and so will the trip
    -- ticket migration (FogTripTickets.VehicleID points at a vehicle by its V1 id).
    ;WITH AccountsWithoutDuplicates AS
    (
        SELECT accounts.*,
            ROW_NUMBER() OVER (PARTITION BY accounts.LegacyUserId ORDER BY accounts.LegacyRecordId) AS AccountRank
        FROM MigrationLegacyProfessionalAccounts AS accounts
        WHERE accounts.SourceTable = 'FogTransporters'
    )
    INSERT INTO FogVehicles
        (LegacyRecordId, ProfessionalId, LicensePlateNumber, Manufacturer, ManufacturedYear, Capacity, CapacityType,
         StickerNumber, CreatedById, CreatedTime, DeletedById, DeletedTime)
    SELECT
        legacyVehicles.ID,
        professionals.Id,
        -- V2 requires these three, V1 does not. Empty keeps the vehicle, which trip tickets and permits
        -- point at, rather than dropping it over a blank field.
        LEFT(ISNULL(legacyVehicles.VehicleLicensePlateNumber, ''), 20),
        LEFT(ISNULL(legacyVehicles.VehicleManufacturer, ''), 255),
        ISNULL(legacyVehicles.VehicleYear, 0),
        ISNULL(legacyVehicles.VehicleCapacity, 0),
        -- Value for value: Gallons 0, Cubic Yards 1 in both (FogVolumeUnits in V1).
        ISNULL(legacyVehicles.VehicleCapacityType, 0),
        LEFT(ISNULL(legacyVehicles.VehicleStickerNumber, ''), 50),
        NULL,
        -- Deliberately not defaulted: there is no honest stand-in for when a vehicle was registered, so
        -- a NULL there should stop the run.
        legacyVehicles.CreationDate,
        -- V1 deletes a vehicle outright, so every row it still holds is a live one.
        NULL,
        NULL
    FROM Vepo.dbo.FogVehicles AS legacyVehicles
    INNER JOIN AccountsWithoutDuplicates AS accounts
        ON accounts.LegacyUserId = legacyVehicles.TransporterID
        AND accounts.AccountRank = 1
    INNER JOIN Professionals AS professionals
        ON professionals.LegacyUserId = accounts.LegacyCompanyUserId
    WHERE NOT EXISTS
    (
        SELECT 1
        FROM FogVehicles AS alreadyInserted
        WHERE alreadyInserted.LegacyRecordId = legacyVehicles.ID
    )

    -- A vehicle skipped by an earlier run goes in once its company has been migrated. Its entry no longer
    -- describes anything, so it is cleared to keep this table a list of what is really missing.
    DELETE MigrationSkippedFogVehicles
    FROM MigrationSkippedFogVehicles
    INNER JOIN FogVehicles
        ON FogVehicles.LegacyRecordId = MigrationSkippedFogVehicles.LegacyRecordId
    WHERE MigrationSkippedFogVehicles.SourceTable = 'FogVehicles'

    COMMIT TRAN

END TRY
BEGIN CATCH
    ROLLBACK TRAN;
    THROW;
END CATCH
