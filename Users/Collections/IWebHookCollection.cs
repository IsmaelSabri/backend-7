using Users.Models;

namespace Users.Collections
{
    public interface IWebHookCollection
    {
        Task<StripeWebhookEvent?> GetByIdAsync(string id);
        //Task<List<StripeWebhookEvent>> GetAllAsync();
        Task<StripeWebhookEvent> AddAsync(StripeWebhookEvent stripeWebhookEvent);
        Task<StripeWebhookEvent> UpdateAsync(StripeWebhookEvent stripeWebhookEvent);
        Task<bool> DeleteAsync(string id);
        Task SaveChangesAsync();
    } 
}