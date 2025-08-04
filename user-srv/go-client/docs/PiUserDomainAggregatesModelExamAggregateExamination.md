# PiUserDomainAggregatesModelExamAggregateExamination

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**RowVersion** | **string** |  | [readonly] 
**CreatedAt** | Pointer to **time.Time** |  | [optional] 
**UpdatedAt** | Pointer to **time.Time** |  | [optional] 
**Id** | Pointer to **string** |  | [optional] 
**ExamId** | Pointer to **string** |  | [optional] 
**UserId** | Pointer to **string** |  | [optional] 
**ExamName** | Pointer to **NullableString** |  | [optional] 
**Score** | Pointer to **int32** |  | [optional] 
**ExpiredAt** | Pointer to **time.Time** |  | [optional] 
**User** | Pointer to [**PiUserDomainAggregatesModelUserInfoAggregateUserInfo**](PiUserDomainAggregatesModelUserInfoAggregateUserInfo.md) |  | [optional] 

## Methods

### NewPiUserDomainAggregatesModelExamAggregateExamination

`func NewPiUserDomainAggregatesModelExamAggregateExamination(rowVersion string, ) *PiUserDomainAggregatesModelExamAggregateExamination`

NewPiUserDomainAggregatesModelExamAggregateExamination instantiates a new PiUserDomainAggregatesModelExamAggregateExamination object
This constructor will assign default values to properties that have it defined,
and makes sure properties required by API are set, but the set of arguments
will change when the set of required properties is changed

### NewPiUserDomainAggregatesModelExamAggregateExaminationWithDefaults

`func NewPiUserDomainAggregatesModelExamAggregateExaminationWithDefaults() *PiUserDomainAggregatesModelExamAggregateExamination`

NewPiUserDomainAggregatesModelExamAggregateExaminationWithDefaults instantiates a new PiUserDomainAggregatesModelExamAggregateExamination object
This constructor will only assign default values to properties that have it defined,
but it doesn't guarantee that properties required by API are set

### GetRowVersion

`func (o *PiUserDomainAggregatesModelExamAggregateExamination) GetRowVersion() string`

GetRowVersion returns the RowVersion field if non-nil, zero value otherwise.

### GetRowVersionOk

`func (o *PiUserDomainAggregatesModelExamAggregateExamination) GetRowVersionOk() (*string, bool)`

GetRowVersionOk returns a tuple with the RowVersion field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetRowVersion

`func (o *PiUserDomainAggregatesModelExamAggregateExamination) SetRowVersion(v string)`

SetRowVersion sets RowVersion field to given value.


### GetCreatedAt

`func (o *PiUserDomainAggregatesModelExamAggregateExamination) GetCreatedAt() time.Time`

GetCreatedAt returns the CreatedAt field if non-nil, zero value otherwise.

### GetCreatedAtOk

`func (o *PiUserDomainAggregatesModelExamAggregateExamination) GetCreatedAtOk() (*time.Time, bool)`

GetCreatedAtOk returns a tuple with the CreatedAt field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetCreatedAt

`func (o *PiUserDomainAggregatesModelExamAggregateExamination) SetCreatedAt(v time.Time)`

SetCreatedAt sets CreatedAt field to given value.

### HasCreatedAt

`func (o *PiUserDomainAggregatesModelExamAggregateExamination) HasCreatedAt() bool`

HasCreatedAt returns a boolean if a field has been set.

### GetUpdatedAt

`func (o *PiUserDomainAggregatesModelExamAggregateExamination) GetUpdatedAt() time.Time`

GetUpdatedAt returns the UpdatedAt field if non-nil, zero value otherwise.

### GetUpdatedAtOk

`func (o *PiUserDomainAggregatesModelExamAggregateExamination) GetUpdatedAtOk() (*time.Time, bool)`

GetUpdatedAtOk returns a tuple with the UpdatedAt field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetUpdatedAt

`func (o *PiUserDomainAggregatesModelExamAggregateExamination) SetUpdatedAt(v time.Time)`

SetUpdatedAt sets UpdatedAt field to given value.

### HasUpdatedAt

`func (o *PiUserDomainAggregatesModelExamAggregateExamination) HasUpdatedAt() bool`

HasUpdatedAt returns a boolean if a field has been set.

### GetId

`func (o *PiUserDomainAggregatesModelExamAggregateExamination) GetId() string`

GetId returns the Id field if non-nil, zero value otherwise.

### GetIdOk

`func (o *PiUserDomainAggregatesModelExamAggregateExamination) GetIdOk() (*string, bool)`

GetIdOk returns a tuple with the Id field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetId

`func (o *PiUserDomainAggregatesModelExamAggregateExamination) SetId(v string)`

SetId sets Id field to given value.

### HasId

`func (o *PiUserDomainAggregatesModelExamAggregateExamination) HasId() bool`

HasId returns a boolean if a field has been set.

