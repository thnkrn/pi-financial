using Microsoft.EntityFrameworkCore;
using Pi.Common.SeedWork;
using INumberGeneratorRepository = Pi.Common.Generators.Number.INumberGeneratorRepository;
using NumberGenerator = Pi.Common.Generators.Number.NumberGenerator;

namespace Pi.SetService.Infrastructure.Repositories;

public class NumberGeneratorRepository : INumberGeneratorRepository
{
    private readonly NumberGeneratorDbContext _context;
    public IUnitOfWork UnitOfWork => _context;

    public NumberGeneratorRepository(NumberGeneratorDbContext context)
    {
        _context = context;
    }

    public async Task<List<NumberGenerator>> GetAsync(string module, CancellationToken cancellationToken = default)
    {
        return await _context.NumberGenerators.Where(q => q.Module == module).ToListAsync(cancellationToken: cancellationToken);
    }

    public async Task<NumberGenerator?> GetAsync(string module, string prefix, bool daily, CancellationToken cancellationToken = default)
    {
        return await _context.NumberGenerators.FirstOrDefaultAsync(q => q.Module == module && q.Prefix == prefix && q.DailyReset == daily, cancellationToken: cancellationToken);
    }

    public async Task<NumberGenerator> CreateAsync(NumberGenerator numberGenerator, CancellationToken cancellationToken = default)
    {
        var result = await _context.NumberGenerators.AddAsync(numberGenerator, cancellationToken);
        return result.Entity;
    }

    public void Update(NumberGenerator numberGenerator)
    {
        _context.Update(numberGenerator);
    }
}
