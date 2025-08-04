using System.Globalization;
using System.Net.Sockets;
using Pi.SetMarketDataRealTime.DataHandler.Models;
using Pi.SetMarketDataRealTime.Infrastructure.Interfaces.SoupBinTcp;
using Pi.SetMarketDataRealTime.Infrastructure.Models.SoupBinTcp;
using Pi.SetMarketDataRealTime.Infrastructure.Models.SoupBinTcp.Messages;
using Polly;

namespace Pi.SetMarketDataRealTime.DataHandler.Helpers;

public static class ClientSubscriptionServiceHelper
{
    private const string IanaTimeZoneId = "Asia/Bangkok";
    private const string WindowsTimeZoneId = "SE Asia Standard Time";
    private static readonly CultureInfo CultureInformation = new("en-US");

    public static TimeZoneInfo ThailandTimeZone { get; } = GetThailandTimeZone();

    private static TimeZoneInfo GetThailandTimeZone()
    {
        try
        {
            // Try IANA ID first (for non-Windows systems)
            return TimeZoneInfo.FindSystemTimeZoneById(IanaTimeZoneId);
        }
        catch
        {
            try
            {
                // Fallback to Windows ID
                return TimeZoneInfo.FindSystemTimeZoneById(WindowsTimeZoneId);
            }
            catch (TimeZoneNotFoundException)
            {
                // If both attempts fail, throw a more informative exception
                throw new TimeZoneNotFoundException(
                    $"Unable to find time zone for Thailand. Neither '{IanaTimeZoneId}' nor '{WindowsTimeZoneId}' were recognized.");
            }
        }
    }

    public static bool IsEquals(DateTime time1, DateTime time2)
    {
        return TruncateSeconds(time1) == TruncateSeconds(time2);
    }

    public static (bool IsWithinValidTimeRange, bool HasMissedStart, bool HasMissedStop) EvaluateSchedule(
        DateTime thaiNow,
        DateTime nextOpenRun,
        DateTime nextCloseRun,
        bool isSubscriptionRunning)
    {
        // Truncate seconds for consistency
        thaiNow = TruncateSeconds(thaiNow);
        nextOpenRun = TruncateSeconds(nextOpenRun);
        nextCloseRun = TruncateSeconds(nextCloseRun);

        bool isWithinValidTimeRange;
        bool hasMissedStart;
        bool hasMissedStop;

        // Determine if the schedule crosses midnight
        var crossesMidnight = nextCloseRun < nextOpenRun;

        if (crossesMidnight)
        {
            // For schedules that cross midnight (e.g., 22:00 - 04:00)
            isWithinValidTimeRange = thaiNow >= nextOpenRun || thaiNow < nextCloseRun;

            // Missed start if we're in the valid time range but not running
            hasMissedStart = isWithinValidTimeRange && !isSubscriptionRunning;

            // Missed stop if we're in the gap between close and next open
            hasMissedStop = !isWithinValidTimeRange && isSubscriptionRunning;
        }
        else
        {
            // For schedules within the same day (e.g., 09:00 - 17:00)
            isWithinValidTimeRange = thaiNow >= nextOpenRun && thaiNow < nextCloseRun;

            // Missed start if we're in the valid time range but not running
            hasMissedStart = isWithinValidTimeRange && !isSubscriptionRunning;

            // Missed stop if we're past the closing time
            hasMissedStop = !isWithinValidTimeRange && isSubscriptionRunning;
        }

        return (isWithinValidTimeRange, hasMissedStart, hasMissedStop);
    }

    public static (bool IsWithinValidTimeRange, bool HasMissedStart, bool HasMissedStop) EvaluateSchedule(
        DateTime thaiNow, DateTime nextOpenRun, DateTime nextCloseRun)
    {
        // Truncate seconds for all DateTime objects
        thaiNow = TruncateSeconds(thaiNow);
        nextOpenRun = TruncateSeconds(nextOpenRun);
        nextCloseRun = TruncateSeconds(nextCloseRun);

        bool isWithinValidTimeRange;
        bool hasMissedStart;
        bool hasMissedStop;

        // If nextCloseRun is less than nextOpenRun, it indicates a time range that spans across midnight
        if (nextCloseRun < nextOpenRun)
        {
            isWithinValidTimeRange = thaiNow >= nextOpenRun || thaiNow < nextCloseRun;
            hasMissedStart = thaiNow > nextOpenRun && thaiNow < nextCloseRun;
            hasMissedStop = thaiNow >= nextCloseRun && thaiNow < nextOpenRun;
        }
        else
        {
            isWithinValidTimeRange = thaiNow >= nextOpenRun && thaiNow < nextCloseRun;
            hasMissedStart = thaiNow > nextOpenRun && thaiNow < nextCloseRun;
            hasMissedStop = thaiNow >= nextCloseRun || thaiNow < nextOpenRun;
        }

        return (isWithinValidTimeRange, hasMissedStart, hasMissedStop);
    }

