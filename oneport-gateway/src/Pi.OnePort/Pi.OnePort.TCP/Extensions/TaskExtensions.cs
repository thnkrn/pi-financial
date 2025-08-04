namespace Pi.OnePort.TCP.Extensions;

internal static class TaskExtensions
{
    public static Task WithTimeout(this Task task, int? msTimeout = null)
    {
        AwaitTaskWithTimeout(task, msTimeout ?? null);
        return task;
    }

    public static Task<TR> WithTimeout<TR>(this Task<TR> task, int? msTimeout = null)
    {
        AwaitTaskWithTimeout(task, msTimeout ?? null);
        return task;
    }

    private static void AwaitTaskWithTimeout(Task task, int? msTimeout)
    {
        var waitResult = Task.WaitAny(new[] { task }, msTimeout ?? 3000);

        if (waitResult == -1)
        {
            throw new TimeoutException();
        }
    }
}
