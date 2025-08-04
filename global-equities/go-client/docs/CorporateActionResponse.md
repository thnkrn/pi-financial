# CorporateActionResponse

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**Id** | Pointer to **NullableString** |  | [optional] 
**Venue** | Pointer to **NullableString** |  | [optional] 
**Symbol** | Pointer to **NullableString** |  | [optional] 
**SymbolId** | Pointer to **NullableString** |  | [optional] [readonly] 
**OperationType** | Pointer to [**OperationType**](OperationType.md) |  | [optional] 
**AssetType** | Pointer to [**CorporateAssetType**](CorporateAssetType.md) |  | [optional] 
**Currency** | Pointer to [**Currency**](Currency.md) |  | [optional] 
**Value** | Pointer to **NullableFloat32** |  | [optional] 
**ValueUSD** | Pointer to **NullableFloat32** |  | [optional] 
**CreatedAt** | Pointer to **time.Time** |  | [optional] 

## Methods

### NewCorporateActionResponse

`func NewCorporateActionResponse() *CorporateActionResponse`

NewCorporateActionResponse instantiates a new CorporateActionResponse object
This constructor will assign default values to properties that have it defined,
and makes sure properties required by API are set, but the set of arguments
will change when the set of required properties is changed

### NewCorporateActionResponseWithDefaults

`func NewCorporateActionResponseWithDefaults() *CorporateActionResponse`

NewCorporateActionResponseWithDefaults instantiates a new CorporateActionResponse object
This constructor will only assign default values to properties that have it defined,
but it doesn't guarantee that properties required by API are set

### GetId

`func (o *CorporateActionResponse) GetId() string`

GetId returns the Id field if non-nil, zero value otherwise.

### GetIdOk

`func (o *CorporateActionResponse) GetIdOk() (*string, bool)`

GetIdOk returns a tuple with the Id field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetId

`func (o *CorporateActionResponse) SetId(v string)`

SetId sets Id field to given value.

### HasId

`func (o *CorporateActionResponse) HasId() bool`

HasId returns a boolean if a field has been set.

### SetIdNil

`func (o *CorporateActionResponse) SetIdNil(b bool)`

 SetIdNil sets the value for Id to be an explicit nil

### UnsetId
`func (o *CorporateActionResponse) UnsetId()`

UnsetId ensures that no value is present for Id, not even an explicit nil
### GetVenue

`func (o *CorporateActionResponse) GetVenue() string`

GetVenue returns the Venue field if non-nil, zero value otherwise.

### GetVenueOk

`func (o *CorporateActionResponse) GetVenueOk() (*string, bool)`

GetVenueOk returns a tuple with the Venue field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetVenue

`func (o *CorporateActionResponse) SetVenue(v string)`

SetVenue sets Venue field to given value.

### HasVenue

`func (o *CorporateActionResponse) HasVenue() bool`

HasVenue returns a boolean if a field has been set.

### SetVenueNil

`func (o *CorporateActionResponse) SetVenueNil(b bool)`

 SetVenueNil sets the value for Venue to be an explicit nil

### UnsetVenue
`func (o *CorporateActionResponse) UnsetVenue()`

UnsetVenue ensures that no value is present for Venue, not even an explicit nil
### GetSymbol

`func (o *CorporateActionResponse) GetSymbol() string`

GetSymbol returns the Symbol field if non-nil, zero value otherwise.

### GetSymbolOk

`func (o *CorporateActionResponse) GetSymbolOk() (*string, bool)`

GetSymbolOk returns a tuple with the Symbol field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetSymbol

`func (o *CorporateActionResponse) SetSymbol(v string)`

SetSymbol sets Symbol field to given value.

### HasSymbol

`func (o *CorporateActionResponse) HasSymbol() bool`

HasSymbol returns a boolean if a field has been set.

### SetSymbolNil

`func (o *CorporateActionResponse) SetSymbolNil(b bool)`

 SetSymbolNil sets the value for Symbol to be an explicit nil

### UnsetSymbol
`func (o *CorporateActionResponse) UnsetSymbol()`

UnsetSymbol ensures that no value is present for Symbol, not even an explicit nil
### GetSymbolId

`func (o *CorporateActionResponse) GetSymbolId() string`

GetSymbolId returns the SymbolId field if non-nil, zero value otherwise.

### GetSymbolIdOk

`func (o *CorporateActionResponse) GetSymbolIdOk() (*string, bool)`

GetSymbolIdOk returns a tuple with the SymbolId field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetSymbolId

`func (o *CorporateActionResponse) SetSymbolId(v string)`

SetSymbolId sets SymbolId field to given value.

### HasSymbolId

`func (o *CorporateActionResponse) HasSymbolId() bool`

HasSymbolId returns a boolean if a field has been set.

