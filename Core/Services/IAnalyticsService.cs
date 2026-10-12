using Core.Dto;
using Core.Models;

namespace Core.Services;

public interface IAnalyticsService
{
    Task TrackAsync(TrackEvent trackEvent, CancellationToken cancellationToken = default);
    Task ResolveIdentityAsync(string anonymousId, string sessionId, string userId, CancellationToken cancellationToken = default);
    Task<AnalyticsOverviewDto> GetOverviewAsync(DateTime from, DateTime to, CancellationToken cancellationToken = default);
    Task<EmailMetricsDto> GetEmailMetricsAsync(DateTime from, DateTime to, CancellationToken cancellationToken = default);
    Task<ContactMetricsDto> GetContactMetricsAsync(DateTime from, DateTime to, CancellationToken cancellationToken = default);
    Task<ChatMetricsDto> GetChatMetricsAsync(DateTime from, DateTime to, CancellationToken cancellationToken = default);
    Task<List<ActivityPointDto>> GetActivityAsync(DateTime from, DateTime to, CancellationToken cancellationToken = default);
    Task<PropertyRankingResponseDto> GetPropertiesAsync(DateTime from, DateTime to, CancellationToken cancellationToken = default);
}
