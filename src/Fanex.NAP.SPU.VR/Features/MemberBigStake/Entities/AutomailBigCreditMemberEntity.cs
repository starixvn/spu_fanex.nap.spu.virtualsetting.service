namespace Fanex.NAP.SPU.VR.Features.MemberBigStake.Entities
{
    public class AutomailBigCreditMemberEntity
    {
        public string Member { get; set; }

        public string Site { get; set; }

        public decimal GivenCredit_RM { get; set; }

        public decimal MemberTurnover_RM { get; set; }

        public decimal MemberWinloss_RM { get; set; }

        public int SettleBetCount {  get; set; }

        public int TotalBetCount { get; set; }
    }
}
