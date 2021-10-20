namespace Fanex.NAP.SPU.CTS.CC.Features.SabaSportDangerLevel
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class SabaSportDangerLevelUpdateCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "CustList")]
        public string CustsToXML { get; set; }

        [SpParam(SqlParamName = "Enviroment")]
        public string Enviroment { get; set; }

        [SpParam(SqlParamName = "IncludeUUS")]
        public int IncludeUUS { get; set; }

        [SpParam(SqlParamName = "ErrCode")]
        public int ErrorCode { get; set; }

        [SpParam(SqlParamName = "ErrMsg")]
        public string ErrorMessage { get; set; }

        public override string GetSettingKey()
        {
            return "Acc_CustomerUpdDangerLevel";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}