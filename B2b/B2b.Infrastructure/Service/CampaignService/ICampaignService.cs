using B2b.Dal.Entity;
using B2b.Infrastructure.ResponseDto;

namespace B2b.Infrastructure.Service.CampaignService
{
    public interface ICampaignService
    {
        ResultDto<Campaign> Add(Campaign data);
        ResultDto<Campaign> GetById(int id);
        ResultDto<Campaign> Remove(int id);
        ResultDto<Campaign> GetAll();
        ResultDto<Campaign> Update(Campaign data);
    }
}
