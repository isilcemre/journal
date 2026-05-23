using Microsoft.EntityFrameworkCore;
using deneme2._0.Models;

namespace deneme2._0.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Hangi tabloların olacağını DbSet ile belirtiyoruz
        public DbSet<User> Users { get; set; }
        public DbSet<JournalEntry> JournalEntries { get; set; }
        public DbSet<JournalImage> JournalImages { get; set; }

        public DbSet<Folder> Folders { get; set; }
        public DbSet<JournalEntryFolder> JournalEntryFolders { get; set; }

        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User - JournalEntry ilişkisi
            modelBuilder.Entity<JournalEntry>()
                .HasOne(j => j.User)
                .WithMany()
                .HasForeignKey(j => j.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // JournalEntry - JournalImage ilişkisi (1'e çok)
            modelBuilder.Entity<JournalImage>()
                .HasOne(i => i.JournalEntry)
                .WithMany(j => j.Images)
                .HasForeignKey(i => i.JournalEntryId)
                .OnDelete(DeleteBehavior.Cascade);

            // Folder - User ilişkisi
            modelBuilder.Entity<Folder>()
                .HasOne(f => f.User)
                .WithMany()
                .HasForeignKey(f => f.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // JournalEntryFolder ilişkileri
            modelBuilder.Entity<JournalEntryFolder>()
                .HasOne(jef => jef.JournalEntry)
                .WithMany(j => j.JournalEntryFolders)
                .HasForeignKey(jef => jef.JournalEntryId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<JournalEntryFolder>()
                .HasOne(jef => jef.Folder)
                .WithMany(f => f.JournalEntryFolders)
                .HasForeignKey(jef => jef.FolderId)
                .OnDelete(DeleteBehavior.NoAction); // Cascade çakışmasını önler
        }
    }
}