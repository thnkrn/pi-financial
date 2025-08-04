# PiFinancialFundServiceAPIModelsSiriusFundOrderResponse

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**Unit** | **float64** |  | 
**Amount** | **float64** |  | 
**PaymentMethod** | **NullableString** |  | 
**BankAccount** | **NullableString** |  | 
**Edd** | **NullableString** |  | 
**SwitchIn** | **NullableString** |  | 
**SwitchTo** | **NullableString** |  | 
**BankCode** | **NullableString** |  | 
**BankShortName** | **NullableString** |  | 
**OrderId** | **NullableString** |  | 
**Account** | **NullableString** |  | 
**FundCode** | **NullableString** |  | 
**OrderType** | **NullableString** |  | 
**Status** | **string** |  | 
**Nav** | **NullableFloat64** |  | 
**TransactionLastUpdated** | **NullableString** |  | 
**EffectiveDate** | **NullableString** |  | 
**TransactionDateTime** | **NullableString** |  | 
**SettlementDate** | Pointer to **NullableString** |  | [optional] 
**OrderNo** | Pointer to **NullableString** |  | [optional] 

## Methods

### NewPiFinancialFundServiceAPIModelsSiriusFundOrderResponse

`func NewPiFinancialFundServiceAPIModelsSiriusFundOrderResponse(unit float64, amount float64, paymentMethod NullableString, bankAccount NullableString, edd NullableString, switchIn NullableString, switchTo NullableString, bankCode NullableString, bankShortName NullableString, orderId NullableString, account NullableString, fundCode NullableString, orderType NullableString, status string, nav NullableFloat64, transactionLastUpdated NullableString, effectiveDate NullableString, transactionDateTime NullableString, ) *PiFinancialFundServiceAPIModelsSiriusFundOrderResponse`

NewPiFinancialFundServiceAPIModelsSiriusFundOrderResponse instantiates a new PiFinancialFundServiceAPIModelsSiriusFundOrderResponse object
This constructor will assign default values to properties that have it defined,
and makes sure properties required by API are set, but the set of arguments
will change when the set of required properties is changed

### NewPiFinancialFundServiceAPIModelsSiriusFundOrderResponseWithDefaults

`func NewPiFinancialFundServiceAPIModelsSiriusFundOrderResponseWithDefaults() *PiFinancialFundServiceAPIModelsSiriusFundOrderResponse`

NewPiFinancialFundServiceAPIModelsSiriusFundOrderResponseWithDefaults instantiates a new PiFinancialFundServiceAPIModelsSiriusFundOrderResponse object
This constructor will only assign default values to properties that have it defined,
but it doesn't guarantee that properties required by API are set

### GetUnit

`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderResponse) GetUnit() float64`

GetUnit returns the Unit field if non-nil, zero value otherwise.

### GetUnitOk

`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderResponse) GetUnitOk() (*float64, bool)`

GetUnitOk returns a tuple with the Unit field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetUnit

`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderResponse) SetUnit(v float64)`

SetUnit sets Unit field to given value.


### GetAmount

`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderResponse) GetAmount() float64`

GetAmount returns the Amount field if non-nil, zero value otherwise.

### GetAmountOk

`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderResponse) GetAmountOk() (*float64, bool)`

GetAmountOk returns a tuple with the Amount field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetAmount

`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderResponse) SetAmount(v float64)`

SetAmount sets Amount field to given value.


### GetPaymentMethod

`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderResponse) GetPaymentMethod() string`

GetPaymentMethod returns the PaymentMethod field if non-nil, zero value otherwise.

### GetPaymentMethodOk

`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderResponse) GetPaymentMethodOk() (*string, bool)`

GetPaymentMethodOk returns a tuple with the PaymentMethod field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetPaymentMethod

`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderResponse) SetPaymentMethod(v string)`

SetPaymentMethod sets PaymentMethod field to given value.


### SetPaymentMethodNil

`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderResponse) SetPaymentMethodNil(b bool)`

 SetPaymentMethodNil sets the value for PaymentMethod to be an explicit nil

### UnsetPaymentMethod
`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderResponse) UnsetPaymentMethod()`

UnsetPaymentMethod ensures that no value is present for PaymentMethod, not even an explicit nil
### GetBankAccount

