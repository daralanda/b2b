using B2b.Dal.Entity;
using B2b.Infrastructure.ResponseDto;

namespace B2b.Infrastructure.Service
{
    public interface IRoleService
    {
        void AddAsync(Role role);
        Role GetById(int roleId);
        void Remove(int roleId);
        List<Role> GetAll();
        void Update(Role role);
        List<RolePermissionDto> GetRolePermissions();
        List<RolePermissionDto> FindRolePermission(Role role);
    }
}
