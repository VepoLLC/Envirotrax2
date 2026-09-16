IF COL_LENGTH('Professionals', 'LegacyRecordId') IS NULL
BEGIN
    ALTER TABLE Professionals
    ADD LegacyRecordId INT NULL;
END

IF COL_LENGTH('Professionals', 'LegacyUserId') IS NULL
BEGIN
    ALTER TABLE Professionals
    ADD LegacyUserId NVARCHAR(100) NULL;
END

IF COL_LENGTH('ProfessionalUsers', 'LegacyRecordId') IS NULL
BEGIN
    ALTER TABLE ProfessionalUsers
    ADD LegacyRecordId INT NULL;
END

IF COL_LENGTH('ProfessionalUsers', 'LegacyUserId') IS NULL
BEGIN
    ALTER TABLE ProfessionalUsers
    ADD LegacyUserId NVARCHAR(100) NULL;
END

IF COL_LENGTH('ProfessionalWaterSuppliers', 'LegacyRecordId') IS NULL
BEGIN
    ALTER TABLE ProfessionalWaterSuppliers
    ADD LegacyRecordId INT NULL;
END

IF COL_LENGTH('ProfessionalUserLicenses', 'LegacyRecordId') IS NULL
BEGIN
    ALTER TABLE ProfessionalUserLicenses
    ADD LegacyRecordId INT NULL;
END

IF COL_LENGTH('ProfessionalInsurances', 'LegacyRecordId') IS NULL
BEGIN
    ALTER TABLE ProfessionalInsurances
    ADD LegacyRecordId INT NULL;
END

-- Where the policy file sits on the legacy file server, until it is copied into Azure Storage.
IF COL_LENGTH('ProfessionalInsurances', 'LegacyFilePath') IS NULL
BEGIN
    ALTER TABLE ProfessionalInsurances
    ADD LegacyFilePath NVARCHAR(500) NULL;
END

IF OBJECT_ID('MigrationSkippedProfessionals', 'U') IS NULL
BEGIN
    CREATE TABLE MigrationSkippedProfessionals (
        LegacyRecordId INT NULL,
        LegacyUserId NVARCHAR(100) NULL,
        SourceTable NVARCHAR(256),
        Reason NVARCHAR(500),
        MigratedAt DATETIME DEFAULT GETUTCDATE()
    )
END

-- In V1 every professional type lives in its own table, and one row holds the company, the person and
-- the login all at once. This table brings all four sources to a single shape: LegacyCompanyUserId is
-- the login of the master account, which becomes the company, and LegacyUserId is the login of the
-- person. It also stays behind as the record of which V1 rows each V2 row was built from.
-- Scratch: 02 rebuilds it from V1 on every run, so it is dropped rather than kept, and the columns stay
-- nullable because V1 leaves several of them empty. The scripts that read it decide what an empty one
-- means; forcing NOT NULL here would only fail the load before anything had a chance to.
IF OBJECT_ID('MigrationLegacyProfessionalAccounts', 'U') IS NOT NULL
BEGIN
    DROP TABLE MigrationLegacyProfessionalAccounts
END

CREATE TABLE MigrationLegacyProfessionalAccounts (
    SourceTable NVARCHAR(50) NOT NULL,
    SourcePriority INT NOT NULL,
    LegacyRecordId INT NOT NULL,
    LegacyUserId NVARCHAR(100) NOT NULL,
    LegacyCompanyUserId NVARCHAR(100) NOT NULL,
    LegacyUserType INT NOT NULL,
    IsCompanyOwner BIT NOT NULL,
    CompanyName NVARCHAR(255) NULL,
    ContactName NVARCHAR(255) NULL,
    JobTitle NVARCHAR(150) NULL,
    EmailAddress NVARCHAR(255) NULL,
    Address NVARCHAR(255) NULL,
    City NVARCHAR(255) NULL,
    State NVARCHAR(255) NULL,
    DefaultState NVARCHAR(50) NULL,
    ZipCode NVARCHAR(255) NULL,
    WorkNumber NVARCHAR(50) NULL,
    FaxNumber NVARCHAR(50) NULL,
    WebsiteUrl NVARCHAR(255) NULL,
    HidePublicListing BIT NULL,
    AdminAccount BIT NULL,
    AccountBalance MONEY NULL,
    CreationDate DATETIME NULL,
    BillingFirstName NVARCHAR(255) NULL,
    BillingLastName NVARCHAR(255) NULL,
    BillingAddress NVARCHAR(255) NULL,
    BillingCity NVARCHAR(255) NULL,
    BillingState NVARCHAR(255) NULL,
    BillingZipCode NVARCHAR(255) NULL
)

CREATE INDEX IX_MigrationLegacyProfessionalAccounts_Company
    ON MigrationLegacyProfessionalAccounts (LegacyCompanyUserId)

CREATE INDEX IX_MigrationLegacyProfessionalAccounts_User
    ON MigrationLegacyProfessionalAccounts (LegacyUserId, LegacyUserType)
