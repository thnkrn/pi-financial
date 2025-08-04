//namespace Pi.StructureNotes.Infrastructure.Tests.Services;

//public class ExanteClientTest
//{
//    CancellationToken _ct => It.IsAny<CancellationToken>();

//    [Fact]
//    public async Task GetStockPrice_Success()
//    {
//        var symbol = "AAPL.NASDAQ";

//        using var client = CreateClient();

//        var exante = new ExanteClient(client);

//        var price = await exante.GetStockPrice(symbol);

//        Assert.NotNull(price);
//        Assert.True(price.Value > 0);
//    }

//    [Fact]
//    public async Task GetExchangeRate_Success()
//    {
//        using var client = CreateClient();

//        var exante = new ExanteClient(client);

//        var rate = await exante.GetExchangeRate("USD", "THB");

//        Assert.NotNull(rate);
//        Assert.True(rate > 1);
//    }


//    static HttpClient CreateClient()
//    {
//        var bearer = "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJhdWQiOlsic3ltYm9scyIsIm9obGMiLCJmZWVkIiwiY2hhbmdlIiwiY3Jvc3NyYXRlcyIsIm9yZGVycyIsInN1bW1hcnkiLCJhY2NvdW50cyIsInRyYW5zYWN0aW9ucyJdLCJleHAiOjE3NjcyMjU1OTksImlhdCI6MTY4MDg1OTIzNiwiaXNzIjoiYjIxY2JlYzktOTg1Mi00MjM4LWI1MTktNTVkNTA0MWNhZTkyIiwic3ViIjoiOWE3NDc5MzEtZTc4MS00NDJiLWEwN2ItMWUxNGFmYWU0YTE5In0.Z0lkep0GSxyIGJ9LbwrP7EvPTiS7qdwXjsYiSACeUGI";

//        var cookie = "Cookie: __cf_bm=_SulSpJ5iiFLnL5qLXp0utsqFZZjGqAJzN4EmVZxGuI-1700651969-0-AQQdlNie6hs5/gwlcjuH77a0IThawPNe4oFnu4372LsSuXRDwHS2mz67U7kfE/aJslRVVb6RvMyV6p6wYvumDJs=";

//        var client = new HttpClient
//        {
//            BaseAddress = new Uri("https://api-demo.exante.eu")
//        };

//        client.DefaultRequestHeaders.Add("Authorization", bearer);
//        client.DefaultRequestHeaders.Add("Cookie", cookie);

//        return client;
//    }
//}


