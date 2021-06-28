namespace Fanex.NAP.SPU.VR.JobService.Services
{
    using Fanex.NAP.SPU.VR.Features.SpecialLicensee;
    using Sentry;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public interface ISpecialLicenseeJobService
    {
        Task<bool> Execute();
    }

    public class SpecialLicenseeJobService : ISpecialLicenseeJobService
    {
        private readonly ISentryClient sentryClient;
        private readonly IAppSettings appSettings;
        private readonly ISpecialLicenseeService specialLicenseeService;
        private string notifiedMessage = string.Empty;

        public SpecialLicenseeJobService(ISentryClient sentryClient, IAppSettings appSettings, ISpecialLicenseeService specialLicenseeService)
        {
            this.sentryClient = sentryClient;
            this.appSettings = appSettings;
            this.specialLicenseeService = specialLicenseeService;
        }

        public Task<bool> Execute()
        {
            try
            {
                UpdateVRForBasketball();
                //UpdateVRForSpecialSportsbook();
                //NotifyMessage();
            }
            catch (Exception ex)
            {
                sentryClient.CaptureException(ex);

                return Task.FromResult(false);
            }

            return Task.FromResult(true);
        }

        private void UpdateVRForBasketball()
        {
            IOthersportsService othersportsService = new OthersportsService();

            othersportsService.AutoAdjustVirtualSettings();
        }

        private void AdjustVRPassedCustomers(IEnumerable<int> customerList, int sportType)
        {

        }

        private void UpdateStatusUnpassedCustomers(IEnumerable<int> customerList, int sportType)
        {

        }
    }
}
