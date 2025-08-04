using System.Globalization;
using Microsoft.Extensions.Logging;
using Pi.Common.Features;
using Pi.Financial.Client.Freewill.Api;
using Pi.Financial.Client.Freewill.Model;
using Pi.Financial.FundService.Application.Models;
using Pi.Financial.FundService.Application.Models.Bank;
using Pi.Financial.FundService.Application.Models.Customer;
using Pi.Financial.FundService.Application.Models.Enums;
using Pi.Financial.FundService.Application.Services.CustomerService;
using Pi.Financial.FundService.Application.Services.FundConnextService;
using Pi.Financial.FundService.Domain.AggregatesModel.AccountOpeningAggregate;

namespace Pi.Financial.FundService.Infrastructure.Services
{
    public class FreewillCustomerService : ICustomerService
    {
        private readonly ICustomerModuleApi _freeWillClient;
        private readonly ILogger<FreewillCustomerService> _logger;
        private readonly IFeatureService _featureService;
        private const string DateFormat = "yyyyMMdd";
        private const string ResponseDateFormat = "dd/MM/yyyy";
        private const string TimeFormat = "HH:mm:ss";

        public FreewillCustomerService(
            ICustomerModuleApi freeWillClient,
            ILogger<FreewillCustomerService> logger,
            IFeatureService featureService)
        {
            _freeWillClient = freeWillClient;
            _logger = logger;
            _featureService = featureService;
        }

        private async Task<(
            GetCustomerInfoByCustomerCodeResponseItem? custInfo,
            GetKYCInfoResponseItem? kycInfo,
            GetSuitInfoResponseItem? suitInfo,
            List<GetSuitInfoQuestionnaireItem> questionnaireItems,
            // NDID is not able to use at this point will need to use another source
            // GetNDIDInfoResponse ndidInfo,
            List<GetAddressInfoResponseItem>? addressInfo,
            GetEmailInfoResponseItem? emailInfo,
            GetMobileInfoResponseItem? mobileInfo,
            List<GetRelatedPersonInfoResponseItem>? relatedPersonInfo,
            List<GetRelatedPersonInfoResponseAddressItem>? relatedPersonAddressInfo,
            FatcaInfo fatcaInfo,
            GetCustomerInfoByCardIdResponseItem? customerCardIdInfo,
            CustomerAccount? customerAccount
            )> GetAllCustomerInfo(string custCode)
        {
            var sendDateTime = GetSendDateAndTime();
            var sendDate = sendDateTime.sendDate;
            var sendTime = sendDateTime.sendTime;
            var referId = "QA" + sendDateTime;

            var custInfoRequestParam = new GetCustomerInfoByCustomerCodeRequest(referId, sendDate, sendTime, custCode);
            var kycInfoRequestParam = new GetKYCInfoRequest(referId, sendDate, sendTime, custCode);
            var suitInfoRequestParam = new GetSuitInfoRequest(referId, sendDate, sendTime, custCode);
            // var ndidInfoRequestParam = new GetNDIDInfoRequest(referId, sendDate, sendTime, custCode);
            var addressInfoRequestParam = new GetAddressInfoRequest(referId, sendDate, sendTime, custCode);
            var emailInfoRequestParam = new GetEmailInfoRequest(referId, sendDate, sendTime, custCode);
            var mobileInfoRequestParam = new GetMobileInfoRequest(referId, sendDate, sendTime, custCode);
            var relatedPersonInfoRequestParam = new GetRelatedPersonInfoRequest(referId, sendDate, sendTime, custCode);

            var customerInfoTask =
                _freeWillClient.QueryCustomerCustomerInfoAsync(custInfoRequestParam);
            var kycInfoTask = _freeWillClient.QueryCustomerKycInfoAsync(kycInfoRequestParam);
            var suitInfoTask = _freeWillClient.QueryCustomerSuitInfoAsync(suitInfoRequestParam);
            var fatcaInfoTask = GetFatcaInfo(customerId: custCode);
            // var ndidInfoTask = _freeWillClient.QueryCustomerNdidInfoAsync(ndidInfoRequestParam);
            var addressInfoTask =
                _freeWillClient.QueryCustomerAddressInfoAsync(addressInfoRequestParam);
            var emailInfoTask = _freeWillClient.QueryCustomerEmailInfoAsync(emailInfoRequestParam);
            var mobileInfoTask = _freeWillClient.QueryCustomerMobileInfoAsync(mobileInfoRequestParam);
            var relatedPersonInfoTask =
                _freeWillClient.QueryCustomerRelatedPersonInfoAsync(relatedPersonInfoRequestParam);
            var customerAccountTask = GetCustomerAccount(custCode);

            Task? aggTask = null;
            try
            {
                aggTask = Task.WhenAll(
                    customerInfoTask,
                    kycInfoTask,
                    suitInfoTask,
                    fatcaInfoTask,
                    // ndidInfoTask,
                    addressInfoTask,
                    emailInfoTask,
                    mobileInfoTask,
                    relatedPersonInfoTask,
                    customerAccountTask
                );
                await aggTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unable to call Freewill with Exception. Message: {Exception}", ex.Message);
                if (aggTask?.Exception?.InnerExceptions.Any() == true)
                {
                    foreach (var exception in aggTask.Exception.InnerExceptions)
                    {
                        _logger.LogError(exception ?? null, "Unable to call Freewill with Exception. Message: {Exception}", exception?.Message);
                        throw;
                    }
                }
            }

            GetCustomerInfoByCustomerCodeResponseItem? customerInfoResult;
            try
            {
                customerInfoResult = customerInfoTask.Result.ResultList.FirstOrDefault();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Freewill customer info is null. Message: {Exception}", e.Message);
                throw new MissingRequiredFieldException("Freewill CustomerInfo");
            }

            GetKYCInfoResponseItem? kycInfoResult;
            try
            {
                kycInfoResult = kycInfoTask.Result.ResultList.FirstOrDefault();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Freewill kyc info is null. Message: {Exception}", e.Message);
                throw new MissingRequiredFieldException("Freewill KYCInfo");
            }

            // var ndidInfoResult = ndidInfoTask.Result;

            GetMobileInfoResponseItem? mobileInfoResult;
            try
            {
                mobileInfoResult = mobileInfoTask.Result.ResultList.FirstOrDefault();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Freewill mobile info is null. Message: {Exception}", e.Message);
                throw new MissingRequiredFieldException("Freewill MobileInfo");
            }

            GetEmailInfoResponseItem? emailInfoResult;
            try
            {
                emailInfoResult = emailInfoTask.Result.ResultList.FirstOrDefault();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Freewill email info is null. Message: {Exception}", e.Message);
                throw new MissingRequiredFieldException("Freewill EmailInfo");
            }

            var addressInfoResult = addressInfoTask.Result.ResultList;

            GetSuitInfoResponseItem? suitInfoResult;
            try
            {
                suitInfoResult = suitInfoTask.Result.ResultList.FirstOrDefault();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Freewill suit info is null. Message: {Exception}", e.Message);
                throw new MissingRequiredFieldException("Freewill SuitInfo");
            }

            var suitQuestionnaireResults = suitInfoTask.Result.DetailList;
            var relatedPersonInfoList = relatedPersonInfoTask.Result.RelatepersonList;
            var relatedPersonInfoResponseAddressList = relatedPersonInfoTask.Result.RelatepersonaddressList;
            var fatcaInfoResult = fatcaInfoTask.Result;
            var customerAccountResult = customerAccountTask.Result;

            GetCustomerInfoByCardIdResponseItem? customerCardIdResult = null;

            if (string.IsNullOrWhiteSpace(customerInfoResult?.Cardid))
            {
                return (
                    customerInfoResult,
                    kycInfoResult,
                    suitInfoResult,
                    suitQuestionnaireResults,
                    // ndidInfoResult,
                    addressInfoResult,
                    emailInfoResult,
                    mobileInfoResult,
                    relatedPersonInfoList,
                    relatedPersonInfoResponseAddressList,
                    fatcaInfoResult,
                    customerCardIdResult,
                    customerAccountResult
                );
            }

            var cardInfoRequestParam = new GetCustomerInfoByCardIdRequest(referId, sendDate, sendTime, customerInfoResult.Cardid);
            try
            {
                customerCardIdResult = (await _freeWillClient.QueryCustomerCardIdInfoAsync(cardInfoRequestParam))?.ResultList?.FirstOrDefault();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Unable to call Freewill with Exception. Message: {Exception}", e.Message);
                throw new MissingRequiredFieldException("Freewill CustomerCardId");
            }

            return (
                customerInfoResult,
                kycInfoResult,
                suitInfoResult,
                suitQuestionnaireResults,
                // ndidInfoResult,
                addressInfoResult,
                emailInfoResult,
                mobileInfoResult,
                relatedPersonInfoList,
                relatedPersonInfoResponseAddressList,
                fatcaInfoResult,
                customerCardIdResult,
                customerAccountResult
            );
        }

