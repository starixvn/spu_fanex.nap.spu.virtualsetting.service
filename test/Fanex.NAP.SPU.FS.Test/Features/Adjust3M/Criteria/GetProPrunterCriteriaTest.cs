namespace Fanex.NAP.SPU.VR.Tests.Features.Adjust3M.Criteria
{
    using Fanex.NAP.SPU.VR.Features.Adjust3M.Criteria;
    using Xunit;

    public class GetProPrunterCriteriaTest
    {
        private readonly GetProPrunterCriteria criteria;

        public GetProPrunterCriteriaTest()
        {
            criteria = new GetProPrunterCriteria();
        }

        [Fact]
        public void GetSettingKey_Any_ReturnRightSPName()
        {
            // Arrange
            var expected = "NAP_Report_CustMngt_GetProPunters";

            // Act
            var spName = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, spName);
        }
    }
}
