using Fanex.NAP.SPU.VR.Features.BetLimit;
using Sentry;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Fanex.NAP.SPU.VR.JobService.Services
{
    public interface ICustomerBetLimitJobService
    {        
        Task<bool> Execute();
    }

    public class CustomerBetLimitJobService: ICustomerBetLimitJobService
    {
        private readonly ISentryClient sentryClient;
        private readonly IAppSettings appSettings;
        private readonly IBetLimitServices betLimitService;

        public CustomerBetLimitJobService(ISentryClient sentryClient, IAppSettings appSettings, IBetLimitServices betLimitService)
        {
            this.sentryClient = sentryClient;           
            this.appSettings = appSettings;
            this.betLimitService = betLimitService;
        }

        public Task<bool> Execute()
        {           
            try
            {
                this.StartTime();
                this.UpdateBetLimit();
                this.NotifyMessage();
            }
            catch (System.Exception ex)
            {
                sentryClient.CaptureException(ex);

                return Task.FromResult(false);
            }

            return Task.FromResult(true);
        }

        private void NotifyMessage()
        {
            //sent letstalk
        }

        private void StartTime()
        {
            this.sentryClient.CaptureMessage("Start executing for Auto Update Bet Limit.");
        }

        private void UpdateBetLimit()
        {
            var customerList = betLimitService.GetBetLimitCustomers();

            if(customerList != null && customerList.Any())
            {
                var customerListString = string.Join(",", customerList);

                var (removeBetLimitCustomers, updateBetLimitCustomer) = betLimitService.VerifyBetLimitCustomers(customerListString);

                betLimitService.RemoveBetLimit(removeBetLimitCustomers, appSettings.PunterChunkSize);
                betLimitService.UpdateBetLimit(updateBetLimitCustomer, appSettings.PunterChunkSize);
            }
        }
    }
}