        public async Task<CustomerInfo> GetCustomerInfo(string customerId)
        {
            var (
                custInfo,
                kycInfo,
                suitInfo,
                suitQuestionnaireResults,
                // ndidInfo,
                addressInfo,
                emailInfo,
                mobileInfo,
                relatedPersonInfo,
                relatedPersonAddressInfo,
                fatcaInfo,
                customerCardIdInfo,
                customerAccount
                ) = await GetAllCustomerInfo(customerId);

            if (custInfo == null)
            {
                throw new UnableToGetCustomerInfoException();
            }

            var title = custInfo.Etitle.ToLower() switch
            {
                "mr." or "mr" => Title.Mr,
                "mrs." or "mrs" => Title.Mrs,
                "miss" or "miss." => Title.Miss,
                _ => Title.Other,
            };

            var cardLifetimeDate = new DateOnly(9999, 12, 31);
            DateOnly? cardIssuedDate = string.IsNullOrWhiteSpace(custInfo.Cardissue)
                ? null
                : DateOnly.ParseExact(custInfo.Cardissue, ResponseDateFormat, CultureInfo.InvariantCulture);
            var cardExpiryDate = string.IsNullOrWhiteSpace(custInfo.Cardexpire)
                ? cardLifetimeDate
                : DateOnly.ParseExact(custInfo.Cardexpire, ResponseDateFormat, CultureInfo.InvariantCulture);

            Occupation occupation;
            string? occupationOther;
            BusinessType? businessType;
            string? businessTypeOther;

            bool isFeatureOn = _featureService.IsOn(Features.MappingOccupationSupportLegacyCustomer);
            _logger.LogInformation("Features MappingOccupationSupportLegacyCustomer is ON: {IsFeatureOn}", isFeatureOn);

            if (isFeatureOn)
            {
                (occupation, occupationOther, businessType, businessTypeOther) = GetOccupationAndBusinessType(kycInfo);
            }
            else
            {
                string[]? occupationSplit = kycInfo?.Occpdetail.Split("|");
                occupation = MapOccupation(occupationSplit?[0] ?? "170");
                occupationOther = occupationSplit?.ElementAtOrDefault(1);
                businessType = MapBusinessType(occupation, kycInfo?.Occupationcode);
                businessTypeOther = businessType == BusinessType.Other && occupation is Occupation.Freelance
                    or Occupation.BusinessOwner or Occupation.FamilyBusiness or Occupation.Other
                    or Occupation.Individual
                    ? string.IsNullOrWhiteSpace(kycInfo?.Occpdetail)
                        ? throw new MissingRequiredFieldException("BusinessTypeOther")
                        : occupationOther ?? throw new MissingRequiredFieldException("BusinessTypeOther")
                    : null;
            }

            //var ndidResultList = ndidInfo.ResultList?.FirstOrDefault();

            var spouse = relatedPersonInfo?.Find(r => r.Persontype == "SP");
            var martialStatus = string.IsNullOrWhiteSpace(kycInfo?.Marital)
                ? MaritalStatus.Single
                : kycInfo switch
                {
                    { Marital: "2" } => MaritalStatus.Married,
                    _ => MaritalStatus.Single
                };

            var birthDate =
                DateOnly.ParseExact(custInfo.Birthday, ResponseDateFormat, CultureInfo.InvariantCulture);

            var suitabilityForm = ParseSuitabilityQuestionnaire(suitQuestionnaireResults);

            var vulnerableList = new List<string>
            {
                suitabilityForm.SuitNo1 == SuitabilityAnswer.One
                    ? "60YearsOld"
                    : string.Empty,
                suitabilityForm.SuitNo13 == SuitabilityAnswer.One
                    ? "NoInvestmentKnowledge"
                    : string.Empty,
                suitabilityForm.SuitNo14 == SuitabilityAnswer.One
                    ? "Disability"
                    : string.Empty
            }.FindAll(x => x != "");

            var vulnerableDetail = string.Join(
                ",",
                vulnerableList
            );

            var incomeSource = kycInfo?.Investsource?.Split(",").ToLookup(i =>
            {
                return i.Trim() switch
                {
                    "เงินเดือน" => IncomeSource.Salary,
                    "มรดก" => IncomeSource.Heritage,
                    "เงินออม" => IncomeSource.Savings,
                    "การลงทุน" => IncomeSource.Investment,
                    "เงินเกษียณ" => IncomeSource.Retirement,
                    "ประกอบธุรกิจ" => IncomeSource.Business,
                    _ => IncomeSource.Other
                };
            }, i => i);

            const IdentificationCardType identificationCardType = IdentificationCardType.Citizen;
            AccompanyingDocument? accompanyingDocument = AccompanyingDocument.Citizen;

            var occupationRequiresAdditionalInfo =
                occupation is Occupation.BusinessOwner
                    or Occupation.CompanyEmployee
                    or Occupation.DoctorOrNurse
                    or Occupation.FamilyBusiness
                    or Occupation.GovernmentOfficer
                    or Occupation.Politician
                    or Occupation.StateEnterpriseEmployee
                    or Occupation.Freelance
                    or Occupation.Teacher
                    or Occupation.Individual
                    or Occupation.Other;

            var vulnerableFlag = !string.IsNullOrWhiteSpace(vulnerableDetail) && vulnerableList.Count >= 2;

            var sendingAddress = addressInfo?.Find(a => a.Addrtype == "1");
            var identificationAddress = addressInfo?.Find(a => a.Addrtype == "2");
            var workAddress = addressInfo?.Find(a => a.Addrtype == "4");

            bool checkMoreWhenFeatureIsOn = true;
            if (_featureService.IsOn(Features.EnhanceCurrentAddressSameAsFlag))
            {
                checkMoreWhenFeatureIsOn = sendingAddress?.Building == identificationAddress?.Building &&
                                           sendingAddress?.Village == identificationAddress?.Village &&
                                           sendingAddress?.Town == identificationAddress?.Town;
            }


            var currentAddressSameAsFlag = sendingAddress?.Homeno == identificationAddress?.Homeno &&
                                           sendingAddress?.Soi == identificationAddress?.Soi &&
                                           sendingAddress?.Addr2 == identificationAddress?.Addr2 &&
                                           sendingAddress?.Addr3 == identificationAddress?.Addr3 &&
                                           sendingAddress?.Provincedesc == identificationAddress?.Provincedesc &&
                                           sendingAddress?.Ctycode == identificationAddress?.Ctycode &&
                                           checkMoreWhenFeatureIsOn
                ? CurrentAddressSameAsFlag.True
                : CurrentAddressSameAsFlag.False;

            var relatedPoliticalPerson = kycInfo?.Politicalflag != "999";
            var mailingAddrOption = currentAddressSameAsFlag == CurrentAddressSameAsFlag.True
                ? MailingAddressOption.IdAddress
                : MailingAddressOption.CurrentAddress;
            const MailingMethod mailingMethod = MailingMethod.Email;

            var companyName = string.IsNullOrWhiteSpace(kycInfo?.Comptname)
                ? string.IsNullOrWhiteSpace(kycInfo?.Compename)
                    ? null
                    : kycInfo.Compename
                : kycInfo.Comptname;

            if (string.IsNullOrWhiteSpace(companyName)) companyName = "ไม่ระบุ";

            var kycLastReviewDate = string.IsNullOrWhiteSpace(kycInfo?.Lastreviewdate)
                ? DateOnly.FromDateTime(DateTime.UtcNow.AddHours(7))
                : DateOnly.ParseExact(kycInfo.Lastreviewdate, ResponseDateFormat, CultureInfo.InvariantCulture);

            _logger.LogInformation(
                "KycOccp: {KycOccp}, Occupation: {Occupation}, Need OccupationOther {NeedOccupationOther}, OccupationOther: {OccupationOther}, Actual: {Actual}",
                kycInfo?.Occpdetail,
                occupation,
                occupation is Occupation.Other or Occupation.Individual,
                occupationOther,
                occupation is Occupation.Other or Occupation.Individual
                    ? occupationOther ?? "None"
                    : null
            );

            var workPosition = string.IsNullOrWhiteSpace("NOT IMPL")
                ? occupationRequiresAdditionalInfo
                    ? throw new MissingRequiredFieldException("WorkPosition")
                    : null
                : string.IsNullOrWhiteSpace(kycInfo?.Comptitle)
                    ? kycInfo?.Position
                    : kycInfo?.Comptitle;
            if (string.IsNullOrWhiteSpace(workPosition)) workPosition = "ไม่ระบุ";

            var identificationAddressBuilding = GetBuilding(identificationAddress);
            var sendAddressBuilding = GetBuilding(sendingAddress);
            var workAddressBuilding = GetBuilding(workAddress);

            return new CustomerInfo(
                IdentificationCardType: IdentificationCardType.Citizen, // Only allow Thai Citizen only
                CardNumber: custInfo.Cardid,
                CardIssuedDate: cardIssuedDate,
                CardExpiryDate: cardExpiryDate,
                CardIsLifetime: cardExpiryDate.CompareTo(cardLifetimeDate) == 0,
                Title: title,
                FirstNameTh: custInfo.Tname,
                LastNameTh: custInfo.Tsurname,
                FirstNameEn: custInfo.Ename,
                LastNameEn: custInfo.Esurname,
                BirthDate: birthDate,
                Nationality: MapNationality(custInfo.Nationcode),
                MobileNumber: string.IsNullOrWhiteSpace(mobileInfo?.Mobileno)
                    ? throw new MissingRequiredFieldException("MobileNumber")
                    : mobileInfo.Mobileno.Replace("-", string.Empty),
                MaritalStatus: martialStatus,
                OccupationId: occupation,
                MonthlyIncomeLevel: kycInfo != null
                    ? kycInfo switch
                    {
                        { Salary: >= 0 and <= 15000 } => MonthlyIncomeLevel.Level1,
                        { Salary: >= 15001 and <= 30000 } => MonthlyIncomeLevel.Level2,
                        { Salary: >= 30001 and <= 50000 } => MonthlyIncomeLevel.Level3,
                        { Salary: >= 50001 and <= 100000 } => MonthlyIncomeLevel.Level4,
                        { Salary: >= 100001 and <= 500000 } => MonthlyIncomeLevel.Level5,
                        { Salary: >= 500001 and <= 1000000 } => MonthlyIncomeLevel.Level6,
                        { Salary: >= 1000001 and <= 4000000 } => MonthlyIncomeLevel.Level7,
                        { Salary: >= 4000001 and <= 10000000 } => MonthlyIncomeLevel.Level8,
                        { Salary: >= 10000001 } => MonthlyIncomeLevel.Level9,
                        _ => MonthlyIncomeLevel.Level1
                    }
                    : MonthlyIncomeLevel.Level1,
                AssetValue: 0, // Freewill dont have this
                IncomeSource: incomeSource ?? new Dictionary<IncomeSource, string> { { IncomeSource.Other, "Missing from source" } }.ToLookup(i => i.Key, i => i.Value),
                IdentificationDocument: identificationAddress != null
                    ? new Address(
                        No: identificationAddress.Homeno,
                        SubDistrict: identificationAddress?.Subdistrict ?? string.Empty,
                        District: identificationAddress?.District ?? string.Empty,
                        Province: identificationAddress?.Provincedesc ?? string.Empty,
                        PostalCode: identificationAddress?.Zipcode ?? string.Empty,
                        Country: MapCountry(identificationAddress?.Ctycode ?? string.Empty)
                    )
                    {
                        Moo = identificationAddress?.Town,
                        Building = identificationAddressBuilding,
                        Floor = identificationAddress?.Floor,
                        Soi = identificationAddress?.Soi,
                        Road = identificationAddress?.Road,
                    }
                    : throw new MissingRequiredFieldException("IdentificationDocument"),
                CurrentAddressSameAsFlag: currentAddressSameAsFlag, // missing
                RelatedPoliticalPerson: relatedPoliticalPerson,
                CanAcceptFxRisk: suitabilityForm.SuitNo12 == SuitabilityAnswer.Two,
                CanAcceptDerivativeInvestment: suitabilityForm.SuitNo11 == SuitabilityAnswer.Two,
                SuitabilityRiskLevel: MapSuitRiskLevel(suitInfo?.Score),
                SuitabilityEvaluationDate: DateOnly.ParseExact(
                    suitInfo?.Completedate ?? DateTime.Now.ToString(DateFormat, CultureInfo.InvariantCulture),
                    DateFormat, CultureInfo.InvariantCulture),
                Fatca: false,
                FatcaDeclarationDate: customerAccount?.OpenDate ?? DateOnly.FromDateTime(DateTime.UtcNow.AddHours(7)),
                ApplicationDate: customerAccount?.OpenDate ?? DateOnly.FromDateTime(DateTime.UtcNow.AddHours(7)),
                IncomeSourceCountry: MapCountry(kycInfo?.Investsourcecountry),
                OpenFundConnextFormFlag: OpenFundConnextFormFlag.SingleForm, // missing
                Approved: true,
                VulnerableFlag: vulnerableFlag,
                MailingAddressOption: mailingAddrOption,
                MailingMethod: mailingMethod,
                SuitabilityForm: suitabilityForm,
                InvestmentObjective: kycInfo?.Investpurpose
                    .Split("|")
                    .ToLookup(p =>
                    {
                        return p switch
                        {
                            "1" or "เพื่อการลงทุน" => InvestmentObjective.Investment,
                            "2" or "เพื่อการเกษียณ" => InvestmentObjective.RetirementInvestment,
                            "3" or "เพื่อสิทธิประโยชน์ทางภาษี" => InvestmentObjective.ForTaxBenefits,
                            _ => InvestmentObjective.PleaseSpecify
                        };
                    }, p => p) ?? new Dictionary<InvestmentObjective, string> { { InvestmentObjective.PleaseSpecify, "Missing from source" } }.ToLookup(i => i.Key, i => i.Value)
            )
            {
                BusinessTypeId = (occupation is Occupation.Freelance or Occupation.BusinessOwner or Occupation.FamilyBusiness or Occupation.Other or Occupation.Individual)
                    ? businessType ?? throw new MissingRequiredFieldException("BusinessTypeId")
                    : null,
                AccompanyingDocument = accompanyingDocument ??
                                       (identificationCardType == IdentificationCardType.Citizen
                                           ? throw new MissingRequiredFieldException("AccompanyingDocument")
                                           : null),
                TitleOther =
                    title == Title.Other
                        ? string.IsNullOrWhiteSpace(custInfo.Etitle)
                            ? throw new MissingRequiredFieldException("TitleOther")
                            : custInfo.Etitle
                        : null,
                Email = (mailingMethod == MailingMethod.Email && string.IsNullOrWhiteSpace(emailInfo?.Email))
                    ? throw new MissingRequiredFieldException("Email")
                    : emailInfo?.Email,
                Phone = sendingAddress?.Telno2.Replace("-", string.Empty),
                Fax = sendingAddress?.Faxno1.Replace("-", string.Empty) ??
                      (mailingMethod == MailingMethod.Fax
                          ? throw new MissingRequiredFieldException("Fax")
                          : null),
                Spouse = spouse != null
                    ? relatedPersonAddressInfo?.Where(r => r.Psnkey == spouse.Psnkey)
                        .Select(r => new Spouse(r.Tname, r.Tsurname, r.Ename, r.Esurname))
                        .FirstOrDefault()
                    : null,
                OccupationOther = occupation is Occupation.Other or Occupation.Individual
                    // ? occupationOther ?? throw new MissingRequiredField("OccupationOther")
                    ? occupationOther
                    : null,
                BusinessTypeOther = businessTypeOther,
                IncomeSourceOther = incomeSource?.Any(i => i.Key == IncomeSource.Other) ?? false
                    ? string.Join(", ", incomeSource.Where(i => i.Key == IncomeSource.Other).SelectMany(i => i))
                    : null,
                Current = currentAddressSameAsFlag == CurrentAddressSameAsFlag.False
                    ? sendingAddress != null
                        ? new Address(
                            No: sendingAddress.Homeno, // must not be null
                            SubDistrict: sendingAddress.Subdistrict, // must not be null
                            District: sendingAddress.District, // must not be null
                            Province: sendingAddress?.Provincedesc ?? string.Empty, // must not be null
                            PostalCode: sendingAddress?.Zipcode ?? string.Empty, // must not be null
                            Country: MapCountry(sendingAddress?.Ctycode ?? string.Empty) // must not be null
                        )
                        {
                            Moo = sendingAddress?.Town,
                            Building = sendAddressBuilding,
                            Floor = sendingAddress?.Floor,
                            Soi = sendingAddress?.Soi,
                            Road = sendingAddress?.Road,
                        }
                        : throw new MissingRequiredFieldException("Current (Address)")
                    : null,
                CompanyName = companyName,
                Work = workAddress != null && !string.IsNullOrWhiteSpace(workAddress.Homeno)
                    ? new Address(
                        No: workAddress.Homeno, // must not be null
                        SubDistrict: workAddress.Subdistrict, // must not be null
                        District: workAddress.District, // must not be null
                        Province: workAddress?.Provincedesc ?? string.Empty, // must not be null
                        PostalCode: workAddress?.Zipcode ?? string.Empty, // must not be null
                        Country: MapCountry(workAddress?.Ctycode ?? string.Empty) // must not be null
                    )
                    {
                        Moo = workAddress?.Town,
                        Building = workAddressBuilding,
                        Floor = workAddress?.Floor,
                        Soi = workAddress?.Soi,
                        Road = workAddress?.Road,
                    }
                    : occupationRequiresAdditionalInfo
                        ? throw new MissingRequiredFieldException("Work (Address)")
                        : null,
                WorkPosition = workPosition,
                PoliticalRelatedPersonPosition =
                    relatedPoliticalPerson
                        ? kycInfo?.Politicaldetail
                          ?? throw new MissingRequiredFieldException("PoliticalRelatedPersonPosition")
                        : null,
                CddScore = kycInfo?.Riskgroup switch
                {
                    "1" => CddScore.Score1,
                    "2" => CddScore.Score2,
                    "3" => CddScore.Score3,
                    _ => throw new MissingFieldException("CddScore Invalid")
                },
                CddDate = kycLastReviewDate,
                ReferralPerson = customerCardIdInfo?.Mktid + ' ' + customerCardIdInfo?.Mkttname,
                AcceptBy = "ออนไลน์",
                VulnerableDetail =
                    vulnerableFlag
                        ? string.IsNullOrWhiteSpace(vulnerableDetail)
                            ? throw new MissingRequiredFieldException("VulnerableDetail")
                            : vulnerableDetail
                        : null,
                OpenChannel = OpenChannel.Online,
                InvestorClass = custInfo.Wealthtype switch
                {
                    "UHNW" => InvestorClass.UltraHighNetWorth,
                    "HNW" => InvestorClass.HighNetWorth,
                    "Retail" => InvestorClass.Retail,
                    _ => InvestorClass.Retail
                }, // dont have
                SuitabilityScore = suitInfo?.Score ?? 0,
                FinancialEducation = custInfo.Financialeducation.Equals("Y"),
                EquityExperience = custInfo.Equityexperience.Equals("Y"),
                HighRiskProductExperience = custInfo.Highriskproductexperience.Equals("Y"),
            };
        }

