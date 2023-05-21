using Microsoft.EntityFrameworkCore;

namespace AFRI_AusCare.Models
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options)
        : base(options)
        {
        }

        public virtual DbSet<Event> Events { get; set; }
        public virtual DbSet<Gallery> Galleries { get; set; }
        public virtual DbSet<Team> Teams { get; set; }
        public virtual DbSet<BoardMember> BoardMembers { get; set; }
        public virtual DbSet<KeyPartner> KeyPartners { get; set; }
        public virtual DbSet<AdminSetting> AdminSettings { get; set; }
        public virtual DbSet<Album> Albums { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Event>().HasKey(x => x.Id);
            modelBuilder.Entity<Gallery>().HasKey(x => x.Id);
            modelBuilder.Entity<Team>().HasKey(x => x.Id);
            modelBuilder.Entity<BoardMember>().HasKey(x => x.Id);
            modelBuilder.Entity<KeyPartner>().HasKey(x => x.Id);
            modelBuilder.Entity<AdminSetting>().HasKey(x => x.Id);
            modelBuilder.Entity<Gallery>().HasOne(g => g.Album).WithMany(x => x.Galleries)
                .HasForeignKey(x => x.AlbumId);
            modelBuilder.Entity<Event>().HasOne(g => g.Album).WithOne(x => x.Event).
                HasForeignKey<Event>(x => x.AlbumId).IsRequired();
        }
    }
}
