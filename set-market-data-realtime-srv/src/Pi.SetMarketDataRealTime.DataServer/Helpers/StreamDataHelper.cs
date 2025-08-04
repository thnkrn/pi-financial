using DotNetty.Transport.Channels;
using Pi.SetMarketDataRealTime.Infrastructure.Models.SoupBinTcp.Messages;

namespace Pi.SetMarketDataRealTime.DataServer.Helpers;

public static class StreamDataHelper
{
    private static volatile bool _streaming = true;

    public static async Task StreamDataAsync(IChannelHandlerContext context)
    {
        var configuration = ConfigurationHelper.GetConfiguration();
        var folderPath = configuration["ServerConfig:StreamDataPath"];
        var endSequence = "M128487371"u8.ToArray();

        if (!string.IsNullOrEmpty(folderPath))
        {
            // Get all file paths in the folder
            var filePaths = Directory.GetFiles(folderPath).OrderBy(Path.GetFileName);

            foreach (var filePath in filePaths)
            {
                if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
                {
                    Console.WriteLine("The file does not exist.");
                    return;
                }

                await StreamData(context, filePath, endSequence);
            }
        }
        else
        {
            Console.WriteLine("The stream data path does not exist.");
        }
    }

    private static async Task StreamData(IChannelHandlerContext context, string filePath, byte[] endSequence)
    {
        try
        {
            await using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            using var binaryReader = new BinaryReader(fileStream);
            var observeList = new List<byte>();

            while (fileStream.Position < fileStream.Length && _streaming)
            {
                var currentByte = binaryReader.ReadByte();

                observeList.Add(currentByte);

                if (observeList.Count <= 10) continue;
                var lastBytes = observeList.Skip(observeList.Count - 10);

                var match = lastBytes.SequenceEqual(endSequence);
                if (!match) continue;

                var message = observeList.Take(observeList.Count - 10).ToList();
                var messageLength = GetMessageLength((char)message[0]);

                try
                {
                    if (messageLength != 0)
                    {
                        var messageToParse = message.AsEnumerable().Take(messageLength).ToArray();

                        if (context.Channel is { Active: true })
                        {
                            await context.WriteAndFlushAsync(new SequencedData(messageToParse));
                        }
                        else
                        {
                            // handle streaming data to client
                            _streaming = false;
                            await context.CloseAsync();
                            throw new ConnectException("Client disconnected!", null);
                        }

                        // Add a small delay to control the streaming rate (optional)
                        await Task.Delay(10);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

                observeList.Clear();
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"An error occurred: {e.Message}");
        }
    }

    private static int GetMessageLength(char messageType)
    {
        var messageLengths = new Dictionary<char, int>
        {
            { 'T', 5 },
            { 'R', 321 },
            { 'e', 38 },
            { 'm', 43 },
            { 'M', 18 },
            { 'L', 25 },
            { 'k', 17 },
            { 'S', 6 },
            { 'O', 29 },
            { 'l', 29 },
            { 'Z', 53 },
            { 'i', 42 },
            { 'I', 73 },
            { 'f', 29 },
            { 'J', 93 },
            { 'g', 84 },
            { 'Q', 22 },
            { 'h', 25 },
            { 'j', 15 },
            { 'G', 21 },
            { 'b', 171 },
            { 'N', 1231 }
        };

        return messageLengths.GetValueOrDefault(messageType, 0);
    }
}