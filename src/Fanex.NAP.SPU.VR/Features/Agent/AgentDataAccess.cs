namespace Fanex.NAP.SPU.VR.Features.Agent
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Fanex.NAP.SPU.VR.Features.Agent.Criteria;
    using Fanex.NAP.SPU.VR.Features.Agent.Model;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface IAgentDataAccess
    {
        void InsertNewAgentAdjust();

        IEnumerable<int> VerifySportsAgentPerformance();

        void UpdateFlagAbnormalAgent(int type);

        void UpdateUnflagAbnormalAgent(string customerList, int type);

        (IEnumerable<VRAgentUpdatedToZeroModel>, IEnumerable<VRAgentUpdatedToFollowSuperModel>) GetVRAgentUpdated();
    }

    public class AgentDataAccess : IAgentDataAccess
    {
        private readonly IDynamicRepository _repository;

        public void InsertNewAgentAdjust()
        {
            var criteria = new InsertNewAgentAdjustCriteria();

            using (var objectDb = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                objectDb.ExecuteNonQuery(criteria);
            }
        }

        public IEnumerable<int> VerifySportsAgentPerformance()
        {
            var criteria = new VerifySportsAgentPerformanceCriteria();

            return _repository.Fetch<int>(criteria);
        }

        public void UpdateFlagAbnormalAgent(int type)
        {
            var criteria = new UpdateFlagAbnormalAgentCriteria
            {
                Type = type
            };

            using (var objectDb = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                objectDb.ExecuteNonQuery(criteria);
            }
        }

        public void UpdateUnflagAbnormalAgent(string customerList, int type)
        {
            var criteria = new UpdateUnflagAbnormalAgentCriteria
            {
                CustIDList = customerList,
                Type = type
            };

            using (var objectDb = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                objectDb.ExecuteNonQuery(criteria);
            }
        }

        public (IEnumerable<VRAgentUpdatedToZeroModel>, IEnumerable<VRAgentUpdatedToFollowSuperModel>) GetVRAgentUpdated()
        {
            var criteria = new GetVRAgentUpdatedCriteria();

            return _repository.FetchMultiple<VRAgentUpdatedToZeroModel, VRAgentUpdatedToFollowSuperModel>(criteria);
        }
    }
}
