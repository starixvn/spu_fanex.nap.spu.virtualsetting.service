namespace Fanex.NAP.SPU.VR.Features.SpecialLicensee
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Fanex.NAP.SPU.VR.Features.SpecialLicensee.Criteria;
    using Fanex.NAP.SPU.VR.Features.SpecialLicensee.Entities;
    using System.Collections.Generic;
    using System.Linq;

    public interface ISpecialLicenseeDataAccess
    {
        IEnumerable<int> GetSpecialSportBookVRAdjustmentCustomers(int sportType, bool isTest);

        (IEnumerable<CustomerUpdFirstBetDateEntity>,
            IEnumerable<CustomerUpdFirstBetDateEntity>,
            IEnumerable<PassedPerfCheckCustomerEntity>,
            IEnumerable<int>) VerifyVRAdjustmentCustomers(string customerList, bool isTest);

        void UpdateFirstBetDateCustomers(IEnumerable<CustomerUpdFirstBetDateEntity> custInfos, bool isTest);

        void UpdateVRFollowUpline(string customerList, int sportType, bool isTest);

        void UpdateVRAutoAdjustmentStatus(string customerList, int sportType, bool isTest);
    }

    public class SpecialLicenseeDataAccess : ISpecialLicenseeDataAccess
    {
        private readonly IDynamicRepository _repository;

        public SpecialLicenseeDataAccess()
        {
            this._repository = new DynamicRepository();
        }

        public IEnumerable<int> GetSpecialSportBookVRAdjustmentCustomers(int sportType, bool isTest)
        {
            var criteria = new GetSpecialSportBookVRCustomerCriteria
            {
                Sporttype = sportType,
                IsTest = isTest
            };

            return this._repository.Fetch<int>(criteria);
        }

        public (IEnumerable<CustomerUpdFirstBetDateEntity>,
            IEnumerable<CustomerUpdFirstBetDateEntity>,
            IEnumerable<PassedPerfCheckCustomerEntity>, 
            IEnumerable<int>) VerifyVRAdjustmentCustomers(string customerList, bool isTest)
        {
            var criteria = new VerifyVRCustomerCriteria
            {
                ListCustID = customerList,
                IsTest = isTest
            };

            using (var objectDb = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                using (var result = objectDb.QueryMultiple(criteria))
                {
                    var custUpdFirstBetDateSportsbook = result.Read<CustomerUpdFirstBetDateEntity>();
                    var custUpdFirstBetDateBasketball = result.Read<CustomerUpdFirstBetDateEntity>();
                    var passedPerfCheckCustomers = result.Read<PassedPerfCheckCustomerEntity>();
                    var unpassedPerfCheckCustomers = result.Read<int>();

                    return (custUpdFirstBetDateSportsbook, custUpdFirstBetDateBasketball, passedPerfCheckCustomers, unpassedPerfCheckCustomers);
                }
            }
        }

        public void UpdateFirstBetDateCustomers(IEnumerable<CustomerUpdFirstBetDateEntity> custInfos, bool isTest)
        {
            var firstCust = custInfos.FirstOrDefault();

            var criteria = new UpdateFirstBetDateCriteria
            {
                CustIDList = string.Join(",", custInfos.Select(x => x.CustID)),
                Sporttype = firstCust.Sporttype,
                BetDate = firstCust.BetDate,
                IsTest = isTest
            };

            using (var objectDb = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                objectDb.ExecuteNonQuery(criteria);
            }
        }

        public void UpdateVRFollowUpline(string customerList, int sportType, bool isTest)
        {
            var criteria = new UpdateVRFollowUplineCriteria
            {
                CustIDList = customerList,
                Sporttype = sportType,
                IsTest = isTest
            };

            using (var objectDb = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                objectDb.ExecuteNonQuery(criteria);
            }
        }

        public void UpdateVRAutoAdjustmentStatus(string customerList, int sportType, bool isTest)
        {
            var criteria = new UpdateVRAutoAdjustmentStatusCriteria
            {
                CustIDList = customerList,
                Sporttype = sportType,
                IsTest = isTest
            };

            using (var objectDb = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                objectDb.ExecuteNonQuery(criteria);
            }
        }

    }
}
