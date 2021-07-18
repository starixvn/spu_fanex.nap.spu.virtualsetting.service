namespace Fanex.NAP.SPU.VR.Features.Adjust3M.Model
{
    using Fanex.NAP.SPU.VR.Features.Adjust3M.Entities;
    using System;
    using System.Collections.Generic;

    public class VirtualSettingModel
    {
        public DateTime AdjustDate { get; set; }

        public byte SportGroupID { get; set; }

        public string SiteIds { get; set; }

        public string SuperIds { get; set; }

        public string Ip { get; set; }

        public int DummyUser { get; set; }

        public double VirtualRateThreshold { get; set; }        

        public List<PunterEntity> FailedUpdatePunterList { get; set; }

        public PunterEqualityComparer PunterEqualityComparer { get; set; }
    }
}
