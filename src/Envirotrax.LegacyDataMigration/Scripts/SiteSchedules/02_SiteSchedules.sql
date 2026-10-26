BEGIN TRAN

BEGIN TRY

    INSERT INTO MigrationSkippedSiteSchedules (LegacyRecordId, LegacyUserId, SourceTable, Reason)
    SELECT legacySchedules.ID, legacySchedules.UserID, 'AccountSchedules', 'No matching migrated site'
    FROM Vepo.dbo.AccountSchedules AS legacySchedules
    LEFT JOIN Sites
        ON Sites.LegacyRecordId = legacySchedules.SiteID
    LEFT JOIN MigrationSkippedSiteSchedules AS alreadySkipped
        ON alreadySkipped.LegacyRecordId = legacySchedules.ID
        AND alreadySkipped.SourceTable = 'AccountSchedules'
    WHERE Sites.Id IS NULL
        AND alreadySkipped.LegacyRecordId IS NULL

    ;WITH AccountsWithoutDuplicates AS
    (
        SELECT accounts.*,
            ROW_NUMBER() OVER (PARTITION BY accounts.LegacyUserId, accounts.LegacyUserType ORDER BY accounts.LegacyRecordId) AS AccountRank
        FROM MigrationLegacyProfessionalAccounts AS accounts
    )
    INSERT INTO MigrationSkippedSiteSchedules (LegacyRecordId, LegacyUserId, SourceTable, Reason)
    SELECT legacySchedules.ID, legacySchedules.UserID, 'AccountSchedules', 'No migrated professional user for the login'
    FROM Vepo.dbo.AccountSchedules AS legacySchedules
    INNER JOIN Sites
        ON Sites.LegacyRecordId = legacySchedules.SiteID
    LEFT JOIN AccountsWithoutDuplicates AS accounts
        ON accounts.LegacyUserId = legacySchedules.UserID
        AND accounts.LegacyUserType = legacySchedules.UserType
        AND accounts.AccountRank = 1
    LEFT JOIN Professionals
        ON Professionals.LegacyUserId = accounts.LegacyCompanyUserId
    LEFT JOIN AspNetUsers
        ON AspNetUsers.UserName = legacySchedules.UserID
    LEFT JOIN ProfessionalUsers
        ON ProfessionalUsers.ProfessionalId = Professionals.Id
        AND ProfessionalUsers.UserId = AspNetUsers.Id
    LEFT JOIN MigrationSkippedSiteSchedules AS alreadySkipped
        ON alreadySkipped.LegacyRecordId = legacySchedules.ID
        AND alreadySkipped.SourceTable = 'AccountSchedules'
    WHERE ProfessionalUsers.ProfessionalId IS NULL
        AND alreadySkipped.LegacyRecordId IS NULL

    ;WITH AccountsWithoutDuplicates AS
    (
        SELECT accounts.*,
            ROW_NUMBER() OVER (PARTITION BY accounts.LegacyUserId, accounts.LegacyUserType ORDER BY accounts.LegacyRecordId) AS AccountRank
        FROM MigrationLegacyProfessionalAccounts AS accounts
    ),
    RankedSchedules AS
    (
        SELECT legacySchedules.*,
            ROW_NUMBER() OVER (PARTITION BY legacySchedules.SiteID, legacySchedules.UserID, legacySchedules.UserType ORDER BY legacySchedules.ID DESC) AS ScheduleRank,
            Sites.Id AS MigratedSiteId,
            ProfessionalUsers.ProfessionalId AS MigratedProfessionalId,
            ProfessionalUsers.UserId AS MigratedUserId
        FROM Vepo.dbo.AccountSchedules AS legacySchedules
        INNER JOIN Sites
            ON Sites.LegacyRecordId = legacySchedules.SiteID
        INNER JOIN AccountsWithoutDuplicates AS accounts
            ON accounts.LegacyUserId = legacySchedules.UserID
            AND accounts.LegacyUserType = legacySchedules.UserType
            AND accounts.AccountRank = 1
        INNER JOIN Professionals
            ON Professionals.LegacyUserId = accounts.LegacyCompanyUserId
        INNER JOIN AspNetUsers
            ON AspNetUsers.UserName = legacySchedules.UserID
        INNER JOIN ProfessionalUsers
            ON ProfessionalUsers.ProfessionalId = Professionals.Id
            AND ProfessionalUsers.UserId = AspNetUsers.Id
    )
    INSERT INTO MigrationSkippedSiteSchedules (LegacyRecordId, LegacyUserId, SourceTable, Reason)
    SELECT rankedSchedules.ID, rankedSchedules.UserID, 'AccountSchedules',
        CASE
            WHEN rankedSchedules.ScheduleRank > 1 THEN 'Superseded by a newer V1 schedule for the same site and user'
            ELSE 'The site already has a different schedule for this user in V2'
        END
    FROM RankedSchedules AS rankedSchedules
    LEFT JOIN SiteSchedules AS existingSchedules
        ON existingSchedules.SiteId = rankedSchedules.MigratedSiteId
        AND existingSchedules.ProfessionalId = rankedSchedules.MigratedProfessionalId
        AND existingSchedules.UserId = rankedSchedules.MigratedUserId
        AND existingSchedules.ProfessionalType = rankedSchedules.UserType
    LEFT JOIN MigrationSkippedSiteSchedules AS alreadySkipped
        ON alreadySkipped.LegacyRecordId = rankedSchedules.ID
        AND alreadySkipped.SourceTable = 'AccountSchedules'
    WHERE alreadySkipped.LegacyRecordId IS NULL
        AND
        (
            rankedSchedules.ScheduleRank > 1
            OR
            (
                existingSchedules.Id IS NOT NULL
                AND ISNULL(existingSchedules.LegacyRecordId, 0) <> rankedSchedules.ID
            )
        )

    ;WITH AccountsWithoutDuplicates AS
    (
        SELECT accounts.*,
            ROW_NUMBER() OVER (PARTITION BY accounts.LegacyUserId, accounts.LegacyUserType ORDER BY accounts.LegacyRecordId) AS AccountRank
        FROM MigrationLegacyProfessionalAccounts AS accounts
    ),
    RankedSchedules AS
    (
        SELECT legacySchedules.*,
            ROW_NUMBER() OVER (PARTITION BY legacySchedules.SiteID, legacySchedules.UserID, legacySchedules.UserType ORDER BY legacySchedules.ID DESC) AS ScheduleRank,
            Sites.Id AS MigratedSiteId,
            ProfessionalUsers.ProfessionalId AS MigratedProfessionalId,
            ProfessionalUsers.UserId AS MigratedUserId
        FROM Vepo.dbo.AccountSchedules AS legacySchedules
        INNER JOIN Sites
            ON Sites.LegacyRecordId = legacySchedules.SiteID
        INNER JOIN AccountsWithoutDuplicates AS accounts
            ON accounts.LegacyUserId = legacySchedules.UserID
            AND accounts.LegacyUserType = legacySchedules.UserType
            AND accounts.AccountRank = 1
        INNER JOIN Professionals
            ON Professionals.LegacyUserId = accounts.LegacyCompanyUserId
        INNER JOIN AspNetUsers
            ON AspNetUsers.UserName = legacySchedules.UserID
        INNER JOIN ProfessionalUsers
            ON ProfessionalUsers.ProfessionalId = Professionals.Id
            AND ProfessionalUsers.UserId = AspNetUsers.Id
    )
    INSERT INTO SiteSchedules
        (LegacyRecordId, ProfessionalId, UserId, ProfessionalType, SiteId, ScheduleDate, CreatedById, CreatedTime)
    SELECT
        rankedSchedules.ID,
        rankedSchedules.MigratedProfessionalId,
        rankedSchedules.MigratedUserId,
        rankedSchedules.UserType,
        rankedSchedules.MigratedSiteId,
        rankedSchedules.ScheduleDate,
        NULL,
        rankedSchedules.CreationDate
    FROM RankedSchedules AS rankedSchedules
    WHERE rankedSchedules.ScheduleRank = 1
        AND NOT EXISTS
        (
            SELECT 1
            FROM SiteSchedules AS existingSchedules
            WHERE existingSchedules.SiteId = rankedSchedules.MigratedSiteId
                AND existingSchedules.ProfessionalId = rankedSchedules.MigratedProfessionalId
                AND existingSchedules.UserId = rankedSchedules.MigratedUserId
                AND existingSchedules.ProfessionalType = rankedSchedules.UserType
        )

    COMMIT TRAN

END TRY
BEGIN CATCH
    ROLLBACK TRAN;
    THROW;
END CATCH
