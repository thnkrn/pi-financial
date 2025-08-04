# PiUserDomainAggregatesModelSuitabilityTestAggregateSuitabilityTest

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**RowVersion** | **string** |  | [readonly] 
**Id** | Pointer to **string** |  | [optional] [readonly] 
**Grade** | Pointer to **NullableString** |  | [optional] 
**Score** | Pointer to **int32** |  | [optional] 
**Version** | Pointer to **NullableString** |  | [optional] 
**ReviewDate** | Pointer to **time.Time** |  | [optional] 
**ExpiredDate** | Pointer to **time.Time** |  | [optional] 
**UserId** | Pointer to **string** |  | [optional] 

## Methods

### NewPiUserDomainAggregatesModelSuitabilityTestAggregateSuitabilityTest

`func NewPiUserDomainAggregatesModelSuitabilityTestAggregateSuitabilityTest(rowVersion string, ) *PiUserDomainAggregatesModelSuitabilityTestAggregateSuitabilityTest`

NewPiUserDomainAggregatesModelSuitabilityTestAggregateSuitabilityTest instantiates a new PiUserDomainAggregatesModelSuitabilityTestAggregateSuitabilityTest object
This constructor will assign default values to properties that have it defined,
and makes sure properties required by API are set, but the set of arguments
will change when the set of required properties is changed

### NewPiUserDomainAggregatesModelSuitabilityTestAggregateSuitabilityTestWithDefaults

`func NewPiUserDomainAggregatesModelSuitabilityTestAggregateSuitabilityTestWithDefaults() *PiUserDomainAggregatesModelSuitabilityTestAggregateSuitabilityTest`

NewPiUserDomainAggregatesModelSuitabilityTestAggregateSuitabilityTestWithDefaults instantiates a new PiUserDomainAggregatesModelSuitabilityTestAggregateSuitabilityTest object
This constructor will only assign default values to properties that have it defined,
but it doesn't guarantee that properties required by API are set

### GetRowVersion

`func (o *PiUserDomainAggregatesModelSuitabilityTestAggregateSuitabilityTest) GetRowVersion() string`

GetRowVersion returns the RowVersion field if non-nil, zero value otherwise.

### GetRowVersionOk

`func (o *PiUserDomainAggregatesModelSuitabilityTestAggregateSuitabilityTest) GetRowVersionOk() (*string, bool)`

GetRowVersionOk returns a tuple with the RowVersion field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetRowVersion

`func (o *PiUserDomainAggregatesModelSuitabilityTestAggregateSuitabilityTest) SetRowVersion(v string)`

SetRowVersion sets RowVersion field to given value.


### GetId

`func (o *PiUserDomainAggregatesModelSuitabilityTestAggregateSuitabilityTest) GetId() string`

GetId returns the Id field if non-nil, zero value otherwise.

### GetIdOk

`func (o *PiUserDomainAggregatesModelSuitabilityTestAggregateSuitabilityTest) GetIdOk() (*string, bool)`

GetIdOk returns a tuple with the Id field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetId

`func (o *PiUserDomainAggregatesModelSuitabilityTestAggregateSuitabilityTest) SetId(v string)`

SetId sets Id field to given value.

### HasId

`func (o *PiUserDomainAggregatesModelSuitabilityTestAggregateSuitabilityTest) HasId() bool`

HasId returns a boolean if a field has been set.

### GetGrade

`func (o *PiUserDomainAggregatesModelSuitabilityTestAggregateSuitabilityTest) GetGrade() string`

GetGrade returns the Grade field if non-nil, zero value otherwise.

### GetGradeOk

`func (o *PiUserDomainAggregatesModelSuitabilityTestAggregateSuitabilityTest) GetGradeOk() (*string, bool)`

GetGradeOk returns a tuple with the Grade field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetGrade

`func (o *PiUserDomainAggregatesModelSuitabilityTestAggregateSuitabilityTest) SetGrade(v string)`

SetGrade sets Grade field to given value.

### HasGrade

`func (o *PiUserDomainAggregatesModelSuitabilityTestAggregateSuitabilityTest) HasGrade() bool`

HasGrade returns a boolean if a field has been set.

### SetGradeNil

`func (o *PiUserDomainAggregatesModelSuitabilityTestAggregateSuitabilityTest) SetGradeNil(b bool)`

 SetGradeNil sets the value for Grade to be an explicit nil

### UnsetGrade
`func (o *PiUserDomainAggregatesModelSuitabilityTestAggregateSuitabilityTest) UnsetGrade()`

UnsetGrade ensures that no value is present for Grade, not even an explicit nil
### GetScore

