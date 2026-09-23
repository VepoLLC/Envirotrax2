BEGIN TRAN

BEGIN TRY

    -- Insert water suppliers that don't have parents
    INSERT INTO WaterSuppliers
        ([Name], Domain, LegacyRecordId, ParentId,
         PwsId, ContactName, Address, City, StateId, ZipCode, PhoneNumber, FaxNumber, EmailAddress,
         LetterCompanyName, LetterContactName, LetterAddress, LetterCity, LetterStateId, LetterZipCode,
         LetterContactCompanyName, LetterContactContactName, LetterContactAddress, LetterContactCity,
         LetterContactStateId, LetterContactZipCode, LetterContactPhoneNumber, LetterContactFaxNumber, LetterContactEmailAddress,
         GisCenterLatitude, GisCenterLongitude, GisCenterZoom, IsActive, CreatedTime)
    SELECT
        legacyWaterSuppliers.[Name], legacyWaterSuppliers.Subdomain, legacyWaterSuppliers.ID, NULL,
        legacyWaterSuppliers.PwsID, legacyWaterSuppliers.ContactName, legacyWaterSuppliers.Address, legacyWaterSuppliers.City, states.Id, legacyWaterSuppliers.ZIP,
        legacyWaterSuppliers.PhoneNumber, legacyWaterSuppliers.FaxNumber, legacyWaterSuppliers.EmailAddress,
        legacyWaterSuppliers.LetterCompanyName, legacyWaterSuppliers.LetterContactName, legacyWaterSuppliers.LetterAddress, legacyWaterSuppliers.LetterCity,
        letterStates.Id, legacyWaterSuppliers.LetterZIP,
        legacyWaterSuppliers.LetterContactCompanyName, legacyWaterSuppliers.LetterContactContactName, legacyWaterSuppliers.LetterContactAddress,
        legacyWaterSuppliers.LetterContactCity, letterContactStates.Id, legacyWaterSuppliers.LetterContactZIP,
        legacyWaterSuppliers.LetterContactPhoneNumber, legacyWaterSuppliers.LetterContactFaxNumber, legacyWaterSuppliers.LetterContactEmailAddress,
        legacyWaterSuppliers.GISCenterLatitude, legacyWaterSuppliers.GISCenterLongitude, legacyWaterSuppliers.GISCenterZoom, legacyWaterSuppliers.Active, legacyWaterSuppliers.CreationDate
    FROM Vepo.dbo.WaterSuppliers AS legacyWaterSuppliers
    LEFT JOIN States AS states
        ON states.Code = legacyWaterSuppliers.State
    LEFT JOIN States AS letterStates
        ON letterStates.Code = legacyWaterSuppliers.LetterState
    LEFT JOIN States AS letterContactStates
        ON letterContactStates.Code = legacyWaterSuppliers.LetterContactState
    WHERE (legacyWaterSuppliers.MasterWaterSupplierID2 > 0
        -- Supplier IDs up to 3 don't actually exist in the system, but there are records with those parent IDs
        OR (legacyWaterSuppliers.MasterWaterSupplierID < 4 AND legacyWaterSuppliers.MasterWaterSupplierID2 = 0))
        AND NOT EXISTS (
            SELECT 1
            FROM WaterSuppliers AS alreadyInserted
            WHERE alreadyInserted.LegacyRecordId = legacyWaterSuppliers.ID
        )

    -- Insert remaining water suppliers one generation at a time, once their parent has already been migrated
    WHILE @@ROWCOUNT > 0
    BEGIN
        INSERT INTO WaterSuppliers
            ([Name], Domain, LegacyRecordId, ParentId,
             PwsId, ContactName, Address, City, StateId, ZipCode, PhoneNumber, FaxNumber, EmailAddress,
             LetterCompanyName, LetterContactName, LetterAddress, LetterCity, LetterStateId, LetterZipCode,
             LetterContactCompanyName, LetterContactContactName, LetterContactAddress, LetterContactCity,
             LetterContactStateId, LetterContactZipCode, LetterContactPhoneNumber, LetterContactFaxNumber, LetterContactEmailAddress,
             GisCenterLatitude, GisCenterLongitude, GisCenterZoom, IsActive, CreatedTime)
        SELECT
            legacyWaterSuppliers.[Name], legacyWaterSuppliers.Subdomain, legacyWaterSuppliers.ID, parents.Id,
            legacyWaterSuppliers.PwsID, legacyWaterSuppliers.ContactName, legacyWaterSuppliers.Address, legacyWaterSuppliers.City, states.Id, legacyWaterSuppliers.ZIP,
            legacyWaterSuppliers.PhoneNumber, legacyWaterSuppliers.FaxNumber, legacyWaterSuppliers.EmailAddress,
            legacyWaterSuppliers.LetterCompanyName, legacyWaterSuppliers.LetterContactName, legacyWaterSuppliers.LetterAddress, legacyWaterSuppliers.LetterCity,
            letterStates.Id, legacyWaterSuppliers.LetterZIP,
            legacyWaterSuppliers.LetterContactCompanyName, legacyWaterSuppliers.LetterContactContactName, legacyWaterSuppliers.LetterContactAddress,
            legacyWaterSuppliers.LetterContactCity, letterContactStates.Id, legacyWaterSuppliers.LetterContactZIP,
            legacyWaterSuppliers.LetterContactPhoneNumber, legacyWaterSuppliers.LetterContactFaxNumber, legacyWaterSuppliers.LetterContactEmailAddress,
            legacyWaterSuppliers.GISCenterLatitude, legacyWaterSuppliers.GISCenterLongitude, legacyWaterSuppliers.GISCenterZoom, legacyWaterSuppliers.Active, legacyWaterSuppliers.CreationDate
        FROM Vepo.dbo.WaterSuppliers AS legacyWaterSuppliers
        INNER JOIN WaterSuppliers AS parents
            ON parents.LegacyRecordId = legacyWaterSuppliers.MasterWaterSupplierID
        LEFT JOIN States AS states
            ON states.Code = legacyWaterSuppliers.State
        LEFT JOIN States AS letterStates
            ON letterStates.Code = legacyWaterSuppliers.LetterState
        LEFT JOIN States AS letterContactStates
            ON letterContactStates.Code = legacyWaterSuppliers.LetterContactState
        WHERE NOT EXISTS (
            SELECT 1
            FROM WaterSuppliers AS alreadyInserted
            WHERE alreadyInserted.LegacyRecordId = legacyWaterSuppliers.ID
        )
    END

    COMMIT TRAN

END TRY
BEGIN CATCH
    ROLLBACK TRAN;
    THROW;
END CATCH