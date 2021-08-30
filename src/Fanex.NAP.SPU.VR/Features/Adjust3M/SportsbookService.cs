namespace Fanex.NAP.SPU.VR.Features.Adjust3M
{
    using Fanex.NAP.SPU.VR.Features.Adjust3M.Entities;
    using Fanex.NAP.SPU.VR.Features.Adjust3M.Model;
    using System;
    using System.Collections.Generic;

    public interface ISportsbookService
    {
        int GetCountAffected();

        ISportsbookService InitModel(string siteIds, string superIds, DateTime adjustDate, double virtualRateThresholdForWinning);

        ISportsbookService AutoAdjust(UpdateVirtualSettingEntity updateVirtualSetting, double NewVirtualMultiplierForWinning);
    }

    public class SportsbookService : ISportsbookService
    {
        private IPunterService punterService;
        private readonly IAdjust3MDataAccess dataAccess;                

        public SportsbookService(IAdjust3MDataAccess dataAccess)
        {
            this.dataAccess = dataAccess;
        }

        public ISportsbookService InitModel(string siteIds, string superIds, DateTime adjustDate, double virtualRateThresholdForWinning)
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
            
            this.punterService = new PunterService(this.dataAccess, model, false);

            return this;                 
        }

        public ISportsbookService AutoAdjust(UpdateVirtualSettingEntity updateVirtualSetting, double NewVirtualMultiplierForWinning)
        {
            if (this.punterService != null)
            {
                this.punterService.GetWinningPunterList()
                   .FilterPunterBaseOnCurrentVirtualRate(0, true)
                   .AdjustVirtualSettingForNormal(updateVirtualSetting, NewVirtualMultiplierForWinning);                                
            }
           
            return this;
        }

        public int GetCountAffected()
        {
            return this.punterService.CountAffected();
        }
    }
}
