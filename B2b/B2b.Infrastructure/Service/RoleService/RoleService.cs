using B2b.Dal.Context;
using B2b.Dal.Entity;
using B2b.Infrastructure.ResponseDto;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace B2b.Infrastructure.Service.RoleService
{
    public class RoleService(B2bDbContext context) : IRoleService
    {
        private readonly B2bDbContext _context = context;

        public void AddAsync(Role role)
        {
            try
            {
                _context.Roles.Add(role);
                _context.SaveChanges();
            }
            catch (Exception )
            {

            }
        }
        public Role GetById(int roleId)
        {
            try
            {
                return _context.Roles.Where(p => p.RoleId == roleId).FirstOrDefault();
            }
            catch
            {
                return null;
            }
        }
        public void Remove(int roleId)
        {
           
            try
            {
                var role = _context.Roles.Where(p => p.RoleId == roleId).FirstOrDefault();
                if (role != null)
                {
                    _context.Roles.Remove(role);
                    _context.SaveChanges();
                }
            }
            catch
            {
            }
        }
        public List<Role> GetAll()
        {
            try
            {
                var data= _context.Roles.ToList();
                return data;
            }
            catch
            {
                return null;
            }
        }
        public void Update(Role role)
        {
            try
            {
                _context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.TrackAll;
                var data = _context.Roles.Where(p => p.RoleId == role.RoleId).FirstOrDefault();
                if (data != null)
                {
                    data.RoleName = role.RoleName;
                    data.IsActive = role.IsActive;                   
                    _context.SaveChanges();
                    var deleteData = _context.RolePermissions.Where(p => p.RoleId == role.RoleId).ToList();
                    _context.RolePermissions.RemoveRange(deleteData);
                    _context.RolePermissions.AddRange(role.RolePermissions);
                    _context.SaveChanges();
                }
                _context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

            }
            catch 
            {
            }
        }
        public List<RolePermissionDto> GetRolePermissions() 
        {
            var result = new List<RolePermissionDto>();
            try
            {
               var data= _context.SecurityObjects.Select(p => new
                {
                    p.SecurityObjectId,
                    p.ObjectDisplayName,
                    p.MainSecurityObjectId,
                   Selected = false,
                   Deleted = false,
                   Updated = false,
                   Inserted = false,
                    RoleId = 0,
                    IsActive = false,
                    RoleName = "",
                    p.Queno,
                }).ToList();
                foreach (var item in data)
                {
                    result.Add(new RolePermissionDto
                    {
                        SecurityObjectId = item.SecurityObjectId,
                        SecurityObjectName = item.ObjectDisplayName,
                        MainSecurityObjectId = item.MainSecurityObjectId,
                        Selected = item.Selected,
                        Deleted = item.Deleted,
                        Updated = item.Updated,
                        Inserted = item.Inserted,
                        RoleId = item.RoleId,
                        IsActive = item.IsActive,
                        RoleName = item.RoleName,
                        Queno = item.Queno,
                    });
                }
            }
            catch (Exception)
            {
               
            }
            return result;
        }
        public List<RolePermissionDto> FindRolePermission(Role role) 
        {
            var result = new List<RolePermissionDto>();
            try
            {
                var data =_context.RolePermissions.Where(p => p.RoleId == role.RoleId).Select(p => new {
                    p.SecurityObjectId,
                    p.Deleted,
                    p.Selected,
                    p.Updated,
                    p.Inserted,
                    p.SecurityObject.ObjectDisplayName,
                    p.SecurityObject.MainSecurityObjectId,
                    p.RoleId,
                    p.SecurityObject.Queno
                }).ToList();
                foreach (var item in data)
                {
                    result.Add(new RolePermissionDto
                    {
                        SecurityObjectId = item.SecurityObjectId,
                        SecurityObjectName = item.ObjectDisplayName,
                        MainSecurityObjectId = item.MainSecurityObjectId,
                        Selected = item.Selected,
                        Deleted = item.Deleted,
                        Updated = item.Updated,
                        Inserted = item.Inserted,
                        RoleId = item.RoleId,
                        Queno = item.Queno,
                    });
                }
            }
            catch (Exception)
            {
            }
            return result;
        }
    }
}
