using Fanex.NAP.SPU.VR.Features.BetLimit;
using Fanex.NAP.SPU.VR.JobService.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Fanex.NAP.SPU.VR.JobService.Services
{
    public static class InitService
    {
        public static void RegisterService(this IServiceCollection services)
        {            
            RegisterBetLimitService(services);            
        }

        private static void RegisterBetLimitService(this IServiceCollection services)
        {
            services.AddSingleton<IBetLimitDataAccess, BetLimitDataAccess>();
            services.AddSingleton<IBetLimitServices, BetLimitServices>();
            services.AddSingleton<ICustomerBetLimitJobService, CustomerBetLimitJobService>();
        }        
    }
}
