namespace Fanex.NAP.SPU.VR.Tests.Features.Shares
{
    using Fanex.NAP.SPU.CTS.Shares;
    using FluentAssertions;
    using System;
    using System.Reflection;
    using Xunit;

    /// <summary>
    /// Unit tests for XNameAttribute class
    /// </summary>
    public class XNameAttributeTest
    {
        #region Constructor Tests

        [Fact]
        public void Constructor_WhenValidName_ShouldSetElementName()
        {
            // Arrange
            var expectedName = "TestElement";

            // Act
            var attribute = new XNameAttribute(expectedName);

            // Assert
            attribute.ElementName.Should().Be(expectedName);
        }

        [Fact]
        public void Constructor_WhenEmptyName_ShouldSetElementName()
        {
            // Arrange
            var expectedName = "";

            // Act
            var attribute = new XNameAttribute(expectedName);

            // Assert
            attribute.ElementName.Should().Be(expectedName);
        }

        [Fact]
        public void Constructor_WhenNullName_ShouldSetElementName()
        {
            // Arrange
            string expectedName = null;

            // Act
            var attribute = new XNameAttribute(expectedName);

            // Assert
            attribute.ElementName.Should().Be(expectedName);
        }

        [Fact]
        public void Constructor_WhenNameWithSpecialCharacters_ShouldSetElementName()
        {
            // Arrange
            var expectedName = "Test-Element_With.Special@Characters#123";

            // Act
            var attribute = new XNameAttribute(expectedName);

            // Assert
            attribute.ElementName.Should().Be(expectedName);
        }

        [Fact]
        public void Constructor_WhenNameWithSpaces_ShouldSetElementName()
        {
            // Arrange
            var expectedName = "Test Element With Spaces";

            // Act
            var attribute = new XNameAttribute(expectedName);

            // Assert
            attribute.ElementName.Should().Be(expectedName);
        }

        [Fact]
        public void Constructor_WhenNameWithUnicodeCharacters_ShouldSetElementName()
        {
            // Arrange
            var expectedName = "TestElement_测试_العربية_русский";

            // Act
            var attribute = new XNameAttribute(expectedName);

            // Assert
            attribute.ElementName.Should().Be(expectedName);
        }

        [Fact]
        public void Constructor_WhenVeryLongName_ShouldSetElementName()
        {
            // Arrange
            var expectedName = new string('A', 10000);

            // Act
            var attribute = new XNameAttribute(expectedName);

            // Assert
            attribute.ElementName.Should().Be(expectedName);
        }

        #endregion

        #region ElementName Property Tests

        [Fact]
        public void ElementName_WhenSet_ShouldReturnCorrectValue()
        {
            // Arrange
            var expectedName = "TestElement";
            var attribute = new XNameAttribute(expectedName);

            // Act
            var actualName = attribute.ElementName;

            // Assert
            actualName.Should().Be(expectedName);
        }

        [Fact]
        public void ElementName_WhenSetToEmpty_ShouldReturnEmpty()
        {
            // Arrange
            var expectedName = "";
            var attribute = new XNameAttribute(expectedName);

            // Act
            var actualName = attribute.ElementName;

            // Assert
            actualName.Should().Be(expectedName);
        }

        [Fact]
        public void ElementName_WhenSetToNull_ShouldReturnNull()
        {
            // Arrange
            string expectedName = null;
            var attribute = new XNameAttribute(expectedName);

            // Act
            var actualName = attribute.ElementName;

            // Assert
            actualName.Should().Be(expectedName);
        }

        [Fact]
        public void ElementName_ShouldBeReadOnly()
        {
            // Arrange
            var attribute = new XNameAttribute("TestElement");

            // Act & Assert
            attribute.GetType().GetProperty("ElementName").Should().NotBeNull();
            attribute.GetType().GetProperty("ElementName").CanWrite.Should().BeFalse();
        }

        #endregion

        #region Attribute Usage Tests

