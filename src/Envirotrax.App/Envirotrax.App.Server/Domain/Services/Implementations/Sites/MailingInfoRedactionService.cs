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
/// or sent to a professional. Inside a TransactionScope, call it before Complete(), because it reads
/// GeneralSettings. SiteService.GetAsync must stay unredacted, because test and inspection submissions
/// snapshot the site's real mailing information from it.
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

    private async Task<HashSet<int>> GetRedactingWaterSupplierIdsAsync<TDto>(List<TDto> dtos, CancellationToken cancellationToken)
        where TDto : IRedactableMailingInfoDto
    {
        var waterSupplierIds = dtos
            .Select(dto => dto.WaterSupplier?.Id)
            .OfType<int>()
            .Distinct();

        var redactingSupplierIds = new HashSet<int>();

        foreach (var waterSupplierId in waterSupplierIds)
        {
            var settings = await _generalSettingsService.GetAsync(waterSupplierId, cancellationToken);

            if (settings?.RedactMailingInfo == true)
            {
                redactingSupplierIds.Add(waterSupplierId);
            }
        }

        return redactingSupplierIds;
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
