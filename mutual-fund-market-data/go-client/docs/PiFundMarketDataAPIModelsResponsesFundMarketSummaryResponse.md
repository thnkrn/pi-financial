# PiFundMarketDataAPIModelsResponsesFundMarketSummaryResponse

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**Symbol** | Pointer to **NullableString** |  | [optional] 
**AsOfDate** | Pointer to **time.Time** |  | [optional] 
**RiskLevel** | Pointer to **NullableInt32** |  | [optional] 
**Name** | Pointer to **NullableString** |  | [optional] 
**Nav** | Pointer to **float32** |  | [optional] 
**ReturnPercentage** | Pointer to **NullableFloat32** |  | [optional] 
**Currency** | Pointer to **NullableString** |  | [optional] 
**FundCategory** | Pointer to **NullableString** |  | [optional] 
**AmcLogo** | Pointer to **NullableString** |  | [optional] 

## Methods

### NewPiFundMarketDataAPIModelsResponsesFundMarketSummaryResponse

`func NewPiFundMarketDataAPIModelsResponsesFundMarketSummaryResponse() *PiFundMarketDataAPIModelsResponsesFundMarketSummaryResponse`

NewPiFundMarketDataAPIModelsResponsesFundMarketSummaryResponse instantiates a new PiFundMarketDataAPIModelsResponsesFundMarketSummaryResponse object
This constructor will assign default values to properties that have it defined,
and makes sure properties required by API are set, but the set of arguments
will change when the set of required properties is changed

### NewPiFundMarketDataAPIModelsResponsesFundMarketSummaryResponseWithDefaults

`func NewPiFundMarketDataAPIModelsResponsesFundMarketSummaryResponseWithDefaults() *PiFundMarketDataAPIModelsResponsesFundMarketSummaryResponse`

NewPiFundMarketDataAPIModelsResponsesFundMarketSummaryResponseWithDefaults instantiates a new PiFundMarketDataAPIModelsResponsesFundMarketSummaryResponse object
This constructor will only assign default values to properties that have it defined,
but it doesn't guarantee that properties required by API are set

### GetSymbol

`func (o *PiFundMarketDataAPIModelsResponsesFundMarketSummaryResponse) GetSymbol() string`

GetSymbol returns the Symbol field if non-nil, zero value otherwise.

### GetSymbolOk

`func (o *PiFundMarketDataAPIModelsResponsesFundMarketSummaryResponse) GetSymbolOk() (*string, bool)`

GetSymbolOk returns a tuple with the Symbol field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetSymbol

`func (o *PiFundMarketDataAPIModelsResponsesFundMarketSummaryResponse) SetSymbol(v string)`

SetSymbol sets Symbol field to given value.

### HasSymbol

`func (o *PiFundMarketDataAPIModelsResponsesFundMarketSummaryResponse) HasSymbol() bool`

HasSymbol returns a boolean if a field has been set.

### SetSymbolNil

`func (o *PiFundMarketDataAPIModelsResponsesFundMarketSummaryResponse) SetSymbolNil(b bool)`

 SetSymbolNil sets the value for Symbol to be an explicit nil

### UnsetSymbol
`func (o *PiFundMarketDataAPIModelsResponsesFundMarketSummaryResponse) UnsetSymbol()`

UnsetSymbol ensures that no value is present for Symbol, not even an explicit nil
### GetAsOfDate

`func (o *PiFundMarketDataAPIModelsResponsesFundMarketSummaryResponse) GetAsOfDate() time.Time`

GetAsOfDate returns the AsOfDate field if non-nil, zero value otherwise.

### GetAsOfDateOk

`func (o *PiFundMarketDataAPIModelsResponsesFundMarketSummaryResponse) GetAsOfDateOk() (*time.Time, bool)`

GetAsOfDateOk returns a tuple with the AsOfDate field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetAsOfDate

`func (o *PiFundMarketDataAPIModelsResponsesFundMarketSummaryResponse) SetAsOfDate(v time.Time)`

SetAsOfDate sets AsOfDate field to given value.

### HasAsOfDate

`func (o *PiFundMarketDataAPIModelsResponsesFundMarketSummaryResponse) HasAsOfDate() bool`

HasAsOfDate returns a boolean if a field has been set.

### GetRiskLevel

