namespace BookShop.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public double Price { get; set; }
        
        // Foreign key
        public int CategoryId { get; set; }
        
        // Navigation property
        public Category Category { get; set; } = null!;
    }
}
