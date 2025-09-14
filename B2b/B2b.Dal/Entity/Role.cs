using System.ComponentModel.DataAnnotations;
namespace B2b.Dal.Entity
{
    public class Role
    {
        [Key]
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public bool IsActive { get; set; }
        public ICollection<User> Users { get; set; }
        public List<RolePermission> RolePermissions { get; set; }
    }
}
