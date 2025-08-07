using auth.Data;
using auth.Events.Incoming;
using auth.Events.Outgoing;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace auth.Consumers
{
    public class UserInfoConsumer : IConsumer<UserInfo>
    {
        private readonly AuthDbContext _context;
        private readonly ILogger<UserInfoConsumer> _logger;
        public UserInfoConsumer(AuthDbContext context, ILogger<UserInfoConsumer> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<UserInfo> context)
        {
            var userId = context.Message.id;
            var user = await _context.AuthUsers
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user is null)
            {
                _logger.LogCritical("User with ID {id} not found! \nJwt key possibly reached! \nChange Jwt Key ASAP!", userId);
                return;
            }

            await context.Publish(new UserCreated(user.Id, user.Email, user.CreatedAt));
        }
    }
}
