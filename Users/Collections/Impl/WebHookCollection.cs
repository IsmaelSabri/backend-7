using Users.Data;
using Users.Models;

namespace Users.Collections.impl
{
    public class WebHookCollection : IWebHookCollection
    {
        private readonly UserDb db;

        public WebHookCollection(UserDb db)
        {
            this.db = db;
        }

        public async Task<StripeWebhookEvent?> GetByIdAsync(string id)
        {
            return await db.StripeWebhookEvents.FindAsync(id);
        }

        public async Task<StripeWebhookEvent> AddAsync(StripeWebhookEvent stripeWebhookEvent)
        {
            db.StripeWebhookEvents.Add(stripeWebhookEvent);
            await SaveChangesAsync();
            return stripeWebhookEvent;
        }

        public async Task<StripeWebhookEvent> UpdateAsync(StripeWebhookEvent stripeWebhookEvent)
        {
            db.StripeWebhookEvents.Update(stripeWebhookEvent);
            await SaveChangesAsync();
            return stripeWebhookEvent;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var entity = await GetByIdAsync(id);
            if (entity == null) return false;
            db.StripeWebhookEvents.Remove(entity);
            await SaveChangesAsync();
            return true;
        }

        public async Task SaveChangesAsync()
        {
            await db.SaveChangesAsync();
        }
    }
}