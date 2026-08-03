BEGIN TRAN

BEGIN TRY

    -- Insert water suppliers that don't have parents
    INSERT INTO Envirotrax2Dev.dbo.WaterSuppliers
        ([Name], Domain, LegacyRecordId, ParentId,
         PwsId, ContactName, Address, City, StateId, ZipCode, PhoneNumber, FaxNumber, EmailAddress,
         LetterCompanyName, LetterContactName, LetterAddress, LetterCity, LetterStateId, LetterZipCode,
         LetterContactCompanyName, LetterContactContactName, LetterContactAddress, LetterContactCity,
         LetterContactStateId, LetterContactZipCode, LetterContactPhoneNumber, LetterContactFaxNumber, LetterContactEmailAddress,
         GisCenterLatitude, GisCenterLongitude, GisCenterZoom, IsActive, CreatedTime)
    SELECT
        WaterSuppliers.[Name], WaterSuppliers.Subdomain, WaterSuppliers.ID, NULL,
        WaterSuppliers.PwsID, WaterSuppliers.ContactName, WaterSuppliers.Address, WaterSuppliers.City, states.Id, WaterSuppliers.ZIP,
        WaterSuppliers.PhoneNumber, WaterSuppliers.FaxNumber, WaterSuppliers.EmailAddress,
        WaterSuppliers.LetterCompanyName, WaterSuppliers.LetterContactName, WaterSuppliers.LetterAddress, WaterSuppliers.LetterCity,
        letterStates.Id, WaterSuppliers.LetterZIP,
        WaterSuppliers.LetterContactCompanyName, WaterSuppliers.LetterContactContactName, WaterSuppliers.LetterContactAddress,
        WaterSuppliers.LetterContactCity, letterContactStates.Id, WaterSuppliers.LetterContactZIP,
        WaterSuppliers.LetterContactPhoneNumber, WaterSuppliers.LetterContactFaxNumber, WaterSuppliers.LetterContactEmailAddress,
        WaterSuppliers.GISCenterLatitude, WaterSuppliers.GISCenterLongitude, WaterSuppliers.GISCenterZoom, WaterSuppliers.Active, WaterSuppliers.CreationDate
    FROM WaterSuppliers
    LEFT JOIN Envirotrax2Dev.dbo.States AS states
        ON states.Code = WaterSuppliers.State
    LEFT JOIN Envirotrax2Dev.dbo.States AS letterStates
        ON letterStates.Code = WaterSuppliers.LetterState
    LEFT JOIN Envirotrax2Dev.dbo.States AS letterContactStates
        ON letterContactStates.Code = WaterSuppliers.LetterContactState
    WHERE (MasterWaterSupplierID2 > 0
        -- Supplier IDs up to 3 don't actually exist in the system, but there are records with those parent IDs
        OR (MasterWaterSupplierID < 4 AND MasterWaterSupplierID2 = 0))
        AND NOT EXISTS (
            SELECT 1
            FROM Envirotrax2Dev.dbo.WaterSuppliers AS alreadyInserted
            WHERE alreadyInserted.LegacyRecordId = WaterSuppliers.ID
        )

    -- Insert remaining water suppliers one generation at a time, once their parent has already been migrated
    WHILE @@ROWCOUNT > 0
    BEGIN
        INSERT INTO Envirotrax2Dev.dbo.WaterSuppliers
            ([Name], Domain, LegacyRecordId, ParentId,
             PwsId, ContactName, Address, City, StateId, ZipCode, PhoneNumber, FaxNumber, EmailAddress,
             LetterCompanyName, LetterContactName, LetterAddress, LetterCity, LetterStateId, LetterZipCode,
             LetterContactCompanyName, LetterContactContactName, LetterContactAddress, LetterContactCity,
             LetterContactStateId, LetterContactZipCode, LetterContactPhoneNumber, LetterContactFaxNumber, LetterContactEmailAddress,
             GisCenterLatitude, GisCenterLongitude, GisCenterZoom, IsActive, CreatedTime)
        SELECT
            WaterSuppliers.[Name], WaterSuppliers.Subdomain, WaterSuppliers.ID, parents.Id,
            WaterSuppliers.PwsID, WaterSuppliers.ContactName, WaterSuppliers.Address, WaterSuppliers.City, states.Id, WaterSuppliers.ZIP,
            WaterSuppliers.PhoneNumber, WaterSuppliers.FaxNumber, WaterSuppliers.EmailAddress,
            WaterSuppliers.LetterCompanyName, WaterSuppliers.LetterContactName, WaterSuppliers.LetterAddress, WaterSuppliers.LetterCity,
            letterStates.Id, WaterSuppliers.LetterZIP,
            WaterSuppliers.LetterContactCompanyName, WaterSuppliers.LetterContactContactName, WaterSuppliers.LetterContactAddress,
            WaterSuppliers.LetterContactCity, letterContactStates.Id, WaterSuppliers.LetterContactZIP,
            WaterSuppliers.LetterContactPhoneNumber, WaterSuppliers.LetterContactFaxNumber, WaterSuppliers.LetterContactEmailAddress,
            WaterSuppliers.GISCenterLatitude, WaterSuppliers.GISCenterLongitude, WaterSuppliers.GISCenterZoom, WaterSuppliers.Active, WaterSuppliers.CreationDate
        FROM WaterSuppliers
        INNER JOIN Envirotrax2Dev.dbo.WaterSuppliers AS parents
            ON parents.LegacyRecordId = WaterSuppliers.MasterWaterSupplierID
        LEFT JOIN Envirotrax2Dev.dbo.States AS states
            ON states.Code = WaterSuppliers.State
        LEFT JOIN Envirotrax2Dev.dbo.States AS letterStates
            ON letterStates.Code = WaterSuppliers.LetterState
        LEFT JOIN Envirotrax2Dev.dbo.States AS letterContactStates
            ON letterContactStates.Code = WaterSuppliers.LetterContactState
        WHERE NOT EXISTS (
            SELECT 1
            FROM Envirotrax2Dev.dbo.WaterSuppliers AS alreadyInserted
            WHERE alreadyInserted.LegacyRecordId = WaterSuppliers.ID
        )
    END

    COMMIT TRAN

END TRY
BEGIN CATCH
    ROLLBACK TRAN;
    THROW;
END CATCH