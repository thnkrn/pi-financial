# PiFinancialFundServiceAPIModelsSiriusFundAssetResponse

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**CustCode** | **NullableString** |  | 
**MarketPrice** | **float64** |  | 
**RemainUnit** | **float64** |  | 
**MarketValue** | **float64** |  | 
**CostValue** | **float64** |  | 
**Upnl** | **float64** |  | 
**UpnlPercentage** | **float64** |  | 
**FriendlyName** | Pointer to **NullableString** |  | [optional] 
**InstrumentCategory** | Pointer to **NullableString** |  | [optional] 
**UnitHolderId** | **NullableString** |  | 
**AverageCostPrice** | **float64** |  | 
**AsOfDate** | **NullableString** |  | 
**Symbol** | **NullableString** |  | 
**Logo** | **NullableString** |  | 
**RemainAmount** | **float64** |  | 
**AvailableVolume** | **float64** |  | 

## Methods

### NewPiFinancialFundServiceAPIModelsSiriusFundAssetResponse

`func NewPiFinancialFundServiceAPIModelsSiriusFundAssetResponse(custCode NullableString, marketPrice float64, remainUnit float64, marketValue float64, costValue float64, upnl float64, upnlPercentage float64, unitHolderId NullableString, averageCostPrice float64, asOfDate NullableString, symbol NullableString, logo NullableString, remainAmount float64, availableVolume float64, ) *PiFinancialFundServiceAPIModelsSiriusFundAssetResponse`

NewPiFinancialFundServiceAPIModelsSiriusFundAssetResponse instantiates a new PiFinancialFundServiceAPIModelsSiriusFundAssetResponse object
This constructor will assign default values to properties that have it defined,
and makes sure properties required by API are set, but the set of arguments
will change when the set of required properties is changed

### NewPiFinancialFundServiceAPIModelsSiriusFundAssetResponseWithDefaults

`func NewPiFinancialFundServiceAPIModelsSiriusFundAssetResponseWithDefaults() *PiFinancialFundServiceAPIModelsSiriusFundAssetResponse`

NewPiFinancialFundServiceAPIModelsSiriusFundAssetResponseWithDefaults instantiates a new PiFinancialFundServiceAPIModelsSiriusFundAssetResponse object
This constructor will only assign default values to properties that have it defined,
but it doesn't guarantee that properties required by API are set

### GetCustCode

`func (o *PiFinancialFundServiceAPIModelsSiriusFundAssetResponse) GetCustCode() string`

GetCustCode returns the CustCode field if non-nil, zero value otherwise.

### GetCustCodeOk

`func (o *PiFinancialFundServiceAPIModelsSiriusFundAssetResponse) GetCustCodeOk() (*string, bool)`

GetCustCodeOk returns a tuple with the CustCode field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetCustCode

`func (o *PiFinancialFundServiceAPIModelsSiriusFundAssetResponse) SetCustCode(v string)`

SetCustCode sets CustCode field to given value.


### SetCustCodeNil

`func (o *PiFinancialFundServiceAPIModelsSiriusFundAssetResponse) SetCustCodeNil(b bool)`

 SetCustCodeNil sets the value for CustCode to be an explicit nil

### UnsetCustCode
`func (o *PiFinancialFundServiceAPIModelsSiriusFundAssetResponse) UnsetCustCode()`

UnsetCustCode ensures that no value is present for CustCode, not even an explicit nil
### GetMarketPrice

`func (o *PiFinancialFundServiceAPIModelsSiriusFundAssetResponse) GetMarketPrice() float64`

GetMarketPrice returns the MarketPrice field if non-nil, zero value otherwise.

### GetMarketPriceOk

`func (o *PiFinancialFundServiceAPIModelsSiriusFundAssetResponse) GetMarketPriceOk() (*float64, bool)`

GetMarketPriceOk returns a tuple with the MarketPrice field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetMarketPrice

`func (o *PiFinancialFundServiceAPIModelsSiriusFundAssetResponse) SetMarketPrice(v float64)`

SetMarketPrice sets MarketPrice field to given value.


### GetRemainUnit

`func (o *PiFinancialFundServiceAPIModelsSiriusFundAssetResponse) GetRemainUnit() float64`

