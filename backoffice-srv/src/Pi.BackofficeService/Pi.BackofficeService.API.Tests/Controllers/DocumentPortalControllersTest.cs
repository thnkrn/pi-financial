using CsvHelper.Configuration.Attributes;
using Moq;
using Pi.BackofficeService.Application.Models;
using Pi.BackofficeService.Application.Queries;
using Pi.BackofficeService.API.Controllers;
using Pi.BackofficeService.API.Factories;
using Pi.Common.Http;
using Pi.BackofficeService.API.Models;

namespace Pi.BackofficeService.API.Tests.Controllers
{
    public class OnboardingControllersTest
    {
        private readonly Mock<IOnboardingQueries> _onboardingQueries;

        private OpenAccountInfoDto _expectedResult1;
        private OpenAccountInfoDto _expectedResult2;

        public OnboardingControllersTest()
        {
            _onboardingQueries = new Mock<IOnboardingQueries>();
            _expectedResult1 = new OpenAccountInfoDto()
            {
                BpmReceived = true,
                CreatedDate = "23/Sept/2023",
                CustCode = "123",
                Id = "123",
                Status = "Pending",
                Identification = new Identification()
                {
                    CitizenId = "456",
                    DateOfBirth = "11/09/2023",
                    FirstNameEn = "John",
                    LastNameEn = "Smith",
                    FirstNameTh = "สมชาย",
                    LastNameTh = "ชัยดี",
                    LaserCode = "123",
                    Title = "Mr"
                },
                Documents = new List<Document>()
                {
                    new Document("Url 1", "Test1.jpg", "Passport"),
                    new Document("Url 2", "Test2.jpg", "Selfie"),
                }
            };

            _expectedResult2 = new OpenAccountInfoDto()
            {
                BpmReceived = true,
                CreatedDate = "23/Jan/2023",
                CustCode = "111",
                Id = "222",
                Status = "Submitted",
                Identification = new Identification()
                {
                    CitizenId = "333",
                    DateOfBirth = "11/09/2023",
                    FirstNameEn = "Ellen",
                    LastNameEn = "Bell",
                    FirstNameTh = "อาทิตย์",
                    LastNameTh = "น้อย",
                    LaserCode = "444",
                    Title = "Mr"
                },
                Documents = new List<Document>()
                {
                    new Document("Url 3", "Test3.jpg", "BankStatement"),
                    new Document("Url 4", "Test4.jpg", "Passport"),
                }
            };
        }

        [Fact]
        public async Task Should_Return_ListOfAccounts_When_Valid_Data_Is_Passed_Through()
        {
            var request = new Models.OnBoardingFilterRequest()
            {
                UserId = Guid.NewGuid(),
                Status = OpenAccountRequestStatus.PENDING,
                CitizenId = "456",
                Custcode = "789",
                Date = null,
                Page = 2,
                PageSize = 10,
                OrderBy = "custcode",
                OrderDir = "Desc"
            };

            var expectedAccounts = new List<OpenAccountInfoDto>()
            {
                _expectedResult1,
                _expectedResult2
            };

            var filterModel = DtoFactory.NewOnboardAccountFilter(request);
            var expectedApiResult = new PaginateOpenAccountListResult(expectedAccounts, request.Page, request.PageSize, 20, request.OrderBy, request.OrderDir);

            _onboardingQueries.Setup(x => x.GetOpenAccountsPaginate(request.Page, request.PageSize, request.OrderBy, request.OrderDir, filterModel))
                .ReturnsAsync(expectedApiResult);

            var controller = new OnboardingController(_onboardingQueries.Object);

            //ACT
            var response = await controller.OnboardingOpenAccounts(request);

            // ASSERT
            _onboardingQueries.Verify(p => p.GetOpenAccountsPaginate(request.Page, request.PageSize, request.OrderBy, request.OrderDir, filterModel), Times.Once());
            Assert.NotNull(response);
            var responseObj = ((Microsoft.AspNetCore.Mvc.OkObjectResult)response).Value as ApiPaginateResponse<List<OnboardingOpenAccountResponse>>;

            Assert.NotNull(responseObj);
            Assert.Equal(expectedAccounts.Count, responseObj.Data.Count);
        }

