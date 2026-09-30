namespace Envirotrax.App.Server.Data.Models.Professionals;

public interface IPayableModel
{
    int Id { get; set; }
    decimal Amount { get; set; }
    decimal AmountShare { get; set; }
}
