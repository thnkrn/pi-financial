using System.Reflection;

namespace Pi.BackofficeService.Application.Models;

public static class Metrics
{
    #region ticket

    public const string TicketReceived = "ticket.received";
    public const string TicketCreated = "ticket.created";
    public const string TicketPending = "ticket.pending";
    public const string TicketApproved = "ticket.approved";
    public const string TicketRejected = "ticket.rejected";
    public const string TicketSuccess = "ticket.success";
    public const string TicketFailed = "ticket.failed";
    public const string TicketRequestToCheckTime = "ticket.rtc_time";
    public const string TicketExecutionTime = "ticket.execution_time";

    #endregion

    public static List<T> GetAllPublicConstValues<T>()
    {
        return typeof(Metrics)
            .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
            .Where(fi => fi.IsLiteral && !fi.IsInitOnly)
            .Select(x => x.GetRawConstantValue())
            .OfType<T>()
            .ToList();
    }
}
