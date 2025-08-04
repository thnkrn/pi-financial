# DtoLinkUserAccountRequest

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**Status** | Pointer to [**DomainUserAccountStatus**](DomainUserAccountStatus.md) |  | [optional] 
**UserAccountId** | **string** |  | 
**UserAccountType** | [**DomainUserAccountType**](DomainUserAccountType.md) |  | 

## Methods

### NewDtoLinkUserAccountRequest

`func NewDtoLinkUserAccountRequest(userAccountId string, userAccountType DomainUserAccountType, ) *DtoLinkUserAccountRequest`

NewDtoLinkUserAccountRequest instantiates a new DtoLinkUserAccountRequest object
This constructor will assign default values to properties that have it defined,
and makes sure properties required by API are set, but the set of arguments
will change when the set of required properties is changed

### NewDtoLinkUserAccountRequestWithDefaults

`func NewDtoLinkUserAccountRequestWithDefaults() *DtoLinkUserAccountRequest`

NewDtoLinkUserAccountRequestWithDefaults instantiates a new DtoLinkUserAccountRequest object
This constructor will only assign default values to properties that have it defined,
but it doesn't guarantee that properties required by API are set

### GetStatus

`func (o *DtoLinkUserAccountRequest) GetStatus() DomainUserAccountStatus`

GetStatus returns the Status field if non-nil, zero value otherwise.

### GetStatusOk

`func (o *DtoLinkUserAccountRequest) GetStatusOk() (*DomainUserAccountStatus, bool)`

GetStatusOk returns a tuple with the Status field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetStatus

`func (o *DtoLinkUserAccountRequest) SetStatus(v DomainUserAccountStatus)`

SetStatus sets Status field to given value.

### HasStatus

`func (o *DtoLinkUserAccountRequest) HasStatus() bool`

HasStatus returns a boolean if a field has been set.

### GetUserAccountId

`func (o *DtoLinkUserAccountRequest) GetUserAccountId() string`

GetUserAccountId returns the UserAccountId field if non-nil, zero value otherwise.

### GetUserAccountIdOk

`func (o *DtoLinkUserAccountRequest) GetUserAccountIdOk() (*string, bool)`

GetUserAccountIdOk returns a tuple with the UserAccountId field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetUserAccountId

`func (o *DtoLinkUserAccountRequest) SetUserAccountId(v string)`

SetUserAccountId sets UserAccountId field to given value.


### GetUserAccountType

`func (o *DtoLinkUserAccountRequest) GetUserAccountType() DomainUserAccountType`

GetUserAccountType returns the UserAccountType field if non-nil, zero value otherwise.

### GetUserAccountTypeOk

`func (o *DtoLinkUserAccountRequest) GetUserAccountTypeOk() (*DomainUserAccountType, bool)`

GetUserAccountTypeOk returns a tuple with the UserAccountType field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetUserAccountType

`func (o *DtoLinkUserAccountRequest) SetUserAccountType(v DomainUserAccountType)`

SetUserAccountType sets UserAccountType field to given value.



[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


