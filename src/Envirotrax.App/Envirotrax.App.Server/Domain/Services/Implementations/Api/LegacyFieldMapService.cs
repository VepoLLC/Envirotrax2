using Envirotrax.App.Server.Data.Models.Backflow;
using Envirotrax.App.Server.Data.Models.Professionals;
using Envirotrax.App.Server.Data.Models.Sites;
using Envirotrax.App.Server.Data.Models.States;
using Envirotrax.App.Server.Data.Models.Users;
using Envirotrax.App.Server.Domain.DataTransferObjects.Api;
using Envirotrax.App.Server.Domain.Services.Definitions.Api;

namespace Envirotrax.App.Server.Domain.Services.Implementations.Api;

/// <summary>
/// The V1-to-V2 mapping tables. Adding a supported field or criterion is one row here; there is no
/// per-query code. Property paths are built with nameof so a V2 rename breaks the build rather than
/// the response.
///
/// Only fields with an exact, renamed or safely derived V2 equivalent appear here. V1 fields that
/// depend on legacy-id translation, or that V2 has no equivalent for, are deliberately absent.
/// </summary>
public class LegacyFieldMapService : ILegacyFieldMapService
{
    private static readonly string StateCodePath = $"{nameof(Site.State)}.{nameof(State.Code)}";
    private static readonly string MailingStateCodePath = $"{nameof(Site.MailingState)}.{nameof(State.Code)}";
    private static readonly string UpdatedByEmailPath = $"{nameof(BackflowTest.UpdatedBy)}.{nameof(AppUser.Email)}";

    // V1 BpatID holds the tester's login string (Vepo.dbo.SaveBpats.UserID). The migration copied
    // that value into AspNetUsers.UserName/Email, so the legacy value is recovered through the
    // BPAT's user rather than from V2's numeric BpatId.
    private static readonly string BpatUserEmailPath =
        $"{nameof(BackflowTest.Bpat)}.{nameof(ProfessionalUser.User)}.{nameof(AppUser.Email)}";

    private static readonly IReadOnlyCollection<LegacyFieldDescriptor> SiteFields = BuildSiteFields();
    private static readonly IReadOnlyCollection<LegacyFieldDescriptor> BackflowTestFields = BuildBackflowTestFields();

    private static readonly IReadOnlyCollection<LegacyCriterionDescriptor> SiteCriteria = BuildSiteCriteria();
    private static readonly IReadOnlyCollection<LegacyCriterionDescriptor> BackflowTestCriteria = BuildBackflowTestCriteria();

    public IReadOnlyCollection<LegacyFieldDescriptor> GetFields(LegacyApiTable table)
    {
        if (table == LegacyApiTable.CsiBackflowSites)
        {
            return SiteFields;
        }

        return BackflowTestFields;
    }

    public IReadOnlyCollection<LegacyCriterionDescriptor> GetCriteria(LegacyApiTable table)
    {
        if (table == LegacyApiTable.CsiBackflowSites)
        {
            return SiteCriteria;
        }

        return BackflowTestCriteria;
    }

    public LegacyFieldDescriptor? FindField(LegacyApiTable table, string wireName)
    {
        return GetFields(table)
            .FirstOrDefault(field => string.Equals(field.WireName, wireName, StringComparison.Ordinal));
    }

    public LegacyFieldDescriptor? FindOrderByField(LegacyApiTable table, string wireName)
    {
        return GetFields(table)
            .FirstOrDefault(field => string.Equals(field.WireName, wireName, StringComparison.OrdinalIgnoreCase));
    }

    public LegacyCriterionDescriptor? FindCriterion(LegacyApiTable table, string parameterName)
    {
        return GetCriteria(table)
            .FirstOrDefault(criterion => string.Equals(criterion.ParameterName, parameterName, StringComparison.OrdinalIgnoreCase));
    }

    private static List<LegacyFieldDescriptor> BuildSiteFields()
    {
        return new List<LegacyFieldDescriptor>
        {
            Primary("AccountNumber", nameof(Site.AccountNumber), LegacyValueKind.String),
            Primary("CreationDate", nameof(Site.CreatedTime), LegacyValueKind.DateTime),
            Primary("LastModifiedDate", nameof(Site.UpdatedTime), LegacyValueKind.DateTime),
            Primary("PropertyType", nameof(Site.PropertyType), LegacyValueKind.Int),
            Primary("PropertyBusinessName", nameof(Site.BusinessName), LegacyValueKind.String),
            Primary("PropertyStreetNumber", nameof(Site.StreetNumber), LegacyValueKind.String),
            Primary("PropertyStreetName", nameof(Site.StreetName), LegacyValueKind.String),
            Primary("PropertyNumber", nameof(Site.PropertyNumber), LegacyValueKind.String),
            Primary("PropertyCity", nameof(Site.City), LegacyValueKind.String),
            Primary("PropertyState", StateCodePath, LegacyValueKind.String),
            Primary("PropertyZIP", nameof(Site.ZipCode), LegacyValueKind.String),
            Primary("GisLatitude", nameof(Site.GisLatitude), LegacyValueKind.Number),
            Primary("GisLongitude", nameof(Site.GisLongitude), LegacyValueKind.Number),
            Primary("GisStatus", nameof(Site.GisStatus), LegacyValueKind.Int),
            Primary("GisDate", nameof(Site.GisDate), LegacyValueKind.Date)
        };
    }

