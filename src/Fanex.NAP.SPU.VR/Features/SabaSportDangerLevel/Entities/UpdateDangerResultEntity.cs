namespace Fanex.NAP.SPU.VR.Features.SabaSportDangerLevel
{
    using System.Collections.Generic;

    public class UpdateDangerResultEntity
    {
        public int ErrorCode { get; set; }

        public string ErrorMsg { get; set; }

        public IEnumerable<SapaSportDangerLevelEntity> InPutParam { get; set; }
    }
}