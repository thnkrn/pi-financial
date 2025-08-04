using Confluent.Kafka;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Pi.GlobalMarketData.Domain.ConstantConfigurations;
using Pi.GlobalMarketData.Infrastructure.Interfaces.Kafka;
using Pi.GlobalMarketData.DataMigrationJobProducer.Entities;
using Pi.GlobalMarketData.DataMigrationJobProducer.interfaces;
using Pi.GlobalMarketData.Infrastructure.Helpers;

namespace Pi.GlobalMarketData.DataMigrationJobProducer.Services;

public class MigrationJobsProducerService : IMigrationJobsProducerService
{
    private readonly string _topicName;
    private const int MigrationWindowInDays = 7;
    private readonly ILogger<MigrationJobsProducerService> _logger;
    private readonly IKafkaPublisher<string, string> _publisher;

    public MigrationJobsProducerService(
        IConfiguration configuration,
        ILogger<MigrationJobsProducerService> logger,
        IKafkaPublisher<string, string> publisher
    )
    {
        _publisher = publisher;
        _topicName = ConfigurationHelper.GetTopicList(configuration, ConfigurationKeys.KafkaTopic).FirstOrDefault()
            ?? throw new InvalidOperationException("KafkaTopic is not configured.");
        _logger = logger;
    }

    public async Task ProduceMigrationJobsAsync(DateTime migrationDateFrom, DateTime migrationDateTo, string venue, string[] stockSymbols)
    {
        var jobsCount = 0;
        try
        {
            foreach (var s in stockSymbols)
            {
                var symbol = s.Trim();
                for (var date = migrationDateFrom; date <= migrationDateTo; date = date.AddDays(MigrationWindowInDays))
                {
                    var from = date;
                    var to = date.AddDays(MigrationWindowInDays).AddSeconds(-1);
                    if (to.Date >= migrationDateTo.Date)
                    {
                        to = migrationDateTo;
                    }
                    var job = CreateMigrationJob(symbol, venue, from, to);
                    var jobMessage = JsonConvert.SerializeObject(job);
                    var deliveryResult = await _publisher.PublishAsync(_topicName, new Message<string, string> { Key = symbol, Value = jobMessage });

                    LogJobProduction(job, deliveryResult);
                    jobsCount++;
                }
            }
        }
        catch (ProduceException<string, string> e)
        {
            _logger.LogInformation(e, "Delivery failed: {Reason}", e.Error.Reason);
        }
        finally
        {
            _logger.LogInformation("Total {JobsCount} jobs sent", jobsCount);
        }
    }

    private MigrationJob CreateMigrationJob(string symbol, string venue, DateTime dateFrom, DateTime dateTo)
    {
        return new MigrationJob
        {
            Symbol = symbol,
            Venue = venue,
            DateTimeFrom = dateFrom,
            DateTimeTo = dateTo
        };
    }

    private void LogJobProduction(MigrationJob job, DeliveryResult<string, string> deliveryResult)
    {
        _logger.LogInformation("Produced job for {Symbol} from {DateTimeFrom:yyyy-MM-dd HH:mm:ss} to {DateTimeTo:yyyy-MM-dd HH:mm:ss}", job.Symbol, job.DateTimeFrom, job.DateTimeTo);
        _logger.LogInformation("Delivered '{Value}' to '{TopicPartitionOffset}' with key '{Key}'", deliveryResult.Value, deliveryResult.TopicPartitionOffset, deliveryResult.Key);
    }
}
