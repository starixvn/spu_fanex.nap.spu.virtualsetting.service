using Fanex.NAP.SPU.VR.JobService.Services;
using Microsoft.AspNetCore.Mvc;
using Sentry;
using System;
using System.Threading.Tasks;

namespace Fanex.NAP.SPU.DCS.JobService.Controllers
{
    [ApiController]
    [Route("api/job")]
    public class JobController : ControllerBase
    {
        private readonly ISentryClient sentryClient;
        private readonly ICustomerBetLimitJobService customerBetLimitJobService;       

        public JobController(ISentryClient sentryClient,
            ICustomerBetLimitJobService customerBetLimitJobService)
        {
            this.sentryClient = sentryClient;
            this.customerBetLimitJobService = customerBetLimitJobService;
            
        }

        [HttpPost("betlimit")]
        public async Task<bool> BetLimit(string jobName)
        {
            sentryClient.CaptureMessage($"{jobName} is running at {DateTime.Now}");

            return await this.customerBetLimitJobService.Execute();

        }

        [HttpPost("memberbigstake")]
        public async Task<bool> MemberBigStake(string jobName)
        {
            sentryClient.CaptureMessage($"{jobName} is running at {DateTime.Now}");

            //return await this.customerBetLimitJobService.Execute();

            return await Task.FromResult(true);

        }

        [HttpPost("spaciallicensee")]
        public async Task<bool> SpecialLicensee(string jobName)
        {
            sentryClient.CaptureMessage($"{jobName} is running at {DateTime.Now}");

            return await Task.FromResult(true);

        }

        [HttpPost("agent")]
        public async Task<bool> Agent(string jobName)
        {
            sentryClient.CaptureMessage($"{jobName} is running at {DateTime.Now}");

            return await Task.FromResult(true);

        }

        [HttpPost("adjustvrsetting")]
        public async Task<bool>AdjustVRSetting (string jobName)
        {
            sentryClient.CaptureMessage($"{jobName} is running at {DateTime.Now}");

            return await Task.FromResult(true);

        }

        [HttpPost("WriteLog")]
        public void WriteLog(string jobName)
        {
            sentryClient.CaptureMessage($"{jobName} is running at {DateTime.Now}");                    

        }
    }
}
