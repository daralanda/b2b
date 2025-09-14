using B2b.Infrastructure.ResponseDto;
using B2b.Dal.Context;
using B2b.Dal.Entity;
using Microsoft.EntityFrameworkCore;

namespace B2b.Infrastructure.Service.BannerService
{
    public class BannerService(B2bDbContext context) : IBannerService
    {
        private readonly B2bDbContext _context = context;
        bool state = false;
        string message = string.Empty;
        List<Banner> banners = new();
        Banner banner = new();
        public ResultDto<Banner> Add(Banner data)
        {
            try
            {
                _context.Banners.Add(data);
                _context.SaveChanges();
                state = true;
                message = "Banner added successfully.";
            }
            catch (Exception ex)
            {
                message = ex.Message;
                state = false;
            }
            return new ResultDto<Banner>
            {
                State = state,
                Message = message,
                Data = data
            };
        }
        public ResultDto<Banner> GetAll()
        {
            try
            {
                banners = _context.Banners.ToList();
                state = true;
                message = "Banners retrieved successfully.";
            }
            catch (Exception ex)
            {
                state = false;
                message = ex.Message;
            }
            return new ResultDto<Banner>
            {
                State = state,
                Message = message,
                List = banners
            };
        }
        public ResultDto<Banner> GetById(int id)
        {
            try
            {
                banner = _context.Banners.Where(p => p.BannerId == id).FirstOrDefault();
                state = true;
                message = "Banner retrieved successfully.";
            }
            catch (Exception ex)
            {
                state = false;
                message = ex.Message;
            }
            return new ResultDto<Banner>
            {
                State = state,
                Message = message,
                Data = banner
            };
        }
        public ResultDto<Banner> Remove(int id)
        {
            try
            {
                var del = _context.Banners.Where(p => p.BannerId == id).ToList();
                _context.Banners.RemoveRange(del);
                _context.SaveChanges();
                foreach (var item in del)
                {
                    GeneralService.DeletedFile(item.ImageUrl);
                }
                state = true;
                message = "Banner removed successfully.";
            }
            catch (Exception ex)
            {
                state = false;
                message = ex.Message;
            }
            return new ResultDto<Banner>
            {
                State = state,
                Message = message,
            };
        }
        public ResultDto<Banner> Update(Banner data)
        {
            try
            {

                _context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.TrackAll;
                var upd = _context.Banners.Where(p => p.BannerId == data.BannerId).First();
                string oldImg = upd.ImageUrl;
                upd.BannerName = data.BannerName;
                upd.BannerUrl = data.BannerUrl;
                upd.ImageUrl = data.ImageUrl;
                upd.IsActive = data.IsActive;
                upd.BannerType = data.BannerType;
                upd.Queno = data.Queno;
                _context.SaveChanges();
                if (oldImg != data.ImageUrl)
                {
                    GeneralService.DeletedFile(oldImg);
                }
                _context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
                state = true;
                message= "Banner updated successfully.";
            }
            catch (Exception ex)
            {
                state = false;
                message = ex.Message;
            }
            return new ResultDto<Banner>
            {
                State = state,
                Message = message
            };
        }
    }
}
