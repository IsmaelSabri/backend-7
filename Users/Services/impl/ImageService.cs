using System.Text.Json;
using Images.Models;

namespace Users.Services.impl
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

        public async Task<Image?> GetProfileImageByUserIdAsync(Guid userId)
        {
            try
            {
                var query = $"filters=Id=={userId},ownerType@=*ProfileImage";
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
                if (root.ValueKind == System.Text.Json.JsonValueKind.Array && root.GetArrayLength() > 0)
                {
                    var imageJson = root[0].GetRawText();
                    return JsonSerializer.Deserialize<Image>(imageJson, options);
                }
                
                // Si es un objeto directo
                if (root.ValueKind == System.Text.Json.JsonValueKind.Object)
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
    }
}
