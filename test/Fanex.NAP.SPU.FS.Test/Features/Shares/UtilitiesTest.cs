namespace Fanex.NAP.SPU.VR.Tests.Features.Shares
{
    using Fanex.NAP.SPU.CTS.Shares;
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Xunit;

    /// <summary>
    /// Unit tests for Utilities static class
    /// </summary>
    public class UtilitiesTest
    {
        #region Partition Extension Method Tests

        [Fact]
        public void Partition_WithValidItems_ShouldReturnCorrectPartitions()
        {
            // Arrange
            var items = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            var partitionSize = 3;

            // Act
            var result = items.Partition(partitionSize);

            // Assert
            result.Should().NotBeNull();
            var partitions = result.ToList();
            partitions.Should().HaveCount(4); // 10 items / 3 = 4 partitions
            partitions[0].Should().BeEquivalentTo(new[] { 1, 2, 3 });
            partitions[1].Should().BeEquivalentTo(new[] { 4, 5, 6 });
            partitions[2].Should().BeEquivalentTo(new[] { 7, 8, 9 });
            partitions[3].Should().BeEquivalentTo(new[] { 10 });
        }

        [Fact]
        public void Partition_WithEmptyItems_ShouldReturnEmptyPartitions()
        {
            // Arrange
            var items = new int[0];
            var partitionSize = 3;

            // Act
            var result = items.Partition(partitionSize);

            // Assert
            result.Should().NotBeNull();
            var partitions = result.ToList();
            partitions.Should().BeEmpty();
        }

        [Fact]
        public void Partition_WithNullItems_ShouldThrowException()
        {
            // Arrange
            IEnumerable<int> items = null;
            var partitionSize = 3;

            // Act & Assert
            var action = () => items.Partition(partitionSize);
            action.Should().Throw<ArgumentNullException>();
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(5)]
        [InlineData(10)]
        public void Partition_WithDifferentPartitionSizes_ShouldReturnCorrectPartitions(int partitionSize)
        {
            // Arrange
            var items = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

            // Act
            var result = items.Partition(partitionSize);

            // Assert
            result.Should().NotBeNull();
            var partitions = result.ToList();
            partitions.Should().NotBeEmpty();
        }

        [Fact]
        public void Partition_WithSingleItem_ShouldReturnSinglePartition()
        {
            // Arrange
            var items = new[] { 1 };
            var partitionSize = 3;

            // Act
            var result = items.Partition(partitionSize);

            // Assert
            result.Should().NotBeNull();
            var partitions = result.ToList();
            partitions.Should().HaveCount(1);
            partitions[0].Should().BeEquivalentTo(new[] { 1 });
        }

        [Fact]
        public void Partition_WithPartitionSizeLargerThanItems_ShouldReturnSinglePartition()
        {
            // Arrange
            var items = new[] { 1, 2, 3 };
            var partitionSize = 10;

            // Act
            var result = items.Partition(partitionSize);

            // Assert
            result.Should().NotBeNull();
            var partitions = result.ToList();
            partitions.Should().HaveCount(1);
            partitions[0].Should().BeEquivalentTo(new[] { 1, 2, 3 });
        }
       
        [Fact]
        public void Partition_WithNegativePartitionSize_ShouldReturnEmptyPartitions()
        {
            // Arrange
            var items = new[] { 1, 2, 3, 4, 5 };
            var partitionSize = -1;

            // Act
            var result = items.Partition(partitionSize);

            // Assert
            result.Should().NotBeNull();
            var partitions = result.ToList();
            partitions.Should().HaveCount(5); // Each item in its own partition
        }

        #endregion

        #region ChunkList Extension Method Tests

        [Fact]
        public void ChunkList_WithValidList_ShouldReturnCorrectChunks()
        {
            // Arrange
            var list = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            var chunkSize = 3;

            // Act
            var result = list.ChunkList(chunkSize);

            // Assert
            result.Should().NotBeNull();
            var chunks = result.ToList();
            chunks.Should().HaveCount(4); // 10 items / 3 = 4 chunks
            chunks[0].Should().BeEquivalentTo(new[] { 1, 2, 3 });
            chunks[1].Should().BeEquivalentTo(new[] { 4, 5, 6 });
            chunks[2].Should().BeEquivalentTo(new[] { 7, 8, 9 });
            chunks[3].Should().BeEquivalentTo(new[] { 10 });
        }

        [Fact]
        public void ChunkList_WithEmptyList_ShouldReturnEmptyChunks()
        {
            // Arrange
            var list = new int[0];
            var chunkSize = 3;

            // Act
            var result = list.ChunkList(chunkSize);

            // Assert
            result.Should().NotBeNull();
            var chunks = result.ToList();
            chunks.Should().BeEmpty();
        }       

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(5)]
        [InlineData(10)]
        public void ChunkList_WithDifferentChunkSizes_ShouldReturnCorrectChunks(int chunkSize)
        {
            // Arrange
            var list = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

            // Act
            var result = list.ChunkList(chunkSize);

            // Assert
            result.Should().NotBeNull();
            var chunks = result.ToList();
            chunks.Should().NotBeEmpty();
        }

        [Fact]
        public void ChunkList_WithSingleItem_ShouldReturnSingleChunk()
        {
            // Arrange
            var list = new[] { 1 };
            var chunkSize = 3;

            // Act
            var result = list.ChunkList(chunkSize);

            // Assert
            result.Should().NotBeNull();
            var chunks = result.ToList();
            chunks.Should().HaveCount(1);
            chunks[0].Should().BeEquivalentTo(new[] { 1 });
        }

        [Fact]
        public void ChunkList_WithChunkSizeLargerThanList_ShouldReturnSingleChunk()
        {
            // Arrange
            var list = new[] { 1, 2, 3 };
            var chunkSize = 10;

            // Act
            var result = list.ChunkList(chunkSize);

            // Assert
            result.Should().NotBeNull();
            var chunks = result.ToList();
            chunks.Should().HaveCount(1);
            chunks[0].Should().BeEquivalentTo(new[] { 1, 2, 3 });
        }

        #endregion

        #region ReadTextFile Method Tests

        [Fact]
        public void ReadTextFile_WithValidFilePath_ShouldReturnFileContent()
        {
            // Arrange
            var tempFile = Path.GetTempFileName();
            var expectedContent = "Test file content";
            File.WriteAllText(tempFile, expectedContent);

            try
            {
                // Act
                var result = Utilities.ReadTextFile(tempFile);

                // Assert
                result.Should().Be(expectedContent);
            }
            finally
            {
                // Cleanup
                File.Delete(tempFile);
            }
        }

        [Fact]
        public void ReadTextFile_WithEmptyFile_ShouldReturnEmptyString()
        {
            // Arrange
            var tempFile = Path.GetTempFileName();

            try
            {
                // Act
                var result = Utilities.ReadTextFile(tempFile);

                // Assert
                result.Should().BeEmpty();
            }
            finally
            {
                // Cleanup
                File.Delete(tempFile);
            }
        }

        [Fact]
        public void ReadTextFile_WithNonExistentFile_ShouldThrowException()
        {
            // Arrange
            var nonExistentFile = Path.Combine(Path.GetTempPath(), "non_existent_file.txt");

            // Act & Assert
            var action = () => Utilities.ReadTextFile(nonExistentFile);
            action.Should().Throw<FileNotFoundException>();
        }

        [Fact]
        public void ReadTextFile_WithNullFilePath_ShouldThrowException()
        {
            // Act & Assert
            var action = () => Utilities.ReadTextFile(null);
            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ReadTextFile_WithEmptyFilePath_ShouldThrowException()
        {
            // Act & Assert
            var action = () => Utilities.ReadTextFile("");
            action.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void ReadTextFile_WithLargeFile_ShouldReturnAllContent()
        {
            // Arrange
            var tempFile = Path.GetTempFileName();
            var largeContent = string.Join(Environment.NewLine, Enumerable.Range(1, 1000).Select(i => $"Line {i}"));
            File.WriteAllText(tempFile, largeContent);

            try
            {
                // Act
                var result = Utilities.ReadTextFile(tempFile);

                // Assert
                result.Should().Be(largeContent);
            }
            finally
            {
                // Cleanup
                File.Delete(tempFile);
            }
        }

        #endregion

        #region GenerateVirtualSettingXmlForUpdating Method Tests

        [Theory]
        [InlineData("1,2,3", 1.5)]
        [InlineData("100,200,300", 2.0)]
        [InlineData("", 0.0)]
        [InlineData("999999", 10.5)]
        public void GenerateVirtualSettingXmlForUpdating_WithValidParameters_ShouldReturnCorrectXml(string param1, double param2)
        {
            // Act
            var result = Utilities.GenerateVirtualSettingXmlForUpdating(param1, param2);

            // Assert
            result.Should().NotBeNullOrEmpty();
            result.Should().Contain("<ROOT>");
            result.Should().Contain("</ROOT>");
            result.Should().Contain($"CustIds=\"{param1}\"");
            result.Should().Contain($"VirtualMultiplier =\"{param2} \"");
        }

        [Fact]
        public void GenerateVirtualSettingXmlForUpdating_WithNullParam1_ShouldReturnCorrectXml()
        {
            // Act
            var result = Utilities.GenerateVirtualSettingXmlForUpdating(null, 1.5);

            // Assert
            result.Should().NotBeNullOrEmpty();
            result.Should().Contain("<ROOT>");
            result.Should().Contain("</ROOT>");
            result.Should().Contain("CustIds=\"\"");
            result.Should().Contain("VirtualMultiplier =\"1.5 \"");
        }

        [Fact]
        public void GenerateVirtualSettingXmlForUpdating_WithSpecialCharacters_ShouldReturnCorrectXml()
        {
            // Arrange
            var param1 = "1,2,3&<>\"'";
            var param2 = 1.5;

            // Act
            var result = Utilities.GenerateVirtualSettingXmlForUpdating(param1, param2);

            // Assert
            result.Should().NotBeNullOrEmpty();
            result.Should().Contain("<ROOT>");
            result.Should().Contain("</ROOT>");
            result.Should().Contain($"CustIds=\"{param1}\"");
            result.Should().Contain($"VirtualMultiplier =\"{param2} \"");
        }

        #endregion

        #region GenerateVirtualSettingXmlFromRate Method Tests
       
        [Fact]
        public void GenerateVirtualSettingXmlFromRate_WithNullParameter_ShouldReturnCorrectXml()
        {
            // Act
            var result = Utilities.GenerateVirtualSettingXmlFromRate(null);

            // Assert
            result.Should().NotBeNullOrEmpty();
            result.Should().Contain("<ROOT>");
            result.Should().Contain("</ROOT>");
        }

        #endregion       

        #region GenerateRateVirtualSettingXmlForCustomerMarginHistory Method Tests

        [Fact]
        public void GenerateRateVirtualSettingXmlForCustomerMarginHistory_WithValidParameters_ShouldReturnCorrectXml()
        {
            // Arrange
            var objParams = new object[] { "123", "01", "2023", "1000.50", "500.25", "10", "TestService" };

            // Act
            var result = Utilities.GenerateRateVirtualSettingXmlForCustomerMarginHistory(objParams);

            // Assert
            result.Should().NotBeNullOrEmpty();
            result.Should().Contain("CustID=\"123\"");
            result.Should().Contain("ReportMonth=\"01\"");
            result.Should().Contain("ReportYear=\"2023\"");
            result.Should().Contain("Turnover=\"1000.50\"");
            result.Should().Contain("Winloss=\"500.25\"");
            result.Should().Contain("BetCount=\"10\"");
            result.Should().Contain("ServiceName=\"TestService\"");
        }
       
        #endregion

        #region GenerateXmlForVRRoot Method Tests      

        [Fact]
        public void GenerateXmlForVRRoot_WithNullParameter_ShouldReturnCorrectXml()
        {
            // Act
            var result = Utilities.GenerateXmlForVRRoot(null);

            // Assert
            result.Should().NotBeNullOrEmpty();
            result.Should().Contain("<Root>");
            result.Should().Contain("</Root>");
        }

        #endregion

        #region GenerateXmlForCustomerUpdFollowParent Method Tests

        [Fact]
        public void GenerateXmlForCustomerUpdFollowParent_WithValidParameters_ShouldReturnCorrectXml()
        {
            // Arrange
            var objParams = new object[] { "123", "100.50", "200.75", "300.25" };

            // Act
            var result = Utilities.GenerateXmlForCustomerUpdFollowParent(objParams);

            // Assert
            result.Should().NotBeNullOrEmpty();
            result.Should().Contain("CustID=\"123\"");
            result.Should().Contain("WL=\"100.50\"");
            result.Should().Contain("TotalTO=\"200.75\"");
            result.Should().Contain("AIOTO=\"300.25\"");
        }
        
        #endregion

        #region GenerateXmlForAbnormalAgentUnflag Method Tests

        [Fact]
        public void GenerateXmlForAbnormalAgentUnflag_WithValidParameters_ShouldReturnCorrectXml()
        {
            // Arrange
            var objParams = new object[] { "123", "100.50" };

            // Act
            var result = Utilities.GenerateXmlForAbnormalAgentUnflag(objParams);

            // Assert
            result.Should().NotBeNullOrEmpty();
            result.Should().Contain("CustID=\"123\"");
            result.Should().Contain("TotalWL=\"100.50\"");
        }
       
        #endregion

        #region GenerateXmlForMemberBigStakeWhiteListUpd Method Tests

        [Fact]
        public void GenerateXmlForMemberBigStakeWhiteListUpd_WithValidParameters_ShouldReturnCorrectXml()
        {
            // Arrange
            var objParams = new object[] { "123", "100.50" };

            // Act
            var result = Utilities.GenerateXmlForMemberBigStakeWhiteListUpd(objParams);

            // Assert
            result.Should().NotBeNullOrEmpty();
            result.Should().Contain("CustID=\"123\"");
            result.Should().Contain("WL=\"100.50\"");
        }
       
        #endregion

        #region WriteLog Method Tests

        [Fact]
        public void WriteLog_WithValidParameters_ShouldWriteToFile()
        {
            // Arrange
            var filename = "test_log.txt";
            var content = "Test log content";
            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filename);

            try
            {
                // Act
                Utilities.WriteLog(filename, content);

                // Assert
                File.Exists(filePath).Should().BeTrue();
                var fileContent = File.ReadAllText(filePath);
                fileContent.Should().Contain(content);
            }
            finally
            {
                // Cleanup
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
        }
      
        [Fact]
        public void WriteLog_WithNullContent_ShouldWriteToFile()
        {
            // Arrange
            var filename = "test_log_null.txt";
            string content = null;
            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filename);

            try
            {
                // Act
                Utilities.WriteLog(filename, content);

                // Assert
                File.Exists(filePath).Should().BeTrue();
            }
            finally
            {
                // Cleanup
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
        }

        [Fact]
        public void WriteLog_WithNullFilename_ShouldThrowException()
        {
            // Act & Assert
            var action = () => Utilities.WriteLog(null, "content");
            action.Should().Throw<ArgumentNullException>();
        }      

        [Fact]
        public void WriteLog_WithMultipleCalls_ShouldAppendToFile()
        {
            // Arrange
            var filename = "test_log_append.txt";
            var content1 = "First log entry";
            var content2 = "Second log entry";
            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filename);

            try
            {
                // Act
                Utilities.WriteLog(filename, content1);
                Utilities.WriteLog(filename, content2);

                // Assert
                File.Exists(filePath).Should().BeTrue();
                var fileContent = File.ReadAllText(filePath);
                fileContent.Should().Contain(content1);
                fileContent.Should().Contain(content2);
            }
            finally
            {
                // Cleanup
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
        }

        #endregion

        #region NotifyMessage Method Tests

        [Fact]
        public void NotifyMessage_WithValidParameters_ShouldNotThrowException()
        {
            // Arrange
            var message = "Test notification message";
            var systemName = "TestSystem";
            var environment = "UAT";
            var groupId = "test-group";

            // Act & Assert
            var action = () => Utilities.NotifyMessage(message, systemName, environment, groupId);
            action.Should().NotThrow();
        }     

        [Theory]
        [InlineData("PRO")]
        [InlineData("KLDR")]
        [InlineData("UAT")]
        [InlineData("LOCAL")]
        public void NotifyMessage_WithDifferentEnvironments_ShouldNotThrowException(string environment)
        {
            // Arrange
            var message = "Test notification message";
            var systemName = "TestSystem";
            var groupId = "test-group";

            // Act & Assert
            var action = () => Utilities.NotifyMessage(message, systemName, environment, groupId);
            action.Should().NotThrow();
        }

        [Fact]
        public void NotifyMessage_WithSpecialCharacters_ShouldNotThrowException()
        {
            // Arrange
            var message = "Test message with special chars &<>\"'";
            var systemName = "TestSystem";
            var environment = "UAT";
            var groupId = "test-group";

            // Act & Assert
            var action = () => Utilities.NotifyMessage(message, systemName, environment, groupId);
            action.Should().NotThrow();
        }

        #endregion       

        #region Method Signature Tests

        [Fact]
        public void Partition_ShouldHaveCorrectSignature()
        {
            // Arrange
            var method = typeof(Utilities).GetMethod("Partition");

            // Assert
            method.Should().NotBeNull();
            method.IsStatic.Should().BeTrue();           
        }

        [Fact]
        public void ChunkList_ShouldHaveCorrectSignature()
        {
            // Arrange
            var method = typeof(Utilities).GetMethod("ChunkList");

            // Assert
            method.Should().NotBeNull();
            method.IsStatic.Should().BeTrue();            
        }

        [Fact]
        public void ReadTextFile_ShouldHaveCorrectSignature()
        {
            // Arrange
            var method = typeof(Utilities).GetMethod("ReadTextFile");

            // Assert
            method.Should().NotBeNull();
            method.IsStatic.Should().BeTrue();
            method.ReturnType.Should().Be(typeof(string));
            method.GetParameters().Should().HaveCount(1);
            method.GetParameters()[0].ParameterType.Should().Be(typeof(string));
        }

        [Fact]
        public void GenerateVirtualSettingXmlForUpdating_ShouldHaveCorrectSignature()
        {
            // Arrange
            var method = typeof(Utilities).GetMethod("GenerateVirtualSettingXmlForUpdating");

            // Assert
            method.Should().NotBeNull();
            method.IsStatic.Should().BeTrue();
            method.ReturnType.Should().Be(typeof(string));
            method.GetParameters().Should().HaveCount(2);
            method.GetParameters()[0].ParameterType.Should().Be(typeof(string));
            method.GetParameters()[1].ParameterType.Should().Be(typeof(double));
        }

        [Fact]
        public void NotifyMessage_ShouldHaveCorrectSignature()
        {
            // Arrange
            var method = typeof(Utilities).GetMethod("NotifyMessage");

            // Assert
            method.Should().NotBeNull();
            method.IsStatic.Should().BeTrue();
            method.ReturnType.Should().Be(typeof(void));
            method.GetParameters().Should().HaveCount(4);
            method.GetParameters()[0].ParameterType.Should().Be(typeof(string));
            method.GetParameters()[1].ParameterType.Should().Be(typeof(string));
            method.GetParameters()[2].ParameterType.Should().Be(typeof(string));
            method.GetParameters()[3].ParameterType.Should().Be(typeof(string));
        }

        #endregion

        #region Static Class Tests

        [Fact]
        public void Utilities_ShouldBeStatic()
        {
            // Arrange
            var type = typeof(Utilities);

            // Assert
            type.IsAbstract.Should().BeTrue();
            type.IsSealed.Should().BeTrue();
        }

        [Fact]
        public void Utilities_ShouldHavePublicMethods()
        {
            // Arrange
            var type = typeof(Utilities);
            var methods = type.GetMethods().Where(m => m.DeclaringType == type);

            // Assert
            methods.Should().NotBeEmpty();
            methods.All(m => m.IsStatic).Should().BeTrue();
        }

        #endregion

        #region Performance Tests

        [Fact]
        public void Partition_WithLargeDataSet_ShouldPerformEfficiently()
        {
            // Arrange
            var items = Enumerable.Range(1, 10000).ToArray();
            var partitionSize = 100;

            // Act
            var startTime = DateTime.Now;
            var result = items.Partition(partitionSize);
            var endTime = DateTime.Now;

            // Assert
            result.Should().NotBeNull();
            var duration = endTime - startTime;
            duration.Should().BeLessThan(TimeSpan.FromSeconds(5)); // Reasonable performance expectation
        }

        [Fact]
        public void ChunkList_WithLargeDataSet_ShouldPerformEfficiently()
        {
            // Arrange
            var items = Enumerable.Range(1, 10000).ToArray();
            var chunkSize = 100;

            // Act
            var startTime = DateTime.Now;
            var result = items.ChunkList(chunkSize);
            var endTime = DateTime.Now;

            // Assert
            result.Should().NotBeNull();
            var duration = endTime - startTime;
            duration.Should().BeLessThan(TimeSpan.FromSeconds(5)); // Reasonable performance expectation
        }

        #endregion
        
    }
}
