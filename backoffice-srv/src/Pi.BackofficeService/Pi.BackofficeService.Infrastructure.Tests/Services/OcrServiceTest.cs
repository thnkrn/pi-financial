using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using Pi.BackofficeService.Application.Models;
using Pi.BackofficeService.Infrastructure.Services;
using System.Net;
using System.Text;

namespace Pi.BackofficeService.Infrastructure.Tests.Services
{
    public class OcrServiceTest
    {
        private readonly Mock<ILogger<OcrService>> _logger;
        private readonly string testContent1;
        private readonly string testContent2;
        private readonly string testContent3;

        public OcrServiceTest()
        {
            _logger = new Mock<ILogger<OcrService>>();
            testContent1 = "{\"balance_data\":{\"2023-05-01\":65428.73,\"2023-05-02\":65428.73,\"2023-05-03\":65795.66,},\"ocr_confidence\":0.99,\"metadata\":{\"account_number\":\"XXX-1-38924-X\",\"branch\":\"สาขาหนองประทีปเชียงใหม่\",\"bank\":\"Krungsri\"}}";
            testContent2 = "{\r\n    \"data\": \"account_number,branch,bank,Date1,EOD Balance1,Date2,EOD Balance2,Date3,EOD Balance3\\nXXX-1-38924-X,สาขาหนองประทีป เชียงใหม่,Krungsri,,,,,,\\n,,,,,,,,\\n,,,2023-02-08,142.2,2023-03-10,83.72,2023-04-09,138578.8\\n,,,2023-02-09,142.2,2023-03-11,100083.72,2023-04-10,138578.8\\n,,,2023-02-10,142.2,2023-03-12,100083.72,2023-04-11,119736.8\\n,,,2023-02-11,142.2,2023-03-13,100083.72,2023-04-12,119736.8\\n,,,2023-02-12,83.72,2023-03-14,100083.72,2023-04-13,119736.8\\n,,,2023-02-13,83.72,2023-03-15,100083.72,2023-04-14,119436.8\\n,,,2023-02-14,83.72,2023-03-16,100083.72,2023-04-15,118694.8\\n,,,2023-02-15,83.72,2023-03-17,100083.72,2023-04-16,85408.73\\n,,,2023-02-16,83.72,2023-03-18,100083.72,2023-04-17,85048.73\\n,,,2023-02-17,83.72,2023-03-19,100083.72,2023-04-18,85048.73\\n,,,2023-02-18,83.72,2023-03-20,100083.72,2023-04-19,85048.73\\n,,,2023-02-19,83.72,2023-03-21,100083.72,2023-04-20,85048.73\\n,,,2023-02-20,83.72,2023-03-22,100083.72,2023-04-21,85048.73\\n,,,2023-02-21,83.72,2023-03-23,100083.72,2023-04-22,83586.73\\n,,,2023-02-22,83.72,2023-03-24,100083.72,2023-04-23,72031.73\\n,,,2023-02-23,83.72,2023-03-25,100083.72,2023-04-24,71531.73\\n,,,2023-02-24,83.72,2023-03-26,100083.72,2023-04-25,70831.73\\n,,,2023-02-25,83.72,2023-03-27,100083.72,2023-04-26,70831.73\\n,,,2023-02-26,83.72,2023-03-28,100083.72,2023-04-27,70831.73\\n,,,2023-02-27,83.72,2023-03-29,100083.72,2023-04-28,70013.73\\n,,,2023-02-28,83.72,2023-03-30,95083.72,2023-04-29,66013.73\\n,,,2023-03-01,83.72,2023-03-31,95083.72,2023-04-30,65428.73\\n,,,2023-03-02,83.72,2023-04-01,95083.72,2023-05-01,65428.73\\n,,,2023-03-03,83.72,2023-04-02,95083.72,2023-05-02,65428.73\\n,,,2023-03-04,83.72,2023-04-03,88586.72,2023-05-03,65795.66\\n,,,2023-03-05,83.72,2023-04-04,81856.8,2023-05-04,64193.66\\n,,,2023-03-06,83.72,2023-04-05,139856.8,2023-05-05,62931.06\\n,,,2023-03-07,83.72,2023-04-06,139856.8,2023-05-06,62571.06\\n,,,2023-03-08,83.72,2023-04-07,139556.8,2023-05-07,62571.06\\n,,,2023-03-09,83.72,2023-04-08,139556.8,2023-05-08,58251.06\\n\",\r\n    \"ocr_confidence\": 0.99,\r\n}";
            testContent3 = "{\r\n    \"error\": \"File type not allowed\"\r\n}";
        }

