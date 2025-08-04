# PiMarketDataDomainModelsResponseProfileFinancialsResponse

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**Sales** | Pointer to [**PiMarketDataDomainModelsResponseSales**](PiMarketDataDomainModelsResponseSales.md) |  | [optional] 
**OperatingIncome** | Pointer to [**PiMarketDataDomainModelsResponseOperatingIncome**](PiMarketDataDomainModelsResponseOperatingIncome.md) |  | [optional] 
**NetIncome** | Pointer to [**PiMarketDataDomainModelsResponseNetIncome**](PiMarketDataDomainModelsResponseNetIncome.md) |  | [optional] 
**EarningsPerShare** | Pointer to [**PiMarketDataDomainModelsResponseEarningsPerShare**](PiMarketDataDomainModelsResponseEarningsPerShare.md) |  | [optional] 
**DividendPerShare** | Pointer to [**PiMarketDataDomainModelsResponseDividendPerShare**](PiMarketDataDomainModelsResponseDividendPerShare.md) |  | [optional] 
**CashflowPerShare** | Pointer to [**PiMarketDataDomainModelsResponseCashflowPerShare**](PiMarketDataDomainModelsResponseCashflowPerShare.md) |  | [optional] 
**TotalAssets** | Pointer to [**PiMarketDataDomainModelsResponseTotalAssets**](PiMarketDataDomainModelsResponseTotalAssets.md) |  | [optional] 
**TotalLiabilities** | Pointer to [**PiMarketDataDomainModelsResponseTotalLiabilities**](PiMarketDataDomainModelsResponseTotalLiabilities.md) |  | [optional] 
**OperatingMargin** | Pointer to [**PiMarketDataDomainModelsResponseOperatingMargin**](PiMarketDataDomainModelsResponseOperatingMargin.md) |  | [optional] 
**LiabilitiesToAssets** | Pointer to [**PiMarketDataDomainModelsResponseLiabilitiesToAssets**](PiMarketDataDomainModelsResponseLiabilitiesToAssets.md) |  | [optional] 
**AverageShareCount** | Pointer to [**PiMarketDataDomainModelsResponseAverageShareCount**](PiMarketDataDomainModelsResponseAverageShareCount.md) |  | [optional] 
**Units** | Pointer to **NullableString** |  | [optional] 
**LatestFinancials** | Pointer to **NullableString** |  | [optional] 
**Source** | Pointer to **NullableString** |  | [optional] 

## Methods

### NewPiMarketDataDomainModelsResponseProfileFinancialsResponse

`func NewPiMarketDataDomainModelsResponseProfileFinancialsResponse() *PiMarketDataDomainModelsResponseProfileFinancialsResponse`

NewPiMarketDataDomainModelsResponseProfileFinancialsResponse instantiates a new PiMarketDataDomainModelsResponseProfileFinancialsResponse object
This constructor will assign default values to properties that have it defined,
and makes sure properties required by API are set, but the set of arguments
will change when the set of required properties is changed

### NewPiMarketDataDomainModelsResponseProfileFinancialsResponseWithDefaults

`func NewPiMarketDataDomainModelsResponseProfileFinancialsResponseWithDefaults() *PiMarketDataDomainModelsResponseProfileFinancialsResponse`

NewPiMarketDataDomainModelsResponseProfileFinancialsResponseWithDefaults instantiates a new PiMarketDataDomainModelsResponseProfileFinancialsResponse object
This constructor will only assign default values to properties that have it defined,
but it doesn't guarantee that properties required by API are set

### GetSales

`func (o *PiMarketDataDomainModelsResponseProfileFinancialsResponse) GetSales() PiMarketDataDomainModelsResponseSales`

GetSales returns the Sales field if non-nil, zero value otherwise.

### GetSalesOk

`func (o *PiMarketDataDomainModelsResponseProfileFinancialsResponse) GetSalesOk() (*PiMarketDataDomainModelsResponseSales, bool)`

GetSalesOk returns a tuple with the Sales field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetSales

`func (o *PiMarketDataDomainModelsResponseProfileFinancialsResponse) SetSales(v PiMarketDataDomainModelsResponseSales)`

SetSales sets Sales field to given value.

### HasSales

`func (o *PiMarketDataDomainModelsResponseProfileFinancialsResponse) HasSales() bool`

HasSales returns a boolean if a field has been set.

### GetOperatingIncome

`func (o *PiMarketDataDomainModelsResponseProfileFinancialsResponse) GetOperatingIncome() PiMarketDataDomainModelsResponseOperatingIncome`

GetOperatingIncome returns the OperatingIncome field if non-nil, zero value otherwise.

### GetOperatingIncomeOk

