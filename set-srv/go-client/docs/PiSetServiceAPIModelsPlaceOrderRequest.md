# PiSetServiceAPIModelsPlaceOrderRequest

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**TradingAccountNo** | **string** |  | 
**OrderType** | [**PiSetServiceDomainAggregatesModelFinancialAssetAggregateConditionPrice**](PiSetServiceDomainAggregatesModelFinancialAssetAggregateConditionPrice.md) |  | 
**Side** | [**PiSetServiceDomainAggregatesModelFinancialAssetAggregateOrderAction**](PiSetServiceDomainAggregatesModelFinancialAssetAggregateOrderAction.md) |  | 
**Symbol** | **string** |  | 
**Quantity** | **string** |  | 
**Price** | Pointer to **NullableFloat64** |  | [optional] 
**Tif** | Pointer to [**PiSetServiceDomainAggregatesModelFinancialAssetAggregateCondition**](PiSetServiceDomainAggregatesModelFinancialAssetAggregateCondition.md) |  | [optional] 
**Flag** | Pointer to [**[]PiSetServiceAPIModelsOrderFlag**](PiSetServiceAPIModelsOrderFlag.md) |  | [optional] 

## Methods

### NewPiSetServiceAPIModelsPlaceOrderRequest

`func NewPiSetServiceAPIModelsPlaceOrderRequest(tradingAccountNo string, orderType PiSetServiceDomainAggregatesModelFinancialAssetAggregateConditionPrice, side PiSetServiceDomainAggregatesModelFinancialAssetAggregateOrderAction, symbol string, quantity string, ) *PiSetServiceAPIModelsPlaceOrderRequest`

NewPiSetServiceAPIModelsPlaceOrderRequest instantiates a new PiSetServiceAPIModelsPlaceOrderRequest object
This constructor will assign default values to properties that have it defined,
and makes sure properties required by API are set, but the set of arguments
will change when the set of required properties is changed

### NewPiSetServiceAPIModelsPlaceOrderRequestWithDefaults

`func NewPiSetServiceAPIModelsPlaceOrderRequestWithDefaults() *PiSetServiceAPIModelsPlaceOrderRequest`

NewPiSetServiceAPIModelsPlaceOrderRequestWithDefaults instantiates a new PiSetServiceAPIModelsPlaceOrderRequest object
This constructor will only assign default values to properties that have it defined,
but it doesn't guarantee that properties required by API are set

### GetTradingAccountNo

`func (o *PiSetServiceAPIModelsPlaceOrderRequest) GetTradingAccountNo() string`

GetTradingAccountNo returns the TradingAccountNo field if non-nil, zero value otherwise.

### GetTradingAccountNoOk

`func (o *PiSetServiceAPIModelsPlaceOrderRequest) GetTradingAccountNoOk() (*string, bool)`

GetTradingAccountNoOk returns a tuple with the TradingAccountNo field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetTradingAccountNo

`func (o *PiSetServiceAPIModelsPlaceOrderRequest) SetTradingAccountNo(v string)`

SetTradingAccountNo sets TradingAccountNo field to given value.


### GetOrderType

`func (o *PiSetServiceAPIModelsPlaceOrderRequest) GetOrderType() PiSetServiceDomainAggregatesModelFinancialAssetAggregateConditionPrice`

GetOrderType returns the OrderType field if non-nil, zero value otherwise.

### GetOrderTypeOk

`func (o *PiSetServiceAPIModelsPlaceOrderRequest) GetOrderTypeOk() (*PiSetServiceDomainAggregatesModelFinancialAssetAggregateConditionPrice, bool)`

GetOrderTypeOk returns a tuple with the OrderType field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetOrderType

`func (o *PiSetServiceAPIModelsPlaceOrderRequest) SetOrderType(v PiSetServiceDomainAggregatesModelFinancialAssetAggregateConditionPrice)`

SetOrderType sets OrderType field to given value.


### GetSide

`func (o *PiSetServiceAPIModelsPlaceOrderRequest) GetSide() PiSetServiceDomainAggregatesModelFinancialAssetAggregateOrderAction`

GetSide returns the Side field if non-nil, zero value otherwise.

### GetSideOk

`func (o *PiSetServiceAPIModelsPlaceOrderRequest) GetSideOk() (*PiSetServiceDomainAggregatesModelFinancialAssetAggregateOrderAction, bool)`

GetSideOk returns a tuple with the Side field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetSide

