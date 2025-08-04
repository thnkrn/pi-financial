namespace Pi.SetService.Application.Exceptions;

public class RequiredFieldNotfoundException(string propertyName) : Exception($"Property '{propertyName}' cannot be null.");
