namespace Fanex.NAP.SPU.VR.Features.Adjust3M.Model
{
    using Fanex.NAP.SPU.VR.Features.Adjust3M.Entities;
    using System;
    using System.Collections.Generic;

    public class VirtualSettingModel
    {
        public string ServiceName { get; set; }

        public DateTime AdjustDate { get; set; }

        public byte SportGroupID { get; set; }

        public string SiteIds { get; set; }

        public string SuperIds { get; set; }

        public double VirtualRateThreshold { get; set; }

        public bool IsSuccessful { get; set; }

        public List<PunterEntity> FailedUpdatePunterList { get; set; }

        public PunterEqualityComparer PunterEqualityComparer { get; set; }
    }
}
