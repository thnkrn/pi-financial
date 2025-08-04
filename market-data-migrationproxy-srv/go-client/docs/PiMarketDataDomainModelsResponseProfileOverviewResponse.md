# PiMarketDataDomainModelsResponseProfileOverviewResponse

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**Market** | Pointer to **NullableString** |  | [optional] 
**Exchange** | Pointer to **NullableString** |  | [optional] 
**ExchangeTime** | Pointer to **NullableString** |  | [optional] 
**LastPrice** | Pointer to **NullableString** |  | [optional] 
**PriorClose** | Pointer to **NullableString** |  | [optional] 
**PriceChange** | Pointer to **NullableString** |  | [optional] 
**PriceChangePercentage** | Pointer to **NullableString** |  | [optional] 
**MinimumOrderSize** | Pointer to **NullableString** |  | [optional] 
**High52W** | Pointer to **NullableString** |  | [optional] 
**Low52W** | Pointer to **NullableString** |  | [optional] 
**ContractMonth** | Pointer to **NullableString** |  | [optional] 
**Currency** | Pointer to **NullableString** |  | [optional] 
**CorporateActions** | Pointer to [**[]PiMarketDataDomainModelsResponseCorporateActionResponse**](PiMarketDataDomainModelsResponseCorporateActionResponse.md) |  | [optional] 
**TradingSign** | Pointer to **[]string** |  | [optional] 

## Methods

### NewPiMarketDataDomainModelsResponseProfileOverviewResponse

`func NewPiMarketDataDomainModelsResponseProfileOverviewResponse() *PiMarketDataDomainModelsResponseProfileOverviewResponse`

NewPiMarketDataDomainModelsResponseProfileOverviewResponse instantiates a new PiMarketDataDomainModelsResponseProfileOverviewResponse object
This constructor will assign default values to properties that have it defined,
and makes sure properties required by API are set, but the set of arguments
will change when the set of required properties is changed

### NewPiMarketDataDomainModelsResponseProfileOverviewResponseWithDefaults

`func NewPiMarketDataDomainModelsResponseProfileOverviewResponseWithDefaults() *PiMarketDataDomainModelsResponseProfileOverviewResponse`

NewPiMarketDataDomainModelsResponseProfileOverviewResponseWithDefaults instantiates a new PiMarketDataDomainModelsResponseProfileOverviewResponse object
This constructor will only assign default values to properties that have it defined,
but it doesn't guarantee that properties required by API are set

### GetMarket

`func (o *PiMarketDataDomainModelsResponseProfileOverviewResponse) GetMarket() string`

GetMarket returns the Market field if non-nil, zero value otherwise.

### GetMarketOk

`func (o *PiMarketDataDomainModelsResponseProfileOverviewResponse) GetMarketOk() (*string, bool)`

GetMarketOk returns a tuple with the Market field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetMarket

`func (o *PiMarketDataDomainModelsResponseProfileOverviewResponse) SetMarket(v string)`

SetMarket sets Market field to given value.

### HasMarket

`func (o *PiMarketDataDomainModelsResponseProfileOverviewResponse) HasMarket() bool`

HasMarket returns a boolean if a field has been set.

### SetMarketNil

`func (o *PiMarketDataDomainModelsResponseProfileOverviewResponse) SetMarketNil(b bool)`

 SetMarketNil sets the value for Market to be an explicit nil

### UnsetMarket
`func (o *PiMarketDataDomainModelsResponseProfileOverviewResponse) UnsetMarket()`

UnsetMarket ensures that no value is present for Market, not even an explicit nil
### GetExchange

`func (o *PiMarketDataDomainModelsResponseProfileOverviewResponse) GetExchange() string`

GetExchange returns the Exchange field if non-nil, zero value otherwise.

### GetExchangeOk

`func (o *PiMarketDataDomainModelsResponseProfileOverviewResponse) GetExchangeOk() (*string, bool)`

GetExchangeOk returns a tuple with the Exchange field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetExchange

`func (o *PiMarketDataDomainModelsResponseProfileOverviewResponse) SetExchange(v string)`

