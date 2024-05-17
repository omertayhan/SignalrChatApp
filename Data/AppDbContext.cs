using Microsoft.EntityFrameworkCore;
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
    }
}
