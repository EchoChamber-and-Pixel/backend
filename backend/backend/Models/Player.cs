using System;

namespace backend.Models
{
    public class Player
    {
        public long SteamID64 { get; set; }
        public string Name { get; set; }
        public DateTime LastConnect { get; set; }
        public DateTime Created { get; set; }
    }
}