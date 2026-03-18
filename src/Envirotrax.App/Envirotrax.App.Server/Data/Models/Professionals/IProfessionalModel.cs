
namespace Envirotrax.App.Server.Data.Models.Professionals;

public interface IProfessionalModel
{
    int ProfessionalId { get; set; }
    Professional? Professional { get; set; }
}