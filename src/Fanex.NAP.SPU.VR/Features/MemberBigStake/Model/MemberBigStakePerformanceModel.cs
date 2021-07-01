namespace Fanex.NAP.SPU.VR.Features.MemberBigStake.Model
{
    using Fanex.NAP.SPU.VR.Features.MemberBigStake.Entities;
    using System.Collections.Generic;

    public class MemberBigStakePerformanceModel
    {
        public IEnumerable<MemberBigStakePerformanceFirstDayEntity> FirstBetDateList { get; set; }

        public IEnumerable<MemberBigStakePerformanceStatusEntity> StatusList { get; set; }
    }
}