        [Fact]
        public async Task Should_Return_1_Account_When_Api_Is_Called_Successfully()
        {
            var request = new Models.OnBoardingFilterRequest()
            {
                UserId = Guid.NewGuid(),
                Status = OpenAccountRequestStatus.PENDING,
                CitizenId = "456",
                Custcode = "789",
                Date = null,
                Page = 2,
                PageSize = 10,
                OrderBy = "custcode",
                OrderDir = "Desc"
            };

            var expectedAccounts = new List<OpenAccountInfoDto>()
            {
                _expectedResult1,
            };

            var filterModel = DtoFactory.NewOnboardAccountFilter(request);
            var expectedApiResult = new PaginateOpenAccountListResult(expectedAccounts, request.Page, request.PageSize, 20, request.OrderBy, request.OrderDir);

            _onboardingQueries.Setup(x => x.GetOpenAccountsPaginate(request.Page, request.PageSize, request.OrderBy, request.OrderDir, filterModel))
                .ReturnsAsync(expectedApiResult);

            var controller = new OnboardingController(_onboardingQueries.Object);

            //ACT
            var response = await controller.OnboardingOpenAccounts(request);

            // ASSERT
            _onboardingQueries.Verify(p => p.GetOpenAccountsPaginate(request.Page, request.PageSize, request.OrderBy, request.OrderDir, filterModel), Times.Once());
            Assert.NotNull(response);
            var responseObj = ((Microsoft.AspNetCore.Mvc.OkObjectResult)response).Value as ApiPaginateResponse<List<OnboardingOpenAccountResponse>>;

            Assert.NotNull(responseObj);
            Assert.Equal(expectedAccounts.Count, responseObj.Data.Count);
        }

        [Fact]
        public async Task Should_Return_Multiple_Account_Details_When_Valid_Filter_Is_Passed()
        {
            var request = new Models.OnBoardingFilterRequest()
            {
                UserId = Guid.NewGuid(),
                Status = OpenAccountRequestStatus.PENDING,
                CitizenId = "456",
                Custcode = "789",
                Date = null,
                Page = 2,
                PageSize = 10,
                OrderBy = "custcode",
                OrderDir = "Desc"
            };

            var expectedAccounts = new List<OpenAccountInfoDto>()
            {
                _expectedResult1,
                _expectedResult2
            };

            var filterModel = DtoFactory.NewOnboardAccountFilter(request);
            var expectedApiResult = new PaginateOpenAccountListResult(expectedAccounts, request.Page, request.PageSize, 20, request.OrderBy, request.OrderDir);

            _onboardingQueries.Setup(x => x.GetOpenAccountsPaginate(request.Page, request.PageSize, request.OrderBy, request.OrderDir, filterModel))
                .ReturnsAsync(expectedApiResult);

            var controller = new OnboardingController(_onboardingQueries.Object);

            //ACT
            var response = await controller.OnboardingOpenAccounts(request);

            // ASSERT
            _onboardingQueries.Verify(p => p.GetOpenAccountsPaginate(request.Page, request.PageSize, request.OrderBy, request.OrderDir, filterModel), Times.Once());
            Assert.NotNull(response);
            var responseObj = ((Microsoft.AspNetCore.Mvc.OkObjectResult)response).Value as ApiPaginateResponse<List<OnboardingOpenAccountResponse>>;

            Assert.NotNull(responseObj);

            var actualAccount = responseObj.Data.FirstOrDefault(x => x.Id == _expectedResult1.Id);
            Assert.NotNull(actualAccount);
            Assert.Equal(_expectedResult1.Id, actualAccount.Id);
            Assert.Equal(_expectedResult1.Status, actualAccount.Status);
            Assert.Equal(_expectedResult1.BpmReceived, actualAccount.BpmReceived);
            Assert.Equal(_expectedResult1.CreatedDate, actualAccount.CreatedDate);
            //Assert.Equal(_expectedResult1.CustCode, actualAccount.CustCode);

            actualAccount = responseObj.Data.FirstOrDefault(x => x.Id == _expectedResult2.Id);
            Assert.NotNull(actualAccount);
            Assert.Equal(_expectedResult2.Id, actualAccount.Id);
            Assert.Equal(_expectedResult2.Status, actualAccount.Status);
            Assert.Equal(_expectedResult2.BpmReceived, actualAccount.BpmReceived);
            Assert.Equal(_expectedResult2.CreatedDate, actualAccount.CreatedDate);
        }

