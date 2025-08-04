# HandlerCreatePinRequest

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**Custcode** | **[]string** |  | 
**EncryptNewPin** | **string** |  | 

## Methods

### NewHandlerCreatePinRequest

`func NewHandlerCreatePinRequest(custcode []string, encryptNewPin string, ) *HandlerCreatePinRequest`

NewHandlerCreatePinRequest instantiates a new HandlerCreatePinRequest object
This constructor will assign default values to properties that have it defined,
and makes sure properties required by API are set, but the set of arguments
will change when the set of required properties is changed

### NewHandlerCreatePinRequestWithDefaults

`func NewHandlerCreatePinRequestWithDefaults() *HandlerCreatePinRequest`

NewHandlerCreatePinRequestWithDefaults instantiates a new HandlerCreatePinRequest object
This constructor will only assign default values to properties that have it defined,
but it doesn't guarantee that properties required by API are set

### GetCustcode

`func (o *HandlerCreatePinRequest) GetCustcode() []string`

GetCustcode returns the Custcode field if non-nil, zero value otherwise.

### GetCustcodeOk

`func (o *HandlerCreatePinRequest) GetCustcodeOk() (*[]string, bool)`

GetCustcodeOk returns a tuple with the Custcode field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetCustcode

`func (o *HandlerCreatePinRequest) SetCustcode(v []string)`

SetCustcode sets Custcode field to given value.


### GetEncryptNewPin

`func (o *HandlerCreatePinRequest) GetEncryptNewPin() string`

GetEncryptNewPin returns the EncryptNewPin field if non-nil, zero value otherwise.

### GetEncryptNewPinOk

`func (o *HandlerCreatePinRequest) GetEncryptNewPinOk() (*string, bool)`

GetEncryptNewPinOk returns a tuple with the EncryptNewPin field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetEncryptNewPin

`func (o *HandlerCreatePinRequest) SetEncryptNewPin(v string)`

SetEncryptNewPin sets EncryptNewPin field to given value.



[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


