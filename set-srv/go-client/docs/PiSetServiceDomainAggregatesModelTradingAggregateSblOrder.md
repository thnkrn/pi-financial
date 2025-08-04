# PiSetServiceDomainAggregatesModelTradingAggregateSblOrder

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**Id** | Pointer to **string** |  | [optional] 
**TradingAccountId** | Pointer to **string** |  | [optional] 
**TradingAccountNo** | Pointer to **NullableString** |  | [optional] 
**CustomerCode** | Pointer to **NullableString** |  | [optional] 
**OrderId** | Pointer to **int64** |  | [optional] 
**Symbol** | Pointer to **NullableString** |  | [optional] 
**Type** | Pointer to [**PiSetServiceDomainAggregatesModelFinancialAssetAggregateSblOrderType**](PiSetServiceDomainAggregatesModelFinancialAssetAggregateSblOrderType.md) |  | [optional] 
**Volume** | Pointer to **string** |  | [optional] 
**Status** | Pointer to [**PiSetServiceDomainAggregatesModelFinancialAssetAggregateSblOrderStatus**](PiSetServiceDomainAggregatesModelFinancialAssetAggregateSblOrderStatus.md) |  | [optional] 
**RejectedReason** | Pointer to **NullableString** |  | [optional] 
**ReviewerId** | Pointer to **NullableString** |  | [optional] 
**CreatedAt** | Pointer to **time.Time** |  | [optional] 
**CreatedAtDate** | Pointer to **string** |  | [optional] 
**UpdatedAt** | Pointer to **NullableTime** |  | [optional] 

## Methods

### NewPiSetServiceDomainAggregatesModelTradingAggregateSblOrder

`func NewPiSetServiceDomainAggregatesModelTradingAggregateSblOrder() *PiSetServiceDomainAggregatesModelTradingAggregateSblOrder`

NewPiSetServiceDomainAggregatesModelTradingAggregateSblOrder instantiates a new PiSetServiceDomainAggregatesModelTradingAggregateSblOrder object
This constructor will assign default values to properties that have it defined,
and makes sure properties required by API are set, but the set of arguments
will change when the set of required properties is changed

### NewPiSetServiceDomainAggregatesModelTradingAggregateSblOrderWithDefaults

`func NewPiSetServiceDomainAggregatesModelTradingAggregateSblOrderWithDefaults() *PiSetServiceDomainAggregatesModelTradingAggregateSblOrder`

NewPiSetServiceDomainAggregatesModelTradingAggregateSblOrderWithDefaults instantiates a new PiSetServiceDomainAggregatesModelTradingAggregateSblOrder object
This constructor will only assign default values to properties that have it defined,
but it doesn't guarantee that properties required by API are set

### GetId

`func (o *PiSetServiceDomainAggregatesModelTradingAggregateSblOrder) GetId() string`

GetId returns the Id field if non-nil, zero value otherwise.

### GetIdOk

`func (o *PiSetServiceDomainAggregatesModelTradingAggregateSblOrder) GetIdOk() (*string, bool)`

GetIdOk returns a tuple with the Id field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetId

`func (o *PiSetServiceDomainAggregatesModelTradingAggregateSblOrder) SetId(v string)`

SetId sets Id field to given value.

### HasId

`func (o *PiSetServiceDomainAggregatesModelTradingAggregateSblOrder) HasId() bool`

HasId returns a boolean if a field has been set.

### GetTradingAccountId

`func (o *PiSetServiceDomainAggregatesModelTradingAggregateSblOrder) GetTradingAccountId() string`

GetTradingAccountId returns the TradingAccountId field if non-nil, zero value otherwise.

### GetTradingAccountIdOk

`func (o *PiSetServiceDomainAggregatesModelTradingAggregateSblOrder) GetTradingAccountIdOk() (*string, bool)`

GetTradingAccountIdOk returns a tuple with the TradingAccountId field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetTradingAccountId

`func (o *PiSetServiceDomainAggregatesModelTradingAggregateSblOrder) SetTradingAccountId(v string)`

SetTradingAccountId sets TradingAccountId field to given value.

### HasTradingAccountId

`func (o *PiSetServiceDomainAggregatesModelTradingAggregateSblOrder) HasTradingAccountId() bool`

HasTradingAccountId returns a boolean if a field has been set.

### GetTradingAccountNo

`func (o *PiSetServiceDomainAggregatesModelTradingAggregateSblOrder) GetTradingAccountNo() string`

