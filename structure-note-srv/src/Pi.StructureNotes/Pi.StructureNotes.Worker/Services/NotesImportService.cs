using Pi.StructureNotes.Domain.Models;
using Pi.StructureNotes.Infrastructure.Repositories;
using Pi.StructureNotes.Infrastructure.Repositories.Entities;
using Pi.StructureNotes.Infrastructure.Services;

namespace Pi.StructureNotes.Worker.Services;

public class NotesImportService : BackgroundService
{
    private readonly IAccountService _accService;
    private readonly ILogger _logger;
    private readonly INoteRepository _noteRepository;
    private readonly INotesSource _noteSource;

    public NotesImportService(IAccountService accService, INotesSource noteSource, INoteRepository noteRepository,
        ILogger<NotesImportService> logger)
    {
        _accService = accService;
        _noteSource = noteSource;
        _noteRepository = noteRepository;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        DateTime sinceUtc = default;
        while (!ct.IsCancellationRequested)
        {
            try
            {
                DateTime cycleTimeUtc = DateTime.UtcNow;
                IEnumerable<AccountEntities>? data = await _noteSource.GetAccountEntities(sinceUtc, ct);
                if (data.Any())
                {
                    List<Task> tasks = new();
                    using SemaphoreSlim semaLock = new(10);
                    foreach (AccountEntities? accData in data)
                    {
                        await semaLock.WaitAsync(ct);
                        tasks.Add(ImportAccount(accData, semaLock, ct));
                    }

                    await Task.WhenAll(tasks);

                    sinceUtc = cycleTimeUtc;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Could not import notes from source");
            }
            finally
            {
                try
                {
                    await _noteRepository.CleanUp(ct);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Could not cleanup old data");
                }
                await Task.Delay(TimeSpan.FromMinutes(30));
            }
        }
    }

    private async Task ImportAccount(AccountEntities accData, SemaphoreSlim semaLock, CancellationToken ct)
    {
        try
        {
            AccountInfo? account = await _accService.GetSnAccountByAccountNo(accData.AccountNo, ct);
            if (account == null)
            {
                throw new Exception($"Can not find account for AccountNo: {accData.AccountNo}");
            }

            accData.SetAccountId(account.AccountId);
            await _noteRepository.Reset(accData, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Could not import notes for account {accData.AccountNo}");
            throw;
        }
        finally
        {
            semaLock.Release();
        }
    }
}
