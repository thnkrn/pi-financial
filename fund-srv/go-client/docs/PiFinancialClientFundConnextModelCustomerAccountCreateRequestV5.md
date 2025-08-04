# PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**IdentificationCardType** | Pointer to **string** |  | [optional] 
**AccompanyingDocument** | Pointer to **NullableString** |  | [optional] 
**Title** | Pointer to **string** |  | [optional] 
**MaritalStatus** | Pointer to **string** |  | [optional] 
**OccupationId** | Pointer to **string** |  | [optional] 
**BusinessTypeId** | Pointer to **NullableString** |  | [optional] 
**MonthlyIncomeLevel** | Pointer to **string** |  | [optional] 
**CurrentAddressSameAsFlag** | Pointer to **NullableString** |  | [optional] 
**SuitabilityRiskLevel** | Pointer to **string** |  | [optional] 
**CddScore** | Pointer to **NullableString** |  | [optional] 
**OpenFundConnextFormFlag** | Pointer to **string** |  | [optional] 
**OpenChannel** | Pointer to **string** |  | [optional] 
**InvestorClass** | Pointer to **NullableString** |  | [optional] 
**PassportCountry** | Pointer to **NullableString** |  | [optional] 
**CardNumber** | Pointer to **NullableString** |  | [optional] 
**CardExpiryDate** | Pointer to **NullableString** |  | [optional] 
**TitleOther** | Pointer to **NullableString** |  | [optional] 
**EnFirstName** | Pointer to **NullableString** |  | [optional] 
**EnLastName** | Pointer to **NullableString** |  | [optional] 
**ThFirstName** | Pointer to **NullableString** |  | [optional] 
**ThLastName** | Pointer to **NullableString** |  | [optional] 
**BirthDate** | Pointer to **NullableString** |  | [optional] 
**Nationality** | Pointer to **NullableString** |  | [optional] 
**MobileNumber** | Pointer to **NullableString** |  | [optional] 
**Email** | Pointer to **NullableString** |  | [optional] 
**Phone** | Pointer to **NullableString** |  | [optional] 
**Fax** | Pointer to **NullableString** |  | [optional] 
**Spouse** | Pointer to [**PiFinancialClientFundConnextModelSpouse**](PiFinancialClientFundConnextModelSpouse.md) |  | [optional] 
**OccupationOther** | Pointer to **NullableString** |  | [optional] 
**BusinessTypeOther** | Pointer to **NullableString** |  | [optional] 
**AssetValue** | Pointer to **NullableFloat64** |  | [optional] 
**IncomeSource** | Pointer to **NullableString** |  | [optional] 
**IncomeSourceOther** | Pointer to **NullableString** |  | [optional] 
**IdentificationDocument** | Pointer to [**PiFinancialClientFundConnextModelAddressForProfileV5**](PiFinancialClientFundConnextModelAddressForProfileV5.md) |  | [optional] 
**Current** | Pointer to [**PiFinancialClientFundConnextModelAddressForProfileV5**](PiFinancialClientFundConnextModelAddressForProfileV5.md) |  | [optional] 
**CompanyName** | Pointer to **NullableString** |  | [optional] 
**Work** | Pointer to [**PiFinancialClientFundConnextModelAddressForProfileV5**](PiFinancialClientFundConnextModelAddressForProfileV5.md) |  | [optional] 
**WorkPosition** | Pointer to **NullableString** |  | [optional] 
**RelatedPoliticalPerson** | Pointer to **NullableBool** |  | [optional] 
**PoliticalRelatedPersonPosition** | Pointer to **NullableString** |  | [optional] 
**CanAcceptFxRisk** | Pointer to **bool** |  | [optional] 
**CanAcceptDerivativeInvestment** | Pointer to **bool** |  | [optional] 
**SuitabilityEvaluationDate** | Pointer to **NullableString** |  | [optional] 
**Fatca** | Pointer to **bool** |  | [optional] 
**FatcaDeclarationDate** | Pointer to **NullableString** |  | [optional] 
**CrsPlaceOfBirthCountry** | Pointer to **NullableString** |  | [optional] 
**CrsPlaceOfBirthCity** | Pointer to **NullableString** |  | [optional] 
**CrsTaxResidenceInCountriesOtherThanTheUS** | Pointer to **NullableBool** |  | [optional] 
**CrsDetails** | Pointer to [**[]PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5CrsDetailsInner**](PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5CrsDetailsInner.md) |  | [optional] 
**CrsDeclarationDate** | Pointer to **NullableString** |  | [optional] 
**CddDate** | Pointer to **NullableString** |  | [optional] 
**ReferralPerson** | Pointer to **NullableString** |  | [optional] 
**ApplicationDate** | Pointer to **NullableString** |  | [optional] 
**IncomeSourceCountry** | Pointer to **NullableString** |  | [optional] 
**AcceptedBy** | Pointer to **NullableString** |  | [optional] 
**Approved** | Pointer to **bool** |  | [optional] 
**VulnerableFlag** | Pointer to **NullableBool** |  | [optional] 
**VulnerableDetail** | Pointer to **NullableString** |  | [optional] 
**NdidFlag** | Pointer to **NullableBool** |  | [optional] 
**NdidRequestId** | Pointer to **NullableString** |  | [optional] 
**SuitabilityForm** | Pointer to [**PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5SuitabilityForm**](PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5SuitabilityForm.md) |  | [optional] 
**KnowledgeAssessmentResult** | Pointer to **NullableBool** |  | [optional] 
**KnowledgeAssessmentForm** | Pointer to [**PiFinancialClientFundConnextModelIndividualInvestorV5ResponseKnowledgeAssessmentForm**](PiFinancialClientFundConnextModelIndividualInvestorV5ResponseKnowledgeAssessmentForm.md) |  | [optional] 

## Methods

### NewPiFinancialClientFundConnextModelCustomerAccountCreateRequestV5

`func NewPiFinancialClientFundConnextModelCustomerAccountCreateRequestV5() *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5`

NewPiFinancialClientFundConnextModelCustomerAccountCreateRequestV5 instantiates a new PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5 object
This constructor will assign default values to properties that have it defined,
and makes sure properties required by API are set, but the set of arguments
will change when the set of required properties is changed

### NewPiFinancialClientFundConnextModelCustomerAccountCreateRequestV5WithDefaults

`func NewPiFinancialClientFundConnextModelCustomerAccountCreateRequestV5WithDefaults() *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5`

NewPiFinancialClientFundConnextModelCustomerAccountCreateRequestV5WithDefaults instantiates a new PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5 object
This constructor will only assign default values to properties that have it defined,
but it doesn't guarantee that properties required by API are set

### GetIdentificationCardType

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) GetIdentificationCardType() string`

GetIdentificationCardType returns the IdentificationCardType field if non-nil, zero value otherwise.

### GetIdentificationCardTypeOk

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) GetIdentificationCardTypeOk() (*string, bool)`

GetIdentificationCardTypeOk returns a tuple with the IdentificationCardType field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetIdentificationCardType

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) SetIdentificationCardType(v string)`

SetIdentificationCardType sets IdentificationCardType field to given value.

### HasIdentificationCardType

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) HasIdentificationCardType() bool`

HasIdentificationCardType returns a boolean if a field has been set.

### GetAccompanyingDocument

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) GetAccompanyingDocument() string`

GetAccompanyingDocument returns the AccompanyingDocument field if non-nil, zero value otherwise.

### GetAccompanyingDocumentOk

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) GetAccompanyingDocumentOk() (*string, bool)`