GetTradingAccountNo returns the TradingAccountNo field if non-nil, zero value otherwise.

### GetTradingAccountNoOk

`func (o *PiSetServiceDomainAggregatesModelTradingAggregateSblOrder) GetTradingAccountNoOk() (*string, bool)`

GetTradingAccountNoOk returns a tuple with the TradingAccountNo field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetTradingAccountNo

`func (o *PiSetServiceDomainAggregatesModelTradingAggregateSblOrder) SetTradingAccountNo(v string)`

SetTradingAccountNo sets TradingAccountNo field to given value.

### HasTradingAccountNo

`func (o *PiSetServiceDomainAggregatesModelTradingAggregateSblOrder) HasTradingAccountNo() bool`

HasTradingAccountNo returns a boolean if a field has been set.

### SetTradingAccountNoNil

`func (o *PiSetServiceDomainAggregatesModelTradingAggregateSblOrder) SetTradingAccountNoNil(b bool)`

 SetTradingAccountNoNil sets the value for TradingAccountNo to be an explicit nil

### UnsetTradingAccountNo
`func (o *PiSetServiceDomainAggregatesModelTradingAggregateSblOrder) UnsetTradingAccountNo()`

UnsetTradingAccountNo ensures that no value is present for TradingAccountNo, not even an explicit nil
### GetCustomerCode

`func (o *PiSetServiceDomainAggregatesModelTradingAggregateSblOrder) GetCustomerCode() string`

GetCustomerCode returns the CustomerCode field if non-nil, zero value otherwise.

### GetCustomerCodeOk

`func (o *PiSetServiceDomainAggregatesModelTradingAggregateSblOrder) GetCustomerCodeOk() (*string, bool)`

GetCustomerCodeOk returns a tuple with the CustomerCode field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetCustomerCode

`func (o *PiSetServiceDomainAggregatesModelTradingAggregateSblOrder) SetCustomerCode(v string)`

SetCustomerCode sets CustomerCode field to given value.

### HasCustomerCode

`func (o *PiSetServiceDomainAggregatesModelTradingAggregateSblOrder) HasCustomerCode() bool`

HasCustomerCode returns a boolean if a field has been set.

### SetCustomerCodeNil

`func (o *PiSetServiceDomainAggregatesModelTradingAggregateSblOrder) SetCustomerCodeNil(b bool)`

 SetCustomerCodeNil sets the value for CustomerCode to be an explicit nil

### UnsetCustomerCode
`func (o *PiSetServiceDomainAggregatesModelTradingAggregateSblOrder) UnsetCustomerCode()`

UnsetCustomerCode ensures that no value is present for CustomerCode, not even an explicit nil
### GetOrderId

`func (o *PiSetServiceDomainAggregatesModelTradingAggregateSblOrder) GetOrderId() int64`

GetOrderId returns the OrderId field if non-nil, zero value otherwise.

### GetOrderIdOk

`func (o *PiSetServiceDomainAggregatesModelTradingAggregateSblOrder) GetOrderIdOk() (*int64, bool)`

GetOrderIdOk returns a tuple with the OrderId field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetOrderId

`func (o *PiSetServiceDomainAggregatesModelTradingAggregateSblOrder) SetOrderId(v int64)`

SetOrderId sets OrderId field to given value.

### HasOrderId

`func (o *PiSetServiceDomainAggregatesModelTradingAggregateSblOrder) HasOrderId() bool`

HasOrderId returns a boolean if a field has been set.

### GetSymbol

`func (o *PiSetServiceDomainAggregatesModelTradingAggregateSblOrder) GetSymbol() string`

GetSymbol returns the Symbol field if non-nil, zero value otherwise.

### GetSymbolOk

`func (o *PiSetServiceDomainAggregatesModelTradingAggregateSblOrder) GetSymbolOk() (*string, bool)`

GetSymbolOk returns a tuple with the Symbol field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetSymbol

`func (o *PiSetServiceDomainAggregatesModelTradingAggregateSblOrder) SetSymbol(v string)`

SetSymbol sets Symbol field to given value.

### HasSymbol

`func (o *PiSetServiceDomainAggregatesModelTradingAggregateSblOrder) HasSymbol() bool`

HasSymbol returns a boolean if a field has been set.

### SetSymbolNil

`func (o *PiSetServiceDomainAggregatesModelTradingAggregateSblOrder) SetSymbolNil(b bool)`

 SetSymbolNil sets the value for Symbol to be an explicit nil