        public async Task<CustomerInfoForSyncCustomerFundAccount> GetCustomerInfoForSyncFundCustomerAccount(string customerCode)
        {
            var sendDateTime = GetSendDateAndTime();
            var sendDate = sendDateTime.sendDate;
            var sendTime = sendDateTime.sendTime;
            var referId = "QA" + sendDateTime;
            var custInfoRequestParam = new GetCustomerInfoByCustomerCodeRequest(referId, sendDate, sendTime, customerCode);
            var kycInfoRequestParam = new GetKYCInfoRequest(referId, sendDate, sendTime, customerCode);
            var addressInfoRequestParam = new GetAddressInfoRequest(referId, sendDate, sendTime, customerCode);
            var custInfo =
                (await _freeWillClient.QueryCustomerCustomerInfoAsync(custInfoRequestParam)).ResultList.FirstOrDefault() ?? throw new MissingRequiredFieldException("Freewill CustomerInfo");
            var kycInfo = (await _freeWillClient.QueryCustomerKycInfoAsync(kycInfoRequestParam)).ResultList.FirstOrDefault() ?? throw new MissingRequiredFieldException("Freewill KycInfo");
            var addressInfo =
                (await _freeWillClient.QueryCustomerAddressInfoAsync(addressInfoRequestParam)).ResultList;

            if (custInfo == null)
            {
                throw new UnableToGetCustomerInfoException();
            }

            var sendingAddress = addressInfo?.Find(a => a.Addrtype == "1");
            var identificationAddress = addressInfo?.Find(a => a.Addrtype == "2");

            bool checkMoreWhenFeatureIsOn = true;
            if (_featureService.IsOn(Features.EnhanceCurrentAddressSameAsFlag))
            {
                checkMoreWhenFeatureIsOn = sendingAddress?.Building == identificationAddress?.Building &&
                                           sendingAddress?.Village == identificationAddress?.Village &&
                                           sendingAddress?.Town == identificationAddress?.Town;
            }


            var currentAddressSameAsFlag = sendingAddress?.Homeno == identificationAddress?.Homeno &&
                                           sendingAddress?.Soi == identificationAddress?.Soi &&
                                           sendingAddress?.Addr2 == identificationAddress?.Addr2 &&
                                           sendingAddress?.Addr3 == identificationAddress?.Addr3 &&
                                           sendingAddress?.Provincedesc == identificationAddress?.Provincedesc &&
                                           sendingAddress?.Ctycode == identificationAddress?.Ctycode &&
                                           checkMoreWhenFeatureIsOn
                ? CurrentAddressSameAsFlag.True
                : CurrentAddressSameAsFlag.False;

            var mailingAddrOption = currentAddressSameAsFlag == CurrentAddressSameAsFlag.True
                ? MailingAddressOption.IdAddress
                : MailingAddressOption.CurrentAddress;
            const MailingMethod mailingMethod = MailingMethod.Email;

            return new CustomerInfoForSyncCustomerFundAccount(
                IdentificationCardType: IdentificationCardType.Citizen, // Only allow Thai Citizen only
                CardNumber: custInfo.Cardid,
                PassportCountry: null,
                MailingAddressOption: mailingAddrOption,
                MailingMethod: mailingMethod,
                InvestmentObjective: kycInfo?.Investpurpose
                    .Split("|")
                    .ToLookup(p =>
                    {
                        return p switch
                        {
                            "1" or "เพื่อการลงทุน" => InvestmentObjective.Investment,
                            "2" or "เพื่อการเกษียณ" => InvestmentObjective.RetirementInvestment,
                            "3" or "เพื่อสิทธิประโยชน์ทางภาษี" => InvestmentObjective.ForTaxBenefits,
                            _ => InvestmentObjective.PleaseSpecify
                        };
                    }, p => p) ?? new Dictionary<InvestmentObjective, string> { { InvestmentObjective.PleaseSpecify, "Missing from source" } }.ToLookup(i => i.Key, i => i.Value),
                Approved: true
            );
        }