GetAccompanyingDocumentOk returns a tuple with the AccompanyingDocument field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetAccompanyingDocument

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) SetAccompanyingDocument(v string)`

SetAccompanyingDocument sets AccompanyingDocument field to given value.

### HasAccompanyingDocument

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) HasAccompanyingDocument() bool`

HasAccompanyingDocument returns a boolean if a field has been set.

### SetAccompanyingDocumentNil

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) SetAccompanyingDocumentNil(b bool)`

 SetAccompanyingDocumentNil sets the value for AccompanyingDocument to be an explicit nil

### UnsetAccompanyingDocument
`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) UnsetAccompanyingDocument()`

UnsetAccompanyingDocument ensures that no value is present for AccompanyingDocument, not even an explicit nil
### GetTitle

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) GetTitle() string`

GetTitle returns the Title field if non-nil, zero value otherwise.

### GetTitleOk

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) GetTitleOk() (*string, bool)`

GetTitleOk returns a tuple with the Title field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetTitle

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) SetTitle(v string)`

SetTitle sets Title field to given value.

### HasTitle

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) HasTitle() bool`

HasTitle returns a boolean if a field has been set.

### GetMaritalStatus

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) GetMaritalStatus() string`

GetMaritalStatus returns the MaritalStatus field if non-nil, zero value otherwise.

### GetMaritalStatusOk

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) GetMaritalStatusOk() (*string, bool)`

GetMaritalStatusOk returns a tuple with the MaritalStatus field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetMaritalStatus

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) SetMaritalStatus(v string)`

SetMaritalStatus sets MaritalStatus field to given value.

### HasMaritalStatus

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) HasMaritalStatus() bool`

HasMaritalStatus returns a boolean if a field has been set.

### GetOccupationId

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) GetOccupationId() string`

GetOccupationId returns the OccupationId field if non-nil, zero value otherwise.

### GetOccupationIdOk

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) GetOccupationIdOk() (*string, bool)`

GetOccupationIdOk returns a tuple with the OccupationId field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetOccupationId

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) SetOccupationId(v string)`

SetOccupationId sets OccupationId field to given value.

### HasOccupationId

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) HasOccupationId() bool`

HasOccupationId returns a boolean if a field has been set.

### GetBusinessTypeId

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) GetBusinessTypeId() string`

GetBusinessTypeId returns the BusinessTypeId field if non-nil, zero value otherwise.

### GetBusinessTypeIdOk

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) GetBusinessTypeIdOk() (*string, bool)`

GetBusinessTypeIdOk returns a tuple with the BusinessTypeId field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetBusinessTypeId

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) SetBusinessTypeId(v string)`

SetBusinessTypeId sets BusinessTypeId field to given value.

### HasBusinessTypeId

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) HasBusinessTypeId() bool`

HasBusinessTypeId returns a boolean if a field has been set.

### SetBusinessTypeIdNil

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) SetBusinessTypeIdNil(b bool)`

 SetBusinessTypeIdNil sets the value for BusinessTypeId to be an explicit nil

### UnsetBusinessTypeId
`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) UnsetBusinessTypeId()`

UnsetBusinessTypeId ensures that no value is present for BusinessTypeId, not even an explicit nil
### GetMonthlyIncomeLevel

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) GetMonthlyIncomeLevel() string`

GetMonthlyIncomeLevel returns the MonthlyIncomeLevel field if non-nil, zero value otherwise.

### GetMonthlyIncomeLevelOk

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) GetMonthlyIncomeLevelOk() (*string, bool)`

GetMonthlyIncomeLevelOk returns a tuple with the MonthlyIncomeLevel field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetMonthlyIncomeLevel

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) SetMonthlyIncomeLevel(v string)`

SetMonthlyIncomeLevel sets MonthlyIncomeLevel field to given value.

### HasMonthlyIncomeLevel

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) HasMonthlyIncomeLevel() bool`

HasMonthlyIncomeLevel returns a boolean if a field has been set.

### GetCurrentAddressSameAsFlag

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) GetCurrentAddressSameAsFlag() string`

GetCurrentAddressSameAsFlag returns the CurrentAddressSameAsFlag field if non-nil, zero value otherwise.

### GetCurrentAddressSameAsFlagOk

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) GetCurrentAddressSameAsFlagOk() (*string, bool)`

GetCurrentAddressSameAsFlagOk returns a tuple with the CurrentAddressSameAsFlag field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetCurrentAddressSameAsFlag

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) SetCurrentAddressSameAsFlag(v string)`

SetCurrentAddressSameAsFlag sets CurrentAddressSameAsFlag field to given value.

### HasCurrentAddressSameAsFlag

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) HasCurrentAddressSameAsFlag() bool`

HasCurrentAddressSameAsFlag returns a boolean if a field has been set.

### SetCurrentAddressSameAsFlagNil

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) SetCurrentAddressSameAsFlagNil(b bool)`

 SetCurrentAddressSameAsFlagNil sets the value for CurrentAddressSameAsFlag to be an explicit nil

### UnsetCurrentAddressSameAsFlag
`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) UnsetCurrentAddressSameAsFlag()`

UnsetCurrentAddressSameAsFlag ensures that no value is present for CurrentAddressSameAsFlag, not even an explicit nil
### GetSuitabilityRiskLevel

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) GetSuitabilityRiskLevel() string`

GetSuitabilityRiskLevel returns the SuitabilityRiskLevel field if non-nil, zero value otherwise.

### GetSuitabilityRiskLevelOk

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) GetSuitabilityRiskLevelOk() (*string, bool)`

GetSuitabilityRiskLevelOk returns a tuple with the SuitabilityRiskLevel field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetSuitabilityRiskLevel

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) SetSuitabilityRiskLevel(v string)`

SetSuitabilityRiskLevel sets SuitabilityRiskLevel field to given value.

### HasSuitabilityRiskLevel

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) HasSuitabilityRiskLevel() bool`

HasSuitabilityRiskLevel returns a boolean if a field has been set.

### GetCddScore

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) GetCddScore() string`

GetCddScore returns the CddScore field if non-nil, zero value otherwise.

### GetCddScoreOk

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) GetCddScoreOk() (*string, bool)`

GetCddScoreOk returns a tuple with the CddScore field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetCddScore

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) SetCddScore(v string)`

SetCddScore sets CddScore field to given value.

### HasCddScore

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) HasCddScore() bool`

HasCddScore returns a boolean if a field has been set.

### SetCddScoreNil

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) SetCddScoreNil(b bool)`

 SetCddScoreNil sets the value for CddScore to be an explicit nil

### UnsetCddScore
`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) UnsetCddScore()`

UnsetCddScore ensures that no value is present for CddScore, not even an explicit nil
### GetOpenFundConnextFormFlag

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) GetOpenFundConnextFormFlag() string`

GetOpenFundConnextFormFlag returns the OpenFundConnextFormFlag field if non-nil, zero value otherwise.

### GetOpenFundConnextFormFlagOk

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) GetOpenFundConnextFormFlagOk() (*string, bool)`

GetOpenFundConnextFormFlagOk returns a tuple with the OpenFundConnextFormFlag field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetOpenFundConnextFormFlag

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) SetOpenFundConnextFormFlag(v string)`

SetOpenFundConnextFormFlag sets OpenFundConnextFormFlag field to given value.

### HasOpenFundConnextFormFlag

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) HasOpenFundConnextFormFlag() bool`

HasOpenFundConnextFormFlag returns a boolean if a field has been set.

### GetOpenChannel

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) GetOpenChannel() string`

GetOpenChannel returns the OpenChannel field if non-nil, zero value otherwise.

### GetOpenChannelOk

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) GetOpenChannelOk() (*string, bool)`

