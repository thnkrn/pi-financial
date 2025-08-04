using MassTransit;
using Microsoft.Extensions.Logging;
using Moq;
using Pi.BackofficeService.Application.Commands.Ticket;
using Pi.BackofficeService.Application.Models;
using Pi.BackofficeService.Infrastructure.Services;
using Pi.Client.OnboardService.Api;
using Pi.Client.OnboardService.Client;
using Pi.Client.OnboardService.Model;
using System;
using static System.Collections.Specialized.BitVector32;
using OpenAccountInfoDto = Pi.Client.OnboardService.Model.PiOnboardServiceAPIModelsOpenAccountOpenAccountInfoDto;


namespace Pi.BackofficeService.Infrastructure.Tests.Services
{
    public class OnboardServiceTest
    {
        private readonly Mock<IOpenAccountApi> _openAccountApi;
        private readonly Mock<IAtsApi> _atsApi = new();
        private readonly Mock<ILogger<OnboardService>> _logger;

        private PiOnboardServiceAPIModelsOpenAccountOpenAccountInfoDtoListApiPaginateResponse _expectedAccounts;

        private OpenAccountInfoDto _expectedAccount1;
        private OpenAccountInfoDto _expectedAccount2;

        private PiOnboardServiceAPIModelsOpenAccountOpenAccountInfoDtoListApiPaginateResponse _expectedInvalidAccounts;

        public OnboardServiceTest()
        {
            _logger = new Mock<ILogger<OnboardService>>();
            _openAccountApi = new Mock<IOpenAccountApi>();
            _expectedAccount1 = new OpenAccountInfoDto()
            {
                Identification = new PiOnboardServiceApplicationModelsOpenAccountIdentification()
                {
                    CitizenId = "456",
                    DateOfBirth = "11/09/2023",
                    FirstNameEn = "John",
                    LastNameEn = "Smith",
                    FirstNameTh = "สมชาย",
                    LastNameTh = "ชัยดี",
                    LaserCode = "123",
                    Title = "Mr",

                },
                Documents = new List<PiClientUserServiceModelPiUserApplicationModelsDocumentDocumentDto>()
                {
                    new PiClientUserServiceModelPiUserApplicationModelsDocumentDocumentDto(PiClientUserServiceModelPiUserApplicationModelsDocumentDocumentDto.DocumentTypeEnum.BookBank, "Url 1", "Test1.jpg"),
                    new PiClientUserServiceModelPiUserApplicationModelsDocumentDocumentDto(PiClientUserServiceModelPiUserApplicationModelsDocumentDocumentDto.DocumentTypeEnum.CitizenCard, "Url 2", "Test2.jpg"),
                }
            };

            _expectedAccount2 = new OpenAccountInfoDto()
            {
                Identification = new PiOnboardServiceApplicationModelsOpenAccountIdentification()
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
                Documents = new List<PiClientUserServiceModelPiUserApplicationModelsDocumentDocumentDto>()
                {
                    new PiClientUserServiceModelPiUserApplicationModelsDocumentDocumentDto(PiClientUserServiceModelPiUserApplicationModelsDocumentDocumentDto.DocumentTypeEnum.BookBank, "Url 3", "Test1.jpg"),
                    new PiClientUserServiceModelPiUserApplicationModelsDocumentDocumentDto(PiClientUserServiceModelPiUserApplicationModelsDocumentDocumentDto.DocumentTypeEnum.CitizenCard, "Url 4", "Test2.jpg"),
                }
            };


            _expectedAccounts = new PiOnboardServiceAPIModelsOpenAccountOpenAccountInfoDtoListApiPaginateResponse()
            {
                Data = new List<OpenAccountInfoDto>()
                {
                    _expectedAccount1,
                    _expectedAccount2,
                },
                Page = 2,
                PageSize = 100,
                OrderBy = "Date",
                OrderDir = "Desc",
                Total = 100
            };

            _expectedInvalidAccounts = new PiOnboardServiceAPIModelsOpenAccountOpenAccountInfoDtoListApiPaginateResponse()
            {
                Data = new List<OpenAccountInfoDto>()
                {
                    new OpenAccountInfoDto(),
                    new OpenAccountInfoDto(),
                },
                Page = 2,
                PageSize = 100,
                OrderBy = "Date",
                OrderDir = "Desc",
                Total = 100
            };
        }

        [Fact]


        public async Task Should_Return_ListOfAccounts_When_Valid_Data_Is_Passed_In()
        {
            //expected data to pass in,
            var pageNum = 2;
            var pageSize = 10;
            var orderBy = "Date";
            var orderDir = "Asc";
            var filters = new OnboardingAccountListFilter(Guid.NewGuid(), null, null, null, null, false);

            _openAccountApi.Setup(x => x.InternalGetOpenAccountListAsync(filters.Status, filters.CitizenId, filters.Custcode, filters.UserId, filters.Date, filters.BpmReceived, pageNum, pageSize, orderBy, orderDir, It.IsAny<CancellationToken>()))
                .ReturnsAsync(_expectedAccounts);

            var service = new OnboardService(_openAccountApi.Object, _atsApi.Object, _logger.Object);

            //ACT
            var response = await service.GetOpenAccounts(pageNum, pageSize, orderBy, orderDir, filters);

            // ASSERT
            _openAccountApi.Verify(p => p.InternalGetOpenAccountListAsync(filters.Status, filters.CitizenId, filters.Custcode, filters.UserId, filters.Date, filters.BpmReceived, pageNum, pageSize, orderBy, orderDir, new CancellationToken()), Times.Once());
            Assert.NotNull(response);
            Assert.Equal(_expectedAccounts.Data.Count, response.OpenAccountInfos.Count);
        }

