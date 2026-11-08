-- Not unique on its own: one V1 comment becomes a note under every water supplier its author belongs to.
IF COL_LENGTH('ProfessionalNotes', 'LegacyRecordId') IS NULL
BEGIN
    ALTER TABLE ProfessionalNotes
    ADD LegacyRecordId INT NULL;
END

IF COL_LENGTH('ProfessionalUserNotes', 'LegacyRecordId') IS NULL
BEGIN
    ALTER TABLE ProfessionalUserNotes
    ADD LegacyRecordId INT NULL;
END
