namespace Fanex.NAP.SPU.VR.Features.SpecialLicensee
{
    using Fanex.Logging;
    using Fanex.NAP.SPU.CTS.Shares;
    using Fanex.NAP.SPU.VR.Features.SpecialLicensee.Model;
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using System.Linq;

    public interface ISpecialLicenseeService
    {
        int PunterBasketballUpdated { get; }

        int PunterSportsbookUpdated { get; }

        ISpecialLicenseeService AutoAdjustForBasketball(int punterChunkSize, bool isTest);

        ISpecialLicenseeService AutoAdjustForSpecialSportsbook(int punterChunkSize, bool isTest);
        
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

        public ISpecialLicenseeService AutoAdjustForBasketball(int punterChunkSize, bool isTest)
        {
            var passedPerfCheckCustomerList = Enumerable.Empty<PassedPerfCheckCustomerModel>();
            var customerList = this.specialLicenseeDataAccess.GetSpecialSportBookVRAdjustmentCustomers(99, isTest).ToList();
            
            if (customerList.Any())
            {
                var customerListString = string.Join(",", customerList);

                var (custUpdFirstBetDateSportsbook,
                    custUpdFirstBetDateBasketball,
                    passedPerfCheckCustomers, 
                    unpassedPerfCheckCustomers) = this.specialLicenseeDataAccess.VerifyVRAdjustmentCustomers(customerListString, isTest);            

                if (custUpdFirstBetDateBasketball != null && custUpdFirstBetDateBasketball.Any())
                {
                    specialLicenseeDataAccess.UpdateFirstBetDateCustomers(custUpdFirstBetDateBasketball, isTest);
                }

                if (passedPerfCheckCustomers != null && passedPerfCheckCustomers.Any())
                {
                    passedPerfCheckCustomerList = passedPerfCheckCustomers.Select(x => new PassedPerfCheckCustomerModel
                    {
                        CustID = x.CustID,
                        WL = x.MemberWinloss,
                        TotalTO = x.MemberTotalTurnover,
                        AIOTO = x.MemberAIOTurnover
                    });

                    AdjustVRPassedCustomers(passedPerfCheckCustomerList, 99, punterChunkSize, isTest);
                }

                this.punterBasketballUpdated = passedPerfCheckCustomerList.Count();

                UpdateStatusUnpassedCustomers(unpassedPerfCheckCustomers, 99, punterChunkSize, isTest);
            }

            return this;
        }

        public ISpecialLicenseeService AutoAdjustForSpecialSportsbook(int punterChunkSize, bool isTest)
        {
            var passedPerfCheckCustomerList = Enumerable.Empty<PassedPerfCheckCustomerModel>();

            var customerList = this.specialLicenseeDataAccess.GetSpecialSportBookVRAdjustmentCustomers(100, isTest).ToList();
          
            if (customerList.Any())
            {
                var customerListString = string.Join(",", customerList);

                LogService.Info($"VR - Special Licensee SportBook - IsTest { isTest }, Raw Customer list: {customerListString}");

                var (custUpdFirstBetDateSportsbook,
                    custUpdFirstBetDateBasketball,
                    passedPerfCheckCustomers, 
                    unpassedPerfCheckCustomers) = this.specialLicenseeDataAccess.VerifyVRAdjustmentCustomers(customerListString, isTest);

                if (custUpdFirstBetDateSportsbook !=null && custUpdFirstBetDateSportsbook.Any())
                {
                    specialLicenseeDataAccess.UpdateFirstBetDateCustomers(custUpdFirstBetDateSportsbook, isTest);
                }

                if (passedPerfCheckCustomers != null && passedPerfCheckCustomers.Any())
                {
                    passedPerfCheckCustomerList = passedPerfCheckCustomers.Select(x => new PassedPerfCheckCustomerModel
                    {
                        CustID = x.CustID,
                        WL = x.MemberWinloss,
                        TotalTO = x.MemberTotalTurnover,
                        AIOTO = x.MemberAIOTurnover
                    });

                    AdjustVRPassedCustomers(passedPerfCheckCustomerList, 100, punterChunkSize, isTest);
                }
                else
                {
                    LogService.Info($"VR - Special Licensee SportBook - passedPerfCheckCustomers empty");
                }

                this.punterSportsbookUpdated = passedPerfCheckCustomerList.Count();

                UpdateStatusUnpassedCustomers(unpassedPerfCheckCustomers, 100, punterChunkSize, isTest);
            }

            return this;
        }

        private void AdjustVRPassedCustomers(IEnumerable<PassedPerfCheckCustomerModel> customerList, int sportType, int punterChunkSize, bool isTest)
        {
            var customerListString = string.Join(",", customerList.Select(x => x.CustID));

            if (sportType == 99)
                LogService.Info("VR - Special Licensee - Other Sports - Customers have passed Performance Check: " + customerListString);
            else
                LogService.Info("VR - Special Licensee - Special SportBook - Customers have passed Performance Check: " + customerListString);

            var customerChunks = Utilities.ChunkList(customerList, punterChunkSize).ToList();

            var custXml = string.Empty;

            for (int i = 0; i < customerChunks.Count(); i++)
            {
                foreach (var item in customerChunks[i])
                {
                    object[] objParams =
                    {
                        item.CustID,
                        item.WL,
                        item.TotalTO,
                        item.AIOTO
                    };

                    custXml += Utilities.GenerateXmlForCustomerUpdFollowParent(objParams);
                }

                custXml = Utilities.GenerateXmlForVRRoot(custXml);

                this.specialLicenseeDataAccess.UpdateVRFollowUpline(custXml, sportType, isTest);

                LogService.Info($"VR - Special Licensee - Other Sports - UpdateVRFollowUpline: {custXml}");
            }
        }

        private void UpdateStatusUnpassedCustomers(IEnumerable<int> customerList, int sportType, int punterChunkSize, bool isTest)
        {
            var customerListString = string.Join(",", customerList);

            if (sportType == 99)
                LogService.Info("VR - Special Licensee - Customers have NOT passed Performance Check: " + customerListString);
            else
                LogService.Info("VR - Special Licensee - Customers have NOT passed Performance Check: " + customerListString);

            var customerChunks = Utilities.ChunkList(customerList, punterChunkSize).ToList();

            for (int i = 0; i < customerChunks.Count(); i++)
            {
                this.specialLicenseeDataAccess.UpdateVRAutoAdjustmentStatus(string.Join(",", customerChunks[i]), sportType, isTest);

                LogService.Info($"VR - Special Licensee - Other Sports - UpdateStatusUnpassedCustomers: {string.Join(",", customerChunks[i])}");
            }
        }
       
    }
}
