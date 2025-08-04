# PiUserDomainAggregatesModelUserInfoAggregateDevice

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**RowVersion** | **string** |  | [readonly] 
**Id** | Pointer to **string** |  | [optional] [readonly] 
**DeviceId** | Pointer to **string** |  | [optional] 
**DeviceToken** | Pointer to **NullableString** |  | [optional] 
**DeviceIdentifier** | Pointer to **NullableString** |  | [optional] 
**Language** | Pointer to **NullableString** |  | [optional] 
**Platform** | Pointer to **NullableString** |  | [optional] 
**IsActive** | Pointer to **bool** |  | [optional] [readonly] 
**SubscriptionIdentifier** | Pointer to **NullableString** |  | [optional] 
**NotificationPreference** | Pointer to [**PiUserDomainAggregatesModelUserInfoAggregateNotificationPreference**](PiUserDomainAggregatesModelUserInfoAggregateNotificationPreference.md) |  | [optional] 
**UserInfoId** | Pointer to **NullableString** |  | [optional] [readonly] 
**UserInfo** | Pointer to [**PiUserDomainAggregatesModelUserInfoAggregateUserInfo**](PiUserDomainAggregatesModelUserInfoAggregateUserInfo.md) |  | [optional] 
**CreatedAt** | Pointer to **time.Time** |  | [optional] 
**UpdatedAt** | Pointer to **NullableTime** |  | [optional] 

## Methods

### NewPiUserDomainAggregatesModelUserInfoAggregateDevice

`func NewPiUserDomainAggregatesModelUserInfoAggregateDevice(rowVersion string, ) *PiUserDomainAggregatesModelUserInfoAggregateDevice`

NewPiUserDomainAggregatesModelUserInfoAggregateDevice instantiates a new PiUserDomainAggregatesModelUserInfoAggregateDevice object
This constructor will assign default values to properties that have it defined,
and makes sure properties required by API are set, but the set of arguments
will change when the set of required properties is changed

### NewPiUserDomainAggregatesModelUserInfoAggregateDeviceWithDefaults

`func NewPiUserDomainAggregatesModelUserInfoAggregateDeviceWithDefaults() *PiUserDomainAggregatesModelUserInfoAggregateDevice`

NewPiUserDomainAggregatesModelUserInfoAggregateDeviceWithDefaults instantiates a new PiUserDomainAggregatesModelUserInfoAggregateDevice object
This constructor will only assign default values to properties that have it defined,
but it doesn't guarantee that properties required by API are set

### GetRowVersion

`func (o *PiUserDomainAggregatesModelUserInfoAggregateDevice) GetRowVersion() string`

GetRowVersion returns the RowVersion field if non-nil, zero value otherwise.

### GetRowVersionOk

`func (o *PiUserDomainAggregatesModelUserInfoAggregateDevice) GetRowVersionOk() (*string, bool)`

GetRowVersionOk returns a tuple with the RowVersion field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetRowVersion

`func (o *PiUserDomainAggregatesModelUserInfoAggregateDevice) SetRowVersion(v string)`

SetRowVersion sets RowVersion field to given value.


### GetId

`func (o *PiUserDomainAggregatesModelUserInfoAggregateDevice) GetId() string`

GetId returns the Id field if non-nil, zero value otherwise.

### GetIdOk

`func (o *PiUserDomainAggregatesModelUserInfoAggregateDevice) GetIdOk() (*string, bool)`

GetIdOk returns a tuple with the Id field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetId

`func (o *PiUserDomainAggregatesModelUserInfoAggregateDevice) SetId(v string)`

SetId sets Id field to given value.

### HasId

`func (o *PiUserDomainAggregatesModelUserInfoAggregateDevice) HasId() bool`

HasId returns a boolean if a field has been set.

### GetDeviceId

`func (o *PiUserDomainAggregatesModelUserInfoAggregateDevice) GetDeviceId() string`

GetDeviceId returns the DeviceId field if non-nil, zero value otherwise.

### GetDeviceIdOk

`func (o *PiUserDomainAggregatesModelUserInfoAggregateDevice) GetDeviceIdOk() (*string, bool)`

GetDeviceIdOk returns a tuple with the DeviceId field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetDeviceId

`func (o *PiUserDomainAggregatesModelUserInfoAggregateDevice) SetDeviceId(v string)`

SetDeviceId sets DeviceId field to given value.

### HasDeviceId

