
namespace Envirotrax.App.Server.Data.Models.Professionals;

public interface IProfessionalModel
{
    int ProfessionalId { get; set; }
    Professional? Professional { get; set; }
}

// A row visible to every professional (no ProfessionalDbContext query filter), but only the
// professional that created it can change or delete it. See ProfessionalDbContext.SetSecurityProperties.
public interface ISharedProfessionalModel : IProfessionalModel
{
}