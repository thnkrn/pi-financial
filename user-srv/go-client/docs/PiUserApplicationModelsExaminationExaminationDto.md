# PiUserApplicationModelsExaminationExaminationDto

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**Id** | Pointer to **string** |  | [optional] 
**ExamId** | Pointer to **string** |  | [optional] 
**ExamName** | Pointer to **NullableString** |  | [optional] 
**Score** | Pointer to **int32** |  | [optional] 
**ExpiredAt** | Pointer to **time.Time** |  | [optional] 
**Grade** | Pointer to **NullableString** |  | [optional] 
**UpdatedAt** | Pointer to **time.Time** |  | [optional] 

## Methods

### NewPiUserApplicationModelsExaminationExaminationDto

`func NewPiUserApplicationModelsExaminationExaminationDto() *PiUserApplicationModelsExaminationExaminationDto`

NewPiUserApplicationModelsExaminationExaminationDto instantiates a new PiUserApplicationModelsExaminationExaminationDto object
This constructor will assign default values to properties that have it defined,
and makes sure properties required by API are set, but the set of arguments
will change when the set of required properties is changed

### NewPiUserApplicationModelsExaminationExaminationDtoWithDefaults

`func NewPiUserApplicationModelsExaminationExaminationDtoWithDefaults() *PiUserApplicationModelsExaminationExaminationDto`

NewPiUserApplicationModelsExaminationExaminationDtoWithDefaults instantiates a new PiUserApplicationModelsExaminationExaminationDto object
This constructor will only assign default values to properties that have it defined,
but it doesn't guarantee that properties required by API are set

### GetId

`func (o *PiUserApplicationModelsExaminationExaminationDto) GetId() string`

GetId returns the Id field if non-nil, zero value otherwise.

### GetIdOk

`func (o *PiUserApplicationModelsExaminationExaminationDto) GetIdOk() (*string, bool)`

GetIdOk returns a tuple with the Id field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetId

`func (o *PiUserApplicationModelsExaminationExaminationDto) SetId(v string)`

SetId sets Id field to given value.

### HasId

`func (o *PiUserApplicationModelsExaminationExaminationDto) HasId() bool`

HasId returns a boolean if a field has been set.

### GetExamId

`func (o *PiUserApplicationModelsExaminationExaminationDto) GetExamId() string`

GetExamId returns the ExamId field if non-nil, zero value otherwise.

### GetExamIdOk

`func (o *PiUserApplicationModelsExaminationExaminationDto) GetExamIdOk() (*string, bool)`

GetExamIdOk returns a tuple with the ExamId field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetExamId

`func (o *PiUserApplicationModelsExaminationExaminationDto) SetExamId(v string)`

SetExamId sets ExamId field to given value.

### HasExamId

`func (o *PiUserApplicationModelsExaminationExaminationDto) HasExamId() bool`

HasExamId returns a boolean if a field has been set.

### GetExamName

`func (o *PiUserApplicationModelsExaminationExaminationDto) GetExamName() string`

GetExamName returns the ExamName field if non-nil, zero value otherwise.

### GetExamNameOk

`func (o *PiUserApplicationModelsExaminationExaminationDto) GetExamNameOk() (*string, bool)`

GetExamNameOk returns a tuple with the ExamName field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetExamName

`func (o *PiUserApplicationModelsExaminationExaminationDto) SetExamName(v string)`

SetExamName sets ExamName field to given value.

### HasExamName

`func (o *PiUserApplicationModelsExaminationExaminationDto) HasExamName() bool`

HasExamName returns a boolean if a field has been set.

### SetExamNameNil

`func (o *PiUserApplicationModelsExaminationExaminationDto) SetExamNameNil(b bool)`

 SetExamNameNil sets the value for ExamName to be an explicit nil

### UnsetExamName
`func (o *PiUserApplicationModelsExaminationExaminationDto) UnsetExamName()`

UnsetExamName ensures that no value is present for ExamName, not even an explicit nil
### GetScore

`func (o *PiUserApplicationModelsExaminationExaminationDto) GetScore() int32`

GetScore returns the Score field if non-nil, zero value otherwise.

### GetScoreOk

`func (o *PiUserApplicationModelsExaminationExaminationDto) GetScoreOk() (*int32, bool)`

GetScoreOk returns a tuple with the Score field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetScore

`func (o *PiUserApplicationModelsExaminationExaminationDto) SetScore(v int32)`

SetScore sets Score field to given value.

### HasScore

`func (o *PiUserApplicationModelsExaminationExaminationDto) HasScore() bool`

HasScore returns a boolean if a field has been set.

### GetExpiredAt

`func (o *PiUserApplicationModelsExaminationExaminationDto) GetExpiredAt() time.Time`

GetExpiredAt returns the ExpiredAt field if non-nil, zero value otherwise.

### GetExpiredAtOk

`func (o *PiUserApplicationModelsExaminationExaminationDto) GetExpiredAtOk() (*time.Time, bool)`

GetExpiredAtOk returns a tuple with the ExpiredAt field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetExpiredAt

`func (o *PiUserApplicationModelsExaminationExaminationDto) SetExpiredAt(v time.Time)`

SetExpiredAt sets ExpiredAt field to given value.

### HasExpiredAt

`func (o *PiUserApplicationModelsExaminationExaminationDto) HasExpiredAt() bool`

HasExpiredAt returns a boolean if a field has been set.

### GetGrade

`func (o *PiUserApplicationModelsExaminationExaminationDto) GetGrade() string`

GetGrade returns the Grade field if non-nil, zero value otherwise.

### GetGradeOk

`func (o *PiUserApplicationModelsExaminationExaminationDto) GetGradeOk() (*string, bool)`

GetGradeOk returns a tuple with the Grade field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetGrade

`func (o *PiUserApplicationModelsExaminationExaminationDto) SetGrade(v string)`

SetGrade sets Grade field to given value.

### HasGrade

`func (o *PiUserApplicationModelsExaminationExaminationDto) HasGrade() bool`

HasGrade returns a boolean if a field has been set.

### SetGradeNil

`func (o *PiUserApplicationModelsExaminationExaminationDto) SetGradeNil(b bool)`

 SetGradeNil sets the value for Grade to be an explicit nil

### UnsetGrade
`func (o *PiUserApplicationModelsExaminationExaminationDto) UnsetGrade()`

UnsetGrade ensures that no value is present for Grade, not even an explicit nil
### GetUpdatedAt

`func (o *PiUserApplicationModelsExaminationExaminationDto) GetUpdatedAt() time.Time`

GetUpdatedAt returns the UpdatedAt field if non-nil, zero value otherwise.

### GetUpdatedAtOk

`func (o *PiUserApplicationModelsExaminationExaminationDto) GetUpdatedAtOk() (*time.Time, bool)`

GetUpdatedAtOk returns a tuple with the UpdatedAt field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetUpdatedAt

`func (o *PiUserApplicationModelsExaminationExaminationDto) SetUpdatedAt(v time.Time)`

SetUpdatedAt sets UpdatedAt field to given value.

### HasUpdatedAt

`func (o *PiUserApplicationModelsExaminationExaminationDto) HasUpdatedAt() bool`

HasUpdatedAt returns a boolean if a field has been set.


[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


