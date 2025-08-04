# PositionResponse

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**Symbol** | Pointer to **NullableString** |  | [optional] 
**Venue** | Pointer to **NullableString** |  | [optional] 
**Currency** | Pointer to [**Currency**](Currency.md) |  | [optional] 
**EntireQuantity** | Pointer to **float32** |  | [optional] 
**AvailableQuantity** | Pointer to **float32** |  | [optional] 
**LastPrice** | Pointer to **float32** |  | [optional] 
**MarketValue** | Pointer to **float32** |  | [optional] 
**AveragePrice** | Pointer to **float32** |  | [optional] 
**Upnl** | Pointer to **float32** |  | [optional] 
**Cost** | Pointer to **float32** |  | [optional] 
**UpnlPercentage** | Pointer to **float32** |  | [optional] 
**Logo** | Pointer to **NullableString** |  | [optional] [readonly] 

## Methods

### NewPositionResponse

`func NewPositionResponse() *PositionResponse`

NewPositionResponse instantiates a new PositionResponse object
This constructor will assign default values to properties that have it defined,
and makes sure properties required by API are set, but the set of arguments
will change when the set of required properties is changed

### NewPositionResponseWithDefaults

`func NewPositionResponseWithDefaults() *PositionResponse`

NewPositionResponseWithDefaults instantiates a new PositionResponse object
This constructor will only assign default values to properties that have it defined,
but it doesn't guarantee that properties required by API are set

### GetSymbol

`func (o *PositionResponse) GetSymbol() string`

GetSymbol returns the Symbol field if non-nil, zero value otherwise.

### GetSymbolOk

`func (o *PositionResponse) GetSymbolOk() (*string, bool)`

GetSymbolOk returns a tuple with the Symbol field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetSymbol

`func (o *PositionResponse) SetSymbol(v string)`

SetSymbol sets Symbol field to given value.

### HasSymbol

`func (o *PositionResponse) HasSymbol() bool`

HasSymbol returns a boolean if a field has been set.

### SetSymbolNil

`func (o *PositionResponse) SetSymbolNil(b bool)`

 SetSymbolNil sets the value for Symbol to be an explicit nil

### UnsetSymbol
`func (o *PositionResponse) UnsetSymbol()`

UnsetSymbol ensures that no value is present for Symbol, not even an explicit nil
### GetVenue

`func (o *PositionResponse) GetVenue() string`

GetVenue returns the Venue field if non-nil, zero value otherwise.

### GetVenueOk

`func (o *PositionResponse) GetVenueOk() (*string, bool)`

GetVenueOk returns a tuple with the Venue field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetVenue

`func (o *PositionResponse) SetVenue(v string)`

SetVenue sets Venue field to given value.

### HasVenue

`func (o *PositionResponse) HasVenue() bool`

HasVenue returns a boolean if a field has been set.

### SetVenueNil

`func (o *PositionResponse) SetVenueNil(b bool)`

 SetVenueNil sets the value for Venue to be an explicit nil

### UnsetVenue
`func (o *PositionResponse) UnsetVenue()`

UnsetVenue ensures that no value is present for Venue, not even an explicit nil
### GetCurrency

`func (o *PositionResponse) GetCurrency() Currency`

GetCurrency returns the Currency field if non-nil, zero value otherwise.

### GetCurrencyOk

`func (o *PositionResponse) GetCurrencyOk() (*Currency, bool)`

GetCurrencyOk returns a tuple with the Currency field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetCurrency

`func (o *PositionResponse) SetCurrency(v Currency)`

SetCurrency sets Currency field to given value.

### HasCurrency

`func (o *PositionResponse) HasCurrency() bool`

HasCurrency returns a boolean if a field has been set.

### GetEntireQuantity

`func (o *PositionResponse) GetEntireQuantity() float32`

GetEntireQuantity returns the EntireQuantity field if non-nil, zero value otherwise.

### GetEntireQuantityOk

`func (o *PositionResponse) GetEntireQuantityOk() (*float32, bool)`

GetEntireQuantityOk returns a tuple with the EntireQuantity field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetEntireQuantity

`func (o *PositionResponse) SetEntireQuantity(v float32)`

SetEntireQuantity sets EntireQuantity field to given value.

### HasEntireQuantity

`func (o *PositionResponse) HasEntireQuantity() bool`

HasEntireQuantity returns a boolean if a field has been set.

### GetAvailableQuantity

`func (o *PositionResponse) GetAvailableQuantity() float32`

GetAvailableQuantity returns the AvailableQuantity field if non-nil, zero value otherwise.

### GetAvailableQuantityOk

`func (o *PositionResponse) GetAvailableQuantityOk() (*float32, bool)`

GetAvailableQuantityOk returns a tuple with the AvailableQuantity field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetAvailableQuantity

