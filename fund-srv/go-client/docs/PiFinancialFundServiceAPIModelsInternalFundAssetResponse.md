# PiFinancialFundServiceAPIModelsInternalFundAssetResponse

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
**AsOfDate** | **string** |  | 
**FundCode** | **NullableString** |  | 
**TradingAccountNo** | **NullableString** |  | 
**Unit** | **float64** |  | 
**AvgCostPrice** | **float64** |  | 
**RemainAmount** | **float64** |  | 
**PendingAmount** | **float64** |  | 
**PendingUnit** | **float64** |  | 

## Methods

### NewPiFinancialFundServiceAPIModelsInternalFundAssetResponse

`func NewPiFinancialFundServiceAPIModelsInternalFundAssetResponse(custCode NullableString, marketPrice float64, remainUnit float64, marketValue float64, costValue float64, upnl float64, upnlPercentage float64, unitHolderId NullableString, asOfDate string, fundCode NullableString, tradingAccountNo NullableString, unit float64, avgCostPrice float64, remainAmount float64, pendingAmount float64, pendingUnit float64, ) *PiFinancialFundServiceAPIModelsInternalFundAssetResponse`

NewPiFinancialFundServiceAPIModelsInternalFundAssetResponse instantiates a new PiFinancialFundServiceAPIModelsInternalFundAssetResponse object
This constructor will assign default values to properties that have it defined,
and makes sure properties required by API are set, but the set of arguments
will change when the set of required properties is changed

### NewPiFinancialFundServiceAPIModelsInternalFundAssetResponseWithDefaults

`func NewPiFinancialFundServiceAPIModelsInternalFundAssetResponseWithDefaults() *PiFinancialFundServiceAPIModelsInternalFundAssetResponse`

NewPiFinancialFundServiceAPIModelsInternalFundAssetResponseWithDefaults instantiates a new PiFinancialFundServiceAPIModelsInternalFundAssetResponse object
This constructor will only assign default values to properties that have it defined,
but it doesn't guarantee that properties required by API are set

### GetCustCode

`func (o *PiFinancialFundServiceAPIModelsInternalFundAssetResponse) GetCustCode() string`

GetCustCode returns the CustCode field if non-nil, zero value otherwise.

### GetCustCodeOk

`func (o *PiFinancialFundServiceAPIModelsInternalFundAssetResponse) GetCustCodeOk() (*string, bool)`

GetCustCodeOk returns a tuple with the CustCode field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetCustCode

`func (o *PiFinancialFundServiceAPIModelsInternalFundAssetResponse) SetCustCode(v string)`

SetCustCode sets CustCode field to given value.


### SetCustCodeNil

`func (o *PiFinancialFundServiceAPIModelsInternalFundAssetResponse) SetCustCodeNil(b bool)`

 SetCustCodeNil sets the value for CustCode to be an explicit nil

### UnsetCustCode
`func (o *PiFinancialFundServiceAPIModelsInternalFundAssetResponse) UnsetCustCode()`

UnsetCustCode ensures that no value is present for CustCode, not even an explicit nil
### GetMarketPrice

`func (o *PiFinancialFundServiceAPIModelsInternalFundAssetResponse) GetMarketPrice() float64`

GetMarketPrice returns the MarketPrice field if non-nil, zero value otherwise.

### GetMarketPriceOk

`func (o *PiFinancialFundServiceAPIModelsInternalFundAssetResponse) GetMarketPriceOk() (*float64, bool)`

GetMarketPriceOk returns a tuple with the MarketPrice field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetMarketPrice

`func (o *PiFinancialFundServiceAPIModelsInternalFundAssetResponse) SetMarketPrice(v float64)`

SetMarketPrice sets MarketPrice field to given value.


### GetRemainUnit

`func (o *PiFinancialFundServiceAPIModelsInternalFundAssetResponse) GetRemainUnit() float64`

GetRemainUnit returns the RemainUnit field if non-nil, zero value otherwise.

### GetRemainUnitOk

`func (o *PiFinancialFundServiceAPIModelsInternalFundAssetResponse) GetRemainUnitOk() (*float64, bool)`

GetRemainUnitOk returns a tuple with the RemainUnit field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetRemainUnit

`func (o *PiFinancialFundServiceAPIModelsInternalFundAssetResponse) SetRemainUnit(v float64)`