        [Fact]
        public void XNameAttribute_WhenAppliedToClass_ShouldBeAccessible()
        {
            // Arrange
            var testClass = new TestClassWithXNameAttribute();

            // Act
            var attributes = testClass.GetType().GetCustomAttributes(typeof(XNameAttribute), false);

            // Assert
            attributes.Should().HaveCount(1);
            var xNameAttribute = attributes[0] as XNameAttribute;
            xNameAttribute.Should().NotBeNull();
            xNameAttribute.ElementName.Should().Be("TestClass");
        }

        [Fact]
        public void XNameAttribute_WhenAppliedToProperty_ShouldBeAccessible()
        {
            // Arrange
            var testClass = new TestClassWithXNameAttribute();

            // Act
            var property = testClass.GetType().GetProperty("TestProperty");
            var attributes = property.GetCustomAttributes(typeof(XNameAttribute), false);

            // Assert
            attributes.Should().HaveCount(1);
            var xNameAttribute = attributes[0] as XNameAttribute;
            xNameAttribute.Should().NotBeNull();
            xNameAttribute.ElementName.Should().Be("TestProperty");
        }

        [Fact]
        public void XNameAttribute_WhenAppliedToMultipleProperties_ShouldBeAccessibleOnEach()
        {
            // Arrange
            var testClass = new TestClassWithMultipleXNameAttributes();

            // Act
            var property1 = testClass.GetType().GetProperty("Property1");
            var property2 = testClass.GetType().GetProperty("Property2");
            var attributes1 = property1.GetCustomAttributes(typeof(XNameAttribute), false);
            var attributes2 = property2.GetCustomAttributes(typeof(XNameAttribute), false);

            // Assert
            attributes1.Should().HaveCount(1);
            attributes2.Should().HaveCount(1);
            
            var xNameAttribute1 = attributes1[0] as XNameAttribute;
            var xNameAttribute2 = attributes2[0] as XNameAttribute;
            
            xNameAttribute1.ElementName.Should().Be("Property1");
            xNameAttribute2.ElementName.Should().Be("Property2");
        }

        [Fact]
        public void XNameAttribute_WhenNotApplied_ShouldReturnEmptyArray()
        {
            // Arrange
            var testClass = new TestClassWithoutXNameAttribute();

            // Act
            var attributes = testClass.GetType().GetCustomAttributes(typeof(XNameAttribute), false);

            // Assert
            attributes.Should().BeEmpty();
        }

        #endregion

        #region Attribute Target Tests

        [Fact]
        public void XNameAttribute_ShouldAllowClassTarget()
        {
            // Arrange & Act
            var attribute = new XNameAttribute("Test");

            // Assert
            attribute.GetType().GetCustomAttributes(typeof(AttributeUsageAttribute), false)
                .Should().HaveCount(1);
            
            var usageAttribute = attribute.GetType().GetCustomAttributes(typeof(AttributeUsageAttribute), false)[0] as AttributeUsageAttribute;
            usageAttribute.ValidOn.Should().HaveFlag(AttributeTargets.Class);
        }

        [Fact]
        public void XNameAttribute_ShouldAllowPropertyTarget()
        {
            // Arrange & Act
            var attribute = new XNameAttribute("Test");

            // Assert
            var usageAttribute = attribute.GetType().GetCustomAttributes(typeof(AttributeUsageAttribute), false)[0] as AttributeUsageAttribute;
            usageAttribute.ValidOn.Should().HaveFlag(AttributeTargets.Property);
        }

        [Fact]
        public void XNameAttribute_ShouldNotAllowMethodTarget()
        {
            // Arrange & Act
            var attribute = new XNameAttribute("Test");

            // Assert
            var usageAttribute = attribute.GetType().GetCustomAttributes(typeof(AttributeUsageAttribute), false)[0] as AttributeUsageAttribute;
            usageAttribute.ValidOn.Should().NotHaveFlag(AttributeTargets.Method);
        }

        [Fact]
        public void XNameAttribute_ShouldNotAllowFieldTarget()
        {
            // Arrange & Act
            var attribute = new XNameAttribute("Test");

            // Assert
            var usageAttribute = attribute.GetType().GetCustomAttributes(typeof(AttributeUsageAttribute), false)[0] as AttributeUsageAttribute;
            usageAttribute.ValidOn.Should().NotHaveFlag(AttributeTargets.Field);
        }

        #endregion

        #region Edge Case Tests

