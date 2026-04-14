using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.Sqlite;
using SecureMarketplace.Models;
using System.Collections.Generic;

namespace SecureMarketplace.Pages.Products;

[Authorize]
public class SearchModel : PageModel
{
    private readonly IConfiguration _configuration;

    public SearchModel(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string? SearchTerm { get; set; }
    public string? ErrorMessage { get; set; }
    public List<Product>? Products { get; set; }

    // ==========================================
    // VULNERABLE CODE - SQL INJECTION
    // This concatenates user input directly into SQL query
    // ==========================================
    public IActionResult OnGet(string searchTerm)
    {
        SearchTerm = searchTerm;
        
        if (string.IsNullOrEmpty(searchTerm))
        {
            return Page();
        }

        try
        {
            // DANGEROUS: String concatenation allows SQL injection
            // Attackers can input: ' OR '1'='1' --
            // This will return ALL products!
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            
            using var connection = new SqliteConnection(connectionString);
            connection.Open();
            
            // VULNERABLE QUERY - NEVER DO THIS IN PRODUCTION
            string sql = "SELECT Id, Title, Description, Price, SellerId, CreatedAt FROM Products WHERE Title LIKE '%" + searchTerm + "%'";
            
            using var command = new SqliteCommand(sql, connection);
            using var reader = command.ExecuteReader();
            
            Products = new List<Product>();
            while (reader.Read())
            {
                Products.Add(new Product
                {
                    Id = reader.GetInt32(0),
                    Title = reader.GetString(1),
                    Description = reader.GetString(2),
                    Price = reader.GetDecimal(3),
                    SellerId = reader.GetString(4),
                    CreatedAt = reader.GetDateTime(5)
                });
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = "Search error: " + ex.Message;
        }

        return Page();
    }
}