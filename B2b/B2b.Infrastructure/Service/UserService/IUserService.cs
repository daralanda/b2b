using B2b.Dal.Entity;
namespace B2b.Infrastructure.Service.UserService
{
    public interface IUserService
    {
        void Add(User user);
        Task<User> GetByIdAsync(int userId);
        User GetById(int userId);
        void Remove(int userId);
        List<User> GetAll();
        User UserUpdate(User user);
        void Update(User user);
    }
}
