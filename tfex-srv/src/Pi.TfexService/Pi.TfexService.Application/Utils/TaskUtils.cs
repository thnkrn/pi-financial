namespace Pi.TfexService.Application.Utils;

public static class TaskExt
{
    /// <summary>
    /// A workaround for getting all of AggregateException.InnerExceptions with try/await/catch
    /// </summary>
    public static Task WithAggregatedExceptions(this Task @this)
    {
        // using AggregateException.Flatten as a bonus
        return @this.ContinueWith(
            continuationFunction: anteTask =>
                anteTask is { IsFaulted: true, Exception: { } ex } &&
                (ex.InnerExceptions.Count > 1 || ex.InnerException is AggregateException)
                    ? Task.FromException(ex.Flatten())
                    : anteTask,
            cancellationToken: CancellationToken.None,
            TaskContinuationOptions.ExecuteSynchronously,
            scheduler: TaskScheduler.Default).Unwrap();
    }
}