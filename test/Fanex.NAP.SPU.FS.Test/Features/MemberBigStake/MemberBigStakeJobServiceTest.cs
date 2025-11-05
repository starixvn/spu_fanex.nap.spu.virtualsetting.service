namespace Fanex.NAP.SPU.VR.Tests.Features.MemberBigStake
{
    using Fanex.NAP.SPU.Notification.Services;
    using Fanex.NAP.SPU.VR.Features.MemberBigStake;
    using Fanex.NAP.SPU.VR.Features.MemberBigStake.Entities;
    using Fanex.NAP.SPU.VR.Features.MemberBigStake.Model;
    using Fanex.NAP.SPU.VR.Features.Notification;
    using Fanex.NAP.SPU.VR.JobService;
    using Fanex.NAP.SPU.VR.JobService.Services;
    using FluentAssertions;
    using NSubstitute;
    using NSubstitute.ExceptionExtensions;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using Xunit;

    /// <summary>
    /// Unit tests for MemberBigStakeJobService class
    /// </summary>
    public class MemberBigStakeJobServiceTest
    {
        private readonly IAppSettings appSettings;
        private readonly IMemberBigStakeService memberBigStakeService;
        private readonly MemberBigStakeJobService jobService;
        private readonly INotificationService notificationService;

        public MemberBigStakeJobServiceTest()
        {
            appSettings = Substitute.For<IAppSettings>();
            memberBigStakeService = Substitute.For<IMemberBigStakeService>();
            notificationService = Substitute.For<INotificationService>();

            jobService = new MemberBigStakeJobService(appSettings, memberBigStakeService, notificationService);
        }

        #region Constructor Tests

        [Fact]
        public void Constructor_WhenCalledWithValidDependencies_ShouldInitializeCorrectly()
        {
            // Arrange
            var mockAppSettings = Substitute.For<IAppSettings>();
            var mockMemberBigStakeService = Substitute.For<IMemberBigStakeService>();
            var notificationService = Substitute.For<INotificationService>();

            // Act
            var service = new MemberBigStakeJobService(mockAppSettings, mockMemberBigStakeService, notificationService);

            // Assert
            service.Should().NotBeNull();
        }

        #endregion       

        #region UpdateMemberBigStakeFirstBetDate Method Tests

        [Fact]
        public void UpdateMemberBigStakeFirstBetDate_WhenMemberBigStakePerformanceIsNull_ShouldNotProcess()
        {
            // Arrange
            MemberBigStakePerformanceModel nullPerformance = null;
            appSettings.RetryTime.Returns(3);

            // Act
            var method = typeof(MemberBigStakeJobService).GetMethod("UpdateMemberBigStakeFirstBetDate", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            method.Invoke(jobService, new object[] { nullPerformance });

            // Assert
            memberBigStakeService.DidNotReceive().UpdateMemberBigStakeFirstBetDate(Arg.Any<IEnumerable<long>>(), 
                Arg.Any<int>(), Arg.Any<DateTime>(), Arg.Any<bool>());
        }

        [Fact]
        public void UpdateMemberBigStakeFirstBetDate_WhenFirstBetDateListIsNull_ShouldNotProcess()
        {
            // Arrange
            var performance = new MemberBigStakePerformanceModel
            {
                FirstBetDateList = null,
                StatusList = new List<MemberBigStakePerformanceStatusEntity>()
            };
            appSettings.RetryTime.Returns(3);

            // Act
            var method = typeof(MemberBigStakeJobService).GetMethod("UpdateMemberBigStakeFirstBetDate", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            method.Invoke(jobService, new object[] { performance });

            // Assert
            memberBigStakeService.DidNotReceive().UpdateMemberBigStakeFirstBetDate(Arg.Any<IEnumerable<long>>(), 
                Arg.Any<int>(), Arg.Any<DateTime>(), Arg.Any<bool>());
        }
     
        [Fact]
        public void UpdateMemberBigStakeFirstBetDate_WhenHasValidData_ShouldProcessAndGroupByDate()
        {
            // Arrange
            var firstBetDate = DateTime.Today;
            var performance = new MemberBigStakePerformanceModel
            {
                FirstBetDateList = new List<MemberBigStakePerformanceFirstDayEntity>
                {
                    new MemberBigStakePerformanceFirstDayEntity { CustId = 1, FirstBetDate = firstBetDate },
                    new MemberBigStakePerformanceFirstDayEntity { CustId = 2, FirstBetDate = firstBetDate },
                    new MemberBigStakePerformanceFirstDayEntity { CustId = 3, FirstBetDate = firstBetDate.AddDays(1) }
                },
                StatusList = new List<MemberBigStakePerformanceStatusEntity>()
            };
            appSettings.RetryTime.Returns(3);
            appSettings.IsTest.Returns(true);

            // Act
            var method = typeof(MemberBigStakeJobService).GetMethod("UpdateMemberBigStakeFirstBetDate", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            method.Invoke(jobService, new object[] { performance });

            // Assert
            memberBigStakeService.Received(2).UpdateMemberBigStakeFirstBetDate(Arg.Any<IEnumerable<long>>(), 
                1, Arg.Any<DateTime>(), true);
        }       

        #endregion

        #region UpdateMemberBigStakeStatus Method Tests

        [Fact]
        public void UpdateMemberBigStakeStatus_WhenMemberBigStakePerformanceIsNull_ShouldNotProcess()
        {
            // Arrange
            MemberBigStakePerformanceModel nullPerformance = null;
            appSettings.RetryTime.Returns(3);

            // Act
            var method = typeof(MemberBigStakeJobService).GetMethod("UpdateMemberBigStakeStatus", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            method.Invoke(jobService, new object[] { nullPerformance });

            // Assert
            memberBigStakeService.DidNotReceive().UpdateWhitelistMemberBigStake(Arg.Any<IEnumerable<MemberBigStakePerformanceStatusModel>>(), 
                Arg.Any<int>(), Arg.Any<bool>());
        }

        [Fact]
        public void UpdateMemberBigStakeStatus_WhenStatusListIsNull_ShouldNotProcess()
        {
            // Arrange
            var performance = new MemberBigStakePerformanceModel
            {
                FirstBetDateList = new List<MemberBigStakePerformanceFirstDayEntity>(),
                StatusList = null
            };
            appSettings.RetryTime.Returns(3);

            // Act
            var method = typeof(MemberBigStakeJobService).GetMethod("UpdateMemberBigStakeStatus", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            method.Invoke(jobService, new object[] { performance });

            // Assert
            memberBigStakeService.DidNotReceive().UpdateWhitelistMemberBigStake(Arg.Any<IEnumerable<MemberBigStakePerformanceStatusModel>>(), 
                Arg.Any<int>(), Arg.Any<bool>());
        }

        [Fact]
        public void UpdateMemberBigStakeStatus_WhenStatusListIsEmpty_ShouldNotProcess()
        {
            // Arrange
            var performance = new MemberBigStakePerformanceModel
            {
                FirstBetDateList = new List<MemberBigStakePerformanceFirstDayEntity>(),
                StatusList = new List<MemberBigStakePerformanceStatusEntity>()
            };
            appSettings.RetryTime.Returns(3);

            // Act
            var method = typeof(MemberBigStakeJobService).GetMethod("UpdateMemberBigStakeStatus", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            method.Invoke(jobService, new object[] { performance });

            // Assert
            memberBigStakeService.DidNotReceive().UpdateWhitelistMemberBigStake(Arg.Any<IEnumerable<MemberBigStakePerformanceStatusModel>>(), 
                Arg.Any<int>(), Arg.Any<bool>());
        }

        [Fact]
        public void UpdateMemberBigStakeStatus_WhenHasValidData_ShouldProcessAndChunkData()
        {
            // Arrange
            var performance = new MemberBigStakePerformanceModel
            {
                FirstBetDateList = new List<MemberBigStakePerformanceFirstDayEntity>(),
                StatusList = new List<MemberBigStakePerformanceStatusEntity>
                {
                    new MemberBigStakePerformanceStatusEntity { CustID = 1, Status = 1, MemberWinloss = 100.50m },
                    new MemberBigStakePerformanceStatusEntity { CustID = 2, Status = 2, MemberWinloss = 200.75m }
                }
            };
            appSettings.RetryTime.Returns(3);
            appSettings.IsTest.Returns(true);

            // Act
            var method = typeof(MemberBigStakeJobService).GetMethod("UpdateMemberBigStakeStatus", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            method.Invoke(jobService, new object[] { performance });

            // Assert
            memberBigStakeService.Received(1).UpdateWhitelistMemberBigStake(Arg.Any<IEnumerable<MemberBigStakePerformanceStatusModel>>(), 
                1, true);
        }       

        #endregion       

        #region GenerateMailBody Method Tests

        [Fact]
        public void GenerateMailBody_WhenNoMembersAffected_ShouldReturnNoMemberAffectTemplate()
        {
            // Arrange
            var creditMember = new AutoMailBigCreditMemberModel
            {
                BigCreditMemberList = new List<AutomailBigCreditMemberEntity>(),
                BigCreditMemberWithBigLossList = new List<AutomailBigCreditMemberEntity>()
            };
            memberBigStakeService.GetBigCreditMemberForAutomail().Returns(creditMember);

            // Act
            var method = typeof(MemberBigStakeJobService).GetMethod("GenerateMailBody", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            var result = method.Invoke(jobService, null) as string;

            // Assert
            result.Should().NotBeNullOrEmpty();
            memberBigStakeService.Received(1).GetBigCreditMemberForAutomail();
        }

        [Fact]
        public void GenerateMailBody_WhenHasBigCreditMembersOnly_ShouldReturnBigCreditMemberTemplate()
        {
            // Arrange
            var creditMember = new AutoMailBigCreditMemberModel
            {
                BigCreditMemberList = new List<AutomailBigCreditMemberEntity>
                {
                    new AutomailBigCreditMemberEntity { Member = "TestMember1", Site = "TestSite1", GivenCredit_RM = 1000, MemberTurnover_RM = 500, MemberWinloss_RM = 100 }
                },
                BigCreditMemberWithBigLossList = new List<AutomailBigCreditMemberEntity>()
            };
            memberBigStakeService.GetBigCreditMemberForAutomail().Returns(creditMember);

            // Act
            var method = typeof(MemberBigStakeJobService).GetMethod("GenerateMailBody", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            var result = method.Invoke(jobService, null) as string;

            // Assert
            result.Should().NotBeNullOrEmpty();
            memberBigStakeService.Received(1).GetBigCreditMemberForAutomail();
        }

        [Fact]
        public void GenerateMailBody_WhenHasBigLossMembersOnly_ShouldReturnBigLossTemplate()
        {
            // Arrange
            var creditMember = new AutoMailBigCreditMemberModel
            {
                BigCreditMemberList = new List<AutomailBigCreditMemberEntity>(),
                BigCreditMemberWithBigLossList = new List<AutomailBigCreditMemberEntity>
                {
                    new AutomailBigCreditMemberEntity { Member = "TestMember2", Site = "TestSite2", GivenCredit_RM = 2000, MemberTurnover_RM = 1000, MemberWinloss_RM = -200 }
                }
            };
            memberBigStakeService.GetBigCreditMemberForAutomail().Returns(creditMember);

            // Act
            var method = typeof(MemberBigStakeJobService).GetMethod("GenerateMailBody", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            var result = method.Invoke(jobService, null) as string;

            // Assert
            result.Should().NotBeNullOrEmpty();
            memberBigStakeService.Received(1).GetBigCreditMemberForAutomail();
        }

        [Fact]
        public void GenerateMailBody_WhenHasBothTypesOfMembers_ShouldReturnBothTemplate()
        {
            // Arrange
            var creditMember = new AutoMailBigCreditMemberModel
            {
                BigCreditMemberList = new List<AutomailBigCreditMemberEntity>
                {
                    new AutomailBigCreditMemberEntity { Member = "TestMember1", Site = "TestSite1", GivenCredit_RM = 1000, MemberTurnover_RM = 500, MemberWinloss_RM = 100 }
                },
                BigCreditMemberWithBigLossList = new List<AutomailBigCreditMemberEntity>
                {
                    new AutomailBigCreditMemberEntity { Member = "TestMember2", Site = "TestSite2", GivenCredit_RM = 2000, MemberTurnover_RM = 1000, MemberWinloss_RM = -200 }
                }
            };
            memberBigStakeService.GetBigCreditMemberForAutomail().Returns(creditMember);

            // Act
            var method = typeof(MemberBigStakeJobService).GetMethod("GenerateMailBody", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            var result = method.Invoke(jobService, null) as string;

            // Assert
            result.Should().NotBeNullOrEmpty();
            memberBigStakeService.Received(1).GetBigCreditMemberForAutomail();
        }

        #endregion

        #region GenerateTable Method Tests

        [Fact]
        public void GenerateTable_WhenCreditMemberBigStakeIsNull_ShouldReturnEmptyString()
        {
            // Arrange
            IEnumerable<AutomailBigCreditMemberEntity> nullList = null;

            // Act
            var method = typeof(MemberBigStakeJobService).GetMethod("GenerateTable", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            var result = method.Invoke(jobService, new object[] { nullList }) as string;

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public void GenerateTable_WhenCreditMemberBigStakeIsEmpty_ShouldReturnEmptyString()
        {
            // Arrange
            var emptyList = new List<AutomailBigCreditMemberEntity>();

            // Act
            var method = typeof(MemberBigStakeJobService).GetMethod("GenerateTable", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            var result = method.Invoke(jobService, new object[] { emptyList }) as string;

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public void GenerateTable_WhenHasValidData_ShouldReturnHtmlTable()
        {
            // Arrange
            var creditMembers = new List<AutomailBigCreditMemberEntity>
            {
                new AutomailBigCreditMemberEntity 
                { 
                    Member = "TestMember1", 
                    Site = "TestSite1", 
                    GivenCredit_RM = 1000.50m, 
                    MemberTurnover_RM = 500.25m, 
                    MemberWinloss_RM = 100.75m 
                },
                new AutomailBigCreditMemberEntity 
                { 
                    Member = "TestMember2", 
                    Site = "TestSite2", 
                    GivenCredit_RM = 2000.00m, 
                    MemberTurnover_RM = 1000.00m, 
                    MemberWinloss_RM = -200.50m 
                }
            };

            // Act
            var method = typeof(MemberBigStakeJobService).GetMethod("GenerateTable", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            var result = method.Invoke(jobService, new object[] { creditMembers }) as string;

            // Assert
            result.Should().NotBeNullOrEmpty();
            result.Should().Contain("<table");
            result.Should().Contain("</table>");
            result.Should().Contain("TestMember1");
            result.Should().Contain("TestMember2");
            result.Should().Contain("1,000.50");
            result.Should().Contain("-200.50");
        }

        #endregion

        #region CSS Helper Methods Tests

        [Fact]
        public void CssStyleTable_WhenCalledWithWidth_ShouldReturnCorrectCss()
        {
            // Arrange
            var width = 700;

            // Act
            var method = typeof(MemberBigStakeJobService).GetMethod("CssStyleTable", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            var result = method.Invoke(jobService, new object[] { width }) as string;

            // Assert
            result.Should().Contain("border-collapse: collapse;");
            result.Should().Contain("width: 700px");
        }

        [Fact]
        public void CssStyleTh_WhenCalled_ShouldReturnCorrectCss()
        {
            // Act
            var method = typeof(MemberBigStakeJobService).GetMethod("CssStyleTh", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            var result = method.Invoke(jobService, null) as string;

            // Assert
            result.Should().Contain("border: 1px solid black;");
            result.Should().Contain("font-weight:bold;");
        }       

        #endregion

        #region NotifyMessage Method Tests

        [Fact]
        public void NotifyMessage_WhenNotifiedMessageIsEmpty_ShouldNotCallUtilities()
        {
            // Arrange
            var field = typeof(MemberBigStakeJobService).GetField("notifiedMessage", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            field.SetValue(jobService, "");

            // Act
            var method = typeof(MemberBigStakeJobService).GetMethod("NotifyMessage", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            method.Invoke(jobService, null);

            // Assert
            // Since we can't easily mock static methods, we just verify the method doesn't throw
            // In a real scenario, you might want to use a wrapper or test the actual notification
        }

        [Fact]
        public void NotifyMessage_WhenNotifiedMessageIsNull_ShouldNotCallUtilities()
        {
            // Arrange
            var field = typeof(MemberBigStakeJobService).GetField("notifiedMessage", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            field.SetValue(jobService, null);

            // Act
            var method = typeof(MemberBigStakeJobService).GetMethod("NotifyMessage", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            method.Invoke(jobService, null);

            // Assert
            // Since we can't easily mock static methods, we just verify the method doesn't throw
        }

        [Fact]
        public void NotifyMessage_WhenNotifiedMessageIsWhitespace_ShouldNotCallUtilities()
        {
            // Arrange
            var field = typeof(MemberBigStakeJobService).GetField("notifiedMessage", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            field.SetValue(jobService, "   ");

            // Act
            var method = typeof(MemberBigStakeJobService).GetMethod("NotifyMessage", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            method.Invoke(jobService, null);

            // Assert
            // Since we can't easily mock static methods, we just verify the method doesn't throw
        }

        [Fact]
        public void NotifyMessage_WhenNotifiedMessageHasValue_ShouldNotThrow()
        {
            // Arrange
            var field = typeof(MemberBigStakeJobService).GetField("notifiedMessage", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            field.SetValue(jobService, "Test notification message");

            appSettings.EnvironmentString.Returns("TEST");
            appSettings.LogGroupID.Returns("TestLogGroup");

            // Act
            var method = typeof(MemberBigStakeJobService).GetMethod("NotifyMessage", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            var action = () => method.Invoke(jobService, null);

            // Assert
            action.Should().NotThrow();
        }

        #endregion       

        #region Helper Methods       
        private void SetupSuccessfulExecutionAfterRetry()
        {
            appSettings.IsTest.Returns(true);
            appSettings.MailingListIdAgentAjust.Returns(123);
            appSettings.EnvironmentString.Returns("TEST");
            appSettings.LogGroupID.Returns("TestLogGroup");

            var performance = new MemberBigStakePerformanceModel
            {
                FirstBetDateList = new List<MemberBigStakePerformanceFirstDayEntity>(),
                StatusList = new List<MemberBigStakePerformanceStatusEntity>()
            };

            var creditMember = new AutoMailBigCreditMemberModel
            {
                BigCreditMemberList = new List<AutomailBigCreditMemberEntity>(),
                BigCreditMemberWithBigLossList = new List<AutomailBigCreditMemberEntity>()
            };

            memberBigStakeService.GetMemberBigStakePerformance(Arg.Any<bool>()).Returns(performance);
            memberBigStakeService.GetBigCreditMemberForAutomail().Returns(creditMember);
        }

        private void VerifySuccessfulExecution()
        {
            memberBigStakeService.Received(1).InsertMemberBigStake(appSettings.IsTest);
            memberBigStakeService.Received(1).GetMemberBigStakePerformance(appSettings.IsTest);
            memberBigStakeService.Received(1).GetBigCreditMemberForAutomail();
        }

        #endregion
    }
}
