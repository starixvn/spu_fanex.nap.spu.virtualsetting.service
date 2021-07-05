namespace Fanex.NAP.SPU.VR.Tests.Features.Adjust3M
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public static class Adjust3MExpectedData
    {
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
        
        public static IEnumerable<object[]> ExpectedGetWinningPunterSoccerList
        {
            get
            {
                return new List<object[]> {
                    new object[] { "12Bet", "111", 100 },
                    new object[] { "F88", "222", 2 }
                };
            }
        }

        public static IEnumerable<object[]> ExpectedGetWinningPunterList
        {
            get
            {
                return new List<object[]> {
                    new object[] { "12Bet", "111", 100 },
                    new object[] { "F88", "222", 2 }
                };
            }
        }

        public static IEnumerable<object[]> ExpectedFilterPunterBaseOnCurrentVirtualRate
        {
            get
            {
                return new List<object[]> {
                    new object[] { 1, true },
                    new object[] { 1, false }
                };
            }
        }
        
        public static IEnumerable<object[]> ExpectedAdjustVirtualSettingForSuperProPunter
        {
            get
            {
                return new List<object[]> {
                    new object[] { 74562, "192.168.1.1", 500, 1 },
                    new object[] { 74562, "", 500, 0 },
                };
            }
        }

        public static IEnumerable<object[]> ExpectedAdjustVirtualSettingForNormal
        {
            get
            {
                return new List<object[]> {
                    new object[] { 74562, "192.168.1.1", 500, 1 },
                    new object[] { 74562, "", 500, 0 },
                };
            }
        }

    }
}
