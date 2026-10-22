DECLARE @NextNumber BIGINT;

SELECT @NextNumber = ISNULL(MAX(TRY_CAST(AccountNumber AS BIGINT)), 99999) + 1
FROM dbo.Sites;

DECLARE @Sql NVARCHAR(MAX) =
    N'ALTER SEQUENCE dbo.SiteAccountNumberSequence RESTART WITH '
    + CAST(@NextNumber AS NVARCHAR(20));

EXEC sp_executesql @Sql;
