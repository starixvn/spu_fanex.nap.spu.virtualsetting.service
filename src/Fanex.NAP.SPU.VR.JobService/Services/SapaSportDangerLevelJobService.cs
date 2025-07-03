namespace Fanex.NAP.SPU.VR.JobService.Services
{
    using Fanex.Logging;
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.Database;
    using Fanex.NAP.SPU.CTS.Shares;
    using Fanex.NAP.SPU.VR.Features.SabaSportDangerLevel;
    using System;
    using System.Threading.Tasks;

    public interface ISapaSportDangerLevelJobService
    {
        Task<bool> Execute(int batchSize);
    }

    public class SapaSportDangerLevelJobService : ISapaSportDangerLevelJobService
    {     
        private readonly IAppSettings appSettings;
        private readonly IDangerLevelSapaSportService service;
        private string notifiedMessage;

        public SapaSportDangerLevelJobService(IAppSettings appSettings, IDangerLevelSapaSportService service)
        {          
            this.appSettings = appSettings;
            this.service = service;
        }

        public Task<bool> Execute(int batchSize)
        {
            bool result;
            notifiedMessage = string.Empty;

            try
            {
                result = JobRun(batchSize);
            }
            catch (Exception ex)
            {
                var logMessage = new LogMessage(ex.Message)
                    .AddMessage($"VR - { appSettings.EnvironmentString} - Update Sapa Sport DangerLevel Job")
                    .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                    .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                LogService.Error($"{ logMessage } - DateTime: {DateTime.Now}");

                this.notifiedMessage = $"🆘 Update Sapa Sport Danger Level job FAIL." +
                  $"\n\r - {ex.Message}";                

                result = false;
            }

            NotifyMessage();

            return Task.FromResult(result);
        }

        private bool JobRun(int batchSize)
        {
            if (!service.IsDataSabaSportDangerLevelReady(appSettings.RetryTimesSapaSportDangerLevelForCheckingDataReady))
            {
                LogService.Error($"VR - {appSettings.EnvironmentString} - Update Sapa Sport Danger Level Job - Data for checking NOT READY - DateTime: {DateTime.Now}");

                this.notifiedMessage = $"🆘 Update Danger Level FAIL." +
                    $"\n\r - Checking data for SABASportsDangerLevel DO NOT READY";

                return true;
            }

            var result = service
                            .Start()
                            .GetSapaSportDangerLevel(appSettings.Environment)
                            .RetryUpdateDangerLevelToMainDB(appSettings.RetryTimesSapaSportDangerLevelForUpdateDangerLevel, batchSize, appSettings.Environment);

            if (result.ErrorCode != 0)
            {
                this.notifiedMessage = $"🆘 Update Danger Level FAIL." +
                    $"\n\r - Updating DangerLevel of SABASport to CustDB FAIL";
            }

            return result.ErrorCode == 0;
        }

        private void NotifyMessage()
        {
            if (!string.IsNullOrEmpty(notifiedMessage))
            {               
                Utilities.NotifyMessage(notifiedMessage, "VR - Auto Update Sapa Sport DangerLevel", appSettings.EnvironmentString, appSettings.LogGroupID);
            }
        }
    }
}