SetRemainUnit sets RemainUnit field to given value.


### GetMarketValue

`func (o *PiFinancialFundServiceAPIModelsInternalFundAssetResponse) GetMarketValue() float64`

GetMarketValue returns the MarketValue field if non-nil, zero value otherwise.

### GetMarketValueOk

`func (o *PiFinancialFundServiceAPIModelsInternalFundAssetResponse) GetMarketValueOk() (*float64, bool)`

GetMarketValueOk returns a tuple with the MarketValue field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetMarketValue

`func (o *PiFinancialFundServiceAPIModelsInternalFundAssetResponse) SetMarketValue(v float64)`

SetMarketValue sets MarketValue field to given value.


### GetCostValue

`func (o *PiFinancialFundServiceAPIModelsInternalFundAssetResponse) GetCostValue() float64`

GetCostValue returns the CostValue field if non-nil, zero value otherwise.

### GetCostValueOk

`func (o *PiFinancialFundServiceAPIModelsInternalFundAssetResponse) GetCostValueOk() (*float64, bool)`

GetCostValueOk returns a tuple with the CostValue field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetCostValue

`func (o *PiFinancialFundServiceAPIModelsInternalFundAssetResponse) SetCostValue(v float64)`

SetCostValue sets CostValue field to given value.


### GetUpnl

`func (o *PiFinancialFundServiceAPIModelsInternalFundAssetResponse) GetUpnl() float64`

GetUpnl returns the Upnl field if non-nil, zero value otherwise.

### GetUpnlOk

`func (o *PiFinancialFundServiceAPIModelsInternalFundAssetResponse) GetUpnlOk() (*float64, bool)`

GetUpnlOk returns a tuple with the Upnl field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetUpnl

`func (o *PiFinancialFundServiceAPIModelsInternalFundAssetResponse) SetUpnl(v float64)`

SetUpnl sets Upnl field to given value.


### GetUpnlPercentage

`func (o *PiFinancialFundServiceAPIModelsInternalFundAssetResponse) GetUpnlPercentage() float64`

GetUpnlPercentage returns the UpnlPercentage field if non-nil, zero value otherwise.

### GetUpnlPercentageOk

`func (o *PiFinancialFundServiceAPIModelsInternalFundAssetResponse) GetUpnlPercentageOk() (*float64, bool)`

GetUpnlPercentageOk returns a tuple with the UpnlPercentage field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetUpnlPercentage

`func (o *PiFinancialFundServiceAPIModelsInternalFundAssetResponse) SetUpnlPercentage(v float64)`

SetUpnlPercentage sets UpnlPercentage field to given value.


### GetFriendlyName

`func (o *PiFinancialFundServiceAPIModelsInternalFundAssetResponse) GetFriendlyName() string`

GetFriendlyName returns the FriendlyName field if non-nil, zero value otherwise.

### GetFriendlyNameOk

`func (o *PiFinancialFundServiceAPIModelsInternalFundAssetResponse) GetFriendlyNameOk() (*string, bool)`

GetFriendlyNameOk returns a tuple with the FriendlyName field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetFriendlyName

`func (o *PiFinancialFundServiceAPIModelsInternalFundAssetResponse) SetFriendlyName(v string)`

SetFriendlyName sets FriendlyName field to given value.

### HasFriendlyName

`func (o *PiFinancialFundServiceAPIModelsInternalFundAssetResponse) HasFriendlyName() bool`

HasFriendlyName returns a boolean if a field has been set.

### SetFriendlyNameNil

`func (o *PiFinancialFundServiceAPIModelsInternalFundAssetResponse) SetFriendlyNameNil(b bool)`

 SetFriendlyNameNil sets the value for FriendlyName to be an explicit nil

### UnsetFriendlyName
`func (o *PiFinancialFundServiceAPIModelsInternalFundAssetResponse) UnsetFriendlyName()`

UnsetFriendlyName ensures that no value is present for FriendlyName, not even an explicit nil
### GetInstrumentCategory

`func (o *PiFinancialFundServiceAPIModelsInternalFundAssetResponse) GetInstrumentCategory() string`

GetInstrumentCategory returns the InstrumentCategory field if non-nil, zero value otherwise.

### GetInstrumentCategoryOk