### SetSymbolIdNil

`func (o *CorporateActionResponse) SetSymbolIdNil(b bool)`

 SetSymbolIdNil sets the value for SymbolId to be an explicit nil

### UnsetSymbolId
`func (o *CorporateActionResponse) UnsetSymbolId()`

UnsetSymbolId ensures that no value is present for SymbolId, not even an explicit nil
### GetOperationType

`func (o *CorporateActionResponse) GetOperationType() OperationType`

GetOperationType returns the OperationType field if non-nil, zero value otherwise.

### GetOperationTypeOk

`func (o *CorporateActionResponse) GetOperationTypeOk() (*OperationType, bool)`

GetOperationTypeOk returns a tuple with the OperationType field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetOperationType

`func (o *CorporateActionResponse) SetOperationType(v OperationType)`

SetOperationType sets OperationType field to given value.

### HasOperationType

`func (o *CorporateActionResponse) HasOperationType() bool`

HasOperationType returns a boolean if a field has been set.

### GetAssetType

`func (o *CorporateActionResponse) GetAssetType() CorporateAssetType`

GetAssetType returns the AssetType field if non-nil, zero value otherwise.

### GetAssetTypeOk

`func (o *CorporateActionResponse) GetAssetTypeOk() (*CorporateAssetType, bool)`

GetAssetTypeOk returns a tuple with the AssetType field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetAssetType

`func (o *CorporateActionResponse) SetAssetType(v CorporateAssetType)`

SetAssetType sets AssetType field to given value.

### HasAssetType

`func (o *CorporateActionResponse) HasAssetType() bool`

HasAssetType returns a boolean if a field has been set.

### GetCurrency

`func (o *CorporateActionResponse) GetCurrency() Currency`

GetCurrency returns the Currency field if non-nil, zero value otherwise.

### GetCurrencyOk

`func (o *CorporateActionResponse) GetCurrencyOk() (*Currency, bool)`

GetCurrencyOk returns a tuple with the Currency field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetCurrency

`func (o *CorporateActionResponse) SetCurrency(v Currency)`

SetCurrency sets Currency field to given value.

### HasCurrency

`func (o *CorporateActionResponse) HasCurrency() bool`

HasCurrency returns a boolean if a field has been set.

### GetValue

`func (o *CorporateActionResponse) GetValue() float32`

GetValue returns the Value field if non-nil, zero value otherwise.

### GetValueOk

`func (o *CorporateActionResponse) GetValueOk() (*float32, bool)`

GetValueOk returns a tuple with the Value field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetValue

`func (o *CorporateActionResponse) SetValue(v float32)`

SetValue sets Value field to given value.

### HasValue

`func (o *CorporateActionResponse) HasValue() bool`

HasValue returns a boolean if a field has been set.

### SetValueNil

`func (o *CorporateActionResponse) SetValueNil(b bool)`

 SetValueNil sets the value for Value to be an explicit nil

### UnsetValue
`func (o *CorporateActionResponse) UnsetValue()`

UnsetValue ensures that no value is present for Value, not even an explicit nil
### GetValueUSD

`func (o *CorporateActionResponse) GetValueUSD() float32`

GetValueUSD returns the ValueUSD field if non-nil, zero value otherwise.

### GetValueUSDOk

`func (o *CorporateActionResponse) GetValueUSDOk() (*float32, bool)`

GetValueUSDOk returns a tuple with the ValueUSD field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetValueUSD

`func (o *CorporateActionResponse) SetValueUSD(v float32)`

SetValueUSD sets ValueUSD field to given value.

### HasValueUSD

`func (o *CorporateActionResponse) HasValueUSD() bool`

HasValueUSD returns a boolean if a field has been set.

### SetValueUSDNil

`func (o *CorporateActionResponse) SetValueUSDNil(b bool)`

 SetValueUSDNil sets the value for ValueUSD to be an explicit nil

### UnsetValueUSD
`func (o *CorporateActionResponse) UnsetValueUSD()`

UnsetValueUSD ensures that no value is present for ValueUSD, not even an explicit nil
### GetCreatedAt

`func (o *CorporateActionResponse) GetCreatedAt() time.Time`

GetCreatedAt returns the CreatedAt field if non-nil, zero value otherwise.

### GetCreatedAtOk

`func (o *CorporateActionResponse) GetCreatedAtOk() (*time.Time, bool)`

GetCreatedAtOk returns a tuple with the CreatedAt field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetCreatedAt

`func (o *CorporateActionResponse) SetCreatedAt(v time.Time)`

SetCreatedAt sets CreatedAt field to given value.

### HasCreatedAt

`func (o *CorporateActionResponse) HasCreatedAt() bool`

HasCreatedAt returns a boolean if a field has been set.


[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


