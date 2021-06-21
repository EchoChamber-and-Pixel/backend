namespace EchoChamber.API.DTO
{
    public abstract class Parameter
    {
        public int? Limit { get; set; }
        public int? Offset { get; set; }        
    }
}