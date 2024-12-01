using Chat.Models;
using Microsoft.EntityFrameworkCore;

namespace Chat.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<ChatMessage> ChatMessages { get; set; }
    public DbSet<ChatConnection> ChatConnections { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Add indexes for commonly queried fields
        modelBuilder.Entity<ChatMessage>()
            .HasIndex(m => m.ChatRoom);

        modelBuilder.Entity<ChatMessage>()
            .HasIndex(m => m.SentAt);

        modelBuilder.Entity<ChatConnection>()
            .HasIndex(c => c.ChatRoom);
    }
}