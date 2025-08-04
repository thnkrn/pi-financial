# HandlerInternalCreatePasswordRequest

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**EncryptNewPassword** | **string** |  | 
**UserId** | **string** |  | 
**Username** | **[]string** |  | 

## Methods

### NewHandlerInternalCreatePasswordRequest

`func NewHandlerInternalCreatePasswordRequest(encryptNewPassword string, userId string, username []string, ) *HandlerInternalCreatePasswordRequest`

NewHandlerInternalCreatePasswordRequest instantiates a new HandlerInternalCreatePasswordRequest object
This constructor will assign default values to properties that have it defined,
and makes sure properties required by API are set, but the set of arguments
will change when the set of required properties is changed

### NewHandlerInternalCreatePasswordRequestWithDefaults

`func NewHandlerInternalCreatePasswordRequestWithDefaults() *HandlerInternalCreatePasswordRequest`

NewHandlerInternalCreatePasswordRequestWithDefaults instantiates a new HandlerInternalCreatePasswordRequest object
This constructor will only assign default values to properties that have it defined,
but it doesn't guarantee that properties required by API are set

### GetEncryptNewPassword

`func (o *HandlerInternalCreatePasswordRequest) GetEncryptNewPassword() string`

GetEncryptNewPassword returns the EncryptNewPassword field if non-nil, zero value otherwise.

### GetEncryptNewPasswordOk

`func (o *HandlerInternalCreatePasswordRequest) GetEncryptNewPasswordOk() (*string, bool)`

GetEncryptNewPasswordOk returns a tuple with the EncryptNewPassword field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetEncryptNewPassword

`func (o *HandlerInternalCreatePasswordRequest) SetEncryptNewPassword(v string)`

SetEncryptNewPassword sets EncryptNewPassword field to given value.


### GetUserId

`func (o *HandlerInternalCreatePasswordRequest) GetUserId() string`

GetUserId returns the UserId field if non-nil, zero value otherwise.

### GetUserIdOk

`func (o *HandlerInternalCreatePasswordRequest) GetUserIdOk() (*string, bool)`

GetUserIdOk returns a tuple with the UserId field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetUserId

`func (o *HandlerInternalCreatePasswordRequest) SetUserId(v string)`

SetUserId sets UserId field to given value.


### GetUsername

`func (o *HandlerInternalCreatePasswordRequest) GetUsername() []string`

GetUsername returns the Username field if non-nil, zero value otherwise.

### GetUsernameOk

`func (o *HandlerInternalCreatePasswordRequest) GetUsernameOk() (*[]string, bool)`

GetUsernameOk returns a tuple with the Username field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetUsername

`func (o *HandlerInternalCreatePasswordRequest) SetUsername(v []string)`

SetUsername sets Username field to given value.



[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


