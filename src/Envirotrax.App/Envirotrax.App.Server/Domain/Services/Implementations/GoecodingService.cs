
using System.Text.Json;
using System.Text.Json.Serialization;
using Envirotrax.App.Server.Domain.Configuration;
using Envirotrax.App.Server.Domain.DataTransferObjects;
using Envirotrax.App.Server.Domain.Services.Definitions;
using Microsoft.Extensions.Options;

namespace Envirotrax.App.Server.Domain.Services.Implementations;

public class GeocodingService : IGeocodingService
{
    private readonly HttpClient _http;

    public GeocodingService(HttpClient http, IOptions<GeocodingOptions> options)
    {
        _http = http;

        _http.BaseAddress = new(options.Value.BaseUrl);

        _http.DefaultRequestHeaders.Clear();
        _http.DefaultRequestHeaders.Add("X-Goog-Api-Key", options.Value.ApiKey);
    }

    public async Task<GeocodingResponseDto> GeocodeAsync(string address, CancellationToken cancellationToken)
    {
        var response = await _http.GetAsync($"v4/geocode/address?addressQuery={Uri.EscapeDataString(address)}", cancellationToken);

        if (response.IsSuccessStatusCode)
        {
            var json = await response.Content.ReadAsStringAsync(cancellationToken);
            var responseContent = JsonSerializer.Deserialize<Response>(json);

            if (responseContent != null)
            {
                if (responseContent.Results != null && responseContent.Results.Count > 0)
                {
                    var dto = responseContent
                        .Results
                        .Where(r => r.Location != null)
                        .Select(result => new GeocodingResponseDto
                        {
                            Longitude = result.Location!.Longitude,
                            Latitude = result.Location!.Latitude
                        }).FirstOrDefault();

                    if (dto != null)
                    {
                        return dto;
                    }
                }
            }

            throw new InvalidOperationException($"Geocoding location failed. JSON: {json}");
        }

        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        throw new InvalidOperationException($"Geocoding failed. Response: {content}");
    }

    public bool IsPointInArea(IList<CoordinateDto> points, CoordinateDto point)
    {
        int winding = 0;
        int count = points.Count;

        for (int i = 0; i < count; i++)
        {
            var a = points[i];
            var b = points[(i + 1) % count];

            if (a.Latitude <= point.Latitude)
            {
                if (b.Latitude > point.Latitude && Cross(a, b, point) > 0)
                {
                    winding++;
                }
            }
            else
            {
                if (b.Latitude <= point.Latitude && Cross(a, b, point) < 0)
                {
                    winding--;
                }
            }
        }

        return winding != 0;
    }

    // 2D cross product of vectors (a→b) and (a→point)
    private static double Cross(CoordinateDto a, CoordinateDto b, CoordinateDto p)
        => (b.Longitude - a.Longitude) * (p.Latitude - a.Latitude)
         - (b.Latitude - a.Latitude) * (p.Longitude - a.Longitude);

    class Response
    {
        [JsonPropertyName("results")]
        public ICollection<Result> Results { get; set; } = [];
    }

    class Result
    {
        [JsonPropertyName("location")]
        public Location? Location { get; set; }
    }

    class Location
    {
        [JsonPropertyName("latitude")]
        public double Latitude { get; set; }

        [JsonPropertyName("longitude")]
        public double Longitude { get; set; }
    }
}