GetOpenChannelOk returns a tuple with the OpenChannel field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetOpenChannel

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) SetOpenChannel(v string)`

SetOpenChannel sets OpenChannel field to given value.

### HasOpenChannel

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) HasOpenChannel() bool`

HasOpenChannel returns a boolean if a field has been set.

### GetInvestorClass

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) GetInvestorClass() string`

GetInvestorClass returns the InvestorClass field if non-nil, zero value otherwise.

### GetInvestorClassOk

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) GetInvestorClassOk() (*string, bool)`

GetInvestorClassOk returns a tuple with the InvestorClass field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetInvestorClass

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) SetInvestorClass(v string)`

SetInvestorClass sets InvestorClass field to given value.

### HasInvestorClass

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) HasInvestorClass() bool`

HasInvestorClass returns a boolean if a field has been set.

### SetInvestorClassNil

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) SetInvestorClassNil(b bool)`

 SetInvestorClassNil sets the value for InvestorClass to be an explicit nil

### UnsetInvestorClass
`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) UnsetInvestorClass()`

UnsetInvestorClass ensures that no value is present for InvestorClass, not even an explicit nil
### GetPassportCountry

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) GetPassportCountry() string`

GetPassportCountry returns the PassportCountry field if non-nil, zero value otherwise.

### GetPassportCountryOk

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) GetPassportCountryOk() (*string, bool)`

GetPassportCountryOk returns a tuple with the PassportCountry field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetPassportCountry

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) SetPassportCountry(v string)`

SetPassportCountry sets PassportCountry field to given value.

### HasPassportCountry

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) HasPassportCountry() bool`

HasPassportCountry returns a boolean if a field has been set.

### SetPassportCountryNil

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) SetPassportCountryNil(b bool)`

 SetPassportCountryNil sets the value for PassportCountry to be an explicit nil

### UnsetPassportCountry
`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) UnsetPassportCountry()`

UnsetPassportCountry ensures that no value is present for PassportCountry, not even an explicit nil
### GetCardNumber

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) GetCardNumber() string`

GetCardNumber returns the CardNumber field if non-nil, zero value otherwise.

### GetCardNumberOk

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) GetCardNumberOk() (*string, bool)`

GetCardNumberOk returns a tuple with the CardNumber field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetCardNumber

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) SetCardNumber(v string)`

SetCardNumber sets CardNumber field to given value.

### HasCardNumber

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) HasCardNumber() bool`

HasCardNumber returns a boolean if a field has been set.

### SetCardNumberNil

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) SetCardNumberNil(b bool)`

 SetCardNumberNil sets the value for CardNumber to be an explicit nil

### UnsetCardNumber
`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) UnsetCardNumber()`

UnsetCardNumber ensures that no value is present for CardNumber, not even an explicit nil
### GetCardExpiryDate

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) GetCardExpiryDate() string`

GetCardExpiryDate returns the CardExpiryDate field if non-nil, zero value otherwise.

### GetCardExpiryDateOk

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) GetCardExpiryDateOk() (*string, bool)`

GetCardExpiryDateOk returns a tuple with the CardExpiryDate field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetCardExpiryDate

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) SetCardExpiryDate(v string)`

SetCardExpiryDate sets CardExpiryDate field to given value.

### HasCardExpiryDate

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) HasCardExpiryDate() bool`

HasCardExpiryDate returns a boolean if a field has been set.

### SetCardExpiryDateNil

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) SetCardExpiryDateNil(b bool)`

 SetCardExpiryDateNil sets the value for CardExpiryDate to be an explicit nil

### UnsetCardExpiryDate
`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) UnsetCardExpiryDate()`

UnsetCardExpiryDate ensures that no value is present for CardExpiryDate, not even an explicit nil
### GetTitleOther

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) GetTitleOther() string`

GetTitleOther returns the TitleOther field if non-nil, zero value otherwise.

### GetTitleOtherOk

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) GetTitleOtherOk() (*string, bool)`

GetTitleOtherOk returns a tuple with the TitleOther field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetTitleOther

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) SetTitleOther(v string)`

SetTitleOther sets TitleOther field to given value.

### HasTitleOther

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) HasTitleOther() bool`

HasTitleOther returns a boolean if a field has been set.

### SetTitleOtherNil

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) SetTitleOtherNil(b bool)`

 SetTitleOtherNil sets the value for TitleOther to be an explicit nil

### UnsetTitleOther
`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) UnsetTitleOther()`

UnsetTitleOther ensures that no value is present for TitleOther, not even an explicit nil
### GetEnFirstName

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) GetEnFirstName() string`

GetEnFirstName returns the EnFirstName field if non-nil, zero value otherwise.

### GetEnFirstNameOk

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) GetEnFirstNameOk() (*string, bool)`

GetEnFirstNameOk returns a tuple with the EnFirstName field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetEnFirstName

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) SetEnFirstName(v string)`

SetEnFirstName sets EnFirstName field to given value.

### HasEnFirstName

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) HasEnFirstName() bool`

HasEnFirstName returns a boolean if a field has been set.

### SetEnFirstNameNil

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) SetEnFirstNameNil(b bool)`

 SetEnFirstNameNil sets the value for EnFirstName to be an explicit nil

### UnsetEnFirstName
`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) UnsetEnFirstName()`

UnsetEnFirstName ensures that no value is present for EnFirstName, not even an explicit nil
### GetEnLastName

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) GetEnLastName() string`

GetEnLastName returns the EnLastName field if non-nil, zero value otherwise.

### GetEnLastNameOk

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) GetEnLastNameOk() (*string, bool)`

GetEnLastNameOk returns a tuple with the EnLastName field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetEnLastName

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) SetEnLastName(v string)`

SetEnLastName sets EnLastName field to given value.

### HasEnLastName

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) HasEnLastName() bool`

HasEnLastName returns a boolean if a field has been set.

### SetEnLastNameNil

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) SetEnLastNameNil(b bool)`

 SetEnLastNameNil sets the value for EnLastName to be an explicit nil

### UnsetEnLastName
`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) UnsetEnLastName()`

UnsetEnLastName ensures that no value is present for EnLastName, not even an explicit nil
### GetThFirstName

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) GetThFirstName() string`

GetThFirstName returns the ThFirstName field if non-nil, zero value otherwise.

### GetThFirstNameOk

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) GetThFirstNameOk() (*string, bool)`

GetThFirstNameOk returns a tuple with the ThFirstName field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetThFirstName

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) SetThFirstName(v string)`

SetThFirstName sets ThFirstName field to given value.

### HasThFirstName

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) HasThFirstName() bool`

HasThFirstName returns a boolean if a field has been set.

### SetThFirstNameNil

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) SetThFirstNameNil(b bool)`

 SetThFirstNameNil sets the value for ThFirstName to be an explicit nil

### UnsetThFirstName
`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) UnsetThFirstName()`

UnsetThFirstName ensures that no value is present for ThFirstName, not even an explicit nil
### GetThLastName

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) GetThLastName() string`

GetThLastName returns the ThLastName field if non-nil, zero value otherwise.

### GetThLastNameOk

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) GetThLastNameOk() (*string, bool)`

GetThLastNameOk returns a tuple with the ThLastName field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetThLastName

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) SetThLastName(v string)`

SetThLastName sets ThLastName field to given value.

### HasThLastName

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) HasThLastName() bool`

HasThLastName returns a boolean if a field has been set.

