# PiMarketDataDomainModelsResponseIndicatorResponse

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**Meta** | Pointer to [**PiMarketDataDomainModelsResponseMeta**](PiMarketDataDomainModelsResponseMeta.md) |  | [optional] 
**Venue** | Pointer to **NullableString** |  | [optional] 
**Symbol** | Pointer to **NullableString** |  | [optional] 
**CandleType** | Pointer to **NullableString** |  | [optional] 
**Candles** | Pointer to **[][]interface{}** |  | [optional] 
**Indicators** | Pointer to [**PiMarketDataDomainModelsResponseTechnicalIndicatorsResponse**](PiMarketDataDomainModelsResponseTechnicalIndicatorsResponse.md) |  | [optional] 
**FirstCandleTime** | Pointer to **int32** |  | [optional] 

## Methods

### NewPiMarketDataDomainModelsResponseIndicatorResponse

`func NewPiMarketDataDomainModelsResponseIndicatorResponse() *PiMarketDataDomainModelsResponseIndicatorResponse`

NewPiMarketDataDomainModelsResponseIndicatorResponse instantiates a new PiMarketDataDomainModelsResponseIndicatorResponse object
This constructor will assign default values to properties that have it defined,
and makes sure properties required by API are set, but the set of arguments
will change when the set of required properties is changed

### NewPiMarketDataDomainModelsResponseIndicatorResponseWithDefaults

`func NewPiMarketDataDomainModelsResponseIndicatorResponseWithDefaults() *PiMarketDataDomainModelsResponseIndicatorResponse`

NewPiMarketDataDomainModelsResponseIndicatorResponseWithDefaults instantiates a new PiMarketDataDomainModelsResponseIndicatorResponse object
This constructor will only assign default values to properties that have it defined,
but it doesn't guarantee that properties required by API are set

### GetMeta

`func (o *PiMarketDataDomainModelsResponseIndicatorResponse) GetMeta() PiMarketDataDomainModelsResponseMeta`

GetMeta returns the Meta field if non-nil, zero value otherwise.

### GetMetaOk

`func (o *PiMarketDataDomainModelsResponseIndicatorResponse) GetMetaOk() (*PiMarketDataDomainModelsResponseMeta, bool)`

GetMetaOk returns a tuple with the Meta field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetMeta

`func (o *PiMarketDataDomainModelsResponseIndicatorResponse) SetMeta(v PiMarketDataDomainModelsResponseMeta)`

SetMeta sets Meta field to given value.

### HasMeta

`func (o *PiMarketDataDomainModelsResponseIndicatorResponse) HasMeta() bool`

HasMeta returns a boolean if a field has been set.

### GetVenue

`func (o *PiMarketDataDomainModelsResponseIndicatorResponse) GetVenue() string`

GetVenue returns the Venue field if non-nil, zero value otherwise.

### GetVenueOk

`func (o *PiMarketDataDomainModelsResponseIndicatorResponse) GetVenueOk() (*string, bool)`

GetVenueOk returns a tuple with the Venue field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetVenue

`func (o *PiMarketDataDomainModelsResponseIndicatorResponse) SetVenue(v string)`

SetVenue sets Venue field to given value.

### HasVenue

`func (o *PiMarketDataDomainModelsResponseIndicatorResponse) HasVenue() bool`

HasVenue returns a boolean if a field has been set.

### SetVenueNil

`func (o *PiMarketDataDomainModelsResponseIndicatorResponse) SetVenueNil(b bool)`

 SetVenueNil sets the value for Venue to be an explicit nil

### UnsetVenue
`func (o *PiMarketDataDomainModelsResponseIndicatorResponse) UnsetVenue()`

UnsetVenue ensures that no value is present for Venue, not even an explicit nil
### GetSymbol

`func (o *PiMarketDataDomainModelsResponseIndicatorResponse) GetSymbol() string`

GetSymbol returns the Symbol field if non-nil, zero value otherwise.

### GetSymbolOk

`func (o *PiMarketDataDomainModelsResponseIndicatorResponse) GetSymbolOk() (*string, bool)`

GetSymbolOk returns a tuple with the Symbol field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetSymbol

`func (o *PiMarketDataDomainModelsResponseIndicatorResponse) SetSymbol(v string)`

SetSymbol sets Symbol field to given value.

### HasSymbol

`func (o *PiMarketDataDomainModelsResponseIndicatorResponse) HasSymbol() bool`

HasSymbol returns a boolean if a field has been set.

### SetSymbolNil