GetRemainUnit returns the RemainUnit field if non-nil, zero value otherwise.

### GetRemainUnitOk

`func (o *PiFinancialFundServiceAPIModelsSiriusFundAssetResponse) GetRemainUnitOk() (*float64, bool)`

GetRemainUnitOk returns a tuple with the RemainUnit field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetRemainUnit

`func (o *PiFinancialFundServiceAPIModelsSiriusFundAssetResponse) SetRemainUnit(v float64)`

SetRemainUnit sets RemainUnit field to given value.


### GetMarketValue

`func (o *PiFinancialFundServiceAPIModelsSiriusFundAssetResponse) GetMarketValue() float64`

GetMarketValue returns the MarketValue field if non-nil, zero value otherwise.

### GetMarketValueOk

`func (o *PiFinancialFundServiceAPIModelsSiriusFundAssetResponse) GetMarketValueOk() (*float64, bool)`

GetMarketValueOk returns a tuple with the MarketValue field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetMarketValue

`func (o *PiFinancialFundServiceAPIModelsSiriusFundAssetResponse) SetMarketValue(v float64)`

SetMarketValue sets MarketValue field to given value.


### GetCostValue

`func (o *PiFinancialFundServiceAPIModelsSiriusFundAssetResponse) GetCostValue() float64`

GetCostValue returns the CostValue field if non-nil, zero value otherwise.

### GetCostValueOk

`func (o *PiFinancialFundServiceAPIModelsSiriusFundAssetResponse) GetCostValueOk() (*float64, bool)`

GetCostValueOk returns a tuple with the CostValue field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetCostValue

`func (o *PiFinancialFundServiceAPIModelsSiriusFundAssetResponse) SetCostValue(v float64)`

SetCostValue sets CostValue field to given value.


### GetUpnl

`func (o *PiFinancialFundServiceAPIModelsSiriusFundAssetResponse) GetUpnl() float64`

GetUpnl returns the Upnl field if non-nil, zero value otherwise.

### GetUpnlOk

`func (o *PiFinancialFundServiceAPIModelsSiriusFundAssetResponse) GetUpnlOk() (*float64, bool)`

GetUpnlOk returns a tuple with the Upnl field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetUpnl

`func (o *PiFinancialFundServiceAPIModelsSiriusFundAssetResponse) SetUpnl(v float64)`

SetUpnl sets Upnl field to given value.


### GetUpnlPercentage

`func (o *PiFinancialFundServiceAPIModelsSiriusFundAssetResponse) GetUpnlPercentage() float64`

GetUpnlPercentage returns the UpnlPercentage field if non-nil, zero value otherwise.

### GetUpnlPercentageOk

`func (o *PiFinancialFundServiceAPIModelsSiriusFundAssetResponse) GetUpnlPercentageOk() (*float64, bool)`

GetUpnlPercentageOk returns a tuple with the UpnlPercentage field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetUpnlPercentage

`func (o *PiFinancialFundServiceAPIModelsSiriusFundAssetResponse) SetUpnlPercentage(v float64)`

SetUpnlPercentage sets UpnlPercentage field to given value.


### GetFriendlyName

`func (o *PiFinancialFundServiceAPIModelsSiriusFundAssetResponse) GetFriendlyName() string`

GetFriendlyName returns the FriendlyName field if non-nil, zero value otherwise.

### GetFriendlyNameOk

`func (o *PiFinancialFundServiceAPIModelsSiriusFundAssetResponse) GetFriendlyNameOk() (*string, bool)`

GetFriendlyNameOk returns a tuple with the FriendlyName field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetFriendlyName

`func (o *PiFinancialFundServiceAPIModelsSiriusFundAssetResponse) SetFriendlyName(v string)`

SetFriendlyName sets FriendlyName field to given value.

### HasFriendlyName

`func (o *PiFinancialFundServiceAPIModelsSiriusFundAssetResponse) HasFriendlyName() bool`

HasFriendlyName returns a boolean if a field has been set.

### SetFriendlyNameNil

