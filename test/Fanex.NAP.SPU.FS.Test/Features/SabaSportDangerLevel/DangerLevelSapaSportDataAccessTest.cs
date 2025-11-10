namespace Fanex.NAP.SPU.VR.Tests.Features.SabaSportDangerLevel
{
    using Fanex.Data.Repository;
    using Fanex.NAP.SPU.VR.Features.SabaSportDangerLevel;
    using Fanex.NAP.SPU.VR.Shares;
    using FluentAssertions;
    using NSubstitute;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Xunit;

    /// <summary>
    /// Unit tests for DangerLevelSapaSportDataAccess class
    /// </summary>
    public class DangerLevelSapaSportDataAccessTest
    {
        private readonly IDynamicRepository repository;
        private readonly DangerLevelSapaSportDataAccess dataAccess;

        public DangerLevelSapaSportDataAccessTest()
        {
            repository = Substitute.For<IDynamicRepository>();
            dataAccess = new DangerLevelSapaSportDataAccess();
            
            // Use reflection to inject the mocked repository
            var repositoryField = typeof(DangerLevelSapaSportDataAccess).GetField("_repository", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            repositoryField?.SetValue(dataAccess, repository);
        }

        #region CheckDataSabaSportDangerLevelReady Tests

        [Fact]
        public void CheckDataSabaSportDangerLevelReady_WhenDataIsReady_ShouldReturnOne()
        {
            // Arrange
            var expectedResult = new List<int> { 1 };
            repository.Fetch<int>(Arg.Any<ICriteria>()).Returns(expectedResult);

            // Act
            var result = dataAccess.CheckDataSabaSportDangerLevelReady();

            // Assert
            result.Should().Be(1);
            repository.Received(1).Fetch<int>(Arg.Any<ICriteria>());
        }

        [Fact]
        public void CheckDataSabaSportDangerLevelReady_WhenDataIsNotReady_ShouldReturnZero()
        {
            // Arrange
            var expectedResult = new List<int> { 0 };
            repository.Fetch<int>(Arg.Any<ICriteria>()).Returns(expectedResult);

            // Act
            var result = dataAccess.CheckDataSabaSportDangerLevelReady();

            // Assert
            result.Should().Be(0);
            repository.Received(1).Fetch<int>(Arg.Any<ICriteria>());
        }

        [Fact]
        public void CheckDataSabaSportDangerLevelReady_WhenRepositoryReturnsNull_ShouldReturnZero()
        {
            // Arrange
            repository.Fetch<int>(Arg.Any<ICriteria>()).Returns((IEnumerable<int>)null);

            // Act
            var result = dataAccess.CheckDataSabaSportDangerLevelReady();

            // Assert
            result.Should().Be(0);
            repository.Received(1).Fetch<int>(Arg.Any<ICriteria>());
        }

        [Fact]
        public void CheckDataSabaSportDangerLevelReady_WhenRepositoryReturnsEmptyList_ShouldReturnZero()
        {
            // Arrange
            var expectedResult = new List<int>();
            repository.Fetch<int>(Arg.Any<ICriteria>()).Returns(expectedResult);

            // Act
            var result = dataAccess.CheckDataSabaSportDangerLevelReady();

            // Assert
            result.Should().Be(0);
            repository.Received(1).Fetch<int>(Arg.Any<ICriteria>());
        }

        [Fact]
        public void CheckDataSabaSportDangerLevelReady_WhenRepositoryThrowsException_ShouldPropagateException()
        {
            // Arrange
            repository.When(x => x.Fetch<int>(Arg.Any<ICriteria>())).Do(x => throw new Exception("Database error"));

            // Act & Assert
            Assert.Throws<Exception>(() => dataAccess.CheckDataSabaSportDangerLevelReady());
            repository.Received(1).Fetch<int>(Arg.Any<ICriteria>());
        }

        #endregion

        #region GetSapaSportDangerLevel Tests

        [Fact]
        public void GetSapaSportDangerLevel_WhenEnvironmentIsPRO_ShouldIncludeUUS()
        {
            // Arrange
            var environment = (int)Consts.Environment.PRO;
            var expectedEntities = new List<SapaSportDangerLevelEntity>
            {
                new SapaSportDangerLevelEntity { CustID = 1, SportType = 1, IsParlay = false, MemberWinloss = 100.50m },
                new SapaSportDangerLevelEntity { CustID = 2, SportType = 2, IsParlay = true, MemberWinloss = -50.25m }
            };
            repository.Fetch<SapaSportDangerLevelEntity>(Arg.Any<ICriteria>()).Returns(expectedEntities);

            // Act
            var result = dataAccess.GetSapaSportDangerLevel(environment);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedEntities);
            repository.Received(1).Fetch<SapaSportDangerLevelEntity>(Arg.Any<ICriteria>());
        }

        [Fact]
        public void GetSapaSportDangerLevel_WhenEnvironmentIsKLDR_ShouldIncludeUUS()
        {
            // Arrange
            var environment = (int)Consts.Environment.KLDR;
            var expectedEntities = new List<SapaSportDangerLevelEntity>
            {
                new SapaSportDangerLevelEntity { CustID = 1, SportType = 1, IsParlay = false, MemberWinloss = 100.50m }
            };
            repository.Fetch<SapaSportDangerLevelEntity>(Arg.Any<ICriteria>()).Returns(expectedEntities);

            // Act
            var result = dataAccess.GetSapaSportDangerLevel(environment);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedEntities);
            repository.Received(1).Fetch<SapaSportDangerLevelEntity>(Arg.Any<ICriteria>());
        }

        [Fact]
        public void GetSapaSportDangerLevel_WhenEnvironmentIsLOCAL_ShouldIncludeUUS()
        {
            // Arrange
            var environment = (int)Consts.Environment.LOCAL;
            var expectedEntities = new List<SapaSportDangerLevelEntity>();
            repository.Fetch<SapaSportDangerLevelEntity>(Arg.Any<ICriteria>()).Returns(expectedEntities);

            // Act
            var result = dataAccess.GetSapaSportDangerLevel(environment);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
            repository.Received(1).Fetch<SapaSportDangerLevelEntity>(Arg.Any<ICriteria>());
        }

        [Fact]
        public void GetSapaSportDangerLevel_WhenEnvironmentIsUAT_ShouldNotIncludeUUS()
        {
            // Arrange
            var environment = (int)Consts.Environment.UAT;
            var expectedEntities = new List<SapaSportDangerLevelEntity>
            {
                new SapaSportDangerLevelEntity { CustID = 1, SportType = 1, IsParlay = false, MemberWinloss = 100.50m }
            };
            repository.Fetch<SapaSportDangerLevelEntity>(Arg.Any<ICriteria>()).Returns(expectedEntities);

            // Act
            var result = dataAccess.GetSapaSportDangerLevel(environment);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedEntities);
            repository.Received(1).Fetch<SapaSportDangerLevelEntity>(Arg.Any<ICriteria>());
        }

        [Fact]
        public void GetSapaSportDangerLevel_WhenEnvironmentIsInvalid_ShouldNotIncludeUUS()
        {
            // Arrange
            var environment = 999; // Invalid environment
            var expectedEntities = new List<SapaSportDangerLevelEntity>();
            repository.Fetch<SapaSportDangerLevelEntity>(Arg.Any<ICriteria>()).Returns(expectedEntities);

            // Act
            var result = dataAccess.GetSapaSportDangerLevel(environment);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
            repository.Received(1).Fetch<SapaSportDangerLevelEntity>(Arg.Any<ICriteria>());
        }

        [Fact]
        public void GetSapaSportDangerLevel_WhenRepositoryReturnsNull_ShouldReturnNull()
        {
            // Arrange
            var environment = (int)Consts.Environment.PRO;
            repository.Fetch<SapaSportDangerLevelEntity>(Arg.Any<ICriteria>()).Returns((IEnumerable<SapaSportDangerLevelEntity>)null);

            // Act
            var result = dataAccess.GetSapaSportDangerLevel(environment);

            // Assert
            result.Should().BeNull();
            repository.Received(1).Fetch<SapaSportDangerLevelEntity>(Arg.Any<ICriteria>());
        }

        [Fact]
        public void GetSapaSportDangerLevel_WhenRepositoryThrowsException_ShouldPropagateException()
        {
            // Arrange
            var environment = (int)Consts.Environment.PRO;
            repository.When(x => x.Fetch<SapaSportDangerLevelEntity>(Arg.Any<ICriteria>())).Do(x => throw new Exception("Database error"));

            // Act & Assert
            Assert.Throws<Exception>(() => dataAccess.GetSapaSportDangerLevel(environment));
            repository.Received(1).Fetch<SapaSportDangerLevelEntity>(Arg.Any<ICriteria>());
        }

        #endregion

        #region UpdateDangerLevelToMainDB Tests

        [Fact]
        public void UpdateDangerLevelToMainDB_WhenEnvironmentIsPRO_ShouldSetCorrectParameters()
        {
            // Arrange
            var environment = (int)Consts.Environment.PRO;
            var winLossSapaSportInfo = new List<SapaSportDangerLevelEntity>
            {
                new SapaSportDangerLevelEntity { CustID = 1, SportType = 1, IsParlay = false, MemberWinloss = 100.50m }
            };

            // Act
            var result = dataAccess.UpdateDangerLevelToMainDB(winLossSapaSportInfo, environment);

            // Assert
            result.Should().NotBeNull();
            result.InPutParam.Should().BeEquivalentTo(winLossSapaSportInfo);
            repository.Received(1).Execute(Arg.Any<INonQueryCommand>());
        }

        [Fact]
        public void UpdateDangerLevelToMainDB_WhenEnvironmentIsKLDR_ShouldSetCorrectParameters()
        {
            // Arrange
            var environment = (int)Consts.Environment.KLDR;
            var winLossSapaSportInfo = new List<SapaSportDangerLevelEntity>
            {
                new SapaSportDangerLevelEntity { CustID = 1, SportType = 1, IsParlay = false, MemberWinloss = 100.50m }
            };

            // Act
            var result = dataAccess.UpdateDangerLevelToMainDB(winLossSapaSportInfo, environment);

            // Assert
            result.Should().NotBeNull();
            result.InPutParam.Should().BeEquivalentTo(winLossSapaSportInfo);
            repository.Received(1).Execute(Arg.Any<INonQueryCommand>());
        }

        [Fact]
        public void UpdateDangerLevelToMainDB_WhenEnvironmentIsLOCAL_ShouldSetCorrectParameters()
        {
            // Arrange
            var environment = (int)Consts.Environment.LOCAL;
            var winLossSapaSportInfo = new List<SapaSportDangerLevelEntity>
            {
                new SapaSportDangerLevelEntity { CustID = 1, SportType = 1, IsParlay = false, MemberWinloss = 100.50m }
            };

            // Act
            var result = dataAccess.UpdateDangerLevelToMainDB(winLossSapaSportInfo, environment);

            // Assert
            result.Should().NotBeNull();
            result.InPutParam.Should().BeEquivalentTo(winLossSapaSportInfo);
            repository.Received(1).Execute(Arg.Any<INonQueryCommand>());
        }

        [Fact]
        public void UpdateDangerLevelToMainDB_WhenEnvironmentIsUAT_ShouldSetCorrectParameters()
        {
            // Arrange
            var environment = (int)Consts.Environment.UAT;
            var winLossSapaSportInfo = new List<SapaSportDangerLevelEntity>
            {
                new SapaSportDangerLevelEntity { CustID = 1, SportType = 1, IsParlay = false, MemberWinloss = 100.50m }
            };

            // Act
            var result = dataAccess.UpdateDangerLevelToMainDB(winLossSapaSportInfo, environment);

            // Assert
            result.Should().NotBeNull();
            result.InPutParam.Should().BeEquivalentTo(winLossSapaSportInfo);
            repository.Received(1).Execute(Arg.Any<INonQueryCommand>());
        }

        [Fact]
        public void UpdateDangerLevelToMainDB_WhenEnvironmentIsInvalid_ShouldNotSetIncludeUUS()
        {
            // Arrange
            var environment = 999; // Invalid environment
            var winLossSapaSportInfo = new List<SapaSportDangerLevelEntity>
            {
                new SapaSportDangerLevelEntity { CustID = 1, SportType = 1, IsParlay = false, MemberWinloss = 100.50m }
            };

            // Act
            var result = dataAccess.UpdateDangerLevelToMainDB(winLossSapaSportInfo, environment);

            // Assert
            result.Should().NotBeNull();
            result.InPutParam.Should().BeEquivalentTo(winLossSapaSportInfo);
            repository.Received(1).Execute(Arg.Any<INonQueryCommand>());
        }

        [Fact]
        public void UpdateDangerLevelToMainDB_WhenWinLossSapaSportInfoIsEmpty_ShouldExecuteSuccessfully()
        {
            // Arrange
            var environment = (int)Consts.Environment.PRO;
            var winLossSapaSportInfo = new List<SapaSportDangerLevelEntity>();

            // Act
            var result = dataAccess.UpdateDangerLevelToMainDB(winLossSapaSportInfo, environment);

            // Assert
            result.Should().NotBeNull();
            result.InPutParam.Should().BeEquivalentTo(winLossSapaSportInfo);
            repository.Received(1).Execute(Arg.Any<INonQueryCommand>());
        }
       

        [Fact]
        public void UpdateDangerLevelToMainDB_WhenRepositoryThrowsException_ShouldPropagateException()
        {
            // Arrange
            var environment = (int)Consts.Environment.PRO;
            var winLossSapaSportInfo = new List<SapaSportDangerLevelEntity>
            {
                new SapaSportDangerLevelEntity { CustID = 1, SportType = 1, IsParlay = false, MemberWinloss = 100.50m }
            };
            repository.When(x => x.Execute(Arg.Any<INonQueryCommand>())).Do(x => throw new Exception("Database error"));

            // Act & Assert
            Assert.Throws<Exception>(() => dataAccess.UpdateDangerLevelToMainDB(winLossSapaSportInfo, environment));
            repository.Received(1).Execute(Arg.Any<INonQueryCommand>());
        }

        #endregion

        #region Edge Case Tests

        [Fact]
        public void CheckDataSabaSportDangerLevelReady_WhenRepositoryReturnsMultipleValues_ShouldReturnFirstValue()
        {
            // Arrange
            var expectedResult = new List<int> { 1, 0, 1 };
            repository.Fetch<int>(Arg.Any<ICriteria>()).Returns(expectedResult);

            // Act
            var result = dataAccess.CheckDataSabaSportDangerLevelReady();

            // Assert
            result.Should().Be(1);
            repository.Received(1).Fetch<int>(Arg.Any<ICriteria>());
        }

        [Fact]
        public void GetSapaSportDangerLevel_WhenRepositoryReturnsLargeDataset_ShouldHandleGracefully()
        {
            // Arrange
            var environment = (int)Consts.Environment.PRO;
            var expectedEntities = Enumerable.Range(1, 1000).Select(i => new SapaSportDangerLevelEntity
            {
                CustID = i,
                SportType = i % 3 + 1,
                IsParlay = i % 2 == 0,
                MemberWinloss = (decimal)(i * 10.5)
            }).ToList();
            repository.Fetch<SapaSportDangerLevelEntity>(Arg.Any<ICriteria>()).Returns(expectedEntities);

            // Act
            var result = dataAccess.GetSapaSportDangerLevel(environment);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(1000);
            result.Should().BeEquivalentTo(expectedEntities);
            repository.Received(1).Fetch<SapaSportDangerLevelEntity>(Arg.Any<ICriteria>());
        }

        [Fact]
        public void UpdateDangerLevelToMainDB_WhenRepositoryReturnsLargeDataset_ShouldHandleGracefully()
        {
            // Arrange
            var environment = (int)Consts.Environment.PRO;
            var winLossSapaSportInfo = Enumerable.Range(1, 1000).Select(i => new SapaSportDangerLevelEntity
            {
                CustID = i,
                SportType = i % 3 + 1,
                IsParlay = i % 2 == 0,
                MemberWinloss = (decimal)(i * 10.5)
            }).ToList();

            // Act
            var result = dataAccess.UpdateDangerLevelToMainDB(winLossSapaSportInfo, environment);

            // Assert
            result.Should().NotBeNull();
            result.InPutParam.Should().HaveCount(1000);
            result.InPutParam.Should().BeEquivalentTo(winLossSapaSportInfo);
            repository.Received(1).Execute(Arg.Any<INonQueryCommand>());
        }

        [Fact]
        public void GetSapaSportDangerLevel_WhenEnvironmentIsZero_ShouldNotIncludeUUS()
        {
            // Arrange
            var environment = 0;
            var expectedEntities = new List<SapaSportDangerLevelEntity>();
            repository.Fetch<SapaSportDangerLevelEntity>(Arg.Any<ICriteria>()).Returns(expectedEntities);

            // Act
            var result = dataAccess.GetSapaSportDangerLevel(environment);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
            repository.Received(1).Fetch<SapaSportDangerLevelEntity>(Arg.Any<ICriteria>());
        }

        [Fact]
        public void UpdateDangerLevelToMainDB_WhenEnvironmentIsZero_ShouldNotSetIncludeUUS()
        {
            // Arrange
            var environment = 0;
            var winLossSapaSportInfo = new List<SapaSportDangerLevelEntity>
            {
                new SapaSportDangerLevelEntity { CustID = 1, SportType = 1, IsParlay = false, MemberWinloss = 100.50m }
            };

            // Act
            var result = dataAccess.UpdateDangerLevelToMainDB(winLossSapaSportInfo, environment);

            // Assert
            result.Should().NotBeNull();
            result.InPutParam.Should().BeEquivalentTo(winLossSapaSportInfo);
            repository.Received(1).Execute(Arg.Any<INonQueryCommand>());
        }

        [Fact]
        public void GetSapaSportDangerLevel_WhenEnvironmentIsNegative_ShouldNotIncludeUUS()
        {
            // Arrange
            var environment = -1;
            var expectedEntities = new List<SapaSportDangerLevelEntity>();
            repository.Fetch<SapaSportDangerLevelEntity>(Arg.Any<ICriteria>()).Returns(expectedEntities);

            // Act
            var result = dataAccess.GetSapaSportDangerLevel(environment);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
            repository.Received(1).Fetch<SapaSportDangerLevelEntity>(Arg.Any<ICriteria>());
        }

        [Fact]
        public void UpdateDangerLevelToMainDB_WhenEnvironmentIsNegative_ShouldNotSetIncludeUUS()
        {
            // Arrange
            var environment = -1;
            var winLossSapaSportInfo = new List<SapaSportDangerLevelEntity>
            {
                new SapaSportDangerLevelEntity { CustID = 1, SportType = 1, IsParlay = false, MemberWinloss = 100.50m }
            };

            // Act
            var result = dataAccess.UpdateDangerLevelToMainDB(winLossSapaSportInfo, environment);

            // Assert
            result.Should().NotBeNull();
            result.InPutParam.Should().BeEquivalentTo(winLossSapaSportInfo);
            repository.Received(1).Execute(Arg.Any<INonQueryCommand>());
        }

        #endregion
    }
}
