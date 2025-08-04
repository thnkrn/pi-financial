using FluentAssertions;
using Pi.Common.CommonModels;
using Pi.GlobalEquities.DomainModels;
using Pi.GlobalEquities.Models;
using Xunit;

namespace Pi.GlobalEquities.Tests.DomainModels;

public class OrderTransactionTest
{
    static TransactionItem NewTransactionItem
    (
        string id,
        string parentId,
        string orderId,
        string venue,
        string symbol,
        string asset,
        decimal value,
        OperationType operationType,
        long timestamp,
        IList<TransactionItem> transactionItems)
    {
        var transactionItem = new TransactionItem
        {
            Id = id,
            ParentId = parentId,
            OrderId = orderId,
            Venue = venue,
            Symbol = symbol,
            Asset = asset,
            Value = value,
            OperationType = operationType,
            Timestamp = timestamp,
            Children = transactionItems
        };
        return transactionItem;
    }


    public class BuildTransaction_Test
    {
        [Fact]
        void WhenThereIsNoTransaction_ReturnEmptyList()
        {
            var orderId = "91c72754-536d-4343-84d2-43d9e5013f22";
            var transactionItems = Enumerable.Empty<TransactionItem>();

            var transaction = new OrderTransaction(orderId, transactionItems, Currency.USD);

            transaction.Transactions.Should().BeEquivalentTo(Enumerable.Empty<TransactionItem>());
        }

        [Fact]
        void WhenTransactionIsNull_ReturnEmptyList()
        {
            var orderId = "91c72754-536d-4343-84d2-43d9e5013f22";

            var transaction = new OrderTransaction(orderId, null, Currency.USD);

            transaction.Transactions.Should().BeEquivalentTo(Enumerable.Empty<TransactionItem>());
        }

        [Fact]
        void WhenBuildTransaction_ReturnTransactionItem()
        {
            var orderId = "920c8e25-33b9-4bcc-b333-434830ab2baa";
            var parentUUid = "12ceb493-eb81-4bc6-8843-9a573145497b";

            var trn1 = NewTransactionItem("7b29506a-e046-4443-805e-2239157fa5fd", parentUUid, orderId, "HKEX",
                "1810", "1810.HKEX", 200m, OperationType.Trade, 1714714936990, new List<TransactionItem>());
            var trn2 = NewTransactionItem(parentUUid, null, orderId, "HKEX",
                "1810", "HKD", -3624m, OperationType.Trade, 1714714936990, new List<TransactionItem>());
            var trn3 = NewTransactionItem("15757a0c-ca7c-4344-9d0c-2384ec34d5ce", parentUUid, orderId, null,
                null, "HKD", 3624m, OperationType.AutoConversion, 1714714936990, new List<TransactionItem>());
            var trn4 = NewTransactionItem("18971777-096f-4b0a-96a0-2a4c3bbfcaac", parentUUid, orderId, "HKEX",
                "1810", "USD", -463.86m, OperationType.AutoConversion, 1714714936990, new List<TransactionItem>());

            var transactions = new List<TransactionItem> { trn1, trn2, trn3, trn4 };
            var result = new OrderTransaction(orderId, transactions, Currency.USD);

            trn2.Children.Add(trn1);
            trn2.Children.Add(trn1);
            trn2.Children.Add(trn1);
            var expectedValue = new List<TransactionItem> { trn2 };
            result.Transactions.Should().BeEquivalentTo(expectedValue);
        }
    }

    public class GetTradeCost_Test
    {
        [Fact]
        void WhenOrderCurrencyEqualsTargetCurrency_ReturnCost()
        {
            var orderId = "040298bf-c88f-4703-937a-7f85dd0bdb3b";
            var trn1 = NewTransactionItem("abd4de29-0db5-4e57-944d-48f40b2bc632", null, orderId, "NASDAQ",
                "DDOG", "DDOG.NASDAQ", 0.01m, OperationType.Trade, 1711719001552, new List<TransactionItem>());
            var trn2 = NewTransactionItem("72a5ac50-677d-4915-a1a6-f69eb17974f1", null, orderId, "NASDAQ",
                "DDOG", "USD", -1.25m, OperationType.Trade, 1711719001552, new List<TransactionItem>());
            var trn3 = NewTransactionItem("323fa7c9-f04d-4d44-9c31-58cc9bc872bd", null, orderId, "NASDAQ",
                "DDOG", "USD", -0.01m, OperationType.Commission, 1711719001552, new List<TransactionItem>());

            var transactions = new List<TransactionItem> { trn1, trn2, trn3 };
            var orderTran = new OrderTransaction(orderId, transactions, Currency.USD);

            var result = orderTran.GetTradeCost(Currency.USD);

            Assert.Equal(-1.25m, result);
        }

