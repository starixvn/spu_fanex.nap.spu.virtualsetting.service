namespace Fanex.NAP.SPU.VR.Features.Agent
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Fanex.NAP.SPU.VR.Features.Agent.Criteria;
    using Fanex.NAP.SPU.VR.Features.Agent.Entities;
    using Fanex.NAP.SPU.VR.Features.Agent.Model;    
    using System.Collections.Generic;

    public interface IAgentDataAccess
    {
        void InsertNewAgentAdjust(bool isTest);

        IEnumerable<UnflagAbnormalAgentEntity> VerifySportsAgentPerformance(bool isTest);

        void UpdateFlagAbnormalAgent(int type, bool isTest);

        void UpdateUnflagAbnormalAgent(string customerList, int type, bool isTest);

        (IEnumerable<VRAgentUpdatedToZeroModel>, IEnumerable<VRAgentUpdatedToFollowSuperModel>) GetVRAgentUpdated();
    }

    public class AgentDataAccess : IAgentDataAccess
    {
        private readonly IDynamicRepository repository;

        public AgentDataAccess()
        {
            this.repository = new DynamicRepository();
        }

        public void InsertNewAgentAdjust(bool isTest)
        {
            var criteria = new InsertNewAgentAdjustCriteria { IsTest = isTest };

            using (var objectDb = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                objectDb.ExecuteNonQuery(criteria);
            }
        }

        public IEnumerable<UnflagAbnormalAgentEntity> VerifySportsAgentPerformance(bool isTest)
        {
            var criteria = new VerifySportsAgentPerformanceCriteria { IsTest = isTest };

            return this.repository.Fetch<UnflagAbnormalAgentEntity>(criteria);
        }

        public void UpdateFlagAbnormalAgent(int type, bool isTest)
        {
            var criteria = new UpdateFlagAbnormalAgentCriteria
            {
                Type = type,
                IsTest = isTest
            };

            using (var objectDb = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                objectDb.ExecuteNonQuery(criteria);
            }
        }

        public void UpdateUnflagAbnormalAgent(string customerList, int type, bool isTest)
        {
            var criteria = new UpdateUnflagAbnormalAgentCriteria
            {
                CustIDList = customerList,
                Type = type,
                IsTest = isTest
            };

            using (var objectDb = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                objectDb.ExecuteNonQuery(criteria);
            }
        }

        public (IEnumerable<VRAgentUpdatedToZeroModel>, IEnumerable<VRAgentUpdatedToFollowSuperModel>) GetVRAgentUpdated()
        {
            var criteria = new GetVRAgentUpdatedCriteria();

            return this.repository.FetchMultiple<VRAgentUpdatedToZeroModel, VRAgentUpdatedToFollowSuperModel>(criteria);
        }
    }
}
