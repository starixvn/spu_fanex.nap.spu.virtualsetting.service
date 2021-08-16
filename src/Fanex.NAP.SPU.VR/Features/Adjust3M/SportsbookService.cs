namespace Fanex.NAP.SPU.VR.Features.Adjust3M
{
    using Fanex.NAP.SPU.VR.Features.Adjust3M.Entities;
    using Fanex.NAP.SPU.VR.Features.Adjust3M.Model;
    using System;
    using System.Collections.Generic;

    public interface ISportsbookService
    {
        int PuntersUpdated { get; set; }

        ISportsbookService InitModel(string siteIds, string superIds, DateTime adjustDate, double virtualRateThresholdForWinning);

        ISportsbookService AutoAdjustVirtualSettings(UpdateVirtualSettingEntity updateVirtualSetting, double NewVirtualMultiplierForWinning);
    }

    public class SportsbookService : ISportsbookService
    {
        private IPunterService punterService;
        private IAdjust3MDataAccess dataAccess;

        public int PuntersUpdated { get; set; }

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

        public ISportsbookService AutoAdjustVirtualSettings(UpdateVirtualSettingEntity updateVirtualSetting, double NewVirtualMultiplierForWinning)
        {
            if (this.punterService != null)
            {
                this.punterService.GetWinningPunterList()
                   .FilterPunterBaseOnCurrentVirtualRate(0, true)
                   .AdjustVirtualSettingForNormal(updateVirtualSetting, NewVirtualMultiplierForWinning);

                PuntersUpdated = this.punterService.CountAffected();
            }
           
            return this;
        }
    }
}
