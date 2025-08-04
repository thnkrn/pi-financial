using System.Net;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using Pi.BackofficeService.Application.Models;
using Pi.BackofficeService.Application.Models.CuratedManager;
using Pi.BackofficeService.Application.Services.CuratedManagerService;
using Pi.BackofficeService.Infrastructure.CuratedManager;
using Pi.BackofficeService.Infrastructure.Models.CuratedManager;
using Pi.BackofficeService.Infrastructure.Services;

namespace Pi.BackofficeService.Infrastructure.Tests.Services
{
    public class CuratedManagerServiceTest
    {
        private readonly string _baseUrl = "https://api.test.com";
        private readonly string _authToken = "test-token";
        private readonly Mock<ILogger<CuratedManagerService>> _loggerMock;
        private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
        private readonly HttpClient _httpClient;
        private readonly ICuratedManagerService _curatedManagerService;

        public CuratedManagerServiceTest()
        {
            _loggerMock = new Mock<ILogger<CuratedManagerService>>();
            _httpMessageHandlerMock = new Mock<HttpMessageHandler>();
            _httpClient = new HttpClient(_httpMessageHandlerMock.Object);
            _curatedManagerService = new CuratedManagerService(_httpClient, _baseUrl, _loggerMock.Object);
        }

        private void SetupMockResponse<T>(HttpStatusCode statusCode, T content)
        {
            var responseMessage = new HttpResponseMessage(statusCode);

            if (content != null)
            {
                var json = JsonConvert.SerializeObject(content);
                responseMessage.Content = new StringContent(json, Encoding.UTF8, "application/json");
            }

            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(responseMessage);
        }

        private void VerifyHttpRequest(HttpMethod method, string url)
        {
            _httpMessageHandlerMock
                .Protected()
                .Verify(
                    "SendAsync",
                    Times.AtLeastOnce(),
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == method &&
                        req.RequestUri!.ToString() == url),
                    ItExpr.IsAny<CancellationToken>());
        }

        private static CuratedListItem CreateCuratedListItem(string id = "test-id", string curatedType = "Logical", CuratedListSource? source = CuratedListSource.SET, string name = "Test List", string hashtag = "#test")
        {
            return new CuratedListItem(
                id,
                id,
                1,
                "TEST001",
                curatedType,
                "ALL",
                name,
                hashtag,
                1,
                source,
                DateTime.Now,
                DateTime.Now,
                "test-user"
            );
        }

        private static CuratedFilterItem CreateCuratedFilterItem(string id, string name, int ordering)
        {
            return new CuratedFilterItem(
                id,
                1,
                name,
                "TestCategory",
                "TestType",
                1,
                123,
                "Test List",
                "SET",
                false,
                true,
                ordering
            );
        }

        private static CuratedMemberItem CreateCuratedMemberItem(string symbol, string friendlyName)
        {
            return new CuratedMemberItem(
                symbol,
                friendlyName,
                "logo.png",
                "FIGI123",
                "1",
                "NYSE",
                "VC123",
                "VC456"
            );
        }

        [Fact]
        public async Task GetCuratedList_ShouldReturnTransformedList()
        {
            // Arrange
            var mockResponse = new CuratedListThirdPartyApiResponse(
                new CuratedListData(
                    [
                        CreateCuratedListItem(id: "set-id-1", curatedType: "Logical", source: CuratedListSource.SET),
                        CreateCuratedListItem(id: "set-id-2", curatedType: "Manual", source: CuratedListSource.SET)
                    ]
                ),
                new CuratedListData(
                    [
                        CreateCuratedListItem(id: "ge-id-1", curatedType: "Logical", source: CuratedListSource.GE),
                        CreateCuratedListItem(id: "ge-id-2", curatedType: "Manual", source: CuratedListSource.GE)
                    ]
                )
            );

            SetupMockResponse(HttpStatusCode.OK, mockResponse);

            // Act
            var result = await _curatedManagerService.GetCuratedList(_authToken);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Logical!.Count);
            Assert.Equal(2, result.Manual!.Count);
            Assert.Contains(result.Logical, item => item.Id == "set-id-1");
            Assert.Contains(result.Logical, item => item.Id == "ge-id-1");
            Assert.Contains(result.Manual, item => item.Id == "set-id-2");
            Assert.Contains(result.Manual, item => item.Id == "ge-id-2");