        [Fact]
        public async Task Should_Throw_An_Error_When_API_Throws_APIException()
        {
            //expected data to pass in,
            var pageNum = 2;
            var pageSize = 10;
            var orderBy = "Date";
            var orderDir = "Asc";
            var filters = new OnboardingAccountListFilter(Guid.NewGuid(), null, null, null, null, null);

            _openAccountApi.Setup(x => x.InternalGetOpenAccountListAsync(filters.Status, filters.CitizenId, filters.Custcode, filters.UserId, filters.Date, filters.BpmReceived, pageNum, pageSize, orderBy, orderDir, It.IsAny<CancellationToken>()))
            .Throws(new ApiException(404, "Unit Test Expected Error"));

            var service = new OnboardService(_openAccountApi.Object, _atsApi.Object, _logger.Object);

            var action = async () => await service.GetOpenAccounts(pageNum, pageSize, orderBy, orderDir, filters);

            // ACT and Assert
            var exception = await Assert.ThrowsAsync<ApiException>(action);

            //Assert.Equal("Error - calling InternalGetOpenAccountListAsync", exception.Message);
        }

        [Fact]
        public async Task Should_Throw_An_Error_When_Object_Couldnt_Convert_To_DTO()
        {
            //expected data to pass in,
            var pageNum = 2;
            var pageSize = 10;
            var orderBy = "Date";
            var orderDir = "Asc";
            var filters = new OnboardingAccountListFilter(Guid.NewGuid(), null, null, null, null, null);

            _openAccountApi.Setup(x => x.InternalGetOpenAccountListAsync(filters.Status, filters.CitizenId, filters.Custcode, filters.UserId, filters.Date, filters.BpmReceived, pageNum, pageSize, orderBy, orderDir, It.IsAny<CancellationToken>()))
                .ReturnsAsync(_expectedInvalidAccounts);

            var service = new OnboardService(_openAccountApi.Object, _atsApi.Object, _logger.Object);

            // ACT and Assert
            var action = async () => await service.GetOpenAccounts(pageNum, pageSize, orderBy, orderDir, filters);
            var exception = await Assert.ThrowsAsync<NullReferenceException>(action);
            Assert.IsType<NullReferenceException>(exception);
        }

        [Fact]
        public async Task Should_Return_ListOfAccounts_When_Searching_by_userid_and_valid_Data_Is_Passed_In()
        {
            //expected data to pass in,
            var userid = Guid.NewGuid().ToString();
            var apiResponse = new PiOnboardServiceAPIModelsOpenAccountOpenAccountInfoDtoListApiResponse(new List<OpenAccountInfoDto>()
            {
                _expectedAccount1,
                _expectedAccount2
            });

            _openAccountApi.Setup(x => x.InternalGetOpenAccountAsync(userid, It.IsAny<CancellationToken>()))
                .ReturnsAsync(apiResponse);

            var service = new OnboardService(_openAccountApi.Object, _atsApi.Object, _logger.Object);

            //ACT
            var response = await service.GetOpenAccounts(userid);

            // ASSERT
            _openAccountApi.Verify(p => p.InternalGetOpenAccountAsync(userid, new CancellationToken()), Times.Once());
            Assert.NotNull(response);
            Assert.Equal(_expectedAccounts.Data.Count, response.Count);
        }

        [Fact]
        public async Task Should_Throw_An_Error_When_Searching_by_userid_and_API_Throws_APIException()
        {
            //expected data to pass in,
            var userid = Guid.NewGuid().ToString();

            _openAccountApi.Setup(x => x.InternalGetOpenAccountAsync(userid, It.IsAny<CancellationToken>()))
                .Throws(new ApiException(404, "Unit Test Expected Error"));

            var service = new OnboardService(_openAccountApi.Object, _atsApi.Object, _logger.Object);

            var action = async () => await service.GetOpenAccounts(userid);

            // ACT and Assert
            var exception = await Assert.ThrowsAsync<ApiException>(action);
            Assert.IsType<ApiException>(exception);
        }

        [Fact]
        public async Task Should_Throw_An_Error_When_Searching_by_userid_and_Object_Couldnt_Convert_To_DTO()
        {
            //expected data to pass in,
            var userid = Guid.NewGuid().ToString();
            var apiResponse = new PiOnboardServiceAPIModelsOpenAccountOpenAccountInfoDtoListApiResponse(new List<OpenAccountInfoDto>()
            {
                new OpenAccountInfoDto(),
                new OpenAccountInfoDto(),
            });

            _openAccountApi.Setup(x => x.InternalGetOpenAccountAsync(userid, It.IsAny<CancellationToken>()))
                .ReturnsAsync(apiResponse);

            var service = new OnboardService(_openAccountApi.Object, _atsApi.Object, _logger.Object);

            // ACT and Assert
            var action = async () => await service.GetOpenAccounts(userid);
            var exception = await Assert.ThrowsAsync<NullReferenceException>(action);
            Assert.IsType<NullReferenceException>(exception);
        }
    }
}
