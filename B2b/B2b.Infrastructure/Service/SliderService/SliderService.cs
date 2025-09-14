using B2b.Dal.Context;
using B2b.Dal.Entity;
using B2b.Infrastructure.ResponseDto;
using B2b.Infrastructure.Service.BrandService;
using Microsoft.EntityFrameworkCore;

namespace B2b.Infrastructure.Service.SliderService
{
    public class SliderService(B2bDbContext context) : ISliderService
    {
        private readonly B2bDbContext _context = context;
        bool state = false;
        string message = string.Empty;
        List<Slider> brands = new();
        Slider brand = new();
        public ResultDto<Slider> Add(Slider data)
        {
            try
            {
                _context.Sliders.Add(data);
                _context.SaveChanges();
                state = true;
                message = "Brand added successfully.";
            }
            catch (Exception ex)
            {
                message = ex.Message;
                state = false;
            }
            return new ResultDto<Slider>
            {
                State = state,
                Message = message,
            };
        }
        public ResultDto<Slider> GetAll()
        {
            try
            {
                brands = _context.Sliders.ToList();
                state = true;
                message = "Brands retrieved successfully.";
            }
            catch (Exception ex)
            {
                message = ex.Message;
                state = false;
            }
            return new ResultDto<Slider>
            {
                State = state,
                Message = message,
                List = brands
            };
        }
        public ResultDto<Slider> GetById(int id)
        {
            try
            {
                brand = _context.Sliders.Where(p => p.SliderId == id).FirstOrDefault();
                state = true;
                message = "Brands retrieved successfully.";
            }
            catch (Exception ex)
            {
                message = ex.Message;
                state = false;
            }
            return new ResultDto<Slider>
            {
                State = state,
                Message = message,
                Data = brand
            };
        }
        public ResultDto<Slider> Remove(int id)
        {
            try
            {
                _context.Sliders.RemoveRange(_context.Sliders.Where(p => p.SliderId == id).ToList());
                _context.SaveChanges();
                state = true;
                message = "Brand removed successfully.";
            }
            catch (Exception ex)
            {
                message = ex.Message;
                state = false;
            }
            return new ResultDto<Slider>
            {
                State = state,
                Message = message,
            };

        }
        public ResultDto<Slider> Update(Slider data)
        {

            try
            {
                _context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.TrackAll;
                var upd = _context.Sliders.Where(p => p.SliderId == data.SliderId).First();
                string oldImg = upd.SliderUrl;
                upd.SliderName = data.SliderName;
                upd.SliderUrl = data.SliderUrl;
                upd.Queno = data.Queno;
                _context.SaveChanges();
                _context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
                if (oldImg != data.SliderUrl)
                {
                    GeneralService.DeletedFile(oldImg);
                }
                _context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
                state = true;
                message = "Slider updated successfully.";
            }
            catch (Exception ex)
            {
                state = false;
                message = ex.Message;
            }
            return new ResultDto<Slider>
            {
                State = state,
                Message = message
            };
        }
    }
}
