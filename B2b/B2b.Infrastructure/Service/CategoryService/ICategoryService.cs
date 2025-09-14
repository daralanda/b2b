using B2b.Dal.Entity;
using B2b.Infrastructure.ResponseDto;

namespace B2b.Infrastructure.Service.CategoryService
{
    public interface ICategoryService
    {
        ResultDto<Category> Add(Category data);
        ResultDto<Category> GetById(int id);
        ResultDto<Category> Remove(int id);
        ResultDto<Category> GetAll();
        ResultDto<Category> Update(Category data);
    }
}
