namespace Fanex.NAP.SPU.VR.Tests.Features.BetLimit
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class BetLimitExpectedData
    {
        public static IEnumerable<int> ExpectedCustomers
        {
            get
            {
                return new List<int> { 123, 456, 789 };
            }
        }

        public static IEnumerable<object[]> ExpectedRemoveBetLimit
        {
            get 
            {
                return new List<object[]> {
                    new object[] { new List<int> { 123, 456 }, 100 },
                    new object[] { new List<int> { 789 }, 50 }
                };
            }
        }    

        public static (IEnumerable<int>, IEnumerable<int>) ExpectedVerifyCustomers
        {
            get
            {
                var custs1 = new List<int> { 123 };
                var custs2 = new List<int> { 456 };

                return (custs1, custs2);
            }
        }
        
    }
}
