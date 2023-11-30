namespace Fanex.NAP.SPU.VR.JobService.Services
{
    using Fanex.Logging;
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.Database;
    using Fanex.MailDelivery.Client;
    using Fanex.MailDelivery.Core.Models;
    using Fanex.NAP.SPU.CTS.Shares;
    using Fanex.NAP.SPU.Notification.Services;
    using Fanex.NAP.SPU.VR.Features.Agent;
    using Fanex.NAP.SPU.VR.Features.Agent.Entities;
    using Fanex.NAP.SPU.VR.Features.Agent.Model;
    using Newtonsoft.Json;
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
        private const string VSMailInformNoAgentAffectTemplate = @"MailTemplate/Agent/MailInformNoAgentAffectTemplate.cshtml";
        private const string VSMailInformTemplate = @"MailTemplate/Agent/MailInformTemplate.cshtml";
        private readonly ISentryClient sentryClient;
        private readonly IAppSettings appSettings;
        private readonly IAgentService agentService;           
        private string notifiedMessage = string.Empty;       

        public AgentJobService(ISentryClient sentryClient, IAppSettings appSettings, IAgentService agentService)
        {
            this.sentryClient = sentryClient;
            this.appSettings = appSettings;
            this.agentService = agentService;           
        }

        public Task<bool> Execute()
        {
            var countRetry = 0;           

            while (countRetry < appSettings.RetryTime)
            {
                try
                {
                    agentService.InsertNewAgentAdjust(appSettings.IsTest);

                    var updateUnflagAbnormalAgents = agentService.VerifySportsAgentPerformance(appSettings.IsTest);

                    agentService.UpdateFlagAbnormalAgent(1, appSettings.IsTest);

                    UpdateUnflagAbnormalAgent(updateUnflagAbnormalAgents, 1);

                    SendMail(this.appSettings.MailingListIdAgentAjust);

                    LogService.Info($"VR - Agent job Execute COMPLETED");

                    countRetry = appSettings.RetryTime + 1;                    

                    NotifyMessage($"VR - Agent job Execute COMPLETED", true);
                }
                catch (Exception ex)
                {
                    var logMessage = new LogMessage(ex.Message)
                       .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                       .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                    LogService.Error($"VR - Agent job Execute exception message:  {logMessage}");

                    NotifyMessage($"VR - Agent job Execute exception message:  {ex.Message}", false);

                    sentryClient.CaptureException(ex);

                    countRetry += 1;

                    if (countRetry == appSettings.RetryTime)
                    {
                        return Task.FromResult(false);
                    }
                }
            }            

            return Task.FromResult(true);
        }

        private void UpdateUnflagAbnormalAgent(IEnumerable<UnflagAbnormalAgentEntity> customerList, int type)
        {
            var customerChunks = Utilities.ChunkList(customerList, this.appSettings.PunterChunkSize).ToList();
            var isTest = appSettings.IsTest;

            for (int i = 0; i < customerChunks.Count(); i++)
            {
                var custList = customerChunks[i].Select(x => new UnflagAbnormalAgentModel
                {
                    CustID = x.AgentID,
                    TotalWL = x.MemberWinloss
                });

                var custXml = string.Empty;

                foreach (var item in custList)
                {
                    object[] objParams =
                    {
                        item.CustID,
                        item.TotalWL
                    };

                    custXml += Utilities.GenerateXmlForAbnormalAgentUnflag(objParams);
                }

                custXml = Utilities.GenerateXmlForVRRoot(custXml);                

                agentService.UpdateUnflagAbnormalAgent(custXml, type, isTest);
            }

            var customerListString = string.Join(",", customerList.Select(x => x.AgentID));

            LogService.Info("VR - Agent - List Cust Update Unflag Abnormal Agent: " + customerListString);
        }

        private void NotifyMessage(string message, bool isInfo)
        {
            if (!string.IsNullOrWhiteSpace(message))
            {               
                Utilities.NotifyMessage(notifiedMessage, "VR - Auto Agent Ajust", appSettings.EnvironmentString, "-1001400862084", isInfo);
            }
        }

        private void SendMail(int mailingListId)
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

                NotifyMessage($"VR - Auto Agent Adjustment service - SendMailExternal exception:  {ex.Message}", false);
            }
        }

        private string GenerateMailBody()
        {
            var body = string.Empty;

            try
            {
                var (updatedToZeroList, updatedToFollowSupperList) = agentService.GetVRAgentUpdated();

                var isExistAgent = (updatedToZeroList != null && updatedToZeroList.Any())
                                || (updatedToFollowSupperList != null && updatedToFollowSupperList.Any());

                var mailTemplatePath = isExistAgent
                    ? AppDomain.CurrentDomain.BaseDirectory + VSMailInformTemplate
                    : AppDomain.CurrentDomain.BaseDirectory + VSMailInformNoAgentAffectTemplate;

                body = Utilities.ReadTextFile(mailTemplatePath);

                var html = string.Empty;

                html += GenerateTableUpdateToZero(updatedToZeroList);
                html += GenerateTableUpdateToFollowSupper(updatedToFollowSupperList);

                body = string.Format(body, html);

                this.notifiedMessage = $"Auto update member big stake job run COMPLETED." +
                   $"\n\r - Member updated to zero: {updatedToZeroList.Count()}" +
                   $"\n\r - Member updated to follow supper: {updatedToFollowSupperList.Count()}";
            }
            catch (Exception ex)
            {
                var logMessage = new LogMessage(ex.Message)
                    .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                    .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();           

                LogService.Error($"VR - Auto Agent Adjustment service - GenerateMailBody exception: message:  {logMessage}");

                NotifyMessage($"VR - Auto Agent Adjustment service - GenerateMailBody exception: message:  {ex.Message}", false);
            }

            return body;
        }

        private string GenerateTableUpdateToZero(IEnumerable<VRAgentUpdatedToZeroModel> updatedToZeroList)
        {
            var table = "<br />Table: agents whose VR got updated to 0 <br /><br />";

            if (updatedToZeroList != null && updatedToZeroList.Any())
            {
                table += "<table style='" + CssStyleTable(300) + "'>";
                table += "<tr>";
                table += "<td style='" + CssStyleTh() + "'>No</td>";
                table += "<td style='" + CssStyleTh() + "'>Site</td>";
                table += "<td style='" + CssStyleTh() + "'>Agent</td>";
                table += "</tr>";
                var index = 1;

                foreach (var item in updatedToZeroList)
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

        private string GenerateTableUpdateToFollowSupper(IEnumerable<VRAgentUpdatedToFollowSuperModel> updatedToFollowSupperList)
        {
            var table = "<br />Table: agents whose VR got updated to follow Super <br /><br />";

            if (updatedToFollowSupperList != null && updatedToFollowSupperList.Any())
            {
                table += "<table style='" + CssStyleTable(400) + "'>";
                table += "<tr>";
                table += "<td style='" + CssStyleTh() + "'>No</td>";
                table += "<td style='" + CssStyleTh() + "'>Site</td>";
                table += "<td style='" + CssStyleTh() + "'>Agent</td>";
                table += "<td style='" + CssStyleTh() + "'>Member's Winloss</td>";
                table += "</tr>";
                var index = 1;

                foreach (var item in updatedToFollowSupperList)
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
