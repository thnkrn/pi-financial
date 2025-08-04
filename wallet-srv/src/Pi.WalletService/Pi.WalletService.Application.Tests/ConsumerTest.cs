using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
namespace Pi.WalletService.Application.Tests;

public class ConsumerTest : IAsyncLifetime
{
    protected ITestHarness Harness { get; private set; } = null!;
    protected ServiceProvider Provider { get; init; } = null!;

    public async Task InitializeAsync()
    {
        Harness = Provider.GetRequiredService<ITestHarness>();
        await Harness.Start();
    }

    public async Task DisposeAsync()
    {
        await Harness.Stop();
        await Provider.DisposeAsync();
    }
}