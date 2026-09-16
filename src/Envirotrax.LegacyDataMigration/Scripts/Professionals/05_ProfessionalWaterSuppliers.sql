BEGIN TRAN

BEGIN TRY

    INSERT INTO MigrationSkippedProfessionals (LegacyRecordId, LegacyUserId, SourceTable, Reason)
    SELECT registrations.ID, registrations.UserID, 'SaveWaterSupplierRegistrations', 'No professional account for the registration'
    FROM Vepo.dbo.SaveWaterSupplierRegistrations AS registrations
    WHERE registrations.Active = 1
        AND NOT EXISTS
        (
            SELECT 1
            FROM MigrationLegacyProfessionalAccounts AS accounts
            WHERE accounts.LegacyUserId = registrations.UserID
                AND accounts.LegacyUserType = registrations.UserType
        )
        AND NOT EXISTS
        (
            SELECT 1
            FROM MigrationSkippedProfessionals AS skipped
            WHERE skipped.LegacyRecordId = registrations.ID
                AND skipped.SourceTable = 'SaveWaterSupplierRegistrations'
        )

    -- In V1 a registration belongs to a login and to one professional type, so a single company ends up
    -- with several rows per water supplier. In V2 that is one row carrying the flags and the fees.
    ;WITH Registrations AS
    (
        SELECT
            registrations.ID,
            registrations.WaterSupplierID,
            accounts.LegacyCompanyUserId,
            registrations.UserType,
            registrations.Banned,
            registrations.CommercialFee,
            registrations.ResidentialFee,
            registrations.FogFee
        FROM Vepo.dbo.SaveWaterSupplierRegistrations AS registrations
        INNER JOIN MigrationLegacyProfessionalAccounts AS accounts
            ON accounts.LegacyUserId = registrations.UserID
            AND accounts.LegacyUserType = registrations.UserType
        WHERE registrations.Active = 1
    ),
    CompanyRegistrations AS
    (
        SELECT
            WaterSupplierID,
            LegacyCompanyUserId,
            MAX(ID) AS LegacyRecordId,
            MAX(CASE WHEN UserType = 2 THEN 1 ELSE 0 END) AS HasBackflowTesting,
            MAX(CASE WHEN UserType = 4 THEN 1 ELSE 0 END) AS HasCsiInpection,
            MAX(CASE WHEN UserType = 5 THEN 1 ELSE 0 END) AS HasFogTransportation,
            MAX(CASE WHEN UserType = 6 THEN 1 ELSE 0 END) AS HasFogInspection,
            MAX(CAST(Banned AS INT)) AS IsBanned,
            -- V1 stores -1 when the professional has no fee of their own and the water supplier's fee
            -- applies (the `wsReg.CommercialFee >= 0` check in backflow_test_submit.aspx.vb). In V2 that
            -- is NULL, while zero stays a real override meaning the work is free.
            MAX(CASE WHEN UserType = 2 AND CommercialFee >= 0 THEN CommercialFee END) AS BackflowCommercialTestFee,
            MAX(CASE WHEN UserType = 2 AND ResidentialFee >= 0 THEN ResidentialFee END) AS BackflowResidentialTestFee,
            MAX(CASE WHEN UserType = 4 AND CommercialFee >= 0 THEN CommercialFee END) AS CsiCommercialInspectionFee,
            MAX(CASE WHEN UserType = 4 AND ResidentialFee >= 0 THEN ResidentialFee END) AS CsiResidentialInspectionFee,
            MAX(CASE WHEN UserType = 5 AND FogFee >= 0 THEN FogFee END) AS FogTransportFee,
            MAX(CASE WHEN UserType = 6 AND FogFee >= 0 THEN FogFee END) AS FogInspectorFee
        FROM Registrations
        GROUP BY WaterSupplierID, LegacyCompanyUserId
    )
    INSERT INTO ProfessionalWaterSuppliers
        (WaterSupplierId, ProfessionalId, LegacyRecordId,
         HasWiseGuys, HasBackflowTesting, HasCsiInpection, HasFogInspection, HasFogTransportation, IsBanned,
         BackflowCommercialTestFee, BackflowResidentialTestFee,
         CsiCommercialInspectionFee, CsiResidentialInspectionFee,
         FogTransportFee, FogInspectorFee)
    SELECT
        suppliers.Id,
        professionals.Id,
        companyRegistrations.LegacyRecordId,
        0,
        companyRegistrations.HasBackflowTesting,
        companyRegistrations.HasCsiInpection,
        companyRegistrations.HasFogInspection,
        companyRegistrations.HasFogTransportation,
        companyRegistrations.IsBanned,
        companyRegistrations.BackflowCommercialTestFee,
        companyRegistrations.BackflowResidentialTestFee,
        companyRegistrations.CsiCommercialInspectionFee,
        companyRegistrations.CsiResidentialInspectionFee,
        companyRegistrations.FogTransportFee,
        companyRegistrations.FogInspectorFee
    FROM CompanyRegistrations AS companyRegistrations
    INNER JOIN WaterSuppliers AS suppliers
        ON suppliers.LegacyRecordId = companyRegistrations.WaterSupplierID
    INNER JOIN Professionals AS professionals
        ON professionals.LegacyUserId = companyRegistrations.LegacyCompanyUserId
    WHERE NOT EXISTS
    (
        SELECT 1
        FROM ProfessionalWaterSuppliers AS alreadyInserted
        WHERE alreadyInserted.WaterSupplierId = suppliers.Id
            AND alreadyInserted.ProfessionalId = professionals.Id
    )

    COMMIT TRAN

END TRY
BEGIN CATCH
    ROLLBACK TRAN;
    THROW;
END CATCH
