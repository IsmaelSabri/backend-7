using System.ComponentModel.DataAnnotations.Schema;

namespace Users.Models;
[Table("Chat")]
public sealed class Chat
{
    public string? Id { get; set; }
    public string? UserId { get; set; }
    public string? ToUserId { get; set; }
    public string Message { get; set; } = string.Empty;
    public DateTime Date {  get; set; }
}
