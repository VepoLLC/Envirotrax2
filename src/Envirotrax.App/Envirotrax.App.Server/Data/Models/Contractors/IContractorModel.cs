
namespace Envirotrax.App.Server.Data.Models.Contractors;

public interface IContractorModel
{
    int ContractorId { get; set; }
    Contractor? Contractor { get; set; }
}