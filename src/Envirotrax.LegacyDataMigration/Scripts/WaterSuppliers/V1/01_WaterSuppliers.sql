BEGIN TRAN

BEGIN TRY

    -- Insert water suppliers that don't have parents
	INSERT INTO Envirotrax2Dev.dbo.WaterSuppliers
		([Name], Domain)
	SELECT [Name], Subdomain
	FROM WaterSuppliers
	WHERE MasterWaterSupplierID < 4 -- Supplier IDs up to 3 don't actually exist in the system, but there are records with those parent IDs
		AND MasterWaterSupplierID2 = 0

    --COMMIT TRAN

END TRY
BEGIN CATCH
    ROLLBACK TRAN
    THROW
END CATCH
