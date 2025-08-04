using System.ComponentModel;

namespace Pi.User.API.Models;

public enum ErrorCodes
{
    [Description("Unknown error")]
    Usr0000,
    [Description("Unable to find data")]
    Usr0001,
    [Description("Unable to Create User Info")]
    Usr0002,
    [Description("Unable to Deregister Device")]
    Usr0003,
    [Description("User id not found")]
    Usr0004,
    [Description("Internal Server Error")]
    Usr0005,
    [Description("Customer not found")]
    Usr0006
}