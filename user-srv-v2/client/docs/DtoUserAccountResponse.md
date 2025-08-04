# DtoUserAccountResponse

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**Status** | Pointer to [**DomainUserAccountStatus**](DomainUserAccountStatus.md) |  | [optional] 
**UserAccountId** | Pointer to **string** |  | [optional] 
**UserAccountType** | Pointer to [**DomainUserAccountType**](DomainUserAccountType.md) |  | [optional] 

## Methods

### NewDtoUserAccountResponse

`func NewDtoUserAccountResponse() *DtoUserAccountResponse`

NewDtoUserAccountResponse instantiates a new DtoUserAccountResponse object
This constructor will assign default values to properties that have it defined,
and makes sure properties required by API are set, but the set of arguments
will change when the set of required properties is changed

### NewDtoUserAccountResponseWithDefaults

`func NewDtoUserAccountResponseWithDefaults() *DtoUserAccountResponse`

NewDtoUserAccountResponseWithDefaults instantiates a new DtoUserAccountResponse object
This constructor will only assign default values to properties that have it defined,
but it doesn't guarantee that properties required by API are set

### GetStatus

`func (o *DtoUserAccountResponse) GetStatus() DomainUserAccountStatus`

GetStatus returns the Status field if non-nil, zero value otherwise.

### GetStatusOk

`func (o *DtoUserAccountResponse) GetStatusOk() (*DomainUserAccountStatus, bool)`

GetStatusOk returns a tuple with the Status field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetStatus

`func (o *DtoUserAccountResponse) SetStatus(v DomainUserAccountStatus)`

SetStatus sets Status field to given value.

### HasStatus

`func (o *DtoUserAccountResponse) HasStatus() bool`

HasStatus returns a boolean if a field has been set.

### GetUserAccountId

`func (o *DtoUserAccountResponse) GetUserAccountId() string`

GetUserAccountId returns the UserAccountId field if non-nil, zero value otherwise.

### GetUserAccountIdOk

`func (o *DtoUserAccountResponse) GetUserAccountIdOk() (*string, bool)`

GetUserAccountIdOk returns a tuple with the UserAccountId field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetUserAccountId

`func (o *DtoUserAccountResponse) SetUserAccountId(v string)`

SetUserAccountId sets UserAccountId field to given value.

### HasUserAccountId

`func (o *DtoUserAccountResponse) HasUserAccountId() bool`

HasUserAccountId returns a boolean if a field has been set.

### GetUserAccountType

`func (o *DtoUserAccountResponse) GetUserAccountType() DomainUserAccountType`

GetUserAccountType returns the UserAccountType field if non-nil, zero value otherwise.

### GetUserAccountTypeOk

`func (o *DtoUserAccountResponse) GetUserAccountTypeOk() (*DomainUserAccountType, bool)`

GetUserAccountTypeOk returns a tuple with the UserAccountType field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetUserAccountType

`func (o *DtoUserAccountResponse) SetUserAccountType(v DomainUserAccountType)`

SetUserAccountType sets UserAccountType field to given value.

### HasUserAccountType

`func (o *DtoUserAccountResponse) HasUserAccountType() bool`

HasUserAccountType returns a boolean if a field has been set.


[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


