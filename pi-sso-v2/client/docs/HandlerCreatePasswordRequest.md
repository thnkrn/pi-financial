# HandlerCreatePasswordRequest

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**EncryptNewPassword** | **string** |  | 
**Username** | **[]string** |  | 

## Methods

### NewHandlerCreatePasswordRequest

`func NewHandlerCreatePasswordRequest(encryptNewPassword string, username []string, ) *HandlerCreatePasswordRequest`

NewHandlerCreatePasswordRequest instantiates a new HandlerCreatePasswordRequest object
This constructor will assign default values to properties that have it defined,
and makes sure properties required by API are set, but the set of arguments
will change when the set of required properties is changed

### NewHandlerCreatePasswordRequestWithDefaults

`func NewHandlerCreatePasswordRequestWithDefaults() *HandlerCreatePasswordRequest`

NewHandlerCreatePasswordRequestWithDefaults instantiates a new HandlerCreatePasswordRequest object
This constructor will only assign default values to properties that have it defined,
but it doesn't guarantee that properties required by API are set

### GetEncryptNewPassword

`func (o *HandlerCreatePasswordRequest) GetEncryptNewPassword() string`

GetEncryptNewPassword returns the EncryptNewPassword field if non-nil, zero value otherwise.

### GetEncryptNewPasswordOk

`func (o *HandlerCreatePasswordRequest) GetEncryptNewPasswordOk() (*string, bool)`

GetEncryptNewPasswordOk returns a tuple with the EncryptNewPassword field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetEncryptNewPassword

`func (o *HandlerCreatePasswordRequest) SetEncryptNewPassword(v string)`

SetEncryptNewPassword sets EncryptNewPassword field to given value.


### GetUsername

`func (o *HandlerCreatePasswordRequest) GetUsername() []string`

GetUsername returns the Username field if non-nil, zero value otherwise.

### GetUsernameOk

`func (o *HandlerCreatePasswordRequest) GetUsernameOk() (*[]string, bool)`

GetUsernameOk returns a tuple with the Username field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetUsername

`func (o *HandlerCreatePasswordRequest) SetUsername(v []string)`

SetUsername sets Username field to given value.



[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


