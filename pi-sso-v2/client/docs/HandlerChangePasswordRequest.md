# HandlerChangePasswordRequest

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**EncryptNewPassword** | **string** |  | 
**EncryptOldPassword** | **string** |  | 
**Username** | **string** |  | 

## Methods

### NewHandlerChangePasswordRequest

`func NewHandlerChangePasswordRequest(encryptNewPassword string, encryptOldPassword string, username string, ) *HandlerChangePasswordRequest`

NewHandlerChangePasswordRequest instantiates a new HandlerChangePasswordRequest object
This constructor will assign default values to properties that have it defined,
and makes sure properties required by API are set, but the set of arguments
will change when the set of required properties is changed

### NewHandlerChangePasswordRequestWithDefaults

`func NewHandlerChangePasswordRequestWithDefaults() *HandlerChangePasswordRequest`

NewHandlerChangePasswordRequestWithDefaults instantiates a new HandlerChangePasswordRequest object
This constructor will only assign default values to properties that have it defined,
but it doesn't guarantee that properties required by API are set

### GetEncryptNewPassword

`func (o *HandlerChangePasswordRequest) GetEncryptNewPassword() string`

GetEncryptNewPassword returns the EncryptNewPassword field if non-nil, zero value otherwise.

### GetEncryptNewPasswordOk

`func (o *HandlerChangePasswordRequest) GetEncryptNewPasswordOk() (*string, bool)`

GetEncryptNewPasswordOk returns a tuple with the EncryptNewPassword field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetEncryptNewPassword

`func (o *HandlerChangePasswordRequest) SetEncryptNewPassword(v string)`

SetEncryptNewPassword sets EncryptNewPassword field to given value.


### GetEncryptOldPassword

`func (o *HandlerChangePasswordRequest) GetEncryptOldPassword() string`

GetEncryptOldPassword returns the EncryptOldPassword field if non-nil, zero value otherwise.

### GetEncryptOldPasswordOk

`func (o *HandlerChangePasswordRequest) GetEncryptOldPasswordOk() (*string, bool)`

GetEncryptOldPasswordOk returns a tuple with the EncryptOldPassword field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetEncryptOldPassword

`func (o *HandlerChangePasswordRequest) SetEncryptOldPassword(v string)`

SetEncryptOldPassword sets EncryptOldPassword field to given value.


### GetUsername

`func (o *HandlerChangePasswordRequest) GetUsername() string`

GetUsername returns the Username field if non-nil, zero value otherwise.

### GetUsernameOk

`func (o *HandlerChangePasswordRequest) GetUsernameOk() (*string, bool)`

GetUsernameOk returns a tuple with the Username field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetUsername

`func (o *HandlerChangePasswordRequest) SetUsername(v string)`

SetUsername sets Username field to given value.



[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


