namespace Fanex.NAP.SPU.VR.Features.SabaSportDangerLevel
{
    using Fanex.NAP.SPU.CTS.Shares;
    using System;

    [XName("r")]
    public class SapaSportDangerLevelEntity
    {
        [XName("id")]
        public Int64 CustID { get; set; }

        [XName("sport")]
        public int SportType { get; set; }

        [XName("parlay")]
        public bool IsParlay { get; set; }

        [XName("winloss")]
        public decimal MemberWinloss { get; set; }
    }
}