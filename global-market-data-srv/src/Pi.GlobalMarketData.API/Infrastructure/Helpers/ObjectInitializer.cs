using System.Collections;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Pi.GlobalMarketData.API.Infrastructure.Helpers;

/// <summary>
///     Provides functionality to dynamically initialize objects with default values.
/// </summary>
public static class ObjectInitializer
{
    private const int MaxRecursionDepth = 10;

    /// <summary>
    ///     Initializes an object of type T with default values.
    /// </summary>
    /// <typeparam name="T">The type of object to initialize</typeparam>
    /// <param name="options">Initialization options</param>
    /// <returns>An initialized instance of type T</returns>
    /// <exception cref="ArgumentNullException">Thrown when options is null after default initialization</exception>
    public static T InitializeObject<T>(InitializerOptions? options = null) where T : class
    {
        options ??= new InitializerOptions();
        ArgumentNullException.ThrowIfNull(options);

        var obj = Activator.CreateInstance(typeof(T)) ??
                  throw new InvalidOperationException($"Cannot create an instance of {typeof(T)}");
        InitializeProperties(obj, options, [], 0);
        return (T)obj;
    }

    private static void InitializeProperties(object obj, InitializerOptions options,
        HashSet<object> initializedObjects, int depth)
    {
        if (depth > MaxRecursionDepth || !initializedObjects.Add(obj)) return;

        var type = obj.GetType();
        var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.CanWrite && p.GetIndexParameters().Length == 0);

        foreach (var property in properties)
            InitializeProperty(obj, property, options, initializedObjects, type, depth);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void InitializeProperty(object obj, PropertyInfo property,
        InitializerOptions options, HashSet<object> initializedObjects,
        Type type, int depth)
    {
        try
        {
            var propertyType = property.PropertyType;

            if (IsCollectionType(propertyType))
            {
                InitializeCollection(obj, property, options, initializedObjects, depth);
            }
            else if (IsComplexType(propertyType))
            {
                InitializeComplexType(obj, property, options, initializedObjects, type, depth);
            }
            else
            {
                var value = GetDefaultValue(propertyType);
                property.SetValue(obj, value);
            }
        }
        catch (Exception ex) when (ex is not OutOfMemoryException)
        {
            HandleInitializationError(property, ex, options);
        }
    }

    private static bool IsCollectionType(Type type)
    {
        return typeof(IEnumerable).IsAssignableFrom(type) && type != typeof(string);
    }

    private static bool IsComplexType(Type type)
    {
        return !type.IsValueType && type != typeof(string);
    }

    private static void InitializeComplexType(object obj, PropertyInfo property,
        InitializerOptions options, HashSet<object> initializedObjects,
        Type parentType, int depth)
    {
        var propertyType = property.PropertyType;
        if (propertyType.Assembly != parentType.Assembly) return;

        try
        {
            var complexObject = Activator.CreateInstance(propertyType);
            if (complexObject != null)
            {
                InitializeProperties(complexObject, options, initializedObjects, depth + 1);
                property.SetValue(obj, complexObject);
            }
        }
        catch (Exception ex) when (ex is not OutOfMemoryException)
        {
            HandleInitializationError(property, ex, options);
        }
    }

    private static void InitializeCollection(object obj, PropertyInfo property,
        InitializerOptions options, HashSet<object> initializedObjects, int depth)
    {
        var propertyType = property.PropertyType;
        var elementType = propertyType.IsGenericType
            ? propertyType.GetGenericArguments()[0]
            : typeof(object);

        try
        {
            var collectionType = typeof(List<>).MakeGenericType(elementType);

            if (Activator.CreateInstance(collectionType) is not IList collection) return;

            for (var i = 0; i < options.CollectionSize; i++)
            {
                var item = CreateCollectionItem(obj, elementType, options, initializedObjects, depth);
                if (item != null) collection.Add(item);
            }

            property.SetValue(obj, collection);
        }
        catch (Exception ex) when (ex is not OutOfMemoryException)
        {
            HandleInitializationError(property, ex, options);
        }
    }

    private static object? CreateCollectionItem(object obj, Type elementType,
        InitializerOptions options, HashSet<object> initializedObjects, int depth)
    {
        if (!elementType.IsValueType && elementType != typeof(string))
        {
            if (elementType.Assembly != obj.GetType().Assembly) return null;

            var item = Activator.CreateInstance(elementType);
            if (item != null)
            {
                InitializeProperties(item, options, initializedObjects, depth + 1);
                return item;
            }
        }

        return GetDefaultValue(elementType);
    }

    private static object? GetDefaultValue(Type type)
    {
        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            type = Nullable.GetUnderlyingType(type) ?? type;

        return type switch
        {
            { } t when t == typeof(string) => string.Empty,
            { } t when t == typeof(int) => 0,
            { } t when t == typeof(long) => 0L,
            { } t when t == typeof(double) => 0.0,
            { } t when t == typeof(decimal) => 0M,
            { } t when t == typeof(DateTime) => DateTime.MinValue,
            { } t when t == typeof(DateTimeOffset) => DateTimeOffset.MinValue,
            { } t when t == typeof(bool) => false,
            { } t when t == typeof(Guid) => Guid.Empty,
            { IsEnum: true } t => Enum.GetValues(t).Cast<object>().FirstOrDefault(),
            { IsValueType: true } t => Activator.CreateInstance(t),
            _ => null
        };
    }

    private static void HandleInitializationError(MemberInfo property, Exception ex, InitializerOptions options)
    {
        var errorMessage = $"Error initializing property {property.Name}: {ex.Message}";
        options.ErrorHandler?.Invoke(errorMessage);
    }
}

/// <summary>
///     Options for controlling object initialization behavior.
/// </summary>
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
public sealed class InitializerOptions
{
    /// <summary>
    ///     Gets or sets the number of items to create in collection properties.
    ///     Default is 1.
    /// </summary>
    public int CollectionSize { get; init; } = 1;

    /// <summary>
    ///     Gets or sets the error handler callback.
    /// </summary>
    public Action<string>? ErrorHandler { get; init; }
}