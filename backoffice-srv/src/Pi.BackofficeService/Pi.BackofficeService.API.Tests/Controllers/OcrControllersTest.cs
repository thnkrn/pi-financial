using Moq;
using Pi.BackofficeService.Application.Models;
using Pi.BackofficeService.Application.Queries;
using Pi.BackofficeService.API.Controllers;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Pi.BackofficeService.Application.Services.OcrService;

namespace Pi.BackofficeService.API.Tests.Controllers
{
    public class OcrControllersTest
    {
        private readonly Mock<IOcrService> _ocrService;

        private readonly IFormFile _file1;
        private readonly IFormFile _file2;

        private readonly OcrThirdPartyApiResponse _expectedResult1;
        private readonly OcrThirdPartyApiResponse _expectedResult2;
        public OcrControllersTest()
        {
            _ocrService = new Mock<IOcrService>();

            //Setup mock file using a memory stream
            var content = "Mocked File 1";
            var fileName = "test.pdf";
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(content);
            writer.Flush();
            stream.Position = 0;

            //create FormFile with desired data
            _file1 = new FormFile(stream, 0, stream.Length, "files", fileName);


            //Setup mock file using a memory stream
            var content2 = "Mocked File 2";
            var fileName2 = "test.pdf";
            var stream2 = new MemoryStream();
            var writer2 = new StreamWriter(stream);
            writer2.Write(content2);
            writer2.Flush();
            stream2.Position = 0;

            //create FormFile with desired data
            _file2 = new FormFile(stream2, 0, stream2.Length, "files", fileName2);

            // expected OCR results
            _expectedResult1 = new OcrThirdPartyApiResponse()
            {
                BalanceData = new Dictionary<string, string>
                {
                    { "date1", "data1" },
                    { "date2", "data2" },
                    { "date3", "data3" },
                },
                Metadata = new Dictionary<string, string>
                {
                    { "AccountNumber", "account 1" },
                    { "Bank", "bank 1" },
                    { "Branch", "branch 1" },
                },
                OcrConfidence = 0.99,
            };

            _expectedResult2 = new OcrThirdPartyApiResponse()
            {
                Data = "{\r\n    \"data\": \"account_number,branch,bank,Date1,EOD Balance1,Date2,EOD Balance2,Date3,EOD Balance3\\nXXX-1-38924-X,สาขาหนองประทีป เชียงใหม่,Krungsri,,,,,,\\n,,,,,,,,\\n,,,2023-02-08,142.2,2023-03-10,83.72,2023-04-09,138578.8\\n,,,2023-02-09,142.2,2023-03-11,100083.72,2023-04-10,138578.8\\n,,,2023-02-10,142.2,2023-03-12,100083.72,2023-04-11,119736.8\\n,,,2023-02-11,142.2,2023-03-13,100083.72,2023-04-12,119736.8\\n,,,2023-02-12,83.72,2023-03-14,100083.72,2023-04-13,119736.8\\n,,,2023-02-13,83.72,2023-03-15,100083.72,2023-04-14,119436.8\\n,,,2023-02-14,83.72,2023-03-16,100083.72,2023-04-15,118694.8\\n,,,2023-02-15,83.72,2023-03-17,100083.72,2023-04-16,85408.73\\n,,,2023-02-16,83.72,2023-03-18,100083.72,2023-04-17,85048.73\\n,,,2023-02-17,83.72,2023-03-19,100083.72,2023-04-18,85048.73\\n,,,2023-02-18,83.72,2023-03-20,100083.72,2023-04-19,85048.73\\n,,,2023-02-19,83.72,2023-03-21,100083.72,2023-04-20,85048.73\\n,,,2023-02-20,83.72,2023-03-22,100083.72,2023-04-21,85048.73\\n,,,2023-02-21,83.72,2023-03-23,100083.72,2023-04-22,83586.73\\n,,,2023-02-22,83.72,2023-03-24,100083.72,2023-04-23,72031.73\\n,,,2023-02-23,83.72,2023-03-25,100083.72,2023-04-24,71531.73\\n,,,2023-02-24,83.72,2023-03-26,100083.72,2023-04-25,70831.73\\n,,,2023-02-25,83.72,2023-03-27,100083.72,2023-04-26,70831.73\\n,,,2023-02-26,83.72,2023-03-28,100083.72,2023-04-27,70831.73\\n,,,2023-02-27,83.72,2023-03-29,100083.72,2023-04-28,70013.73\\n,,,2023-02-28,83.72,2023-03-30,95083.72,2023-04-29,66013.73\\n,,,2023-03-01,83.72,2023-03-31,95083.72,2023-04-30,65428.73\\n,,,2023-03-02,83.72,2023-04-01,95083.72,2023-05-01,65428.73\\n,,,2023-03-03,83.72,2023-04-02,95083.72,2023-05-02,65428.73\\n,,,2023-03-04,83.72,2023-04-03,88586.72,2023-05-03,65795.66\\n,,,2023-03-05,83.72,2023-04-04,81856.8,2023-05-04,64193.66\\n,,,2023-03-06,83.72,2023-04-05,139856.8,2023-05-05,62931.06\\n,,,2023-03-07,83.72,2023-04-06,139856.8,2023-05-06,62571.06\\n,,,2023-03-08,83.72,2023-04-07,139556.8,2023-05-07,62571.06\\n,,,2023-03-09,83.72,2023-04-08,139556.8,2023-05-08,58251.06\\n\",\r\n    \"ocr_confidence\": 0.99,\r\n}",
                OcrConfidence = 0.98,
            };
        }

