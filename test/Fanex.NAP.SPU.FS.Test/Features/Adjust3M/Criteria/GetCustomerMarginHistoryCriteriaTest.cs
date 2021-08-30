namespace Fanex.NAP.SPU.VR.Tests.Features.Adjust3M.Criteria
{
    using Fanex.NAP.SPU.VR.Features.Adjust3M.Criteria;
    using Xunit;

    public class GetCustomerMarginHistoryCriteriaTest
    {
        private readonly GetCustomerMarginHistoryCriteria criteria;

        public GetCustomerMarginHistoryCriteriaTest()
        {
            criteria = new GetCustomerMarginHistoryCriteria();
        }

        [Fact]
        public void GetSettingKey_Any_ReturnRightSPName()
        {
            // Arrange
            var expected = "NAP_Report_CustMngt_GetCustomerMarginHistory";

            // Act
            var spName = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, spName);
        }
    }
}
