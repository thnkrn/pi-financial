namespace Pi.BackofficeService.Domain.AggregateModels.TransactionAggregate;

public class Bank
{
    public Bank(string code, string name, string abbreviation)
    {
        Code = code;
        Name = name;
        Abbreviation = abbreviation;
    }

    public string Code { get; set; }
    public string Name { get; set; }
    public string Abbreviation { get; set; }
}
