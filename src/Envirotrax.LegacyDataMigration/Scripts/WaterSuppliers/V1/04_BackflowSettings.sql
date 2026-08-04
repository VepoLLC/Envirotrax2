BEGIN TRAN

BEGIN TRY

    -- Assumes "Renewal" = Expiring, "Expired" = Expired, "Cutoff" = NonCompliant, and color set 1 = Expiring, 2 = Expired, 3 = NonCompliant
    INSERT INTO Envirotrax2Dev.dbo.BackflowSettings
        (WaterSupplierId, TestingMethod, GracePeriodDays, AdjustBackflowCreepingDates,
         NewInstallationsRequireApproval, ReplacementsRequireApproval, DetectorAssembliesRequireMeterReading,
         OutOfServiceRequiresApproval, OutOfServiceType, RequireBackflowTestImages,
         ExpiringNotice1, ExpiringNotice2, ExpiredNotice1, ExpiredNotice2, BackflowNonCompliant1, BackflowNonCompliant2,
         ShowWaterMeterNumber, ShowRainSensor, ShowOSSF, ShowPermitNumber,
         ExpiringLettersBackgroundColor, ExpiringLettersForegroundColor, ExpiringLettersBorderColor,
         ExpiredLettersBackgroundColor, ExpiredLettersForegroundColor, ExpiredLettersBorderColor,
         NonCompliantLettersBackgroundColor, NonCompliantLettersForegroundColor, NonCompliantLettersBorderColor,
         NoticeBodyFont, NoticeBodyFontSize,
         ExpiringTitle, ExpiringMessage, ExpiredTitle, ExpiredMessage, NonCompliantTitle, NonCompliantMessage)
    SELECT
        newWaterSuppliers.Id, WaterSuppliers.BackflowTestingMethod, WaterSuppliers.BackflowGracePeriodDays, WaterSuppliers.BackflowAdjustCreepingDates,
        WaterSuppliers.NewBackflowTestsRequireApproval, WaterSuppliers.ReplacementBackflowTestsRequireApproval, WaterSuppliers.RequireDetectorMeterReading,
        WaterSuppliers.OutOfServiceRequireApproval, WaterSuppliers.OutOfServiceManagementType, WaterSuppliers.RequireBackflowTestImages,
        WaterSuppliers.SaveBackflowNoticeRenewal1, WaterSuppliers.SaveBackflowNoticeRenewal2,
        WaterSuppliers.SaveBackflowNoticeExpired1, WaterSuppliers.SaveBackflowNoticeExpired2,
        WaterSuppliers.SaveBackflowCutoffDays, WaterSuppliers.SaveBackflowCutoffDays2,
        WaterSuppliers.SaveBackflowTestingShowWaterMeterNumber, WaterSuppliers.SaveBackflowTestingShowRainSensor,
        WaterSuppliers.SaveBackflowTestingShowOssf, WaterSuppliers.BackflowTestingShowPermitNumber,
        WaterSuppliers.BackflowNoticeHeaderBackgroundColor1, WaterSuppliers.BackflowNoticeHeaderForegroundColor1, WaterSuppliers.BackflowNoticeHeaderBorderColor1,
        WaterSuppliers.BackflowNoticeHeaderBackgroundColor2, WaterSuppliers.BackflowNoticeHeaderForegroundColor2, WaterSuppliers.BackflowNoticeHeaderBorderColor2,
        WaterSuppliers.BackflowNoticeHeaderBackgroundColor3, WaterSuppliers.BackflowNoticeHeaderForegroundColor3, WaterSuppliers.BackflowNoticeHeaderBorderColor3,
        WaterSuppliers.BackflowNoticeBodyFont, WaterSuppliers.BackflowNoticeBodyFontSize,
        WaterSuppliers.SaveBackflowRenewalTitle, WaterSuppliers.SaveBackflowRenewalMessage,
        WaterSuppliers.SaveBackflowExpirationTitle, WaterSuppliers.SaveBackflowExpirationMessage,
        WaterSuppliers.SaveBackflowCutoffTitle, WaterSuppliers.SaveBackflowCutoffMessage
    FROM WaterSuppliers
    INNER JOIN Envirotrax2Dev.dbo.WaterSuppliers AS newWaterSuppliers
        ON newWaterSuppliers.LegacyRecordId = WaterSuppliers.ID
    WHERE NOT EXISTS (
        SELECT 1
        FROM Envirotrax2Dev.dbo.BackflowSettings AS alreadyInserted
        WHERE alreadyInserted.WaterSupplierId = newWaterSuppliers.Id
    )

    COMMIT TRAN

END TRY
BEGIN CATCH
    ROLLBACK TRAN;
    THROW;
END CATCH