### SetThLastNameNil

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) SetThLastNameNil(b bool)`

 SetThLastNameNil sets the value for ThLastName to be an explicit nil

### UnsetThLastName
`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) UnsetThLastName()`

UnsetThLastName ensures that no value is present for ThLastName, not even an explicit nil
### GetBirthDate

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) GetBirthDate() string`

GetBirthDate returns the BirthDate field if non-nil, zero value otherwise.

### GetBirthDateOk

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) GetBirthDateOk() (*string, bool)`

GetBirthDateOk returns a tuple with the BirthDate field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetBirthDate

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) SetBirthDate(v string)`

SetBirthDate sets BirthDate field to given value.

### HasBirthDate

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) HasBirthDate() bool`

HasBirthDate returns a boolean if a field has been set.

### SetBirthDateNil

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) SetBirthDateNil(b bool)`

 SetBirthDateNil sets the value for BirthDate to be an explicit nil

### UnsetBirthDate
`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) UnsetBirthDate()`

UnsetBirthDate ensures that no value is present for BirthDate, not even an explicit nil
### GetNationality

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) GetNationality() string`

GetNationality returns the Nationality field if non-nil, zero value otherwise.

### GetNationalityOk

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) GetNationalityOk() (*string, bool)`

GetNationalityOk returns a tuple with the Nationality field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetNationality

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) SetNationality(v string)`

SetNationality sets Nationality field to given value.

### HasNationality

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) HasNationality() bool`

HasNationality returns a boolean if a field has been set.

### SetNationalityNil

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) SetNationalityNil(b bool)`

 SetNationalityNil sets the value for Nationality to be an explicit nil

### UnsetNationality
`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) UnsetNationality()`

UnsetNationality ensures that no value is present for Nationality, not even an explicit nil
### GetMobileNumber

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) GetMobileNumber() string`

GetMobileNumber returns the MobileNumber field if non-nil, zero value otherwise.

### GetMobileNumberOk

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) GetMobileNumberOk() (*string, bool)`

GetMobileNumberOk returns a tuple with the MobileNumber field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetMobileNumber

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) SetMobileNumber(v string)`

SetMobileNumber sets MobileNumber field to given value.

### HasMobileNumber

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) HasMobileNumber() bool`

HasMobileNumber returns a boolean if a field has been set.

### SetMobileNumberNil

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) SetMobileNumberNil(b bool)`

 SetMobileNumberNil sets the value for MobileNumber to be an explicit nil

### UnsetMobileNumber
`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) UnsetMobileNumber()`

UnsetMobileNumber ensures that no value is present for MobileNumber, not even an explicit nil
### GetEmail

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) GetEmail() string`

GetEmail returns the Email field if non-nil, zero value otherwise.

### GetEmailOk

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) GetEmailOk() (*string, bool)`

GetEmailOk returns a tuple with the Email field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetEmail

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) SetEmail(v string)`

SetEmail sets Email field to given value.

### HasEmail

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) HasEmail() bool`

HasEmail returns a boolean if a field has been set.

### SetEmailNil

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) SetEmailNil(b bool)`

 SetEmailNil sets the value for Email to be an explicit nil

### UnsetEmail
`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) UnsetEmail()`

UnsetEmail ensures that no value is present for Email, not even an explicit nil
### GetPhone

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) GetPhone() string`

GetPhone returns the Phone field if non-nil, zero value otherwise.

### GetPhoneOk

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) GetPhoneOk() (*string, bool)`

GetPhoneOk returns a tuple with the Phone field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetPhone

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) SetPhone(v string)`

SetPhone sets Phone field to given value.

### HasPhone

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) HasPhone() bool`

HasPhone returns a boolean if a field has been set.

### SetPhoneNil

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) SetPhoneNil(b bool)`

 SetPhoneNil sets the value for Phone to be an explicit nil

### UnsetPhone
`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) UnsetPhone()`

UnsetPhone ensures that no value is present for Phone, not even an explicit nil
### GetFax

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) GetFax() string`

GetFax returns the Fax field if non-nil, zero value otherwise.

### GetFaxOk

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) GetFaxOk() (*string, bool)`

GetFaxOk returns a tuple with the Fax field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetFax

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) SetFax(v string)`

SetFax sets Fax field to given value.

### HasFax

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) HasFax() bool`

HasFax returns a boolean if a field has been set.

### SetFaxNil

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) SetFaxNil(b bool)`

 SetFaxNil sets the value for Fax to be an explicit nil

### UnsetFax
`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) UnsetFax()`

UnsetFax ensures that no value is present for Fax, not even an explicit nil
### GetSpouse

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) GetSpouse() PiFinancialClientFundConnextModelSpouse`

GetSpouse returns the Spouse field if non-nil, zero value otherwise.

### GetSpouseOk

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) GetSpouseOk() (*PiFinancialClientFundConnextModelSpouse, bool)`

GetSpouseOk returns a tuple with the Spouse field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetSpouse

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) SetSpouse(v PiFinancialClientFundConnextModelSpouse)`

SetSpouse sets Spouse field to given value.

### HasSpouse

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) HasSpouse() bool`

HasSpouse returns a boolean if a field has been set.

### GetOccupationOther

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) GetOccupationOther() string`

GetOccupationOther returns the OccupationOther field if non-nil, zero value otherwise.

### GetOccupationOtherOk

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) GetOccupationOtherOk() (*string, bool)`

GetOccupationOtherOk returns a tuple with the OccupationOther field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetOccupationOther

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) SetOccupationOther(v string)`

SetOccupationOther sets OccupationOther field to given value.

### HasOccupationOther

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) HasOccupationOther() bool`

HasOccupationOther returns a boolean if a field has been set.

### SetOccupationOtherNil

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) SetOccupationOtherNil(b bool)`

 SetOccupationOtherNil sets the value for OccupationOther to be an explicit nil

### UnsetOccupationOther
`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) UnsetOccupationOther()`

UnsetOccupationOther ensures that no value is present for OccupationOther, not even an explicit nil
### GetBusinessTypeOther

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) GetBusinessTypeOther() string`

GetBusinessTypeOther returns the BusinessTypeOther field if non-nil, zero value otherwise.

### GetBusinessTypeOtherOk

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) GetBusinessTypeOtherOk() (*string, bool)`

GetBusinessTypeOtherOk returns a tuple with the BusinessTypeOther field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetBusinessTypeOther

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) SetBusinessTypeOther(v string)`

SetBusinessTypeOther sets BusinessTypeOther field to given value.

### HasBusinessTypeOther

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) HasBusinessTypeOther() bool`

HasBusinessTypeOther returns a boolean if a field has been set.

### SetBusinessTypeOtherNil

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) SetBusinessTypeOtherNil(b bool)`

 SetBusinessTypeOtherNil sets the value for BusinessTypeOther to be an explicit nil

### UnsetBusinessTypeOther
`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) UnsetBusinessTypeOther()`

UnsetBusinessTypeOther ensures that no value is present for BusinessTypeOther, not even an explicit nil
### GetAssetValue

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) GetAssetValue() float64`

GetAssetValue returns the AssetValue field if non-nil, zero value otherwise.

### GetAssetValueOk

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) GetAssetValueOk() (*float64, bool)`

GetAssetValueOk returns a tuple with the AssetValue field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetAssetValue

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) SetAssetValue(v float64)`

SetAssetValue sets AssetValue field to given value.

### HasAssetValue

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) HasAssetValue() bool`

HasAssetValue returns a boolean if a field has been set.

### SetAssetValueNil

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) SetAssetValueNil(b bool)`

 SetAssetValueNil sets the value for AssetValue to be an explicit nil

### UnsetAssetValue
`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) UnsetAssetValue()`

