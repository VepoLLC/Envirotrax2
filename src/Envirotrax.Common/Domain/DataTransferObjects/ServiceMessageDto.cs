
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Envirotrax.Common.Domain.DataTransferObjects;

public class ServiceMessageDto<T>
{
    public int WaterSupplierId { get; private set; }

    public int? LoggedInUserId { get; private set; }

    public T? Data { get; set; }

    [JsonConstructor]
    public ServiceMessageDto(int waterSupplierId, int? loggedInUserId)
    {
        if (waterSupplierId <= 0)
        {
            throw new ArgumentException("TenantId must be greater than 0", nameof(waterSupplierId));
        }

        if (loggedInUserId.HasValue && loggedInUserId <= 0)
        {
            throw new ArgumentException("LoggedInUserId must be greater than 0", nameof(loggedInUserId));
        }

        WaterSupplierId = waterSupplierId;
        LoggedInUserId = loggedInUserId;
    }
}