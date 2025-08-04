using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using Pi.Client.OnboardService.Api;
using Pi.Client.OnboardService.Model;
using Pi.User.Infrastructure.Services;
using Xunit;

namespace Pi.User.Infrastructure.Tests.Services;

public class CustomerServiceTests
{
    private readonly Mock<ICustomerInfoApi> _mockCustomerInfoApi = new();
    private readonly Mock<ILogger<CustomerService>> _mockLogger = new();
    private readonly CustomerService _customerService;

    public CustomerServiceTests()
    {
        _customerService = new CustomerService(_mockCustomerInfoApi.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task GetCustomerInfoByCustomerCode_ReturnsCustomerInfo_WhenValidCustomer()
    {
        // Arrange
        var customerCode = "customerCode";
        var customerInfo = new PiOnboardServiceApplicationQueriesGetCustomerInfoByCodeResult
        {
            IsIndividualCustomer = true,
            IsThaiIdCard = true,
            IsForStructureNote = false
        };
        var apiResponse = new PiOnboardServiceApplicationQueriesGetCustomerInfoByCodeResultApiResponse { Data = customerInfo };

        _mockCustomerInfoApi.Setup(api => api.InternalGetCustomerInfoByCustomerCodeAsync(customerCode, default))
            .ReturnsAsync(apiResponse);

        // Act
        var result = await _customerService.GetCustomerInfoByCustomerCode(customerCode);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(customerInfo, result);
    }

    [Fact]
    public async Task GetCustomerInfoByCustomerCode_ReturnsNull_WhenExceptionOccurs()
    {
        // Arrange
        var customerCode = "customerCode";

        _mockCustomerInfoApi.Setup(api => api.InternalGetCustomerInfoByCustomerCodeAsync(customerCode, default))
            .ThrowsAsync(new Exception("Test exception"));

        // Act
        var result = await _customerService.GetCustomerInfoByCustomerCode(customerCode);

        // Assert
        Assert.Null(result);
    }
}
