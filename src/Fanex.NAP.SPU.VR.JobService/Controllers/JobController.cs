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

        [HttpPost("autoadjustbetlimit")]
        public async Task<bool> AutoAdjustBetLimit(string jobName)
        {
            sentryClient.CaptureMessage($"{jobName} is running at {DateTime.Now}");

            return await this.customerBetLimitJobService.Execute();

        }        
    }
}
