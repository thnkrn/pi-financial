using Pi.SetService.Application.Exceptions;

namespace Pi.SetService.Application.Extensions;

public static class NullableExtensions
{
    public static T GetRequiredValue<T>(this T? value, string propertyName) where T : struct
    {
        if (value == null)
        {
            throw new RequiredFieldNotfoundException(propertyName);
        }

        return value.Value;
    }
}
