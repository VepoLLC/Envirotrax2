BEGIN TRAN

BEGIN TRY

    -- Only pending verifications move across: a verified address is already in the site's
    -- MailingEmailAddress, and an unsubscribed one must not be re-subscribed by an old link. Expired
    -- ones are kept too, so their link shows "expired" in V2 instead of "invalid".
    INSERT INTO MigrationSkippedRenewalEmailVerifications (VerificationId, SourceTable, Reason)
    SELECT legacyVerifications.ID, 'RenewalEmailVerifications', 'No matching migrated site'
    FROM Vepo.dbo.RenewalEmailVerifications AS legacyVerifications
    LEFT JOIN Sites
        ON Sites.LegacyRecordId = legacyVerifications.SiteID
    LEFT JOIN MigrationSkippedRenewalEmailVerifications AS alreadySkipped
        ON alreadySkipped.VerificationId = legacyVerifications.ID
        AND alreadySkipped.SourceTable = 'RenewalEmailVerifications'
    WHERE legacyVerifications.IsVerified = 0
        AND legacyVerifications.UnsubscribedDate IS NULL
        AND Sites.Id IS NULL
        AND alreadySkipped.VerificationId IS NULL

    -- The verification link in a V1 email carries the V1 row id, so the row keeps that id in V2.
    -- A V2 row already holding it was created in V2 itself, and the V1 link cannot be honoured.
    INSERT INTO MigrationSkippedRenewalEmailVerifications (VerificationId, SourceTable, Reason)
    SELECT legacyVerifications.ID, 'RenewalEmailVerifications', 'Id is already used by a verification created in V2'
    FROM Vepo.dbo.RenewalEmailVerifications AS legacyVerifications
    INNER JOIN RenewalEmailVerifications AS existing
        ON existing.Id = legacyVerifications.ID
        AND existing.LegacyRecordId IS NULL
    LEFT JOIN MigrationSkippedRenewalEmailVerifications AS alreadySkipped
        ON alreadySkipped.VerificationId = legacyVerifications.ID
        AND alreadySkipped.SourceTable = 'RenewalEmailVerifications'
    WHERE legacyVerifications.IsVerified = 0
        AND legacyVerifications.UnsubscribedDate IS NULL
        AND alreadySkipped.VerificationId IS NULL

    SET IDENTITY_INSERT RenewalEmailVerifications ON;

    -- TokenHash is copied as is. It is a BCrypt hash in V1, and RenewalOptInService accepts those
    -- alongside the SHA-256 hashes V2 writes.
    INSERT INTO RenewalEmailVerifications
        (Id, LegacyRecordId, SiteId, EmailAddress, TokenHash, IsVerified, VerifiedDate, CreatedDate, UnsubscribeTokenHash, UnsubscribedDate)
    SELECT
        legacyVerifications.ID,
        legacyVerifications.ID,
        sites.Id,
        legacyVerifications.EmailAddress,
        legacyVerifications.TokenHash,
        0,
        NULL,
        legacyVerifications.CreatedDate,
        NULL,
        NULL
    FROM Vepo.dbo.RenewalEmailVerifications AS legacyVerifications
    INNER JOIN Sites AS sites
        ON sites.LegacyRecordId = legacyVerifications.SiteID
    WHERE legacyVerifications.IsVerified = 0
        AND legacyVerifications.UnsubscribedDate IS NULL
        AND NOT EXISTS
        (
            SELECT 1
            FROM RenewalEmailVerifications AS alreadyInserted
            WHERE alreadyInserted.Id = legacyVerifications.ID
        )

    SET IDENTITY_INSERT RenewalEmailVerifications OFF;

    COMMIT TRAN

END TRY
BEGIN CATCH
    ROLLBACK TRAN;
    THROW;
END CATCH
