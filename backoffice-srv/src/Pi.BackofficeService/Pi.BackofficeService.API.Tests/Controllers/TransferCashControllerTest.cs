using Moq;
using Microsoft.AspNetCore.Mvc;
using Pi.BackofficeService.API.Controllers;
using Pi.BackofficeService.API.Models;
using Pi.BackofficeService.Application.Queries;
using Pi.BackofficeService.Domain.AggregateModels.TransactionAggregate;
using Pi.BackofficeService.Domain.AggregateModels.TransferCashAggregate;
using Pi.Common.Http;

namespace Pi.BackofficeService.API.Tests.Controllers
{
    public class TransferCashControllerTest
    {
        private readonly Mock<IBackofficeQueries> _mockBackofficeQueries;
        private readonly TransferCashController _controller;

        public TransferCashControllerTest()
        {
            _mockBackofficeQueries = new Mock<IBackofficeQueries>();
            _controller = new TransferCashController(_mockBackofficeQueries.Object);
        }

        [Fact]
        public async Task TransferCashDetail_ShouldReturnCorrectResponse()
        {
            // Arrange
            var transactionNo = "transactionNo";
            var transactionDetailResult = new TransactionDetailResult<TransferCash>
            {
                Transaction = new TransferCash
                {
                    TransactionNo = transactionNo,
                    Amount = 10,
                    TransferFromAccountCode = "transferFrom",
                    TransferFromExchangeMarket = Product.Cash,
                    TransferToAccountCode = "transferTo",
                    TransferToExchangeMarket = Product.CashBalance
                },
                ResponseCodeDetail = null // set this as needed
            };
            _mockBackofficeQueries.Setup(service =>
                    service.GetTransferCashByTransactionNo(transactionNo, It.IsAny<CancellationToken>()))
                .ReturnsAsync(transactionDetailResult);

            // Act
            var result = await _controller.TransferCashByTransactionNo(transactionNo);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<ApiResponse<TransferCashDetailResponse>>(okResult.Value);
            Assert.Equal(transactionNo, returnValue.Data.TransactionNo);
        }
    }
}