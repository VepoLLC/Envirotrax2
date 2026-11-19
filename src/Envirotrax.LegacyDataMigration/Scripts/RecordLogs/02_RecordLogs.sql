BEGIN TRAN

BEGIN TRY

    ;WITH AccountsWithoutDuplicates AS
    (
        SELECT accounts.*,
            ROW_NUMBER() OVER (PARTITION BY accounts.LegacyUserId, accounts.LegacyUserType ORDER BY accounts.LegacyRecordId) AS AccountRank
        FROM MigrationLegacyProfessionalAccounts AS accounts
    ),
    TranslatedLogs AS
    (
        SELECT
            legacyLogs.ID AS LegacyLogId,
            'Sites' AS TableName,
            Sites.Id AS RecordId,
            Sites.WaterSupplierId AS WaterSupplierId,
            CAST(NULL AS INT) AS ProfessionalId
        FROM Vepo.dbo.UserLog AS legacyLogs
        INNER JOIN Sites
            ON Sites.LegacyRecordId = legacyLogs.RecordID
        WHERE legacyLogs.TableName = 'CsiBackflowSites'

        UNION ALL

        SELECT
            legacyLogs.ID,
            'BackflowTests',
            BackflowTests.Id,
            BackflowTests.WaterSupplierId,
            actingProfessionalUsers.ProfessionalId
        FROM Vepo.dbo.UserLog AS legacyLogs
        INNER JOIN BackflowTests
            ON BackflowTests.LegacyRecordId = legacyLogs.RecordID
        LEFT JOIN AspNetUsers AS actors
            ON actors.UserName = NULLIF(legacyLogs.UserID, '')
        LEFT JOIN ProfessionalUsers AS actingProfessionalUsers
            ON actingProfessionalUsers.ProfessionalId = BackflowTests.ProfessionalId
            AND actingProfessionalUsers.UserId = actors.Id
        WHERE legacyLogs.TableName = 'SaveBackflowDeviceTests'

        UNION ALL

        SELECT
            legacyLogs.ID,
            'CsiInspections',
            CsiInspections.Id,
            CsiInspections.WaterSupplierId,
            actingProfessionalUsers.ProfessionalId
        FROM Vepo.dbo.UserLog AS legacyLogs
        INNER JOIN CsiInspections
            ON CsiInspections.LegacyRecordId = legacyLogs.RecordID
        LEFT JOIN AspNetUsers AS actors
            ON actors.UserName = NULLIF(legacyLogs.UserID, '')
        LEFT JOIN ProfessionalUsers AS actingProfessionalUsers
            ON actingProfessionalUsers.ProfessionalId = CsiInspections.ProfessionalId
            AND actingProfessionalUsers.UserId = actors.Id
        WHERE legacyLogs.TableName IN ('CsiInspections', 'CSIInspections')

        UNION ALL

        SELECT
            legacyLogs.ID,
            'FogInspections',
            FogInspections.Id,
            FogInspections.WaterSupplierId,
            actingProfessionalUsers.ProfessionalId
        FROM Vepo.dbo.UserLog AS legacyLogs
        INNER JOIN FogInspections
            ON FogInspections.LegacyRecordId = legacyLogs.RecordID
        LEFT JOIN AspNetUsers AS actors
            ON actors.UserName = NULLIF(legacyLogs.UserID, '')
        LEFT JOIN ProfessionalUsers AS actingProfessionalUsers
            ON actingProfessionalUsers.ProfessionalId = FogInspections.ProfessionalId
            AND actingProfessionalUsers.UserId = actors.Id
        WHERE legacyLogs.TableName = 'FogInspections'

        UNION ALL

        SELECT
            legacyLogs.ID,
            'FogTripTickets',
            FogTripTickets.Id,
            FogTripTickets.WaterSupplierId,
            actingProfessionalUsers.ProfessionalId
        FROM Vepo.dbo.UserLog AS legacyLogs
        INNER JOIN FogTripTickets
            ON FogTripTickets.LegacyRecordId = legacyLogs.RecordID
        LEFT JOIN AspNetUsers AS actors
            ON actors.UserName = NULLIF(legacyLogs.UserID, '')
        LEFT JOIN ProfessionalUsers AS actingProfessionalUsers
            ON actingProfessionalUsers.ProfessionalId = FogTripTickets.ProfessionalId
            AND actingProfessionalUsers.UserId = actors.Id
        WHERE legacyLogs.TableName = 'FogTripTickets'

        UNION ALL

        SELECT
            legacyLogs.ID,
            'WaterSuppliers',
            WaterSuppliers.Id,
            WaterSuppliers.Id,
            NULL
        FROM Vepo.dbo.UserLog AS legacyLogs
        INNER JOIN WaterSuppliers
            ON WaterSuppliers.LegacyRecordId = legacyLogs.RecordID
        WHERE legacyLogs.TableName = 'WaterSuppliers'

        UNION ALL

        SELECT
            legacyLogs.ID,
            'ProfessionalUsers',
            ProfessionalUsers.UserId,
            NULL,
            ProfessionalUsers.ProfessionalId
        FROM Vepo.dbo.UserLog AS legacyLogs
        INNER JOIN MigrationLegacyProfessionalAccounts AS accounts
            ON accounts.SourceTable = legacyLogs.TableName
            AND accounts.LegacyRecordId = legacyLogs.RecordID
        INNER JOIN Professionals
            ON Professionals.LegacyUserId = accounts.LegacyCompanyUserId
        INNER JOIN AspNetUsers
            ON AspNetUsers.UserName = accounts.LegacyUserId
        INNER JOIN ProfessionalUsers
            ON ProfessionalUsers.ProfessionalId = Professionals.Id
            AND ProfessionalUsers.UserId = AspNetUsers.Id
        WHERE legacyLogs.TableName IN ('SaveBpats', 'CsiInspectors', 'FogInspectors', 'FogTransporters')

        UNION ALL

        SELECT
            legacyLogs.ID,
            'ProfessionalLicenses',
            ProfessionalLicenses.Id,
            actingSuppliers.Id,
            ProfessionalLicenses.ProfessionalId
        FROM Vepo.dbo.UserLog AS legacyLogs
        INNER JOIN ProfessionalLicenses
            ON ProfessionalLicenses.LegacyRecordId = legacyLogs.RecordID
        LEFT JOIN WaterSuppliers AS actingSuppliers
            ON actingSuppliers.LegacyRecordId = legacyLogs.WaterSupplierID
        WHERE legacyLogs.TableName = 'SaveLicenses'

        UNION ALL

        SELECT
            legacyLogs.ID,
            'ProfessionalUserLicenses',
            ProfessionalUserLicenses.Id,
            actingSuppliers.Id,
            ProfessionalUserLicenses.ProfessionalId
        FROM Vepo.dbo.UserLog AS legacyLogs
        INNER JOIN ProfessionalUserLicenses
            ON ProfessionalUserLicenses.LegacyRecordId = legacyLogs.RecordID
        LEFT JOIN WaterSuppliers AS actingSuppliers
            ON actingSuppliers.LegacyRecordId = legacyLogs.WaterSupplierID
        WHERE legacyLogs.TableName = 'SaveLicenses'

        UNION ALL

        SELECT
            legacyLogs.ID,
            'ProfessionalInsurances',
            ProfessionalInsurances.Id,
            actingSuppliers.Id,
            ProfessionalInsurances.ProfessionalId
        FROM Vepo.dbo.UserLog AS legacyLogs
        INNER JOIN ProfessionalInsurances
            ON ProfessionalInsurances.LegacyRecordId = legacyLogs.RecordID
        LEFT JOIN WaterSuppliers AS actingSuppliers
            ON actingSuppliers.LegacyRecordId = legacyLogs.WaterSupplierID
        WHERE legacyLogs.TableName = 'SaveLicenses'

        UNION ALL

        SELECT
            legacyLogs.ID,
            'BackflowGauges',
            BackflowGauges.Id,
            NULL,
            BackflowGauges.ProfessionalId
        FROM Vepo.dbo.UserLog AS legacyLogs
        INNER JOIN BackflowGauges
            ON BackflowGauges.LegacyRecordId = legacyLogs.RecordID
        WHERE legacyLogs.TableName = 'SaveBpatGauges'

        UNION ALL

        SELECT
            legacyLogs.ID,
            'FogVehicles',
            FogVehicles.Id,
            NULL,
            FogVehicles.ProfessionalId
        FROM Vepo.dbo.UserLog AS legacyLogs
        INNER JOIN FogVehicles
            ON FogVehicles.LegacyRecordId = legacyLogs.RecordID
        WHERE legacyLogs.TableName = 'FogVehicles'

        UNION ALL

        SELECT
            legacyLogs.ID,
            'FogDisposalSites',
            FogDisposalSites.Id,
            NULL,
            NULL
        FROM Vepo.dbo.UserLog AS legacyLogs
        INNER JOIN FogDisposalSites
            ON FogDisposalSites.LegacyRecordId = legacyLogs.RecordID
        WHERE legacyLogs.TableName = 'FogDisposalSites'

        UNION ALL

        SELECT
            legacyLogs.ID,
            'NotificationSettings',
            NotificationSettings.Id,
            NotificationSettings.WaterSupplierId,
            NULL
        FROM Vepo.dbo.UserLog AS legacyLogs
        INNER JOIN NotificationSettings
            ON NotificationSettings.LegacyRecordId = legacyLogs.RecordID
        WHERE legacyLogs.TableName = 'WaterSupplierNotificationSettings'

        UNION ALL

        SELECT
            legacyLogs.ID,
            'WaterSupplierUserAccounts',
            WaterSupplierUsers.UserId,
            WaterSupplierUsers.WaterSupplierId,
            NULL
        FROM Vepo.dbo.UserLog AS legacyLogs
        INNER JOIN WaterSupplierUsers
            ON WaterSupplierUsers.LegacyRecordId = legacyLogs.RecordID
        WHERE legacyLogs.TableName = 'WaterSupplierUserAccounts'

        UNION ALL

        SELECT
            legacyLogs.ID,
            'ProfessionalWaterSupplierRegistrations',
            ProfessionalWaterSuppliers.WaterSupplierId,
            ProfessionalWaterSuppliers.WaterSupplierId,
            ProfessionalWaterSuppliers.ProfessionalId
        FROM Vepo.dbo.UserLog AS legacyLogs
        INNER JOIN Vepo.dbo.SaveWaterSupplierRegistrations AS registrations
            ON registrations.ID = legacyLogs.RecordID
        INNER JOIN AccountsWithoutDuplicates AS accounts
            ON accounts.LegacyUserId = registrations.UserID
            AND accounts.LegacyUserType = registrations.UserType
            AND accounts.AccountRank = 1
        INNER JOIN Professionals
            ON Professionals.LegacyUserId = accounts.LegacyCompanyUserId
        INNER JOIN WaterSuppliers
            ON WaterSuppliers.LegacyRecordId = registrations.WaterSupplierID
        INNER JOIN ProfessionalWaterSuppliers
            ON ProfessionalWaterSuppliers.WaterSupplierId = WaterSuppliers.Id
            AND ProfessionalWaterSuppliers.ProfessionalId = Professionals.Id
        WHERE legacyLogs.TableName = 'SaveWaterSupplierRegistrations'
            AND registrations.UserID = accounts.LegacyCompanyUserId
    )
    INSERT INTO MigrationTranslatedRecordLogs (LegacyLogId, TableName, RecordId, WaterSupplierId, ProfessionalId)
    SELECT LegacyLogId, TableName, RecordId, WaterSupplierId, ProfessionalId
    FROM TranslatedLogs

    INSERT INTO RecordLogs
        (LegacyRecordId, LegacyLoggedRecordId, LogType, TableName, RecordId, Description, WaterSupplierId, ProfessionalId,
         IpAddress, SessionId, CreatedById, CreatedTime)
    SELECT
        legacyLogs.ID,
        legacyLogs.RecordID,
        CASE
            WHEN legacyLogs.LogType IN ('Login', '0') THEN 0
            WHEN legacyLogs.LogType IN ('View', '1') THEN 1
            WHEN legacyLogs.LogType IN ('Add', '2') THEN 2
            WHEN legacyLogs.LogType IN ('Edit', '3') THEN 3
            WHEN legacyLogs.LogType IN ('Delete', '4') THEN 4
            WHEN legacyLogs.LogType IN ('Export', '5') THEN 5
        END,
        ISNULL(translatedLogs.TableName, legacyLogs.TableName),
        ISNULL(translatedLogs.RecordId, 0),
        NULLIF(CAST(legacyLogs.Description AS NVARCHAR(MAX)), ''),
        CASE
            WHEN translatedLogs.LegacyLogId IS NULL THEN loggedSuppliers.Id
            ELSE translatedLogs.WaterSupplierId
        END,
        translatedLogs.ProfessionalId,
        NULLIF(legacyLogs.IpAddress, ''),
        NULLIF(legacyLogs.SessionID, ''),
        authors.Id,
        legacyLogs.LogDate
    FROM Vepo.dbo.UserLog AS legacyLogs
    LEFT JOIN MigrationTranslatedRecordLogs AS translatedLogs
        ON translatedLogs.LegacyLogId = legacyLogs.ID
    LEFT JOIN WaterSuppliers AS loggedSuppliers
        ON loggedSuppliers.LegacyRecordId = legacyLogs.WaterSupplierID
    LEFT JOIN AspNetUsers AS authors
        ON authors.UserName = NULLIF(legacyLogs.UserID, '')
    WHERE NOT EXISTS
        (
            SELECT 1
            FROM RecordLogs AS alreadyInserted
            WHERE alreadyInserted.LegacyRecordId = legacyLogs.ID
        )

    UPDATE RecordLogs
    SET TableName = translatedLogs.TableName,
        RecordId = translatedLogs.RecordId,
        WaterSupplierId = translatedLogs.WaterSupplierId,
        ProfessionalId = translatedLogs.ProfessionalId
    FROM RecordLogs
    INNER JOIN MigrationTranslatedRecordLogs AS translatedLogs
        ON translatedLogs.LegacyLogId = RecordLogs.LegacyRecordId
    WHERE RecordLogs.RecordId = 0

    COMMIT TRAN

END TRY
BEGIN CATCH
    ROLLBACK TRAN;
    THROW;
END CATCH