### UnsetSymbol
`func (o *PiSetServiceDomainAggregatesModelTradingAggregateSblOrder) UnsetSymbol()`

UnsetSymbol ensures that no value is present for Symbol, not even an explicit nil
### GetType

`func (o *PiSetServiceDomainAggregatesModelTradingAggregateSblOrder) GetType() PiSetServiceDomainAggregatesModelFinancialAssetAggregateSblOrderType`

GetType returns the Type field if non-nil, zero value otherwise.

### GetTypeOk

`func (o *PiSetServiceDomainAggregatesModelTradingAggregateSblOrder) GetTypeOk() (*PiSetServiceDomainAggregatesModelFinancialAssetAggregateSblOrderType, bool)`

GetTypeOk returns a tuple with the Type field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetType

`func (o *PiSetServiceDomainAggregatesModelTradingAggregateSblOrder) SetType(v PiSetServiceDomainAggregatesModelFinancialAssetAggregateSblOrderType)`

SetType sets Type field to given value.

### HasType

`func (o *PiSetServiceDomainAggregatesModelTradingAggregateSblOrder) HasType() bool`

HasType returns a boolean if a field has been set.

### GetVolume

`func (o *PiSetServiceDomainAggregatesModelTradingAggregateSblOrder) GetVolume() string`

GetVolume returns the Volume field if non-nil, zero value otherwise.

### GetVolumeOk

`func (o *PiSetServiceDomainAggregatesModelTradingAggregateSblOrder) GetVolumeOk() (*string, bool)`

GetVolumeOk returns a tuple with the Volume field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetVolume

`func (o *PiSetServiceDomainAggregatesModelTradingAggregateSblOrder) SetVolume(v string)`

SetVolume sets Volume field to given value.

### HasVolume

`func (o *PiSetServiceDomainAggregatesModelTradingAggregateSblOrder) HasVolume() bool`

HasVolume returns a boolean if a field has been set.

### GetStatus

`func (o *PiSetServiceDomainAggregatesModelTradingAggregateSblOrder) GetStatus() PiSetServiceDomainAggregatesModelFinancialAssetAggregateSblOrderStatus`

GetStatus returns the Status field if non-nil, zero value otherwise.

### GetStatusOk

`func (o *PiSetServiceDomainAggregatesModelTradingAggregateSblOrder) GetStatusOk() (*PiSetServiceDomainAggregatesModelFinancialAssetAggregateSblOrderStatus, bool)`

GetStatusOk returns a tuple with the Status field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetStatus

`func (o *PiSetServiceDomainAggregatesModelTradingAggregateSblOrder) SetStatus(v PiSetServiceDomainAggregatesModelFinancialAssetAggregateSblOrderStatus)`

SetStatus sets Status field to given value.

### HasStatus

`func (o *PiSetServiceDomainAggregatesModelTradingAggregateSblOrder) HasStatus() bool`

HasStatus returns a boolean if a field has been set.

### GetRejectedReason

`func (o *PiSetServiceDomainAggregatesModelTradingAggregateSblOrder) GetRejectedReason() string`

GetRejectedReason returns the RejectedReason field if non-nil, zero value otherwise.

### GetRejectedReasonOk

`func (o *PiSetServiceDomainAggregatesModelTradingAggregateSblOrder) GetRejectedReasonOk() (*string, bool)`

GetRejectedReasonOk returns a tuple with the RejectedReason field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetRejectedReason

`func (o *PiSetServiceDomainAggregatesModelTradingAggregateSblOrder) SetRejectedReason(v string)`

SetRejectedReason sets RejectedReason field to given value.

### HasRejectedReason

`func (o *PiSetServiceDomainAggregatesModelTradingAggregateSblOrder) HasRejectedReason() bool`

HasRejectedReason returns a boolean if a field has been set.

### SetRejectedReasonNil

`func (o *PiSetServiceDomainAggregatesModelTradingAggregateSblOrder) SetRejectedReasonNil(b bool)`

 SetRejectedReasonNil sets the value for RejectedReason to be an explicit nil

### UnsetRejectedReason
`func (o *PiSetServiceDomainAggregatesModelTradingAggregateSblOrder) UnsetRejectedReason()`

UnsetRejectedReason ensures that no value is present for RejectedReason, not even an explicit nil
### GetReviewerId

`func (o *PiSetServiceDomainAggregatesModelTradingAggregateSblOrder) GetReviewerId() string`

