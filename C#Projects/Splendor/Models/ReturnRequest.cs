using System.ComponentModel.DataAnnotations;

namespace Splendor.Models
{
    public class ReturnRequest
    {
        [Required]
        public Dictionary<Token, int> Tokens { get; set; } = new Dictionary<Token, int>();

        public string? ReservingCardImageName { get; set; }
    }
}