### GetExamId

`func (o *PiUserDomainAggregatesModelExamAggregateExamination) GetExamId() string`

GetExamId returns the ExamId field if non-nil, zero value otherwise.

### GetExamIdOk

`func (o *PiUserDomainAggregatesModelExamAggregateExamination) GetExamIdOk() (*string, bool)`

GetExamIdOk returns a tuple with the ExamId field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetExamId

`func (o *PiUserDomainAggregatesModelExamAggregateExamination) SetExamId(v string)`

SetExamId sets ExamId field to given value.

### HasExamId

`func (o *PiUserDomainAggregatesModelExamAggregateExamination) HasExamId() bool`

HasExamId returns a boolean if a field has been set.

### GetUserId

`func (o *PiUserDomainAggregatesModelExamAggregateExamination) GetUserId() string`

GetUserId returns the UserId field if non-nil, zero value otherwise.

### GetUserIdOk

`func (o *PiUserDomainAggregatesModelExamAggregateExamination) GetUserIdOk() (*string, bool)`

GetUserIdOk returns a tuple with the UserId field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetUserId

`func (o *PiUserDomainAggregatesModelExamAggregateExamination) SetUserId(v string)`

SetUserId sets UserId field to given value.

### HasUserId

`func (o *PiUserDomainAggregatesModelExamAggregateExamination) HasUserId() bool`

HasUserId returns a boolean if a field has been set.

### GetExamName

`func (o *PiUserDomainAggregatesModelExamAggregateExamination) GetExamName() string`

GetExamName returns the ExamName field if non-nil, zero value otherwise.

### GetExamNameOk

`func (o *PiUserDomainAggregatesModelExamAggregateExamination) GetExamNameOk() (*string, bool)`

GetExamNameOk returns a tuple with the ExamName field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetExamName

`func (o *PiUserDomainAggregatesModelExamAggregateExamination) SetExamName(v string)`

SetExamName sets ExamName field to given value.

### HasExamName

`func (o *PiUserDomainAggregatesModelExamAggregateExamination) HasExamName() bool`

HasExamName returns a boolean if a field has been set.

### SetExamNameNil

`func (o *PiUserDomainAggregatesModelExamAggregateExamination) SetExamNameNil(b bool)`

 SetExamNameNil sets the value for ExamName to be an explicit nil

### UnsetExamName
`func (o *PiUserDomainAggregatesModelExamAggregateExamination) UnsetExamName()`

UnsetExamName ensures that no value is present for ExamName, not even an explicit nil
### GetScore

`func (o *PiUserDomainAggregatesModelExamAggregateExamination) GetScore() int32`

GetScore returns the Score field if non-nil, zero value otherwise.

### GetScoreOk

`func (o *PiUserDomainAggregatesModelExamAggregateExamination) GetScoreOk() (*int32, bool)`

GetScoreOk returns a tuple with the Score field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetScore

`func (o *PiUserDomainAggregatesModelExamAggregateExamination) SetScore(v int32)`

SetScore sets Score field to given value.

### HasScore

`func (o *PiUserDomainAggregatesModelExamAggregateExamination) HasScore() bool`

HasScore returns a boolean if a field has been set.

### GetExpiredAt

`func (o *PiUserDomainAggregatesModelExamAggregateExamination) GetExpiredAt() time.Time`

GetExpiredAt returns the ExpiredAt field if non-nil, zero value otherwise.

### GetExpiredAtOk

`func (o *PiUserDomainAggregatesModelExamAggregateExamination) GetExpiredAtOk() (*time.Time, bool)`

GetExpiredAtOk returns a tuple with the ExpiredAt field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetExpiredAt

`func (o *PiUserDomainAggregatesModelExamAggregateExamination) SetExpiredAt(v time.Time)`

SetExpiredAt sets ExpiredAt field to given value.

### HasExpiredAt

`func (o *PiUserDomainAggregatesModelExamAggregateExamination) HasExpiredAt() bool`

HasExpiredAt returns a boolean if a field has been set.

### GetUser

`func (o *PiUserDomainAggregatesModelExamAggregateExamination) GetUser() PiUserDomainAggregatesModelUserInfoAggregateUserInfo`

GetUser returns the User field if non-nil, zero value otherwise.

### GetUserOk

`func (o *PiUserDomainAggregatesModelExamAggregateExamination) GetUserOk() (*PiUserDomainAggregatesModelUserInfoAggregateUserInfo, bool)`

GetUserOk returns a tuple with the User field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetUser

`func (o *PiUserDomainAggregatesModelExamAggregateExamination) SetUser(v PiUserDomainAggregatesModelUserInfoAggregateUserInfo)`

SetUser sets User field to given value.

### HasUser

`func (o *PiUserDomainAggregatesModelExamAggregateExamination) HasUser() bool`

HasUser returns a boolean if a field has been set.


[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


