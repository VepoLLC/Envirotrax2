BEGIN TRAN

BEGIN TRY

    DELETE MigrationSkippedBackflowComplianceSnapshots
    FROM MigrationSkippedBackflowComplianceSnapshots
    INNER JOIN Vepo.dbo.WaterSupplierReports AS legacyReports
        ON legacyReports.ID = MigrationSkippedBackflowComplianceSnapshots.LegacyRecordId
    INNER JOIN WaterSuppliers
        ON WaterSuppliers.LegacyRecordId = legacyReports.WaterSupplierID
    WHERE MigrationSkippedBackflowComplianceSnapshots.SourceTable = 'WaterSupplierReports'
        AND MigrationSkippedBackflowComplianceSnapshots.Reason = 'No matching migrated water supplier'

    INSERT INTO MigrationSkippedBackflowComplianceSnapshots (LegacyRecordId, SourceTable, Reason)
    SELECT legacyReports.ID, 'WaterSupplierReports', 'No matching migrated water supplier'
    FROM Vepo.dbo.WaterSupplierReports AS legacyReports
    LEFT JOIN WaterSuppliers
        ON WaterSuppliers.LegacyRecordId = legacyReports.WaterSupplierID
    LEFT JOIN MigrationSkippedBackflowComplianceSnapshots AS alreadySkipped
        ON alreadySkipped.LegacyRecordId = legacyReports.ID
        AND alreadySkipped.SourceTable = 'WaterSupplierReports'
    WHERE WaterSuppliers.Id IS NULL
        AND alreadySkipped.LegacyRecordId IS NULL

    ;WITH RankedReports AS
    (
        SELECT legacyReports.ID,
            legacyReports.ReportDate,
            legacyReports.Total,
            legacyReports.Compliant,
            WaterSuppliers.Id AS MigratedWaterSupplierId,
            ROW_NUMBER() OVER (PARTITION BY WaterSuppliers.Id, legacyReports.ReportDate ORDER BY legacyReports.ID DESC) AS ReportRank
        FROM Vepo.dbo.WaterSupplierReports AS legacyReports
        INNER JOIN WaterSuppliers
            ON WaterSuppliers.LegacyRecordId = legacyReports.WaterSupplierID
    )
    INSERT INTO MigrationSkippedBackflowComplianceSnapshots (LegacyRecordId, SourceTable, Reason)
    SELECT rankedReports.ID, 'WaterSupplierReports',
        CASE
            WHEN rankedReports.ReportRank > 1 THEN 'Superseded by a newer V1 report for the same water supplier and date'
            ELSE 'The water supplier already has a different snapshot for this date in V2'
        END
    FROM RankedReports AS rankedReports
    LEFT JOIN BackflowComplianceSnapshots AS existingSnapshots
        ON existingSnapshots.WaterSupplierId = rankedReports.MigratedWaterSupplierId
        AND existingSnapshots.ReportDate = rankedReports.ReportDate
    LEFT JOIN MigrationSkippedBackflowComplianceSnapshots AS alreadySkipped
        ON alreadySkipped.LegacyRecordId = rankedReports.ID
        AND alreadySkipped.SourceTable = 'WaterSupplierReports'
    WHERE alreadySkipped.LegacyRecordId IS NULL
        AND
        (
            rankedReports.ReportRank > 1
            OR
            (
                existingSnapshots.WaterSupplierId IS NOT NULL
                AND
                (
                    existingSnapshots.Total <> rankedReports.Total
                    OR existingSnapshots.Compliant <> rankedReports.Compliant
                )
            )
        )

    ;WITH RankedReports AS
    (
        SELECT legacyReports.ReportDate,
            legacyReports.Total,
            legacyReports.Compliant,
            WaterSuppliers.Id AS MigratedWaterSupplierId,
            ROW_NUMBER() OVER (PARTITION BY WaterSuppliers.Id, legacyReports.ReportDate ORDER BY legacyReports.ID DESC) AS ReportRank
        FROM Vepo.dbo.WaterSupplierReports AS legacyReports
        INNER JOIN WaterSuppliers
            ON WaterSuppliers.LegacyRecordId = legacyReports.WaterSupplierID
    )
    INSERT INTO BackflowComplianceSnapshots
        (WaterSupplierId, ReportDate, Total, Compliant)
    SELECT
        rankedReports.MigratedWaterSupplierId,
        rankedReports.ReportDate,
        rankedReports.Total,
        rankedReports.Compliant
    FROM RankedReports AS rankedReports
    WHERE rankedReports.ReportRank = 1
        AND NOT EXISTS
        (
            SELECT 1
            FROM BackflowComplianceSnapshots AS existingSnapshots
            WHERE existingSnapshots.WaterSupplierId = rankedReports.MigratedWaterSupplierId
                AND existingSnapshots.ReportDate = rankedReports.ReportDate
        )

    COMMIT TRAN

END TRY
BEGIN CATCH
    ROLLBACK TRAN;
    THROW;
END CATCH
