# PiSetServiceApplicationCommandsReviewSblOrderResponse

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**Id** | **string** |  | 
**TradingAccountId** | **string** |  | 
**TradingAccountNo** | **NullableString** |  | 
**CustomerCode** | **NullableString** |  | 
**OrderId** | **int64** |  | 
**Symbol** | **NullableString** |  | 
**Type** | [**PiSetServiceDomainAggregatesModelFinancialAssetAggregateSblOrderType**](PiSetServiceDomainAggregatesModelFinancialAssetAggregateSblOrderType.md) |  | 
**Volume** | **string** |  | 
**Status** | [**PiSetServiceDomainAggregatesModelFinancialAssetAggregateSblOrderStatus**](PiSetServiceDomainAggregatesModelFinancialAssetAggregateSblOrderStatus.md) |  | 
**RejectedReason** | Pointer to **NullableString** |  | [optional] 
**ReviewerId** | Pointer to **NullableString** |  | [optional] 
**CreatedAt** | Pointer to **time.Time** |  | [optional] 
**UpdatedAt** | Pointer to **NullableTime** |  | [optional] 

## Methods

### NewPiSetServiceApplicationCommandsReviewSblOrderResponse

`func NewPiSetServiceApplicationCommandsReviewSblOrderResponse(id string, tradingAccountId string, tradingAccountNo NullableString, customerCode NullableString, orderId int64, symbol NullableString, type_ PiSetServiceDomainAggregatesModelFinancialAssetAggregateSblOrderType, volume string, status PiSetServiceDomainAggregatesModelFinancialAssetAggregateSblOrderStatus, ) *PiSetServiceApplicationCommandsReviewSblOrderResponse`

NewPiSetServiceApplicationCommandsReviewSblOrderResponse instantiates a new PiSetServiceApplicationCommandsReviewSblOrderResponse object
This constructor will assign default values to properties that have it defined,
and makes sure properties required by API are set, but the set of arguments
will change when the set of required properties is changed

### NewPiSetServiceApplicationCommandsReviewSblOrderResponseWithDefaults

`func NewPiSetServiceApplicationCommandsReviewSblOrderResponseWithDefaults() *PiSetServiceApplicationCommandsReviewSblOrderResponse`

NewPiSetServiceApplicationCommandsReviewSblOrderResponseWithDefaults instantiates a new PiSetServiceApplicationCommandsReviewSblOrderResponse object
This constructor will only assign default values to properties that have it defined,
but it doesn't guarantee that properties required by API are set

### GetId

`func (o *PiSetServiceApplicationCommandsReviewSblOrderResponse) GetId() string`

GetId returns the Id field if non-nil, zero value otherwise.

### GetIdOk

`func (o *PiSetServiceApplicationCommandsReviewSblOrderResponse) GetIdOk() (*string, bool)`

GetIdOk returns a tuple with the Id field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetId

`func (o *PiSetServiceApplicationCommandsReviewSblOrderResponse) SetId(v string)`

SetId sets Id field to given value.


### GetTradingAccountId

`func (o *PiSetServiceApplicationCommandsReviewSblOrderResponse) GetTradingAccountId() string`

GetTradingAccountId returns the TradingAccountId field if non-nil, zero value otherwise.

### GetTradingAccountIdOk

`func (o *PiSetServiceApplicationCommandsReviewSblOrderResponse) GetTradingAccountIdOk() (*string, bool)`

GetTradingAccountIdOk returns a tuple with the TradingAccountId field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetTradingAccountId

`func (o *PiSetServiceApplicationCommandsReviewSblOrderResponse) SetTradingAccountId(v string)`

SetTradingAccountId sets TradingAccountId field to given value.


### GetTradingAccountNo

`func (o *PiSetServiceApplicationCommandsReviewSblOrderResponse) GetTradingAccountNo() string`

GetTradingAccountNo returns the TradingAccountNo field if non-nil, zero value otherwise.

### GetTradingAccountNoOk

`func (o *PiSetServiceApplicationCommandsReviewSblOrderResponse) GetTradingAccountNoOk() (*string, bool)`

GetTradingAccountNoOk returns a tuple with the TradingAccountNo field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetTradingAccountNo

`func (o *PiSetServiceApplicationCommandsReviewSblOrderResponse) SetTradingAccountNo(v string)`

SetTradingAccountNo sets TradingAccountNo field to given value.


### SetTradingAccountNoNil

`func (o *PiSetServiceApplicationCommandsReviewSblOrderResponse) SetTradingAccountNoNil(b bool)`

 SetTradingAccountNoNil sets the value for TradingAccountNo to be an explicit nil

### UnsetTradingAccountNo
`func (o *PiSetServiceApplicationCommandsReviewSblOrderResponse) UnsetTradingAccountNo()`

