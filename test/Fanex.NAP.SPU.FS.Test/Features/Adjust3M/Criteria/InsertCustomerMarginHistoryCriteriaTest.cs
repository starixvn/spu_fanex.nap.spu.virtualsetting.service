namespace Fanex.NAP.SPU.VR.Tests.Features.Adjust3M.Criteria
{
    using Fanex.NAP.SPU.VR.Features.Adjust3M.Criteria;
    using Xunit;

    public class InsertCustomerMarginHistoryCriteriaTest
    {
        private readonly InsertCustomerMarginHistoryCriteria criteria;

        public InsertCustomerMarginHistoryCriteriaTest()
        {
            criteria = new InsertCustomerMarginHistoryCriteria();
        }

        [Fact]
        public void GetSettingKey_Any_ReturnRightSPName()
        {
            // Arrange
            var expected = "NAP_UserLog_InsertCustomerMarginHistory";

            // Act
            var spName = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, spName);
        }
    }
}
