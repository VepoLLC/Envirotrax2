BEGIN TRAN

BEGIN TRY

    -- Rows with InspectionID = 0 belong to inspection forms that were never submitted, so they are neither
    -- migrated nor recorded as skipped.
    INSERT INTO MigrationSkippedCsiInspections (LegacyRecordId, LegacyUserId, SourceTable, Reason)
    SELECT legacyAssemblies.ID, NULL, 'CsiInspectionVisuallyIdentifiedAssemblies', 'No matching migrated inspection'
    FROM Vepo.dbo.CsiInspectionVisuallyIdentifiedAssemblies AS legacyAssemblies
    LEFT JOIN CsiInspections
        ON CsiInspections.LegacyRecordId = legacyAssemblies.InspectionID
    LEFT JOIN MigrationSkippedCsiInspections AS alreadySkipped
        ON alreadySkipped.LegacyRecordId = legacyAssemblies.ID
        AND alreadySkipped.SourceTable = 'CsiInspectionVisuallyIdentifiedAssemblies'
    WHERE ISNULL(legacyAssemblies.InspectionID, 0) <> 0
        AND CsiInspections.Id IS NULL
        AND alreadySkipped.LegacyRecordId IS NULL

    INSERT INTO CsiInspectionVisuallyIdentifiedAssemblies
        (LegacyRecordId, WaterSupplierId, ProfessionalId, InspectionId, TestId, AddedOnInspection, SubmissionId,
         VisuallyIdentified, DeviceType, AssemblyDescription, SerialNumber, AssemblyDescription2, SerialNumber2,
         HazardType, HazardTypeOtherDescription, LocationDescription, IsCurrent, TestResult, OutOfService,
         TestDate, ExpirationDate, TransactionId, CreatedTime)
    SELECT
        legacyAssemblies.ID,
        inspections.WaterSupplierId,
        inspections.ProfessionalId,
        inspections.Id,
        tests.Id,
        -- A test added with "+ Add Assembly" on this inspection carries the inspection's SubmissionID.
        CASE
            WHEN tests.SubmissionId = NULLIF(inspections.SubmissionId, '') THEN 1
            ELSE 0
        END,
        legacyAssemblies.SubmissionID,
        ISNULL(legacyAssemblies.VisuallyIdentified, 0),
        legacyAssemblies.DeviceType,
        legacyAssemblies.AssemblyDescription,
        legacyAssemblies.SerialNumber,
        legacyAssemblies.AssemblyDescription2,
        legacyAssemblies.SerialNumber2,
        legacyAssemblies.HazardType,
        legacyAssemblies.HazardTypeOtherDescription,
        legacyAssemblies.LocationDescription,
        ISNULL(legacyAssemblies.IsCurrent, 0),
        ISNULL(legacyAssemblies.TestResult, 0),
        ISNULL(legacyAssemblies.OutOfService, 0),
        legacyAssemblies.TestDate,
        legacyAssemblies.ExpirationDate,
        NULLIF(legacyAssemblies.TransactionID, ''),
        ISNULL(legacyAssemblies.CreationDate, inspections.CreatedTime)
    FROM Vepo.dbo.CsiInspectionVisuallyIdentifiedAssemblies AS legacyAssemblies
    INNER JOIN CsiInspections AS inspections
        ON inspections.LegacyRecordId = legacyAssemblies.InspectionID
    -- The foreign key to the test includes WaterSupplierId. A test that was not migrated or belongs to another
    -- supplier leaves TestId NULL, and the assembly keeps its own snapshot of the device details.
    LEFT JOIN BackflowTests AS tests
        ON tests.LegacyRecordId = NULLIF(legacyAssemblies.TestID, 0)
        AND tests.WaterSupplierId = inspections.WaterSupplierId
    WHERE NOT EXISTS
    (
        SELECT 1
        FROM CsiInspectionVisuallyIdentifiedAssemblies AS alreadyInserted
        WHERE alreadyInserted.LegacyRecordId = legacyAssemblies.ID
    )

    COMMIT TRAN

END TRY
BEGIN CATCH
    ROLLBACK TRAN;
    THROW;
END CATCH
