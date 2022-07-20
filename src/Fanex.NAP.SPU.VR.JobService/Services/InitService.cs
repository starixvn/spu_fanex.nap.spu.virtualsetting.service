namespace Fanex.NAP.SPU.VR.JobService.Services
{
    using Fanex.NAP.SPU.VR.Features.Adjust3M;
    using Fanex.NAP.SPU.VR.Features.Agent;
    using Fanex.NAP.SPU.VR.Features.BetLimit;
    using Fanex.NAP.SPU.VR.Features.MemberBigStake;
    using Fanex.NAP.SPU.VR.Features.SabaSportDangerLevel;
    using Fanex.NAP.SPU.VR.Features.SpecialLicensee;
    using Microsoft.Extensions.DependencyInjection;

    public static class InitService
    {
        public static void RegisterService(this IServiceCollection services)
        {            
            RegisterBetLimitService(services);

            RegisterAdjust3MService(services);

            RegisterAgentService(services);

            RegisterMemberBigStakeService(services);

            RegisterSpecialLicenseeService(services);

            RegisterSapsSportDangerLevelService(services);
        }

        private static void RegisterBetLimitService(this IServiceCollection services)
        {
            services.AddSingleton<IBetLimitDataAccess, BetLimitDataAccess>();
            services.AddSingleton<IBetLimitService, BetLimitService>();
            services.AddSingleton<ICustomerBetLimitJobService, CustomerBetLimitJobService>();
        }        

        private static void RegisterAdjust3MService(this IServiceCollection services)
        {
            services.AddSingleton<IAdjust3MDataAccess, Adjust3MDataAccess>();
            services.AddSingleton<ISoccerService, SoccerService>();
            services.AddSingleton<ISportsbookService, SportsbookService>();
            services.AddSingleton<IAdjust3MJobService, Adjust3MJobService>();
        }

        private static void RegisterAgentService(this IServiceCollection services)
        {
            services.AddSingleton<IAgentDataAccess, AgentDataAccess>();
            services.AddSingleton<IAgentService, AgentService>();
            services.AddSingleton<IAgentJobService, AgentJobService>();
        }

        private static void RegisterMemberBigStakeService(this IServiceCollection services)
        {
            services.AddSingleton<IMemberBigStakeDataAccess, MemberBigStakeDataAccess>();
            services.AddSingleton<IMemberBigStakeService, MemberBigStakeService>();
            services.AddSingleton<IMemberBigStakeJobService, MemberBigStakeJobService>();
        }

        private static void RegisterSpecialLicenseeService(this IServiceCollection services)
        {
            services.AddSingleton<ISpecialLicenseeDataAccess, SpecialLicenseeDataAccess>();
            services.AddSingleton<ISpecialLicenseeService, SpecialLicenseeService>();            
            services.AddSingleton<ISpecialLicenseeJobService, SpecialLicenseeJobService>();
        }

        private static void RegisterSapsSportDangerLevelService(this IServiceCollection services)
        {
            services.AddSingleton<IDangerLevelSapaSportDataAccess, DangerLevelSapaSportDataAccess>();
            services.AddSingleton<IDangerLevelSabaSportService, DangerLevelSapaSportService>();
            services.AddSingleton<ISabaSportDangerLevelJobService, SabaSportDangerLevelJobService>();
        }
    }
}
