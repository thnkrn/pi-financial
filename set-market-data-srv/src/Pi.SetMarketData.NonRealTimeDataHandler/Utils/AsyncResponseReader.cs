namespace Pi.SetMarketData.NonRealTimeDataHandler.Utils;

public class AsyncResponseReader
{
    public static async Task<string?> StreamToString(HttpResponseMessage response)
    {
        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        try
        {
            await using var responseStream = await response.Content.ReadAsStreamAsync();
            using var outSr = new StreamReader(responseStream);
            string? sXML = await outSr.ReadToEndAsync();

            return sXML;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Cannot convert to string.");
            Console.WriteLine(ex);
            return null;
        }
    }
}