        private Country MapCountry(string? countryCode)
        {
            if (string.IsNullOrWhiteSpace(countryCode))
            {
                throw new InvalidFormat("Country Code must not be null");
            }

            return countryCode switch
            {
                "000" => Country.Thailand,
                "A15" => Country.Argentina,
                "A20" => Country.Australia,
                "A30" => Country.Austria,
                "A41" => Country.Afghanistan,
                "A43" => Country.Albania,
                "A44" => Country.Algeria,
                "A46" => Country.Andorra,
                "A47" => Country.Angola,
                "B02" => Country.Bahamas,
                "B03" => Country.Bahrain,
                "B05" => Country.Belgium,
                "B06" => Country.Bermuda,
                "B10" or "E10" => Country.UnitedKingdomOfGreatBritainAndNorthernIreland,
                "B20" => Country.Myanmar,
                "B30" => Country.Brazil,
                "B41" => Country.Barbados,
                "B45" => Country.BosniaAndHerzegovina,
                "B50" => Country.BruneiDarussalam,
                "C10" => Country.Canada,
                "C12" => Country.CaymanIslands,
                //"C15" => Country.ChennalIslands,
                "C20" => Country.China,
                "C30" => Country.Colombia,
                "C40" or "C49" => Country.Cambodia,
                "C41" => Country.Cameroon,
                "C47" => Country.Cuba,
                "C90" => Country.Cyprus,
                "D10" => Country.Denmark,
                "D20" => Country.Netherlands,
                "E20" => Country.Ecuador,
                "E70" => Country.Ethiopia,
                "F10" => Country.Philippines,
                "F20" => Country.France,
                "F30" => Country.Finland,
                "G10" => Country.Germany,
                "G20" => Country.Guernsey,
                "G30" => Country.Greece,
                "G55" => Country.Grenada,
                "G58" => Country.Guyana,
                "H10" => Country.HongKong,
                "H20" => Country.Haiti,
                "H30" => Country.Honduras,
                "I10" => Country.India,
                "I15" => Country.Indonesia,
                "I16" => Country.Iceland,
                "I17" => Country.Israel,
                "I20" => Country.Italy,
                "I21" or "I50" => Country.Iran,
                "I22" => Country.Iraq,
                "I30" => Country.Iceland,
                "I40" => Country.Ireland,
                "J10" => Country.Japan,
                "J20" => Country.Jamaica,
                "J40" => Country.Jordan,
                "K10" => Country.Korea,
                "K20" => Country.Kenya,
                "K40" => Country.Kuwait,
                "K50" => Country.NorthKorea,
                "L05" => Country.LaoPeopleDemocraticRepublic,
                "L07" => Country.Liberia,
                "L10" => Country.Luxembourg,
                "L20" => Country.Liberia,
                "L40" => Country.Lebanon,
                "L50" => Country.Liechtenstein,
                "M10" => Country.Malaysia,
                "M20" => Country.Mexico,
                "M30" => Country.Mauritius,
                "M41" => Country.Monaco,
                "M42" => Country.Mongolia,
                "N20" => Country.Norway,
                "N10" => Country.NewZealand,
                "N30" => Country.Nepal,
                "N60" => Country.Namibia,
                "N70" => Country.Nicaragua,
                "P05" => Country.Pakistan,
                "P07" => Country.Panama,
                "P10" => Country.Philippines,
                "P15" => Country.Poland,
                "P20" => Country.Portugal,
                "P30" => Country.PapuaNewGuinea,
                "P40" => Country.Peru,
                "Q10" => Country.Qatar,
                "S10" => Country.UnitedKingdomOfGreatBritainAndNorthernIreland,
                "S20" => Country.Singapore,
                "S30" => Country.Spain,
                "S40" => Country.Sweden,
                "S50" => Country.Switzerland,
                "S60" => Country.SriLanka,
                "S80" => Country.SaudiArabia,
                "S83" => Country.Sudan,
                "S87" => Country.SyrianArabRepublic,
                "T10" => Country.TaiwanProvinceOfChina,
                "T20" => Country.Tonga,
                "T30" => Country.Turkey,
                "T43" => Country.Tajikistan,
                "T44" => Country.TanzaniaUnitedRepublicOf,
                "U10" => Country.Uganda,
                "U20" => Country.UnitedStatesOfAmerica,
                "V10" => Country.VietNam,
                "Y10" => Country.Yemen,
                "Z30" => Country.Zimbabwe,
                _ => Country.Thailand
            };
        }

