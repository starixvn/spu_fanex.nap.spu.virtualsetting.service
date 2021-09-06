namespace Fanex.NAP.SPU.VR.Tests.Features.Adjust3M.Criteria
{
    using Fanex.NAP.SPU.VR.Features.Adjust3M.Criteria;   
    using Xunit;

    public class GetBAAccountsCriteriaTest
    {
        private readonly GetBAAccountsCriteria criteria;

        public GetBAAccountsCriteriaTest()
        {
            criteria = new GetBAAccountsCriteria();
        }

        [Fact]
        public void GetSettingKey_Any_ReturnRightSPName()
        {
            // Arrange
            var expected = "Acc_CustomerVirtualRateSelectBA";

            // Act
            var spName = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, spName);
        }
    }
}
