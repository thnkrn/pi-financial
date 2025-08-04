using Pi.BackofficeService.Domain.AggregateModels.TransactionAggregate;
using Pi.Client.WalletService.Model;

namespace Pi.BackofficeService.Infrastructure.Factories;

public static class ClientFactory
{
    public static PiWalletServiceIntegrationEventsAggregatesModelProduct? NewProduct(Product? product)
    {
        return product switch
        {
            Product.Funds => PiWalletServiceIntegrationEventsAggregatesModelProduct.Funds,
            Product.TFEX => PiWalletServiceIntegrationEventsAggregatesModelProduct.Derivatives,
            Product.Cash => PiWalletServiceIntegrationEventsAggregatesModelProduct.Cash,
            Product.CashBalance => PiWalletServiceIntegrationEventsAggregatesModelProduct.CashBalance,
            Product.Crypto => PiWalletServiceIntegrationEventsAggregatesModelProduct.Crypto,
            Product.Margin => PiWalletServiceIntegrationEventsAggregatesModelProduct.CreditBalanceSbl,
            Product.GlobalEquity => PiWalletServiceIntegrationEventsAggregatesModelProduct.GlobalEquities,
            _ => null
        };
    }

    public static PiWalletServiceIntegrationEventsAggregatesModelChannel NewChannelV2(Channel? channel)
    {
        return channel switch
        {
            Channel.SetTrade => PiWalletServiceIntegrationEventsAggregatesModelChannel.SetTrade,
            Channel.QR => PiWalletServiceIntegrationEventsAggregatesModelChannel.QR,
            Channel.AtsBatch => PiWalletServiceIntegrationEventsAggregatesModelChannel.ATS,
            Channel.OnlineTransfer => PiWalletServiceIntegrationEventsAggregatesModelChannel.OnlineViaKKP,
            Channel.ODD => PiWalletServiceIntegrationEventsAggregatesModelChannel.ODD,
            Channel.BillPayment => PiWalletServiceIntegrationEventsAggregatesModelChannel.BillPayment,
            _ => throw new ArgumentOutOfRangeException(nameof(channel))
        };
    }
}
