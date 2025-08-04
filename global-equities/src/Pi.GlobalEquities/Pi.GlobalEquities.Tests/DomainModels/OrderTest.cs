using FluentAssertions;
using Pi.Common.CommonModels;
using Pi.GlobalEquities.DomainModels;
using Pi.GlobalEquities.Models;
using Xunit;

namespace Pi.GlobalEquities.Tests.DomainModels;

public class OrderTest
{
    public static Order NewOrder(
        string id = null,
        string userId = null,
        string accountId = null,
        string venue = null,
        string symbol = null,
        OrderType type = OrderType.Unknown,
        OrderSide side = OrderSide.Buy,
        OrderDuration duration = OrderDuration.Day,
        decimal quantity = 0,
        decimal? limitPrice = null,
        decimal? stopPrice = null,
        IEnumerable<OrderFill> fills = null,
        OrderStatus status = OrderStatus.Unknown,
        ProviderInfo provInfo = null,
        DateTime createAt = new(),
        DateTime updateAt = new(),
        Channel chan = Channel.Unknown)
    {
        var order = new Order
        {
            Id = id,
            UserId = userId,
            AccountId = accountId,
            Venue = venue,
            Symbol = symbol,
            OrderType = type,
            Side = side,
            Duration = duration,
            Quantity = quantity,
            LimitPrice = limitPrice,
            StopPrice = stopPrice,
            Fills = fills,
            Status = status,
            ProviderInfo = provInfo,
            CreatedAt = createAt,
            UpdatedAt = updateAt,
            Channel = chan
        };
        return order;
    }

    static IOrderUpdates NewOrderUpdates(
        string id = "5a23aea0-d2fe-4d2d-a07a-8e6d01ac6497",
        decimal quantity = 0.99m,
        decimal? limitPrice = 121m,
        decimal? stopPrice = 151m,
        OrderStatus status = OrderStatus.Processing,
        IEnumerable<OrderFill> orderFill = null,
        ProviderInfo providerInfo = null
    )
    {
        var orderUpdate = new OrderUpdates
        {
            ProviderId = id,
            Quantity = quantity,
            LimitPrice = limitPrice,
            StopPrice = stopPrice,
            Status = status,
            Fills = orderFill,
            ProviderInfo = providerInfo
        };
        return orderUpdate;
    }

    static ProviderInfo NewProviderInfo(
        Provider provider = Provider.Velexa,
        string accountId = "91c72754-536d-4343-84d2-43d9e5013f22",
        string orderId = "5a23aea0-d2fe-4d2d-a07a-8e6d01ac6497",
        string modId = "e09934e1-04d9-4fc0-9d82-41d81323a5aa",
        string status = "working",
        string reason = "",
        DateTime createdAt = new(),
        DateTime updatedAt = new()
        )
    {
        var providerInfo = new ProviderInfo
        {
            ProviderName = provider,
            AccountId = accountId,
            OrderId = orderId,
            ModificationId = modId,
            Status = status,
            StatusReason = reason,
            CreatedAt = createdAt,
            ModifiedAt = updatedAt
        };
        return providerInfo;
    }

    public class Id_Test
    {
        [Fact]
        void WhenExistingIdIsNotNullOrEmpty_OrderIdWillBeSet()
        {
            var order = NewOrder(id: null);

            var id = "7375f85c-ca86-4101-a838-e7e7842779bc";
            order.Id = id;

            Assert.Equal(id, order.Id);
        }

        [Fact]
        void WhenExistingIdIsNotChanged_OrderIdWillNotBeChanged()
        {
            var order = NewOrder(id: "7375f85c-ca86-4101-a838-e7e7842779bc");

            var id = "7375f85c-ca86-4101-a838-e7e7842779bc";
            order.Id = id;

            Assert.Equal(id, order.Id);
        }

        [Fact]
        void WhenExistingIdIsChange_ThrowException()
        {
            var order = NewOrder(id: "5a23aea0-d2fe-4d2d-a07a-8e6d01ac6497");

            var action = () => order.Id = "7375f85c-ca86-4101-a838-e7e7842779bc";

            var exception = Assert.Throws<Exception>(action);
            Assert.Equal("Can not change order Id", exception.Message);
        }
    }

