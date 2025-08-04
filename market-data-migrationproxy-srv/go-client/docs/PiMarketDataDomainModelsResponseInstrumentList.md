# PiMarketDataDomainModelsResponseInstrumentList

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**Order** | Pointer to **int32** |  | [optional] 
**InstrumentType** | Pointer to **NullableString** |  | [optional] 
**InstrumentCategory** | Pointer to **NullableString** |  | [optional] 
**Venue** | Pointer to **NullableString** |  | [optional] 
**Symbol** | Pointer to **NullableString** |  | [optional] 
**FriendlyName** | Pointer to **NullableString** |  | [optional] 
**Logo** | Pointer to **NullableString** |  | [optional] 
**Unit** | Pointer to **NullableString** |  | [optional] 
**Price** | Pointer to **NullableString** |  | [optional] 
**PriceChange** | Pointer to **NullableString** |  | [optional] 
**PriceChangeRatio** | Pointer to **NullableString** |  | [optional] 
**TotalValue** | Pointer to **NullableString** |  | [optional] 
**TotalVolume** | Pointer to **NullableString** |  | [optional] 

## Methods

### NewPiMarketDataDomainModelsResponseInstrumentList

`func NewPiMarketDataDomainModelsResponseInstrumentList() *PiMarketDataDomainModelsResponseInstrumentList`

NewPiMarketDataDomainModelsResponseInstrumentList instantiates a new PiMarketDataDomainModelsResponseInstrumentList object
This constructor will assign default values to properties that have it defined,
and makes sure properties required by API are set, but the set of arguments
will change when the set of required properties is changed

### NewPiMarketDataDomainModelsResponseInstrumentListWithDefaults

`func NewPiMarketDataDomainModelsResponseInstrumentListWithDefaults() *PiMarketDataDomainModelsResponseInstrumentList`

NewPiMarketDataDomainModelsResponseInstrumentListWithDefaults instantiates a new PiMarketDataDomainModelsResponseInstrumentList object
This constructor will only assign default values to properties that have it defined,
but it doesn't guarantee that properties required by API are set

### GetOrder

`func (o *PiMarketDataDomainModelsResponseInstrumentList) GetOrder() int32`

GetOrder returns the Order field if non-nil, zero value otherwise.

### GetOrderOk

`func (o *PiMarketDataDomainModelsResponseInstrumentList) GetOrderOk() (*int32, bool)`

GetOrderOk returns a tuple with the Order field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetOrder

`func (o *PiMarketDataDomainModelsResponseInstrumentList) SetOrder(v int32)`

SetOrder sets Order field to given value.

### HasOrder

`func (o *PiMarketDataDomainModelsResponseInstrumentList) HasOrder() bool`

HasOrder returns a boolean if a field has been set.

### GetInstrumentType

`func (o *PiMarketDataDomainModelsResponseInstrumentList) GetInstrumentType() string`

GetInstrumentType returns the InstrumentType field if non-nil, zero value otherwise.

### GetInstrumentTypeOk

`func (o *PiMarketDataDomainModelsResponseInstrumentList) GetInstrumentTypeOk() (*string, bool)`

GetInstrumentTypeOk returns a tuple with the InstrumentType field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetInstrumentType

`func (o *PiMarketDataDomainModelsResponseInstrumentList) SetInstrumentType(v string)`

SetInstrumentType sets InstrumentType field to given value.

### HasInstrumentType

`func (o *PiMarketDataDomainModelsResponseInstrumentList) HasInstrumentType() bool`

HasInstrumentType returns a boolean if a field has been set.

### SetInstrumentTypeNil

`func (o *PiMarketDataDomainModelsResponseInstrumentList) SetInstrumentTypeNil(b bool)`

 SetInstrumentTypeNil sets the value for InstrumentType to be an explicit nil

### UnsetInstrumentType
`func (o *PiMarketDataDomainModelsResponseInstrumentList) UnsetInstrumentType()`

UnsetInstrumentType ensures that no value is present for InstrumentType, not even an explicit nil
### GetInstrumentCategory

`func (o *PiMarketDataDomainModelsResponseInstrumentList) GetInstrumentCategory() string`

GetInstrumentCategory returns the InstrumentCategory field if non-nil, zero value otherwise.

