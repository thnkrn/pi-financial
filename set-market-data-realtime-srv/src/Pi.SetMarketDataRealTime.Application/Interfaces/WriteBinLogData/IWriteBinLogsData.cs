namespace Pi.SetMarketDataRealTime.Application.Interfaces.WriteBinlogData;

public interface IWriteBinLogsData : IDisposable
{
    Task WriteBinLogsDataAsync(byte[] bytes, string logPrefix);
    void EnqueueLogData(byte[] bytes, string logPrefix);
    Task FlushAsync();
    void CombineLogFiles(string folderPath, string logPrefix, DateTime startDate, DateTime endDate, string outputFile);
}