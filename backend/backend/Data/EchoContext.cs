using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Data
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
    }
}