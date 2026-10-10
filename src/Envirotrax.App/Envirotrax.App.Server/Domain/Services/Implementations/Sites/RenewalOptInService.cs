using System.Net.Mail;
using Envirotrax.App.Server.Data.Models.Sites;
using Envirotrax.App.Server.Data.Repositories.Definitions.Sites;
using Envirotrax.App.Server.Domain.DataTransferObjects.Sites;
using Envirotrax.App.Server.Domain.Services.Definitions.Sites;
using Envirotrax.App.Server.Templates.Emails.Sites;
using Envirotrax.Common.Data.Services.Definitions;
using Envirotrax.Common.Domain.DataTransferObjects;
using Envirotrax.Common.Domain.Services.Defintions;

namespace Envirotrax.App.Server.Domain.Services.Implementations.Sites;

public class RenewalOptInService : IRenewalOptInService
{
    private const int VerificationLinkValidDays = 7;
    private const int ZipCodePrefixLength = 3;
    private const int EmailAddressMaxLength = 320;
    private const int MailingEmailAddressMaxLength = 500;
    private const string MailingEmailAddressSeparator = "; ";

    private readonly IRenewalOptInRepository _renewalOptInRepository;
    private readonly IKeyHashingService _keyHashingService;
    private readonly IEmailService _emailService;
    private readonly ITenantProvidersService _tenantProvider;

    public RenewalOptInService(
        IRenewalOptInRepository renewalOptInRepository,
        IKeyHashingService keyHashingService,
        IEmailService emailService,
        ITenantProvidersService tenantProvider)
    {
        _renewalOptInRepository = renewalOptInRepository;
        _keyHashingService = keyHashingService;
        _emailService = emailService;
        _tenantProvider = tenantProvider;
    }

    public async Task<RenewalOptInSiteDto> GetSiteAsync(int siteId, CancellationToken cancellationToken)
    {
        var site = await _renewalOptInRepository.GetSiteAsync(siteId, cancellationToken);

        if (site == null)
        {
            return new RenewalOptInSiteDto { Found = false };
        }

        return new RenewalOptInSiteDto
        {
            Found = true,
            OptInType = site.RenewalOptInType
        };
    }

    public async Task<RenewalOptInResultDto> SaveAsync(int siteId, RenewalOptInRequestDto request, CancellationToken cancellationToken)
    {
        var site = await _renewalOptInRepository.GetTrackedSiteAsync(siteId, cancellationToken);

        if (site == null)
        {
            return Failed("Site not found.");
        }

        if (site.OptInCodeHash == null || site.OptInCodeExpirationDate == null || DateTime.UtcNow > site.OptInCodeExpirationDate)
        {
            return Failed("This opt-in code is invalid or expired.");
        }

        if (!BCrypt.Net.BCrypt.Verify(request.Passcode.Trim(), site.OptInCodeHash)
            || request.ZipCodePrefix.Trim() != GetZipCodePrefix(site.ZipCode))
        {
            return Failed("The Opt-In Code or Property Zip Code does not match our records.");
        }

        var emailAddresses = request.OptInType == RenewalOptInType.OptedIn
            ? SplitEmailAddresses(request.EmailAddresses).Select(email => email.ToLowerInvariant()).Distinct().ToList()
            : [];

        if (request.OptInType == RenewalOptInType.OptedIn && emailAddresses.Count == 0)
        {
            return Failed("Please enter at least one mailing email address.");
        }

        if (emailAddresses.Any(email => !IsValidEmailAddress(email)))
        {
            return Failed("One or more mailing email addresses are invalid.");
        }

        ApplyOptIn(site, request.OptInType);

        var pendingVerifications = emailAddresses
            .Select(email => CreatePendingVerification(site, email))
            .ToList();

        _renewalOptInRepository.AddVerifications(pendingVerifications.Select(pending => pending.Verification));

        await _renewalOptInRepository.SaveAsync(cancellationToken);

        foreach (var pending in pendingVerifications)
        {
            await SendEmailAsync(
                pending.Verification,
                pending.Token,
                "Envirotrax - Verify Your Email Address for Renewal Notifications",
                "Sites.RenewalEmailVerification");
        }

        if (pendingVerifications.Count == 0)
        {
            return Succeeded("Your renewal opt-in preference has been saved.");
        }

        return Succeeded("Your renewal opt-in preference has been saved. Verification emails have been sent to each address — "
            + "they will be added to your renewal notifications once each recipient confirms their address.");
    }

