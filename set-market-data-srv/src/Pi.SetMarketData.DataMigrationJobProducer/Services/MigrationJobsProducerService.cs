using Confluent.Kafka;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Pi.SetMarketData.Domain.ConstantConfigurations;
using Pi.SetMarketData.Infrastructure.Interfaces.Kafka;
using Pi.SetMarketData.DataMigrationJobProducer.Entities;
using Pi.SetMarketData.DataMigrationJobProducer.interfaces;
using Pi.SetMarketData.Infrastructure.Helpers;
using Pi.SetMarketData.Domain.Models.Indicator;

namespace Pi.SetMarketData.DataMigrationJobProducer.Services;

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
                    var job = CreateMigrationJob(symbol, venue, date);
                    var jobMessage = JsonConvert.SerializeObject(job);
                    var deliveryResult = await _publisher.PublishAsync(_topicName, new Message<string, string> { Value = jobMessage });

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

    private MigrationJob CreateMigrationJob(string symbol, string venue, DateTime date)
    {
        return new MigrationJob
        {
            Symbol = symbol,
            Venue = venue,
            DateTimeFrom = date,
            DateTimeTo = date.AddDays(MigrationWindowInDays).AddSeconds(-1) // End of the day
        };
    }

    private void LogJobProduction(MigrationJob job, DeliveryResult<string, string> deliveryResult)
    {
        _logger.LogInformation("Produced job for {Symbol} from {DateTimeFrom:yyyy-MM-dd HH:mm:ss} to {DateTimeTo:yyyy-MM-dd HH:mm:ss}", job.Symbol, job.DateTimeFrom, job.DateTimeTo);
        _logger.LogInformation("Delivered '{Value}' to '{TopicPartitionOffset}' with key '{Key}'", deliveryResult.Value, deliveryResult.TopicPartitionOffset, deliveryResult.Key);
    }

    public async Task ProduceIndicatorsMigrationJobsAsync(DateTime migrationDateFrom, DateTime migrationDateTo, string venue, string[] stockSymbols, string timeframe)
    {
        var jobsCount = 0;
        var indicatorMigrationTopic = "Migration.Indicator.Job";
        try
        {
            foreach (var s in stockSymbols)
            {
                var symbol = s.Trim();
                for (var date = migrationDateFrom; date <= migrationDateTo;)
                {
                    DateTime nextBatchEnd;

                    if (date.AddMonths(1) <= migrationDateTo) 
                    {
                        nextBatchEnd = date.AddMonths(1);
                    }
                    else if (date.AddDays(7) <= migrationDateTo) 
                    {
                        nextBatchEnd = date.AddDays(7);
                    }
                    else 
                    {
                        nextBatchEnd = date.AddDays(1);
                    }

                    var job = new IndicatorMigrationMessage
                    {
                        Venue = venue,
                        Symbol = symbol,
                        DateTimeFrom = date,
                        DateTimeTo = nextBatchEnd,
                        Timeframe = timeframe
                    };
    
                    var jobMessage = JsonConvert.SerializeObject(job);
                    var deliveryResult = await _publisher.PublishAsync(indicatorMigrationTopic, new Message<string, string> { Value = jobMessage });

                    LogIndicatorJobProduction(job, deliveryResult);
                    jobsCount++;

                    date = nextBatchEnd;
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

    private void LogIndicatorJobProduction(IndicatorMigrationMessage job, DeliveryResult<string, string> deliveryResult)
    {
        _logger.LogInformation("Produced indicator job for {Symbol} from {DateTimeFrom:yyyy-MM-dd HH:mm:ss} to {DateTimeTo:yyyy-MM-dd HH:mm:ss}", job.Symbol, job.DateTimeFrom, job.DateTimeTo);
        _logger.LogInformation("Delivered '{Value}' to '{TopicPartitionOffset}' with key '{Key}'", deliveryResult.Value, deliveryResult.TopicPartitionOffset, deliveryResult.Key);
    }
}
