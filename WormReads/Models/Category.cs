using System.ComponentModel.DataAnnotations;

namespace WormReads.Models
{
    public class Category
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Range(1, 100)]
        [Required]
        public int DisplayOrder { get; set; }
    }
}