    public class UserId_Test
    {
        [Fact]
        void WhenExistingUserIdIsNotNullOrEmpty_UserIdWillBeSet()
        {
            var order = NewOrder(userId: null);

            var userId = "7375f85c-ca86-4101-a838-e7e7842779bc";
            order.UserId = userId;

            Assert.Equal(userId, order.UserId);
        }

        [Fact]
        void WhenExistingUserIdIsNotChanged_UserIdWillNotBeChanged()
        {
            var order = NewOrder(userId: "7375f85c-ca86-4101-a838-e7e7842779bc");

            var userId = "7375f85c-ca86-4101-a838-e7e7842779bc";
            order.UserId = userId;

            Assert.Equal(userId, order.UserId);
        }

        [Fact]
        void WhenExistingUserIdIsChange_ThrowException()
        {
            var order = NewOrder(userId: "5a23aea0-d2fe-4d2d-a07a-8e6d01ac6497");

            var action = () => order.UserId = "7375f85c-ca86-4101-a838-e7e7842779bc";

            var exception = Assert.Throws<Exception>(action);
            Assert.Equal("Order owner cannot be changed (UserId)", exception.Message);
        }
    }

    public class AccountId_Test
    {
        [Fact]
        void WhenExistingAccountIdIsNotNullOrEmpty_AccountIdWillBeSet()
        {
            var order = NewOrder(accountId: null);

            var accountId = "7375f85c-ca86-4101-a838-e7e7842779bc";
            order.AccountId = accountId;

            Assert.Equal(accountId, order.AccountId);
        }

        [Fact]
        void WhenExistingAccountIdIsNotChanged_AccountIdWillNotBeChanged()
        {
            var order = NewOrder(accountId: "7375f85c-ca86-4101-a838-e7e7842779bc");

            var accountId = "7375f85c-ca86-4101-a838-e7e7842779bc";
            order.AccountId = accountId;

            Assert.Equal(accountId, order.AccountId);
        }

        [Fact]
        void WhenExistingAccountIdIsChange_ThrowException()
        {
            var order = NewOrder(accountId: "5a23aea0-d2fe-4d2d-a07a-8e6d01ac6497");

            var action = () => order.AccountId = "7375f85c-ca86-4101-a838-e7e7842779bc";

            var exception = Assert.Throws<Exception>(action);
            Assert.Equal("Order owner cannot be changed (AccountId)", exception.Message);
        }
    }

    public class SymbolId_Test
    {
        [Fact]
        void WhenSymbolAndVenueIsSet_SymbolIdWIllBeSet()
        {
            var order = NewOrder(symbol: "DDOG", venue: "NASDAQ");

            Assert.Equal("DDOG.NASDAQ", order.SymbolId);
        }
    }

    public class CurrencyFromVenue_Test
    {
        [Fact]
        void WhenVenueIsHKEX_ReturnHKD()
        {
            var venue = "HKEX";

            var currency = Order.CurrencyFromVenue(venue);

            Assert.Equal(Currency.HKD, currency);
        }

        [Fact]
        void WhenVenueIsNotHKEX_ReturnUSD()
        {
            var venue = "USD";

            var currency = Order.CurrencyFromVenue(venue);

            Assert.Equal(Currency.USD, currency);
        }
    }

    public class QuantityCancelled_Test
    {
        [Theory]
        [InlineData(OrderStatus.PartiallyMatched)]
        [InlineData(OrderStatus.Cancelled)]
        [InlineData(OrderStatus.Rejected)]
        void WhenThereAreFillsBeforeOrderReachFinalStatus_QuantityCancelledWillBeSet(OrderStatus status)
        {
            var fills = new List<OrderFill> { new(DateTime.UtcNow, 0.02m, 121m) };

            var order = NewOrder(status: status, quantity: 0.09m, fills: fills);

            Assert.Equal(0.07m, order.QuantityCancelled);
        }

