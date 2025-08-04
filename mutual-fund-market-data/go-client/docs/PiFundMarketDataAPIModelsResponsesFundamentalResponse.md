# PiFundMarketDataAPIModelsResponsesFundamentalResponse

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**RiskLevel** | Pointer to **int32** |  | [optional] 
**IsForeignInvestment** | Pointer to **bool** |  | [optional] 
**InvestmentPolicy** | Pointer to **NullableString** |  | [optional] 
**HasCurrencyRisk** | Pointer to **bool** |  | [optional] 
**AssetClassFocus** | Pointer to **NullableString** |  | [optional] 
**AllowSwitchOut** | Pointer to **bool** |  | [optional] 
**InceptionDate** | Pointer to **NullableTime** |  | [optional] 
**Objective** | Pointer to **NullableString** |  | [optional] 
**FundSize** | Pointer to **NullableFloat32** |  | [optional] 
**IsDividend** | Pointer to **bool** |  | [optional] 
**TaxType** | Pointer to **NullableString** |  | [optional] 
**Currency** | Pointer to **NullableString** |  | [optional] 
**AsOfDate** | Pointer to **time.Time** |  | [optional] 

## Methods

### NewPiFundMarketDataAPIModelsResponsesFundamentalResponse

`func NewPiFundMarketDataAPIModelsResponsesFundamentalResponse() *PiFundMarketDataAPIModelsResponsesFundamentalResponse`

NewPiFundMarketDataAPIModelsResponsesFundamentalResponse instantiates a new PiFundMarketDataAPIModelsResponsesFundamentalResponse object
This constructor will assign default values to properties that have it defined,
and makes sure properties required by API are set, but the set of arguments
will change when the set of required properties is changed

### NewPiFundMarketDataAPIModelsResponsesFundamentalResponseWithDefaults

`func NewPiFundMarketDataAPIModelsResponsesFundamentalResponseWithDefaults() *PiFundMarketDataAPIModelsResponsesFundamentalResponse`

NewPiFundMarketDataAPIModelsResponsesFundamentalResponseWithDefaults instantiates a new PiFundMarketDataAPIModelsResponsesFundamentalResponse object
This constructor will only assign default values to properties that have it defined,
but it doesn't guarantee that properties required by API are set

### GetRiskLevel

`func (o *PiFundMarketDataAPIModelsResponsesFundamentalResponse) GetRiskLevel() int32`

GetRiskLevel returns the RiskLevel field if non-nil, zero value otherwise.

### GetRiskLevelOk

`func (o *PiFundMarketDataAPIModelsResponsesFundamentalResponse) GetRiskLevelOk() (*int32, bool)`

GetRiskLevelOk returns a tuple with the RiskLevel field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetRiskLevel

`func (o *PiFundMarketDataAPIModelsResponsesFundamentalResponse) SetRiskLevel(v int32)`

SetRiskLevel sets RiskLevel field to given value.

### HasRiskLevel

`func (o *PiFundMarketDataAPIModelsResponsesFundamentalResponse) HasRiskLevel() bool`

HasRiskLevel returns a boolean if a field has been set.

### GetIsForeignInvestment

`func (o *PiFundMarketDataAPIModelsResponsesFundamentalResponse) GetIsForeignInvestment() bool`

GetIsForeignInvestment returns the IsForeignInvestment field if non-nil, zero value otherwise.

### GetIsForeignInvestmentOk

`func (o *PiFundMarketDataAPIModelsResponsesFundamentalResponse) GetIsForeignInvestmentOk() (*bool, bool)`

GetIsForeignInvestmentOk returns a tuple with the IsForeignInvestment field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetIsForeignInvestment

`func (o *PiFundMarketDataAPIModelsResponsesFundamentalResponse) SetIsForeignInvestment(v bool)`

SetIsForeignInvestment sets IsForeignInvestment field to given value.

### HasIsForeignInvestment

`func (o *PiFundMarketDataAPIModelsResponsesFundamentalResponse) HasIsForeignInvestment() bool`

HasIsForeignInvestment returns a boolean if a field has been set.

### GetInvestmentPolicy

`func (o *PiFundMarketDataAPIModelsResponsesFundamentalResponse) GetInvestmentPolicy() string`

