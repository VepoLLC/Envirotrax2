using Envirotrax.App.Server.Data.Models.Sites;
using Envirotrax.App.Server.Domain.DataTransferObjects.Lookup;

namespace Envirotrax.App.Server.Domain.DataTransferObjects.Sites;

/// <summary>
/// The site's property location as a professional submits it with a backflow test, CSI inspection, FOG inspection
/// or trip ticket, which snapshot it.
/// </summary>
public interface IPropertyLocationDto
{
    PropertyType PropertyType { get; set; }
    string? PropertyBusinessName { get; set; }
    string? PropertyStreetNumber { get; set; }
    string? PropertyStreetName { get; set; }
    string? PropertyNumber { get; set; }
    string? PropertyCity { get; set; }
    ReferencedStateDto? PropertyState { get; set; }
    string? PropertyZip { get; set; }
}
