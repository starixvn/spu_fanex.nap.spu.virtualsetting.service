namespace Fanex.NAP.SPU.VR.Features.Adjust3M.Model
{
    using Fanex.NAP.SPU.VR.Features.Adjust3M.Entities;
    using System.Collections.Generic;

    public class PunterEqualityComparer : IEqualityComparer<PunterEntity>
    {
        public bool Equals(PunterEntity x, PunterEntity y)
        {
            if (x == null && y == null)
            {
                return true;
            }
            else if (x == null | y == null)
            {
                return false;
            }
            else if (x.CustID == y.CustID)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public int GetHashCode(PunterEntity obj)
        {
            return obj.CustID.GetHashCode();
        }
    }
}
