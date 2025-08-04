# PiUserApplicationCommandsSubmitExaminationRequest

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**UserId** | Pointer to **string** |  | [optional] 
**ExamId** | Pointer to **string** |  | [optional] 
**ExamName** | Pointer to **NullableString** |  | [optional] 
**Score** | Pointer to **int32** |  | [optional] 
**ExpiredAt** | Pointer to **time.Time** |  | [optional] 

## Methods

### NewPiUserApplicationCommandsSubmitExaminationRequest

`func NewPiUserApplicationCommandsSubmitExaminationRequest() *PiUserApplicationCommandsSubmitExaminationRequest`

NewPiUserApplicationCommandsSubmitExaminationRequest instantiates a new PiUserApplicationCommandsSubmitExaminationRequest object
This constructor will assign default values to properties that have it defined,
and makes sure properties required by API are set, but the set of arguments
will change when the set of required properties is changed

### NewPiUserApplicationCommandsSubmitExaminationRequestWithDefaults

`func NewPiUserApplicationCommandsSubmitExaminationRequestWithDefaults() *PiUserApplicationCommandsSubmitExaminationRequest`

NewPiUserApplicationCommandsSubmitExaminationRequestWithDefaults instantiates a new PiUserApplicationCommandsSubmitExaminationRequest object
This constructor will only assign default values to properties that have it defined,
but it doesn't guarantee that properties required by API are set

### GetUserId

`func (o *PiUserApplicationCommandsSubmitExaminationRequest) GetUserId() string`

GetUserId returns the UserId field if non-nil, zero value otherwise.

### GetUserIdOk

`func (o *PiUserApplicationCommandsSubmitExaminationRequest) GetUserIdOk() (*string, bool)`

GetUserIdOk returns a tuple with the UserId field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetUserId

`func (o *PiUserApplicationCommandsSubmitExaminationRequest) SetUserId(v string)`

SetUserId sets UserId field to given value.

### HasUserId

`func (o *PiUserApplicationCommandsSubmitExaminationRequest) HasUserId() bool`

HasUserId returns a boolean if a field has been set.

### GetExamId

`func (o *PiUserApplicationCommandsSubmitExaminationRequest) GetExamId() string`

GetExamId returns the ExamId field if non-nil, zero value otherwise.

### GetExamIdOk

`func (o *PiUserApplicationCommandsSubmitExaminationRequest) GetExamIdOk() (*string, bool)`

GetExamIdOk returns a tuple with the ExamId field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetExamId

`func (o *PiUserApplicationCommandsSubmitExaminationRequest) SetExamId(v string)`

SetExamId sets ExamId field to given value.

### HasExamId

`func (o *PiUserApplicationCommandsSubmitExaminationRequest) HasExamId() bool`

HasExamId returns a boolean if a field has been set.

### GetExamName

`func (o *PiUserApplicationCommandsSubmitExaminationRequest) GetExamName() string`

GetExamName returns the ExamName field if non-nil, zero value otherwise.

### GetExamNameOk

`func (o *PiUserApplicationCommandsSubmitExaminationRequest) GetExamNameOk() (*string, bool)`

GetExamNameOk returns a tuple with the ExamName field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetExamName

`func (o *PiUserApplicationCommandsSubmitExaminationRequest) SetExamName(v string)`

SetExamName sets ExamName field to given value.

### HasExamName

`func (o *PiUserApplicationCommandsSubmitExaminationRequest) HasExamName() bool`

HasExamName returns a boolean if a field has been set.

### SetExamNameNil

`func (o *PiUserApplicationCommandsSubmitExaminationRequest) SetExamNameNil(b bool)`

 SetExamNameNil sets the value for ExamName to be an explicit nil

### UnsetExamName
`func (o *PiUserApplicationCommandsSubmitExaminationRequest) UnsetExamName()`

UnsetExamName ensures that no value is present for ExamName, not even an explicit nil
### GetScore

`func (o *PiUserApplicationCommandsSubmitExaminationRequest) GetScore() int32`

GetScore returns the Score field if non-nil, zero value otherwise.

### GetScoreOk

`func (o *PiUserApplicationCommandsSubmitExaminationRequest) GetScoreOk() (*int32, bool)`

GetScoreOk returns a tuple with the Score field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetScore

`func (o *PiUserApplicationCommandsSubmitExaminationRequest) SetScore(v int32)`

SetScore sets Score field to given value.

### HasScore

`func (o *PiUserApplicationCommandsSubmitExaminationRequest) HasScore() bool`

HasScore returns a boolean if a field has been set.

### GetExpiredAt

`func (o *PiUserApplicationCommandsSubmitExaminationRequest) GetExpiredAt() time.Time`

GetExpiredAt returns the ExpiredAt field if non-nil, zero value otherwise.

### GetExpiredAtOk

`func (o *PiUserApplicationCommandsSubmitExaminationRequest) GetExpiredAtOk() (*time.Time, bool)`

GetExpiredAtOk returns a tuple with the ExpiredAt field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetExpiredAt

`func (o *PiUserApplicationCommandsSubmitExaminationRequest) SetExpiredAt(v time.Time)`

SetExpiredAt sets ExpiredAt field to given value.

### HasExpiredAt

`func (o *PiUserApplicationCommandsSubmitExaminationRequest) HasExpiredAt() bool`

HasExpiredAt returns a boolean if a field has been set.


[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


