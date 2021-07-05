namespace Fanex.NAP.SPU.VR.Features.SpecialLicensee
{
    using Fanex.Logging;
    using Fanex.NAP.SPU.CTS.Shares;    
    using System.Collections.Generic;
    using System.Linq;

    public interface ISpecialLicenseeService
    {
        int PunterBasketballUpdated { get; }

        int PunterSportsbookUpdated { get; }

        ISpecialLicenseeService AutoAdjustForBasketball(int punterChunkSize);

        ISpecialLicenseeService AutoAdjustForSpecialSportsbook(int punterChunkSize);
        
    }

    public class SpecialLicenseeService : ISpecialLicenseeService
    {
        private readonly ISpecialLicenseeDataAccess specialLicenseeDataAccess;
        private int punterBasketballUpdated;
        private int punterSportsbookUpdated;

        public int PunterBasketballUpdated => punterBasketballUpdated;

        public int PunterSportsbookUpdated => punterSportsbookUpdated;

        public SpecialLicenseeService(ISpecialLicenseeDataAccess specialLicenseeDataAccess)
        {
            this.specialLicenseeDataAccess = specialLicenseeDataAccess;
        }

        public ISpecialLicenseeService AutoAdjustForBasketball(int punterChunkSize)
        {
            var customerList = this.specialLicenseeDataAccess.GetSpecialSportBookVRAdjustmentCustomers(99).ToList();

            if (customerList.Any())
            {
                var customerListString = string.Join(",", customerList);

                var (passedPerfCheckCustomers, unpassedPerfCheckCustomers) = this.specialLicenseeDataAccess.VerifyVRAdjustmentCustomers(customerListString);

                AdjustVRPassedCustomers(passedPerfCheckCustomers, 99, punterChunkSize);

                this.punterBasketballUpdated = passedPerfCheckCustomers.Count();

                UpdateStatusUnpassedCustomers(unpassedPerfCheckCustomers, 99, punterChunkSize);
            }

            return this;
        }

        public ISpecialLicenseeService AutoAdjustForSpecialSportsbook(int punterChunkSize)
        {
            var customerList = this.specialLicenseeDataAccess.GetSpecialSportBookVRAdjustmentCustomers(100).ToList();

            if (customerList.Any())
            {
                var customerListString = string.Join(",", customerList);

                LogService.Info("VR Setting - Special Licensee - Special SportBook - Raw Customer list: " + customerListString);

                var (passedPerfCheckCustomers, unpassedPerfCheckCustomers) = this.specialLicenseeDataAccess.VerifyVRAdjustmentCustomers(customerListString);

                AdjustVRPassedCustomers(passedPerfCheckCustomers, 100, punterChunkSize);

                this.punterSportsbookUpdated = passedPerfCheckCustomers.Count();

                UpdateStatusUnpassedCustomers(unpassedPerfCheckCustomers, 100, punterChunkSize);

            }

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
                this.specialLicenseeDataAccess.UpdateVRFollowUpline(string.Join(",", customerChunks[i]), sportType);
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
                this.specialLicenseeDataAccess.UpdateVRAutoAdjustmentStatus(string.Join(",", customerChunks[i]), sportType);
            }
        }
       
    }
}
