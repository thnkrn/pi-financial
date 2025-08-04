namespace Pi.StructureNotes.Worker.Startup;

public static class HttpClientRegistrationExtensions
{
    public static IServiceCollection AddHttpClients(this IServiceCollection services, IConfiguration configuration) =>
        services;
}
