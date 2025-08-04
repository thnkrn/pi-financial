# PiFinancialFundServiceAPIModelsSiriusPlaceSellOrderRequest

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**Symbol** | **string** |  | 
**EffectiveDate** | **string** |  | 
**TradingAccountNo** | **string** |  | 
**Quantity** | **float64** |  | 
**UnitType** | **string** |  | 
**UnitHolderId** | **string** |  | 
**BankAccount** | **string** |  | 
**BankCode** | **string** |  | 
**SellAllFlag** | Pointer to **NullableBool** |  | [optional] 

## Methods

### NewPiFinancialFundServiceAPIModelsSiriusPlaceSellOrderRequest

`func NewPiFinancialFundServiceAPIModelsSiriusPlaceSellOrderRequest(symbol string, effectiveDate string, tradingAccountNo string, quantity float64, unitType string, unitHolderId string, bankAccount string, bankCode string, ) *PiFinancialFundServiceAPIModelsSiriusPlaceSellOrderRequest`

NewPiFinancialFundServiceAPIModelsSiriusPlaceSellOrderRequest instantiates a new PiFinancialFundServiceAPIModelsSiriusPlaceSellOrderRequest object
This constructor will assign default values to properties that have it defined,
and makes sure properties required by API are set, but the set of arguments
will change when the set of required properties is changed

### NewPiFinancialFundServiceAPIModelsSiriusPlaceSellOrderRequestWithDefaults

`func NewPiFinancialFundServiceAPIModelsSiriusPlaceSellOrderRequestWithDefaults() *PiFinancialFundServiceAPIModelsSiriusPlaceSellOrderRequest`

NewPiFinancialFundServiceAPIModelsSiriusPlaceSellOrderRequestWithDefaults instantiates a new PiFinancialFundServiceAPIModelsSiriusPlaceSellOrderRequest object
This constructor will only assign default values to properties that have it defined,
but it doesn't guarantee that properties required by API are set

### GetSymbol

`func (o *PiFinancialFundServiceAPIModelsSiriusPlaceSellOrderRequest) GetSymbol() string`

GetSymbol returns the Symbol field if non-nil, zero value otherwise.

### GetSymbolOk

`func (o *PiFinancialFundServiceAPIModelsSiriusPlaceSellOrderRequest) GetSymbolOk() (*string, bool)`

GetSymbolOk returns a tuple with the Symbol field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetSymbol

`func (o *PiFinancialFundServiceAPIModelsSiriusPlaceSellOrderRequest) SetSymbol(v string)`

SetSymbol sets Symbol field to given value.


### GetEffectiveDate

`func (o *PiFinancialFundServiceAPIModelsSiriusPlaceSellOrderRequest) GetEffectiveDate() string`

GetEffectiveDate returns the EffectiveDate field if non-nil, zero value otherwise.

### GetEffectiveDateOk

`func (o *PiFinancialFundServiceAPIModelsSiriusPlaceSellOrderRequest) GetEffectiveDateOk() (*string, bool)`

GetEffectiveDateOk returns a tuple with the EffectiveDate field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetEffectiveDate

`func (o *PiFinancialFundServiceAPIModelsSiriusPlaceSellOrderRequest) SetEffectiveDate(v string)`

SetEffectiveDate sets EffectiveDate field to given value.


### GetTradingAccountNo

`func (o *PiFinancialFundServiceAPIModelsSiriusPlaceSellOrderRequest) GetTradingAccountNo() string`

GetTradingAccountNo returns the TradingAccountNo field if non-nil, zero value otherwise.

### GetTradingAccountNoOk

`func (o *PiFinancialFundServiceAPIModelsSiriusPlaceSellOrderRequest) GetTradingAccountNoOk() (*string, bool)`

GetTradingAccountNoOk returns a tuple with the TradingAccountNo field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetTradingAccountNo

`func (o *PiFinancialFundServiceAPIModelsSiriusPlaceSellOrderRequest) SetTradingAccountNo(v string)`

SetTradingAccountNo sets TradingAccountNo field to given value.


### GetQuantity

`func (o *PiFinancialFundServiceAPIModelsSiriusPlaceSellOrderRequest) GetQuantity() float64`

GetQuantity returns the Quantity field if non-nil, zero value otherwise.

### GetQuantityOk

`func (o *PiFinancialFundServiceAPIModelsSiriusPlaceSellOrderRequest) GetQuantityOk() (*float64, bool)`

