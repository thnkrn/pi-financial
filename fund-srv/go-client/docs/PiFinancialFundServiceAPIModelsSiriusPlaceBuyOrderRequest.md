# PiFinancialFundServiceAPIModelsSiriusPlaceBuyOrderRequest

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**Symbol** | **string** |  | 
**EffectiveDate** | **string** |  | 
**TradingAccountNo** | **string** |  | 
**Quantity** | **float64** |  | 
**PaymentMethod** | **string** |  | 
**BankAccount** | **string** |  | 
**BankCode** | **string** |  | 
**UnitHolderId** | Pointer to **NullableString** |  | [optional] 
**Recurring** | Pointer to **NullableBool** |  | [optional] 

## Methods

### NewPiFinancialFundServiceAPIModelsSiriusPlaceBuyOrderRequest

`func NewPiFinancialFundServiceAPIModelsSiriusPlaceBuyOrderRequest(symbol string, effectiveDate string, tradingAccountNo string, quantity float64, paymentMethod string, bankAccount string, bankCode string, ) *PiFinancialFundServiceAPIModelsSiriusPlaceBuyOrderRequest`

NewPiFinancialFundServiceAPIModelsSiriusPlaceBuyOrderRequest instantiates a new PiFinancialFundServiceAPIModelsSiriusPlaceBuyOrderRequest object
This constructor will assign default values to properties that have it defined,
and makes sure properties required by API are set, but the set of arguments
will change when the set of required properties is changed

### NewPiFinancialFundServiceAPIModelsSiriusPlaceBuyOrderRequestWithDefaults

`func NewPiFinancialFundServiceAPIModelsSiriusPlaceBuyOrderRequestWithDefaults() *PiFinancialFundServiceAPIModelsSiriusPlaceBuyOrderRequest`

NewPiFinancialFundServiceAPIModelsSiriusPlaceBuyOrderRequestWithDefaults instantiates a new PiFinancialFundServiceAPIModelsSiriusPlaceBuyOrderRequest object
This constructor will only assign default values to properties that have it defined,
but it doesn't guarantee that properties required by API are set

### GetSymbol

`func (o *PiFinancialFundServiceAPIModelsSiriusPlaceBuyOrderRequest) GetSymbol() string`

GetSymbol returns the Symbol field if non-nil, zero value otherwise.

### GetSymbolOk

`func (o *PiFinancialFundServiceAPIModelsSiriusPlaceBuyOrderRequest) GetSymbolOk() (*string, bool)`

GetSymbolOk returns a tuple with the Symbol field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetSymbol

`func (o *PiFinancialFundServiceAPIModelsSiriusPlaceBuyOrderRequest) SetSymbol(v string)`

SetSymbol sets Symbol field to given value.


### GetEffectiveDate

`func (o *PiFinancialFundServiceAPIModelsSiriusPlaceBuyOrderRequest) GetEffectiveDate() string`

GetEffectiveDate returns the EffectiveDate field if non-nil, zero value otherwise.

### GetEffectiveDateOk

`func (o *PiFinancialFundServiceAPIModelsSiriusPlaceBuyOrderRequest) GetEffectiveDateOk() (*string, bool)`

GetEffectiveDateOk returns a tuple with the EffectiveDate field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetEffectiveDate

`func (o *PiFinancialFundServiceAPIModelsSiriusPlaceBuyOrderRequest) SetEffectiveDate(v string)`

SetEffectiveDate sets EffectiveDate field to given value.


### GetTradingAccountNo

`func (o *PiFinancialFundServiceAPIModelsSiriusPlaceBuyOrderRequest) GetTradingAccountNo() string`

GetTradingAccountNo returns the TradingAccountNo field if non-nil, zero value otherwise.

### GetTradingAccountNoOk

`func (o *PiFinancialFundServiceAPIModelsSiriusPlaceBuyOrderRequest) GetTradingAccountNoOk() (*string, bool)`

GetTradingAccountNoOk returns a tuple with the TradingAccountNo field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetTradingAccountNo

`func (o *PiFinancialFundServiceAPIModelsSiriusPlaceBuyOrderRequest) SetTradingAccountNo(v string)`

SetTradingAccountNo sets TradingAccountNo field to given value.


### GetQuantity

`func (o *PiFinancialFundServiceAPIModelsSiriusPlaceBuyOrderRequest) GetQuantity() float64`

GetQuantity returns the Quantity field if non-nil, zero value otherwise.

### GetQuantityOk

`func (o *PiFinancialFundServiceAPIModelsSiriusPlaceBuyOrderRequest) GetQuantityOk() (*float64, bool)`

GetQuantityOk returns a tuple with the Quantity field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetQuantity

`func (o *PiFinancialFundServiceAPIModelsSiriusPlaceBuyOrderRequest) SetQuantity(v float64)`

SetQuantity sets Quantity field to given value.


