
namespace Envirotrax.App.Server.Domain.DataTransferObjects.Professionals;

/// <summary>
/// A single row of the public "Registered Professionals" directory.
/// Only fields V1's registrations.aspx published to anonymous visitors are exposed here.
/// </summary>
public class RegisteredProfessionalDto : IDto
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
}

public class RegisteredProfessionalSupplierDto
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;
}