`func (o *PiUserDomainAggregatesModelUserInfoAggregateDevice) HasDeviceId() bool`

HasDeviceId returns a boolean if a field has been set.

### GetDeviceToken

`func (o *PiUserDomainAggregatesModelUserInfoAggregateDevice) GetDeviceToken() string`

GetDeviceToken returns the DeviceToken field if non-nil, zero value otherwise.

### GetDeviceTokenOk

`func (o *PiUserDomainAggregatesModelUserInfoAggregateDevice) GetDeviceTokenOk() (*string, bool)`

GetDeviceTokenOk returns a tuple with the DeviceToken field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetDeviceToken

`func (o *PiUserDomainAggregatesModelUserInfoAggregateDevice) SetDeviceToken(v string)`

SetDeviceToken sets DeviceToken field to given value.

### HasDeviceToken

`func (o *PiUserDomainAggregatesModelUserInfoAggregateDevice) HasDeviceToken() bool`

HasDeviceToken returns a boolean if a field has been set.

### SetDeviceTokenNil

`func (o *PiUserDomainAggregatesModelUserInfoAggregateDevice) SetDeviceTokenNil(b bool)`

 SetDeviceTokenNil sets the value for DeviceToken to be an explicit nil

### UnsetDeviceToken
`func (o *PiUserDomainAggregatesModelUserInfoAggregateDevice) UnsetDeviceToken()`

UnsetDeviceToken ensures that no value is present for DeviceToken, not even an explicit nil
### GetDeviceIdentifier

`func (o *PiUserDomainAggregatesModelUserInfoAggregateDevice) GetDeviceIdentifier() string`

GetDeviceIdentifier returns the DeviceIdentifier field if non-nil, zero value otherwise.

### GetDeviceIdentifierOk

`func (o *PiUserDomainAggregatesModelUserInfoAggregateDevice) GetDeviceIdentifierOk() (*string, bool)`

GetDeviceIdentifierOk returns a tuple with the DeviceIdentifier field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetDeviceIdentifier

`func (o *PiUserDomainAggregatesModelUserInfoAggregateDevice) SetDeviceIdentifier(v string)`

SetDeviceIdentifier sets DeviceIdentifier field to given value.

### HasDeviceIdentifier

`func (o *PiUserDomainAggregatesModelUserInfoAggregateDevice) HasDeviceIdentifier() bool`

HasDeviceIdentifier returns a boolean if a field has been set.

### SetDeviceIdentifierNil

`func (o *PiUserDomainAggregatesModelUserInfoAggregateDevice) SetDeviceIdentifierNil(b bool)`

 SetDeviceIdentifierNil sets the value for DeviceIdentifier to be an explicit nil

### UnsetDeviceIdentifier
`func (o *PiUserDomainAggregatesModelUserInfoAggregateDevice) UnsetDeviceIdentifier()`

UnsetDeviceIdentifier ensures that no value is present for DeviceIdentifier, not even an explicit nil
### GetLanguage

`func (o *PiUserDomainAggregatesModelUserInfoAggregateDevice) GetLanguage() string`

GetLanguage returns the Language field if non-nil, zero value otherwise.

### GetLanguageOk

`func (o *PiUserDomainAggregatesModelUserInfoAggregateDevice) GetLanguageOk() (*string, bool)`

GetLanguageOk returns a tuple with the Language field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetLanguage

`func (o *PiUserDomainAggregatesModelUserInfoAggregateDevice) SetLanguage(v string)`

SetLanguage sets Language field to given value.

### HasLanguage

`func (o *PiUserDomainAggregatesModelUserInfoAggregateDevice) HasLanguage() bool`

HasLanguage returns a boolean if a field has been set.

### SetLanguageNil

`func (o *PiUserDomainAggregatesModelUserInfoAggregateDevice) SetLanguageNil(b bool)`

 SetLanguageNil sets the value for Language to be an explicit nil

### UnsetLanguage
`func (o *PiUserDomainAggregatesModelUserInfoAggregateDevice) UnsetLanguage()`

UnsetLanguage ensures that no value is present for Language, not even an explicit nil
### GetPlatform

`func (o *PiUserDomainAggregatesModelUserInfoAggregateDevice) GetPlatform() string`

GetPlatform returns the Platform field if non-nil, zero value otherwise.

### GetPlatformOk

`func (o *PiUserDomainAggregatesModelUserInfoAggregateDevice) GetPlatformOk() (*string, bool)`