        [Fact]
        public async Task Should_Return_Single_Account_With_Valid_Identification_Details_When_Valid_Data_Is_Passed()
        {
            var request = new Models.OnBoardingFilterRequest()
            {
                UserId = Guid.NewGuid(),
                Status = OpenAccountRequestStatus.PENDING,
                CitizenId = "456",
                Custcode = "789",
                Date = null,
                Page = 2,
                PageSize = 10,
                OrderBy = "custcode",
                OrderDir = "Desc"
            };

            Assert.NotNull(_expectedResult1);
            Assert.NotNull(_expectedResult1.Identification);

            var expectedAccounts = new List<OpenAccountInfoDto>()
            {
                _expectedResult1,
            };

            var filterModel = DtoFactory.NewOnboardAccountFilter(request);
            var expectedApiResult = new PaginateOpenAccountListResult(expectedAccounts, request.Page, request.PageSize, 20, request.OrderBy, request.OrderDir);

            _onboardingQueries.Setup(x => x.GetOpenAccountsPaginate(request.Page, request.PageSize, request.OrderBy, request.OrderDir, filterModel))
                .ReturnsAsync(expectedApiResult);

            var controller = new OnboardingController(_onboardingQueries.Object);

            //ACT
            var response = await controller.OnboardingOpenAccounts(request);

            // ASSERT
            _onboardingQueries.Verify(p => p.GetOpenAccountsPaginate(request.Page, request.PageSize, request.OrderBy, request.OrderDir, filterModel), Times.Once());
            Assert.NotNull(response);
            var responseObj = ((Microsoft.AspNetCore.Mvc.OkObjectResult)response).Value as ApiPaginateResponse<List<OnboardingOpenAccountResponse>>;

            Assert.NotNull(responseObj);

            var actualAccount = responseObj.Data.FirstOrDefault(x => x.Id == _expectedResult1.Id);
            Assert.NotNull(actualAccount);

            var identityObj = actualAccount.Identification;
            Assert.NotNull(identityObj);
            Assert.Equal(_expectedResult1.Identification.LaserCode, identityObj.LaserCode);
            Assert.Equal(_expectedResult1.Identification.CitizenId, identityObj.CitizenId);
            Assert.Equal(_expectedResult1.Identification.DateOfBirth, identityObj.DateOfBirth);
            Assert.Equal(_expectedResult1.Identification.FirstNameEn, identityObj.FirstNameEn);
            Assert.Equal(_expectedResult1.Identification.LastNameEn, identityObj.LastNameEn);
            Assert.Equal(_expectedResult1.Identification.FirstNameTh, identityObj.FirstNameTh);
            Assert.Equal(_expectedResult1.Identification.LastNameTh, identityObj.LastNameTh);
            Assert.Equal(_expectedResult1.Identification.LaserCode, identityObj.LaserCode);
            Assert.Equal(_expectedResult1.Identification.Title, identityObj.Title);
        }