`func (o *PiMarketDataDomainModelsResponseProfileFinancialsResponse) GetOperatingIncomeOk() (*PiMarketDataDomainModelsResponseOperatingIncome, bool)`

GetOperatingIncomeOk returns a tuple with the OperatingIncome field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetOperatingIncome

`func (o *PiMarketDataDomainModelsResponseProfileFinancialsResponse) SetOperatingIncome(v PiMarketDataDomainModelsResponseOperatingIncome)`

SetOperatingIncome sets OperatingIncome field to given value.

### HasOperatingIncome

`func (o *PiMarketDataDomainModelsResponseProfileFinancialsResponse) HasOperatingIncome() bool`

HasOperatingIncome returns a boolean if a field has been set.

### GetNetIncome

`func (o *PiMarketDataDomainModelsResponseProfileFinancialsResponse) GetNetIncome() PiMarketDataDomainModelsResponseNetIncome`

GetNetIncome returns the NetIncome field if non-nil, zero value otherwise.

### GetNetIncomeOk

`func (o *PiMarketDataDomainModelsResponseProfileFinancialsResponse) GetNetIncomeOk() (*PiMarketDataDomainModelsResponseNetIncome, bool)`

GetNetIncomeOk returns a tuple with the NetIncome field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetNetIncome

`func (o *PiMarketDataDomainModelsResponseProfileFinancialsResponse) SetNetIncome(v PiMarketDataDomainModelsResponseNetIncome)`

SetNetIncome sets NetIncome field to given value.

### HasNetIncome

`func (o *PiMarketDataDomainModelsResponseProfileFinancialsResponse) HasNetIncome() bool`

HasNetIncome returns a boolean if a field has been set.

### GetEarningsPerShare

`func (o *PiMarketDataDomainModelsResponseProfileFinancialsResponse) GetEarningsPerShare() PiMarketDataDomainModelsResponseEarningsPerShare`

GetEarningsPerShare returns the EarningsPerShare field if non-nil, zero value otherwise.

### GetEarningsPerShareOk

`func (o *PiMarketDataDomainModelsResponseProfileFinancialsResponse) GetEarningsPerShareOk() (*PiMarketDataDomainModelsResponseEarningsPerShare, bool)`

GetEarningsPerShareOk returns a tuple with the EarningsPerShare field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetEarningsPerShare

`func (o *PiMarketDataDomainModelsResponseProfileFinancialsResponse) SetEarningsPerShare(v PiMarketDataDomainModelsResponseEarningsPerShare)`

SetEarningsPerShare sets EarningsPerShare field to given value.

### HasEarningsPerShare

`func (o *PiMarketDataDomainModelsResponseProfileFinancialsResponse) HasEarningsPerShare() bool`

HasEarningsPerShare returns a boolean if a field has been set.

### GetDividendPerShare

`func (o *PiMarketDataDomainModelsResponseProfileFinancialsResponse) GetDividendPerShare() PiMarketDataDomainModelsResponseDividendPerShare`

GetDividendPerShare returns the DividendPerShare field if non-nil, zero value otherwise.

### GetDividendPerShareOk

`func (o *PiMarketDataDomainModelsResponseProfileFinancialsResponse) GetDividendPerShareOk() (*PiMarketDataDomainModelsResponseDividendPerShare, bool)`

GetDividendPerShareOk returns a tuple with the DividendPerShare field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetDividendPerShare

`func (o *PiMarketDataDomainModelsResponseProfileFinancialsResponse) SetDividendPerShare(v PiMarketDataDomainModelsResponseDividendPerShare)`

SetDividendPerShare sets DividendPerShare field to given value.

### HasDividendPerShare

`func (o *PiMarketDataDomainModelsResponseProfileFinancialsResponse) HasDividendPerShare() bool`

HasDividendPerShare returns a boolean if a field has been set.

### GetCashflowPerShare

`func (o *PiMarketDataDomainModelsResponseProfileFinancialsResponse) GetCashflowPerShare() PiMarketDataDomainModelsResponseCashflowPerShare`

GetCashflowPerShare returns the CashflowPerShare field if non-nil, zero value otherwise.

### GetCashflowPerShareOk

`func (o *PiMarketDataDomainModelsResponseProfileFinancialsResponse) GetCashflowPerShareOk() (*PiMarketDataDomainModelsResponseCashflowPerShare, bool)`

GetCashflowPerShareOk returns a tuple with the CashflowPerShare field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetCashflowPerShare

`func (o *PiMarketDataDomainModelsResponseProfileFinancialsResponse) SetCashflowPerShare(v PiMarketDataDomainModelsResponseCashflowPerShare)`