`func (o *PiFundMarketDataAPIModelsResponsesFundMarketSummaryResponse) GetRiskLevel() int32`

GetRiskLevel returns the RiskLevel field if non-nil, zero value otherwise.

### GetRiskLevelOk

`func (o *PiFundMarketDataAPIModelsResponsesFundMarketSummaryResponse) GetRiskLevelOk() (*int32, bool)`

GetRiskLevelOk returns a tuple with the RiskLevel field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetRiskLevel

`func (o *PiFundMarketDataAPIModelsResponsesFundMarketSummaryResponse) SetRiskLevel(v int32)`

SetRiskLevel sets RiskLevel field to given value.

### HasRiskLevel

`func (o *PiFundMarketDataAPIModelsResponsesFundMarketSummaryResponse) HasRiskLevel() bool`

HasRiskLevel returns a boolean if a field has been set.

### SetRiskLevelNil

`func (o *PiFundMarketDataAPIModelsResponsesFundMarketSummaryResponse) SetRiskLevelNil(b bool)`

 SetRiskLevelNil sets the value for RiskLevel to be an explicit nil

### UnsetRiskLevel
`func (o *PiFundMarketDataAPIModelsResponsesFundMarketSummaryResponse) UnsetRiskLevel()`

UnsetRiskLevel ensures that no value is present for RiskLevel, not even an explicit nil
### GetName

`func (o *PiFundMarketDataAPIModelsResponsesFundMarketSummaryResponse) GetName() string`

GetName returns the Name field if non-nil, zero value otherwise.

### GetNameOk

`func (o *PiFundMarketDataAPIModelsResponsesFundMarketSummaryResponse) GetNameOk() (*string, bool)`

GetNameOk returns a tuple with the Name field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetName

`func (o *PiFundMarketDataAPIModelsResponsesFundMarketSummaryResponse) SetName(v string)`

SetName sets Name field to given value.

### HasName

`func (o *PiFundMarketDataAPIModelsResponsesFundMarketSummaryResponse) HasName() bool`

HasName returns a boolean if a field has been set.

### SetNameNil

`func (o *PiFundMarketDataAPIModelsResponsesFundMarketSummaryResponse) SetNameNil(b bool)`

 SetNameNil sets the value for Name to be an explicit nil

### UnsetName
`func (o *PiFundMarketDataAPIModelsResponsesFundMarketSummaryResponse) UnsetName()`

UnsetName ensures that no value is present for Name, not even an explicit nil
### GetNav

`func (o *PiFundMarketDataAPIModelsResponsesFundMarketSummaryResponse) GetNav() float32`

GetNav returns the Nav field if non-nil, zero value otherwise.

### GetNavOk

`func (o *PiFundMarketDataAPIModelsResponsesFundMarketSummaryResponse) GetNavOk() (*float32, bool)`

GetNavOk returns a tuple with the Nav field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetNav

`func (o *PiFundMarketDataAPIModelsResponsesFundMarketSummaryResponse) SetNav(v float32)`

SetNav sets Nav field to given value.

### HasNav

`func (o *PiFundMarketDataAPIModelsResponsesFundMarketSummaryResponse) HasNav() bool`

HasNav returns a boolean if a field has been set.

### GetReturnPercentage

`func (o *PiFundMarketDataAPIModelsResponsesFundMarketSummaryResponse) GetReturnPercentage() float32`

GetReturnPercentage returns the ReturnPercentage field if non-nil, zero value otherwise.

### GetReturnPercentageOk

`func (o *PiFundMarketDataAPIModelsResponsesFundMarketSummaryResponse) GetReturnPercentageOk() (*float32, bool)`

GetReturnPercentageOk returns a tuple with the ReturnPercentage field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetReturnPercentage

`func (o *PiFundMarketDataAPIModelsResponsesFundMarketSummaryResponse) SetReturnPercentage(v float32)`

SetReturnPercentage sets ReturnPercentage field to given value.

### HasReturnPercentage

`func (o *PiFundMarketDataAPIModelsResponsesFundMarketSummaryResponse) HasReturnPercentage() bool`

HasReturnPercentage returns a boolean if a field has been set.

### SetReturnPercentageNil

`func (o *PiFundMarketDataAPIModelsResponsesFundMarketSummaryResponse) SetReturnPercentageNil(b bool)`

 SetReturnPercentageNil sets the value for ReturnPercentage to be an explicit nil

