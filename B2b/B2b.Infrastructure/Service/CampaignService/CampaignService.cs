using B2b.Dal.Context;
using B2b.Dal.Entity;
using B2b.Infrastructure.ResponseDto;
using Microsoft.EntityFrameworkCore;

namespace B2b.Infrastructure.Service.CampaignService
{
    public class CampaignService(B2bDbContext context) :ICampaignService
    {
        private readonly B2bDbContext _context = context;
        bool state = false;
        string message = string.Empty;
        List<Campaign> accountNumbers = new();
        Campaign accountNumber = new();
        public ResultDto<Campaign> Add(Campaign data)
        {
            try
            {
                _context.Campaigns.Add(data);
                _context.SaveChanges();
                state = true;
                message = "Campaign added successfully.";
            }
            catch (Exception ex)
            {
                message = ex.Message;
                state = false;
            }
            return new ResultDto<Campaign>
            {
                State = state,
                Message = message,
                Data = data
            };
        }
        public ResultDto<Campaign> GetAll()
        {
            try
            {
                accountNumbers = _context.Campaigns.ToList();
                state = true;
                message = "Campaigns retrieved successfully.";
            }
            catch (Exception ex)
            {
                state = false;
                message = ex.Message;
            }
            return new ResultDto<Campaign>
            {
                State = state,
                Message = message,
                List = accountNumbers
            };
        }
        public ResultDto<Campaign> GetById(int id)
        {
            try
            {
                accountNumber = _context.Campaigns.Where(p => p.CampaignId == id).FirstOrDefault();
                state = true;
                message = "Campaign retrieved successfully.";
            }
            catch (Exception ex)
            {
                state = false;
                message = ex.Message;
            }
            return new ResultDto<Campaign>
            {
                State = state,
                Message = message,
                Data = accountNumber
            };
        }
        public ResultDto<Campaign> Remove(int id)
        {
            try
            {
                var del = _context.Campaigns.Where(p => p.CampaignId == id).ToList();
                _context.Campaigns.RemoveRange(del);
                _context.SaveChanges();
                state = true;
                message = "Campaign removed successfully.";
            }
            catch (Exception ex)
            {
                state = false;
                message = ex.Message;
            }
            return new ResultDto<Campaign>
            {
                State = state,
                Message = message,
            };
        }
        public ResultDto<Campaign> Update(Campaign data)
        {
            try
            {

                _context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.TrackAll;
                var upd = _context.Campaigns.Where(p => p.CampaignId == data.CampaignId).First();
                upd.CampaignName = data.CampaignName;
                upd.EndDate = data.EndDate;
                upd.StartDate = data.StartDate;
                upd.IsPercentage = data.IsPercentage;
                upd.IsActive = data.IsActive;
                upd.DiscountValue = data.DiscountValue;
                upd.CategoryId = data.CategoryId;
                _context.SaveChanges();
                _context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
                state = true;
                message = "Campaign updated successfully.";
            }
            catch (Exception ex)
            {
                state = false;
                message = ex.Message;
            }
            return new ResultDto<Campaign>
            {
                State = state,
                Message = message
            };
        }
    }
}
