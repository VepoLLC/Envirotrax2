using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Data.Models.Users;
using Envirotrax.App.Server.Domain.DataTransferObjects.Users;

namespace Envirotrax.App.Server.Domain.Services.Definitions.Users;

public interface IUserService : IService<WaterSupplierUser, WaterSupplierUserDto>
{
    Task<WaterSupplierUserDto?> ResendInvitationAsync(int id, CancellationToken cancellationToken);

    Task<IPagedData<WaterSupplierUserDto>> GetAllForWaterSupplierAsync(int waterSupplierId, PageInfo pageInfo, Query query, CancellationToken cancellationToken);
}