        [Fact]
        void WhenOrderCurrencyNotEqualTargetCurrency_ReturnCost()
        {
            var orderId = "920c8e25-33b9-4bcc-b333-434830ab2baa";
            var parentUuid = "6de247e2-7941-4406-af0f-48d487299d06";

            var trn1 = NewTransactionItem(parentUuid, null, orderId, "HKEX",
                "1810", "HKD", -12.78m, OperationType.Commission, 1714714936990, new List<TransactionItem>());
            var trn2 = NewTransactionItem("ed149bed-d9ca-4979-968a-f6aacdc6767f", parentUuid, orderId, null,
                null, "HKD", 12.78m, OperationType.AutoConversion, 1714714936990, new List<TransactionItem>());
            var trn3 = NewTransactionItem("b733a17f-fa09-4b0e-ad64-54a78286f915", parentUuid, orderId, null,
                null, "USD", -1.64m, OperationType.AutoConversion, 1714714936990, new List<TransactionItem>());

            var transactions = new List<TransactionItem> { trn1, trn2, trn3 };
            var orderTran = new OrderTransaction(orderId, transactions, Currency.HKD);

            var result = orderTran.GetCommission(Currency.USD);

            Assert.Equal(-1.64m, result);
        }

        [Fact]
        void WhenOrderCurrencyNotEqualTargetCurrencyAndHaveMoreThan2Child_ReturnCost()
        {
            var orderId = "920c8e25-33b9-4bcc-b333-434830ab2baa";
            var parentUuid = "6de247e2-7941-4406-af0f-48d487299d06";

            var trn1 = NewTransactionItem(parentUuid, null, orderId, "HKEX",
                "1810", "HKD", -12.78m, OperationType.Commission, 1714714936990, new List<TransactionItem>());
            var trn2 = NewTransactionItem("ed149bed-d9ca-4979-968a-f6aacdc6767f", parentUuid, orderId, null,
                null, "HKD", 12.78m, OperationType.AutoConversion, 1714714936990, new List<TransactionItem>());
            var trn3 = NewTransactionItem("b733a17f-fa09-4b0e-ad64-54a78286f915", parentUuid, orderId, null,
                null, "USD", -1.64m, OperationType.AutoConversion, 1714714936990, new List<TransactionItem>());
            var trn4 = NewTransactionItem("91c72754-536d-4343-84d2-43d9e5013f22", "b733a17f-fa09-4b0e-ad64-54a78286f915", orderId, null,
                null, "EUR", -1.57m, OperationType.AutoConversion, 1714714936990, new List<TransactionItem>());

            var transactions = new List<TransactionItem> { trn1, trn2, trn3, trn4 };
            var orderTran = new OrderTransaction(orderId, transactions, Currency.HKD);

            var result = orderTran.GetCommission(Currency.EUR);

            Assert.Equal(-1.57m, result);
        }
    }

    public class GetTotalCost_Test
    {
        [Fact]
        void WhenGetCostOfTradeTransactions_ReturnTotalCost()
        {
            var orderId = "040298bf-c88f-4703-937a-7f85dd0bdb3b";
            var trn1 = NewTransactionItem("abd4de29-0db5-4e57-944d-48f40b2bc632", null, orderId, "NASDAQ",
                "DDOG", "DDOG.NASDAQ", 0.01m, OperationType.Trade, 1711719001552, new List<TransactionItem>());
            var trn2 = NewTransactionItem("72a5ac50-677d-4915-a1a6-f69eb17974f1", null, orderId, "NASDAQ",
                "DDOG", "USD", -1.25m, OperationType.Trade, 1711719001552, new List<TransactionItem>());
            var trn3 = NewTransactionItem("323fa7c9-f04d-4d44-9c31-58cc9bc872bd", null, orderId, "NASDAQ",
                "DDOG", "USD", -0.01m, OperationType.Commission, 1711719001552, new List<TransactionItem>());

            var transactions = new List<TransactionItem> { trn1, trn2, trn3 };
            var orderTran = new OrderTransaction(orderId, transactions, Currency.USD);

            var result = orderTran.GetTotalCost(Currency.USD);

            Assert.Equal(-1.26m, result);
        }
    }
}

