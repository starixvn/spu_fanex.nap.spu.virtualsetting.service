namespace Fanex.NAP.SPU.VR.Features.Agent
{
    using Fanex.NAP.SPU.VR.Features.Agent.Entities;
    using Fanex.NAP.SPU.VR.Features.Agent.Model;
    using System.Collections.Generic;

    public interface IAgentService
    {
        void InsertNewAgentAdjust(bool isTest);

        IEnumerable<UnflagAbnormalAgentEntity> VerifySportsAgentPerformance(bool isTest);

        void UpdateFlagAbnormalAgent(int type, bool isTest);

        void UpdateUnflagAbnormalAgent(string customerList, int type, bool isTest);

        (IEnumerable<VRAgentUpdatedToZeroModel>, IEnumerable<VRAgentUpdatedToFollowSuperModel>) GetVRAgentUpdated();
    }

    public class AgentService : IAgentService
    {
        private readonly IAgentDataAccess agentDataAccess;

        public AgentService(IAgentDataAccess agentDataAccess)
        {
            this.agentDataAccess = agentDataAccess;
        }

        public void InsertNewAgentAdjust(bool isTest)
        {
            agentDataAccess.InsertNewAgentAdjust(isTest);
        }

        public IEnumerable<UnflagAbnormalAgentEntity> VerifySportsAgentPerformance(bool isTest)
        {
            return agentDataAccess.VerifySportsAgentPerformance(isTest);
        }

        public void UpdateFlagAbnormalAgent(int type, bool isTest)
        {
            agentDataAccess.UpdateFlagAbnormalAgent(type, isTest);
        }

        public void UpdateUnflagAbnormalAgent(string customerList, int type, bool isTest)
        {
            if (!string.IsNullOrEmpty(customerList))
            {
                agentDataAccess.UpdateUnflagAbnormalAgent(customerList, type, isTest);
            }            
        }

        public (IEnumerable<VRAgentUpdatedToZeroModel>, IEnumerable<VRAgentUpdatedToFollowSuperModel>) GetVRAgentUpdated()
        {
            return agentDataAccess.GetVRAgentUpdated();
        }

    }
}
