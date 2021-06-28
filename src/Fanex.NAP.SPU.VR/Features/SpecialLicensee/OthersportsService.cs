namespace Fanex.NAP.SPU.VR.Features.SpecialLicensee
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface IOthersportsService
    {
        void AutoAdjustVirtualSettings();
    }

    public class OthersportsService : IOthersportsService
    {
        private readonly ISpecialLicenseeService specialLicenseeService;
        public OthersportsService()
        {
            specialLicenseeService = new SpecialLicenseeService();
        }

        public void AutoAdjustVirtualSettings()
        {
            var customerList = specialLicenseeService.GetSpecialSportBookVRAdjustmentCustomers(99).ToList();

            var customerListString = string.Join(",", customerList);

            var (passedPerfCheckCustomers, unpassedPerfCheckCustomers) = specialLicenseeService.VerifyVRAdjustmentCustomers(customerListString);

            AdjustVRPassedCustomers(passedPerfCheckCustomers, 99);

            UpdateStatusUnpassedCustomers(unpassedPerfCheckCustomers, 99);
        }

        private void AdjustVRPassedCustomers(IEnumerable<int> customerList, int sportType)
        {

        }

        private void UpdateStatusUnpassedCustomers(IEnumerable<int> customerList, int sportType)
        {

        }


    }
}
