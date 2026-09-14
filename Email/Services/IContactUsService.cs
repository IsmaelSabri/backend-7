using Email.Models;

namespace Email.Services
{
    public interface IContactUsService
    {
        Task<IEnumerable<ContactMessage>> GetAllAsync();
        Task<ContactMessage?> GetByIdAsync(Guid id);
        Task<IEnumerable<ContactMessage>> GetByStatusAsync(string status);
        Task<IEnumerable<ContactMessage>> GetByCategoryAsync(string category);
        Task<object> GetDashboardAsync();
        Task<ContactMessage> CreateAsync(ContactMessage message);
        Task<ContactMessage?> UpdateAsync(Guid id, ContactMessage message);
        Task<bool> DeleteAsync(Guid id);
    }
}
