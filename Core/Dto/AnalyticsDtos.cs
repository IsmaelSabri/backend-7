namespace Core.Dto;

public class AnalyticsRecentActivityDto
{
    public Guid Id { get; set; }
    public string EventType { get; set; } = string.Empty;
    public DateTime OccurredAt { get; set; }
    public string? UserId { get; set; }
    public string? Username { get; set; }
    public string? ViviendaId { get; set; }
    public string? Channel { get; set; }
    public Dictionary<string, object>? Metadata { get; set; }
}

public class AnalyticsDashboardDto
{
    public AnalyticsOverviewDto Overview { get; set; } = new();
    public List<AnalyticsRecentActivityDto> RecentActivity { get; set; } = [];
}

public class AnalyticsOverviewDto
{
    public int TotalEvents { get; set; }
    public int Users { get; set; }
    public int Views { get; set; }
    public int Favorites { get; set; }
    public int Unfavorites { get; set; }
    public int EmailsSent { get; set; }
    public int EmailsFailed { get; set; }
    public int ContactsCreated { get; set; }
    public int ChatsStarted { get; set; }
    public int MessagesSent { get; set; }
    public int MessagesReceived { get; set; }
}

public class EmailMetricsDto
{
    public int Sent { get; set; }
    public int Failed { get; set; }
    public int Delivered { get; set; }
    public int Opened { get; set; }
}

public class ContactMetricsDto
{
    public int Created { get; set; }
    public int Answered { get; set; }
    public int Closed { get; set; }
}

public class ChatMetricsDto
{
    public int Started { get; set; }
    public int MessagesSent { get; set; }
    public int MessagesReceived { get; set; }
}

public class ActivityPointDto
{
    public DateTime Date { get; set; }
    public int Views { get; set; }
    public int Favorites { get; set; }
    public int Contacts { get; set; }
    public int Chats { get; set; }
}

public class PropertyRankingItemDto
{
    public string HomeId { get; set; } = string.Empty;
    public int Views { get; set; }
    public int Favorites { get; set; }
    public int Unfavorites { get; set; }
    public int Contacts { get; set; }
    public int Chats { get; set; }
    public int MessagesSent { get; set; }
    public int MessagesReceived { get; set; }
}

public class PropertyRankingResponseDto
{
    public List<PropertyRankingItemDto> Items { get; set; } = new();
}

public class AnalyticsIdentityResolveDto
{
    public string AnonymousId { get; set; } = string.Empty;
    public string SessionId { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
}
