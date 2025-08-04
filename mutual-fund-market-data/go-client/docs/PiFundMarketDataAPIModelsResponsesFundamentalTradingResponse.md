# PiFundMarketDataAPIModelsResponsesFundamentalTradingResponse

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**TaxType** | Pointer to **NullableString** |  | [optional] 
**FundRiskLevel** | Pointer to **NullableInt32** |  | [optional] 
**FundType** | Pointer to [**PiFundMarketDataConstantsFundType**](PiFundMarketDataConstantsFundType.md) |  | [optional] 
**ProjectType** | Pointer to [**PiFundMarketDataConstantsProjectType**](PiFundMarketDataConstantsProjectType.md) |  | [optional] 
**IsFatcaAllow** | Pointer to **NullableBool** |  | [optional] 
**HasCurrencyRisk** | Pointer to **NullableBool** |  | [optional] 
**IsDerivative** | Pointer to **NullableBool** |  | [optional] 
**HasHealthInsuranceBenefit** | Pointer to **NullableBool** |  | [optional] 
**InvestorAlerts** | Pointer to [**[]PiFundMarketDataConstantsInvestorAlert**](PiFundMarketDataConstantsInvestorAlert.md) |  | [optional] 

## Methods

### NewPiFundMarketDataAPIModelsResponsesFundamentalTradingResponse

`func NewPiFundMarketDataAPIModelsResponsesFundamentalTradingResponse() *PiFundMarketDataAPIModelsResponsesFundamentalTradingResponse`

NewPiFundMarketDataAPIModelsResponsesFundamentalTradingResponse instantiates a new PiFundMarketDataAPIModelsResponsesFundamentalTradingResponse object
This constructor will assign default values to properties that have it defined,
and makes sure properties required by API are set, but the set of arguments
will change when the set of required properties is changed

### NewPiFundMarketDataAPIModelsResponsesFundamentalTradingResponseWithDefaults

`func NewPiFundMarketDataAPIModelsResponsesFundamentalTradingResponseWithDefaults() *PiFundMarketDataAPIModelsResponsesFundamentalTradingResponse`

NewPiFundMarketDataAPIModelsResponsesFundamentalTradingResponseWithDefaults instantiates a new PiFundMarketDataAPIModelsResponsesFundamentalTradingResponse object
This constructor will only assign default values to properties that have it defined,
but it doesn't guarantee that properties required by API are set

### GetTaxType

`func (o *PiFundMarketDataAPIModelsResponsesFundamentalTradingResponse) GetTaxType() string`

GetTaxType returns the TaxType field if non-nil, zero value otherwise.

### GetTaxTypeOk

`func (o *PiFundMarketDataAPIModelsResponsesFundamentalTradingResponse) GetTaxTypeOk() (*string, bool)`

GetTaxTypeOk returns a tuple with the TaxType field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetTaxType

`func (o *PiFundMarketDataAPIModelsResponsesFundamentalTradingResponse) SetTaxType(v string)`

SetTaxType sets TaxType field to given value.

### HasTaxType

`func (o *PiFundMarketDataAPIModelsResponsesFundamentalTradingResponse) HasTaxType() bool`

HasTaxType returns a boolean if a field has been set.

### SetTaxTypeNil

`func (o *PiFundMarketDataAPIModelsResponsesFundamentalTradingResponse) SetTaxTypeNil(b bool)`

 SetTaxTypeNil sets the value for TaxType to be an explicit nil

### UnsetTaxType
`func (o *PiFundMarketDataAPIModelsResponsesFundamentalTradingResponse) UnsetTaxType()`

UnsetTaxType ensures that no value is present for TaxType, not even an explicit nil
### GetFundRiskLevel

`func (o *PiFundMarketDataAPIModelsResponsesFundamentalTradingResponse) GetFundRiskLevel() int32`

GetFundRiskLevel returns the FundRiskLevel field if non-nil, zero value otherwise.

### GetFundRiskLevelOk

`func (o *PiFundMarketDataAPIModelsResponsesFundamentalTradingResponse) GetFundRiskLevelOk() (*int32, bool)`

GetFundRiskLevelOk returns a tuple with the FundRiskLevel field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetFundRiskLevel

`func (o *PiFundMarketDataAPIModelsResponsesFundamentalTradingResponse) SetFundRiskLevel(v int32)`

SetFundRiskLevel sets FundRiskLevel field to given value.

### HasFundRiskLevel

`func (o *PiFundMarketDataAPIModelsResponsesFundamentalTradingResponse) HasFundRiskLevel() bool`

HasFundRiskLevel returns a boolean if a field has been set.

### SetFundRiskLevelNil

`func (o *PiFundMarketDataAPIModelsResponsesFundamentalTradingResponse) SetFundRiskLevelNil(b bool)`

 SetFundRiskLevelNil sets the value for FundRiskLevel to be an explicit nil

### UnsetFundRiskLevel
`func (o *PiFundMarketDataAPIModelsResponsesFundamentalTradingResponse) UnsetFundRiskLevel()`

