using System.ComponentModel.DataAnnotations;
namespace B2b.Dal.Entity
{
    public class RolePermission
    {
        [Key]
        public int RolePermissionId { get; set; }
        public int RoleId { get; set; }
        public int SecurityObjectId { get; set; }
        public bool Inserted { get; set; }
        public bool Selected { get; set; }
        public bool Deleted { get; set; }
        public bool Updated { get; set; }

        public virtual Role Role { get; set; }
        public virtual SecurityObject SecurityObject { get; set; }
    }
}
