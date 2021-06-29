namespace Fanex.NAP.SPU.VR.JobService.Services
{
    using Fanex.Logging;
    using Fanex.MailDelivery.Client;
    using Fanex.MailDelivery.Core.Models;
    using Fanex.NAP.SPU.CTS.Shares;
    using Fanex.NAP.SPU.Notification.Services;
    using Fanex.NAP.SPU.VR.Features.Agent;
    using Fanex.NAP.SPU.VR.Features.Agent.Model;
    using Sentry;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public interface IAgentJobService
    {
        Task<bool> Execute();
    }

    public class AgentJobService : IAgentJobService
    {
        private const string VSMailInformNoAgentAffectTemplate = @"MailTemplate\Agent\MailInformNoAgentAffectTemplate.cshtml";
        private const string VSMailInformTemplate = @"MailTemplate\Agent\MailInformTemplate.cshtml";
        private readonly ISentryClient sentryClient;
        private readonly IAppSettings appSettings;
        private readonly IAgentService agentService;           
        private string notifiedMessage = string.Empty;
        private IEnumerable<VRAgentUpdatedToZeroModel> updatedToZeroList;
        private IEnumerable<VRAgentUpdatedToFollowSuperModel> updatedToFollowSupperList;

        public AgentJobService(ISentryClient sentryClient, IAppSettings appSettings, IAgentService agentService)
        {
            this.sentryClient = sentryClient;
            this.appSettings = appSettings;
            this.agentService = agentService;
            this.updatedToZeroList = new List<VRAgentUpdatedToZeroModel>();
            this.updatedToFollowSupperList = new List<VRAgentUpdatedToFollowSuperModel>();
        }

        public Task<bool> Execute()
        {
            try
            {
                agentService.InsertNewAgentAdjust();

                var updateUnflagAbnormalAgents = agentService.VerifySportsAgentPerformance();

                agentService.UpdateFlagAbnormalAgent(1);

                UpdateUnflagAbnormalAgent(updateUnflagAbnormalAgents, 1);

                (this.updatedToZeroList, this.updatedToFollowSupperList) = agentService.GetVRAgentUpdated();

                SendMailExternal(this.appSettings.MailingListIdAgentAjust);

                NotifyMessage();
            }
            catch (System.Exception ex)
            {
                sentryClient.CaptureException(ex);

                return Task.FromResult(false);
            }

            return Task.FromResult(true);
        }

        private void UpdateUnflagAbnormalAgent(IEnumerable<int> customerList, int type)
        {
            var customerChunks = Utilities.ChunkList(customerList, this.appSettings.PunterChunkSize).ToList();

            for (int i = 0; i < customerChunks.Count(); i++)
            {
                agentService.UpdateUnflagAbnormalAgent(string.Join(",", customerChunks[i]), type);
            }
        }

        private void NotifyMessage()
        {
            if (!string.IsNullOrWhiteSpace(notifiedMessage))
            {
                var telegramService = new NotificationService();
                telegramService
                   .Config("0A40aOc7CtpzjbBSJu8KNWQRoXyjRvf9", "-1001400862084")
                   .Message(new MessageModel("VR - Auto Agent Ajust", DateTime.Now, appSettings.EnvironmentString, notifiedMessage))
                   .SendWithoutDelay(isInfo: true);
            }
        }

        private void SendMailExternal(int mailingListId)
        {
            var subject = "List of new agents got VR updated by Agent profiling job";
            var body = GenerateMailBody();

            try
            {
                if (!string.IsNullOrEmpty(body))
                {
                    var mail = new Mail
                    {
                        Subject = subject,
                        Title = subject,
                        Body = body,
                        MailingListId = mailingListId,
                        Attachments = new List<MailAttachment>()
                    };

                    SendMail(mail);
                }
            }
            catch (Exception ex)
            {
                var message = $"VR - Auto Agent Adjustment service - SendMailExternal exception: { ex.Message } with body content: { body }";

                LogService.Error(message, ex);
            }
        }

        private string GenerateMailBody()
        {
            var body = string.Empty;

            try
            {
                var isExistAgent = (updatedToZeroList != null && updatedToZeroList.Any())
                                || (updatedToFollowSupperList != null && updatedToFollowSupperList.Any());

                var mailTemplatePath = isExistAgent
                    ? AppDomain.CurrentDomain.BaseDirectory + VSMailInformTemplate
                    : AppDomain.CurrentDomain.BaseDirectory + VSMailInformNoAgentAffectTemplate;

                body = Utilities.ReadTextFile(mailTemplatePath);

                var html = string.Empty;

                html += GenerateTableUpdateToZero();
                html += GenerateTableUpdateToFollowSupper();

                body = string.Format(body, html);
            }
            catch (Exception ex)
            {
                var message = $"VR - Auto Agent Adjustment service - GenerateMailBody exception: { ex.Message }";

                LogService.Error(message, ex);
            }

            return body;
        }

        private string GenerateTableUpdateToZero()
        {
            var table = "<br />Table: agents whose VR got updated to 0 <br /><br />";

            if (this.updatedToZeroList != null && this.updatedToZeroList.Any())
            {
                table += "<table style='" + CssStyleTable(300) + "'>";
                table += "<tr>";
                table += "<td style='" + CssStyleTh() + "'>No</td>";
                table += "<td style='" + CssStyleTh() + "'>Site</td>";
                table += "<td style='" + CssStyleTh() + "'>Agent</td>";
                table += "</tr>";
                var index = 1;

                foreach (var item in this.updatedToZeroList)
                {
                    table += "<tr>";
                    table += "<td style='" + CssStyleTd() + "'>" + index + "</td>";
                    table += "<td style='" + CssStyleTd() + "'>" + item.Site + "</td>";
                    table += "<td style='" + CssStyleTd() + "'>" + item.Agent + "</td>";
                    table += "</tr>";

                    index++;
                }

                table += "</table></br>";
            }

            return table;
        }

        private string GenerateTableUpdateToFollowSupper()
        {
            var table = "<br />Table: agents whose VR got updated to follow Super <br /><br />";

            if (this.updatedToFollowSupperList != null && this.updatedToFollowSupperList.Any())
            {
                table += "<table style='" + CssStyleTable(400) + "'>";
                table += "<tr>";
                table += "<td style='" + CssStyleTh() + "'>No</td>";
                table += "<td style='" + CssStyleTh() + "'>Site</td>";
                table += "<td style='" + CssStyleTh() + "'>Agent</td>";
                table += "<td style='" + CssStyleTh() + "'>Member's Winloss</td>";
                table += "</tr>";
                var index = 1;

                foreach (var item in this.updatedToFollowSupperList)
                {
                    table += "<tr>";
                    table += "<td style='" + CssStyleTd() + "'>" + index + "</td>";
                    table += "<td style='" + CssStyleTd() + "'>" + item.Site + "</td>";
                    table += "<td style='" + CssStyleTd() + "'>" + item.Agent + "</td>";
                    table += "<td style='" + CssStyleTd() + "'>" + item.MemberWinloss.ToString("0") + "RM" + "</td>";
                    table += "</tr>";

                    index++;
                }

                table += "</table>";
            }

            return table;
        }

        private string CssStyleTable(int width)
        {
            var css = "border-collapse: collapse;";
            css += "width: " + width + "px";

            return css;
        }

        private string CssStyleTh()
        {
            var css = "border: 1px solid black;";
            css += "font-weight:bold;";

            return css;
        }

        private string CssStyleTd()
        {
            var css = "border: 1px solid black";

            return css;
        }

        private void SendMail(Mail mail)
        {
            MailClientHandler.Start();

            MailClientHandler.SendFunctionMail(mail);

            MailClientHandler.Stop();
        }
    }
}
