using Fanex.NAP.SPU.VR.Features.MemberBigStake.Entities;
using System.Collections.Generic;

namespace Fanex.NAP.SPU.VR.Features.MemberBigStake.Model
{
    public class MemberBigStakePerformanceModel
    {
        public IEnumerable<MemberBigStakePerformanceFirstDayEntity> FirstBetDateList { get; set; }

        public IEnumerable<MemberBigStakePerformanceStatusEntity> StatusList { get; set; }
    }
}