UnsetFundRiskLevel ensures that no value is present for FundRiskLevel, not even an explicit nil
### GetFundType

`func (o *PiFundMarketDataAPIModelsResponsesFundamentalTradingResponse) GetFundType() PiFundMarketDataConstantsFundType`

GetFundType returns the FundType field if non-nil, zero value otherwise.

### GetFundTypeOk

`func (o *PiFundMarketDataAPIModelsResponsesFundamentalTradingResponse) GetFundTypeOk() (*PiFundMarketDataConstantsFundType, bool)`

GetFundTypeOk returns a tuple with the FundType field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetFundType

`func (o *PiFundMarketDataAPIModelsResponsesFundamentalTradingResponse) SetFundType(v PiFundMarketDataConstantsFundType)`

SetFundType sets FundType field to given value.

### HasFundType

`func (o *PiFundMarketDataAPIModelsResponsesFundamentalTradingResponse) HasFundType() bool`

HasFundType returns a boolean if a field has been set.

### GetProjectType

`func (o *PiFundMarketDataAPIModelsResponsesFundamentalTradingResponse) GetProjectType() PiFundMarketDataConstantsProjectType`

GetProjectType returns the ProjectType field if non-nil, zero value otherwise.

### GetProjectTypeOk

`func (o *PiFundMarketDataAPIModelsResponsesFundamentalTradingResponse) GetProjectTypeOk() (*PiFundMarketDataConstantsProjectType, bool)`

GetProjectTypeOk returns a tuple with the ProjectType field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetProjectType

`func (o *PiFundMarketDataAPIModelsResponsesFundamentalTradingResponse) SetProjectType(v PiFundMarketDataConstantsProjectType)`

SetProjectType sets ProjectType field to given value.

### HasProjectType

`func (o *PiFundMarketDataAPIModelsResponsesFundamentalTradingResponse) HasProjectType() bool`

HasProjectType returns a boolean if a field has been set.

### GetIsFatcaAllow

`func (o *PiFundMarketDataAPIModelsResponsesFundamentalTradingResponse) GetIsFatcaAllow() bool`

GetIsFatcaAllow returns the IsFatcaAllow field if non-nil, zero value otherwise.

### GetIsFatcaAllowOk

`func (o *PiFundMarketDataAPIModelsResponsesFundamentalTradingResponse) GetIsFatcaAllowOk() (*bool, bool)`

GetIsFatcaAllowOk returns a tuple with the IsFatcaAllow field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetIsFatcaAllow

`func (o *PiFundMarketDataAPIModelsResponsesFundamentalTradingResponse) SetIsFatcaAllow(v bool)`

SetIsFatcaAllow sets IsFatcaAllow field to given value.

### HasIsFatcaAllow

`func (o *PiFundMarketDataAPIModelsResponsesFundamentalTradingResponse) HasIsFatcaAllow() bool`

HasIsFatcaAllow returns a boolean if a field has been set.

### SetIsFatcaAllowNil

`func (o *PiFundMarketDataAPIModelsResponsesFundamentalTradingResponse) SetIsFatcaAllowNil(b bool)`

 SetIsFatcaAllowNil sets the value for IsFatcaAllow to be an explicit nil

### UnsetIsFatcaAllow
`func (o *PiFundMarketDataAPIModelsResponsesFundamentalTradingResponse) UnsetIsFatcaAllow()`

UnsetIsFatcaAllow ensures that no value is present for IsFatcaAllow, not even an explicit nil
### GetHasCurrencyRisk

`func (o *PiFundMarketDataAPIModelsResponsesFundamentalTradingResponse) GetHasCurrencyRisk() bool`

GetHasCurrencyRisk returns the HasCurrencyRisk field if non-nil, zero value otherwise.

### GetHasCurrencyRiskOk

`func (o *PiFundMarketDataAPIModelsResponsesFundamentalTradingResponse) GetHasCurrencyRiskOk() (*bool, bool)`

GetHasCurrencyRiskOk returns a tuple with the HasCurrencyRisk field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetHasCurrencyRisk

`func (o *PiFundMarketDataAPIModelsResponsesFundamentalTradingResponse) SetHasCurrencyRisk(v bool)`

SetHasCurrencyRisk sets HasCurrencyRisk field to given value.

### HasHasCurrencyRisk

`func (o *PiFundMarketDataAPIModelsResponsesFundamentalTradingResponse) HasHasCurrencyRisk() bool`

HasHasCurrencyRisk returns a boolean if a field has been set.

### SetHasCurrencyRiskNil

`func (o *PiFundMarketDataAPIModelsResponsesFundamentalTradingResponse) SetHasCurrencyRiskNil(b bool)`

 SetHasCurrencyRiskNil sets the value for HasCurrencyRisk to be an explicit nil

### UnsetHasCurrencyRisk
`func (o *PiFundMarketDataAPIModelsResponsesFundamentalTradingResponse) UnsetHasCurrencyRisk()`

