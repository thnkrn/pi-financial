# PiUserDomainAggregatesModelDocumentAggregateDocument

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**RowVersion** | **string** |  | [readonly] 
**Id** | Pointer to **string** |  | [optional] [readonly] 
**UserId** | Pointer to **string** |  | [optional] 
**DocumentType** | Pointer to **string** |  | [optional] 
**FileUrl** | Pointer to **NullableString** |  | [optional] 
**FileName** | Pointer to **NullableString** |  | [optional] 
**User** | Pointer to [**PiUserDomainAggregatesModelUserInfoAggregateUserInfo**](PiUserDomainAggregatesModelUserInfoAggregateUserInfo.md) |  | [optional] 
**CreatedAt** | Pointer to **time.Time** |  | [optional] 
**UpdatedAt** | Pointer to **NullableTime** |  | [optional] 

## Methods

### NewPiUserDomainAggregatesModelDocumentAggregateDocument

`func NewPiUserDomainAggregatesModelDocumentAggregateDocument(rowVersion string, ) *PiUserDomainAggregatesModelDocumentAggregateDocument`

NewPiUserDomainAggregatesModelDocumentAggregateDocument instantiates a new PiUserDomainAggregatesModelDocumentAggregateDocument object
This constructor will assign default values to properties that have it defined,
and makes sure properties required by API are set, but the set of arguments
will change when the set of required properties is changed

### NewPiUserDomainAggregatesModelDocumentAggregateDocumentWithDefaults

`func NewPiUserDomainAggregatesModelDocumentAggregateDocumentWithDefaults() *PiUserDomainAggregatesModelDocumentAggregateDocument`

NewPiUserDomainAggregatesModelDocumentAggregateDocumentWithDefaults instantiates a new PiUserDomainAggregatesModelDocumentAggregateDocument object
This constructor will only assign default values to properties that have it defined,
but it doesn't guarantee that properties required by API are set

### GetRowVersion

`func (o *PiUserDomainAggregatesModelDocumentAggregateDocument) GetRowVersion() string`

GetRowVersion returns the RowVersion field if non-nil, zero value otherwise.

### GetRowVersionOk

`func (o *PiUserDomainAggregatesModelDocumentAggregateDocument) GetRowVersionOk() (*string, bool)`

GetRowVersionOk returns a tuple with the RowVersion field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetRowVersion

`func (o *PiUserDomainAggregatesModelDocumentAggregateDocument) SetRowVersion(v string)`

SetRowVersion sets RowVersion field to given value.


### GetId

`func (o *PiUserDomainAggregatesModelDocumentAggregateDocument) GetId() string`

GetId returns the Id field if non-nil, zero value otherwise.

### GetIdOk

`func (o *PiUserDomainAggregatesModelDocumentAggregateDocument) GetIdOk() (*string, bool)`

GetIdOk returns a tuple with the Id field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetId

`func (o *PiUserDomainAggregatesModelDocumentAggregateDocument) SetId(v string)`

SetId sets Id field to given value.

### HasId

`func (o *PiUserDomainAggregatesModelDocumentAggregateDocument) HasId() bool`

HasId returns a boolean if a field has been set.

### GetUserId

`func (o *PiUserDomainAggregatesModelDocumentAggregateDocument) GetUserId() string`

GetUserId returns the UserId field if non-nil, zero value otherwise.

### GetUserIdOk

`func (o *PiUserDomainAggregatesModelDocumentAggregateDocument) GetUserIdOk() (*string, bool)`

GetUserIdOk returns a tuple with the UserId field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetUserId

`func (o *PiUserDomainAggregatesModelDocumentAggregateDocument) SetUserId(v string)`

SetUserId sets UserId field to given value.

### HasUserId

`func (o *PiUserDomainAggregatesModelDocumentAggregateDocument) HasUserId() bool`

HasUserId returns a boolean if a field has been set.

### GetDocumentType

`func (o *PiUserDomainAggregatesModelDocumentAggregateDocument) GetDocumentType() string`

GetDocumentType returns the DocumentType field if non-nil, zero value otherwise.

### GetDocumentTypeOk

`func (o *PiUserDomainAggregatesModelDocumentAggregateDocument) GetDocumentTypeOk() (*string, bool)`

GetDocumentTypeOk returns a tuple with the DocumentType field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetDocumentType

`func (o *PiUserDomainAggregatesModelDocumentAggregateDocument) SetDocumentType(v string)`

SetDocumentType sets DocumentType field to given value.

### HasDocumentType

`func (o *PiUserDomainAggregatesModelDocumentAggregateDocument) HasDocumentType() bool`

HasDocumentType returns a boolean if a field has been set.

### GetFileUrl

`func (o *PiUserDomainAggregatesModelDocumentAggregateDocument) GetFileUrl() string`

GetFileUrl returns the FileUrl field if non-nil, zero value otherwise.

### GetFileUrlOk

`func (o *PiUserDomainAggregatesModelDocumentAggregateDocument) GetFileUrlOk() (*string, bool)`

