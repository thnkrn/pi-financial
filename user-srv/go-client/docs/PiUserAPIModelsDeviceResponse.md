# PiUserAPIModelsDeviceResponse

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**DeviceId** | Pointer to **string** |  | [optional] 
**DeviceToken** | Pointer to **NullableString** |  | [optional] 
**DeviceIdentifier** | Pointer to **NullableString** |  | [optional] 
**Language** | Pointer to **NullableString** |  | [optional] 
**Platform** | Pointer to **NullableString** |  | [optional] 
**NotificationPreference** | Pointer to [**PiUserAPIModelsNotificatonPreferenceResponse**](PiUserAPIModelsNotificatonPreferenceResponse.md) |  | [optional] 

## Methods

### NewPiUserAPIModelsDeviceResponse

`func NewPiUserAPIModelsDeviceResponse() *PiUserAPIModelsDeviceResponse`

NewPiUserAPIModelsDeviceResponse instantiates a new PiUserAPIModelsDeviceResponse object
This constructor will assign default values to properties that have it defined,
and makes sure properties required by API are set, but the set of arguments
will change when the set of required properties is changed

### NewPiUserAPIModelsDeviceResponseWithDefaults

`func NewPiUserAPIModelsDeviceResponseWithDefaults() *PiUserAPIModelsDeviceResponse`

NewPiUserAPIModelsDeviceResponseWithDefaults instantiates a new PiUserAPIModelsDeviceResponse object
This constructor will only assign default values to properties that have it defined,
but it doesn't guarantee that properties required by API are set

### GetDeviceId

`func (o *PiUserAPIModelsDeviceResponse) GetDeviceId() string`

GetDeviceId returns the DeviceId field if non-nil, zero value otherwise.

### GetDeviceIdOk

`func (o *PiUserAPIModelsDeviceResponse) GetDeviceIdOk() (*string, bool)`

GetDeviceIdOk returns a tuple with the DeviceId field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetDeviceId

`func (o *PiUserAPIModelsDeviceResponse) SetDeviceId(v string)`

SetDeviceId sets DeviceId field to given value.

### HasDeviceId

`func (o *PiUserAPIModelsDeviceResponse) HasDeviceId() bool`

HasDeviceId returns a boolean if a field has been set.

### GetDeviceToken

`func (o *PiUserAPIModelsDeviceResponse) GetDeviceToken() string`

GetDeviceToken returns the DeviceToken field if non-nil, zero value otherwise.

### GetDeviceTokenOk

`func (o *PiUserAPIModelsDeviceResponse) GetDeviceTokenOk() (*string, bool)`

GetDeviceTokenOk returns a tuple with the DeviceToken field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetDeviceToken

`func (o *PiUserAPIModelsDeviceResponse) SetDeviceToken(v string)`

SetDeviceToken sets DeviceToken field to given value.

### HasDeviceToken

`func (o *PiUserAPIModelsDeviceResponse) HasDeviceToken() bool`

HasDeviceToken returns a boolean if a field has been set.

### SetDeviceTokenNil

`func (o *PiUserAPIModelsDeviceResponse) SetDeviceTokenNil(b bool)`

 SetDeviceTokenNil sets the value for DeviceToken to be an explicit nil

### UnsetDeviceToken
`func (o *PiUserAPIModelsDeviceResponse) UnsetDeviceToken()`

UnsetDeviceToken ensures that no value is present for DeviceToken, not even an explicit nil
### GetDeviceIdentifier

`func (o *PiUserAPIModelsDeviceResponse) GetDeviceIdentifier() string`

GetDeviceIdentifier returns the DeviceIdentifier field if non-nil, zero value otherwise.

### GetDeviceIdentifierOk

`func (o *PiUserAPIModelsDeviceResponse) GetDeviceIdentifierOk() (*string, bool)`

GetDeviceIdentifierOk returns a tuple with the DeviceIdentifier field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetDeviceIdentifier

