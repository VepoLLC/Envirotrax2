BEGIN TRAN

BEGIN TRY

    -- V1 Vepo Manager keeps a company's other contacts in ContactManagerAdditionalContacts. Their ContactID is the
    -- company's ContactManagerContacts.ID, which 02_Companies.sql keeps in crm.Companies.LegacyRecordId.
    --
    -- Two kinds of V1 rows are left out, and logged here first:
    --  - contacts of companies V1 no longer has: it has no foreign key from a contact to its company;
    --  - rows blank in every field, which are not contacts. Every V1 column here is NOT NULL, with '' for a blank
    --    field, so the fields joined together are '' only when all of them are blank.
    INSERT INTO MigrationSkippedCompanyContacts (LegacyRecordId, SourceTable, Reason)
    SELECT
        legacyContacts.ID,
        'ContactManagerAdditionalContacts',
        CASE
            WHEN companies.Id IS NULL THEN 'No migrated company for the contact'
            ELSE 'Blank in every field'
        END
    FROM Vepo.dbo.ContactManagerAdditionalContacts AS legacyContacts
    LEFT JOIN crm.Companies AS companies
        ON companies.LegacyRecordId = legacyContacts.ContactID
    WHERE
    (
        companies.Id IS NULL
        OR legacyContacts.Position + legacyContacts.ContactName + legacyContacts.Address + legacyContacts.City
            + legacyContacts.State + legacyContacts.ZIP + legacyContacts.PhoneNumber + legacyContacts.CellNumber
            + legacyContacts.EmailAddress = ''
    )
    AND NOT EXISTS
    (
        SELECT 1
        FROM MigrationSkippedCompanyContacts AS alreadySkipped
        WHERE alreadySkipped.LegacyRecordId = legacyContacts.ID
            AND alreadySkipped.SourceTable = 'ContactManagerAdditionalContacts'
    )

    -- The key to the company includes its water supplier, so a contact takes the company's.
    INSERT INTO crm.CompanyContacts
        (LegacyRecordId, WaterSupplierId, CompanyId, Position, ContactName, Address, City, StateId, ZipCode,
         PhoneNumber, CellNumber, EmailAddress, CreatedById, CreatedTime)
    SELECT
        legacyContacts.ID,
        companies.WaterSupplierId,
        companies.Id,
        -- V1 keeps a blank field as an empty string in its NOT NULL columns; V2 keeps it as NULL.
        NULLIF(legacyContacts.Position, ''),
        NULLIF(legacyContacts.ContactName, ''),
        NULLIF(legacyContacts.Address, ''),
        NULLIF(legacyContacts.City, ''),
        states.Id,
        NULLIF(legacyContacts.ZIP, ''),
        NULLIF(legacyContacts.PhoneNumber, ''),
        NULLIF(legacyContacts.CellNumber, ''),
        NULLIF(legacyContacts.EmailAddress, ''),
        -- V1 does not record who added a contact.
        NULL,
        legacyContacts.CreationDate
    FROM Vepo.dbo.ContactManagerAdditionalContacts AS legacyContacts
    INNER JOIN crm.Companies AS companies
        ON companies.LegacyRecordId = legacyContacts.ContactID
    -- V1 stores the state code here (TX), but the name is matched too in case some rows hold a full name.
    LEFT JOIN States AS states
        ON states.Code = legacyContacts.State
        OR states.Name = legacyContacts.State
    WHERE legacyContacts.Position + legacyContacts.ContactName + legacyContacts.Address + legacyContacts.City
            + legacyContacts.State + legacyContacts.ZIP + legacyContacts.PhoneNumber + legacyContacts.CellNumber
            + legacyContacts.EmailAddress <> ''
        AND NOT EXISTS
        (
            SELECT 1
            FROM crm.CompanyContacts AS alreadyInserted
            WHERE alreadyInserted.LegacyRecordId = legacyContacts.ID
        )
    -- V2 lists a company's contacts by Id, so the rows go in in the order V1 added them. MAXDOP 1 keeps the plan
    -- serial, otherwise identity values may not follow it.
    ORDER BY legacyContacts.ID
    OPTION (MAXDOP 1)

    -- A contact skipped by an earlier run goes in once its company has been migrated. Its entry no longer
    -- describes anything, so it is cleared to keep this table a list of what is really missing.
    DELETE skipped
    FROM MigrationSkippedCompanyContacts AS skipped
    INNER JOIN crm.CompanyContacts AS contacts
        ON contacts.LegacyRecordId = skipped.LegacyRecordId
    WHERE skipped.SourceTable = 'ContactManagerAdditionalContacts'

    COMMIT TRAN

END TRY
BEGIN CATCH
    ROLLBACK TRAN;
    THROW;
END CATCH