UnsetTradingAccountNo ensures that no value is present for TradingAccountNo, not even an explicit nil
### GetCustomerCode

`func (o *PiSetServiceApplicationCommandsReviewSblOrderResponse) GetCustomerCode() string`

GetCustomerCode returns the CustomerCode field if non-nil, zero value otherwise.

### GetCustomerCodeOk

`func (o *PiSetServiceApplicationCommandsReviewSblOrderResponse) GetCustomerCodeOk() (*string, bool)`

GetCustomerCodeOk returns a tuple with the CustomerCode field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetCustomerCode

`func (o *PiSetServiceApplicationCommandsReviewSblOrderResponse) SetCustomerCode(v string)`

SetCustomerCode sets CustomerCode field to given value.


### SetCustomerCodeNil

`func (o *PiSetServiceApplicationCommandsReviewSblOrderResponse) SetCustomerCodeNil(b bool)`

 SetCustomerCodeNil sets the value for CustomerCode to be an explicit nil

### UnsetCustomerCode
`func (o *PiSetServiceApplicationCommandsReviewSblOrderResponse) UnsetCustomerCode()`

UnsetCustomerCode ensures that no value is present for CustomerCode, not even an explicit nil
### GetOrderId

`func (o *PiSetServiceApplicationCommandsReviewSblOrderResponse) GetOrderId() int64`

GetOrderId returns the OrderId field if non-nil, zero value otherwise.

### GetOrderIdOk

`func (o *PiSetServiceApplicationCommandsReviewSblOrderResponse) GetOrderIdOk() (*int64, bool)`

GetOrderIdOk returns a tuple with the OrderId field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetOrderId

`func (o *PiSetServiceApplicationCommandsReviewSblOrderResponse) SetOrderId(v int64)`

SetOrderId sets OrderId field to given value.


### GetSymbol

`func (o *PiSetServiceApplicationCommandsReviewSblOrderResponse) GetSymbol() string`

GetSymbol returns the Symbol field if non-nil, zero value otherwise.

### GetSymbolOk

`func (o *PiSetServiceApplicationCommandsReviewSblOrderResponse) GetSymbolOk() (*string, bool)`

GetSymbolOk returns a tuple with the Symbol field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetSymbol

`func (o *PiSetServiceApplicationCommandsReviewSblOrderResponse) SetSymbol(v string)`

SetSymbol sets Symbol field to given value.


### SetSymbolNil

`func (o *PiSetServiceApplicationCommandsReviewSblOrderResponse) SetSymbolNil(b bool)`

 SetSymbolNil sets the value for Symbol to be an explicit nil

### UnsetSymbol
`func (o *PiSetServiceApplicationCommandsReviewSblOrderResponse) UnsetSymbol()`

UnsetSymbol ensures that no value is present for Symbol, not even an explicit nil
### GetType

`func (o *PiSetServiceApplicationCommandsReviewSblOrderResponse) GetType() PiSetServiceDomainAggregatesModelFinancialAssetAggregateSblOrderType`

GetType returns the Type field if non-nil, zero value otherwise.

### GetTypeOk

`func (o *PiSetServiceApplicationCommandsReviewSblOrderResponse) GetTypeOk() (*PiSetServiceDomainAggregatesModelFinancialAssetAggregateSblOrderType, bool)`

GetTypeOk returns a tuple with the Type field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetType

`func (o *PiSetServiceApplicationCommandsReviewSblOrderResponse) SetType(v PiSetServiceDomainAggregatesModelFinancialAssetAggregateSblOrderType)`

SetType sets Type field to given value.


### GetVolume

`func (o *PiSetServiceApplicationCommandsReviewSblOrderResponse) GetVolume() string`

GetVolume returns the Volume field if non-nil, zero value otherwise.

### GetVolumeOk

`func (o *PiSetServiceApplicationCommandsReviewSblOrderResponse) GetVolumeOk() (*string, bool)`

GetVolumeOk returns a tuple with the Volume field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetVolume

`func (o *PiSetServiceApplicationCommandsReviewSblOrderResponse) SetVolume(v string)`

SetVolume sets Volume field to given value.


### GetStatus

`func (o *PiSetServiceApplicationCommandsReviewSblOrderResponse) GetStatus() PiSetServiceDomainAggregatesModelFinancialAssetAggregateSblOrderStatus`

GetStatus returns the Status field if non-nil, zero value otherwise.

### GetStatusOk

`func (o *PiSetServiceApplicationCommandsReviewSblOrderResponse) GetStatusOk() (*PiSetServiceDomainAggregatesModelFinancialAssetAggregateSblOrderStatus, bool)`

GetStatusOk returns a tuple with the Status field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetStatus

`func (o *PiSetServiceApplicationCommandsReviewSblOrderResponse) SetStatus(v PiSetServiceDomainAggregatesModelFinancialAssetAggregateSblOrderStatus)`

