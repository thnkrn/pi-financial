# PiSetServiceAPIModelsSetOrderHistoryResponse

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**OrderId** | **int64** |  | 
**IsNVDR** | **bool** |  | 
**Status** | **NullableString** |  | 
**Symbol** | **NullableString** |  | 
**Side** | [**PiSetServiceDomainAggregatesModelFinancialAssetAggregateOrderAction**](PiSetServiceDomainAggregatesModelFinancialAssetAggregateOrderAction.md) |  | 
**TradeType** | **NullableString** |  | 
**OrderNo** | **int64** |  | 
**MatchedPrice** | **float64** |  | 
**Amount** | **float64** |  | 
**Price** | **float64** |  | 
**OrderTimeStamp** | Pointer to **NullableInt64** |  | [optional] 
**InterestRate** | Pointer to **NullableFloat64** |  | [optional] 
**RealizedPL** | Pointer to **NullableString** |  | [optional] 
**MatchVolume** | Pointer to **NullableString** |  | [optional] 
**CancelVolume** | Pointer to **NullableFloat64** |  | [optional] 
**Detail** | Pointer to **NullableString** |  | [optional] 

## Methods

### NewPiSetServiceAPIModelsSetOrderHistoryResponse

`func NewPiSetServiceAPIModelsSetOrderHistoryResponse(orderId int64, isNVDR bool, status NullableString, symbol NullableString, side PiSetServiceDomainAggregatesModelFinancialAssetAggregateOrderAction, tradeType NullableString, orderNo int64, matchedPrice float64, amount float64, price float64, ) *PiSetServiceAPIModelsSetOrderHistoryResponse`

NewPiSetServiceAPIModelsSetOrderHistoryResponse instantiates a new PiSetServiceAPIModelsSetOrderHistoryResponse object
This constructor will assign default values to properties that have it defined,
and makes sure properties required by API are set, but the set of arguments
will change when the set of required properties is changed

### NewPiSetServiceAPIModelsSetOrderHistoryResponseWithDefaults

`func NewPiSetServiceAPIModelsSetOrderHistoryResponseWithDefaults() *PiSetServiceAPIModelsSetOrderHistoryResponse`

NewPiSetServiceAPIModelsSetOrderHistoryResponseWithDefaults instantiates a new PiSetServiceAPIModelsSetOrderHistoryResponse object
This constructor will only assign default values to properties that have it defined,
but it doesn't guarantee that properties required by API are set

### GetOrderId

`func (o *PiSetServiceAPIModelsSetOrderHistoryResponse) GetOrderId() int64`

GetOrderId returns the OrderId field if non-nil, zero value otherwise.

### GetOrderIdOk

`func (o *PiSetServiceAPIModelsSetOrderHistoryResponse) GetOrderIdOk() (*int64, bool)`

GetOrderIdOk returns a tuple with the OrderId field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetOrderId

`func (o *PiSetServiceAPIModelsSetOrderHistoryResponse) SetOrderId(v int64)`

SetOrderId sets OrderId field to given value.


### GetIsNVDR

`func (o *PiSetServiceAPIModelsSetOrderHistoryResponse) GetIsNVDR() bool`

GetIsNVDR returns the IsNVDR field if non-nil, zero value otherwise.

### GetIsNVDROk

`func (o *PiSetServiceAPIModelsSetOrderHistoryResponse) GetIsNVDROk() (*bool, bool)`

GetIsNVDROk returns a tuple with the IsNVDR field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetIsNVDR

`func (o *PiSetServiceAPIModelsSetOrderHistoryResponse) SetIsNVDR(v bool)`

SetIsNVDR sets IsNVDR field to given value.


### GetStatus

`func (o *PiSetServiceAPIModelsSetOrderHistoryResponse) GetStatus() string`

GetStatus returns the Status field if non-nil, zero value otherwise.

### GetStatusOk

`func (o *PiSetServiceAPIModelsSetOrderHistoryResponse) GetStatusOk() (*string, bool)`

GetStatusOk returns a tuple with the Status field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetStatus

`func (o *PiSetServiceAPIModelsSetOrderHistoryResponse) SetStatus(v string)`

SetStatus sets Status field to given value.


### SetStatusNil

`func (o *PiSetServiceAPIModelsSetOrderHistoryResponse) SetStatusNil(b bool)`

 SetStatusNil sets the value for Status to be an explicit nil