        [Fact]
        public async Task Should_Return_BankStatementDetails_When_Api_Is_Called_Successfully_StatusCodeOk()
        {
            var arg1 = new List<IFormFile>() { _file1, _file2 };
            var arg2 = OcrDocumentType.BankStatement;
            var arg3 = OcrOutputType.Data;

            _ocrService.Setup(x => x.ScanDocumentsAsync(arg2, It.IsAny<IList<OcrFileUploadModel>>(), arg3, It.IsAny<string?>()))
                .ReturnsAsync(_expectedResult1);

            var controller = new OcrController(_ocrService.Object);

            //ACT
            var actual = await controller.OcrProcess(arg1, arg2, arg3, null);

            // ASSERT
            _ocrService.Verify(p => p.ScanDocumentsAsync(arg2, It.IsAny<IList<OcrFileUploadModel>>(), arg3, It.IsAny<string?>()), Times.Once());

            Assert.NotNull(actual);
            var result = (Microsoft.AspNetCore.Mvc.OkObjectResult)actual;
            Assert.Equal(200, result.StatusCode);
            var returnValue = result.Value as OcrThirdPartyApiResponse;

            Assert.NotNull(returnValue);
            Assert.NotNull(returnValue.BalanceData);
            Assert.NotNull(returnValue.Metadata);
            Assert.Equal(_expectedResult1.OcrConfidence, returnValue.OcrConfidence);
        }

        [Fact]
        public async Task Should_Return_Csv_BankStatementDetails_When_Api_Is_Called_Successfully_StatusCodeOk()
        {
            var arg1 = new List<IFormFile>() { _file1, _file2 };
            var arg2 = OcrDocumentType.BankStatement;
            var arg3 = OcrOutputType.Csv;

            _ocrService.Setup(x => x.ScanDocumentsAsync(arg2, It.IsAny<IList<OcrFileUploadModel>>(), arg3, It.IsAny<string?>()))
                .ReturnsAsync(_expectedResult2);

            var controller = new OcrController(_ocrService.Object);

            //ACT
            var actual = await controller.OcrProcess(arg1, arg2, arg3, null);

            // ASSERT
            _ocrService.Verify(p => p.ScanDocumentsAsync(arg2, It.IsAny<IList<OcrFileUploadModel>>(), arg3, It.IsAny<string?>()), Times.Once());

            Assert.NotNull(actual);
            var result = (Microsoft.AspNetCore.Mvc.OkObjectResult)actual;
            Assert.Equal(200, result.StatusCode);
            var returnValue = result.Value as OcrThirdPartyApiResponse;

            Assert.NotNull(returnValue);
            Assert.NotNull(returnValue.Data);
            Assert.Equal(_expectedResult2.OcrConfidence, returnValue.OcrConfidence);
        }


        [Fact]
        public async Task Should_Return_An_Error_Message_When_OCR_Thrown_An_Error_StatusCode500()
        {
            var arg1 = new List<IFormFile>() { _file1, _file2 };
            var arg2 = OcrDocumentType.BankStatement;
            var arg3 = OcrOutputType.Csv;

            _ocrService.Setup(x => x.ScanDocumentsAsync(arg2, It.IsAny<IList<OcrFileUploadModel>>(), arg3, null))
                .Throws(new ApplicationException("Mock Error Occured"));

            var controller = new OcrController(_ocrService.Object);

            //ACT
            var actual = await controller.OcrProcess(arg1, arg2, arg3, null);

            // ASSERT
            Assert.NotNull(actual);
            var result = (Microsoft.AspNetCore.Mvc.ObjectResult)actual;
            Assert.Equal(500, result.StatusCode);
            var returnValue = result.Value as ProblemDetails;
            Assert.NotNull(returnValue);
            Assert.Equal("Mock Error Occured", returnValue.Detail);
        }
    }
}
