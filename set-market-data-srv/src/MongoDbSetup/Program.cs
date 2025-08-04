using System.Text.RegularExpressions;
using MongoDB.Bson;
using MongoDB.Driver;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson.Serialization;

namespace MongoDbSetup;

internal abstract class Program
{
    private static IConfiguration? Configuration { get; set; }

    private static void Main()
    {
        DbSetup();
    }

    private static void DbSetup()
    {
        // Build configuration
        Configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        // MongoDB connection setup
        var connectionString = Configuration["MongoDbSettings:ConnectionString"];
        var databaseNames = Configuration.GetSection("MongoDbSettings:DatabaseNames")
            .GetChildren()
            .Select(x => x.Value)
            .Where(x => !string.IsNullOrEmpty(x))
            .ToList();

        if (databaseNames.Count == 0)
        {
            Console.WriteLine("No database names found in configuration.");
            return;
        }

        const string basePath = "../../../../Pi.SetMarketData.Domain/Entities/";

        var fileNames = new List<string>
        {
            "CorporateAction.cs", "Filter.cs", "Financial.cs",
            "Indicator.cs", "Instrument.cs", "InstrumentDetail.cs",
            "Intermission.cs", "MarketStatus.cs", "NavList.cs",
            "OrderBook.cs", "PriceInfo.cs", "PublicTrade.cs", "TradingSign.cs",
            "GeInstrument.cs", "MorningStarFlag.cs", "WhiteList.cs",
            "CuratedFilter.cs","CuratedList.cs","CuratedMember.cs",
            "SetVenueMapping.cs","GeVenueMapping.cs","MarketCode.cs"
        };

        var client = new MongoClient(connectionString);

        foreach (var databaseName in databaseNames)
        {
            Console.WriteLine($"Processing database: {databaseName}");
            var database = client.GetDatabase(databaseName);

            foreach (var fileName in fileNames)
            {
                var classContent = File.ReadAllText(Path.Combine(basePath, fileName));
                var className = Path.GetFileNameWithoutExtension(fileName);
                var collectionName = ConvertToSnakeCase(className);
                var attributes = ParseClassAttributes(classContent);

                CreateMongoCollection(database, collectionName, attributes);
                AddDefaultDocument(database, collectionName, attributes);
            }

            Console.WriteLine($"Database '{databaseName}' processed successfully.");

            try
            {
                SeedData(database);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred during the seeding process: {ex}");
            }
        }

        Console.WriteLine(
            "All databases processed. Collections created and initialized with default documents successfully.");
        Console.WriteLine("Press any key to continue!");
        Console.ReadKey();
    }