`func (o *PiFinancialFundServiceAPIModelsSiriusFundAssetResponse) SetFriendlyNameNil(b bool)`

 SetFriendlyNameNil sets the value for FriendlyName to be an explicit nil

### UnsetFriendlyName
`func (o *PiFinancialFundServiceAPIModelsSiriusFundAssetResponse) UnsetFriendlyName()`

UnsetFriendlyName ensures that no value is present for FriendlyName, not even an explicit nil
### GetInstrumentCategory

`func (o *PiFinancialFundServiceAPIModelsSiriusFundAssetResponse) GetInstrumentCategory() string`

GetInstrumentCategory returns the InstrumentCategory field if non-nil, zero value otherwise.

### GetInstrumentCategoryOk

`func (o *PiFinancialFundServiceAPIModelsSiriusFundAssetResponse) GetInstrumentCategoryOk() (*string, bool)`

GetInstrumentCategoryOk returns a tuple with the InstrumentCategory field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetInstrumentCategory

`func (o *PiFinancialFundServiceAPIModelsSiriusFundAssetResponse) SetInstrumentCategory(v string)`

SetInstrumentCategory sets InstrumentCategory field to given value.

### HasInstrumentCategory

`func (o *PiFinancialFundServiceAPIModelsSiriusFundAssetResponse) HasInstrumentCategory() bool`

HasInstrumentCategory returns a boolean if a field has been set.

### SetInstrumentCategoryNil

`func (o *PiFinancialFundServiceAPIModelsSiriusFundAssetResponse) SetInstrumentCategoryNil(b bool)`

 SetInstrumentCategoryNil sets the value for InstrumentCategory to be an explicit nil

### UnsetInstrumentCategory
`func (o *PiFinancialFundServiceAPIModelsSiriusFundAssetResponse) UnsetInstrumentCategory()`

UnsetInstrumentCategory ensures that no value is present for InstrumentCategory, not even an explicit nil
### GetUnitHolderId

`func (o *PiFinancialFundServiceAPIModelsSiriusFundAssetResponse) GetUnitHolderId() string`

GetUnitHolderId returns the UnitHolderId field if non-nil, zero value otherwise.

### GetUnitHolderIdOk

`func (o *PiFinancialFundServiceAPIModelsSiriusFundAssetResponse) GetUnitHolderIdOk() (*string, bool)`

GetUnitHolderIdOk returns a tuple with the UnitHolderId field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetUnitHolderId

`func (o *PiFinancialFundServiceAPIModelsSiriusFundAssetResponse) SetUnitHolderId(v string)`

SetUnitHolderId sets UnitHolderId field to given value.


### SetUnitHolderIdNil

`func (o *PiFinancialFundServiceAPIModelsSiriusFundAssetResponse) SetUnitHolderIdNil(b bool)`

 SetUnitHolderIdNil sets the value for UnitHolderId to be an explicit nil

### UnsetUnitHolderId
`func (o *PiFinancialFundServiceAPIModelsSiriusFundAssetResponse) UnsetUnitHolderId()`

UnsetUnitHolderId ensures that no value is present for UnitHolderId, not even an explicit nil
### GetAverageCostPrice

`func (o *PiFinancialFundServiceAPIModelsSiriusFundAssetResponse) GetAverageCostPrice() float64`

GetAverageCostPrice returns the AverageCostPrice field if non-nil, zero value otherwise.

### GetAverageCostPriceOk

`func (o *PiFinancialFundServiceAPIModelsSiriusFundAssetResponse) GetAverageCostPriceOk() (*float64, bool)`

GetAverageCostPriceOk returns a tuple with the AverageCostPrice field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetAverageCostPrice

`func (o *PiFinancialFundServiceAPIModelsSiriusFundAssetResponse) SetAverageCostPrice(v float64)`

SetAverageCostPrice sets AverageCostPrice field to given value.


### GetAsOfDate

`func (o *PiFinancialFundServiceAPIModelsSiriusFundAssetResponse) GetAsOfDate() string`

GetAsOfDate returns the AsOfDate field if non-nil, zero value otherwise.

### GetAsOfDateOk

`func (o *PiFinancialFundServiceAPIModelsSiriusFundAssetResponse) GetAsOfDateOk() (*string, bool)`

