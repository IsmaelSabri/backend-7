using System.Net;
using Elastic.Clients.Elasticsearch;
using Elasticsearch.Net;
using LinqToElasticSearch;
using Microsoft.OpenApi.Any;
using Nest;
using RestSharp;
using Users.Enums;
using Users.Models;

namespace Users.Services.Impl;
public class ElasticService<T>(ElasticsearchClient elasticClient, ElasticClient elClient) : IElasticService<T> where T : class
{
    private readonly ElasticsearchClient _elasticClient = elasticClient; // ES 8.X
    private readonly ElasticClient _elClient = elClient; // NEST

    public async Task<string> AddDocumentAsync(T value)
    {
        var response = await _elasticClient.IndexAsync(value, "users", Elastic.Clients.Elasticsearch.Id.From(value));
        return response.IsValidResponse ? "Document created successfully" : "Failed to create document";
    }

    public async Task<string> DeleteDocumentAsync(long id)
    {
        var response = await _elasticClient.DeleteAsync<T>(id, idx => idx.Index("users"));
        return response.IsValidResponse ? "Document deleted successfully" : "Failed to delete document";
    }

    public async Task<T> GetDocumentAsync(long id)
    {
        var response = await _elasticClient.GetAsync<T>(id, idx => idx.Index("users"));
        return response.Source;
    }

    public async Task<User> GetDocumentByEmailAsync(string email)
    {
        var response = await _elasticClient.SearchAsync<User>(s => s
             .Index("users")
             .From(0)
             .Size(1)
             .Query(q => q
            .Match(t => t.Field(p => p.Email).Query(email))));
        return response.Documents.FirstOrDefault();
    }

    public async Task<User> GetDocumentByUsernameAsync(string username)
    {
        var response = await _elasticClient.SearchAsync<User>(s => s
             .Index("users")
             .From(0)
             .Size(1)
             .Query(q => q
            .Match(t => t.Field(p => p.Username).Query(username))));
        return response.Documents.FirstOrDefault();
    }

    public async Task<User> GetDocumentByUserIdAsync(string userId)
    {
        var response = await _elasticClient.SearchAsync<User>(s => s
             .Index("users")
             .From(0)
             .Size(1)
             .Query(q => q
            .Match(t => t.Field(p => p.UserId).Query(userId))));
        return response.Documents.FirstOrDefault();
    }

    public async Task<string> UpdateDocumentAsync(T value)
    {
        var response = await _elasticClient.UpdateAsync<T, T>("users", Elastic.Clients.Elasticsearch.Id.From(value), u => u
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
        .Index("users")
        .Size(10000));
        return searchResponse.Documents;
    }

    public IQueryable<User> GetAllPagedDocuments()
    {
        var el = new ElasticQueryable<User>(_elClient, "users");
        return el.AsQueryable();
    }

    /*
    *
    * Info Emails
    *
    *
    */

    public async void SendWelcomeEmail(User user)
    {
        try
        {
            var options = new RestClientOptions("https://localhost:4040/api/email/setpassword");
            var client = new RestClient(options);
            var request = new RestRequest
            {
                Method = Method.Get
            };
            request.AddHeader("Content-Type", "application/json");
            request.AddJsonBody(new
            {
                Name = user.Firstname,
                ToEmail = user.Email,
                Subject = "Activar cuenta",
                Message = user.UserId,
            });
            var response = await client.ExecuteAsync(request);
            Console.WriteLine($"Content response: {response.Content}");
            Console.WriteLine($"Status: {response.StatusCode}");
            if (response.Headers != null)
            {
                foreach (var header in response.Headers)
                {
                    Console.WriteLine(header);
                }
            }
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async void SendResetEmail(User user)
    {
        try
        {
            var options = new RestClientOptions("https://localhost:4040/api/email/resendpassword");
            var client = new RestClient(options);
            var request = new RestRequest
            {
                Method = Method.Get
            };
            request.AddHeader("Content-Type", "application/json");
            request.AddJsonBody(new
            {
                Name = user.Firstname,
                ToEmail = user.Email,
                Subject = "Cambiar contraseña",
                Message = user.UserId,
            });
            var response = await client.ExecuteAsync(request);
            Console.WriteLine($"Content response: {response.Content}");
            Console.WriteLine($"Status: {response.StatusCode}");
            if (response.Headers != null)
            {
                foreach (var header in response.Headers)
                {
                    Console.WriteLine(header);
                }
            }
        }
        catch (Exception)
        {
            throw;
        }
    }
}