### UnsetStatus
`func (o *PiSetServiceAPIModelsSetOrderHistoryResponse) UnsetStatus()`

UnsetStatus ensures that no value is present for Status, not even an explicit nil
### GetSymbol

`func (o *PiSetServiceAPIModelsSetOrderHistoryResponse) GetSymbol() string`

GetSymbol returns the Symbol field if non-nil, zero value otherwise.

### GetSymbolOk

`func (o *PiSetServiceAPIModelsSetOrderHistoryResponse) GetSymbolOk() (*string, bool)`

GetSymbolOk returns a tuple with the Symbol field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetSymbol

`func (o *PiSetServiceAPIModelsSetOrderHistoryResponse) SetSymbol(v string)`

SetSymbol sets Symbol field to given value.


### SetSymbolNil

`func (o *PiSetServiceAPIModelsSetOrderHistoryResponse) SetSymbolNil(b bool)`

 SetSymbolNil sets the value for Symbol to be an explicit nil

### UnsetSymbol
`func (o *PiSetServiceAPIModelsSetOrderHistoryResponse) UnsetSymbol()`

UnsetSymbol ensures that no value is present for Symbol, not even an explicit nil
### GetSide

`func (o *PiSetServiceAPIModelsSetOrderHistoryResponse) GetSide() PiSetServiceDomainAggregatesModelFinancialAssetAggregateOrderAction`

GetSide returns the Side field if non-nil, zero value otherwise.

### GetSideOk

`func (o *PiSetServiceAPIModelsSetOrderHistoryResponse) GetSideOk() (*PiSetServiceDomainAggregatesModelFinancialAssetAggregateOrderAction, bool)`

GetSideOk returns a tuple with the Side field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetSide

`func (o *PiSetServiceAPIModelsSetOrderHistoryResponse) SetSide(v PiSetServiceDomainAggregatesModelFinancialAssetAggregateOrderAction)`

SetSide sets Side field to given value.


### GetTradeType

`func (o *PiSetServiceAPIModelsSetOrderHistoryResponse) GetTradeType() string`

GetTradeType returns the TradeType field if non-nil, zero value otherwise.

### GetTradeTypeOk

`func (o *PiSetServiceAPIModelsSetOrderHistoryResponse) GetTradeTypeOk() (*string, bool)`

GetTradeTypeOk returns a tuple with the TradeType field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetTradeType

`func (o *PiSetServiceAPIModelsSetOrderHistoryResponse) SetTradeType(v string)`

SetTradeType sets TradeType field to given value.


### SetTradeTypeNil

`func (o *PiSetServiceAPIModelsSetOrderHistoryResponse) SetTradeTypeNil(b bool)`

 SetTradeTypeNil sets the value for TradeType to be an explicit nil

### UnsetTradeType
`func (o *PiSetServiceAPIModelsSetOrderHistoryResponse) UnsetTradeType()`

UnsetTradeType ensures that no value is present for TradeType, not even an explicit nil
### GetOrderNo

`func (o *PiSetServiceAPIModelsSetOrderHistoryResponse) GetOrderNo() int64`

GetOrderNo returns the OrderNo field if non-nil, zero value otherwise.

### GetOrderNoOk

`func (o *PiSetServiceAPIModelsSetOrderHistoryResponse) GetOrderNoOk() (*int64, bool)`

GetOrderNoOk returns a tuple with the OrderNo field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetOrderNo

`func (o *PiSetServiceAPIModelsSetOrderHistoryResponse) SetOrderNo(v int64)`

SetOrderNo sets OrderNo field to given value.


### GetMatchedPrice

`func (o *PiSetServiceAPIModelsSetOrderHistoryResponse) GetMatchedPrice() float64`

GetMatchedPrice returns the MatchedPrice field if non-nil, zero value otherwise.

### GetMatchedPriceOk

`func (o *PiSetServiceAPIModelsSetOrderHistoryResponse) GetMatchedPriceOk() (*float64, bool)`

GetMatchedPriceOk returns a tuple with the MatchedPrice field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetMatchedPrice

`func (o *PiSetServiceAPIModelsSetOrderHistoryResponse) SetMatchedPrice(v float64)`

SetMatchedPrice sets MatchedPrice field to given value.


### GetAmount

`func (o *PiSetServiceAPIModelsSetOrderHistoryResponse) GetAmount() float64`

