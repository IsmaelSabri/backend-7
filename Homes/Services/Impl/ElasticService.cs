using Nest;
namespace Homes.Services.Impl;
public class ElasticService<T> : IElasticService<T> where T : class
{
    private readonly ElasticClient _elasticClient;
    public ElasticService(ElasticClient elasticClient)
    {
        _elasticClient = elasticClient;
    }
    public async Task<string> AddDocumentAsync(T value)
    {
        var response = await _elasticClient.IndexDocumentAsync(value);
        return response.IsValid ? "Document created successfully" : "Failed to create document";
    }

    public async Task<string> DeleteDocumentAsync(int id)
    {
        var response = await _elasticClient.DeleteAsync(new DocumentPath<T>(id));
        return response.IsValid ? "Document deleted successfully" : "Failed to delete document";
    }

    public async Task<IEnumerable<T>> GetAllDocuments()
    {
        var searchResponse = await _elasticClient.SearchAsync<T>(s => s
        .MatchAll()
        .Size(10000));
        return searchResponse.Documents;
    }

    public async Task<T> GetDocumentAsync(int id)
    {
        var response = await _elasticClient.GetAsync(new DocumentPath<T>(id));
        return response.Source;
    }

    public async Task<string> UpdateDocumentAsync(T value)
    {
        var response = await _elasticClient.UpdateAsync(new DocumentPath<T>(value), u => u
        .Doc(value)
        .RetryOnConflict(3));
        return response.IsValid ? "Document updated successfully" : "Failed to update document";

    }

    public string GenerateRandomAlphanumericString()
    {
        const string chars = "1234567890";
        var random = new Random();
        return new string(Enumerable.Repeat(chars, 18)
                                                .Select(s => s[random.Next(s.Length)]).ToArray());
    }
}