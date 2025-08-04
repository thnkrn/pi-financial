# HandlerGuestRegisterRequest

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**Password** | **string** |  | 
**PhoneNumber** | Pointer to **string** |  | [optional] 
**Username** | **string** |  | 

## Methods

### NewHandlerGuestRegisterRequest

`func NewHandlerGuestRegisterRequest(password string, username string, ) *HandlerGuestRegisterRequest`

NewHandlerGuestRegisterRequest instantiates a new HandlerGuestRegisterRequest object
This constructor will assign default values to properties that have it defined,
and makes sure properties required by API are set, but the set of arguments
will change when the set of required properties is changed

### NewHandlerGuestRegisterRequestWithDefaults

`func NewHandlerGuestRegisterRequestWithDefaults() *HandlerGuestRegisterRequest`

NewHandlerGuestRegisterRequestWithDefaults instantiates a new HandlerGuestRegisterRequest object
This constructor will only assign default values to properties that have it defined,
but it doesn't guarantee that properties required by API are set

### GetPassword

`func (o *HandlerGuestRegisterRequest) GetPassword() string`

GetPassword returns the Password field if non-nil, zero value otherwise.

### GetPasswordOk

`func (o *HandlerGuestRegisterRequest) GetPasswordOk() (*string, bool)`

GetPasswordOk returns a tuple with the Password field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetPassword

`func (o *HandlerGuestRegisterRequest) SetPassword(v string)`

SetPassword sets Password field to given value.


### GetPhoneNumber

`func (o *HandlerGuestRegisterRequest) GetPhoneNumber() string`

GetPhoneNumber returns the PhoneNumber field if non-nil, zero value otherwise.

### GetPhoneNumberOk

`func (o *HandlerGuestRegisterRequest) GetPhoneNumberOk() (*string, bool)`

GetPhoneNumberOk returns a tuple with the PhoneNumber field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetPhoneNumber

`func (o *HandlerGuestRegisterRequest) SetPhoneNumber(v string)`

SetPhoneNumber sets PhoneNumber field to given value.

### HasPhoneNumber

`func (o *HandlerGuestRegisterRequest) HasPhoneNumber() bool`

HasPhoneNumber returns a boolean if a field has been set.

### GetUsername

`func (o *HandlerGuestRegisterRequest) GetUsername() string`

GetUsername returns the Username field if non-nil, zero value otherwise.

### GetUsernameOk

`func (o *HandlerGuestRegisterRequest) GetUsernameOk() (*string, bool)`

GetUsernameOk returns a tuple with the Username field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetUsername

`func (o *HandlerGuestRegisterRequest) SetUsername(v string)`

SetUsername sets Username field to given value.



[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


