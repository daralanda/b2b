using B2b.Dal.Entity;
using B2b.Infrastructure.ResponseDto;

namespace B2b.Infrastructure.Service.CommerceService
{
    public interface ICommerceService
    {
        ResultDto<ProductDetailDto> GetProducts(int CustomerId);
        ResultDto<Category> GetCategories();
        ResultDto<ProductDetailsDto> GetProduct(int ProductId, int CustomerId);
        ResultDto<ProductImage> GetProductImage(int ProductId);
        ResultDto<ProductDetailDto> GetCampaignProducts(int CustomerId);
        ResultDto<ProductDetailDto> GetCampaignProductList(int CustomerId);
        ResultDto<ProductDetailDto> GetLatestProducts(int CustomerId);
        ResultDto<ProductDetailDto> GetProductInfo(int CustomerId, int ProductId);
        ResultDto<Order> GetAllOrder(int id);
        ResultDto<OrderDto> GetByOrderId(int id);

    }
}
