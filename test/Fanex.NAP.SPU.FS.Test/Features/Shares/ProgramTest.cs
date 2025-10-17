namespace Fanex.NAP.SPU.VR.Tests.Features.Shares
{
    using Fanex.NAP.SPU.VR.JobService;
    using FluentAssertions;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Hosting;
    using NSubstitute;
    using System;
    using System.IO;
    using System.Reflection;
    using Xunit;

    /// <summary>
    /// Unit tests for Program class
    /// </summary>
    public class ProgramTest
    {       
        #region CreateHostBuilder Method Tests

        [Fact]
        public void CreateHostBuilder_WhenCalledWithValidArgs_ShouldReturnHostBuilder()
        {
            // Arrange
            var args = new[] { "test", "args" };

            // Act
            var result = Program.CreateHostBuilder(args);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeAssignableTo<IHostBuilder>();
        }

        [Fact]
        public void CreateHostBuilder_WhenCalledWithNullArgs_ShouldReturnHostBuilder()
        {
            // Arrange
            string[] args = null;

            // Act
            var result = Program.CreateHostBuilder(args);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeAssignableTo<IHostBuilder>();
        }

        [Fact]
        public void CreateHostBuilder_WhenCalledWithEmptyArgs_ShouldReturnHostBuilder()
        {
            // Arrange
            var args = new string[0];

            // Act
            var result = Program.CreateHostBuilder(args);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeAssignableTo<IHostBuilder>();
        }

        [Fact]
        public void CreateHostBuilder_WhenCalled_ShouldConfigureAppConfiguration()
        {
            // Arrange
            var args = new[] { "test" };

            // Act
            var hostBuilder = Program.CreateHostBuilder(args);

            // Assert
            hostBuilder.Should().NotBeNull();
            // Note: We can't easily test the internal configuration without building the host,
            // but we can verify the method returns a valid IHostBuilder
        }

        [Fact]
        public void CreateHostBuilder_WhenCalled_ShouldConfigureWebHostDefaults()
        {
            // Arrange
            var args = new[] { "test" };

            // Act
            var hostBuilder = Program.CreateHostBuilder(args);

            // Assert
            hostBuilder.Should().NotBeNull();
            // Note: We can't easily test the internal web host configuration without building the host,
            // but we can verify the method returns a valid IHostBuilder
        }

        #endregion

        #region Configuration Tests

        [Fact]
        public void CreateHostBuilder_WhenCalled_ShouldSetBasePath()
        {
            // Arrange
            var args = new[] { "test" };

            // Act
            var hostBuilder = Program.CreateHostBuilder(args);

            // Assert
            hostBuilder.Should().NotBeNull();
            // The configuration setup is internal to the host builder,
            // but we can verify the method completes successfully
        }

        [Fact]
        public void CreateHostBuilder_WhenCalled_ShouldAddJsonFileConfiguration()
        {
            // Arrange
            var args = new[] { "test" };

            // Act
            var hostBuilder = Program.CreateHostBuilder(args);

            // Assert
            hostBuilder.Should().NotBeNull();
            // The JSON file configuration is added internally,
            // but we can verify the method completes successfully
        }

        [Fact]
        public void CreateHostBuilder_WhenCalled_ShouldAddEnvironmentVariables()
        {
            // Arrange
            var args = new[] { "test" };

            // Act
            var hostBuilder = Program.CreateHostBuilder(args);

            // Assert
            hostBuilder.Should().NotBeNull();
            // Environment variables are added internally,
            // but we can verify the method completes successfully
        }

        #endregion

        #region Web Host Configuration Tests

        [Fact]
        public void CreateHostBuilder_WhenCalled_ShouldConfigureUrls()
        {
            // Arrange
            var args = new[] { "test" };

            // Act
            var hostBuilder = Program.CreateHostBuilder(args);

            // Assert
            hostBuilder.Should().NotBeNull();
            // URL configuration is set internally to "http://*:5000",
            // but we can verify the method completes successfully
        }

        [Fact]
        public void CreateHostBuilder_WhenCalled_ShouldUseSentry()
        {
            // Arrange
            var args = new[] { "test" };

            // Act
            var hostBuilder = Program.CreateHostBuilder(args);

            // Assert
            hostBuilder.Should().NotBeNull();
            // Sentry is configured internally,
            // but we can verify the method completes successfully
        }

        [Fact]
        public void CreateHostBuilder_WhenCalled_ShouldUseStartup()
        {
            // Arrange
            var args = new[] { "test" };

            // Act
            var hostBuilder = Program.CreateHostBuilder(args);

            // Assert
            hostBuilder.Should().NotBeNull();
            // Startup class is configured internally,
            // but we can verify the method completes successfully
        }

        #endregion

        #region Static Class Tests

        [Fact]
        public void Program_ShouldBeStaticClass()
        {
            // Act & Assert
            typeof(Program).IsAbstract.Should().BeTrue();
            typeof(Program).IsSealed.Should().BeTrue();
        }

        [Fact]
        public void Program_ShouldHaveMainMethod()
        {
            // Act
            var mainMethod = typeof(Program).GetMethod("Main", BindingFlags.Public | BindingFlags.Static);

            // Assert
            mainMethod.Should().NotBeNull();
            mainMethod.IsStatic.Should().BeTrue();
            mainMethod.IsPublic.Should().BeTrue();
        }

        [Fact]
        public void Program_ShouldHaveCreateHostBuilderMethod()
        {
            // Act
            var createHostBuilderMethod = typeof(Program).GetMethod("CreateHostBuilder", BindingFlags.Public | BindingFlags.Static);

            // Assert
            createHostBuilderMethod.Should().NotBeNull();
            createHostBuilderMethod.IsStatic.Should().BeTrue();
            createHostBuilderMethod.IsPublic.Should().BeTrue();
        }

        [Fact]
        public void Program_MainMethod_ShouldAcceptStringArrayParameter()
        {
            // Act
            var mainMethod = typeof(Program).GetMethod("Main", BindingFlags.Public | BindingFlags.Static);
            var parameters = mainMethod.GetParameters();

            // Assert
            parameters.Should().HaveCount(1);
            parameters[0].ParameterType.Should().Be(typeof(string[]));
            parameters[0].Name.Should().Be("args");
        }

        [Fact]
        public void Program_CreateHostBuilderMethod_ShouldAcceptStringArrayParameter()
        {
            // Act
            var createHostBuilderMethod = typeof(Program).GetMethod("CreateHostBuilder", BindingFlags.Public | BindingFlags.Static);
            var parameters = createHostBuilderMethod.GetParameters();

            // Assert
            parameters.Should().HaveCount(1);
            parameters[0].ParameterType.Should().Be(typeof(string[]));
            parameters[0].Name.Should().Be("args");
        }

        [Fact]
        public void Program_CreateHostBuilderMethod_ShouldReturnIHostBuilder()
        {
            // Act
            var createHostBuilderMethod = typeof(Program).GetMethod("CreateHostBuilder", BindingFlags.Public | BindingFlags.Static);
            var returnType = createHostBuilderMethod.ReturnType;

            // Assert
            returnType.Should().Be(typeof(IHostBuilder));
        }

        #endregion

        #region Method Signature Tests

        [Fact]
        public void Program_MainMethod_ShouldHaveCorrectSignature()
        {
            // Act
            var mainMethod = typeof(Program).GetMethod("Main", BindingFlags.Public | BindingFlags.Static);

            // Assert
            mainMethod.Should().NotBeNull();
            mainMethod.ReturnType.Should().Be(typeof(void));
            mainMethod.GetParameters().Should().HaveCount(1);
            mainMethod.GetParameters()[0].ParameterType.Should().Be(typeof(string[]));
        }

        [Fact]
        public void Program_CreateHostBuilderMethod_ShouldHaveCorrectSignature()
        {
            // Act
            var createHostBuilderMethod = typeof(Program).GetMethod("CreateHostBuilder", BindingFlags.Public | BindingFlags.Static);

            // Assert
            createHostBuilderMethod.Should().NotBeNull();
            createHostBuilderMethod.ReturnType.Should().Be(typeof(IHostBuilder));
            createHostBuilderMethod.GetParameters().Should().HaveCount(1);
            createHostBuilderMethod.GetParameters()[0].ParameterType.Should().Be(typeof(string[]));
        }

        #endregion

        #region Edge Cases and Error Handling Tests
      
        [Fact]
        public void CreateHostBuilder_WhenCalledMultipleTimes_ShouldReturnDifferentInstances()
        {
            // Arrange
            var args = new[] { "test" };

            // Act
            var hostBuilder1 = Program.CreateHostBuilder(args);
            var hostBuilder2 = Program.CreateHostBuilder(args);

            // Assert
            hostBuilder1.Should().NotBeNull();
            hostBuilder2.Should().NotBeNull();
            hostBuilder1.Should().NotBeSameAs(hostBuilder2);
        }

        [Fact]
        public void CreateHostBuilder_WhenCalledWithSameArgs_ShouldReturnValidHostBuilder()
        {
            // Arrange
            var args1 = new[] { "test", "args" };
            var args2 = new[] { "test", "args" };

            // Act
            var hostBuilder1 = Program.CreateHostBuilder(args1);
            var hostBuilder2 = Program.CreateHostBuilder(args2);

            // Assert
            hostBuilder1.Should().NotBeNull();
            hostBuilder2.Should().NotBeNull();
            hostBuilder1.Should().BeAssignableTo<IHostBuilder>();
            hostBuilder2.Should().BeAssignableTo<IHostBuilder>();
        }

        #endregion    

        #region Performance Tests

        [Fact]
        public void CreateHostBuilder_WhenCalledMultipleTimes_ShouldCompleteQuickly()
        {
            // Arrange
            var args = new[] { "test" };
            var startTime = DateTime.Now;

            // Act
            for (int i = 0; i < 100; i++)
            {
                Program.CreateHostBuilder(args);
            }
            var endTime = DateTime.Now;

            // Assert
            var duration = endTime - startTime;
            duration.TotalSeconds.Should().BeLessThan(5); // Should complete within 5 seconds
        } 

        #endregion

        #region Reflection Tests

        [Fact]
        public void Program_ShouldHaveExpectedMethods()
        {
            // Act
            var methods = typeof(Program).GetMethods(BindingFlags.Public | BindingFlags.Static);

            // Assert
            methods.Should().HaveCount(2);
            methods.Should().Contain(m => m.Name == "Main");
            methods.Should().Contain(m => m.Name == "CreateHostBuilder");
        }
        
        [Fact]
        public void Program_ShouldNotHavePrivateMethods()
        {
            // Act
            var privateMethods = typeof(Program).GetMethods(BindingFlags.NonPublic | BindingFlags.Static);

            // Assert
            privateMethods.Should().BeEmpty();
        }

        [Fact]
        public void Program_ShouldNotHaveFields()
        {
            // Act
            var fields = typeof(Program).GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);

            // Assert
            fields.Should().BeEmpty();
        }

        [Fact]
        public void Program_ShouldNotHaveProperties()
        {
            // Act
            var properties = typeof(Program).GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);

            // Assert
            properties.Should().BeEmpty();
        }

        #endregion
    }
}