        [Fact]
        void WhenThereAreFillsWhileOrderIsOpened_QuantityCancelledWillBeZero()
        {
            var fills = new List<OrderFill> { new(DateTime.UtcNow, 0.02m, 121m) };

            var order = NewOrder(status: OrderStatus.Processing, quantity: 0.09m, fills: fills);

            Assert.Equal(0m, order.QuantityCancelled);
        }
    }

    public class SetOwner_Test
    {
        [Fact]
        void WhenSetOwner_UserIdAndAccountIdWillBeSet()
        {
            var order = NewOrder(userId: null, accountId: null);

            var userId = "cb25452b-3e99-4656-9b92-ce391bb3b551";
            var accountId = "91c72754-536d-4343-84d2-43d9e5013f22";
            order.SetOwner(userId, accountId);

            Assert.Equal(userId, order.UserId);
            Assert.Equal(accountId, order.AccountId);
        }
    }

    public class Update_IOrderUpdates_Test
    {
        [Fact]
        void WhenThereAreModifiedUpdateOrderWithIOrderUpdate_ReturnTrue()
        {
            var dateTimeNow = DateTime.UtcNow;
            var providerInfo = NewProviderInfo(provider: Provider.Velexa, orderId: "e09934e1-04d9-4fc0-9d82-41d81323a5aa", updatedAt: dateTimeNow);
            var order = NewOrder(id: "e09934e1-04d9-4fc0-9d82-41d81323a5aa", type: OrderType.Limit, quantity: 0.02m, limitPrice: 111m, stopPrice: null, status: OrderStatus.Processing, provInfo: providerInfo);
            var updatedProviderInfo = NewProviderInfo(provider: Provider.Velexa, orderId: "e09934e1-04d9-4fc0-9d82-41d81323a5aa", updatedAt: dateTimeNow);
            var orderUpdate = NewOrderUpdates(id: "7375f85c-ca86-4101-a838-e7e7842779bc", quantity: 0.01m,
                limitPrice: 121m, stopPrice: null, status: OrderStatus.Processing, providerInfo: updatedProviderInfo);

            var result = order.Update(orderUpdate, out var changes);

            Assert.True(result);
            var propertyChanges = new List<PropertyChange>
            {
                new PropertyChange<decimal?>(nameof(Order.LimitPrice), 111m, 121m),
                new PropertyChange<decimal>(nameof(Order.Quantity), 0.02m ,0.01m)
            };
            changes.Should().BeEquivalentTo(propertyChanges);
        }
    }

