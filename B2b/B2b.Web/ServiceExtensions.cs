using B2b.Dal.Context;
using B2b.Infrastructure.Service;
using B2b.Infrastructure.Service.AccountService;
using B2b.Infrastructure.Service.Authorization;
using B2b.Infrastructure.Service.BannerService;
using B2b.Infrastructure.Service.BrandService;
using B2b.Infrastructure.Service.CampaignService;
using B2b.Infrastructure.Service.CartService;
using B2b.Infrastructure.Service.CategoryService;
using B2b.Infrastructure.Service.CommerceService;
using B2b.Infrastructure.Service.CurrencyService;
using B2b.Infrastructure.Service.ExchangeService;
using B2b.Infrastructure.Service.MainService;
using B2b.Infrastructure.Service.OrderService;
using B2b.Infrastructure.Service.ProductService;
using B2b.Infrastructure.Service.RoleService;
using B2b.Infrastructure.Service.SliderService;
using B2b.Infrastructure.Service.UnitTypeService;
using B2b.Infrastructure.Service.UserService;
namespace B2b.Web
{
    public static class ServiceExtensions
    {
        static IServiceCollection _services;
        static IConfiguration _configuration;
        static ServiceProvider _serviceProvider;
        public static void ServiceRegisterAll(this IServiceCollection services, IConfiguration configuration)
        {
            _services = services;
            _configuration = configuration;
            _serviceProvider = services.BuildServiceProvider();
            JWTServiceRegister();
            MainServiceRegister();
            RoleServiceRegister();
            UserServiceRegister();
            BannerServiceRegister();
            BrandServiceRegister();
            CategoryServiceRegister();
            AccountServiceRegister();
            ProductServiceRegister();
            OrderServiceRegister();
            UnitTypeServiceRegister();
            CurrencyServiceRegister();
            SliderServiceRegister();
            CommerceServiceRegister();
            CartServiceRegister();
            ExchangeServiceRegister();
            CampaignServiceRegister();
        }
        private static void JWTServiceRegister()
        {
            _services.AddScoped<IJwtUtils, JwtUtils>(p => { return new JwtUtils(_configuration["Jwt:SecretKey"], _serviceProvider.GetService<B2bDbContext>()); });
        }
        private static void MainServiceRegister()
        {
            _services.AddScoped<IMainService, MainService>(p => { return new MainService(_serviceProvider.GetService<B2bDbContext>()); });
        }
        private static void RoleServiceRegister()
        {
            _services.AddScoped<IRoleService, RoleService>(p => { return new RoleService(_serviceProvider.GetService<B2bDbContext>()); });
        }
        private static void UserServiceRegister()
        {
            _services.AddScoped<IUserService, UserService>(p => { return new UserService(_serviceProvider.GetService<B2bDbContext>()); });
        }
        private static void BannerServiceRegister()
        {
            _services.AddScoped<IBannerService, BannerService>(p => { return new BannerService(_serviceProvider.GetService<B2bDbContext>()); });
        }
        private static void BrandServiceRegister()
        {
            _services.AddScoped<IBrandService, BrandService>(p => { return new BrandService(_serviceProvider.GetService<B2bDbContext>()); });
        }
        private static void CategoryServiceRegister()
        {
            _services.AddScoped<ICategoryService, CategoryService>(p => { return new CategoryService(_serviceProvider.GetService<B2bDbContext>()); });
        }
        private static void AccountServiceRegister()
        {
            _services.AddScoped<IAccountService, AccountService>(p => { return new AccountService(_serviceProvider.GetService<B2bDbContext>()); });
        }
        private static void ProductServiceRegister()
        {
            _services.AddScoped<IProductService, ProductService>(p => { return new ProductService(_serviceProvider.GetService<B2bDbContext>()); });
        }
        private static void OrderServiceRegister()
        {
            _services.AddScoped<IOrderService, OrderService>(p => { return new OrderService(_serviceProvider.GetService<B2bDbContext>()); });
        }
        private static void UnitTypeServiceRegister()
        {
            _services.AddScoped<IUnitTypeService, UnitTypeService>(p => { return new UnitTypeService(_serviceProvider.GetService<B2bDbContext>()); });
        }
        private static void CurrencyServiceRegister()
        {
            _services.AddScoped<ICurrencyService, CurrencyService>(p => { return new CurrencyService(_serviceProvider.GetService<B2bDbContext>()); });
        }
        private static void SliderServiceRegister()
        {
            _services.AddScoped<ISliderService, SliderService>(p => { return new SliderService(_serviceProvider.GetService<B2bDbContext>()); });
        }
       
        private static void CommerceServiceRegister()
        {
            _services.AddScoped<ICommerceService, CommerceService>(p => { return new CommerceService(_serviceProvider.GetService<B2bDbContext>()); });
        }
        private static void CartServiceRegister()
        {
            _services.AddScoped<ICartService, CartService>(p => { return new CartService(_serviceProvider.GetService<B2bDbContext>()); });
        }
        private static void ExchangeServiceRegister()
        {
            _services.AddScoped<IExchangeService, ExchangeService>(p => { return new ExchangeService(_serviceProvider.GetService<B2bDbContext>()); });
        }
        private static void CampaignServiceRegister()
        {
            _services.AddScoped<ICampaignService, CampaignService>(p => { return new CampaignService(_serviceProvider.GetService<B2bDbContext>()); });
        }
    }
}
