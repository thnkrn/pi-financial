using Pi.StructureNotes.Infrastructure.Models;
using Pi.StructureNotes.Infrastructure.Repositories.Entities;

namespace Pi.StructureNotes.Infrastructure.Tests.Services;

public class CsvNoteSourceTest
{
    private CancellationToken _ct => It.IsAny<CancellationToken>();
    private DateTime _anyTime => It.IsAny<DateTime>();

    [Fact(Skip = "For instant testing only")]
    public async Task GetAccountData_Success()
    {
        Mock<INoteFileReader> readerMock =
            CreateReaderMock(out DateTime notesDate, out DateTime stocksDate, out DateTime cashDate);

        CsvNoteSource csvSource = new(readerMock.Object);

        IEnumerable<AccountEntities>? result = await csvSource.GetAccountEntities(_anyTime, _ct);

        AccountEntities[] expected = GetExpectedCsvData(notesDate, stocksDate, cashDate);

        expected.Should().BeEquivalentTo(result,
            opt =>
            {
                opt.For(x => x.Notes).Exclude(x => x.Id);
                opt.For(x => x.Notes).Exclude(x => x.CreatedAt);
                opt.For(x => x.Notes).Exclude(x => x.UpdatedAt);

                opt.For(x => x.Stocks).Exclude(x => x.Id);
                opt.For(x => x.Stocks).Exclude(x => x.CreatedAt);
                opt.For(x => x.Stocks).Exclude(x => x.UpdatedAt);

                opt.For(x => x.Cash).Exclude(x => x.Id);
                opt.For(x => x.Cash).Exclude(x => x.CreatedAt);
                opt.For(x => x.Cash).Exclude(x => x.UpdatedAt);
                return opt;
            });
    }

    private Mock<INoteFileReader> CreateReaderMock(out DateTime notesDate, out DateTime stocksDate,
        out DateTime cashDate)
    {
        string nHeaders =
            "AccountNo|Symbol|Currency|ISIN|Issuer|Type|MarketValue|CostValue|Yield|Rebate|TradeDate|IssueDate|ValuationDate|Underlying|Tenors";
        string n1 = "1|S1|USD|I1|Issuer1|Type1|100|100|0|1|2023-01-01|2023-01-02|2024-01-03|U1|1";
        string n2 = "2|S2|USD|I2|Issuer2|Type2|200|200|0|2|2023-02-01|2023-02-02|2024-02-03|U2|2";

        string sHeaders = "AccountNo|Symbol|Currency|Units|Available|CostPrice|MarketPrice";
        string s1 = "1|S1|USD|1|1|1|1";
        string s2 = "2|S2|USD|2|2|2|2";


        string cHeaders = "AccountNo|Symbol|Currency|MarketValue|CostValue|GainInPortfolio";
        string c1 = "1|USD|USD|1|1|1|1";
        string c2 = "2|USD|USD|2|2|2|2";


        Mock<INoteFileReader> readerMock = new();

        notesDate = new DateTime(2023, 01, 10);
        stocksDate = new DateTime(2023, 01, 20);
        cashDate = new DateTime(2023, 01, 30);
        string notesFile = $"notes_{notesDate:yyyy-MM-dd}.csv";
        string stocksFile = $"stocks_{stocksDate:yyyy-MM-dd}.csv";
        string cashFile = $"cash_{cashDate:yyyy-MM-dd}.csv";

        (FileType Note, string notesFile) notes = (FileType.Note, notesFile);
        (FileType Stock, string stocksFile) stocks = (FileType.Stock, stocksFile);
        (FileType Cash, string cashFile) cash = (FileType.Cash, cashFile);

        IAsyncEnumerable<(FileType, string)> filesToRead = new[] { notes, stocks, cash }.ToAsyncEnumerable();
        readerMock.Setup(x => x.GetFilesToRead(_anyTime, _ct)).Returns(filesToRead);
        readerMock.Setup(x => x.ReadFileAsync(notesFile, _ct)).Returns(CreateSteram(nHeaders, n1, n2));
        readerMock.Setup(x => x.ReadFileAsync(stocksFile, _ct)).Returns(CreateSteram(sHeaders, s1, s2));
        readerMock.Setup(x => x.ReadFileAsync(cashFile, _ct)).Returns(CreateSteram(cHeaders, c1, c2));

        return readerMock;
    }

    private async Task<Stream> CreateSteram(string header, params string[] lines)
    {
        MemoryStream ms = new();
        using StreamWriter sr = new(ms, leaveOpen: true);
        await sr.WriteLineAsync(header);
        foreach (string line in lines)
        {
            await sr.WriteLineAsync(line);
        }

        await sr.FlushAsync();

        ms.Position = 0;
        return ms;
    }

    private AccountEntities[] GetExpectedCsvData(DateTime notesDate, DateTime stocksDate, DateTime cashDate) =>
        new AccountEntities[]
        {
            new()
            {
                AccountNo = "1",
                Notes =
                    new NoteEntity[]
                    {
                        new()
                        {
                            AsOf = notesDate,
                            Currency = "USD",
                            AccountId = "1",
                            ISIN = "I1",
                            Symbol = "S1",
                            MarketValue = 100,
                            CostValue = 100,
                            Type = "Type1",
                            Yield = 0,
                            Rebate = 1,
                            Underlying = "U1",
                            Tenors = 1,
                            TradeDate = DateTime.Parse("2023-01-01T00:00:00"),
                            IssueDate = DateTime.Parse("2023-01-02T00:00:00"),
                            ValuationDate = DateTime.Parse("2024-01-03T00:00:00"),
                            Issuer = "Issuer1"
                        }
                    },
                Stocks =
                    new StockEntity[]
                    {
                        new()
                        {
                            AsOf = stocksDate,
                            AccountId = "1",
                            Symbol = "S1",
                            Currency = "USD",
                            Units = 1,
                            Available = 1,
                            CostPrice = 1
                        }
                    },
                Cash =
                    new CashEntity[]
                    {
                        new()
                        {
                            AsOf = cashDate,
                            AccountId = "1",
                            Currency = "USD",
                            MarketValue = 1,
                            CostValue = 1,
                            GainInPortfolio = 1
                        }
                    }
            },
            new()
            {
                AccountNo = "2",
                Notes = new NoteEntity[]
                {
                    new()
                    {
                        AsOf = notesDate,
                        AccountId = "2",
                        Currency = "USD",
                        ISIN = "I2",
                        Symbol = "S2",
                        MarketValue = 200,
                        CostValue = 200,
                        Type = "Type2",
                        Yield = 0,
                        Rebate = 2,
                        Underlying = "U2",
                        Tenors = 2,
                        TradeDate = DateTime.Parse("2023-02-01T00:00:00"),
                        IssueDate = DateTime.Parse("2023-02-02T00:00:00"),
                        ValuationDate = DateTime.Parse("2024-02-03T00:00:00"),
                        Issuer = "Issuer2"
                    }
                },
                Stocks = new StockEntity[]
                {
                    new()
                    {
                        AsOf = stocksDate,
                        AccountId = "2",
                        Symbol = "S2",
                        Currency = "USD",
                        Units = 2,
                        Available = 2,
                        CostPrice = 2
                    }
                },
                Cash = new CashEntity[]
                {
                    new()
                    {
                        AsOf = cashDate,
                        AccountId = "2",
                        Currency = "USD",
                        MarketValue = 2,
                        CostValue = 2,
                        GainInPortfolio = 2
                    }
                }
            }
        };
}
