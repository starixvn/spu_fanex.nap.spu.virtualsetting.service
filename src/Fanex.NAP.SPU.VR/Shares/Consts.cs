namespace Fanex.NAP.SPU.VR.Shares
{
    public class Consts
    {
        public const string VSXmlRoot = "<ROOT>{0}</ROOT>";

        public const string VSXmlForUpdating = "<Rate CustIds=\"{0}\" VirtualMultiplier =\"{1} \" />";

        public const string VSXmlForLogging = "<Rate CustID=\"{0}\" ReportDate=\"{1}\" SiteID=\"{2}\" SRecommend=\"{3}\"" +
                                                        " MRecommend=\"{4}\" Recommend=\"{5}\" OldVirtualRate=\"{6}\" NewVirtualRate =\"{7}\" IsBA =\"{8}\" />";

        public const string VSXmlForCustomerMarginHistory = "<Rate CustID=\"{0}\" ReportMonth=\"{1}\" ReportYear=\"{2}\" Turnover=\"{3}\"" +
                                                        " Winloss=\"{4}\" BetCount=\"{5}\" ServiceName=\"{6}\" />";

        public const string VSNormalPunter = "Normal Punter(s)";

        public const string VSProPunter = "Pro Punter(s)";

        public const string VSMailInformTemplate = @"MailTemplate\MailInformTemplate.cshtml";

        public const string VSSoccer = "Soccer";

        public const string VSSportbook = "Sport book";

        public const int SleepTime_SapaSportDangerLevel = 120000 ;

        public enum Environment
        {
            LOCAL = 1,
            UAT = 2,
            PRO = 3
        }

    }
}