`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderResponse) GetBankAccount() string`

GetBankAccount returns the BankAccount field if non-nil, zero value otherwise.

### GetBankAccountOk

`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderResponse) GetBankAccountOk() (*string, bool)`

GetBankAccountOk returns a tuple with the BankAccount field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetBankAccount

`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderResponse) SetBankAccount(v string)`

SetBankAccount sets BankAccount field to given value.


### SetBankAccountNil

`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderResponse) SetBankAccountNil(b bool)`

 SetBankAccountNil sets the value for BankAccount to be an explicit nil

### UnsetBankAccount
`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderResponse) UnsetBankAccount()`

UnsetBankAccount ensures that no value is present for BankAccount, not even an explicit nil
### GetEdd

`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderResponse) GetEdd() string`

GetEdd returns the Edd field if non-nil, zero value otherwise.

### GetEddOk

`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderResponse) GetEddOk() (*string, bool)`

GetEddOk returns a tuple with the Edd field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetEdd

`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderResponse) SetEdd(v string)`

SetEdd sets Edd field to given value.


### SetEddNil

`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderResponse) SetEddNil(b bool)`

 SetEddNil sets the value for Edd to be an explicit nil

### UnsetEdd
`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderResponse) UnsetEdd()`

UnsetEdd ensures that no value is present for Edd, not even an explicit nil
### GetSwitchIn

`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderResponse) GetSwitchIn() string`

GetSwitchIn returns the SwitchIn field if non-nil, zero value otherwise.

### GetSwitchInOk

`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderResponse) GetSwitchInOk() (*string, bool)`

GetSwitchInOk returns a tuple with the SwitchIn field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetSwitchIn

`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderResponse) SetSwitchIn(v string)`

SetSwitchIn sets SwitchIn field to given value.


### SetSwitchInNil

`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderResponse) SetSwitchInNil(b bool)`

 SetSwitchInNil sets the value for SwitchIn to be an explicit nil

### UnsetSwitchIn
`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderResponse) UnsetSwitchIn()`

UnsetSwitchIn ensures that no value is present for SwitchIn, not even an explicit nil
### GetSwitchTo

`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderResponse) GetSwitchTo() string`

GetSwitchTo returns the SwitchTo field if non-nil, zero value otherwise.

### GetSwitchToOk

`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderResponse) GetSwitchToOk() (*string, bool)`

GetSwitchToOk returns a tuple with the SwitchTo field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetSwitchTo

`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderResponse) SetSwitchTo(v string)`

SetSwitchTo sets SwitchTo field to given value.


### SetSwitchToNil

`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderResponse) SetSwitchToNil(b bool)`

 SetSwitchToNil sets the value for SwitchTo to be an explicit nil

### UnsetSwitchTo
`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderResponse) UnsetSwitchTo()`

UnsetSwitchTo ensures that no value is present for SwitchTo, not even an explicit nil
### GetBankCode

`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderResponse) GetBankCode() string`

GetBankCode returns the BankCode field if non-nil, zero value otherwise.

### GetBankCodeOk

`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderResponse) GetBankCodeOk() (*string, bool)`

GetBankCodeOk returns a tuple with the BankCode field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetBankCode

`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderResponse) SetBankCode(v string)`

SetBankCode sets BankCode field to given value.


### SetBankCodeNil

`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderResponse) SetBankCodeNil(b bool)`

 SetBankCodeNil sets the value for BankCode to be an explicit nil

### UnsetBankCode
`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderResponse) UnsetBankCode()`

UnsetBankCode ensures that no value is present for BankCode, not even an explicit nil
### GetBankShortName

`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderResponse) GetBankShortName() string`

GetBankShortName returns the BankShortName field if non-nil, zero value otherwise.

### GetBankShortNameOk

`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderResponse) GetBankShortNameOk() (*string, bool)`

GetBankShortNameOk returns a tuple with the BankShortName field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetBankShortName

`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderResponse) SetBankShortName(v string)`

SetBankShortName sets BankShortName field to given value.


### SetBankShortNameNil

`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderResponse) SetBankShortNameNil(b bool)`

 SetBankShortNameNil sets the value for BankShortName to be an explicit nil

