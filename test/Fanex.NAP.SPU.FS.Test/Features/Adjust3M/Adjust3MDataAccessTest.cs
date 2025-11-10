namespace Fanex.NAP.SPU.VR.Tests.Features.Adjust3M
{
    using Fanex.NAP.SPU.VR.Features.Adjust3M;
    using Fanex.NAP.SPU.VR.Features.Adjust3M.Entities;
    using Fanex.NAP.SPU.CTS.Shares;
    using FluentAssertions;
    using NSubstitute;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Xunit;

    /// <summary>
    /// Unit tests for Adjust3MDataAccess class
    /// </summary>
    public class Adjust3MDataAccessTest
    {
        private readonly Adjust3MDataAccess dataAccess;

        public Adjust3MDataAccessTest()
        {
            dataAccess = new Adjust3MDataAccess();
        }

        private PunterEntity CreatePunterEntity(int custId, bool isSuperPro = false, short sportType = 1)
        {
            return new PunterEntity
            {
                CustID = custId,
                IsSuperProPunters = isSuperPro,
                SportType = sportType,
                OldVirtualRate = 1.0,
                NewVirtualRate = 2.0,
                SiteID = 1,
                ReportDate = DateTime.Now,
                SRecommend = 0,
                MRecommend = 0,
                Recommend = 0,
                IsBA = false
            };
        }

        private CustomerVirtualRateEntity CreateCustomerVirtualRateEntity(int custId, double vrSoccer = 1.0, double vrOtherSports = 1.0)
        {
            return new CustomerVirtualRateEntity
            {
                CustID = custId,
                VRSoccer = vrSoccer,
                VROtherSports = vrOtherSports
            };
        }

        private CustomerMarginEntity CreateCustomerMarginEntity(int custId, int reportYear, int reportMonth, decimal turnover, decimal winloss, int betCount)
        {
            return new CustomerMarginEntity
            {
                CustId = custId,
                ReportYear = reportYear,
                ReportMonth = reportMonth,
                Turnover = turnover,
                Winloss = winloss,
                BetCount = betCount
            };
        }

        private LicenseeBAEntity CreateLicenseeBAEntity(int custId)
        {
            return new LicenseeBAEntity
            {
                CustID = custId
            };
        }

        #region Constructor Tests

        [Fact]
        public void Constructor_ShouldInitializeCorrectly()
        {
            // Act
            var dataAccess = new Adjust3MDataAccess();

            // Assert
            dataAccess.Should().NotBeNull();
        }

        #endregion                      
      
        #region Private Method Tests       

        [Fact]
        public void GetCustomerMarginHistory_WithEmptyPunters_ShouldReturnCustomerMargins()
        {
            // Arrange
            var punters = new List<PunterEntity>();
            var sportGroupID = (byte)1;
            var numOfMonth = 12;

            var method = typeof(Adjust3MDataAccess).GetMethod("GetCustomerMarginHistory", BindingFlags.NonPublic | BindingFlags.Instance);

            // Act
            var result = method.Invoke(dataAccess, new object[] { punters, sportGroupID, numOfMonth });

            // Assert
            result.Should().NotBeNull();
        }      
              
        #endregion       

        #region Method Signature Tests

        [Fact]
        public void GetBACustIds_ShouldHaveCorrectSignature()
        {
            // Arrange
            var method = typeof(Adjust3MDataAccess).GetMethod("GetBACustIds");

            // Assert
            method.Should().NotBeNull();
            method.ReturnType.Should().Be(typeof(IEnumerable<int>));
            method.GetParameters().Should().HaveCount(1);
            method.GetParameters()[0].ParameterType.Should().Be(typeof(string));
        }

        [Fact]
        public void GetCurrentCustomerVirtualRate_ShouldHaveCorrectSignature()
        {
            // Arrange
            var method = typeof(Adjust3MDataAccess).GetMethod("GetCurrentCustomerVirtualRate");

            // Assert
            method.Should().NotBeNull();
            method.ReturnType.Should().Be(typeof(IEnumerable<CustomerVirtualRateEntity>));
            method.GetParameters().Should().HaveCount(1);
            method.GetParameters()[0].ParameterType.Should().Be(typeof(string));
        }

        [Fact]
        public void GetWinningPunterSoccerList_ShouldHaveCorrectSignature()
        {
            // Arrange
            var method = typeof(Adjust3MDataAccess).GetMethod("GetWinningPunterSoccerList");

            // Assert
            method.Should().NotBeNull();
            method.ReturnType.Should().Be(typeof(IEnumerable<PunterEntity>));
            method.GetParameters().Should().HaveCount(3);
            method.GetParameters()[0].ParameterType.Should().Be(typeof(string));
            method.GetParameters()[1].ParameterType.Should().Be(typeof(string));
            method.GetParameters()[2].ParameterType.Should().Be(typeof(double));
        }

        [Fact]
        public void GetWinningPunterList_ShouldHaveCorrectSignature()
        {
            // Arrange
            var method = typeof(Adjust3MDataAccess).GetMethod("GetWinningPunterList");

            // Assert
            method.Should().NotBeNull();
            method.ReturnType.Should().Be(typeof(IEnumerable<PunterEntity>));
            method.GetParameters().Should().HaveCount(3);
            method.GetParameters()[0].ParameterType.Should().Be(typeof(string));
            method.GetParameters()[1].ParameterType.Should().Be(typeof(string));
            method.GetParameters()[2].ParameterType.Should().Be(typeof(double));
        }

        [Fact]
        public void UpdatePunterVirtualSetting_ShouldHaveCorrectSignature()
        {
            // Arrange
            var method = typeof(Adjust3MDataAccess).GetMethod("UpdatePunterVirtualSetting");

            // Assert
            method.Should().NotBeNull();
            method.ReturnType.Should().Be(typeof(DataResult));
            method.GetParameters().Should().HaveCount(6);
            method.GetParameters()[0].ParameterType.Should().Be(typeof(IEnumerable<PunterEntity>));
            method.GetParameters()[1].ParameterType.Should().Be(typeof(double));
            method.GetParameters()[2].ParameterType.Should().Be(typeof(int));
            method.GetParameters()[3].ParameterType.Should().Be(typeof(string));
            method.GetParameters()[4].ParameterType.Should().Be(typeof(short));
            method.GetParameters()[5].ParameterType.Should().Be(typeof(string));
        }

        [Fact]
        public void LogPunterVirtualSetting_ShouldHaveCorrectSignature()
        {
            // Arrange
            var method = typeof(Adjust3MDataAccess).GetMethod("LogPunterVirtualSetting");

            // Assert
            method.Should().NotBeNull();
            method.ReturnType.Should().Be(typeof(DataResult));
            method.GetParameters().Should().HaveCount(4);
            method.GetParameters()[0].ParameterType.Should().Be(typeof(string));
            method.GetParameters()[1].ParameterType.Should().Be(typeof(IEnumerable<PunterEntity>));
            method.GetParameters()[2].ParameterType.Should().Be(typeof(int));
            method.GetParameters()[3].ParameterType.Should().Be(typeof(DateTime));
        }

        [Fact]
        public void InsertCustomerMarginHistory_ShouldHaveCorrectSignature()
        {
            // Arrange
            var method = typeof(Adjust3MDataAccess).GetMethod("InsertCustomerMarginHistory");

            // Assert
            method.Should().NotBeNull();
            method.ReturnType.Should().Be(typeof(DataResult));
            method.GetParameters().Should().HaveCount(5);
            method.GetParameters()[0].ParameterType.Should().Be(typeof(IEnumerable<PunterEntity>));
            method.GetParameters()[1].ParameterType.Should().Be(typeof(byte));
            method.GetParameters()[2].ParameterType.Should().Be(typeof(int));
            method.GetParameters()[3].ParameterType.Should().Be(typeof(string));
            method.GetParameters()[4].ParameterType.Should().Be(typeof(DateTime));
        }

        [Fact]
        public void GetLicenseeBACusts_ShouldHaveCorrectSignature()
        {
            // Arrange
            var method = typeof(Adjust3MDataAccess).GetMethod("GetLicenseeBACusts");

            // Assert
            method.Should().NotBeNull();
            method.ReturnType.Should().Be(typeof(IEnumerable<LicenseeBAEntity>));
            method.GetParameters().Should().BeEmpty();
        }

        #endregion

        #region Interface Implementation Tests

        [Fact]
        public void ShouldImplementIAdjust3MDataAccess()
        {
            // Assert
            dataAccess.Should().BeAssignableTo<IAdjust3MDataAccess>();
        }

        [Fact]
        public void ShouldHaveAllInterfaceMethods()
        {
            // Arrange
            var interfaceType = typeof(IAdjust3MDataAccess);
            var implementationType = typeof(Adjust3MDataAccess);

            // Act
            var interfaceMethods = interfaceType.GetMethods();
            var implementationMethods = implementationType.GetMethods()
                .Where(m => m.DeclaringType == implementationType)
                .ToArray();

            // Assert
            foreach (var interfaceMethod in interfaceMethods)
            {
                var implementationMethod = implementationMethods.FirstOrDefault(m => 
                    m.Name == interfaceMethod.Name && 
                    m.GetParameters().Length == interfaceMethod.GetParameters().Length);
                
                implementationMethod.Should().NotBeNull($"Method {interfaceMethod.Name} should be implemented");
            }
        }

        #endregion       
    }
}