namespace Fanex.NAP.SPU.VR.Tests.Features.MemberBigStake
{
    using Fanex.NAP.SPU.VR.Features.MemberBigStake;
    using Fanex.NAP.SPU.VR.Features.MemberBigStake.Model;
    using FluentAssertions;
    using NSubstitute;
    using System;
    using System.Collections.Generic;
    using Xunit;

    public class MemberBigStakeServiceTest
    {
        private readonly IMemberBigStakeDataAccess dataAccess;
        private readonly IMemberBigStakeService service;

        public MemberBigStakeServiceTest()
        {
            dataAccess = Substitute.For<IMemberBigStakeDataAccess>();
            service = new MemberBigStakeService(dataAccess);
        }

        [Fact]
        public void InsertMemberBigStake_WhenSuccess()
        {
            // Act
            service.InsertMemberBigStake(false);

            // Assert
            dataAccess.Received(1).InsertMemberBigStake(false);
        }

        [Fact]
        public void GetMemberBigStakePerformance_WhenSuccess()
        {
            // Arrange
            var expectedResult = MemberBigStakeExpectedData.ExpectedMemberBigStakeModel;
            dataAccess.GetMemberBigStakePerformance(false).Returns(expectedResult);

            // Act
            var result = service.GetMemberBigStakePerformance(false);

            // Assert
            result.Should().BeEquivalentTo(expectedResult);
            dataAccess.Received(1).GetMemberBigStakePerformance(false);
        }

        [Theory]
        [MemberData(nameof(MemberBigStakeExpectedData.ExpectedUpdateMemberBigStakeFirstBetDate), MemberType = typeof(MemberBigStakeExpectedData))]
        public void UpdateMemberBigStakeFirstBetDate_WhenSuccess(List<long> customerList, int type, DateTime betDate, bool isTest)
        {
            // Act            
            service.UpdateMemberBigStakeFirstBetDate(customerList, type, betDate, isTest);

            // Assert
            dataAccess.Received(1).UpdateMemeberBigStakeFirstBetDate(
                Arg.Is<string>(s => s == string.Join(",", customerList)),
                type,
                betDate,
                isTest);
        }

        [Fact]
        public void UpdateMemberBigStakeFirstBetDate_WhenCustomerListIsEmpty_ShouldCallDataAccessWithEmptyString()
        {
            // Arrange
            var emptyList = new List<long>();
            var type = 1;
            var betDate = DateTime.Now;
            var isTest = false;

            // Act
            service.UpdateMemberBigStakeFirstBetDate(emptyList, type, betDate, isTest);

            // Assert
            dataAccess.Received(1).UpdateMemeberBigStakeFirstBetDate("", type, betDate, isTest);
        }       

        [Theory]
        [MemberData(nameof(MemberBigStakeExpectedData.ExpectedUpdateWhitelistMemberBigStake), MemberType = typeof(MemberBigStakeExpectedData))]
        public void UpdateWhitelistMemberBigStake_WhenSuccess(IEnumerable<MemberBigStakePerformanceStatusModel> customerList, int type)
        {
            // Act
            service.UpdateWhitelistMemberBigStake(customerList, type, false);

            // Assert
            dataAccess.Received(1).UpdateWhitelistMemberBigStake(
                Arg.Is<string>(s => !string.IsNullOrEmpty(s)), // Verify XML is generated
                type,
                false);
        }

        [Fact]
        public void UpdateWhitelistMemberBigStake_WhenCustomerListIsEmpty_ShouldCallDataAccessWithEmptyXml()
        {
            // Arrange
            var emptyList = new List<MemberBigStakePerformanceStatusModel>();
            var type = 1;

            // Act
            service.UpdateWhitelistMemberBigStake(emptyList, type, false);

            // Assert
            dataAccess.Received(1).UpdateWhitelistMemberBigStake(
                Arg.Is<string>(s => s == "<Root></Root>"), // Empty XML root
                type,
                false);
        }      

        [Fact]
        public void GetBigCreditMemberForAutomail_WhenSuccess()
        {
            // Arrange
            var expectedResult = MemberBigStakeExpectedData.ExpectedGetBigCreditMemberForAutomail;
            dataAccess.GetBigCreditMemberForAutomail().Returns(expectedResult);

            // Act
            var result = service.GetBigCreditMemberForAutomail();

            // Assert
            result.Should().BeEquivalentTo(expectedResult);
            dataAccess.Received(1).GetBigCreditMemberForAutomail();
        }
    }
}
