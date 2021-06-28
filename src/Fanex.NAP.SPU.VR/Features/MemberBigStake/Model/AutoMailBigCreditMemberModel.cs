using Fanex.NAP.SPU.VR.Features.MemberBigStake.Entities;
using System.Collections.Generic;

namespace Fanex.NAP.SPU.VR.Features.MemberBigStake.Model
{
    public class AutoMailBigCreditMemberModel
    {
        public IEnumerable<AutomailBigCreditMemberEntity> BigCreditMemberList { get; set; }

        public IEnumerable<AutomailBigCreditMemberEntity> BigCreditMemberWithBigLossList { get; set; }
    }
}
