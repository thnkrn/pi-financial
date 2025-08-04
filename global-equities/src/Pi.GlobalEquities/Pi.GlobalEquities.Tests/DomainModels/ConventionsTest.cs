using FluentAssertions;
using Pi.GlobalEquities.DomainModels;
using Pi.GlobalEquities.Models;
using Xunit;
namespace Pi.GlobalEquities.Tests.DomainModels;

public class ConventionsTest
{
    public class IsGeId_Test
    {
        [Fact]
        public void WhenPrefixIsCorrectAndNotNull_ReturnTrue()
        {
            var str =
                "GE01|cb25452b-3e99-4656-9b92-ce391bb3b551|91c72754-536d-4343-84d2-43d9e5013f22.jcM/Ga9TfGjb/CiW3FxuxxwN4Jf3zSETiLhubQOO30uPQJ2sA+XvbzPsulgFGFuy3hDtq8nJFA3aNiK7TCVS/q3qU6K2J5JiNV7jkPRuivHb58ouTY6arvIaxCRF1TmXQeaNLBzAS4XOWPnxYMEsq6cCRBqbus+iJKqPYTOH7BE=";

            var result = Conventions.OrderPrefix.IsGeId(str);

            Assert.True(result);
        }

        [Theory]
        [InlineData("GE|Test")]
        [InlineData("GE01")]
        [InlineData("")]
        [InlineData(null)]
        public void WhenPrefixIsNotCorrectOrNull_ReturnFalse(string str)
        {
            var result = Conventions.OrderPrefix.IsGeId(str);

            Assert.False(result);
        }
    }


    public class Create_Test
    {
        [Fact]
        public void WhenCreateClientTag_ReturnClientTag()
        {
            var userId = "cb25452b-3e99-4656-9b92-ce391bb3b551";
            var accountId = "91c72754-536d-4343-84d2-43d9e5013f22";

            var result = Conventions.OrderPrefix.Create(userId, accountId);

            var clientTag = "GE01|cb25452b-3e99-4656-9b92-ce391bb3b551|91c72754-536d-4343-84d2-43d9e5013f22";
            Assert.Equal(clientTag, result);
        }
    }

    public class Extract_Test
    {
        [Fact]
        public void WhenExtractClientTag_ReturnOrderTagInfo()
        {
            var clientTag = "GE01|cb25452b-3e99-4656-9b92-ce391bb3b551|91c72754-536d-4343-84d2-43d9e5013f22|orderType=TakeProfit";

            var result = Conventions.OrderPrefix.Extract(clientTag);

            var orderidentity = new OrderTagInfo
            {
                UserId = "cb25452b-3e99-4656-9b92-ce391bb3b551",
                AccountId = "91c72754-536d-4343-84d2-43d9e5013f22",
                OrderType = OrderType.TakeProfit
            };

            result.Should().BeEquivalentTo(orderidentity);
        }

        [Theory]
        [InlineData("")]
        [InlineData("GE01")]
        [InlineData("GE01|cb25452b-3e99-4656-9b92-ce391bb3b551")]
        [InlineData("GE01|cb25452b-3e99-4656-9b92-ce391bb3b551|91c72754-536d-4343-84d2-43d9e5013f22|123|xxx")]
        public void WhenExtractIncorrectClientTagPattern_ThrowError(string clientTag)
        {
            var action = () => Conventions.OrderPrefix.Extract(clientTag);

            var exception = Assert.Throws<InvalidDataException>(action);

            var errMes = clientTag + " is not valid.";
            Assert.Equal(errMes, exception.Message);
        }
    }
}