        public async Task<CustomerAccount> GetCustomerAccount(string customerCode)
        {
            // this method is only use inside this class, to get fund customer account please get in from FundConnextService
            // this will be obsolete in the future
            var dateTimeNow = DateTime.UtcNow.AddHours(7);
            var sendDate = dateTimeNow.ToString(DateFormat);
            var sendTime = dateTimeNow.ToString(TimeFormat);
            var referId = $"QA{dateTimeNow.ToString("yyyyMMddHHmmss")}";
            var getBankInfoReq = new GetBankAccInfoRequest
            (
                referId: referId,
                custcode: customerCode,
                sendDate: sendDate,
                sendTime: sendTime
            );

            var bankAccountResp =
                await _freeWillClient.QueryCustomerBankAccountInfoAsync(getBankInfoReq);

            var getAccountInRequest = new GetAccountInfoRequest
            (
                custcode: customerCode, referId: referId, sendDate: sendDate, sendTime: sendTime
            );

            var accountInfoResp =
                await _freeWillClient.QueryCustomerAccountInfoAsync(getAccountInRequest);

            var bankAccount = new List<BankAccount>();
            if (bankAccountResp is not null && bankAccountResp.ResultListTotal > 0)
            {
                bankAccount = bankAccountResp.ResultList
                    .Where(r => DateTime.UtcNow.AddHours(7).Date
                                    .CompareTo(
                                        DateTime.ParseExact(
                                            string.IsNullOrWhiteSpace(r.Effdate)
                                                ? dateTimeNow.ToString(ResponseDateFormat)
                                                : r.Effdate, ResponseDateFormat,
                                            CultureInfo.InvariantCulture).Date) >=
                                0 &&
                                r is { Transtype: "TRADE", Rptype: "P" } && r.Account.Split("-").Last() == "1")
                    .Select((r, index) => new BankAccount(r.Bankcode, r.Bankaccno,
                        string.IsNullOrWhiteSpace(r.Bankbranchcode) || r.Bankbranchcode == "0000"
                            ? "00000"
                            : r.Bankbranchcode,
                        IsDefault: index == 0))
                    .GroupBy(b => b.BankAccountNo)
                    .Select(g => g.First())
                    .Take(5)
                    .ToList();
            }


            var openDate = accountInfoResp?.AccountList?.Find(a => !string.IsNullOrWhiteSpace(a.Opendate))?.Opendate;

            return new CustomerAccount
            (
                // Since create only Omnibus Account we will construct -M here
                AccountId: $"{customerCode}-M", // ????
                SaleLicense: accountInfoResp?.AccountList?.Where(a => !string.IsNullOrWhiteSpace(a.Salelicence))
                    .Select(a => a.Salelicence).First() ?? string.Empty,
                RedemptionBankAccount: bankAccount,
                SubscriptionBankAccount: bankAccount,
                OpenDate: openDate != null
                    ? DateOnly.ParseExact(openDate, ResponseDateFormat, CultureInfo.InvariantCulture)
                    : DateOnly.FromDateTime(DateTime.UtcNow.AddHours(7))
            );
        }