GetQuantityOk returns a tuple with the Quantity field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetQuantity

`func (o *PiFinancialFundServiceAPIModelsSiriusPlaceSellOrderRequest) SetQuantity(v float64)`

SetQuantity sets Quantity field to given value.


### GetUnitType

`func (o *PiFinancialFundServiceAPIModelsSiriusPlaceSellOrderRequest) GetUnitType() string`

GetUnitType returns the UnitType field if non-nil, zero value otherwise.

### GetUnitTypeOk

`func (o *PiFinancialFundServiceAPIModelsSiriusPlaceSellOrderRequest) GetUnitTypeOk() (*string, bool)`

GetUnitTypeOk returns a tuple with the UnitType field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetUnitType

`func (o *PiFinancialFundServiceAPIModelsSiriusPlaceSellOrderRequest) SetUnitType(v string)`

SetUnitType sets UnitType field to given value.


### GetUnitHolderId

`func (o *PiFinancialFundServiceAPIModelsSiriusPlaceSellOrderRequest) GetUnitHolderId() string`

GetUnitHolderId returns the UnitHolderId field if non-nil, zero value otherwise.

### GetUnitHolderIdOk

`func (o *PiFinancialFundServiceAPIModelsSiriusPlaceSellOrderRequest) GetUnitHolderIdOk() (*string, bool)`

GetUnitHolderIdOk returns a tuple with the UnitHolderId field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetUnitHolderId

`func (o *PiFinancialFundServiceAPIModelsSiriusPlaceSellOrderRequest) SetUnitHolderId(v string)`

SetUnitHolderId sets UnitHolderId field to given value.


### GetBankAccount

`func (o *PiFinancialFundServiceAPIModelsSiriusPlaceSellOrderRequest) GetBankAccount() string`

GetBankAccount returns the BankAccount field if non-nil, zero value otherwise.

### GetBankAccountOk

`func (o *PiFinancialFundServiceAPIModelsSiriusPlaceSellOrderRequest) GetBankAccountOk() (*string, bool)`

GetBankAccountOk returns a tuple with the BankAccount field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetBankAccount

`func (o *PiFinancialFundServiceAPIModelsSiriusPlaceSellOrderRequest) SetBankAccount(v string)`

SetBankAccount sets BankAccount field to given value.


### GetBankCode

`func (o *PiFinancialFundServiceAPIModelsSiriusPlaceSellOrderRequest) GetBankCode() string`

GetBankCode returns the BankCode field if non-nil, zero value otherwise.

### GetBankCodeOk

`func (o *PiFinancialFundServiceAPIModelsSiriusPlaceSellOrderRequest) GetBankCodeOk() (*string, bool)`

GetBankCodeOk returns a tuple with the BankCode field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetBankCode

`func (o *PiFinancialFundServiceAPIModelsSiriusPlaceSellOrderRequest) SetBankCode(v string)`

SetBankCode sets BankCode field to given value.


### GetSellAllFlag

`func (o *PiFinancialFundServiceAPIModelsSiriusPlaceSellOrderRequest) GetSellAllFlag() bool`

GetSellAllFlag returns the SellAllFlag field if non-nil, zero value otherwise.

### GetSellAllFlagOk

`func (o *PiFinancialFundServiceAPIModelsSiriusPlaceSellOrderRequest) GetSellAllFlagOk() (*bool, bool)`

GetSellAllFlagOk returns a tuple with the SellAllFlag field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetSellAllFlag

`func (o *PiFinancialFundServiceAPIModelsSiriusPlaceSellOrderRequest) SetSellAllFlag(v bool)`

SetSellAllFlag sets SellAllFlag field to given value.

### HasSellAllFlag

`func (o *PiFinancialFundServiceAPIModelsSiriusPlaceSellOrderRequest) HasSellAllFlag() bool`

HasSellAllFlag returns a boolean if a field has been set.

### SetSellAllFlagNil

`func (o *PiFinancialFundServiceAPIModelsSiriusPlaceSellOrderRequest) SetSellAllFlagNil(b bool)`

 SetSellAllFlagNil sets the value for SellAllFlag to be an explicit nil

### UnsetSellAllFlag
`func (o *PiFinancialFundServiceAPIModelsSiriusPlaceSellOrderRequest) UnsetSellAllFlag()`

UnsetSellAllFlag ensures that no value is present for SellAllFlag, not even an explicit nil

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


