# PiMarketDataDomainModelsResponseBody

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**FilterCategory** | Pointer to **NullableString** |  | [optional] 
**FilterType** | Pointer to **NullableString** |  | [optional] 
**SupportSecondaryFilter** | Pointer to **bool** |  | [optional] 
**Filters** | Pointer to [**[]PiMarketDataDomainModelsResponseFilter**](PiMarketDataDomainModelsResponseFilter.md) |  | [optional] 
**Order** | Pointer to **int32** |  | [optional] 

## Methods

### NewPiMarketDataDomainModelsResponseBody

`func NewPiMarketDataDomainModelsResponseBody() *PiMarketDataDomainModelsResponseBody`

NewPiMarketDataDomainModelsResponseBody instantiates a new PiMarketDataDomainModelsResponseBody object
This constructor will assign default values to properties that have it defined,
and makes sure properties required by API are set, but the set of arguments
will change when the set of required properties is changed

### NewPiMarketDataDomainModelsResponseBodyWithDefaults

`func NewPiMarketDataDomainModelsResponseBodyWithDefaults() *PiMarketDataDomainModelsResponseBody`

NewPiMarketDataDomainModelsResponseBodyWithDefaults instantiates a new PiMarketDataDomainModelsResponseBody object
This constructor will only assign default values to properties that have it defined,
but it doesn't guarantee that properties required by API are set

### GetFilterCategory

`func (o *PiMarketDataDomainModelsResponseBody) GetFilterCategory() string`

GetFilterCategory returns the FilterCategory field if non-nil, zero value otherwise.

### GetFilterCategoryOk

`func (o *PiMarketDataDomainModelsResponseBody) GetFilterCategoryOk() (*string, bool)`

GetFilterCategoryOk returns a tuple with the FilterCategory field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetFilterCategory

`func (o *PiMarketDataDomainModelsResponseBody) SetFilterCategory(v string)`

SetFilterCategory sets FilterCategory field to given value.

### HasFilterCategory

`func (o *PiMarketDataDomainModelsResponseBody) HasFilterCategory() bool`

HasFilterCategory returns a boolean if a field has been set.

### SetFilterCategoryNil

`func (o *PiMarketDataDomainModelsResponseBody) SetFilterCategoryNil(b bool)`

 SetFilterCategoryNil sets the value for FilterCategory to be an explicit nil

### UnsetFilterCategory
`func (o *PiMarketDataDomainModelsResponseBody) UnsetFilterCategory()`

UnsetFilterCategory ensures that no value is present for FilterCategory, not even an explicit nil
### GetFilterType

`func (o *PiMarketDataDomainModelsResponseBody) GetFilterType() string`

GetFilterType returns the FilterType field if non-nil, zero value otherwise.

### GetFilterTypeOk

`func (o *PiMarketDataDomainModelsResponseBody) GetFilterTypeOk() (*string, bool)`

GetFilterTypeOk returns a tuple with the FilterType field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetFilterType

`func (o *PiMarketDataDomainModelsResponseBody) SetFilterType(v string)`

SetFilterType sets FilterType field to given value.

### HasFilterType

`func (o *PiMarketDataDomainModelsResponseBody) HasFilterType() bool`

HasFilterType returns a boolean if a field has been set.

### SetFilterTypeNil

`func (o *PiMarketDataDomainModelsResponseBody) SetFilterTypeNil(b bool)`

 SetFilterTypeNil sets the value for FilterType to be an explicit nil

### UnsetFilterType
`func (o *PiMarketDataDomainModelsResponseBody) UnsetFilterType()`

UnsetFilterType ensures that no value is present for FilterType, not even an explicit nil
### GetSupportSecondaryFilter

`func (o *PiMarketDataDomainModelsResponseBody) GetSupportSecondaryFilter() bool`

GetSupportSecondaryFilter returns the SupportSecondaryFilter field if non-nil, zero value otherwise.

### GetSupportSecondaryFilterOk

`func (o *PiMarketDataDomainModelsResponseBody) GetSupportSecondaryFilterOk() (*bool, bool)`

GetSupportSecondaryFilterOk returns a tuple with the SupportSecondaryFilter field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetSupportSecondaryFilter

`func (o *PiMarketDataDomainModelsResponseBody) SetSupportSecondaryFilter(v bool)`

SetSupportSecondaryFilter sets SupportSecondaryFilter field to given value.

### HasSupportSecondaryFilter

`func (o *PiMarketDataDomainModelsResponseBody) HasSupportSecondaryFilter() bool`

HasSupportSecondaryFilter returns a boolean if a field has been set.

### GetFilters

`func (o *PiMarketDataDomainModelsResponseBody) GetFilters() []PiMarketDataDomainModelsResponseFilter`

GetFilters returns the Filters field if non-nil, zero value otherwise.

### GetFiltersOk

`func (o *PiMarketDataDomainModelsResponseBody) GetFiltersOk() (*[]PiMarketDataDomainModelsResponseFilter, bool)`

GetFiltersOk returns a tuple with the Filters field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetFilters

`func (o *PiMarketDataDomainModelsResponseBody) SetFilters(v []PiMarketDataDomainModelsResponseFilter)`

SetFilters sets Filters field to given value.

### HasFilters

`func (o *PiMarketDataDomainModelsResponseBody) HasFilters() bool`

HasFilters returns a boolean if a field has been set.

### SetFiltersNil

`func (o *PiMarketDataDomainModelsResponseBody) SetFiltersNil(b bool)`

 SetFiltersNil sets the value for Filters to be an explicit nil

### UnsetFilters
`func (o *PiMarketDataDomainModelsResponseBody) UnsetFilters()`

UnsetFilters ensures that no value is present for Filters, not even an explicit nil
### GetOrder

`func (o *PiMarketDataDomainModelsResponseBody) GetOrder() int32`

GetOrder returns the Order field if non-nil, zero value otherwise.

### GetOrderOk

`func (o *PiMarketDataDomainModelsResponseBody) GetOrderOk() (*int32, bool)`

GetOrderOk returns a tuple with the Order field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetOrder

`func (o *PiMarketDataDomainModelsResponseBody) SetOrder(v int32)`

SetOrder sets Order field to given value.

### HasOrder

`func (o *PiMarketDataDomainModelsResponseBody) HasOrder() bool`

HasOrder returns a boolean if a field has been set.


[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


