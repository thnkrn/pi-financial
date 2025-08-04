# PiFinancialFundServiceAPIModelsSiriusFundOrderHistoryResponse

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**Unit** | **float64** |  | 
**Amount** | **float64** |  | 
**FundCode** | **NullableString** |  | 
**PaymentMethod** | **NullableString** |  | 
**BankCode** | **NullableString** |  | 
**BankShortName** | **NullableString** |  | 
**BankAccount** | **NullableString** |  | 
**OrderId** | **NullableString** |  | 
**Account** | **NullableString** |  | 
**OrderType** | **NullableString** |  | 
**Status** | **string** |  | 
**Nav** | **NullableFloat64** |  | 
**EffectiveDate** | **NullableString** |  | 
**TransactionDateTime** | **NullableString** |  | 
**UnitHolderId** | **NullableString** |  | 
**Channel** | **string** |  | 
**AccountType** | **NullableString** |  | 
**SettlementDate** | Pointer to **NullableString** |  | [optional] 
**OrderNo** | Pointer to **NullableString** |  | [optional] 
**RejectReason** | Pointer to **NullableString** |  | [optional] 
**TransactionLastUpdated** | Pointer to **NullableString** |  | [optional] 
**PaymentStatus** | Pointer to **NullableString** |  | [optional] 

## Methods

### NewPiFinancialFundServiceAPIModelsSiriusFundOrderHistoryResponse

`func NewPiFinancialFundServiceAPIModelsSiriusFundOrderHistoryResponse(unit float64, amount float64, fundCode NullableString, paymentMethod NullableString, bankCode NullableString, bankShortName NullableString, bankAccount NullableString, orderId NullableString, account NullableString, orderType NullableString, status string, nav NullableFloat64, effectiveDate NullableString, transactionDateTime NullableString, unitHolderId NullableString, channel string, accountType NullableString, ) *PiFinancialFundServiceAPIModelsSiriusFundOrderHistoryResponse`

NewPiFinancialFundServiceAPIModelsSiriusFundOrderHistoryResponse instantiates a new PiFinancialFundServiceAPIModelsSiriusFundOrderHistoryResponse object
This constructor will assign default values to properties that have it defined,
and makes sure properties required by API are set, but the set of arguments
will change when the set of required properties is changed

### NewPiFinancialFundServiceAPIModelsSiriusFundOrderHistoryResponseWithDefaults

`func NewPiFinancialFundServiceAPIModelsSiriusFundOrderHistoryResponseWithDefaults() *PiFinancialFundServiceAPIModelsSiriusFundOrderHistoryResponse`

NewPiFinancialFundServiceAPIModelsSiriusFundOrderHistoryResponseWithDefaults instantiates a new PiFinancialFundServiceAPIModelsSiriusFundOrderHistoryResponse object
This constructor will only assign default values to properties that have it defined,
but it doesn't guarantee that properties required by API are set

### GetUnit

`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderHistoryResponse) GetUnit() float64`

GetUnit returns the Unit field if non-nil, zero value otherwise.

### GetUnitOk

`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderHistoryResponse) GetUnitOk() (*float64, bool)`

GetUnitOk returns a tuple with the Unit field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetUnit

`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderHistoryResponse) SetUnit(v float64)`

SetUnit sets Unit field to given value.


### GetAmount

`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderHistoryResponse) GetAmount() float64`

GetAmount returns the Amount field if non-nil, zero value otherwise.

### GetAmountOk

`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderHistoryResponse) GetAmountOk() (*float64, bool)`

GetAmountOk returns a tuple with the Amount field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetAmount

`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderHistoryResponse) SetAmount(v float64)`

SetAmount sets Amount field to given value.


### GetFundCode

`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderHistoryResponse) GetFundCode() string`

GetFundCode returns the FundCode field if non-nil, zero value otherwise.

### GetFundCodeOk

`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderHistoryResponse) GetFundCodeOk() (*string, bool)`

GetFundCodeOk returns a tuple with the FundCode field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetFundCode

