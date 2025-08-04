using Polly;

namespace Pi.PortfolioService.API.Startup
{
    public static class HttpClientRegistrationExtensions
    {
        public static IServiceCollection AddHttpClients(
            this IServiceCollection services,
            ConfigurationManager configuration
        )
        {
            services
                .AddHttpClient("Sirius")
                .ConfigureHttpClient(
                    (sp, client) =>
                    {
                        client.Timeout = TimeSpan.FromSeconds(30);
                    }
                )
                .SetHandlerLifetime(TimeSpan.FromMinutes(5))
                .AddTransientHttpErrorPolicy(
                    builder =>
                        builder.WaitAndRetryAsync(
                            new[]
                            {
                                TimeSpan.FromSeconds(0.5),
                                TimeSpan.FromSeconds(0.5),
                                TimeSpan.FromSeconds(0.5)
                            }
                        )
                );

            services
                .AddHttpClient("StructureNote")
                .ConfigureHttpClient(
                    (sp, client) =>
                    {
                        client.Timeout = TimeSpan.FromSeconds(30);
                    }
                )
                .SetHandlerLifetime(TimeSpan.FromMinutes(5))
                .AddTransientHttpErrorPolicy(
                    builder =>
                        builder.WaitAndRetryAsync(
                            new[]
                            {
                                TimeSpan.FromSeconds(0.5),
                                TimeSpan.FromSeconds(0.5),
                                TimeSpan.FromSeconds(0.5)
                            }
                        )
                );

            services
                .AddHttpClient("GrowthBook")
                .ConfigureHttpClient(
                    (_, client) =>
                    {
                        client.BaseAddress = new Uri(
                            configuration.GetValue<string>("GrowthBook:Host")!
                        );
                        client.Timeout = TimeSpan.FromSeconds(30);
                    }
                )
                .SetHandlerLifetime(TimeSpan.FromMinutes(5))
                .AddTransientHttpErrorPolicy(
                    builder =>
                        builder.WaitAndRetryAsync(
                            new[]
                            {
                                TimeSpan.FromSeconds(0.5),
                                TimeSpan.FromSeconds(0.5),
                                TimeSpan.FromSeconds(0.5)
                            }
                        )
                );

            services
                .AddHttpClient("Fund")
                .ConfigureHttpClient(
                    (_, client) =>
                    {
                        client.Timeout = TimeSpan.FromSeconds(30);
                    }
                )
                .SetHandlerLifetime(TimeSpan.FromMinutes(5))
                .AddTransientHttpErrorPolicy(
                    builder =>
                        builder.WaitAndRetryAsync(
                            new[]
                            {
                                TimeSpan.FromSeconds(0.5),
                                TimeSpan.FromSeconds(0.5),
                                TimeSpan.FromSeconds(0.5)
                            }
                        )
                );

            services
                .AddHttpClient("Set")
                .ConfigureHttpClient(
                    (_, client) =>
                    {
                        client.Timeout = TimeSpan.FromSeconds(30);
                    }
                )
                .SetHandlerLifetime(TimeSpan.FromMinutes(5))
                .AddTransientHttpErrorPolicy(
                    builder =>
                        builder.WaitAndRetryAsync(
                            new[]
                            {
                                TimeSpan.FromSeconds(0.5),
                                TimeSpan.FromSeconds(0.5),
                                TimeSpan.FromSeconds(0.5)
                            }
                        )
                );

            services
                .AddHttpClient("GrowthBook")
                .ConfigureHttpClient(
                    (_, client) =>
                    {
                        client.BaseAddress = new Uri(configuration.GetValue<string>("GrowthBook:Host")!);
                        client.Timeout = TimeSpan.FromSeconds(30);
                    }
                )
                .SetHandlerLifetime(TimeSpan.FromMinutes(5))
                .AddTransientHttpErrorPolicy(
                    builder =>
                        builder.WaitAndRetryAsync(
                            new[]
                            {
                                TimeSpan.FromSeconds(0.5),
                                TimeSpan.FromSeconds(0.5),
                                TimeSpan.FromSeconds(0.5)
                            }
                        )
                );

            services
                .AddHttpClient("User")
                .ConfigureHttpClient(
                    (_, client) =>
                    {
                        client.BaseAddress = new Uri(configuration.GetValue<string>("User:Host")!);
                        client.Timeout = TimeSpan.FromSeconds(30);
                    }
                )
                .SetHandlerLifetime(TimeSpan.FromMinutes(5))
                .AddTransientHttpErrorPolicy(
                    builder =>
                        builder.WaitAndRetryAsync(
                            new[]
                            {
                                TimeSpan.FromSeconds(0.5),
                                TimeSpan.FromSeconds(0.5),
                                TimeSpan.FromSeconds(0.5)
                            }
                        )
                );

            services.AddHttpClient("UserServiceV2")
                .ConfigureHttpClient(
                    (_, client) =>
                    {
                        client.BaseAddress = new Uri(configuration.GetValue<string>("UserV2:Host")!);
                        client.Timeout = TimeSpan.FromSeconds(30);
                    }
                )
                .SetHandlerLifetime(TimeSpan.FromMinutes(5))
                .AddTransientHttpErrorPolicy(
                    builder =>
                        builder.WaitAndRetryAsync(
                            new[]
                            {
                                TimeSpan.FromSeconds(0.5),
                                TimeSpan.FromSeconds(0.5),
                                TimeSpan.FromSeconds(0.5)
                            }
                        )
                );

            return services;
        }
    }
}
