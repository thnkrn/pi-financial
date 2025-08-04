namespace Pi.GlobalEquities.Repositories;

public interface IWorkerJobRepository
{
    Task<DomainModels.WorkerJob<T>> GetJobDetails<T>(string jobName, CancellationToken ct);
    Task ReplaceJobDetails<T>(DomainModels.WorkerJob<T> job, CancellationToken ct);
}
