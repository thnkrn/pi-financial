# HandlerResetPinRequest

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**Custcode** | **[]string** |  | 
**EncryptNewPin** | **string** |  | 
**EncryptPassword** | **string** |  | 

## Methods

### NewHandlerResetPinRequest

`func NewHandlerResetPinRequest(custcode []string, encryptNewPin string, encryptPassword string, ) *HandlerResetPinRequest`

NewHandlerResetPinRequest instantiates a new HandlerResetPinRequest object
This constructor will assign default values to properties that have it defined,
and makes sure properties required by API are set, but the set of arguments
will change when the set of required properties is changed

### NewHandlerResetPinRequestWithDefaults

`func NewHandlerResetPinRequestWithDefaults() *HandlerResetPinRequest`

NewHandlerResetPinRequestWithDefaults instantiates a new HandlerResetPinRequest object
This constructor will only assign default values to properties that have it defined,
but it doesn't guarantee that properties required by API are set

### GetCustcode

`func (o *HandlerResetPinRequest) GetCustcode() []string`

GetCustcode returns the Custcode field if non-nil, zero value otherwise.

### GetCustcodeOk

`func (o *HandlerResetPinRequest) GetCustcodeOk() (*[]string, bool)`

GetCustcodeOk returns a tuple with the Custcode field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetCustcode

`func (o *HandlerResetPinRequest) SetCustcode(v []string)`

SetCustcode sets Custcode field to given value.


### GetEncryptNewPin

`func (o *HandlerResetPinRequest) GetEncryptNewPin() string`

GetEncryptNewPin returns the EncryptNewPin field if non-nil, zero value otherwise.

### GetEncryptNewPinOk

`func (o *HandlerResetPinRequest) GetEncryptNewPinOk() (*string, bool)`

GetEncryptNewPinOk returns a tuple with the EncryptNewPin field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetEncryptNewPin

`func (o *HandlerResetPinRequest) SetEncryptNewPin(v string)`

SetEncryptNewPin sets EncryptNewPin field to given value.


### GetEncryptPassword

`func (o *HandlerResetPinRequest) GetEncryptPassword() string`

GetEncryptPassword returns the EncryptPassword field if non-nil, zero value otherwise.

### GetEncryptPasswordOk

`func (o *HandlerResetPinRequest) GetEncryptPasswordOk() (*string, bool)`

GetEncryptPasswordOk returns a tuple with the EncryptPassword field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetEncryptPassword

`func (o *HandlerResetPinRequest) SetEncryptPassword(v string)`

SetEncryptPassword sets EncryptPassword field to given value.



[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


