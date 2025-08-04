// using System.Net;
// using Microsoft.Extensions.Logging;
// using Moq;
// using Pi.GlobalMarketData.Domain.Models.Request;
// using Pi.GlobalMarketData.Infrastructure.Interfaces.Helpers;
// using Pi.GlobalMarketData.NonRealTimeDataHandler.constants;
// using Pi.GlobalMarketData.NonRealTimeDataHandler.Helpers;
// using Pi.GlobalMarketData.NonRealTimeDataHandler.Services;
//
// namespace Pi.GlobalMarketData.NonRealTimeDataHandler.Tests.Helpers;
//
// public class MorningStarDataHelperTest
// {
//     private readonly MorningStarDataHelper _handler;
//     private readonly Mock<IHttpRequestHelper> _client;
//     private readonly MorningStarStatementTypeRequest _morningStarStatementTypeRequest;
//     private readonly MorningStarPeriodRequest _morningStarPeriodRequest;
//     private readonly MorningStarCommonRequest _exchangeListRequest;
//     private readonly MorningStarExchangeIdRequest _morningStarRequest;
//     private readonly MorningStarExcludingPeriodRequest _morningStarExcludingPeriodRequest;
//     private readonly string _token = "token";
//
//     public MorningStarDataHelperTest()
//     {
//         _morningStarStatementTypeRequest = new MorningStarStatementTypeRequest
//         {
//             ExchangeId = "exchangeId",
//             Identifier = "identifier",
//             DataType = "dataType",
//             StartDate = "startDate",
//             EndDate = "endDate",
//             ResponseType = "responseType"
//         };
//
//         _morningStarPeriodRequest = new MorningStarPeriodRequest
//         {
//             ExchangeId = "exchangeId",
//             Identifier = "identifier",
//             Period = Period.Snapshot
//         };
//
//         _exchangeListRequest = new MorningStarCommonRequest { Identifier = "identifier" };
//
//         _morningStarRequest = new MorningStarExchangeIdRequest
//         {
//             ExchangeId = "exchangeId",
//             Identifier = "identifier"
//         };
//
//         _morningStarExcludingPeriodRequest = new MorningStarExcludingPeriodRequest
//         {
//             ExchangeId = "exchangeId",
//             Identifier = "identifier",
//             ExcludingFrom = "excludingFrom",
//             ExcludingTo = "excludingTo"
//         };
//
//         _client = new Mock<IHttpRequestHelper>();
//         var mockLogger = new Mock<ILogger<MorningStarDataService>>();
//
//         _handler = new MorningStarDataHelper(_client.Object, mockLogger.Object);
//     }
//
//     [Fact]
//     public async Task Call_Login_ShouldRunCorrectly()
//     {
//         // Arrange
//         var expectedUrl = MorningStarEndpoints.login;
//         var expectedMethod = HttpMethod.Get;
//         var expectedProtocol = SecurityProtocolType.Tls12;
//         var email = "email";
//         var password = "password";
//
//         string sURL = string.Format(expectedUrl, email, password);
//
//         _client
//             .Setup(x => x.RequestByUrl(sURL, expectedMethod, expectedProtocol))
//             .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));
//
//         // Act
//         await _handler.Login(email, password);
//
//         // Assert
//         _client.Verify(x => x.RequestByUrl(sURL, null, null), Times.Once);
//     }
//
//     [Fact]
//     public async Task Call_GetIncomeStatement_ShouldRunCorrectly()
//     {
//         // Arrange
//         var expectedUrl = MorningStarEndpoints.incomeStatement;
//         var expectedMethod = HttpMethod.Get;
//         var expectedProtocol = SecurityProtocolType.Tls12;
//
//         string sURL = string.Format(
//             expectedUrl,
//             _token,
//             _morningStarStatementTypeRequest.ExchangeId,
//             _morningStarStatementTypeRequest.IdentifierType,
//             _morningStarStatementTypeRequest.Identifier,
//             _morningStarStatementTypeRequest.StatementType,
//             _morningStarStatementTypeRequest.DataType,
//             _morningStarStatementTypeRequest.StartDate,
//             _morningStarStatementTypeRequest.EndDate,
//             _morningStarStatementTypeRequest.ResponseType
//         );
//
//         _client
//             .Setup(x => x.RequestByUrl(sURL, expectedMethod, expectedProtocol))
//             .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));
//
//         // Act
//         await _handler.GetIncomeStatement(_morningStarStatementTypeRequest, _token);
//
//         // Assert
//         _client.Verify(x => x.RequestByUrl(sURL, null, null), Times.Once);
//     }
//
//     [Fact]
//     public async Task Call_GetValuationRatios_ShouldRunCorrectly()
//     {
//         // Arrange
//         var expectedUrl = MorningStarEndpoints.valuationRatios;
//         var expectedMethod = HttpMethod.Get;
//         var expectedProtocol = SecurityProtocolType.Tls12;
//
//         string sURL = string.Format(
//             expectedUrl,
//             _token,
//             _morningStarPeriodRequest.ExchangeId,
//             _morningStarPeriodRequest.IdentifierType,
//             _morningStarPeriodRequest.Identifier,
//             _morningStarPeriodRequest.Period,
//             _morningStarPeriodRequest.ResponseType
//         );
//
//         _client
//             .Setup(x => x.RequestByUrl(sURL, expectedMethod, expectedProtocol))
//             .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));
//
//         // Act
//         await _handler.GetValuationRatios(_morningStarPeriodRequest, _token);
//
//         // Assert
//         _client.Verify(x => x.RequestByUrl(sURL, null, null), Times.Once);
//     }
//
//     [Fact]
//     public async Task Call_GetBalanceSheet_ShouldRunCorrectly()
//     {
//         // Arrange
//         var expectedUrl = MorningStarEndpoints.balanceSheet;
//         var expectedMethod = HttpMethod.Get;
//         var expectedProtocol = SecurityProtocolType.Tls12;
//
//         string sURL = string.Format(
//             expectedUrl,
//             _token,
//             _morningStarStatementTypeRequest.ExchangeId,
//             _morningStarStatementTypeRequest.IdentifierType,
//             _morningStarStatementTypeRequest.Identifier,
//             _morningStarStatementTypeRequest.StatementType,
//             _morningStarStatementTypeRequest.DataType,
//             _morningStarStatementTypeRequest.StartDate,
//             _morningStarStatementTypeRequest.EndDate,
//             _morningStarStatementTypeRequest.ResponseType
//         );
//
//         _client
//             .Setup(x => x.RequestByUrl(sURL, expectedMethod, expectedProtocol))
//             .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));
//
//         // Act
//         await _handler.GetBalanceSheet(_morningStarStatementTypeRequest, _token);
//
//         // Assert
//         _client.Verify(x => x.RequestByUrl(sURL, null, null), Times.Once);
//     }
//
//     [Fact]
//     public async Task Call_GetProfitabilityRatios_ShouldRunCorrectly()
//     {
//         // Arrange
//         var expectedUrl = MorningStarEndpoints.profitabilityRatios;
//         var expectedMethod = HttpMethod.Get;
//         var expectedProtocol = SecurityProtocolType.Tls12;
//
//         string sURL = string.Format(
//             expectedUrl,
//             _token,
//             _morningStarStatementTypeRequest.ExchangeId,
//             _morningStarStatementTypeRequest.IdentifierType,
//             _morningStarStatementTypeRequest.Identifier,
//             _morningStarStatementTypeRequest.StatementType,
//             _morningStarStatementTypeRequest.DataType,
//             _morningStarStatementTypeRequest.StartDate,
//             _morningStarStatementTypeRequest.EndDate,
//             _morningStarStatementTypeRequest.ResponseType
//         );
//
//         _client
//             .Setup(x => x.RequestByUrl(sURL, expectedMethod, expectedProtocol))
//             .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));
//
//         // Act
//         await _handler.GetProfitabilityRatios(_morningStarStatementTypeRequest, _token);
//
//         // Assert
//         _client.Verify(x => x.RequestByUrl(sURL, null, null), Times.Once);
//     }
//
//     [Fact]
//     public async Task Call_GetSegmentSheets_ShouldRunCorrectly()
//     {
//         // Arrange
//         var expectedUrl = MorningStarEndpoints.segmentSheets;
//         var expectedMethod = HttpMethod.Get;
//         var expectedProtocol = SecurityProtocolType.Tls12;
//
//         string sURL = string.Format(
//             expectedUrl,
//             _token,
//             _morningStarStatementTypeRequest.ExchangeId,
//             _morningStarStatementTypeRequest.IdentifierType,
//             _morningStarStatementTypeRequest.Identifier,
//             _morningStarStatementTypeRequest.StatementType,
//             _morningStarStatementTypeRequest.DataType,
//             _morningStarStatementTypeRequest.StartDate,
//             _morningStarStatementTypeRequest.EndDate,
//             _morningStarStatementTypeRequest.ResponseType
//         );
//
//         _client
//             .Setup(x => x.RequestByUrl(sURL, expectedMethod, expectedProtocol))
//             .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));
//
//         // Act
//         await _handler.GetSegmentSheets(_morningStarStatementTypeRequest, _token);
//
//         // Assert
//         _client.Verify(x => x.RequestByUrl(sURL, null, null), Times.Once);
//     }
//
//     [Fact]
//     public async Task Call_GetCompanyFinancialAvailabilityList_ShouldRunCorrectly()
//     {
//         // Arrange
//         var expectedUrl = MorningStarEndpoints.getCompanyFinancialAvailabilityList;
//         var expectedMethod = HttpMethod.Get;
//         var expectedProtocol = SecurityProtocolType.Tls12;
//
//         string sURL = string.Format(
//             expectedUrl,
//             _token,
//             _morningStarRequest.ExchangeId,
//             _morningStarRequest.IdentifierType,
//             _morningStarRequest.Identifier,
//             _morningStarRequest.ResponseType
//         );
//
//         _client
//             .Setup(x => x.RequestByUrl(sURL, expectedMethod, expectedProtocol))
//             .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));
//
//         // Act
//         await _handler.GetCompanyFinancialAvailabilityList(_morningStarRequest, _token);
//
//         // Assert
//         _client.Verify(x => x.RequestByUrl(sURL, null, null), Times.Once);
//     }
//
//     [Fact]
//     public async Task Call_GetCurrentMarketCapitalisation_ShouldRunCorrectly()
//     {
//         // Arrange
//         var expectedUrl = MorningStarEndpoints.getCurrentMarketCapitalization;
//         var expectedMethod = HttpMethod.Get;
//         var expectedProtocol = SecurityProtocolType.Tls12;
//
//         string sURL = string.Format(
//             expectedUrl,
//             _token,
//             _morningStarRequest.ExchangeId,
//             _morningStarRequest.IdentifierType,
//             _morningStarRequest.Identifier,
//             _morningStarRequest.ResponseType
//         );
//
//         _client
//             .Setup(x => x.RequestByUrl(sURL, expectedMethod, expectedProtocol))
//             .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));
//
//         // Act
//         await _handler.GetCurrentMarketCapitalisation(_morningStarRequest, _token);
//
//         // Assert
//         _client.Verify(x => x.RequestByUrl(sURL, null, null), Times.Once);
//     }
//
//     [Fact]
//     public async Task Call_GetSharesSnapshot_ShouldRunCorrectly()
//     {
//         // Arrange
//         var expectedUrl = MorningStarEndpoints.getSharesSnapshot;
//         var expectedMethod = HttpMethod.Get;
//         var expectedProtocol = SecurityProtocolType.Tls12;
//
//         string sURL = string.Format(
//             expectedUrl,
//             _token,
//             _morningStarRequest.ExchangeId,
//             _morningStarRequest.IdentifierType,
//             _morningStarRequest.Identifier,
//             _morningStarRequest.ResponseType
//         );
//
//         _client
//             .Setup(x => x.RequestByUrl(sURL, expectedMethod, expectedProtocol))
//             .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));
//
//         // Act
//         await _handler.GetSharesSnapshot(_morningStarRequest, _token);
//
//         // Assert
//         _client.Verify(x => x.RequestByUrl(sURL, null, null), Times.Once);
//     }
//
//     [Fact]
//     public async Task Call_GetCompanyGeneralInformation_ShouldRunCorrectly()
//     {
//         // Arrange
//         var expectedUrl = MorningStarEndpoints.getCompanyGeneralInformation;
//         var expectedMethod = HttpMethod.Get;
//         var expectedProtocol = SecurityProtocolType.Tls12;
//
//         string sURL = string.Format(
//             expectedUrl,
//             _token,
//             _morningStarRequest.ExchangeId,
//             _morningStarRequest.IdentifierType,
//             _morningStarRequest.Identifier,
//             _morningStarRequest.ResponseType
//         );
//
//         _client
//             .Setup(x => x.RequestByUrl(sURL, expectedMethod, expectedProtocol))
//             .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));
//
//         // Act
//         await _handler.GetCompanyGeneralInformation(_morningStarRequest, _token);
//
//         // Assert
//         _client.Verify(x => x.RequestByUrl(sURL, null, null), Times.Once);
//     }
//
//     [Fact]
//     public async Task Call_GetCashDividends_ShouldRunCorrectly()
//     {
//         // Arrange
//         var expectedUrl = MorningStarEndpoints.getCashDividends;
//         var expectedMethod = HttpMethod.Get;
//         var expectedProtocol = SecurityProtocolType.Tls12;
//
//         string sURL = string.Format(
//             expectedUrl,
//             _token,
//             _morningStarExcludingPeriodRequest.ExchangeId,
//             _morningStarExcludingPeriodRequest.IdentifierType,
//             _morningStarExcludingPeriodRequest.Identifier,
//             _morningStarExcludingPeriodRequest.ExcludingFrom,
//             _morningStarExcludingPeriodRequest.ExcludingTo,
//             _morningStarExcludingPeriodRequest.ResponseType
//         );
//
//         _client
//             .Setup(x => x.RequestByUrl(sURL, expectedMethod, expectedProtocol))
//             .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));
//
//         // Act
//         await _handler.GetCashDividends(_morningStarExcludingPeriodRequest, _token);
//
//         // Assert
//         _client.Verify(x => x.RequestByUrl(sURL, null, null), Times.Once);
//     }
//
//     [Fact]
//     public async Task Call_GetBusinessDescription_ShouldRunCorrectly()
//     {
//         // Arrange
//         var expectedUrl = MorningStarEndpoints.getBusinessDescription;
//         var expectedMethod = HttpMethod.Get;
//         var expectedProtocol = SecurityProtocolType.Tls12;
//
//         string sURL = string.Format(
//             expectedUrl,
//             _token,
//             _morningStarRequest.ExchangeId,
//             _morningStarRequest.IdentifierType,
//             _morningStarRequest.Identifier,
//             _morningStarRequest.ResponseType
//         );
//
//         _client
//             .Setup(x => x.RequestByUrl(sURL, expectedMethod, expectedProtocol))
//             .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));
//
//         // Act
//         await _handler.GetBusinessDescription(_morningStarRequest, _token);
//
//         // Assert
//         _client.Verify(x => x.RequestByUrl(sURL, null, null), Times.Once);
//     }
// }