        [Fact]
        public async Task Should_Return_List_Of_BankStatement_When_Valid_Data_Is_Passed_In()
        {
            var mockMessageHandler = new Mock<HttpMessageHandler>();
            mockMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(testContent1)
                });
            var underTest = new OcrService(new HttpClient(mockMessageHandler.Object) { BaseAddress = new Uri("https://www.mock.url") }, _logger.Object);


            var docs = new List<OcrFileUploadModel>();
            docs.Add(new OcrFileUploadModel(Encoding.ASCII.GetBytes("Unit Test File 1"), "File1.pdf"));
            docs.Add(new OcrFileUploadModel(Encoding.ASCII.GetBytes("Unit Test File 2"), "File2.jpg"));

            // Act
            var result = await underTest.ScanDocumentsAsync(OcrDocumentType.BankStatement, docs, OcrOutputType.Data, null);

            // Assert
            Assert.NotNull(result);
            Assert.Null(result.Error);
            Assert.NotNull(result.BalanceData);
            Assert.Equal(3, result.BalanceData.Count);

            Assert.Equal("2023-05-01", result.BalanceData.ElementAt(0).Key);
            Assert.Equal("65428.73", result.BalanceData.ElementAt(0).Value);

            Assert.Equal("2023-05-02", result.BalanceData.ElementAt(1).Key);
            Assert.Equal("65428.73", result.BalanceData.ElementAt(1).Value);

            Assert.Equal("2023-05-03", result.BalanceData.ElementAt(2).Key);
            Assert.Equal("65795.66", result.BalanceData.ElementAt(2).Value);

            Assert.Equal(0.99, result.OcrConfidence);

            Assert.NotNull(result.Metadata);

            Assert.Equal("XXX-1-38924-X", result.Metadata["account_number"]);
            Assert.Equal("สาขาหนองประทีปเชียงใหม่", result.Metadata["branch"]);
            Assert.Equal("Krungsri", result.Metadata["bank"]);
        }

        [Fact]
        public async Task Should_Return_Csv_Data_When_Csv_Is_requested()
        {
            var mockMessageHandler = new Mock<HttpMessageHandler>();
            mockMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(testContent2)
                });
            var underTest = new OcrService(new HttpClient(mockMessageHandler.Object) { BaseAddress = new Uri("https://www.mock.url") }, _logger.Object);
            var password = "password";

            var docs = new List<OcrFileUploadModel>();

            // Act
            var result = await underTest.ScanDocumentsAsync(OcrDocumentType.BankStatement, docs, OcrOutputType.Csv, password);

            // Assert
            Assert.Null(result.Error);
            Assert.NotNull(result.Data);
            Assert.Equal(0.99, result.OcrConfidence);
        }

        [Fact]
        public async Task Should_Throw_An_Exception_When_Not_Supported_File_Is_Sent()
        {
            var mockMessageHandler = new Mock<HttpMessageHandler>();
            mockMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.UnprocessableEntity,
                    Content = new StringContent(testContent3)
                });
            var underTest = new OcrService(new HttpClient(mockMessageHandler.Object) { BaseAddress = new Uri("https://www.mock.url") }, _logger.Object);


            var docs = new List<OcrFileUploadModel>();

            // Act
            try
            {
                var result = await underTest.ScanDocumentsAsync(OcrDocumentType.BankStatement, docs, OcrOutputType.Csv, null);
            }
            catch (Exception ex)
            {
                Assert.NotNull(ex.Message);
                Assert.Equal("OCR Error - File type not allowed", ex.Message);
            }
        }

        [Fact]
        public async Task Should_Throw_An_Exception_When_It_Failed_To_Call_OCR()
        {
            var exception = new Exception();
            var mockMessageHandler = new Mock<HttpMessageHandler>();
            mockMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .Throws(new IOException("Mock Error"));

            var underTest = new OcrService(new HttpClient(mockMessageHandler.Object) { BaseAddress = new Uri("https://www.mock.url") }, _logger.Object);


            var docs = new List<OcrFileUploadModel>();

            // Act
            try
            {
                var result = await underTest.ScanDocumentsAsync(OcrDocumentType.BankStatement, docs, OcrOutputType.Csv, null);
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            Assert.NotNull(exception.Message);
            Assert.Equal("Mock Error", exception.Message);


        }
    }
}
