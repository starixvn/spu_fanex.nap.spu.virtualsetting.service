namespace Fanex.NAP.SPU.CTS.CC.Features.SabaSportDangerLevel
{
    using Fanex.Data.Repository;

    public class SabaSportDangerLevelDateReadyCheckCriteria : NonQueryCommand
    {
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