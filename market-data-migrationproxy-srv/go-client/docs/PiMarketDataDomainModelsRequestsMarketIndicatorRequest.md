# PiMarketDataDomainModelsRequestsMarketIndicatorRequest

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**Symbol** | Pointer to **NullableString** |  | [optional] 
**Venue** | Pointer to **NullableString** |  | [optional] 
**CandleType** | Pointer to **NullableString** |  | [optional] 
**CompleteTradingDay** | Pointer to **bool** |  | [optional] 
**Limit** | Pointer to **NullableInt32** |  | [optional] 
**FromTimestamp** | Pointer to **int32** |  | [optional] 
**Indicators** | Pointer to [**PiMarketDataDomainModelsRequestsIndicators**](PiMarketDataDomainModelsRequestsIndicators.md) |  | [optional] 
**ToTimestamp** | Pointer to **int32** |  | [optional] 

## Methods

### NewPiMarketDataDomainModelsRequestsMarketIndicatorRequest

`func NewPiMarketDataDomainModelsRequestsMarketIndicatorRequest() *PiMarketDataDomainModelsRequestsMarketIndicatorRequest`

NewPiMarketDataDomainModelsRequestsMarketIndicatorRequest instantiates a new PiMarketDataDomainModelsRequestsMarketIndicatorRequest object
This constructor will assign default values to properties that have it defined,
and makes sure properties required by API are set, but the set of arguments
will change when the set of required properties is changed

### NewPiMarketDataDomainModelsRequestsMarketIndicatorRequestWithDefaults

`func NewPiMarketDataDomainModelsRequestsMarketIndicatorRequestWithDefaults() *PiMarketDataDomainModelsRequestsMarketIndicatorRequest`

NewPiMarketDataDomainModelsRequestsMarketIndicatorRequestWithDefaults instantiates a new PiMarketDataDomainModelsRequestsMarketIndicatorRequest object
This constructor will only assign default values to properties that have it defined,
but it doesn't guarantee that properties required by API are set

### GetSymbol

`func (o *PiMarketDataDomainModelsRequestsMarketIndicatorRequest) GetSymbol() string`

GetSymbol returns the Symbol field if non-nil, zero value otherwise.

### GetSymbolOk

`func (o *PiMarketDataDomainModelsRequestsMarketIndicatorRequest) GetSymbolOk() (*string, bool)`

GetSymbolOk returns a tuple with the Symbol field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetSymbol

`func (o *PiMarketDataDomainModelsRequestsMarketIndicatorRequest) SetSymbol(v string)`

SetSymbol sets Symbol field to given value.

### HasSymbol

`func (o *PiMarketDataDomainModelsRequestsMarketIndicatorRequest) HasSymbol() bool`

HasSymbol returns a boolean if a field has been set.

### SetSymbolNil

`func (o *PiMarketDataDomainModelsRequestsMarketIndicatorRequest) SetSymbolNil(b bool)`

 SetSymbolNil sets the value for Symbol to be an explicit nil

### UnsetSymbol
`func (o *PiMarketDataDomainModelsRequestsMarketIndicatorRequest) UnsetSymbol()`

UnsetSymbol ensures that no value is present for Symbol, not even an explicit nil
### GetVenue

`func (o *PiMarketDataDomainModelsRequestsMarketIndicatorRequest) GetVenue() string`

GetVenue returns the Venue field if non-nil, zero value otherwise.

### GetVenueOk

`func (o *PiMarketDataDomainModelsRequestsMarketIndicatorRequest) GetVenueOk() (*string, bool)`

GetVenueOk returns a tuple with the Venue field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetVenue

`func (o *PiMarketDataDomainModelsRequestsMarketIndicatorRequest) SetVenue(v string)`

SetVenue sets Venue field to given value.

### HasVenue

`func (o *PiMarketDataDomainModelsRequestsMarketIndicatorRequest) HasVenue() bool`

HasVenue returns a boolean if a field has been set.

### SetVenueNil

`func (o *PiMarketDataDomainModelsRequestsMarketIndicatorRequest) SetVenueNil(b bool)`

 SetVenueNil sets the value for Venue to be an explicit nil

### UnsetVenue
`func (o *PiMarketDataDomainModelsRequestsMarketIndicatorRequest) UnsetVenue()`

UnsetVenue ensures that no value is present for Venue, not even an explicit nil
### GetCandleType

`func (o *PiMarketDataDomainModelsRequestsMarketIndicatorRequest) GetCandleType() string`

GetCandleType returns the CandleType field if non-nil, zero value otherwise.

### GetCandleTypeOk

`func (o *PiMarketDataDomainModelsRequestsMarketIndicatorRequest) GetCandleTypeOk() (*string, bool)`

GetCandleTypeOk returns a tuple with the CandleType field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetCandleType

`func (o *PiMarketDataDomainModelsRequestsMarketIndicatorRequest) SetCandleType(v string)`

SetCandleType sets CandleType field to given value.

### HasCandleType

`func (o *PiMarketDataDomainModelsRequestsMarketIndicatorRequest) HasCandleType() bool`

HasCandleType returns a boolean if a field has been set.

### SetCandleTypeNil

