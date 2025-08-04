using System.Linq.Expressions;
using System.Reflection;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Pi.SetMarketData.Domain.Entities.SetSmart;
using Pi.SetMarketData.Domain.Models.Request.BrokerInfo;
using Pi.SetMarketData.Infrastructure.Exceptions;
using Pi.SetMarketData.Infrastructure.Interfaces.Mongo;

namespace Pi.SetMarketData.Infrastructure.Services.Mongo;

public class MongoRepository<TEntity> : IMongoRepository<TEntity>
    where TEntity : class
{
    private const string UpsertErrorMessage = "Failed to upsert entity with filterExpression.";
    private readonly IMongoContext _context;
    private readonly ILogger<MongoRepository<TEntity>> _logger;

    /// <summary>
    /// </summary>
    /// <param name="context"></param>
    /// <param name="logger"></param>
    public MongoRepository(IMongoContext context, ILogger<MongoRepository<TEntity>> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<TEntity>> GetAllAsync()
    {
        try
        {
            return await _context.GetCollection<TEntity>().Find(_ => true).ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get all entities.");
            throw new MongoRepositoryException("Failed to get all entities.", ex);
        }
    }

    public async Task<TEntity?> GetByIdAsync(string id)
    {
        try
        {
            if (!ObjectId.TryParse(id, out var objectId))
                return default;
            var filter = Builders<TEntity>.Filter.Eq("_id", objectId);
            return await _context.GetCollection<TEntity>().Find(filter).FirstOrDefaultAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get entity with id {Id}.", id);
            throw new MongoRepositoryException($"Failed to get entity with id {id}.", ex);
        }
    }

    public async Task<TEntity?> GetByFilterAsync(Expression<Func<TEntity, bool>> filterExpression)
    {
        try
        {
            return await _context
                .GetCollection<TEntity>()
                .Find(filterExpression)
                .FirstOrDefaultAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to get entity with current filter.{FilterExpression}",
                filterExpression
            );
            throw new MongoRepositoryException("Failed to get entity with current filter.", ex);
        }
    }

    public async Task<IEnumerable<TEntity>> GetAllByFilterAsync(Expression<Func<TEntity, bool>> filterExpression)
    {
        try
        {
            return await _context
                .GetCollection<TEntity>()
                .Find(filterExpression)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to get all entity with current filter. {FilterExpression}",
                filterExpression
            );
            throw new MongoRepositoryException("Failed to get all entity with current filter.", ex);
        }
    }

    public async Task<IEnumerable<TEntity>> GetAllByFiltersAsync(IEnumerable<Expression<Func<TEntity, bool>>> expressions)
    {
        try
        {
            var query = _context.GetCollection<TEntity>().AsQueryable();
            if (query == null)
            {
                return [];
            }
            
            query = expressions.Aggregate(query, (current, expression) => current.Where(expression));

            return await query.ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to get all entity with current filter. {@Expressions}",
                expressions
            );
            throw new MongoRepositoryException("Failed to get all entity with current filter.", ex);
        }
    }

    public async Task CreateAsync(TEntity entity)
    {
        var idProperty = entity.GetType().GetProperties()
            .FirstOrDefault(p => p.GetCustomAttributes(typeof(BsonIdAttribute), false).Any());

        if (idProperty != null)
        {
            var currentId = idProperty.GetValue(entity);

            if (currentId == null ||
                (currentId is ObjectId objectId && objectId == ObjectId.Empty) ||
                (currentId is string stringId && string.IsNullOrWhiteSpace(stringId)) ||
                (currentId is Guid guid && guid == Guid.Empty))
            {
                object? newId = currentId switch
                {
                    ObjectId => ObjectId.GenerateNewId(),
                    string => ObjectId.GenerateNewId().ToString(),
                    Guid => Guid.NewGuid(),
                    _ => null
                };

                if (newId != null)
                {
                    idProperty.SetValue(entity, newId);
                }
            }
        }
        try
        {
            await _context.GetCollection<TEntity>().InsertOneAsync(entity);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create entity.");
            throw new MongoRepositoryException("Failed to create entity.", ex);
        }
    }

    public async Task UpdateAsync(string id, TEntity entity)
    {
        try
        {
            if (ObjectId.TryParse(id, out var objectId))
            {
                var bsonEntity = entity.ToBsonDocument();
                bsonEntity["_id"] = objectId; // Ensure the entity has the correct _id
                var updatedEntity = BsonSerializer.Deserialize<TEntity>(bsonEntity);
                var filter = Builders<TEntity>.Filter.Eq("_id", objectId);
                var updateDefinition = Builders<TEntity>.Update.Combine(
                    typeof(TEntity)
                        .GetProperties()
                        .Where(property => property.GetValue(entity) != null && property.Name != "IdString")
                        .Select(property =>
                        {
                            var value = property.GetValue(updatedEntity);
                            return Builders<TEntity>.Update.Set(property.Name, value);
                        })
                );

                await _context.GetCollection<TEntity>().UpdateOneAsync(filter, updateDefinition);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update entity with id {Id}.", id);
            throw new MongoRepositoryException("Failed to update entity.", ex);
        }
    }

    public async Task UpdateManyAsync(IEnumerable<TEntity> entities, Expression<Func<TEntity, object>> idSelector)
    {
        var idProperty = GetPropertyFromExpression(idSelector);
        var bulkOperations = new List<WriteModel<TEntity>>();

        foreach (var entity in entities)
        {
            var idValue = idProperty.GetValue(entity);
            var filter = Builders<TEntity>.Filter.Eq(idSelector, idValue);

            var updates = new List<UpdateDefinition<TEntity>>();
            updates = UpdateEntityDefinitions(updates, entity);

            if (updates.Count > 0)
            {
                var combinedUpdate = Builders<TEntity>.Update.Combine(updates);
                bulkOperations.Add(new UpdateOneModel<TEntity>(filter, combinedUpdate) { IsUpsert = false });
            }
        }

        if (bulkOperations.Count > 0)
        {
            var bulkWriteOptions = new BulkWriteOptions { IsOrdered = false };
            try
            {
                await _context.GetCollection<TEntity>().BulkWriteAsync(bulkOperations, bulkWriteOptions);
            }
            catch (MongoBulkWriteException<TEntity> ex)
            {
                var failedOperations = ex.WriteErrors
                    .Select(error => (error.Index, Error: error.Message));
                throw new MongoBulkOperationException("Bulk update failed for some documents.", failedOperations);
            }
        }
    }

    public async Task UpsertAsync(string id, TEntity entity)
    {
        try
        {
            if (ObjectId.TryParse(id, out var objectId))
            {
                var filter = Builders<TEntity>.Filter.Eq("_id", objectId);
                var updateDefinitions = new List<UpdateDefinition<TEntity>>();

                foreach (var property in typeof(TEntity).GetProperties())
                {
                    var propertyValue = property.GetValue(entity);
                    if (propertyValue == null)
                        continue;
                    if (
                        property.PropertyType.IsGenericType
                        && property.PropertyType.GetGenericTypeDefinition() == typeof(List<>)
                    )
                    {
                        // Property is a list, use PushEach to append elements
                        var pushUpdate =
                            Builders<TEntity>.Update.PushEach(property.Name, (IEnumerable<object>)propertyValue);
                        updateDefinitions.Add(pushUpdate);
                    }
                    else
                    {
                        // Property is not a list, use Set to replace the value
                        var setUpdate = Builders<TEntity>.Update.Set(property.Name, propertyValue);
                        updateDefinitions.Add(setUpdate);
                    }
                }

                var update = Builders<TEntity>.Update.Combine(updateDefinitions);
                var options = new UpdateOptions { IsUpsert = true };
                await _context.GetCollection<TEntity>().UpdateOneAsync(filter, update, options);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to upsert entity with id {Id}.", id);
            throw new MongoRepositoryException($"Failed to upsert entity with id {id}.", ex);
        }
    }

    public async Task<BulkWriteResult?> UpsertManyAsync(IEnumerable<TEntity> entities, Expression<Func<TEntity, object>> idSelector)
    {
        var idProperty = GetPropertyFromExpression(idSelector);
        var bulkOperations = new List<WriteModel<TEntity>>();

        foreach (var entity in entities)
        {
            var idValue = idProperty.GetValue(entity);
            // Generate a new ID if the entity doesn't have one
            if (idValue == null || (idValue is ObjectId objectId && objectId == ObjectId.Empty))
            {
                var newId = ObjectId.GenerateNewId();
                idProperty.SetValue(entity, newId);
                idValue = newId;
            }

            var filter = Builders<TEntity>.Filter.Eq(idSelector, idValue);

            var updateDefinitions = new List<UpdateDefinition<TEntity>>();
            updateDefinitions = UpdateEntityDefinitions(updateDefinitions, entity);

            if (updateDefinitions.Count > 0)
            {
                var combinedUpdate = Builders<TEntity>.Update.Combine(updateDefinitions);
                bulkOperations.Add(new UpdateOneModel<TEntity>(filter, combinedUpdate) { IsUpsert = true });
            }
        }

        if (bulkOperations.Count > 0)
        {
            var bulkWriteOptions = new BulkWriteOptions { IsOrdered = false };
            try
            {
                var result = await _context.GetCollection<TEntity>().BulkWriteAsync(bulkOperations, bulkWriteOptions);
                return result;
            }
            catch (MongoBulkWriteException<TEntity> ex)
            {
                var failedOperations = ex.WriteErrors
                    .Select(error => (error.Index, Error: error.Message));
                throw new MongoBulkOperationException("Bulk upsert failed for some documents.", failedOperations);
            }
        }
        return null;
    }

    public async Task UpsertAsyncByFilter(Expression<Func<TEntity, bool>> filterExpression, TEntity entity)
    {
        try
        {
            var existingDoc = await _context
                .GetCollection<TEntity>()
                .Find(filterExpression)
                .FirstOrDefaultAsync();
            if (existingDoc == null)
            {
                // Insert if the document does not exist, also generate _id
                var idProperty = typeof(TEntity).GetProperty("Id");
                idProperty?.SetValue(entity, ObjectId.GenerateNewId());
                await _context.GetCollection<TEntity>().InsertOneAsync(entity);
            }
            else
            {
                var updateDefinitions = new List<UpdateDefinition<TEntity>>();
                updateDefinitions = UpdateEntityDefinitions(updateDefinitions, entity);

                var update = Builders<TEntity>.Update.Combine(updateDefinitions);
                await _context.GetCollection<TEntity>().UpdateOneAsync(filterExpression, update);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, UpsertErrorMessage);
            throw new MongoRepositoryException(UpsertErrorMessage, ex);
        }
    }

    public async Task UpsertAsyncByFilterBsonDocument(Expression<Func<TEntity, bool>> filterExpression, TEntity entity)
    {
        try
        {
            var existingDoc = await _context
                .GetCollection<TEntity>()
                .Find(filterExpression)
                .FirstOrDefaultAsync();

            if (existingDoc == null)
            {
                var idProperty = typeof(TEntity).GetProperty("Id");
                idProperty?.SetValue(entity, ObjectId.GenerateNewId());
                await _context.GetCollection<TEntity>().InsertOneAsync(entity);
            }
            else
            {
                var bsonDoc = entity.ToBsonDocument();
                var updates = new List<UpdateDefinition<TEntity>>();

                foreach (var property in typeof(TEntity).GetProperties())
                {
                    var fieldInfo = GetMongoFieldInfo(property);
                    if (!fieldInfo.IsValid) continue;

                    var value = bsonDoc[fieldInfo.FieldName];
                    if (value == null) continue;

                    updates.Add(Builders<TEntity>.Update.Set(fieldInfo.FieldName, value));
                }

                if (updates.Any())
                {
                    var combinedUpdate = Builders<TEntity>.Update.Combine(updates);
                    await _context.GetCollection<TEntity>().UpdateOneAsync(filterExpression, combinedUpdate);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, UpsertErrorMessage);
            throw new MongoRepositoryException(UpsertErrorMessage, ex);
        }
    }

    public async Task UpsertBrokerInfoAsync(BrokerInfoRequest request)
    {
        try
        {
            foreach (var data in request.Data!)
                if (string.IsNullOrWhiteSpace(data.BrokerCode) ||
                    string.IsNullOrWhiteSpace(data.BrokerId))
                    throw new ValidationException("Broker code and ID are required");

            var _collection = _context.GetCollection<TEntity>();
            var bulkOperations = new List<WriteModel<BrokerInfo>>();

            foreach (var data in request.Data)
            {
                var filter = Builders<BrokerInfo>.Filter.And(
                    Builders<BrokerInfo>.Filter.Eq(x => x.BrokerCode, data.BrokerCode),
                    Builders<BrokerInfo>.Filter.Eq(x => x.BrokerId, data.BrokerId),
                    Builders<BrokerInfo>.Filter.Eq(x => x.AsOfDate, request.AsOfDate)
                );
                var update = Builders<BrokerInfo>.Update
                    .Set(x => x.BrokerCode, data.BrokerCode)
                    .Set(x => x.BrokerId, data.BrokerId)
                    .Set(x => x.UpdatedAt, DateTime.UtcNow)
                    .Set(x => x.IsActive, true)
                    .SetOnInsert(x => x.CreatedAt, DateTime.UtcNow);
                bulkOperations.Add(new UpdateOneModel<BrokerInfo>(filter, update)
                {
                    IsUpsert = true
                });
            }

            if (bulkOperations.Count > 0)
                await _collection.BulkWriteAsync((IEnumerable<WriteModel<TEntity>>)bulkOperations, new BulkWriteOptions
                {
                    IsOrdered = true
                });
        }
        catch (ValidationException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error upserting broker info data");
            throw new ApiException("Failed to upsert broker info data");
        }
    }

    public async Task ReplaceAsync(string id, TEntity entity)
    {
        try
        {
            if (ObjectId.TryParse(id, out var objectId))
            {
                var filter = Builders<TEntity>.Filter.Eq("_id", objectId);
                await _context.GetCollection<TEntity>().ReplaceOneAsync(filter, entity);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update entity.");
            throw new MongoRepositoryException("Failed to update entity.", ex);
        }
    }

    public async Task DeleteAsync(string id)
    {
        try
        {
            if (ObjectId.TryParse(id, out var objectId))
            {
                var filter = Builders<TEntity>.Filter.Eq("_id", objectId);
                await _context.GetCollection<TEntity>().DeleteOneAsync(filter);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete entity with id {Id}.", id);
            throw new MongoRepositoryException($"Failed to delete entity with id {id}.", ex);
        }
    }

    public async Task<TEntity?> GetByOrderBookId(string orderBookId)
    {
        try
        {
            if (!int.TryParse(orderBookId, out var id))
                return default;
            var filter = Builders<TEntity>.Filter.Eq("order_book_id", id);
            return await _context.GetCollection<TEntity>().Find(filter).FirstOrDefaultAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to get entity with orderBookId {OrderBookId}.",
                orderBookId
            );
            throw new MongoRepositoryException(
                $"Failed to get entity with orderBookId {orderBookId}.",
                ex
            );
        }
    }

    private static List<UpdateDefinition<TEntity>> UpdateEntityDefinitions(
        List<UpdateDefinition<TEntity>> updateDefinitions,
        TEntity entity)
    {
        foreach (var property in typeof(TEntity).GetProperties())
        {
            var fieldInfo = GetMongoFieldInfo(property);
            if (!fieldInfo.IsValid) continue;

            var propertyValue = property.GetValue(entity);
            if (propertyValue == null) continue;

            ProcessProperty(updateDefinitions, property, fieldInfo.FieldName, propertyValue);
        }

        return updateDefinitions;
    }

    private static (bool IsValid, string? FieldName) GetMongoFieldInfo(PropertyInfo property)
    {
        var bsonAttr = property.GetCustomAttribute<BsonElementAttribute>();
        if (bsonAttr == null) return (false, null);

        var fieldName = bsonAttr.ElementName;
        if (fieldName == "_id") return (false, null);

        return (true, fieldName);
    }

    private static void ProcessProperty(
        List<UpdateDefinition<TEntity>> updateDefinitions,
        PropertyInfo property,
        string fieldName,
        object propertyValue)
    {
        if (IsCollectionType(property.PropertyType))
            ProcessCollectionProperty(updateDefinitions, fieldName, propertyValue);
        else
            updateDefinitions.Add(Builders<TEntity>.Update.Set(fieldName, propertyValue));
    }

    private static void ProcessCollectionProperty(
        List<UpdateDefinition<TEntity>> updateDefinitions,
        string fieldName,
        object propertyValue)
    {
        if (propertyValue is IEnumerable<TEntity> enumerable)
        {
            var elements = enumerable.Cast<object>().ToList();
            if (elements.Any()) updateDefinitions.Add(Builders<TEntity>.Update.Set(fieldName, elements));
        }
    }

    private static bool IsCollectionType(Type type)
    {
        if (type == typeof(string))
            return false;

        if (type.IsGenericType)
        {
            var genericTypeDefinition = type.GetGenericTypeDefinition();
            if (genericTypeDefinition == typeof(List<>) ||
                genericTypeDefinition == typeof(ICollection<>) ||
                genericTypeDefinition == typeof(IEnumerable<>))
                return true;
        }

        foreach (var interfaceType in type.GetInterfaces())
            if (interfaceType.IsGenericType)
            {
                var interfaceGenericTypeDefinition = interfaceType.GetGenericTypeDefinition();
                if (interfaceGenericTypeDefinition == typeof(ICollection<>) ||
                    interfaceGenericTypeDefinition == typeof(IEnumerable<>))
                    return true;
            }

        return false;
    }
    
    private static PropertyInfo GetPropertyFromExpression<T>(Expression<Func<T, object>> selector)
    {
        return selector.Body switch
        {
            // Handle expressions like: x => (object)x.Id or x => x.SomeIntProperty
            UnaryExpression unaryExpr when unaryExpr.Operand is MemberExpression memberExpr =>
                (PropertyInfo)memberExpr.Member,

            // Handle expressions like: x => x.Id (when Id is already object/string)
            MemberExpression memberExpr => (PropertyInfo)memberExpr.Member,

            _ => throw new ArgumentException(
                $"Invalid expression type '{selector.Body.GetType().Name}' for idSelector. Expected property access expression.")
        };
    }
}
