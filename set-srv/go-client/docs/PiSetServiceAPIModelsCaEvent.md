# PiSetServiceAPIModelsCaEvent

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**EventId** | Pointer to **string** |  | [optional] 
**Date** | **NullableString** |  | 
**CaType** | **NullableString** |  | 

## Methods

### NewPiSetServiceAPIModelsCaEvent

`func NewPiSetServiceAPIModelsCaEvent(date NullableString, caType NullableString, ) *PiSetServiceAPIModelsCaEvent`

NewPiSetServiceAPIModelsCaEvent instantiates a new PiSetServiceAPIModelsCaEvent object
This constructor will assign default values to properties that have it defined,
and makes sure properties required by API are set, but the set of arguments
will change when the set of required properties is changed

### NewPiSetServiceAPIModelsCaEventWithDefaults

`func NewPiSetServiceAPIModelsCaEventWithDefaults() *PiSetServiceAPIModelsCaEvent`

NewPiSetServiceAPIModelsCaEventWithDefaults instantiates a new PiSetServiceAPIModelsCaEvent object
This constructor will only assign default values to properties that have it defined,
but it doesn't guarantee that properties required by API are set

### GetEventId

`func (o *PiSetServiceAPIModelsCaEvent) GetEventId() string`

GetEventId returns the EventId field if non-nil, zero value otherwise.

### GetEventIdOk

`func (o *PiSetServiceAPIModelsCaEvent) GetEventIdOk() (*string, bool)`

GetEventIdOk returns a tuple with the EventId field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetEventId

`func (o *PiSetServiceAPIModelsCaEvent) SetEventId(v string)`

SetEventId sets EventId field to given value.

### HasEventId

`func (o *PiSetServiceAPIModelsCaEvent) HasEventId() bool`

HasEventId returns a boolean if a field has been set.

### GetDate

`func (o *PiSetServiceAPIModelsCaEvent) GetDate() string`

GetDate returns the Date field if non-nil, zero value otherwise.

### GetDateOk

`func (o *PiSetServiceAPIModelsCaEvent) GetDateOk() (*string, bool)`

GetDateOk returns a tuple with the Date field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetDate

`func (o *PiSetServiceAPIModelsCaEvent) SetDate(v string)`

SetDate sets Date field to given value.


### SetDateNil

`func (o *PiSetServiceAPIModelsCaEvent) SetDateNil(b bool)`

 SetDateNil sets the value for Date to be an explicit nil

### UnsetDate
`func (o *PiSetServiceAPIModelsCaEvent) UnsetDate()`

UnsetDate ensures that no value is present for Date, not even an explicit nil
### GetCaType

`func (o *PiSetServiceAPIModelsCaEvent) GetCaType() string`

GetCaType returns the CaType field if non-nil, zero value otherwise.

### GetCaTypeOk

`func (o *PiSetServiceAPIModelsCaEvent) GetCaTypeOk() (*string, bool)`

GetCaTypeOk returns a tuple with the CaType field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetCaType

`func (o *PiSetServiceAPIModelsCaEvent) SetCaType(v string)`

SetCaType sets CaType field to given value.


### SetCaTypeNil

`func (o *PiSetServiceAPIModelsCaEvent) SetCaTypeNil(b bool)`

 SetCaTypeNil sets the value for CaType to be an explicit nil

### UnsetCaType
`func (o *PiSetServiceAPIModelsCaEvent) UnsetCaType()`

UnsetCaType ensures that no value is present for CaType, not even an explicit nil

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


