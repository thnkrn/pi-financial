# PiSetServiceAPIModelsSetAccountAssetsResponse

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**Symbol** | **NullableString** |  | 
**MarketPrice** | **float64** |  | 
**Nvdr** | **bool** |  | 
**AssetType** | **NullableString** |  | 
**IsNew** | **bool** |  | 
**CaFlag** | **bool** |  | 
**AverageCostPrice** | **float64** |  | 
**CostValue** | **float64** |  | 
**MarketValue** | **float64** |  | 
**Upnl** | **float64** |  | 
**UpnlPercentage** | **float64** |  | 
**Side** | **NullableString** |  | 
**AvailableVolume** | **float64** |  | 
**SellableVolume** | **float64** |  | 
**CaEventList** | [**[]PiSetServiceAPIModelsCaEvent**](PiSetServiceAPIModelsCaEvent.md) |  | 
**LendingVolume** | Pointer to **NullableFloat64** |  | [optional] 
**RealizedPnl** | Pointer to **NullableFloat64** |  | [optional] 
**StockType** | Pointer to **NullableString** |  | [optional] 
**Logo** | Pointer to **NullableString** |  | [optional] 
**FriendlyName** | Pointer to **NullableString** |  | [optional] 
**InstrumentCategory** | Pointer to **NullableString** |  | [optional] 

## Methods

### NewPiSetServiceAPIModelsSetAccountAssetsResponse

`func NewPiSetServiceAPIModelsSetAccountAssetsResponse(symbol NullableString, marketPrice float64, nvdr bool, assetType NullableString, isNew bool, caFlag bool, averageCostPrice float64, costValue float64, marketValue float64, upnl float64, upnlPercentage float64, side NullableString, availableVolume float64, sellableVolume float64, caEventList []PiSetServiceAPIModelsCaEvent, ) *PiSetServiceAPIModelsSetAccountAssetsResponse`

NewPiSetServiceAPIModelsSetAccountAssetsResponse instantiates a new PiSetServiceAPIModelsSetAccountAssetsResponse object
This constructor will assign default values to properties that have it defined,
and makes sure properties required by API are set, but the set of arguments
will change when the set of required properties is changed

### NewPiSetServiceAPIModelsSetAccountAssetsResponseWithDefaults

`func NewPiSetServiceAPIModelsSetAccountAssetsResponseWithDefaults() *PiSetServiceAPIModelsSetAccountAssetsResponse`

NewPiSetServiceAPIModelsSetAccountAssetsResponseWithDefaults instantiates a new PiSetServiceAPIModelsSetAccountAssetsResponse object
This constructor will only assign default values to properties that have it defined,
but it doesn't guarantee that properties required by API are set

### GetSymbol

`func (o *PiSetServiceAPIModelsSetAccountAssetsResponse) GetSymbol() string`

GetSymbol returns the Symbol field if non-nil, zero value otherwise.

### GetSymbolOk

`func (o *PiSetServiceAPIModelsSetAccountAssetsResponse) GetSymbolOk() (*string, bool)`

GetSymbolOk returns a tuple with the Symbol field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetSymbol

`func (o *PiSetServiceAPIModelsSetAccountAssetsResponse) SetSymbol(v string)`

SetSymbol sets Symbol field to given value.


### SetSymbolNil

`func (o *PiSetServiceAPIModelsSetAccountAssetsResponse) SetSymbolNil(b bool)`

 SetSymbolNil sets the value for Symbol to be an explicit nil

### UnsetSymbol
`func (o *PiSetServiceAPIModelsSetAccountAssetsResponse) UnsetSymbol()`

UnsetSymbol ensures that no value is present for Symbol, not even an explicit nil
### GetMarketPrice

`func (o *PiSetServiceAPIModelsSetAccountAssetsResponse) GetMarketPrice() float64`

GetMarketPrice returns the MarketPrice field if non-nil, zero value otherwise.

### GetMarketPriceOk

`func (o *PiSetServiceAPIModelsSetAccountAssetsResponse) GetMarketPriceOk() (*float64, bool)`

GetMarketPriceOk returns a tuple with the MarketPrice field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetMarketPrice

`func (o *PiSetServiceAPIModelsSetAccountAssetsResponse) SetMarketPrice(v float64)`