    public class Update_IOrderValues_Test
    {
        [Fact]
        void WhenValueIsNull_ReturnTrue()
        {
            var order = NewOrder(id: "e09934e1-04d9-4fc0-9d82-41d81323a5aa", type: OrderType.Limit, quantity: 0.02m, limitPrice: 111m, stopPrice: null, status: OrderStatus.Processing);

            var action = () => order.Update((IOrderValues)null, out var changes);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        void WhenLimitPriceIsChanged_ReturnTrue()
        {
            var order = NewOrder(id: "e09934e1-04d9-4fc0-9d82-41d81323a5aa", type: OrderType.Limit, quantity: 0.02m, limitPrice: 111m, stopPrice: null, status: OrderStatus.Processing);
            var providerInfo = NewProviderInfo(provider: Provider.Velexa, orderId: "e09934e1-04d9-4fc0-9d82-41d81323a5aa");
            var orderUpdate = NewOrderUpdates(id: "7375f85c-ca86-4101-a838-e7e7842779bc", quantity: 0.02m,
                limitPrice: 121m, stopPrice: null, status: OrderStatus.Processing, providerInfo: providerInfo);

            var result = order.Update((IOrderValues)orderUpdate, out var changes);

            Assert.True(result);
            var propertyChanges =
                new List<PropertyChange> { new PropertyChange<decimal?>(nameof(Order.LimitPrice), 111m, 121m) };
            changes.Should().BeEquivalentTo(propertyChanges);
        }

        [Fact]
        void WhenStopPriceIsChanged_ReturnTrue()
        {
            var order = NewOrder(id: "e09934e1-04d9-4fc0-9d82-41d81323a5aa", type: OrderType.StopLimit, quantity: 0.02m, limitPrice: 111m, stopPrice: 120m, status: OrderStatus.Processing);
            var providerInfo = NewProviderInfo(provider: Provider.Velexa, orderId: "e09934e1-04d9-4fc0-9d82-41d81323a5aa");
            var orderUpdate = NewOrderUpdates(id: "7375f85c-ca86-4101-a838-e7e7842779bc", quantity: 0.02m,
                limitPrice: 111m, stopPrice: 130m, status: OrderStatus.Processing, providerInfo: providerInfo);

            var result = order.Update((IOrderValues)orderUpdate, out var changes);

            Assert.True(result);
            var propertyChanges =
                new List<PropertyChange> { new PropertyChange<decimal?>(nameof(Order.StopPrice), 120m, 130m) };
            changes.Should().BeEquivalentTo(propertyChanges);
        }

        [Fact]
        void WhenQuantityIsChanged_ReturnTrue()
        {
            var order = NewOrder(id: "e09934e1-04d9-4fc0-9d82-41d81323a5aa", type: OrderType.Limit, quantity: 0.02m, limitPrice: 111m, stopPrice: null, status: OrderStatus.Processing);
            var providerInfo = NewProviderInfo(provider: Provider.Velexa, orderId: "e09934e1-04d9-4fc0-9d82-41d81323a5aa");
            var orderUpdate = NewOrderUpdates(id: "7375f85c-ca86-4101-a838-e7e7842779bc", quantity: 0.09m,
                limitPrice: 111m, stopPrice: null, status: OrderStatus.Processing, providerInfo: providerInfo);

            var result = order.Update((IOrderValues)orderUpdate, out var changes);

            Assert.True(result);
            var propertyChanges =
                new List<PropertyChange> { new PropertyChange<decimal>(nameof(Order.Quantity), 0.02m, 0.09m) };
            changes.Should().BeEquivalentTo(propertyChanges);
        }
    }

