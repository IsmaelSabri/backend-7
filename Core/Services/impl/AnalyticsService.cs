using Core.Constants;
using Core.Data;
using Core.Dto;
using Core.Models;
using Microsoft.EntityFrameworkCore;

namespace Core.Services.impl;

public class AnalyticsService(CoreDb db) : IAnalyticsService
{
    public async Task TrackAsync(TrackEvent trackEvent, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(trackEvent);

        if (string.IsNullOrWhiteSpace(trackEvent.EventType))
        {
            throw new ArgumentException("EventType is required.", nameof(trackEvent));
        }

        if (!TrackEventTypes.Supported.Contains(trackEvent.EventType, StringComparer.OrdinalIgnoreCase))
        {
            throw new ArgumentException($"EventType '{trackEvent.EventType}' is not supported.", nameof(trackEvent));
        }

        if (string.IsNullOrWhiteSpace(trackEvent.AnonymousId))
        {
            throw new ArgumentException("AnonymousId is required.", nameof(trackEvent));
        }

        if (string.IsNullOrWhiteSpace(trackEvent.SessionId))
        {
            throw new ArgumentException("SessionId is required.", nameof(trackEvent));
        }

        trackEvent.Id = Guid.NewGuid();
        trackEvent.CreatedAt = DateTime.UtcNow;
        trackEvent.OccurredAt = trackEvent.OccurredAt == default ? DateTime.UtcNow : trackEvent.OccurredAt;
        trackEvent.Metadata ??= new Dictionary<string, object>();

        await db.TrackEvents.AddAsync(trackEvent, cancellationToken);
        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task ResolveIdentityAsync(string anonymousId, string sessionId, string userId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(anonymousId))
        {
            throw new ArgumentException("AnonymousId is required.", nameof(anonymousId));
        }

        if (string.IsNullOrWhiteSpace(sessionId))
        {
            throw new ArgumentException("SessionId is required.", nameof(sessionId));
        }

        if (string.IsNullOrWhiteSpace(userId))
        {
            throw new ArgumentException("UserId is required.", nameof(userId));
        }

        var identity = await db.AnalyticsIdentities
            .FirstOrDefaultAsync(x => x.AnonymousId == anonymousId && x.SessionId == sessionId, cancellationToken);

        if (identity is null)
        {
            identity = new AnalyticsIdentity
            {
                Id = Guid.NewGuid(),
                AnonymousId = anonymousId,
                SessionId = sessionId,
                UserId = userId,
                Resolved = true,
                FirstSeenAt = DateTime.UtcNow,
                LastSeenAt = DateTime.UtcNow,
            };

            await db.AnalyticsIdentities.AddAsync(identity, cancellationToken);
        }
        else
        {
            identity.UserId = userId;
            identity.Resolved = true;
            identity.LastSeenAt = DateTime.UtcNow;
        }

        var orphaned = await db.TrackEvents
            .Where(e => e.AnonymousId == anonymousId && e.SessionId == sessionId && string.IsNullOrWhiteSpace(e.UserId))
            .ToListAsync(cancellationToken);

        foreach (var eventItem in orphaned)
        {
            eventItem.UserId = userId;
        }

        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task<AnalyticsOverviewDto> GetOverviewAsync(DateTime from, DateTime to, CancellationToken cancellationToken = default)
    {
        var events = await db.TrackEvents
            .Where(e => e.OccurredAt >= from && e.OccurredAt <= to)
            .ToListAsync(cancellationToken);

        return new AnalyticsOverviewDto
        {
            TotalEvents = events.Count,
            Users = await db.AnalyticsIdentities
                .Where(x => x.UserId != null && x.LastSeenAt >= from && x.LastSeenAt <= to)
                .Select(x => x.UserId)
                .Distinct()
                .CountAsync(cancellationToken),
            Views = CountByType(events, TrackEventTypes.PropertyViewed),
            Favorites = CountByType(events, TrackEventTypes.PropertyFavorited),
            Unfavorites = CountByType(events, TrackEventTypes.PropertyUnfavorited),
            EmailsSent = CountByType(events, TrackEventTypes.EmailSent),
            EmailsFailed = CountByType(events, TrackEventTypes.EmailFailed),
            ContactsCreated = CountByType(events, TrackEventTypes.UserPhoneViewed),
            ChatsStarted = CountByType(events, TrackEventTypes.ChatStarted),
            MessagesSent = CountByType(events, TrackEventTypes.ChatMessageSent),
            MessagesReceived = CountByType(events, TrackEventTypes.ChatMessageReceived)
        };
    }

    public async Task<EmailMetricsDto> GetEmailMetricsAsync(DateTime from, DateTime to, CancellationToken cancellationToken = default)
    {
        var events = await db.TrackEvents
            .Where(e => e.OccurredAt >= from && e.OccurredAt <= to)
            .ToListAsync(cancellationToken);

        return new EmailMetricsDto
        {
            Sent = CountByType(events, TrackEventTypes.EmailSent),
            Failed = CountByType(events, TrackEventTypes.EmailFailed),
            Delivered = CountByType(events, TrackEventTypes.EmailDelivered),
            Opened = CountByType(events, TrackEventTypes.EmailOpened)
        };
    }

    public async Task<ContactMetricsDto> GetContactMetricsAsync(DateTime from, DateTime to, CancellationToken cancellationToken = default)
    {
        var events = await db.TrackEvents
            .Where(e => e.OccurredAt >= from && e.OccurredAt <= to)
            .ToListAsync(cancellationToken);

        return new ContactMetricsDto
        {
            Created = CountByType(events, TrackEventTypes.UserPhoneViewed),
            Answered = CountByType(events, TrackEventTypes.EmailSent),
            Closed = CountByType(events, TrackEventTypes.EmailDelivered)
        };
    }

    public async Task<ChatMetricsDto> GetChatMetricsAsync(DateTime from, DateTime to, CancellationToken cancellationToken = default)
    {
        var events = await db.TrackEvents
            .Where(e => e.OccurredAt >= from && e.OccurredAt <= to)
            .ToListAsync(cancellationToken);

        return new ChatMetricsDto
        {
            Started = CountByType(events, TrackEventTypes.ChatStarted),
            MessagesSent = CountByType(events, TrackEventTypes.ChatMessageSent),
            MessagesReceived = CountByType(events, TrackEventTypes.ChatMessageReceived)
        };
    }

    public async Task<List<ActivityPointDto>> GetActivityAsync(DateTime from, DateTime to, CancellationToken cancellationToken = default)
    {
        var events = await db.TrackEvents
            .Where(e => e.OccurredAt >= from && e.OccurredAt <= to)
            .ToListAsync(cancellationToken);

        return events
            .GroupBy(e => e.OccurredAt.Date)
            .Select(g => new ActivityPointDto
            {
                Date = g.Key,
                Views = g.Count(e => e.EventType == TrackEventTypes.PropertyViewed),
                Favorites = g.Count(e => e.EventType == TrackEventTypes.PropertyFavorited),
                Contacts = g.Count(e => e.EventType == TrackEventTypes.UserPhoneViewed),
                Chats = g.Count(e => e.EventType == TrackEventTypes.ChatStarted)
            })
            .OrderBy(x => x.Date)
            .ToList();
    }

    public async Task<PropertyRankingResponseDto> GetPropertiesAsync(DateTime from, DateTime to, CancellationToken cancellationToken = default)
    {
        var events = await db.TrackEvents
            .Where(e => e.OccurredAt >= from && e.OccurredAt <= to && !string.IsNullOrWhiteSpace(e.HomeId))
            .ToListAsync(cancellationToken);

        var grouped = events
            .GroupBy(e => e.HomeId)
            .Select(g => new PropertyRankingItemDto
            {
                HomeId = g.Key ?? string.Empty,
                Views = g.Count(e => e.EventType == TrackEventTypes.PropertyViewed),
                Favorites = g.Count(e => e.EventType == TrackEventTypes.PropertyFavorited),
                Unfavorites = g.Count(e => e.EventType == TrackEventTypes.PropertyUnfavorited),
                Contacts = g.Count(e => e.EventType == TrackEventTypes.UserPhoneViewed)
            })
            .OrderByDescending(x => x.Views + x.Favorites + x.Contacts)
            .Take(10)
            .ToList();

        return new PropertyRankingResponseDto { Items = grouped };
    }

    private static int CountByType(IEnumerable<TrackEvent> events, string type) =>
        events.Count(e => string.Equals(e.EventType, type, StringComparison.OrdinalIgnoreCase));
}
