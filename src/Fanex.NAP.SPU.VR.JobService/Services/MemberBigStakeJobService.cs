namespace Fanex.NAP.SPU.VR.JobService.Services
{
    using Fanex.Logging;
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.Database;
    using Fanex.MailDelivery.Client;
    using Fanex.MailDelivery.Core.Models;
    using Fanex.NAP.SPU.CTS.Shares;
    using Fanex.NAP.SPU.Notification.Services;
    using Fanex.NAP.SPU.VR.Features.MemberBigStake;
    using Fanex.NAP.SPU.VR.Features.MemberBigStake.Entities;
    using Fanex.NAP.SPU.VR.Features.MemberBigStake.Model;
    using Newtonsoft.Json;
    using Sentry;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public interface IMemberBigStakeJobService
    {
        Task<bool> Execute();
    }

    public class MemberBigStakeJobService : IMemberBigStakeJobService
    {
        private const string MailInformNoMemberAffectTemplate = @"MailTemplate/MemberBigStake/MailInformNoMemberAffectTemplate.cshtml";
        private const string MailInformBigCreditMemberTemplate = @"MailTemplate/MemberBigStake/MailInformBigCreditMemberTemplate.cshtml";
        private const string MailInformBigLossTemplate = @"MailTemplate/MemberBigStake/MailInformBigLossTemplate.cshtml";
        private const string MailInformBigCreditMemberAndBigLossTemplate = @"MailTemplate/MemberBigStake/MailInformBigCreditMemberAndBigLossTemplate.cshtml";
        private const string FormatDecimalToString_Place = "#,##0.00";
        private readonly ISentryClient sentryClient;
        private readonly IAppSettings appSettings;
        private readonly IMemberBigStakeService memberBigStakeServices;
        private string notifiedMessage = string.Empty;

        public MemberBigStakeJobService(ISentryClient sentryClient, IAppSettings appSettings, IMemberBigStakeService memberBigStakeServices)
        {
            this.sentryClient = sentryClient;
            this.appSettings = appSettings;
            this.memberBigStakeServices = memberBigStakeServices;
        }

        public Task<bool> Execute()
        {
            var countRetry = 0;

            while (countRetry < appSettings.RetryTime)
            {
                try
                {
                    memberBigStakeServices.InsertMemberBigStake();

                    var memberBigStakePerformance = memberBigStakeServices.GetMemberBigStakePerformance();

                    UpdateMemberBigStakeFirstBetDate(memberBigStakePerformance);

                    UpdateMemberBigStakeStatus(memberBigStakePerformance);

                    //SendMail(this.appSettings.MailingListIdAgentAjust);

                    LogService.Info($"VR - Member Big Stake job run COMPLETED");

                    countRetry = appSettings.RetryTime + 1;                  

                    //NotifyMessage();
                }
                catch (Exception ex)
                {
                    sentryClient.CaptureException(ex);

                    var logMessage = new LogMessage(ex.Message)
                        .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                        .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                    LogService.Error($"VR - Member Big Stake job Execute exeption message:  {logMessage}");

                    countRetry += 1;

                    if (countRetry == appSettings.RetryTime)
                    {
                        return Task.FromResult(false);
                    }
                }
            }           

            return Task.FromResult(true);
        }

        private void UpdateMemberBigStakeFirstBetDate(MemberBigStakePerformanceModel memberBigStakePerformance)
        {
            var countRetry = 0;

            while (countRetry < this.appSettings.RetryTime)
            {
                try
                {
                    if (memberBigStakePerformance != null && memberBigStakePerformance.FirstBetDateList != null && memberBigStakePerformance.FirstBetDateList.Any())
                    {
                        memberBigStakePerformance.FirstBetDateList.GroupBy(x => x.FirstBetDate).ToList().ForEach(r =>
                        {
                            var custIds = r.Select(x => x.CustId);

                            custIds.ChunkList(500).ToList().ForEach(x =>
                            {
                                memberBigStakeServices.UpdateMemberBigStakeFirstBetDate(x, 1, r.Key);
                            });
                        });

                        var json = JsonConvert.SerializeObject(memberBigStakePerformance.FirstBetDateList);

                        LogService.Info($"VR - List { memberBigStakePerformance.FirstBetDateList.Count() } member(s) [First Bet Date Member] update Big Stake: {json}");
                    }

                    countRetry = this.appSettings.RetryTime + 1;
                }
                catch (Exception ex)
                {
                    var logMessage = new LogMessage(ex.Message)
                     .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                     .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                    LogService.Error($"VR - Update First Bet Date Member Big Stake - Retry: {countRetry} message:  {logMessage}");

                    countRetry += 1;

                    if (countRetry == this.appSettings.RetryTime)
                    {
                        throw;
                    }
                }
            }
        }

        private void UpdateMemberBigStakeStatus(MemberBigStakePerformanceModel memberBigStakePerformance)
        {
            var countRetry = 0;

            while (countRetry < this.appSettings.RetryTime)
            {
                try
                {
                    if (memberBigStakePerformance != null && memberBigStakePerformance.StatusList != null && memberBigStakePerformance.StatusList.Any())
                    {
                        var custIdByStatus = memberBigStakePerformance.StatusList.Select(x => x.CustId);

                        custIdByStatus.ChunkList(500).ToList().ForEach(x => {
                            this.memberBigStakeServices.UpdateWhitelistMemberBigStake(custIdByStatus, 1);
                        });

                        var json = JsonConvert.SerializeObject(memberBigStakePerformance.StatusList);

                        LogService.Info($"VR - List { memberBigStakePerformance.StatusList.Count() } member(s) [Status] update Member Big Stake: {json}");
                    }

                    countRetry = this.appSettings.RetryTime + 1;
                }
                catch (Exception ex)
                {
                    var logMessage = new LogMessage(ex.Message)
                     .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                     .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                    LogService.Error($"VR - Update White list Member Big Stake Status - Retry: {countRetry} message:  {logMessage}");

                    countRetry += 1;

                    if (countRetry == this.appSettings.RetryTime)
                    {
                        throw;
                    }
                }
            }
        }

        private void SendMail(int mailingListId)
        {
            if (mailingListId <= 0)            
                return;            

            var subject = DateTime.Now.ToString("yyyyMMdd") + " List of big credit members just having their first ticket.";
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
                var logMessage = new LogMessage(ex.Message)
                   .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                   .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                LogService.Error($"MemberBigStake - send mail exception: message:  {logMessage}");
            }
        }

        private string GenerateMailBody()
        {
            var creditMemberBigStake = this.memberBigStakeServices.GetBigCreditMemberForAutomail();
            var mailTemplatePath = AppDomain.CurrentDomain.BaseDirectory;
            var table1 = string.Empty;
            var table2 = string.Empty;

            if (!creditMemberBigStake.BigCreditMemberList.Any() && !creditMemberBigStake.BigCreditMemberWithBigLossList.Any())
            {
                mailTemplatePath += MailInformNoMemberAffectTemplate;
            }
            else if (creditMemberBigStake.BigCreditMemberList.Any() && !creditMemberBigStake.BigCreditMemberWithBigLossList.Any())
            {
                mailTemplatePath += MailInformBigCreditMemberTemplate;

                table1 = GenerateTable(creditMemberBigStake.BigCreditMemberList);
            }
            else if (!creditMemberBigStake.BigCreditMemberList.Any() && creditMemberBigStake.BigCreditMemberWithBigLossList.Any())
            {
                mailTemplatePath += MailInformBigLossTemplate;
                table1 = GenerateTable(creditMemberBigStake.BigCreditMemberWithBigLossList);
            }
            else
            {
                mailTemplatePath += MailInformBigCreditMemberAndBigLossTemplate;

                table1 = GenerateTable(creditMemberBigStake.BigCreditMemberList);

                table2 = GenerateTable(creditMemberBigStake.BigCreditMemberWithBigLossList);
            }

            var body = Utilities.ReadTextFile(mailTemplatePath);

            body = string.Format(body, table1, table2);

            var json = JsonConvert.SerializeObject(creditMemberBigStake);

            LogService.Info($"VR - List member(s) Big Stake for send mail: {json}");

            this.notifiedMessage = $"Auto update member big stake job run COMPLETED." +
               $"\n\r - Big credit members: {creditMemberBigStake.BigCreditMemberList.Count()}" +
               $"\n\r - Big credit member with big loss: {creditMemberBigStake.BigCreditMemberWithBigLossList.Count()}";

            return body;
        }

        private string GenerateTable(IEnumerable<AutomailBigCreditMemberEntity> creditMemberBigStake)
        {
            var table = string.Empty;

            if (creditMemberBigStake != null && creditMemberBigStake.Any())
            {
                table += "<table style='" + CssStyleTable(700) + "'>";
                table += "<tr>";
                table += "<td style='" + CssStyleTh() + "'>No.</td>";
                table += "<td style='" + CssStyleTh() + "'>Member</td>";
                table += "<td style='" + CssStyleTh() + "'>Site</td>";
                table += "<td style='" + CssStyleTh() + "'>Given Credit (RM)</td>";
                table += "<td style='" + CssStyleTh() + "'>Member Turnover (RM)</td>";
                table += "<td style='" + CssStyleTh() + "'>Member Winloss (RM)</td>";
                table += "</tr>";
                var index = 1;

                foreach (var item in creditMemberBigStake)
                {
                    table += "<tr>";
                    table += "<td style='" + CssStyleTd() + "'>" + index + "</td>";
                    table += "<td style='" + CssStyleTd() + "'>" + item.Member + "</td>";
                    table += "<td style='" + CssStyleTd() + "'>" + item.Site + "</td>";
                    table += "<td style='" + CssStyleTd() + "'>" + item.GivenCredit_RM.ToString(FormatDecimalToString_Place) + "</td>";
                    table += "<td style='" + CssStyleTd() + "'>" + item.MemberTurnover_RM.ToString(FormatDecimalToString_Place) + "</td>";
                    table += "<td style='" + CssStyleTd() + "'>" + item.MemberWinloss_RM.ToString(FormatDecimalToString_Place) + "</td>";
                    table += "</tr>";

                    index++;
                }

                table += "</table></br>";
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

        private void NotifyMessage()
        {
            if (!string.IsNullOrWhiteSpace(notifiedMessage))
            {
                var telegramService = new NotificationService();
                telegramService
                   .Config("0A40aOc7CtpzjbBSJu8KNWQRoXyjRvf9", "-1001400862084")
                   .Message(new MessageModel("VR - Member Big Stake", DateTime.Now, appSettings.EnvironmentString, notifiedMessage))
                   .SendWithoutDelay(isInfo: true);
            }
        }
    }
}
