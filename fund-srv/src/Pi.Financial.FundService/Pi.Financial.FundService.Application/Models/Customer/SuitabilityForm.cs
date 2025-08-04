namespace Pi.Financial.FundService.Application.Models.Customer;

public enum SuitabilityAnswer
{
    None = 0,
    One = 1,
    Two,
    Three,
    Four
}

public record SuitabilityForm(
    SuitabilityAnswer SuitNo1,
    SuitabilityAnswer SuitNo2,
    SuitabilityAnswer SuitNo3,
    List<SuitabilityAnswer> SuitNo4,
    SuitabilityAnswer SuitNo5,
    SuitabilityAnswer SuitNo6,
    SuitabilityAnswer SuitNo7,
    SuitabilityAnswer SuitNo8,
    SuitabilityAnswer SuitNo9,
    SuitabilityAnswer SuitNo10,
    SuitabilityAnswer SuitNo11,
    SuitabilityAnswer SuitNo12,
    SuitabilityAnswer SuitNo13,
    SuitabilityAnswer SuitNo14
);
