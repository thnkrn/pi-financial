using System;
using MassTransit;
using Microsoft.Extensions.Logging;
using Pi.Common.Utilities;
using Pi.User.Application.Services.LegacyUserInfo;
using Pi.User.Domain.AggregatesModel.TransactionIdAggregate;
using Pi.User.Domain.AggregatesModel.UserInfoAggregate;
using Pi.User.IntegrationEvents;

namespace Pi.User.Application.Commands
{
    public record UpdateBankAccountEffectiveDate(Guid UserId, string CustomerCode, string BankAccountNo, string BankCode, string? BankBranchCode);

    public record UpdateBankAccountEffectiveDateConsumer : IConsumer<UpdateBankAccountEffectiveDate>
    {
        private readonly IUserBankAccountService _userBankAccountService;
        private readonly DateTimeProvider _dateTimeProvider;
        private readonly ILogger<UpdateBankAccountEffectiveDateConsumer> _logger;
        private readonly ITransactionIdRepository _transactionIdRepository;
        private readonly IUserInfoRepository _userInfoRepository;
        private readonly IBus _bus;

        public UpdateBankAccountEffectiveDateConsumer(
            IUserBankAccountService userBankAccountService,
            DateTimeProvider dateTimeProvider,
            ILogger<UpdateBankAccountEffectiveDateConsumer> logger,
            ITransactionIdRepository transactionIdRepository,
            IUserInfoRepository userInfoRepository,
            IBus bus)
        {
            _userBankAccountService = userBankAccountService;
            _dateTimeProvider = dateTimeProvider;
            _logger = logger;
            _transactionIdRepository = transactionIdRepository;
            _userInfoRepository = userInfoRepository;
            _bus = bus;
        }

        public async Task Consume(ConsumeContext<UpdateBankAccountEffectiveDate> context)
        {
            var bankAccountInfo = await this._userBankAccountService.GetBankAccountInfoAsync(context.Message.CustomerCode, context.CancellationToken);

            if (bankAccountInfo is null)
            {
                this._logger.LogInformation("[{UserId}][{CustomerCode}] There is no bank account info", context.Message.UserId, context.Message.CustomerCode);
                var dateTime = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(_dateTimeProvider.OffsetNow(), "Asia/Bangkok");
                var referId = $"UB{dateTime:yyyyMMddHHmmss}";
                var transactionId = await _transactionIdRepository.GetNextAsync("UB", DateOnly.FromDateTime(dateTime.Date), referId, context.Message.CustomerCode, context.CancellationToken);
                await _transactionIdRepository.UnitOfWork.SaveChangesAsync(context.CancellationToken);

                var userInfo = await _userInfoRepository.Get(context.Message.UserId);
                await _bus.Publish(
                    new SubmitBankAccountRequest(
                        context.Message.UserId,
                        context.Message.BankAccountNo,
                        $"{userInfo!.FirstnameEn} {userInfo.LastnameEn}",
                        context.Message.BankCode,
                        context.Message.BankBranchCode,
                        null,
                        null
                    )
                );
                await _bus.Publish(
                    new AddBankAccountWithEffectiveEvent(
                        context.Message.UserId,
                        referId,
                        transactionId.ToString(),
                        context.Message.CustomerCode,
                        context.Message.BankAccountNo,
                        context.Message.BankCode
                    )
                );
                return;
            }

            this._logger.LogInformation("[{UserId}][{CustomerCode}] Current bank account info is {@BankAccountInfo}",
                context.Message.UserId, context.Message.CustomerCode, bankAccountInfo);
            var updatedBankAccountInfoItems = new List<BankAccountInfoItem>();
            foreach (var o in bankAccountInfo.Items)
            {
                if (o.BankAccountNo == context.Message.BankAccountNo && o.BankCode == context.Message.BankCode && o.RPType == "R")
                {
                    o.EffectiveDate = DateOnly.FromDateTime(_dateTimeProvider.Now());
                    o.EndDate = DateOnly.MaxValue;
                    updatedBankAccountInfoItems.Add(o);
                }
            }

            if (!updatedBankAccountInfoItems.Any())
            {
                this._logger.LogWarning("[{UserId}][{CustomerCode}] There is not any bank account to update", context.Message.UserId, context.Message.CustomerCode);
                await context.Publish(new UpdateBankAccountEffectiveDateFailedEvent(context.Message.CustomerCode, "BANK_ACCOUNT_MISMATCH"));
                return;
            }

            var updatedBankAccountInfo = new BankAccountInfo(bankAccountInfo.CustomerCode, updatedBankAccountInfoItems, updatedBankAccountInfoItems.Count);
            this._logger.LogInformation("[{UserId}][{CustomerCode}] New bank account info is {@BankAccountInfo}",
                context.Message.UserId, context.Message.CustomerCode, updatedBankAccountInfo);
            await this._userBankAccountService.UpdateBankAccountInfoAsync(updatedBankAccountInfo, context.CancellationToken);
            this._logger.LogInformation("[{UserId}][{CustomerCode}] Updating bank account effective", context.Message.UserId, context.Message.CustomerCode);
        }
    }
}

