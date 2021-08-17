namespace Fanex.NAP.SPU.VR.Features.Adjust3M
{
    using Fanex.NAP.SPU.VR.Features.Adjust3M.Entities;
    using Fanex.NAP.SPU.VR.Features.Adjust3M.Model;
    using System;
    using System.Collections.Generic;

    public interface ISoccerService
    {        
        ISoccerService InitModel(string siteIds, string superIds, DateTime adjustDate, double virtualRateThresholdForWinning);

        ISoccerService AutoAdjust(UpdateVirtualSettingEntity updateVirtualSetting
           , double newVirtualMultiplierForSuperProPunter
           , double NewVirtualMultiplierForWinning);

        int GetCountAffected();
    }

    public class SoccerService : ISoccerService
    {
        private IPunterService punterService;
        private readonly IAdjust3MDataAccess dataAccess;                

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

        public ISoccerService AutoAdjust(UpdateVirtualSettingEntity updateVirtualSetting
            , double newVirtualMultiplierForSuperProPunter
            , double NewVirtualMultiplierForWinning)
        {
            if (this.punterService != null)
            {
                this.punterService.GetWinningPunterSoccerList()
                   .FilterPunterBaseOnCurrentVirtualRate(1, false)
                   .AdjustVirtualSettingForSuperProPunter(updateVirtualSetting, newVirtualMultiplierForSuperProPunter)
                   .AdjustVirtualSettingForNormal(updateVirtualSetting, NewVirtualMultiplierForWinning);                                
            }

            return this;
        }

        public int GetCountAffected()
        {
            if(this.punterService.CountAffected() != 0)
            {
                return this.punterService.CountAffected();
            }

            return 0;
        }
    }
}
