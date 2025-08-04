using Pi.SetService.Application.Factories;
using Pi.SetService.Application.Models;

namespace Pi.SetService.Application.Tests.Factories;

public class FilterFactoryTest
{
    [Theory]
    [InlineData(true, false, true)]
    [InlineData(true, true, true)]
    [InlineData(true, null, true)]
    [InlineData(false, true, false)]
    [InlineData(null, true, false)]
    [InlineData(false, false, null)]
    [InlineData(null, false, null)]
    [InlineData(null, null, null)]
    public void Should_Return_Expected_When_NewSblOrderFilters_With_SetOrderFilters(bool? open, bool? history, bool? expected)
    {
        // Arrange
        var setFilter = new SetOrderFilters
        {
            EffectiveDateFrom = DateOnly.FromDateTime(DateTime.Now),
            EffectiveDateTo = DateOnly.FromDateTime(DateTime.Now),
            OpenOrder = open,
            HistoryOrder = history
        };

        // Act
        var actual = FilterFactory.NewSblOrderFilters(setFilter);

        // Assert
        Assert.Equal(setFilter.EffectiveDateFrom, actual.CreatedDateFrom);
        Assert.Equal(setFilter.EffectiveDateTo, actual.CreatedDateTo);
        Assert.Equal(expected, actual.Open);
    }
}