GetInvestmentPolicy returns the InvestmentPolicy field if non-nil, zero value otherwise.

### GetInvestmentPolicyOk

`func (o *PiFundMarketDataAPIModelsResponsesFundamentalResponse) GetInvestmentPolicyOk() (*string, bool)`

GetInvestmentPolicyOk returns a tuple with the InvestmentPolicy field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetInvestmentPolicy

`func (o *PiFundMarketDataAPIModelsResponsesFundamentalResponse) SetInvestmentPolicy(v string)`

SetInvestmentPolicy sets InvestmentPolicy field to given value.

### HasInvestmentPolicy

`func (o *PiFundMarketDataAPIModelsResponsesFundamentalResponse) HasInvestmentPolicy() bool`

HasInvestmentPolicy returns a boolean if a field has been set.

### SetInvestmentPolicyNil

`func (o *PiFundMarketDataAPIModelsResponsesFundamentalResponse) SetInvestmentPolicyNil(b bool)`

 SetInvestmentPolicyNil sets the value for InvestmentPolicy to be an explicit nil

### UnsetInvestmentPolicy
`func (o *PiFundMarketDataAPIModelsResponsesFundamentalResponse) UnsetInvestmentPolicy()`

UnsetInvestmentPolicy ensures that no value is present for InvestmentPolicy, not even an explicit nil
### GetHasCurrencyRisk

`func (o *PiFundMarketDataAPIModelsResponsesFundamentalResponse) GetHasCurrencyRisk() bool`

GetHasCurrencyRisk returns the HasCurrencyRisk field if non-nil, zero value otherwise.

### GetHasCurrencyRiskOk

`func (o *PiFundMarketDataAPIModelsResponsesFundamentalResponse) GetHasCurrencyRiskOk() (*bool, bool)`

GetHasCurrencyRiskOk returns a tuple with the HasCurrencyRisk field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetHasCurrencyRisk

`func (o *PiFundMarketDataAPIModelsResponsesFundamentalResponse) SetHasCurrencyRisk(v bool)`

SetHasCurrencyRisk sets HasCurrencyRisk field to given value.

### HasHasCurrencyRisk

`func (o *PiFundMarketDataAPIModelsResponsesFundamentalResponse) HasHasCurrencyRisk() bool`

HasHasCurrencyRisk returns a boolean if a field has been set.

### GetAssetClassFocus

`func (o *PiFundMarketDataAPIModelsResponsesFundamentalResponse) GetAssetClassFocus() string`

GetAssetClassFocus returns the AssetClassFocus field if non-nil, zero value otherwise.

### GetAssetClassFocusOk

`func (o *PiFundMarketDataAPIModelsResponsesFundamentalResponse) GetAssetClassFocusOk() (*string, bool)`

GetAssetClassFocusOk returns a tuple with the AssetClassFocus field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetAssetClassFocus

`func (o *PiFundMarketDataAPIModelsResponsesFundamentalResponse) SetAssetClassFocus(v string)`

SetAssetClassFocus sets AssetClassFocus field to given value.

### HasAssetClassFocus

`func (o *PiFundMarketDataAPIModelsResponsesFundamentalResponse) HasAssetClassFocus() bool`

HasAssetClassFocus returns a boolean if a field has been set.

### SetAssetClassFocusNil

`func (o *PiFundMarketDataAPIModelsResponsesFundamentalResponse) SetAssetClassFocusNil(b bool)`

 SetAssetClassFocusNil sets the value for AssetClassFocus to be an explicit nil

### UnsetAssetClassFocus
`func (o *PiFundMarketDataAPIModelsResponsesFundamentalResponse) UnsetAssetClassFocus()`

UnsetAssetClassFocus ensures that no value is present for AssetClassFocus, not even an explicit nil
### GetAllowSwitchOut

`func (o *PiFundMarketDataAPIModelsResponsesFundamentalResponse) GetAllowSwitchOut() bool`

GetAllowSwitchOut returns the AllowSwitchOut field if non-nil, zero value otherwise.

### GetAllowSwitchOutOk

`func (o *PiFundMarketDataAPIModelsResponsesFundamentalResponse) GetAllowSwitchOutOk() (*bool, bool)`

