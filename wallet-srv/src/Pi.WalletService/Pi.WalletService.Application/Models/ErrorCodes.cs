using System;
namespace Pi.WalletService.Application.Models
{
    public static class ErrorCodes
    {
        public const string InternalServerError = "WAL0000";
        public const string InvalidUserId = "WAL0001";
        public const string BankServiceError = "WAL0002";
        public const string UserOtpRequestLimitReached = "WAL0003";
        public const string UserOtpVerificationLimitReached = "WAL0004";
        public const string OutsideWorkingHourError = "WAL0005";
        public const string NoBankAccountFound = "WAL0006";
        public const string InvalidFormat = "WAL0007";
        public const string InvalidData = "WAL0008";
        public const string BankMaintenance = "WAL0009";
        public const string InvalidCustomerCode = "WAL0010";
        public const string DepositWithdrawDisabled = "WAL0011";
    }

    public static class ErrorMessages
    {
        public const string OutsideWorkingHourError = "The request was received outside working hours.";
        public const string BankMaintenance = "The request was received inside bank maintenance periods.";
    }
}