SetCashflowPerShare sets CashflowPerShare field to given value.

### HasCashflowPerShare

`func (o *PiMarketDataDomainModelsResponseProfileFinancialsResponse) HasCashflowPerShare() bool`

HasCashflowPerShare returns a boolean if a field has been set.

### GetTotalAssets

`func (o *PiMarketDataDomainModelsResponseProfileFinancialsResponse) GetTotalAssets() PiMarketDataDomainModelsResponseTotalAssets`

GetTotalAssets returns the TotalAssets field if non-nil, zero value otherwise.

### GetTotalAssetsOk

`func (o *PiMarketDataDomainModelsResponseProfileFinancialsResponse) GetTotalAssetsOk() (*PiMarketDataDomainModelsResponseTotalAssets, bool)`

GetTotalAssetsOk returns a tuple with the TotalAssets field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetTotalAssets

`func (o *PiMarketDataDomainModelsResponseProfileFinancialsResponse) SetTotalAssets(v PiMarketDataDomainModelsResponseTotalAssets)`

SetTotalAssets sets TotalAssets field to given value.

### HasTotalAssets

`func (o *PiMarketDataDomainModelsResponseProfileFinancialsResponse) HasTotalAssets() bool`

HasTotalAssets returns a boolean if a field has been set.

### GetTotalLiabilities

`func (o *PiMarketDataDomainModelsResponseProfileFinancialsResponse) GetTotalLiabilities() PiMarketDataDomainModelsResponseTotalLiabilities`

GetTotalLiabilities returns the TotalLiabilities field if non-nil, zero value otherwise.

### GetTotalLiabilitiesOk

`func (o *PiMarketDataDomainModelsResponseProfileFinancialsResponse) GetTotalLiabilitiesOk() (*PiMarketDataDomainModelsResponseTotalLiabilities, bool)`

GetTotalLiabilitiesOk returns a tuple with the TotalLiabilities field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetTotalLiabilities

`func (o *PiMarketDataDomainModelsResponseProfileFinancialsResponse) SetTotalLiabilities(v PiMarketDataDomainModelsResponseTotalLiabilities)`

SetTotalLiabilities sets TotalLiabilities field to given value.

### HasTotalLiabilities

`func (o *PiMarketDataDomainModelsResponseProfileFinancialsResponse) HasTotalLiabilities() bool`

HasTotalLiabilities returns a boolean if a field has been set.

### GetOperatingMargin

`func (o *PiMarketDataDomainModelsResponseProfileFinancialsResponse) GetOperatingMargin() PiMarketDataDomainModelsResponseOperatingMargin`

GetOperatingMargin returns the OperatingMargin field if non-nil, zero value otherwise.

### GetOperatingMarginOk

`func (o *PiMarketDataDomainModelsResponseProfileFinancialsResponse) GetOperatingMarginOk() (*PiMarketDataDomainModelsResponseOperatingMargin, bool)`

GetOperatingMarginOk returns a tuple with the OperatingMargin field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetOperatingMargin

`func (o *PiMarketDataDomainModelsResponseProfileFinancialsResponse) SetOperatingMargin(v PiMarketDataDomainModelsResponseOperatingMargin)`

SetOperatingMargin sets OperatingMargin field to given value.

### HasOperatingMargin

`func (o *PiMarketDataDomainModelsResponseProfileFinancialsResponse) HasOperatingMargin() bool`

HasOperatingMargin returns a boolean if a field has been set.

### GetLiabilitiesToAssets

`func (o *PiMarketDataDomainModelsResponseProfileFinancialsResponse) GetLiabilitiesToAssets() PiMarketDataDomainModelsResponseLiabilitiesToAssets`

GetLiabilitiesToAssets returns the LiabilitiesToAssets field if non-nil, zero value otherwise.

### GetLiabilitiesToAssetsOk

`func (o *PiMarketDataDomainModelsResponseProfileFinancialsResponse) GetLiabilitiesToAssetsOk() (*PiMarketDataDomainModelsResponseLiabilitiesToAssets, bool)`

GetLiabilitiesToAssetsOk returns a tuple with the LiabilitiesToAssets field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetLiabilitiesToAssets

`func (o *PiMarketDataDomainModelsResponseProfileFinancialsResponse) SetLiabilitiesToAssets(v PiMarketDataDomainModelsResponseLiabilitiesToAssets)`

SetLiabilitiesToAssets sets LiabilitiesToAssets field to given value.

### HasLiabilitiesToAssets

`func (o *PiMarketDataDomainModelsResponseProfileFinancialsResponse) HasLiabilitiesToAssets() bool`

HasLiabilitiesToAssets returns a boolean if a field has been set.