            VerifyHttpRequest(HttpMethod.Get, $"{_baseUrl}/marketdata-migrationproxy/market-data-management/curated-lists");
        }

        [Fact]
        public async Task GetCuratedList_ShouldHandleNullResponse()
        {
            // Arrange
            SetupMockResponse<CuratedListThirdPartyApiResponse?>(HttpStatusCode.OK, null);

            // Act
            var result = await _curatedManagerService.GetCuratedList(_authToken);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result.Logical!);
            Assert.Empty(result.Manual!);
        }

        [Fact]
        public async Task GetCuratedList_ShouldThrowException_WhenError()
        {
            // Arrange
            SetupMockResponse(HttpStatusCode.InternalServerError, "Server error");

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() => _curatedManagerService.GetCuratedList(_authToken));
        }

        [Fact]
        public async Task UploadCuratedManualList_ShouldUploadAndReturnUpdatedList()
        {
            // Arrange
            var mockFile = new Mock<IFormFile>();
            mockFile.Setup(f => f.FileName).Returns("test.csv");
            mockFile.Setup(f => f.OpenReadStream()).Returns(new MemoryStream());

            var mockResponse = new CuratedListThirdPartyApiResponse(
                new CuratedListData(
                    [CreateCuratedListItem(id: "new-set-id", curatedType: "Manual", source: CuratedListSource.SET)]
                ),
                new CuratedListData(
                    [CreateCuratedListItem(id: "new-ge-id", curatedType: "Manual", source: CuratedListSource.GE)]
                )
            );

            SetupMockResponse(HttpStatusCode.OK, mockResponse);

            // Act
            var result = await _curatedManagerService.UploadCuratedManualList(
                _authToken, mockFile.Object, "GE");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Manual!.Count);
            Assert.Empty(result.Logical!);
            Assert.Contains(result.Manual, item => item.Id == "new-set-id");
            Assert.Contains(result.Manual, item => item.Id == "new-ge-id");

            VerifyHttpRequest(HttpMethod.Post, $"{_baseUrl}/marketdata-migrationproxy/market-data-management/ge/curated-lists");
        }

        [Fact]
        public async Task UploadCuratedManualList_ShouldThrowException_WhenError()
        {
            // Arrange
            var mockFile = new Mock<IFormFile>();
            mockFile.Setup(f => f.FileName).Returns("test.csv");
            mockFile.Setup(f => f.OpenReadStream()).Returns(new MemoryStream());

            SetupMockResponse(HttpStatusCode.InternalServerError, "Server error");

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() =>
                _curatedManagerService.UploadCuratedManualList(_authToken, mockFile.Object, "GE"));
        }

        [Fact]
        public async Task UpdateCuratedListById_ShouldUpdateAndReturnUpdatedItem()
        {
            // Arrange
            var listId = "list-123";
            var updatedName = "Updated List Name";
            var updatedHashtag = "#updated";
            var mockResponse = new UpdateCuratedListByIdThirdPartyResponse(
                null,
                new CuratedListUpdateData(
                    true,
                    CreateCuratedListItem(id: listId, name: updatedName, hashtag: updatedHashtag)
                )
            );

            SetupMockResponse(HttpStatusCode.OK, mockResponse);

            // Act
            var result = await _curatedManagerService.UpdateCuratedListById(
                _authToken, listId, updatedName, updatedHashtag, "GE");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(updatedName, result.Name);
            Assert.Equal(updatedHashtag, result.Hashtag);

            VerifyHttpRequest(HttpMethod.Patch, $"{_baseUrl}/marketdata-migrationproxy/market-data-management/ge/curated-lists/{listId}");
        }

        [Fact]
        public async Task UpdateCuratedListById_ShouldThrowException_WhenError()
        {
            // Arrange
            var listId = "list-123";
            SetupMockResponse(HttpStatusCode.InternalServerError, "Server error");

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() =>
                _curatedManagerService.UpdateCuratedListById(_authToken, listId, "name", "#tag", "GE"));
        }

        [Fact]
        public async Task DeleteCuratedListById_ShouldReturnTrue_WhenSuccessful()
        {
            // Arrange
            var listId = "list-123";
            SetupMockResponse(HttpStatusCode.OK, "");

            // Act
            var result = await _curatedManagerService.DeleteCuratedListById(_authToken, listId, "SET");

            // Assert
            Assert.True(result);

            VerifyHttpRequest(HttpMethod.Delete, $"{_baseUrl}/marketdata-migrationproxy/market-data-management/set/curated-lists/{listId}");
        }

        [Fact]
        public async Task DeleteCuratedListById_ShouldThrowException_WhenError()
        {
            // Arrange
            var listId = "list-123";
            SetupMockResponse(HttpStatusCode.InternalServerError, "Server error");

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() =>
                _curatedManagerService.DeleteCuratedListById(_authToken, listId, "SET"));
        }

        [Fact]
        public async Task GetCuratedFilters_ShouldReturnTransformedFilters()
        {
            // Arrange
            var groupName = "TestGroup";
            var mockResponse = new CuratedFiltersThirdPartyApiResponse(
                [
                    new CuratedFilterData(
                        "TestGroup",
                        "TestSubGroup",
                        [
                            CreateCuratedFilterItem("filter-1", "Filter 1", 1),
                            CreateCuratedFilterItem("filter-2", "Filter 2", 2)
                        ]
                    ),
                    new CuratedFilterData(
                        "TestGroup",
                        "AnotherSubGroup",
                        [
                            CreateCuratedFilterItem("filter-3", "Filter 3", 3)
                        ]
                    )
                ]
            );

            SetupMockResponse(HttpStatusCode.OK, mockResponse);

            // Act
            var result = await _curatedManagerService.GetCuratedFilters(_authToken, groupName);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal("TestSubGroup", result[0].Name);
            Assert.Equal("AnotherSubGroup", result[1].Name);
            Assert.Equal(2, result[0].Data.Count);
            Assert.Single(result[1].Data);

            VerifyHttpRequest(HttpMethod.Get, $"{_baseUrl}/marketdata-migrationproxy/market-data-management/curated-filters?groupName={Uri.EscapeDataString(groupName)}");
        }

        [Fact]
        public async Task GetCuratedFilters_ShouldThrowException_WhenError()
        {
            // Arrange
            SetupMockResponse(HttpStatusCode.InternalServerError, "Server error");

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() =>
                _curatedManagerService.GetCuratedFilters(_authToken, "TestGroup"));
        }

        [Fact]
        public async Task UploadCuratedFilters_ShouldUploadAndReturnFilters()
        {
            // Arrange
            var mockFile = new Mock<IFormFile>();
            mockFile.Setup(f => f.FileName).Returns("filters.csv");
            mockFile.Setup(f => f.OpenReadStream()).Returns(new MemoryStream());

            var mockResponse = new CuratedFiltersThirdPartyApiResponse(
                [
                    new CuratedFilterData(
                        "UploadedGroup",
                        "UploadedSubGroup",
                        [
                            CreateCuratedFilterItem("new-filter-1", "New Filter", 1)
                        ]
                    )
                ]
            );

            SetupMockResponse(HttpStatusCode.OK, mockResponse);

            // Act
            var result = await _curatedManagerService.UploadCuratedFilters(
                _authToken, mockFile.Object, "SET");

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("UploadedSubGroup", result[0].Name);
            Assert.Single(result[0].Data);

            VerifyHttpRequest(HttpMethod.Post, $"{_baseUrl}/marketdata-migrationproxy/market-data-management/set/curated-filters");
        }

        [Fact]
        public async Task UploadCuratedFilters_ShouldThrowException_WhenError()
        {
            // Arrange
            var mockFile = new Mock<IFormFile>();
            mockFile.Setup(f => f.FileName).Returns("filters.csv");
            mockFile.Setup(f => f.OpenReadStream()).Returns(new MemoryStream());

            SetupMockResponse(HttpStatusCode.InternalServerError, "Server error");

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() =>
                _curatedManagerService.UploadCuratedFilters(_authToken, mockFile.Object, "SET"));
        }

        [Fact]
        public async Task GetCuratedMembersByCuratedListId_ShouldReturnTransformedMembers()
        {
            // Arrange
            var listId = "list-123";
            var mockResponse = new CuratedMembersThirdPartyApiResponse(
                new CuratedMemberData(
                    [
                        CreateCuratedMemberItem("AAPL.US", "Apple Inc"),
                        CreateCuratedMemberItem("MSFT.US", "Microsoft Corp")
                    ]
                ),
                new CuratedMemberData(
                    [
                        CreateCuratedMemberItem("GOOG.US", "Alphabet Inc"),
                        CreateCuratedMemberItem("AMZN.US", "Amazon.com Inc")
                    ]
                )
            );

            SetupMockResponse(HttpStatusCode.OK, mockResponse);

            // Act
            var result = await _curatedManagerService.GetCuratedMembersByCuratedListId(_authToken, listId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(4, result.Count);
            Assert.Contains(result, item => item.Symbol == "AAPL.US" && item.Source == CuratedListSource.SET);
            Assert.Contains(result, item => item.Symbol == "MSFT.US" && item.Source == CuratedListSource.SET);
            Assert.Contains(result, item => item.Symbol == "GOOG.US" && item.Source == CuratedListSource.GE);
            Assert.Contains(result, item => item.Symbol == "AMZN.US" && item.Source == CuratedListSource.GE);

            VerifyHttpRequest(HttpMethod.Get, $"{_baseUrl}/marketdata-migrationproxy/market-data-management/curated-members?curatedListId={listId}");
        }

        [Fact]
        public async Task GetCuratedMembersByCuratedListId_ShouldThrowException_WhenError()
        {
            // Arrange
            var listId = "list-123";
            SetupMockResponse(HttpStatusCode.InternalServerError, "Server error");

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() =>
                _curatedManagerService.GetCuratedMembersByCuratedListId(_authToken, listId));
        }
    }
}
