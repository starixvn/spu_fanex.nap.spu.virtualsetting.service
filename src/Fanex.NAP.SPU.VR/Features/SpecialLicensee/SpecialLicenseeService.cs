namespace Fanex.NAP.SPU.VR.Features.SpecialLicensee
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface ISpecialLicenseeService
    {
        IEnumerable<int> GetSpecialSportBookVRAdjustmentCustomers(int sportType);
        (IEnumerable<int>, IEnumerable<int>) VerifyVRAdjustmentCustomers(string customerList);
    }

    public class SpecialLicenseeService : ISpecialLicenseeService
    {
        public IEnumerable<int> GetSpecialSportBookVRAdjustmentCustomers(int sportType)
        {
            throw new NotImplementedException();
        }

        public (IEnumerable<int>, IEnumerable<int>) VerifyVRAdjustmentCustomers(string customerList)
        {
            throw new NotImplementedException();
        }
    }
}
