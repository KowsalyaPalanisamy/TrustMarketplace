using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SecureMarketplace.Models;

public class Product
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;
    
    [Required]
    [StringLength(2000)]
    public string Description { get; set; } = string.Empty;
    
    [Required]
    [Range(0, 999999.99)]
    [DataType(DataType.Currency)]
    public decimal Price { get; set; }
    
    public string? ImageUrl { get; set; }
    
    // Explicit foreign key property
    public string? SellerId { get; set; }
    
    // Navigation property
    [ForeignKey("SellerId")]
    public virtual ApplicationUser? Seller { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}