using System.Globalization;
using Envirotrax.App.Server.Data.Models.Fog;
using Envirotrax.App.Server.Domain.DataTransferObjects.Payments;
using Envirotrax.App.Server.Domain.DataTransferObjects.Sites;

namespace Envirotrax.App.Server.Domain.Services.Implementations.Payments;

public static class ReceiptFormatter
{
    private static readonly CultureInfo UsCulture = CultureInfo.GetCultureInfo("en-US");
    private static readonly string[] CapacityLabels = ["Gallons", "Cubic Yards"];

    public static string Money(decimal amount)
    {
        return (amount < 0 ? "-" : string.Empty) + Math.Abs(amount).ToString("C2", UsCulture);
    }

    public static string DateTimeText(DateTime? value)
    {
        return value?.ToString("M/d/yyyy h:mm tt", UsCulture) ?? string.Empty;
    }

    public static string CapacityLabel(int capacityType)
    {
        return capacityType >= 0 && capacityType < CapacityLabels.Length ? CapacityLabels[capacityType] : string.Empty;
    }

    public static string CapacityLabel(FogVehicleCapacityType capacityType)
    {
        return CapacityLabel((int)capacityType);
    }

    public static string Join(string separator, params string?[] parts)
    {
        return string.Join(separator, parts.Where(part => !string.IsNullOrWhiteSpace(part)));
    }

    public static ReceiptLineDto Text(string? value)
    {
        return new ReceiptLineDto { Value = value };
    }

    public static ReceiptLineDto Labeled(string label, string? value)
    {
        return new ReceiptLineDto { Label = label, Value = value };
    }

    public static List<ReceiptLineDto> AmountCell(decimal amount)
    {
        return [Text(Money(amount))];
    }

    public static List<ReceiptLineDto> PropertyLines(IPropertyLocationDto property)
    {
        return TextLines(
            property.PropertyBusinessName,
            StreetLine(property.PropertyStreetNumber, property.PropertyStreetName, property.PropertyNumber),
            CityLine(property.PropertyCity, property.PropertyState?.Code, property.PropertyZip));
    }

    public static List<ReceiptLineDto> MailingLines(IMailingInfoDto mailing)
    {
        return TextLines(
            mailing.MailingCompanyName,
            mailing.MailingContactName,
            StreetLine(mailing.MailingStreetNumber, mailing.MailingStreetName, mailing.MailingNumber),
            CityLine(mailing.MailingCity, mailing.MailingState?.Code, mailing.MailingZip));
    }

    public static List<ReceiptLineDto> TextLines(params string?[] values)
    {
        return NonBlank(values)
            .Select(Text)
            .ToList();
    }

    public static List<string> NonBlank(params string?[] values)
    {
        return values
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .Select(value => value!)
            .ToList();
    }

    private static string StreetLine(string? streetNumber, string? streetName, string? number)
    {
        var street = Join(" ", streetNumber, streetName);

        return string.IsNullOrWhiteSpace(number) ? street : Join(" ", street, $"#{number}");
    }

    private static string CityLine(string? city, string? stateCode, string? zip)
    {
        return Join(", ", city, Join(" ", stateCode, zip));
    }
}
