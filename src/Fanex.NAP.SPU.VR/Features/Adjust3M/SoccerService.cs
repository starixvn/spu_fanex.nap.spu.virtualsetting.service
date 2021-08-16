namespace Fanex.NAP.SPU.VR.Features.Adjust3M
{
    using Fanex.NAP.SPU.VR.Features.Adjust3M.Entities;
    using Fanex.NAP.SPU.VR.Features.Adjust3M.Model;
    using System;
    using System.Collections.Generic;

    public interface ISoccerService
    {
        int PuntersUpdated { get; }

        ISoccerService InitModel(string siteIds, string superIds, DateTime adjustDate, double virtualRateThresholdForWinning);

        ISoccerService AutoAdjustVirtualSettings(UpdateVirtualSettingEntity updateVirtualSetting
           , double newVirtualMultiplierForSuperProPunter
           , double NewVirtualMultiplierForWinning);
    }

    public class SoccerService : ISoccerService
    {
        private IPunterService punterService;
        private IAdjust3MDataAccess dataAccess;

        public int PuntersUpdated { get; set; }

        public SoccerService(IAdjust3MDataAccess dataAccess)
        {
            this.dataAccess = dataAccess;
        }

        public ISoccerService InitModel(string siteIds, string superIds, DateTime adjustDate, double virtualRateThresholdForWinning)
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

            this.punterService = new PunterService(this.dataAccess, model, true);

            return this;
        }

        public ISoccerService AutoAdjustVirtualSettings(UpdateVirtualSettingEntity updateVirtualSetting
            , double newVirtualMultiplierForSuperProPunter
            , double NewVirtualMultiplierForWinning)
        {
            if (this.punterService != null)
            {
                this.punterService.GetWinningPunterSoccerList()
                   .FilterPunterBaseOnCurrentVirtualRate(1, false)
                   .AdjustVirtualSettingForSuperProPunter(updateVirtualSetting, newVirtualMultiplierForSuperProPunter)
                   .AdjustVirtualSettingForNormal(updateVirtualSetting, NewVirtualMultiplierForWinning);

                PuntersUpdated = this.punterService.CountAffected();
            }

            return this;
        }
    }
}
