# PiMarketDataDomainModelsResponseSchedulesResponse

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**ServerTimestamp** | Pointer to **int64** |  | [optional] 
**Schedules** | Pointer to [**[]PiMarketDataDomainModelsResponseSchedules**](PiMarketDataDomainModelsResponseSchedules.md) |  | [optional] 

## Methods

### NewPiMarketDataDomainModelsResponseSchedulesResponse

`func NewPiMarketDataDomainModelsResponseSchedulesResponse() *PiMarketDataDomainModelsResponseSchedulesResponse`

NewPiMarketDataDomainModelsResponseSchedulesResponse instantiates a new PiMarketDataDomainModelsResponseSchedulesResponse object
This constructor will assign default values to properties that have it defined,
and makes sure properties required by API are set, but the set of arguments
will change when the set of required properties is changed

### NewPiMarketDataDomainModelsResponseSchedulesResponseWithDefaults

`func NewPiMarketDataDomainModelsResponseSchedulesResponseWithDefaults() *PiMarketDataDomainModelsResponseSchedulesResponse`

NewPiMarketDataDomainModelsResponseSchedulesResponseWithDefaults instantiates a new PiMarketDataDomainModelsResponseSchedulesResponse object
This constructor will only assign default values to properties that have it defined,
but it doesn't guarantee that properties required by API are set

### GetServerTimestamp

`func (o *PiMarketDataDomainModelsResponseSchedulesResponse) GetServerTimestamp() int64`

GetServerTimestamp returns the ServerTimestamp field if non-nil, zero value otherwise.

### GetServerTimestampOk

`func (o *PiMarketDataDomainModelsResponseSchedulesResponse) GetServerTimestampOk() (*int64, bool)`

GetServerTimestampOk returns a tuple with the ServerTimestamp field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetServerTimestamp

`func (o *PiMarketDataDomainModelsResponseSchedulesResponse) SetServerTimestamp(v int64)`

SetServerTimestamp sets ServerTimestamp field to given value.

### HasServerTimestamp

`func (o *PiMarketDataDomainModelsResponseSchedulesResponse) HasServerTimestamp() bool`

HasServerTimestamp returns a boolean if a field has been set.

### GetSchedules

`func (o *PiMarketDataDomainModelsResponseSchedulesResponse) GetSchedules() []PiMarketDataDomainModelsResponseSchedules`

GetSchedules returns the Schedules field if non-nil, zero value otherwise.

### GetSchedulesOk

`func (o *PiMarketDataDomainModelsResponseSchedulesResponse) GetSchedulesOk() (*[]PiMarketDataDomainModelsResponseSchedules, bool)`

GetSchedulesOk returns a tuple with the Schedules field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetSchedules

`func (o *PiMarketDataDomainModelsResponseSchedulesResponse) SetSchedules(v []PiMarketDataDomainModelsResponseSchedules)`

SetSchedules sets Schedules field to given value.

### HasSchedules

`func (o *PiMarketDataDomainModelsResponseSchedulesResponse) HasSchedules() bool`

HasSchedules returns a boolean if a field has been set.

### SetSchedulesNil

`func (o *PiMarketDataDomainModelsResponseSchedulesResponse) SetSchedulesNil(b bool)`

 SetSchedulesNil sets the value for Schedules to be an explicit nil

### UnsetSchedules
`func (o *PiMarketDataDomainModelsResponseSchedulesResponse) UnsetSchedules()`

UnsetSchedules ensures that no value is present for Schedules, not even an explicit nil

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