### UnsetBankShortName
`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderResponse) UnsetBankShortName()`

UnsetBankShortName ensures that no value is present for BankShortName, not even an explicit nil
### GetOrderId

`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderResponse) GetOrderId() string`

GetOrderId returns the OrderId field if non-nil, zero value otherwise.

### GetOrderIdOk

`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderResponse) GetOrderIdOk() (*string, bool)`

GetOrderIdOk returns a tuple with the OrderId field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetOrderId

`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderResponse) SetOrderId(v string)`

SetOrderId sets OrderId field to given value.


### SetOrderIdNil

`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderResponse) SetOrderIdNil(b bool)`

 SetOrderIdNil sets the value for OrderId to be an explicit nil

### UnsetOrderId
`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderResponse) UnsetOrderId()`

UnsetOrderId ensures that no value is present for OrderId, not even an explicit nil
### GetAccount

`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderResponse) GetAccount() string`

GetAccount returns the Account field if non-nil, zero value otherwise.

### GetAccountOk

`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderResponse) GetAccountOk() (*string, bool)`

GetAccountOk returns a tuple with the Account field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetAccount

`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderResponse) SetAccount(v string)`

SetAccount sets Account field to given value.


### SetAccountNil

`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderResponse) SetAccountNil(b bool)`

 SetAccountNil sets the value for Account to be an explicit nil

### UnsetAccount
`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderResponse) UnsetAccount()`

UnsetAccount ensures that no value is present for Account, not even an explicit nil
### GetFundCode

`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderResponse) GetFundCode() string`

GetFundCode returns the FundCode field if non-nil, zero value otherwise.

### GetFundCodeOk

`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderResponse) GetFundCodeOk() (*string, bool)`

GetFundCodeOk returns a tuple with the FundCode field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetFundCode

`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderResponse) SetFundCode(v string)`

SetFundCode sets FundCode field to given value.


### SetFundCodeNil

`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderResponse) SetFundCodeNil(b bool)`

 SetFundCodeNil sets the value for FundCode to be an explicit nil

### UnsetFundCode
`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderResponse) UnsetFundCode()`

UnsetFundCode ensures that no value is present for FundCode, not even an explicit nil
### GetOrderType

`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderResponse) GetOrderType() string`

GetOrderType returns the OrderType field if non-nil, zero value otherwise.

### GetOrderTypeOk

`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderResponse) GetOrderTypeOk() (*string, bool)`

GetOrderTypeOk returns a tuple with the OrderType field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetOrderType

`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderResponse) SetOrderType(v string)`

SetOrderType sets OrderType field to given value.


### SetOrderTypeNil

`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderResponse) SetOrderTypeNil(b bool)`

 SetOrderTypeNil sets the value for OrderType to be an explicit nil

### UnsetOrderType
`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderResponse) UnsetOrderType()`

UnsetOrderType ensures that no value is present for OrderType, not even an explicit nil
### GetStatus

`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderResponse) GetStatus() string`

GetStatus returns the Status field if non-nil, zero value otherwise.

### GetStatusOk

`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderResponse) GetStatusOk() (*string, bool)`

GetStatusOk returns a tuple with the Status field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetStatus

`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderResponse) SetStatus(v string)`

SetStatus sets Status field to given value.


### GetNav

`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderResponse) GetNav() float64`

GetNav returns the Nav field if non-nil, zero value otherwise.

### GetNavOk

`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderResponse) GetNavOk() (*float64, bool)`

GetNavOk returns a tuple with the Nav field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetNav

`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderResponse) SetNav(v float64)`

SetNav sets Nav field to given value.


### SetNavNil

`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderResponse) SetNavNil(b bool)`

 SetNavNil sets the value for Nav to be an explicit nil

### UnsetNav
`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderResponse) UnsetNav()`

UnsetNav ensures that no value is present for Nav, not even an explicit nil
### GetTransactionLastUpdated

`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderResponse) GetTransactionLastUpdated() string`

GetTransactionLastUpdated returns the TransactionLastUpdated field if non-nil, zero value otherwise.

### GetTransactionLastUpdatedOk

`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderResponse) GetTransactionLastUpdatedOk() (*string, bool)`

