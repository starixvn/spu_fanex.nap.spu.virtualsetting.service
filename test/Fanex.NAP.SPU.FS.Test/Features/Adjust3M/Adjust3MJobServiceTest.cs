namespace Fanex.NAP.SPU.VR.Tests.Features.Adjust3M
{
    using Fanex.NAP.SPU.Notification.Services;
    using Fanex.NAP.SPU.VR.Features.Adjust3M;
    using Fanex.NAP.SPU.VR.Features.Notification;
    using Fanex.NAP.SPU.VR.JobService;
    using Fanex.NAP.SPU.VR.JobService.Services;
    using FluentAssertions;
    using NSubstitute;
    using NSubstitute.ExceptionExtensions;
    using System;
    using Xunit;

    /// <summary>
    /// Unit tests for Adjust3MJobService class
    /// </summary>
    public class Adjust3MJobServiceTest
    {
        private readonly IAppSettings appSettings;
        private readonly ISoccerService soccerService;
        private readonly ISportsbookService sportsbookService;
        private readonly Adjust3MJobService jobService;
        private readonly INotificationService notificationService;

        public Adjust3MJobServiceTest()
        {
            appSettings = Substitute.For<IAppSettings>();
            soccerService = Substitute.For<ISoccerService>();
            sportsbookService = Substitute.For<ISportsbookService>();
            notificationService = Substitute.For<INotificationService>();

            jobService = new Adjust3MJobService(appSettings, soccerService, sportsbookService, notificationService);
        }       

        #region IsExecutedDate Tests

        [Fact]
        public void IsExecutedDate_WhenCurrentDayIsInExecutedDates_ShouldReturnTrue()
        {
            // Arrange
            var currentDate = DateTime.Today;
            var executedDates = $"{currentDate.Day};15;20";
            appSettings.Adjust3MExecutedDate.Returns(executedDates);

            // Act
            var result = jobService.GetType()
                .GetMethod("IsExecutedDate", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .Invoke(jobService, null);

            // Assert
            result.Should().Be(true);
        }
    
        [Fact]
        public void IsExecutedDate_WhenExecutedDatesIsNull_ShouldThrowException()
        {
            // Arrange
            appSettings.Adjust3MExecutedDate.Returns((string)null);

            // Act & Assert
            var action = () => jobService.GetType()
                .GetMethod("IsExecutedDate", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .Invoke(jobService, null);

            action.Should().Throw<Exception>();
        }

        #endregion

       
        #region UpdateForSportbooks Tests     
        [Fact]
        public void UpdateForSportbooks_WhenSportsbookServiceThrowsException_ShouldPropagateException()
        {
            // Arrange
            var jsonLicenseeBACusts = "{\"customers\":[1,2,3]}";
            appSettings.Adjust3MDummyUser.Returns(999);
            appSettings.Adjust3MIP.Returns("192.168.1.1");
            appSettings.Adjust3MPunterChunkSize.Returns(100);
            appSettings.Adjust3MSiteIds.Returns("1,2,3");
            appSettings.Adjust3MSuperIds.Returns("4,5,6");          

            sportsbookService.InitModel(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<DateTime>(), Arg.Any<double>(), Arg.Any<string>())
                .Throws(new Exception("Sportsbook service error"));

            // Act & Assert
            var action = () => jobService.UpdateForSportbooks(jsonLicenseeBACusts);

            action.Should().Throw<Exception>().WithMessage("Sportsbook service error");
        }
      
        #endregion        
    }
}