    public static DateTime GetCurrentThailandTime()
    {
        return TruncateSeconds(TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, ThailandTimeZone));
    }

    public static string FormatThaiDateTime(DateTime thaiDateTime)
    {
        return thaiDateTime.ToString("yyyy-MM-dd HH:mm:ss", CultureInformation);
    }

    public static DateTime ConvertToThailandTime(DateTime utcDateTime)
    {
        return TruncateSeconds(TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, ThailandTimeZone));
    }

    public static DateTime ConvertFromThailandTime(DateTime thaiDateTime, TimeZoneInfo targetTimeZone)
    {
        return TruncateSeconds(TimeZoneInfo.ConvertTime(thaiDateTime, ThailandTimeZone, targetTimeZone));
    }

    public static async Task<bool> QuickConnectivityCheck(string ipAddress, int port, ILogger logger)
    {
        using var tcpClient = new TcpClient();
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        try
        {
            await tcpClient.ConnectAsync(ipAddress, port, cts.Token);
            return true;
        }
        catch (OperationCanceledException ex)
        {
            logger.LogWarning(ex, "Quick connectivity check timed out for {IpAddress}:{Port}", ipAddress, port);
            return false;
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Quick connectivity check failed for {IpAddress}:{Port}", ipAddress, port);
            return false;
        }
    }

    public static LoginDetails CreateLoginDetails(Gateway gateway, string gatewayType, ulong sequenceNumber = 1)
    {
        if (string.IsNullOrEmpty(gateway.USER))
            throw new InvalidOperationException($"{gatewayType} Gateway User is null or empty");

        if (string.IsNullOrEmpty(gateway.PASSWORD))
            throw new InvalidOperationException($"{gatewayType} Gateway Password is null or empty");

        return new LoginDetails
        {
            UserName = gateway.USER,
            Password = gateway.PASSWORD,
            RequestedSequenceNumber = sequenceNumber
        };
    }

    public static DateTime TruncateSeconds(DateTime dateTime)
    {
        return new DateTime(
            dateTime.Year,
            dateTime.Month,
            dateTime.Day,
            dateTime.Hour,
            dateTime.Minute,
            0,
            dateTime.Kind
        );
    }

    public static IAsyncPolicy CreateRetryPolicy(ILogger logger)
    {
        return Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(1, retryAttempt =>
                    TimeSpan.FromSeconds(Math.Pow(2, retryAttempt) + new Random().NextDouble()),
                (exception, timeSpan, retryCount, _) =>
                {
                    logger.LogWarning(exception,
                        "Attempt {RetryCount} failed. Waiting {TimeSpan} before next retry.",
                        retryCount, timeSpan);
                });
    }

    public static async Task SafeLogout(IClient? client, ILogger logger, CancellationToken stoppingToken = default)
    {
        try
        {
            if (client != null)
                await client.ShutdownAsync(stoppingToken);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex,
                "Error during client logout. This may be normal if the client was already logged out.");
        }
    }

    public static async Task RunLocalClientAsync(IClient client, CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var cmd = Console.ReadLine();
            if (string.IsNullOrEmpty(cmd)) continue;
            var message = cmd.ToLower() switch
            {
                "d" => new Debug("test debug"),
                "o" => await HandleLogout(client, stoppingToken),
                "s" => new UnSequencedData("request data"u8.ToArray()),
                _ => null
            };

            if (message != null) await client.SendAsync(message.Bytes, stoppingToken);
        }
    }

    public static Task RunProductionClientAsync(IClient client, Func<Task> stopSubscriptionTask, ILogger logger,
        CancellationToken stoppingToken)
    {
        return Task.Run(async () =>
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var cmd = Console.ReadLine();
                if (string.IsNullOrEmpty(cmd)) continue;
                Message? message;
                switch (cmd.ToLower())
                {
                    case "d":
                        message = new Debug("test debug");
                        break;
                    case "o":
                        message = new LogoutRequest();
                        await client.SendAsync(message.Bytes, stoppingToken);
                        await stopSubscriptionTask();
                        logger.LogWarning("The system logs out completely and terminates the session on command!");
                        return;
                    case "s":
                        message = new UnSequencedData("request data"u8.ToArray());
                        break;
                    default:
                        message = null;
                        break;
                }

                if (message != null) await client.SendAsync(message.Bytes, stoppingToken);
            }
        }, stoppingToken);
    }

    private static async Task<Message> HandleLogout(IClient client, CancellationToken stoppingToken)
    {
        await client.ShutdownAsync(stoppingToken);
        client.Dispose();
        await Task.Delay(TimeSpan.FromMilliseconds(100), stoppingToken);
        return new LogoutRequest();
    }
}