SetMarketPrice sets MarketPrice field to given value.


### GetNvdr

`func (o *PiSetServiceAPIModelsSetAccountAssetsResponse) GetNvdr() bool`

GetNvdr returns the Nvdr field if non-nil, zero value otherwise.

### GetNvdrOk

`func (o *PiSetServiceAPIModelsSetAccountAssetsResponse) GetNvdrOk() (*bool, bool)`

GetNvdrOk returns a tuple with the Nvdr field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetNvdr

`func (o *PiSetServiceAPIModelsSetAccountAssetsResponse) SetNvdr(v bool)`

SetNvdr sets Nvdr field to given value.


### GetAssetType

`func (o *PiSetServiceAPIModelsSetAccountAssetsResponse) GetAssetType() string`

GetAssetType returns the AssetType field if non-nil, zero value otherwise.

### GetAssetTypeOk

`func (o *PiSetServiceAPIModelsSetAccountAssetsResponse) GetAssetTypeOk() (*string, bool)`

GetAssetTypeOk returns a tuple with the AssetType field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetAssetType

`func (o *PiSetServiceAPIModelsSetAccountAssetsResponse) SetAssetType(v string)`

SetAssetType sets AssetType field to given value.


### SetAssetTypeNil

`func (o *PiSetServiceAPIModelsSetAccountAssetsResponse) SetAssetTypeNil(b bool)`

 SetAssetTypeNil sets the value for AssetType to be an explicit nil

### UnsetAssetType
`func (o *PiSetServiceAPIModelsSetAccountAssetsResponse) UnsetAssetType()`

UnsetAssetType ensures that no value is present for AssetType, not even an explicit nil
### GetIsNew

`func (o *PiSetServiceAPIModelsSetAccountAssetsResponse) GetIsNew() bool`

GetIsNew returns the IsNew field if non-nil, zero value otherwise.

### GetIsNewOk

`func (o *PiSetServiceAPIModelsSetAccountAssetsResponse) GetIsNewOk() (*bool, bool)`

GetIsNewOk returns a tuple with the IsNew field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetIsNew

`func (o *PiSetServiceAPIModelsSetAccountAssetsResponse) SetIsNew(v bool)`

SetIsNew sets IsNew field to given value.


### GetCaFlag

`func (o *PiSetServiceAPIModelsSetAccountAssetsResponse) GetCaFlag() bool`

GetCaFlag returns the CaFlag field if non-nil, zero value otherwise.

### GetCaFlagOk

`func (o *PiSetServiceAPIModelsSetAccountAssetsResponse) GetCaFlagOk() (*bool, bool)`

GetCaFlagOk returns a tuple with the CaFlag field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetCaFlag

`func (o *PiSetServiceAPIModelsSetAccountAssetsResponse) SetCaFlag(v bool)`

SetCaFlag sets CaFlag field to given value.


### GetAverageCostPrice

`func (o *PiSetServiceAPIModelsSetAccountAssetsResponse) GetAverageCostPrice() float64`

GetAverageCostPrice returns the AverageCostPrice field if non-nil, zero value otherwise.

### GetAverageCostPriceOk

`func (o *PiSetServiceAPIModelsSetAccountAssetsResponse) GetAverageCostPriceOk() (*float64, bool)`

GetAverageCostPriceOk returns a tuple with the AverageCostPrice field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetAverageCostPrice

`func (o *PiSetServiceAPIModelsSetAccountAssetsResponse) SetAverageCostPrice(v float64)`

SetAverageCostPrice sets AverageCostPrice field to given value.


### GetCostValue

`func (o *PiSetServiceAPIModelsSetAccountAssetsResponse) GetCostValue() float64`

GetCostValue returns the CostValue field if non-nil, zero value otherwise.

### GetCostValueOk

`func (o *PiSetServiceAPIModelsSetAccountAssetsResponse) GetCostValueOk() (*float64, bool)`

GetCostValueOk returns a tuple with the CostValue field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetCostValue

`func (o *PiSetServiceAPIModelsSetAccountAssetsResponse) SetCostValue(v float64)`

SetCostValue sets CostValue field to given value.


### GetMarketValue

`func (o *PiSetServiceAPIModelsSetAccountAssetsResponse) GetMarketValue() float64`

