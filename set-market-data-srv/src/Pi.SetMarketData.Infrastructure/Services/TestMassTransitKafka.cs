// using Confluent.Kafka;
// using MassTransit;
// using Microsoft.Extensions.DependencyInjection;
//
// namespace Pi.SetMarketData.Infrastructure.Services
// {
//     public class Program
//     {
//         public static async Task Main()
//         {
//             if (!CheckKafkaConnectivity("localhost:2181"))
//             {
//                 Console.WriteLine("Failed to connect to Kafka. Please check the Kafka server and configuration.");
//                 return;
//             }
//
//             var services = new ServiceCollection();
//
             // services.AddMassTransit(x =>
             // {
             //     x.UsingInMemory();
             //
             //     x.AddRider(rider =>
             //     {
             //         rider.AddProducer<KafkaMessage>("market_stock_data");
             //
             //         rider.UsingKafka((context, k) => { k.Host("localhost:2181"); });
             //     });
             // });
//
//             var provider = services.BuildServiceProvider();
//
//             var busControl = provider.GetRequiredService<IBusControl>();
//
//             try
//             {
//                 await busControl.StartAsync(new CancellationTokenSource(TimeSpan.FromSeconds(10)).Token);
//                 var producer = provider.GetRequiredService<ITopicProducer<KafkaMessage>>();
//
//                 do
//                 {
//                     string? value = await Task.Run(() =>
//                     {
//                         Console.WriteLine("Enter text (or quit to exit)");
//                         Console.Write("> ");
//                         return Console.ReadLine();
//                     });
//
//                     if ("quit".Equals(value, StringComparison.OrdinalIgnoreCase))
//                         break;
//
//                     try
//                     {
//                         await producer.Produce(new KafkaMessage { Text = value });
//                     }
//                     catch (Exception ex)
//                     {
//                         Console.WriteLine($"Error producing message: {ex.Message}");
//                     }
//                 } while (true);
//             }
//             catch (Exception ex)
//             {
//                 Console.WriteLine($"An error occurred: {ex.Message}");
//             }
//             finally
//             {
//                 await busControl.StopAsync();
//             }
//         }
//
//         private static bool CheckKafkaConnectivity(string kafkaConnectionString)
//         {
//             try
//             {
//                 var config = new ProducerConfig { BootstrapServers = kafkaConnectionString };
//                 using var producer = new ProducerBuilder<Null, string>(config).Build();
//                 producer.ProduceAsync("connection-test", new Message<Null, string> { Value = "test" }).GetAwaiter().GetResult();
//                 return true;
//             }
//             catch (Exception ex)
//             {
//                 Console.WriteLine($"Kafka connection check failed: {ex.Message}");
//                 return false;
//             }
//         }
//
//         public record KafkaMessage
//         {
//             public string? Text { get; init; }
//         }
//     }
// }