`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderHistoryResponse) SetFundCode(v string)`

SetFundCode sets FundCode field to given value.


### SetFundCodeNil

`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderHistoryResponse) SetFundCodeNil(b bool)`

 SetFundCodeNil sets the value for FundCode to be an explicit nil

### UnsetFundCode
`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderHistoryResponse) UnsetFundCode()`

UnsetFundCode ensures that no value is present for FundCode, not even an explicit nil
### GetPaymentMethod

`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderHistoryResponse) GetPaymentMethod() string`

GetPaymentMethod returns the PaymentMethod field if non-nil, zero value otherwise.

### GetPaymentMethodOk

`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderHistoryResponse) GetPaymentMethodOk() (*string, bool)`

GetPaymentMethodOk returns a tuple with the PaymentMethod field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetPaymentMethod

`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderHistoryResponse) SetPaymentMethod(v string)`

SetPaymentMethod sets PaymentMethod field to given value.


### SetPaymentMethodNil

`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderHistoryResponse) SetPaymentMethodNil(b bool)`

 SetPaymentMethodNil sets the value for PaymentMethod to be an explicit nil

### UnsetPaymentMethod
`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderHistoryResponse) UnsetPaymentMethod()`

UnsetPaymentMethod ensures that no value is present for PaymentMethod, not even an explicit nil
### GetBankCode

`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderHistoryResponse) GetBankCode() string`

GetBankCode returns the BankCode field if non-nil, zero value otherwise.

### GetBankCodeOk

`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderHistoryResponse) GetBankCodeOk() (*string, bool)`

GetBankCodeOk returns a tuple with the BankCode field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetBankCode

`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderHistoryResponse) SetBankCode(v string)`

SetBankCode sets BankCode field to given value.


### SetBankCodeNil

`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderHistoryResponse) SetBankCodeNil(b bool)`

 SetBankCodeNil sets the value for BankCode to be an explicit nil

### UnsetBankCode
`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderHistoryResponse) UnsetBankCode()`

UnsetBankCode ensures that no value is present for BankCode, not even an explicit nil
### GetBankShortName

`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderHistoryResponse) GetBankShortName() string`

GetBankShortName returns the BankShortName field if non-nil, zero value otherwise.

### GetBankShortNameOk

`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderHistoryResponse) GetBankShortNameOk() (*string, bool)`

GetBankShortNameOk returns a tuple with the BankShortName field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetBankShortName

`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderHistoryResponse) SetBankShortName(v string)`

SetBankShortName sets BankShortName field to given value.


### SetBankShortNameNil

`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderHistoryResponse) SetBankShortNameNil(b bool)`

 SetBankShortNameNil sets the value for BankShortName to be an explicit nil

### UnsetBankShortName
`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderHistoryResponse) UnsetBankShortName()`

UnsetBankShortName ensures that no value is present for BankShortName, not even an explicit nil
### GetBankAccount

`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderHistoryResponse) GetBankAccount() string`

GetBankAccount returns the BankAccount field if non-nil, zero value otherwise.

### GetBankAccountOk

`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderHistoryResponse) GetBankAccountOk() (*string, bool)`

GetBankAccountOk returns a tuple with the BankAccount field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetBankAccount

`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderHistoryResponse) SetBankAccount(v string)`

SetBankAccount sets BankAccount field to given value.


### SetBankAccountNil

`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderHistoryResponse) SetBankAccountNil(b bool)`

 SetBankAccountNil sets the value for BankAccount to be an explicit nil

### UnsetBankAccount
`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderHistoryResponse) UnsetBankAccount()`

UnsetBankAccount ensures that no value is present for BankAccount, not even an explicit nil
### GetOrderId

`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderHistoryResponse) GetOrderId() string`

GetOrderId returns the OrderId field if non-nil, zero value otherwise.

### GetOrderIdOk

`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderHistoryResponse) GetOrderIdOk() (*string, bool)`

GetOrderIdOk returns a tuple with the OrderId field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetOrderId

