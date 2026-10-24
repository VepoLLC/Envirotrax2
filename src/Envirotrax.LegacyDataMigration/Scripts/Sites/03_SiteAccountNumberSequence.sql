DECLARE @NextNumber BIGINT;
DECLARE @CurrentValue BIGINT;

SELECT @NextNumber = ISNULL(MAX(Id), 0) + 1
FROM dbo.Sites;

SELECT @CurrentValue = CAST(current_value AS BIGINT)
FROM sys.sequences
WHERE object_id = OBJECT_ID('dbo.SiteAccountNumberSequence');

IF @NextNumber > @CurrentValue
BEGIN
    DECLARE @Sql NVARCHAR(MAX) =
        N'ALTER SEQUENCE dbo.SiteAccountNumberSequence RESTART WITH '
        + CAST(@NextNumber AS NVARCHAR(20));

    EXEC sp_executesql @Sql;
END