`func (o *PiUserDomainAggregatesModelSuitabilityTestAggregateSuitabilityTest) GetScore() int32`

GetScore returns the Score field if non-nil, zero value otherwise.

### GetScoreOk

`func (o *PiUserDomainAggregatesModelSuitabilityTestAggregateSuitabilityTest) GetScoreOk() (*int32, bool)`

GetScoreOk returns a tuple with the Score field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetScore

`func (o *PiUserDomainAggregatesModelSuitabilityTestAggregateSuitabilityTest) SetScore(v int32)`

SetScore sets Score field to given value.

### HasScore

`func (o *PiUserDomainAggregatesModelSuitabilityTestAggregateSuitabilityTest) HasScore() bool`

HasScore returns a boolean if a field has been set.

### GetVersion

`func (o *PiUserDomainAggregatesModelSuitabilityTestAggregateSuitabilityTest) GetVersion() string`

GetVersion returns the Version field if non-nil, zero value otherwise.

### GetVersionOk

`func (o *PiUserDomainAggregatesModelSuitabilityTestAggregateSuitabilityTest) GetVersionOk() (*string, bool)`

GetVersionOk returns a tuple with the Version field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetVersion

`func (o *PiUserDomainAggregatesModelSuitabilityTestAggregateSuitabilityTest) SetVersion(v string)`

SetVersion sets Version field to given value.

### HasVersion

`func (o *PiUserDomainAggregatesModelSuitabilityTestAggregateSuitabilityTest) HasVersion() bool`

HasVersion returns a boolean if a field has been set.

### SetVersionNil

`func (o *PiUserDomainAggregatesModelSuitabilityTestAggregateSuitabilityTest) SetVersionNil(b bool)`

 SetVersionNil sets the value for Version to be an explicit nil

### UnsetVersion
`func (o *PiUserDomainAggregatesModelSuitabilityTestAggregateSuitabilityTest) UnsetVersion()`

UnsetVersion ensures that no value is present for Version, not even an explicit nil
### GetReviewDate

`func (o *PiUserDomainAggregatesModelSuitabilityTestAggregateSuitabilityTest) GetReviewDate() time.Time`

GetReviewDate returns the ReviewDate field if non-nil, zero value otherwise.

### GetReviewDateOk

`func (o *PiUserDomainAggregatesModelSuitabilityTestAggregateSuitabilityTest) GetReviewDateOk() (*time.Time, bool)`

GetReviewDateOk returns a tuple with the ReviewDate field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetReviewDate

`func (o *PiUserDomainAggregatesModelSuitabilityTestAggregateSuitabilityTest) SetReviewDate(v time.Time)`

SetReviewDate sets ReviewDate field to given value.

### HasReviewDate

`func (o *PiUserDomainAggregatesModelSuitabilityTestAggregateSuitabilityTest) HasReviewDate() bool`

HasReviewDate returns a boolean if a field has been set.

### GetExpiredDate

`func (o *PiUserDomainAggregatesModelSuitabilityTestAggregateSuitabilityTest) GetExpiredDate() time.Time`

GetExpiredDate returns the ExpiredDate field if non-nil, zero value otherwise.

### GetExpiredDateOk

`func (o *PiUserDomainAggregatesModelSuitabilityTestAggregateSuitabilityTest) GetExpiredDateOk() (*time.Time, bool)`

GetExpiredDateOk returns a tuple with the ExpiredDate field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetExpiredDate

`func (o *PiUserDomainAggregatesModelSuitabilityTestAggregateSuitabilityTest) SetExpiredDate(v time.Time)`

SetExpiredDate sets ExpiredDate field to given value.

### HasExpiredDate

`func (o *PiUserDomainAggregatesModelSuitabilityTestAggregateSuitabilityTest) HasExpiredDate() bool`

HasExpiredDate returns a boolean if a field has been set.

### GetUserId

`func (o *PiUserDomainAggregatesModelSuitabilityTestAggregateSuitabilityTest) GetUserId() string`

GetUserId returns the UserId field if non-nil, zero value otherwise.

### GetUserIdOk

`func (o *PiUserDomainAggregatesModelSuitabilityTestAggregateSuitabilityTest) GetUserIdOk() (*string, bool)`

GetUserIdOk returns a tuple with the UserId field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetUserId

`func (o *PiUserDomainAggregatesModelSuitabilityTestAggregateSuitabilityTest) SetUserId(v string)`

SetUserId sets UserId field to given value.

### HasUserId

`func (o *PiUserDomainAggregatesModelSuitabilityTestAggregateSuitabilityTest) HasUserId() bool`

HasUserId returns a boolean if a field has been set.


[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


