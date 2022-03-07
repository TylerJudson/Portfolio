namespace Splendor.Models
{
    public class ReturnRequest
    {
        public Dictionary<Token, int> Tokens { get; set; }
        public string? ReservingCardImageName { get; set; }
    }
}
