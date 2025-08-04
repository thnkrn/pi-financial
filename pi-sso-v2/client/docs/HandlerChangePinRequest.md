# HandlerChangePinRequest

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**Custcode** | **string** |  | 
**EncryptNewPin** | **string** |  | 
**EncryptOldPin** | **string** |  | 

## Methods

### NewHandlerChangePinRequest

`func NewHandlerChangePinRequest(custcode string, encryptNewPin string, encryptOldPin string, ) *HandlerChangePinRequest`

NewHandlerChangePinRequest instantiates a new HandlerChangePinRequest object
This constructor will assign default values to properties that have it defined,
and makes sure properties required by API are set, but the set of arguments
will change when the set of required properties is changed

### NewHandlerChangePinRequestWithDefaults

`func NewHandlerChangePinRequestWithDefaults() *HandlerChangePinRequest`

NewHandlerChangePinRequestWithDefaults instantiates a new HandlerChangePinRequest object
This constructor will only assign default values to properties that have it defined,
but it doesn't guarantee that properties required by API are set

### GetCustcode

`func (o *HandlerChangePinRequest) GetCustcode() string`

GetCustcode returns the Custcode field if non-nil, zero value otherwise.

### GetCustcodeOk

`func (o *HandlerChangePinRequest) GetCustcodeOk() (*string, bool)`

GetCustcodeOk returns a tuple with the Custcode field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetCustcode

`func (o *HandlerChangePinRequest) SetCustcode(v string)`

SetCustcode sets Custcode field to given value.


### GetEncryptNewPin

`func (o *HandlerChangePinRequest) GetEncryptNewPin() string`

GetEncryptNewPin returns the EncryptNewPin field if non-nil, zero value otherwise.

### GetEncryptNewPinOk

`func (o *HandlerChangePinRequest) GetEncryptNewPinOk() (*string, bool)`

GetEncryptNewPinOk returns a tuple with the EncryptNewPin field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetEncryptNewPin

`func (o *HandlerChangePinRequest) SetEncryptNewPin(v string)`

SetEncryptNewPin sets EncryptNewPin field to given value.


### GetEncryptOldPin

`func (o *HandlerChangePinRequest) GetEncryptOldPin() string`

GetEncryptOldPin returns the EncryptOldPin field if non-nil, zero value otherwise.

### GetEncryptOldPinOk

`func (o *HandlerChangePinRequest) GetEncryptOldPinOk() (*string, bool)`

GetEncryptOldPinOk returns a tuple with the EncryptOldPin field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetEncryptOldPin

`func (o *HandlerChangePinRequest) SetEncryptOldPin(v string)`

SetEncryptOldPin sets EncryptOldPin field to given value.



[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


