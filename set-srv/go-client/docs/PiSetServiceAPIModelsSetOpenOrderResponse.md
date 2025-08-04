# PiSetServiceAPIModelsSetOpenOrderResponse

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**OrderId** | **int64** |  | 
**CustCode** | **NullableString** |  | 
**Side** | [**PiSetServiceDomainAggregatesModelFinancialAssetAggregateOrderAction**](PiSetServiceDomainAggregatesModelFinancialAssetAggregateOrderAction.md) |  | 
**Status** | **NullableString** |  | 
**OrderType** | **NullableString** |  | 
**AvgPrice** | **float64** |  | 
**BrokerOrderId** | **int64** |  | 
**MatchedPrice** | **float64** |  | 
**IsNVDR** | **bool** |  | 
**OrderNo** | **NullableString** |  | 
**Symbol** | **NullableString** |  | 
**Price** | **float64** |  | 
**Amount** | **float64** |  | 
**QuantityExecuted** | Pointer to **NullableString** |  | [optional] 
**InterestRate** | Pointer to **NullableFloat64** |  | [optional] 
**Logo** | Pointer to **NullableString** |  | [optional] 
**FriendlyName** | Pointer to **NullableString** |  | [optional] 
**InstrumentCategory** | Pointer to **NullableString** |  | [optional] 
**OrderTimeStamp** | Pointer to **NullableInt64** |  | [optional] 

## Methods

### NewPiSetServiceAPIModelsSetOpenOrderResponse

`func NewPiSetServiceAPIModelsSetOpenOrderResponse(orderId int64, custCode NullableString, side PiSetServiceDomainAggregatesModelFinancialAssetAggregateOrderAction, status NullableString, orderType NullableString, avgPrice float64, brokerOrderId int64, matchedPrice float64, isNVDR bool, orderNo NullableString, symbol NullableString, price float64, amount float64, ) *PiSetServiceAPIModelsSetOpenOrderResponse`

NewPiSetServiceAPIModelsSetOpenOrderResponse instantiates a new PiSetServiceAPIModelsSetOpenOrderResponse object
This constructor will assign default values to properties that have it defined,
and makes sure properties required by API are set, but the set of arguments
will change when the set of required properties is changed

### NewPiSetServiceAPIModelsSetOpenOrderResponseWithDefaults

`func NewPiSetServiceAPIModelsSetOpenOrderResponseWithDefaults() *PiSetServiceAPIModelsSetOpenOrderResponse`

NewPiSetServiceAPIModelsSetOpenOrderResponseWithDefaults instantiates a new PiSetServiceAPIModelsSetOpenOrderResponse object
This constructor will only assign default values to properties that have it defined,
but it doesn't guarantee that properties required by API are set

### GetOrderId

`func (o *PiSetServiceAPIModelsSetOpenOrderResponse) GetOrderId() int64`

GetOrderId returns the OrderId field if non-nil, zero value otherwise.

### GetOrderIdOk

`func (o *PiSetServiceAPIModelsSetOpenOrderResponse) GetOrderIdOk() (*int64, bool)`

GetOrderIdOk returns a tuple with the OrderId field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetOrderId

`func (o *PiSetServiceAPIModelsSetOpenOrderResponse) SetOrderId(v int64)`

SetOrderId sets OrderId field to given value.


### GetCustCode

`func (o *PiSetServiceAPIModelsSetOpenOrderResponse) GetCustCode() string`

GetCustCode returns the CustCode field if non-nil, zero value otherwise.

### GetCustCodeOk

`func (o *PiSetServiceAPIModelsSetOpenOrderResponse) GetCustCodeOk() (*string, bool)`

GetCustCodeOk returns a tuple with the CustCode field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetCustCode

`func (o *PiSetServiceAPIModelsSetOpenOrderResponse) SetCustCode(v string)`

SetCustCode sets CustCode field to given value.


### SetCustCodeNil

`func (o *PiSetServiceAPIModelsSetOpenOrderResponse) SetCustCodeNil(b bool)`

 SetCustCodeNil sets the value for CustCode to be an explicit nil

