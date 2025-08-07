using MassTransit;

namespace auth.Events.Outgoing
{
    [EntityName("user.created")]
    public record UserCreated(Guid id, string email, DateTime createdAt);
}
