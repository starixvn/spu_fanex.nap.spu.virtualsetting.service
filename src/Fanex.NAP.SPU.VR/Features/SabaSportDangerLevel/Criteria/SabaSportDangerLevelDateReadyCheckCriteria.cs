namespace Fanex.NAP.SPU.CTS.CC.Features.SabaSportDangerLevel
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class SabaSportDangerLevelDateReadyCheckCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "IncludeUUS")]
        public bool IncludeUUS { get; set; }

        public override string GetSettingKey()
        {
            return "Acc_Rpt_SABASportsDangerLevel_CheckDataReady";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}