GetPlatformOk returns a tuple with the Platform field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetPlatform

`func (o *PiUserDomainAggregatesModelUserInfoAggregateDevice) SetPlatform(v string)`

SetPlatform sets Platform field to given value.

### HasPlatform

`func (o *PiUserDomainAggregatesModelUserInfoAggregateDevice) HasPlatform() bool`

HasPlatform returns a boolean if a field has been set.

### SetPlatformNil

`func (o *PiUserDomainAggregatesModelUserInfoAggregateDevice) SetPlatformNil(b bool)`

 SetPlatformNil sets the value for Platform to be an explicit nil

### UnsetPlatform
`func (o *PiUserDomainAggregatesModelUserInfoAggregateDevice) UnsetPlatform()`

UnsetPlatform ensures that no value is present for Platform, not even an explicit nil
### GetIsActive

`func (o *PiUserDomainAggregatesModelUserInfoAggregateDevice) GetIsActive() bool`

GetIsActive returns the IsActive field if non-nil, zero value otherwise.

### GetIsActiveOk

`func (o *PiUserDomainAggregatesModelUserInfoAggregateDevice) GetIsActiveOk() (*bool, bool)`

GetIsActiveOk returns a tuple with the IsActive field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetIsActive

`func (o *PiUserDomainAggregatesModelUserInfoAggregateDevice) SetIsActive(v bool)`

SetIsActive sets IsActive field to given value.

### HasIsActive

`func (o *PiUserDomainAggregatesModelUserInfoAggregateDevice) HasIsActive() bool`

HasIsActive returns a boolean if a field has been set.

### GetSubscriptionIdentifier

`func (o *PiUserDomainAggregatesModelUserInfoAggregateDevice) GetSubscriptionIdentifier() string`

GetSubscriptionIdentifier returns the SubscriptionIdentifier field if non-nil, zero value otherwise.

### GetSubscriptionIdentifierOk

`func (o *PiUserDomainAggregatesModelUserInfoAggregateDevice) GetSubscriptionIdentifierOk() (*string, bool)`

GetSubscriptionIdentifierOk returns a tuple with the SubscriptionIdentifier field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetSubscriptionIdentifier

`func (o *PiUserDomainAggregatesModelUserInfoAggregateDevice) SetSubscriptionIdentifier(v string)`

SetSubscriptionIdentifier sets SubscriptionIdentifier field to given value.

### HasSubscriptionIdentifier

`func (o *PiUserDomainAggregatesModelUserInfoAggregateDevice) HasSubscriptionIdentifier() bool`

HasSubscriptionIdentifier returns a boolean if a field has been set.

### SetSubscriptionIdentifierNil

`func (o *PiUserDomainAggregatesModelUserInfoAggregateDevice) SetSubscriptionIdentifierNil(b bool)`

 SetSubscriptionIdentifierNil sets the value for SubscriptionIdentifier to be an explicit nil

### UnsetSubscriptionIdentifier
`func (o *PiUserDomainAggregatesModelUserInfoAggregateDevice) UnsetSubscriptionIdentifier()`

UnsetSubscriptionIdentifier ensures that no value is present for SubscriptionIdentifier, not even an explicit nil
### GetNotificationPreference

`func (o *PiUserDomainAggregatesModelUserInfoAggregateDevice) GetNotificationPreference() PiUserDomainAggregatesModelUserInfoAggregateNotificationPreference`

GetNotificationPreference returns the NotificationPreference field if non-nil, zero value otherwise.

### GetNotificationPreferenceOk

`func (o *PiUserDomainAggregatesModelUserInfoAggregateDevice) GetNotificationPreferenceOk() (*PiUserDomainAggregatesModelUserInfoAggregateNotificationPreference, bool)`

GetNotificationPreferenceOk returns a tuple with the NotificationPreference field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetNotificationPreference

`func (o *PiUserDomainAggregatesModelUserInfoAggregateDevice) SetNotificationPreference(v PiUserDomainAggregatesModelUserInfoAggregateNotificationPreference)`

SetNotificationPreference sets NotificationPreference field to given value.

### HasNotificationPreference

`func (o *PiUserDomainAggregatesModelUserInfoAggregateDevice) HasNotificationPreference() bool`

HasNotificationPreference returns a boolean if a field has been set.

### GetUserInfoId

`func (o *PiUserDomainAggregatesModelUserInfoAggregateDevice) GetUserInfoId() string`

