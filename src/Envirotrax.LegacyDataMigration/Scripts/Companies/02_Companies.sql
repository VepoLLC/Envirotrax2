BEGIN TRAN

BEGIN TRY

    -- V1 Vepo Manager's Contact Manager keeps the companies Vepo works with in ContactManagerContacts. Only Vepo
    -- administrators use it, and V2 has them as users of the Vepo LLC water supplier (WaterSupplier.EnvirotraxAdminDomain),
    -- so every company belongs to that supplier.
    INSERT INTO crm.Companies
        (LegacyRecordId, WaterSupplierId, ContactType, Name, Position, ContactName, Address, City, StateId, ZipCode,
         EmailAddress, PhoneNumber, CellNumber,
         HasBackflowTesting, BackflowTestingStartDate, HasCsiInspection, CsiInspectionStartDate,
         HasFogInspection, FogInspectionStartDate, HasFogTransportation, FogTransportationStartDate,
         LastContactedById, LastContactDate, NextContactDate, CreatedById, CreatedTime)
    SELECT
        legacyContacts.ID,
        vepoSupplier.Id,
        -- V1 Type and the V2 ContactType enum share their values: 0 Existing Client, 1 Prospective Client,
        -- 2 Past Client, 3 Competitor Client, 4 Technology.
        legacyContacts.Type,
        legacyContacts.Name,
        -- V1 keeps a blank field as an empty string in its NOT NULL columns; V2 keeps it as NULL.
        NULLIF(legacyContacts.Position, ''),
        NULLIF(legacyContacts.ContactName, ''),
        NULLIF(legacyContacts.Address, ''),
        NULLIF(legacyContacts.City, ''),
        states.Id,
        NULLIF(legacyContacts.ZIP, ''),
        NULLIF(legacyContacts.EmailAddress, ''),
        NULLIF(legacyContacts.PhoneNumber, ''),
        NULLIF(legacyContacts.CellNumber, ''),
        legacyContacts.BackflowProgram,
        legacyContacts.BackflowProgramStartDate,
        legacyContacts.CsiProgram,
        legacyContacts.CsiProgramStartDate,
        legacyContacts.FogProgram,
        legacyContacts.FogProgramStartDate,
        -- V1 has no FOG transportation program.
        0,
        NULL,
        -- V1 names the administrator by login. One whose login did not migrate leaves the field blank rather
        -- than dropping the company.
        lastContactedBy.Id,
        legacyContacts.LastContactDate,
        legacyContacts.NextContactDate,
        NULL,
        legacyContacts.CreationDate
    FROM Vepo.dbo.ContactManagerContacts AS legacyContacts
    INNER JOIN WaterSuppliers AS vepoSupplier
        ON vepoSupplier.Domain = 'vepollc'
    -- V1 stores the full state name here (Texas), but the code is matched too in case some rows hold an abbreviation.
    LEFT JOIN States AS states
        ON states.Name = legacyContacts.State
        OR states.Code = legacyContacts.State
    LEFT JOIN AspNetUsers AS lastContactedBy
        ON lastContactedBy.UserName = NULLIF(legacyContacts.LastContactedBy, '')
    WHERE NOT EXISTS
    (
        SELECT 1
        FROM crm.Companies AS alreadyInserted
        WHERE alreadyInserted.LegacyRecordId = legacyContacts.ID
    )

    COMMIT TRAN

END TRY
BEGIN CATCH
    ROLLBACK TRAN;
    THROW;
END CATCH
