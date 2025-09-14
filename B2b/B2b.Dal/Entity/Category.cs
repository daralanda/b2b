using System.ComponentModel.DataAnnotations;

namespace B2b.Dal.Entity
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int MainCategoryId { get; set; }
        public bool IsActive { get; set; }

    }
}