UnsetAssetValue ensures that no value is present for AssetValue, not even an explicit nil
### GetIncomeSource

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) GetIncomeSource() string`

GetIncomeSource returns the IncomeSource field if non-nil, zero value otherwise.

### GetIncomeSourceOk

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) GetIncomeSourceOk() (*string, bool)`

GetIncomeSourceOk returns a tuple with the IncomeSource field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetIncomeSource

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) SetIncomeSource(v string)`

SetIncomeSource sets IncomeSource field to given value.

### HasIncomeSource

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) HasIncomeSource() bool`

HasIncomeSource returns a boolean if a field has been set.

### SetIncomeSourceNil

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) SetIncomeSourceNil(b bool)`

 SetIncomeSourceNil sets the value for IncomeSource to be an explicit nil

### UnsetIncomeSource
`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) UnsetIncomeSource()`

UnsetIncomeSource ensures that no value is present for IncomeSource, not even an explicit nil
### GetIncomeSourceOther

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) GetIncomeSourceOther() string`

GetIncomeSourceOther returns the IncomeSourceOther field if non-nil, zero value otherwise.

### GetIncomeSourceOtherOk

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) GetIncomeSourceOtherOk() (*string, bool)`

GetIncomeSourceOtherOk returns a tuple with the IncomeSourceOther field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetIncomeSourceOther

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) SetIncomeSourceOther(v string)`

SetIncomeSourceOther sets IncomeSourceOther field to given value.

### HasIncomeSourceOther

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) HasIncomeSourceOther() bool`

HasIncomeSourceOther returns a boolean if a field has been set.

### SetIncomeSourceOtherNil

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) SetIncomeSourceOtherNil(b bool)`

 SetIncomeSourceOtherNil sets the value for IncomeSourceOther to be an explicit nil

### UnsetIncomeSourceOther
`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) UnsetIncomeSourceOther()`

UnsetIncomeSourceOther ensures that no value is present for IncomeSourceOther, not even an explicit nil
### GetIdentificationDocument

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) GetIdentificationDocument() PiFinancialClientFundConnextModelAddressForProfileV5`

GetIdentificationDocument returns the IdentificationDocument field if non-nil, zero value otherwise.

### GetIdentificationDocumentOk

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) GetIdentificationDocumentOk() (*PiFinancialClientFundConnextModelAddressForProfileV5, bool)`

GetIdentificationDocumentOk returns a tuple with the IdentificationDocument field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetIdentificationDocument

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) SetIdentificationDocument(v PiFinancialClientFundConnextModelAddressForProfileV5)`

SetIdentificationDocument sets IdentificationDocument field to given value.

### HasIdentificationDocument

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) HasIdentificationDocument() bool`

HasIdentificationDocument returns a boolean if a field has been set.

### GetCurrent

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) GetCurrent() PiFinancialClientFundConnextModelAddressForProfileV5`

GetCurrent returns the Current field if non-nil, zero value otherwise.

### GetCurrentOk

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) GetCurrentOk() (*PiFinancialClientFundConnextModelAddressForProfileV5, bool)`

GetCurrentOk returns a tuple with the Current field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetCurrent

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) SetCurrent(v PiFinancialClientFundConnextModelAddressForProfileV5)`

SetCurrent sets Current field to given value.

### HasCurrent

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) HasCurrent() bool`

HasCurrent returns a boolean if a field has been set.

### GetCompanyName

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) GetCompanyName() string`

GetCompanyName returns the CompanyName field if non-nil, zero value otherwise.

### GetCompanyNameOk

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) GetCompanyNameOk() (*string, bool)`

GetCompanyNameOk returns a tuple with the CompanyName field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetCompanyName

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) SetCompanyName(v string)`

SetCompanyName sets CompanyName field to given value.

### HasCompanyName

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) HasCompanyName() bool`

HasCompanyName returns a boolean if a field has been set.

### SetCompanyNameNil

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) SetCompanyNameNil(b bool)`

 SetCompanyNameNil sets the value for CompanyName to be an explicit nil

### UnsetCompanyName
`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) UnsetCompanyName()`

UnsetCompanyName ensures that no value is present for CompanyName, not even an explicit nil
### GetWork

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) GetWork() PiFinancialClientFundConnextModelAddressForProfileV5`

GetWork returns the Work field if non-nil, zero value otherwise.

### GetWorkOk

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) GetWorkOk() (*PiFinancialClientFundConnextModelAddressForProfileV5, bool)`

GetWorkOk returns a tuple with the Work field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetWork

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) SetWork(v PiFinancialClientFundConnextModelAddressForProfileV5)`

SetWork sets Work field to given value.

### HasWork

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) HasWork() bool`

HasWork returns a boolean if a field has been set.

### GetWorkPosition

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) GetWorkPosition() string`

GetWorkPosition returns the WorkPosition field if non-nil, zero value otherwise.

### GetWorkPositionOk

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) GetWorkPositionOk() (*string, bool)`

GetWorkPositionOk returns a tuple with the WorkPosition field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetWorkPosition

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) SetWorkPosition(v string)`

SetWorkPosition sets WorkPosition field to given value.

### HasWorkPosition

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) HasWorkPosition() bool`

HasWorkPosition returns a boolean if a field has been set.

### SetWorkPositionNil

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) SetWorkPositionNil(b bool)`

 SetWorkPositionNil sets the value for WorkPosition to be an explicit nil

### UnsetWorkPosition
`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) UnsetWorkPosition()`

UnsetWorkPosition ensures that no value is present for WorkPosition, not even an explicit nil
### GetRelatedPoliticalPerson

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) GetRelatedPoliticalPerson() bool`

GetRelatedPoliticalPerson returns the RelatedPoliticalPerson field if non-nil, zero value otherwise.

### GetRelatedPoliticalPersonOk

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) GetRelatedPoliticalPersonOk() (*bool, bool)`

GetRelatedPoliticalPersonOk returns a tuple with the RelatedPoliticalPerson field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetRelatedPoliticalPerson

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) SetRelatedPoliticalPerson(v bool)`

SetRelatedPoliticalPerson sets RelatedPoliticalPerson field to given value.

### HasRelatedPoliticalPerson

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) HasRelatedPoliticalPerson() bool`

HasRelatedPoliticalPerson returns a boolean if a field has been set.

### SetRelatedPoliticalPersonNil

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) SetRelatedPoliticalPersonNil(b bool)`

 SetRelatedPoliticalPersonNil sets the value for RelatedPoliticalPerson to be an explicit nil

### UnsetRelatedPoliticalPerson
`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) UnsetRelatedPoliticalPerson()`

UnsetRelatedPoliticalPerson ensures that no value is present for RelatedPoliticalPerson, not even an explicit nil
### GetPoliticalRelatedPersonPosition

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) GetPoliticalRelatedPersonPosition() string`

GetPoliticalRelatedPersonPosition returns the PoliticalRelatedPersonPosition field if non-nil, zero value otherwise.

### GetPoliticalRelatedPersonPositionOk

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) GetPoliticalRelatedPersonPositionOk() (*string, bool)`

GetPoliticalRelatedPersonPositionOk returns a tuple with the PoliticalRelatedPersonPosition field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetPoliticalRelatedPersonPosition

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) SetPoliticalRelatedPersonPosition(v string)`

SetPoliticalRelatedPersonPosition sets PoliticalRelatedPersonPosition field to given value.

### HasPoliticalRelatedPersonPosition

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) HasPoliticalRelatedPersonPosition() bool`

HasPoliticalRelatedPersonPosition returns a boolean if a field has been set.

### SetPoliticalRelatedPersonPositionNil

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) SetPoliticalRelatedPersonPositionNil(b bool)`

 SetPoliticalRelatedPersonPositionNil sets the value for PoliticalRelatedPersonPosition to be an explicit nil

