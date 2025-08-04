# PiFinancialFundServiceAPIModelsSiriusPlaceSwitchOrderRequest

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**Symbol** | **string** |  | 
**EffectiveDate** | **string** |  | 
**TradingAccountNo** | **string** |  | 
**Quantity** | **float64** |  | 
**TargetSymbol** | **string** |  | 
**UnitHolderId** | **string** |  | 
**UnitType** | **string** |  | 
**SellAllFlag** | Pointer to **NullableBool** |  | [optional] 

## Methods

### NewPiFinancialFundServiceAPIModelsSiriusPlaceSwitchOrderRequest

`func NewPiFinancialFundServiceAPIModelsSiriusPlaceSwitchOrderRequest(symbol string, effectiveDate string, tradingAccountNo string, quantity float64, targetSymbol string, unitHolderId string, unitType string, ) *PiFinancialFundServiceAPIModelsSiriusPlaceSwitchOrderRequest`

NewPiFinancialFundServiceAPIModelsSiriusPlaceSwitchOrderRequest instantiates a new PiFinancialFundServiceAPIModelsSiriusPlaceSwitchOrderRequest object
This constructor will assign default values to properties that have it defined,
and makes sure properties required by API are set, but the set of arguments
will change when the set of required properties is changed

### NewPiFinancialFundServiceAPIModelsSiriusPlaceSwitchOrderRequestWithDefaults

`func NewPiFinancialFundServiceAPIModelsSiriusPlaceSwitchOrderRequestWithDefaults() *PiFinancialFundServiceAPIModelsSiriusPlaceSwitchOrderRequest`

NewPiFinancialFundServiceAPIModelsSiriusPlaceSwitchOrderRequestWithDefaults instantiates a new PiFinancialFundServiceAPIModelsSiriusPlaceSwitchOrderRequest object
This constructor will only assign default values to properties that have it defined,
but it doesn't guarantee that properties required by API are set

### GetSymbol

`func (o *PiFinancialFundServiceAPIModelsSiriusPlaceSwitchOrderRequest) GetSymbol() string`

GetSymbol returns the Symbol field if non-nil, zero value otherwise.

### GetSymbolOk

`func (o *PiFinancialFundServiceAPIModelsSiriusPlaceSwitchOrderRequest) GetSymbolOk() (*string, bool)`

GetSymbolOk returns a tuple with the Symbol field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetSymbol

`func (o *PiFinancialFundServiceAPIModelsSiriusPlaceSwitchOrderRequest) SetSymbol(v string)`

SetSymbol sets Symbol field to given value.


### GetEffectiveDate

`func (o *PiFinancialFundServiceAPIModelsSiriusPlaceSwitchOrderRequest) GetEffectiveDate() string`

GetEffectiveDate returns the EffectiveDate field if non-nil, zero value otherwise.

### GetEffectiveDateOk

`func (o *PiFinancialFundServiceAPIModelsSiriusPlaceSwitchOrderRequest) GetEffectiveDateOk() (*string, bool)`

GetEffectiveDateOk returns a tuple with the EffectiveDate field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetEffectiveDate

`func (o *PiFinancialFundServiceAPIModelsSiriusPlaceSwitchOrderRequest) SetEffectiveDate(v string)`

SetEffectiveDate sets EffectiveDate field to given value.


### GetTradingAccountNo

`func (o *PiFinancialFundServiceAPIModelsSiriusPlaceSwitchOrderRequest) GetTradingAccountNo() string`

GetTradingAccountNo returns the TradingAccountNo field if non-nil, zero value otherwise.

### GetTradingAccountNoOk

`func (o *PiFinancialFundServiceAPIModelsSiriusPlaceSwitchOrderRequest) GetTradingAccountNoOk() (*string, bool)`

GetTradingAccountNoOk returns a tuple with the TradingAccountNo field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetTradingAccountNo

`func (o *PiFinancialFundServiceAPIModelsSiriusPlaceSwitchOrderRequest) SetTradingAccountNo(v string)`

SetTradingAccountNo sets TradingAccountNo field to given value.


### GetQuantity

`func (o *PiFinancialFundServiceAPIModelsSiriusPlaceSwitchOrderRequest) GetQuantity() float64`

