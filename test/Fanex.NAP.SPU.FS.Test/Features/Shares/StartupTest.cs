namespace Fanex.NAP.SPU.VR.Tests.Features.Shares
{
    using Fanex.NAP.SPU.VR.JobService;
    using FluentAssertions;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using NSubstitute;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using Xunit;

    /// <summary>
    /// Unit tests for Startup class
    /// </summary>
    public class StartupTest
    {
        #region Constructor Tests

        [Fact]
        public void Constructor_WhenCalledWithValidConfiguration_ShouldInitializeCorrectly()
        {
            // Arrange
            var configuration = Substitute.For<IConfiguration>();

            // Act
            var startup = new Startup(configuration);

            // Assert
            startup.Should().NotBeNull();
            startup.Configuration.Should().BeSameAs(configuration);
        }
        
        [Fact]
        public void Constructor_WhenCalled_ShouldSetConfigurationProperty()
        {
            // Arrange
            var configuration = Substitute.For<IConfiguration>();

            // Act
            var startup = new Startup(configuration);

            // Assert
            startup.Configuration.Should().BeSameAs(configuration);
        }

        #endregion        

        #region Configure Method Tests      
     
        [Fact]
        public void Configure_WhenCalledWithNullEnv_ShouldThrowArgumentNullException()
        {
            // Arrange
            var configuration = Substitute.For<IConfiguration>();
            var startup = new Startup(configuration);
            var app = Substitute.For<IApplicationBuilder>();
            IWebHostEnvironment nullEnv = null;

            // Act & Assert
            var action = () => startup.Configure(app, nullEnv);
            action.Should().Throw<ArgumentNullException>();
        }
       
        #endregion       

        #region Property Tests

        [Fact]
        public void Configuration_WhenSet_ShouldReturnCorrectValue()
        {
            // Arrange
            var configuration = Substitute.For<IConfiguration>();
            var startup = new Startup(configuration);

            // Act
            var result = startup.Configuration;

            // Assert
            result.Should().BeSameAs(configuration);
        }

        [Fact]
        public void Configuration_WhenSet_ShouldBeReadOnly()
        {
            // Arrange
            var configuration = Substitute.For<IConfiguration>();
            var startup = new Startup(configuration);

            // Act
            var property = typeof(Startup).GetProperty("Configuration");

            // Assert
            property.Should().NotBeNull();
            property.CanRead.Should().BeTrue();
            property.CanWrite.Should().BeFalse();
        }

        #endregion

        #region Method Signature Tests

        [Fact]
        public void ConfigureServices_ShouldHaveCorrectSignature()
        {
            // Act
            var method = typeof(Startup).GetMethod("ConfigureServices", 
                BindingFlags.Public | BindingFlags.Instance);

            // Assert
            method.Should().NotBeNull();
            method.ReturnType.Should().Be(typeof(void));
            method.GetParameters().Should().HaveCount(1);
            method.GetParameters()[0].ParameterType.Should().Be(typeof(IServiceCollection));
        }

        [Fact]
        public void Configure_ShouldHaveCorrectSignature()
        {
            // Act
            var method = typeof(Startup).GetMethod("Configure", 
                BindingFlags.Public | BindingFlags.Instance);

            // Assert
            method.Should().NotBeNull();
            method.ReturnType.Should().Be(typeof(void));
            method.GetParameters().Should().HaveCount(2);
            method.GetParameters()[0].ParameterType.Should().Be(typeof(IApplicationBuilder));
            method.GetParameters()[1].ParameterType.Should().Be(typeof(IWebHostEnvironment));
        }

        [Fact]
        public void FanexDataConfig_ShouldHaveCorrectSignature()
        {
            // Act
            var method = typeof(Startup).GetMethod("FanexDataConfig", 
                BindingFlags.NonPublic | BindingFlags.Instance);

            // Assert
            method.Should().NotBeNull();
            method.ReturnType.Should().Be(typeof(void));
            method.GetParameters().Should().HaveCount(1);
            method.GetParameters()[0].ParameterType.Should().Be(typeof(IAppSettings));
        }

        #endregion        

        #region Reflection Tests
      
        [Fact]
        public void Startup_ShouldHaveExpectedProperties()
        {
            // Act
            var properties = typeof(Startup).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            // Assert
            properties.Should().HaveCount(1);
            properties.Should().Contain(p => p.Name == "Configuration");
        }

        [Fact]
        public void Startup_ShouldHaveExpectedPrivateMethods()
        {
            // Act
            var privateMethods = typeof(Startup).GetMethods(BindingFlags.NonPublic | BindingFlags.Instance);

            // Assert
            privateMethods.Should().Contain(m => m.Name == "FanexDataConfig");
        }

        [Fact]
        public void Startup_ShouldNotHaveStaticMethods()
        {
            // Act
            var staticMethods = typeof(Startup).GetMethods(BindingFlags.Public | BindingFlags.Static);

            // Assert
            staticMethods.Should().BeEmpty();
        }        

        #endregion
       
    }
}