`func (o *PositionResponse) SetAvailableQuantity(v float32)`

SetAvailableQuantity sets AvailableQuantity field to given value.

### HasAvailableQuantity

`func (o *PositionResponse) HasAvailableQuantity() bool`

HasAvailableQuantity returns a boolean if a field has been set.

### GetLastPrice

`func (o *PositionResponse) GetLastPrice() float32`

GetLastPrice returns the LastPrice field if non-nil, zero value otherwise.

### GetLastPriceOk

`func (o *PositionResponse) GetLastPriceOk() (*float32, bool)`

GetLastPriceOk returns a tuple with the LastPrice field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetLastPrice

`func (o *PositionResponse) SetLastPrice(v float32)`

SetLastPrice sets LastPrice field to given value.

### HasLastPrice

`func (o *PositionResponse) HasLastPrice() bool`

HasLastPrice returns a boolean if a field has been set.

### GetMarketValue

`func (o *PositionResponse) GetMarketValue() float32`

GetMarketValue returns the MarketValue field if non-nil, zero value otherwise.

### GetMarketValueOk

`func (o *PositionResponse) GetMarketValueOk() (*float32, bool)`

GetMarketValueOk returns a tuple with the MarketValue field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetMarketValue

`func (o *PositionResponse) SetMarketValue(v float32)`

SetMarketValue sets MarketValue field to given value.

### HasMarketValue

`func (o *PositionResponse) HasMarketValue() bool`

HasMarketValue returns a boolean if a field has been set.

### GetAveragePrice

`func (o *PositionResponse) GetAveragePrice() float32`

GetAveragePrice returns the AveragePrice field if non-nil, zero value otherwise.

### GetAveragePriceOk

`func (o *PositionResponse) GetAveragePriceOk() (*float32, bool)`

GetAveragePriceOk returns a tuple with the AveragePrice field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetAveragePrice

`func (o *PositionResponse) SetAveragePrice(v float32)`

SetAveragePrice sets AveragePrice field to given value.

### HasAveragePrice

`func (o *PositionResponse) HasAveragePrice() bool`

HasAveragePrice returns a boolean if a field has been set.

### GetUpnl

`func (o *PositionResponse) GetUpnl() float32`

GetUpnl returns the Upnl field if non-nil, zero value otherwise.

### GetUpnlOk

`func (o *PositionResponse) GetUpnlOk() (*float32, bool)`

GetUpnlOk returns a tuple with the Upnl field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetUpnl

`func (o *PositionResponse) SetUpnl(v float32)`

SetUpnl sets Upnl field to given value.

### HasUpnl

`func (o *PositionResponse) HasUpnl() bool`

HasUpnl returns a boolean if a field has been set.

### GetCost

`func (o *PositionResponse) GetCost() float32`

GetCost returns the Cost field if non-nil, zero value otherwise.

### GetCostOk

`func (o *PositionResponse) GetCostOk() (*float32, bool)`

GetCostOk returns a tuple with the Cost field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetCost

`func (o *PositionResponse) SetCost(v float32)`

SetCost sets Cost field to given value.

### HasCost

`func (o *PositionResponse) HasCost() bool`

HasCost returns a boolean if a field has been set.

### GetUpnlPercentage

`func (o *PositionResponse) GetUpnlPercentage() float32`

GetUpnlPercentage returns the UpnlPercentage field if non-nil, zero value otherwise.

### GetUpnlPercentageOk

`func (o *PositionResponse) GetUpnlPercentageOk() (*float32, bool)`

GetUpnlPercentageOk returns a tuple with the UpnlPercentage field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetUpnlPercentage

`func (o *PositionResponse) SetUpnlPercentage(v float32)`

SetUpnlPercentage sets UpnlPercentage field to given value.

### HasUpnlPercentage

`func (o *PositionResponse) HasUpnlPercentage() bool`

HasUpnlPercentage returns a boolean if a field has been set.

### GetLogo

`func (o *PositionResponse) GetLogo() string`

GetLogo returns the Logo field if non-nil, zero value otherwise.

### GetLogoOk

`func (o *PositionResponse) GetLogoOk() (*string, bool)`

GetLogoOk returns a tuple with the Logo field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetLogo

`func (o *PositionResponse) SetLogo(v string)`

SetLogo sets Logo field to given value.

### HasLogo

`func (o *PositionResponse) HasLogo() bool`

HasLogo returns a boolean if a field has been set.

### SetLogoNil

`func (o *PositionResponse) SetLogoNil(b bool)`

 SetLogoNil sets the value for Logo to be an explicit nil

### UnsetLogo
`func (o *PositionResponse) UnsetLogo()`

UnsetLogo ensures that no value is present for Logo, not even an explicit nil

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


