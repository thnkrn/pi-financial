# PiUserDomainAggregatesModelUserInfoAggregateTradingAccount

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**RowVersion** | **string** |  | [readonly] 
**Id** | Pointer to **string** |  | [optional] [readonly] 
**TradingAccountId** | Pointer to **NullableString** |  | [optional] 
**AcctCode** | Pointer to **NullableString** |  | [optional] 
**UserInfoId** | Pointer to **NullableString** |  | [optional] [readonly] 
**UserInfo** | Pointer to [**PiUserDomainAggregatesModelUserInfoAggregateUserInfo**](PiUserDomainAggregatesModelUserInfoAggregateUserInfo.md) |  | [optional] 
**CreatedAt** | Pointer to **time.Time** |  | [optional] 
**UpdatedAt** | Pointer to **NullableTime** |  | [optional] 

## Methods

### NewPiUserDomainAggregatesModelUserInfoAggregateTradingAccount

`func NewPiUserDomainAggregatesModelUserInfoAggregateTradingAccount(rowVersion string, ) *PiUserDomainAggregatesModelUserInfoAggregateTradingAccount`

NewPiUserDomainAggregatesModelUserInfoAggregateTradingAccount instantiates a new PiUserDomainAggregatesModelUserInfoAggregateTradingAccount object
This constructor will assign default values to properties that have it defined,
and makes sure properties required by API are set, but the set of arguments
will change when the set of required properties is changed

### NewPiUserDomainAggregatesModelUserInfoAggregateTradingAccountWithDefaults

`func NewPiUserDomainAggregatesModelUserInfoAggregateTradingAccountWithDefaults() *PiUserDomainAggregatesModelUserInfoAggregateTradingAccount`

NewPiUserDomainAggregatesModelUserInfoAggregateTradingAccountWithDefaults instantiates a new PiUserDomainAggregatesModelUserInfoAggregateTradingAccount object
This constructor will only assign default values to properties that have it defined,
but it doesn't guarantee that properties required by API are set

### GetRowVersion

`func (o *PiUserDomainAggregatesModelUserInfoAggregateTradingAccount) GetRowVersion() string`

GetRowVersion returns the RowVersion field if non-nil, zero value otherwise.

### GetRowVersionOk

`func (o *PiUserDomainAggregatesModelUserInfoAggregateTradingAccount) GetRowVersionOk() (*string, bool)`

GetRowVersionOk returns a tuple with the RowVersion field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetRowVersion

`func (o *PiUserDomainAggregatesModelUserInfoAggregateTradingAccount) SetRowVersion(v string)`

SetRowVersion sets RowVersion field to given value.


### GetId

`func (o *PiUserDomainAggregatesModelUserInfoAggregateTradingAccount) GetId() string`

GetId returns the Id field if non-nil, zero value otherwise.

### GetIdOk

`func (o *PiUserDomainAggregatesModelUserInfoAggregateTradingAccount) GetIdOk() (*string, bool)`

GetIdOk returns a tuple with the Id field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetId

`func (o *PiUserDomainAggregatesModelUserInfoAggregateTradingAccount) SetId(v string)`

SetId sets Id field to given value.

### HasId

`func (o *PiUserDomainAggregatesModelUserInfoAggregateTradingAccount) HasId() bool`

HasId returns a boolean if a field has been set.

### GetTradingAccountId

`func (o *PiUserDomainAggregatesModelUserInfoAggregateTradingAccount) GetTradingAccountId() string`

GetTradingAccountId returns the TradingAccountId field if non-nil, zero value otherwise.

### GetTradingAccountIdOk

`func (o *PiUserDomainAggregatesModelUserInfoAggregateTradingAccount) GetTradingAccountIdOk() (*string, bool)`

GetTradingAccountIdOk returns a tuple with the TradingAccountId field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetTradingAccountId

`func (o *PiUserDomainAggregatesModelUserInfoAggregateTradingAccount) SetTradingAccountId(v string)`

SetTradingAccountId sets TradingAccountId field to given value.

### HasTradingAccountId

`func (o *PiUserDomainAggregatesModelUserInfoAggregateTradingAccount) HasTradingAccountId() bool`

HasTradingAccountId returns a boolean if a field has been set.

### SetTradingAccountIdNil

`func (o *PiUserDomainAggregatesModelUserInfoAggregateTradingAccount) SetTradingAccountIdNil(b bool)`

 SetTradingAccountIdNil sets the value for TradingAccountId to be an explicit nil

### UnsetTradingAccountId
`func (o *PiUserDomainAggregatesModelUserInfoAggregateTradingAccount) UnsetTradingAccountId()`

UnsetTradingAccountId ensures that no value is present for TradingAccountId, not even an explicit nil
### GetAcctCode

`func (o *PiUserDomainAggregatesModelUserInfoAggregateTradingAccount) GetAcctCode() string`

GetAcctCode returns the AcctCode field if non-nil, zero value otherwise.

### GetAcctCodeOk

`func (o *PiUserDomainAggregatesModelUserInfoAggregateTradingAccount) GetAcctCodeOk() (*string, bool)`

GetAcctCodeOk returns a tuple with the AcctCode field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetAcctCode

`func (o *PiUserDomainAggregatesModelUserInfoAggregateTradingAccount) SetAcctCode(v string)`

SetAcctCode sets AcctCode field to given value.

