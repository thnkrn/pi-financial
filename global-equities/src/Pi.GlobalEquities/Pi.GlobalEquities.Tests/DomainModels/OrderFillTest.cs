using Pi.GlobalEquities.DomainModels;
using Xunit;

namespace Pi.GlobalEquities.Tests.DomainModels;

public class OrderFillTest
{
    public class Equals_Test
    {
        [Fact]
        public void Equals_WhenObjectHaveSamePropertiesValue_ReturnTrue()
        {
            var utcNow = DateTime.UtcNow;
            var left = new OrderFill { Quantity = 100.23m, Price = 9.19m, Timestamp = utcNow.AddMilliseconds(1) };
            var right = new OrderFill { Quantity = 100.23m, Price = 9.19m, Timestamp = utcNow.AddMilliseconds(1) };

            Assert.True(left.Equals(right));
        }

        public static IEnumerable<object[]> FalseEqualsOrderFills()
        {
            var utcNow = DateTime.UtcNow;
            yield return new object[]
            {
                new OrderFill { Quantity = 100.23m, Price = 9.19m, Timestamp = utcNow.AddMilliseconds(1) },
                new OrderFill { Quantity = 100.22m, Price = 9.19m, Timestamp = utcNow.AddMilliseconds(1) }
            };
            yield return new object[]
            {
                new OrderFill { Quantity = 100.23m, Price = 9.19m, Timestamp = utcNow.AddMilliseconds(1) },
                new OrderFill { Quantity = 100.23m, Price = 9.18m, Timestamp = utcNow.AddMilliseconds(1) }
            };
            yield return new object[]
            {
                new OrderFill { Quantity = 100.23m, Price = 9.19m, Timestamp = utcNow.AddMilliseconds(1) },
                new OrderFill { Quantity = 100.23m, Price = 9.19m, Timestamp = utcNow.AddMilliseconds(-1) }
            };
        }

        [Theory]
        [MemberData(nameof(FalseEqualsOrderFills))]
        public void Equals_WhenObjectHaveDifferentPropertiesValue_ReturnFalse(OrderFill left, OrderFill right)
        {
            Assert.NotEqual(left, right);
        }

        [Fact]
        public void Equals_WhenRightObjectIsNull_ReturnFalse()
        {
            var left = new OrderFill { Quantity = 100.23m, Price = 9.19m, Timestamp = DateTime.UtcNow };
            OrderFill right = default;

            Assert.NotEqual(left, right);
        }

        [Fact]
        public void Equals_WhenRightObjectIsNotOrderFill_ReturnFalse()
        {
            var utcNow = DateTime.UtcNow;
            var left = new OrderFill { Quantity = 100.23m, Price = 9.19m, Timestamp = utcNow.AddMilliseconds(1) };
            var right = new { Quantity = 100.23m, Price = 9.19m, Timestamp = utcNow.AddMilliseconds(1) };

            Assert.False(left.Equals(right));
        }
    }

    public class GetHashCode_Test
    {
        [Fact]
        public void GetHashCode_WhenObjectsAreEqual_ReturnsSameHashCode()
        {
            var utcNow = DateTime.UtcNow;
            var left = new OrderFill { Quantity = 100.23m, Price = 9.19m, Timestamp = utcNow.AddMilliseconds(1) };
            var right = new OrderFill { Quantity = 100.23m, Price = 9.19m, Timestamp = utcNow.AddMilliseconds(1) };

            Assert.Equal(left.GetHashCode(), right.GetHashCode());
        }

        [Fact]
        public void GetHashCode_WhenObjectsAreNotEqual_ReturnsDifferentHashCode()
        {
            var utcNow = DateTime.UtcNow;
            var left = new OrderFill { Quantity = 100.23m, Price = 9.19m, Timestamp = utcNow.AddMilliseconds(1) };
            var right = new OrderFill { Quantity = 100.23m, Price = 9.19m, Timestamp = utcNow.AddMilliseconds(-1) };

            Assert.NotEqual(left.GetHashCode(), right.GetHashCode());
        }
    }
}
