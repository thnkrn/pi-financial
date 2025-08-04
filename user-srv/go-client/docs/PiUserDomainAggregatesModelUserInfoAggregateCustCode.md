# PiUserDomainAggregatesModelUserInfoAggregateCustCode

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**RowVersion** | **string** |  | [readonly] 
**Id** | Pointer to **string** |  | [optional] [readonly] 
**CustomerCode** | Pointer to **NullableString** |  | [optional] 
**UserInfoId** | Pointer to **NullableString** |  | [optional] [readonly] 
**UserInfo** | Pointer to [**PiUserDomainAggregatesModelUserInfoAggregateUserInfo**](PiUserDomainAggregatesModelUserInfoAggregateUserInfo.md) |  | [optional] 
**CreatedAt** | Pointer to **time.Time** |  | [optional] 
**UpdatedAt** | Pointer to **NullableTime** |  | [optional] 

## Methods

### NewPiUserDomainAggregatesModelUserInfoAggregateCustCode

`func NewPiUserDomainAggregatesModelUserInfoAggregateCustCode(rowVersion string, ) *PiUserDomainAggregatesModelUserInfoAggregateCustCode`

NewPiUserDomainAggregatesModelUserInfoAggregateCustCode instantiates a new PiUserDomainAggregatesModelUserInfoAggregateCustCode object
This constructor will assign default values to properties that have it defined,
and makes sure properties required by API are set, but the set of arguments
will change when the set of required properties is changed

### NewPiUserDomainAggregatesModelUserInfoAggregateCustCodeWithDefaults

`func NewPiUserDomainAggregatesModelUserInfoAggregateCustCodeWithDefaults() *PiUserDomainAggregatesModelUserInfoAggregateCustCode`

NewPiUserDomainAggregatesModelUserInfoAggregateCustCodeWithDefaults instantiates a new PiUserDomainAggregatesModelUserInfoAggregateCustCode object
This constructor will only assign default values to properties that have it defined,
but it doesn't guarantee that properties required by API are set

### GetRowVersion

`func (o *PiUserDomainAggregatesModelUserInfoAggregateCustCode) GetRowVersion() string`

GetRowVersion returns the RowVersion field if non-nil, zero value otherwise.

### GetRowVersionOk

`func (o *PiUserDomainAggregatesModelUserInfoAggregateCustCode) GetRowVersionOk() (*string, bool)`

GetRowVersionOk returns a tuple with the RowVersion field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetRowVersion

`func (o *PiUserDomainAggregatesModelUserInfoAggregateCustCode) SetRowVersion(v string)`

SetRowVersion sets RowVersion field to given value.


### GetId

`func (o *PiUserDomainAggregatesModelUserInfoAggregateCustCode) GetId() string`

GetId returns the Id field if non-nil, zero value otherwise.

### GetIdOk

`func (o *PiUserDomainAggregatesModelUserInfoAggregateCustCode) GetIdOk() (*string, bool)`

GetIdOk returns a tuple with the Id field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetId

`func (o *PiUserDomainAggregatesModelUserInfoAggregateCustCode) SetId(v string)`

SetId sets Id field to given value.

### HasId

`func (o *PiUserDomainAggregatesModelUserInfoAggregateCustCode) HasId() bool`

HasId returns a boolean if a field has been set.

### GetCustomerCode

`func (o *PiUserDomainAggregatesModelUserInfoAggregateCustCode) GetCustomerCode() string`

GetCustomerCode returns the CustomerCode field if non-nil, zero value otherwise.

### GetCustomerCodeOk

`func (o *PiUserDomainAggregatesModelUserInfoAggregateCustCode) GetCustomerCodeOk() (*string, bool)`

GetCustomerCodeOk returns a tuple with the CustomerCode field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetCustomerCode

`func (o *PiUserDomainAggregatesModelUserInfoAggregateCustCode) SetCustomerCode(v string)`

SetCustomerCode sets CustomerCode field to given value.

### HasCustomerCode

`func (o *PiUserDomainAggregatesModelUserInfoAggregateCustCode) HasCustomerCode() bool`

HasCustomerCode returns a boolean if a field has been set.

### SetCustomerCodeNil

`func (o *PiUserDomainAggregatesModelUserInfoAggregateCustCode) SetCustomerCodeNil(b bool)`

 SetCustomerCodeNil sets the value for CustomerCode to be an explicit nil

### UnsetCustomerCode
`func (o *PiUserDomainAggregatesModelUserInfoAggregateCustCode) UnsetCustomerCode()`

UnsetCustomerCode ensures that no value is present for CustomerCode, not even an explicit nil
### GetUserInfoId