        [Fact]
        public async Task Should_Return_Multiple_Accounts_With_Valid_Identification_Details__When_Valid_Data_Is_Passed()
        {
            var request = new Models.OnBoardingFilterRequest()
            {
                UserId = Guid.NewGuid(),
                Status = OpenAccountRequestStatus.PENDING,
                CitizenId = "456",
                Custcode = "789",
                Date = null,
                Page = 2,
                PageSize = 10,
                OrderBy = "custcode",
                OrderDir = "Desc"
            };

            Assert.NotNull(_expectedResult2);
            Assert.NotNull(_expectedResult2.Identification);

            var expectedAccounts = new List<OpenAccountInfoDto>()
            {
                _expectedResult1,
                _expectedResult2
            };

            var filterModel = DtoFactory.NewOnboardAccountFilter(request);
            var expectedApiResult = new PaginateOpenAccountListResult(expectedAccounts, request.Page, request.PageSize, 20, request.OrderBy, request.OrderDir);

            _onboardingQueries.Setup(x => x.GetOpenAccountsPaginate(request.Page, request.PageSize, request.OrderBy, request.OrderDir, filterModel))
                .ReturnsAsync(expectedApiResult);

            var controller = new OnboardingController(_onboardingQueries.Object);

            //ACT
            var response = await controller.OnboardingOpenAccounts(request);

            // ASSERT
            _onboardingQueries.Verify(p => p.GetOpenAccountsPaginate(request.Page, request.PageSize, request.OrderBy, request.OrderDir, filterModel), Times.Once());
            Assert.NotNull(response);
            var responseObj = ((Microsoft.AspNetCore.Mvc.OkObjectResult)response).Value as ApiPaginateResponse<List<OnboardingOpenAccountResponse>>;

            Assert.NotNull(responseObj);

            var actualAccount = responseObj.Data.FirstOrDefault(x => x.Id == _expectedResult2.Id);
            Assert.NotNull(actualAccount);

            var identityObj = actualAccount.Identification;
            Assert.NotNull(identityObj);
            Assert.Equal(_expectedResult2.Identification.LaserCode, identityObj.LaserCode);
            Assert.Equal(_expectedResult2.Identification.CitizenId, identityObj.CitizenId);
            Assert.Equal(_expectedResult2.Identification.DateOfBirth, identityObj.DateOfBirth);
            Assert.Equal(_expectedResult2.Identification.FirstNameEn, identityObj.FirstNameEn);
            Assert.Equal(_expectedResult2.Identification.LastNameEn, identityObj.LastNameEn);
            Assert.Equal(_expectedResult2.Identification.FirstNameTh, identityObj.FirstNameTh);
            Assert.Equal(_expectedResult2.Identification.LastNameTh, identityObj.LastNameTh);
            Assert.Equal(_expectedResult2.Identification.LaserCode, identityObj.LaserCode);
            Assert.Equal(_expectedResult2.Identification.Title, identityObj.Title);
        }

