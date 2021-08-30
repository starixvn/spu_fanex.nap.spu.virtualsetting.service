namespace Fanex.NAP.SPU.VR.Tests.Features.Adjust3M.Criteria
{
    using Fanex.NAP.SPU.VR.Features.Adjust3M.Criteria;   
    using Xunit;

    public class UpdateVirtualSettingCriteriaTest
    {
        private readonly UpdateVirtualSettingCriteria criteria;

        public UpdateVirtualSettingCriteriaTest()
        {
            criteria = new UpdateVirtualSettingCriteria();
        }

        [Fact]
        public void GetSettingKey_Any_ReturnRightSPName()
        {
            // Arrange
            var expected = "Acc_CustomerUpdVirtualMultiplier_Multiple";

            // Act
            var spName = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, spName);
        }
    }
}
