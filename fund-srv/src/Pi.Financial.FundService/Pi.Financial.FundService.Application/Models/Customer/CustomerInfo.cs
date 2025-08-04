using System.Globalization;
using Pi.Financial.Client.FundConnext.Model;
using Pi.Financial.FundService.Application.Models.Enums;
using Pi.Financial.FundService.Application.Utils;
using Pi.Financial.FundService.Domain.AggregatesModel.AccountOpeningAggregate;
using MonthlyIncomeLevel = Pi.Financial.FundService.Application.Models.Enums.MonthlyIncomeLevel;

namespace Pi.Financial.FundService.Application.Models.Customer;

public record CustomerInfo(
    IdentificationCardType IdentificationCardType,
    string CardNumber,
    DateOnly? CardIssuedDate,
    DateOnly? CardExpiryDate,
    bool CardIsLifetime,
    Title Title,
    string FirstNameTh,
    string LastNameTh,
    string FirstNameEn,
    string LastNameEn,
    DateOnly BirthDate,
    Nationality Nationality,
    string MobileNumber,
    MaritalStatus MaritalStatus,
    Occupation OccupationId,
    MonthlyIncomeLevel MonthlyIncomeLevel,
    int AssetValue,
    ILookup<IncomeSource, string> IncomeSource,
    Address IdentificationDocument,
    CurrentAddressSameAsFlag CurrentAddressSameAsFlag,
    bool RelatedPoliticalPerson,
    bool CanAcceptFxRisk,
    bool CanAcceptDerivativeInvestment,
    SuitabilityRiskLevel SuitabilityRiskLevel,
    DateOnly SuitabilityEvaluationDate,
    bool Fatca,
    DateOnly FatcaDeclarationDate,
    DateOnly ApplicationDate,
    Country IncomeSourceCountry,
    OpenFundConnextFormFlag OpenFundConnextFormFlag,
    bool Approved,
    bool VulnerableFlag,
    MailingAddressOption MailingAddressOption,
    MailingMethod MailingMethod,
    SuitabilityForm SuitabilityForm,
    ILookup<InvestmentObjective, string> InvestmentObjective)
{
    public Country? PassportCountry { get; init; }
    public AccompanyingDocument? AccompanyingDocument { get; init; }
    public string? TitleOther { get; init; }
    public string? Email { get; init; }
    public string? Phone { get; init; }
    public string? Fax { get; init; }
    public Spouse? Spouse { get; init; }
    public string? OccupationOther { get; init; }
    public string? BusinessTypeOther { get; init; }
    public string? IncomeSourceOther { get; init; }
    public Address? Current { get; init; }
    public string? CompanyName { get; init; }
    public Address? Work { get; init; }
    public string? WorkPosition { get; init; }
    public string? PoliticalRelatedPersonPosition { get; init; }
    public CddScore? CddScore { get; init; }
    public DateOnly? CddDate { get; init; }
    public string? ReferralPerson { get; init; }
    public string? AcceptBy { get; init; }
    public string? VulnerableDetail { get; init; }
    public OpenChannel? OpenChannel { get; init; }
    public InvestorClass InvestorClass { get; init; }
    public decimal SuitabilityScore { get; init; }
    public BusinessType? BusinessTypeId { get; init; }
    public bool FinancialEducation { get; init; }
    public bool EquityExperience { get; init; }
    public bool HighRiskProductExperience { get; init; }

    public CustomerAccountCreateRequestV5 ToAccountCreateRequestV5Payload(Crs? crs, string? ndidRequestId)
    {
        ValidateCrs(crs);
        var DateFormatPattern = "yyyyMMdd";

        var identificationCardType = IdentificationCardType switch
        {
            IdentificationCardType.Citizen => CustomerAccountCreateRequestV5.IdentificationCardTypeEnum.CITIZENCARD,
            IdentificationCardType.Passport => CustomerAccountCreateRequestV5.IdentificationCardTypeEnum.PASSPORT,
            _ => throw new NotImplementedException("IdentificationCardType was not implement")
        };

        CustomerAccountCreateRequestV5.AccompanyingDocumentEnum? accompanyingDocument = AccompanyingDocument switch
        {
            Enums.AccompanyingDocument.Citizen => CustomerAccountCreateRequestV5.AccompanyingDocumentEnum.CITIZENCARD,
            Enums.AccompanyingDocument.AlienCard => CustomerAccountCreateRequestV5.AccompanyingDocumentEnum.ALIENCARD,
            _ => null
        };

        var title = Title switch
        {
            Title.Mr => CustomerAccountCreateRequestV5.TitleEnum.MR,
            Title.Mrs => CustomerAccountCreateRequestV5.TitleEnum.MRS,
            Title.Miss => CustomerAccountCreateRequestV5.TitleEnum.MISS,
            Title.Other => CustomerAccountCreateRequestV5.TitleEnum.OTHER,
            _ => throw new NotImplementedException("Title was not implement")
        };

        var maritalStatus = MaritalStatus switch
        {
            MaritalStatus.Single => CustomerAccountCreateRequestV5.MaritalStatusEnum.Single,
            MaritalStatus.Divorced => CustomerAccountCreateRequestV5.MaritalStatusEnum.Single,
            MaritalStatus.Widowed => CustomerAccountCreateRequestV5.MaritalStatusEnum.Single,
            MaritalStatus.Married => CustomerAccountCreateRequestV5.MaritalStatusEnum.Married,
            _ => throw new NotImplementedException("MaritalStatus was not implement")
        };

        CustomerAccountCreateRequestV5.CurrentAddressSameAsFlagEnum? currentAddressSameAsFlag = CurrentAddressSameAsFlag switch
        {
            CurrentAddressSameAsFlag.False => null,
            CurrentAddressSameAsFlag.True => CustomerAccountCreateRequestV5.CurrentAddressSameAsFlagEnum.IdDocument,
            _ => null
        };

        var openFundConnextFormFlag = OpenFundConnextFormFlag switch
        {
            OpenFundConnextFormFlag.Yes => CustomerAccountCreateRequestV5.OpenFundConnextFormFlagEnum.Y,
            OpenFundConnextFormFlag.No => CustomerAccountCreateRequestV5.OpenFundConnextFormFlagEnum.N,
            OpenFundConnextFormFlag.SingleForm => CustomerAccountCreateRequestV5.OpenFundConnextFormFlagEnum.S,
            _ => throw new NotImplementedException("OpenFundConnextFormFlag was not implement")
        };

        var openChannel = OpenChannel switch
        {
            Enums.OpenChannel.Online => CustomerAccountCreateRequestV5.OpenChannelEnum._1,
            Enums.OpenChannel.Offline => CustomerAccountCreateRequestV5.OpenChannelEnum._2,
            _ => throw new NotImplementedException("OpenChannel was not implement")
        };

        CustomerAccountCreateRequestV5.InvestorClassEnum? investorClass = InvestorClass switch
        {
            InvestorClass.UltraHighNetWorth => CustomerAccountCreateRequestV5.InvestorClassEnum._1,
            InvestorClass.HighNetWorth => CustomerAccountCreateRequestV5.InvestorClassEnum._2,
            InvestorClass.Retail => CustomerAccountCreateRequestV5.InvestorClassEnum._3,
            InvestorClass.Institutional => CustomerAccountCreateRequestV5.InvestorClassEnum._4,
            _ => null
        };

        return new CustomerAccountCreateRequestV5(
            identificationCardType,
            passportCountry: PassportCountry?.ToDescriptionString(),
            cardNumber: CardNumber,
            cardExpiryDate: GetCardExpiryDate(DateFormatPattern),
            accompanyingDocument,
            title,
            titleOther: TitleOther,
            enFirstName: FirstNameEn,
            enLastName: LastNameEn,
            thFirstName: FirstNameTh,
            thLastName: LastNameTh,
            birthDate: BirthDate.ToString(DateFormatPattern, CultureInfo.InvariantCulture),
            nationality: Nationality.ToDescriptionString(),
            mobileNumber: MobileNumber,
            email: Email,
            phone: Phone,
            fax: Fax,
            maritalStatus,
            spouse: MapSpouse(),
            occupationId: MapOccupationId(),
            occupationOther: OccupationOther,
            businessTypeId: MapBusinessTypeId(),
            businessTypeOther: BusinessTypeOther,
            monthlyIncomeLevel: MapMonthlyIncome(),
            assetValue: AssetValue,
            incomeSource: string.Join(",", IncomeSource.Select(i => i.Key.ToDescriptionString())),
            incomeSourceOther: IncomeSourceOther,
            identificationDocument: MapWorkAddress(IdentificationDocument),
            currentAddressSameAsFlag,
            current: MapWorkAddress(Current),
            companyName: CompanyName,
            work: MapWorkAddress(Work),
            workPosition: WorkPosition,
            relatedPoliticalPerson: RelatedPoliticalPerson,
            politicalRelatedPersonPosition: PoliticalRelatedPersonPosition,
            canAcceptFxRisk: CanAcceptFxRisk,
            canAcceptDerivativeInvestment: CanAcceptDerivativeInvestment,
            suitabilityRiskLevel: Enum.Parse<CustomerAccountCreateRequestV5.SuitabilityRiskLevelEnum>(SuitabilityRiskLevel.NumberString()),
            suitabilityEvaluationDate: SuitabilityEvaluationDate.ToString(DateFormatPattern,
                CultureInfo.InvariantCulture),
            fatca: Fatca,
            fatcaDeclarationDate: FatcaDeclarationDate.ToString(DateFormatPattern,
                CultureInfo.InvariantCulture),
            crsPlaceOfBirthCountry: crs?.PlaceOfBirthCountry,
            crsPlaceOfBirthCity: crs?.PlaceOfBirthCity,
            crsTaxResidenceInCountriesOtherThanTheUS: crs?.TaxResidenceInCountriesOtherThanTheUS,
            crsDetails: crs?.Details
                .Select(
                    x => new CustomerAccountCreateRequestV5CrsDetailsInner(
                        countryOfTaxResidence: x.CountryOfTaxResidence,
                        tin: x.Tin,
                        reason: MapCrsReason(x.Reason),
                        reasonDesc: x.ReasonDesc
                    )
                )
                .ToList(),
            crsDeclarationDate: crs?.DeclarationDate,
            cddScore: CddScore is not null ? Enum.Parse<CustomerAccountCreateRequestV5.CddScoreEnum>(CddScore!.NumberString()) : null,
            cddDate: CddDate?.ToString(DateFormatPattern, CultureInfo.InvariantCulture),
            referralPerson: ReferralPerson,
            applicationDate: ApplicationDate.ToString(DateFormatPattern,
                CultureInfo.InvariantCulture),
            incomeSourceCountry: IncomeSourceCountry.ToDescriptionString(),
            openFundConnextFormFlag: openFundConnextFormFlag,
            approved: Approved,
            vulnerableFlag: VulnerableFlag,
            vulnerableDetail: VulnerableDetail,
            ndidFlag: !string.IsNullOrWhiteSpace(ndidRequestId),
            ndidRequestId: ndidRequestId,
            openChannel: openChannel,
            suitabilityForm: MapSuitForm(),
            investorClass: investorClass,
            knowledgeAssessmentResult: true,
            knowledgeAssessmentForm: new IndividualInvestorV5ResponseKnowledgeAssessmentForm(
                FinancialEducation,
                EquityExperience,
                HighRiskProductExperience)
        );
    }
    public CustomerAccountCreateRequestV6 ToAccountCreateRequestV6Payload(Crs? crs, string? ndidRequestId, string identityVerificationDateTime, string dopaVerificationDateTime)
    {
        ValidateCrs(crs);
        var DateFormatPattern = "yyyyMMdd";

        var identificationCardType = IdentificationCardType switch
        {
            IdentificationCardType.Citizen => CustomerAccountCreateRequestV6.IdentificationCardTypeEnum.CITIZENCARD,
            IdentificationCardType.Passport => CustomerAccountCreateRequestV6.IdentificationCardTypeEnum.PASSPORT,
            _ => throw new NotImplementedException("IdentificationCardType was not implement")
        };

        CustomerAccountCreateRequestV6.AccompanyingDocumentEnum? accompanyingDocument = AccompanyingDocument switch
        {
            Enums.AccompanyingDocument.Citizen => CustomerAccountCreateRequestV6.AccompanyingDocumentEnum.CITIZENCARD,
            Enums.AccompanyingDocument.AlienCard => CustomerAccountCreateRequestV6.AccompanyingDocumentEnum.ALIENCARD,
            _ => null
        };

        var title = Title switch
        {
            Title.Mr => CustomerAccountCreateRequestV6.TitleEnum.MR,
            Title.Mrs => CustomerAccountCreateRequestV6.TitleEnum.MRS,
            Title.Miss => CustomerAccountCreateRequestV6.TitleEnum.MISS,
            Title.Other => CustomerAccountCreateRequestV6.TitleEnum.OTHER,
            _ => throw new NotImplementedException("Title was not implement")
        };

        var maritalStatus = MaritalStatus switch
        {
            MaritalStatus.Single => CustomerAccountCreateRequestV6.MaritalStatusEnum.Single,
            MaritalStatus.Divorced => CustomerAccountCreateRequestV6.MaritalStatusEnum.Single,
            MaritalStatus.Widowed => CustomerAccountCreateRequestV6.MaritalStatusEnum.Single,
            MaritalStatus.Married => CustomerAccountCreateRequestV6.MaritalStatusEnum.Married,
            _ => throw new NotImplementedException("MaritalStatus was not implement")
        };

        CustomerAccountCreateRequestV6.CurrentAddressSameAsFlagEnum? currentAddressSameAsFlag = CurrentAddressSameAsFlag switch
        {
            CurrentAddressSameAsFlag.False => null,
            CurrentAddressSameAsFlag.True => CustomerAccountCreateRequestV6.CurrentAddressSameAsFlagEnum.IdDocument,
            _ => null
        };

        var openFundConnextFormFlag = OpenFundConnextFormFlag switch
        {
            OpenFundConnextFormFlag.Yes => CustomerAccountCreateRequestV6.OpenFundConnextFormFlagEnum.Y,
            OpenFundConnextFormFlag.No => CustomerAccountCreateRequestV6.OpenFundConnextFormFlagEnum.N,
            OpenFundConnextFormFlag.SingleForm => CustomerAccountCreateRequestV6.OpenFundConnextFormFlagEnum.S,
            _ => throw new NotImplementedException("OpenFundConnextFormFlag was not implement")
        };

        var openChannel = OpenChannel switch
        {
            Enums.OpenChannel.Online => CustomerAccountCreateRequestV6.OpenChannelEnum._1,
            Enums.OpenChannel.Offline => CustomerAccountCreateRequestV6.OpenChannelEnum._2,
            _ => throw new NotImplementedException("OpenChannel was not implement")
        };

        CustomerAccountCreateRequestV6.InvestorClassEnum? investorClass = InvestorClass switch
        {
            InvestorClass.UltraHighNetWorth => CustomerAccountCreateRequestV6.InvestorClassEnum._1,
            InvestorClass.HighNetWorth => CustomerAccountCreateRequestV6.InvestorClassEnum._2,
            InvestorClass.Retail => CustomerAccountCreateRequestV6.InvestorClassEnum._3,
            InvestorClass.Institutional => CustomerAccountCreateRequestV6.InvestorClassEnum._4,
            _ => null
        };

        return new CustomerAccountCreateRequestV6(
            identificationCardType,
            passportCountry: PassportCountry?.ToDescriptionString(),
            cardNumber: CardNumber,
            cardExpiryDate: GetCardExpiryDate(DateFormatPattern),
            accompanyingDocument,
            title,
            titleOther: TitleOther,
            enFirstName: FirstNameEn,
            enLastName: LastNameEn,
            thFirstName: FirstNameTh,
            thLastName: LastNameTh,
            birthDate: BirthDate.ToString(DateFormatPattern, CultureInfo.InvariantCulture),
            nationality: Nationality.ToDescriptionString(),
            mobileNumber: MobileNumber,
            email: Email,
            phone: Phone,
            fax: Fax,
            maritalStatus,
            spouse: MapSpouse(),
            occupationId: MapOccupationId(),
            occupationOther: OccupationOther,
            businessTypeId: MapBusinessTypeId(),
            businessTypeOther: BusinessTypeOther,
            monthlyIncomeLevel: MapMonthlyIncome(),
            assetValue: AssetValue,
            incomeSource: string.Join(",", IncomeSource.Select(i => i.Key.ToDescriptionString())),
            incomeSourceOther: IncomeSourceOther,
            identificationDocument: MapWorkAddress(IdentificationDocument),
            currentAddressSameAsFlag,
            current: MapWorkAddress(Current),
            companyName: CompanyName,
            work: MapWorkAddress(Work),
            workPosition: WorkPosition,
            relatedPoliticalPerson: RelatedPoliticalPerson,
            politicalRelatedPersonPosition: PoliticalRelatedPersonPosition,
            canAcceptFxRisk: CanAcceptFxRisk,
            canAcceptDerivativeInvestment: CanAcceptDerivativeInvestment,
            suitabilityRiskLevel: Enum.Parse<CustomerAccountCreateRequestV6.SuitabilityRiskLevelEnum>(SuitabilityRiskLevel.NumberString()),
            suitabilityEvaluationDate: SuitabilityEvaluationDate.ToString(DateFormatPattern,
                CultureInfo.InvariantCulture),
            fatca: Fatca,
            fatcaDeclarationDate: FatcaDeclarationDate.ToString(DateFormatPattern,
                CultureInfo.InvariantCulture),
            crsPlaceOfBirthCountry: crs?.PlaceOfBirthCountry,
            crsPlaceOfBirthCity: crs?.PlaceOfBirthCity,
            crsTaxResidenceInCountriesOtherThanTheUS: crs?.TaxResidenceInCountriesOtherThanTheUS,
            crsDetails: crs?.Details
                .Select(
                    x => new CustomerAccountCreateRequestV5CrsDetailsInner(
                        countryOfTaxResidence: x.CountryOfTaxResidence,
                        tin: x.Tin,
                        reason: MapCrsReason(x.Reason),
                        reasonDesc: x.ReasonDesc
                    )
                )
                .ToList(),
            crsDeclarationDate: crs?.DeclarationDate,
            cddScore: CddScore is not null ? Enum.Parse<CustomerAccountCreateRequestV6.CddScoreEnum>(CddScore!.NumberString()) : null,
            cddDate: CddDate?.ToString(DateFormatPattern, CultureInfo.InvariantCulture),
            referralPerson: ReferralPerson,
            applicationDate: ApplicationDate.ToString(DateFormatPattern,
                CultureInfo.InvariantCulture),
            incomeSourceCountry: IncomeSourceCountry.ToDescriptionString(),
            openFundConnextFormFlag: openFundConnextFormFlag,
            approved: Approved,
            vulnerableFlag: VulnerableFlag,
            vulnerableDetail: VulnerableDetail,
            ndidFlag: !string.IsNullOrWhiteSpace(ndidRequestId),
            ndidRequestId: ndidRequestId,
            openChannel: openChannel,
            suitabilityForm: MapSuitForm(),
            investorClass: investorClass,
            knowledgeAssessmentResult: true,
            knowledgeAssessmentForm: new IndividualInvestorV5ResponseKnowledgeAssessmentForm(
                FinancialEducation,
                EquityExperience,
                HighRiskProductExperience),
            identityVerificationDateTime: identityVerificationDateTime,
            dopaVerificationDateTime: dopaVerificationDateTime
        );
    }

    private OccupationId MapOccupationId() => OccupationId switch
    {
        Occupation.Farmer => Client.FundConnext.Model.OccupationId.NUMBER_20,
        Occupation.MonkOrPriest => Client.FundConnext.Model.OccupationId.NUMBER_25,
        Occupation.BusinessOwner => Client.FundConnext.Model.OccupationId.NUMBER_30,
        Occupation.CompanyEmployee => Client.FundConnext.Model.OccupationId.NUMBER_40,
        Occupation.DoctorOrNurse => Client.FundConnext.Model.OccupationId.NUMBER_50,
        Occupation.FamilyBusiness => Client.FundConnext.Model.OccupationId.NUMBER_60,
        Occupation.GovernmentOfficer => Client.FundConnext.Model.OccupationId.NUMBER_70,
        Occupation.HousewifeOrSteward => Client.FundConnext.Model.OccupationId.NUMBER_80,
        Occupation.Investor => Client.FundConnext.Model.OccupationId.NUMBER_90,
        Occupation.Politician => Client.FundConnext.Model.OccupationId.NUMBER_110,
        Occupation.Retired => Client.FundConnext.Model.OccupationId.NUMBER_120,
        Occupation.StateEnterpriseEmployee => Client.FundConnext.Model.OccupationId.NUMBER_130,
        Occupation.Student => Client.FundConnext.Model.OccupationId.NUMBER_140,
        Occupation.Freelance => Client.FundConnext.Model.OccupationId.NUMBER_150,
        Occupation.Teacher => Client.FundConnext.Model.OccupationId.NUMBER_160,
        Occupation.Other => Client.FundConnext.Model.OccupationId.NUMBER_170,
        _ => throw new NotImplementedException("OccupationId was not implement")
    };
    private Pi.Financial.Client.FundConnext.Model.MonthlyIncomeLevel MapMonthlyIncome() => MonthlyIncomeLevel switch
    {
        MonthlyIncomeLevel.Level1 => Client.FundConnext.Model.MonthlyIncomeLevel.LEVEL1,
        MonthlyIncomeLevel.Level2 => Client.FundConnext.Model.MonthlyIncomeLevel.LEVEL2,
        MonthlyIncomeLevel.Level3 => Client.FundConnext.Model.MonthlyIncomeLevel.LEVEL3,
        MonthlyIncomeLevel.Level4 => Client.FundConnext.Model.MonthlyIncomeLevel.LEVEL4,
        MonthlyIncomeLevel.Level5 => Client.FundConnext.Model.MonthlyIncomeLevel.LEVEL5,
        MonthlyIncomeLevel.Level6 => Client.FundConnext.Model.MonthlyIncomeLevel.LEVEL6,
        MonthlyIncomeLevel.Level7 => Client.FundConnext.Model.MonthlyIncomeLevel.LEVEL7,
        MonthlyIncomeLevel.Level8 => Client.FundConnext.Model.MonthlyIncomeLevel.LEVEL8,
        MonthlyIncomeLevel.Level9 => Client.FundConnext.Model.MonthlyIncomeLevel.LEVEL9,
        _ => throw new NotImplementedException("MonthlyIncomeLevel was not implement")
    };
    private BusinessTypeId? MapBusinessTypeId() => BusinessTypeId switch
    {
        BusinessType.Antiques => Client.FundConnext.Model.BusinessTypeId.NUMBER_20,
        BusinessType.BankOrFinance => Client.FundConnext.Model.BusinessTypeId.NUMBER_30,
        BusinessType.CasinoOrGambling => Client.FundConnext.Model.BusinessTypeId.NUMBER_40,
        BusinessType.CooperativeOrTempleOrShrine => Client.FundConnext.Model.BusinessTypeId.NUMBER_60,
        BusinessType.Entertainment => Client.FundConnext.Model.BusinessTypeId.NUMBER_70,
        BusinessType.ForeignExchange => Client.FundConnext.Model.BusinessTypeId.NUMBER_80,
        BusinessType.HotelOrRestaurant => Client.FundConnext.Model.BusinessTypeId.NUMBER_90,
        BusinessType.Insurance => Client.FundConnext.Model.BusinessTypeId.NUMBER_110,
        BusinessType.GoldOrGem => Client.FundConnext.Model.BusinessTypeId.NUMBER_120,
        BusinessType.TransferMoney => Client.FundConnext.Model.BusinessTypeId.NUMBER_130,
        BusinessType.RealEstate => Client.FundConnext.Model.BusinessTypeId.NUMBER_140,
        BusinessType.Education => Client.FundConnext.Model.BusinessTypeId.NUMBER_150,
        BusinessType.GlobalOrLocalRecruitment => Client.FundConnext.Model.BusinessTypeId.NUMBER_155,
        BusinessType.Tourism => Client.FundConnext.Model.BusinessTypeId.NUMBER_160,
        BusinessType.Weapon => Client.FundConnext.Model.BusinessTypeId.NUMBER_170,
        BusinessType.Other => Client.FundConnext.Model.BusinessTypeId.NUMBER_180,
        _ => null
    };
    private CustomerAccountCreateRequestV5SuitabilityForm MapSuitForm() => new CustomerAccountCreateRequestV5SuitabilityForm(
                    suitNo1: Enum.Parse<CustomerAccountCreateRequestV5SuitabilityForm.SuitNo1Enum>(SuitabilityForm.SuitNo1.NumberString()),
                    suitNo2: Enum.Parse<CustomerAccountCreateRequestV5SuitabilityForm.SuitNo2Enum>(SuitabilityForm.SuitNo2.NumberString()),
                    suitNo3: Enum.Parse<CustomerAccountCreateRequestV5SuitabilityForm.SuitNo3Enum>(SuitabilityForm.SuitNo3.NumberString()),
                    suitNo4: SuitabilityForm.SuitNo4.Select(s => Enum.Parse<CustomerAccountCreateRequestV5SuitabilityForm.SuitNo4Enum>(s.NumberString())).ToList(),
                    suitNo5: Enum.Parse<CustomerAccountCreateRequestV5SuitabilityForm.SuitNo5Enum>(SuitabilityForm.SuitNo5.NumberString()),
                    suitNo6: Enum.Parse<CustomerAccountCreateRequestV5SuitabilityForm.SuitNo6Enum>(SuitabilityForm.SuitNo6.NumberString()),
                    suitNo7: Enum.Parse<CustomerAccountCreateRequestV5SuitabilityForm.SuitNo7Enum>(SuitabilityForm.SuitNo7.NumberString()),
                    suitNo8: Enum.Parse<CustomerAccountCreateRequestV5SuitabilityForm.SuitNo8Enum>(SuitabilityForm.SuitNo8.NumberString()),
                    suitNo9: Enum.Parse<CustomerAccountCreateRequestV5SuitabilityForm.SuitNo9Enum>(SuitabilityForm.SuitNo9.NumberString()),
                    suitNo10: Enum.Parse<CustomerAccountCreateRequestV5SuitabilityForm.SuitNo10Enum>(SuitabilityForm.SuitNo10.NumberString()),
                    suitNo11: Enum.Parse<CustomerAccountCreateRequestV5SuitabilityForm.SuitNo11Enum>(SuitabilityForm.SuitNo11.NumberString()),
                    suitNo12: Enum.Parse<CustomerAccountCreateRequestV5SuitabilityForm.SuitNo12Enum>(SuitabilityForm.SuitNo12.NumberString())
                );

    private static CustomerAccountCreateRequestV5CrsDetailsInner.ReasonEnum? MapCrsReason(CrsReason? crsReason)
    {
        return crsReason switch
        {
            CrsReason.A => CustomerAccountCreateRequestV5CrsDetailsInner.ReasonEnum.A,
            CrsReason.B => CustomerAccountCreateRequestV5CrsDetailsInner.ReasonEnum.B,
            CrsReason.C => CustomerAccountCreateRequestV5CrsDetailsInner.ReasonEnum.C,
            _ => null,
        };
    }

    private Client.FundConnext.Model.Spouse? MapSpouse()
    {
        return Spouse != null
            ? new Client.FundConnext.Model.Spouse(
                thFirstName: Spouse.FirstNameTh,
                thLastName: Spouse.LastNameTh,
                enFirstName: Spouse.FirstNameEn,
                enLastName: Spouse.LastNameEn)
            : null;
    }

    private static AddressForProfileV5? MapWorkAddress(Address? address)
    {
        return address != null
            ? new AddressForProfileV5(
                no: address.No,
                subdistrict: address.SubDistrict,
                district: address.District,
                province: address.Province,
                postalCode: address.PostalCode,
                country: address.Country.ToDescriptionString(),
                floor: address.Floor,
                building: address.Building,
                roomNo: address.RoomNo,
                soi: address.Soi,
                road: address.Road,
                moo: address.Moo)
            : null;
    }

    private string? GetCardExpiryDate(string dateFormatPattern)
    {
        return CardIsLifetime ? "N/A" : CardExpiryDate?.ToString(dateFormatPattern, CultureInfo.InvariantCulture);
    }

    private static void ValidateCrs(Crs? crs)
    {
        if (crs is null)
        {
            throw new InvalidDataException("Missing CrsAgreement");
        }
    }
}
