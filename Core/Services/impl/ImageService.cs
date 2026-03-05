using System.Text.Json;
using Images.Models;
using Core.Enums;
using Core.Dto;

namespace Core.Services.impl
{
    public class ImageService : IImageService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ImageService> _logger;

        public ImageService(HttpClient httpClient, ILogger<ImageService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<Image?> GetProfileImageByUserIdAsync(string userId)
        {
            try
            {
                var query = $"filters=OwnerId=={userId},ownerType@=*ProfileImage";
                var url = $"http://localhost:3030/api/images/query?{query}";

                var response = await _httpClient.GetAsync(url);
                
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning($"Failed to fetch profile image for user {userId}. Status: {response.StatusCode}");
                    return null;
                }

                var content = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                
                // Asumiendo que la API retorna un array o un objeto envuelto
                var jsonDocument = JsonDocument.Parse(content);
                var root = jsonDocument.RootElement;

                // Si es un array, tomar el primer elemento
                if (root.ValueKind == JsonValueKind.Array && root.GetArrayLength() > 0)
                {
                    var imageJson = root[0].GetRawText();
                    return JsonSerializer.Deserialize<Image>(imageJson, options);
                }
                
                // Si es un objeto directo
                if (root.ValueKind == JsonValueKind.Object)
                {
                    return JsonSerializer.Deserialize<Image>(content, options);
                }

                return null;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError($"HTTP error fetching profile image for user {userId}: {ex.Message}");
                return null;
            }
            catch (JsonException ex)
            {
                _logger.LogError($"JSON parsing error for profile image of user {userId}: {ex.Message}");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Unexpected error fetching profile image for user {userId}: {ex.Message}");
                return null;
            }
        }

        public async Task<HomeImagesDto?> GetHomeImagesByViviendaIdAsync(string viviendaId)
        {
            try
            {
                var query = $"filters=OwnerId=={viviendaId}";
                var url = $"http://localhost:3030/api/images/query?{query}";

                var response = await _httpClient.GetAsync(url);
                
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning($"Failed to fetch home images for vivienda {viviendaId}. Status: {response.StatusCode}");
                    return null;
                }

                var content = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                
                // Parsear el array de imágenes
                var jsonDocument = JsonDocument.Parse(content);
                var root = jsonDocument.RootElement;

                if (root.ValueKind != JsonValueKind.Array || root.GetArrayLength() == 0)
                {
                    return null;
                }

                var allImages = JsonSerializer.Deserialize<Image[]>(content, options) ?? Array.Empty<Image>();
                
                // Agrupar imágenes por su OwnerType
                var images = allImages
                    .Where(img => img.OwnerType == nameof(OwnerType.HomeImages))
                    .ToArray();
                
                var schemes = allImages
                    .Where(img => img.OwnerType == nameof(OwnerType.HomeSchemes))
                    .ToArray();
                
                var energyCert = allImages
                    .FirstOrDefault(img => img.OwnerType == nameof(OwnerType.EnergyCertImage));

                // Retornar DTO con imágenes categorizadas
                return new HomeImagesDto
                {
                    Images = images.Length > 0 ? images : null,
                    Schemes = schemes.Length > 0 ? schemes : null,
                    EnergyCert = energyCert
                };
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError($"HTTP error fetching home images for vivienda {viviendaId}: {ex.Message}");
                return null;
            }
            catch (JsonException ex)
            {
                _logger.LogError($"JSON parsing error for home images of vivienda {viviendaId}: {ex.Message}");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Unexpected error fetching home images for vivienda {viviendaId}: {ex.Message}");
                return null;
            }
        }

        public async Task<string?> ComposeImageAsync(Guid imageId)
        {
            try
            {
                var url = $"http://localhost:3030/api/images/{imageId}";
                var response = await _httpClient.GetAsync(url);
                
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning($"Failed to fetch image {imageId}. Status: {response.StatusCode}");
                    return null;
                }

                var content = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                
                var image = JsonSerializer.Deserialize<Image>(content, options);
                return image?.ImageUrl;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError($"HTTP error fetching image {imageId}: {ex.Message}");
                return null;
            }
            catch (JsonException ex)
            {
                _logger.LogError($"JSON parsing error for image {imageId}: {ex.Message}");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Unexpected error fetching image {imageId}: {ex.Message}");
                return null;
            }
        }
    }
}
