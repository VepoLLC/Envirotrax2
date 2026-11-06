using System.Globalization;
using Envirotrax.App.Server.Domain.DataTransferObjects.Professionals.Licenses;
using Envirotrax.App.Server.Domain.Services.Definitions.Professionals.Licenses;
using HtmlAgilityPack;

namespace Envirotrax.App.Server.Domain.Services.Implementations.Professionals.Licenses;

public class TceqLicenseService : ITceqLicenseService
{
    private const string SearchUrl = "https://www2.tceq.texas.gov/lic_dpa/index.cfm";
    private const string BpatLicenseType = "BACKFLOW PREVENTION ASSEMBLY TESTER";

    private const string LicenseeNameXPath = "//body[1]/div[1]/div[2]/div[1]/p[2]";
    private const string LicenseTableXPath = "//body[1]/div[1]/div[2]/table[1]";

    private const int LicenseeNameLineIndex = 4;
    private const int TypeCellIndex = 3;
    private const int NumberCellIndex = 5;
    private const int IssuedDateCellIndex = 7;
    private const int ExpirationDateCellIndex = 9;

    private static readonly CultureInfo TceqCulture = CultureInfo.GetCultureInfo("en-US");

    private readonly HttpClient _http;

    public TceqLicenseService(HttpClient http)
    {
        _http = http;
    }

    public async Task<TceqLicenseDto?> FindBpatLicenseAsync(string licenseNumber, CancellationToken cancellationToken)
    {
        var formFields = new Dictionary<string, string>
        {
            ["fuseaction"] = "licall.valSearch",
            ["searchmode"] = "indivlic",
            ["first_name"] = string.Empty,
            ["last_name"] = string.Empty,
            ["lic_num_txt"] = licenseNumber
        };

        using var content = new FormUrlEncodedContent(formFields);
        using var response = await _http.PostAsync(SearchUrl, content, cancellationToken);

        response.EnsureSuccessStatusCode();

        var html = await response.Content.ReadAsStringAsync(cancellationToken);

        var document = new HtmlDocument();
        document.LoadHtml(html);

        var licenseeName = GetLicenseeName(document);

        if (licenseeName == null)
        {
            return null;
        }

        return FindLicense(document, licenseeName, licenseNumber);
    }

    private static string? GetLicenseeName(HtmlDocument document)
    {
        var nameNode = document.DocumentNode.SelectSingleNode(LicenseeNameXPath);

        if (nameNode == null)
        {
            return null;
        }

        var lines = nameNode.InnerText.Split(new[] { '\r', '\n' });

        if (lines.Length <= LicenseeNameLineIndex)
        {
            return null;
        }

        return lines[LicenseeNameLineIndex].Trim();
    }

    private static TceqLicenseDto? FindLicense(HtmlDocument document, string licenseeName, string licenseNumber)
    {
        var table = document.DocumentNode.SelectSingleNode(LicenseTableXPath);

        if (table == null)
        {
            return null;
        }

        var rowNumber = 0;

        foreach (var row in table.ChildNodes)
        {
            if (row.Name != "tr")
            {
                continue;
            }

            rowNumber++;

            if (rowNumber == 1)
            {
                continue;
            }

            var license = ReadLicense(row, licenseeName, licenseNumber);

            if (license != null)
            {
                return license;
            }
        }

        return null;
    }

    private static TceqLicenseDto? ReadLicense(HtmlNode row, string licenseeName, string licenseNumber)
    {
        var cells = row.ChildNodes;

        if (cells.Count <= ExpirationDateCellIndex)
        {
            return null;
        }

        var type = cells[TypeCellIndex].InnerText.Trim();
        var number = cells[NumberCellIndex].InnerText.Trim();

        if (type != BpatLicenseType || !string.Equals(number, licenseNumber, StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        var issuedDateText = cells[IssuedDateCellIndex].InnerText.Trim();
        var expirationDateText = cells[ExpirationDateCellIndex].InnerText.Trim();

        if (!DateTime.TryParse(issuedDateText, TceqCulture, DateTimeStyles.None, out _))
        {
            return null;
        }

        if (!DateTime.TryParse(expirationDateText, TceqCulture, DateTimeStyles.None, out var expirationDate))
        {
            return null;
        }

        return new TceqLicenseDto
        {
            Name = licenseeName,
            LicenseNumber = number,
            ExpirationDate = expirationDate
        };
    }
}
