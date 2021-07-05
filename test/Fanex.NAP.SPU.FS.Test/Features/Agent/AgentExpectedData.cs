namespace Fanex.NAP.SPU.VR.Tests.Features.Agent
{
    using Fanex.NAP.SPU.VR.Features.Agent.Model;    
    using System.Collections.Generic;

    public static class AgentExpectedData
    {
        public static IEnumerable<int> ExpectedVerifySportsAgentPerformance
        {
            get
            {
                return new List<int> { 461185, 898415, 58314 };
            }
        }

        public static (IEnumerable<VRAgentUpdatedToZeroModel>, IEnumerable<VRAgentUpdatedToFollowSuperModel>) ExpectedGetVRAgentUpdated
        {
            get
            {
                var agent1 = new List<VRAgentUpdatedToZeroModel>
                {
                    new VRAgentUpdatedToZeroModel { Agent = "agent01", Site = "12Bet" },
                    new VRAgentUpdatedToZeroModel { Agent = "agent02", Site = "FB88" }
                };

                var agent2 = new List<VRAgentUpdatedToFollowSuperModel>
                {
                    new VRAgentUpdatedToFollowSuperModel { Agent = "agent03", Site = "88Bet", MemberWinloss = new decimal(56.87) }
                };

                return (agent1, agent2);
            }
        }
    }
}
