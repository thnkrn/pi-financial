// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.Extensions.Logging;
using Moq;
using Pi.Financial.FundService.Application.Commands;
using Pi.Financial.FundService.Application.Models.User;
using Pi.Financial.FundService.Application.Services.CustomerService;
using Pi.Financial.FundService.Application.Services.FundConnextService;
using Pi.Financial.FundService.Application.Services.OnboardService;
using Pi.Financial.FundService.Application.Services.PdfService;
using Pi.Financial.FundService.Application.Services.UserService;

namespace Pi.Financial.FundService.Application.Tests.Commands;

public class GenerateFundAccountOpeningDocumentsConsumerTests
{
    private readonly Mock<ILogger<GenerateFundAccountOpeningDocumentsConsumer>> _logger = new();
    private readonly Mock<ICustomerService> _customerService = new();
    private readonly Mock<IFundConnextService> _fundConnextService = new();
    private readonly Mock<IPdfService> _pdfService = new();
    private readonly Mock<IOnboardService> _onboardService = new();
    private readonly Mock<IUserService> _userService = new();
    private readonly GenerateFundAccountOpeningDocumentsConsumer _consumer;

    public GenerateFundAccountOpeningDocumentsConsumerTests()
    {
        _consumer = new GenerateFundAccountOpeningDocumentsConsumer(
            _logger.Object,
            _customerService.Object,
            _fundConnextService.Object,
            _pdfService.Object,
            _onboardService.Object,
            _userService.Object);
    }

}
