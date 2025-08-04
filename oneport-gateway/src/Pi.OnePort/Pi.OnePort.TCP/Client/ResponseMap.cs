using System.Collections.Concurrent;
using Pi.OnePort.TCP.Extensions;
using Pi.OnePort.TCP.Models;

namespace Pi.OnePort.TCP.Client;

public interface IResponseMap
{
    bool AddKey(string key);
    bool UpdateKey(string key, Packet? packet);
    bool TryGetValue(string key, out Packet? value);
    bool TryRemove(string key, out Packet? value);
    Task<Packet?> GetAndRemoveWithWaitingAsync(string key, CancellationToken ct);
}

public class ResponseMap : IResponseMap
{
    private ConcurrentDictionary<string, Packet?> Responses { get; set; }
    private int Timeout { get; set; }
    private int Delay { get; set; }

    public ResponseMap(int timeout = 5000, int delay = 100)
    {
        Responses = new ConcurrentDictionary<string, Packet?>();
        Timeout = timeout;
        Delay = delay;
    }

    public bool AddKey(string key)
    {
        return Responses.TryAdd(key, null);
    }

    public bool UpdateKey(string key, Packet? packet)
    {
        if (!Responses.ContainsKey(key))
        {
            return false;
        }

        Responses[key] = packet;

        return true;
    }

    public bool TryGetValue(string key, out Packet? value)
    {
        return Responses.TryGetValue(key, out value);
    }

    public bool TryRemove(string key, out Packet? value)
    {
        return Responses.TryRemove(key, out value);
    }

    public async Task<Packet?> GetAndRemoveWithWaitingAsync(string key, CancellationToken ct)
    {
        var func = async () =>
        {
            while (!ct.IsCancellationRequested)
            {
                if (TryGetValue(key, out var packet) && packet != null)
                {
                    TryRemove(key, out _);
                    return packet;
                }

                await Task.Delay(Delay, ct);
            }

            return null;
        };

        return await func().WithTimeout(Timeout);
    }
}
