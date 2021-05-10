using Fanex.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NAP.SPU.VR.Features.BetLimit.Criteria
{
    public class RemoveCustomerBetLimitCriteria: CriteriaBase
    {
        public string CustIdList { get; set; }

        public override string GetSettingKey()
        {
            return "NAP_BLControl_PerformanceChecking_RemoveBetLimit";
        }

        public override bool IsValid()
        {
            return !string.IsNullOrEmpty(CustIdList);
        }
    }
}