### GetInstrumentCategoryOk

`func (o *PiMarketDataDomainModelsResponseInstrumentList) GetInstrumentCategoryOk() (*string, bool)`

GetInstrumentCategoryOk returns a tuple with the InstrumentCategory field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetInstrumentCategory

`func (o *PiMarketDataDomainModelsResponseInstrumentList) SetInstrumentCategory(v string)`

SetInstrumentCategory sets InstrumentCategory field to given value.

### HasInstrumentCategory

`func (o *PiMarketDataDomainModelsResponseInstrumentList) HasInstrumentCategory() bool`

HasInstrumentCategory returns a boolean if a field has been set.

### SetInstrumentCategoryNil

`func (o *PiMarketDataDomainModelsResponseInstrumentList) SetInstrumentCategoryNil(b bool)`

 SetInstrumentCategoryNil sets the value for InstrumentCategory to be an explicit nil

### UnsetInstrumentCategory
`func (o *PiMarketDataDomainModelsResponseInstrumentList) UnsetInstrumentCategory()`

UnsetInstrumentCategory ensures that no value is present for InstrumentCategory, not even an explicit nil
### GetVenue

`func (o *PiMarketDataDomainModelsResponseInstrumentList) GetVenue() string`

GetVenue returns the Venue field if non-nil, zero value otherwise.

### GetVenueOk

`func (o *PiMarketDataDomainModelsResponseInstrumentList) GetVenueOk() (*string, bool)`

GetVenueOk returns a tuple with the Venue field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetVenue

`func (o *PiMarketDataDomainModelsResponseInstrumentList) SetVenue(v string)`

SetVenue sets Venue field to given value.

### HasVenue

`func (o *PiMarketDataDomainModelsResponseInstrumentList) HasVenue() bool`

HasVenue returns a boolean if a field has been set.

### SetVenueNil

`func (o *PiMarketDataDomainModelsResponseInstrumentList) SetVenueNil(b bool)`

 SetVenueNil sets the value for Venue to be an explicit nil

### UnsetVenue
`func (o *PiMarketDataDomainModelsResponseInstrumentList) UnsetVenue()`

UnsetVenue ensures that no value is present for Venue, not even an explicit nil
### GetSymbol

`func (o *PiMarketDataDomainModelsResponseInstrumentList) GetSymbol() string`

GetSymbol returns the Symbol field if non-nil, zero value otherwise.

### GetSymbolOk

`func (o *PiMarketDataDomainModelsResponseInstrumentList) GetSymbolOk() (*string, bool)`

GetSymbolOk returns a tuple with the Symbol field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetSymbol

`func (o *PiMarketDataDomainModelsResponseInstrumentList) SetSymbol(v string)`

SetSymbol sets Symbol field to given value.

### HasSymbol

`func (o *PiMarketDataDomainModelsResponseInstrumentList) HasSymbol() bool`

HasSymbol returns a boolean if a field has been set.

### SetSymbolNil

`func (o *PiMarketDataDomainModelsResponseInstrumentList) SetSymbolNil(b bool)`

 SetSymbolNil sets the value for Symbol to be an explicit nil

### UnsetSymbol
`func (o *PiMarketDataDomainModelsResponseInstrumentList) UnsetSymbol()`

UnsetSymbol ensures that no value is present for Symbol, not even an explicit nil
### GetFriendlyName

`func (o *PiMarketDataDomainModelsResponseInstrumentList) GetFriendlyName() string`

GetFriendlyName returns the FriendlyName field if non-nil, zero value otherwise.

### GetFriendlyNameOk

`func (o *PiMarketDataDomainModelsResponseInstrumentList) GetFriendlyNameOk() (*string, bool)`

GetFriendlyNameOk returns a tuple with the FriendlyName field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetFriendlyName

`func (o *PiMarketDataDomainModelsResponseInstrumentList) SetFriendlyName(v string)`

SetFriendlyName sets FriendlyName field to given value.

### HasFriendlyName

`func (o *PiMarketDataDomainModelsResponseInstrumentList) HasFriendlyName() bool`

HasFriendlyName returns a boolean if a field has been set.

### SetFriendlyNameNil

`func (o *PiMarketDataDomainModelsResponseInstrumentList) SetFriendlyNameNil(b bool)`

 SetFriendlyNameNil sets the value for FriendlyName to be an explicit nil

