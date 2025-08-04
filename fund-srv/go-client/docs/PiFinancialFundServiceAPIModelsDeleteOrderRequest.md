# PiFinancialFundServiceAPIModelsDeleteOrderRequest

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**BrokerOrderId** | **NullableString** |  | 
**OrderSide** | **string** |  | 
**TradingAccountNo** | **string** |  | 
**Force** | Pointer to **NullableBool** |  | [optional] 

## Methods

### NewPiFinancialFundServiceAPIModelsDeleteOrderRequest

`func NewPiFinancialFundServiceAPIModelsDeleteOrderRequest(brokerOrderId NullableString, orderSide string, tradingAccountNo string, ) *PiFinancialFundServiceAPIModelsDeleteOrderRequest`

NewPiFinancialFundServiceAPIModelsDeleteOrderRequest instantiates a new PiFinancialFundServiceAPIModelsDeleteOrderRequest object
This constructor will assign default values to properties that have it defined,
and makes sure properties required by API are set, but the set of arguments
will change when the set of required properties is changed

### NewPiFinancialFundServiceAPIModelsDeleteOrderRequestWithDefaults

`func NewPiFinancialFundServiceAPIModelsDeleteOrderRequestWithDefaults() *PiFinancialFundServiceAPIModelsDeleteOrderRequest`

NewPiFinancialFundServiceAPIModelsDeleteOrderRequestWithDefaults instantiates a new PiFinancialFundServiceAPIModelsDeleteOrderRequest object
This constructor will only assign default values to properties that have it defined,
but it doesn't guarantee that properties required by API are set

### GetBrokerOrderId

`func (o *PiFinancialFundServiceAPIModelsDeleteOrderRequest) GetBrokerOrderId() string`

GetBrokerOrderId returns the BrokerOrderId field if non-nil, zero value otherwise.

### GetBrokerOrderIdOk

`func (o *PiFinancialFundServiceAPIModelsDeleteOrderRequest) GetBrokerOrderIdOk() (*string, bool)`

GetBrokerOrderIdOk returns a tuple with the BrokerOrderId field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetBrokerOrderId

`func (o *PiFinancialFundServiceAPIModelsDeleteOrderRequest) SetBrokerOrderId(v string)`

SetBrokerOrderId sets BrokerOrderId field to given value.


### SetBrokerOrderIdNil

`func (o *PiFinancialFundServiceAPIModelsDeleteOrderRequest) SetBrokerOrderIdNil(b bool)`

 SetBrokerOrderIdNil sets the value for BrokerOrderId to be an explicit nil

### UnsetBrokerOrderId
`func (o *PiFinancialFundServiceAPIModelsDeleteOrderRequest) UnsetBrokerOrderId()`

UnsetBrokerOrderId ensures that no value is present for BrokerOrderId, not even an explicit nil
### GetOrderSide

`func (o *PiFinancialFundServiceAPIModelsDeleteOrderRequest) GetOrderSide() string`

GetOrderSide returns the OrderSide field if non-nil, zero value otherwise.

### GetOrderSideOk

`func (o *PiFinancialFundServiceAPIModelsDeleteOrderRequest) GetOrderSideOk() (*string, bool)`

GetOrderSideOk returns a tuple with the OrderSide field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetOrderSide

`func (o *PiFinancialFundServiceAPIModelsDeleteOrderRequest) SetOrderSide(v string)`

SetOrderSide sets OrderSide field to given value.


### GetTradingAccountNo

`func (o *PiFinancialFundServiceAPIModelsDeleteOrderRequest) GetTradingAccountNo() string`

GetTradingAccountNo returns the TradingAccountNo field if non-nil, zero value otherwise.

### GetTradingAccountNoOk

`func (o *PiFinancialFundServiceAPIModelsDeleteOrderRequest) GetTradingAccountNoOk() (*string, bool)`

GetTradingAccountNoOk returns a tuple with the TradingAccountNo field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetTradingAccountNo

`func (o *PiFinancialFundServiceAPIModelsDeleteOrderRequest) SetTradingAccountNo(v string)`

SetTradingAccountNo sets TradingAccountNo field to given value.


### GetForce

`func (o *PiFinancialFundServiceAPIModelsDeleteOrderRequest) GetForce() bool`

GetForce returns the Force field if non-nil, zero value otherwise.

### GetForceOk

`func (o *PiFinancialFundServiceAPIModelsDeleteOrderRequest) GetForceOk() (*bool, bool)`

GetForceOk returns a tuple with the Force field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetForce

`func (o *PiFinancialFundServiceAPIModelsDeleteOrderRequest) SetForce(v bool)`

SetForce sets Force field to given value.

### HasForce

`func (o *PiFinancialFundServiceAPIModelsDeleteOrderRequest) HasForce() bool`

HasForce returns a boolean if a field has been set.

### SetForceNil

`func (o *PiFinancialFundServiceAPIModelsDeleteOrderRequest) SetForceNil(b bool)`

 SetForceNil sets the value for Force to be an explicit nil

### UnsetForce
`func (o *PiFinancialFundServiceAPIModelsDeleteOrderRequest) UnsetForce()`

UnsetForce ensures that no value is present for Force, not even an explicit nil

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


