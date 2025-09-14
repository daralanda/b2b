using B2b.Dal.Entity;
using B2b.Infrastructure.ResponseDto;

namespace B2b.Infrastructure.Service.UnitTypeService
{
    public interface IUnitTypeService
    {
        ResultDto<UnitType> Add(UnitType data);
        ResultDto<UnitType> GetById(int id);
        ResultDto<UnitType> Remove(int id);
        ResultDto<UnitType> GetAll();
        ResultDto<UnitType> Update(UnitType data);
    }
}
