using B2b.Dal.Context;
using B2b.Dal.Entity;
using B2b.Infrastructure.ResponseDto;
using B2b.Plugin.EPPlus;
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

        public ResultDto<string> ProductAllSet(Stream stream)
        {
            var result = new ResultDto<string>();
            EPPLusPlugin ePPLusPlugin = new EPPLusPlugin();
            stream.Position = 0;
            var data= ePPLusPlugin.ExcelUpload(stream);
            int i = 1;
            List<Brand> brands = _context.Brands.ToList();
            List<Category> categories = _context.Categories.ToList();
            List<UnitType> unitTypes = _context.UnitTypes.ToList();    
            List<Currency> currencies = _context.Currencies.ToList();
            List<Product> Products = _context.Products.ToList();
            List<ProductPrice> productPrices = _context.ProductPrices.ToList();
            var list= new List<string>();
            foreach (var item in data)
            {

                if (item.Result.State!=false)
                {
                    var findProduct= Products.Where(p => p.ProductCode == item.ProductCode).FirstOrDefault();
                    var currency = currencies.Where(c => c.CurrencyCode == item.CurrencyName).FirstOrDefault();
                    if (currency == null)
                    {
                        _context.Currencies.Add(new Currency { CurrencyCode = item.CurrencyName , CurrencyName=item.CurrencyName});
                        _context.SaveChanges();
                        currencies = _context.Currencies.ToList();
                    }
                    var unittype = unitTypes.Where(c => c.UnitTypeName == item.UnitTypeName).FirstOrDefault();
                    if (unittype == null)
                    {
                        _context.UnitTypes.Add(new UnitType { UnitTypeName = item.UnitTypeName });
                        _context.SaveChanges();
                        unitTypes = _context.UnitTypes.ToList();
                    }
                    var category = categories.Where(c => c.CategoryName == item.CategoryName).FirstOrDefault();
                    if (category == null)
                    {
                        _context.Categories.Add(new Category { CategoryName = item.CategoryName, IsActive=true, MainCategoryId=0 });
                        _context.SaveChanges();
                        categories = _context.Categories.ToList();
                    }
                    var brand = brands.Where(c => c.BrandName == item.BrandName).FirstOrDefault();   
                    if (brand == null)
                    {
                        _context.Brands.Add(new Brand { BrandName = item.BrandName , Queno=1, ImageUrl=""});
                        _context.SaveChanges();
                        brands = _context.Brands.ToList();
                    }
                    if (findProduct==null)
                    {
                        Product product = new()
                        {
                            ProductCode = item.ProductCode,
                            ProductName = item.ProductName,
                            BrandId = brands.Where(c => c.BrandName == item.BrandName).FirstOrDefault().BrandId,
                            CategoryId= categories.Where(c => c.CategoryName == item.CategoryName).FirstOrDefault().CategoryId,
                            CurrencyId= currencies.Where(c=>c.CurrencyCode == item.CurrencyName).FirstOrDefault().CurrencyId,
                            Description = item.Description,
                            Vat = item.Vat,
                            IsActive = true,
                            StockQuantity= item.StockQuantity
                        };
                        _context.Products.Add(product);
                        _context.SaveChanges();
                        ProductPrice productPrice = new()
                        {
                            ProductId = product.ProductId,
                            Price = item.Price,
                            UnitTypeId = unitTypes.Where(c => c.UnitTypeName == item.UnitTypeName).FirstOrDefault().UnitTypeId,
                            IsDefault = item.IsDefault.ToLower() == "evet" ? true : false
                        };
                        _context.ProductPrices.Add(productPrice);
                        _context.SaveChanges();
                    }
                    else
                    {
                        ProductPrice productPrice = new()
                        {
                            ProductId = findProduct.ProductId,
                            Price = item.Price,
                            UnitTypeId = unitTypes.Where(c => c.UnitTypeName == item.UnitTypeName).FirstOrDefault().UnitTypeId,
                            IsDefault = item.IsDefault.ToLower() == "evet" ? true : false
                        };
                        var price = _context.ProductPrices.Where(p => p.ProductId == findProduct.ProductId && p.UnitTypeId == productPrice.UnitTypeId).FirstOrDefault();
                        if (price == null)
                        {
                            _context.ProductPrices.Add(productPrice);
                        }
                        else
                        {
                            price.Price = item.Price;
                        }
                        _context.SaveChanges();

                    }
                }
                else
                {
                    list.Add("Satır " + i + " : " + item.Result.Message);
                }
                    i++;
            }
            result.List = list;
            return result;
        }
    }
}
