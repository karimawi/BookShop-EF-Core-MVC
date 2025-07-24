using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookShop.Models
{
    [Table("Categories", Schema = "MasterSchema")]
    public class Category
    {
        [Key]
        public int Id { get; set; }

        public string CatName { get; set; }

        public int CatOrder { get; set; }

        [NotMapped]
        public DateTime CreatedDate { get; set; }

        public bool IsDeleted { get; set; }
    }
}