GetFileUrlOk returns a tuple with the FileUrl field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetFileUrl

`func (o *PiUserDomainAggregatesModelDocumentAggregateDocument) SetFileUrl(v string)`

SetFileUrl sets FileUrl field to given value.

### HasFileUrl

`func (o *PiUserDomainAggregatesModelDocumentAggregateDocument) HasFileUrl() bool`

HasFileUrl returns a boolean if a field has been set.

### SetFileUrlNil

`func (o *PiUserDomainAggregatesModelDocumentAggregateDocument) SetFileUrlNil(b bool)`

 SetFileUrlNil sets the value for FileUrl to be an explicit nil

### UnsetFileUrl
`func (o *PiUserDomainAggregatesModelDocumentAggregateDocument) UnsetFileUrl()`

UnsetFileUrl ensures that no value is present for FileUrl, not even an explicit nil
### GetFileName

`func (o *PiUserDomainAggregatesModelDocumentAggregateDocument) GetFileName() string`

GetFileName returns the FileName field if non-nil, zero value otherwise.

### GetFileNameOk

`func (o *PiUserDomainAggregatesModelDocumentAggregateDocument) GetFileNameOk() (*string, bool)`

GetFileNameOk returns a tuple with the FileName field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetFileName

`func (o *PiUserDomainAggregatesModelDocumentAggregateDocument) SetFileName(v string)`

SetFileName sets FileName field to given value.

### HasFileName

`func (o *PiUserDomainAggregatesModelDocumentAggregateDocument) HasFileName() bool`

HasFileName returns a boolean if a field has been set.

### SetFileNameNil

`func (o *PiUserDomainAggregatesModelDocumentAggregateDocument) SetFileNameNil(b bool)`

 SetFileNameNil sets the value for FileName to be an explicit nil

### UnsetFileName
`func (o *PiUserDomainAggregatesModelDocumentAggregateDocument) UnsetFileName()`

UnsetFileName ensures that no value is present for FileName, not even an explicit nil
### GetUser

`func (o *PiUserDomainAggregatesModelDocumentAggregateDocument) GetUser() PiUserDomainAggregatesModelUserInfoAggregateUserInfo`

GetUser returns the User field if non-nil, zero value otherwise.

### GetUserOk

`func (o *PiUserDomainAggregatesModelDocumentAggregateDocument) GetUserOk() (*PiUserDomainAggregatesModelUserInfoAggregateUserInfo, bool)`

GetUserOk returns a tuple with the User field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetUser

`func (o *PiUserDomainAggregatesModelDocumentAggregateDocument) SetUser(v PiUserDomainAggregatesModelUserInfoAggregateUserInfo)`

SetUser sets User field to given value.

### HasUser

`func (o *PiUserDomainAggregatesModelDocumentAggregateDocument) HasUser() bool`

HasUser returns a boolean if a field has been set.

### GetCreatedAt

`func (o *PiUserDomainAggregatesModelDocumentAggregateDocument) GetCreatedAt() time.Time`

GetCreatedAt returns the CreatedAt field if non-nil, zero value otherwise.

### GetCreatedAtOk

`func (o *PiUserDomainAggregatesModelDocumentAggregateDocument) GetCreatedAtOk() (*time.Time, bool)`

GetCreatedAtOk returns a tuple with the CreatedAt field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetCreatedAt

`func (o *PiUserDomainAggregatesModelDocumentAggregateDocument) SetCreatedAt(v time.Time)`

SetCreatedAt sets CreatedAt field to given value.

### HasCreatedAt

`func (o *PiUserDomainAggregatesModelDocumentAggregateDocument) HasCreatedAt() bool`

HasCreatedAt returns a boolean if a field has been set.

### GetUpdatedAt

`func (o *PiUserDomainAggregatesModelDocumentAggregateDocument) GetUpdatedAt() time.Time`

GetUpdatedAt returns the UpdatedAt field if non-nil, zero value otherwise.

### GetUpdatedAtOk

`func (o *PiUserDomainAggregatesModelDocumentAggregateDocument) GetUpdatedAtOk() (*time.Time, bool)`

GetUpdatedAtOk returns a tuple with the UpdatedAt field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetUpdatedAt

`func (o *PiUserDomainAggregatesModelDocumentAggregateDocument) SetUpdatedAt(v time.Time)`

SetUpdatedAt sets UpdatedAt field to given value.

### HasUpdatedAt

`func (o *PiUserDomainAggregatesModelDocumentAggregateDocument) HasUpdatedAt() bool`

HasUpdatedAt returns a boolean if a field has been set.

### SetUpdatedAtNil

`func (o *PiUserDomainAggregatesModelDocumentAggregateDocument) SetUpdatedAtNil(b bool)`

 SetUpdatedAtNil sets the value for UpdatedAt to be an explicit nil

### UnsetUpdatedAt
`func (o *PiUserDomainAggregatesModelDocumentAggregateDocument) UnsetUpdatedAt()`

UnsetUpdatedAt ensures that no value is present for UpdatedAt, not even an explicit nil

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


