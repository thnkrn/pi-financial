// using MassTransit;
// using MassTransit.KafkaIntegration;
// using Microsoft.Extensions.DependencyInjection;
//
// namespace Pi.SetMarketData.Infrastructure.Services;
//
// public class KafkaMessagePublisher
// {
//     public KafkaMessagePublisher()
//     {
//         var services = new ServiceCollection();
//
//         services.AddMassTransit(x =>
//         {
//             x.UsingInMemory();
//             x.AddRider(rider =>
//             {
//                 rider.AddConsumer<KafkaMessageConsumer>();
//                 rider.UsingKafka((context, k) =>
//                 {
//                     k.Host("localhost:9092");
//                     k.TopicEndpoint<KafkaMessage>("topic-name", "consumer-group-name", e =>
//                     {
//                         e.ConfigureConsumer<KafkaMessageConsumer>(context);
//                     });
//                 });
//             });
//         });
//     }
//
//     public void PublishMessage(Object message)
//     {
//         
//     }
// }