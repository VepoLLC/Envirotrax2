BEGIN TRAN

BEGIN TRY

    -- Assumes color/notice set 1 = Impending, set 2 = PastDue, set 3 = NonCompliant
    INSERT INTO CsiSettings
        (WaterSupplierId, ModificationGracePeriodDays, NewlyCreatedBackflowTestExpirationDays, RequireInspectionImages,
         ImpendingNotice1, ImpendingNotice2, PastDueNotice1, PastDueNotice2, NonCompliant1, NonCompliant2,
         ImpendingLettersBackgroundColor, ImpendingLettersForegroundColor, ImpendingLettersBorderColor,
         PastDueLettersBackgroundColor, PastDueLettersForegroundColor, PastDueLettersBorderColor,
         NonCompliantLettersBackgroundColor, NonCompliantLettersForegroundColor, NonCompliantLettersBorderColor,
         NoticeBodyFont, NoticeBodyFontSize,
         ImpendingTitle, ImpendingMessage, PastDueTitle, PastDueMessage, NonCompliantTitle, NonCompliantMessage)
    SELECT
        newWaterSuppliers.Id, WaterSuppliers.CsiModificationDays, WaterSuppliers.CsiBackflowExpirationDays, WaterSuppliers.RequireCsiInspectionImages,
        WaterSuppliers.CsiNoticeImpending1, WaterSuppliers.CsiNoticeImpending2,
        WaterSuppliers.CsiNoticePastDue1, WaterSuppliers.CsiNoticePastDue2,
        WaterSuppliers.CsiNonCompliant, WaterSuppliers.CsiNonCompliant2,
        WaterSuppliers.CsiNoticeHeaderBackgroundColor1, WaterSuppliers.CsiNoticeHeaderForegroundColor1, WaterSuppliers.CsiNoticeHeaderBorderColor1,
        WaterSuppliers.CsiNoticeHeaderBackgroundColor2, WaterSuppliers.CsiNoticeHeaderForegroundColor2, WaterSuppliers.CsiNoticeHeaderBorderColor2,
        WaterSuppliers.CsiNoticeHeaderBackgroundColor3, WaterSuppliers.CsiNoticeHeaderForegroundColor3, WaterSuppliers.CsiNoticeHeaderBorderColor3,
        WaterSuppliers.CsiNoticeBodyFont, WaterSuppliers.CsiNoticeBodyFontSize,
        WaterSuppliers.CsiImpendingTitle, WaterSuppliers.CsiImpendingMessage,
        WaterSuppliers.CsiPastDueTitle, WaterSuppliers.CsiPastDueMessage,
        WaterSuppliers.CsiNonCompliantTitle, WaterSuppliers.CsiNonCompliantMessage
    FROM Vepo.dbo.WaterSuppliers
    INNER JOIN WaterSuppliers AS newWaterSuppliers
        ON newWaterSuppliers.LegacyRecordId = WaterSuppliers.ID
    WHERE NOT EXISTS (
        SELECT 1
        FROM CsiSettings AS alreadyInserted
        WHERE alreadyInserted.WaterSupplierId = newWaterSuppliers.Id
    )

    COMMIT TRAN

END TRY
BEGIN CATCH
    ROLLBACK TRAN;
    THROW;
END CATCH
