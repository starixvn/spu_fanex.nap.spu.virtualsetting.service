namespace Fanex.NAP.SPU.VR.Features.SpecialLicensee
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Fanex.NAP.SPU.VR.Features.SpecialLicensee.Criteria;
    using Fanex.NAP.SPU.VR.Features.SpecialLicensee.Entities;
    using System.Collections.Generic;

    public interface ISpecialLicenseeDataAccess
    {
        IEnumerable<int> GetSpecialSportBookVRAdjustmentCustomers(int sportType);

        (IEnumerable<PassedPerfCheckCustomerEntity>, IEnumerable<int>) VerifyVRAdjustmentCustomers(string customerList);

        void UpdateVRFollowUpline(string customerList, int sportType);

        void UpdateVRAutoAdjustmentStatus(string customerList, int sportType);
    }

    public class SpecialLicenseeDataAccess : ISpecialLicenseeDataAccess
    {
        private readonly IDynamicRepository _repository;

        public SpecialLicenseeDataAccess()
        {
            this._repository = new DynamicRepository();
        }

        public IEnumerable<int> GetSpecialSportBookVRAdjustmentCustomers(int sportType)
        {
            var criteria = new GetSpecialSportBookVRCustomerCriteria
            {
                Sporttype = sportType
            };

            return this._repository.Fetch<int>(criteria);
        }

        public (IEnumerable<PassedPerfCheckCustomerEntity>, IEnumerable<int>) VerifyVRAdjustmentCustomers(string customerList)
        {
            var criteria = new VerifyVRCustomerCriteria
            {
                ListCustID = customerList
            };

            return this._repository.FetchMultiple<PassedPerfCheckCustomerEntity, int>(criteria);
        }

        public void UpdateVRFollowUpline(string customerList, int sportType)
        {
            var criteria = new UpdateVRFollowUplineCriteria
            {
                CustIDList = customerList,
                Sporttype = sportType
            };

            using (var objectDb = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                objectDb.ExecuteNonQuery(criteria);
            }
        }

        public void UpdateVRAutoAdjustmentStatus(string customerList, int sportType)
        {
            var criteria = new UpdateVRAutoAdjustmentStatusCriteria
            {
                CustIDList = customerList,
                Sporttype = sportType
            };

            using (var objectDb = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                objectDb.ExecuteNonQuery(criteria);
            }
        }

    }
}
