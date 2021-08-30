namespace Fanex.NAP.SPU.VR.Tests.Features.Adjust3M.Criteria
{
    using Fanex.NAP.SPU.VR.Features.Adjust3M.Criteria;  
    using Xunit;

    public class GetCurrentCustomerVirtualRateCriteriaTest
    {
        private readonly GetCurrentCustomerVirtualRateCriteria criteria;

        public GetCurrentCustomerVirtualRateCriteriaTest()
        {
            criteria = new GetCurrentCustomerVirtualRateCriteria();
        }

        [Fact]
        public void GetSettingKey_Any_ReturnRightSPName()
        {
            // Arrange
            var expected = "NAP_Report_CustMngt_GetCustomersVirtualRate";

            // Act
            var spName = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, spName);
        }
    }
}
