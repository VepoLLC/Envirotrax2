BEGIN TRAN

BEGIN TRY

    TRUNCATE TABLE MigrationLegacyProfessionalAccounts

    INSERT INTO MigrationLegacyProfessionalAccounts
        (SourceTable, SourcePriority, LegacyRecordId, LegacyUserId, LegacyCompanyUserId, LegacyUserType, IsCompanyOwner,
         CompanyName, ContactName, JobTitle, EmailAddress,
         Address, City, State, DefaultState, ZipCode, WorkNumber, FaxNumber, WebsiteUrl,
         HidePublicListing, AdminAccount, AccountBalance, CreationDate,
         BillingFirstName, BillingLastName, BillingAddress, BillingCity, BillingState, BillingZipCode)
    SELECT
        'SaveBpats', 1, bpats.ID, bpats.UserID,
        ISNULL(NULLIF(bpats.MasterBpatID, ''), bpats.UserID), 2,
        CASE WHEN ISNULL(bpats.MasterBpatID, '') = '' THEN 1 ELSE 0 END,
        bpats.CompanyName, bpats.ContactName, NULL, NULL,
        bpats.Address, bpats.City, bpats.State, bpats.DefaultState, bpats.ZIP, bpats.WorkNumber, bpats.FaxNumber, bpats.WebsiteUrl,
        bpats.HidePublicListing, bpats.AdminAccount, bpats.AccountBalance, bpats.CreationDate,
        bpats.BillingFirstName, bpats.BillingLastName, bpats.BillingAddress, bpats.BillingCity, bpats.BillingState, bpats.BillingZIP
    FROM Vepo.dbo.SaveBpats AS bpats

    INSERT INTO MigrationLegacyProfessionalAccounts
        (SourceTable, SourcePriority, LegacyRecordId, LegacyUserId, LegacyCompanyUserId, LegacyUserType, IsCompanyOwner,
         CompanyName, ContactName, JobTitle, EmailAddress,
         Address, City, State, DefaultState, ZipCode, WorkNumber, FaxNumber, WebsiteUrl,
         HidePublicListing, AdminAccount, AccountBalance, CreationDate,
         BillingFirstName, BillingLastName, BillingAddress, BillingCity, BillingState, BillingZipCode)
    SELECT
        'CsiInspectors', 2, inspectors.ID, inspectors.UserID,
        ISNULL(NULLIF(inspectors.MasterInspectorID, ''), inspectors.UserID), 4,
        CASE WHEN ISNULL(inspectors.MasterInspectorID, '') = '' THEN 1 ELSE 0 END,
        inspectors.CompanyName, inspectors.ContactName, inspectors.JobTitle, NULL,
        inspectors.Address, inspectors.City, inspectors.State, inspectors.DefaultState, inspectors.ZIP, inspectors.WorkNumber, inspectors.FaxNumber, inspectors.WebsiteUrl,
        inspectors.HidePublicListing, inspectors.AdminAccount, inspectors.AccountBalance, inspectors.CreationDate,
        inspectors.BillingFirstName, inspectors.BillingLastName, inspectors.BillingAddress, inspectors.BillingCity, inspectors.BillingState, inspectors.BillingZIP
    FROM Vepo.dbo.CsiInspectors AS inspectors

    INSERT INTO MigrationLegacyProfessionalAccounts
        (SourceTable, SourcePriority, LegacyRecordId, LegacyUserId, LegacyCompanyUserId, LegacyUserType, IsCompanyOwner,
         CompanyName, ContactName, JobTitle, EmailAddress,
         Address, City, State, DefaultState, ZipCode, WorkNumber, FaxNumber, WebsiteUrl,
         HidePublicListing, AdminAccount, AccountBalance, CreationDate,
         BillingFirstName, BillingLastName, BillingAddress, BillingCity, BillingState, BillingZipCode)
    SELECT
        'FogTransporters', 3, transporters.ID, transporters.UserID,
        ISNULL(NULLIF(transporters.MasterTransporterID, ''), transporters.UserID), 5,
        CASE WHEN ISNULL(transporters.MasterTransporterID, '') = '' THEN 1 ELSE 0 END,
        transporters.CompanyName, transporters.ContactName, NULL, transporters.EmailAddress,
        transporters.Address, transporters.City, transporters.State, transporters.DefaultState, transporters.ZIP, transporters.WorkNumber, transporters.FaxNumber, transporters.WebsiteUrl,
        transporters.HidePublicListing, transporters.AdminAccount, transporters.AccountBalance, transporters.CreationDate,
        transporters.BillingFirstName, transporters.BillingLastName, transporters.BillingAddress, transporters.BillingCity, transporters.BillingState, transporters.BillingZIP
    FROM Vepo.dbo.FogTransporters AS transporters

    INSERT INTO MigrationLegacyProfessionalAccounts
        (SourceTable, SourcePriority, LegacyRecordId, LegacyUserId, LegacyCompanyUserId, LegacyUserType, IsCompanyOwner,
         CompanyName, ContactName, JobTitle, EmailAddress,
         Address, City, State, DefaultState, ZipCode, WorkNumber, FaxNumber, WebsiteUrl,
         HidePublicListing, AdminAccount, AccountBalance, CreationDate,
         BillingFirstName, BillingLastName, BillingAddress, BillingCity, BillingState, BillingZipCode)
    SELECT
        'FogInspectors', 4, inspectors.ID, inspectors.UserID,
        ISNULL(NULLIF(inspectors.MasterInspectorID, ''), inspectors.UserID), 6,
        CASE WHEN ISNULL(inspectors.MasterInspectorID, '') = '' THEN 1 ELSE 0 END,
        inspectors.CompanyName, inspectors.ContactName, NULL, inspectors.EmailAddress,
        inspectors.Address, inspectors.City, inspectors.State, inspectors.DefaultState, inspectors.ZIP, inspectors.WorkNumber, inspectors.FaxNumber, inspectors.WebsiteUrl,
        inspectors.HidePublicListing, inspectors.AdminAccount, inspectors.AccountBalance, inspectors.CreationDate,
        inspectors.BillingFirstName, inspectors.BillingLastName, inspectors.BillingAddress, inspectors.BillingCity, inspectors.BillingState, inspectors.BillingZIP
    FROM Vepo.dbo.FogInspectors AS inspectors

    COMMIT TRAN

END TRY
BEGIN CATCH
    ROLLBACK TRAN;
    THROW;
END CATCH
