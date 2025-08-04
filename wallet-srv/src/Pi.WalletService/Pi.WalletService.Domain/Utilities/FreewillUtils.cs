using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Runtime.Serialization;

namespace Pi.WalletService.Domain.Utilities;

public static class FreewillUtils
{
    public enum FreewillResultCode
    {
        [Display(Description = "Can not Approve to Front Office")]
        CannotApproveToFrontOffice = 006,

        [Display(Description = "Lock Table in Back Office")]
        LockTableInBackOffice = 008,

        [Display(Description = "Deposit Withdraw Disabled")]
        DepositWithdrawDisabled = 023,

        [Display(Description = "Connection Time Out")]
        ConnectionTimeout = 900,

        [Display(Description = "Internal Server Error")]
        InternalServerError = 906,
    }

    public static string GetResultMessage(string resultCode, string defaultValue = "")
    {
        var result = Enum.TryParse(resultCode, out FreewillResultCode resultCodeEnum) && Enum.IsDefined(typeof(FreewillResultCode), resultCodeEnum);
        if (!result)
        {
            return defaultValue;
        }

        var metadata = resultCodeEnum.GetDisplayAttributes(typeof(FreewillResultCode));
        return metadata?.Description ?? defaultValue;
    }

    private static DisplayAttribute GetDisplayAttributes(this Enum enumValue, Type enumType)
    {
        return enumType.GetMember(enumValue.ToString())
            .FirstOrDefault()?
            .GetCustomAttribute<DisplayAttribute>()!;
    }
}