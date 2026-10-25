namespace Envirotrax.Admin.Server.Domain.DataTransferObjects.Fog;

public class FogTripTicketSearchDto
{
    public string? UserEmail { get; set; }

    public IDictionary<string, string> ToParameters()
    {
        return new Dictionary<string, string>
        {
            [nameof(UserEmail)] = UserEmail ?? string.Empty
        };
    }
}
