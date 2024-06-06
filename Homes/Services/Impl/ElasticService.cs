using Elastic.Clients.Elasticsearch;
using Homes.Models;
using LinqToElasticSearch;
using Nest;
namespace Homes.Services.Impl;
public class ElasticService<T>(ElasticsearchClient elasticClient, ElasticClient elClient) : IElasticService<T> where T : class
{
    private readonly ElasticsearchClient _elasticClient = elasticClient; // ES 8.X
    private readonly ElasticClient _elClient = elClient; // NEST

    public async Task<string> AddDocumentAsync(T value)
    {
        var response = await _elasticClient.IndexAsync(value, "housesdb", Elastic.Clients.Elasticsearch.Id.From(value));
        return response.IsValidResponse ? "Document created successfully" : "Failed to create document";
    }

    public async Task<string> DeleteDocumentAsync(string id)
    {
        var response = await _elasticClient.DeleteAsync<T>(id, idx => idx.Index("housesdb"));
        return response.IsValidResponse ? "Document deleted successfully" : "Failed to delete document";
    }

    public async Task<T> GetDocumentAsync(string id)
    {
        var response = await _elasticClient.GetAsync<T>(id, idx => idx.Index("housesdb"));
        return response.Source;
    }

    public async Task<string> UpdateDocumentAsync(T value)
    {
        var response = await _elasticClient.UpdateAsync<T, T>("housesdb", Elastic.Clients.Elasticsearch.Id.From(value), u => u
    .Doc(value));
        return response.IsValidResponse ? "Document updated successfully" : "Failed to update document";
    }

    public string GenerateRandomAlphanumericString()
    {
        const string chars = "123456789";
        var random = new Random();
        return new string(Enumerable.Repeat(chars, 18)
                                                .Select(s => s[random.Next(s.Length)]).ToArray());
    }

    public async Task<IEnumerable<T>> GetAllDocuments()
    {
        var searchResponse = await _elasticClient.SearchAsync<T>(s => s
        .Index("housesdb")
        .Size(10000));
        return searchResponse.Documents;
    }

    /*
    *
    *
    */
    public IQueryable<T> GetAllPagedDocuments()
    {
        var el = new ElasticQueryable<T>(_elClient, "housesdb");
        return el.AsQueryable();
    }

    public IQueryable<Flat> SearchFlatDocumentsAsync()
    {
        var el = new ElasticQueryable<Flat>(_elClient, "housesdb");
        return el.AsQueryable();
    }

    public IQueryable<House> SearchHouseDocumentsAsync()
    {
        var el = new ElasticQueryable<House>(_elClient, "housesdb");
        return el.AsQueryable();
    }

    public IQueryable<Room> SearchRoomDocumentsAsync()
    {
        var el = new ElasticQueryable<Room>(_elClient, "housesdb");
        return el.AsQueryable();
    }

    public IQueryable<HolidayRent> SearchHolidayRentDocumentsAsync()
    {
        var el = new ElasticQueryable<HolidayRent>(_elClient, "housesdb");
        return el.AsQueryable();
    }

    public IQueryable<NewProject> SearchNewProjectDocumentsAsync()
    {
        var el = new ElasticQueryable<NewProject>(_elClient, "housesdb");
        return el.AsQueryable();
    }
}