GetAmount returns the Amount field if non-nil, zero value otherwise.

### GetAmountOk

`func (o *PiSetServiceAPIModelsSetOrderHistoryResponse) GetAmountOk() (*float64, bool)`

GetAmountOk returns a tuple with the Amount field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetAmount

`func (o *PiSetServiceAPIModelsSetOrderHistoryResponse) SetAmount(v float64)`

SetAmount sets Amount field to given value.


### GetPrice

`func (o *PiSetServiceAPIModelsSetOrderHistoryResponse) GetPrice() float64`

GetPrice returns the Price field if non-nil, zero value otherwise.

### GetPriceOk

`func (o *PiSetServiceAPIModelsSetOrderHistoryResponse) GetPriceOk() (*float64, bool)`

GetPriceOk returns a tuple with the Price field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetPrice

`func (o *PiSetServiceAPIModelsSetOrderHistoryResponse) SetPrice(v float64)`

SetPrice sets Price field to given value.


### GetOrderTimeStamp

`func (o *PiSetServiceAPIModelsSetOrderHistoryResponse) GetOrderTimeStamp() int64`

GetOrderTimeStamp returns the OrderTimeStamp field if non-nil, zero value otherwise.

### GetOrderTimeStampOk

`func (o *PiSetServiceAPIModelsSetOrderHistoryResponse) GetOrderTimeStampOk() (*int64, bool)`

GetOrderTimeStampOk returns a tuple with the OrderTimeStamp field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetOrderTimeStamp

`func (o *PiSetServiceAPIModelsSetOrderHistoryResponse) SetOrderTimeStamp(v int64)`

SetOrderTimeStamp sets OrderTimeStamp field to given value.

### HasOrderTimeStamp

`func (o *PiSetServiceAPIModelsSetOrderHistoryResponse) HasOrderTimeStamp() bool`

HasOrderTimeStamp returns a boolean if a field has been set.

### SetOrderTimeStampNil

`func (o *PiSetServiceAPIModelsSetOrderHistoryResponse) SetOrderTimeStampNil(b bool)`

 SetOrderTimeStampNil sets the value for OrderTimeStamp to be an explicit nil

### UnsetOrderTimeStamp
`func (o *PiSetServiceAPIModelsSetOrderHistoryResponse) UnsetOrderTimeStamp()`

UnsetOrderTimeStamp ensures that no value is present for OrderTimeStamp, not even an explicit nil
### GetInterestRate

`func (o *PiSetServiceAPIModelsSetOrderHistoryResponse) GetInterestRate() float64`

GetInterestRate returns the InterestRate field if non-nil, zero value otherwise.

### GetInterestRateOk

`func (o *PiSetServiceAPIModelsSetOrderHistoryResponse) GetInterestRateOk() (*float64, bool)`

GetInterestRateOk returns a tuple with the InterestRate field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetInterestRate

`func (o *PiSetServiceAPIModelsSetOrderHistoryResponse) SetInterestRate(v float64)`

SetInterestRate sets InterestRate field to given value.

### HasInterestRate

`func (o *PiSetServiceAPIModelsSetOrderHistoryResponse) HasInterestRate() bool`

HasInterestRate returns a boolean if a field has been set.

### SetInterestRateNil

`func (o *PiSetServiceAPIModelsSetOrderHistoryResponse) SetInterestRateNil(b bool)`

 SetInterestRateNil sets the value for InterestRate to be an explicit nil

### UnsetInterestRate
`func (o *PiSetServiceAPIModelsSetOrderHistoryResponse) UnsetInterestRate()`

UnsetInterestRate ensures that no value is present for InterestRate, not even an explicit nil
### GetRealizedPL

`func (o *PiSetServiceAPIModelsSetOrderHistoryResponse) GetRealizedPL() string`

GetRealizedPL returns the RealizedPL field if non-nil, zero value otherwise.

### GetRealizedPLOk

`func (o *PiSetServiceAPIModelsSetOrderHistoryResponse) GetRealizedPLOk() (*string, bool)`

GetRealizedPLOk returns a tuple with the RealizedPL field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetRealizedPL

`func (o *PiSetServiceAPIModelsSetOrderHistoryResponse) SetRealizedPL(v string)`

SetRealizedPL sets RealizedPL field to given value.

### HasRealizedPL