`func (o *PiMarketDataDomainModelsResponseIndicatorResponse) SetSymbolNil(b bool)`

 SetSymbolNil sets the value for Symbol to be an explicit nil

### UnsetSymbol
`func (o *PiMarketDataDomainModelsResponseIndicatorResponse) UnsetSymbol()`

UnsetSymbol ensures that no value is present for Symbol, not even an explicit nil
### GetCandleType

`func (o *PiMarketDataDomainModelsResponseIndicatorResponse) GetCandleType() string`

GetCandleType returns the CandleType field if non-nil, zero value otherwise.

### GetCandleTypeOk

`func (o *PiMarketDataDomainModelsResponseIndicatorResponse) GetCandleTypeOk() (*string, bool)`

GetCandleTypeOk returns a tuple with the CandleType field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetCandleType

`func (o *PiMarketDataDomainModelsResponseIndicatorResponse) SetCandleType(v string)`

SetCandleType sets CandleType field to given value.

### HasCandleType

`func (o *PiMarketDataDomainModelsResponseIndicatorResponse) HasCandleType() bool`

HasCandleType returns a boolean if a field has been set.

### SetCandleTypeNil

`func (o *PiMarketDataDomainModelsResponseIndicatorResponse) SetCandleTypeNil(b bool)`

 SetCandleTypeNil sets the value for CandleType to be an explicit nil

### UnsetCandleType
`func (o *PiMarketDataDomainModelsResponseIndicatorResponse) UnsetCandleType()`

UnsetCandleType ensures that no value is present for CandleType, not even an explicit nil
### GetCandles

`func (o *PiMarketDataDomainModelsResponseIndicatorResponse) GetCandles() [][]interface{}`

GetCandles returns the Candles field if non-nil, zero value otherwise.

### GetCandlesOk

`func (o *PiMarketDataDomainModelsResponseIndicatorResponse) GetCandlesOk() (*[][]interface{}, bool)`

GetCandlesOk returns a tuple with the Candles field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetCandles

`func (o *PiMarketDataDomainModelsResponseIndicatorResponse) SetCandles(v [][]interface{})`

SetCandles sets Candles field to given value.

### HasCandles

`func (o *PiMarketDataDomainModelsResponseIndicatorResponse) HasCandles() bool`

HasCandles returns a boolean if a field has been set.

### SetCandlesNil

`func (o *PiMarketDataDomainModelsResponseIndicatorResponse) SetCandlesNil(b bool)`

 SetCandlesNil sets the value for Candles to be an explicit nil

### UnsetCandles
`func (o *PiMarketDataDomainModelsResponseIndicatorResponse) UnsetCandles()`

UnsetCandles ensures that no value is present for Candles, not even an explicit nil
### GetIndicators

`func (o *PiMarketDataDomainModelsResponseIndicatorResponse) GetIndicators() PiMarketDataDomainModelsResponseTechnicalIndicatorsResponse`

GetIndicators returns the Indicators field if non-nil, zero value otherwise.

### GetIndicatorsOk

`func (o *PiMarketDataDomainModelsResponseIndicatorResponse) GetIndicatorsOk() (*PiMarketDataDomainModelsResponseTechnicalIndicatorsResponse, bool)`

GetIndicatorsOk returns a tuple with the Indicators field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetIndicators

`func (o *PiMarketDataDomainModelsResponseIndicatorResponse) SetIndicators(v PiMarketDataDomainModelsResponseTechnicalIndicatorsResponse)`

SetIndicators sets Indicators field to given value.

### HasIndicators

`func (o *PiMarketDataDomainModelsResponseIndicatorResponse) HasIndicators() bool`

HasIndicators returns a boolean if a field has been set.

### GetFirstCandleTime

`func (o *PiMarketDataDomainModelsResponseIndicatorResponse) GetFirstCandleTime() int32`

GetFirstCandleTime returns the FirstCandleTime field if non-nil, zero value otherwise.

### GetFirstCandleTimeOk

`func (o *PiMarketDataDomainModelsResponseIndicatorResponse) GetFirstCandleTimeOk() (*int32, bool)`

GetFirstCandleTimeOk returns a tuple with the FirstCandleTime field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetFirstCandleTime

`func (o *PiMarketDataDomainModelsResponseIndicatorResponse) SetFirstCandleTime(v int32)`

SetFirstCandleTime sets FirstCandleTime field to given value.

### HasFirstCandleTime

`func (o *PiMarketDataDomainModelsResponseIndicatorResponse) HasFirstCandleTime() bool`

HasFirstCandleTime returns a boolean if a field has been set.


[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


