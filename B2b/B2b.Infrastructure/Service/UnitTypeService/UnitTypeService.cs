using B2b.Dal.Context;
using B2b.Dal.Entity;
using B2b.Infrastructure.ResponseDto;
using Microsoft.EntityFrameworkCore;

namespace B2b.Infrastructure.Service.UnitTypeService
{
    public class UnitTypeService(B2bDbContext context) : IUnitTypeService
    {
        private readonly B2bDbContext _context = context;
        bool state = false;
        string message = string.Empty;
        List<UnitType> list = new();
        UnitType row = new();

        public ResultDto<UnitType> Add(UnitType data)
        {
            try
            {
                _context.UnitTypes.Add(data);
                _context.SaveChanges();
                state = true;
                message = "UnitType added successfully.";
            }
            catch (Exception ex)
            {
                message = ex.Message;
                state = false;
            }
            return new ResultDto<UnitType>
            {
                State = state,
                Message = message,
            };
        }

        public ResultDto<UnitType> GetAll()
        {
            try
            {
                list = _context.UnitTypes.ToList();
                state = true;
                message = "Brands retrieved successfully.";
            }
            catch (Exception ex)
            {
                message = ex.Message;
                state = false;
            }
            return new ResultDto<UnitType>
            {
                State = state,
                Message = message,
                List = list
            };
        }

        public ResultDto<UnitType> GetById(int id)
        {
            try
            {
                row = _context.UnitTypes.Where(p => p.UnitTypeId == id).FirstOrDefault();
                state = true;
                message = "Brands retrieved successfully.";
            }
            catch (Exception ex)
            {
                message = ex.Message;
                state = false;
            }
            return new ResultDto<UnitType>
            {
                State = state,
                Message = message,
                Data = row
            };
        }

        public ResultDto<UnitType> Remove(int id)
        {
            try
            {
                _context.UnitTypes.RemoveRange(_context.UnitTypes.Where(p => p.UnitTypeId == id).ToList());
                _context.SaveChanges();
                state = true;
                message = "UnitTypes removed successfully.";
            }
            catch (Exception ex)
            {
                message = ex.Message;
                state = false;
            }
            return new ResultDto<UnitType>
            {
                State = state,
                Message = message,
            };

        }

        public ResultDto<UnitType> Update(UnitType data)
        {

            try
            {
                _context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.TrackAll;
                var upd = _context.UnitTypes.Where(p => p.UnitTypeId == data.UnitTypeId).First();
                upd.UnitTypeName = data.UnitTypeName;
                _context.SaveChanges();
                _context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
                state = true;
                message = "Brand updated successfully.";
            }
            catch (Exception ex)
            {
                state = false;
                message = ex.Message;
            }
            return new ResultDto<UnitType>
            {
                State = state,
                Message = message
            };
        }
    }
}