UnsetHasCurrencyRisk ensures that no value is present for HasCurrencyRisk, not even an explicit nil
### GetIsDerivative

`func (o *PiFundMarketDataAPIModelsResponsesFundamentalTradingResponse) GetIsDerivative() bool`

GetIsDerivative returns the IsDerivative field if non-nil, zero value otherwise.

### GetIsDerivativeOk

`func (o *PiFundMarketDataAPIModelsResponsesFundamentalTradingResponse) GetIsDerivativeOk() (*bool, bool)`

GetIsDerivativeOk returns a tuple with the IsDerivative field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetIsDerivative

`func (o *PiFundMarketDataAPIModelsResponsesFundamentalTradingResponse) SetIsDerivative(v bool)`

SetIsDerivative sets IsDerivative field to given value.

### HasIsDerivative

`func (o *PiFundMarketDataAPIModelsResponsesFundamentalTradingResponse) HasIsDerivative() bool`

HasIsDerivative returns a boolean if a field has been set.

### SetIsDerivativeNil

`func (o *PiFundMarketDataAPIModelsResponsesFundamentalTradingResponse) SetIsDerivativeNil(b bool)`

 SetIsDerivativeNil sets the value for IsDerivative to be an explicit nil

### UnsetIsDerivative
`func (o *PiFundMarketDataAPIModelsResponsesFundamentalTradingResponse) UnsetIsDerivative()`

UnsetIsDerivative ensures that no value is present for IsDerivative, not even an explicit nil
### GetHasHealthInsuranceBenefit

`func (o *PiFundMarketDataAPIModelsResponsesFundamentalTradingResponse) GetHasHealthInsuranceBenefit() bool`

GetHasHealthInsuranceBenefit returns the HasHealthInsuranceBenefit field if non-nil, zero value otherwise.

### GetHasHealthInsuranceBenefitOk

`func (o *PiFundMarketDataAPIModelsResponsesFundamentalTradingResponse) GetHasHealthInsuranceBenefitOk() (*bool, bool)`

GetHasHealthInsuranceBenefitOk returns a tuple with the HasHealthInsuranceBenefit field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetHasHealthInsuranceBenefit

`func (o *PiFundMarketDataAPIModelsResponsesFundamentalTradingResponse) SetHasHealthInsuranceBenefit(v bool)`

SetHasHealthInsuranceBenefit sets HasHealthInsuranceBenefit field to given value.

### HasHasHealthInsuranceBenefit

`func (o *PiFundMarketDataAPIModelsResponsesFundamentalTradingResponse) HasHasHealthInsuranceBenefit() bool`

HasHasHealthInsuranceBenefit returns a boolean if a field has been set.

### SetHasHealthInsuranceBenefitNil

`func (o *PiFundMarketDataAPIModelsResponsesFundamentalTradingResponse) SetHasHealthInsuranceBenefitNil(b bool)`

 SetHasHealthInsuranceBenefitNil sets the value for HasHealthInsuranceBenefit to be an explicit nil

### UnsetHasHealthInsuranceBenefit
`func (o *PiFundMarketDataAPIModelsResponsesFundamentalTradingResponse) UnsetHasHealthInsuranceBenefit()`

UnsetHasHealthInsuranceBenefit ensures that no value is present for HasHealthInsuranceBenefit, not even an explicit nil
### GetInvestorAlerts

`func (o *PiFundMarketDataAPIModelsResponsesFundamentalTradingResponse) GetInvestorAlerts() []PiFundMarketDataConstantsInvestorAlert`

GetInvestorAlerts returns the InvestorAlerts field if non-nil, zero value otherwise.

### GetInvestorAlertsOk

`func (o *PiFundMarketDataAPIModelsResponsesFundamentalTradingResponse) GetInvestorAlertsOk() (*[]PiFundMarketDataConstantsInvestorAlert, bool)`

GetInvestorAlertsOk returns a tuple with the InvestorAlerts field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetInvestorAlerts

`func (o *PiFundMarketDataAPIModelsResponsesFundamentalTradingResponse) SetInvestorAlerts(v []PiFundMarketDataConstantsInvestorAlert)`

SetInvestorAlerts sets InvestorAlerts field to given value.

### HasInvestorAlerts

`func (o *PiFundMarketDataAPIModelsResponsesFundamentalTradingResponse) HasInvestorAlerts() bool`

HasInvestorAlerts returns a boolean if a field has been set.

### SetInvestorAlertsNil

`func (o *PiFundMarketDataAPIModelsResponsesFundamentalTradingResponse) SetInvestorAlertsNil(b bool)`

 SetInvestorAlertsNil sets the value for InvestorAlerts to be an explicit nil

### UnsetInvestorAlerts
`func (o *PiFundMarketDataAPIModelsResponsesFundamentalTradingResponse) UnsetInvestorAlerts()`

UnsetInvestorAlerts ensures that no value is present for InvestorAlerts, not even an explicit nil

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


