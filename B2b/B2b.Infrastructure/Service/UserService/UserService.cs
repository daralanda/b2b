using B2b.Dal.Context;
using B2b.Dal.Entity;
using B2b.Infrastructure.RequestDto;
using B2b.Infrastructure.ResponseDto;
using B2b.Plugin.Mail;
using Microsoft.EntityFrameworkCore;

namespace B2b.Infrastructure.Service.UserService
{
    public class UserService(B2bDbContext context) : IUserService
    {
        private readonly B2bDbContext _context = context;
        ResultDto<string> result = new ResultDto<string>();
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
                return null;

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
            var data = _context.Users.ToList();
            return data;
        }
        public void UserUpdate(User user)
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
                    data.Phone = user.Phone;
                    data.CustomerName = user.CustomerName;
                    data.TaxOffice = user.TaxOffice;
                    data.TaxNo = user.TaxNo;
                    data.Address = user.Address;
                    data.DistrictId = user.DistrictId;
                    data.CityId = user.CityId;
                    data.IsIndividual = user.IsIndividual;
                    data.DiscountRate = user.DiscountRate;
                    _context.SaveChanges();
                    _context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
                }
            }
            catch
            {
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
                    data.CustomerName = user.CustomerName;
                    data.TaxOffice = user.TaxOffice;
                    data.TaxNo = user.TaxNo;
                    data.Address = user.Address;
                    data.DistrictId = user.DistrictId;
                    data.CityId = user.CityId;
                    data.IsIndividual = user.IsIndividual;
                    data.DiscountRate = user.DiscountRate;
                    _context.SaveChanges();
                    _context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
                }
            }
            catch
            {
            }
        }

        public List<City> GetCities()
        {
            return _context.Cities.ToList();
        }
        public List<District> GetDistricts()
        {
            return _context.Districts.ToList();
        }

        public ResultDto<string> RecorveryPassword(string email)
        {
            var user = _context.Users.Where(u => u.Email == email).FirstOrDefault();
            if (user != null)
            {
                var mailConf= _context.MailSettings.FirstOrDefault();
                result.State=MailPlugin.MailPost(mailConf.Host, mailConf.Port, mailConf.Email, mailConf.Password, mailConf.EnableSsl,user.Email,"Şifre Sıfırlama","Kullanıcı şifreniz : "+user.Password );
                if (result.State)
                {
                    result.Message = "Şifreniz Mail adresinize başarılı bir şekilde gönderilmiştir.";
                }
                else
                {
                    result.Message = "Mail gönderilirken bir hata oluştu.Lütfen daha sonra tekrar deneyiniz.";
                }
            }
            else
            {
                result.State = false;
                result.Message = "Kullanıcı mail adresiniz bulunamamıştır.";
            }
            return result;
        }
        public ResultDto<string> UserRegister(UserDto User)
        {
            try
            {
                var saveUser = new User
                {
                    FirstName=User.FirstName,
                    LastName=User.LastName,
                    Email=User.Email,
                    Phone=User.Phone,
                    Password=User.Password,
                    IsActive=true,
                    RoleId=2,
                    CreateDate=DateTime.Now
                };
                _context.Users.Add(saveUser);
                _context.SaveChanges();
                result.State = true;
                result.Message = "Kayıt işlemi başarılı bir şekilde gerçekleştirilmiştir.";
            }
            catch (Exception ex)
            {
                result.State = false;
                result.Message = "Kayıt işlemi sırasında bir hata oluştu.Lütfen daha sonra tekrar deneyiniz.";
            }
            return result;
        }

    }
}