### GetAverageShareCount

`func (o *PiMarketDataDomainModelsResponseProfileFinancialsResponse) GetAverageShareCount() PiMarketDataDomainModelsResponseAverageShareCount`

GetAverageShareCount returns the AverageShareCount field if non-nil, zero value otherwise.

### GetAverageShareCountOk

`func (o *PiMarketDataDomainModelsResponseProfileFinancialsResponse) GetAverageShareCountOk() (*PiMarketDataDomainModelsResponseAverageShareCount, bool)`

GetAverageShareCountOk returns a tuple with the AverageShareCount field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetAverageShareCount

`func (o *PiMarketDataDomainModelsResponseProfileFinancialsResponse) SetAverageShareCount(v PiMarketDataDomainModelsResponseAverageShareCount)`

SetAverageShareCount sets AverageShareCount field to given value.

### HasAverageShareCount

`func (o *PiMarketDataDomainModelsResponseProfileFinancialsResponse) HasAverageShareCount() bool`

HasAverageShareCount returns a boolean if a field has been set.

### GetUnits

`func (o *PiMarketDataDomainModelsResponseProfileFinancialsResponse) GetUnits() string`

GetUnits returns the Units field if non-nil, zero value otherwise.

### GetUnitsOk

`func (o *PiMarketDataDomainModelsResponseProfileFinancialsResponse) GetUnitsOk() (*string, bool)`

GetUnitsOk returns a tuple with the Units field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetUnits

`func (o *PiMarketDataDomainModelsResponseProfileFinancialsResponse) SetUnits(v string)`

SetUnits sets Units field to given value.

### HasUnits

`func (o *PiMarketDataDomainModelsResponseProfileFinancialsResponse) HasUnits() bool`

HasUnits returns a boolean if a field has been set.

### SetUnitsNil

`func (o *PiMarketDataDomainModelsResponseProfileFinancialsResponse) SetUnitsNil(b bool)`

 SetUnitsNil sets the value for Units to be an explicit nil

### UnsetUnits
`func (o *PiMarketDataDomainModelsResponseProfileFinancialsResponse) UnsetUnits()`

UnsetUnits ensures that no value is present for Units, not even an explicit nil
### GetLatestFinancials

`func (o *PiMarketDataDomainModelsResponseProfileFinancialsResponse) GetLatestFinancials() string`

GetLatestFinancials returns the LatestFinancials field if non-nil, zero value otherwise.

### GetLatestFinancialsOk

`func (o *PiMarketDataDomainModelsResponseProfileFinancialsResponse) GetLatestFinancialsOk() (*string, bool)`

GetLatestFinancialsOk returns a tuple with the LatestFinancials field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetLatestFinancials

`func (o *PiMarketDataDomainModelsResponseProfileFinancialsResponse) SetLatestFinancials(v string)`

SetLatestFinancials sets LatestFinancials field to given value.

### HasLatestFinancials

`func (o *PiMarketDataDomainModelsResponseProfileFinancialsResponse) HasLatestFinancials() bool`

HasLatestFinancials returns a boolean if a field has been set.

### SetLatestFinancialsNil

`func (o *PiMarketDataDomainModelsResponseProfileFinancialsResponse) SetLatestFinancialsNil(b bool)`

 SetLatestFinancialsNil sets the value for LatestFinancials to be an explicit nil

### UnsetLatestFinancials
`func (o *PiMarketDataDomainModelsResponseProfileFinancialsResponse) UnsetLatestFinancials()`

UnsetLatestFinancials ensures that no value is present for LatestFinancials, not even an explicit nil
### GetSource

`func (o *PiMarketDataDomainModelsResponseProfileFinancialsResponse) GetSource() string`

GetSource returns the Source field if non-nil, zero value otherwise.

### GetSourceOk

`func (o *PiMarketDataDomainModelsResponseProfileFinancialsResponse) GetSourceOk() (*string, bool)`

GetSourceOk returns a tuple with the Source field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetSource

`func (o *PiMarketDataDomainModelsResponseProfileFinancialsResponse) SetSource(v string)`

SetSource sets Source field to given value.

### HasSource

`func (o *PiMarketDataDomainModelsResponseProfileFinancialsResponse) HasSource() bool`

HasSource returns a boolean if a field has been set.

### SetSourceNil

`func (o *PiMarketDataDomainModelsResponseProfileFinancialsResponse) SetSourceNil(b bool)`

 SetSourceNil sets the value for Source to be an explicit nil

### UnsetSource
`func (o *PiMarketDataDomainModelsResponseProfileFinancialsResponse) UnsetSource()`

UnsetSource ensures that no value is present for Source, not even an explicit nil

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


