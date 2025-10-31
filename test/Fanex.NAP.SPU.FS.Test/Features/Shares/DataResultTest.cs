namespace Fanex.NAP.SPU.VR.Tests.Features.Shares
{
    using Fanex.NAP.SPU.CTS.Shares;
    using FluentAssertions;
    using System;
    using Xunit;

    /// <summary>
    /// Unit tests for DataResult class
    /// </summary>
    public class DataResultTest
    {
        #region Constructor Tests

        [Fact]
        public void Constructor_WhenCalled_ShouldInitializeWithDefaultValues()
        {
            // Act
            var result = new DataResult();

            // Assert
            result.Should().NotBeNull();
            result.ErrorCode.Should().Be(0);
            result.ErrorMessage.Should().BeNull();
        }

        [Fact]
        public void Constructor_WhenCalled_ShouldCreateNewInstance()
        {
            // Act
            var result1 = new DataResult();
            var result2 = new DataResult();

            // Assert
            result1.Should().NotBeSameAs(result2);
        }

        #endregion

        #region ErrorCode Property Tests

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(-1)]
        [InlineData(int.MaxValue)]
        [InlineData(int.MinValue)]
        [InlineData(42)]
        [InlineData(-999)]
        public void ErrorCode_WhenSet_ShouldReturnCorrectValue(int errorCode)
        {
            // Arrange
            var result = new DataResult();

            // Act
            result.ErrorCode = errorCode;

            // Assert
            result.ErrorCode.Should().Be(errorCode);
        }

        [Fact]
        public void ErrorCode_WhenSetMultipleTimes_ShouldReturnLastValue()
        {
            // Arrange
            var result = new DataResult();

            // Act
            result.ErrorCode = 10;
            result.ErrorCode = 20;
            result.ErrorCode = 30;

            // Assert
            result.ErrorCode.Should().Be(30);
        }

        [Fact]
        public void ErrorCode_WhenSetToZero_ShouldReturnZero()
        {
            // Arrange
            var result = new DataResult();
            result.ErrorCode = 100;

            // Act
            result.ErrorCode = 0;

            // Assert
            result.ErrorCode.Should().Be(0);
        }

        #endregion

        #region ErrorMessage Property Tests

        [Theory]
        [InlineData("")]
        [InlineData("Error message")]
        [InlineData("Database connection failed")]
        [InlineData("Invalid parameter")]
        [InlineData("System error occurred")]
        [InlineData("Success")]
        public void ErrorMessage_WhenSet_ShouldReturnCorrectValue(string errorMessage)
        {
            // Arrange
            var result = new DataResult();

            // Act
            result.ErrorMessage = errorMessage;

            // Assert
            result.ErrorMessage.Should().Be(errorMessage);
        }

        [Fact]
        public void ErrorMessage_WhenSetToNull_ShouldReturnNull()
        {
            // Arrange
            var result = new DataResult();
            result.ErrorMessage = "Some error";

            // Act
            result.ErrorMessage = null;

            // Assert
            result.ErrorMessage.Should().BeNull();
        }

        [Fact]
        public void ErrorMessage_WhenSetMultipleTimes_ShouldReturnLastValue()
        {
            // Arrange
            var result = new DataResult();

            // Act
            result.ErrorMessage = "First error";
            result.ErrorMessage = "Second error";
            result.ErrorMessage = "Third error";

            // Assert
            result.ErrorMessage.Should().Be("Third error");
        }

        [Fact]
        public void ErrorMessage_WhenSetToEmptyString_ShouldReturnEmptyString()
        {
            // Arrange
            var result = new DataResult();

            // Act
            result.ErrorMessage = "";

            // Assert
            result.ErrorMessage.Should().Be("");
        }

        [Fact]
        public void ErrorMessage_WhenSetToWhitespace_ShouldReturnWhitespace()
        {
            // Arrange
            var result = new DataResult();

            // Act
            result.ErrorMessage = "   ";

            // Assert
            result.ErrorMessage.Should().Be("   ");
        }

        [Fact]
        public void ErrorMessage_WhenSetToLongString_ShouldReturnLongString()
        {
            // Arrange
            var result = new DataResult();
            var longMessage = new string('A', 10000);

            // Act
            result.ErrorMessage = longMessage;

            // Assert
            result.ErrorMessage.Should().Be(longMessage);
            result.ErrorMessage.Should().HaveLength(10000);
        }

        [Fact]
        public void ErrorMessage_WhenSetToSpecialCharacters_ShouldReturnSpecialCharacters()
        {
            // Arrange
            var result = new DataResult();
            var specialMessage = "Error: & < > \" ' \n \r \t \\ /";

            // Act
            result.ErrorMessage = specialMessage;

            // Assert
            result.ErrorMessage.Should().Be(specialMessage);
        }

        [Fact]
        public void ErrorMessage_WhenSetToUnicodeCharacters_ShouldReturnUnicodeCharacters()
        {
            // Arrange
            var result = new DataResult();
            var unicodeMessage = "错误消息: 数据库连接失败 🚫 ❌ ⚠️";

            // Act
            result.ErrorMessage = unicodeMessage;

            // Assert
            result.ErrorMessage.Should().Be(unicodeMessage);
        }

        #endregion

        #region Combined Property Tests

        [Fact]
        public void Properties_WhenSetTogether_ShouldMaintainBothValues()
        {
            // Arrange
            var result = new DataResult();

            // Act
            result.ErrorCode = 404;
            result.ErrorMessage = "Not Found";

            // Assert
            result.ErrorCode.Should().Be(404);
            result.ErrorMessage.Should().Be("Not Found");
        }

        [Fact]
        public void Properties_WhenSetInDifferentOrder_ShouldMaintainBothValues()
        {
            // Arrange
            var result = new DataResult();

            // Act
            result.ErrorMessage = "Database Error";
            result.ErrorCode = 500;

            // Assert
            result.ErrorCode.Should().Be(500);
            result.ErrorMessage.Should().Be("Database Error");
        }

        [Fact]
        public void Properties_WhenSetMultipleTimes_ShouldMaintainLastValues()
        {
            // Arrange
            var result = new DataResult();

            // Act
            result.ErrorCode = 100;
            result.ErrorMessage = "First error";
            result.ErrorCode = 200;
            result.ErrorMessage = "Second error";
            result.ErrorCode = 300;
            result.ErrorMessage = "Third error";

            // Assert
            result.ErrorCode.Should().Be(300);
            result.ErrorMessage.Should().Be("Third error");
        }

        #endregion

        #region Object Equality Tests

        [Fact]
        public void Equals_WhenComparingSameInstance_ShouldReturnTrue()
        {
            // Arrange
            var result = new DataResult { ErrorCode = 1, ErrorMessage = "Test" };

            // Act
            var equals = result.Equals(result);

            // Assert
            equals.Should().BeTrue();
        }

        [Fact]
        public void Equals_WhenComparingWithNull_ShouldReturnFalse()
        {
            // Arrange
            var result = new DataResult { ErrorCode = 1, ErrorMessage = "Test" };

            // Act
            var equals = result.Equals(null);

            // Assert
            equals.Should().BeFalse();
        }

        [Fact]
        public void Equals_WhenComparingWithDifferentType_ShouldReturnFalse()
        {
            // Arrange
            var result = new DataResult { ErrorCode = 1, ErrorMessage = "Test" };
            var otherObject = "Not a DataResult";

            // Act
            var equals = result.Equals(otherObject);

            // Assert
            equals.Should().BeFalse();
        }      

        [Fact]
        public void Equals_WhenComparingWithDifferentErrorCode_ShouldReturnFalse()
        {
            // Arrange
            var result1 = new DataResult { ErrorCode = 1, ErrorMessage = "Test" };
            var result2 = new DataResult { ErrorCode = 2, ErrorMessage = "Test" };

            // Act
            var equals = result1.Equals(result2);

            // Assert
            equals.Should().BeFalse();
        }

        [Fact]
        public void Equals_WhenComparingWithDifferentErrorMessage_ShouldReturnFalse()
        {
            // Arrange
            var result1 = new DataResult { ErrorCode = 1, ErrorMessage = "Test1" };
            var result2 = new DataResult { ErrorCode = 1, ErrorMessage = "Test2" };

            // Act
            var equals = result1.Equals(result2);

            // Assert
            equals.Should().BeFalse();
        }

        [Fact]
        public void Equals_WhenComparingWithNullErrorMessage_ShouldReturnFalse()
        {
            // Arrange
            var result1 = new DataResult { ErrorCode = 1, ErrorMessage = "Test" };
            var result2 = new DataResult { ErrorCode = 1, ErrorMessage = null };

            // Act
            var equals = result1.Equals(result2);

            // Assert
            equals.Should().BeFalse();
        }
       
        #endregion

        #region GetHashCode Tests

        [Fact]
        public void GetHashCode_WhenCalledMultipleTimesOnSameInstance_ShouldReturnSameValue()
        {
            // Arrange
            var result = new DataResult { ErrorCode = 1, ErrorMessage = "Test" };

            // Act
            var hash1 = result.GetHashCode();
            var hash2 = result.GetHashCode();

            // Assert
            hash1.Should().Be(hash2);
        }      

        [Fact]
        public void GetHashCode_WhenCalledOnInstancesWithDifferentValues_ShouldReturnDifferentValues()
        {
            // Arrange
            var result1 = new DataResult { ErrorCode = 1, ErrorMessage = "Test1" };
            var result2 = new DataResult { ErrorCode = 2, ErrorMessage = "Test2" };

            // Act
            var hash1 = result1.GetHashCode();
            var hash2 = result2.GetHashCode();

            // Assert
            hash1.Should().NotBe(hash2);
        }

        #endregion
        
        #region Edge Cases and Boundary Tests

        [Fact]
        public void ErrorCode_WhenSetToMaxValue_ShouldReturnMaxValue()
        {
            // Arrange
            var result = new DataResult();

            // Act
            result.ErrorCode = int.MaxValue;

            // Assert
            result.ErrorCode.Should().Be(int.MaxValue);
        }

        [Fact]
        public void ErrorCode_WhenSetToMinValue_ShouldReturnMinValue()
        {
            // Arrange
            var result = new DataResult();

            // Act
            result.ErrorCode = int.MinValue;

            // Assert
            result.ErrorCode.Should().Be(int.MinValue);
        }

        [Fact]
        public void ErrorMessage_WhenSetToVeryLongString_ShouldHandleCorrectly()
        {
            // Arrange
            var result = new DataResult();
            var veryLongMessage = new string('X', 1000000); // 1 million characters

            // Act
            result.ErrorMessage = veryLongMessage;

            // Assert
            result.ErrorMessage.Should().Be(veryLongMessage);
            result.ErrorMessage.Should().HaveLength(1000000);
        }

        [Fact]
        public void ErrorMessage_WhenSetToEmptyString_ShouldHandleCorrectly()
        {
            // Arrange
            var result = new DataResult();

            // Act
            result.ErrorMessage = "";

            // Assert
            result.ErrorMessage.Should().Be("");
            result.ErrorMessage.Should().HaveLength(0);
        }

        [Fact]
        public void ErrorMessage_WhenSetToWhitespaceOnly_ShouldHandleCorrectly()
        {
            // Arrange
            var result = new DataResult();

            // Act
            result.ErrorMessage = "   \t\n\r   ";

            // Assert
            result.ErrorMessage.Should().Be("   \t\n\r   ");
        }

        #endregion

        #region Success and Error Scenarios Tests

        [Fact]
        public void DataResult_WhenUsedForSuccess_ShouldHaveZeroErrorCode()
        {
            // Arrange & Act
            var successResult = new DataResult
            {
                ErrorCode = 0,
                ErrorMessage = "Success"
            };

            // Assert
            successResult.ErrorCode.Should().Be(0);
            successResult.ErrorMessage.Should().Be("Success");
        }

        [Fact]
        public void DataResult_WhenUsedForError_ShouldHaveNonZeroErrorCode()
        {
            // Arrange & Act
            var errorResult = new DataResult
            {
                ErrorCode = -1,
                ErrorMessage = "An error occurred"
            };

            // Assert
            errorResult.ErrorCode.Should().Be(-1);
            errorResult.ErrorMessage.Should().Be("An error occurred");
        }

        [Fact]
        public void DataResult_WhenUsedForDatabaseError_ShouldHaveAppropriateValues()
        {
            // Arrange & Act
            var dbErrorResult = new DataResult
            {
                ErrorCode = 500,
                ErrorMessage = "Database connection failed"
            };

            // Assert
            dbErrorResult.ErrorCode.Should().Be(500);
            dbErrorResult.ErrorMessage.Should().Be("Database connection failed");
        }

        [Fact]
        public void DataResult_WhenUsedForValidationError_ShouldHaveAppropriateValues()
        {
            // Arrange & Act
            var validationErrorResult = new DataResult
            {
                ErrorCode = 400,
                ErrorMessage = "Invalid input parameters"
            };

            // Assert
            validationErrorResult.ErrorCode.Should().Be(400);
            validationErrorResult.ErrorMessage.Should().Be("Invalid input parameters");
        }

        #endregion

        #region Immutability and Thread Safety Tests

        [Fact]
        public void DataResult_WhenPropertiesAreSet_ShouldNotAffectOtherInstances()
        {
            // Arrange
            var result1 = new DataResult { ErrorCode = 1, ErrorMessage = "Error 1" };
            var result2 = new DataResult { ErrorCode = 2, ErrorMessage = "Error 2" };

            // Act
            result1.ErrorCode = 100;
            result1.ErrorMessage = "Updated Error 1";

            // Assert
            result1.ErrorCode.Should().Be(100);
            result1.ErrorMessage.Should().Be("Updated Error 1");
            result2.ErrorCode.Should().Be(2);
            result2.ErrorMessage.Should().Be("Error 2");
        }

        #endregion
    }
}
