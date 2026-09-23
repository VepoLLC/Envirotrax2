
namespace Envirotrax.App.Server.Data.Models.Professionals;

/// <summary>
/// Flattened, read-only projection of a professional that is publicly listed in the
/// "Registered Professionals" directory of a single water supplier.
///
/// This is not an EF entity. It exists so the public search can be filtered, sorted and paginated
/// with the standard <c>Query</c> pipeline against the same property names the DTO exposes,
/// the way <see cref="AvailableWaterSupplier"/> does for the supplier registration page.
/// </summary>
public class RegisteredProfessional
{
    public int Id { get; set; }

    public string CompanyName { get; set; } = null!;

    public string? ContactName { get; set; }

    public DateTime RegisteredDate { get; set; }

    public string? Address { get; set; }

    public string? City { get; set; }

    public string? State { get; set; }

    public string? ZipCode { get; set; }

    public string? WorkNumber { get; set; }

    public string? CellNumber { get; set; }

    public string? FaxNumber { get; set; }

    public string? EmailAddress { get; set; }

    public string? WebsiteUrl { get; set; }

    public bool HasFireLicense { get; set; }

    /// <summary>
    /// True for accounts registered within the last six months. V1 listed those first so newly
    /// registered companies are not buried at the bottom of a long public directory.
    /// </summary>
    public bool IsNewAccount { get; set; }
}

/// <summary>
/// Water supplier entry of the public "Registered Professionals" search filter.
/// Deliberately minimal — this endpoint is anonymous, so it exposes nothing but the picker label.
/// </summary>
public class RegisteredProfessionalSupplier
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;
}
