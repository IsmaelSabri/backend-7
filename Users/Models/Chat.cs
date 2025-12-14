using System.ComponentModel.DataAnnotations.Schema;

namespace Users.Models;

using Sieve.Attributes;
[Table("Chat")]
public sealed class Chat
{
    [Sieve(CanFilter = true, CanSort = true)]
    public string? Id { get; set; }
    [Sieve(CanFilter = true, CanSort = true)]
    public string? UserId { get; set; }
    [Sieve(CanFilter = true, CanSort = true)]
    public string? ToUserId { get; set; }
    [Sieve(CanFilter = true, CanSort = true)]
    public string Message { get; set; } = string.Empty;
    [Sieve(CanFilter = true, CanSort = true)]
    public DateTime Date { get; set; }
    [Sieve(CanFilter = true, CanSort = true)]
    public string? State { get; set; }
    [Sieve(CanFilter = true, CanSort = true)]
    public string? ViviendaId { get; set; }
}
