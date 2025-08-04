using Pi.Common.CommonModels;
using Pi.GlobalEquities.DomainModels;
using Xunit;

namespace Pi.GlobalEquities.Tests.DomainModels;

public class PositionTest
{
    public class CanSell_Test
    {
        [Fact]
        void QuantitySellMoreThanAvaiQuantity_ReturnTrue()
        {
            var pos = new Position { AvailableQuantity = 200m };

            var result = pos.CanSell(300);

            Assert.False(result);
        }

        [Fact]
        void QuantitySellLessThanAvaiQuantity_ReturnTrue()
        {
            var pos = new Position { AvailableQuantity = 200m };

            var result = pos.CanSell(100);

            Assert.True(result);
        }

        [Fact]
        void QuantitySellLessThanZero_ReturnTrue()
        {
            var pos = new Position { AvailableQuantity = 200m };

            var result = pos.CanSell(-100);

            Assert.False(result);
        }
    }
}