    private static List<LegacyFieldDescriptor> BuildBackflowTestFields()
    {
        var fields = new List<LegacyFieldDescriptor>
        {
            // Columns on the test row itself.
            Primary("CreationDate", nameof(BackflowTest.CreatedTime), LegacyValueKind.DateTime),
            Primary("LastModifiedDate", nameof(BackflowTest.UpdatedTime), LegacyValueKind.DateTime),
            Primary("LastModifiedBy", UpdatedByEmailPath, LegacyValueKind.String),
            Primary("BpatID", BpatUserEmailPath, LegacyValueKind.String),
            Primary("JobNumber", nameof(BackflowTest.JobNumber), LegacyValueKind.String),
            Primary("OutOfService", nameof(BackflowTest.OutOfService), LegacyValueKind.Bool),
            Primary("OutOfServiceDate", nameof(BackflowTest.OutOfServiceDate), LegacyValueKind.DateTime),
            Primary("HazardType", nameof(BackflowTest.HazardType), LegacyValueKind.String),
            Primary("HazardTypeOtherDescription", nameof(BackflowTest.HazardTypeOtherDescription), LegacyValueKind.String),
            Primary("LocationDescription", nameof(BackflowTest.LocationDescription), LegacyValueKind.String),
            Primary("DeviceType", nameof(BackflowTest.DeviceType), LegacyValueKind.String),
            Primary("Manufacturer", nameof(BackflowTest.Manufacturer), LegacyValueKind.String),
            Primary("Model", nameof(BackflowTest.Model), LegacyValueKind.String),
            Primary("Size", nameof(BackflowTest.Size), LegacyValueKind.String),
            Primary("SerialNumber", nameof(BackflowTest.SerialNumber), LegacyValueKind.String),
            Primary("Manufacturer2", nameof(BackflowTest.Manufacturer2), LegacyValueKind.String),
            Primary("Model2", nameof(BackflowTest.Model2), LegacyValueKind.String),
            Primary("Size2", nameof(BackflowTest.Size2), LegacyValueKind.String),
            Primary("SerialNumber2", nameof(BackflowTest.SerialNumber2), LegacyValueKind.String),
            Primary("UnknownSerialNumber", nameof(BackflowTest.UnknownSerialNumber), LegacyValueKind.Bool),
            Primary("MeterNumber", nameof(BackflowTest.MeterNumber), LegacyValueKind.String),
            Primary("TestResult", nameof(BackflowTest.TestResult), LegacyValueKind.Int),
            Primary("TestDate", nameof(BackflowTest.TestDate), LegacyValueKind.DateTime),
            Primary("InitialTestDate", nameof(BackflowTest.InitialTestDate), LegacyValueKind.DateTime),
            Primary("FinalTestDate", nameof(BackflowTest.FinalTestDate), LegacyValueKind.DateTime),
            Primary("RepairTestDate", nameof(BackflowTest.RepairTestDate), LegacyValueKind.DateTime),
            Primary("RenewalRequired", nameof(BackflowTest.RenewalRequired), LegacyValueKind.Bool),
            Primary("ExpirationDate", nameof(BackflowTest.ExpirationDate), LegacyValueKind.DateTime),
            Primary("IsCurrent", nameof(BackflowTest.IsCurrent), LegacyValueKind.Bool),
            Primary("SiteScheduleMonth", nameof(BackflowTest.BackflowScheduleMonth), LegacyValueKind.Int)
        };

        // V1 reads these 22 names from the joined site row, never from the test row, even where the
        // test table has a column of the same name. See EraDatabase.IsSiteField.
        fields.AddRange(new[]
        {
            FromSite("PropertyType", nameof(Site.PropertyType), LegacyValueKind.Int),
            FromSite("PropertyBusinessName", nameof(Site.BusinessName), LegacyValueKind.String),
            FromSite("PropertyStreetNumber", nameof(Site.StreetNumber), LegacyValueKind.String),
            FromSite("PropertyStreetName", nameof(Site.StreetName), LegacyValueKind.String),
            FromSite("PropertyNumber", nameof(Site.PropertyNumber), LegacyValueKind.String),
            FromSite("PropertyCity", nameof(Site.City), LegacyValueKind.String),
            FromSite("PropertyState", StateCodePath, LegacyValueKind.String),
            FromSite("PropertyZIP", nameof(Site.ZipCode), LegacyValueKind.String),
            FromSite("MailingCompanyName", nameof(Site.MailingCompanyName), LegacyValueKind.String),
            FromSite("MailingContactName", nameof(Site.MailingContactName), LegacyValueKind.String),
            FromSite("MailingStreetNumber", nameof(Site.MailingStreetNumber), LegacyValueKind.String),
            FromSite("MailingStreetName", nameof(Site.MailingStreetName), LegacyValueKind.String),
            FromSite("MailingNumber", nameof(Site.MailingNumber), LegacyValueKind.String),
            FromSite("MailingCity", nameof(Site.MailingCity), LegacyValueKind.String),
            FromSite("MailingState", MailingStateCodePath, LegacyValueKind.String),
            FromSite("MailingZIP", nameof(Site.MailingZipCode), LegacyValueKind.String),
            FromSite("MailingPhoneNumber", nameof(Site.MailingPhoneNumber), LegacyValueKind.String),
            FromSite("MailingEmailAddress", nameof(Site.MailingEmailAddress), LegacyValueKind.String),
            FromSite("GisLatitude", nameof(Site.GisLatitude), LegacyValueKind.Number),
            FromSite("GisLongitude", nameof(Site.GisLongitude), LegacyValueKind.Number),
            FromSite("GisStatus", nameof(Site.GisStatus), LegacyValueKind.Int),
            FromSite("GisDate", nameof(Site.GisDate), LegacyValueKind.Date)
        });

        return fields;
    }

