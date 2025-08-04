using System.Net.Sockets;

namespace Pi.OnePort.TCP.Extensions;

public static class TcpClientExtensions
{
    public static async Task<byte[]> Read(this TcpClient tcpClient)
    {
        var response = new byte[2];
        _ = await tcpClient.GetStream().ReadAsync(response);

        if (BitConverter.IsLittleEndian) Array.Reverse(response);
        var size = BitConverter.ToInt16(response);

        return await tcpClient.Read(size);
    }

    public static async Task<byte[]> Read(this TcpClient tcpClient, short size)
    {
        var response = new byte[size];
        _ = await tcpClient.GetStream().ReadAsync(response);

        return response;
    }
}