    public async Task<RenewalOptInResultDto> VerifyEmailAsync(RenewalOptInTokenDto request, CancellationToken cancellationToken)
    {
        var verification = await _renewalOptInRepository.GetTrackedVerificationAsync(request.Id, cancellationToken);

        if (verification == null || !_keyHashingService.VerifyHashedText(request.Token, verification.TokenHash))
        {
            return Failed("This verification link is invalid.");
        }

        if (verification.IsVerified)
        {
            return Succeeded("Your email address has already been verified. Thank you!");
        }

        if (verification.UnsubscribedDate != null)
        {
            return Failed("This email address has been unsubscribed from renewal notifications.");
        }

        if (DateTime.UtcNow > verification.CreatedDate.AddDays(VerificationLinkValidDays))
        {
            return Failed("This verification link has expired. Please contact us if you need a new one.");
        }

        var site = verification.Site!;
        var mailingEmailAddress = AddEmailAddress(site.MailingEmailAddress, verification.EmailAddress);

        if (mailingEmailAddress.Length > MailingEmailAddressMaxLength)
        {
            return Failed("No more email addresses can be added to this site's renewal notifications.");
        }

        var unsubscribeToken = _keyHashingService.GenerateApiKey();

        verification.IsVerified = true;
        verification.VerifiedDate = DateTime.UtcNow;
        verification.UnsubscribeTokenHash = _keyHashingService.HashText(unsubscribeToken);
        site.MailingEmailAddress = mailingEmailAddress;

        await _renewalOptInRepository.SaveAsync(cancellationToken);

        await SendEmailAsync(
            verification,
            unsubscribeToken,
            "Envirotrax - You Are Subscribed to Renewal Notifications",
            "Sites.RenewalEmailSubscribed");

        return Succeeded($"Your email address {verification.EmailAddress} has been verified and will receive renewal notifications. Thank you!");
    }

    public async Task<RenewalOptInResultDto> UnsubscribeAsync(RenewalOptInTokenDto request, CancellationToken cancellationToken)
    {
        var verification = await _renewalOptInRepository.GetTrackedVerificationAsync(request.Id, cancellationToken);

        if (verification == null
            || verification.UnsubscribeTokenHash == null
            || !_keyHashingService.VerifyHashedText(request.Token, verification.UnsubscribeTokenHash))
        {
            return Failed("This unsubscribe link is invalid.");
        }

        if (!verification.IsVerified)
        {
            return Succeeded("This email address is not currently subscribed to renewal notifications.");
        }

        var site = verification.Site!;

        site.MailingEmailAddress = RemoveEmailAddress(site.MailingEmailAddress, verification.EmailAddress);
        verification.IsVerified = false;
        verification.VerifiedDate = null;
        verification.UnsubscribedDate = DateTime.UtcNow;

        await _renewalOptInRepository.SaveAsync(cancellationToken);

        return Succeeded($"You have been unsubscribed. {verification.EmailAddress} will no longer receive renewal notifications.");
    }

    private void ApplyOptIn(Site site, RenewalOptInType optInType)
    {
        site.RenewalOptInType = optInType;
        site.RenewalOptInOutDate = DateTime.UtcNow;
        site.RenewalOptInOrigin = RenewalOptInOrigin.Website;
        site.RenewalOptInIpAddress = _tenantProvider.IpAddress;

        // The code printed on the letter is single-use.
        site.OptInCodeHash = null;
        site.OptInCodeExpirationDate = null;
    }

    private PendingVerification CreatePendingVerification(Site site, string emailAddress)
    {
        var token = _keyHashingService.GenerateApiKey();

        var verification = new RenewalEmailVerification
        {
            WaterSupplierId = site.WaterSupplierId,
            SiteId = site.Id,
            EmailAddress = emailAddress,
            TokenHash = _keyHashingService.HashText(token),
            CreatedDate = DateTime.UtcNow
        };

        return new PendingVerification(verification, token);
    }

    private async Task SendEmailAsync(RenewalEmailVerification verification, string token, string subject, string templateId)
    {
        await _emailService.SendAsync(new EmailDto<RenewalEmailVm>
        {
            Recipients = [verification.EmailAddress],
            Subject = subject,
            TemplateId = templateId,
            TemplateData = new RenewalEmailVm
            {
                VerificationId = verification.Id,
                Token = token,
                EmailAddress = verification.EmailAddress
            }
        });
    }

    private static string? GetZipCodePrefix(string? zipCode)
    {
        var trimmed = zipCode?.Trim();

        if (trimmed == null || trimmed.Length < ZipCodePrefixLength)
        {
            return trimmed;
        }

        return trimmed[..ZipCodePrefixLength];
    }

    private static bool IsValidEmailAddress(string emailAddress)
    {
        return emailAddress.Length <= EmailAddressMaxLength
            && MailAddress.TryCreate(emailAddress, out var address)
            && address.Address == emailAddress;
    }

    private static List<string> SplitEmailAddresses(string? emailAddresses)
    {
        return (emailAddresses ?? string.Empty)
            .Split([';', ','], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .ToList();
    }

    private static string AddEmailAddress(string? mailingEmailAddress, string emailAddress)
    {
        var emailAddresses = SplitEmailAddresses(mailingEmailAddress);

        if (!emailAddresses.Contains(emailAddress, StringComparer.OrdinalIgnoreCase))
        {
            emailAddresses.Add(emailAddress);
        }

        return string.Join(MailingEmailAddressSeparator, emailAddresses);
    }

    private static string? RemoveEmailAddress(string? mailingEmailAddress, string emailAddress)
    {
        var emailAddresses = SplitEmailAddresses(mailingEmailAddress)
            .Where(existing => !string.Equals(existing, emailAddress, StringComparison.OrdinalIgnoreCase))
            .ToList();

        if (emailAddresses.Count == 0)
        {
            return null;
        }

        return string.Join(MailingEmailAddressSeparator, emailAddresses);
    }

    private static RenewalOptInResultDto Succeeded(string message)
    {
        return new RenewalOptInResultDto { Succeeded = true, Message = message };
    }

    private static RenewalOptInResultDto Failed(string message)
    {
        return new RenewalOptInResultDto { Succeeded = false, Message = message };
    }

    private record PendingVerification(RenewalEmailVerification Verification, string Token);
}
