namespace Fanex.NAP.SPU.VR.JobService.Services
{
    using Fanex.Logging;
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.Database;
    using Fanex.NAP.SPU.Notification.Services;
    using Fanex.NAP.SPU.VR.Features.SabaSportDangerLevel;
    using Newtonsoft.Json;
    using Sentry;
    using System;
    using System.Threading.Tasks;

    public interface ISapaSportDangerLevelJobService
    {
        Task<bool> Execute(int batchSize);
    }

    public class SapaSportDangerLevelJobService : ISapaSportDangerLevelJobService
    {
        private readonly ISentryClient sentryClient;
        private readonly IAppSettings appSettings;
        private readonly IDangerLevelSapaSportService service;
        private string notifiedMessage = string.Empty;

        public SapaSportDangerLevelJobService(ISentryClient sentryClient, IAppSettings appSettings, IDangerLevelSapaSportService service)
        {
            this.sentryClient = sentryClient;
            this.appSettings = appSettings;
            this.service = service;
        }

        public Task<bool> Execute(int batchSize)
        {
            var result = false;

            try
            {
                result = JobRun(batchSize);
            }
            catch (Exception ex)
            {
                var logMessage = new LogMessage(ex.Message)
                    .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                    .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                this.notifiedMessage = $"Update Sapa Sport Danger Level job FAIL." +
                  $"\n\r - {ex.Message}";

                sentryClient.CaptureMessage(logMessage, Sentry.Protocol.SentryLevel.Error);
            }

            NotifyMessage();

            return Task.FromResult(result);
        }

        private bool JobRun(int batchSize)
        {
            if (!service.IsDataSabaSportDangerLevelReady(appSettings.RetryTimesSapaSportDangerLevelForCheckingDataReady))
            {
                LogService.Error($"VR - Update Sapa Sport Danger Level Job - Data for checking NOT READY - DateTime: {DateTime.Now}");

                this.notifiedMessage = $"Update Sapa Sport Danger Level job FAIL." +
                    $"\n\r - Checking data for SABASportsDangerLevel DO NOT READY";

                return false;
            }

            var result = service
                            .Start()
                            .GetSapaSportDangerLevel(appSettings.Environment)
                            .RetryUpdateDangerLevelToMainDB(appSettings.RetryTimesSapaSportDangerLevelForUpdateDangerLevel, batchSize);

            if (result.ErrorCode != 0)
            {
                var logMessage = new LogMessage("VR - Update Sapa Sport Danger Level Job")
                                            .AddMessage($"Inner Exception: {result.ErrorMsg}")
                                            .AddMessage($"DateTime: {DateTime.Now}")
                                            .AddMessage($"Update DangerLevel To CustDB FAIL: {JsonConvert.SerializeObject(result.InPutParam)}").ToString();

                this.notifiedMessage = $"Update Sapa Sport Danger Level job FAIL." +
                    $"\n\r - Updating DangerLevel of SABASport to CustDB FAIL";

                sentryClient.CaptureMessage(logMessage, Sentry.Protocol.SentryLevel.Error);
            }

            return result.ErrorCode == 0;
        }

        private void NotifyMessage()
        {
            if (!string.IsNullOrWhiteSpace(notifiedMessage))
            {
                var telegramService = new NotificationService();
                telegramService
                   .Config("0A40aOc7CtpzjbBSJu8KNWQRoXyjRvf9", "-1001400862084")
                   .Message(new MessageModel("VR - Auto Update Bet Limit", DateTime.Now, appSettings.EnvironmentString, notifiedMessage))
                   .SendWithoutDelay(isInfo: true);
            }
        }
    }
}