        public async Task<bool> IsOpenFundAccount(string customerId)
        {
            var dateTimeNow = DateTime.UtcNow.AddHours(7);
            var sendDate = dateTimeNow.ToString(DateFormat);
            var sendTime = dateTimeNow.ToString(TimeFormat);
            var referId = $"QA{dateTimeNow.ToString("yyyyMMddHHmmss")}";

            var getAccountInRequest = new GetAccountInfoRequest
            (
                custcode: customerId, referId: referId, sendDate: sendDate, sendTime: sendTime
            );

            var accountInfoResp =
                await _freeWillClient.QueryCustomerAccountInfoAsync(getAccountInRequest);

            var fundAccount = accountInfoResp.AccountList.Find(x => x.Acctcode is "UT");

            return fundAccount is not null;
        }

        public async Task<FatcaInfo> GetFatcaInfo(string customerId)
        {
            const string referId = "referId";
            (string? sendDate, string? sendTime) = GetSendDateAndTime();

            var custInfoRequestParam =
                new GetCustomerInfoByCustomerCodeRequest(referId, sendDate, sendTime, customerId);
            var fatcaInfoRequestParam = new GetFATCAInfoRequest(referId, sendDate, sendTime, customerId);

            var customerInfoTask =
                _freeWillClient.QueryCustomerCustomerInfoAsync(custInfoRequestParam);
            var fatcaInfoTask = _freeWillClient.QueryCustomerFatcaInfoAsync(fatcaInfoRequestParam);


            var aggTask = Task.WhenAll(
                customerInfoTask,
                fatcaInfoTask
            );

            await aggTask;

            var customerInfoResult = customerInfoTask.Result.ResultList.First();
            GetFATCAInfoResponseItem? fatcaInfoResult;
            try
            {
                fatcaInfoResult = fatcaInfoTask.Result.ResultList.FirstOrDefault() ?? throw new MissingFieldException("Freewill Fatca");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Freewill fatca info is null. Message: {Exception}", e.Message);
                throw new MissingRequiredFieldException("Freewill Fatca");
            }

            var cardInfoRequestParam = new GetCustomerInfoByCardIdRequest(referId, sendDate, sendTime, customerInfoResult.Cardid);
            var customerCardIdResult = await _freeWillClient.QueryCustomerCardIdInfoAsync(cardInfoRequestParam);
            string mktName;
            try
            {
                mktName = customerCardIdResult?.ResultList?.FirstOrDefault()?.Mkttname ?? string.Empty;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Freewill customerCardId info is null. Message: {Exception}", e.Message);
                throw new MissingRequiredFieldException("Freewill CustomerCardId");
            }

            return new FatcaInfo(
                RecipientTH: "บล. พาย จำกัด (มหาชน)",
                RecipientEN: "Pi Securities Public Company Limited",
                ApplicantNameTH: $"{customerInfoResult.Ttitle} {customerInfoResult.Tname} {customerInfoResult.Tsurname}",
                ApplicantNameEN: $"{customerInfoResult.Etitle} {customerInfoResult.Ename} {customerInfoResult.Esurname}",
                Nationality: Nationality.Thai, // Only Support Thai for now
                Date: string.IsNullOrEmpty(fatcaInfoResult.Dateonform)
                    ? DateOnly.FromDateTime(DateTime.UtcNow.AddHours(7))
                    : DateOnly.ParseExact(fatcaInfoResult.Dateonform, ResponseDateFormat, CultureInfo.InvariantCulture),
                ThaiIdNumber: customerInfoResult.Cardid,
                PassportNumber: "",
                IsUSCitizen: false,
                IsGreenCardHolder: false,
                IsUSForTax: fatcaInfoResult.Taxpayerid != "",
                IsBornInUS: false,
                HaveUSResidence: false,
                HaveUSPhoneNumber: false,
                HaveUSAccountReceiver: false,
                HaveUSSignatoryAuthority: false,
                OfficerSignature: mktName,
                OfficerName: "" // TODO: Confirm with P'Bank
            );
        }