### UnsetCustCode
`func (o *PiSetServiceAPIModelsSetOpenOrderResponse) UnsetCustCode()`

UnsetCustCode ensures that no value is present for CustCode, not even an explicit nil
### GetSide

`func (o *PiSetServiceAPIModelsSetOpenOrderResponse) GetSide() PiSetServiceDomainAggregatesModelFinancialAssetAggregateOrderAction`

GetSide returns the Side field if non-nil, zero value otherwise.

### GetSideOk

`func (o *PiSetServiceAPIModelsSetOpenOrderResponse) GetSideOk() (*PiSetServiceDomainAggregatesModelFinancialAssetAggregateOrderAction, bool)`

GetSideOk returns a tuple with the Side field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetSide

`func (o *PiSetServiceAPIModelsSetOpenOrderResponse) SetSide(v PiSetServiceDomainAggregatesModelFinancialAssetAggregateOrderAction)`

SetSide sets Side field to given value.


### GetStatus

`func (o *PiSetServiceAPIModelsSetOpenOrderResponse) GetStatus() string`

GetStatus returns the Status field if non-nil, zero value otherwise.

### GetStatusOk

`func (o *PiSetServiceAPIModelsSetOpenOrderResponse) GetStatusOk() (*string, bool)`

GetStatusOk returns a tuple with the Status field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetStatus

`func (o *PiSetServiceAPIModelsSetOpenOrderResponse) SetStatus(v string)`

SetStatus sets Status field to given value.


### SetStatusNil

`func (o *PiSetServiceAPIModelsSetOpenOrderResponse) SetStatusNil(b bool)`

 SetStatusNil sets the value for Status to be an explicit nil

### UnsetStatus
`func (o *PiSetServiceAPIModelsSetOpenOrderResponse) UnsetStatus()`

UnsetStatus ensures that no value is present for Status, not even an explicit nil
### GetOrderType

`func (o *PiSetServiceAPIModelsSetOpenOrderResponse) GetOrderType() string`

GetOrderType returns the OrderType field if non-nil, zero value otherwise.

### GetOrderTypeOk

`func (o *PiSetServiceAPIModelsSetOpenOrderResponse) GetOrderTypeOk() (*string, bool)`

GetOrderTypeOk returns a tuple with the OrderType field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetOrderType

`func (o *PiSetServiceAPIModelsSetOpenOrderResponse) SetOrderType(v string)`

SetOrderType sets OrderType field to given value.


### SetOrderTypeNil

`func (o *PiSetServiceAPIModelsSetOpenOrderResponse) SetOrderTypeNil(b bool)`

 SetOrderTypeNil sets the value for OrderType to be an explicit nil

### UnsetOrderType
`func (o *PiSetServiceAPIModelsSetOpenOrderResponse) UnsetOrderType()`

UnsetOrderType ensures that no value is present for OrderType, not even an explicit nil
### GetAvgPrice

`func (o *PiSetServiceAPIModelsSetOpenOrderResponse) GetAvgPrice() float64`

GetAvgPrice returns the AvgPrice field if non-nil, zero value otherwise.

### GetAvgPriceOk

`func (o *PiSetServiceAPIModelsSetOpenOrderResponse) GetAvgPriceOk() (*float64, bool)`

GetAvgPriceOk returns a tuple with the AvgPrice field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetAvgPrice

`func (o *PiSetServiceAPIModelsSetOpenOrderResponse) SetAvgPrice(v float64)`

SetAvgPrice sets AvgPrice field to given value.


### GetBrokerOrderId

`func (o *PiSetServiceAPIModelsSetOpenOrderResponse) GetBrokerOrderId() int64`

GetBrokerOrderId returns the BrokerOrderId field if non-nil, zero value otherwise.

### GetBrokerOrderIdOk

`func (o *PiSetServiceAPIModelsSetOpenOrderResponse) GetBrokerOrderIdOk() (*int64, bool)`

GetBrokerOrderIdOk returns a tuple with the BrokerOrderId field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetBrokerOrderId

`func (o *PiSetServiceAPIModelsSetOpenOrderResponse) SetBrokerOrderId(v int64)`

