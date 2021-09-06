namespace Fanex.NAP.SPU.VR.Tests.Features.Adjust3M.Criteria
{
    using Fanex.NAP.SPU.VR.Features.Adjust3M.Criteria;
    using Xunit;

    public class GetProPrunterSoccerCriteriaTest
    {
        private readonly GetProPrunterSoccerCriteria criteria;

        public GetProPrunterSoccerCriteriaTest()
        {
            criteria = new GetProPrunterSoccerCriteria();
        }

        [Fact]
        public void GetSettingKey_Any_ReturnRightSPName()
        {
            // Arrange
            var expected = "NAP_Report_CustMngt_GetProPunters_Soccer";

            // Act
            var spName = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, spName);
        }
    }
}