`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderHistoryResponse) SetOrderId(v string)`

SetOrderId sets OrderId field to given value.


### SetOrderIdNil

`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderHistoryResponse) SetOrderIdNil(b bool)`

 SetOrderIdNil sets the value for OrderId to be an explicit nil

### UnsetOrderId
`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderHistoryResponse) UnsetOrderId()`

UnsetOrderId ensures that no value is present for OrderId, not even an explicit nil
### GetAccount

`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderHistoryResponse) GetAccount() string`

GetAccount returns the Account field if non-nil, zero value otherwise.

### GetAccountOk

`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderHistoryResponse) GetAccountOk() (*string, bool)`

GetAccountOk returns a tuple with the Account field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetAccount

`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderHistoryResponse) SetAccount(v string)`

SetAccount sets Account field to given value.


### SetAccountNil

`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderHistoryResponse) SetAccountNil(b bool)`

 SetAccountNil sets the value for Account to be an explicit nil

### UnsetAccount
`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderHistoryResponse) UnsetAccount()`

UnsetAccount ensures that no value is present for Account, not even an explicit nil
### GetOrderType

`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderHistoryResponse) GetOrderType() string`

GetOrderType returns the OrderType field if non-nil, zero value otherwise.

### GetOrderTypeOk

`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderHistoryResponse) GetOrderTypeOk() (*string, bool)`

GetOrderTypeOk returns a tuple with the OrderType field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetOrderType

`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderHistoryResponse) SetOrderType(v string)`

SetOrderType sets OrderType field to given value.


### SetOrderTypeNil

`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderHistoryResponse) SetOrderTypeNil(b bool)`

 SetOrderTypeNil sets the value for OrderType to be an explicit nil

### UnsetOrderType
`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderHistoryResponse) UnsetOrderType()`

UnsetOrderType ensures that no value is present for OrderType, not even an explicit nil
### GetStatus

`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderHistoryResponse) GetStatus() string`

GetStatus returns the Status field if non-nil, zero value otherwise.

### GetStatusOk

`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderHistoryResponse) GetStatusOk() (*string, bool)`

GetStatusOk returns a tuple with the Status field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetStatus

`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderHistoryResponse) SetStatus(v string)`

SetStatus sets Status field to given value.


### GetNav

`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderHistoryResponse) GetNav() float64`

GetNav returns the Nav field if non-nil, zero value otherwise.

### GetNavOk

`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderHistoryResponse) GetNavOk() (*float64, bool)`

GetNavOk returns a tuple with the Nav field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetNav

`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderHistoryResponse) SetNav(v float64)`

SetNav sets Nav field to given value.


### SetNavNil

`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderHistoryResponse) SetNavNil(b bool)`

 SetNavNil sets the value for Nav to be an explicit nil

### UnsetNav
`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderHistoryResponse) UnsetNav()`

UnsetNav ensures that no value is present for Nav, not even an explicit nil
### GetEffectiveDate

`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderHistoryResponse) GetEffectiveDate() string`

GetEffectiveDate returns the EffectiveDate field if non-nil, zero value otherwise.

### GetEffectiveDateOk

`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderHistoryResponse) GetEffectiveDateOk() (*string, bool)`

GetEffectiveDateOk returns a tuple with the EffectiveDate field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetEffectiveDate

`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderHistoryResponse) SetEffectiveDate(v string)`

SetEffectiveDate sets EffectiveDate field to given value.


### SetEffectiveDateNil

`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderHistoryResponse) SetEffectiveDateNil(b bool)`

 SetEffectiveDateNil sets the value for EffectiveDate to be an explicit nil

### UnsetEffectiveDate
`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderHistoryResponse) UnsetEffectiveDate()`

UnsetEffectiveDate ensures that no value is present for EffectiveDate, not even an explicit nil
### GetTransactionDateTime

`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderHistoryResponse) GetTransactionDateTime() string`

GetTransactionDateTime returns the TransactionDateTime field if non-nil, zero value otherwise.

