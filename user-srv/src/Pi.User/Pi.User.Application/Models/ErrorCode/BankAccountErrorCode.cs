using System.ComponentModel;

namespace Pi.User.Application.Models.ErrorCode;

public enum BankAccountErrorCode
{
    [Description("Unknown Error")]
    BA000,
    [Description("Upload File Failed")]
    BA001,
    [Description("Bank account not found")]
    BA002,
    [Description("Missing bank account info")]
    BA003,
}