SetExchange sets Exchange field to given value.

### HasExchange

`func (o *PiMarketDataDomainModelsResponseProfileOverviewResponse) HasExchange() bool`

HasExchange returns a boolean if a field has been set.

### SetExchangeNil

`func (o *PiMarketDataDomainModelsResponseProfileOverviewResponse) SetExchangeNil(b bool)`

 SetExchangeNil sets the value for Exchange to be an explicit nil

### UnsetExchange
`func (o *PiMarketDataDomainModelsResponseProfileOverviewResponse) UnsetExchange()`

UnsetExchange ensures that no value is present for Exchange, not even an explicit nil
### GetExchangeTime

`func (o *PiMarketDataDomainModelsResponseProfileOverviewResponse) GetExchangeTime() string`

GetExchangeTime returns the ExchangeTime field if non-nil, zero value otherwise.

### GetExchangeTimeOk

`func (o *PiMarketDataDomainModelsResponseProfileOverviewResponse) GetExchangeTimeOk() (*string, bool)`

GetExchangeTimeOk returns a tuple with the ExchangeTime field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetExchangeTime

`func (o *PiMarketDataDomainModelsResponseProfileOverviewResponse) SetExchangeTime(v string)`

SetExchangeTime sets ExchangeTime field to given value.

### HasExchangeTime

`func (o *PiMarketDataDomainModelsResponseProfileOverviewResponse) HasExchangeTime() bool`

HasExchangeTime returns a boolean if a field has been set.

### SetExchangeTimeNil

`func (o *PiMarketDataDomainModelsResponseProfileOverviewResponse) SetExchangeTimeNil(b bool)`

 SetExchangeTimeNil sets the value for ExchangeTime to be an explicit nil

### UnsetExchangeTime
`func (o *PiMarketDataDomainModelsResponseProfileOverviewResponse) UnsetExchangeTime()`

UnsetExchangeTime ensures that no value is present for ExchangeTime, not even an explicit nil
### GetLastPrice

`func (o *PiMarketDataDomainModelsResponseProfileOverviewResponse) GetLastPrice() string`

GetLastPrice returns the LastPrice field if non-nil, zero value otherwise.

### GetLastPriceOk

`func (o *PiMarketDataDomainModelsResponseProfileOverviewResponse) GetLastPriceOk() (*string, bool)`

GetLastPriceOk returns a tuple with the LastPrice field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetLastPrice

`func (o *PiMarketDataDomainModelsResponseProfileOverviewResponse) SetLastPrice(v string)`

SetLastPrice sets LastPrice field to given value.

### HasLastPrice

`func (o *PiMarketDataDomainModelsResponseProfileOverviewResponse) HasLastPrice() bool`

HasLastPrice returns a boolean if a field has been set.

### SetLastPriceNil

`func (o *PiMarketDataDomainModelsResponseProfileOverviewResponse) SetLastPriceNil(b bool)`

 SetLastPriceNil sets the value for LastPrice to be an explicit nil

### UnsetLastPrice
`func (o *PiMarketDataDomainModelsResponseProfileOverviewResponse) UnsetLastPrice()`

UnsetLastPrice ensures that no value is present for LastPrice, not even an explicit nil
### GetPriorClose

`func (o *PiMarketDataDomainModelsResponseProfileOverviewResponse) GetPriorClose() string`

GetPriorClose returns the PriorClose field if non-nil, zero value otherwise.

### GetPriorCloseOk

`func (o *PiMarketDataDomainModelsResponseProfileOverviewResponse) GetPriorCloseOk() (*string, bool)`

GetPriorCloseOk returns a tuple with the PriorClose field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetPriorClose

`func (o *PiMarketDataDomainModelsResponseProfileOverviewResponse) SetPriorClose(v string)`

SetPriorClose sets PriorClose field to given value.

### HasPriorClose

`func (o *PiMarketDataDomainModelsResponseProfileOverviewResponse) HasPriorClose() bool`

HasPriorClose returns a boolean if a field has been set.

### SetPriorCloseNil