### GetTransactionDateTimeOk

`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderHistoryResponse) GetTransactionDateTimeOk() (*string, bool)`

GetTransactionDateTimeOk returns a tuple with the TransactionDateTime field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetTransactionDateTime

`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderHistoryResponse) SetTransactionDateTime(v string)`

SetTransactionDateTime sets TransactionDateTime field to given value.


### SetTransactionDateTimeNil

`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderHistoryResponse) SetTransactionDateTimeNil(b bool)`

 SetTransactionDateTimeNil sets the value for TransactionDateTime to be an explicit nil

### UnsetTransactionDateTime
`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderHistoryResponse) UnsetTransactionDateTime()`

UnsetTransactionDateTime ensures that no value is present for TransactionDateTime, not even an explicit nil
### GetUnitHolderId

`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderHistoryResponse) GetUnitHolderId() string`

GetUnitHolderId returns the UnitHolderId field if non-nil, zero value otherwise.

### GetUnitHolderIdOk

`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderHistoryResponse) GetUnitHolderIdOk() (*string, bool)`

GetUnitHolderIdOk returns a tuple with the UnitHolderId field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetUnitHolderId

`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderHistoryResponse) SetUnitHolderId(v string)`

SetUnitHolderId sets UnitHolderId field to given value.


### SetUnitHolderIdNil

`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderHistoryResponse) SetUnitHolderIdNil(b bool)`

 SetUnitHolderIdNil sets the value for UnitHolderId to be an explicit nil

### UnsetUnitHolderId
`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderHistoryResponse) UnsetUnitHolderId()`

UnsetUnitHolderId ensures that no value is present for UnitHolderId, not even an explicit nil
### GetChannel

`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderHistoryResponse) GetChannel() string`

GetChannel returns the Channel field if non-nil, zero value otherwise.

### GetChannelOk

`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderHistoryResponse) GetChannelOk() (*string, bool)`

GetChannelOk returns a tuple with the Channel field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetChannel

`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderHistoryResponse) SetChannel(v string)`

SetChannel sets Channel field to given value.


### GetAccountType

`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderHistoryResponse) GetAccountType() string`

GetAccountType returns the AccountType field if non-nil, zero value otherwise.

### GetAccountTypeOk

`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderHistoryResponse) GetAccountTypeOk() (*string, bool)`

GetAccountTypeOk returns a tuple with the AccountType field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetAccountType

`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderHistoryResponse) SetAccountType(v string)`

SetAccountType sets AccountType field to given value.


### SetAccountTypeNil

`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderHistoryResponse) SetAccountTypeNil(b bool)`

 SetAccountTypeNil sets the value for AccountType to be an explicit nil

### UnsetAccountType
`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderHistoryResponse) UnsetAccountType()`

UnsetAccountType ensures that no value is present for AccountType, not even an explicit nil
### GetSettlementDate

`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderHistoryResponse) GetSettlementDate() string`

GetSettlementDate returns the SettlementDate field if non-nil, zero value otherwise.

### GetSettlementDateOk

`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderHistoryResponse) GetSettlementDateOk() (*string, bool)`

GetSettlementDateOk returns a tuple with the SettlementDate field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetSettlementDate

`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderHistoryResponse) SetSettlementDate(v string)`

SetSettlementDate sets SettlementDate field to given value.

### HasSettlementDate

`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderHistoryResponse) HasSettlementDate() bool`

HasSettlementDate returns a boolean if a field has been set.

### SetSettlementDateNil

`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderHistoryResponse) SetSettlementDateNil(b bool)`

 SetSettlementDateNil sets the value for SettlementDate to be an explicit nil

### UnsetSettlementDate
`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderHistoryResponse) UnsetSettlementDate()`

UnsetSettlementDate ensures that no value is present for SettlementDate, not even an explicit nil
### GetOrderNo

`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderHistoryResponse) GetOrderNo() string`

GetOrderNo returns the OrderNo field if non-nil, zero value otherwise.

### GetOrderNoOk