### GetPaymentMethod

`func (o *PiFinancialFundServiceAPIModelsSiriusPlaceBuyOrderRequest) GetPaymentMethod() string`

GetPaymentMethod returns the PaymentMethod field if non-nil, zero value otherwise.

### GetPaymentMethodOk

`func (o *PiFinancialFundServiceAPIModelsSiriusPlaceBuyOrderRequest) GetPaymentMethodOk() (*string, bool)`

GetPaymentMethodOk returns a tuple with the PaymentMethod field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetPaymentMethod

`func (o *PiFinancialFundServiceAPIModelsSiriusPlaceBuyOrderRequest) SetPaymentMethod(v string)`

SetPaymentMethod sets PaymentMethod field to given value.


### GetBankAccount

`func (o *PiFinancialFundServiceAPIModelsSiriusPlaceBuyOrderRequest) GetBankAccount() string`

GetBankAccount returns the BankAccount field if non-nil, zero value otherwise.

### GetBankAccountOk

`func (o *PiFinancialFundServiceAPIModelsSiriusPlaceBuyOrderRequest) GetBankAccountOk() (*string, bool)`

GetBankAccountOk returns a tuple with the BankAccount field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetBankAccount

`func (o *PiFinancialFundServiceAPIModelsSiriusPlaceBuyOrderRequest) SetBankAccount(v string)`

SetBankAccount sets BankAccount field to given value.


### GetBankCode

`func (o *PiFinancialFundServiceAPIModelsSiriusPlaceBuyOrderRequest) GetBankCode() string`

GetBankCode returns the BankCode field if non-nil, zero value otherwise.

### GetBankCodeOk

`func (o *PiFinancialFundServiceAPIModelsSiriusPlaceBuyOrderRequest) GetBankCodeOk() (*string, bool)`

GetBankCodeOk returns a tuple with the BankCode field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetBankCode

`func (o *PiFinancialFundServiceAPIModelsSiriusPlaceBuyOrderRequest) SetBankCode(v string)`

SetBankCode sets BankCode field to given value.


### GetUnitHolderId

`func (o *PiFinancialFundServiceAPIModelsSiriusPlaceBuyOrderRequest) GetUnitHolderId() string`

GetUnitHolderId returns the UnitHolderId field if non-nil, zero value otherwise.

### GetUnitHolderIdOk

`func (o *PiFinancialFundServiceAPIModelsSiriusPlaceBuyOrderRequest) GetUnitHolderIdOk() (*string, bool)`

GetUnitHolderIdOk returns a tuple with the UnitHolderId field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetUnitHolderId

`func (o *PiFinancialFundServiceAPIModelsSiriusPlaceBuyOrderRequest) SetUnitHolderId(v string)`

SetUnitHolderId sets UnitHolderId field to given value.

### HasUnitHolderId

`func (o *PiFinancialFundServiceAPIModelsSiriusPlaceBuyOrderRequest) HasUnitHolderId() bool`

HasUnitHolderId returns a boolean if a field has been set.

### SetUnitHolderIdNil

`func (o *PiFinancialFundServiceAPIModelsSiriusPlaceBuyOrderRequest) SetUnitHolderIdNil(b bool)`

 SetUnitHolderIdNil sets the value for UnitHolderId to be an explicit nil

### UnsetUnitHolderId
`func (o *PiFinancialFundServiceAPIModelsSiriusPlaceBuyOrderRequest) UnsetUnitHolderId()`

UnsetUnitHolderId ensures that no value is present for UnitHolderId, not even an explicit nil
### GetRecurring

`func (o *PiFinancialFundServiceAPIModelsSiriusPlaceBuyOrderRequest) GetRecurring() bool`

GetRecurring returns the Recurring field if non-nil, zero value otherwise.

### GetRecurringOk

`func (o *PiFinancialFundServiceAPIModelsSiriusPlaceBuyOrderRequest) GetRecurringOk() (*bool, bool)`

GetRecurringOk returns a tuple with the Recurring field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetRecurring

`func (o *PiFinancialFundServiceAPIModelsSiriusPlaceBuyOrderRequest) SetRecurring(v bool)`

SetRecurring sets Recurring field to given value.

### HasRecurring

`func (o *PiFinancialFundServiceAPIModelsSiriusPlaceBuyOrderRequest) HasRecurring() bool`

HasRecurring returns a boolean if a field has been set.

### SetRecurringNil

`func (o *PiFinancialFundServiceAPIModelsSiriusPlaceBuyOrderRequest) SetRecurringNil(b bool)`

 SetRecurringNil sets the value for Recurring to be an explicit nil

### UnsetRecurring
`func (o *PiFinancialFundServiceAPIModelsSiriusPlaceBuyOrderRequest) UnsetRecurring()`

UnsetRecurring ensures that no value is present for Recurring, not even an explicit nil

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