GetAllowSwitchOutOk returns a tuple with the AllowSwitchOut field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetAllowSwitchOut

`func (o *PiFundMarketDataAPIModelsResponsesFundamentalResponse) SetAllowSwitchOut(v bool)`

SetAllowSwitchOut sets AllowSwitchOut field to given value.

### HasAllowSwitchOut

`func (o *PiFundMarketDataAPIModelsResponsesFundamentalResponse) HasAllowSwitchOut() bool`

HasAllowSwitchOut returns a boolean if a field has been set.

### GetInceptionDate

`func (o *PiFundMarketDataAPIModelsResponsesFundamentalResponse) GetInceptionDate() time.Time`

GetInceptionDate returns the InceptionDate field if non-nil, zero value otherwise.

### GetInceptionDateOk

`func (o *PiFundMarketDataAPIModelsResponsesFundamentalResponse) GetInceptionDateOk() (*time.Time, bool)`

GetInceptionDateOk returns a tuple with the InceptionDate field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetInceptionDate

`func (o *PiFundMarketDataAPIModelsResponsesFundamentalResponse) SetInceptionDate(v time.Time)`

SetInceptionDate sets InceptionDate field to given value.

### HasInceptionDate

`func (o *PiFundMarketDataAPIModelsResponsesFundamentalResponse) HasInceptionDate() bool`

HasInceptionDate returns a boolean if a field has been set.

### SetInceptionDateNil

`func (o *PiFundMarketDataAPIModelsResponsesFundamentalResponse) SetInceptionDateNil(b bool)`

 SetInceptionDateNil sets the value for InceptionDate to be an explicit nil

### UnsetInceptionDate
`func (o *PiFundMarketDataAPIModelsResponsesFundamentalResponse) UnsetInceptionDate()`

UnsetInceptionDate ensures that no value is present for InceptionDate, not even an explicit nil
### GetObjective

`func (o *PiFundMarketDataAPIModelsResponsesFundamentalResponse) GetObjective() string`

GetObjective returns the Objective field if non-nil, zero value otherwise.

### GetObjectiveOk

`func (o *PiFundMarketDataAPIModelsResponsesFundamentalResponse) GetObjectiveOk() (*string, bool)`

GetObjectiveOk returns a tuple with the Objective field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetObjective

`func (o *PiFundMarketDataAPIModelsResponsesFundamentalResponse) SetObjective(v string)`

SetObjective sets Objective field to given value.

### HasObjective

`func (o *PiFundMarketDataAPIModelsResponsesFundamentalResponse) HasObjective() bool`

HasObjective returns a boolean if a field has been set.

### SetObjectiveNil

`func (o *PiFundMarketDataAPIModelsResponsesFundamentalResponse) SetObjectiveNil(b bool)`

 SetObjectiveNil sets the value for Objective to be an explicit nil

### UnsetObjective
`func (o *PiFundMarketDataAPIModelsResponsesFundamentalResponse) UnsetObjective()`

UnsetObjective ensures that no value is present for Objective, not even an explicit nil
### GetFundSize

`func (o *PiFundMarketDataAPIModelsResponsesFundamentalResponse) GetFundSize() float32`

GetFundSize returns the FundSize field if non-nil, zero value otherwise.

### GetFundSizeOk

`func (o *PiFundMarketDataAPIModelsResponsesFundamentalResponse) GetFundSizeOk() (*float32, bool)`

GetFundSizeOk returns a tuple with the FundSize field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetFundSize

`func (o *PiFundMarketDataAPIModelsResponsesFundamentalResponse) SetFundSize(v float32)`

SetFundSize sets FundSize field to given value.

### HasFundSize

`func (o *PiFundMarketDataAPIModelsResponsesFundamentalResponse) HasFundSize() bool`

HasFundSize returns a boolean if a field has been set.

### SetFundSizeNil

`func (o *PiFundMarketDataAPIModelsResponsesFundamentalResponse) SetFundSizeNil(b bool)`

 SetFundSizeNil sets the value for FundSize to be an explicit nil

### UnsetFundSize
`func (o *PiFundMarketDataAPIModelsResponsesFundamentalResponse) UnsetFundSize()`