    private static List<LegacyCriterionDescriptor> BuildSiteCriteria()
    {
        return new List<LegacyCriterionDescriptor>
        {
            new("WhereCreationDateFrom", "CreationDate", LegacyFieldSource.Primary, nameof(Site.CreatedTime), LegacyCriterionOperator.GreaterThanOrEqual, LegacyValueKind.DateTime),
            new("WhereLastModifiedDateTo", "LastModifiedDate", LegacyFieldSource.Primary, nameof(Site.UpdatedTime), LegacyCriterionOperator.LessThan, LegacyValueKind.DateTime),
            new("WhereGisStatus", "GisStatus", LegacyFieldSource.Primary, nameof(Site.GisStatus), LegacyCriterionOperator.Equals, LegacyValueKind.Int),
            new("WhereGisDateFrom", "GisDate", LegacyFieldSource.Primary, nameof(Site.GisDate), LegacyCriterionOperator.GreaterThanOrEqual, LegacyValueKind.Date)
        };
    }

    private static List<LegacyCriterionDescriptor> BuildBackflowTestCriteria()
    {
        return new List<LegacyCriterionDescriptor>
        {
            new("WhereCreationDateFrom", "CreationDate", LegacyFieldSource.Primary, nameof(BackflowTest.CreatedTime), LegacyCriterionOperator.GreaterThanOrEqual, LegacyValueKind.DateTime),
            new("WhereLastModifiedDateTo", "LastModifiedDate", LegacyFieldSource.Primary, nameof(BackflowTest.UpdatedTime), LegacyCriterionOperator.LessThan, LegacyValueKind.DateTime),

            // V1 validates IsCurrent as an integer in the inclusive range 0-1.
            new("WhereIsCurrent", "IsCurrent", LegacyFieldSource.Primary, nameof(BackflowTest.IsCurrent), LegacyCriterionOperator.Equals, LegacyValueKind.Int, 0, 1),

            // GisStatus and GisDate are site fields, so on a test request V1 filters the joined site.
            new("WhereGisStatus", "GisStatus", LegacyFieldSource.Site, nameof(Site.GisStatus), LegacyCriterionOperator.Equals, LegacyValueKind.Int),
            new("WhereGisDateFrom", "GisDate", LegacyFieldSource.Site, nameof(Site.GisDate), LegacyCriterionOperator.GreaterThanOrEqual, LegacyValueKind.Date)
        };
    }

    private static LegacyFieldDescriptor Primary(string wireName, string propertyPath, LegacyValueKind valueKind)
    {
        return new LegacyFieldDescriptor(wireName, LegacyFieldSource.Primary, propertyPath, valueKind);
    }

    private static LegacyFieldDescriptor FromSite(string wireName, string propertyPath, LegacyValueKind valueKind)
    {
        return new LegacyFieldDescriptor(wireName, LegacyFieldSource.Site, propertyPath, valueKind);
    }
}
