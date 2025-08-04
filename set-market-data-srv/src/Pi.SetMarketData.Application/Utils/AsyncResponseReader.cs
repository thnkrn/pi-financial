namespace Pi.SetMarketData.Application.Utils;

public static class AsyncResponseReader
{
    public static async Task<string?> StreamToString(HttpResponseMessage response)
    {
        Stream? responseStream = null;
        StreamReader? outSr = null;
        string? sRet = null;

        if (response.IsSuccessStatusCode)
        {
            try
            {
                string? sXML;

                responseStream = await response.Content.ReadAsStreamAsync();
                outSr = new StreamReader(responseStream);
                sXML = await outSr.ReadToEndAsync();
                if (sXML != null)
                {
                    sRet = sXML;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Cannot convert to string " + outSr);
                Console.WriteLine(ex);
            }
            finally
            {
                outSr?.Close();
                responseStream?.Close();
            }
            return sRet;
        }
        return null;
    }
}
