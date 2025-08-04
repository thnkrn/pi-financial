using System.Globalization;
using System.Text.RegularExpressions;
using CsvHelper;
using CsvHelper.Configuration;
using Pi.StructureNotes.Infrastructure.Models;
using Pi.StructureNotes.Infrastructure.Repositories.Entities;

namespace Pi.StructureNotes.Infrastructure.Services;

public class CsvNoteSource : INotesSource
{
    private static readonly Regex _dateRegex = new(@"^.*_(?<date>\d{4}-\d{2}-\d{2}).*$");
    private readonly CsvConfiguration _config;
    private readonly INoteFileReader _reader;

    public CsvNoteSource(INoteFileReader reader)
    {
        _reader = reader ?? throw new ArgumentNullException(nameof(reader));
        _config = new CsvConfiguration(CultureInfo.InvariantCulture) { Delimiter = "|", HeaderValidated = null };
    }

    public async Task<IEnumerable<AccountEntities>> GetAccountEntities(DateTime sinceUtc,
        CancellationToken ct = default)
    {
        var files = _reader.GetFilesToRead(sinceUtc, ct);
        if (!await files.AnyAsync(ct))
        {
            return Enumerable.Empty<AccountEntities>();
        }

        var notes = Enumerable.Empty<NoteEntity>();
        var stocks = Enumerable.Empty<StockEntity>();
        var cashes = Enumerable.Empty<CashEntity>();

        await foreach ((FileType Type, string File) file in files)
        {
            var asOf = GetDate(file.File);
            using var stream = await _reader.ReadFileAsync(file.File, ct);
            switch (file.Type)
            {
                case FileType.Note:
                    {
                        notes = await ReadStreamAsync<NoteEntity>(stream, asOf, ct);
                        break;
                    }
                case FileType.Stock:
                    {
                        stocks = await ReadStreamAsync<StockEntity>(stream, asOf, ct);
                        break;
                    }
                case FileType.Cash:
                    {
                        cashes = await ReadStreamAsync<CashEntity>(stream, asOf, ct);
                        break;
                    }
            }
        }

        var accountNos = notes.Select(x => x.AccountNo)
            .Union(stocks.Select(x => x.AccountNo))
            .Union(cashes.Select(x => x.AccountNo))
            .Distinct();

        var result = new List<AccountEntities>();
        foreach (string accNo in accountNos)
        {
            var accNotes = notes.Where(x => x.AccountNo == accNo);
            var accStocks = stocks.Where(x => x.AccountNo == accNo);
            var accCash = cashes.Where(x => x.AccountNo == accNo);

            var acc = new AccountEntities() { AccountNo = accNo, Notes = accNotes, Stocks = accStocks, Cash = accCash };

            result.Add(acc);
        }

        return result;
    }

    private async Task<IEnumerable<T>> ReadStreamAsync<T>(Stream stream, DateTime asOf, CancellationToken ct)
        where T : EntityBase
    {
        using var sr = new StreamReader(stream);
        using var reader = new CsvReader(sr, _config, true);
        var records = reader.GetRecordsAsync<T>(ct);
        var result = await records.ToArrayAsync(ct);
        foreach (T item in result)
        {
            item.CreatedAt = DateTime.UtcNow;
            item.UpdatedAt = DateTime.UtcNow;
            item.AsOf = asOf;
        }

        return result;
    }

    private DateTime GetDate(string fileName) => DateTime.Parse(_dateRegex.Match(fileName).Groups["date"].Value);
}