    public class Update_IOrderStatus_Test
    {
        [Fact]
        void WhenValueIsNull_ReturnTrue()
        {
            var order = NewOrder(id: "e09934e1-04d9-4fc0-9d82-41d81323a5aa", type: OrderType.Limit, quantity: 0.02m, limitPrice: 111m, stopPrice: null, status: OrderStatus.Processing);

            var action = () => order.Update((IOrderStatus)null, out var changes);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        void WhenIdIsNullOrWhiteSpace_IdWillBeSet()
        {
            var order = NewOrder(id: null, type: OrderType.Limit, quantity: 0.02m, limitPrice: 111m, stopPrice: null, status: OrderStatus.Processing);
            var modId = "e09934e1-04d9-4fc0-9d82-41d81323a5aa";
            var providerInfo = NewProviderInfo(provider: Provider.Velexa, orderId: modId);
            var orderUpdate = NewOrderUpdates(id: modId, quantity: 0.09m,
                limitPrice: 111m, stopPrice: null, status: OrderStatus.Processing, providerInfo: providerInfo);

            order.Update((IOrderStatus)orderUpdate, out var changes);

            Assert.Equal(modId, order.Id);
        }

        [Fact]
        void WhenModifiedAtIsChanged_ReturnTrue()
        {
            var dateTimeNow = DateTime.UtcNow;
            var providerInfo = NewProviderInfo(provider: Provider.Velexa, orderId: null, updatedAt: dateTimeNow.AddDays(-1));
            var order = NewOrder(id: null, type: OrderType.Limit, quantity: 0.02m, limitPrice: 111m, stopPrice: null, status: OrderStatus.Processing, provInfo: providerInfo);

            var modId = "e09934e1-04d9-4fc0-9d82-41d81323a5aa";
            var updatedProviderInfo = NewProviderInfo(provider: Provider.Velexa, orderId: modId, updatedAt: dateTimeNow);
            var orderUpdate = NewOrderUpdates(id: modId, quantity: 0.09m,
                limitPrice: 111m, stopPrice: null, status: OrderStatus.Processing, providerInfo: updatedProviderInfo);

            order.Update((IOrderStatus)orderUpdate, out var changes);

            Assert.Equal(modId, order.Id);
            var propertyChanges =
                new List<PropertyChange> { new PropertyChange<DateTime?>(nameof(Order.ProviderInfo.ModifiedAt), dateTimeNow.AddDays(-1), dateTimeNow) };
            changes.Should().BeEquivalentTo(propertyChanges);
        }

        [Fact]
        void WhenStatusIsChanged_ReturnTrue()
        {
            var dateTimeNow = DateTime.UtcNow;
            var providerInfo = NewProviderInfo(provider: Provider.Velexa, orderId: null, updatedAt: dateTimeNow);
            var id = "e09934e1-04d9-4fc0-9d82-41d81323a5aa";
            var order = NewOrder(id: id, type: OrderType.Limit, quantity: 0.02m, limitPrice: 111m, stopPrice: null, status: OrderStatus.Processing, provInfo: providerInfo);

            var updatedProviderInfo = NewProviderInfo(provider: Provider.Velexa, orderId: id, updatedAt: dateTimeNow);
            var orderUpdate = NewOrderUpdates(id: id, quantity: 0.02m,
                limitPrice: 111m, stopPrice: null, status: OrderStatus.Matched, providerInfo: updatedProviderInfo);

            order.Update((IOrderStatus)orderUpdate, out var changes);

            Assert.Equal(OrderStatus.Matched, order.Status);
            var propertyChanges =
                new List<PropertyChange> { new PropertyChange<OrderStatus>(nameof(Order.Status), OrderStatus.Processing, OrderStatus.Matched) };
            changes.Should().BeEquivalentTo(propertyChanges);
        }

        [Fact]
        void WhenFilledListIsUpdated_ReturnTrue()
        {
            var dateTimeNow = DateTime.UtcNow;
            var providerInfo = NewProviderInfo(provider: Provider.Velexa, orderId: null, updatedAt: dateTimeNow);
            var id = "e09934e1-04d9-4fc0-9d82-41d81323a5aa";
            var order = NewOrder(id: id, type: OrderType.Limit, quantity: 0.02m, limitPrice: 111m, stopPrice: null, status: OrderStatus.Processing, provInfo: providerInfo);

            var updatedProviderInfo = NewProviderInfo(provider: Provider.Velexa, orderId: id, updatedAt: dateTimeNow);
            var fills = new List<OrderFill> { new(dateTimeNow, 0.01m, 111m) }; // partially matched
            var orderUpdate = NewOrderUpdates(id: id, quantity: 0.02m,
                limitPrice: 111m, stopPrice: null, status: OrderStatus.Processing, providerInfo: updatedProviderInfo, orderFill: fills);

            order.Update((IOrderStatus)orderUpdate, out var changes);

            order.Fills.Should().BeEquivalentTo(fills);
            var propertyChanges =
                new List<PropertyChange> { new PropertyChange<IEnumerable<OrderFill>>(nameof(Order.Fills), Enumerable.Empty<OrderFill>(), orderUpdate.Fills) };
            changes.Should().BeEquivalentTo(propertyChanges);
        }
    }

    public class UpdateFills_Test
    {
        [Fact]
        void WhenPreviousFillsAreNullButValueIsNotNull_QuantityTotalPriceAvgPriceFillsWillBeUpdated()
        {
            var dateTimeNow = DateTime.UtcNow;
            var fill1 = new OrderFill(dateTimeNow, 0.01m, 110.5m);
            var fill2 = new OrderFill(dateTimeNow, 0.01m, 111.5m);
            var providerInfo = NewProviderInfo(provider: Provider.Velexa, orderId: null, updatedAt: dateTimeNow);
            var id = "e09934e1-04d9-4fc0-9d82-41d81323a5aa";
            var order = NewOrder(id: id, type: OrderType.Limit, quantity: 0.02m, limitPrice: 111m, stopPrice: null, status: OrderStatus.Processing, provInfo: providerInfo);

            var updatedProviderInfo = NewProviderInfo(provider: Provider.Velexa, orderId: id, updatedAt: dateTimeNow);
            var fills = new List<OrderFill> { fill1, fill2 };
            var orderUpdate = NewOrderUpdates(id: id, quantity: 0.02m,
                limitPrice: 111m, stopPrice: null, status: OrderStatus.Processing, providerInfo: updatedProviderInfo, orderFill: fills);

            order.Update((IOrderStatus)orderUpdate, out var changes);

            order.Fills.Should().BeEquivalentTo(fills);
            var propertyChanges =
                new List<PropertyChange> { new PropertyChange<IEnumerable<OrderFill>>(nameof(Order.Fills), Enumerable.Empty<OrderFill>(), fills) };
            changes.Should().BeEquivalentTo(propertyChanges);

            Assert.Equal(0.02m, order.QuantityFilled);
            var totalPrice = (fill1.Price * fill1.Quantity) + (fill2.Price * fill2.Quantity);
            Assert.Equal(totalPrice, order.TotalFillPrice);
            Assert.Equal(111m, order.AverageFillPrice);
        }

