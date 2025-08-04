# DtoDevice

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**DeviceId** | Pointer to **string** |  | [optional] 
**DeviceIdentifier** | Pointer to **string** |  | [optional] 
**DeviceToken** | Pointer to **string** |  | [optional] 
**Language** | Pointer to **string** |  | [optional] 
**NotificationPreference** | Pointer to [**DtoNotificationPreference**](DtoNotificationPreference.md) |  | [optional] 
**Platform** | Pointer to **string** |  | [optional] 

## Methods

### NewDtoDevice

`func NewDtoDevice() *DtoDevice`

NewDtoDevice instantiates a new DtoDevice object
This constructor will assign default values to properties that have it defined,
and makes sure properties required by API are set, but the set of arguments
will change when the set of required properties is changed

### NewDtoDeviceWithDefaults

`func NewDtoDeviceWithDefaults() *DtoDevice`

NewDtoDeviceWithDefaults instantiates a new DtoDevice object
This constructor will only assign default values to properties that have it defined,
but it doesn't guarantee that properties required by API are set

### GetDeviceId

`func (o *DtoDevice) GetDeviceId() string`

GetDeviceId returns the DeviceId field if non-nil, zero value otherwise.

### GetDeviceIdOk

`func (o *DtoDevice) GetDeviceIdOk() (*string, bool)`

GetDeviceIdOk returns a tuple with the DeviceId field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetDeviceId

`func (o *DtoDevice) SetDeviceId(v string)`

SetDeviceId sets DeviceId field to given value.

### HasDeviceId

`func (o *DtoDevice) HasDeviceId() bool`

HasDeviceId returns a boolean if a field has been set.

### GetDeviceIdentifier

`func (o *DtoDevice) GetDeviceIdentifier() string`

GetDeviceIdentifier returns the DeviceIdentifier field if non-nil, zero value otherwise.

### GetDeviceIdentifierOk

`func (o *DtoDevice) GetDeviceIdentifierOk() (*string, bool)`

GetDeviceIdentifierOk returns a tuple with the DeviceIdentifier field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetDeviceIdentifier

`func (o *DtoDevice) SetDeviceIdentifier(v string)`

SetDeviceIdentifier sets DeviceIdentifier field to given value.

### HasDeviceIdentifier

`func (o *DtoDevice) HasDeviceIdentifier() bool`

HasDeviceIdentifier returns a boolean if a field has been set.

### GetDeviceToken

`func (o *DtoDevice) GetDeviceToken() string`

GetDeviceToken returns the DeviceToken field if non-nil, zero value otherwise.

### GetDeviceTokenOk

`func (o *DtoDevice) GetDeviceTokenOk() (*string, bool)`

GetDeviceTokenOk returns a tuple with the DeviceToken field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetDeviceToken

`func (o *DtoDevice) SetDeviceToken(v string)`

SetDeviceToken sets DeviceToken field to given value.

### HasDeviceToken

`func (o *DtoDevice) HasDeviceToken() bool`

HasDeviceToken returns a boolean if a field has been set.

### GetLanguage

`func (o *DtoDevice) GetLanguage() string`

GetLanguage returns the Language field if non-nil, zero value otherwise.

### GetLanguageOk

`func (o *DtoDevice) GetLanguageOk() (*string, bool)`

GetLanguageOk returns a tuple with the Language field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetLanguage

`func (o *DtoDevice) SetLanguage(v string)`

SetLanguage sets Language field to given value.

### HasLanguage

`func (o *DtoDevice) HasLanguage() bool`

HasLanguage returns a boolean if a field has been set.

### GetNotificationPreference

`func (o *DtoDevice) GetNotificationPreference() DtoNotificationPreference`

GetNotificationPreference returns the NotificationPreference field if non-nil, zero value otherwise.

### GetNotificationPreferenceOk

`func (o *DtoDevice) GetNotificationPreferenceOk() (*DtoNotificationPreference, bool)`

GetNotificationPreferenceOk returns a tuple with the NotificationPreference field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetNotificationPreference

`func (o *DtoDevice) SetNotificationPreference(v DtoNotificationPreference)`

SetNotificationPreference sets NotificationPreference field to given value.

### HasNotificationPreference

`func (o *DtoDevice) HasNotificationPreference() bool`

HasNotificationPreference returns a boolean if a field has been set.

### GetPlatform

`func (o *DtoDevice) GetPlatform() string`

GetPlatform returns the Platform field if non-nil, zero value otherwise.

### GetPlatformOk

`func (o *DtoDevice) GetPlatformOk() (*string, bool)`

GetPlatformOk returns a tuple with the Platform field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetPlatform

`func (o *DtoDevice) SetPlatform(v string)`

SetPlatform sets Platform field to given value.

### HasPlatform

`func (o *DtoDevice) HasPlatform() bool`

HasPlatform returns a boolean if a field has been set.


[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


