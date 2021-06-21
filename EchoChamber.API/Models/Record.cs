using System;

namespace EchoChamber.API.Models
{
    public class Record
    {
        public int Id { get; set; }
        public long SteamID64 { get; set; }
        public int MapId { get; set; }
        public int Course { get; set; }
        public int Style { get; set; }
        public int Mode { get; set; }
        public int Teleports { get; set; }
        public long Time { get; set; }
        public DateTime Created { get; set; }

        public Map Map { get; set; }
        public Player Player { get; set; }
		public Replay Replay { get; set; }
    }
}