        [Fact]
        public void XNameAttribute_WhenNameContainsXmlSpecialCharacters_ShouldPreserveThem()
        {
            // Arrange
            var expectedName = "<Test>Element</Test>";

            // Act
            var attribute = new XNameAttribute(expectedName);

            // Assert
            attribute.ElementName.Should().Be(expectedName);
        }

        [Fact]
        public void XNameAttribute_WhenNameContainsNewlines_ShouldPreserveThem()
        {
            // Arrange
            var expectedName = "Test\nElement\r\nWith\nNewlines";

            // Act
            var attribute = new XNameAttribute(expectedName);

            // Assert
            attribute.ElementName.Should().Be(expectedName);
        }

        [Fact]
        public void XNameAttribute_WhenNameContainsTabs_ShouldPreserveThem()
        {
            // Arrange
            var expectedName = "Test\tElement\tWith\tTabs";

            // Act
            var attribute = new XNameAttribute(expectedName);

            // Assert
            attribute.ElementName.Should().Be(expectedName);
        }

        [Fact]
        public void XNameAttribute_WhenNameIsWhitespaceOnly_ShouldPreserveIt()
        {
            // Arrange
            var expectedName = "   \t\n   ";

            // Act
            var attribute = new XNameAttribute(expectedName);

            // Assert
            attribute.ElementName.Should().Be(expectedName);
        }

        [Fact]
        public void XNameAttribute_WhenNameContainsQuotes_ShouldPreserveThem()
        {
            // Arrange
            var expectedName = "Test\"Element\"With\"Quotes";

            // Act
            var attribute = new XNameAttribute(expectedName);

            // Assert
            attribute.ElementName.Should().Be(expectedName);
        }

        [Fact]
        public void XNameAttribute_WhenNameContainsApostrophes_ShouldPreserveThem()
        {
            // Arrange
            var expectedName = "Test'Element'With'Apostrophes";

            // Act
            var attribute = new XNameAttribute(expectedName);

            // Assert
            attribute.ElementName.Should().Be(expectedName);
        }

        [Fact]
        public void XNameAttribute_WhenNameContainsBackslashes_ShouldPreserveThem()
        {
            // Arrange
            var expectedName = "Test\\Element\\With\\Backslashes";

            // Act
            var attribute = new XNameAttribute(expectedName);

            // Assert
            attribute.ElementName.Should().Be(expectedName);
        }

        [Fact]
        public void XNameAttribute_WhenNameContainsForwardSlashes_ShouldPreserveThem()
        {
            // Arrange
            var expectedName = "Test/Element/With/ForwardSlashes";

            // Act
            var attribute = new XNameAttribute(expectedName);

            // Assert
            attribute.ElementName.Should().Be(expectedName);
        }

        [Fact]
        public void XNameAttribute_WhenNameContainsNumbers_ShouldPreserveThem()
        {
            // Arrange
            var expectedName = "Test123Element456With789Numbers";

            // Act
            var attribute = new XNameAttribute(expectedName);

            // Assert
            attribute.ElementName.Should().Be(expectedName);
        }

        [Fact]
        public void XNameAttribute_WhenNameStartsWithNumber_ShouldPreserveIt()
        {
            // Arrange
            var expectedName = "123TestElement";

            // Act
            var attribute = new XNameAttribute(expectedName);

            // Assert
            attribute.ElementName.Should().Be(expectedName);
        }

        [Fact]
        public void XNameAttribute_WhenNameEndsWithNumber_ShouldPreserveIt()
        {
            // Arrange
            var expectedName = "TestElement123";

            // Act
            var attribute = new XNameAttribute(expectedName);

            // Assert
            attribute.ElementName.Should().Be(expectedName);
        }

        #endregion

        #region Test Helper Classes

        [XName("TestClass")]
        private class TestClassWithXNameAttribute
        {
            [XName("TestProperty")]
            public string TestProperty { get; set; }
        }

        private class TestClassWithMultipleXNameAttributes
        {
            [XName("Property1")]
            public string Property1 { get; set; }

            [XName("Property2")]
            public string Property2 { get; set; }
        }

        private class TestClassWithoutXNameAttribute
        {
            public string Property { get; set; }
        }

        #endregion
    }
}
