using Microsoft.EntityFrameworkCore;
using Pi.Common.SeedWork;
using Pi.TfexService.Domain.Models.InitialMargin;

namespace Pi.TfexService.Infrastructure.Repositories;

public class InitialMarginRepository(TfexDbContext dbContext) : IInitialMarginRepository
{
    public IUnitOfWork UnitOfWork => dbContext;

    public async Task<InitialMargin?> GetInitialMargin(string symbol, CancellationToken cancellationToken)
    {
        return await dbContext.InitialMargins.Where(im => im.Symbol == symbol).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<List<InitialMargin>> GetInitialMarginList(List<string> symbols, CancellationToken cancellationToken)
    {
        return await dbContext.InitialMargins.Where(im => symbols.Contains(im.Symbol)).ToListAsync(cancellationToken);
    }

    public async Task UpsertInitialMargin(List<InitialMargin> initialMarginList, CancellationToken cancellationToken)
    {
        // Begin a transaction to ensure atomicity
        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            foreach (var initialMargin in initialMarginList)
            {
                // Check if an entity with the same Symbol exists in the database
                var existingEntity = await dbContext.InitialMargins
                    .FirstOrDefaultAsync(x => x.Symbol == initialMargin.Symbol, cancellationToken);

                if (existingEntity != null)
                {
                    // Update the existing entity's properties
                    existingEntity.ProductType = initialMargin.ProductType;
                    existingEntity.Im = initialMargin.Im;
                    existingEntity.ImOutright = initialMargin.ImOutright;
                    existingEntity.ImSpread = initialMargin.ImSpread;
                    existingEntity.AsOfDate = initialMargin.AsOfDate;

                    // Mark the entity as modified so that EF Core will update it in the database
                    dbContext.InitialMargins.Update(existingEntity);
                }
                else
                {
                    // Add a new entity to the database
                    initialMargin.Id = Guid.NewGuid(); // Ensure the ID is set
                    await dbContext.InitialMargins.AddAsync(initialMargin, cancellationToken);
                }
            }

            // Save changes and commit the transaction
            await dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }
        catch (Exception)
        {
            // Roll back the transaction if any error occurs
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
}