SetBrokerOrderId sets BrokerOrderId field to given value.


### GetMatchedPrice

`func (o *PiSetServiceAPIModelsSetOpenOrderResponse) GetMatchedPrice() float64`

GetMatchedPrice returns the MatchedPrice field if non-nil, zero value otherwise.

### GetMatchedPriceOk

`func (o *PiSetServiceAPIModelsSetOpenOrderResponse) GetMatchedPriceOk() (*float64, bool)`

GetMatchedPriceOk returns a tuple with the MatchedPrice field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetMatchedPrice

`func (o *PiSetServiceAPIModelsSetOpenOrderResponse) SetMatchedPrice(v float64)`

SetMatchedPrice sets MatchedPrice field to given value.


### GetIsNVDR

`func (o *PiSetServiceAPIModelsSetOpenOrderResponse) GetIsNVDR() bool`

GetIsNVDR returns the IsNVDR field if non-nil, zero value otherwise.

### GetIsNVDROk

`func (o *PiSetServiceAPIModelsSetOpenOrderResponse) GetIsNVDROk() (*bool, bool)`

GetIsNVDROk returns a tuple with the IsNVDR field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetIsNVDR

`func (o *PiSetServiceAPIModelsSetOpenOrderResponse) SetIsNVDR(v bool)`

SetIsNVDR sets IsNVDR field to given value.


### GetOrderNo

`func (o *PiSetServiceAPIModelsSetOpenOrderResponse) GetOrderNo() string`

GetOrderNo returns the OrderNo field if non-nil, zero value otherwise.

### GetOrderNoOk

`func (o *PiSetServiceAPIModelsSetOpenOrderResponse) GetOrderNoOk() (*string, bool)`

GetOrderNoOk returns a tuple with the OrderNo field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetOrderNo

`func (o *PiSetServiceAPIModelsSetOpenOrderResponse) SetOrderNo(v string)`

SetOrderNo sets OrderNo field to given value.


### SetOrderNoNil

`func (o *PiSetServiceAPIModelsSetOpenOrderResponse) SetOrderNoNil(b bool)`

 SetOrderNoNil sets the value for OrderNo to be an explicit nil

### UnsetOrderNo
`func (o *PiSetServiceAPIModelsSetOpenOrderResponse) UnsetOrderNo()`

UnsetOrderNo ensures that no value is present for OrderNo, not even an explicit nil
### GetSymbol

`func (o *PiSetServiceAPIModelsSetOpenOrderResponse) GetSymbol() string`

GetSymbol returns the Symbol field if non-nil, zero value otherwise.

### GetSymbolOk

`func (o *PiSetServiceAPIModelsSetOpenOrderResponse) GetSymbolOk() (*string, bool)`

GetSymbolOk returns a tuple with the Symbol field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetSymbol

`func (o *PiSetServiceAPIModelsSetOpenOrderResponse) SetSymbol(v string)`

SetSymbol sets Symbol field to given value.


### SetSymbolNil

`func (o *PiSetServiceAPIModelsSetOpenOrderResponse) SetSymbolNil(b bool)`

 SetSymbolNil sets the value for Symbol to be an explicit nil

### UnsetSymbol
`func (o *PiSetServiceAPIModelsSetOpenOrderResponse) UnsetSymbol()`

UnsetSymbol ensures that no value is present for Symbol, not even an explicit nil
### GetPrice

`func (o *PiSetServiceAPIModelsSetOpenOrderResponse) GetPrice() float64`

GetPrice returns the Price field if non-nil, zero value otherwise.

### GetPriceOk

`func (o *PiSetServiceAPIModelsSetOpenOrderResponse) GetPriceOk() (*float64, bool)`

GetPriceOk returns a tuple with the Price field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetPrice

`func (o *PiSetServiceAPIModelsSetOpenOrderResponse) SetPrice(v float64)`

SetPrice sets Price field to given value.


### GetAmount

`func (o *PiSetServiceAPIModelsSetOpenOrderResponse) GetAmount() float64`

GetAmount returns the Amount field if non-nil, zero value otherwise.

