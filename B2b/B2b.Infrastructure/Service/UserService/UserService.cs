using B2b.Dal.Context;
using B2b.Dal.Entity;
using Microsoft.EntityFrameworkCore;

namespace B2b.Infrastructure.Service.UserService
{
    public class UserService(B2bDbContext context) : IUserService
    {
        private readonly B2bDbContext _context = context;

        public void Add(User user)
        {
            _context.Users.Add(user);
            _context.SaveChanges();
        }
        public async Task<User> GetByIdAsync(int userId)
        {
            try
            {
               return await _context.Users.Where(p => p.UserId == userId).FirstOrDefaultAsync();
            }
            catch
            {
                return null;
            }
        }
        public User GetById(int userId)
        {
            try
            {
                return _context.Users.Where(p => p.UserId == userId).FirstOrDefault(); ;
            }
            catch
            {
                return null ;

            }
        }
        public void Remove(int userId)
        {

            try
            {
                var user = _context.Users.Where(p => p.UserId == userId).FirstOrDefault();
                if (user != null)
                {
                    _context.Users.Remove(user);
                    _context.SaveChanges();
                }

            }
            catch
            {
            }
        }
        public List<User> GetAll()
        {
            var data= _context.Users.ToList();
            return data;
        }
        public User UserUpdate(User user)
        {
            try
            {
                _context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.TrackAll;
                User data = _context.Users.Where(p => p.UserId == user.UserId).First();
                if (data != null)
                {
                    data.Password = user.Password;
                    data.FirstName = user.FirstName;
                    data.LastName = user.LastName;
                    _context.Entry<User>(data).State = EntityState.Modified;
                    _context.SaveChanges();
                    _context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
                }
                return data;
            }
            catch 
            {
                return null;
            }
        }
        public void Update(User user)
        {
            try
            {
                _context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.TrackAll;
                var data = _context.Users.Where(p => p.UserId == user.UserId).FirstOrDefault();
                if (data != null)
                {
                    data.Password = user.Password;
                    data.FirstName = user.FirstName;
                    data.LastName = user.LastName;
                    data.Email = user.Email;
                    data.IsActive = user.IsActive;
                    data.RoleId = user.RoleId;
                    data.Phone = user.Phone;
                    _context.SaveChanges();
                    _context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
                }
            }
            catch
            {
            }
        }
        
    }
}
