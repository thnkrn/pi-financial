# HandlerLoginRequest

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**EncryptPassword** | **string** |  | 
**Username** | **string** |  | 

## Methods

### NewHandlerLoginRequest

`func NewHandlerLoginRequest(encryptPassword string, username string, ) *HandlerLoginRequest`

NewHandlerLoginRequest instantiates a new HandlerLoginRequest object
This constructor will assign default values to properties that have it defined,
and makes sure properties required by API are set, but the set of arguments
will change when the set of required properties is changed

### NewHandlerLoginRequestWithDefaults

`func NewHandlerLoginRequestWithDefaults() *HandlerLoginRequest`

NewHandlerLoginRequestWithDefaults instantiates a new HandlerLoginRequest object
This constructor will only assign default values to properties that have it defined,
but it doesn't guarantee that properties required by API are set

### GetEncryptPassword

`func (o *HandlerLoginRequest) GetEncryptPassword() string`

GetEncryptPassword returns the EncryptPassword field if non-nil, zero value otherwise.

### GetEncryptPasswordOk

`func (o *HandlerLoginRequest) GetEncryptPasswordOk() (*string, bool)`

GetEncryptPasswordOk returns a tuple with the EncryptPassword field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetEncryptPassword

`func (o *HandlerLoginRequest) SetEncryptPassword(v string)`

SetEncryptPassword sets EncryptPassword field to given value.


### GetUsername

`func (o *HandlerLoginRequest) GetUsername() string`

GetUsername returns the Username field if non-nil, zero value otherwise.

### GetUsernameOk

`func (o *HandlerLoginRequest) GetUsernameOk() (*string, bool)`

GetUsernameOk returns a tuple with the Username field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetUsername

`func (o *HandlerLoginRequest) SetUsername(v string)`

SetUsername sets Username field to given value.



[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