GetTransactionLastUpdatedOk returns a tuple with the TransactionLastUpdated field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetTransactionLastUpdated

`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderResponse) SetTransactionLastUpdated(v string)`

SetTransactionLastUpdated sets TransactionLastUpdated field to given value.


### SetTransactionLastUpdatedNil

`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderResponse) SetTransactionLastUpdatedNil(b bool)`

 SetTransactionLastUpdatedNil sets the value for TransactionLastUpdated to be an explicit nil

### UnsetTransactionLastUpdated
`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderResponse) UnsetTransactionLastUpdated()`

UnsetTransactionLastUpdated ensures that no value is present for TransactionLastUpdated, not even an explicit nil
### GetEffectiveDate

`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderResponse) GetEffectiveDate() string`

GetEffectiveDate returns the EffectiveDate field if non-nil, zero value otherwise.

### GetEffectiveDateOk

`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderResponse) GetEffectiveDateOk() (*string, bool)`

GetEffectiveDateOk returns a tuple with the EffectiveDate field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetEffectiveDate

`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderResponse) SetEffectiveDate(v string)`

SetEffectiveDate sets EffectiveDate field to given value.


### SetEffectiveDateNil

`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderResponse) SetEffectiveDateNil(b bool)`

 SetEffectiveDateNil sets the value for EffectiveDate to be an explicit nil

### UnsetEffectiveDate
`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderResponse) UnsetEffectiveDate()`

UnsetEffectiveDate ensures that no value is present for EffectiveDate, not even an explicit nil
### GetTransactionDateTime

`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderResponse) GetTransactionDateTime() string`

GetTransactionDateTime returns the TransactionDateTime field if non-nil, zero value otherwise.

### GetTransactionDateTimeOk

`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderResponse) GetTransactionDateTimeOk() (*string, bool)`

GetTransactionDateTimeOk returns a tuple with the TransactionDateTime field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetTransactionDateTime

`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderResponse) SetTransactionDateTime(v string)`

SetTransactionDateTime sets TransactionDateTime field to given value.


### SetTransactionDateTimeNil

`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderResponse) SetTransactionDateTimeNil(b bool)`

 SetTransactionDateTimeNil sets the value for TransactionDateTime to be an explicit nil

### UnsetTransactionDateTime
`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderResponse) UnsetTransactionDateTime()`

UnsetTransactionDateTime ensures that no value is present for TransactionDateTime, not even an explicit nil
### GetSettlementDate

`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderResponse) GetSettlementDate() string`

GetSettlementDate returns the SettlementDate field if non-nil, zero value otherwise.

### GetSettlementDateOk

`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderResponse) GetSettlementDateOk() (*string, bool)`

GetSettlementDateOk returns a tuple with the SettlementDate field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetSettlementDate

`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderResponse) SetSettlementDate(v string)`

SetSettlementDate sets SettlementDate field to given value.

### HasSettlementDate

`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderResponse) HasSettlementDate() bool`

HasSettlementDate returns a boolean if a field has been set.

### SetSettlementDateNil

`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderResponse) SetSettlementDateNil(b bool)`

 SetSettlementDateNil sets the value for SettlementDate to be an explicit nil

### UnsetSettlementDate
`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderResponse) UnsetSettlementDate()`

UnsetSettlementDate ensures that no value is present for SettlementDate, not even an explicit nil
### GetOrderNo

`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderResponse) GetOrderNo() string`

GetOrderNo returns the OrderNo field if non-nil, zero value otherwise.

### GetOrderNoOk

`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderResponse) GetOrderNoOk() (*string, bool)`

GetOrderNoOk returns a tuple with the OrderNo field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetOrderNo

`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderResponse) SetOrderNo(v string)`

SetOrderNo sets OrderNo field to given value.

### HasOrderNo

`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderResponse) HasOrderNo() bool`

HasOrderNo returns a boolean if a field has been set.

### SetOrderNoNil

`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderResponse) SetOrderNoNil(b bool)`

 SetOrderNoNil sets the value for OrderNo to be an explicit nil

### UnsetOrderNo
`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderResponse) UnsetOrderNo()`

UnsetOrderNo ensures that no value is present for OrderNo, not even an explicit nil

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


