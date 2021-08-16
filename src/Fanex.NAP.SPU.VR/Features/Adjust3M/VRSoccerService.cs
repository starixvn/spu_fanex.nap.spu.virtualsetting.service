namespace Fanex.NAP.SPU.VR.Features.Adjust3M
{
    using Fanex.NAP.SPU.VR.Features.Adjust3M.Entities;
    using Fanex.NAP.SPU.VR.Features.Adjust3M.Model;
    using System;
    using System.Collections.Generic;

    public interface ICustomerSettingsService
    {
        int PuntersUpdated { get; }

        ICustomerSettingsService AutoAdjustVirtualSettings(UpdateVirtualSettingEntity updateVirtualSetting
           , double newVirtualMultiplierForSuperProPunter
           , double NewVirtualMultiplierForWinning);
    }

    public class VRSoccerService : ICustomerSettingsService
    {
        private IPunterService _punterService;

        public int PuntersUpdated { get; set; }

        public VRSoccerService(string siteIds, string superIds, DateTime adjustDate, double virtualRateThresholdForWinning)
        {
            var model = new VirtualSettingModel
            {
                ServiceName = "Soccer",
                AdjustDate = adjustDate,
                SportGroupID = 1,
                SiteIds = siteIds,
                SuperIds = superIds,
                VirtualRateThreshold = virtualRateThresholdForWinning,
                FailedUpdatePunterList = new List<PunterEntity>()
            };

            var dataAccess = new VirtualSettingDataAccess();

            _punterService = new PunterService(model, dataAccess, true);
        }

        public ICustomerSettingsService AutoAdjustVirtualSettings(UpdateVirtualSettingEntity updateVirtualSetting
            , double newVirtualMultiplierForSuperProPunter
            , double NewVirtualMultiplierForWinning)
        {
            _punterService.GetWinningPunterSoccerList()
                    .FilterPunterBaseOnCurrentVirtualRate(1, false)
                    .AdjustVirtualSettingForSuperProPunter(updateVirtualSetting, newVirtualMultiplierForSuperProPunter)
                    .AdjustVirtualSettingForNormal(updateVirtualSetting, NewVirtualMultiplierForWinning);

            PuntersUpdated = _punterService.CountAffected();

            return this;
        }
    }
}
