namespace Fanex.NAP.SPU.VR.JobService.Services
{
    using Fanex.Logging;
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.Database;
    using Fanex.NAP.SPU.Notification.Services;
    using Fanex.NAP.SPU.VR.Features.SpecialLicensee;
    using Sentry;
    using System; 
    using System.Threading.Tasks;

    public interface ISpecialLicenseeJobService
    {
        Task<bool> Execute();
    }

    public class SpecialLicenseeJobService : ISpecialLicenseeJobService
    {
        private readonly ISentryClient sentryClient;
        private readonly IAppSettings appSettings;
        private readonly ISpecialLicenseeService specialLicenseeService;
        private string notifiedMessage = string.Empty;

        public SpecialLicenseeJobService(ISentryClient sentryClient, IAppSettings appSettings, ISpecialLicenseeService specialLicenseeService)
        {
            this.sentryClient = sentryClient;
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
                    this.specialLicenseeService.AutoAdjustForBasketball(this.appSettings.PunterChunkSize)
                                 .AutoAdjustForSpecialSportsbook(this.appSettings.PunterChunkSize);

                    countRetry = appSettings.RetryTime + 1;

                    var message = $"VR - Special Licensee job run COMPLETED." +
                    $"\n\r - OtherSports - Customers have passed Performance Check: {this.specialLicenseeService.PunterBasketballUpdated}" +
                    $"\n\r - Special SportBook - Customers have passed Performance Check: {this.specialLicenseeService.PunterSportsbookUpdated}";

                    LogService.Info(message);

                    //NotifyMessage();
                }
                catch (Exception ex)
                {
                    sentryClient.CaptureException(ex);

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
            this.notifiedMessage = $"Auto special licensee job run COMPLETED." +
                 $"\n\r - OtherSports - Customers have passed Performance Check: {this.specialLicenseeService.PunterBasketballUpdated}" +
                 $"\n\r - Special SportBook - Customers have passed Performance Check: {this.specialLicenseeService.PunterSportsbookUpdated}";

            var telegramService = new NotificationService();
            telegramService
               .Config("0A40aOc7CtpzjbBSJu8KNWQRoXyjRvf9", "-1001400862084")
               .Message(new MessageModel("VR - Auto Update Special Licensee", DateTime.Now, appSettings.EnvironmentString, notifiedMessage))
               .SendWithoutDelay(isInfo: true);
        }

    }
}
