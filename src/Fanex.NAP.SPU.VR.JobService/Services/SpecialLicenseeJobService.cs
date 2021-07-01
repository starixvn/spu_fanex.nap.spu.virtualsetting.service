namespace Fanex.NAP.SPU.VR.JobService.Services
{
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
            try
            {
                this.specialLicenseeService.AutoAdjustForBasketball(this.appSettings.PunterChunkSize)
                             .AutoAdjustForSpecialSportsbook(this.appSettings.PunterChunkSize);                

                NotifyMessage();
            }
            catch (Exception ex)
            {
                sentryClient.CaptureException(ex);

                return Task.FromResult(false);
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
