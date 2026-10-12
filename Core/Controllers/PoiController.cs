using System.Globalization;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace Core.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PoiController(IHttpClientFactory httpClientFactory) : ControllerBase
{
    [HttpGet("nearby")]
    public async Task<IActionResult> GetNearby(
        [FromQuery] double lat,
        [FromQuery] double lng,
        [FromQuery] int radius = 500)
    {
        if (lat is < -90 or > 90)
        {
            return BadRequest("Invalid latitude");
        }

        if (lng is < -180 or > 180)
        {
            return BadRequest("Invalid longitude");
        }

        if (radius <= 0 || radius > 5000)
        {
            return BadRequest("radius must be between 1 and 5000 meters");
        }

        var latString = lat.ToString(CultureInfo.InvariantCulture);
        var lngString = lng.ToString(CultureInfo.InvariantCulture);

        var query = $"""
        [out:json][timeout:25];

        (
          nwr(around:{radius},{latString},{lngString})["highway"="bus_stop"];
          nwr(around:{radius},{latString},{lngString})["shop"="supermarket"];
          nwr(around:{radius},{latString},{lngString})["amenity"~"^(pharmacy|school|charging_station)$"];
        );

        out center;
        """;

        var client = httpClientFactory.CreateClient("Overpass");

        using var content = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("data", query)
        });

        using var response = await client.PostAsync(
            "api/interpreter",
            content);

        var json = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            return StatusCode(
                (int)response.StatusCode,
                new
                {
                    status = (int)response.StatusCode,
                    response = json
                });
        }

        using var document = JsonDocument.Parse(json);

        if (!document.RootElement.TryGetProperty("elements", out var elements))
        {
            return Ok(Array.Empty<PoiResponseDto>());
        }

        var poiList = new List<PoiResponseDto>();

        foreach (var element in elements.EnumerateArray())
        {
            if (!element.TryGetProperty("tags", out var tags))
            {
                continue;
            }

            var category = GetCategory(tags);

            if (category is null)
            {
                continue;
            }

            var id = element.TryGetProperty("id", out var idProp)
                ? idProp.ToString()
                : Guid.NewGuid().ToString();

            var elementLat = TryGetDouble(element, "lat");
            var elementLng = TryGetDouble(element, "lon");

            if (elementLat is null || elementLng is null)
            {
                if (element.TryGetProperty("center", out var center))
                {
                    elementLat = TryGetDouble(center, "lat");
                    elementLng = TryGetDouble(center, "lon");
                }
            }

            if (elementLat is null || elementLng is null)
            {
                continue;
            }

            var name = tags.TryGetProperty("name", out var nameProp)
                ? nameProp.GetString()
                : null;

            var type = element.TryGetProperty("type", out var typeProp)
                ? typeProp.ToString()
                : "node";

            poiList.Add(
                new PoiResponseDto(
                    id,
                    category,
                    name,
                    elementLat.Value,
                    elementLng.Value,
                    type));
        }

        return Ok(poiList);
    }

    private static string? GetCategory(JsonElement tags)
    {
        if (tags.TryGetProperty("highway", out var highway) &&
            highway.GetString() == "bus_stop")
        {
            return "transport";
        }

        if (tags.TryGetProperty("shop", out var shop) &&
            shop.GetString() == "supermarket")
        {
            return "supermarket";
        }

        if (tags.TryGetProperty("amenity", out var amenity))
        {
            return amenity.GetString() switch
            {
                "pharmacy" => "pharmacy",
                "school" => "school",
                "charging_station" => "charging",
                _ => null
            };
        }

        return null;
    }

    private static double? TryGetDouble(
        JsonElement element,
        string propertyName)
    {
        if (!element.TryGetProperty(propertyName, out var value))
        {
            return null;
        }

        return value.ValueKind == JsonValueKind.Number &&
               value.TryGetDouble(out var numericValue)
            ? numericValue
            : null;
    }
}

public record PoiResponseDto(
    string Id,
    string Category,
    string? Name,
    double Lat,
    double Lng,
    string Type);