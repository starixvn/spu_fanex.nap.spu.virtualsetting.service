namespace Fanex.NAP.SPU.VR.Features.Adjust3M
{
    using Fanex.NAP.SPU.VR.Features.Adjust3M.Entities;
    using Fanex.NAP.SPU.VR.Features.Adjust3M.Model;
    using System;
    using System.Collections.Generic;

    public class VRSportsbookService : ICustomerSettingsService
    {
        private IPunterService _punterService;

        public int PuntersUpdated { get; set; }

        public VRSportsbookService(string siteIds, string superIds, DateTime adjustDate, double virtualRateThresholdForWinning)
        {
            var model = new VirtualSettingModel
            {
                ServiceName = "Sportsbook",
                AdjustDate = adjustDate,
                SportGroupID = 0,
                SiteIds = siteIds,
                SuperIds = superIds,
                VirtualRateThreshold = virtualRateThresholdForWinning,
                FailedUpdatePunterList = new List<PunterEntity>(),
            };

            var dataAccess = new VirtualSettingDataAccess();

            _punterService = new PunterService(model, dataAccess);
        }

        public ICustomerSettingsService AutoAdjustVirtualSettings(UpdateVirtualSettingEntity updateVirtualSetting
            , double newVirtualMultiplierForSuperProPunter
            , double NewVirtualMultiplierForWinning)
        {
            _punterService.GetWinningPunterList()
                    .FilterPunterBaseOnCurrentVirtualRate(0, true)
                    .AdjustVirtualSettingForNormal(updateVirtualSetting, NewVirtualMultiplierForWinning);

            PuntersUpdated = _punterService.CountAffected();

            return this;
        }
    }
}