`func (o *PiSetServiceAPIModelsPlaceOrderRequest) SetSide(v PiSetServiceDomainAggregatesModelFinancialAssetAggregateOrderAction)`

SetSide sets Side field to given value.


### GetSymbol

`func (o *PiSetServiceAPIModelsPlaceOrderRequest) GetSymbol() string`

GetSymbol returns the Symbol field if non-nil, zero value otherwise.

### GetSymbolOk

`func (o *PiSetServiceAPIModelsPlaceOrderRequest) GetSymbolOk() (*string, bool)`

GetSymbolOk returns a tuple with the Symbol field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetSymbol

`func (o *PiSetServiceAPIModelsPlaceOrderRequest) SetSymbol(v string)`

SetSymbol sets Symbol field to given value.


### GetQuantity

`func (o *PiSetServiceAPIModelsPlaceOrderRequest) GetQuantity() string`

GetQuantity returns the Quantity field if non-nil, zero value otherwise.

### GetQuantityOk

`func (o *PiSetServiceAPIModelsPlaceOrderRequest) GetQuantityOk() (*string, bool)`

GetQuantityOk returns a tuple with the Quantity field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetQuantity

`func (o *PiSetServiceAPIModelsPlaceOrderRequest) SetQuantity(v string)`

SetQuantity sets Quantity field to given value.


### GetPrice

`func (o *PiSetServiceAPIModelsPlaceOrderRequest) GetPrice() float64`

GetPrice returns the Price field if non-nil, zero value otherwise.

### GetPriceOk

`func (o *PiSetServiceAPIModelsPlaceOrderRequest) GetPriceOk() (*float64, bool)`

GetPriceOk returns a tuple with the Price field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetPrice

`func (o *PiSetServiceAPIModelsPlaceOrderRequest) SetPrice(v float64)`

SetPrice sets Price field to given value.

### HasPrice

`func (o *PiSetServiceAPIModelsPlaceOrderRequest) HasPrice() bool`

HasPrice returns a boolean if a field has been set.

### SetPriceNil

`func (o *PiSetServiceAPIModelsPlaceOrderRequest) SetPriceNil(b bool)`

 SetPriceNil sets the value for Price to be an explicit nil

### UnsetPrice
`func (o *PiSetServiceAPIModelsPlaceOrderRequest) UnsetPrice()`

UnsetPrice ensures that no value is present for Price, not even an explicit nil
### GetTif

`func (o *PiSetServiceAPIModelsPlaceOrderRequest) GetTif() PiSetServiceDomainAggregatesModelFinancialAssetAggregateCondition`

GetTif returns the Tif field if non-nil, zero value otherwise.

### GetTifOk

`func (o *PiSetServiceAPIModelsPlaceOrderRequest) GetTifOk() (*PiSetServiceDomainAggregatesModelFinancialAssetAggregateCondition, bool)`

GetTifOk returns a tuple with the Tif field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetTif

`func (o *PiSetServiceAPIModelsPlaceOrderRequest) SetTif(v PiSetServiceDomainAggregatesModelFinancialAssetAggregateCondition)`

SetTif sets Tif field to given value.

### HasTif

`func (o *PiSetServiceAPIModelsPlaceOrderRequest) HasTif() bool`

HasTif returns a boolean if a field has been set.

### GetFlag

`func (o *PiSetServiceAPIModelsPlaceOrderRequest) GetFlag() []PiSetServiceAPIModelsOrderFlag`

GetFlag returns the Flag field if non-nil, zero value otherwise.

### GetFlagOk

`func (o *PiSetServiceAPIModelsPlaceOrderRequest) GetFlagOk() (*[]PiSetServiceAPIModelsOrderFlag, bool)`

GetFlagOk returns a tuple with the Flag field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetFlag

`func (o *PiSetServiceAPIModelsPlaceOrderRequest) SetFlag(v []PiSetServiceAPIModelsOrderFlag)`

SetFlag sets Flag field to given value.

### HasFlag

`func (o *PiSetServiceAPIModelsPlaceOrderRequest) HasFlag() bool`

HasFlag returns a boolean if a field has been set.

### SetFlagNil

`func (o *PiSetServiceAPIModelsPlaceOrderRequest) SetFlagNil(b bool)`

 SetFlagNil sets the value for Flag to be an explicit nil

### UnsetFlag
`func (o *PiSetServiceAPIModelsPlaceOrderRequest) UnsetFlag()`

UnsetFlag ensures that no value is present for Flag, not even an explicit nil

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


