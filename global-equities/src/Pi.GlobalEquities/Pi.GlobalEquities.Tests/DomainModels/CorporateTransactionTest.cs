using Pi.Common.CommonModels;
using Pi.GlobalEquities.DomainModels;
using Pi.GlobalEquities.Models;
using Xunit;

namespace Pi.GlobalEquities.Tests.DomainModels;

public class CorporateTransaction_Test
{
    static TransactionItem NewTransactionItem(
        string id = null,
        string parId = null,
        string orderId = null,
        string venue = null,
        string symbol = null,
        string asset = null,
        decimal val = 0,
        OperationType optype = OperationType.Unknown,
        long timeStamp = 0,
        IList<TransactionItem> transactionItems = null)
    {
        var transaction = new TransactionItem
        {
            Id = id,
            ParentId = parId,
            OrderId = orderId,
            Venue = venue,
            Symbol = symbol,
            Asset = asset,
            Value = val,
            OperationType = optype,
            Timestamp = timeStamp,
            Children = transactionItems
        };
        return transaction;
    }

    public class Constructor_Test
    {
        [Fact]
        void WhenAssetTypeIsDefined_CorporateTransactionWillbeSetAsCashType()
        {
            var transaction = NewTransactionItem(asset: "EUR", optype: OperationType.Commission);

            var corporateTransaction = new CorporateTransaction(transaction);

            Assert.Equal(CorporateAssetType.Cash, corporateTransaction.AssetType);
            Assert.Equal(Currency.EUR, corporateTransaction.Currency);
        }

        [Fact]
        void WhenAssetTypeIsNotDefinedAsCurrencyWithNoSymbol_CorporateTransactionWillbeSetAsUnknown()
        {
            var transaction = NewTransactionItem(asset: "XXX", optype: OperationType.Commission);

            var corporateTransaction = new CorporateTransaction(transaction);

            Assert.Equal(CorporateAssetType.Unknown, corporateTransaction.AssetType);
        }

        [Fact]
        void WhenAssetTypeIsNotDefinedAsCurrencyWithSymbol_CorporateTransactionWillbeSetAsInstrument()
        {
            var transaction = NewTransactionItem(symbol: "DDOG", venue: "NASDAQ", asset: "XXX", optype: OperationType.Commission);

            var corporateTransaction = new CorporateTransaction(transaction);

            Assert.Equal(CorporateAssetType.Instrument, corporateTransaction.AssetType);
        }
    }

    public class GetValue_Test
    {
        [Fact]
        void WhenCorporateAssetTypeIsInstrumentAndCurrencyIsNull_ReturnTransactionValue()
        {
            var transaction = NewTransactionItem(venue: "NASDAQ", symbol: "DDOG", asset: "XXX", val: 10.01m, optype: OperationType.Commission);
            var corporateTransaction = new CorporateTransaction(transaction);

            var result = corporateTransaction.GetValue();

            Assert.Equal(10.01m, result);
        }

        [Fact]
        void WhenCorporateAssetTypeIsCashAndCurrencyIsNull_ReturnTransactionValue()
        {
            var transaction = NewTransactionItem(asset: "EUR", val: 10.01m, optype: OperationType.Commission);
            var corporateTransaction = new CorporateTransaction(transaction);

            var result = corporateTransaction.GetValue();

            Assert.Equal(10.01m, result);
        }

        [Fact]
        void WhenCorporateAssetTypeIsCashAndCurrencyIsNotNull_ReturnTransactionValue()
        {
            var transaction = NewTransactionItem(asset: "USD", val: 10.01m, optype: OperationType.Commission);
            var corporateTransaction = new CorporateTransaction(transaction);

            var result = corporateTransaction.GetValue(Currency.USD);

            Assert.Equal(10.01m, result);
        }

        [Fact]
        void WhenCorporateAssetTypeIsCashWithNoChild_ReturnNull()
        {
            var transaction = NewTransactionItem("123", "000", "999", "NASDAQ", "DDOG", "USD", 10.01m,
                OperationType.Commission, 123456, null);
            var corporateTransaction = new CorporateTransaction(transaction);

            var result = corporateTransaction.GetValue(Currency.EUR);

            Assert.Null(result);
        }

        [Fact]
        void WhenCorporateAssetTypeIsCashWithAutoConversionChild_ReturnChildsTransactionValue()
        {
            var childTransactions = new List<TransactionItem>
            {
                NewTransactionItem("123", "000", "999", "NASDAQ", "DDOG", "EUR", 2003m,
                    OperationType.AutoConversion, 123456, null)
            };
            var transaction = NewTransactionItem("123", "000", "999", "NASDAQ", "DDOG", "USD", 10.01m,
                OperationType.Commission, 123456, childTransactions);
            var corporateTransaction = new CorporateTransaction(transaction);

            var result = corporateTransaction.GetValue(Currency.EUR);

            Assert.Equal(2003m, result);
        }

        [Fact]
        void WhenCorporateAssetTypeIsCashWithChilds_ReturnInnerChildsTransactionValue()
        {
            var innerChildTransactions = new List<TransactionItem>
            {
                NewTransactionItem("123", "000", "999", "NASDAQ", "DDOG", "EUR", 9999m,
                    OperationType.AutoConversion, 123456, null)
            };
            var childTransactions = new List<TransactionItem>
            {
                NewTransactionItem("123", "000", "999", "NASDAQ", "DDOG", "EUR", 2003m,
                    OperationType.Commission, 123456, innerChildTransactions)
            };
            var transaction = NewTransactionItem("123", "000", "999", "NASDAQ", "DDOG", "USD", 10.01m,
                OperationType.Trade, 123456, childTransactions);
            var corporateTransaction = new CorporateTransaction(transaction);

            var result = corporateTransaction.GetValue(Currency.EUR);

            Assert.Equal(9999m, result);
        }

        [Fact]
        void WhenCorporateAssetTypeIsCashWithChilds_ReturnNull()
        {
            var innerChildTransactions = new List<TransactionItem>
            {
                NewTransactionItem("123", "000", "999", "NASDAQ", "DDOG", "EUR", 9999m,
                    OperationType.Tax, 123456, null)
            };
            var childTransactions = new List<TransactionItem>
            {
                NewTransactionItem("123", "000", "999", "NASDAQ", "DDOG", "EUR", 2003m,
                    OperationType.Commission, 123456, innerChildTransactions)
            };
            var transaction = NewTransactionItem("123", "000", "999", "NASDAQ", "DDOG", "USD", 10.01m,
                OperationType.Trade, 123456, childTransactions);
            var corporateTransaction = new CorporateTransaction(transaction);

            var result = corporateTransaction.GetValue(Currency.EUR);

            Assert.Null(result);
        }
    }
}
