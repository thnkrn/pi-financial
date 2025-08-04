# PiFundMarketDataAPIModelsResponsesAssetAllocationResponse

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**AsOfDate** | Pointer to **NullableTime** |  | [optional] 
**AssetClassAllocations** | Pointer to [**[]SystemStringSystemDoubleCategoricalRecord**](SystemStringSystemDoubleCategoricalRecord.md) |  | [optional] 
**RegionalAllocations** | Pointer to [**[]SystemStringSystemDoubleCategoricalRecord**](SystemStringSystemDoubleCategoricalRecord.md) |  | [optional] 
**SectorAllocations** | Pointer to [**[]SystemStringSystemDoubleCategoricalRecord**](SystemStringSystemDoubleCategoricalRecord.md) |  | [optional] 
**TopHoldings** | Pointer to [**[]SystemStringSystemDoubleCategoricalRecord**](SystemStringSystemDoubleCategoricalRecord.md) |  | [optional] 

## Methods

### NewPiFundMarketDataAPIModelsResponsesAssetAllocationResponse

`func NewPiFundMarketDataAPIModelsResponsesAssetAllocationResponse() *PiFundMarketDataAPIModelsResponsesAssetAllocationResponse`

NewPiFundMarketDataAPIModelsResponsesAssetAllocationResponse instantiates a new PiFundMarketDataAPIModelsResponsesAssetAllocationResponse object
This constructor will assign default values to properties that have it defined,
and makes sure properties required by API are set, but the set of arguments
will change when the set of required properties is changed

### NewPiFundMarketDataAPIModelsResponsesAssetAllocationResponseWithDefaults

`func NewPiFundMarketDataAPIModelsResponsesAssetAllocationResponseWithDefaults() *PiFundMarketDataAPIModelsResponsesAssetAllocationResponse`

NewPiFundMarketDataAPIModelsResponsesAssetAllocationResponseWithDefaults instantiates a new PiFundMarketDataAPIModelsResponsesAssetAllocationResponse object
This constructor will only assign default values to properties that have it defined,
but it doesn't guarantee that properties required by API are set

### GetAsOfDate

`func (o *PiFundMarketDataAPIModelsResponsesAssetAllocationResponse) GetAsOfDate() time.Time`

GetAsOfDate returns the AsOfDate field if non-nil, zero value otherwise.

### GetAsOfDateOk

`func (o *PiFundMarketDataAPIModelsResponsesAssetAllocationResponse) GetAsOfDateOk() (*time.Time, bool)`

GetAsOfDateOk returns a tuple with the AsOfDate field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetAsOfDate

`func (o *PiFundMarketDataAPIModelsResponsesAssetAllocationResponse) SetAsOfDate(v time.Time)`

SetAsOfDate sets AsOfDate field to given value.

### HasAsOfDate

`func (o *PiFundMarketDataAPIModelsResponsesAssetAllocationResponse) HasAsOfDate() bool`

HasAsOfDate returns a boolean if a field has been set.

### SetAsOfDateNil

`func (o *PiFundMarketDataAPIModelsResponsesAssetAllocationResponse) SetAsOfDateNil(b bool)`

 SetAsOfDateNil sets the value for AsOfDate to be an explicit nil

### UnsetAsOfDate
`func (o *PiFundMarketDataAPIModelsResponsesAssetAllocationResponse) UnsetAsOfDate()`

UnsetAsOfDate ensures that no value is present for AsOfDate, not even an explicit nil
### GetAssetClassAllocations

`func (o *PiFundMarketDataAPIModelsResponsesAssetAllocationResponse) GetAssetClassAllocations() []SystemStringSystemDoubleCategoricalRecord`

GetAssetClassAllocations returns the AssetClassAllocations field if non-nil, zero value otherwise.

### GetAssetClassAllocationsOk

`func (o *PiFundMarketDataAPIModelsResponsesAssetAllocationResponse) GetAssetClassAllocationsOk() (*[]SystemStringSystemDoubleCategoricalRecord, bool)`

GetAssetClassAllocationsOk returns a tuple with the AssetClassAllocations field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetAssetClassAllocations

`func (o *PiFundMarketDataAPIModelsResponsesAssetAllocationResponse) SetAssetClassAllocations(v []SystemStringSystemDoubleCategoricalRecord)`

SetAssetClassAllocations sets AssetClassAllocations field to given value.

### HasAssetClassAllocations

`func (o *PiFundMarketDataAPIModelsResponsesAssetAllocationResponse) HasAssetClassAllocations() bool`

HasAssetClassAllocations returns a boolean if a field has been set.

### SetAssetClassAllocationsNil

`func (o *PiFundMarketDataAPIModelsResponsesAssetAllocationResponse) SetAssetClassAllocationsNil(b bool)`

 SetAssetClassAllocationsNil sets the value for AssetClassAllocations to be an explicit nil

