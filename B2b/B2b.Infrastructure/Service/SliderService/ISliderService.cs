using B2b.Dal.Entity;
using B2b.Infrastructure.ResponseDto;

namespace B2b.Infrastructure.Service.SliderService
{
    public interface ISliderService
    {
        ResultDto<Slider> Add(Slider data);
        ResultDto<Slider> GetById(int id);
        ResultDto<Slider> Remove(int id);
        ResultDto<Slider> GetAll();
        ResultDto<Slider> Update(Slider data);
    }
}
