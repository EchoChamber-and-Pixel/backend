namespace backend.Models
{
    public class Replay
    {
        public int Id { get; set; }
        public string Source { get; set; }
        public byte[] Data { get; set; }
    }
}