`func (o *PiUserAPIModelsDeviceResponse) SetDeviceIdentifier(v string)`

SetDeviceIdentifier sets DeviceIdentifier field to given value.

### HasDeviceIdentifier

`func (o *PiUserAPIModelsDeviceResponse) HasDeviceIdentifier() bool`

HasDeviceIdentifier returns a boolean if a field has been set.

### SetDeviceIdentifierNil

`func (o *PiUserAPIModelsDeviceResponse) SetDeviceIdentifierNil(b bool)`

 SetDeviceIdentifierNil sets the value for DeviceIdentifier to be an explicit nil

### UnsetDeviceIdentifier
`func (o *PiUserAPIModelsDeviceResponse) UnsetDeviceIdentifier()`

UnsetDeviceIdentifier ensures that no value is present for DeviceIdentifier, not even an explicit nil
### GetLanguage

`func (o *PiUserAPIModelsDeviceResponse) GetLanguage() string`

GetLanguage returns the Language field if non-nil, zero value otherwise.

### GetLanguageOk

`func (o *PiUserAPIModelsDeviceResponse) GetLanguageOk() (*string, bool)`

GetLanguageOk returns a tuple with the Language field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetLanguage

`func (o *PiUserAPIModelsDeviceResponse) SetLanguage(v string)`

SetLanguage sets Language field to given value.

### HasLanguage

`func (o *PiUserAPIModelsDeviceResponse) HasLanguage() bool`

HasLanguage returns a boolean if a field has been set.

### SetLanguageNil

`func (o *PiUserAPIModelsDeviceResponse) SetLanguageNil(b bool)`

 SetLanguageNil sets the value for Language to be an explicit nil

### UnsetLanguage
`func (o *PiUserAPIModelsDeviceResponse) UnsetLanguage()`

UnsetLanguage ensures that no value is present for Language, not even an explicit nil
### GetPlatform

`func (o *PiUserAPIModelsDeviceResponse) GetPlatform() string`

GetPlatform returns the Platform field if non-nil, zero value otherwise.

### GetPlatformOk

`func (o *PiUserAPIModelsDeviceResponse) GetPlatformOk() (*string, bool)`

GetPlatformOk returns a tuple with the Platform field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetPlatform

`func (o *PiUserAPIModelsDeviceResponse) SetPlatform(v string)`

SetPlatform sets Platform field to given value.

### HasPlatform

`func (o *PiUserAPIModelsDeviceResponse) HasPlatform() bool`

HasPlatform returns a boolean if a field has been set.

### SetPlatformNil

`func (o *PiUserAPIModelsDeviceResponse) SetPlatformNil(b bool)`

 SetPlatformNil sets the value for Platform to be an explicit nil

### UnsetPlatform
`func (o *PiUserAPIModelsDeviceResponse) UnsetPlatform()`

UnsetPlatform ensures that no value is present for Platform, not even an explicit nil
### GetNotificationPreference

`func (o *PiUserAPIModelsDeviceResponse) GetNotificationPreference() PiUserAPIModelsNotificatonPreferenceResponse`

GetNotificationPreference returns the NotificationPreference field if non-nil, zero value otherwise.

### GetNotificationPreferenceOk

`func (o *PiUserAPIModelsDeviceResponse) GetNotificationPreferenceOk() (*PiUserAPIModelsNotificatonPreferenceResponse, bool)`

GetNotificationPreferenceOk returns a tuple with the NotificationPreference field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetNotificationPreference

`func (o *PiUserAPIModelsDeviceResponse) SetNotificationPreference(v PiUserAPIModelsNotificatonPreferenceResponse)`

SetNotificationPreference sets NotificationPreference field to given value.

### HasNotificationPreference

`func (o *PiUserAPIModelsDeviceResponse) HasNotificationPreference() bool`

HasNotificationPreference returns a boolean if a field has been set.


[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


