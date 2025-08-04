using OpenSearch.Client;
using OpenSearch.Net;
using System;
using System.Collections.Generic;

class Program
{
    // Model class to represent the document structure
    private class Instrument
    {
        public string Symbol { get; set; }
        public string Name { get; set; }
        public string FriendlyName { get; set; }
        public string Type { get; set; }
        public string Category { get; set; }
    }

    static async Task<ISearchResponse<Instrument>> SearchHighLevel(OpenSearchClient client)
    {
        // Build the search query
        return await client.SearchAsync<Instrument>(s => s
            .Size(100)
            .Source(src => src
                .Includes(i => i
                    .Fields(f => f.Symbol, f => f.Name, f => f.FriendlyName, f => f.Type, f => f.Category)
                )
            )
            .Sort(sort => sort.Descending("_score"))
            .Query(q => q
                .Bool(b => b
                    .Must(mu => mu
                        .Term(t => t.Field(f => f.Type).Value("GlobalEquity"))
                    )
                    .Should(
                        sh => sh.Prefix(p => p.Field(f => f.Symbol).Value("AAPL").Boost(10)),
                        sh => sh.Match(m => m.Field("name.ngram").Query("AAPL").Boost(5)),
                        sh => sh.Match(m => m.Field("friendlyName.ngram").Query("AAPL").Boost(1))
                    )
                    .MinimumShouldMatch(1)
                )
            )
        );
    }

    static async Task Main(string[] args)
    {
        // Configure the connection to OpenSearch
        var settings = new ConnectionSettings(new Uri("https://localhost:9200"))
            .BasicAuthentication("admin", "Search7214!")
            .ServerCertificateValidationCallback((o, cert, chain, errors) => true)
            .DefaultIndex("instrument");
        var client = new OpenSearchClient(settings);

        var searchResponse = await SearchHighLevel(client);

        // Print the results
        if (searchResponse.IsValid)
        {
            Console.WriteLine($"Total hits: {searchResponse.Total}");
            foreach (var hit in searchResponse.Hits)
            {
                Console.WriteLine($"Score: {hit.Score}");
                Console.WriteLine($"Symbol: {hit.Source.Symbol}");
                Console.WriteLine($"Name: {hit.Source.Name}");
                Console.WriteLine($"Friendly Name: {hit.Source.FriendlyName}");
                Console.WriteLine($"Type: {hit.Source.Type}");
                Console.WriteLine($"Category: {hit.Source.Category}");
                Console.WriteLine("-------------------");
            }
        }
        else
        {
            Console.WriteLine($"Error: {searchResponse.DebugInformation}");
        }
    }
}