### GetAmountOk

`func (o *PiSetServiceAPIModelsSetOpenOrderResponse) GetAmountOk() (*float64, bool)`

GetAmountOk returns a tuple with the Amount field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetAmount

`func (o *PiSetServiceAPIModelsSetOpenOrderResponse) SetAmount(v float64)`

SetAmount sets Amount field to given value.


### GetQuantityExecuted

`func (o *PiSetServiceAPIModelsSetOpenOrderResponse) GetQuantityExecuted() string`

GetQuantityExecuted returns the QuantityExecuted field if non-nil, zero value otherwise.

### GetQuantityExecutedOk

`func (o *PiSetServiceAPIModelsSetOpenOrderResponse) GetQuantityExecutedOk() (*string, bool)`

GetQuantityExecutedOk returns a tuple with the QuantityExecuted field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetQuantityExecuted

`func (o *PiSetServiceAPIModelsSetOpenOrderResponse) SetQuantityExecuted(v string)`

SetQuantityExecuted sets QuantityExecuted field to given value.

### HasQuantityExecuted

`func (o *PiSetServiceAPIModelsSetOpenOrderResponse) HasQuantityExecuted() bool`

HasQuantityExecuted returns a boolean if a field has been set.

### SetQuantityExecutedNil

`func (o *PiSetServiceAPIModelsSetOpenOrderResponse) SetQuantityExecutedNil(b bool)`

 SetQuantityExecutedNil sets the value for QuantityExecuted to be an explicit nil

### UnsetQuantityExecuted
`func (o *PiSetServiceAPIModelsSetOpenOrderResponse) UnsetQuantityExecuted()`

UnsetQuantityExecuted ensures that no value is present for QuantityExecuted, not even an explicit nil
### GetInterestRate

`func (o *PiSetServiceAPIModelsSetOpenOrderResponse) GetInterestRate() float64`

GetInterestRate returns the InterestRate field if non-nil, zero value otherwise.

### GetInterestRateOk

`func (o *PiSetServiceAPIModelsSetOpenOrderResponse) GetInterestRateOk() (*float64, bool)`

GetInterestRateOk returns a tuple with the InterestRate field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetInterestRate

`func (o *PiSetServiceAPIModelsSetOpenOrderResponse) SetInterestRate(v float64)`

SetInterestRate sets InterestRate field to given value.

### HasInterestRate

`func (o *PiSetServiceAPIModelsSetOpenOrderResponse) HasInterestRate() bool`

HasInterestRate returns a boolean if a field has been set.

### SetInterestRateNil

`func (o *PiSetServiceAPIModelsSetOpenOrderResponse) SetInterestRateNil(b bool)`

 SetInterestRateNil sets the value for InterestRate to be an explicit nil

### UnsetInterestRate
`func (o *PiSetServiceAPIModelsSetOpenOrderResponse) UnsetInterestRate()`

UnsetInterestRate ensures that no value is present for InterestRate, not even an explicit nil
### GetLogo

`func (o *PiSetServiceAPIModelsSetOpenOrderResponse) GetLogo() string`

GetLogo returns the Logo field if non-nil, zero value otherwise.

### GetLogoOk

`func (o *PiSetServiceAPIModelsSetOpenOrderResponse) GetLogoOk() (*string, bool)`

GetLogoOk returns a tuple with the Logo field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetLogo

`func (o *PiSetServiceAPIModelsSetOpenOrderResponse) SetLogo(v string)`

SetLogo sets Logo field to given value.

### HasLogo

`func (o *PiSetServiceAPIModelsSetOpenOrderResponse) HasLogo() bool`

HasLogo returns a boolean if a field has been set.

### SetLogoNil

`func (o *PiSetServiceAPIModelsSetOpenOrderResponse) SetLogoNil(b bool)`

 SetLogoNil sets the value for Logo to be an explicit nil

### UnsetLogo
`func (o *PiSetServiceAPIModelsSetOpenOrderResponse) UnsetLogo()`

UnsetLogo ensures that no value is present for Logo, not even an explicit nil
### GetFriendlyName

