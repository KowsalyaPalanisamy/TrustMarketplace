using System.ComponentModel.DataAnnotations;

namespace SecureMarketplace.Models
{
    public class Product
    {
        public int Id { get; set; }
    
    [Required]
    public string? Title { get; set; }
    
    [Required]
    public string? Description { get; set; }
    
    [Required]
    [Range(0, 999999.99)]
    public decimal Price { get; set; }
    
    public string? ImageUrl { get; set; }
    
    // Who created this product (Seller)
    public string? SellerId { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}