### UnsetFriendlyName
`func (o *PiMarketDataDomainModelsResponseInstrumentList) UnsetFriendlyName()`

UnsetFriendlyName ensures that no value is present for FriendlyName, not even an explicit nil
### GetLogo

`func (o *PiMarketDataDomainModelsResponseInstrumentList) GetLogo() string`

GetLogo returns the Logo field if non-nil, zero value otherwise.

### GetLogoOk

`func (o *PiMarketDataDomainModelsResponseInstrumentList) GetLogoOk() (*string, bool)`

GetLogoOk returns a tuple with the Logo field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetLogo

`func (o *PiMarketDataDomainModelsResponseInstrumentList) SetLogo(v string)`

SetLogo sets Logo field to given value.

### HasLogo

`func (o *PiMarketDataDomainModelsResponseInstrumentList) HasLogo() bool`

HasLogo returns a boolean if a field has been set.

### SetLogoNil

`func (o *PiMarketDataDomainModelsResponseInstrumentList) SetLogoNil(b bool)`

 SetLogoNil sets the value for Logo to be an explicit nil

### UnsetLogo
`func (o *PiMarketDataDomainModelsResponseInstrumentList) UnsetLogo()`

UnsetLogo ensures that no value is present for Logo, not even an explicit nil
### GetUnit

`func (o *PiMarketDataDomainModelsResponseInstrumentList) GetUnit() string`

GetUnit returns the Unit field if non-nil, zero value otherwise.

### GetUnitOk

`func (o *PiMarketDataDomainModelsResponseInstrumentList) GetUnitOk() (*string, bool)`

GetUnitOk returns a tuple with the Unit field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetUnit

`func (o *PiMarketDataDomainModelsResponseInstrumentList) SetUnit(v string)`

SetUnit sets Unit field to given value.

### HasUnit

`func (o *PiMarketDataDomainModelsResponseInstrumentList) HasUnit() bool`

HasUnit returns a boolean if a field has been set.

### SetUnitNil

`func (o *PiMarketDataDomainModelsResponseInstrumentList) SetUnitNil(b bool)`

 SetUnitNil sets the value for Unit to be an explicit nil

### UnsetUnit
`func (o *PiMarketDataDomainModelsResponseInstrumentList) UnsetUnit()`

UnsetUnit ensures that no value is present for Unit, not even an explicit nil
### GetPrice

`func (o *PiMarketDataDomainModelsResponseInstrumentList) GetPrice() string`

GetPrice returns the Price field if non-nil, zero value otherwise.

### GetPriceOk

`func (o *PiMarketDataDomainModelsResponseInstrumentList) GetPriceOk() (*string, bool)`

GetPriceOk returns a tuple with the Price field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetPrice

`func (o *PiMarketDataDomainModelsResponseInstrumentList) SetPrice(v string)`

SetPrice sets Price field to given value.

### HasPrice

`func (o *PiMarketDataDomainModelsResponseInstrumentList) HasPrice() bool`

HasPrice returns a boolean if a field has been set.

### SetPriceNil

`func (o *PiMarketDataDomainModelsResponseInstrumentList) SetPriceNil(b bool)`

 SetPriceNil sets the value for Price to be an explicit nil

### UnsetPrice
`func (o *PiMarketDataDomainModelsResponseInstrumentList) UnsetPrice()`

UnsetPrice ensures that no value is present for Price, not even an explicit nil
### GetPriceChange

`func (o *PiMarketDataDomainModelsResponseInstrumentList) GetPriceChange() string`

GetPriceChange returns the PriceChange field if non-nil, zero value otherwise.

### GetPriceChangeOk

`func (o *PiMarketDataDomainModelsResponseInstrumentList) GetPriceChangeOk() (*string, bool)`

GetPriceChangeOk returns a tuple with the PriceChange field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetPriceChange

`func (o *PiMarketDataDomainModelsResponseInstrumentList) SetPriceChange(v string)`

SetPriceChange sets PriceChange field to given value.

### HasPriceChange

`func (o *PiMarketDataDomainModelsResponseInstrumentList) HasPriceChange() bool`

HasPriceChange returns a boolean if a field has been set.

### SetPriceChangeNil