        [Fact]
        void WhenFillAreNullAndValueIsNull_QuantityTotalPriceAvgPriceFillsWillNotBeUpdated()
        {
            var dateTimeNow = DateTime.UtcNow;
            var providerInfo = NewProviderInfo(provider: Provider.Velexa, orderId: null, updatedAt: dateTimeNow);
            var id = "e09934e1-04d9-4fc0-9d82-41d81323a5aa";
            var order = NewOrder(id: id, type: OrderType.Limit, quantity: 0.02m, limitPrice: 111m, stopPrice: null, status: OrderStatus.Processing, provInfo: providerInfo);

            var updatedProviderInfo = NewProviderInfo(provider: Provider.Velexa, orderId: id, updatedAt: dateTimeNow);
            var orderUpdate = NewOrderUpdates(id: id, quantity: 0.02m,
                limitPrice: 111m, stopPrice: null, status: OrderStatus.Processing, providerInfo: updatedProviderInfo, orderFill: null);

            order.Update((IOrderStatus)orderUpdate, out var changes);

            order.Fills.Should().BeEquivalentTo(Enumerable.Empty<OrderFill>());
            Assert.Empty(changes);

            Assert.Equal(0m, order.QuantityFilled);
            Assert.Equal(0m, order.TotalFillPrice);
            Assert.Null(order.AverageFillPrice);
        }

        [Fact]
        void WhenFillExistsAndValueIsNotNull_QuantityTotalPriceAvgPriceFillsWillBeUpdated()
        {
            var dateTimeNow = DateTime.UtcNow;
            var fill1 = new OrderFill(dateTimeNow, 0.01m, 110.5m);
            var fill2 = new OrderFill(dateTimeNow, 0.01m, 111.5m);
            var providerInfo = NewProviderInfo(provider: Provider.Velexa, orderId: null, updatedAt: dateTimeNow);
            var prevFills = new List<OrderFill> { fill1 };
            var id = "e09934e1-04d9-4fc0-9d82-41d81323a5aa";
            var order = NewOrder(id: id, type: OrderType.Limit, quantity: 0.02m, limitPrice: 111m, stopPrice: null, status: OrderStatus.Processing, provInfo: providerInfo, fills: prevFills);

            var updatedProviderInfo = NewProviderInfo(provider: Provider.Velexa, orderId: id, updatedAt: dateTimeNow);
            var fills = new List<OrderFill> { fill1, fill2 };
            var orderUpdate = NewOrderUpdates(id: id, quantity: 0.02m,
                limitPrice: 111m, stopPrice: null, status: OrderStatus.Processing, providerInfo: updatedProviderInfo, orderFill: fills);

            order.Update((IOrderStatus)orderUpdate, out var changes);

            order.Fills.Should().BeEquivalentTo(fills);
            var propertyChanges =
                new List<PropertyChange> { new PropertyChange<IEnumerable<OrderFill>>(nameof(Order.Fills), prevFills, fills) };
            changes.Should().BeEquivalentTo(propertyChanges);

            Assert.Equal(0.02m, order.QuantityFilled);
            var totalPrice = (fill1.Price * fill1.Quantity) + (fill2.Price * fill2.Quantity);
            Assert.Equal(totalPrice, order.TotalFillPrice);
            Assert.Equal(111m, order.AverageFillPrice);
        }
    }
}