`func (o *PiSetServiceAPIModelsSetOrderHistoryResponse) HasRealizedPL() bool`

HasRealizedPL returns a boolean if a field has been set.

### SetRealizedPLNil

`func (o *PiSetServiceAPIModelsSetOrderHistoryResponse) SetRealizedPLNil(b bool)`

 SetRealizedPLNil sets the value for RealizedPL to be an explicit nil

### UnsetRealizedPL
`func (o *PiSetServiceAPIModelsSetOrderHistoryResponse) UnsetRealizedPL()`

UnsetRealizedPL ensures that no value is present for RealizedPL, not even an explicit nil
### GetMatchVolume

`func (o *PiSetServiceAPIModelsSetOrderHistoryResponse) GetMatchVolume() string`

GetMatchVolume returns the MatchVolume field if non-nil, zero value otherwise.

### GetMatchVolumeOk

`func (o *PiSetServiceAPIModelsSetOrderHistoryResponse) GetMatchVolumeOk() (*string, bool)`

GetMatchVolumeOk returns a tuple with the MatchVolume field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetMatchVolume

`func (o *PiSetServiceAPIModelsSetOrderHistoryResponse) SetMatchVolume(v string)`

SetMatchVolume sets MatchVolume field to given value.

### HasMatchVolume

`func (o *PiSetServiceAPIModelsSetOrderHistoryResponse) HasMatchVolume() bool`

HasMatchVolume returns a boolean if a field has been set.

### SetMatchVolumeNil

`func (o *PiSetServiceAPIModelsSetOrderHistoryResponse) SetMatchVolumeNil(b bool)`

 SetMatchVolumeNil sets the value for MatchVolume to be an explicit nil

### UnsetMatchVolume
`func (o *PiSetServiceAPIModelsSetOrderHistoryResponse) UnsetMatchVolume()`

UnsetMatchVolume ensures that no value is present for MatchVolume, not even an explicit nil
### GetCancelVolume

`func (o *PiSetServiceAPIModelsSetOrderHistoryResponse) GetCancelVolume() float64`

GetCancelVolume returns the CancelVolume field if non-nil, zero value otherwise.

### GetCancelVolumeOk

`func (o *PiSetServiceAPIModelsSetOrderHistoryResponse) GetCancelVolumeOk() (*float64, bool)`

GetCancelVolumeOk returns a tuple with the CancelVolume field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetCancelVolume

`func (o *PiSetServiceAPIModelsSetOrderHistoryResponse) SetCancelVolume(v float64)`

SetCancelVolume sets CancelVolume field to given value.

### HasCancelVolume

`func (o *PiSetServiceAPIModelsSetOrderHistoryResponse) HasCancelVolume() bool`

HasCancelVolume returns a boolean if a field has been set.

### SetCancelVolumeNil

`func (o *PiSetServiceAPIModelsSetOrderHistoryResponse) SetCancelVolumeNil(b bool)`

 SetCancelVolumeNil sets the value for CancelVolume to be an explicit nil

### UnsetCancelVolume
`func (o *PiSetServiceAPIModelsSetOrderHistoryResponse) UnsetCancelVolume()`

UnsetCancelVolume ensures that no value is present for CancelVolume, not even an explicit nil
### GetDetail

`func (o *PiSetServiceAPIModelsSetOrderHistoryResponse) GetDetail() string`

GetDetail returns the Detail field if non-nil, zero value otherwise.

### GetDetailOk

`func (o *PiSetServiceAPIModelsSetOrderHistoryResponse) GetDetailOk() (*string, bool)`

GetDetailOk returns a tuple with the Detail field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetDetail

`func (o *PiSetServiceAPIModelsSetOrderHistoryResponse) SetDetail(v string)`

SetDetail sets Detail field to given value.

### HasDetail

`func (o *PiSetServiceAPIModelsSetOrderHistoryResponse) HasDetail() bool`

HasDetail returns a boolean if a field has been set.

### SetDetailNil

`func (o *PiSetServiceAPIModelsSetOrderHistoryResponse) SetDetailNil(b bool)`

 SetDetailNil sets the value for Detail to be an explicit nil

### UnsetDetail
`func (o *PiSetServiceAPIModelsSetOrderHistoryResponse) UnsetDetail()`

UnsetDetail ensures that no value is present for Detail, not even an explicit nil

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


