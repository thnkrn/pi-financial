// using System.Globalization;
// using Pi.GlobalMarketData.Application.Services.FixMapper;
// using Pi.GlobalMarketData.Domain.Entities;
// using Pi.GlobalMarketData.Domain.Models.Fix;
//
// namespace Pi.GlobalMarketData.Application.Tests.Services;
//
// public class FixMapperServiceTest
// {
//     [Fact]
//     public void Call_FixMapperService_MapToOrderBook_return_correctly()
//     {
//         // Arrange
//         Entry entry =
//             new()
//             {
//                 MDEntryType = "1",
//                 MDEntryPx = 226.55,
//                 MDEntrySize = 100.0,
//                 MDEntryDate = DateTime.Parse("2024-08-20T00:00:00", new CultureInfo("en-US")),
//                 MDEntryTime = DateTime.Parse("2024-08-21T23:58:16.939", new CultureInfo("en-US"))
//             };
//
//         // Act
//         var result = FixMapperService.MapToOrderBook(entry);
//
//         // Assert
//         Assert.Equal("226.55", result.BidPrice);
//         Assert.Equal("100", result.BidQuantity);
//         Assert.Equal("", result.OfferPrice);
//         Assert.Equal("", result.OfferQuantity);
//     }
//
//     [Fact]
//     public void Call_FixMapperService_MapToPriceInfo_Without_MongoData_return_correctly()
//     {
//         // Arrange
//         Entry entry =
//             new()
//             {
//                 MDEntryType = "2",
//                 MDEntryPx = 226.55,
//                 MDEntrySize = 100.0,
//                 MDEntryDate = DateTime.Parse("2024-08-20T00:00:00", new CultureInfo("en-US")),
//                 MDEntryTime = DateTime.Parse("2024-08-21T23:58:16.939", new CultureInfo("en-US"))
//             };
//
//         PriceInfo? mongoData = null;
//
//         // Act
//         var result = FixMapperService.MapToPriceInfo(entry, mongoData);
//
//         // Assert
//         Assert.Equal(
//             DateTime.Parse("2024-08-21T23:58:16.939", new CultureInfo("en-US")),
//             result.EntryDate
//         );
//         Assert.Equal("226.55", result.Price);
//         Assert.Equal("", result.Currency);
//         Assert.Equal("", result.AuctionPrice);
//         Assert.Equal("", result.AuctionVolume);
//         Assert.Equal("", result.Open);
//         Assert.Equal("226.55", result.High24H);
//         Assert.Equal("226.55", result.Low24H);
//         Assert.Equal("", result.High52W);
//         Assert.Equal("", result.Low52W);
//         Assert.Equal("", result.PriceChanged);
//         Assert.Equal("", result.PriceChangedRate);
//         Assert.Equal("100", result.Volume);
//         Assert.Equal("22655", result.Amount);
//         Assert.Equal("", result.PreClose);
//         Assert.Equal("", result.Status);
//         Assert.Equal("", result.TotalAmount);
//         Assert.Equal("", result.TotalAmountK);
//         Assert.Equal("", result.TotalVolume);
//         Assert.Equal("", result.TotalVolumeK);
//     }
//
//     [Fact]
//     public void Call_FixMapperService_MapToPriceInfo_With_MongoData_return_correctly()
//     {
//         // Arrange
//         Entry entry =
//             new()
//             {
//                 MDEntryType = "2",
//                 MDEntryPx = 226.55,
//                 MDEntrySize = 100.0,
//                 MDEntryDate = DateTime.Parse("2024-08-20T00:00:00", new CultureInfo("en-US")),
//                 MDEntryTime = DateTime.Parse("2024-08-21T23:58:16.939", new CultureInfo("en-US"))
//             };
//
//         PriceInfo mongoData =
//             new()
//             {
//                 EntryDate = DateTime.Parse("2024-08-21T23:58:16.939", new CultureInfo("en-US")),
//                 Price = "226.55",
//                 Currency = "",
//                 AuctionPrice = "",
//                 AuctionVolume = "",
//                 Open = "",
//                 High24H = "500.00",
//                 Low24H = "100.00",
//                 High52W = "",
//                 Low52W = "",
//                 PriceChanged = "",
//                 PriceChangedRate = "",
//                 Volume = "",
//                 Amount = "",
//                 PreClose = "",
//                 Status = "",
//                 TotalAmount = "",
//                 TotalAmountK = "",
//                 TotalVolume = "",
//                 TotalVolumeK = ""
//             };
//
//         // Act
//         var result = FixMapperService.MapToPriceInfo(entry, mongoData);
//
//         // Assert
//         Assert.Equal(
//             DateTime.Parse("2024-08-21T23:58:16.939", new CultureInfo("en-US")),
//             result.EntryDate
//         );
//
//         Assert.Equal("226.55", result.Price);
//         Assert.Equal("", result.Currency);
//         Assert.Equal("", result.AuctionPrice);
//         Assert.Equal("", result.AuctionVolume);
//         Assert.Equal("", result.Open);
//         Assert.Equal("500.00", result.High24H);
//         Assert.Equal("100.00", result.Low24H);
//         Assert.Equal("", result.High52W);
//         Assert.Equal("", result.Low52W);
//         Assert.Equal("", result.PriceChanged);
//         Assert.Equal("", result.PriceChangedRate);
//         Assert.Equal("100", result.Volume);
//         Assert.Equal("22655", result.Amount);
//         Assert.Equal("", result.PreClose);
//         Assert.Equal("", result.Status);
//         Assert.Equal("", result.TotalAmount);
//         Assert.Equal("", result.TotalAmountK);
//         Assert.Equal("", result.TotalVolume);
//         Assert.Equal("", result.TotalVolumeK);
//     }
// }