GetMarketValue returns the MarketValue field if non-nil, zero value otherwise.

### GetMarketValueOk

`func (o *PiSetServiceAPIModelsSetAccountAssetsResponse) GetMarketValueOk() (*float64, bool)`

GetMarketValueOk returns a tuple with the MarketValue field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetMarketValue

`func (o *PiSetServiceAPIModelsSetAccountAssetsResponse) SetMarketValue(v float64)`

SetMarketValue sets MarketValue field to given value.


### GetUpnl

`func (o *PiSetServiceAPIModelsSetAccountAssetsResponse) GetUpnl() float64`

GetUpnl returns the Upnl field if non-nil, zero value otherwise.

### GetUpnlOk

`func (o *PiSetServiceAPIModelsSetAccountAssetsResponse) GetUpnlOk() (*float64, bool)`

GetUpnlOk returns a tuple with the Upnl field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetUpnl

`func (o *PiSetServiceAPIModelsSetAccountAssetsResponse) SetUpnl(v float64)`

SetUpnl sets Upnl field to given value.


### GetUpnlPercentage

`func (o *PiSetServiceAPIModelsSetAccountAssetsResponse) GetUpnlPercentage() float64`

GetUpnlPercentage returns the UpnlPercentage field if non-nil, zero value otherwise.

### GetUpnlPercentageOk

`func (o *PiSetServiceAPIModelsSetAccountAssetsResponse) GetUpnlPercentageOk() (*float64, bool)`

GetUpnlPercentageOk returns a tuple with the UpnlPercentage field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetUpnlPercentage

`func (o *PiSetServiceAPIModelsSetAccountAssetsResponse) SetUpnlPercentage(v float64)`

SetUpnlPercentage sets UpnlPercentage field to given value.


### GetSide

`func (o *PiSetServiceAPIModelsSetAccountAssetsResponse) GetSide() string`

GetSide returns the Side field if non-nil, zero value otherwise.

### GetSideOk

`func (o *PiSetServiceAPIModelsSetAccountAssetsResponse) GetSideOk() (*string, bool)`

GetSideOk returns a tuple with the Side field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetSide

`func (o *PiSetServiceAPIModelsSetAccountAssetsResponse) SetSide(v string)`

SetSide sets Side field to given value.


### SetSideNil

`func (o *PiSetServiceAPIModelsSetAccountAssetsResponse) SetSideNil(b bool)`

 SetSideNil sets the value for Side to be an explicit nil

### UnsetSide
`func (o *PiSetServiceAPIModelsSetAccountAssetsResponse) UnsetSide()`

UnsetSide ensures that no value is present for Side, not even an explicit nil
### GetAvailableVolume

`func (o *PiSetServiceAPIModelsSetAccountAssetsResponse) GetAvailableVolume() float64`

GetAvailableVolume returns the AvailableVolume field if non-nil, zero value otherwise.

### GetAvailableVolumeOk

`func (o *PiSetServiceAPIModelsSetAccountAssetsResponse) GetAvailableVolumeOk() (*float64, bool)`

GetAvailableVolumeOk returns a tuple with the AvailableVolume field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetAvailableVolume

`func (o *PiSetServiceAPIModelsSetAccountAssetsResponse) SetAvailableVolume(v float64)`

SetAvailableVolume sets AvailableVolume field to given value.


### GetSellableVolume

`func (o *PiSetServiceAPIModelsSetAccountAssetsResponse) GetSellableVolume() float64`

GetSellableVolume returns the SellableVolume field if non-nil, zero value otherwise.

### GetSellableVolumeOk

`func (o *PiSetServiceAPIModelsSetAccountAssetsResponse) GetSellableVolumeOk() (*float64, bool)`

GetSellableVolumeOk returns a tuple with the SellableVolume field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetSellableVolume

`func (o *PiSetServiceAPIModelsSetAccountAssetsResponse) SetSellableVolume(v float64)`

SetSellableVolume sets SellableVolume field to given value.


### GetCaEventList

`func (o *PiSetServiceAPIModelsSetAccountAssetsResponse) GetCaEventList() []PiSetServiceAPIModelsCaEvent`

GetCaEventList returns the CaEventList field if non-nil, zero value otherwise.

### GetCaEventListOk

