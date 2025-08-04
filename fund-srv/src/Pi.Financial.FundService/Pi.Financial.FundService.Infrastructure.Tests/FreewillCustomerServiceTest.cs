using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Pi.Common.Features;
using Pi.Financial.Client.Freewill.Api;
using Pi.Financial.Client.Freewill.Model;
using Pi.Financial.FundService.Application.Models.Customer;
using Pi.Financial.FundService.Application.Models.Enums;
using Pi.Financial.FundService.Application.Services.CustomerService;
using Pi.Financial.FundService.Infrastructure.Services;

namespace Pi.Financial.FundService.Infrastructure.Tests;

public class FreewillCustomerServiceTest
{
    private readonly Mock<ICustomerModuleApi> _mockApi;
    private readonly ICustomerService _customerService;
    private readonly Mock<IFeatureService> _mockFeatureService = new();

    public FreewillCustomerServiceTest()
    {
        _mockApi = new Mock<ICustomerModuleApi>();
        _customerService = new FreewillCustomerService(
            _mockApi.Object,
            NullLogger<FreewillCustomerService>.Instance,
            _mockFeatureService.Object);
    }

    [Fact(Skip = "Mock not complete")]
    public async Task TestGetCustomerInfo()
    {
        _mockApi
            .Setup(c => c.QueryCustomerCustomerInfoAsync(It.IsAny<GetCustomerInfoByCustomerCodeRequest>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                () => new GetCustomerInfoByCustomerCodeResponse("referId", "20220102", "12:12:12")
            );
        _mockApi
            .Setup(c => c.QueryCustomerKycInfoAsync(It.IsAny<GetKYCInfoRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                () => new GetKYCInfoResponse("referId", "20220102", "12:12:12")
            );
        _mockApi
            .Setup(c => c.QueryCustomerSuitInfoAsync(It.IsAny<GetSuitInfoRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                () => new GetSuitInfoResponse("referId", "20220102", "12:12:12")
            );
        _mockApi
            .Setup(c => c.QueryCustomerNdidInfoAsync(It.IsAny<GetNDIDInfoRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                () => new GetNDIDInfoResponse("referId", "20220102", "12:12:12")
            );
        _mockApi
            .Setup(c => c.QueryCustomerAddressInfoAsync(It.IsAny<GetAddressInfoRequest>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                () => new GetAddressInfoResponse("referId", "20220102", "12:12:12")
            );
        _mockApi
            .Setup(c => c.QueryCustomerEmailInfoAsync(It.IsAny<GetEmailInfoRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                () => new GetEmailInfoResponse("referId", "20220102", "12:12:12")
            );
        _mockApi
            .Setup(c => c.QueryCustomerMobileInfoAsync(It.IsAny<GetMobileInfoRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                () => new GetMobileInfoResponse("referId", "20220102", "12:12:12")
            );
        _mockApi
            .Setup(c => c.QueryCustomerRelatedPersonInfoAsync(It.IsAny<GetRelatedPersonInfoRequest>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                () => new GetRelatedPersonInfoResponse("referId", "20220102", "12:12:12")
            );
        _mockApi
            .Setup(c => c.QueryCustomerBankAccountInfoAsync(It.IsAny<GetBankAccInfoRequest>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                () => new GetBankAccInfoResponse("referId", "20220102", "12:12:12")
            );
        _mockApi
            .Setup(c => c.QueryCustomerAccountInfoAsync(It.IsAny<GetAccountInfoRequest>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                () => new GetAccountInfoResponse("referId", "20220102", "12:12:12")
            );

        const string custCode = "7711431";
        var customerInfo = await _customerService.GetCustomerInfo(custCode);

        Assert.NotNull(customerInfo);
    }

    [Fact(Skip = "Mock not complete")]
    public async void TestGetFatcaInfo()
    {
        _mockApi
            .Setup(c => c.QueryCustomerCustomerInfoAsync(It.IsAny<GetCustomerInfoByCustomerCodeRequest>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                () => new GetCustomerInfoByCustomerCodeResponse("referId", "20220102", "12:12:12")
            );
        _mockApi
            .Setup(c => c.QueryCustomerFatcaInfoAsync(It.IsAny<GetFATCAInfoRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                () => new GetFATCAInfoResponse("referId", "20220102", "12:12:12")
            );

        string custCode = "7711431";
        // string custCode = "7711256";
        // string custCode = "7711253";
        var fatcaInfo = await _customerService.GetFatcaInfo(custCode);

        Assert.NotNull(fatcaInfo);
    }

    public static IEnumerable<object[]> TestParseSuitabilityQuestionnaireData =>
        new List<object[]>
        {
            new object[]
            {
                new List<GetSuitInfoQuestionnaireItem>(),
                new SuitabilityForm(
                    SuitabilityAnswer.None,
                    SuitabilityAnswer.None,
                    SuitabilityAnswer.None,
                    new List<SuitabilityAnswer>(),
                    SuitabilityAnswer.None,
                    SuitabilityAnswer.None,
                    SuitabilityAnswer.None,
                    SuitabilityAnswer.None,
                    SuitabilityAnswer.None,
                    SuitabilityAnswer.None,
                    SuitabilityAnswer.None,
                    SuitabilityAnswer.None,
                    SuitabilityAnswer.None,
                    SuitabilityAnswer.None
                )
            },
            new object[]
            {
                new List<GetSuitInfoQuestionnaireItem>
                {
                    new(choicecode: 413),
                    new(choicecode: 267),
                    new(choicecode: 260),
                    new(choicecode: 271),
                    new(choicecode: 272),
                    new(choicecode: 277),
                    new(choicecode: 282),
                    new(choicecode: 287),
                    new(choicecode: 297),
                    new(choicecode: 310),
                    new(choicecode: 314),
                    new(choicecode: 408),
                    new(choicecode: 411),
                },
                new SuitabilityForm(
                    SuitabilityAnswer.Two,
                    SuitabilityAnswer.One,
                    SuitabilityAnswer.Four,
                    new List<SuitabilityAnswer> { SuitabilityAnswer.Four },
                    SuitabilityAnswer.One,
                    SuitabilityAnswer.Two,
                    SuitabilityAnswer.Three,
                    SuitabilityAnswer.Four,
                    SuitabilityAnswer.Two,
                    SuitabilityAnswer.None,
                    SuitabilityAnswer.One,
                    SuitabilityAnswer.One,
                    SuitabilityAnswer.One,
                    SuitabilityAnswer.Two
                )
            },
            new object[]
            {
                new List<GetSuitInfoQuestionnaireItem>
                {
                    new(choicecode: 413), new(choicecode: 267), new(choicecode: 260), new(choicecode: 271),
                },
                new SuitabilityForm(
                    SuitabilityAnswer.Two,
                    SuitabilityAnswer.One,
                    SuitabilityAnswer.Four,
                    new List<SuitabilityAnswer> { SuitabilityAnswer.Four },
                    SuitabilityAnswer.None,
                    SuitabilityAnswer.None,
                    SuitabilityAnswer.None,
                    SuitabilityAnswer.None,
                    SuitabilityAnswer.None,
                    SuitabilityAnswer.None,
                    SuitabilityAnswer.None,
                    SuitabilityAnswer.None,
                    SuitabilityAnswer.None,
                    SuitabilityAnswer.None
                )
            }
        };

    [Theory]
    [MemberData(nameof(TestParseSuitabilityQuestionnaireData))]
    public void TestParseSuitabilityQuestionnaire(List<GetSuitInfoQuestionnaireItem>? input, SuitabilityForm output)
    {
        var suitabilityForm = new FreewillCustomerService(
                _mockApi.Object,
                NullLogger<FreewillCustomerService>.Instance,
                _mockFeatureService.Object)
            .ParseSuitabilityQuestionnaire(input);

        Assert.Equivalent(suitabilityForm, output);
    }

    [Fact]
    public void Test_GetBuildingFunctionReturnsCorrectly()
    {
        var data1 = new GetAddressInfoResponseItem(
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "building",
            "",
            "village",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            ""
        );
        var data2 = new GetAddressInfoResponseItem(
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            null!,
            "",
            "village",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            ""
        );
        var data3 = new GetAddressInfoResponseItem(
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "building",
            null!,
            null!,
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            ""
        );
        var data4 = new GetAddressInfoResponseItem(
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "   ",
            null!,
            null!,
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            ""
        );

        //Act
        var resultNotnull = _customerService.GetBuilding(data1);
        var resultBuildingNull = _customerService.GetBuilding(data2);
        var resultVillageNull = _customerService.GetBuilding(data3);
        var resultNull = _customerService.GetBuilding(data4);

        //Assert
        Assert.Equivalent("building, village", resultNotnull);
        Assert.Equivalent("village", resultBuildingNull);
        Assert.Equivalent("building", resultVillageNull);
        Assert.Equivalent(string.Empty, resultNull);
    }

    [Fact]
    public void GetOccupationAndBusinessType_4Fields_OccupationOtherIsNull_ReturnsCorrectly()
    {
        var kyfInfo = new GetKYCInfoResponseItem(occpdetail: "30|เจ้าของกิจการ/ธุรกิจส่วนตัว|180|ขายเครื่องเขียนและอุปกรณ์สำนักงาน");

        //Act
        var result = _customerService.GetOccupationAndBusinessType(kyfInfo);

        //Assert
        Assert.Equal(Occupation.BusinessOwner, result.Item1);
        Assert.Null(result.Item2);
        Assert.Equal(BusinessType.Other, result.Item3);
        Assert.Equal("ขายเครื่องเขียนและอุปกรณ์สำนักงาน", result.Item4);
    }

    [Fact]
    public void GetOccupationAndBusinessType_4Fields_WithOccupationOther_ReturnsCorrectly()
    {
        var kyfInfo = new GetKYCInfoResponseItem(occpdetail: "170|แบ็คอัพลำไยไหทองคำ|180|เต้นออนไลน์");

        //Act
        var result = _customerService.GetOccupationAndBusinessType(kyfInfo);

        //Assert
        Assert.Equal(Occupation.Other, result.Item1);
        Assert.Equal("แบ็คอัพลำไยไหทองคำ", result.Item2);
        Assert.Equal(BusinessType.Other, result.Item3);
        Assert.Equal("เต้นออนไลน์", result.Item4);
    }

    [Fact]
    public void GetOccupationAndBusinessType_2Fields_ReturnsCorrectly()
    {
        var kyfInfo = new GetKYCInfoResponseItem(occpdetail: "40|พนักงานบริษัท");

        //Act
        var result = _customerService.GetOccupationAndBusinessType(kyfInfo);

        //Assert
        Assert.Equal(Occupation.CompanyEmployee, result.Item1);
        Assert.Null(result.Item2);
        Assert.Null(result.Item3);
        Assert.Null(result.Item4);
    }

    [Fact]
    public void GetOccupationAndBusinessType_1Field_ReturnsCorrectly()
    {
        var kyfInfo = new GetKYCInfoResponseItem(
            occpdetail: "ร้านขายยา",
            occupationcode: "003");

        //Act
        var result = _customerService.GetOccupationAndBusinessType(kyfInfo);

        //Assert
        Assert.Equal(Occupation.BusinessOwner, result.Item1);
        Assert.Null(result.Item2);
        Assert.Equal(BusinessType.Other, result.Item3);
        Assert.Equal("ร้านขายยา", result.Item4);
    }

    [Fact]
    public void GetOccupationAndBusinessType_ReturnsCorrectly_1()
    {
        var kyfInfo = new GetKYCInfoResponseItem(occpdetail: "90|นักลงทุน", occupationcode: "180");

        //Act
        var result = _customerService.GetOccupationAndBusinessType(kyfInfo);

        //Assert
        Assert.Equal(Occupation.Investor, result.Item1);
        Assert.Null(result.Item2);
        Assert.Null(result.Item3);
        Assert.Null(result.Item4);
    }
    [Fact]
    public void GetOccupationAndBusinessType_ReturnsCorrectly_2()
    {
        var kyfInfo = new GetKYCInfoResponseItem(occpdetail: "150|อาชีพอิสระ|90|โรงแรม/ภัตตาคาร", occupationcode: "90");

        //Act
        var result = _customerService.GetOccupationAndBusinessType(kyfInfo);

        //Assert
        Assert.Equal(Occupation.Freelance, result.Item1);
        Assert.Null(result.Item2);
        Assert.Equal(BusinessType.HotelOrRestaurant, result.Item3);
        Assert.Null(result.Item4);
    }

    [Fact]
    public void GetOccupationAndBusinessType_ReturnsCorrectly_3()
    {
        var kyfInfo = new GetKYCInfoResponseItem(occpdetail: "", occupationcode: "004");

        //Act
        var result = _customerService.GetOccupationAndBusinessType(kyfInfo);

        //Assert
        Assert.Equal(Occupation.Other, result.Item1);
        Assert.Equal("อื่นๆ", result.Item2);
        Assert.Equal(BusinessType.Other, result.Item3);
        Assert.Equal("อื่นๆ", result.Item4);
    }

    [Fact]
    public void GetOccupationAndBusinessType_ReturnsCorrectly_4()
    {
        var kyfInfo = new GetKYCInfoResponseItem(occpdetail: "90lนักลงทุน", occupationcode: "180");

        //Act
        var result = _customerService.GetOccupationAndBusinessType(kyfInfo);

        //Assert
        Assert.Equal(Occupation.Other, result.Item1);
        Assert.Equal("อื่นๆ", result.Item2);
        Assert.Equal(BusinessType.Other, result.Item3);
        Assert.Equal("90lนักลงทุน", result.Item4);
    }

    [Fact]
    public void GetOccupationAndBusinessType_ReturnsCorrectly_5()
    {
        var kyfInfo = new GetKYCInfoResponseItem(occpdetail: "150|อาชีพอิสระ|180|ค้าขาย", occupationcode: "180");

        //Act
        var result = _customerService.GetOccupationAndBusinessType(kyfInfo);

        //Assert
        Assert.Equal(Occupation.Freelance, result.Item1);
        Assert.Null(result.Item2);
        Assert.Equal(BusinessType.Other, result.Item3);
        Assert.Equal("ค้าขาย", result.Item4);
    }

    [Fact]
    public void GetOccupationAndBusinessType_ReturnsCorrectly_6()
    {
        var kyfInfo = new GetKYCInfoResponseItem(occpdetail: "สถานพยาบาล", occupationcode: "014");

        //Act
        var result = _customerService.GetOccupationAndBusinessType(kyfInfo);

        //Assert
        Assert.Equal(Occupation.CompanyEmployee, result.Item1);
        Assert.Null(result.Item2);
        Assert.Null(result.Item3);
        Assert.Null(result.Item4);
    }

    [Fact]
    public void GetOccupationAndBusinessType_ReturnsCorrectly_7()
    {
        var kyfInfo = new GetKYCInfoResponseItem(occpdetail: "รับราชการ", occupationcode: "015");

        //Act
        var result = _customerService.GetOccupationAndBusinessType(kyfInfo);

        //Assert
        Assert.Equal(Occupation.GovernmentOfficer, result.Item1);
        Assert.Null(result.Item2);
        Assert.Null(result.Item3);
        Assert.Null(result.Item4);
    }

    [Fact]
    public void GetOccupationAndBusinessType_ReturnsCorrectly_8()
    {
        var kyfInfo = new GetKYCInfoResponseItem(occpdetail: "60|กิจการครอบครัว", occupationcode: "018");

        //Act
        var result = _customerService.GetOccupationAndBusinessType(kyfInfo);

        //Assert
        Assert.Equal(Occupation.FamilyBusiness, result.Item1);
        Assert.Null(result.Item2);
        Assert.Equal(BusinessType.Other, result.Item3);
        Assert.Equal("กิจการครอบครัว", result.Item4);
    }

    [Fact]
    public void GetOccupationAndBusinessType_ReturnsCorrectly_9()
    {
        var kyfInfo = new GetKYCInfoResponseItem(occpdetail: "60|", occupationcode: "018");

        //Act
        var result = _customerService.GetOccupationAndBusinessType(kyfInfo);

        //Assert
        Assert.Equal(Occupation.FamilyBusiness, result.Item1);
        Assert.Null(result.Item2);
        Assert.Equal(BusinessType.Other, result.Item3);
        Assert.Equal("อื่นๆ", result.Item4);
    }

    [Fact]
    public void GetOccupationAndBusinessType_ReturnsCorrectly_10()
    {
        var kyfInfo = new GetKYCInfoResponseItem(occpdetail: "30|เจ้าของกิจการ/ธุรกิจส่วนตัว|007|ค้าอัญมณี/ทอง", occupationcode: "008");

        //Act
        var result = _customerService.GetOccupationAndBusinessType(kyfInfo);

        //Assert
        Assert.Equal(Occupation.BusinessOwner, result.Item1);
        Assert.Null(result.Item2);
        Assert.Equal(BusinessType.ForeignExchange, result.Item3);
        Assert.Null(result.Item4);
    }

    [Fact]
    public void GetOccupationAndBusinessType_ReturnsCorrectly_11()
    {
        var kyfInfo = new GetKYCInfoResponseItem(occpdetail: "60", occupationcode: "018");

        //Act
        var result = _customerService.GetOccupationAndBusinessType(kyfInfo);

        //Assert
        Assert.Equal(Occupation.FamilyBusiness, result.Item1);
        Assert.Null(result.Item2);
        Assert.Equal(BusinessType.Other, result.Item3);
        Assert.Equal("60", result.Item4);
    }
}