### HasAcctCode

`func (o *PiUserDomainAggregatesModelUserInfoAggregateTradingAccount) HasAcctCode() bool`

HasAcctCode returns a boolean if a field has been set.

### SetAcctCodeNil

`func (o *PiUserDomainAggregatesModelUserInfoAggregateTradingAccount) SetAcctCodeNil(b bool)`

 SetAcctCodeNil sets the value for AcctCode to be an explicit nil

### UnsetAcctCode
`func (o *PiUserDomainAggregatesModelUserInfoAggregateTradingAccount) UnsetAcctCode()`

UnsetAcctCode ensures that no value is present for AcctCode, not even an explicit nil
### GetUserInfoId

`func (o *PiUserDomainAggregatesModelUserInfoAggregateTradingAccount) GetUserInfoId() string`

GetUserInfoId returns the UserInfoId field if non-nil, zero value otherwise.

### GetUserInfoIdOk

`func (o *PiUserDomainAggregatesModelUserInfoAggregateTradingAccount) GetUserInfoIdOk() (*string, bool)`

GetUserInfoIdOk returns a tuple with the UserInfoId field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetUserInfoId

`func (o *PiUserDomainAggregatesModelUserInfoAggregateTradingAccount) SetUserInfoId(v string)`

SetUserInfoId sets UserInfoId field to given value.

### HasUserInfoId

`func (o *PiUserDomainAggregatesModelUserInfoAggregateTradingAccount) HasUserInfoId() bool`

HasUserInfoId returns a boolean if a field has been set.

### SetUserInfoIdNil

`func (o *PiUserDomainAggregatesModelUserInfoAggregateTradingAccount) SetUserInfoIdNil(b bool)`

 SetUserInfoIdNil sets the value for UserInfoId to be an explicit nil

### UnsetUserInfoId
`func (o *PiUserDomainAggregatesModelUserInfoAggregateTradingAccount) UnsetUserInfoId()`

UnsetUserInfoId ensures that no value is present for UserInfoId, not even an explicit nil
### GetUserInfo

`func (o *PiUserDomainAggregatesModelUserInfoAggregateTradingAccount) GetUserInfo() PiUserDomainAggregatesModelUserInfoAggregateUserInfo`

GetUserInfo returns the UserInfo field if non-nil, zero value otherwise.

### GetUserInfoOk

`func (o *PiUserDomainAggregatesModelUserInfoAggregateTradingAccount) GetUserInfoOk() (*PiUserDomainAggregatesModelUserInfoAggregateUserInfo, bool)`

GetUserInfoOk returns a tuple with the UserInfo field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetUserInfo

`func (o *PiUserDomainAggregatesModelUserInfoAggregateTradingAccount) SetUserInfo(v PiUserDomainAggregatesModelUserInfoAggregateUserInfo)`

SetUserInfo sets UserInfo field to given value.

### HasUserInfo

`func (o *PiUserDomainAggregatesModelUserInfoAggregateTradingAccount) HasUserInfo() bool`

HasUserInfo returns a boolean if a field has been set.

### GetCreatedAt

`func (o *PiUserDomainAggregatesModelUserInfoAggregateTradingAccount) GetCreatedAt() time.Time`

GetCreatedAt returns the CreatedAt field if non-nil, zero value otherwise.

### GetCreatedAtOk

`func (o *PiUserDomainAggregatesModelUserInfoAggregateTradingAccount) GetCreatedAtOk() (*time.Time, bool)`

GetCreatedAtOk returns a tuple with the CreatedAt field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetCreatedAt

`func (o *PiUserDomainAggregatesModelUserInfoAggregateTradingAccount) SetCreatedAt(v time.Time)`

SetCreatedAt sets CreatedAt field to given value.

### HasCreatedAt

`func (o *PiUserDomainAggregatesModelUserInfoAggregateTradingAccount) HasCreatedAt() bool`

HasCreatedAt returns a boolean if a field has been set.

### GetUpdatedAt

`func (o *PiUserDomainAggregatesModelUserInfoAggregateTradingAccount) GetUpdatedAt() time.Time`

GetUpdatedAt returns the UpdatedAt field if non-nil, zero value otherwise.

### GetUpdatedAtOk

`func (o *PiUserDomainAggregatesModelUserInfoAggregateTradingAccount) GetUpdatedAtOk() (*time.Time, bool)`

GetUpdatedAtOk returns a tuple with the UpdatedAt field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetUpdatedAt

`func (o *PiUserDomainAggregatesModelUserInfoAggregateTradingAccount) SetUpdatedAt(v time.Time)`

SetUpdatedAt sets UpdatedAt field to given value.

### HasUpdatedAt

`func (o *PiUserDomainAggregatesModelUserInfoAggregateTradingAccount) HasUpdatedAt() bool`

HasUpdatedAt returns a boolean if a field has been set.

### SetUpdatedAtNil

`func (o *PiUserDomainAggregatesModelUserInfoAggregateTradingAccount) SetUpdatedAtNil(b bool)`

 SetUpdatedAtNil sets the value for UpdatedAt to be an explicit nil

### UnsetUpdatedAt
`func (o *PiUserDomainAggregatesModelUserInfoAggregateTradingAccount) UnsetUpdatedAt()`

UnsetUpdatedAt ensures that no value is present for UpdatedAt, not even an explicit nil

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


