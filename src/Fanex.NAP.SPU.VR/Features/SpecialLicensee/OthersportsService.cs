namespace Fanex.NAP.SPU.VR.Features.SpecialLicensee
{
    using Fanex.Logging;
    using Fanex.NAP.SPU.CTS.Shares;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface IOthersportsService
    {
        IOthersportsService AutoAdjustVirtualSettings(int punterChunkSize);

        IOthersportsService AutoAdjustVRForSpecialSportsbook(int punterChunkSize);
    }

    public class OthersportsService : IOthersportsService
    {
        private readonly ISpecialLicenseeService specialLicenseeService;
       
        public OthersportsService(ISpecialLicenseeService specialLicenseeService)
        {
            this.specialLicenseeService = specialLicenseeService;
        }

        public IOthersportsService AutoAdjustVirtualSettings(int punterChunkSize)
        {
            var customerList = specialLicenseeService.GetSpecialSportBookVRAdjustmentCustomers(99).ToList();

            var customerListString = string.Join(",", customerList);

            var (passedPerfCheckCustomers, unpassedPerfCheckCustomers) = specialLicenseeService.VerifyVRAdjustmentCustomers(customerListString);

            AdjustVRPassedCustomers(passedPerfCheckCustomers, 99, punterChunkSize);

            UpdateStatusUnpassedCustomers(unpassedPerfCheckCustomers, 99, punterChunkSize);

            return this;
        }

        public IOthersportsService AutoAdjustVRForSpecialSportsbook(int punterChunkSize)
        {
            var customerList = specialLicenseeService.GetSpecialSportBookVRAdjustmentCustomers(100).ToList();
            var customerListString = string.Join(",", customerList);

            LogService.Info("VR Setting - Special Licensee - Special SportBook - Raw Customer list: " + customerListString);

            var (passedPerfCheckCustomers, unpassedPerfCheckCustomers) = specialLicenseeService.VerifyVRAdjustmentCustomers(customerListString);

            AdjustVRPassedCustomers(passedPerfCheckCustomers, 100, punterChunkSize);
            
            UpdateStatusUnpassedCustomers(unpassedPerfCheckCustomers, 100, punterChunkSize);

            return this;
        }

        private void AdjustVRPassedCustomers(IEnumerable<int> customerList, int sportType, int punterChunkSize)
        {
            var customerListString = string.Join(",", customerList);

            if (sportType == 99)
                LogService.Info("VR Setting - Special Licensee - OtherSports - Customers have passed Performance Check: " + customerListString);
            else
                LogService.Info("VR Setting - Special Licensee - Special SportBook - Customers have passed Performance Check: " + customerListString);

            var customerChunks = Utilities.ChunkList(customerList, punterChunkSize).ToList();

            for (int i = 0; i < customerChunks.Count(); i++)
            {                
                specialLicenseeService.UpdateVRFollowUpline(string.Join(",", customerChunks[i]), sportType);                
            }
        }

        private void UpdateStatusUnpassedCustomers(IEnumerable<int> customerList, int sportType, int punterChunkSize)
        {
            var customerListString = string.Join(",", customerList);

            if (sportType == 99)
                LogService.Info("VR Setting - Special Licensee - OtherSports - Customers have NOT passed Performance Check: " + customerListString);
            else
                LogService.Info("VR Setting - Special Licensee - Special SportBook - Customers have NOT passed Performance Check: " + customerListString);

            var customerChunks = Utilities.ChunkList(customerList, punterChunkSize).ToList();

            for (int i = 0; i < customerChunks.Count(); i++)
            {
                specialLicenseeService.UpdateVRAutoAdjustmentStatus(string.Join(",", customerChunks[i]), sportType);
            }
        }
       
    }
}
