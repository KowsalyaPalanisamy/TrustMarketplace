using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SecureMarketplace.Models;

// ==========================================
// VULNERABLE CODE - Missing input validation
// No [Required], No [Range], No [StringLength]
// BUT keeps Seller navigation for existing code
// ==========================================
public class Product
{
    public int Id { get; set; }
    
    // VULNERABLE: No validation - can be null, empty, or 10000 characters
    public string? Title { get; set; }
    
    // VULNERABLE: No validation - can be null, empty, or malicious script
    public string? Description { get; set; }
    
    // VULNERABLE: No range validation - can be negative or astronomical
    public decimal Price { get; set; }
    
    public string? ImageUrl { get; set; }
    
    // Foreign key to ApplicationUser (Seller)
    public string? SellerId { get; set; }
    
    // Navigation property - REQUIRED for existing code to work
    [ForeignKey("SellerId")]
    public virtual ApplicationUser? Seller { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}