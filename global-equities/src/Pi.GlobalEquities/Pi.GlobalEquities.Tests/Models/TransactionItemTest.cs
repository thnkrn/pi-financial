using Pi.GlobalEquities.Models;
using Xunit;

namespace Pi.GlobalEquities.Tests.Models;

public class TransactionItems
{
    public class SymbolId_Test
    {
        [Fact]
        void WhenSymbolIsNullOrWhiteSpace_ReturnNull()
        {
            var transactionItem = new TransactionItem { Symbol = null, Venue = "NASDAQ" };

            Assert.Null(transactionItem.SymbolId);
        }

        [Fact]
        void WhenVenueIsNullOrWhiteSpace_ReturnNull()
        {
            var transactionItem = new TransactionItem { Symbol = "DDOG", Venue = null };

            Assert.Null(transactionItem.SymbolId);
        }

        [Fact]
        void WhenSymbolAndVenueIsNotNullOrWhiteSpace_ReturnNull()
        {
            var transactionItem = new TransactionItem { Symbol = "DDOG", Venue = "NASDAQ" };

            Assert.Equal("DDOG.NASDAQ", transactionItem.SymbolId);
        }
    }
}
