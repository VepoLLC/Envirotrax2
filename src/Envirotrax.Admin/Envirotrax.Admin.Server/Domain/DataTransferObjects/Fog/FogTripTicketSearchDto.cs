namespace Envirotrax.Admin.Server.Domain.DataTransferObjects.Fog;

public class FogTripTicketSearchDto
{
    public string? UserEmail { get; set; }

    public FogPaymentStatus? PaymentStatus { get; set; }

    public IDictionary<string, string> ToParameters()
    {
        var parameters = new Dictionary<string, string>
        {
            [nameof(UserEmail)] = UserEmail ?? string.Empty
        };

        if (PaymentStatus.HasValue)
        {
            parameters[nameof(PaymentStatus)] = ((int)PaymentStatus.Value).ToString();
        }

        return parameters;
    }
}
