using Microsoft.Extensions.Logging;
using Pi.SetMarketDataRealTime.Application.Interfaces.MemoryCache;
using Pi.SetMarketDataRealTime.Application.Services.Models.ItchParser;

namespace Pi.SetMarketDataRealTime.Application.Services.ItchParser;

public class ItchMessageMetadataHandler
{
    private const long NanosecondsPerSecond = 1_000_000_000;
    private readonly ILogger<ItchMessageMetadataHandler> _logger;
    private readonly IMemoryCacheHelper _memoryCacheHelper;
    private uint _lastSecond;
    private long _lastTimestamp;

    /// <inheritdoc>
    ///     <cref></cref>
    /// </inheritdoc>
    public ItchMessageMetadataHandler(IMemoryCacheHelper memoryCacheHelper, ILogger<ItchMessageMetadataHandler> logger)
    {
        _memoryCacheHelper = memoryCacheHelper ?? throw new ArgumentNullException(nameof(memoryCacheHelper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task AssignMetadataAsync(ItchMessage message, string orderBookId)
    {
        ArgumentNullException.ThrowIfNull(message);

        try
        {
            // Skip metadata assignment for duplicate SecondsMessages
            if (!await ShouldAssignMetadataAsync(message)) return;

            await UpdateTimestampAsync(message);

            // Get the init values for local memory
            var currentSession = await _memoryCacheHelper.GetCurrentSessionAsync();
            var currentItchSequenceNo = await _memoryCacheHelper.GetCurrentItchSequenceNoAsync();

            message.Metadata = new ItchMessageMetadata
            {
                Timestamp = _lastTimestamp,
                Session = currentSession,
                SequenceNumber = currentItchSequenceNo,
                OrderBookId = orderBookId
            };

            // Set next ItchSequenceNumber and update next GlimpseSequenceNumber to local memory
            var nextSequenceNumber = currentItchSequenceNo + 1;
            await _memoryCacheHelper.SetCurrentItchSequenceNoAsync(nextSequenceNumber);
            await _memoryCacheHelper.SetCurrentGlimpseSequenceNoAsync(nextSequenceNumber);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{Message}", ex.Message);
        }
    }

    private async Task UpdateTimestampAsync(ItchMessage message)
    {
        try
        {
            if (message is SecondsMessage secondsMessage)
            {
                _lastSecond = secondsMessage.Second.Value;
                _lastTimestamp = secondsMessage.Second.Value * NanosecondsPerSecond;
                await _memoryCacheHelper.SetCurrentItchLastSecondAsync(_lastSecond.ToString());
            }
            else if (message.Nanos.Value > 0)
            {
                var lastSecondCache = await _memoryCacheHelper.GetCurrentItchLastSecondAsync();
                _lastTimestamp = string.IsNullOrEmpty(lastSecondCache)
                    ? message.Nanos.Value
                    : uint.Parse(lastSecondCache) * NanosecondsPerSecond + message.Nanos.Value;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{Message}", ex.Message);
        }
    }

    private async Task<bool> ShouldAssignMetadataAsync(ItchMessage message)
    {
        try
        {
            // Always assign metadata for non-SecondsMessage types
            if (message is not SecondsMessage secondsMessage)
                return true;

            var lastSecondCache = await _memoryCacheHelper.GetCurrentItchLastSecondAsync();

            // If there's no cached last second, we should assign metadata
            if (string.IsNullOrEmpty(lastSecondCache))
                return true;

            // Parse the cached last second and compare with the current message's second
            if (uint.TryParse(lastSecondCache, out var lastSecond)) return secondsMessage.Second.Value != lastSecond;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{Message}", ex.Message);
            return true;
        }

        // If parsing fails, we should assign metadata to be safe
        return true;
    }
}