`func (o *PiSetServiceAPIModelsSetAccountAssetsResponse) GetCaEventListOk() (*[]PiSetServiceAPIModelsCaEvent, bool)`

GetCaEventListOk returns a tuple with the CaEventList field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetCaEventList

`func (o *PiSetServiceAPIModelsSetAccountAssetsResponse) SetCaEventList(v []PiSetServiceAPIModelsCaEvent)`

SetCaEventList sets CaEventList field to given value.


### SetCaEventListNil

`func (o *PiSetServiceAPIModelsSetAccountAssetsResponse) SetCaEventListNil(b bool)`

 SetCaEventListNil sets the value for CaEventList to be an explicit nil

### UnsetCaEventList
`func (o *PiSetServiceAPIModelsSetAccountAssetsResponse) UnsetCaEventList()`

UnsetCaEventList ensures that no value is present for CaEventList, not even an explicit nil
### GetLendingVolume

`func (o *PiSetServiceAPIModelsSetAccountAssetsResponse) GetLendingVolume() float64`

GetLendingVolume returns the LendingVolume field if non-nil, zero value otherwise.

### GetLendingVolumeOk

`func (o *PiSetServiceAPIModelsSetAccountAssetsResponse) GetLendingVolumeOk() (*float64, bool)`

GetLendingVolumeOk returns a tuple with the LendingVolume field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetLendingVolume

`func (o *PiSetServiceAPIModelsSetAccountAssetsResponse) SetLendingVolume(v float64)`

SetLendingVolume sets LendingVolume field to given value.

### HasLendingVolume

`func (o *PiSetServiceAPIModelsSetAccountAssetsResponse) HasLendingVolume() bool`

HasLendingVolume returns a boolean if a field has been set.

### SetLendingVolumeNil

`func (o *PiSetServiceAPIModelsSetAccountAssetsResponse) SetLendingVolumeNil(b bool)`

 SetLendingVolumeNil sets the value for LendingVolume to be an explicit nil

### UnsetLendingVolume
`func (o *PiSetServiceAPIModelsSetAccountAssetsResponse) UnsetLendingVolume()`

UnsetLendingVolume ensures that no value is present for LendingVolume, not even an explicit nil
### GetRealizedPnl

`func (o *PiSetServiceAPIModelsSetAccountAssetsResponse) GetRealizedPnl() float64`

GetRealizedPnl returns the RealizedPnl field if non-nil, zero value otherwise.

### GetRealizedPnlOk

`func (o *PiSetServiceAPIModelsSetAccountAssetsResponse) GetRealizedPnlOk() (*float64, bool)`

GetRealizedPnlOk returns a tuple with the RealizedPnl field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetRealizedPnl

`func (o *PiSetServiceAPIModelsSetAccountAssetsResponse) SetRealizedPnl(v float64)`

SetRealizedPnl sets RealizedPnl field to given value.

### HasRealizedPnl

`func (o *PiSetServiceAPIModelsSetAccountAssetsResponse) HasRealizedPnl() bool`

HasRealizedPnl returns a boolean if a field has been set.

### SetRealizedPnlNil

`func (o *PiSetServiceAPIModelsSetAccountAssetsResponse) SetRealizedPnlNil(b bool)`

 SetRealizedPnlNil sets the value for RealizedPnl to be an explicit nil

### UnsetRealizedPnl
`func (o *PiSetServiceAPIModelsSetAccountAssetsResponse) UnsetRealizedPnl()`

UnsetRealizedPnl ensures that no value is present for RealizedPnl, not even an explicit nil
### GetStockType

`func (o *PiSetServiceAPIModelsSetAccountAssetsResponse) GetStockType() string`

GetStockType returns the StockType field if non-nil, zero value otherwise.

### GetStockTypeOk

`func (o *PiSetServiceAPIModelsSetAccountAssetsResponse) GetStockTypeOk() (*string, bool)`

GetStockTypeOk returns a tuple with the StockType field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetStockType

`func (o *PiSetServiceAPIModelsSetAccountAssetsResponse) SetStockType(v string)`

SetStockType sets StockType field to given value.

### HasStockType

`func (o *PiSetServiceAPIModelsSetAccountAssetsResponse) HasStockType() bool`

HasStockType returns a boolean if a field has been set.

