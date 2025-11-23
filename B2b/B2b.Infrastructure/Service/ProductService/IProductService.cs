using B2b.Dal.Entity;
using B2b.Infrastructure.ResponseDto;

namespace B2b.Infrastructure.Service.ProductService
{
    public interface IProductService
    {
        ResultDto<Product> Add(Product data);
        ResultDto<Product> GetById(int id);
        ResultDto<Product> Remove(int id);
        ResultDto<Product> GetAll();
        ResultDto<Product> Update(Product data);
        ResultDto<ProductPrice> GetPrice(int id);
        ResultDto<ProductImage> GetImage(int id);
        ResultDto<string> ProductAllSet(Stream stream);
    }
}
