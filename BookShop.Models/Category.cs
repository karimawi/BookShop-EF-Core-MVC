using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookShop.Models
{
    public class Category
    {
        public int Id { get; set; }
        
        // [Required]
        // [MaxLength(50)]
        public string CatName { get; set; } = string.Empty;
        
        // [Required]
        public int CatOrder { get; set; }
        
        // [DefaultValue(false)]
        public bool IsDeleted { get; set; } = false;
        
        // [DefaultValue(true)]
        public bool IsActive { get; set; } = true;
        
        // [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime CreatedDate { get; set; }
    }
}