`func (o *PiMarketDataDomainModelsRequestsMarketIndicatorRequest) SetCandleTypeNil(b bool)`

 SetCandleTypeNil sets the value for CandleType to be an explicit nil

### UnsetCandleType
`func (o *PiMarketDataDomainModelsRequestsMarketIndicatorRequest) UnsetCandleType()`

UnsetCandleType ensures that no value is present for CandleType, not even an explicit nil
### GetCompleteTradingDay

`func (o *PiMarketDataDomainModelsRequestsMarketIndicatorRequest) GetCompleteTradingDay() bool`

GetCompleteTradingDay returns the CompleteTradingDay field if non-nil, zero value otherwise.

### GetCompleteTradingDayOk

`func (o *PiMarketDataDomainModelsRequestsMarketIndicatorRequest) GetCompleteTradingDayOk() (*bool, bool)`

GetCompleteTradingDayOk returns a tuple with the CompleteTradingDay field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetCompleteTradingDay

`func (o *PiMarketDataDomainModelsRequestsMarketIndicatorRequest) SetCompleteTradingDay(v bool)`

SetCompleteTradingDay sets CompleteTradingDay field to given value.

### HasCompleteTradingDay

`func (o *PiMarketDataDomainModelsRequestsMarketIndicatorRequest) HasCompleteTradingDay() bool`

HasCompleteTradingDay returns a boolean if a field has been set.

### GetLimit

`func (o *PiMarketDataDomainModelsRequestsMarketIndicatorRequest) GetLimit() int32`

GetLimit returns the Limit field if non-nil, zero value otherwise.

### GetLimitOk

`func (o *PiMarketDataDomainModelsRequestsMarketIndicatorRequest) GetLimitOk() (*int32, bool)`

GetLimitOk returns a tuple with the Limit field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetLimit

`func (o *PiMarketDataDomainModelsRequestsMarketIndicatorRequest) SetLimit(v int32)`

SetLimit sets Limit field to given value.

### HasLimit

`func (o *PiMarketDataDomainModelsRequestsMarketIndicatorRequest) HasLimit() bool`

HasLimit returns a boolean if a field has been set.

### SetLimitNil

`func (o *PiMarketDataDomainModelsRequestsMarketIndicatorRequest) SetLimitNil(b bool)`

 SetLimitNil sets the value for Limit to be an explicit nil

### UnsetLimit
`func (o *PiMarketDataDomainModelsRequestsMarketIndicatorRequest) UnsetLimit()`

UnsetLimit ensures that no value is present for Limit, not even an explicit nil
### GetFromTimestamp

`func (o *PiMarketDataDomainModelsRequestsMarketIndicatorRequest) GetFromTimestamp() int32`

GetFromTimestamp returns the FromTimestamp field if non-nil, zero value otherwise.

### GetFromTimestampOk

`func (o *PiMarketDataDomainModelsRequestsMarketIndicatorRequest) GetFromTimestampOk() (*int32, bool)`

GetFromTimestampOk returns a tuple with the FromTimestamp field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetFromTimestamp

`func (o *PiMarketDataDomainModelsRequestsMarketIndicatorRequest) SetFromTimestamp(v int32)`

SetFromTimestamp sets FromTimestamp field to given value.

### HasFromTimestamp

`func (o *PiMarketDataDomainModelsRequestsMarketIndicatorRequest) HasFromTimestamp() bool`

HasFromTimestamp returns a boolean if a field has been set.

### GetIndicators

`func (o *PiMarketDataDomainModelsRequestsMarketIndicatorRequest) GetIndicators() PiMarketDataDomainModelsRequestsIndicators`

GetIndicators returns the Indicators field if non-nil, zero value otherwise.

### GetIndicatorsOk

`func (o *PiMarketDataDomainModelsRequestsMarketIndicatorRequest) GetIndicatorsOk() (*PiMarketDataDomainModelsRequestsIndicators, bool)`

GetIndicatorsOk returns a tuple with the Indicators field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetIndicators

`func (o *PiMarketDataDomainModelsRequestsMarketIndicatorRequest) SetIndicators(v PiMarketDataDomainModelsRequestsIndicators)`

SetIndicators sets Indicators field to given value.

### HasIndicators

`func (o *PiMarketDataDomainModelsRequestsMarketIndicatorRequest) HasIndicators() bool`

HasIndicators returns a boolean if a field has been set.

### GetToTimestamp

`func (o *PiMarketDataDomainModelsRequestsMarketIndicatorRequest) GetToTimestamp() int32`

GetToTimestamp returns the ToTimestamp field if non-nil, zero value otherwise.

### GetToTimestampOk

`func (o *PiMarketDataDomainModelsRequestsMarketIndicatorRequest) GetToTimestampOk() (*int32, bool)`

GetToTimestampOk returns a tuple with the ToTimestamp field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetToTimestamp

`func (o *PiMarketDataDomainModelsRequestsMarketIndicatorRequest) SetToTimestamp(v int32)`

SetToTimestamp sets ToTimestamp field to given value.

### HasToTimestamp

`func (o *PiMarketDataDomainModelsRequestsMarketIndicatorRequest) HasToTimestamp() bool`

HasToTimestamp returns a boolean if a field has been set.


[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


