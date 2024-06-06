using Users.Models;

namespace Users.Services
{
    /// <summary>
    /// An interface which helps to do intermediate calculation before projecting calls to the core component. 
    /// </summary>
    /// <typeparam name="T">Any class which decorates with ElasticsearchType and Description attribute.</typeparam>
    public interface IElasticService<T> //where T : class
    {
        Task<string> AddDocumentAsync(T value);
        Task<T> GetDocumentAsync(long id);
        Task<IEnumerable<T>> GetAllDocuments();
        Task<string> UpdateDocumentAsync(T value);
        Task<string> DeleteDocumentAsync(long id);
        public string GenerateRandomAlphanumericString();
        IQueryable<User> GetAllPagedDocuments();
        Task<User> GetDocumentByEmailAsync(string email);
        Task<User> GetDocumentByUsernameAsync(string username);
        Task<User> GetDocumentByUserIdAsync(string userId);
        void SendWelcomeEmail(User user);
        void SendResetEmail(User user);
    }
}