        private (string sendDate, string sendTime) GetSendDateAndTime()
        {
            var bkkNow = DateTime.UtcNow.AddHours(7);

            return (
                bkkNow.ToString("yyyyMMdd", CultureInfo.InvariantCulture),
                bkkNow.ToString("HH:mm:ss", CultureInfo.InvariantCulture)
            );
        }

        public SuitabilityForm ParseSuitabilityQuestionnaire(List<GetSuitInfoQuestionnaireItem>? suitInfoResults)
        {
            // map by `questioncode`, `choicecode`
            //  1: 58 -> 412, 413, 414, 415
            //  2: 35 -> 260, 261, 262, 263
            //  3: 36 -> 264, 265, 266, 267
            //  4: 37 -> 268, 269, 270, 271
            //  5: 38 -> 272, 273, 274, 275
            //  6: 39 -> 276, 277, 278, 279
            //  7: 40 -> 280, 281, 282, 283
            //  8: 41 -> 284, 285, 286, 287
            //  9: 42 -> 296, 297, 298, 299
            // 10: 43 -> 300, 301, 302, 303
            // 11: 44 -> 310, 311, 312 (TFEX)
            // 12: 45 -> 314, 315, 316 (FX)
            // 13: 56 -> 409(no), 408(yes)
            // 14: 57 -> 411(no), 410(yes)

            var ans = new SuitabilityAnswer?[14];
            List<SuitabilityAnswer> ans4 = new();

            suitInfoResults?.ForEach(item =>
            {

                var choiceCode = item.Choicecode;

                (int index, int prefix) = choiceCode switch
                {
                    >= 412 and <= 415 => (0, 412),
                    >= 260 and <= 263 => (1, 260),
                    >= 264 and <= 267 => (2, 264),
                    >= 268 and <= 271 => (3, 268),
                    >= 272 and <= 275 => (4, 272),
                    >= 276 and <= 279 => (5, 276),
                    >= 280 and <= 283 => (6, 280),
                    >= 284 and <= 287 => (7, 284),
                    >= 296 and <= 299 => (8, 296),
                    >= 300 and <= 303 => (9, 300),
                    310 => (10, 310), // Answer: abs(310-310) + 1 = 1
                    311 => (10, 312), // Answer: abs(311-312) + 1 = 2
                    312 => (10, 313), // Answer: abs(312-313) + 1 = 2
                    313 => (10, 314), // Answer: abs(313-314) + 1 = 2
                    314 => (11, 314), // Answer: abs(314-314) + 1 = 1
                    315 => (11, 316), // Answer: abs(315-316) + 1 = 2
                    316 => (11, 317), // Answer: abs(316-317) + 1 = 2
                    317 => (11, 318), // Answer: abs(317-318) + 1 = 2
                    408 or 409 => (12, 408),
                    410 or 411 => (13, 410),
                    _ => (-1, -1)
                };

                if (index == -1) return;

                var answer = (SuitabilityAnswer)(Math.Abs(choiceCode - prefix) + 1);
                if (index == 3)
                    ans4.Add(answer);
                else
                    ans[index] = answer;
            });

            return new SuitabilityForm(
                ans[0] ?? SuitabilityAnswer.None,
                ans[1] ?? SuitabilityAnswer.None,
                ans[2] ?? SuitabilityAnswer.None,
                ans4,
                ans[4] ?? SuitabilityAnswer.None,
                ans[5] ?? SuitabilityAnswer.None,
                ans[6] ?? SuitabilityAnswer.None,
                ans[7] ?? SuitabilityAnswer.None,
                ans[8] ?? SuitabilityAnswer.None,
                ans[9] ?? SuitabilityAnswer.None,
                ans[10] ?? SuitabilityAnswer.None,
                ans[11] ?? SuitabilityAnswer.None,
                ans[12] ?? SuitabilityAnswer.None,
                ans[13] ?? SuitabilityAnswer.None
            );
        }

        private Occupation MapOccupation(string occpCode)
        {
            return occpCode switch
            {
                "020" or "20" => Occupation.Farmer,
                "025" or "25" => Occupation.MonkOrPriest,
                "050" or "50" => Occupation.DoctorOrNurse,
                "060" or "60" => Occupation.FamilyBusiness,
                "070" or "70" => Occupation.GovernmentOfficer,
                "080" or "80" => Occupation.HousewifeOrSteward,
                "090" or "90" => Occupation.Investor,
                "110" => Occupation.Politician,
                "120" => Occupation.Retired,
                "140" => Occupation.Student,
                "150" => Occupation.Freelance,
                "160" => Occupation.Teacher,
                "030" or "30" or "910" => Occupation.BusinessOwner,
                "040" or "40" or "920" => Occupation.CompanyEmployee,
                "930" => Occupation.StateEnterpriseEmployee,
                "990" => Occupation.Individual,
                _ => Occupation.Other,
            };
        }

        private BusinessType? MapBusinessType(Occupation occupation, string? occupationCode)
        {
            if (occupation is Occupation.Freelance or Occupation.BusinessOwner or Occupation.FamilyBusiness
                or Occupation.Other or Occupation.Individual)
            {
                return occupationCode switch
                {
                    "002" or "2" or "060" or "60" => BusinessType.CooperativeOrTempleOrShrine,
                    "007" or "7" or "120" => BusinessType.GoldOrGem,
                    "008" or "8" or "080" or "80" => BusinessType.ForeignExchange,
                    "009" or "9" or "130" => BusinessType.TransferMoney,
                    "010" or "10" or "040" or "40" => BusinessType.CasinoOrGambling,
                    "011" or "012" or "11" or "12" or "170" => BusinessType.Weapon,
                    "020" or "20" => BusinessType.Antiques,
                    "021" or "21" or "070" or "70" => BusinessType.Entertainment,
                    "022" or "22" or "155" => BusinessType.GlobalOrLocalRecruitment,
                    "023" or "23" or "160" => BusinessType.Tourism,
                    "030" or "30" => BusinessType.BankOrFinance,
                    "090" or "90" => BusinessType.HotelOrRestaurant,
                    "110" => BusinessType.Insurance,
                    "140" => BusinessType.RealEstate,
                    "150" => BusinessType.Education,
                    _ => BusinessType.Other
                };
            }

            return null;
        }

