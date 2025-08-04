using System.Collections;

namespace Pi.GlobalEquities.Models;

public class PropertyChange
{
    public string Name { get; }
    public object OldValue { get; }
    public object NewValue { get; }
    public Type PropertyType { get; }

    protected PropertyChange(string name, Type type, object oldValue, object newValue)
    {
        Name = name;
        PropertyType = type;
        OldValue = oldValue;
        NewValue = newValue;
    }

    public static PropertyChange<T> For<T>(string name, T oldValue, T newValue) =>
        new PropertyChange<T>(name, oldValue, newValue);

    public static bool CheckChange<T>(string name, IEnumerable<T> oldValue, IEnumerable<T> newValue,
        IList<PropertyChange> changes)
    {
        if (oldValue is null && newValue is null)
            return false;

        var oldEnumerable = oldValue ?? Enumerable.Empty<T>();
        var newEnumerable = newValue ?? Enumerable.Empty<T>();
        if (oldEnumerable.SequenceEqual(newEnumerable))
            return false;

        var pc = PropertyChange.For(name, oldValue, newValue);
        changes.Add(pc);
        return true;
    }

    public static bool CheckChange<T>(string name, T oldValue, T newValue, IList<PropertyChange> changes)
    {
        if (typeof(IEnumerable).IsAssignableFrom(typeof(T)) && typeof(T) != typeof(string))
        {
            throw new ArgumentException($"Type T must not be a collection type, but found {typeof(T)}.");
        }

        if (EqualityComparer<T>.Default.Equals(oldValue, newValue))
            return false;

        var pc = PropertyChange.For(name, oldValue, newValue);
        changes.Add(pc);
        return true;
    }
}

public class PropertyChange<T> : PropertyChange
{
    public PropertyChange(string name, T oldValue, T newValue)
        : base(name, typeof(T), oldValue, newValue)
    {
    }
}
