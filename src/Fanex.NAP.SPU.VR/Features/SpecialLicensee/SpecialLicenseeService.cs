namespace Fanex.NAP.SPU.VR.Features.SpecialLicensee
{
    using System.Collections.Generic;

    public interface ISpecialLicenseeService
    {
        IEnumerable<int> GetSpecialSportBookVRAdjustmentCustomers(int sportType);

        (IEnumerable<int>, IEnumerable<int>) VerifyVRAdjustmentCustomers(string customerList);

        void UpdateVRFollowUpline(string customerList, int sportType);

        void UpdateVRAutoAdjustmentStatus(string customerList, int sportType);
    }

    public class SpecialLicenseeService : ISpecialLicenseeService
    {
        private readonly ISpecialLicenseeDataAccess specialLicenseeDataAccess;

        public SpecialLicenseeService(ISpecialLicenseeDataAccess specialLicenseeDataAccess)
        {
            this.specialLicenseeDataAccess = specialLicenseeDataAccess;
        }

        public IEnumerable<int> GetSpecialSportBookVRAdjustmentCustomers(int sportType)
        {
            return specialLicenseeDataAccess.GetSpecialSportBookVRAdjustmentCustomers(sportType);
        }

        public (IEnumerable<int>, IEnumerable<int>) VerifyVRAdjustmentCustomers(string customerList)
        {
            return specialLicenseeDataAccess.VerifyVRAdjustmentCustomers(customerList);
        }

        public void UpdateVRFollowUpline(string customerList, int sportType)
        {
            specialLicenseeDataAccess.UpdateVRFollowUpline(customerList, sportType);
        }

        public void UpdateVRAutoAdjustmentStatus(string customerList, int sportType)
        {
            specialLicenseeDataAccess.UpdateVRAutoAdjustmentStatus(customerList, sportType);
        }

    }
}