`func (o *PiMarketDataDomainModelsResponseProfileOverviewResponse) SetPriorCloseNil(b bool)`

 SetPriorCloseNil sets the value for PriorClose to be an explicit nil

### UnsetPriorClose
`func (o *PiMarketDataDomainModelsResponseProfileOverviewResponse) UnsetPriorClose()`

UnsetPriorClose ensures that no value is present for PriorClose, not even an explicit nil
### GetPriceChange

`func (o *PiMarketDataDomainModelsResponseProfileOverviewResponse) GetPriceChange() string`

GetPriceChange returns the PriceChange field if non-nil, zero value otherwise.

### GetPriceChangeOk

`func (o *PiMarketDataDomainModelsResponseProfileOverviewResponse) GetPriceChangeOk() (*string, bool)`

GetPriceChangeOk returns a tuple with the PriceChange field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetPriceChange

`func (o *PiMarketDataDomainModelsResponseProfileOverviewResponse) SetPriceChange(v string)`

SetPriceChange sets PriceChange field to given value.

### HasPriceChange

`func (o *PiMarketDataDomainModelsResponseProfileOverviewResponse) HasPriceChange() bool`

HasPriceChange returns a boolean if a field has been set.

### SetPriceChangeNil

`func (o *PiMarketDataDomainModelsResponseProfileOverviewResponse) SetPriceChangeNil(b bool)`

 SetPriceChangeNil sets the value for PriceChange to be an explicit nil

### UnsetPriceChange
`func (o *PiMarketDataDomainModelsResponseProfileOverviewResponse) UnsetPriceChange()`

UnsetPriceChange ensures that no value is present for PriceChange, not even an explicit nil
### GetPriceChangePercentage

`func (o *PiMarketDataDomainModelsResponseProfileOverviewResponse) GetPriceChangePercentage() string`

GetPriceChangePercentage returns the PriceChangePercentage field if non-nil, zero value otherwise.

### GetPriceChangePercentageOk

`func (o *PiMarketDataDomainModelsResponseProfileOverviewResponse) GetPriceChangePercentageOk() (*string, bool)`

GetPriceChangePercentageOk returns a tuple with the PriceChangePercentage field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetPriceChangePercentage

`func (o *PiMarketDataDomainModelsResponseProfileOverviewResponse) SetPriceChangePercentage(v string)`

SetPriceChangePercentage sets PriceChangePercentage field to given value.

### HasPriceChangePercentage

`func (o *PiMarketDataDomainModelsResponseProfileOverviewResponse) HasPriceChangePercentage() bool`

HasPriceChangePercentage returns a boolean if a field has been set.

### SetPriceChangePercentageNil

`func (o *PiMarketDataDomainModelsResponseProfileOverviewResponse) SetPriceChangePercentageNil(b bool)`

 SetPriceChangePercentageNil sets the value for PriceChangePercentage to be an explicit nil

### UnsetPriceChangePercentage
`func (o *PiMarketDataDomainModelsResponseProfileOverviewResponse) UnsetPriceChangePercentage()`

UnsetPriceChangePercentage ensures that no value is present for PriceChangePercentage, not even an explicit nil
### GetMinimumOrderSize

`func (o *PiMarketDataDomainModelsResponseProfileOverviewResponse) GetMinimumOrderSize() string`

GetMinimumOrderSize returns the MinimumOrderSize field if non-nil, zero value otherwise.

### GetMinimumOrderSizeOk

`func (o *PiMarketDataDomainModelsResponseProfileOverviewResponse) GetMinimumOrderSizeOk() (*string, bool)`

GetMinimumOrderSizeOk returns a tuple with the MinimumOrderSize field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetMinimumOrderSize

`func (o *PiMarketDataDomainModelsResponseProfileOverviewResponse) SetMinimumOrderSize(v string)`

SetMinimumOrderSize sets MinimumOrderSize field to given value.

### HasMinimumOrderSize

`func (o *PiMarketDataDomainModelsResponseProfileOverviewResponse) HasMinimumOrderSize() bool`

HasMinimumOrderSize returns a boolean if a field has been set.