`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderHistoryResponse) GetOrderNoOk() (*string, bool)`

GetOrderNoOk returns a tuple with the OrderNo field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetOrderNo

`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderHistoryResponse) SetOrderNo(v string)`

SetOrderNo sets OrderNo field to given value.

### HasOrderNo

`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderHistoryResponse) HasOrderNo() bool`

HasOrderNo returns a boolean if a field has been set.

### SetOrderNoNil

`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderHistoryResponse) SetOrderNoNil(b bool)`

 SetOrderNoNil sets the value for OrderNo to be an explicit nil

### UnsetOrderNo
`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderHistoryResponse) UnsetOrderNo()`

UnsetOrderNo ensures that no value is present for OrderNo, not even an explicit nil
### GetRejectReason

`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderHistoryResponse) GetRejectReason() string`

GetRejectReason returns the RejectReason field if non-nil, zero value otherwise.

### GetRejectReasonOk

`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderHistoryResponse) GetRejectReasonOk() (*string, bool)`

GetRejectReasonOk returns a tuple with the RejectReason field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetRejectReason

`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderHistoryResponse) SetRejectReason(v string)`

SetRejectReason sets RejectReason field to given value.

### HasRejectReason

`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderHistoryResponse) HasRejectReason() bool`

HasRejectReason returns a boolean if a field has been set.

### SetRejectReasonNil

`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderHistoryResponse) SetRejectReasonNil(b bool)`

 SetRejectReasonNil sets the value for RejectReason to be an explicit nil

### UnsetRejectReason
`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderHistoryResponse) UnsetRejectReason()`

UnsetRejectReason ensures that no value is present for RejectReason, not even an explicit nil
### GetTransactionLastUpdated

`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderHistoryResponse) GetTransactionLastUpdated() string`

GetTransactionLastUpdated returns the TransactionLastUpdated field if non-nil, zero value otherwise.

### GetTransactionLastUpdatedOk

`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderHistoryResponse) GetTransactionLastUpdatedOk() (*string, bool)`

GetTransactionLastUpdatedOk returns a tuple with the TransactionLastUpdated field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetTransactionLastUpdated

`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderHistoryResponse) SetTransactionLastUpdated(v string)`

SetTransactionLastUpdated sets TransactionLastUpdated field to given value.

### HasTransactionLastUpdated

`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderHistoryResponse) HasTransactionLastUpdated() bool`

HasTransactionLastUpdated returns a boolean if a field has been set.

### SetTransactionLastUpdatedNil

`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderHistoryResponse) SetTransactionLastUpdatedNil(b bool)`

 SetTransactionLastUpdatedNil sets the value for TransactionLastUpdated to be an explicit nil

### UnsetTransactionLastUpdated
`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderHistoryResponse) UnsetTransactionLastUpdated()`

UnsetTransactionLastUpdated ensures that no value is present for TransactionLastUpdated, not even an explicit nil
### GetPaymentStatus

`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderHistoryResponse) GetPaymentStatus() string`

GetPaymentStatus returns the PaymentStatus field if non-nil, zero value otherwise.

### GetPaymentStatusOk

`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderHistoryResponse) GetPaymentStatusOk() (*string, bool)`

GetPaymentStatusOk returns a tuple with the PaymentStatus field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetPaymentStatus

`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderHistoryResponse) SetPaymentStatus(v string)`

SetPaymentStatus sets PaymentStatus field to given value.

### HasPaymentStatus

`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderHistoryResponse) HasPaymentStatus() bool`

HasPaymentStatus returns a boolean if a field has been set.

### SetPaymentStatusNil

`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderHistoryResponse) SetPaymentStatusNil(b bool)`

 SetPaymentStatusNil sets the value for PaymentStatus to be an explicit nil

### UnsetPaymentStatus
`func (o *PiFinancialFundServiceAPIModelsSiriusFundOrderHistoryResponse) UnsetPaymentStatus()`

UnsetPaymentStatus ensures that no value is present for PaymentStatus, not even an explicit nil

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