### UnsetPoliticalRelatedPersonPosition
`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) UnsetPoliticalRelatedPersonPosition()`

UnsetPoliticalRelatedPersonPosition ensures that no value is present for PoliticalRelatedPersonPosition, not even an explicit nil
### GetCanAcceptFxRisk

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) GetCanAcceptFxRisk() bool`

GetCanAcceptFxRisk returns the CanAcceptFxRisk field if non-nil, zero value otherwise.

### GetCanAcceptFxRiskOk

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) GetCanAcceptFxRiskOk() (*bool, bool)`

GetCanAcceptFxRiskOk returns a tuple with the CanAcceptFxRisk field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetCanAcceptFxRisk

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) SetCanAcceptFxRisk(v bool)`

SetCanAcceptFxRisk sets CanAcceptFxRisk field to given value.

### HasCanAcceptFxRisk

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) HasCanAcceptFxRisk() bool`

HasCanAcceptFxRisk returns a boolean if a field has been set.

### GetCanAcceptDerivativeInvestment

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) GetCanAcceptDerivativeInvestment() bool`

GetCanAcceptDerivativeInvestment returns the CanAcceptDerivativeInvestment field if non-nil, zero value otherwise.

### GetCanAcceptDerivativeInvestmentOk

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) GetCanAcceptDerivativeInvestmentOk() (*bool, bool)`

GetCanAcceptDerivativeInvestmentOk returns a tuple with the CanAcceptDerivativeInvestment field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetCanAcceptDerivativeInvestment

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) SetCanAcceptDerivativeInvestment(v bool)`

SetCanAcceptDerivativeInvestment sets CanAcceptDerivativeInvestment field to given value.

### HasCanAcceptDerivativeInvestment

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) HasCanAcceptDerivativeInvestment() bool`

HasCanAcceptDerivativeInvestment returns a boolean if a field has been set.

### GetSuitabilityEvaluationDate

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) GetSuitabilityEvaluationDate() string`

GetSuitabilityEvaluationDate returns the SuitabilityEvaluationDate field if non-nil, zero value otherwise.

### GetSuitabilityEvaluationDateOk

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) GetSuitabilityEvaluationDateOk() (*string, bool)`

GetSuitabilityEvaluationDateOk returns a tuple with the SuitabilityEvaluationDate field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetSuitabilityEvaluationDate

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) SetSuitabilityEvaluationDate(v string)`

SetSuitabilityEvaluationDate sets SuitabilityEvaluationDate field to given value.

### HasSuitabilityEvaluationDate

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) HasSuitabilityEvaluationDate() bool`

HasSuitabilityEvaluationDate returns a boolean if a field has been set.

### SetSuitabilityEvaluationDateNil

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) SetSuitabilityEvaluationDateNil(b bool)`

 SetSuitabilityEvaluationDateNil sets the value for SuitabilityEvaluationDate to be an explicit nil

### UnsetSuitabilityEvaluationDate
`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) UnsetSuitabilityEvaluationDate()`

UnsetSuitabilityEvaluationDate ensures that no value is present for SuitabilityEvaluationDate, not even an explicit nil
### GetFatca

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) GetFatca() bool`

GetFatca returns the Fatca field if non-nil, zero value otherwise.

### GetFatcaOk

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) GetFatcaOk() (*bool, bool)`

GetFatcaOk returns a tuple with the Fatca field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetFatca

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) SetFatca(v bool)`

SetFatca sets Fatca field to given value.

### HasFatca

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) HasFatca() bool`

HasFatca returns a boolean if a field has been set.

### GetFatcaDeclarationDate

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) GetFatcaDeclarationDate() string`

GetFatcaDeclarationDate returns the FatcaDeclarationDate field if non-nil, zero value otherwise.

### GetFatcaDeclarationDateOk

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) GetFatcaDeclarationDateOk() (*string, bool)`

GetFatcaDeclarationDateOk returns a tuple with the FatcaDeclarationDate field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetFatcaDeclarationDate

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) SetFatcaDeclarationDate(v string)`

SetFatcaDeclarationDate sets FatcaDeclarationDate field to given value.

### HasFatcaDeclarationDate

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) HasFatcaDeclarationDate() bool`

HasFatcaDeclarationDate returns a boolean if a field has been set.

### SetFatcaDeclarationDateNil

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) SetFatcaDeclarationDateNil(b bool)`

 SetFatcaDeclarationDateNil sets the value for FatcaDeclarationDate to be an explicit nil

### UnsetFatcaDeclarationDate
`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) UnsetFatcaDeclarationDate()`

UnsetFatcaDeclarationDate ensures that no value is present for FatcaDeclarationDate, not even an explicit nil
### GetCrsPlaceOfBirthCountry

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) GetCrsPlaceOfBirthCountry() string`

GetCrsPlaceOfBirthCountry returns the CrsPlaceOfBirthCountry field if non-nil, zero value otherwise.

### GetCrsPlaceOfBirthCountryOk

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) GetCrsPlaceOfBirthCountryOk() (*string, bool)`

GetCrsPlaceOfBirthCountryOk returns a tuple with the CrsPlaceOfBirthCountry field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetCrsPlaceOfBirthCountry

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) SetCrsPlaceOfBirthCountry(v string)`

SetCrsPlaceOfBirthCountry sets CrsPlaceOfBirthCountry field to given value.

### HasCrsPlaceOfBirthCountry

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) HasCrsPlaceOfBirthCountry() bool`

HasCrsPlaceOfBirthCountry returns a boolean if a field has been set.

### SetCrsPlaceOfBirthCountryNil

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) SetCrsPlaceOfBirthCountryNil(b bool)`

 SetCrsPlaceOfBirthCountryNil sets the value for CrsPlaceOfBirthCountry to be an explicit nil

### UnsetCrsPlaceOfBirthCountry
`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) UnsetCrsPlaceOfBirthCountry()`

UnsetCrsPlaceOfBirthCountry ensures that no value is present for CrsPlaceOfBirthCountry, not even an explicit nil
### GetCrsPlaceOfBirthCity

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) GetCrsPlaceOfBirthCity() string`

GetCrsPlaceOfBirthCity returns the CrsPlaceOfBirthCity field if non-nil, zero value otherwise.

### GetCrsPlaceOfBirthCityOk

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) GetCrsPlaceOfBirthCityOk() (*string, bool)`

GetCrsPlaceOfBirthCityOk returns a tuple with the CrsPlaceOfBirthCity field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetCrsPlaceOfBirthCity

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) SetCrsPlaceOfBirthCity(v string)`

SetCrsPlaceOfBirthCity sets CrsPlaceOfBirthCity field to given value.

### HasCrsPlaceOfBirthCity

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) HasCrsPlaceOfBirthCity() bool`

HasCrsPlaceOfBirthCity returns a boolean if a field has been set.

### SetCrsPlaceOfBirthCityNil

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) SetCrsPlaceOfBirthCityNil(b bool)`

 SetCrsPlaceOfBirthCityNil sets the value for CrsPlaceOfBirthCity to be an explicit nil

### UnsetCrsPlaceOfBirthCity
`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) UnsetCrsPlaceOfBirthCity()`

