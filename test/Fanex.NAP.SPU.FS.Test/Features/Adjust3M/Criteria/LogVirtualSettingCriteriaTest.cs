namespace Fanex.NAP.SPU.VR.Tests.Features.Adjust3M.Criteria
{
    using Fanex.NAP.SPU.VR.Features.Adjust3M.Criteria;
    using Xunit;

    public class LogVirtualSettingCriteriaTest
    {
        private readonly LogVirtualSettingCriteria criteria;

        public LogVirtualSettingCriteriaTest()
        {
            criteria = new LogVirtualSettingCriteria();
        }

        [Fact]
        public void GetSettingKey_Any_ReturnRightSPName()
        {
            // Arrange
            var expected = "NAP_UserLog_UpdateVirtualRateSettings";

            // Act
            var spName = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, spName);
        }
    }
}
