using B2b.Dal.Context;
using B2b.Dal.Entity;
using B2b.Infrastructure.ResponseDto;
using Microsoft.EntityFrameworkCore;

namespace B2b.Infrastructure.Service.ProductService
{
    public class ProductService(B2bDbContext context):IProductService
    {
        private readonly B2bDbContext _context = context;
        bool state = false;
        string message = string.Empty;
        List<Product> products = new();
        Product product = new();
        public ResultDto<Product> Add(Product data)
        {
            try
            {
                _context.Products.Add(data);
                _context.SaveChanges();
                state = true;
                message = "Product added successfully.";
            }
            catch (Exception ex)
            {
                message = ex.Message;
                state = false;
            }
            return new ResultDto<Product>
            {
                State = state,
                Message = message,
            };
        }
        public ResultDto<Product> GetAll()
        {
            try
            {
                products = _context.Products.ToList();
                state = true;
                message = "Products retrieved successfully.";
            }
            catch (Exception ex)
            {
                state = false;
                message = ex.Message;
            }
            return new ResultDto<Product>
            {
                State = state,
                Message = message,
                List = products
            };
        }
        public ResultDto<Product> GetById(int id)
        {
            try
            {
                product = _context.Products.Where(p => p.ProductId == id).FirstOrDefault();
                state = true;
                message = "product retrieved successfully.";
            }
            catch (Exception ex)
            {
                state = false;
                message = ex.Message;
            }
            return new ResultDto<Product>
            {
                State = state,
                Message = message,
                Data = product
            };
        }
        public ResultDto<Product> Remove(int id)
        {

            try
            {
                var del = _context.Products.Where(p => p.ProductId == id).ToList();
                _context.Products.RemoveRange(del);
                _context.SaveChanges();
                state = true;
                message = "Products removed successfully.";
            }
            catch (Exception ex)
            {
                state = false;
                message = ex.Message;
            }
            return new ResultDto<Product>
            {
                State = state,
                Message = message,
            };
        }
        public ResultDto<Product> Update(Product data)
        {
            try
            {
                _context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.TrackAll;
                var upd = _context.Products.Where(p => p.ProductId == data.ProductId).First();
                upd.ProductName = data.ProductName;
                upd.CategoryId = data.CategoryId;
                upd.CurrencyId = data.CurrencyId;
                upd.Vat = data.Vat;
                upd.StockQuantity = data.StockQuantity;
                upd.ProductCode = data.ProductCode;
                upd.BrandId = data.BrandId;
                upd.Description = data.Description;
                upd.IsActive = data.IsActive;
                _context.SaveChanges();
                _context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
                var img=_context.ProductImages.Where(p => p.ProductId == data.ProductId).ToList();
                var price=_context.ProductPrices.Where(p => p.ProductId == data.ProductId).ToList();
                 _context.ProductImages.RemoveRange(img);
                _context.ProductPrices.RemoveRange(price);
               
                foreach (var item in data.ProductImages)
                {
                    _context.ProductImages.Add(new ProductImage
                    {
                         ImageUrl=item.ImageUrl,
                         ProductId=upd.ProductId,
                         Queue=item.Queue
                    });
                    _context.SaveChanges();
                }
                foreach (var item in data.ProductPrices)
                {
                    _context.ProductPrices.Add(new ProductPrice
                    {
                        Price = item.Price,
                        UnitTypeId = item.UnitTypeId,
                        ProductId = upd.ProductId,
                        IsDefault = item.IsDefault
                    });
                    _context.SaveChanges();
                }
                state = true;
                message = "Product updated successfully.";
            }
            catch (Exception ex)
            {
                state = false;
                message = ex.Message;
            }
            return new ResultDto<Product>
            {
                State = state,
                Message = message
            };
        }

        public ResultDto<ProductPrice> GetPrice(int id)
        {
            var result = new ResultDto<ProductPrice>();
            try
            {
                result.List = _context.ProductPrices.Where(p => p.ProductId == id).ToList();
                state = true;
                message = "product retrieved successfully.";
            }
            catch (Exception ex)
            {
                state = false;
                message = ex.Message;
            }
            return result;
        }
        public ResultDto<ProductImage> GetImage(int id)
        {
            var result = new ResultDto<ProductImage>();
            try
            {
                result.List = _context.ProductImages.Where(p => p.ProductId == id).ToList();
                state = true;
                message = "product retrieved successfully.";
            }
            catch (Exception ex)
            {
                state = false;
                message = ex.Message;
            }
            return result;
        }

    }
}
