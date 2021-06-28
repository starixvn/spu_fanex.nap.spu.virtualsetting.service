namespace Fanex.NAP.SPU.VR.JobService.Services
{
    using Fanex.NAP.SPU.Notification.Services;
    using Fanex.NAP.SPU.VR.Features.BetLimit;
    using Sentry;
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    public interface ICustomerBetLimitJobService
    {        
        Task<bool> Execute();
    }

    public class CustomerBetLimitJobService: ICustomerBetLimitJobService
    {
        private readonly ISentryClient sentryClient;
        private readonly IAppSettings appSettings;
        private readonly IBetLimitServices betLimitService;
        private string notifiedMessage = string.Empty;
        
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
            if(!string.IsNullOrWhiteSpace(notifiedMessage))
            {
                var telegramService = new NotificationService();
                telegramService
                   .Config("0A40aOc7CtpzjbBSJu8KNWQRoXyjRvf9", "-1001400862084")
                   .Message(new MessageModel("VR - Auto Update Bet Limit", DateTime.Now, appSettings.EnvironmentString, notifiedMessage))
                   .SendWithoutDelay(isInfo: true);
            }            
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

                this.notifiedMessage = $"Auto update Bet Limit job run COMPLETED." +
                    $"\n\r - Affected customers: {betLimitService.PuntersUpdated}" +
                    $"\n\r - Raw customers: {betLimitService.RawCustomer}"+
                    $"\n\r - Passed customers: {betLimitService.UpdateCustomers}" +
                    $"\n\r - NOT Passed customers: {betLimitService.RemovedCustomers}";

            }
        }
    }
}
