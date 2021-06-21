using EchoChamber.API.Models;
using Microsoft.EntityFrameworkCore;

namespace EchoChamber.API.Data
{
    public class EchoContext : DbContext
    {
        public EchoContext(DbContextOptions<EchoContext> options) : base(options)
        {
        }

        public DbSet<Map> Maps { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<Record> Records{ get; set; }
        public DbSet<Replay> Replays { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Player>()
                .HasKey(p => p.SteamID64);
            modelBuilder.Entity<Player>()
                .Property(p => p.Created)
                .HasDefaultValueSql("now()");

            modelBuilder.Entity<Map>()
                .HasIndex(m => m.Name)
                .IsUnique();
            modelBuilder.Entity<Map>()
                .Property(m => m.Id)
                .ValueGeneratedOnAdd();
            modelBuilder.Entity<Map>()
                .Property(m => m.Created)
                .HasDefaultValueSql("now()");

            modelBuilder.Entity<Record>()
                .Property(r => r.Id)
                .ValueGeneratedOnAdd();
            modelBuilder.Entity<Record>()
                .Property(r => r.Created)
                .HasDefaultValueSql("now()");
            modelBuilder.Entity<Record>()
                .HasOne(p => p.Player)
                .WithMany()
                .HasForeignKey(r => r.SteamID64);
			modelBuilder.Entity<Record>()
                .HasOne(r => r.Replay)
                .WithOne(r => r.Record)
                .HasForeignKey<Replay>(r => r.Id);
        }
    }
}