`func (o *PiUserDomainAggregatesModelUserInfoAggregateCustCode) GetUserInfoId() string`

GetUserInfoId returns the UserInfoId field if non-nil, zero value otherwise.

### GetUserInfoIdOk

`func (o *PiUserDomainAggregatesModelUserInfoAggregateCustCode) GetUserInfoIdOk() (*string, bool)`

GetUserInfoIdOk returns a tuple with the UserInfoId field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetUserInfoId

`func (o *PiUserDomainAggregatesModelUserInfoAggregateCustCode) SetUserInfoId(v string)`

SetUserInfoId sets UserInfoId field to given value.

### HasUserInfoId

`func (o *PiUserDomainAggregatesModelUserInfoAggregateCustCode) HasUserInfoId() bool`

HasUserInfoId returns a boolean if a field has been set.

### SetUserInfoIdNil

`func (o *PiUserDomainAggregatesModelUserInfoAggregateCustCode) SetUserInfoIdNil(b bool)`

 SetUserInfoIdNil sets the value for UserInfoId to be an explicit nil

### UnsetUserInfoId
`func (o *PiUserDomainAggregatesModelUserInfoAggregateCustCode) UnsetUserInfoId()`

UnsetUserInfoId ensures that no value is present for UserInfoId, not even an explicit nil
### GetUserInfo

`func (o *PiUserDomainAggregatesModelUserInfoAggregateCustCode) GetUserInfo() PiUserDomainAggregatesModelUserInfoAggregateUserInfo`

GetUserInfo returns the UserInfo field if non-nil, zero value otherwise.

### GetUserInfoOk

`func (o *PiUserDomainAggregatesModelUserInfoAggregateCustCode) GetUserInfoOk() (*PiUserDomainAggregatesModelUserInfoAggregateUserInfo, bool)`

GetUserInfoOk returns a tuple with the UserInfo field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetUserInfo

`func (o *PiUserDomainAggregatesModelUserInfoAggregateCustCode) SetUserInfo(v PiUserDomainAggregatesModelUserInfoAggregateUserInfo)`

SetUserInfo sets UserInfo field to given value.

### HasUserInfo

`func (o *PiUserDomainAggregatesModelUserInfoAggregateCustCode) HasUserInfo() bool`

HasUserInfo returns a boolean if a field has been set.

### GetCreatedAt

`func (o *PiUserDomainAggregatesModelUserInfoAggregateCustCode) GetCreatedAt() time.Time`

GetCreatedAt returns the CreatedAt field if non-nil, zero value otherwise.

### GetCreatedAtOk

`func (o *PiUserDomainAggregatesModelUserInfoAggregateCustCode) GetCreatedAtOk() (*time.Time, bool)`

GetCreatedAtOk returns a tuple with the CreatedAt field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetCreatedAt

`func (o *PiUserDomainAggregatesModelUserInfoAggregateCustCode) SetCreatedAt(v time.Time)`

SetCreatedAt sets CreatedAt field to given value.

### HasCreatedAt

`func (o *PiUserDomainAggregatesModelUserInfoAggregateCustCode) HasCreatedAt() bool`

HasCreatedAt returns a boolean if a field has been set.

### GetUpdatedAt

`func (o *PiUserDomainAggregatesModelUserInfoAggregateCustCode) GetUpdatedAt() time.Time`

GetUpdatedAt returns the UpdatedAt field if non-nil, zero value otherwise.

### GetUpdatedAtOk

`func (o *PiUserDomainAggregatesModelUserInfoAggregateCustCode) GetUpdatedAtOk() (*time.Time, bool)`

GetUpdatedAtOk returns a tuple with the UpdatedAt field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetUpdatedAt

`func (o *PiUserDomainAggregatesModelUserInfoAggregateCustCode) SetUpdatedAt(v time.Time)`

SetUpdatedAt sets UpdatedAt field to given value.

### HasUpdatedAt

`func (o *PiUserDomainAggregatesModelUserInfoAggregateCustCode) HasUpdatedAt() bool`

HasUpdatedAt returns a boolean if a field has been set.

### SetUpdatedAtNil

`func (o *PiUserDomainAggregatesModelUserInfoAggregateCustCode) SetUpdatedAtNil(b bool)`

 SetUpdatedAtNil sets the value for UpdatedAt to be an explicit nil

### UnsetUpdatedAt
`func (o *PiUserDomainAggregatesModelUserInfoAggregateCustCode) UnsetUpdatedAt()`

UnsetUpdatedAt ensures that no value is present for UpdatedAt, not even an explicit nil

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