GetReviewerId returns the ReviewerId field if non-nil, zero value otherwise.

### GetReviewerIdOk

`func (o *PiSetServiceDomainAggregatesModelTradingAggregateSblOrder) GetReviewerIdOk() (*string, bool)`

GetReviewerIdOk returns a tuple with the ReviewerId field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetReviewerId

`func (o *PiSetServiceDomainAggregatesModelTradingAggregateSblOrder) SetReviewerId(v string)`

SetReviewerId sets ReviewerId field to given value.

### HasReviewerId

`func (o *PiSetServiceDomainAggregatesModelTradingAggregateSblOrder) HasReviewerId() bool`

HasReviewerId returns a boolean if a field has been set.

### SetReviewerIdNil

`func (o *PiSetServiceDomainAggregatesModelTradingAggregateSblOrder) SetReviewerIdNil(b bool)`

 SetReviewerIdNil sets the value for ReviewerId to be an explicit nil

### UnsetReviewerId
`func (o *PiSetServiceDomainAggregatesModelTradingAggregateSblOrder) UnsetReviewerId()`

UnsetReviewerId ensures that no value is present for ReviewerId, not even an explicit nil
### GetCreatedAt

`func (o *PiSetServiceDomainAggregatesModelTradingAggregateSblOrder) GetCreatedAt() time.Time`

GetCreatedAt returns the CreatedAt field if non-nil, zero value otherwise.

### GetCreatedAtOk

`func (o *PiSetServiceDomainAggregatesModelTradingAggregateSblOrder) GetCreatedAtOk() (*time.Time, bool)`

GetCreatedAtOk returns a tuple with the CreatedAt field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetCreatedAt

`func (o *PiSetServiceDomainAggregatesModelTradingAggregateSblOrder) SetCreatedAt(v time.Time)`

SetCreatedAt sets CreatedAt field to given value.

### HasCreatedAt

`func (o *PiSetServiceDomainAggregatesModelTradingAggregateSblOrder) HasCreatedAt() bool`

HasCreatedAt returns a boolean if a field has been set.

### GetCreatedAtDate

`func (o *PiSetServiceDomainAggregatesModelTradingAggregateSblOrder) GetCreatedAtDate() string`

GetCreatedAtDate returns the CreatedAtDate field if non-nil, zero value otherwise.

### GetCreatedAtDateOk

`func (o *PiSetServiceDomainAggregatesModelTradingAggregateSblOrder) GetCreatedAtDateOk() (*string, bool)`

GetCreatedAtDateOk returns a tuple with the CreatedAtDate field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetCreatedAtDate

`func (o *PiSetServiceDomainAggregatesModelTradingAggregateSblOrder) SetCreatedAtDate(v string)`

SetCreatedAtDate sets CreatedAtDate field to given value.

### HasCreatedAtDate

`func (o *PiSetServiceDomainAggregatesModelTradingAggregateSblOrder) HasCreatedAtDate() bool`

HasCreatedAtDate returns a boolean if a field has been set.

### GetUpdatedAt

`func (o *PiSetServiceDomainAggregatesModelTradingAggregateSblOrder) GetUpdatedAt() time.Time`

GetUpdatedAt returns the UpdatedAt field if non-nil, zero value otherwise.

### GetUpdatedAtOk

`func (o *PiSetServiceDomainAggregatesModelTradingAggregateSblOrder) GetUpdatedAtOk() (*time.Time, bool)`

GetUpdatedAtOk returns a tuple with the UpdatedAt field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetUpdatedAt

`func (o *PiSetServiceDomainAggregatesModelTradingAggregateSblOrder) SetUpdatedAt(v time.Time)`

SetUpdatedAt sets UpdatedAt field to given value.

### HasUpdatedAt

`func (o *PiSetServiceDomainAggregatesModelTradingAggregateSblOrder) HasUpdatedAt() bool`

HasUpdatedAt returns a boolean if a field has been set.

### SetUpdatedAtNil

`func (o *PiSetServiceDomainAggregatesModelTradingAggregateSblOrder) SetUpdatedAtNil(b bool)`

 SetUpdatedAtNil sets the value for UpdatedAt to be an explicit nil

### UnsetUpdatedAt
`func (o *PiSetServiceDomainAggregatesModelTradingAggregateSblOrder) UnsetUpdatedAt()`

UnsetUpdatedAt ensures that no value is present for UpdatedAt, not even an explicit nil

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


