using B2b.Dal.Context;
using B2b.Dal.Entity;
using B2b.Infrastructure.ResponseDto;
using Microsoft.EntityFrameworkCore;

namespace B2b.Infrastructure.Service.BrandService
{
    public class BrandService(B2bDbContext context) : IBrandService
    {
        private readonly B2bDbContext _context = context;
        bool state = false;
        string message = string.Empty;
        List<Brand> brands = new();
        Brand brand = new();
        public ResultDto<Brand> Add(Brand data)
        {
            try
            {
                _context.Brands.Add(data);
                _context.SaveChanges();
                state = true;
                message = "Brand added successfully.";
            }
            catch (Exception ex)
            {
                message = ex.Message;
                state = false;
            }
            return new ResultDto<Brand>
            {
                State = state,
                Message = message,
            };
        }
        public ResultDto<Brand> GetAll()
        {
            try
            {
               brands= _context.Brands.ToList();
                state = true;
                message = "Brands retrieved successfully.";
            }
            catch (Exception ex)
            {
                message = ex.Message;
                state = false;
            }
            return new ResultDto<Brand>
            {
                State = state,
                Message = message,
                List = brands
            };
        }
        public ResultDto<Brand> GetById(int id)
        {
            try
            {
                brand = _context.Brands.Where(p=>p.BrandId==id).FirstOrDefault();
                state = true;
                message = "Brands retrieved successfully.";
            }
            catch (Exception ex)
            {
                message = ex.Message;
                state = false;
            }
            return new ResultDto<Brand>
            {
                State = state,
                Message = message,
                Data = brand
            };
        }
        public ResultDto<Brand> Remove(int id)
        {
            try
            {
                _context.Brands.RemoveRange(_context.Brands.Where(p => p.BrandId == id).ToList());
                _context.SaveChanges();
                state = true;
                message = "Brand removed successfully.";
            }
            catch (Exception ex)
            {
                message = ex.Message;
                state = false;
            }
            return new ResultDto<Brand>
            {
                State = state,
                Message = message,
            };
            
        }
        public ResultDto<Brand> Update(Brand data)
        {

            try
            {
                _context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.TrackAll;
                var upd = _context.Brands.Where(p => p.BrandId == data.BrandId).First();
                string oldImg = upd.ImageUrl;
                upd.BrandName = data.BrandName;
                upd.ImageUrl = data.ImageUrl;
                upd.Queno = data.Queno;
                _context.SaveChanges();
                _context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
                if (oldImg != data.ImageUrl)
                {
                    GeneralService.DeletedFile(oldImg);
                }
                _context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
                state = true;
                message = "Brand updated successfully.";
            }
            catch (Exception ex)
            {
                state = false;
                message = ex.Message;
            }
            return new ResultDto<Brand>
            {
                State = state,
                Message = message
            };
        }
    }
}
