using System.ComponentModel.DataAnnotations;
namespace B2b.Dal.Entity
{
    public class SecurityObject
    {
        [Key]
        public int SecurityObjectId { get; set; }
        public string ObjectDisplayName { get; set; }
        public int Queno { get; set; }
        public bool IsMenu { get; set; }
        public int MainSecurityObjectId { get; set; }
        public string Icon { get; set; }
        public string SeoUrl { get; set; }
        public string ControllerName { get; set; }
        public string ActionName { get; set; }
        public List<RolePermission> RolePermissions { get; set; }
    }
}
