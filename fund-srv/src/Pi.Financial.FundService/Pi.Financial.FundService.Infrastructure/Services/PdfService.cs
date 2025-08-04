using System.Globalization;
using Microsoft.Extensions.Logging;
using Pi.Financial.Client.PdfService.Api;
using Pi.Financial.Client.PdfService.Model;
using Pi.Financial.FundService.Application.Models.Bank;
using Pi.Financial.FundService.Application.Models.Customer;
using Pi.Financial.FundService.Application.Models.Enums;
using Pi.Financial.FundService.Application.Services.PdfService;
using Pi.Financial.FundService.Domain.AccountOpening.Events.Models;
using Document = Pi.Financial.Client.PdfService.Model.Document;

namespace Pi.Financial.FundService.Infrastructure.Services
{
    public class PdfService : IPdfService
    {
        private readonly IPdfServiceApi _pdfService;
        private readonly ILogger<PdfService> _logger;
        private const string DateFormat = "ddMMyyyy";

        public PdfService(IPdfServiceApi pdfService, ILogger<PdfService> logger)
        {
            _pdfService = pdfService;
            _logger = logger;
        }

        public async Task<IEnumerable<Domain.AccountOpening.Events.Models.Document>> GenerateFundConnextDocuments(
            CustomerInfo customerInfo,
            CustomerAccount customerAccount,
            FatcaInfo fatcaInfo,
            Crs crs,
            string? ndidRequestId,
            DateTime? ndidDateTime,
            DateTime? dopaDateTime)
        {
            var addressById = new AddressDto(
                no: customerInfo.IdentificationDocument.No,
                district: customerInfo.IdentificationDocument.District,
                subDistrict: customerInfo.IdentificationDocument.SubDistrict,
                province: customerInfo.IdentificationDocument.Province,
                postalCode: customerInfo.IdentificationDocument.PostalCode,
                country: customerInfo.IdentificationDocument.Country.ToString()
            )
            {
                Floor = customerInfo.IdentificationDocument.Floor ?? string.Empty,
                Building = customerInfo.IdentificationDocument.Building ?? string.Empty,
                RoomNo = customerInfo.IdentificationDocument.RoomNo ?? string.Empty,
                Soi = customerInfo.IdentificationDocument.Soi ?? string.Empty,
                Road = customerInfo.IdentificationDocument.Road ?? string.Empty,
                Moo = customerInfo.IdentificationDocument.Moo ?? string.Empty,
            };

            var addressCurrent = customerInfo.Current != null
                ? new AddressDto(
                    no: customerInfo.Current.No,
                    district: customerInfo.Current.District,
                    subDistrict: customerInfo.Current.SubDistrict,
                    province: customerInfo.Current.Province,
                    postalCode: customerInfo.Current.PostalCode,
                    country: customerInfo.Current.Country.ToString()
                )
                {
                    Floor = customerInfo.Current.Floor ?? string.Empty,
                    Building = customerInfo.Current.Building ?? string.Empty,
                    RoomNo = customerInfo.Current.RoomNo ?? string.Empty,
                    Soi = customerInfo.Current.Soi ?? string.Empty,
                    Road = customerInfo.Current.Road ?? string.Empty,
                    Moo = customerInfo.Current.Moo ?? string.Empty,
                }
                : addressById;

            var addressWorkplace = customerInfo.Work != null
                ? new AddressDto(
                    no: customerInfo.Work.No,
                    district: customerInfo.Work.District,
                    subDistrict: customerInfo.Work.SubDistrict,
                    province: customerInfo.Work.Province,
                    postalCode: customerInfo.Work.PostalCode,
                    country: customerInfo.Work.Country.ToString()
                )
                {
                    Floor = customerInfo.Work.Floor ?? string.Empty,
                    Building = customerInfo.Work.Building ?? string.Empty,
                    RoomNo = customerInfo.Work.RoomNo ?? string.Empty,
                    Soi = customerInfo.Work.Soi ?? string.Empty,
                    Road = customerInfo.Work.Road ?? string.Empty,
                    Moo = customerInfo.Work.Moo ?? string.Empty,
                }
                : null;

            (List<BankAccountDto> subscriptionBankAccountsDtos, List<BankAccountDto> redemptionBankAccountDtos) =
                GetBankAccountDtos(
                    customerAccount.SubscriptionBankAccount,
                    customerAccount.RedemptionBankAccount,
                    customerInfo.FirstNameTh,
                    customerInfo.LastNameTh);
            var fundSubscriptionForm = new FundSubscriptionFormDto(
                intermediaryName: "บล. พาย จำกัด (มหาชน)",
                intermediaryProcessDate: customerAccount.OpenDate.ToString(DateFormat, CultureInfo.InvariantCulture),
                thaiId: new CustomerCardDto
                (
                    cardNumber: customerInfo.CardNumber,
                    expiryDate: customerInfo.CardExpiryDate?.ToString(DateFormat, CultureInfo.InvariantCulture) ?? "",
                    isLifetime: customerInfo.CardIsLifetime,
                    issuingCountry: $"{Country.Thailand.ToString()}"
                ),
                titleInfo: new TitleDto
                (
                    title: customerInfo.Title switch
                    {
                        Title.Mrs => TitleDto.TitleEnum.Mrs,
                        Title.Mr => TitleDto.TitleEnum.Mr,
                        Title.Miss => TitleDto.TitleEnum.Miss,
                        _ => TitleDto.TitleEnum.Other,
                    },
                    otherMessage: customerInfo.TitleOther ?? ""
                ),
                name: new NameDto
                (
                    nameTh: $"{customerInfo.FirstNameTh} {customerInfo.LastNameTh}",
                    nameEn: $"{customerInfo.FirstNameEn} {customerInfo.LastNameEn}"
                ),
                birthDate: customerInfo.BirthDate.ToString(DateFormat, CultureInfo.InvariantCulture),
                nationality: customerInfo.Nationality.ToString(),
                maritalStatus: customerInfo.MaritalStatus == MaritalStatus.Married
                    ? FundSubscriptionFormDto.MaritalStatusEnum.Married
                    : FundSubscriptionFormDto.MaritalStatusEnum.Single,
                spouseName: customerInfo.Spouse != null
                    ? new NameDto
                    (
                        nameTh: $"{customerInfo.Spouse.FirstNameTh} {customerInfo.Spouse.LastNameTh}",
                        nameEn: $"{customerInfo.Spouse.FirstNameEn} {customerInfo.Spouse.LastNameEn}"
                    )
                    : null!,
                phoneMobile: customerInfo.MobileNumber,
                phoneHome: customerInfo.Phone ?? string.Empty,
                fax: customerInfo.Fax ?? string.Empty,
                email: customerInfo.Email ?? string.Empty,
                addressById: addressById,
                addressCurrent: new MailingAddressDto(
                    mailingAddress: customerInfo.CurrentAddressSameAsFlag == CurrentAddressSameAsFlag.True
                        ? MailingAddressDto.MailingAddressEnum.Id
                        : MailingAddressDto.MailingAddressEnum.Other,
                    other: customerInfo.CurrentAddressSameAsFlag == CurrentAddressSameAsFlag.True
                        ? addressById
                        : addressCurrent
                ),
                occupationInfo: new OccupationDto
                (
                    occupation: customerInfo.OccupationId switch
                    {
                        Occupation.Farmer => OccupationDto.OccupationEnum.Agriculturist,
                        Occupation.Freelance => OccupationDto.OccupationEnum.SelfEmployed,
                        Occupation.Investor => OccupationDto.OccupationEnum.Investor,
                        Occupation.Politician => OccupationDto.OccupationEnum.Politician,
                        Occupation.Retired => OccupationDto.OccupationEnum.Retirement,
                        Occupation.Student => OccupationDto.OccupationEnum.Student,
                        Occupation.Teacher => OccupationDto.OccupationEnum.Teacher,
                        Occupation.BusinessOwner => OccupationDto.OccupationEnum.BusinessOwner,
                        Occupation.CompanyEmployee => OccupationDto.OccupationEnum.CorporateEmployee,
                        Occupation.FamilyBusiness => OccupationDto.OccupationEnum.FamilyBusiness,
                        Occupation.GovernmentOfficer => OccupationDto.OccupationEnum.GovernmentEmployee,
                        Occupation.DoctorOrNurse => OccupationDto.OccupationEnum.DoctorNurse,
                        Occupation.HousewifeOrSteward => OccupationDto.OccupationEnum.Housewife,
                        Occupation.MonkOrPriest => OccupationDto.OccupationEnum.Priest,
                        Occupation.StateEnterpriseEmployee => OccupationDto.OccupationEnum.StateEnterpriseEmployee,
                        _ => OccupationDto.OccupationEnum.Other
                    },
                    otherMessage: customerInfo.OccupationOther ?? ""
                ),
                businessInfo: new BusinessTypeDto
                (
                    businessType: customerInfo.BusinessTypeId == null
                        ? null
                        : customerInfo.BusinessTypeId switch
                        {
                            BusinessType.Antiques => BusinessTypeDto.BusinessTypeEnum.AntiqueTrading,
                            BusinessType.Education => BusinessTypeDto.BusinessTypeEnum.Education,
                            BusinessType.Entertainment => BusinessTypeDto.BusinessTypeEnum.EntertainmentBusiness,
                            BusinessType.Insurance => BusinessTypeDto.BusinessTypeEnum.InsuranceAssurance,
                            BusinessType.Tourism => BusinessTypeDto.BusinessTypeEnum.Travel,
                            BusinessType.Weapon => BusinessTypeDto.BusinessTypeEnum.Armament,
                            BusinessType.ForeignExchange => BusinessTypeDto.BusinessTypeEnum.ForeignCurrencyExchange,
                            BusinessType.RealEstate => BusinessTypeDto.BusinessTypeEnum.PropertyRealEstate,
                            BusinessType.TransferMoney => BusinessTypeDto.BusinessTypeEnum.MoneyTransfer,
                            BusinessType.BankOrFinance => BusinessTypeDto.BusinessTypeEnum.FinancialBanking,
                            BusinessType.CasinoOrGambling => BusinessTypeDto.BusinessTypeEnum.Casino,
                            BusinessType.GoldOrGem => BusinessTypeDto.BusinessTypeEnum.JewelryGoldTrading,
                            BusinessType.HotelOrRestaurant => BusinessTypeDto.BusinessTypeEnum.HotelRestaurant,
                            BusinessType.GlobalOrLocalRecruitment => BusinessTypeDto.BusinessTypeEnum
                                .ForeignWorkerEmploymentAgency,
                            BusinessType.CooperativeOrTempleOrShrine => BusinessTypeDto.BusinessTypeEnum
                                .CooperativeFoundationAssociationClubTempleMosqueShrine,
                            _ => BusinessTypeDto.BusinessTypeEnum.Other
                        },
                    otherMessage: customerInfo.BusinessTypeOther ?? ""
                ),
                companyName: customerInfo.CompanyName ?? string.Empty,
                addressWorkplace: addressWorkplace!,
                workingPosition: customerInfo.WorkPosition ?? string.Empty,
                incomeSourceCountry: customerInfo.IncomeSourceCountry.ToString(),
                incomeSourceInfo: new IncomeSourceDto
                (
                    incomeSource: customerInfo.IncomeSource
                        .Select(s =>
                        {
                            return s switch
                            {
                                { Key: IncomeSource.Business } => IncomeSourceDto.IncomeSourceEnum.OwnBusiness,
                                { Key: IncomeSource.Heritage } => IncomeSourceDto.IncomeSourceEnum.Inheritance,
                                { Key: IncomeSource.Investment } => IncomeSourceDto.IncomeSourceEnum.Investment,
                                { Key: IncomeSource.Retirement } => IncomeSourceDto.IncomeSourceEnum.RetirementFund,
                                { Key: IncomeSource.Salary } => IncomeSourceDto.IncomeSourceEnum.Salary,
                                { Key: IncomeSource.Savings } => IncomeSourceDto.IncomeSourceEnum.Savings,
                                _ => IncomeSourceDto.IncomeSourceEnum.Other
                            };
                        })
                        .Distinct()
                        .ToList(),
                    otherMessage: customerInfo.IncomeSourceOther ?? string.Empty
                ),
                monthlyIncome: customerInfo.MonthlyIncomeLevel switch
                {
                    MonthlyIncomeLevel.Level1 => FundSubscriptionFormDto.MonthlyIncomeEnum.Below15k,
                    MonthlyIncomeLevel.Level2 => FundSubscriptionFormDto.MonthlyIncomeEnum.From15kTo30k,
                    MonthlyIncomeLevel.Level3 => FundSubscriptionFormDto.MonthlyIncomeEnum.From30kTo50k,
                    MonthlyIncomeLevel.Level4 => FundSubscriptionFormDto.MonthlyIncomeEnum.From50kTo100k,
                    MonthlyIncomeLevel.Level5 => FundSubscriptionFormDto.MonthlyIncomeEnum.From100kTo500k,
                    MonthlyIncomeLevel.Level6 => FundSubscriptionFormDto.MonthlyIncomeEnum.From500kTo1m,
                    MonthlyIncomeLevel.Level7 => FundSubscriptionFormDto.MonthlyIncomeEnum.From1mTo4m,
                    MonthlyIncomeLevel.Level8 => FundSubscriptionFormDto.MonthlyIncomeEnum.From4mTo10m,
                    MonthlyIncomeLevel.Level9 => FundSubscriptionFormDto.MonthlyIncomeEnum.Over10m,
                    _ => throw new ArgumentOutOfRangeException()
                },
                assetValue: customerInfo.AssetValue,
                politicianPosition:
                customerInfo.RelatedPoliticalPerson
                    ? customerInfo.PoliticalRelatedPersonPosition ?? "yes"
                    : string.Empty,
                mailingAddressInfo: new MailingAddressDto
                (
                    mailingAddress: customerInfo.MailingAddressOption switch
                    {
                        MailingAddressOption.IdAddress => MailingAddressDto.MailingAddressEnum.Id,
                        MailingAddressOption.CurrentAddress => MailingAddressDto.MailingAddressEnum.Current,
                        MailingAddressOption.WorkAddress => MailingAddressDto.MailingAddressEnum.Workplace,
                        _ => MailingAddressDto.MailingAddressEnum.Other
                    },
                    other: customerInfo.MailingAddressOption switch
                    {
                        MailingAddressOption.IdAddress => addressById,
                        MailingAddressOption.CurrentAddress => addressCurrent,
                        MailingAddressOption.WorkAddress => addressWorkplace!,
                        _ => addressCurrent,
                    }
                ),
                mailingMethodInfo: new MailingMethodDto
                (
                    mailingMethod: customerInfo.MailingMethod switch
                    {
                        MailingMethod.Email => MailingMethodDto.MailingMethodEnum.Email,
                        MailingMethod.Fax => MailingMethodDto.MailingMethodEnum.Fax,
                        _ => MailingMethodDto.MailingMethodEnum.Post,
                    },
                    faxNumber: customerInfo.Fax ?? string.Empty
                ),
                investmentObjectiveInfo: new InvestmentObjectiveDto(
                    investmentObjective: customerInfo.InvestmentObjective
                        .Select(i =>
                        {
                            return i switch
                            {
                                { Key: InvestmentObjective.Investment } => InvestmentObjectiveDto.InvestmentObjectiveEnum.Investment,
                                { Key: InvestmentObjective.RetirementInvestment } => InvestmentObjectiveDto.InvestmentObjectiveEnum.Retirement,
                                { Key: InvestmentObjective.ForTaxBenefits } => InvestmentObjectiveDto.InvestmentObjectiveEnum.TaxBenefits,
                                _ => InvestmentObjectiveDto.InvestmentObjectiveEnum.Other,
                            };
                        })
                        .Distinct()
                        .ToList(),
                    otherMessage: customerInfo.InvestmentObjective.Any(i => i.Key == InvestmentObjective.PleaseSpecify)
                        ? string.Join(
                            ", ",
                            customerInfo.InvestmentObjective
                                .Where(i => i.Key == InvestmentObjective.PleaseSpecify)
                                .SelectMany(i => i)
                                .ToList())
                        : string.Empty),
                redemptionBankAccounts: redemptionBankAccountDtos,
                subscriptionBankAccounts: subscriptionBankAccountsDtos,
                applicantName: $"{customerInfo.FirstNameTh} {customerInfo.LastNameTh}",
                applicantSignatureHeader1: "", // $"NDID Request ID: {ndidRequestId}", // Ndid Request Id
                applicantSignatureHeader2: "", // $"NDID Request Date: ${ndidDateTime.ToString(CultureInfo.InvariantCulture)}", // Ndid Request Date/Time
                applicantSignature: "",
                applicantSignatureFooter1: "",
                applicantSignatureFooter2: "",
                applicantSignatureFooter3: "",
                applicantSignatureFooter4: $"{customerInfo.FirstNameTh} {customerInfo.LastNameTh}"
            );


            var suitabilityAnswers =
                new List<SuitabilityTestFormAnswerDto>(12 + customerInfo.SuitabilityForm.SuitNo4.Count);

            suitabilityAnswers.AddRange(new[]
                {
                    new SuitabilityTestFormAnswerDto(no: 1,
                        choice: (SuitabilityTestFormAnswerDto.ChoiceEnum)customerInfo.SuitabilityForm.SuitNo1),
                    new SuitabilityTestFormAnswerDto(no: 2,
                        choice: (SuitabilityTestFormAnswerDto.ChoiceEnum)customerInfo.SuitabilityForm.SuitNo2),
                    new SuitabilityTestFormAnswerDto(no: 3,
                        choice: (SuitabilityTestFormAnswerDto.ChoiceEnum)customerInfo.SuitabilityForm.SuitNo3),
                }
            );
            suitabilityAnswers.AddRange(customerInfo.SuitabilityForm.SuitNo4.Select(answer =>
                new SuitabilityTestFormAnswerDto(no: 4, choice: (SuitabilityTestFormAnswerDto.ChoiceEnum)answer)));
            suitabilityAnswers.AddRange(new[]
            {
                new SuitabilityTestFormAnswerDto(no: 1,
                    choice: (SuitabilityTestFormAnswerDto.ChoiceEnum)customerInfo.SuitabilityForm.SuitNo1),
                new SuitabilityTestFormAnswerDto(no: 2,
                    choice: (SuitabilityTestFormAnswerDto.ChoiceEnum)customerInfo.SuitabilityForm.SuitNo2),
                new SuitabilityTestFormAnswerDto(no: 3,
                    choice: (SuitabilityTestFormAnswerDto.ChoiceEnum)customerInfo.SuitabilityForm.SuitNo3),
                new SuitabilityTestFormAnswerDto(no: 5,
                    choice: (SuitabilityTestFormAnswerDto.ChoiceEnum)customerInfo.SuitabilityForm.SuitNo5),
                new SuitabilityTestFormAnswerDto(no: 6,
                    choice: (SuitabilityTestFormAnswerDto.ChoiceEnum)customerInfo.SuitabilityForm.SuitNo6),
                new SuitabilityTestFormAnswerDto(no: 7,
                    choice: (SuitabilityTestFormAnswerDto.ChoiceEnum)customerInfo.SuitabilityForm.SuitNo7),
                new SuitabilityTestFormAnswerDto(no: 8,
                    choice: (SuitabilityTestFormAnswerDto.ChoiceEnum)customerInfo.SuitabilityForm.SuitNo8),
                new SuitabilityTestFormAnswerDto(no: 9,
                    choice: (SuitabilityTestFormAnswerDto.ChoiceEnum)customerInfo.SuitabilityForm.SuitNo9),
                new SuitabilityTestFormAnswerDto(no: 10,
                    choice: (SuitabilityTestFormAnswerDto.ChoiceEnum)customerInfo.SuitabilityForm.SuitNo10),
                new SuitabilityTestFormAnswerDto(no: 11,
                    choice: (SuitabilityTestFormAnswerDto.ChoiceEnum)customerInfo.SuitabilityForm.SuitNo11),
                new SuitabilityTestFormAnswerDto(no: 12,
                    choice: (SuitabilityTestFormAnswerDto.ChoiceEnum)customerInfo.SuitabilityForm.SuitNo12),
            });

            var suitabilityTestForm = new SuitabilityTestFormDto
            (
                suitabilityAnswers: suitabilityAnswers,
                score: customerInfo.SuitabilityScore,
                date: customerInfo.SuitabilityEvaluationDate.ToString(DateFormat, CultureInfo.InvariantCulture),
                // TODO: Find 2 fields
                assessorName: "",
                inspectorName: ""
            );

            FatcaCRSFormDto fatcaCrsForm = new FatcaCRSFormDto
            (
                applicantName: fatcaInfo.ApplicantNameEN,
                applicantNameTH: fatcaInfo.ApplicantNameTH,
                applicantSignature: fatcaInfo.ApplicantNameTH, // If Ndid Available should also attached ndid request id, with date/time of approval
                nationality: fatcaInfo.Nationality.ToString(),
                date: customerAccount.OpenDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture),
                idNumber: fatcaInfo.ThaiIdNumber ?? fatcaInfo.PassportNumber ?? "",
                isUSCitizen: fatcaInfo.IsUSCitizen,
                isGreenCardHolder: fatcaInfo.IsGreenCardHolder,
                isUSForTax: fatcaInfo.IsUSForTax,
                isBornInUS: fatcaInfo.IsBornInUS,
                haveUSResidence: fatcaInfo.HaveUSResidence,
                haveUSPhoneNumber: fatcaInfo.HaveUSPhoneNumber,
                haveUSAccountReceiver: fatcaInfo.HaveUSAccountReceiver,
                haveUSSignatoryAuthority: fatcaInfo.HaveUSSignatoryAuthority,
                haveSoleAddress: false,
                officerSignature: fatcaInfo.OfficerSignature,
                crs: crs.Details.Select(
                    x => new CRSDto(
                        x.CountryOfTaxResidence,
                        x.Tin ?? "",
                        x.Reason?.ToString() ?? "",
                        x.ReasonDesc ?? ""
                    )
                ).ToList() ?? new List<CRSDto>(),
                placeOfBirthCity: crs.PlaceOfBirthCity ?? "",
                placeOfBirthCountry: crs.PlaceOfBirthCountry ?? ""
            );

