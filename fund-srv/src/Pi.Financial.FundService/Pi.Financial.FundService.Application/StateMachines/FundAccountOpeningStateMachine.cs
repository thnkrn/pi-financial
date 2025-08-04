using MassTransit;
using Microsoft.Extensions.Logging;
using Pi.Financial.FundService.Application.Commands;
using Pi.Financial.FundService.Application.Models.Enums;
using Pi.Financial.FundService.Domain.AggregatesModel.AccountOpeningAggregate;
using Pi.Financial.FundService.Domain.Events;
using Pi.Financial.FundService.IntegrationEvents;

namespace Pi.Financial.FundService.Application.StateMachines
{
    public class FundAccountOpeningStateMachine : MassTransitStateMachine<FundAccountOpeningState>
    {
        public FundAccountOpeningStateMachine(ILogger<FundAccountOpeningStateMachine> logger)
        {
            InstanceState(x => x.CurrentState);
            Event(() => AccountOpeningRequestReceived, x => x.CorrelateById(ctx => ctx.Message.TicketId));
            Request(() => CreateFundCustomerRequest, x => { x.Timeout = TimeSpan.Zero; });
            Request(() => CreateFundAccountRequest, x => { x.Timeout = TimeSpan.Zero; });
            Request(() => GenerateFundAccountOpeningDocumentsRequest, x => { x.Timeout = TimeSpan.Zero; });
            Request(() => UploadFundAccountOpeningDocumentsRequest, x => { x.Timeout = TimeSpan.Zero; });

            Initially(
                When(AccountOpeningRequestReceived)
                    .Then(context =>
                    {
                        context.Saga.RequestReceivedTime = context.SentTime ?? DateTime.UtcNow;
                        context.Saga.CustomerCode = context.Message.CustomerCode;
                        context.Saga.Ndid = context.Message.Ndid;
                        context.Saga.NdidRequestId = context.Message.NdidRequestId;
                        context.Saga.NdidDateTime = context.Message.NdidDateTime;
                        context.Saga.OpenAccountRegisterUid = context.Message.OpenAccountRegisterUid;
                    })
                    .TransitionTo(Received)
                    .Request(CreateFundCustomerRequest,
                        context => new CreateFundCustomer(context.Saga.CorrelationId, context.Message.CustomerCode, context.Message.Ndid, context.Message.NdidRequestId, context.Saga.NdidDateTime))
            );

            During(Received,
                When(CreateFundCustomerRequest!.Completed)
                    .Then(context =>
                    {
                        logger.LogInformation("create fund customer request completed");
                        context.Saga.UserId = context.Message.UserId ?? Guid.Empty;
                    })
                    .TransitionTo(CustomerCreated)
                    .Request(CreateFundAccountRequest,
                        ctx => new CreateFundAccount(ctx.Saga.CorrelationId, ctx.Saga.UserId, ctx.Message.CustomerCode, ctx.Message.Ndid, ctx.Saga.NdidRequestId)),
                When(CreateFundCustomerRequest.Faulted)
                    .Then(ctx =>
                    {
                        ctx.Saga.FailedReason = string.Join(", ", ctx.Message.Exceptions.Select(e => e.Message));
                        logger.LogError("create fund customer request failed @{Exceptions}", ctx.Message.Exceptions);
                    })
                    .Publish(ctx => new FundAccountOpeningFailed
                    {
                        TicketId = ctx.Saga.CorrelationId,
                        CustomerCode = ctx.Message.Message.CustomerCode,
                        ErrorMessage = ctx.Message.Exceptions.FirstOrDefault()?.Message
                    })
                    .Send(context => SendOpenSuccessCallback(context.Saga, OpenFundAccountStatus.FAILED))
                    .TransitionTo(CustomerCreateFailed)
            );

            During(CustomerCreated,
                When(CreateFundAccountRequest!.Completed, ctx => ctx.Saga.Ndid)
                    .Then(ctx => { logger.LogInformation("Fund account created: {TicketId}", ctx.Saga.CorrelationId); })
                    .TransitionTo(AccountCreated)
                    .Publish(ctx =>
                        new FundAccountOpened(
                            ctx.Saga.CorrelationId,
                            ctx.Message.AccountCode,
                            ctx.Message.CustomerCode))
                    .Request(GenerateFundAccountOpeningDocumentsRequest, ctx =>
                        new GenerateFundAccountOpeningDocuments(
                            ctx.Saga.CorrelationId,
                            ctx.Message.CustomerCode,
                            ctx.Message.Ndid,
                            ctx.Saga.NdidRequestId,
                            ctx.Saga.NdidDateTime))
                    .Send(context => SendOpenSuccessCallback(context.Saga, OpenFundAccountStatus.SUCCESS)),
                When(CreateFundAccountRequest.Completed, ctx => !ctx.Saga.Ndid)
                    .Then(ctx => { logger.LogInformation("Fund account created: {TicketId}", ctx.Saga.CorrelationId); })
                    .Publish(ctx =>
                        new FundAccountOpened(
                            ctx.Saga.CorrelationId,
                            ctx.Message.AccountCode,
                            ctx.Message.CustomerCode))
                    .Finalize(),
                When(CreateFundAccountRequest.Faulted)
                    .Then(ctx =>
                    {
                        ctx.Saga.FailedReason = string.Join(", ", ctx.Message.Exceptions.Select(e => e.Message));
                        logger.LogError("create fund account request failed @{Exceptions}", ctx.Message.Exceptions);
                    })
                    .Publish(ctx => new FundAccountOpeningFailed
                    {
                        TicketId = ctx.Saga.CorrelationId,
                        CustomerCode = ctx.Message.Message.CustomerCode,
                        ErrorMessage = ctx.Message.Exceptions.FirstOrDefault()?.Message
                    })
                    .Send(context => SendOpenSuccessCallback(context.Saga, OpenFundAccountStatus.FAILED))
                    .TransitionTo(AccountCreateFailed)
            );

            During(AccountCreated,
                When(GenerateFundAccountOpeningDocumentsRequest!.Completed)
                    .Then(ctx =>
                    {
                        logger.LogInformation("create generate fund account opening documents request completed");
                        ctx.Saga.DocumentsGenerationTicketId = ctx.Message.DocumentsGenerationTicketId;
                    })
                    .Request(UploadFundAccountOpeningDocumentsRequest,
                        ctx => new UploadFundAccountOpeningDocuments(
                            ctx.Saga.CorrelationId,
                            ctx.Saga.UserId,
                            ctx.Message.CustomerCode,
                            ctx.Message.Documents,
                            ctx.Message.Ndid,
                            ctx.Saga.NdidRequestId,
                            ctx.Saga.IsOpenSegregateAccount)),
                When(GenerateFundAccountOpeningDocumentsRequest.Faulted)
                    .Then(ctx =>
                    {
                        ctx.Saga.FailedReason = string.Join(", ", ctx.Message.Exceptions.Select(e => e.Message));
                        logger.LogError("create generate fund account opening documents request failed @{Exceptions}", ctx.Message.Exceptions);
                    })
                    .TransitionTo(DocsUploadFailed)
            );

            DuringAny(
                When(UploadFundAccountOpeningDocumentsRequest!.Completed)
                    .Finalize(),
                When(UploadFundAccountOpeningDocumentsRequest.Faulted)
                    .Then(ctx =>
                    {
                        ctx.Saga.FailedReason = string.Join(", ", ctx.Message.Exceptions.Select(e => e.Message));
                        logger.LogError("Upload generate fund account opening documents request failed @{Exceptions}", ctx.Message.Exceptions);
                    })
                    .TransitionTo(DocsUploadFailed)
            );
        }
        private static SendOpenSuccessCallback SendOpenSuccessCallback(FundAccountOpeningState openingState, OpenFundAccountStatus status)
        {
            return new SendOpenSuccessCallback(
                openingState.CustomerCode ?? string.Empty,
                openingState.CustomerId ?? null,
                openingState.OpenAccountRegisterUid ?? string.Empty,
                status);
        }

        public State? Received { get; }
        public State? CustomerCreated { get; }
        public State? CustomerCreateFailed { get; }
        public State? AccountCreated { get; }
        public State? AccountCreateFailed { get; }
        public State? DocsUploadFailed { get; }
        public Event<AccountOpeningRequestReceived>? AccountOpeningRequestReceived { get; }

        public Request<FundAccountOpeningState, CreateFundCustomer, FundCustomerCreated> CreateFundCustomerRequest { get; }

        public Request<FundAccountOpeningState, CreateFundAccount, FundAccountCreated> CreateFundAccountRequest { get; }

        public Request<FundAccountOpeningState, GenerateFundAccountOpeningDocuments, AccountOpeningDocumentsGenerated>
            GenerateFundAccountOpeningDocumentsRequest
        { get; }

        public Request<FundAccountOpeningState, UploadFundAccountOpeningDocuments, AccountOpeningDocumentsUploaded>
            UploadFundAccountOpeningDocumentsRequest
        { get; }
    }
}
