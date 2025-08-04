using Moq;
using Pi.BackofficeService.Application.Models;
using Pi.BackofficeService.Application.Queries;
using Pi.BackofficeService.API.Controllers;

namespace Pi.BackofficeService.API.Tests.Controllers
{
    public class DocumentPortalControllerTest
    {
        private readonly Mock<IOnboardingQueries> _onboardingQueries;

        public DocumentPortalControllerTest()
        {
            _onboardingQueries = new Mock<IOnboardingQueries>();
        }

        [Fact]
        public async Task Should_Return_ListOfAccounts_When_Api_Is_Called_Successfully_StatusCodeOk()
        {
            var expectedCustcode = "123";
            var expectedApiResult = new List<OpenAccountInfoDto>();

            _onboardingQueries.Setup(x => x.GetOpenAccountsByCustCode(expectedCustcode))
                .ReturnsAsync(expectedApiResult);

            var controller = new DocumentPortalController(_onboardingQueries.Object);

            //ACT
            var response = await controller.OnboardingOpenAccounts(expectedCustcode);

            // ASSERT
            _onboardingQueries.Verify(p => p.GetOpenAccountsByCustCode(expectedCustcode), Times.Once());
            Assert.NotNull(response);

        }

        [Fact]
        public async Task Should_Return_Error_When_Custcode_Failed_To_Return_UserData()
        {
            var expectedCustcode = "123";
            _onboardingQueries.Setup(x => x.GetOpenAccountsByCustCode(expectedCustcode))
                .ReturnsAsync((List<OpenAccountInfoDto>?)null);

            var controller = new DocumentPortalController(_onboardingQueries.Object);

            //ACT
            var response = await controller.OnboardingOpenAccounts(expectedCustcode);

            // ASSERT
            _onboardingQueries.Verify(p => p.GetOpenAccountsByCustCode(expectedCustcode), Times.Once());
            Assert.NotNull(response);

            var responseObj = ((Microsoft.AspNetCore.Mvc.ObjectResult)response).Value as Microsoft.AspNetCore.Mvc.ProblemDetails;
            Assert.NotNull(responseObj);
            Assert.Equal(404, responseObj.Status);
            Assert.Equal("Sorry Customer Code not found, please try again", responseObj.Detail);
        }
    }
}
