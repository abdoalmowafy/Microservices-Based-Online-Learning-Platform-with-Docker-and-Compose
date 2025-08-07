using MassTransit;

namespace auth.Events.Incoming
{
    [EntityName("user.info")]
    public record UserInfo(Guid id);
}
