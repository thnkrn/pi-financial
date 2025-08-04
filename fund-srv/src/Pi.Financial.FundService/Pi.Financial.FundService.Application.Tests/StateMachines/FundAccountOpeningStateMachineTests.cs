using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using Pi.Financial.FundService.Application.Commands;
using Pi.Financial.FundService.Application.StateMachines;
using Pi.Financial.FundService.Domain.AggregatesModel.AccountOpeningAggregate;
using Pi.Financial.FundService.Domain.Events;
using Pi.Financial.FundService.IntegrationEvents;

namespace Pi.Financial.FundService.Application.Tests.StateMachines
{
    public class FundAccountOpeningStateMachineTests : IAsyncLifetime
    {
        private ITestHarness _harness = null!;
        private ServiceProvider? _provider;
        private const string CustomerCode = "cust-code";
        private const string AccountCode = "acc-code";
        private readonly Guid _ticketIdAllSuccess = Guid.Parse("8f8df8d3-c99d-4a02-a92e-4300e3c62d2f");
        private readonly Guid _ticketIdFailedCreateFundCustomer = Guid.Parse("0db3dd84-195f-4105-bf37-e3adfea861f2");
        private readonly Guid _ticketIdFailedCreateFundAccount = Guid.Parse("e3d1bc64-dd31-4cef-b330-ddd5402688da");
        private readonly Guid _userId = Guid.NewGuid();

        public async Task InitializeAsync()
        {
            _provider = new ServiceCollection()
                .AddMassTransitTestHarness(cfg =>
                {
                    cfg.AddHandler<CreateFundCustomer>(ctx =>
                    {
                        if (ctx.Message.TicketId == _ticketIdFailedCreateFundCustomer)
                        {
                            throw new Exception("create fund customer failed");
                        }

                        return ctx.RespondAsync(new FundCustomerCreated(CustomerCode, true, _userId));
                    });
                    cfg.AddHandler((ConsumeContext<CreateFundAccount> ctx) =>
                    {
                        if (ctx.Message.TicketId == _ticketIdFailedCreateFundAccount)
                        {
                            throw new Exception("some error");
                        }

                        return ctx.RespondAsync(new FundAccountCreated(ctx.Message.CustomerCode, AccountCode, true));
                    });
                    cfg.AddSagaStateMachine<FundAccountOpeningStateMachine, FundAccountOpeningState>();
                })
                .BuildServiceProvider(true);

            _harness = _provider.GetRequiredService<ITestHarness>();

            await _harness.Start();
        }


        public async Task DisposeAsync()
        {
            await _harness.Stop();
            if (_provider != null)
            {
                await _provider.DisposeAsync();
            }
        }

        [Fact(Skip = "Failed on CI")]
        public async Task WhenAccountOpeningRequestReceived_WithoutNdid_StateChangeToFinalWithoutGenerateDocs()
        {
            // arrange
            var ticketId = _ticketIdAllSuccess;

            // act
            await _harness.Bus.Publish(new AccountOpeningRequestReceived(_ticketIdAllSuccess, CustomerCode, false, string.Empty, null, string.Empty));

            // assert
            var sagaHarness =
                _harness.GetSagaStateMachineHarness<FundAccountOpeningStateMachine, FundAccountOpeningState>();
            Assert.True(await sagaHarness.Created.Any(x => x.CorrelationId == ticketId));
            var instanceId = await sagaHarness.Exists(ticketId, x => x.Received);
            Assert.NotNull(instanceId);
            var instance = sagaHarness.Sagas.Contains(instanceId.Value);
            Assert.Equal(CustomerCode, instance.CustomerCode);
            instanceId = await sagaHarness.Exists(ticketId, x => x.Final);
            Assert.NotNull(instanceId);
            Assert.True(await _harness.Published.Any(m =>
            {
                var message = m.MessageObject as FundAccountOpened;

                return message != null && message.TicketId == ticketId && message is { CustomerCode: CustomerCode, AccountCode: AccountCode };
            }));
        }

        [Fact(Skip = "Failed on CI")]
        public async Task WhenAccountOpeningRequestReceived_ButCreateFundCustomerFailed_StateChangeToFailed()
        {
            // arrange
            var ticketId = _ticketIdFailedCreateFundCustomer;

            // act
            await _harness.Bus.Publish(new AccountOpeningRequestReceived(ticketId, CustomerCode, false, string.Empty, null, string.Empty));

            // assert
            var sagaHarness =
                _harness.GetSagaStateMachineHarness<FundAccountOpeningStateMachine, FundAccountOpeningState>();
            Assert.True(await sagaHarness.Created.Any(x => x.CorrelationId == ticketId));
            var instanceId = await sagaHarness.Exists(ticketId, x => x.Received);
            Assert.NotNull(instanceId);
            var instance = sagaHarness.Sagas.Contains(instanceId.Value);
            Assert.Equal(CustomerCode, instance.CustomerCode);
            instanceId = await sagaHarness.Exists(ticketId, x => x.CustomerCreateFailed);
            Assert.NotNull(instanceId);
            Assert.True(await _harness.Published.Any(m =>
            {
                var message = m.MessageObject as FundAccountOpeningFailed;

                return message != null && message.TicketId == ticketId && message is { CustomerCode: CustomerCode, ErrorMessage: "create fund customer failed" };
            }));
        }
    }
}