            string address1 = string.Join(" ",
                new[]
                {
                    customerInfo.IdentificationDocument.No, customerInfo.IdentificationDocument.Building ?? "",
                    customerInfo.IdentificationDocument.RoomNo ?? "",
                    customerInfo.IdentificationDocument.Floor ?? "", customerInfo.IdentificationDocument.Moo ?? ""
                }.Where(x => x != "")
            );
            string address2 = string.Join(" ",
                new[]
                {
                    customerInfo.IdentificationDocument.Soi ?? "", customerInfo.IdentificationDocument.Road ?? "",
                    customerInfo.IdentificationDocument.SubDistrict, customerInfo.IdentificationDocument.District
                }.Where(x => x != "")
            );
            string address3 = string.Join(" ",
                new[]
                {
                    customerInfo.IdentificationDocument.Province, customerInfo.IdentificationDocument.PostalCode,
                    customerInfo.IdentificationDocument.Country.ToString()
                }
            );

            var thaiIdForm = new ThaiIdFormDto(
                // TODO get img
                imageUrl: "",
                cardNumber: customerInfo.CardNumber,
                fullNameTh: $"{customerInfo.FirstNameTh} {customerInfo.LastNameTh}",
                firstNameEn: $"{customerInfo.FirstNameEn}",
                lastNameEn: $"{customerInfo.LastNameEn}",
                birthDate: customerInfo.BirthDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture),
                addressLine1: address1,
                addressLine2: address2,
                addressLine3: address3,
                cardIssuingDate: customerInfo.CardIssuedDate?.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) ?? string.Empty,
                cardExpiringDate: customerInfo.CardExpiryDate?.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) ?? string.Empty,
                verificationMethodDate: ndidDateTime?.Date.AddHours(7).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) ?? string.Empty,
                verificationMethodTime: ndidDateTime?.AddHours(7).ToString("HH:mm:ss") ?? string.Empty,
                verificationMethodRequestId: ndidRequestId ?? string.Empty,
                dopaCheckDate: dopaDateTime?.Date.AddHours(7).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) ?? string.Empty,
                dopaCheckTime: dopaDateTime?.AddHours(7).ToString("HH:mm:ss") ?? string.Empty,
                isDipChip: string.IsNullOrEmpty(ndidRequestId)
            );


            var req = new GenerateFundAccountOpeningDocumentsDto
            (
                fundSubscriptionForm: fundSubscriptionForm,
                suitabilityTestForm: suitabilityTestForm,
                fatcaCRSForm: fatcaCrsForm,
                thaiIdForm: thaiIdForm
            );
            _logger.LogInformation("fatcaCrsForm: {@fatcaCrsForm}", fatcaCrsForm);

            var response = await _pdfService.FundsControllerGenerateAccountOpeningDocumentsAsync(req);

            return response.Documents.Select(d => new Domain.AccountOpening.Events.Models.Document(
                DocumentType: d.DocType switch
                {
                    Document.DocTypeEnum.FATCAFORM => DocumentType.Fatca,
                    Document.DocTypeEnum.SUITABILITYFORM => DocumentType.SuitabilityForm,
                    Document.DocTypeEnum.FUNDOPENACCOUNTFORM => DocumentType.FundOpenAccountForm,
                    Document.DocTypeEnum.THAIIDFORM => DocumentType.IdCardForm, // TODO recheck
                    Document.DocTypeEnum.PDPAFORM => DocumentType.Pdpa,
                    Document.DocTypeEnum.POWEROFATTORNEYFORM => DocumentType.AttorneyForm,
                    _ => throw new ArgumentOutOfRangeException()
                },
                PreSignedUrl: d.PresignedUrl
            ));
        }

        /// <summary>
        /// Re-arrange bank account data from fund-service to send to pdf-service as Fund Connext Open Account document format
        /// </summary>
        /// <param name="subscriptionBankAccounts">Subscription Bank Accounts</param>
        /// <param name="redemptionBankAccounts">Redemption Bank Accounts</param>
        /// <param name="customerFirstName">First name in Thai</param>
        /// <param name="customerLastName">Last name in Thai</param>
        /// <returns>2 list variables:
        /// <list type="bullet">
        /// <item><description> subscription bank accounts </description></item>
        /// <item><description> redemption bank accounts </description></item>
        /// </list>
        /// </returns>
        public (List<BankAccountDto>, List<BankAccountDto>) GetBankAccountDtos(
            List<BankAccount> subscriptionBankAccounts,
            List<BankAccount> redemptionBankAccounts,
            string customerFirstName,
            string customerLastName
            )
        {
            //When no subscription but has redemption, will move redemption to subscription
            List<BankAccountDto> subscription;
            if (subscriptionBankAccounts.Count == 0)
            {
                subscription = redemptionBankAccounts.Select(b => new BankAccountDto
                (
                    isMainAccount: b.IsDefault,
                    accountName: $"{customerFirstName} {customerLastName}",
                    accountNumber: b.BankAccountNo,
                    bankName: b.BankCode,
                    branchName: b.BankBranchCode
                )).ToList();
            }
            else
            {
                subscription = subscriptionBankAccounts.Select(b => new BankAccountDto
                (
                    isMainAccount: b.IsDefault,
                    accountName: $"{customerFirstName} {customerLastName}",
                    accountNumber: b.BankAccountNo,
                    bankName: b.BankCode,
                    branchName: b.BankBranchCode
                )).ToList();
            }

            var redemption =
                redemptionBankAccounts.Exists(r => subscriptionBankAccounts.Exists(s => s.BankAccountNo != r.BankAccountNo))
                    ? redemptionBankAccounts.Select(b => new BankAccountDto
                    (
                        isMainAccount: b.IsDefault,
                        accountName: $"{customerFirstName} {customerLastName}",
                        accountNumber: b.BankAccountNo,
                        bankName: b.BankCode,
                        branchName: b.BankBranchCode
                    )).ToList()
                    : [];

            return (subscription, redemption);
        }
    }
}
