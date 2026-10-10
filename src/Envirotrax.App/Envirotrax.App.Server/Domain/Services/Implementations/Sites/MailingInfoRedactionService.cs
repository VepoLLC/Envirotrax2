using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Domain.DataTransferObjects.Lookup;
using Envirotrax.App.Server.Domain.DataTransferObjects.Sites;
using Envirotrax.App.Server.Domain.Services.Definitions.Sites;
using Envirotrax.App.Server.Domain.Services.Definitions.WaterSuppliers;

namespace Envirotrax.App.Server.Domain.Services.Implementations.Sites;

/// <summary>
/// Hides the property owner's mailing/contact information from professionals when the record's water
/// supplier has GeneralSettings.RedactMailingInfo turned on. Unlike V1, only the record's own supplier is
/// checked, never its parents: a child account that wants the parent's choice uses "Copy from Parent".
/// Called by the …ForProfessionalAsync service methods and the professional checkout, only on what is returned
/// or sent to a professional. It reads GeneralSettings, so call it after a TransactionScope's using block has
/// ended (the scope commits there, not at Complete()), to keep the transaction short. SiteService.GetAsync must
/// stay unredacted, because KeepSiteLocationWhenRedactedAsync snapshots the site's real values from it.
/// </summary>
public class MailingInfoRedactionService : IMailingInfoRedactionService
{
    // V1's placeholders (CsiBackflowSite.FromReader), shown whatever the real value's length.
    private const string RedactedCompanyName = "***** ******";
    private const string RedactedContactName = "**** ****";
    private const string RedactedStreetNumber = "****";
    private const string RedactedStreetName = "*****";
    private const string RedactedNumber = "**";
    private const string RedactedCity = "*****";
    private const string RedactedState = "****";
    private const string RedactedZip = "*****";
    private const string RedactedPhoneNumber = "**********";
    private const string RedactedEmailAddress = "**********";

    private readonly IGeneralSettingsService _generalSettingsService;

    public MailingInfoRedactionService(IGeneralSettingsService generalSettingsService)
    {
        _generalSettingsService = generalSettingsService;
    }

    public async Task<TDto> RedactAsync<TDto>(TDto dto, CancellationToken cancellationToken)
        where TDto : IRedactableMailingInfoDto
    {
        await RedactAsync(new[] { dto }, cancellationToken);

        return dto;
    }

    public async Task<IPagedData<TDto>> RedactAsync<TDto>(IPagedData<TDto> page, CancellationToken cancellationToken)
        where TDto : IRedactableMailingInfoDto
    {
        var dtos = await RedactAsync(page.Data, cancellationToken);

        return dtos.ToPagedData(page.PageInfo);
    }

    public async Task<List<TDto>> RedactAsync<TDto>(IEnumerable<TDto> dtos, CancellationToken cancellationToken)
        where TDto : IRedactableMailingInfoDto
    {
        // Service results wrap a lazy MapToDto projection. Redacting it in place would be lost the next
        // time it is enumerated (by the serializer or a PDF template), so materialize it first.
        var materialized = dtos.ToList();
        var redactingSupplierIds = await GetRedactingWaterSupplierIdsAsync(materialized, cancellationToken);

        foreach (var dto in materialized)
        {
            if (dto.WaterSupplier?.Id is int waterSupplierId && redactingSupplierIds.Contains(waterSupplierId))
            {
                foreach (var mailingInfo in dto.MailingInfo)
                {
                    Redact(mailingInfo);
                }
            }
        }

        return materialized;
    }

    // Like V1, a professional can't request a site modification while the record's water supplier redacts the
    // owner's mailing information. A submission then carries the redacted placeholders the professional was shown,
    // so the site's own values are kept instead, which also leaves the record nothing to flag as changed.
    public async Task<bool> KeepSiteLocationWhenRedactedAsync(
        IPropertyLocationDto submission,
        SiteDto? site,
        int? waterSupplierId,
        CancellationToken cancellationToken)
    {
        if (site == null || waterSupplierId == null)
        {
            return false;
        }

        var redactingSupplierIds = await _generalSettingsService.GetRedactingWaterSupplierIdsAsync([waterSupplierId.Value], cancellationToken);

        if (!redactingSupplierIds.Contains(waterSupplierId.Value))
        {
            return false;
        }

        submission.PropertyType = site.PropertyType;
        submission.PropertyBusinessName = site.BusinessName;
        submission.PropertyStreetNumber = site.StreetNumber;
        submission.PropertyStreetName = site.StreetName;
        submission.PropertyNumber = site.PropertyNumber;
        submission.PropertyCity = site.City;
        submission.PropertyState = site.State;
        submission.PropertyZip = site.ZipCode;

        if (submission is IMailingInfoDto mailingInfo)
        {
            IMailingInfoDto siteMailingInfo = site;

            mailingInfo.MailingCompanyName = siteMailingInfo.MailingCompanyName;
            mailingInfo.MailingContactName = siteMailingInfo.MailingContactName;
            mailingInfo.MailingStreetNumber = siteMailingInfo.MailingStreetNumber;
            mailingInfo.MailingStreetName = siteMailingInfo.MailingStreetName;
            mailingInfo.MailingNumber = siteMailingInfo.MailingNumber;
            mailingInfo.MailingCity = siteMailingInfo.MailingCity;
            mailingInfo.MailingState = siteMailingInfo.MailingState;
            mailingInfo.MailingZip = siteMailingInfo.MailingZip;
            mailingInfo.MailingPhoneNumber = siteMailingInfo.MailingPhoneNumber;
            mailingInfo.MailingEmailAddress = siteMailingInfo.MailingEmailAddress;
        }

        return true;
    }

    // One query for all the records' suppliers, never a settings lookup per supplier.
    private async Task<HashSet<int>> GetRedactingWaterSupplierIdsAsync<TDto>(List<TDto> dtos, CancellationToken cancellationToken)
        where TDto : IRedactableMailingInfoDto
    {
        var waterSupplierIds = dtos
            .Select(dto => dto.WaterSupplier?.Id)
            .OfType<int>()
            .Distinct()
            .ToList();

        if (waterSupplierIds.Count == 0)
        {
            return [];
        }

        return await _generalSettingsService.GetRedactingWaterSupplierIdsAsync(waterSupplierIds, cancellationToken);
    }

    private static void Redact(IMailingInfoDto? mailingInfo)
    {
        if (mailingInfo == null)
        {
            return;
        }

        mailingInfo.MailingCompanyName = RedactedCompanyName;
        mailingInfo.MailingContactName = RedactedContactName;
        mailingInfo.MailingStreetNumber = RedactedStreetNumber;
        mailingInfo.MailingStreetName = RedactedStreetName;
        mailingInfo.MailingNumber = RedactedNumber;
        mailingInfo.MailingCity = RedactedCity;
        mailingInfo.MailingZip = RedactedZip;
        mailingInfo.MailingPhoneNumber = RedactedPhoneNumber;
        mailingInfo.MailingEmailAddress = RedactedEmailAddress;

        // No Id: the real state must not leak, and a null Id keeps the edit forms from posting it back.
        mailingInfo.MailingState = new ReferencedStateDto
        {
            Name = RedactedState,
            Code = RedactedState
        };
    }
}