GetAsOfDateOk returns a tuple with the AsOfDate field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetAsOfDate

`func (o *PiFinancialFundServiceAPIModelsSiriusFundAssetResponse) SetAsOfDate(v string)`

SetAsOfDate sets AsOfDate field to given value.


### SetAsOfDateNil

`func (o *PiFinancialFundServiceAPIModelsSiriusFundAssetResponse) SetAsOfDateNil(b bool)`

 SetAsOfDateNil sets the value for AsOfDate to be an explicit nil

### UnsetAsOfDate
`func (o *PiFinancialFundServiceAPIModelsSiriusFundAssetResponse) UnsetAsOfDate()`

UnsetAsOfDate ensures that no value is present for AsOfDate, not even an explicit nil
### GetSymbol

`func (o *PiFinancialFundServiceAPIModelsSiriusFundAssetResponse) GetSymbol() string`

GetSymbol returns the Symbol field if non-nil, zero value otherwise.

### GetSymbolOk

`func (o *PiFinancialFundServiceAPIModelsSiriusFundAssetResponse) GetSymbolOk() (*string, bool)`

GetSymbolOk returns a tuple with the Symbol field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetSymbol

`func (o *PiFinancialFundServiceAPIModelsSiriusFundAssetResponse) SetSymbol(v string)`

SetSymbol sets Symbol field to given value.


### SetSymbolNil

`func (o *PiFinancialFundServiceAPIModelsSiriusFundAssetResponse) SetSymbolNil(b bool)`

 SetSymbolNil sets the value for Symbol to be an explicit nil

### UnsetSymbol
`func (o *PiFinancialFundServiceAPIModelsSiriusFundAssetResponse) UnsetSymbol()`

UnsetSymbol ensures that no value is present for Symbol, not even an explicit nil
### GetLogo

`func (o *PiFinancialFundServiceAPIModelsSiriusFundAssetResponse) GetLogo() string`

GetLogo returns the Logo field if non-nil, zero value otherwise.

### GetLogoOk

`func (o *PiFinancialFundServiceAPIModelsSiriusFundAssetResponse) GetLogoOk() (*string, bool)`

GetLogoOk returns a tuple with the Logo field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetLogo

`func (o *PiFinancialFundServiceAPIModelsSiriusFundAssetResponse) SetLogo(v string)`

SetLogo sets Logo field to given value.


### SetLogoNil

`func (o *PiFinancialFundServiceAPIModelsSiriusFundAssetResponse) SetLogoNil(b bool)`

 SetLogoNil sets the value for Logo to be an explicit nil

### UnsetLogo
`func (o *PiFinancialFundServiceAPIModelsSiriusFundAssetResponse) UnsetLogo()`

UnsetLogo ensures that no value is present for Logo, not even an explicit nil
### GetRemainAmount

`func (o *PiFinancialFundServiceAPIModelsSiriusFundAssetResponse) GetRemainAmount() float64`

GetRemainAmount returns the RemainAmount field if non-nil, zero value otherwise.

### GetRemainAmountOk

`func (o *PiFinancialFundServiceAPIModelsSiriusFundAssetResponse) GetRemainAmountOk() (*float64, bool)`

GetRemainAmountOk returns a tuple with the RemainAmount field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetRemainAmount

`func (o *PiFinancialFundServiceAPIModelsSiriusFundAssetResponse) SetRemainAmount(v float64)`

SetRemainAmount sets RemainAmount field to given value.


### GetAvailableVolume

`func (o *PiFinancialFundServiceAPIModelsSiriusFundAssetResponse) GetAvailableVolume() float64`

GetAvailableVolume returns the AvailableVolume field if non-nil, zero value otherwise.

### GetAvailableVolumeOk

`func (o *PiFinancialFundServiceAPIModelsSiriusFundAssetResponse) GetAvailableVolumeOk() (*float64, bool)`

GetAvailableVolumeOk returns a tuple with the AvailableVolume field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetAvailableVolume

`func (o *PiFinancialFundServiceAPIModelsSiriusFundAssetResponse) SetAvailableVolume(v float64)`

SetAvailableVolume sets AvailableVolume field to given value.



[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