        [Fact]
        public async Task Should_Return_Valid_Document_Details_When_One_Account_Found()
        {
            var request = new Models.OnBoardingFilterRequest()
            {
                UserId = Guid.NewGuid(),
                Status = OpenAccountRequestStatus.PENDING,
                CitizenId = "456",
                Custcode = "789",
                Date = null,
                Page = 2,
                PageSize = 10,
                OrderBy = "custcode",
                OrderDir = "Desc"
            };

            Assert.NotNull(_expectedResult1);
            Assert.NotNull(_expectedResult1.Documents);

            var expectedAccounts = new List<OpenAccountInfoDto>()
            {
                _expectedResult1,
            };

            var filterModel = DtoFactory.NewOnboardAccountFilter(request);
            var expectedApiResult = new PaginateOpenAccountListResult(expectedAccounts, request.Page, request.PageSize, 20, request.OrderBy, request.OrderDir);

            _onboardingQueries.Setup(x => x.GetOpenAccountsPaginate(request.Page, request.PageSize, request.OrderBy, request.OrderDir, filterModel))
                .ReturnsAsync(expectedApiResult);

            var controller = new OnboardingController(_onboardingQueries.Object);

            //ACT
            var response = await controller.OnboardingOpenAccounts(request);

            // ASSERT
            _onboardingQueries.Verify(p => p.GetOpenAccountsPaginate(request.Page, request.PageSize, request.OrderBy, request.OrderDir, filterModel), Times.Once());
            Assert.NotNull(response);
            var responseObj = ((Microsoft.AspNetCore.Mvc.OkObjectResult)response).Value as ApiPaginateResponse<List<OnboardingOpenAccountResponse>>;

            Assert.NotNull(responseObj);

            var actualAccount = responseObj.Data.FirstOrDefault(x => x.Id == _expectedResult1.Id);
            Assert.NotNull(actualAccount);

            var identityObj = actualAccount.Documents;
            Assert.NotNull(identityObj);
            Assert.Equal(_expectedResult1.Documents.Count, identityObj.Count);

            var doc1 = identityObj[0];
            Assert.Equal(_expectedResult1.Documents[0].Url, doc1.Url);
            Assert.Equal(_expectedResult1.Documents[0].DocumentType, doc1.DocumentType);
            Assert.Equal(_expectedResult1.Documents[0].FileName, doc1.FileName);

            var doc2 = identityObj[1];
            Assert.Equal(_expectedResult1.Documents[1].Url, doc2.Url);
            Assert.Equal(_expectedResult1.Documents[1].DocumentType, doc2.DocumentType);
            Assert.Equal(_expectedResult1.Documents[1].FileName, doc2.FileName);

        }

        [Fact]
        public async Task Should_Return_Valid_Document_Details_When_Multiple_Account_Found()
        {
            var request = new Models.OnBoardingFilterRequest()
            {
                UserId = Guid.NewGuid(),
                Status = OpenAccountRequestStatus.PENDING,
                CitizenId = "456",
                Custcode = "789",
                Date = null,
                Page = 2,
                PageSize = 10,
                OrderBy = "custcode",
                OrderDir = "Desc"
            };

            Assert.NotNull(_expectedResult2);
            Assert.NotNull(_expectedResult2.Documents);

            var expectedAccounts = new List<OpenAccountInfoDto>()
            {
                _expectedResult1,
                _expectedResult2
            };

            var filterModel = DtoFactory.NewOnboardAccountFilter(request);
            var expectedApiResult = new PaginateOpenAccountListResult(expectedAccounts, request.Page, request.PageSize, 20, request.OrderBy, request.OrderDir);

            _onboardingQueries.Setup(x => x.GetOpenAccountsPaginate(request.Page, request.PageSize, request.OrderBy, request.OrderDir, filterModel))
                .ReturnsAsync(expectedApiResult);

            var controller = new OnboardingController(_onboardingQueries.Object);

            //ACT
            var response = await controller.OnboardingOpenAccounts(request);

            // ASSERT
            _onboardingQueries.Verify(p => p.GetOpenAccountsPaginate(request.Page, request.PageSize, request.OrderBy, request.OrderDir, filterModel), Times.Once());
            Assert.NotNull(response);
            var responseObj = ((Microsoft.AspNetCore.Mvc.OkObjectResult)response).Value as ApiPaginateResponse<List<OnboardingOpenAccountResponse>>;

            Assert.NotNull(responseObj);

            var actualAccount = responseObj.Data.FirstOrDefault(x => x.Id == _expectedResult2.Id);
            Assert.NotNull(actualAccount);

            var identityObj = actualAccount.Documents;
            Assert.NotNull(identityObj);
            Assert.Equal(_expectedResult2.Documents.Count, identityObj.Count);

            var doc1 = identityObj[0];
            Assert.Equal(_expectedResult2.Documents[0].Url, doc1.Url);
            Assert.Equal(_expectedResult2.Documents[0].DocumentType, doc1.DocumentType);
            Assert.Equal(_expectedResult2.Documents[0].FileName, doc1.FileName);

            var doc2 = identityObj[1];
            Assert.Equal(_expectedResult2.Documents[1].Url, doc2.Url);
            Assert.Equal(_expectedResult2.Documents[1].DocumentType, doc2.DocumentType);
            Assert.Equal(_expectedResult2.Documents[1].FileName, doc2.FileName);

        }

