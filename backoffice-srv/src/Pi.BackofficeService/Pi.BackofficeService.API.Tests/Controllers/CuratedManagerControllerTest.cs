using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Pi.BackofficeService.API.Controllers;
using Pi.BackofficeService.Application.Models;
using Pi.BackofficeService.Application.Models.CuratedManager;
using Pi.BackofficeService.Application.Services.CuratedManagerService;
using Pi.Common.Http;

namespace Pi.BackofficeService.API.Tests.Controllers
{
    public class CuratedManagerControllerTests
    {
        private readonly Mock<ICuratedManagerService> _curatedManagerServiceMock;
        private readonly CuratedManagerController _controller;
        private readonly string _authToken = "test-bearer-token";

        public CuratedManagerControllerTests()
        {
            _curatedManagerServiceMock = new Mock<ICuratedManagerService>();
            _controller = new CuratedManagerController(_curatedManagerServiceMock.Object);

            // Setup default authorization header for all tests
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };
            _controller.ControllerContext.HttpContext.Request.Headers["Authorization"] = $"Bearer {_authToken}";
        }

        #region GetCuratedList Tests

        [Fact]
        public async Task GetCuratedList_WithValidToken_ReturnsOkResult()
        {
            // Arrange
            var expectedResponse = new CuratedListResponse(
                new List<TransformedCuratedListItem>(),
                new List<TransformedCuratedListItem>()
            );

            _curatedManagerServiceMock
                .Setup(service => service.GetCuratedList(_authToken))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _controller.GetCuratedList();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<CuratedListResponse>>(okResult.Value);
            Assert.Equal(expectedResponse, apiResponse.Data);
        }