GetUserInfoId returns the UserInfoId field if non-nil, zero value otherwise.

### GetUserInfoIdOk

`func (o *PiUserDomainAggregatesModelUserInfoAggregateDevice) GetUserInfoIdOk() (*string, bool)`

GetUserInfoIdOk returns a tuple with the UserInfoId field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetUserInfoId

`func (o *PiUserDomainAggregatesModelUserInfoAggregateDevice) SetUserInfoId(v string)`

SetUserInfoId sets UserInfoId field to given value.

### HasUserInfoId

`func (o *PiUserDomainAggregatesModelUserInfoAggregateDevice) HasUserInfoId() bool`

HasUserInfoId returns a boolean if a field has been set.

### SetUserInfoIdNil

`func (o *PiUserDomainAggregatesModelUserInfoAggregateDevice) SetUserInfoIdNil(b bool)`

 SetUserInfoIdNil sets the value for UserInfoId to be an explicit nil

### UnsetUserInfoId
`func (o *PiUserDomainAggregatesModelUserInfoAggregateDevice) UnsetUserInfoId()`

UnsetUserInfoId ensures that no value is present for UserInfoId, not even an explicit nil
### GetUserInfo

`func (o *PiUserDomainAggregatesModelUserInfoAggregateDevice) GetUserInfo() PiUserDomainAggregatesModelUserInfoAggregateUserInfo`

GetUserInfo returns the UserInfo field if non-nil, zero value otherwise.

### GetUserInfoOk

`func (o *PiUserDomainAggregatesModelUserInfoAggregateDevice) GetUserInfoOk() (*PiUserDomainAggregatesModelUserInfoAggregateUserInfo, bool)`

GetUserInfoOk returns a tuple with the UserInfo field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetUserInfo

`func (o *PiUserDomainAggregatesModelUserInfoAggregateDevice) SetUserInfo(v PiUserDomainAggregatesModelUserInfoAggregateUserInfo)`

SetUserInfo sets UserInfo field to given value.

### HasUserInfo

`func (o *PiUserDomainAggregatesModelUserInfoAggregateDevice) HasUserInfo() bool`

HasUserInfo returns a boolean if a field has been set.

### GetCreatedAt

`func (o *PiUserDomainAggregatesModelUserInfoAggregateDevice) GetCreatedAt() time.Time`

GetCreatedAt returns the CreatedAt field if non-nil, zero value otherwise.

### GetCreatedAtOk

`func (o *PiUserDomainAggregatesModelUserInfoAggregateDevice) GetCreatedAtOk() (*time.Time, bool)`

GetCreatedAtOk returns a tuple with the CreatedAt field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetCreatedAt

`func (o *PiUserDomainAggregatesModelUserInfoAggregateDevice) SetCreatedAt(v time.Time)`

SetCreatedAt sets CreatedAt field to given value.

### HasCreatedAt

`func (o *PiUserDomainAggregatesModelUserInfoAggregateDevice) HasCreatedAt() bool`

HasCreatedAt returns a boolean if a field has been set.

### GetUpdatedAt

`func (o *PiUserDomainAggregatesModelUserInfoAggregateDevice) GetUpdatedAt() time.Time`

GetUpdatedAt returns the UpdatedAt field if non-nil, zero value otherwise.

### GetUpdatedAtOk

`func (o *PiUserDomainAggregatesModelUserInfoAggregateDevice) GetUpdatedAtOk() (*time.Time, bool)`

GetUpdatedAtOk returns a tuple with the UpdatedAt field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetUpdatedAt

`func (o *PiUserDomainAggregatesModelUserInfoAggregateDevice) SetUpdatedAt(v time.Time)`

SetUpdatedAt sets UpdatedAt field to given value.

### HasUpdatedAt

`func (o *PiUserDomainAggregatesModelUserInfoAggregateDevice) HasUpdatedAt() bool`

HasUpdatedAt returns a boolean if a field has been set.

### SetUpdatedAtNil

`func (o *PiUserDomainAggregatesModelUserInfoAggregateDevice) SetUpdatedAtNil(b bool)`

 SetUpdatedAtNil sets the value for UpdatedAt to be an explicit nil

### UnsetUpdatedAt
`func (o *PiUserDomainAggregatesModelUserInfoAggregateDevice) UnsetUpdatedAt()`

UnsetUpdatedAt ensures that no value is present for UpdatedAt, not even an explicit nil

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


