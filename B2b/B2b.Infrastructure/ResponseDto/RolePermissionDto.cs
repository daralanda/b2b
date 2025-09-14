namespace B2b.Infrastructure.ResponseDto
{
    public class RolePermissionDto
    { 
        public int SecurityObjectId { get; set; }
        public string SecurityObjectName { get; set; }
        public int MainSecurityObjectId { get; set; }
        public bool Selected { get; set; }
        public bool Deleted { get; set; }
        public bool Updated { get; set; }
        public bool Inserted { get; set; }
        public int RoleId { get; set; }
        public bool IsActive { get; set; }
        public string RoleName { get; set; }
        public int Queno { get; set; }
    }
}
