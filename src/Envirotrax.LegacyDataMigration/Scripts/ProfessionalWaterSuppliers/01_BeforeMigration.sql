IF COL_LENGTH('ProfessionalWaterSuppliers', 'LegacyRecordId') IS NULL
BEGIN
    ALTER TABLE ProfessionalWaterSuppliers
    ADD LegacyRecordId INT NULL;
END

IF EXISTS
(
    SELECT 1
    FROM Vepo.dbo.SaveWaterSupplierRegistrations AS registrations
    INNER JOIN MigrationLegacyProfessionalAccounts AS accounts
        ON accounts.LegacyUserId = registrations.UserID
        AND accounts.LegacyUserType = registrations.UserType
    WHERE registrations.Active = 1
        AND registrations.UserType = 6
        AND registrations.UserID = accounts.LegacyCompanyUserId
        AND NOT
        (
            (ISNULL(registrations.CommercialFee, -1) < 0 AND ISNULL(registrations.ResidentialFee, -1) < 0)
            OR (registrations.CommercialFee >= 0 AND registrations.ResidentialFee >= 0
                AND registrations.CommercialFee = registrations.ResidentialFee)
        )
)
BEGIN
    RAISERROR(N'A FOG inspector registration prices residential and commercial work differently, and ProfessionalWaterSuppliers has one FogInspectorFee that the migration fills from CommercialFee. Decide which fee V2 keeps before migrating.', 16, 1)
END
