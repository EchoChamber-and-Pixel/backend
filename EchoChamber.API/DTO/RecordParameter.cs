namespace EchoChamber.API.DTO
{
    public class RecordParameter : Parameter
    {
        public int[] Id { get; set; }
        public long[] SteamID64 { get; set; }
        public string[] MapName { get; set; }
        public int[] Course { get; set; }
        public bool? IsBonus { get; set; } = null;
        public int[] Style { get; set; }
        public bool? IsPro { get; set; } = null;
        public string[] Mode { get; set; }
    }
}