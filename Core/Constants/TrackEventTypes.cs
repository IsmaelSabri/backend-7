namespace Core.Constants;

public static class TrackEventTypes
{
    public const string PropertyViewed = "property.viewed";
    public const string PropertyFavorited = "property.favorited";
    public const string PropertyUnfavorited = "property.unfavorited";
    public const string UserPhoneViewed = "user.phone.viewed";
    public const string EmailSent = "email.sent";
    public const string EmailDelivered = "email.delivered";
    public const string EmailFailed = "email.failed";
    public const string EmailOpened = "email.opened";
    public const string ChatStarted = "chat.started";
    public const string ChatMessageSent = "chat.message.sent";
    public const string ChatMessageReceived = "chat.message.received";

    public static readonly HashSet<string> Supported = new(StringComparer.OrdinalIgnoreCase)
    {
        PropertyViewed,
        PropertyFavorited,
        PropertyUnfavorited,
        UserPhoneViewed,
        EmailSent,
        EmailDelivered,
        EmailFailed,
        EmailOpened,
        ChatStarted,
        ChatMessageSent,
        ChatMessageReceived,
    };
}