        [Fact]
        public async Task GetCuratedList_WithNoAuthToken_ReturnsUnauthorized()
        {
            // Arrange
            _controller.ControllerContext.HttpContext.Request.Headers.Remove("Authorization");

            // Act
            var result = await _controller.GetCuratedList();

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status401Unauthorized, objectResult.StatusCode);
        }

        [Fact]
        public async Task GetCuratedList_WhenServiceThrowsException_ReturnsProblem()
        {
            // Arrange
            _curatedManagerServiceMock
                .Setup(service => service.GetCuratedList(_authToken))
                .ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await _controller.GetCuratedList();

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status500InternalServerError, objectResult.StatusCode);
        }

        #endregion

        #region UploadCuratedManualListCSV Tests

        [Fact]
        public async Task UploadCuratedManualListCSV_WithValidInput_ReturnsCreatedResult()
        {
            // Arrange
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.FileName).Returns("test.csv");
            fileMock.Setup(f => f.ContentType).Returns("text/csv");

            var dataSource = "SET";
            var expectedResponse = new CuratedListResponse(
                new List<TransformedCuratedListItem>(),
                new List<TransformedCuratedListItem>()
            );

            _curatedManagerServiceMock
                .Setup(service => service.UploadCuratedManualList(_authToken, fileMock.Object, dataSource))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _controller.UploadCuratedManualListCSV(fileMock.Object, dataSource);

            // Assert
            var createdResult = Assert.IsType<CreatedResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<CuratedListResponse>>(createdResult.Value);
            Assert.Equal(expectedResponse, apiResponse.Data);
        }

        [Fact]
        public async Task UploadCuratedManualListCSV_WithNullFile_ReturnsBadRequest()
        {
            // Act
            var result = await _controller.UploadCuratedManualListCSV(null!, "SET");

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status400BadRequest, objectResult.StatusCode);
        }

        [Fact]
        public async Task UploadCuratedManualListCSV_WithInvalidFileType_ReturnsBadRequest()
        {
            // Arrange
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.FileName).Returns("test.txt");
            fileMock.Setup(f => f.ContentType).Returns("text/plain");

            // Act
            var result = await _controller.UploadCuratedManualListCSV(fileMock.Object, "SET");

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status400BadRequest, objectResult.StatusCode);
        }

        [Fact]
        public async Task UploadCuratedManualListCSV_WithInvalidDataSource_ReturnsBadRequest()
        {
            // Arrange
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.FileName).Returns("test.csv");
            fileMock.Setup(f => f.ContentType).Returns("text/csv");

            // Act
            var result = await _controller.UploadCuratedManualListCSV(fileMock.Object, "INVALID");

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status400BadRequest, objectResult.StatusCode);
        }

        #endregion

        #region UpdateCuratedListById Tests

        [Fact]
        public async Task UpdateCuratedListById_WithValidInput_ReturnsOkResult()
        {
            // Arrange
            var id = "list-123";
            var name = "Updated List";
            var hashtag = "#updated";
            var dataSource = "GE";

            var request = new CuratedListUpdateRequest(name, hashtag);

            var expectedResponse = new CuratedListItem(
                id,
                id,
                1,
                "CODE123",
                "Manual",
                "ALL",
                name,
                hashtag,
                1,
                CuratedListSource.GE,
                DateTime.Now,
                DateTime.Now,
                "test-user"
            );

            _curatedManagerServiceMock
                .Setup(service => service.UpdateCuratedListById(_authToken, id, name, hashtag, dataSource))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _controller.UpdateCuratedListById(id, request, dataSource);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<CuratedListItem>>(okResult.Value);
            Assert.Equal(expectedResponse, apiResponse.Data);
        }

        [Fact]
        public async Task UpdateCuratedListById_WithNullRequest_ReturnsBadRequest()
        {
            // Arrange
            var request = new CuratedListUpdateRequest(null, null);

            // Act
            var result = await _controller.UpdateCuratedListById("list-123", request, "SET");

            // Assert
            var problemResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status400BadRequest, problemResult.StatusCode);
            var problemDetails = Assert.IsType<ProblemDetails>(problemResult.Value);
            Assert.Equal("Either name or hashtag must be provided", problemDetails.Detail);
        }

        [Fact]
        public async Task UpdateCuratedListById_WithEmptyNameAndHashtag_ReturnsBadRequest()
        {
            // Arrange
            var request = new CuratedListUpdateRequest("", "");

            // Act
            var result = await _controller.UpdateCuratedListById("list-123", request, "SET");

            // Assert
            var problemResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status400BadRequest, problemResult.StatusCode);
            var problemDetails = Assert.IsType<ProblemDetails>(problemResult.Value);
            Assert.Equal("Either name or hashtag must be provided", problemDetails.Detail);
        }

        [Fact]
        public async Task UpdateCuratedListById_WithInvalidDataSource_ReturnsBadRequest()
        {
            // Arrange
            var request = new CuratedListUpdateRequest("Test", "#test");

            // Act
            var result = await _controller.UpdateCuratedListById("list-123", request, "INVALID");

            // Assert
            var problemResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status400BadRequest, problemResult.StatusCode);
            var problemDetails = Assert.IsType<ProblemDetails>(problemResult.Value);
            Assert.Equal("dataSource must be either 'SET' or 'GE'", problemDetails.Detail);
        }

        #endregion

        #region DeleteCuratedListById Tests

        [Fact]
        public async Task DeleteCuratedListById_WithValidInput_ReturnsNoContent()
        {
            // Arrange
            var id = "list-123";
            var dataSource = "SET";

            _curatedManagerServiceMock
                .Setup(service => service.DeleteCuratedListById(_authToken, id, dataSource))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.DeleteCuratedListById(id, dataSource);

            // Assert
            var noContentResult = Assert.IsType<NoContentResult>(result);
            Assert.Equal(StatusCodes.Status204NoContent, noContentResult.StatusCode);
        }

        [Fact]
        public async Task DeleteCuratedListById_WithEmptyDataSource_ReturnsBadRequest()
        {
            // Act
            var result = await _controller.DeleteCuratedListById("list-123", "");

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status400BadRequest, objectResult.StatusCode);
        }

        [Fact]
        public async Task DeleteCuratedListById_WithInvalidDataSource_ReturnsBadRequest()
        {
            // Act
            var result = await _controller.DeleteCuratedListById("list-123", "INVALID");

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status400BadRequest, objectResult.StatusCode);
        }

        #endregion

        #region GetCuratedFilters Tests

        [Fact]
        public async Task GetCuratedFilters_WithValidInput_ReturnsOkResult()
        {
            // Arrange
            var groupName = "TestGroup";
            var expectedResponse = new List<CuratedFilterGroup>
            {
                new CuratedFilterGroup("TestSubGroup", new List<CuratedFilterItem>
                {
                    new CuratedFilterItem("filter-1", 1, "Filter 1", "Category", "Type", 1, 123, "List", "SET", false, true, 1)
                })
            };

            _curatedManagerServiceMock
                .Setup(service => service.GetCuratedFilters(_authToken, groupName))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _controller.GetCuratedFilters(groupName);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<List<CuratedFilterGroup>>>(okResult.Value);
            Assert.Equal(expectedResponse, apiResponse.Data);
        }

        [Fact]
        public async Task GetCuratedFilters_WithEmptyGroupName_ReturnsBadRequest()
        {
            // Act
            var result = await _controller.GetCuratedFilters("");

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status400BadRequest, objectResult.StatusCode);
        }

        #endregion

        #region UploadCuratedFiltersCSV Tests

        [Fact]
        public async Task UploadCuratedFiltersCSV_WithValidInput_ReturnsCreatedResult()
        {
            // Arrange
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.FileName).Returns("filters.csv");
            fileMock.Setup(f => f.ContentType).Returns("text/csv");

            var dataSource = "SET";
            var expectedResponse = new List<CuratedFilterGroup>
            {
                new CuratedFilterGroup("TestSubGroup", new List<CuratedFilterItem>
                {
                    new CuratedFilterItem("filter-1", 1, "Filter 1", "Category", "Type", 1, 123, "List", "SET", false, true, 1)
                })
            };

            _curatedManagerServiceMock
                .Setup(service => service.UploadCuratedFilters(_authToken, fileMock.Object, dataSource))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _controller.UploadCuratedFiltersCSV(fileMock.Object, dataSource);

            // Assert
            var createdResult = Assert.IsType<CreatedResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<List<CuratedFilterGroup>>>(createdResult.Value);
            Assert.Equal(expectedResponse, apiResponse.Data);
        }

        [Fact]
        public async Task UploadCuratedFiltersCSV_WithNullFile_ReturnsBadRequest()
        {
            // Act
            var result = await _controller.UploadCuratedFiltersCSV(null!, "SET");

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status400BadRequest, objectResult.StatusCode);
        }

        #endregion

        #region GetCuratedMembersByCuratedListId Tests

        [Fact]
        public async Task GetCuratedMembersByCuratedListId_WithValidInput_ReturnsOkResult()
        {
            // Arrange
            var listId = "list-123";
            var expectedResponse = new List<TransformedCuratedMemberItem>
            {
                new TransformedCuratedMemberItem(
                    "AAPL.US",
                    "Apple Inc",
                    "logo.png",
                    "FIGI123",
                    "1",
                    "NASDAQ",
                    "VC123",
                    "VC456",
                    CuratedListSource.SET,
                    "member-1"
                )
            };

            _curatedManagerServiceMock
                .Setup(service => service.GetCuratedMembersByCuratedListId(_authToken, listId))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _controller.GetCuratedMembersByCuratedListId(listId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<List<TransformedCuratedMemberItem>>>(okResult.Value);
            Assert.Equal(expectedResponse, apiResponse.Data);
        }

        #endregion
    }
}