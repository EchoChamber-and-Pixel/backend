using System;

namespace EchoChamber.API.Models
{
    public class Map
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime LastPlayed { get; set; }
        public DateTime Created { get; set; }
    }
}