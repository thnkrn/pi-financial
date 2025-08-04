using Moq;
using Microsoft.AspNetCore.Mvc;
using Pi.BackofficeService.API.Controllers;
using Pi.BackofficeService.API.Models;
using Pi.BackofficeService.Application.Queries;
using Pi.BackofficeService.Domain.AggregateModels.TransactionAggregate;
using Pi.Common.Http;

namespace Pi.BackofficeService.API.Tests.Controllers
{
    public class TransactionControllersTest
    {
        private readonly Mock<IBackofficeQueries> _mockBackofficeQueries;
        private readonly TransactionController _controller;

        public TransactionControllersTest()
        {
            _mockBackofficeQueries = new Mock<IBackofficeQueries>();
            _controller = new TransactionController(_mockBackofficeQueries.Object);
        }

        [Fact]
        public async Task TransactionView_ShouldReturnCorrectResponse()
        {
            // Arrange
            var transactionNo = "transactionNo";
            var transactionDetailResult = new TransactionDetailResult<TransactionV2>
            {
                Transaction = new TransactionV2
                {
                    TransactionNo = transactionNo,
                    Product = Product.Cash,
                    Channel = Channel.QR,
                    TransactionType = TransactionType.Deposit,
                    RequestedAmount = "100"
                },
                ResponseCodeDetail = null // set this as needed
            };
            _mockBackofficeQueries.Setup(service => service.GetTransactionV2ByTransactionNo(transactionNo, It.IsAny<CancellationToken>()))
                .ReturnsAsync(transactionDetailResult);

            // Act
            var result = await _controller.TransactionView(transactionNo);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<ApiResponse<TransactionV2DetailResponse>>(okResult.Value);
            Assert.Equal(transactionNo, returnValue.Data.TransactionNo);

        }
    }
}