        [Fact]
        public async Task Should_Return_Error_When_Service_Throw_Unexpected_Error_StatusCodeError500()
        {
            var request = new Models.OnBoardingFilterRequest()
            {
                UserId = Guid.NewGuid(),
                Status = OpenAccountRequestStatus.PENDING,
                CitizenId = "456",
                Custcode = "789",
                Date = null,
                Page = 2,
                PageSize = 10,
                OrderBy = "custcode",
                OrderDir = "Desc"
            };

            var expectedAccounts = new List<OpenAccountInfoDto>() { };
            var filterModel = DtoFactory.NewOnboardAccountFilter(request);

            _onboardingQueries.Setup(x => x.GetOpenAccountsPaginate(request.Page, request.PageSize, request.OrderBy, request.OrderDir, filterModel))
                .ThrowsAsync(new ApplicationException("Unit Test - Error Have Occured"));

            var controller = new OnboardingController(_onboardingQueries.Object);

            //ACT
            var response = await controller.OnboardingOpenAccounts(request);

            // ASSERT
            _onboardingQueries.Verify(p => p.GetOpenAccountsPaginate(request.Page, request.PageSize, request.OrderBy, request.OrderDir, filterModel), Times.Once());
            Assert.NotNull(response);
            var responseObj = ((Microsoft.AspNetCore.Mvc.ObjectResult)response).Value as Microsoft.AspNetCore.Mvc.ProblemDetails;

            Assert.NotNull(responseObj);
            Assert.Equal(500, responseObj.Status);
            Assert.Equal("Unit Test - Error Have Occured", responseObj.Detail);
        }

        [Fact]
        public async Task Should_Return_Account_List_When_Empty_Filter_Is_Sent_Through()
        {
            var request = new Models.OnBoardingFilterRequest();

            var expectedAccounts = new List<OpenAccountInfoDto>()
            {
                _expectedResult1,
                _expectedResult2
            };

            var filterModel = DtoFactory.NewOnboardAccountFilter(request);
            var expectedApiResult = new PaginateOpenAccountListResult(expectedAccounts, request.Page, request.PageSize, 20, request.OrderBy, request.OrderDir);

            _onboardingQueries.Setup(x => x.GetOpenAccountsPaginate(request.Page, request.PageSize, request.OrderBy, request.OrderDir, filterModel))
                .ReturnsAsync(expectedApiResult);

            var controller = new OnboardingController(_onboardingQueries.Object);

            //ACT
            var response = await controller.OnboardingOpenAccounts(request);

            // ASSERT
            Assert.NotNull(_expectedResult1);
            Assert.NotNull(_expectedResult2);

            _onboardingQueries.Verify(p => p.GetOpenAccountsPaginate(request.Page, request.PageSize, request.OrderBy, request.OrderDir, filterModel), Times.Once());
            Assert.NotNull(response);
            var responseObj = ((Microsoft.AspNetCore.Mvc.OkObjectResult)response).Value as ApiPaginateResponse<List<OnboardingOpenAccountResponse>>;

            Assert.NotNull(responseObj);

            var actualAccount1 = responseObj.Data.FirstOrDefault(x => x.Id == _expectedResult1.Id);
            Assert.NotNull(actualAccount1);

            var actualAccount2 = responseObj.Data.FirstOrDefault(x => x.Id == _expectedResult2.Id);
            Assert.NotNull(actualAccount2);
        }
    }
}