### SetStockTypeNil

`func (o *PiSetServiceAPIModelsSetAccountAssetsResponse) SetStockTypeNil(b bool)`

 SetStockTypeNil sets the value for StockType to be an explicit nil

### UnsetStockType
`func (o *PiSetServiceAPIModelsSetAccountAssetsResponse) UnsetStockType()`

UnsetStockType ensures that no value is present for StockType, not even an explicit nil
### GetLogo

`func (o *PiSetServiceAPIModelsSetAccountAssetsResponse) GetLogo() string`

GetLogo returns the Logo field if non-nil, zero value otherwise.

### GetLogoOk

`func (o *PiSetServiceAPIModelsSetAccountAssetsResponse) GetLogoOk() (*string, bool)`

GetLogoOk returns a tuple with the Logo field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetLogo

`func (o *PiSetServiceAPIModelsSetAccountAssetsResponse) SetLogo(v string)`

SetLogo sets Logo field to given value.

### HasLogo

`func (o *PiSetServiceAPIModelsSetAccountAssetsResponse) HasLogo() bool`

HasLogo returns a boolean if a field has been set.

### SetLogoNil

`func (o *PiSetServiceAPIModelsSetAccountAssetsResponse) SetLogoNil(b bool)`

 SetLogoNil sets the value for Logo to be an explicit nil

### UnsetLogo
`func (o *PiSetServiceAPIModelsSetAccountAssetsResponse) UnsetLogo()`

UnsetLogo ensures that no value is present for Logo, not even an explicit nil
### GetFriendlyName

`func (o *PiSetServiceAPIModelsSetAccountAssetsResponse) GetFriendlyName() string`

GetFriendlyName returns the FriendlyName field if non-nil, zero value otherwise.

### GetFriendlyNameOk

`func (o *PiSetServiceAPIModelsSetAccountAssetsResponse) GetFriendlyNameOk() (*string, bool)`

GetFriendlyNameOk returns a tuple with the FriendlyName field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetFriendlyName

`func (o *PiSetServiceAPIModelsSetAccountAssetsResponse) SetFriendlyName(v string)`

SetFriendlyName sets FriendlyName field to given value.

### HasFriendlyName

`func (o *PiSetServiceAPIModelsSetAccountAssetsResponse) HasFriendlyName() bool`

HasFriendlyName returns a boolean if a field has been set.

### SetFriendlyNameNil

`func (o *PiSetServiceAPIModelsSetAccountAssetsResponse) SetFriendlyNameNil(b bool)`

 SetFriendlyNameNil sets the value for FriendlyName to be an explicit nil

### UnsetFriendlyName
`func (o *PiSetServiceAPIModelsSetAccountAssetsResponse) UnsetFriendlyName()`

UnsetFriendlyName ensures that no value is present for FriendlyName, not even an explicit nil
### GetInstrumentCategory

`func (o *PiSetServiceAPIModelsSetAccountAssetsResponse) GetInstrumentCategory() string`

GetInstrumentCategory returns the InstrumentCategory field if non-nil, zero value otherwise.

### GetInstrumentCategoryOk

`func (o *PiSetServiceAPIModelsSetAccountAssetsResponse) GetInstrumentCategoryOk() (*string, bool)`

GetInstrumentCategoryOk returns a tuple with the InstrumentCategory field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetInstrumentCategory

`func (o *PiSetServiceAPIModelsSetAccountAssetsResponse) SetInstrumentCategory(v string)`

SetInstrumentCategory sets InstrumentCategory field to given value.

### HasInstrumentCategory

`func (o *PiSetServiceAPIModelsSetAccountAssetsResponse) HasInstrumentCategory() bool`

HasInstrumentCategory returns a boolean if a field has been set.

### SetInstrumentCategoryNil

`func (o *PiSetServiceAPIModelsSetAccountAssetsResponse) SetInstrumentCategoryNil(b bool)`

 SetInstrumentCategoryNil sets the value for InstrumentCategory to be an explicit nil

### UnsetInstrumentCategory
`func (o *PiSetServiceAPIModelsSetAccountAssetsResponse) UnsetInstrumentCategory()`

UnsetInstrumentCategory ensures that no value is present for InstrumentCategory, not even an explicit nil

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


