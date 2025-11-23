using B2b.Dal.Entity;
using B2b.Infrastructure.RequestDto;
using B2b.Infrastructure.ResponseDto;
namespace B2b.Infrastructure.Service.UserService
{
    public interface IUserService
    {
        void Add(User user);
        Task<User> GetByIdAsync(int userId);
        User GetById(int userId);
        void Remove(int userId);
        List<User> GetAll();
        void UserUpdate(User user);
        void Update(User user);
        List<City> GetCities();
        List<District> GetDistricts();
        ResultDto<string> RecorveryPassword(string email);
        public ResultDto<string> UserRegister(UserDto User);
    }
}