UnsetFundSize ensures that no value is present for FundSize, not even an explicit nil
### GetIsDividend

`func (o *PiFundMarketDataAPIModelsResponsesFundamentalResponse) GetIsDividend() bool`

GetIsDividend returns the IsDividend field if non-nil, zero value otherwise.

### GetIsDividendOk

`func (o *PiFundMarketDataAPIModelsResponsesFundamentalResponse) GetIsDividendOk() (*bool, bool)`

GetIsDividendOk returns a tuple with the IsDividend field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetIsDividend

`func (o *PiFundMarketDataAPIModelsResponsesFundamentalResponse) SetIsDividend(v bool)`

SetIsDividend sets IsDividend field to given value.

### HasIsDividend

`func (o *PiFundMarketDataAPIModelsResponsesFundamentalResponse) HasIsDividend() bool`

HasIsDividend returns a boolean if a field has been set.

### GetTaxType

`func (o *PiFundMarketDataAPIModelsResponsesFundamentalResponse) GetTaxType() string`

GetTaxType returns the TaxType field if non-nil, zero value otherwise.

### GetTaxTypeOk

`func (o *PiFundMarketDataAPIModelsResponsesFundamentalResponse) GetTaxTypeOk() (*string, bool)`

GetTaxTypeOk returns a tuple with the TaxType field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetTaxType

`func (o *PiFundMarketDataAPIModelsResponsesFundamentalResponse) SetTaxType(v string)`

SetTaxType sets TaxType field to given value.

### HasTaxType

`func (o *PiFundMarketDataAPIModelsResponsesFundamentalResponse) HasTaxType() bool`

HasTaxType returns a boolean if a field has been set.

### SetTaxTypeNil

`func (o *PiFundMarketDataAPIModelsResponsesFundamentalResponse) SetTaxTypeNil(b bool)`

 SetTaxTypeNil sets the value for TaxType to be an explicit nil

### UnsetTaxType
`func (o *PiFundMarketDataAPIModelsResponsesFundamentalResponse) UnsetTaxType()`

UnsetTaxType ensures that no value is present for TaxType, not even an explicit nil
### GetCurrency

`func (o *PiFundMarketDataAPIModelsResponsesFundamentalResponse) GetCurrency() string`

GetCurrency returns the Currency field if non-nil, zero value otherwise.

### GetCurrencyOk

`func (o *PiFundMarketDataAPIModelsResponsesFundamentalResponse) GetCurrencyOk() (*string, bool)`

GetCurrencyOk returns a tuple with the Currency field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetCurrency

`func (o *PiFundMarketDataAPIModelsResponsesFundamentalResponse) SetCurrency(v string)`

SetCurrency sets Currency field to given value.

### HasCurrency

`func (o *PiFundMarketDataAPIModelsResponsesFundamentalResponse) HasCurrency() bool`

HasCurrency returns a boolean if a field has been set.

### SetCurrencyNil

`func (o *PiFundMarketDataAPIModelsResponsesFundamentalResponse) SetCurrencyNil(b bool)`

 SetCurrencyNil sets the value for Currency to be an explicit nil

### UnsetCurrency
`func (o *PiFundMarketDataAPIModelsResponsesFundamentalResponse) UnsetCurrency()`

UnsetCurrency ensures that no value is present for Currency, not even an explicit nil
### GetAsOfDate

`func (o *PiFundMarketDataAPIModelsResponsesFundamentalResponse) GetAsOfDate() time.Time`

GetAsOfDate returns the AsOfDate field if non-nil, zero value otherwise.

### GetAsOfDateOk

`func (o *PiFundMarketDataAPIModelsResponsesFundamentalResponse) GetAsOfDateOk() (*time.Time, bool)`

GetAsOfDateOk returns a tuple with the AsOfDate field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetAsOfDate

`func (o *PiFundMarketDataAPIModelsResponsesFundamentalResponse) SetAsOfDate(v time.Time)`

SetAsOfDate sets AsOfDate field to given value.

### HasAsOfDate

`func (o *PiFundMarketDataAPIModelsResponsesFundamentalResponse) HasAsOfDate() bool`

HasAsOfDate returns a boolean if a field has been set.


[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


