using System.ComponentModel.DataAnnotations;

namespace EncurtadorUrl.Models
{
    public class ShortenedUrl
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string? OriginalUrl { get; set; } 
        public string? ShortCode { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
