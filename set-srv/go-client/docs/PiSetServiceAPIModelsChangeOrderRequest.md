# PiSetServiceAPIModelsChangeOrderRequest

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**TradingAccountNo** | **string** |  | 
**OrderId** | **string** |  | 
**Price** | **float64** |  | 
**Volume** | **string** |  | 
**Flag** | Pointer to [**[]PiSetServiceAPIModelsOrderFlag**](PiSetServiceAPIModelsOrderFlag.md) |  | [optional] 

## Methods

### NewPiSetServiceAPIModelsChangeOrderRequest

`func NewPiSetServiceAPIModelsChangeOrderRequest(tradingAccountNo string, orderId string, price float64, volume string, ) *PiSetServiceAPIModelsChangeOrderRequest`

NewPiSetServiceAPIModelsChangeOrderRequest instantiates a new PiSetServiceAPIModelsChangeOrderRequest object
This constructor will assign default values to properties that have it defined,
and makes sure properties required by API are set, but the set of arguments
will change when the set of required properties is changed

### NewPiSetServiceAPIModelsChangeOrderRequestWithDefaults

`func NewPiSetServiceAPIModelsChangeOrderRequestWithDefaults() *PiSetServiceAPIModelsChangeOrderRequest`

NewPiSetServiceAPIModelsChangeOrderRequestWithDefaults instantiates a new PiSetServiceAPIModelsChangeOrderRequest object
This constructor will only assign default values to properties that have it defined,
but it doesn't guarantee that properties required by API are set

### GetTradingAccountNo

`func (o *PiSetServiceAPIModelsChangeOrderRequest) GetTradingAccountNo() string`

GetTradingAccountNo returns the TradingAccountNo field if non-nil, zero value otherwise.

### GetTradingAccountNoOk

`func (o *PiSetServiceAPIModelsChangeOrderRequest) GetTradingAccountNoOk() (*string, bool)`

GetTradingAccountNoOk returns a tuple with the TradingAccountNo field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetTradingAccountNo

`func (o *PiSetServiceAPIModelsChangeOrderRequest) SetTradingAccountNo(v string)`

SetTradingAccountNo sets TradingAccountNo field to given value.


### GetOrderId

`func (o *PiSetServiceAPIModelsChangeOrderRequest) GetOrderId() string`

GetOrderId returns the OrderId field if non-nil, zero value otherwise.

### GetOrderIdOk

`func (o *PiSetServiceAPIModelsChangeOrderRequest) GetOrderIdOk() (*string, bool)`

GetOrderIdOk returns a tuple with the OrderId field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetOrderId

`func (o *PiSetServiceAPIModelsChangeOrderRequest) SetOrderId(v string)`

SetOrderId sets OrderId field to given value.


### GetPrice

`func (o *PiSetServiceAPIModelsChangeOrderRequest) GetPrice() float64`

GetPrice returns the Price field if non-nil, zero value otherwise.

### GetPriceOk

`func (o *PiSetServiceAPIModelsChangeOrderRequest) GetPriceOk() (*float64, bool)`

GetPriceOk returns a tuple with the Price field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetPrice

`func (o *PiSetServiceAPIModelsChangeOrderRequest) SetPrice(v float64)`

SetPrice sets Price field to given value.


### GetVolume

`func (o *PiSetServiceAPIModelsChangeOrderRequest) GetVolume() string`

GetVolume returns the Volume field if non-nil, zero value otherwise.

### GetVolumeOk

`func (o *PiSetServiceAPIModelsChangeOrderRequest) GetVolumeOk() (*string, bool)`

GetVolumeOk returns a tuple with the Volume field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetVolume

`func (o *PiSetServiceAPIModelsChangeOrderRequest) SetVolume(v string)`

SetVolume sets Volume field to given value.


### GetFlag

`func (o *PiSetServiceAPIModelsChangeOrderRequest) GetFlag() []PiSetServiceAPIModelsOrderFlag`

GetFlag returns the Flag field if non-nil, zero value otherwise.

### GetFlagOk

`func (o *PiSetServiceAPIModelsChangeOrderRequest) GetFlagOk() (*[]PiSetServiceAPIModelsOrderFlag, bool)`

GetFlagOk returns a tuple with the Flag field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetFlag

`func (o *PiSetServiceAPIModelsChangeOrderRequest) SetFlag(v []PiSetServiceAPIModelsOrderFlag)`

SetFlag sets Flag field to given value.

### HasFlag

`func (o *PiSetServiceAPIModelsChangeOrderRequest) HasFlag() bool`

HasFlag returns a boolean if a field has been set.

### SetFlagNil

`func (o *PiSetServiceAPIModelsChangeOrderRequest) SetFlagNil(b bool)`

 SetFlagNil sets the value for Flag to be an explicit nil

### UnsetFlag
`func (o *PiSetServiceAPIModelsChangeOrderRequest) UnsetFlag()`

UnsetFlag ensures that no value is present for Flag, not even an explicit nil

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


