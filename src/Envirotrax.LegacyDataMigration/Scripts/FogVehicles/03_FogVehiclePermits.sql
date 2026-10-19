BEGIN TRAN

BEGIN TRY

    INSERT INTO MigrationSkippedFogVehicles (LegacyRecordId, LegacyUserId, SourceTable, Reason)
    SELECT legacyPermits.ID, NULL, 'FogVehiclePermits', 'No matching migrated water supplier'
    FROM Vepo.dbo.FogVehiclePermits AS legacyPermits
    LEFT JOIN WaterSuppliers
        ON WaterSuppliers.LegacyRecordId = legacyPermits.WaterSupplierID
    LEFT JOIN MigrationSkippedFogVehicles AS alreadySkipped
        ON alreadySkipped.LegacyRecordId = legacyPermits.ID
        AND alreadySkipped.SourceTable = 'FogVehiclePermits'
    WHERE WaterSuppliers.Id IS NULL
        AND alreadySkipped.LegacyRecordId IS NULL

    -- V1 deletes a vehicle without deleting its permits (transporters/vehicles.aspx.vb), so a permit can
    -- outlive the vehicle it was issued for. VehicleId is NOT NULL in V2.
    INSERT INTO MigrationSkippedFogVehicles (LegacyRecordId, LegacyUserId, SourceTable, Reason)
    SELECT legacyPermits.ID, NULL, 'FogVehiclePermits', 'No migrated vehicle: it was deleted in V1 or skipped by 02_FogVehicles.sql'
    FROM Vepo.dbo.FogVehiclePermits AS legacyPermits
    INNER JOIN WaterSuppliers
        ON WaterSuppliers.LegacyRecordId = legacyPermits.WaterSupplierID
    LEFT JOIN FogVehicles
        ON FogVehicles.LegacyRecordId = legacyPermits.VehicleID
    LEFT JOIN MigrationSkippedFogVehicles AS alreadySkipped
        ON alreadySkipped.LegacyRecordId = legacyPermits.ID
        AND alreadySkipped.SourceTable = 'FogVehiclePermits'
    WHERE FogVehicles.Id IS NULL
        AND alreadySkipped.LegacyRecordId IS NULL

    -- V2 keeps one permit per vehicle (a unique index on FogVehiclePermits.VehicleId), and so does V1 in
    -- practice: issuing a permit first deletes every permit the vehicle already has, whichever water
    -- supplier issued it (water_suppliers/fog_vehicle_management.aspx.vb). Older rows can still hold
    -- several, so the newest is kept - the one V1 itself would have kept - and ID breaks a tie. The
    -- skip check and the insert below rank the same way, so a permit is always either migrated or
    -- recorded here.
    ;WITH RankedPermits AS
    (
        SELECT legacyPermits.*,
            ROW_NUMBER() OVER (PARTITION BY legacyPermits.VehicleID ORDER BY legacyPermits.CreationDate DESC, legacyPermits.ID DESC) AS PermitRank,
            WaterSuppliers.Id AS MigratedWaterSupplierId,
            FogVehicles.Id AS MigratedVehicleId
        FROM Vepo.dbo.FogVehiclePermits AS legacyPermits
        INNER JOIN WaterSuppliers
            ON WaterSuppliers.LegacyRecordId = legacyPermits.WaterSupplierID
        INNER JOIN FogVehicles
            ON FogVehicles.LegacyRecordId = legacyPermits.VehicleID
    )
    INSERT INTO MigrationSkippedFogVehicles (LegacyRecordId, LegacyUserId, SourceTable, Reason)
    SELECT rankedPermits.ID, NULL, 'FogVehiclePermits',
        CASE
            WHEN rankedPermits.PermitRank > 1 THEN 'Superseded by a newer V1 permit for the same vehicle; V2 holds one permit per vehicle'
            -- Only on a later run: V1 issued a newer permit after this vehicle's first one was migrated,
            -- or the vehicle was given a permit in V2.
            ELSE 'The vehicle already has a different permit in V2'
        END
    FROM RankedPermits AS rankedPermits
    LEFT JOIN FogVehiclePermits AS existingPermits
        ON existingPermits.VehicleId = rankedPermits.MigratedVehicleId
    LEFT JOIN MigrationSkippedFogVehicles AS alreadySkipped
        ON alreadySkipped.LegacyRecordId = rankedPermits.ID
        AND alreadySkipped.SourceTable = 'FogVehiclePermits'
    WHERE alreadySkipped.LegacyRecordId IS NULL
        AND
        (
            rankedPermits.PermitRank > 1
            OR
            (
                existingPermits.VehicleId IS NOT NULL
                AND ISNULL(existingPermits.LegacyRecordId, 0) <> rankedPermits.ID
            )
        )

    ;WITH RankedPermits AS
    (
        SELECT legacyPermits.*,
            ROW_NUMBER() OVER (PARTITION BY legacyPermits.VehicleID ORDER BY legacyPermits.CreationDate DESC, legacyPermits.ID DESC) AS PermitRank,
            WaterSuppliers.Id AS MigratedWaterSupplierId,
            FogVehicles.Id AS MigratedVehicleId
        FROM Vepo.dbo.FogVehiclePermits AS legacyPermits
        INNER JOIN WaterSuppliers
            ON WaterSuppliers.LegacyRecordId = legacyPermits.WaterSupplierID
        INNER JOIN FogVehicles
            ON FogVehicles.LegacyRecordId = legacyPermits.VehicleID
    )
    INSERT INTO FogVehiclePermits
        (WaterSupplierId, VehicleId, LegacyRecordId, PermitNumber, InspectionDueDate, IsActive, CreatedById, CreatedTime)
    SELECT
        rankedPermits.MigratedWaterSupplierId,
        rankedPermits.MigratedVehicleId,
        rankedPermits.ID,
        -- V2 requires it and V1 asks for it on save, but does not enforce it in the table. V1 declares it
        -- nvarchar(20), narrower than the V2 column, so LEFT never cuts a V1 value short.
        LEFT(ISNULL(rankedPermits.PermitNumber, ''), 50),
        rankedPermits.InspectionDueDate,
        ISNULL(rankedPermits.Active, 0),
        NULL,
        -- Deliberately not defaulted: there is no honest stand-in for when a permit was issued, so a NULL
        -- there should stop the run.
        rankedPermits.CreationDate
    FROM RankedPermits AS rankedPermits
    WHERE rankedPermits.PermitRank = 1
        AND NOT EXISTS
        (
            SELECT 1
            FROM FogVehiclePermits AS existingPermits
            WHERE existingPermits.VehicleId = rankedPermits.MigratedVehicleId
        )

    -- A permit skipped by an earlier run goes in once whatever stopped it is fixed. Its entry no longer
    -- describes anything, so it is cleared to keep this table a list of what is really missing.
    DELETE MigrationSkippedFogVehicles
    FROM MigrationSkippedFogVehicles
    INNER JOIN FogVehiclePermits
        ON FogVehiclePermits.LegacyRecordId = MigrationSkippedFogVehicles.LegacyRecordId
    WHERE MigrationSkippedFogVehicles.SourceTable = 'FogVehiclePermits'

    COMMIT TRAN

END TRY
BEGIN CATCH
    ROLLBACK TRAN;
    THROW;
END CATCH