SetStatus sets Status field to given value.


### GetRejectedReason

`func (o *PiSetServiceApplicationCommandsReviewSblOrderResponse) GetRejectedReason() string`

GetRejectedReason returns the RejectedReason field if non-nil, zero value otherwise.

### GetRejectedReasonOk

`func (o *PiSetServiceApplicationCommandsReviewSblOrderResponse) GetRejectedReasonOk() (*string, bool)`

GetRejectedReasonOk returns a tuple with the RejectedReason field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetRejectedReason

`func (o *PiSetServiceApplicationCommandsReviewSblOrderResponse) SetRejectedReason(v string)`

SetRejectedReason sets RejectedReason field to given value.

### HasRejectedReason

`func (o *PiSetServiceApplicationCommandsReviewSblOrderResponse) HasRejectedReason() bool`

HasRejectedReason returns a boolean if a field has been set.

### SetRejectedReasonNil

`func (o *PiSetServiceApplicationCommandsReviewSblOrderResponse) SetRejectedReasonNil(b bool)`

 SetRejectedReasonNil sets the value for RejectedReason to be an explicit nil

### UnsetRejectedReason
`func (o *PiSetServiceApplicationCommandsReviewSblOrderResponse) UnsetRejectedReason()`

UnsetRejectedReason ensures that no value is present for RejectedReason, not even an explicit nil
### GetReviewerId

`func (o *PiSetServiceApplicationCommandsReviewSblOrderResponse) GetReviewerId() string`

GetReviewerId returns the ReviewerId field if non-nil, zero value otherwise.

### GetReviewerIdOk

`func (o *PiSetServiceApplicationCommandsReviewSblOrderResponse) GetReviewerIdOk() (*string, bool)`

GetReviewerIdOk returns a tuple with the ReviewerId field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetReviewerId

`func (o *PiSetServiceApplicationCommandsReviewSblOrderResponse) SetReviewerId(v string)`

SetReviewerId sets ReviewerId field to given value.

### HasReviewerId

`func (o *PiSetServiceApplicationCommandsReviewSblOrderResponse) HasReviewerId() bool`

HasReviewerId returns a boolean if a field has been set.

### SetReviewerIdNil

`func (o *PiSetServiceApplicationCommandsReviewSblOrderResponse) SetReviewerIdNil(b bool)`

 SetReviewerIdNil sets the value for ReviewerId to be an explicit nil

### UnsetReviewerId
`func (o *PiSetServiceApplicationCommandsReviewSblOrderResponse) UnsetReviewerId()`

UnsetReviewerId ensures that no value is present for ReviewerId, not even an explicit nil
### GetCreatedAt

`func (o *PiSetServiceApplicationCommandsReviewSblOrderResponse) GetCreatedAt() time.Time`

GetCreatedAt returns the CreatedAt field if non-nil, zero value otherwise.

### GetCreatedAtOk

`func (o *PiSetServiceApplicationCommandsReviewSblOrderResponse) GetCreatedAtOk() (*time.Time, bool)`

GetCreatedAtOk returns a tuple with the CreatedAt field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetCreatedAt

`func (o *PiSetServiceApplicationCommandsReviewSblOrderResponse) SetCreatedAt(v time.Time)`

SetCreatedAt sets CreatedAt field to given value.

### HasCreatedAt

`func (o *PiSetServiceApplicationCommandsReviewSblOrderResponse) HasCreatedAt() bool`

HasCreatedAt returns a boolean if a field has been set.

### GetUpdatedAt

`func (o *PiSetServiceApplicationCommandsReviewSblOrderResponse) GetUpdatedAt() time.Time`

GetUpdatedAt returns the UpdatedAt field if non-nil, zero value otherwise.

### GetUpdatedAtOk

`func (o *PiSetServiceApplicationCommandsReviewSblOrderResponse) GetUpdatedAtOk() (*time.Time, bool)`

GetUpdatedAtOk returns a tuple with the UpdatedAt field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetUpdatedAt

`func (o *PiSetServiceApplicationCommandsReviewSblOrderResponse) SetUpdatedAt(v time.Time)`

SetUpdatedAt sets UpdatedAt field to given value.

### HasUpdatedAt

`func (o *PiSetServiceApplicationCommandsReviewSblOrderResponse) HasUpdatedAt() bool`

HasUpdatedAt returns a boolean if a field has been set.

### SetUpdatedAtNil

`func (o *PiSetServiceApplicationCommandsReviewSblOrderResponse) SetUpdatedAtNil(b bool)`

 SetUpdatedAtNil sets the value for UpdatedAt to be an explicit nil

### UnsetUpdatedAt
`func (o *PiSetServiceApplicationCommandsReviewSblOrderResponse) UnsetUpdatedAt()`

UnsetUpdatedAt ensures that no value is present for UpdatedAt, not even an explicit nil

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