UnsetCrsPlaceOfBirthCity ensures that no value is present for CrsPlaceOfBirthCity, not even an explicit nil
### GetCrsTaxResidenceInCountriesOtherThanTheUS

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) GetCrsTaxResidenceInCountriesOtherThanTheUS() bool`

GetCrsTaxResidenceInCountriesOtherThanTheUS returns the CrsTaxResidenceInCountriesOtherThanTheUS field if non-nil, zero value otherwise.

### GetCrsTaxResidenceInCountriesOtherThanTheUSOk

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) GetCrsTaxResidenceInCountriesOtherThanTheUSOk() (*bool, bool)`

GetCrsTaxResidenceInCountriesOtherThanTheUSOk returns a tuple with the CrsTaxResidenceInCountriesOtherThanTheUS field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetCrsTaxResidenceInCountriesOtherThanTheUS

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) SetCrsTaxResidenceInCountriesOtherThanTheUS(v bool)`

SetCrsTaxResidenceInCountriesOtherThanTheUS sets CrsTaxResidenceInCountriesOtherThanTheUS field to given value.

### HasCrsTaxResidenceInCountriesOtherThanTheUS

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) HasCrsTaxResidenceInCountriesOtherThanTheUS() bool`

HasCrsTaxResidenceInCountriesOtherThanTheUS returns a boolean if a field has been set.

### SetCrsTaxResidenceInCountriesOtherThanTheUSNil

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) SetCrsTaxResidenceInCountriesOtherThanTheUSNil(b bool)`

 SetCrsTaxResidenceInCountriesOtherThanTheUSNil sets the value for CrsTaxResidenceInCountriesOtherThanTheUS to be an explicit nil

### UnsetCrsTaxResidenceInCountriesOtherThanTheUS
`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) UnsetCrsTaxResidenceInCountriesOtherThanTheUS()`

UnsetCrsTaxResidenceInCountriesOtherThanTheUS ensures that no value is present for CrsTaxResidenceInCountriesOtherThanTheUS, not even an explicit nil
### GetCrsDetails

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) GetCrsDetails() []PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5CrsDetailsInner`

GetCrsDetails returns the CrsDetails field if non-nil, zero value otherwise.

### GetCrsDetailsOk

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) GetCrsDetailsOk() (*[]PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5CrsDetailsInner, bool)`

GetCrsDetailsOk returns a tuple with the CrsDetails field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetCrsDetails

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) SetCrsDetails(v []PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5CrsDetailsInner)`

SetCrsDetails sets CrsDetails field to given value.

### HasCrsDetails

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) HasCrsDetails() bool`

HasCrsDetails returns a boolean if a field has been set.

### SetCrsDetailsNil

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) SetCrsDetailsNil(b bool)`

 SetCrsDetailsNil sets the value for CrsDetails to be an explicit nil

### UnsetCrsDetails
`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) UnsetCrsDetails()`

UnsetCrsDetails ensures that no value is present for CrsDetails, not even an explicit nil
### GetCrsDeclarationDate

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) GetCrsDeclarationDate() string`

GetCrsDeclarationDate returns the CrsDeclarationDate field if non-nil, zero value otherwise.

### GetCrsDeclarationDateOk

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) GetCrsDeclarationDateOk() (*string, bool)`

GetCrsDeclarationDateOk returns a tuple with the CrsDeclarationDate field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetCrsDeclarationDate

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) SetCrsDeclarationDate(v string)`

SetCrsDeclarationDate sets CrsDeclarationDate field to given value.

### HasCrsDeclarationDate

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) HasCrsDeclarationDate() bool`

HasCrsDeclarationDate returns a boolean if a field has been set.

### SetCrsDeclarationDateNil

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) SetCrsDeclarationDateNil(b bool)`

 SetCrsDeclarationDateNil sets the value for CrsDeclarationDate to be an explicit nil

### UnsetCrsDeclarationDate
`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) UnsetCrsDeclarationDate()`

UnsetCrsDeclarationDate ensures that no value is present for CrsDeclarationDate, not even an explicit nil
### GetCddDate

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) GetCddDate() string`

GetCddDate returns the CddDate field if non-nil, zero value otherwise.

### GetCddDateOk

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) GetCddDateOk() (*string, bool)`

GetCddDateOk returns a tuple with the CddDate field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetCddDate

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) SetCddDate(v string)`

SetCddDate sets CddDate field to given value.

### HasCddDate

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) HasCddDate() bool`

HasCddDate returns a boolean if a field has been set.

### SetCddDateNil

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) SetCddDateNil(b bool)`

 SetCddDateNil sets the value for CddDate to be an explicit nil

### UnsetCddDate
`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) UnsetCddDate()`

UnsetCddDate ensures that no value is present for CddDate, not even an explicit nil
### GetReferralPerson

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) GetReferralPerson() string`

GetReferralPerson returns the ReferralPerson field if non-nil, zero value otherwise.

### GetReferralPersonOk

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) GetReferralPersonOk() (*string, bool)`

GetReferralPersonOk returns a tuple with the ReferralPerson field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetReferralPerson

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) SetReferralPerson(v string)`

SetReferralPerson sets ReferralPerson field to given value.

### HasReferralPerson

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) HasReferralPerson() bool`

HasReferralPerson returns a boolean if a field has been set.

### SetReferralPersonNil

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) SetReferralPersonNil(b bool)`

 SetReferralPersonNil sets the value for ReferralPerson to be an explicit nil

### UnsetReferralPerson
`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) UnsetReferralPerson()`

UnsetReferralPerson ensures that no value is present for ReferralPerson, not even an explicit nil
### GetApplicationDate

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) GetApplicationDate() string`

GetApplicationDate returns the ApplicationDate field if non-nil, zero value otherwise.

### GetApplicationDateOk

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) GetApplicationDateOk() (*string, bool)`

GetApplicationDateOk returns a tuple with the ApplicationDate field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetApplicationDate

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) SetApplicationDate(v string)`

SetApplicationDate sets ApplicationDate field to given value.

### HasApplicationDate

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) HasApplicationDate() bool`

HasApplicationDate returns a boolean if a field has been set.

### SetApplicationDateNil

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) SetApplicationDateNil(b bool)`

 SetApplicationDateNil sets the value for ApplicationDate to be an explicit nil

### UnsetApplicationDate
`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) UnsetApplicationDate()`

UnsetApplicationDate ensures that no value is present for ApplicationDate, not even an explicit nil
### GetIncomeSourceCountry

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) GetIncomeSourceCountry() string`

GetIncomeSourceCountry returns the IncomeSourceCountry field if non-nil, zero value otherwise.

### GetIncomeSourceCountryOk

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) GetIncomeSourceCountryOk() (*string, bool)`

GetIncomeSourceCountryOk returns a tuple with the IncomeSourceCountry field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetIncomeSourceCountry

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) SetIncomeSourceCountry(v string)`

SetIncomeSourceCountry sets IncomeSourceCountry field to given value.

### HasIncomeSourceCountry

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) HasIncomeSourceCountry() bool`

HasIncomeSourceCountry returns a boolean if a field has been set.

### SetIncomeSourceCountryNil

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) SetIncomeSourceCountryNil(b bool)`

 SetIncomeSourceCountryNil sets the value for IncomeSourceCountry to be an explicit nil

### UnsetIncomeSourceCountry
`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) UnsetIncomeSourceCountry()`

UnsetIncomeSourceCountry ensures that no value is present for IncomeSourceCountry, not even an explicit nil
### GetAcceptedBy

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) GetAcceptedBy() string`

GetAcceptedBy returns the AcceptedBy field if non-nil, zero value otherwise.

### GetAcceptedByOk

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) GetAcceptedByOk() (*string, bool)`

GetAcceptedByOk returns a tuple with the AcceptedBy field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetAcceptedBy

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) SetAcceptedBy(v string)`

