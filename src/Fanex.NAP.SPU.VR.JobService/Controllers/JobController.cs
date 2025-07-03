namespace Fanex.NAP.SPU.VR.JobService.Controllers
{
    using Fanex.Logging;
    using Fanex.NAP.SPU.VR.JobService.Services;
    using Microsoft.AspNetCore.Mvc;
    using Sentry;
    using System;
    using System.Threading.Tasks;

    [ApiController]
    [Route("api/job")]
    public class JobController : ControllerBase
    {        
        private readonly ICustomerBetLimitJobService customerBetLimitJobService;
        private readonly ISpecialLicenseeJobService specialLicenseeJobService;
        private readonly IAgentJobService agentJobService;
        private readonly IMemberBigStakeJobService memberBigStakeJobService;
        private readonly IAdjust3MJobService adjust3MJobService;
        private readonly ISapaSportDangerLevelJobService sapaSportDangerLevelJobService;

        public JobController(ICustomerBetLimitJobService customerBetLimitJobService,
            ISpecialLicenseeJobService specialLicenseeJobService,
            IAgentJobService agentJobService,
            IMemberBigStakeJobService memberBigStakeJobService,
            IAdjust3MJobService adjust3MJobService,
            ISapaSportDangerLevelJobService sapaSportDangerLevelJobService)
        {          
            this.customerBetLimitJobService = customerBetLimitJobService;
            this.specialLicenseeJobService = specialLicenseeJobService;
            this.agentJobService = agentJobService;
            this.memberBigStakeJobService = memberBigStakeJobService;
            this.adjust3MJobService = adjust3MJobService;
            this.sapaSportDangerLevelJobService = sapaSportDangerLevelJobService;
        }

        [HttpPost("betlimit")]
        public async Task<bool> BetLimit(string jobName)
        {
            LogService.Info($"{jobName} is running at {DateTime.Now}");

            return await this.customerBetLimitJobService.Execute();

        }

        [HttpPost("memberbigstake")]
        public async Task<bool> MemberBigStake(string jobName)
        {
            LogService.Info($"{jobName} is running at {DateTime.Now}");

            return await this.memberBigStakeJobService.Execute();
        }

        [HttpPost("spaciallicensee")]
        public async Task<bool> SpecialLicensee(string jobName)
        {
            LogService.Info($"{jobName} is running at {DateTime.Now}");

            return await this.specialLicenseeJobService.Execute();
        }

        [HttpPost("agent")]
        public async Task<bool> Agent(string jobName)
        {
            LogService.Info($"{jobName} is running at {DateTime.Now}");

            return await this.agentJobService.Execute();
        }

        [HttpPost("adjustvrsetting")]
        public async Task<bool>AdjustVRSetting (string jobName)
        {
            LogService.Info($"{jobName} is running at {DateTime.Now}");

            return await this.adjust3MJobService.Execute();

        }

        [HttpPost("WriteLog")]
        public void WriteLog(string message)
        {
            LogService.Error($"✅ {message} is test error at {DateTime.Now}");

            LogService.Info($"✅ {message} is test info at {DateTime.Now}");
        }       

        [HttpPost("SapaSportDangerLevel")]
        public async Task<bool> SapaSportDangerLevel([FromForm] int batchSize)
        {
            return await this.sapaSportDangerLevelJobService.Execute(batchSize);
        }
    }
}
