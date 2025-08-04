using AutoMapper;
using Pi.User.Application.Models.BankAccount;
using Pi.User.Domain.AggregatesModel.BankAccountAggregate;

namespace Pi.User.Application.Queries.BankAccount;

public class BankAccountQueries : IBankAccountQueries
{
    private readonly IBankAccountRepository _bankAccountRepository;


    public BankAccountQueries(
        IBankAccountRepository bankAccountRepository)
    {
        _bankAccountRepository = bankAccountRepository;
    }

    public async Task<BankAccountDto> GetBankAccountByUserId(Guid userId)
    {
        var bankAccount = await _bankAccountRepository.GetByUserIdAsync(userId) ??
                          throw new InvalidDataException($"Bank account not found with userId: {userId}");
        var config = new MapperConfiguration(cfg =>
            cfg
                .CreateMap<Domain.AggregatesModel.BankAccountAggregate.BankAccount, BankAccountDto>()
                .ForAllMembers(
                    opts => opts.Condition((src, dest, member) => member != null)
                )
        );
        var mapper = config.CreateMapper();
        var mapResp = mapper.Map<Domain.AggregatesModel.BankAccountAggregate.BankAccount, BankAccountDto>(bankAccount);

        return mapResp;
    }
}