SetAcceptedBy sets AcceptedBy field to given value.

### HasAcceptedBy

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) HasAcceptedBy() bool`

HasAcceptedBy returns a boolean if a field has been set.

### SetAcceptedByNil

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) SetAcceptedByNil(b bool)`

 SetAcceptedByNil sets the value for AcceptedBy to be an explicit nil

### UnsetAcceptedBy
`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) UnsetAcceptedBy()`

UnsetAcceptedBy ensures that no value is present for AcceptedBy, not even an explicit nil
### GetApproved

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) GetApproved() bool`

GetApproved returns the Approved field if non-nil, zero value otherwise.

### GetApprovedOk

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) GetApprovedOk() (*bool, bool)`

GetApprovedOk returns a tuple with the Approved field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetApproved

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) SetApproved(v bool)`

SetApproved sets Approved field to given value.

### HasApproved

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) HasApproved() bool`

HasApproved returns a boolean if a field has been set.

### GetVulnerableFlag

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) GetVulnerableFlag() bool`

GetVulnerableFlag returns the VulnerableFlag field if non-nil, zero value otherwise.

### GetVulnerableFlagOk

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) GetVulnerableFlagOk() (*bool, bool)`

GetVulnerableFlagOk returns a tuple with the VulnerableFlag field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetVulnerableFlag

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) SetVulnerableFlag(v bool)`

SetVulnerableFlag sets VulnerableFlag field to given value.

### HasVulnerableFlag

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) HasVulnerableFlag() bool`

HasVulnerableFlag returns a boolean if a field has been set.

### SetVulnerableFlagNil

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) SetVulnerableFlagNil(b bool)`

 SetVulnerableFlagNil sets the value for VulnerableFlag to be an explicit nil

### UnsetVulnerableFlag
`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) UnsetVulnerableFlag()`

UnsetVulnerableFlag ensures that no value is present for VulnerableFlag, not even an explicit nil
### GetVulnerableDetail

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) GetVulnerableDetail() string`

GetVulnerableDetail returns the VulnerableDetail field if non-nil, zero value otherwise.

### GetVulnerableDetailOk

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) GetVulnerableDetailOk() (*string, bool)`

GetVulnerableDetailOk returns a tuple with the VulnerableDetail field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetVulnerableDetail

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) SetVulnerableDetail(v string)`

SetVulnerableDetail sets VulnerableDetail field to given value.

### HasVulnerableDetail

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) HasVulnerableDetail() bool`

HasVulnerableDetail returns a boolean if a field has been set.

### SetVulnerableDetailNil

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) SetVulnerableDetailNil(b bool)`

 SetVulnerableDetailNil sets the value for VulnerableDetail to be an explicit nil

### UnsetVulnerableDetail
`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) UnsetVulnerableDetail()`

UnsetVulnerableDetail ensures that no value is present for VulnerableDetail, not even an explicit nil
### GetNdidFlag

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) GetNdidFlag() bool`

GetNdidFlag returns the NdidFlag field if non-nil, zero value otherwise.

### GetNdidFlagOk

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) GetNdidFlagOk() (*bool, bool)`

GetNdidFlagOk returns a tuple with the NdidFlag field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetNdidFlag

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) SetNdidFlag(v bool)`

SetNdidFlag sets NdidFlag field to given value.

### HasNdidFlag

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) HasNdidFlag() bool`

HasNdidFlag returns a boolean if a field has been set.

### SetNdidFlagNil

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) SetNdidFlagNil(b bool)`

 SetNdidFlagNil sets the value for NdidFlag to be an explicit nil

### UnsetNdidFlag
`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) UnsetNdidFlag()`

UnsetNdidFlag ensures that no value is present for NdidFlag, not even an explicit nil
### GetNdidRequestId

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) GetNdidRequestId() string`

GetNdidRequestId returns the NdidRequestId field if non-nil, zero value otherwise.

### GetNdidRequestIdOk

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) GetNdidRequestIdOk() (*string, bool)`

GetNdidRequestIdOk returns a tuple with the NdidRequestId field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetNdidRequestId

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) SetNdidRequestId(v string)`

SetNdidRequestId sets NdidRequestId field to given value.

### HasNdidRequestId

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) HasNdidRequestId() bool`

HasNdidRequestId returns a boolean if a field has been set.

### SetNdidRequestIdNil

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) SetNdidRequestIdNil(b bool)`

 SetNdidRequestIdNil sets the value for NdidRequestId to be an explicit nil

### UnsetNdidRequestId
`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) UnsetNdidRequestId()`

UnsetNdidRequestId ensures that no value is present for NdidRequestId, not even an explicit nil
### GetSuitabilityForm

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) GetSuitabilityForm() PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5SuitabilityForm`

GetSuitabilityForm returns the SuitabilityForm field if non-nil, zero value otherwise.

### GetSuitabilityFormOk

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) GetSuitabilityFormOk() (*PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5SuitabilityForm, bool)`

GetSuitabilityFormOk returns a tuple with the SuitabilityForm field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetSuitabilityForm

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) SetSuitabilityForm(v PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5SuitabilityForm)`

SetSuitabilityForm sets SuitabilityForm field to given value.

### HasSuitabilityForm

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) HasSuitabilityForm() bool`

HasSuitabilityForm returns a boolean if a field has been set.

### GetKnowledgeAssessmentResult

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) GetKnowledgeAssessmentResult() bool`

GetKnowledgeAssessmentResult returns the KnowledgeAssessmentResult field if non-nil, zero value otherwise.

### GetKnowledgeAssessmentResultOk

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) GetKnowledgeAssessmentResultOk() (*bool, bool)`

GetKnowledgeAssessmentResultOk returns a tuple with the KnowledgeAssessmentResult field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetKnowledgeAssessmentResult

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) SetKnowledgeAssessmentResult(v bool)`

SetKnowledgeAssessmentResult sets KnowledgeAssessmentResult field to given value.

### HasKnowledgeAssessmentResult

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) HasKnowledgeAssessmentResult() bool`

HasKnowledgeAssessmentResult returns a boolean if a field has been set.

### SetKnowledgeAssessmentResultNil

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) SetKnowledgeAssessmentResultNil(b bool)`

 SetKnowledgeAssessmentResultNil sets the value for KnowledgeAssessmentResult to be an explicit nil

### UnsetKnowledgeAssessmentResult
`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) UnsetKnowledgeAssessmentResult()`

UnsetKnowledgeAssessmentResult ensures that no value is present for KnowledgeAssessmentResult, not even an explicit nil
### GetKnowledgeAssessmentForm

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) GetKnowledgeAssessmentForm() PiFinancialClientFundConnextModelIndividualInvestorV5ResponseKnowledgeAssessmentForm`

GetKnowledgeAssessmentForm returns the KnowledgeAssessmentForm field if non-nil, zero value otherwise.

### GetKnowledgeAssessmentFormOk

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) GetKnowledgeAssessmentFormOk() (*PiFinancialClientFundConnextModelIndividualInvestorV5ResponseKnowledgeAssessmentForm, bool)`

GetKnowledgeAssessmentFormOk returns a tuple with the KnowledgeAssessmentForm field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetKnowledgeAssessmentForm

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) SetKnowledgeAssessmentForm(v PiFinancialClientFundConnextModelIndividualInvestorV5ResponseKnowledgeAssessmentForm)`

SetKnowledgeAssessmentForm sets KnowledgeAssessmentForm field to given value.

### HasKnowledgeAssessmentForm

`func (o *PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5) HasKnowledgeAssessmentForm() bool`

HasKnowledgeAssessmentForm returns a boolean if a field has been set.


[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


