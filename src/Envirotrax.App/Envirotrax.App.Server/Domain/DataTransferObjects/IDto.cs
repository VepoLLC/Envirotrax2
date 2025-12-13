
namespace Envirotrax.App.Server.Domain.DataTransferObjects;

public interface IDto : IDto<int>
{
}

public interface IDto<TId>
{
    TId Id { get; set; }
}