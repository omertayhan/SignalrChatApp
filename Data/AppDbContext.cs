using Microsoft.EntityFrameworkCore;
using SignalrChatApp.Common;
using SignalrChatApp.Models;

namespace SignalrChatApp.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        public DbSet<Messages> Messages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Messages>()
                .HasKey(m => m.MessageId);
        }

        public async Task SaveMessageAsync(Messages message)
        {
            Messages.Add(message);
            await SaveChangesAsync();
        }

        public async Task<List<Messages>> GetMessagesAsync()
        {
            return await Messages.OrderBy(m => m.CreatedTime).ToListAsync();
        }

        public async Task<List<Messages>> GetMessagesByGroupId(string messageGroupId)
        {
            return await Messages.OrderBy(m => m.CreatedTime)
                                 .Where(m => m.MessageGroupId == messageGroupId && m.MessageType == MessageTypes.Group.ToString())
                                 .ToListAsync();
        }

        public async Task<List<Messages>> GetMessagesByPrivateGroupId(string messagePrivateGroupId)
        {
            return await Messages.OrderBy(m => m.CreatedTime)
                                 .Where(m => m.MessageGroupId == messagePrivateGroupId && m.MessageType == MessageTypes.Private.ToString())
                                 .ToListAsync();
        }
    }
}
