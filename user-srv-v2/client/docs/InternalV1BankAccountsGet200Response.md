# InternalV1BankAccountsGet200Response

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**Code** | Pointer to **string** |  | [optional]
**Msg** | Pointer to **string** |  | [optional]
**Data** | Pointer to [**[]DtoBankAccountResponse**](DtoBankAccountResponse.md) |  | [optional]

## Methods

### NewInternalV1BankAccountsGet200Response

`func NewInternalV1BankAccountsGet200Response() *InternalV1BankAccountsGet200Response`

NewInternalV1BankAccountsGet200Response instantiates a new InternalV1BankAccountsGet200Response object
This constructor will assign default values to properties that have it defined,
and makes sure properties required by API are set, but the set of arguments
will change when the set of required properties is changed

### NewInternalV1BankAccountsGet200ResponseWithDefaults

`func NewInternalV1BankAccountsGet200ResponseWithDefaults() *InternalV1BankAccountsGet200Response`

NewInternalV1BankAccountsGet200ResponseWithDefaults instantiates a new InternalV1BankAccountsGet200Response object
This constructor will only assign default values to properties that have it defined,
but it doesn't guarantee that properties required by API are set

### GetCode

`func (o *InternalV1BankAccountsGet200Response) GetCode() string`

GetCode returns the Code field if non-nil, zero value otherwise.

### GetCodeOk

`func (o *InternalV1BankAccountsGet200Response) GetCodeOk() (*string, bool)`

GetCodeOk returns a tuple with the Code field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetCode

`func (o *InternalV1BankAccountsGet200Response) SetCode(v string)`

SetCode sets Code field to given value.

### HasCode

`func (o *InternalV1BankAccountsGet200Response) HasCode() bool`

HasCode returns a boolean if a field has been set.

### GetMsg

`func (o *InternalV1BankAccountsGet200Response) GetMsg() string`

GetMsg returns the Msg field if non-nil, zero value otherwise.

### GetMsgOk

`func (o *InternalV1BankAccountsGet200Response) GetMsgOk() (*string, bool)`

GetMsgOk returns a tuple with the Msg field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetMsg

`func (o *InternalV1BankAccountsGet200Response) SetMsg(v string)`

SetMsg sets Msg field to given value.

### HasMsg

`func (o *InternalV1BankAccountsGet200Response) HasMsg() bool`

HasMsg returns a boolean if a field has been set.

### GetData

`func (o *InternalV1BankAccountsGet200Response) GetData() []DtoBankAccountResponse`

GetData returns the Data field if non-nil, zero value otherwise.

### GetDataOk

`func (o *InternalV1BankAccountsGet200Response) GetDataOk() (*[]DtoBankAccountResponse, bool)`

GetDataOk returns a tuple with the Data field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetData

`func (o *InternalV1BankAccountsGet200Response) SetData(v []DtoBankAccountResponse)`

SetData sets Data field to given value.

### HasData

`func (o *InternalV1BankAccountsGet200Response) HasData() bool`

HasData returns a boolean if a field has been set.


[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)