`func (o *PiFinancialFundServiceAPIModelsInternalFundAssetResponse) GetInstrumentCategoryOk() (*string, bool)`

GetInstrumentCategoryOk returns a tuple with the InstrumentCategory field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetInstrumentCategory

`func (o *PiFinancialFundServiceAPIModelsInternalFundAssetResponse) SetInstrumentCategory(v string)`

SetInstrumentCategory sets InstrumentCategory field to given value.

### HasInstrumentCategory

`func (o *PiFinancialFundServiceAPIModelsInternalFundAssetResponse) HasInstrumentCategory() bool`

HasInstrumentCategory returns a boolean if a field has been set.

### SetInstrumentCategoryNil

`func (o *PiFinancialFundServiceAPIModelsInternalFundAssetResponse) SetInstrumentCategoryNil(b bool)`

 SetInstrumentCategoryNil sets the value for InstrumentCategory to be an explicit nil

### UnsetInstrumentCategory
`func (o *PiFinancialFundServiceAPIModelsInternalFundAssetResponse) UnsetInstrumentCategory()`

UnsetInstrumentCategory ensures that no value is present for InstrumentCategory, not even an explicit nil
### GetUnitHolderId

`func (o *PiFinancialFundServiceAPIModelsInternalFundAssetResponse) GetUnitHolderId() string`

GetUnitHolderId returns the UnitHolderId field if non-nil, zero value otherwise.

### GetUnitHolderIdOk

`func (o *PiFinancialFundServiceAPIModelsInternalFundAssetResponse) GetUnitHolderIdOk() (*string, bool)`

GetUnitHolderIdOk returns a tuple with the UnitHolderId field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetUnitHolderId

`func (o *PiFinancialFundServiceAPIModelsInternalFundAssetResponse) SetUnitHolderId(v string)`

SetUnitHolderId sets UnitHolderId field to given value.


### SetUnitHolderIdNil

`func (o *PiFinancialFundServiceAPIModelsInternalFundAssetResponse) SetUnitHolderIdNil(b bool)`

 SetUnitHolderIdNil sets the value for UnitHolderId to be an explicit nil

### UnsetUnitHolderId
`func (o *PiFinancialFundServiceAPIModelsInternalFundAssetResponse) UnsetUnitHolderId()`

UnsetUnitHolderId ensures that no value is present for UnitHolderId, not even an explicit nil
### GetAsOfDate

`func (o *PiFinancialFundServiceAPIModelsInternalFundAssetResponse) GetAsOfDate() string`

GetAsOfDate returns the AsOfDate field if non-nil, zero value otherwise.

### GetAsOfDateOk

`func (o *PiFinancialFundServiceAPIModelsInternalFundAssetResponse) GetAsOfDateOk() (*string, bool)`

GetAsOfDateOk returns a tuple with the AsOfDate field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetAsOfDate

`func (o *PiFinancialFundServiceAPIModelsInternalFundAssetResponse) SetAsOfDate(v string)`

SetAsOfDate sets AsOfDate field to given value.


### GetFundCode

`func (o *PiFinancialFundServiceAPIModelsInternalFundAssetResponse) GetFundCode() string`

GetFundCode returns the FundCode field if non-nil, zero value otherwise.

### GetFundCodeOk

`func (o *PiFinancialFundServiceAPIModelsInternalFundAssetResponse) GetFundCodeOk() (*string, bool)`

GetFundCodeOk returns a tuple with the FundCode field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetFundCode

`func (o *PiFinancialFundServiceAPIModelsInternalFundAssetResponse) SetFundCode(v string)`

SetFundCode sets FundCode field to given value.


### SetFundCodeNil

`func (o *PiFinancialFundServiceAPIModelsInternalFundAssetResponse) SetFundCodeNil(b bool)`

 SetFundCodeNil sets the value for FundCode to be an explicit nil

### UnsetFundCode
`func (o *PiFinancialFundServiceAPIModelsInternalFundAssetResponse) UnsetFundCode()`

UnsetFundCode ensures that no value is present for FundCode, not even an explicit nil
### GetTradingAccountNo

`func (o *PiFinancialFundServiceAPIModelsInternalFundAssetResponse) GetTradingAccountNo() string`

GetTradingAccountNo returns the TradingAccountNo field if non-nil, zero value otherwise.

