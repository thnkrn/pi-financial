using System.ComponentModel;
using Pi.Financial.FundService.Application.Utils;

namespace Pi.Financial.FundService.Application.Tests.Utils;

public class MyEnumExtensionsTest
{
    public enum Mock
    {
        [Description("Mock 1")]
        Mock1 = 1,
        [Description("Mock 2")]
        Mock2 = 2,
        [Description("Mock 3")]
        Mock3 = 3,
    }

    [Theory]
    [InlineData(Mock.Mock1, "Mock 1")]
    [InlineData(Mock.Mock2, "Mock 2")]
    [InlineData(Mock.Mock3, "Mock 3")]
    public void Should_ReturnDescription_When_Call_ToDescriptionString(Mock mock, string expected)
    {
        // Act
        var actual = mock.ToDescriptionString();

        // Assert
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(Mock.Mock1, "1")]
    [InlineData(Mock.Mock2, "2")]
    [InlineData(Mock.Mock3, "3")]
    public void Should_ReturnDescription_When_Call_NumberString(Mock mock, string expected)
    {
        // Act
        var actual = mock.NumberString();

        // Assert
        Assert.Equal(expected, actual);
    }
}
