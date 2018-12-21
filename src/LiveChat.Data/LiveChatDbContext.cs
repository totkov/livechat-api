using Microsoft.EntityFrameworkCore;

using LiveChat.Data.Models;

namespace LiveChat.Data
{
    public class LiveChatDbContext : DbContext
    {
        public LiveChatDbContext(DbContextOptions<LiveChatDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }

        public DbSet<Chat> Chats { get; set; }

        public DbSet<Message> Messages { get; set; }

        public DbSet<UserChat> UserChats { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserChat>()
                .HasKey(uc => new { uc.UserId, uc.ChatId });

            modelBuilder.Entity<Message>()
                .HasOne(m => m.Chat)
                .WithMany(c => c.Messages)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Message>()
                .HasOne(m => m.Author)
                .WithMany(u => u.Messages)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
