# InternalV1UserAccountsGet200Response

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**Code** | Pointer to **string** |  | [optional]
**Msg** | Pointer to **string** |  | [optional]
**Data** | Pointer to [**[]DtoUserAccountResponse**](DtoUserAccountResponse.md) |  | [optional]

## Methods

### NewInternalV1UserAccountsGet200Response

`func NewInternalV1UserAccountsGet200Response() *InternalV1UserAccountsGet200Response`

NewInternalV1UserAccountsGet200Response instantiates a new InternalV1UserAccountsGet200Response object
This constructor will assign default values to properties that have it defined,
and makes sure properties required by API are set, but the set of arguments
will change when the set of required properties is changed

### NewInternalV1UserAccountsGet200ResponseWithDefaults

`func NewInternalV1UserAccountsGet200ResponseWithDefaults() *InternalV1UserAccountsGet200Response`

NewInternalV1UserAccountsGet200ResponseWithDefaults instantiates a new InternalV1UserAccountsGet200Response object
This constructor will only assign default values to properties that have it defined,
but it doesn't guarantee that properties required by API are set

### GetCode

`func (o *InternalV1UserAccountsGet200Response) GetCode() string`

GetCode returns the Code field if non-nil, zero value otherwise.

### GetCodeOk

`func (o *InternalV1UserAccountsGet200Response) GetCodeOk() (*string, bool)`

GetCodeOk returns a tuple with the Code field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetCode

`func (o *InternalV1UserAccountsGet200Response) SetCode(v string)`

SetCode sets Code field to given value.

### HasCode

`func (o *InternalV1UserAccountsGet200Response) HasCode() bool`

HasCode returns a boolean if a field has been set.

### GetMsg

`func (o *InternalV1UserAccountsGet200Response) GetMsg() string`

GetMsg returns the Msg field if non-nil, zero value otherwise.

### GetMsgOk

`func (o *InternalV1UserAccountsGet200Response) GetMsgOk() (*string, bool)`

GetMsgOk returns a tuple with the Msg field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetMsg

`func (o *InternalV1UserAccountsGet200Response) SetMsg(v string)`

SetMsg sets Msg field to given value.

### HasMsg

`func (o *InternalV1UserAccountsGet200Response) HasMsg() bool`

HasMsg returns a boolean if a field has been set.

### GetData

`func (o *InternalV1UserAccountsGet200Response) GetData() []DtoUserAccountResponse`

GetData returns the Data field if non-nil, zero value otherwise.

### GetDataOk

`func (o *InternalV1UserAccountsGet200Response) GetDataOk() (*[]DtoUserAccountResponse, bool)`

GetDataOk returns a tuple with the Data field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetData

`func (o *InternalV1UserAccountsGet200Response) SetData(v []DtoUserAccountResponse)`

SetData sets Data field to given value.

### HasData

`func (o *InternalV1UserAccountsGet200Response) HasData() bool`

HasData returns a boolean if a field has been set.


[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)