### UnsetReturnPercentage
`func (o *PiFundMarketDataAPIModelsResponsesFundMarketSummaryResponse) UnsetReturnPercentage()`

UnsetReturnPercentage ensures that no value is present for ReturnPercentage, not even an explicit nil
### GetCurrency

`func (o *PiFundMarketDataAPIModelsResponsesFundMarketSummaryResponse) GetCurrency() string`

GetCurrency returns the Currency field if non-nil, zero value otherwise.

### GetCurrencyOk

`func (o *PiFundMarketDataAPIModelsResponsesFundMarketSummaryResponse) GetCurrencyOk() (*string, bool)`

GetCurrencyOk returns a tuple with the Currency field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetCurrency

`func (o *PiFundMarketDataAPIModelsResponsesFundMarketSummaryResponse) SetCurrency(v string)`

SetCurrency sets Currency field to given value.

### HasCurrency

`func (o *PiFundMarketDataAPIModelsResponsesFundMarketSummaryResponse) HasCurrency() bool`

HasCurrency returns a boolean if a field has been set.

### SetCurrencyNil

`func (o *PiFundMarketDataAPIModelsResponsesFundMarketSummaryResponse) SetCurrencyNil(b bool)`

 SetCurrencyNil sets the value for Currency to be an explicit nil

### UnsetCurrency
`func (o *PiFundMarketDataAPIModelsResponsesFundMarketSummaryResponse) UnsetCurrency()`

UnsetCurrency ensures that no value is present for Currency, not even an explicit nil
### GetFundCategory

`func (o *PiFundMarketDataAPIModelsResponsesFundMarketSummaryResponse) GetFundCategory() string`

GetFundCategory returns the FundCategory field if non-nil, zero value otherwise.

### GetFundCategoryOk

`func (o *PiFundMarketDataAPIModelsResponsesFundMarketSummaryResponse) GetFundCategoryOk() (*string, bool)`

GetFundCategoryOk returns a tuple with the FundCategory field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetFundCategory

`func (o *PiFundMarketDataAPIModelsResponsesFundMarketSummaryResponse) SetFundCategory(v string)`

SetFundCategory sets FundCategory field to given value.

### HasFundCategory

`func (o *PiFundMarketDataAPIModelsResponsesFundMarketSummaryResponse) HasFundCategory() bool`

HasFundCategory returns a boolean if a field has been set.

### SetFundCategoryNil

`func (o *PiFundMarketDataAPIModelsResponsesFundMarketSummaryResponse) SetFundCategoryNil(b bool)`

 SetFundCategoryNil sets the value for FundCategory to be an explicit nil

### UnsetFundCategory
`func (o *PiFundMarketDataAPIModelsResponsesFundMarketSummaryResponse) UnsetFundCategory()`

UnsetFundCategory ensures that no value is present for FundCategory, not even an explicit nil
### GetAmcLogo

`func (o *PiFundMarketDataAPIModelsResponsesFundMarketSummaryResponse) GetAmcLogo() string`

GetAmcLogo returns the AmcLogo field if non-nil, zero value otherwise.

### GetAmcLogoOk

`func (o *PiFundMarketDataAPIModelsResponsesFundMarketSummaryResponse) GetAmcLogoOk() (*string, bool)`

GetAmcLogoOk returns a tuple with the AmcLogo field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetAmcLogo

`func (o *PiFundMarketDataAPIModelsResponsesFundMarketSummaryResponse) SetAmcLogo(v string)`

SetAmcLogo sets AmcLogo field to given value.

### HasAmcLogo

`func (o *PiFundMarketDataAPIModelsResponsesFundMarketSummaryResponse) HasAmcLogo() bool`

HasAmcLogo returns a boolean if a field has been set.

### SetAmcLogoNil

`func (o *PiFundMarketDataAPIModelsResponsesFundMarketSummaryResponse) SetAmcLogoNil(b bool)`

 SetAmcLogoNil sets the value for AmcLogo to be an explicit nil

### UnsetAmcLogo
`func (o *PiFundMarketDataAPIModelsResponsesFundMarketSummaryResponse) UnsetAmcLogo()`

UnsetAmcLogo ensures that no value is present for AmcLogo, not even an explicit nil

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