        private Nationality MapNationality(string code)
        {
            return code switch
            {
                "000" => Nationality.Thai,
                // "999" => Nationality.Others,
                "A10" => Nationality.American,
                "A15" => Nationality.Argentinian,
                "A20" => Nationality.Australian,
                "A30" => Nationality.Austrian,
                "A40" => Nationality.Emirian,
                "A46" => Nationality.Andorran,
                "B02" => Nationality.Bahamian,
                "B03" => Nationality.Bahrainian,
                "B04" => Nationality.Bangladeshi,
                "B05" => Nationality.Belgian,
                "B06" => Nationality.Bermudan,
                "B07" => Nationality.Brazilian,
                "B08" => Nationality.Bolivian,
                "B10" => Nationality.British,
                "B20" => Nationality.Burmese,
                "B30" => Nationality.Bruneian,
                "B40" => Nationality.VirginIslander,
                "C10" => Nationality.Canadian,
                "C12" => Nationality.Caymanian,
                // "C15" => Nationality.Channel island,
                "C20" => Nationality.Chinese,
                "C30" => Nationality.Colombian,
                "C40" => Nationality.Cypriot,
                "C41" => Nationality.Cameroonian,
                "C46" => Nationality.Croatian,
                "C49" => Nationality.Cambodian,
                "D10" => Nationality.Danish,
                "D20" => Nationality.Dutch,
                "D30" => Nationality.German,
                "D40" => Nationality.Dominican,
                "E10" => Nationality.British,
                "E20" => Nationality.Ecuadorean,
                "F10" => Nationality.Filipino,
                "F20" => Nationality.French,
                "F30" => Nationality.Finnish,
                "G10" => Nationality.German,
                // "G20" => Nationality.Guernsey,
                "G30" => Nationality.Gibralterian,
                "G40" => Nationality.Greek,
                // "G50" => Nationality.Guernsey,
                "H03" => Nationality.Haitian,
                "H08" => Nationality.Dutch,
                "H10" => Nationality.Honduran,
                "H20" => Nationality.HongKonger,
                "I05" => Nationality.Icelander,
                "I10" => Nationality.Indian,
                "I15" => Nationality.Indonesian,
                "I16" => Nationality.Irish,
                "I17" => Nationality.Israeli,
                "I20" => Nationality.Italian,
                "I21" => Nationality.Iranian,
                "J10" => Nationality.Japanese,
                "J40" => Nationality.Jordanian,
                "K10" => Nationality.SouthKorean,
                "K50" => Nationality.NorthKorean,
                "L05" => Nationality.Laotian,
                "L06" => Nationality.Lebanese,
                "L10" => Nationality.Luxembourger,
                "L20" => Nationality.Liberian,
                "L30" => Nationality.Liechtensteiner,
                "M10" => Nationality.Malaysian,
                "M20" => Nationality.Mexican,
                "M30" => Nationality.Mauritian,
                "M43" => Nationality.Moroccan,
                "N10" => Nationality.NewZealander,
                "N20" => Nationality.Norwegian,
                "N30" => Nationality.Nepalese,
                "N40" => Nationality.Nigerian,
                "P05" => Nationality.Pakistani,
                "P07" => Nationality.Panamanian,
                "P10" => Nationality.Portuguese,
                "P20" => Nationality.Peruvian,
                "Q10" => Nationality.Qatari,
                "R10" => Nationality.Russian,
                "S10" => Nationality.British, // Scottish
                "S20" => Nationality.Singaporean,
                "S30" => Nationality.Spanish,
                "S35" => Nationality.SriLankan,
                "S40" => Nationality.Swedish,
                "S50" => Nationality.Swiss,
                "S60" => Nationality.Samoan,
                "S70" => Nationality.SouthKorean,
                "S74" => Nationality.SaudiArabian,
                "T10" => Nationality.Taiwanese,
                "T30" => Nationality.Turkish,
                "T40" => Nationality.Tongan,
                "V10" => Nationality.Vietnamese,
                // "Y01" => Nationality.Yugoslav,
                // "Z99" => Nationality.Others,
                _ => throw new NotSupportedException("Invalid Nationality Code")
            };
        }

        private SuitabilityRiskLevel MapSuitRiskLevel(Decimal? score)
        {
            return score switch
            {
                < 15 => SuitabilityRiskLevel.Level1,
                >= 15 and < 22 => SuitabilityRiskLevel.Level2,
                >= 22 and < 30 => SuitabilityRiskLevel.Level3,
                >= 30 and < 37 => SuitabilityRiskLevel.Level4,
                <= 37 or _ => SuitabilityRiskLevel.Level5,
            };
        }

        public string GetBuilding(GetAddressInfoResponseItem? address)
        {
            var building = string.Empty;
            if (!string.IsNullOrWhiteSpace(address?.Building) && !string.IsNullOrWhiteSpace(address.Village))
            {
                building = address.Building + ", " + address.Village;
            }
            else if (string.IsNullOrWhiteSpace(address?.Building) && !string.IsNullOrWhiteSpace(address?.Village))
            {
                building = address.Village;
            }
            else if (!string.IsNullOrWhiteSpace(address?.Building) && string.IsNullOrWhiteSpace(address.Village))
            {
                building = address.Building;
            }

            return building;
        }

        private static Occupation MapKycOccupationCodeToOccupation(string kycOccpCode)
        {
            return kycOccpCode switch
            {
                "001" or "015" => Occupation.GovernmentOfficer,
                "003" or "005" or "006" => Occupation.BusinessOwner,
                "014" => Occupation.CompanyEmployee,
                "016" => Occupation.Freelance,
                "018" => Occupation.HousewifeOrSteward,
                // "004" or "013" or "017" or "019" => Occupation.Other,
                _ => Occupation.Other
            };
        }

        public (Occupation, string?, BusinessType?, string?) GetOccupationAndBusinessType(
            GetKYCInfoResponseItem? kycInfo)
        {
            string[]? occupationSplit = kycInfo?.Occpdetail.Split("|");

            //1. Occupation
            var occupation = MapOccupation(occupationSplit?.ElementAtOrDefault(0) ?? "170");
            if (occupation == Occupation.Other)
                occupation = MapKycOccupationCodeToOccupation(kycInfo?.Occupationcode ?? string.Empty);

            //2. Occupation Other
            string? occupationOther = occupation == Occupation.Other
                ? occupationSplit?.ElementAtOrDefault(1) ?? "อื่นๆ"
                : null;

            //3. Business Type
            var businessType = MapBusinessType(occupation, kycInfo?.Occupationcode);

            //4. Business Type Other
            string? businessTypeOther = occupationSplit?.ElementAtOrDefault(3) ?? occupationSplit?[^1];
            if (businessType == BusinessType.Other && occupation is Occupation.Freelance
                    or Occupation.BusinessOwner or Occupation.FamilyBusiness or Occupation.Other
                    or Occupation.Individual)
            {
                if (string.IsNullOrEmpty(businessTypeOther)) businessTypeOther = "อื่นๆ";
            }
            else
            {
                businessTypeOther = null;
            }

            return (occupation, occupationOther, businessType, businessTypeOther);
        }
    }
}
