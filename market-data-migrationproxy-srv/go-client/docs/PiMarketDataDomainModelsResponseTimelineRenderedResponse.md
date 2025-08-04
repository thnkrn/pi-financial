# PiMarketDataDomainModelsResponseTimelineRenderedResponse

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**Venue** | Pointer to **NullableString** |  | [optional] 
**Symbol** | Pointer to **NullableString** |  | [optional] 
**Data** | Pointer to **[][]interface{}** |  | [optional] 
**Intermissions** | Pointer to [**[]PiMarketDataDomainModelsResponseIntermission**](PiMarketDataDomainModelsResponseIntermission.md) |  | [optional] 
**Meta** | Pointer to [**PiMarketDataDomainModelsResponseTimelineMeta**](PiMarketDataDomainModelsResponseTimelineMeta.md) |  | [optional] 

## Methods

### NewPiMarketDataDomainModelsResponseTimelineRenderedResponse

`func NewPiMarketDataDomainModelsResponseTimelineRenderedResponse() *PiMarketDataDomainModelsResponseTimelineRenderedResponse`

NewPiMarketDataDomainModelsResponseTimelineRenderedResponse instantiates a new PiMarketDataDomainModelsResponseTimelineRenderedResponse object
This constructor will assign default values to properties that have it defined,
and makes sure properties required by API are set, but the set of arguments
will change when the set of required properties is changed

### NewPiMarketDataDomainModelsResponseTimelineRenderedResponseWithDefaults

`func NewPiMarketDataDomainModelsResponseTimelineRenderedResponseWithDefaults() *PiMarketDataDomainModelsResponseTimelineRenderedResponse`

NewPiMarketDataDomainModelsResponseTimelineRenderedResponseWithDefaults instantiates a new PiMarketDataDomainModelsResponseTimelineRenderedResponse object
This constructor will only assign default values to properties that have it defined,
but it doesn't guarantee that properties required by API are set

### GetVenue

`func (o *PiMarketDataDomainModelsResponseTimelineRenderedResponse) GetVenue() string`

GetVenue returns the Venue field if non-nil, zero value otherwise.

### GetVenueOk

`func (o *PiMarketDataDomainModelsResponseTimelineRenderedResponse) GetVenueOk() (*string, bool)`

GetVenueOk returns a tuple with the Venue field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetVenue

`func (o *PiMarketDataDomainModelsResponseTimelineRenderedResponse) SetVenue(v string)`

SetVenue sets Venue field to given value.

### HasVenue

`func (o *PiMarketDataDomainModelsResponseTimelineRenderedResponse) HasVenue() bool`

HasVenue returns a boolean if a field has been set.

### SetVenueNil

`func (o *PiMarketDataDomainModelsResponseTimelineRenderedResponse) SetVenueNil(b bool)`

 SetVenueNil sets the value for Venue to be an explicit nil

### UnsetVenue
`func (o *PiMarketDataDomainModelsResponseTimelineRenderedResponse) UnsetVenue()`

UnsetVenue ensures that no value is present for Venue, not even an explicit nil
### GetSymbol

`func (o *PiMarketDataDomainModelsResponseTimelineRenderedResponse) GetSymbol() string`

GetSymbol returns the Symbol field if non-nil, zero value otherwise.

### GetSymbolOk

`func (o *PiMarketDataDomainModelsResponseTimelineRenderedResponse) GetSymbolOk() (*string, bool)`

GetSymbolOk returns a tuple with the Symbol field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetSymbol

`func (o *PiMarketDataDomainModelsResponseTimelineRenderedResponse) SetSymbol(v string)`

SetSymbol sets Symbol field to given value.

### HasSymbol

`func (o *PiMarketDataDomainModelsResponseTimelineRenderedResponse) HasSymbol() bool`

HasSymbol returns a boolean if a field has been set.

### SetSymbolNil

`func (o *PiMarketDataDomainModelsResponseTimelineRenderedResponse) SetSymbolNil(b bool)`

 SetSymbolNil sets the value for Symbol to be an explicit nil

### UnsetSymbol
`func (o *PiMarketDataDomainModelsResponseTimelineRenderedResponse) UnsetSymbol()`

UnsetSymbol ensures that no value is present for Symbol, not even an explicit nil
### GetData

`func (o *PiMarketDataDomainModelsResponseTimelineRenderedResponse) GetData() [][]interface{}`

GetData returns the Data field if non-nil, zero value otherwise.

### GetDataOk

`func (o *PiMarketDataDomainModelsResponseTimelineRenderedResponse) GetDataOk() (*[][]interface{}, bool)`

GetDataOk returns a tuple with the Data field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetData

`func (o *PiMarketDataDomainModelsResponseTimelineRenderedResponse) SetData(v [][]interface{})`

SetData sets Data field to given value.

### HasData

`func (o *PiMarketDataDomainModelsResponseTimelineRenderedResponse) HasData() bool`

HasData returns a boolean if a field has been set.

### SetDataNil

`func (o *PiMarketDataDomainModelsResponseTimelineRenderedResponse) SetDataNil(b bool)`

 SetDataNil sets the value for Data to be an explicit nil

### UnsetData
`func (o *PiMarketDataDomainModelsResponseTimelineRenderedResponse) UnsetData()`

UnsetData ensures that no value is present for Data, not even an explicit nil
### GetIntermissions

`func (o *PiMarketDataDomainModelsResponseTimelineRenderedResponse) GetIntermissions() []PiMarketDataDomainModelsResponseIntermission`

GetIntermissions returns the Intermissions field if non-nil, zero value otherwise.

### GetIntermissionsOk

`func (o *PiMarketDataDomainModelsResponseTimelineRenderedResponse) GetIntermissionsOk() (*[]PiMarketDataDomainModelsResponseIntermission, bool)`

GetIntermissionsOk returns a tuple with the Intermissions field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetIntermissions

`func (o *PiMarketDataDomainModelsResponseTimelineRenderedResponse) SetIntermissions(v []PiMarketDataDomainModelsResponseIntermission)`

SetIntermissions sets Intermissions field to given value.

### HasIntermissions

`func (o *PiMarketDataDomainModelsResponseTimelineRenderedResponse) HasIntermissions() bool`

HasIntermissions returns a boolean if a field has been set.

### SetIntermissionsNil

`func (o *PiMarketDataDomainModelsResponseTimelineRenderedResponse) SetIntermissionsNil(b bool)`

 SetIntermissionsNil sets the value for Intermissions to be an explicit nil

### UnsetIntermissions
`func (o *PiMarketDataDomainModelsResponseTimelineRenderedResponse) UnsetIntermissions()`

UnsetIntermissions ensures that no value is present for Intermissions, not even an explicit nil
### GetMeta

`func (o *PiMarketDataDomainModelsResponseTimelineRenderedResponse) GetMeta() PiMarketDataDomainModelsResponseTimelineMeta`

GetMeta returns the Meta field if non-nil, zero value otherwise.

### GetMetaOk

`func (o *PiMarketDataDomainModelsResponseTimelineRenderedResponse) GetMetaOk() (*PiMarketDataDomainModelsResponseTimelineMeta, bool)`

GetMetaOk returns a tuple with the Meta field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetMeta

`func (o *PiMarketDataDomainModelsResponseTimelineRenderedResponse) SetMeta(v PiMarketDataDomainModelsResponseTimelineMeta)`

SetMeta sets Meta field to given value.

### HasMeta

`func (o *PiMarketDataDomainModelsResponseTimelineRenderedResponse) HasMeta() bool`

HasMeta returns a boolean if a field has been set.


[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