GetQuantity returns the Quantity field if non-nil, zero value otherwise.

### GetQuantityOk

`func (o *PiFinancialFundServiceAPIModelsSiriusPlaceSwitchOrderRequest) GetQuantityOk() (*float64, bool)`

GetQuantityOk returns a tuple with the Quantity field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetQuantity

`func (o *PiFinancialFundServiceAPIModelsSiriusPlaceSwitchOrderRequest) SetQuantity(v float64)`

SetQuantity sets Quantity field to given value.


### GetTargetSymbol

`func (o *PiFinancialFundServiceAPIModelsSiriusPlaceSwitchOrderRequest) GetTargetSymbol() string`

GetTargetSymbol returns the TargetSymbol field if non-nil, zero value otherwise.

### GetTargetSymbolOk

`func (o *PiFinancialFundServiceAPIModelsSiriusPlaceSwitchOrderRequest) GetTargetSymbolOk() (*string, bool)`

GetTargetSymbolOk returns a tuple with the TargetSymbol field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetTargetSymbol

`func (o *PiFinancialFundServiceAPIModelsSiriusPlaceSwitchOrderRequest) SetTargetSymbol(v string)`

SetTargetSymbol sets TargetSymbol field to given value.


### GetUnitHolderId

`func (o *PiFinancialFundServiceAPIModelsSiriusPlaceSwitchOrderRequest) GetUnitHolderId() string`

GetUnitHolderId returns the UnitHolderId field if non-nil, zero value otherwise.

### GetUnitHolderIdOk

`func (o *PiFinancialFundServiceAPIModelsSiriusPlaceSwitchOrderRequest) GetUnitHolderIdOk() (*string, bool)`

GetUnitHolderIdOk returns a tuple with the UnitHolderId field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetUnitHolderId

`func (o *PiFinancialFundServiceAPIModelsSiriusPlaceSwitchOrderRequest) SetUnitHolderId(v string)`

SetUnitHolderId sets UnitHolderId field to given value.


### GetUnitType

`func (o *PiFinancialFundServiceAPIModelsSiriusPlaceSwitchOrderRequest) GetUnitType() string`

GetUnitType returns the UnitType field if non-nil, zero value otherwise.

### GetUnitTypeOk

`func (o *PiFinancialFundServiceAPIModelsSiriusPlaceSwitchOrderRequest) GetUnitTypeOk() (*string, bool)`

GetUnitTypeOk returns a tuple with the UnitType field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetUnitType

`func (o *PiFinancialFundServiceAPIModelsSiriusPlaceSwitchOrderRequest) SetUnitType(v string)`

SetUnitType sets UnitType field to given value.


### GetSellAllFlag

`func (o *PiFinancialFundServiceAPIModelsSiriusPlaceSwitchOrderRequest) GetSellAllFlag() bool`

GetSellAllFlag returns the SellAllFlag field if non-nil, zero value otherwise.

### GetSellAllFlagOk

`func (o *PiFinancialFundServiceAPIModelsSiriusPlaceSwitchOrderRequest) GetSellAllFlagOk() (*bool, bool)`

GetSellAllFlagOk returns a tuple with the SellAllFlag field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetSellAllFlag

`func (o *PiFinancialFundServiceAPIModelsSiriusPlaceSwitchOrderRequest) SetSellAllFlag(v bool)`

SetSellAllFlag sets SellAllFlag field to given value.

### HasSellAllFlag

`func (o *PiFinancialFundServiceAPIModelsSiriusPlaceSwitchOrderRequest) HasSellAllFlag() bool`

HasSellAllFlag returns a boolean if a field has been set.

### SetSellAllFlagNil

`func (o *PiFinancialFundServiceAPIModelsSiriusPlaceSwitchOrderRequest) SetSellAllFlagNil(b bool)`

 SetSellAllFlagNil sets the value for SellAllFlag to be an explicit nil

### UnsetSellAllFlag
`func (o *PiFinancialFundServiceAPIModelsSiriusPlaceSwitchOrderRequest) UnsetSellAllFlag()`

UnsetSellAllFlag ensures that no value is present for SellAllFlag, not even an explicit nil

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


