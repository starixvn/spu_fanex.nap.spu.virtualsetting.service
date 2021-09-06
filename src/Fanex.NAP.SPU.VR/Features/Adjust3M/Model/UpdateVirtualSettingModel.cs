namespace Fanex.NAP.SPU.VR.Features.Adjust3M.Model
{
    using Fanex.NAP.SPU.VR.Features.Adjust3M.Entities;
    using System;
    using System.Collections.Generic;

    public class UpdateVirtualSettingModel
    {
        public int DummyUser { get; set; }

        public string IP { get; set; }

        public int PunterChunkSize { get; set; }

        public DateTime AdjustDate { get; set; }

        public IEnumerable<PunterEntity> Punters { get; set; }
    }
}
