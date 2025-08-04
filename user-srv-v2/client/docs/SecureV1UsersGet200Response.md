# SecureV1UsersGet200Response

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**Code** | Pointer to **string** |  | [optional]
**Msg** | Pointer to **string** |  | [optional]
**Data** | Pointer to [**DtoUserInfo**](DtoUserInfo.md) |  | [optional]

## Methods

### NewSecureV1UsersGet200Response

`func NewSecureV1UsersGet200Response() *SecureV1UsersGet200Response`

NewSecureV1UsersGet200Response instantiates a new SecureV1UsersGet200Response object
This constructor will assign default values to properties that have it defined,
and makes sure properties required by API are set, but the set of arguments
will change when the set of required properties is changed

### NewSecureV1UsersGet200ResponseWithDefaults

`func NewSecureV1UsersGet200ResponseWithDefaults() *SecureV1UsersGet200Response`

NewSecureV1UsersGet200ResponseWithDefaults instantiates a new SecureV1UsersGet200Response object
This constructor will only assign default values to properties that have it defined,
but it doesn't guarantee that properties required by API are set

### GetCode

`func (o *SecureV1UsersGet200Response) GetCode() string`

GetCode returns the Code field if non-nil, zero value otherwise.

### GetCodeOk

`func (o *SecureV1UsersGet200Response) GetCodeOk() (*string, bool)`

GetCodeOk returns a tuple with the Code field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetCode

`func (o *SecureV1UsersGet200Response) SetCode(v string)`

SetCode sets Code field to given value.

### HasCode

`func (o *SecureV1UsersGet200Response) HasCode() bool`

HasCode returns a boolean if a field has been set.

### GetMsg

`func (o *SecureV1UsersGet200Response) GetMsg() string`

GetMsg returns the Msg field if non-nil, zero value otherwise.

### GetMsgOk

`func (o *SecureV1UsersGet200Response) GetMsgOk() (*string, bool)`

GetMsgOk returns a tuple with the Msg field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetMsg

`func (o *SecureV1UsersGet200Response) SetMsg(v string)`

SetMsg sets Msg field to given value.

### HasMsg

`func (o *SecureV1UsersGet200Response) HasMsg() bool`

HasMsg returns a boolean if a field has been set.

### GetData

`func (o *SecureV1UsersGet200Response) GetData() DtoUserInfo`

GetData returns the Data field if non-nil, zero value otherwise.

### GetDataOk

`func (o *SecureV1UsersGet200Response) GetDataOk() (*DtoUserInfo, bool)`

GetDataOk returns a tuple with the Data field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetData

`func (o *SecureV1UsersGet200Response) SetData(v DtoUserInfo)`

SetData sets Data field to given value.

### HasData

`func (o *SecureV1UsersGet200Response) HasData() bool`

HasData returns a boolean if a field has been set.


[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)
