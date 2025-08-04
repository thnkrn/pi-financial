# PlaceOrderRequest

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**AccountId** | **string** |  | 
**Venue** | **string** |  | 
**Symbol** | **string** |  | 
**OrderType** | [**OrderType**](OrderType.md) |  | 
**Side** | [**OrderSide**](OrderSide.md) |  | 
**Quantity** | **float32** |  | 
**LimitPrice** | Pointer to **NullableFloat32** |  | [optional] 
**StopPrice** | Pointer to **NullableFloat32** |  | [optional] 

## Methods

### NewPlaceOrderRequest

`func NewPlaceOrderRequest(accountId string, venue string, symbol string, orderType OrderType, side OrderSide, quantity float32, ) *PlaceOrderRequest`

NewPlaceOrderRequest instantiates a new PlaceOrderRequest object
This constructor will assign default values to properties that have it defined,
and makes sure properties required by API are set, but the set of arguments
will change when the set of required properties is changed

### NewPlaceOrderRequestWithDefaults

`func NewPlaceOrderRequestWithDefaults() *PlaceOrderRequest`

NewPlaceOrderRequestWithDefaults instantiates a new PlaceOrderRequest object
This constructor will only assign default values to properties that have it defined,
but it doesn't guarantee that properties required by API are set

### GetAccountId

`func (o *PlaceOrderRequest) GetAccountId() string`

GetAccountId returns the AccountId field if non-nil, zero value otherwise.

### GetAccountIdOk

`func (o *PlaceOrderRequest) GetAccountIdOk() (*string, bool)`

GetAccountIdOk returns a tuple with the AccountId field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetAccountId

`func (o *PlaceOrderRequest) SetAccountId(v string)`

SetAccountId sets AccountId field to given value.


### GetVenue

`func (o *PlaceOrderRequest) GetVenue() string`

GetVenue returns the Venue field if non-nil, zero value otherwise.

### GetVenueOk

`func (o *PlaceOrderRequest) GetVenueOk() (*string, bool)`

GetVenueOk returns a tuple with the Venue field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetVenue

`func (o *PlaceOrderRequest) SetVenue(v string)`

SetVenue sets Venue field to given value.


### GetSymbol

`func (o *PlaceOrderRequest) GetSymbol() string`

GetSymbol returns the Symbol field if non-nil, zero value otherwise.

### GetSymbolOk

`func (o *PlaceOrderRequest) GetSymbolOk() (*string, bool)`

GetSymbolOk returns a tuple with the Symbol field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetSymbol

`func (o *PlaceOrderRequest) SetSymbol(v string)`

SetSymbol sets Symbol field to given value.


### GetOrderType

`func (o *PlaceOrderRequest) GetOrderType() OrderType`

GetOrderType returns the OrderType field if non-nil, zero value otherwise.

### GetOrderTypeOk

`func (o *PlaceOrderRequest) GetOrderTypeOk() (*OrderType, bool)`

GetOrderTypeOk returns a tuple with the OrderType field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetOrderType

`func (o *PlaceOrderRequest) SetOrderType(v OrderType)`

SetOrderType sets OrderType field to given value.


### GetSide

`func (o *PlaceOrderRequest) GetSide() OrderSide`

GetSide returns the Side field if non-nil, zero value otherwise.

### GetSideOk

`func (o *PlaceOrderRequest) GetSideOk() (*OrderSide, bool)`

GetSideOk returns a tuple with the Side field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetSide

`func (o *PlaceOrderRequest) SetSide(v OrderSide)`

SetSide sets Side field to given value.


### GetQuantity

`func (o *PlaceOrderRequest) GetQuantity() float32`

GetQuantity returns the Quantity field if non-nil, zero value otherwise.

### GetQuantityOk

`func (o *PlaceOrderRequest) GetQuantityOk() (*float32, bool)`

GetQuantityOk returns a tuple with the Quantity field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetQuantity

`func (o *PlaceOrderRequest) SetQuantity(v float32)`

SetQuantity sets Quantity field to given value.


### GetLimitPrice

`func (o *PlaceOrderRequest) GetLimitPrice() float32`

GetLimitPrice returns the LimitPrice field if non-nil, zero value otherwise.

### GetLimitPriceOk

`func (o *PlaceOrderRequest) GetLimitPriceOk() (*float32, bool)`

GetLimitPriceOk returns a tuple with the LimitPrice field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetLimitPrice

`func (o *PlaceOrderRequest) SetLimitPrice(v float32)`

SetLimitPrice sets LimitPrice field to given value.

### HasLimitPrice

`func (o *PlaceOrderRequest) HasLimitPrice() bool`

HasLimitPrice returns a boolean if a field has been set.

### SetLimitPriceNil

`func (o *PlaceOrderRequest) SetLimitPriceNil(b bool)`

 SetLimitPriceNil sets the value for LimitPrice to be an explicit nil

### UnsetLimitPrice
`func (o *PlaceOrderRequest) UnsetLimitPrice()`

UnsetLimitPrice ensures that no value is present for LimitPrice, not even an explicit nil
### GetStopPrice

`func (o *PlaceOrderRequest) GetStopPrice() float32`

GetStopPrice returns the StopPrice field if non-nil, zero value otherwise.

### GetStopPriceOk

`func (o *PlaceOrderRequest) GetStopPriceOk() (*float32, bool)`

GetStopPriceOk returns a tuple with the StopPrice field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetStopPrice

`func (o *PlaceOrderRequest) SetStopPrice(v float32)`

SetStopPrice sets StopPrice field to given value.

### HasStopPrice

`func (o *PlaceOrderRequest) HasStopPrice() bool`

HasStopPrice returns a boolean if a field has been set.

### SetStopPriceNil

`func (o *PlaceOrderRequest) SetStopPriceNil(b bool)`

 SetStopPriceNil sets the value for StopPrice to be an explicit nil

### UnsetStopPrice
`func (o *PlaceOrderRequest) UnsetStopPrice()`

UnsetStopPrice ensures that no value is present for StopPrice, not even an explicit nil

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