### UnsetAssetClassAllocations
`func (o *PiFundMarketDataAPIModelsResponsesAssetAllocationResponse) UnsetAssetClassAllocations()`

UnsetAssetClassAllocations ensures that no value is present for AssetClassAllocations, not even an explicit nil
### GetRegionalAllocations

`func (o *PiFundMarketDataAPIModelsResponsesAssetAllocationResponse) GetRegionalAllocations() []SystemStringSystemDoubleCategoricalRecord`

GetRegionalAllocations returns the RegionalAllocations field if non-nil, zero value otherwise.

### GetRegionalAllocationsOk

`func (o *PiFundMarketDataAPIModelsResponsesAssetAllocationResponse) GetRegionalAllocationsOk() (*[]SystemStringSystemDoubleCategoricalRecord, bool)`

GetRegionalAllocationsOk returns a tuple with the RegionalAllocations field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetRegionalAllocations

`func (o *PiFundMarketDataAPIModelsResponsesAssetAllocationResponse) SetRegionalAllocations(v []SystemStringSystemDoubleCategoricalRecord)`

SetRegionalAllocations sets RegionalAllocations field to given value.

### HasRegionalAllocations

`func (o *PiFundMarketDataAPIModelsResponsesAssetAllocationResponse) HasRegionalAllocations() bool`

HasRegionalAllocations returns a boolean if a field has been set.

### SetRegionalAllocationsNil

`func (o *PiFundMarketDataAPIModelsResponsesAssetAllocationResponse) SetRegionalAllocationsNil(b bool)`

 SetRegionalAllocationsNil sets the value for RegionalAllocations to be an explicit nil

### UnsetRegionalAllocations
`func (o *PiFundMarketDataAPIModelsResponsesAssetAllocationResponse) UnsetRegionalAllocations()`

UnsetRegionalAllocations ensures that no value is present for RegionalAllocations, not even an explicit nil
### GetSectorAllocations

`func (o *PiFundMarketDataAPIModelsResponsesAssetAllocationResponse) GetSectorAllocations() []SystemStringSystemDoubleCategoricalRecord`

GetSectorAllocations returns the SectorAllocations field if non-nil, zero value otherwise.

### GetSectorAllocationsOk

`func (o *PiFundMarketDataAPIModelsResponsesAssetAllocationResponse) GetSectorAllocationsOk() (*[]SystemStringSystemDoubleCategoricalRecord, bool)`

GetSectorAllocationsOk returns a tuple with the SectorAllocations field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetSectorAllocations

`func (o *PiFundMarketDataAPIModelsResponsesAssetAllocationResponse) SetSectorAllocations(v []SystemStringSystemDoubleCategoricalRecord)`

SetSectorAllocations sets SectorAllocations field to given value.

### HasSectorAllocations

`func (o *PiFundMarketDataAPIModelsResponsesAssetAllocationResponse) HasSectorAllocations() bool`

HasSectorAllocations returns a boolean if a field has been set.

### SetSectorAllocationsNil

`func (o *PiFundMarketDataAPIModelsResponsesAssetAllocationResponse) SetSectorAllocationsNil(b bool)`

 SetSectorAllocationsNil sets the value for SectorAllocations to be an explicit nil

### UnsetSectorAllocations
`func (o *PiFundMarketDataAPIModelsResponsesAssetAllocationResponse) UnsetSectorAllocations()`

UnsetSectorAllocations ensures that no value is present for SectorAllocations, not even an explicit nil
### GetTopHoldings

`func (o *PiFundMarketDataAPIModelsResponsesAssetAllocationResponse) GetTopHoldings() []SystemStringSystemDoubleCategoricalRecord`

GetTopHoldings returns the TopHoldings field if non-nil, zero value otherwise.

### GetTopHoldingsOk

`func (o *PiFundMarketDataAPIModelsResponsesAssetAllocationResponse) GetTopHoldingsOk() (*[]SystemStringSystemDoubleCategoricalRecord, bool)`

GetTopHoldingsOk returns a tuple with the TopHoldings field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetTopHoldings

`func (o *PiFundMarketDataAPIModelsResponsesAssetAllocationResponse) SetTopHoldings(v []SystemStringSystemDoubleCategoricalRecord)`

SetTopHoldings sets TopHoldings field to given value.

### HasTopHoldings

`func (o *PiFundMarketDataAPIModelsResponsesAssetAllocationResponse) HasTopHoldings() bool`

HasTopHoldings returns a boolean if a field has been set.

### SetTopHoldingsNil

`func (o *PiFundMarketDataAPIModelsResponsesAssetAllocationResponse) SetTopHoldingsNil(b bool)`

 SetTopHoldingsNil sets the value for TopHoldings to be an explicit nil

### UnsetTopHoldings
`func (o *PiFundMarketDataAPIModelsResponsesAssetAllocationResponse) UnsetTopHoldings()`

UnsetTopHoldings ensures that no value is present for TopHoldings, not even an explicit nil

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