`func (o *PiSetServiceAPIModelsSetOpenOrderResponse) GetFriendlyName() string`

GetFriendlyName returns the FriendlyName field if non-nil, zero value otherwise.

### GetFriendlyNameOk

`func (o *PiSetServiceAPIModelsSetOpenOrderResponse) GetFriendlyNameOk() (*string, bool)`

GetFriendlyNameOk returns a tuple with the FriendlyName field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetFriendlyName

`func (o *PiSetServiceAPIModelsSetOpenOrderResponse) SetFriendlyName(v string)`

SetFriendlyName sets FriendlyName field to given value.

### HasFriendlyName

`func (o *PiSetServiceAPIModelsSetOpenOrderResponse) HasFriendlyName() bool`

HasFriendlyName returns a boolean if a field has been set.

### SetFriendlyNameNil

`func (o *PiSetServiceAPIModelsSetOpenOrderResponse) SetFriendlyNameNil(b bool)`

 SetFriendlyNameNil sets the value for FriendlyName to be an explicit nil

### UnsetFriendlyName
`func (o *PiSetServiceAPIModelsSetOpenOrderResponse) UnsetFriendlyName()`

UnsetFriendlyName ensures that no value is present for FriendlyName, not even an explicit nil
### GetInstrumentCategory

`func (o *PiSetServiceAPIModelsSetOpenOrderResponse) GetInstrumentCategory() string`

GetInstrumentCategory returns the InstrumentCategory field if non-nil, zero value otherwise.

### GetInstrumentCategoryOk

`func (o *PiSetServiceAPIModelsSetOpenOrderResponse) GetInstrumentCategoryOk() (*string, bool)`

GetInstrumentCategoryOk returns a tuple with the InstrumentCategory field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetInstrumentCategory

`func (o *PiSetServiceAPIModelsSetOpenOrderResponse) SetInstrumentCategory(v string)`

SetInstrumentCategory sets InstrumentCategory field to given value.

### HasInstrumentCategory

`func (o *PiSetServiceAPIModelsSetOpenOrderResponse) HasInstrumentCategory() bool`

HasInstrumentCategory returns a boolean if a field has been set.

### SetInstrumentCategoryNil

`func (o *PiSetServiceAPIModelsSetOpenOrderResponse) SetInstrumentCategoryNil(b bool)`

 SetInstrumentCategoryNil sets the value for InstrumentCategory to be an explicit nil

### UnsetInstrumentCategory
`func (o *PiSetServiceAPIModelsSetOpenOrderResponse) UnsetInstrumentCategory()`

UnsetInstrumentCategory ensures that no value is present for InstrumentCategory, not even an explicit nil
### GetOrderTimeStamp

`func (o *PiSetServiceAPIModelsSetOpenOrderResponse) GetOrderTimeStamp() int64`

GetOrderTimeStamp returns the OrderTimeStamp field if non-nil, zero value otherwise.

### GetOrderTimeStampOk

`func (o *PiSetServiceAPIModelsSetOpenOrderResponse) GetOrderTimeStampOk() (*int64, bool)`

GetOrderTimeStampOk returns a tuple with the OrderTimeStamp field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetOrderTimeStamp

`func (o *PiSetServiceAPIModelsSetOpenOrderResponse) SetOrderTimeStamp(v int64)`

SetOrderTimeStamp sets OrderTimeStamp field to given value.

### HasOrderTimeStamp

`func (o *PiSetServiceAPIModelsSetOpenOrderResponse) HasOrderTimeStamp() bool`

HasOrderTimeStamp returns a boolean if a field has been set.

### SetOrderTimeStampNil

`func (o *PiSetServiceAPIModelsSetOpenOrderResponse) SetOrderTimeStampNil(b bool)`

 SetOrderTimeStampNil sets the value for OrderTimeStamp to be an explicit nil

### UnsetOrderTimeStamp
`func (o *PiSetServiceAPIModelsSetOpenOrderResponse) UnsetOrderTimeStamp()`

UnsetOrderTimeStamp ensures that no value is present for OrderTimeStamp, not even an explicit nil

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


