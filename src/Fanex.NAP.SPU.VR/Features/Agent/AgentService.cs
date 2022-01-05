namespace Fanex.NAP.SPU.VR.Features.Agent
{
    using Fanex.NAP.SPU.VR.Features.Agent.Entities;
    using Fanex.NAP.SPU.VR.Features.Agent.Model;
    using System.Collections.Generic;

    public interface IAgentService
    {
        void InsertNewAgentAdjust();

        IEnumerable<UnflagAbnormalAgentEntity> VerifySportsAgentPerformance();

        void UpdateFlagAbnormalAgent(int type);

        void UpdateUnflagAbnormalAgent(string customerList, int type);

        (IEnumerable<VRAgentUpdatedToZeroModel>, IEnumerable<VRAgentUpdatedToFollowSuperModel>) GetVRAgentUpdated();
    }

    public class AgentService : IAgentService
    {
        private readonly IAgentDataAccess agentDataAccess;

        public AgentService(IAgentDataAccess agentDataAccess)
        {
            this.agentDataAccess = agentDataAccess;
        }

        public void InsertNewAgentAdjust()
        {
            agentDataAccess.InsertNewAgentAdjust();
        }

        public IEnumerable<UnflagAbnormalAgentEntity> VerifySportsAgentPerformance()
        {
            return agentDataAccess.VerifySportsAgentPerformance();
        }

        public void UpdateFlagAbnormalAgent(int type)
        {
            agentDataAccess.UpdateFlagAbnormalAgent(type);
        }

        public void UpdateUnflagAbnormalAgent(string customerList, int type)
        {
            if (!string.IsNullOrEmpty(customerList))
            {
                agentDataAccess.UpdateUnflagAbnormalAgent(customerList, type);
            }            
        }

        public (IEnumerable<VRAgentUpdatedToZeroModel>, IEnumerable<VRAgentUpdatedToFollowSuperModel>) GetVRAgentUpdated()
        {
            return agentDataAccess.GetVRAgentUpdated();
        }

    }
}
