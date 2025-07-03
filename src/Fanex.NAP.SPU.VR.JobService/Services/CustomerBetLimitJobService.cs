namespace Fanex.NAP.SPU.VR.JobService.Services
{
    using Fanex.Logging;
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.Database;
    using Fanex.NAP.SPU.CTS.Shares;
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
        private readonly IAppSettings appSettings;
        private readonly IBetLimitService betLimitService;
        private string notifiedMessage = string.Empty;
        
        public CustomerBetLimitJobService(IAppSettings appSettings, IBetLimitService betLimitService)
        {          
            this.appSettings = appSettings;
            this.betLimitService = betLimitService;
        }

        public Task<bool> Execute()
        {
            var countRetry = 0;

            while (countRetry < appSettings.RetryTime)
            {
                try
                {
                    this.StartTime();

                    this.UpdateBetLimit();

                    countRetry = appSettings.RetryTime + 1;
                 
                    LogService.Info(this.notifiedMessage);

                    this.NotifyMessage();
                }
                catch (System.Exception ex)
                {
                    var logMessage = new LogMessage(ex.Message)
                      .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                      .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                    LogService.Error($"VR - Bet Limit job Execute exception message:  {logMessage}");
                    
                    countRetry += 1;

                    if (countRetry == appSettings.RetryTime)
                    {
                        return Task.FromResult(false);
                    }
                }
            }           

            return Task.FromResult(true);
        }

        private void NotifyMessage()
        {
            if(!string.IsNullOrWhiteSpace(notifiedMessage))
            {                
                Utilities.NotifyMessage(notifiedMessage, "VR - Auto Update Bet Limit", appSettings.EnvironmentString, appSettings.LogGroupID);
            }            
        }

        private void StartTime()
        {
            LogService.Info("Start executing for Auto Update Bet Limit.");          
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

                this.notifiedMessage = $"✅ Auto update Bet Limit job run COMPLETED." +
                    $"\n\r - Affected customers: {betLimitService.PuntersUpdated}" +
                    $"\n\r - Raw customers: {betLimitService.RawCustomer}"+
                    $"\n\r - Passed customers: {betLimitService.UpdateCustomers}" +
                    $"\n\r - NOT Passed customers: {betLimitService.RemovedCustomers}";
            }
            else
            {
                this.notifiedMessage = $"✅ Auto update Bet Limit job run COMPLETED." +                    
                    $"\n\r - Raw customers: 0";                
            }
        }
    }
}
