using Fanex.Data;
using Fanex.Data.Repository;
using Fanex.NAP.SPU.VR.Features.BetLimit.Criteria;
using System.Collections.Generic;

namespace Fanex.NAP.SPU.VR.Features.BetLimit
{
    public interface IBetLimitDataAccess
    {
        IEnumerable<int> GetCustomersBetLimit();

        (IEnumerable<int>, IEnumerable<int>) VerifyCustomersBetLimit(string customerList);

        void RemoveBetLimit(string customerList);

        void UpdateBetLimit(string customerList);
    }

    public class BetLimitDataAccess : IBetLimitDataAccess
    {
        private readonly IDynamicRepository _repository;

        public BetLimitDataAccess()
        {
            _repository = new DynamicRepository();
        }

        public IEnumerable<int> GetCustomersBetLimit()
        {
            return _repository.Fetch<int>(new GetCustomerBetLimitCriteria());
        }

        public void RemoveBetLimit(string customerList)
        {
            var criteria = new RemoveCustomerBetLimitCriteria
            {
                CustIdList = customerList
            };

            using (var objectDb = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                objectDb.ExecuteNonQuery(criteria);
            }
        }

        public void UpdateBetLimit(string customerList)
        {
            var criteria = new UpdateCustomerBetLimitCriteria
            {
                CustIdList = customerList
            };

            using (var objectDb = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                objectDb.ExecuteNonQuery(criteria);
            }
        }

        public (IEnumerable<int>, IEnumerable<int>) VerifyCustomersBetLimit(string customerList)
        {
            var criteria = new VerifyCustomerBetLimitCriteria
            {
                ListCustID = customerList
            };

            return _repository.FetchMultiple<int, int>(criteria);
        }
    }
}