### SetMinimumOrderSizeNil

`func (o *PiMarketDataDomainModelsResponseProfileOverviewResponse) SetMinimumOrderSizeNil(b bool)`

 SetMinimumOrderSizeNil sets the value for MinimumOrderSize to be an explicit nil

### UnsetMinimumOrderSize
`func (o *PiMarketDataDomainModelsResponseProfileOverviewResponse) UnsetMinimumOrderSize()`

UnsetMinimumOrderSize ensures that no value is present for MinimumOrderSize, not even an explicit nil
### GetHigh52W

`func (o *PiMarketDataDomainModelsResponseProfileOverviewResponse) GetHigh52W() string`

GetHigh52W returns the High52W field if non-nil, zero value otherwise.

### GetHigh52WOk

`func (o *PiMarketDataDomainModelsResponseProfileOverviewResponse) GetHigh52WOk() (*string, bool)`

GetHigh52WOk returns a tuple with the High52W field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetHigh52W

`func (o *PiMarketDataDomainModelsResponseProfileOverviewResponse) SetHigh52W(v string)`

SetHigh52W sets High52W field to given value.

### HasHigh52W

`func (o *PiMarketDataDomainModelsResponseProfileOverviewResponse) HasHigh52W() bool`

HasHigh52W returns a boolean if a field has been set.

### SetHigh52WNil

`func (o *PiMarketDataDomainModelsResponseProfileOverviewResponse) SetHigh52WNil(b bool)`

 SetHigh52WNil sets the value for High52W to be an explicit nil

### UnsetHigh52W
`func (o *PiMarketDataDomainModelsResponseProfileOverviewResponse) UnsetHigh52W()`

UnsetHigh52W ensures that no value is present for High52W, not even an explicit nil
### GetLow52W

`func (o *PiMarketDataDomainModelsResponseProfileOverviewResponse) GetLow52W() string`

GetLow52W returns the Low52W field if non-nil, zero value otherwise.

### GetLow52WOk

`func (o *PiMarketDataDomainModelsResponseProfileOverviewResponse) GetLow52WOk() (*string, bool)`

GetLow52WOk returns a tuple with the Low52W field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetLow52W

`func (o *PiMarketDataDomainModelsResponseProfileOverviewResponse) SetLow52W(v string)`

SetLow52W sets Low52W field to given value.

### HasLow52W

`func (o *PiMarketDataDomainModelsResponseProfileOverviewResponse) HasLow52W() bool`

HasLow52W returns a boolean if a field has been set.

### SetLow52WNil

`func (o *PiMarketDataDomainModelsResponseProfileOverviewResponse) SetLow52WNil(b bool)`

 SetLow52WNil sets the value for Low52W to be an explicit nil

### UnsetLow52W
`func (o *PiMarketDataDomainModelsResponseProfileOverviewResponse) UnsetLow52W()`

UnsetLow52W ensures that no value is present for Low52W, not even an explicit nil
### GetContractMonth

`func (o *PiMarketDataDomainModelsResponseProfileOverviewResponse) GetContractMonth() string`

GetContractMonth returns the ContractMonth field if non-nil, zero value otherwise.

### GetContractMonthOk

`func (o *PiMarketDataDomainModelsResponseProfileOverviewResponse) GetContractMonthOk() (*string, bool)`

GetContractMonthOk returns a tuple with the ContractMonth field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetContractMonth

`func (o *PiMarketDataDomainModelsResponseProfileOverviewResponse) SetContractMonth(v string)`

SetContractMonth sets ContractMonth field to given value.

### HasContractMonth

`func (o *PiMarketDataDomainModelsResponseProfileOverviewResponse) HasContractMonth() bool`

HasContractMonth returns a boolean if a field has been set.

### SetContractMonthNil

`func (o *PiMarketDataDomainModelsResponseProfileOverviewResponse) SetContractMonthNil(b bool)`

 SetContractMonthNil sets the value for ContractMonth to be an explicit nil

### UnsetContractMonth
`func (o *PiMarketDataDomainModelsResponseProfileOverviewResponse) UnsetContractMonth()`

