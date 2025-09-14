using B2b.Dal.Entity;
using B2b.Infrastructure.ResponseDto;

namespace B2b.Infrastructure.Service.BannerService
{
    public interface IBannerService
    {
        ResultDto<Banner> Add(Banner data);
        ResultDto<Banner> GetById(int id);
        ResultDto<Banner> Remove(int id);
        ResultDto<Banner> GetAll();
        ResultDto<Banner> Update(Banner data);
    }
}
