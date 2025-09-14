using B2b.Dal.Context;
using B2b.Dal.Entity;
using B2b.Infrastructure.ResponseDto;
using Microsoft.EntityFrameworkCore;

namespace B2b.Infrastructure.Service.OrderService
{
    public class OrderService(B2bDbContext context): IOrderService
    {
        private readonly B2bDbContext _context = context;
        bool state = false;
        string message = string.Empty;
        List<Order> orders = new();
        Order order = new();
        public ResultDto<Order> Add(Order data)
        {
            try
            {
                _context.Orders.Add(data);
                _context.SaveChanges();
                state = true;
                message = "Order added successfully.";
            }
            catch (Exception ex)
            {
                message = ex.Message;
                state = false;
            }
            return new ResultDto<Order>
            {
                State = state,
                Message = message,
            };
        }
        public ResultDto<Order> GetAll()
        {
            try
            {
                orders = _context.Orders.ToList();
                state = true;
                message = "Orders retrieved successfully.";
            }
            catch (Exception ex)
            {
                message = ex.Message;
                state = false;
            }
            return new ResultDto<Order>
            {
                State = state,
                Message = message,
                List = orders
            };
        }
        public ResultDto<Order> GetById(int id)
        {
            try
            {
                order = _context.Orders.Where(p => p.OrderId == id).FirstOrDefault();
                state = true;
                message = "Orders retrieved successfully.";
            }
            catch (Exception ex)
            {
                message = ex.Message;
                state = false;
            }
            return new ResultDto<Order>
            {
                State = state,
                Message = message,
                Data = order
            };
        }
        public ResultDto<Order> Remove(int id)
        {
            try
            {
                _context.Orders.RemoveRange(_context.Orders.Where(p => p.OrderId == id).ToList());
                _context.SaveChanges();
                state = true;
                message = "Order removed successfully.";
            }
            catch (Exception ex)
            {
                message = ex.Message;
                state = false;
            }
            return new ResultDto<Order>
            {
                State = state,
                Message = message,
            };

        }
        public ResultDto<Order> Update(Order data)
        {
            try
            {
                _context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.TrackAll;
                var upd = _context.Orders.Where(p => p.OrderId == data.OrderId).First();
                _context.SaveChanges();
                _context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
                state = true;
                message = "Order updated successfully.";
            }
            catch (Exception ex)
            {
                state = false;
                message = ex.Message;
            }
            return new ResultDto<Order>
            {
                State = state,
                Message = message
            };
        }
    }
}
