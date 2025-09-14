using B2b.Dal.Entity;
using B2b.Infrastructure.ResponseDto;

namespace B2b.Infrastructure.Service.BrandService
{
    public interface IBrandService
    {
        ResultDto<Brand> Add(Brand data);
        ResultDto<Brand> GetById(int id);
        ResultDto<Brand> Remove(int id);
        ResultDto<Brand> GetAll();
        ResultDto<Brand> Update(Brand data);
    }
}