`func (o *PiMarketDataDomainModelsResponseInstrumentList) SetPriceChangeNil(b bool)`

 SetPriceChangeNil sets the value for PriceChange to be an explicit nil

### UnsetPriceChange
`func (o *PiMarketDataDomainModelsResponseInstrumentList) UnsetPriceChange()`

UnsetPriceChange ensures that no value is present for PriceChange, not even an explicit nil
### GetPriceChangeRatio

`func (o *PiMarketDataDomainModelsResponseInstrumentList) GetPriceChangeRatio() string`

GetPriceChangeRatio returns the PriceChangeRatio field if non-nil, zero value otherwise.

### GetPriceChangeRatioOk

`func (o *PiMarketDataDomainModelsResponseInstrumentList) GetPriceChangeRatioOk() (*string, bool)`

GetPriceChangeRatioOk returns a tuple with the PriceChangeRatio field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetPriceChangeRatio

`func (o *PiMarketDataDomainModelsResponseInstrumentList) SetPriceChangeRatio(v string)`

SetPriceChangeRatio sets PriceChangeRatio field to given value.

### HasPriceChangeRatio

`func (o *PiMarketDataDomainModelsResponseInstrumentList) HasPriceChangeRatio() bool`

HasPriceChangeRatio returns a boolean if a field has been set.

### SetPriceChangeRatioNil

`func (o *PiMarketDataDomainModelsResponseInstrumentList) SetPriceChangeRatioNil(b bool)`

 SetPriceChangeRatioNil sets the value for PriceChangeRatio to be an explicit nil

### UnsetPriceChangeRatio
`func (o *PiMarketDataDomainModelsResponseInstrumentList) UnsetPriceChangeRatio()`

UnsetPriceChangeRatio ensures that no value is present for PriceChangeRatio, not even an explicit nil
### GetTotalValue

`func (o *PiMarketDataDomainModelsResponseInstrumentList) GetTotalValue() string`

GetTotalValue returns the TotalValue field if non-nil, zero value otherwise.

### GetTotalValueOk

`func (o *PiMarketDataDomainModelsResponseInstrumentList) GetTotalValueOk() (*string, bool)`

GetTotalValueOk returns a tuple with the TotalValue field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetTotalValue

`func (o *PiMarketDataDomainModelsResponseInstrumentList) SetTotalValue(v string)`

SetTotalValue sets TotalValue field to given value.

### HasTotalValue

`func (o *PiMarketDataDomainModelsResponseInstrumentList) HasTotalValue() bool`

HasTotalValue returns a boolean if a field has been set.

### SetTotalValueNil

`func (o *PiMarketDataDomainModelsResponseInstrumentList) SetTotalValueNil(b bool)`

 SetTotalValueNil sets the value for TotalValue to be an explicit nil

### UnsetTotalValue
`func (o *PiMarketDataDomainModelsResponseInstrumentList) UnsetTotalValue()`

UnsetTotalValue ensures that no value is present for TotalValue, not even an explicit nil
### GetTotalVolume

`func (o *PiMarketDataDomainModelsResponseInstrumentList) GetTotalVolume() string`

GetTotalVolume returns the TotalVolume field if non-nil, zero value otherwise.

### GetTotalVolumeOk

`func (o *PiMarketDataDomainModelsResponseInstrumentList) GetTotalVolumeOk() (*string, bool)`

GetTotalVolumeOk returns a tuple with the TotalVolume field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetTotalVolume

`func (o *PiMarketDataDomainModelsResponseInstrumentList) SetTotalVolume(v string)`

SetTotalVolume sets TotalVolume field to given value.

### HasTotalVolume

`func (o *PiMarketDataDomainModelsResponseInstrumentList) HasTotalVolume() bool`

HasTotalVolume returns a boolean if a field has been set.

### SetTotalVolumeNil

`func (o *PiMarketDataDomainModelsResponseInstrumentList) SetTotalVolumeNil(b bool)`

 SetTotalVolumeNil sets the value for TotalVolume to be an explicit nil

### UnsetTotalVolume
`func (o *PiMarketDataDomainModelsResponseInstrumentList) UnsetTotalVolume()`

UnsetTotalVolume ensures that no value is present for TotalVolume, not even an explicit nil

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


