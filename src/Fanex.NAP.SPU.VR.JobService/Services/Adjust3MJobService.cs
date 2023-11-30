namespace Fanex.NAP.SPU.VR.JobService.Services
{
    using Fanex.Logging;
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.Database;
    using Fanex.MailDelivery.Client;
    using Fanex.NAP.SPU.CTS.Shares;
    using Fanex.NAP.SPU.Notification.Services;
    using Fanex.NAP.SPU.VR.Features.Adjust3M;
    using Fanex.NAP.SPU.VR.Features.Adjust3M.Entities;
    using Fanex.NAP.SPU.VR.Features.Adjust3M.Model;
    using Sentry;
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    public interface IAdjust3MJobService
    {
        Task<bool> Execute();
    }

    public class Adjust3MJobService : IAdjust3MJobService
    {
        private const string VSMailInformTemplate = @"MailTemplate/Adjust3M/MailInformTemplate.cshtml";
        private readonly ISentryClient sentryClient;
        private readonly IAppSettings appSettings;
        private readonly ISoccerService soccerService;
        private readonly ISportsbookService sportsbookService;
        private DateTime stopTime;

        public Adjust3MJobService(ISentryClient sentryClient, IAppSettings appSettings, ISoccerService soccerService, ISportsbookService sportsbookService)
        {
            this.sentryClient = sentryClient;
            this.appSettings = appSettings;
            this.soccerService = soccerService;
            this.sportsbookService = sportsbookService;
        }

        public Task<bool> Execute()
        {           
            try
            {
                if (IsExecutedDate())                
                {
                    var startTime = DateTime.Now;

                    var puntersUpdatedSoccer = UpdateVRForSoccer();

                    var puntersUpdatedSportbooks = UpdateForSportbooks();

                    SendMail(appSettings.Adjust3MMailingListId
                        , startTime
                        , stopTime
                        , (puntersUpdatedSoccer + puntersUpdatedSportbooks));

                    var message = $"VR - Auto adjust 3M job run COMPLETED." +
                      $"\n\r - Members updated for soccer: {puntersUpdatedSoccer}" +
                      $"\n\r - Members updated for sportbooks: {puntersUpdatedSportbooks}";

                    LogService.Info(message);

                    NotifyMessage(message, true);
                }
                //else
                //{
                //    SendMailInternal(appSettings.MailingListIdToSPU);

                //    LogService.Info($"VR - Ajust 3M job run COMPLETED");
                //}
            }
            catch (Exception ex)
            {
                sentryClient.CaptureException(ex);

                var logMessage = new LogMessage(ex.Message)
                   .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                   .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                LogService.Error($"VR - Ajust 3M job Execute exception message:  {logMessage}");

                NotifyMessage($"VR - Ajust 3M job Execute exception message:  {ex.Message}", false);
            }

            return Task.FromResult(true);
        }

        private bool IsExecutedDate()
        {
            var currentDate = DateTime.Today;

            var adjust3MExecutedDate = this.appSettings.Adjust3MExecutedDate.Split(';').Select(x => Convert.ToInt32(x)).ToList();

            return adjust3MExecutedDate.Contains(currentDate.Day);
        }

        private int UpdateVRForSoccer()
        {
            var updateVirtualSettingEntity = new UpdateVirtualSettingEntity
            {
                DummyUser = appSettings.Adjust3MDummyUser,
                IP = appSettings.Adjust3MIP,
                PunterChunkSize = appSettings.Adjust3MPunterChunkSize
            };

            var countAffected = this.soccerService
                    .InitModel(appSettings.Adjust3MSiteIds
                            , appSettings.Adjust3MSuperIds
                            , DateTime.Now
                            , appSettings.Adjust3MVirtualRateThresholdForWinning)
                    .AutoAdjust(updateVirtualSettingEntity
                            , appSettings.Adjust3MNewVirtualMultiplierForSuperProPunter
                            , appSettings.Adjust3MNewVirtualMultiplierForWinning)
                    .GetCountAffected();

            stopTime = DateTime.Now;

            LogService.Info($"VR - Ajust 3M ExecuteForSoccer is called stop time {stopTime}");

            return countAffected;
        }

        public int UpdateForSportbooks()
        {
            var updateVirtualSettingEntity = new UpdateVirtualSettingEntity
            {
                DummyUser = appSettings.Adjust3MDummyUser,
                IP = appSettings.Adjust3MIP,
                PunterChunkSize = appSettings.Adjust3MPunterChunkSize
            };

            var countAffected = this.sportsbookService
                     .InitModel(appSettings.Adjust3MSiteIds
                             , appSettings.Adjust3MSuperIds
                             , DateTime.Now
                             , appSettings.Adjust3MVirtualRateThresholdForWinning)
                     .AutoAdjust(updateVirtualSettingEntity
                             , appSettings.Adjust3MNewVirtualMultiplierForWinning)
                     .GetCountAffected();

            stopTime = DateTime.Now;

            LogService.Info($"VR - Ajust 3M ExecuteForSportbooks is called stop time {stopTime}");

            return countAffected;
        }

        private void NotifyMessage(string message, bool isInfo)
        {                        
            Utilities.NotifyMessage(message, "VR - Adjust 3M", appSettings.EnvironmentString, "-1001400862084", isInfo);
        }

        private void SendMailInternal(int mailingListId)
        {
            try
            {
                var newMail = new Fanex.MailDelivery.Core.Models.Mail
                {
                    Subject = "VR Auto Mail Service - Still alive",
                    Title = "VR Auto Mail Service - Still alive",
                    Body = $"Is NOT the execution date for Updating Virtual Settings.",
                    MailingListId = mailingListId
                };                

                SendMail(newMail);
            }
            catch (Exception ex)
            {
                var logMessage = new LogMessage(ex.Message)
                  .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                  .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                LogService.Error($"VR - Ajust 3M job SendMailInternal exception message:  {logMessage}");

                NotifyMessage($"VR - Ajust 3M job SendMailInternal exception message:  {ex.Message}", false);

                throw;
            }
        }

        private void SendMail(int mailingListId, DateTime startTime, DateTime endTime, int puntersUpdated)
        {
            try
            {
                var mailTemplatePath = AppDomain.CurrentDomain.BaseDirectory + VSMailInformTemplate;

                var body = Utilities.ReadTextFile(mailTemplatePath);

                body = string.Format(body, startTime.ToString("MM/dd/yyyy hh:mm:ss"), endTime.ToString("MM/dd/yyyy hh:mm:ss"), puntersUpdated);

                var newMail = new Fanex.MailDelivery.Core.Models.Mail
                {
                    Subject = "VR Auto Adjustment finished" + " (" + this.appSettings.EnvironmentString + ")",
                    Title = "VR Auto Mail Service",
                    Body = body,
                    MailingListId = mailingListId
                };

                SendMail(newMail);                
            }
            catch (Exception ex)
            {
                var logMessage = new LogMessage(ex.Message)
                    .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                    .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                LogService.Error($"VR - Adjust 3M job SendMail exeption TotalAffectedMembers: {puntersUpdated} at start time { startTime } message:  {logMessage}");

                NotifyMessage($"VR - Adjust 3M job SendMail exeption TotalAffectedMembers: {puntersUpdated} at start time { startTime } message:  {ex.Message}", false);

                throw;
            }
        }

        private static void SendMail(Fanex.MailDelivery.Core.Models.Mail mail)
        {
            MailClientHandler.Start();

            MailClientHandler.SendFunctionMail(mail);

            MailClientHandler.Stop();
        }
        
    }
}
