using B2b.Dal.Entity;
using B2b.Infrastructure.ResponseDto;

namespace B2b.Infrastructure.Service.CustomerService
{
    public interface ICustomerService
    {
        ResultDto<Customer> Add(Customer data);
        ResultDto<Customer> GetById(int id);
        ResultDto<Customer> Remove(int id);
        ResultDto<Customer> GetAll();
        ResultDto<Customer> Update(Customer data);
        ResultDto<City> GetAllCities();
        ResultDto<District> GetAllDistricts();
    }
}
