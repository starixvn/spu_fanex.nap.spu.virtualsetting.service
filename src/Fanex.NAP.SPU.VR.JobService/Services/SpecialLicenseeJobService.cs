namespace Fanex.NAP.SPU.VR.JobService.Services
{
    using Fanex.Logging;
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.Database;
    using Fanex.NAP.SPU.CTS.Shares;
    using Fanex.NAP.SPU.VR.Features.Notification;
    using Fanex.NAP.SPU.VR.Features.SpecialLicensee;
    using System; 
    using System.Threading.Tasks;

    public interface ISpecialLicenseeJobService
    {
        Task<bool> Execute();
    }

    public class SpecialLicenseeJobService : ISpecialLicenseeJobService
    {   
        private readonly IAppSettings appSettings;
        private readonly ISpecialLicenseeService specialLicenseeService;

        public SpecialLicenseeJobService(IAppSettings appSettings, ISpecialLicenseeService specialLicenseeService)
        {           
            this.appSettings = appSettings;
            this.specialLicenseeService = specialLicenseeService;
        }

        public Task<bool> Execute()
        {
            var countRetry = 0;

            while (countRetry < appSettings.RetryTime)
            {
                try
                {
                    this.specialLicenseeService.AutoAdjustForBasketball(this.appSettings.PunterChunkSize, appSettings.IsTest)
                                 .AutoAdjustForSpecialSportsbook(this.appSettings.PunterChunkSize, appSettings.IsTest);

                    countRetry = appSettings.RetryTime + 1;

                    var message = $"VR - Special Licensee job run COMPLETED." +
                    $"\n\r - OtherSports - Customers have passed Performance Check: {this.specialLicenseeService.PunterBasketballUpdated}" +
                    $"\n\r - Special SportBook - Customers have passed Performance Check: {this.specialLicenseeService.PunterSportsbookUpdated}";

                    LogService.Info(message);

                    NotifyMessage();
                }
                catch (Exception ex)
                {                    
                    var logMessage = new LogMessage(ex.Message)
                     .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                     .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                    LogService.Error($"VR - Special Licensee job Execute exception: message:  {logMessage}");

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
            var notifiedMessage = $"✅ Auto special licensee job run COMPLETED." +
                 $"\n\r - OtherSports - Customers have passed Performance Check: {this.specialLicenseeService.PunterBasketballUpdated}" +
                 $"\n\r - Special SportBook - Customers have passed Performance Check: {this.specialLicenseeService.PunterSportsbookUpdated}";
          
            var notificationService = new NotificationService(appSettings.EnvironmentString);

            notificationService
               .Config(appSettings.LogGroupID)
               .Message(new MessageModel("VR - Auto Update Special Licensee", DateTimeOffset.Now, appSettings.EnvironmentString, notifiedMessage))
               .SendWithoutDelay(true);
        }
    }
}
