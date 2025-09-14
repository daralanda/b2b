using B2b.Dal.Context;
using B2b.Dal.Entity;
using B2b.Infrastructure.ResponseDto;
using Microsoft.EntityFrameworkCore;

namespace B2b.Infrastructure.Service.CustomerService
{
    public class CustomerService(B2bDbContext context) :ICustomerService
    {
        private readonly B2bDbContext _context = context;
        bool state = false;
        string message = string.Empty;
        List<Customer> customers = new();
        Customer customer = new();
        public ResultDto<Customer> Add(Customer data)
        {
            try
            {
                _context.Customers.Add(data);
                _context.SaveChanges();
                state = true;
                message = "Customer added successfully.";
            }
            catch (Exception ex)
            {
                message = ex.Message;
                state = false;
            }
            return new ResultDto<Customer>
            {
                State = state,
                Message = message,
            };
      
        }
        public ResultDto<Customer> GetAll() 
        {
            try
            {
                customers = _context.Customers.ToList();
                state = true;
                message = "customer retrieved successfully.";
            }
            catch (Exception ex)
            {
                state = false;
                message = ex.Message;
            }
            return new ResultDto<Customer>
            {
                State = state,
                Message = message,
                List = customers
            };
        }
        public ResultDto<Customer> GetById(int id)
        {
            try
            {
                customer = _context.Customers.Where(p => p.CustomerId == id).FirstOrDefault();
                state = true;
                message = "customer retrieved successfully.";
            }
            catch (Exception ex)
            {
                state = false;
                message = ex.Message;
            }
            return new ResultDto<Customer>
            {
                State = state,
                Message = message,
                Data = customer
            };
        }
        public ResultDto<Customer> Remove(int id)
        {
            try
            {
                var del = _context.Customers.Where(p => p.CustomerId == id).ToList();
                _context.Customers.RemoveRange(del);
                _context.SaveChanges();
                state = true;
                message = "Customer removed successfully.";
            }
            catch (Exception ex)
            {
                state = false;
                message = ex.Message;
            }
            return new ResultDto<Customer>
            {
                State = state,
                Message = message,
            };
        }
        public ResultDto<Customer> Update(Customer data)
        {
            try
            {
                _context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.TrackAll;
                var upd = _context.Customers.Where(p => p.CustomerId == data.CustomerId).FirstOrDefault();
                upd.CustomerName = data.CustomerName;
                upd.Address = data.Address; 
                upd.TaxOffice = data.TaxOffice;
                upd.CityId = data.CityId;
                upd.DistrictId = data.DistrictId; 
                upd.TaxNo = data.TaxNo;
                upd.IsActive = data.IsActive;
                upd.Mail = data.Mail;
                upd.Phone = data.Phone;
                upd.IsIndividual = data.IsIndividual;
                _context.SaveChanges();
                _context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
                state = true;
                message = "Customer updated successfully.";
            }
            catch (Exception ex)
            {
                state = false;
                message = ex.Message;
            }
            return new ResultDto<Customer>
            {
                State = state,
                Message = message
            };
        }
        public ResultDto<City> GetAllCities()
        {
            var cities = new List<City>();
            try
            {
                cities = _context.Cities.ToList() ;
                state = true;
                message = "Cities retrieved successfully.";
            }
            catch (Exception ex)
            {
                state = false;
                message = ex.Message;
            }
            return new ResultDto<City>
            {
                State = state,
                Message = message,
                List = cities
            };
        }
        public ResultDto<District> GetAllDistricts()
        {
            var cities = new List<District>();
            try
            {
                cities = _context.Districts.ToList();
                state = true;
                message = "Districts retrieved successfully.";
            }
            catch (Exception ex)
            {
                state = false;
                message = ex.Message;
            }
            return new ResultDto<District>
            {
                State = state,
                Message = message,
                List = cities
            };
        }

    }
}
