using System.Globalization;
using Microsoft.Extensions.Options;
using Pi.WalletService.Application.Models;
using Pi.WalletService.Application.Options;
using Pi.WalletService.Application.Services;
using Pi.WalletService.IntegrationEvents.AggregatesModel;

namespace Pi.WalletService.Infrastructure.Services;

public class ValidationService : IValidationService
{
    private readonly FeaturesOptions _featuresOptions;
    public ValidationService(IOptionsSnapshot<FeaturesOptions> featuresOptions)
    {
        _featuresOptions = featuresOptions.Value;
    }

    private ValidationResult Ok()
    {
        return new ValidationResult
        {
            Success = true
        };
    }

    private ValidationResult Error(string errorCode, string errorMessage)
    {
        return new ValidationResult
        {
            Success = false,
            ErrorCode = errorCode,
            ErrorMessage = errorMessage
        };
    }

    public bool IsOutsideWorkingHour(Product product, Channel channel, DateTime currentDateTime, out ValidationResult validationResult)
    {
        var timeNow = TimeOnly.FromDateTime(currentDateTime);
        var freewillOpeningTime = TimeOnly.Parse(_featuresOptions.FreewillOpeningTime, CultureInfo.InvariantCulture);
        var freewillClosingTime = TimeOnly.Parse(_featuresOptions.FreewillClosingTime, CultureInfo.InvariantCulture);
        var kkpOpeningTime = TimeOnly.Parse(_featuresOptions.KkpOpeningTime, CultureInfo.InvariantCulture);
        var kkpClosingTime = TimeOnly.Parse(_featuresOptions.KkpClosingTime, CultureInfo.InvariantCulture);

        // todo: remove once force-update v2
        var bankMaintenanceErrorCode = _featuresOptions.ShouldUseNewErrorCodeOnBankMaintenance
            ? ErrorCodes.BankMaintenance
            : ErrorCodes.OutsideWorkingHourError;
        var bankMaintenanceErrorMessage = _featuresOptions.ShouldUseNewErrorCodeOnBankMaintenance
            ? ErrorMessages.BankMaintenance
            : ErrorMessages.OutsideWorkingHourError;

        validationResult = Ok();
        if (product == Product.GlobalEquities)
        {
            switch (channel)
            {
                case Channel.ATS when OutsideFreewillWorkingHour():
                    validationResult = Error(ErrorCodes.OutsideWorkingHourError, ErrorMessages.OutsideWorkingHourError);
                    return true;
                case Channel.OnlineViaKKP or Channel.QR when OutsideBankWorkingHour():
                    validationResult = Error(bankMaintenanceErrorCode, bankMaintenanceErrorMessage);
                    return true;
                default:
                    return false;
            }
        }

        if (OutsideFreewillWorkingHour())
        {
            validationResult = Error(ErrorCodes.OutsideWorkingHourError, ErrorMessages.OutsideWorkingHourError);
            return true;
        }

        if (channel is Channel.OnlineViaKKP or Channel.QR && OutsideBankWorkingHour())
        {
            validationResult = Error(bankMaintenanceErrorCode, bankMaintenanceErrorMessage);
            return true;
        }

        return false;

        bool OutsideBankWorkingHour() => timeNow < kkpOpeningTime || timeNow > kkpClosingTime;
        bool OutsideFreewillWorkingHour() => timeNow < freewillOpeningTime || timeNow > freewillClosingTime;
    }
}