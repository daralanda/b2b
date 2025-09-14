using B2b.Dal.Context;
using B2b.Dal.Entity;
using B2b.Infrastructure.ResponseDto;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace B2b.Infrastructure.Service.CategoryService
{
    public class CategoryService(B2bDbContext context):ICategoryService
    {
        private readonly B2bDbContext _context = context;
        bool state = false;
        string message = string.Empty;
        List<Category> categories = new();
        Category category = new();
        public ResultDto<Category> Add(Category data)
        {
            try
            {
                _context.Categories.Add(data);
                _context.SaveChanges();
                state = true;
                message = "Category added successfully.";
            }
            catch (Exception ex)
            {
                message = ex.Message;
                state = false;
            }
            return new ResultDto<Category>
            {
                State = state,
                Message = message,
            };
        }
        public ResultDto<Category> GetAll()
        {
            try
            {
                categories = _context.Categories.ToList();
                state = true;
                message = "Categories retrieved successfully.";
            }
            catch (Exception ex)
            {
                state = false;
                message = ex.Message;
            }
            return new ResultDto<Category>
            {
                State = state,
                Message = message,
                List = categories
            };
        }
        public ResultDto<Category> GetById(int id)
        {
            try
            {
                category = _context.Categories.Where(p => p.CategoryId == id).FirstOrDefault();
                state = true;
                message = "Banner retrieved successfully.";
            }
            catch (Exception ex)
            {
                state = false;
                message = ex.Message;
            }
            return new ResultDto<Category>
            {
                State = state,
                Message = message,
                Data = category
            };
        }
        public ResultDto<Category> Remove(int id)
        {
            try
            {
                var del = _context.Categories.Where(p => p.CategoryId == id).ToList();
                _context.Categories.RemoveRange(del);
                _context.SaveChanges();
                state = true;
                message = "Category removed successfully.";
            }
            catch (Exception ex)
            {
                state = false;
                message = ex.Message;
            }
            return new ResultDto<Category>
            {
                State = state,
                Message = message,
            };
        }
        public ResultDto<Category> Update(Category data)
        {
            try
            {
                _context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.TrackAll;
                var upd = _context.Categories.Where(p => p.CategoryId == data.CategoryId).First();
                upd.CategoryName = data.CategoryName;
                upd.MainCategoryId = data.MainCategoryId;
                upd.IsActive = data.IsActive;
                _context.SaveChanges();
                _context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
                state = true;
                message = "Category updated successfully.";
            }
            catch (Exception ex)
            {
                state = false;
                message = ex.Message;
            }
            return new ResultDto<Category>
            {
                State = state,
                Message = message
            };
        }
    }
}
