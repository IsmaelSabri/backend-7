using Email.Data;
using Email.Models;
using Email.Service;
using Microsoft.EntityFrameworkCore;

namespace Email.Services.impl
{
    public class ContactUsService : IContactUsService
    {
        private readonly EmailDbContext _dbContext;
        private readonly IEmailSender _emailSender;
        private readonly ILogger<ContactUsService> _logger;

        public ContactUsService(EmailDbContext dbContext, IEmailSender emailSender, ILogger<ContactUsService> logger)
        {
            _dbContext = dbContext;
            _emailSender = emailSender;
            _logger = logger;
        }

        public async Task<IEnumerable<ContactMessage>> GetAllAsync()
        {
            return await _dbContext.ContactMessages
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        public async Task<ContactMessage?> GetByIdAsync(Guid id)
        {
            return await _dbContext.ContactMessages.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<IEnumerable<ContactMessage>> GetByStatusAsync(string status)
        {
            var normalizedStatus = (status ?? string.Empty).Trim();
            return await _dbContext.ContactMessages
                .Where(x => x.Status.ToLower() == normalizedStatus.ToLower())
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<ContactMessage>> GetByCategoryAsync(string category)
        {
            var normalizedCategory = (category ?? string.Empty).Trim();
            return await _dbContext.ContactMessages
                .Where(x => x.Category.ToLower() == normalizedCategory.ToLower())
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        public async Task<object> GetDashboardAsync()
        {
            var messages = await _dbContext.ContactMessages
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();

            var total = messages.Count;
            var pending = messages.Count(x => x.Status == "pending");
            var inProgress = messages.Count(x => x.Status == "in progress");
            var answered = messages.Count(x => x.Status == "answered");
            var closed = messages.Count(x => x.Status == "closed");

            var categories = messages
                .GroupBy(x => x.Category)
                .Select(g => new
                {
                    category = string.IsNullOrWhiteSpace(g.Key) ? "Otro" : g.Key,
                    count = g.Count(),
                })
                .OrderByDescending(x => x.count)
                .ToList();

            var last7Days = Enumerable.Range(0, 7)
                .Select(offset => DateTime.UtcNow.Date.AddDays(-offset))
                .OrderBy(date => date)
                .Select(date => new
                {
                    day = date.ToString("ddd"),
                    date = date,
                    count = messages.Count(x => x.CreatedAt.Date == date),
                })
                .ToList();

            var responseTimes = messages
                .Where(x => x.AnsweredAt.HasValue)
                .Select(x => (x.AnsweredAt!.Value - x.CreatedAt).TotalHours)
                .ToList();

            var avgResponseHours = responseTimes.Count > 0 ? responseTimes.Average() : 0;

            return new
            {
                total,
                pending,
                inProgress,
                answered,
                closed,
                categories,
                byDay = last7Days,
                averageResponseHours = avgResponseHours,
                averageResponseText = avgResponseHours >= 24
                    ? ">24h"
                    : avgResponseHours >= 4
                        ? "4-24h"
                        : avgResponseHours >= 1
                            ? "1-4h"
                            : "<1h",
            };
        }

        public async Task<ContactMessage> CreateAsync(ContactMessage message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            message.Reference = string.IsNullOrWhiteSpace(message.Reference)
                ? $"FC-{DateTime.UtcNow:yyMMdd}{DateTime.UtcNow.Ticks % 10000:D4}"
                : message.Reference;

            message.CreatedAt = DateTime.UtcNow;
            message.Status = string.IsNullOrWhiteSpace(message.Status) ? "pending" : message.Status;

            _dbContext.ContactMessages.Add(message);
            await _dbContext.SaveChangesAsync();

            try
            {
                var companyEmail = new ContactMessage
                {
                    Name = message.Name,
                    Email = "isma_sl@inbox.lv",
                    UserId = message.UserId,
                    Subject = $"[Nueva consulta #{message.Reference}] {message.Category}",
                    Reference = message.Reference,
                    Category = message.Category,
                    Message = $"Usuario: {message.Name}\n\nEmail: {message.Email}\n\nMensaje: {message.Message}",
                    Phone = message.Phone,
                    Company = message.Company,
                };

                var customerEmail = new Email.Dto.EmailDto
                {
                    FromName = "FuturCasa",
                    ToEmail = message.Email,
                    Subject = message.Subject,
                    Name = message.Name,
                    Message = message.Message,
                };

                await _emailSender.SendEmailAsync(companyEmail, "Contact");
                await _emailSender.SendEmailAsync(customerEmail, "ContactConfirmation");
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "No se pudieron enviar correos para la consulta {Reference}", message.Reference);
            }

            return message;
        }

        public async Task<ContactMessage?> UpdateAsync(Guid id, ContactMessage message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            var existing = await _dbContext.ContactMessages.FirstOrDefaultAsync(x => x.Id == id);
            if (existing == null)
            {
                return null;
            }

            existing.UserId = message.UserId ?? existing.UserId;
            existing.Email = string.IsNullOrWhiteSpace(message.Email) ? existing.Email : message.Email;
            existing.Name = string.IsNullOrWhiteSpace(message.Name) ? existing.Name : message.Name;
            existing.Company = message.Company ?? existing.Company;
            existing.Subject = string.IsNullOrWhiteSpace(message.Subject) ? existing.Subject : message.Subject;
            existing.Category = string.IsNullOrWhiteSpace(message.Category) ? existing.Category : message.Category;
            existing.Message = string.IsNullOrWhiteSpace(message.Message) ? existing.Message : message.Message;
            existing.Phone = message.Phone ?? existing.Phone;
            existing.Reference = string.IsNullOrWhiteSpace(message.Reference) ? existing.Reference : message.Reference;
            existing.Status = string.IsNullOrWhiteSpace(message.Status) ? existing.Status : message.Status;
            existing.AnsweredBy = message.AnsweredBy ?? existing.AnsweredBy;
            existing.AnsweredAt = message.AnsweredAt ?? existing.AnsweredAt;

            if (!string.IsNullOrWhiteSpace(message.Status) && message.Status != existing.Status)
            {
                if (message.Status == "answered" || message.Status == "closed")
                {
                    existing.AnsweredAt ??= DateTime.UtcNow;
                }
            }

            await _dbContext.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var message = await _dbContext.ContactMessages.FirstOrDefaultAsync(x => x.Id == id);
            if (message == null)
            {
                return false;
            }

            _dbContext.ContactMessages.Remove(message);
            await _dbContext.SaveChangesAsync();
            return true;
        }
    }
}
