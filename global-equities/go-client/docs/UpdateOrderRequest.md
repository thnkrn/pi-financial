# UpdateOrderRequest

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**Quantity** | Pointer to **float32** |  | [optional] 
**LimitPrice** | Pointer to **NullableFloat32** |  | [optional] 
**StopPrice** | Pointer to **NullableFloat32** |  | [optional] 

## Methods

### NewUpdateOrderRequest

`func NewUpdateOrderRequest() *UpdateOrderRequest`

NewUpdateOrderRequest instantiates a new UpdateOrderRequest object
This constructor will assign default values to properties that have it defined,
and makes sure properties required by API are set, but the set of arguments
will change when the set of required properties is changed

### NewUpdateOrderRequestWithDefaults

`func NewUpdateOrderRequestWithDefaults() *UpdateOrderRequest`

NewUpdateOrderRequestWithDefaults instantiates a new UpdateOrderRequest object
This constructor will only assign default values to properties that have it defined,
but it doesn't guarantee that properties required by API are set

### GetQuantity

`func (o *UpdateOrderRequest) GetQuantity() float32`

GetQuantity returns the Quantity field if non-nil, zero value otherwise.

### GetQuantityOk

`func (o *UpdateOrderRequest) GetQuantityOk() (*float32, bool)`

GetQuantityOk returns a tuple with the Quantity field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetQuantity

`func (o *UpdateOrderRequest) SetQuantity(v float32)`

SetQuantity sets Quantity field to given value.

### HasQuantity

`func (o *UpdateOrderRequest) HasQuantity() bool`

HasQuantity returns a boolean if a field has been set.

### GetLimitPrice

`func (o *UpdateOrderRequest) GetLimitPrice() float32`

GetLimitPrice returns the LimitPrice field if non-nil, zero value otherwise.

### GetLimitPriceOk

`func (o *UpdateOrderRequest) GetLimitPriceOk() (*float32, bool)`

GetLimitPriceOk returns a tuple with the LimitPrice field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetLimitPrice

`func (o *UpdateOrderRequest) SetLimitPrice(v float32)`

SetLimitPrice sets LimitPrice field to given value.

### HasLimitPrice

`func (o *UpdateOrderRequest) HasLimitPrice() bool`

HasLimitPrice returns a boolean if a field has been set.

### SetLimitPriceNil

`func (o *UpdateOrderRequest) SetLimitPriceNil(b bool)`

 SetLimitPriceNil sets the value for LimitPrice to be an explicit nil

### UnsetLimitPrice
`func (o *UpdateOrderRequest) UnsetLimitPrice()`

UnsetLimitPrice ensures that no value is present for LimitPrice, not even an explicit nil
### GetStopPrice

`func (o *UpdateOrderRequest) GetStopPrice() float32`

GetStopPrice returns the StopPrice field if non-nil, zero value otherwise.

### GetStopPriceOk

`func (o *UpdateOrderRequest) GetStopPriceOk() (*float32, bool)`

GetStopPriceOk returns a tuple with the StopPrice field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetStopPrice

`func (o *UpdateOrderRequest) SetStopPrice(v float32)`

SetStopPrice sets StopPrice field to given value.

### HasStopPrice

`func (o *UpdateOrderRequest) HasStopPrice() bool`

HasStopPrice returns a boolean if a field has been set.

### SetStopPriceNil

`func (o *UpdateOrderRequest) SetStopPriceNil(b bool)`

 SetStopPriceNil sets the value for StopPrice to be an explicit nil

### UnsetStopPrice
`func (o *UpdateOrderRequest) UnsetStopPrice()`

UnsetStopPrice ensures that no value is present for StopPrice, not even an explicit nil

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