    private static void SeedData(IMongoDatabase database)
    {
        Console.WriteLine("Starting data seeding process...");

        const string basePath = "./SeedData/";

        // Seed GeInstrument collection
        var geInstrumentCollection = database.GetCollection<BsonDocument>("ge_instrument");
        var geInstrumentData = File.ReadAllText(Path.Combine(basePath, "ge_instrument.json"));
        var geInstrumentDocuments = BsonSerializer.Deserialize<List<BsonDocument>>(geInstrumentData);
        var geInstrumentResult = geInstrumentCollection.DeleteMany(new BsonDocument());

        if (geInstrumentResult.DeletedCount >= 0)
        {
            foreach (var doc in geInstrumentDocuments)
            {
                try
                {
                    doc["_id"] = ObjectId.GenerateNewId();
                    ConvertToInt(doc, "ge_instrument_id");
                    geInstrumentCollection.InsertOne(doc);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }

            Console.WriteLine($"Inserted {geInstrumentDocuments.Count} documents into ge_instrument collection.");
        }

        // Seed WhiteList collection
        var whiteListCollection = database.GetCollection<BsonDocument>("white_list");
        var whiteListData = File.ReadAllText(Path.Combine(basePath, "white_list.json"));
        var whiteListDocuments = BsonSerializer.Deserialize<List<BsonDocument>>(whiteListData);
        var whiteListResult = whiteListCollection.DeleteMany(new BsonDocument());

        if (whiteListResult.DeletedCount >= 0)
        {
            foreach (var doc in whiteListDocuments)
            {
                try
                {
                    doc["_id"] = ObjectId.GenerateNewId();
                    ConvertToInt(doc, "white_list_id");
                    ConvertToInt(doc, "ge_instrument_id");

                    var geInstrumentId = doc["ge_instrument_id"].AsInt32;
                    var geInstrumentDoc =
                        geInstrumentDocuments.FirstOrDefault(d => d["ge_instrument_id"].AsInt32 == geInstrumentId);

                    if (geInstrumentDoc != null)
                    {
                        doc["ge_instrument"] = new BsonDocument
                        {
                            { "_id", geInstrumentDoc["_id"] },
                            { "ge_instrument_id", geInstrumentDoc["ge_instrument_id"] },
                            { "symbol", geInstrumentDoc["symbol"] },
                            { "exchange", geInstrumentDoc["exchange"] }
                            // Add other relevant fields from GeInstrument as needed
                        };
                    }

                    whiteListCollection.InsertOne(doc);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }

            Console.WriteLine($"Inserted {whiteListDocuments.Count} documents into white_list collection.");
        }

        // Seed MorningStarFlag collection
        var morningStarFlagCollection = database.GetCollection<BsonDocument>("morning_star_flag");
        var morningStarFlagData = File.ReadAllText(Path.Combine(basePath, "morning_star_flag.json"));
        var morningStarFlagDocuments = BsonSerializer.Deserialize<List<BsonDocument>>(morningStarFlagData);
        var morningStarFlagResult = morningStarFlagCollection.DeleteMany(new BsonDocument());

        if (morningStarFlagResult.DeletedCount >= 0)
        {
            foreach (var doc in morningStarFlagDocuments)
            {
                try
                {
                    doc["_id"] = ObjectId.GenerateNewId();
                    ConvertToInt(doc, "morning_star_flag_id");
                    ConvertToInt(doc, "white_list_id");

                    var whiteListId = doc["white_list_id"].AsInt32;
                    var whiteListDoc =
                        whiteListDocuments.FirstOrDefault(d => d["white_list_id"].AsInt32 == whiteListId);

                    if (whiteListDoc != null)
                    {
                        doc["white_list"] = new BsonDocument
                        {
                            { "_id", whiteListDoc["_id"] },
                            { "white_list_id", whiteListDoc["white_list_id"] },
                            { "symbol", whiteListDoc["symbol"] },
                            { "exchange", whiteListDoc["exchange"] }
                            // Add other relevant fields from WhiteList as needed
                        };
                    }

                    morningStarFlagCollection.InsertOne(doc);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }

            Console.WriteLine(
                $"Inserted {morningStarFlagDocuments.Count} documents into morning_star_flag collection.");
        }

        Console.WriteLine("Data seeding process completed.");

        // Seed CuratedFilter collection
        var curatedFilterCollection = database.GetCollection<BsonDocument>("curated_filter");
        var curatedFilterData = File.ReadAllText(Path.Combine(basePath, "set_curated_filter.json"));
        var curatedFilterDocument = BsonSerializer.Deserialize<List<BsonDocument>>(curatedFilterData);
        var curatedFilterResult = curatedFilterCollection.DeleteMany(new BsonDocument());
        string curatedListId = "curated_list_id";

        if (curatedFilterResult.DeletedCount >= 0)
        {
            foreach (var doc in curatedFilterDocument)
            {
                try
                {
                    doc["_id"] = ObjectId.GenerateNewId();
                    ConvertToInt(doc, "filter_id");
                    ConvertToInt(doc, curatedListId);
                    ConvertToInt(doc, "category_priority");
                    ConvertToInt(doc, "ordering");

                    curatedFilterCollection.InsertOne(doc);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }

            Console.WriteLine($"Inserted {curatedFilterDocument.Count} documents into curated_filter collection.");
        }

        // Seed CuratedList collection
        var curatedListCollection = database.GetCollection<BsonDocument>("curated_list");
        var curatedListData = File.ReadAllText(Path.Combine(basePath, "set_curated_list.json"));
        var curatedListDocuments = BsonSerializer.Deserialize<List<BsonDocument>>(curatedListData);
        var curatedListResult = curatedListCollection.DeleteMany(new BsonDocument());

        if (curatedListResult.DeletedCount >= 0)
        {
            foreach (var doc in curatedListDocuments)
            {
                try
                {
                    doc["_id"] = ObjectId.GenerateNewId();
                    ConvertToInt(doc, "curated_list_id");
                    ConvertToInt(doc, "is_default");
                    curatedListCollection.InsertOne(doc);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }

            Console.WriteLine($"Inserted {curatedListDocuments.Count} documents into curated_list collection.");
        }

        // Seed CuratedMember collection
        var curatedMemberCollection = database.GetCollection<BsonDocument>("curated_member");
        var curatedMemberData = File.ReadAllText(Path.Combine(basePath, "set_curated_member.json"));
        var curatedMemberDocuments = BsonSerializer.Deserialize<List<BsonDocument>>(curatedMemberData);
        var curatedMemberResult = curatedMemberCollection.DeleteMany(new BsonDocument());

        if (curatedMemberResult.DeletedCount >= 0)
        {
            foreach (var doc in curatedMemberDocuments)
            {
                try
                {
                    doc["_id"] = ObjectId.GenerateNewId();
                    ConvertToInt(doc, "curated_list_id");
                    ConvertToInt(doc, "ordering");
                    ConvertToInt(doc, "is_default");

                    curatedMemberCollection.InsertOne(doc);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }

            Console.WriteLine($"Inserted {curatedMemberDocuments.Count} documents into curated_member collection.");
        }

        // Seed SetVenueMapping collection
        var setVenueMappingCollection = database.GetCollection<BsonDocument>("set_venue_mapping");
        var setVenueMappingData = File.ReadAllText(Path.Combine(basePath, "set_venue_mapping.json"));
        var setVenueMappingDocuments = BsonSerializer.Deserialize<List<BsonDocument>>(setVenueMappingData);
        var setVenueMappingResult = setVenueMappingCollection.DeleteMany(new BsonDocument());

        if (setVenueMappingResult.DeletedCount >= 0)
        {
            foreach (var doc in setVenueMappingDocuments)
            {
                try
                {
                    doc["_id"] = ObjectId.GenerateNewId();
                    setVenueMappingCollection.InsertOne(doc);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }

            Console.WriteLine($"Inserted {setVenueMappingDocuments.Count} documents into set_venue_mapping collection.");
        }

        // Seed GeVenueMapping collection
        var geVenueMappingData = database.GetCollection<BsonDocument>("ge_venue_mapping");
        var geVenueMappingDataData = File.ReadAllText(Path.Combine(basePath, "ge_venue_mapping.json"));
        var geVenueMappingDocuments = BsonSerializer.Deserialize<List<BsonDocument>>(geVenueMappingDataData);
        var geVenueMappingResult = geVenueMappingData.DeleteMany(new BsonDocument());

        if (geVenueMappingResult.DeletedCount >= 0)
        {
            foreach (var doc in geVenueMappingDocuments)
            {
                try
                {
                    doc["_id"] = ObjectId.GenerateNewId();
                    geVenueMappingData.InsertOne(doc);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }

            Console.WriteLine($"Inserted {geVenueMappingDocuments.Count} documents into ge_venue_mapping collection.");
        }

        // Seed MarketCode collection
        var marketCodeCollection = database.GetCollection<BsonDocument>("market_code");
        var marketCodeData = File.ReadAllText(Path.Combine(basePath, "market_code.json"));
        var marketCodeDocuments = BsonSerializer.Deserialize<List<BsonDocument>>(marketCodeData);
        var marketCodeResult = marketCodeCollection.DeleteMany(new BsonDocument());

        if (marketCodeResult.DeletedCount >= 0)
        {
            foreach (var doc in marketCodeDocuments)
            {
                try
                {
                    doc["_id"] = ObjectId.GenerateNewId();
                    ConvertToInt(doc, "order_book_id");
                    marketCodeCollection.InsertOne(doc);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }

            Console.WriteLine($"Inserted {marketCodeDocuments.Count} documents into market_code collection.");
        }

    }





    private static void ConvertToInt(BsonDocument doc, string field)
    {
        if (doc.Contains(field) && doc[field].IsString)
        {
            if (int.TryParse(doc[field].AsString, out int result))
            {
                doc[field] = result;
            }
            // If parsing fails, we keep the original string value
        }
    }

    private static List<(string Name, string Type, bool IsId, string OriginalName, bool IsRelationship)>
        ParseClassAttributes(string classContent)
    {
        var attributes = new List<(string Name, string Type, bool IsId, string OriginalName, bool IsRelationship)>();
        var lines = classContent.Split('\n');
        var nextLineIsProperty = false;
        var bsonElement = "";

        foreach (var line in lines)
        {
            var trimmedLine = line.Trim();

            if (trimmedLine.Contains("[BsonElement"))
            {
                var match = Regex.Match(trimmedLine, @"\[BsonElement\(""(.+)""\)\]");
                if (!match.Success) continue;
                bsonElement = match.Groups[1].Value;
                nextLineIsProperty = true;
            }
            else if (trimmedLine.StartsWith("public") && trimmedLine.Contains("{ get; set; }"))
            {
                var parts = trimmedLine.Split(new[] { ' ', '?', '{' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length < 3) continue;

                var type = parts[1];
                var originalName = parts[2];

                if (type == "virtual")
                {
                    type = parts[2];
                    originalName = parts[3];
                }

                var name = nextLineIsProperty ? bsonElement : ConvertToSnakeCase(originalName);
                var isId = trimmedLine.Contains("[BsonId]") || name.Equals("_id", StringComparison.OrdinalIgnoreCase);
                var isRelationship = type.Equals("Instrument", StringComparison.OrdinalIgnoreCase);
                attributes.Add((name, ConvertToBsonType(type), isId, originalName, isRelationship));

                nextLineIsProperty = false;
                bsonElement = "";
            }
        }

        return attributes;
    }

    private static string ConvertToBsonType(string csharpType)
    {
        if (csharpType.Contains("ObjectId"))
        {
            return "objectId";
        }

        if (csharpType.StartsWith("ICollection<") || csharpType.StartsWith("List<"))
        {
            return "array";
        }

        return csharpType.ToLower() switch
        {
            "int" => "int",
            "long" => "long",
            "double" => "double",
            "float" => "double",
            "decimal" => "decimal",
            "string" => "string",
            "bool" => "bool",
            "datetime" => "date",
            _ => "object" // Default type
        };
    }

    private static void CreateMongoCollection(IMongoDatabase database, string collectionName,
        List<(string Name, string Type, bool IsId, string OriginalName, bool IsRelationship)> attributes)
    {
        // Delete the existing collection if it exists
        try
        {
            database.DropCollection(collectionName);
            Console.WriteLine($"Existing collection '{collectionName}' dropped successfully.");
        }
        catch (MongoCommandException ex) when (ex.Code == 26) // Collection doesn't exist
        {
            Console.WriteLine($"Collection '{collectionName}' did not exist, proceeding with creation.");
        }

        // Create the new collection
        database.CreateCollection(collectionName);

        var bsonDoc = new BsonDocument
        {
            { "bsonType", "object" },
            {
                "required",
                new BsonArray(attributes.Where(attr => !attr.IsId && !attr.IsRelationship).Select(attr => attr.Name))
            }
        };

        var properties = new BsonDocument();

        foreach (var (name, type, isId, _, isRelationship) in attributes)
        {
            if (isId) continue;
            var propertyDoc = new BsonDocument { { "bsonType", isRelationship ? "objectId" : type } };
            if (type == "array")
            {
                propertyDoc.Add("items", new BsonDocument { { "bsonType", "objectId" } });
            }

            properties.Add(name, propertyDoc);
        }

        bsonDoc.Add("properties", properties);

        var validator = new BsonDocument { { "$jsonSchema", bsonDoc } };

        var command = new BsonDocument
        {
            { "collMod", collectionName },
            { "validator", validator },
            { "validationLevel", "strict" },
            { "validationAction", "error" }
        };

        try
        {
            database.RunCommand<BsonDocument>(command);
            Console.WriteLine($"Collection '{collectionName}' created successfully with validation.");
        }
        catch (MongoCommandException ex)
        {
            Console.WriteLine($"Error creating collection '{collectionName}': {ex.Message}");
            Console.WriteLine($"Command: {ex.Command.ToJson()}");
            Console.WriteLine($"Result: {ex.Result.ToJson()}");
        }
    }

    private static void AddDefaultDocument(IMongoDatabase database, string collectionName,
        List<(string Name, string Type, bool IsId, string OriginalName, bool IsRelationship)> attributes)
    {
        var collection = database.GetCollection<BsonDocument>(collectionName);
        var document = new BsonDocument();

        foreach (var (name, type, isId, _, isRelationship) in attributes)
        {
            if (isId)
            {
                continue;
            }

            if (isRelationship)
            {
                // Skip relationship properties in the default document
                continue;
            }

            switch (type)
            {
                case "objectId":
                    document.Add(name, ObjectId.Empty);
                    break;
                case "int":
                    document.Add(name, 0);
                    break;
                case "long":
                    document.Add(name, 0L);
                    break;
                case "double":
                    document.Add(name, 0.0);
                    break;
                case "decimal":
                    document.Add(name, 0m);
                    break;
                case "string":
                    document.Add(name, "Default");
                    break;
                case "bool":
                    document.Add(name, false);
                    break;
                case "date":
                    document.Add(name, DateTime.UtcNow);
                    break;
                case "array":
                    document.Add(name, new BsonArray());
                    break;
                case "object":
                    document.Add(name, new BsonDocument());
                    break;
                default:
                    document.Add(name, BsonNull.Value);
                    break;
            }
        }

        try
        {
            var result = collection.DeleteMany(new BsonDocument());

            if (result.DeletedCount >= 0)
            {
                collection.InsertOne(document);
                Console.WriteLine($"Default document added to collection '{collectionName}'.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error adding default document to collection '{collectionName}': {ex.Message}");
        }
    }

    private static string ConvertToSnakeCase(string input)
    {
        if (string.IsNullOrEmpty(input)) return input;

        var startUnderscores = Regex.Match(input, @"^_+");
        return startUnderscores + Regex.Replace(input, @"([a-z0-9])([A-Z])", "$1_$2").ToLower();
    }
}
