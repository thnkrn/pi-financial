using Pi.SetService.Application.Exceptions;
using Pi.SetService.Application.Extensions;

namespace Pi.SetService.Application.Tests.Extensions;

public class NullableExtensionsTest
{
    [Fact]
    public void Should_Error_When_GetRequiredValue()
    {
        // Arrange
        int? nullableValue = 42;
        var propertyName = "nullableValue";

        // Act
        var result = nullableValue.GetRequiredValue(propertyName);

        // Assert
        Assert.Equal(nullableValue, result);
    }

    [Fact]
    public void Should_Error_When_GetRequiredValue_And_Null()
    {
        // Arrange
        int? nullableValue = null;
        var propertyName = "nullableValue";

        // Act
        var exception = Assert.Throws<RequiredFieldNotfoundException>(
            () => nullableValue.GetRequiredValue(propertyName)
        );

        // Assert
        Assert.Contains(propertyName, exception.Message);
    }
}
