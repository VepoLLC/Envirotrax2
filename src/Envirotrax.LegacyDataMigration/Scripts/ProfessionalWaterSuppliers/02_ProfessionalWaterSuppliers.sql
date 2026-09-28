BEGIN TRAN

BEGIN TRY

    INSERT INTO MigrationSkippedProfessionals (LegacyRecordId, LegacyUserId, SourceTable, Reason)
    SELECT registrations.ID, registrations.UserID, 'SaveWaterSupplierRegistrations', 'No professional account for the registration'
    FROM Vepo.dbo.SaveWaterSupplierRegistrations AS registrations
    WHERE (registrations.Active = 1 OR registrations.Banned = 1)
        AND registrations.UserType IN (2, 4, 5, 6)
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

    -- A sub-account can hold a registration its master never had. V1 gives it to nobody, because every
    -- lookup goes to the master, so the company does not become registered there and the row is recorded
    -- rather than migrated.
    INSERT INTO MigrationSkippedProfessionals (LegacyRecordId, LegacyUserId, SourceTable, Reason)
    SELECT registrations.ID, registrations.UserID, 'SaveWaterSupplierRegistrations', 'Registration belongs to a sub-account and its master has none'
    FROM Vepo.dbo.SaveWaterSupplierRegistrations AS registrations
    INNER JOIN MigrationLegacyProfessionalAccounts AS accounts
        ON accounts.LegacyUserId = registrations.UserID
        AND accounts.LegacyUserType = registrations.UserType
    WHERE (registrations.Active = 1 OR registrations.Banned = 1)
        AND registrations.UserID <> accounts.LegacyCompanyUserId
        AND NOT EXISTS
        (
            SELECT 1
            FROM Vepo.dbo.SaveWaterSupplierRegistrations AS masterRegistrations
            WHERE masterRegistrations.WaterSupplierID = registrations.WaterSupplierID
                AND masterRegistrations.UserID = accounts.LegacyCompanyUserId
                AND masterRegistrations.UserType = registrations.UserType
                AND masterRegistrations.Active = 1
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
            registrations.Active,
            registrations.Banned,
            registrations.CommercialFee,
            registrations.ResidentialFee
        FROM Vepo.dbo.SaveWaterSupplierRegistrations AS registrations
        INNER JOIN MigrationLegacyProfessionalAccounts AS accounts
            ON accounts.LegacyUserId = registrations.UserID
            AND accounts.LegacyUserType = registrations.UserType
        -- A registration belongs to the company, and V1 only ever reads the master account's: the submit
        -- pages, the site pickers and the registration page itself all look it up by MasterBpatID /
        -- MasterInspectorID / MasterTransporterID when a sub-account is the one asking
        -- (backflow_test_submit.aspx.vb:373, water_supplier_registration.aspx.vb:121). A registration a
        -- sub-account holds on its own lets nobody work, so it must not register the company here.
        WHERE registrations.UserID = accounts.LegacyCompanyUserId
            -- An inactive registration still carries the ban that stops the company from registering
            -- again (water_supplier_registration_worker.aspx.vb:70 refuses to switch a banned one back
            -- on), so it is kept for that alone. Only active ones say the company works there.
            AND (registrations.Active = 1 OR registrations.Banned = 1)
    ),
    CompanyRegistrations AS
    (
        SELECT
            WaterSupplierID,
            LegacyCompanyUserId,
            -- One of the master's registrations, which is what this row is built from. A company holds
            -- one per professional type, so this names the pair rather than any single fee - the fees
            -- below each come from the registration of their own type.
            MAX(ID) AS LegacyRecordId,
            MAX(CASE WHEN Active = 1 AND UserType = 2 THEN 1 ELSE 0 END) AS HasBackflowTesting,
            MAX(CASE WHEN Active = 1 AND UserType = 4 THEN 1 ELSE 0 END) AS HasCsiInpection,
            MAX(CASE WHEN Active = 1 AND UserType = 5 THEN 1 ELSE 0 END) AS HasFogTransportation,
            MAX(CASE WHEN Active = 1 AND UserType = 6 THEN 1 ELSE 0 END) AS HasFogInspection,
            -- A ban in V1 sits on one registration, so it stops one service at one water supplier and
            -- leaves the company's other services there alone. V2 keeps a suspension flag per service
            -- for the same reason, and each one is filled from the registration of its own type.
            MAX(CASE WHEN Banned = 1 AND UserType = 2 THEN 1 ELSE 0 END) AS IsBackflowTestingSuspended,
            MAX(CASE WHEN Banned = 1 AND UserType = 4 THEN 1 ELSE 0 END) AS IsCsiInspectionSuspended,
            MAX(CASE WHEN Banned = 1 AND UserType = 5 THEN 1 ELSE 0 END) AS IsFogTransportationSuspended,
            MAX(CASE WHEN Banned = 1 AND UserType = 6 THEN 1 ELSE 0 END) AS IsFogInspectionSuspended,
            -- V1 stores -1 when the professional has no fee of their own and the water supplier's fee
            -- applies (the `wsReg.CommercialFee >= 0` check in backflow_test_submit.aspx.vb). In V2 that
            -- is NULL, while zero stays a real override meaning the work is free. FOG keeps one fee here
            -- because the transporter only ever charges CommercialFee (trip_ticket_submit.aspx.vb:1674).
            -- The FOG inspector does read ResidentialFee on a residential property
            -- (inspection_submit.aspx.vb:1192), but V2 has no column for it, so 01_BeforeMigration stops the
            -- run if any registration charges a residential fee that would be dropped here.
            -- SaveWaterSupplierRegistrations.FogFee is only ever written, never read.
            MAX(CASE WHEN Active = 1 AND UserType = 2 AND CommercialFee >= 0 THEN CommercialFee END) AS BackflowCommercialTestFee,
            MAX(CASE WHEN Active = 1 AND UserType = 2 AND ResidentialFee >= 0 THEN ResidentialFee END) AS BackflowResidentialTestFee,
            MAX(CASE WHEN Active = 1 AND UserType = 4 AND CommercialFee >= 0 THEN CommercialFee END) AS CsiCommercialInspectionFee,
            MAX(CASE WHEN Active = 1 AND UserType = 4 AND ResidentialFee >= 0 THEN ResidentialFee END) AS CsiResidentialInspectionFee,
            MAX(CASE WHEN Active = 1 AND UserType = 5 AND CommercialFee >= 0 THEN CommercialFee END) AS FogTransportFee,
            MAX(CASE WHEN Active = 1 AND UserType = 6 AND CommercialFee >= 0 THEN CommercialFee END) AS FogInspectorFee
        FROM Registrations
        GROUP BY WaterSupplierID, LegacyCompanyUserId
    )
    INSERT INTO ProfessionalWaterSuppliers
        (WaterSupplierId, ProfessionalId, LegacyRecordId,
         HasWiseGuys, HasBackflowTesting, HasCsiInpection, HasFogInspection, HasFogTransportation,
         IsBackflowTestingSuspended, IsCsiInspectionSuspended, IsFogInspectionSuspended, IsFogTransportationSuspended,
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
        companyRegistrations.IsBackflowTestingSuspended,
        companyRegistrations.IsCsiInspectionSuspended,
        companyRegistrations.IsFogInspectionSuspended,
        companyRegistrations.IsFogTransportationSuspended,
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