UnsetContractMonth ensures that no value is present for ContractMonth, not even an explicit nil
### GetCurrency

`func (o *PiMarketDataDomainModelsResponseProfileOverviewResponse) GetCurrency() string`

GetCurrency returns the Currency field if non-nil, zero value otherwise.

### GetCurrencyOk

`func (o *PiMarketDataDomainModelsResponseProfileOverviewResponse) GetCurrencyOk() (*string, bool)`

GetCurrencyOk returns a tuple with the Currency field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetCurrency

`func (o *PiMarketDataDomainModelsResponseProfileOverviewResponse) SetCurrency(v string)`

SetCurrency sets Currency field to given value.

### HasCurrency

`func (o *PiMarketDataDomainModelsResponseProfileOverviewResponse) HasCurrency() bool`

HasCurrency returns a boolean if a field has been set.

### SetCurrencyNil

`func (o *PiMarketDataDomainModelsResponseProfileOverviewResponse) SetCurrencyNil(b bool)`

 SetCurrencyNil sets the value for Currency to be an explicit nil

### UnsetCurrency
`func (o *PiMarketDataDomainModelsResponseProfileOverviewResponse) UnsetCurrency()`

UnsetCurrency ensures that no value is present for Currency, not even an explicit nil
### GetCorporateActions

`func (o *PiMarketDataDomainModelsResponseProfileOverviewResponse) GetCorporateActions() []PiMarketDataDomainModelsResponseCorporateActionResponse`

GetCorporateActions returns the CorporateActions field if non-nil, zero value otherwise.

### GetCorporateActionsOk

`func (o *PiMarketDataDomainModelsResponseProfileOverviewResponse) GetCorporateActionsOk() (*[]PiMarketDataDomainModelsResponseCorporateActionResponse, bool)`

GetCorporateActionsOk returns a tuple with the CorporateActions field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetCorporateActions

`func (o *PiMarketDataDomainModelsResponseProfileOverviewResponse) SetCorporateActions(v []PiMarketDataDomainModelsResponseCorporateActionResponse)`

SetCorporateActions sets CorporateActions field to given value.

### HasCorporateActions

`func (o *PiMarketDataDomainModelsResponseProfileOverviewResponse) HasCorporateActions() bool`

HasCorporateActions returns a boolean if a field has been set.

### SetCorporateActionsNil

`func (o *PiMarketDataDomainModelsResponseProfileOverviewResponse) SetCorporateActionsNil(b bool)`

 SetCorporateActionsNil sets the value for CorporateActions to be an explicit nil

### UnsetCorporateActions
`func (o *PiMarketDataDomainModelsResponseProfileOverviewResponse) UnsetCorporateActions()`

UnsetCorporateActions ensures that no value is present for CorporateActions, not even an explicit nil
### GetTradingSign

`func (o *PiMarketDataDomainModelsResponseProfileOverviewResponse) GetTradingSign() []string`

GetTradingSign returns the TradingSign field if non-nil, zero value otherwise.

### GetTradingSignOk

`func (o *PiMarketDataDomainModelsResponseProfileOverviewResponse) GetTradingSignOk() (*[]string, bool)`

GetTradingSignOk returns a tuple with the TradingSign field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetTradingSign

`func (o *PiMarketDataDomainModelsResponseProfileOverviewResponse) SetTradingSign(v []string)`

SetTradingSign sets TradingSign field to given value.

### HasTradingSign

`func (o *PiMarketDataDomainModelsResponseProfileOverviewResponse) HasTradingSign() bool`

HasTradingSign returns a boolean if a field has been set.

### SetTradingSignNil

`func (o *PiMarketDataDomainModelsResponseProfileOverviewResponse) SetTradingSignNil(b bool)`

 SetTradingSignNil sets the value for TradingSign to be an explicit nil

### UnsetTradingSign
`func (o *PiMarketDataDomainModelsResponseProfileOverviewResponse) UnsetTradingSign()`

UnsetTradingSign ensures that no value is present for TradingSign, not even an explicit nil

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