### GetTradingAccountNoOk

`func (o *PiFinancialFundServiceAPIModelsInternalFundAssetResponse) GetTradingAccountNoOk() (*string, bool)`

GetTradingAccountNoOk returns a tuple with the TradingAccountNo field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetTradingAccountNo

`func (o *PiFinancialFundServiceAPIModelsInternalFundAssetResponse) SetTradingAccountNo(v string)`

SetTradingAccountNo sets TradingAccountNo field to given value.


### SetTradingAccountNoNil

`func (o *PiFinancialFundServiceAPIModelsInternalFundAssetResponse) SetTradingAccountNoNil(b bool)`

 SetTradingAccountNoNil sets the value for TradingAccountNo to be an explicit nil

### UnsetTradingAccountNo
`func (o *PiFinancialFundServiceAPIModelsInternalFundAssetResponse) UnsetTradingAccountNo()`

UnsetTradingAccountNo ensures that no value is present for TradingAccountNo, not even an explicit nil
### GetUnit

`func (o *PiFinancialFundServiceAPIModelsInternalFundAssetResponse) GetUnit() float64`

GetUnit returns the Unit field if non-nil, zero value otherwise.

### GetUnitOk

`func (o *PiFinancialFundServiceAPIModelsInternalFundAssetResponse) GetUnitOk() (*float64, bool)`

GetUnitOk returns a tuple with the Unit field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetUnit

`func (o *PiFinancialFundServiceAPIModelsInternalFundAssetResponse) SetUnit(v float64)`

SetUnit sets Unit field to given value.


### GetAvgCostPrice

`func (o *PiFinancialFundServiceAPIModelsInternalFundAssetResponse) GetAvgCostPrice() float64`

GetAvgCostPrice returns the AvgCostPrice field if non-nil, zero value otherwise.

### GetAvgCostPriceOk

`func (o *PiFinancialFundServiceAPIModelsInternalFundAssetResponse) GetAvgCostPriceOk() (*float64, bool)`

GetAvgCostPriceOk returns a tuple with the AvgCostPrice field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetAvgCostPrice

`func (o *PiFinancialFundServiceAPIModelsInternalFundAssetResponse) SetAvgCostPrice(v float64)`

SetAvgCostPrice sets AvgCostPrice field to given value.


### GetRemainAmount

`func (o *PiFinancialFundServiceAPIModelsInternalFundAssetResponse) GetRemainAmount() float64`

GetRemainAmount returns the RemainAmount field if non-nil, zero value otherwise.

### GetRemainAmountOk

`func (o *PiFinancialFundServiceAPIModelsInternalFundAssetResponse) GetRemainAmountOk() (*float64, bool)`

GetRemainAmountOk returns a tuple with the RemainAmount field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetRemainAmount

`func (o *PiFinancialFundServiceAPIModelsInternalFundAssetResponse) SetRemainAmount(v float64)`

SetRemainAmount sets RemainAmount field to given value.


### GetPendingAmount

`func (o *PiFinancialFundServiceAPIModelsInternalFundAssetResponse) GetPendingAmount() float64`

GetPendingAmount returns the PendingAmount field if non-nil, zero value otherwise.

### GetPendingAmountOk

`func (o *PiFinancialFundServiceAPIModelsInternalFundAssetResponse) GetPendingAmountOk() (*float64, bool)`

GetPendingAmountOk returns a tuple with the PendingAmount field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetPendingAmount

`func (o *PiFinancialFundServiceAPIModelsInternalFundAssetResponse) SetPendingAmount(v float64)`

SetPendingAmount sets PendingAmount field to given value.


### GetPendingUnit

`func (o *PiFinancialFundServiceAPIModelsInternalFundAssetResponse) GetPendingUnit() float64`

GetPendingUnit returns the PendingUnit field if non-nil, zero value otherwise.

### GetPendingUnitOk

`func (o *PiFinancialFundServiceAPIModelsInternalFundAssetResponse) GetPendingUnitOk() (*float64, bool)`

GetPendingUnitOk returns a tuple with the PendingUnit field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetPendingUnit

`func (o *PiFinancialFundServiceAPIModelsInternalFundAssetResponse) SetPendingUnit(v float64)`

SetPendingUnit sets PendingUnit field to given value.



[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


