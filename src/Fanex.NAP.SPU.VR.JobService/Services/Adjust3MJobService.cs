namespace Fanex.NAP.SPU.VR.JobService.Services
{
    using Fanex.Logging;
    using Fanex.MailDelivery.Client;
    using Fanex.NAP.SPU.CTS.Shares;
    using Fanex.NAP.SPU.Notification.Services;
    using Fanex.NAP.SPU.VR.Features.Adjust3M;
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
        private const string VSMailInformTemplate = @"MailTemplate\Adjust3M\MailInformTemplate.cshtml";
        private readonly ISentryClient sentryClient;
        private readonly IAppSettings appSettings;
        private readonly IAdjust3MService adjust3MService;
        private string notifiedMessage = string.Empty;        

        public Adjust3MJobService(ISentryClient sentryClient, IAppSettings appSettings, IAdjust3MService adjust3MService)
        {
            this.sentryClient = sentryClient;
            this.appSettings = appSettings;
            this.adjust3MService = adjust3MService;
        }

        public Task<bool> Execute()
        {
            try
            {
                var startTime = DateTime.Now;

                this.adjust3MService.ResetPunter(true)
                    .GetWinningPunterSoccerList(this.appSettings.Adjust3MSiteIds, this.appSettings.Adjust3MSuperIds, this.appSettings.Adjust3MVirtualRateThresholdForWinning)
                    .FilterPunterBaseOnCurrentVirtualRate(1, false)
                    .AdjustVirtualSettingForSuperProPunter(this.appSettings.Adjust3MDummyUser, this.appSettings.Adjust3MIP, this.appSettings.Adjust3MPunterChunkSize, this.appSettings.Adjust3MNewVirtualMultiplierForSuperProPunter)
                    .AdjustVirtualSettingForNormal(this.appSettings.Adjust3MDummyUser, this.appSettings.Adjust3MIP, this.appSettings.Adjust3MPunterChunkSize, this.appSettings.Adjust3MNewVirtualMultiplierForWinning);

                var puntersUpdatedSoccer = this.adjust3MService.CountPunterAffected();

                this.adjust3MService.ResetPunter(false)
                    .GetWinningPunterList(this.appSettings.Adjust3MSiteIds, this.appSettings.Adjust3MSuperIds, this.appSettings.Adjust3MVirtualRateThresholdForWinning)
                    .FilterPunterBaseOnCurrentVirtualRate(0, true)
                    .AdjustVirtualSettingForNormal(this.appSettings.Adjust3MDummyUser, this.appSettings.Adjust3MIP, this.appSettings.Adjust3MPunterChunkSize, this.appSettings.Adjust3MNewVirtualMultiplierForWinning);                

                var puntersUpdatedSportbooks = this.adjust3MService.CountPunterAffected();

                var endTime = DateTime.Now;

                SendMail(this.appSettings.Adjust3MMailingListId, startTime, endTime, puntersUpdatedSoccer + puntersUpdatedSportbooks);

                NotifyMessage(puntersUpdatedSoccer, puntersUpdatedSportbooks);
            }
            catch (Exception ex)
            {
                sentryClient.CaptureException(ex);

                return Task.FromResult(false);
            }

            return Task.FromResult(true);
        }

        private void NotifyMessage(int puntersUpdatedSoccer, int puntersUpdatedSportbooks)
        {
            this.notifiedMessage = $"Auto adjust 3M job run COMPLETED." +
                  $"\n\r - Members updated for soccer: {puntersUpdatedSoccer}" +
                  $"\n\r - Members updated for sportbooks: {puntersUpdatedSportbooks}";

            var telegramService = new NotificationService();
            telegramService
               .Config("0A40aOc7CtpzjbBSJu8KNWQRoXyjRvf9", "-1001400862084")
               .Message(new MessageModel("VR - Adjust 3M", DateTime.Now, appSettings.EnvironmentString, notifiedMessage))
               .SendWithoutDelay(isInfo: true);
        }

        private void SendMail(int mailingListId, DateTime startTime, DateTime endTime, int puntersUpdated)
        {
            var environment = this.appSettings.EnvironmentString;

            try
            {
                var mailTemplatePath = AppDomain.CurrentDomain.BaseDirectory + VSMailInformTemplate;

                var body = Utilities.ReadTextFile(mailTemplatePath);

                body = string.Format(body, startTime.ToString("MM/dd/yyyy hh:mm:ss"), endTime.ToString("MM/dd/yyyy hh:mm:ss"), puntersUpdated);

                var newMail = new Fanex.MailDelivery.Core.Models.Mail
                {
                    Subject = "VR Auto Adjustment finished" + " (" + environment + ")",
                    Title = "VR Auto Mail Service",
                    Body = body,
                    MailingListId = mailingListId
                };

                LogService.Info($"Virtual Setting service ({ environment }) - SendMailExternal start TotalAffectedMembers: {puntersUpdated} at start time { startTime }");

                SendMail(newMail);

                LogService.Info($"Virtual Setting service ({ environment }) - SendMailExternal end");
            }
            catch (Exception ex)
            {
                LogService.Info($"Virtual Setting service ({ environment }) - SendMailExternal exception TotalAffectedMembers: {puntersUpdated} at start time { startTime }", ex);
            }
        }

        private void SendMail(Fanex.MailDelivery.Core.Models.Mail mail)
        {
            MailClientHandler.Start();

            MailClientHandler.SendFunctionMail(mail);

            MailClientHandler.Stop();
        }
    }
}
