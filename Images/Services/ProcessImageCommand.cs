namespace Images.Services;

public sealed record ProcessImageCommand(
    Guid ImageId,
    string RawKey,
    string TargetBaseKey,
    string OwnerType,
    string OwnerId);
