# PiFinancialClientFundConnextModelFundOrder

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**SellAllUnitFlag** | Pointer to **NullableString** |  | [optional] 
**Status** | Pointer to **string** |  | [optional] 
**TransactionId** | Pointer to **NullableString** |  | [optional] 
**SaOrderReferenceNo** | Pointer to **NullableString** |  | [optional] 
**OrderType** | Pointer to **NullableString** |  | [optional] 
**AccountId** | Pointer to **NullableString** |  | [optional] 
**UnitholderId** | Pointer to **NullableString** |  | [optional] 
**FundCode** | Pointer to **NullableString** |  | [optional] 
**RedemptionType** | Pointer to **NullableString** |  | [optional] 
**Unit** | Pointer to **NullableFloat64** |  | [optional] 
**Amount** | Pointer to **NullableFloat64** |  | [optional] 
**TransactionDateTime** | Pointer to **NullableString** |  | [optional] 
**EffectiveDate** | Pointer to **NullableString** |  | [optional] 
**SettlementDate** | Pointer to **NullableString** |  | [optional] 
**AmcOrderReferenceNo** | Pointer to **NullableString** |  | [optional] 
**AllottedUnit** | Pointer to **NullableFloat64** |  | [optional] 
**AllottedAmount** | Pointer to **NullableFloat64** |  | [optional] 
**AllottedNAV** | Pointer to **NullableFloat64** |  | [optional] 
**AllotmentDate** | Pointer to **NullableString** |  | [optional] 
**Fee** | Pointer to **NullableFloat64** |  | [optional] 
**TransactionLastUpdated** | Pointer to **NullableString** |  | [optional] 
**PaymentType** | Pointer to **NullableString** |  | [optional] 
**BankCode** | Pointer to **NullableString** |  | [optional] 
**BankAccount** | Pointer to **NullableString** |  | [optional] 
**Channel** | Pointer to **NullableString** |  | [optional] 
**IcLicense** | Pointer to **NullableString** |  | [optional] 
**BranchNo** | Pointer to **NullableString** |  | [optional] 
**ForceEntry** | Pointer to **NullableString** |  | [optional] 
**SettlementBankCode** | Pointer to **NullableString** |  | [optional] 
**SettlementBankAccount** | Pointer to **NullableString** |  | [optional] 
**ChqBranch** | Pointer to **NullableString** |  | [optional] 
**RejectReason** | Pointer to **NullableString** |  | [optional] 
**NavDate** | Pointer to **NullableString** |  | [optional] 
**CollateralAccount** | Pointer to **NullableString** |  | [optional] 
**AccountType** | Pointer to **NullableString** |  | [optional] 
**RecurringOrderId** | Pointer to **NullableString** |  | [optional] 
**PaymentStatus** | Pointer to **NullableString** |  | [optional] 
**PaymentProcessingType** | Pointer to **NullableString** |  | [optional] 
**SaRecurringOrderRefNo** | Pointer to **NullableString** |  | [optional] 
**CrcApprovalCode** | Pointer to **NullableString** |  | [optional] 
**PointCode** | Pointer to **NullableString** |  | [optional] 
**CounterUnitholderId** | Pointer to **NullableString** |  | [optional] 
**CounterFundCode** | Pointer to **NullableString** |  | [optional] 
**XwtReferenceNo** | Pointer to **NullableString** |  | [optional] 
**OriginalTransactionId** | Pointer to **NullableString** |  | [optional] 
**LmtsAdlsFee** | Pointer to **NullableString** |  | [optional] 
**LmtsLiquidityFee** | Pointer to **NullableString** |  | [optional] 

## Methods

### NewPiFinancialClientFundConnextModelFundOrder

`func NewPiFinancialClientFundConnextModelFundOrder() *PiFinancialClientFundConnextModelFundOrder`

NewPiFinancialClientFundConnextModelFundOrder instantiates a new PiFinancialClientFundConnextModelFundOrder object
This constructor will assign default values to properties that have it defined,
and makes sure properties required by API are set, but the set of arguments
will change when the set of required properties is changed

### NewPiFinancialClientFundConnextModelFundOrderWithDefaults

`func NewPiFinancialClientFundConnextModelFundOrderWithDefaults() *PiFinancialClientFundConnextModelFundOrder`

NewPiFinancialClientFundConnextModelFundOrderWithDefaults instantiates a new PiFinancialClientFundConnextModelFundOrder object
This constructor will only assign default values to properties that have it defined,
but it doesn't guarantee that properties required by API are set

### GetSellAllUnitFlag

`func (o *PiFinancialClientFundConnextModelFundOrder) GetSellAllUnitFlag() string`

GetSellAllUnitFlag returns the SellAllUnitFlag field if non-nil, zero value otherwise.

### GetSellAllUnitFlagOk

`func (o *PiFinancialClientFundConnextModelFundOrder) GetSellAllUnitFlagOk() (*string, bool)`

GetSellAllUnitFlagOk returns a tuple with the SellAllUnitFlag field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetSellAllUnitFlag

`func (o *PiFinancialClientFundConnextModelFundOrder) SetSellAllUnitFlag(v string)`

SetSellAllUnitFlag sets SellAllUnitFlag field to given value.

### HasSellAllUnitFlag

`func (o *PiFinancialClientFundConnextModelFundOrder) HasSellAllUnitFlag() bool`

HasSellAllUnitFlag returns a boolean if a field has been set.

### SetSellAllUnitFlagNil

`func (o *PiFinancialClientFundConnextModelFundOrder) SetSellAllUnitFlagNil(b bool)`

 SetSellAllUnitFlagNil sets the value for SellAllUnitFlag to be an explicit nil

### UnsetSellAllUnitFlag
`func (o *PiFinancialClientFundConnextModelFundOrder) UnsetSellAllUnitFlag()`

UnsetSellAllUnitFlag ensures that no value is present for SellAllUnitFlag, not even an explicit nil
### GetStatus

`func (o *PiFinancialClientFundConnextModelFundOrder) GetStatus() string`

GetStatus returns the Status field if non-nil, zero value otherwise.

### GetStatusOk

`func (o *PiFinancialClientFundConnextModelFundOrder) GetStatusOk() (*string, bool)`

GetStatusOk returns a tuple with the Status field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetStatus

`func (o *PiFinancialClientFundConnextModelFundOrder) SetStatus(v string)`

SetStatus sets Status field to given value.

### HasStatus

`func (o *PiFinancialClientFundConnextModelFundOrder) HasStatus() bool`

HasStatus returns a boolean if a field has been set.

### GetTransactionId

`func (o *PiFinancialClientFundConnextModelFundOrder) GetTransactionId() string`

GetTransactionId returns the TransactionId field if non-nil, zero value otherwise.

### GetTransactionIdOk

`func (o *PiFinancialClientFundConnextModelFundOrder) GetTransactionIdOk() (*string, bool)`

GetTransactionIdOk returns a tuple with the TransactionId field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetTransactionId

`func (o *PiFinancialClientFundConnextModelFundOrder) SetTransactionId(v string)`

SetTransactionId sets TransactionId field to given value.

### HasTransactionId

`func (o *PiFinancialClientFundConnextModelFundOrder) HasTransactionId() bool`

HasTransactionId returns a boolean if a field has been set.

### SetTransactionIdNil

`func (o *PiFinancialClientFundConnextModelFundOrder) SetTransactionIdNil(b bool)`

 SetTransactionIdNil sets the value for TransactionId to be an explicit nil

### UnsetTransactionId
`func (o *PiFinancialClientFundConnextModelFundOrder) UnsetTransactionId()`

UnsetTransactionId ensures that no value is present for TransactionId, not even an explicit nil
### GetSaOrderReferenceNo

`func (o *PiFinancialClientFundConnextModelFundOrder) GetSaOrderReferenceNo() string`

GetSaOrderReferenceNo returns the SaOrderReferenceNo field if non-nil, zero value otherwise.

### GetSaOrderReferenceNoOk

`func (o *PiFinancialClientFundConnextModelFundOrder) GetSaOrderReferenceNoOk() (*string, bool)`

GetSaOrderReferenceNoOk returns a tuple with the SaOrderReferenceNo field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetSaOrderReferenceNo

`func (o *PiFinancialClientFundConnextModelFundOrder) SetSaOrderReferenceNo(v string)`

SetSaOrderReferenceNo sets SaOrderReferenceNo field to given value.

### HasSaOrderReferenceNo

`func (o *PiFinancialClientFundConnextModelFundOrder) HasSaOrderReferenceNo() bool`

HasSaOrderReferenceNo returns a boolean if a field has been set.

### SetSaOrderReferenceNoNil

`func (o *PiFinancialClientFundConnextModelFundOrder) SetSaOrderReferenceNoNil(b bool)`

 SetSaOrderReferenceNoNil sets the value for SaOrderReferenceNo to be an explicit nil

### UnsetSaOrderReferenceNo
`func (o *PiFinancialClientFundConnextModelFundOrder) UnsetSaOrderReferenceNo()`

UnsetSaOrderReferenceNo ensures that no value is present for SaOrderReferenceNo, not even an explicit nil
### GetOrderType

`func (o *PiFinancialClientFundConnextModelFundOrder) GetOrderType() string`

GetOrderType returns the OrderType field if non-nil, zero value otherwise.

### GetOrderTypeOk

`func (o *PiFinancialClientFundConnextModelFundOrder) GetOrderTypeOk() (*string, bool)`

GetOrderTypeOk returns a tuple with the OrderType field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetOrderType

`func (o *PiFinancialClientFundConnextModelFundOrder) SetOrderType(v string)`

SetOrderType sets OrderType field to given value.

### HasOrderType

`func (o *PiFinancialClientFundConnextModelFundOrder) HasOrderType() bool`

HasOrderType returns a boolean if a field has been set.

### SetOrderTypeNil

`func (o *PiFinancialClientFundConnextModelFundOrder) SetOrderTypeNil(b bool)`

 SetOrderTypeNil sets the value for OrderType to be an explicit nil

### UnsetOrderType
`func (o *PiFinancialClientFundConnextModelFundOrder) UnsetOrderType()`

UnsetOrderType ensures that no value is present for OrderType, not even an explicit nil
### GetAccountId

`func (o *PiFinancialClientFundConnextModelFundOrder) GetAccountId() string`

GetAccountId returns the AccountId field if non-nil, zero value otherwise.

### GetAccountIdOk

`func (o *PiFinancialClientFundConnextModelFundOrder) GetAccountIdOk() (*string, bool)`

GetAccountIdOk returns a tuple with the AccountId field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetAccountId

`func (o *PiFinancialClientFundConnextModelFundOrder) SetAccountId(v string)`

SetAccountId sets AccountId field to given value.

### HasAccountId

`func (o *PiFinancialClientFundConnextModelFundOrder) HasAccountId() bool`

HasAccountId returns a boolean if a field has been set.

### SetAccountIdNil

`func (o *PiFinancialClientFundConnextModelFundOrder) SetAccountIdNil(b bool)`

 SetAccountIdNil sets the value for AccountId to be an explicit nil

### UnsetAccountId
`func (o *PiFinancialClientFundConnextModelFundOrder) UnsetAccountId()`

UnsetAccountId ensures that no value is present for AccountId, not even an explicit nil
### GetUnitholderId

`func (o *PiFinancialClientFundConnextModelFundOrder) GetUnitholderId() string`

GetUnitholderId returns the UnitholderId field if non-nil, zero value otherwise.

### GetUnitholderIdOk

`func (o *PiFinancialClientFundConnextModelFundOrder) GetUnitholderIdOk() (*string, bool)`

GetUnitholderIdOk returns a tuple with the UnitholderId field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetUnitholderId

`func (o *PiFinancialClientFundConnextModelFundOrder) SetUnitholderId(v string)`

SetUnitholderId sets UnitholderId field to given value.

### HasUnitholderId

`func (o *PiFinancialClientFundConnextModelFundOrder) HasUnitholderId() bool`

HasUnitholderId returns a boolean if a field has been set.

### SetUnitholderIdNil

`func (o *PiFinancialClientFundConnextModelFundOrder) SetUnitholderIdNil(b bool)`

 SetUnitholderIdNil sets the value for UnitholderId to be an explicit nil

### UnsetUnitholderId
`func (o *PiFinancialClientFundConnextModelFundOrder) UnsetUnitholderId()`

UnsetUnitholderId ensures that no value is present for UnitholderId, not even an explicit nil
### GetFundCode

`func (o *PiFinancialClientFundConnextModelFundOrder) GetFundCode() string`

GetFundCode returns the FundCode field if non-nil, zero value otherwise.

### GetFundCodeOk

`func (o *PiFinancialClientFundConnextModelFundOrder) GetFundCodeOk() (*string, bool)`

GetFundCodeOk returns a tuple with the FundCode field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetFundCode

`func (o *PiFinancialClientFundConnextModelFundOrder) SetFundCode(v string)`

SetFundCode sets FundCode field to given value.

### HasFundCode

`func (o *PiFinancialClientFundConnextModelFundOrder) HasFundCode() bool`

HasFundCode returns a boolean if a field has been set.

### SetFundCodeNil

`func (o *PiFinancialClientFundConnextModelFundOrder) SetFundCodeNil(b bool)`

 SetFundCodeNil sets the value for FundCode to be an explicit nil

### UnsetFundCode
`func (o *PiFinancialClientFundConnextModelFundOrder) UnsetFundCode()`

UnsetFundCode ensures that no value is present for FundCode, not even an explicit nil
### GetRedemptionType

`func (o *PiFinancialClientFundConnextModelFundOrder) GetRedemptionType() string`

GetRedemptionType returns the RedemptionType field if non-nil, zero value otherwise.

### GetRedemptionTypeOk

`func (o *PiFinancialClientFundConnextModelFundOrder) GetRedemptionTypeOk() (*string, bool)`

GetRedemptionTypeOk returns a tuple with the RedemptionType field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetRedemptionType

`func (o *PiFinancialClientFundConnextModelFundOrder) SetRedemptionType(v string)`

SetRedemptionType sets RedemptionType field to given value.

### HasRedemptionType

`func (o *PiFinancialClientFundConnextModelFundOrder) HasRedemptionType() bool`

HasRedemptionType returns a boolean if a field has been set.

### SetRedemptionTypeNil

`func (o *PiFinancialClientFundConnextModelFundOrder) SetRedemptionTypeNil(b bool)`

 SetRedemptionTypeNil sets the value for RedemptionType to be an explicit nil

### UnsetRedemptionType
`func (o *PiFinancialClientFundConnextModelFundOrder) UnsetRedemptionType()`

UnsetRedemptionType ensures that no value is present for RedemptionType, not even an explicit nil
### GetUnit

`func (o *PiFinancialClientFundConnextModelFundOrder) GetUnit() float64`

GetUnit returns the Unit field if non-nil, zero value otherwise.

### GetUnitOk

`func (o *PiFinancialClientFundConnextModelFundOrder) GetUnitOk() (*float64, bool)`

GetUnitOk returns a tuple with the Unit field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetUnit

`func (o *PiFinancialClientFundConnextModelFundOrder) SetUnit(v float64)`

SetUnit sets Unit field to given value.

### HasUnit

`func (o *PiFinancialClientFundConnextModelFundOrder) HasUnit() bool`

HasUnit returns a boolean if a field has been set.

### SetUnitNil

`func (o *PiFinancialClientFundConnextModelFundOrder) SetUnitNil(b bool)`

 SetUnitNil sets the value for Unit to be an explicit nil

### UnsetUnit
`func (o *PiFinancialClientFundConnextModelFundOrder) UnsetUnit()`

UnsetUnit ensures that no value is present for Unit, not even an explicit nil
### GetAmount

`func (o *PiFinancialClientFundConnextModelFundOrder) GetAmount() float64`

GetAmount returns the Amount field if non-nil, zero value otherwise.

### GetAmountOk

`func (o *PiFinancialClientFundConnextModelFundOrder) GetAmountOk() (*float64, bool)`

GetAmountOk returns a tuple with the Amount field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetAmount

`func (o *PiFinancialClientFundConnextModelFundOrder) SetAmount(v float64)`

SetAmount sets Amount field to given value.

### HasAmount

`func (o *PiFinancialClientFundConnextModelFundOrder) HasAmount() bool`

HasAmount returns a boolean if a field has been set.

### SetAmountNil

`func (o *PiFinancialClientFundConnextModelFundOrder) SetAmountNil(b bool)`

 SetAmountNil sets the value for Amount to be an explicit nil

### UnsetAmount
`func (o *PiFinancialClientFundConnextModelFundOrder) UnsetAmount()`

UnsetAmount ensures that no value is present for Amount, not even an explicit nil
### GetTransactionDateTime

`func (o *PiFinancialClientFundConnextModelFundOrder) GetTransactionDateTime() string`

GetTransactionDateTime returns the TransactionDateTime field if non-nil, zero value otherwise.

### GetTransactionDateTimeOk

`func (o *PiFinancialClientFundConnextModelFundOrder) GetTransactionDateTimeOk() (*string, bool)`

GetTransactionDateTimeOk returns a tuple with the TransactionDateTime field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetTransactionDateTime

`func (o *PiFinancialClientFundConnextModelFundOrder) SetTransactionDateTime(v string)`

SetTransactionDateTime sets TransactionDateTime field to given value.

### HasTransactionDateTime

`func (o *PiFinancialClientFundConnextModelFundOrder) HasTransactionDateTime() bool`

HasTransactionDateTime returns a boolean if a field has been set.

### SetTransactionDateTimeNil

`func (o *PiFinancialClientFundConnextModelFundOrder) SetTransactionDateTimeNil(b bool)`

 SetTransactionDateTimeNil sets the value for TransactionDateTime to be an explicit nil

### UnsetTransactionDateTime
`func (o *PiFinancialClientFundConnextModelFundOrder) UnsetTransactionDateTime()`

UnsetTransactionDateTime ensures that no value is present for TransactionDateTime, not even an explicit nil
### GetEffectiveDate

`func (o *PiFinancialClientFundConnextModelFundOrder) GetEffectiveDate() string`

GetEffectiveDate returns the EffectiveDate field if non-nil, zero value otherwise.

### GetEffectiveDateOk

`func (o *PiFinancialClientFundConnextModelFundOrder) GetEffectiveDateOk() (*string, bool)`

GetEffectiveDateOk returns a tuple with the EffectiveDate field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetEffectiveDate

`func (o *PiFinancialClientFundConnextModelFundOrder) SetEffectiveDate(v string)`

SetEffectiveDate sets EffectiveDate field to given value.

### HasEffectiveDate

`func (o *PiFinancialClientFundConnextModelFundOrder) HasEffectiveDate() bool`

HasEffectiveDate returns a boolean if a field has been set.

### SetEffectiveDateNil

`func (o *PiFinancialClientFundConnextModelFundOrder) SetEffectiveDateNil(b bool)`

 SetEffectiveDateNil sets the value for EffectiveDate to be an explicit nil

### UnsetEffectiveDate
`func (o *PiFinancialClientFundConnextModelFundOrder) UnsetEffectiveDate()`

UnsetEffectiveDate ensures that no value is present for EffectiveDate, not even an explicit nil
### GetSettlementDate

`func (o *PiFinancialClientFundConnextModelFundOrder) GetSettlementDate() string`

GetSettlementDate returns the SettlementDate field if non-nil, zero value otherwise.

### GetSettlementDateOk

`func (o *PiFinancialClientFundConnextModelFundOrder) GetSettlementDateOk() (*string, bool)`

GetSettlementDateOk returns a tuple with the SettlementDate field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetSettlementDate

`func (o *PiFinancialClientFundConnextModelFundOrder) SetSettlementDate(v string)`

SetSettlementDate sets SettlementDate field to given value.

### HasSettlementDate

`func (o *PiFinancialClientFundConnextModelFundOrder) HasSettlementDate() bool`

HasSettlementDate returns a boolean if a field has been set.

### SetSettlementDateNil

`func (o *PiFinancialClientFundConnextModelFundOrder) SetSettlementDateNil(b bool)`

 SetSettlementDateNil sets the value for SettlementDate to be an explicit nil

### UnsetSettlementDate
`func (o *PiFinancialClientFundConnextModelFundOrder) UnsetSettlementDate()`

UnsetSettlementDate ensures that no value is present for SettlementDate, not even an explicit nil
### GetAmcOrderReferenceNo

`func (o *PiFinancialClientFundConnextModelFundOrder) GetAmcOrderReferenceNo() string`

GetAmcOrderReferenceNo returns the AmcOrderReferenceNo field if non-nil, zero value otherwise.

### GetAmcOrderReferenceNoOk

`func (o *PiFinancialClientFundConnextModelFundOrder) GetAmcOrderReferenceNoOk() (*string, bool)`

GetAmcOrderReferenceNoOk returns a tuple with the AmcOrderReferenceNo field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetAmcOrderReferenceNo

`func (o *PiFinancialClientFundConnextModelFundOrder) SetAmcOrderReferenceNo(v string)`

SetAmcOrderReferenceNo sets AmcOrderReferenceNo field to given value.

### HasAmcOrderReferenceNo

`func (o *PiFinancialClientFundConnextModelFundOrder) HasAmcOrderReferenceNo() bool`

HasAmcOrderReferenceNo returns a boolean if a field has been set.

### SetAmcOrderReferenceNoNil

`func (o *PiFinancialClientFundConnextModelFundOrder) SetAmcOrderReferenceNoNil(b bool)`

 SetAmcOrderReferenceNoNil sets the value for AmcOrderReferenceNo to be an explicit nil

### UnsetAmcOrderReferenceNo
`func (o *PiFinancialClientFundConnextModelFundOrder) UnsetAmcOrderReferenceNo()`

UnsetAmcOrderReferenceNo ensures that no value is present for AmcOrderReferenceNo, not even an explicit nil
### GetAllottedUnit

`func (o *PiFinancialClientFundConnextModelFundOrder) GetAllottedUnit() float64`

GetAllottedUnit returns the AllottedUnit field if non-nil, zero value otherwise.

### GetAllottedUnitOk

`func (o *PiFinancialClientFundConnextModelFundOrder) GetAllottedUnitOk() (*float64, bool)`

GetAllottedUnitOk returns a tuple with the AllottedUnit field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetAllottedUnit

`func (o *PiFinancialClientFundConnextModelFundOrder) SetAllottedUnit(v float64)`

SetAllottedUnit sets AllottedUnit field to given value.

### HasAllottedUnit

`func (o *PiFinancialClientFundConnextModelFundOrder) HasAllottedUnit() bool`

HasAllottedUnit returns a boolean if a field has been set.

### SetAllottedUnitNil

`func (o *PiFinancialClientFundConnextModelFundOrder) SetAllottedUnitNil(b bool)`

 SetAllottedUnitNil sets the value for AllottedUnit to be an explicit nil

### UnsetAllottedUnit
`func (o *PiFinancialClientFundConnextModelFundOrder) UnsetAllottedUnit()`

UnsetAllottedUnit ensures that no value is present for AllottedUnit, not even an explicit nil
### GetAllottedAmount

`func (o *PiFinancialClientFundConnextModelFundOrder) GetAllottedAmount() float64`

GetAllottedAmount returns the AllottedAmount field if non-nil, zero value otherwise.

### GetAllottedAmountOk

`func (o *PiFinancialClientFundConnextModelFundOrder) GetAllottedAmountOk() (*float64, bool)`

GetAllottedAmountOk returns a tuple with the AllottedAmount field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetAllottedAmount

`func (o *PiFinancialClientFundConnextModelFundOrder) SetAllottedAmount(v float64)`

SetAllottedAmount sets AllottedAmount field to given value.

### HasAllottedAmount

`func (o *PiFinancialClientFundConnextModelFundOrder) HasAllottedAmount() bool`

HasAllottedAmount returns a boolean if a field has been set.

### SetAllottedAmountNil

`func (o *PiFinancialClientFundConnextModelFundOrder) SetAllottedAmountNil(b bool)`

 SetAllottedAmountNil sets the value for AllottedAmount to be an explicit nil

### UnsetAllottedAmount
`func (o *PiFinancialClientFundConnextModelFundOrder) UnsetAllottedAmount()`

UnsetAllottedAmount ensures that no value is present for AllottedAmount, not even an explicit nil
### GetAllottedNAV

`func (o *PiFinancialClientFundConnextModelFundOrder) GetAllottedNAV() float64`

GetAllottedNAV returns the AllottedNAV field if non-nil, zero value otherwise.

### GetAllottedNAVOk

`func (o *PiFinancialClientFundConnextModelFundOrder) GetAllottedNAVOk() (*float64, bool)`

GetAllottedNAVOk returns a tuple with the AllottedNAV field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetAllottedNAV

`func (o *PiFinancialClientFundConnextModelFundOrder) SetAllottedNAV(v float64)`

SetAllottedNAV sets AllottedNAV field to given value.

### HasAllottedNAV

`func (o *PiFinancialClientFundConnextModelFundOrder) HasAllottedNAV() bool`

HasAllottedNAV returns a boolean if a field has been set.

### SetAllottedNAVNil

`func (o *PiFinancialClientFundConnextModelFundOrder) SetAllottedNAVNil(b bool)`

 SetAllottedNAVNil sets the value for AllottedNAV to be an explicit nil

### UnsetAllottedNAV
`func (o *PiFinancialClientFundConnextModelFundOrder) UnsetAllottedNAV()`

UnsetAllottedNAV ensures that no value is present for AllottedNAV, not even an explicit nil
### GetAllotmentDate

`func (o *PiFinancialClientFundConnextModelFundOrder) GetAllotmentDate() string`

GetAllotmentDate returns the AllotmentDate field if non-nil, zero value otherwise.

### GetAllotmentDateOk

`func (o *PiFinancialClientFundConnextModelFundOrder) GetAllotmentDateOk() (*string, bool)`

GetAllotmentDateOk returns a tuple with the AllotmentDate field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetAllotmentDate

`func (o *PiFinancialClientFundConnextModelFundOrder) SetAllotmentDate(v string)`

SetAllotmentDate sets AllotmentDate field to given value.

### HasAllotmentDate

`func (o *PiFinancialClientFundConnextModelFundOrder) HasAllotmentDate() bool`

HasAllotmentDate returns a boolean if a field has been set.

### SetAllotmentDateNil

`func (o *PiFinancialClientFundConnextModelFundOrder) SetAllotmentDateNil(b bool)`

 SetAllotmentDateNil sets the value for AllotmentDate to be an explicit nil

### UnsetAllotmentDate
`func (o *PiFinancialClientFundConnextModelFundOrder) UnsetAllotmentDate()`

UnsetAllotmentDate ensures that no value is present for AllotmentDate, not even an explicit nil
### GetFee

`func (o *PiFinancialClientFundConnextModelFundOrder) GetFee() float64`

GetFee returns the Fee field if non-nil, zero value otherwise.

### GetFeeOk

`func (o *PiFinancialClientFundConnextModelFundOrder) GetFeeOk() (*float64, bool)`

GetFeeOk returns a tuple with the Fee field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetFee

`func (o *PiFinancialClientFundConnextModelFundOrder) SetFee(v float64)`

SetFee sets Fee field to given value.

### HasFee

`func (o *PiFinancialClientFundConnextModelFundOrder) HasFee() bool`

HasFee returns a boolean if a field has been set.

### SetFeeNil

`func (o *PiFinancialClientFundConnextModelFundOrder) SetFeeNil(b bool)`

 SetFeeNil sets the value for Fee to be an explicit nil

### UnsetFee
`func (o *PiFinancialClientFundConnextModelFundOrder) UnsetFee()`

UnsetFee ensures that no value is present for Fee, not even an explicit nil
### GetTransactionLastUpdated

`func (o *PiFinancialClientFundConnextModelFundOrder) GetTransactionLastUpdated() string`

GetTransactionLastUpdated returns the TransactionLastUpdated field if non-nil, zero value otherwise.

### GetTransactionLastUpdatedOk

`func (o *PiFinancialClientFundConnextModelFundOrder) GetTransactionLastUpdatedOk() (*string, bool)`

GetTransactionLastUpdatedOk returns a tuple with the TransactionLastUpdated field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetTransactionLastUpdated

`func (o *PiFinancialClientFundConnextModelFundOrder) SetTransactionLastUpdated(v string)`

SetTransactionLastUpdated sets TransactionLastUpdated field to given value.

### HasTransactionLastUpdated

`func (o *PiFinancialClientFundConnextModelFundOrder) HasTransactionLastUpdated() bool`

HasTransactionLastUpdated returns a boolean if a field has been set.

### SetTransactionLastUpdatedNil

`func (o *PiFinancialClientFundConnextModelFundOrder) SetTransactionLastUpdatedNil(b bool)`

 SetTransactionLastUpdatedNil sets the value for TransactionLastUpdated to be an explicit nil

### UnsetTransactionLastUpdated
`func (o *PiFinancialClientFundConnextModelFundOrder) UnsetTransactionLastUpdated()`

UnsetTransactionLastUpdated ensures that no value is present for TransactionLastUpdated, not even an explicit nil
### GetPaymentType

`func (o *PiFinancialClientFundConnextModelFundOrder) GetPaymentType() string`

GetPaymentType returns the PaymentType field if non-nil, zero value otherwise.

### GetPaymentTypeOk

`func (o *PiFinancialClientFundConnextModelFundOrder) GetPaymentTypeOk() (*string, bool)`

GetPaymentTypeOk returns a tuple with the PaymentType field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetPaymentType

`func (o *PiFinancialClientFundConnextModelFundOrder) SetPaymentType(v string)`

SetPaymentType sets PaymentType field to given value.

### HasPaymentType

`func (o *PiFinancialClientFundConnextModelFundOrder) HasPaymentType() bool`

HasPaymentType returns a boolean if a field has been set.

### SetPaymentTypeNil

`func (o *PiFinancialClientFundConnextModelFundOrder) SetPaymentTypeNil(b bool)`

 SetPaymentTypeNil sets the value for PaymentType to be an explicit nil

### UnsetPaymentType
`func (o *PiFinancialClientFundConnextModelFundOrder) UnsetPaymentType()`

UnsetPaymentType ensures that no value is present for PaymentType, not even an explicit nil
### GetBankCode

`func (o *PiFinancialClientFundConnextModelFundOrder) GetBankCode() string`

GetBankCode returns the BankCode field if non-nil, zero value otherwise.

### GetBankCodeOk

`func (o *PiFinancialClientFundConnextModelFundOrder) GetBankCodeOk() (*string, bool)`

GetBankCodeOk returns a tuple with the BankCode field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetBankCode

`func (o *PiFinancialClientFundConnextModelFundOrder) SetBankCode(v string)`

SetBankCode sets BankCode field to given value.

### HasBankCode

`func (o *PiFinancialClientFundConnextModelFundOrder) HasBankCode() bool`

HasBankCode returns a boolean if a field has been set.

### SetBankCodeNil

`func (o *PiFinancialClientFundConnextModelFundOrder) SetBankCodeNil(b bool)`

 SetBankCodeNil sets the value for BankCode to be an explicit nil

### UnsetBankCode
`func (o *PiFinancialClientFundConnextModelFundOrder) UnsetBankCode()`

UnsetBankCode ensures that no value is present for BankCode, not even an explicit nil
### GetBankAccount

`func (o *PiFinancialClientFundConnextModelFundOrder) GetBankAccount() string`

GetBankAccount returns the BankAccount field if non-nil, zero value otherwise.

### GetBankAccountOk

`func (o *PiFinancialClientFundConnextModelFundOrder) GetBankAccountOk() (*string, bool)`

GetBankAccountOk returns a tuple with the BankAccount field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetBankAccount

`func (o *PiFinancialClientFundConnextModelFundOrder) SetBankAccount(v string)`

SetBankAccount sets BankAccount field to given value.

### HasBankAccount

`func (o *PiFinancialClientFundConnextModelFundOrder) HasBankAccount() bool`

HasBankAccount returns a boolean if a field has been set.

### SetBankAccountNil

`func (o *PiFinancialClientFundConnextModelFundOrder) SetBankAccountNil(b bool)`

 SetBankAccountNil sets the value for BankAccount to be an explicit nil

### UnsetBankAccount
`func (o *PiFinancialClientFundConnextModelFundOrder) UnsetBankAccount()`

UnsetBankAccount ensures that no value is present for BankAccount, not even an explicit nil
### GetChannel

`func (o *PiFinancialClientFundConnextModelFundOrder) GetChannel() string`

GetChannel returns the Channel field if non-nil, zero value otherwise.

### GetChannelOk

`func (o *PiFinancialClientFundConnextModelFundOrder) GetChannelOk() (*string, bool)`

GetChannelOk returns a tuple with the Channel field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetChannel

`func (o *PiFinancialClientFundConnextModelFundOrder) SetChannel(v string)`

SetChannel sets Channel field to given value.

### HasChannel

`func (o *PiFinancialClientFundConnextModelFundOrder) HasChannel() bool`

HasChannel returns a boolean if a field has been set.

### SetChannelNil

`func (o *PiFinancialClientFundConnextModelFundOrder) SetChannelNil(b bool)`

 SetChannelNil sets the value for Channel to be an explicit nil

### UnsetChannel
`func (o *PiFinancialClientFundConnextModelFundOrder) UnsetChannel()`

UnsetChannel ensures that no value is present for Channel, not even an explicit nil
### GetIcLicense

`func (o *PiFinancialClientFundConnextModelFundOrder) GetIcLicense() string`

GetIcLicense returns the IcLicense field if non-nil, zero value otherwise.

### GetIcLicenseOk

`func (o *PiFinancialClientFundConnextModelFundOrder) GetIcLicenseOk() (*string, bool)`

GetIcLicenseOk returns a tuple with the IcLicense field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetIcLicense

`func (o *PiFinancialClientFundConnextModelFundOrder) SetIcLicense(v string)`

SetIcLicense sets IcLicense field to given value.

### HasIcLicense

`func (o *PiFinancialClientFundConnextModelFundOrder) HasIcLicense() bool`

HasIcLicense returns a boolean if a field has been set.

### SetIcLicenseNil

`func (o *PiFinancialClientFundConnextModelFundOrder) SetIcLicenseNil(b bool)`

 SetIcLicenseNil sets the value for IcLicense to be an explicit nil

### UnsetIcLicense
`func (o *PiFinancialClientFundConnextModelFundOrder) UnsetIcLicense()`

UnsetIcLicense ensures that no value is present for IcLicense, not even an explicit nil
### GetBranchNo

`func (o *PiFinancialClientFundConnextModelFundOrder) GetBranchNo() string`

GetBranchNo returns the BranchNo field if non-nil, zero value otherwise.

### GetBranchNoOk

`func (o *PiFinancialClientFundConnextModelFundOrder) GetBranchNoOk() (*string, bool)`

GetBranchNoOk returns a tuple with the BranchNo field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetBranchNo

`func (o *PiFinancialClientFundConnextModelFundOrder) SetBranchNo(v string)`

SetBranchNo sets BranchNo field to given value.

### HasBranchNo

`func (o *PiFinancialClientFundConnextModelFundOrder) HasBranchNo() bool`

HasBranchNo returns a boolean if a field has been set.

### SetBranchNoNil

`func (o *PiFinancialClientFundConnextModelFundOrder) SetBranchNoNil(b bool)`

 SetBranchNoNil sets the value for BranchNo to be an explicit nil

### UnsetBranchNo
`func (o *PiFinancialClientFundConnextModelFundOrder) UnsetBranchNo()`

UnsetBranchNo ensures that no value is present for BranchNo, not even an explicit nil
### GetForceEntry

`func (o *PiFinancialClientFundConnextModelFundOrder) GetForceEntry() string`

GetForceEntry returns the ForceEntry field if non-nil, zero value otherwise.

### GetForceEntryOk

`func (o *PiFinancialClientFundConnextModelFundOrder) GetForceEntryOk() (*string, bool)`

GetForceEntryOk returns a tuple with the ForceEntry field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetForceEntry

`func (o *PiFinancialClientFundConnextModelFundOrder) SetForceEntry(v string)`

SetForceEntry sets ForceEntry field to given value.

### HasForceEntry

`func (o *PiFinancialClientFundConnextModelFundOrder) HasForceEntry() bool`

HasForceEntry returns a boolean if a field has been set.

### SetForceEntryNil

`func (o *PiFinancialClientFundConnextModelFundOrder) SetForceEntryNil(b bool)`

 SetForceEntryNil sets the value for ForceEntry to be an explicit nil

### UnsetForceEntry
`func (o *PiFinancialClientFundConnextModelFundOrder) UnsetForceEntry()`

UnsetForceEntry ensures that no value is present for ForceEntry, not even an explicit nil
### GetSettlementBankCode

`func (o *PiFinancialClientFundConnextModelFundOrder) GetSettlementBankCode() string`

GetSettlementBankCode returns the SettlementBankCode field if non-nil, zero value otherwise.

### GetSettlementBankCodeOk

`func (o *PiFinancialClientFundConnextModelFundOrder) GetSettlementBankCodeOk() (*string, bool)`

GetSettlementBankCodeOk returns a tuple with the SettlementBankCode field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetSettlementBankCode

`func (o *PiFinancialClientFundConnextModelFundOrder) SetSettlementBankCode(v string)`

SetSettlementBankCode sets SettlementBankCode field to given value.

### HasSettlementBankCode

`func (o *PiFinancialClientFundConnextModelFundOrder) HasSettlementBankCode() bool`

HasSettlementBankCode returns a boolean if a field has been set.

### SetSettlementBankCodeNil

`func (o *PiFinancialClientFundConnextModelFundOrder) SetSettlementBankCodeNil(b bool)`

 SetSettlementBankCodeNil sets the value for SettlementBankCode to be an explicit nil

### UnsetSettlementBankCode
`func (o *PiFinancialClientFundConnextModelFundOrder) UnsetSettlementBankCode()`

UnsetSettlementBankCode ensures that no value is present for SettlementBankCode, not even an explicit nil
### GetSettlementBankAccount

`func (o *PiFinancialClientFundConnextModelFundOrder) GetSettlementBankAccount() string`

GetSettlementBankAccount returns the SettlementBankAccount field if non-nil, zero value otherwise.

### GetSettlementBankAccountOk

`func (o *PiFinancialClientFundConnextModelFundOrder) GetSettlementBankAccountOk() (*string, bool)`

GetSettlementBankAccountOk returns a tuple with the SettlementBankAccount field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetSettlementBankAccount

`func (o *PiFinancialClientFundConnextModelFundOrder) SetSettlementBankAccount(v string)`

SetSettlementBankAccount sets SettlementBankAccount field to given value.

### HasSettlementBankAccount

`func (o *PiFinancialClientFundConnextModelFundOrder) HasSettlementBankAccount() bool`

HasSettlementBankAccount returns a boolean if a field has been set.

### SetSettlementBankAccountNil

`func (o *PiFinancialClientFundConnextModelFundOrder) SetSettlementBankAccountNil(b bool)`

 SetSettlementBankAccountNil sets the value for SettlementBankAccount to be an explicit nil

### UnsetSettlementBankAccount
`func (o *PiFinancialClientFundConnextModelFundOrder) UnsetSettlementBankAccount()`

UnsetSettlementBankAccount ensures that no value is present for SettlementBankAccount, not even an explicit nil
### GetChqBranch

`func (o *PiFinancialClientFundConnextModelFundOrder) GetChqBranch() string`

GetChqBranch returns the ChqBranch field if non-nil, zero value otherwise.

### GetChqBranchOk

`func (o *PiFinancialClientFundConnextModelFundOrder) GetChqBranchOk() (*string, bool)`

GetChqBranchOk returns a tuple with the ChqBranch field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetChqBranch

`func (o *PiFinancialClientFundConnextModelFundOrder) SetChqBranch(v string)`

SetChqBranch sets ChqBranch field to given value.

### HasChqBranch

`func (o *PiFinancialClientFundConnextModelFundOrder) HasChqBranch() bool`

HasChqBranch returns a boolean if a field has been set.

### SetChqBranchNil

`func (o *PiFinancialClientFundConnextModelFundOrder) SetChqBranchNil(b bool)`

 SetChqBranchNil sets the value for ChqBranch to be an explicit nil

### UnsetChqBranch
`func (o *PiFinancialClientFundConnextModelFundOrder) UnsetChqBranch()`

UnsetChqBranch ensures that no value is present for ChqBranch, not even an explicit nil
### GetRejectReason

`func (o *PiFinancialClientFundConnextModelFundOrder) GetRejectReason() string`

GetRejectReason returns the RejectReason field if non-nil, zero value otherwise.

### GetRejectReasonOk

`func (o *PiFinancialClientFundConnextModelFundOrder) GetRejectReasonOk() (*string, bool)`

GetRejectReasonOk returns a tuple with the RejectReason field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetRejectReason

`func (o *PiFinancialClientFundConnextModelFundOrder) SetRejectReason(v string)`

SetRejectReason sets RejectReason field to given value.

### HasRejectReason

`func (o *PiFinancialClientFundConnextModelFundOrder) HasRejectReason() bool`

HasRejectReason returns a boolean if a field has been set.

### SetRejectReasonNil

`func (o *PiFinancialClientFundConnextModelFundOrder) SetRejectReasonNil(b bool)`

 SetRejectReasonNil sets the value for RejectReason to be an explicit nil

### UnsetRejectReason
`func (o *PiFinancialClientFundConnextModelFundOrder) UnsetRejectReason()`

UnsetRejectReason ensures that no value is present for RejectReason, not even an explicit nil
### GetNavDate

`func (o *PiFinancialClientFundConnextModelFundOrder) GetNavDate() string`

GetNavDate returns the NavDate field if non-nil, zero value otherwise.

### GetNavDateOk

`func (o *PiFinancialClientFundConnextModelFundOrder) GetNavDateOk() (*string, bool)`

GetNavDateOk returns a tuple with the NavDate field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetNavDate

`func (o *PiFinancialClientFundConnextModelFundOrder) SetNavDate(v string)`

SetNavDate sets NavDate field to given value.

### HasNavDate

`func (o *PiFinancialClientFundConnextModelFundOrder) HasNavDate() bool`

HasNavDate returns a boolean if a field has been set.

### SetNavDateNil

`func (o *PiFinancialClientFundConnextModelFundOrder) SetNavDateNil(b bool)`

 SetNavDateNil sets the value for NavDate to be an explicit nil

### UnsetNavDate
`func (o *PiFinancialClientFundConnextModelFundOrder) UnsetNavDate()`

UnsetNavDate ensures that no value is present for NavDate, not even an explicit nil
### GetCollateralAccount

`func (o *PiFinancialClientFundConnextModelFundOrder) GetCollateralAccount() string`

GetCollateralAccount returns the CollateralAccount field if non-nil, zero value otherwise.

### GetCollateralAccountOk

`func (o *PiFinancialClientFundConnextModelFundOrder) GetCollateralAccountOk() (*string, bool)`

GetCollateralAccountOk returns a tuple with the CollateralAccount field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetCollateralAccount

`func (o *PiFinancialClientFundConnextModelFundOrder) SetCollateralAccount(v string)`

SetCollateralAccount sets CollateralAccount field to given value.

### HasCollateralAccount

`func (o *PiFinancialClientFundConnextModelFundOrder) HasCollateralAccount() bool`

HasCollateralAccount returns a boolean if a field has been set.

### SetCollateralAccountNil

`func (o *PiFinancialClientFundConnextModelFundOrder) SetCollateralAccountNil(b bool)`

 SetCollateralAccountNil sets the value for CollateralAccount to be an explicit nil

### UnsetCollateralAccount
`func (o *PiFinancialClientFundConnextModelFundOrder) UnsetCollateralAccount()`

UnsetCollateralAccount ensures that no value is present for CollateralAccount, not even an explicit nil
### GetAccountType

`func (o *PiFinancialClientFundConnextModelFundOrder) GetAccountType() string`

GetAccountType returns the AccountType field if non-nil, zero value otherwise.

### GetAccountTypeOk

`func (o *PiFinancialClientFundConnextModelFundOrder) GetAccountTypeOk() (*string, bool)`

GetAccountTypeOk returns a tuple with the AccountType field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetAccountType

`func (o *PiFinancialClientFundConnextModelFundOrder) SetAccountType(v string)`

SetAccountType sets AccountType field to given value.

### HasAccountType

`func (o *PiFinancialClientFundConnextModelFundOrder) HasAccountType() bool`

HasAccountType returns a boolean if a field has been set.

### SetAccountTypeNil

`func (o *PiFinancialClientFundConnextModelFundOrder) SetAccountTypeNil(b bool)`

 SetAccountTypeNil sets the value for AccountType to be an explicit nil

### UnsetAccountType
`func (o *PiFinancialClientFundConnextModelFundOrder) UnsetAccountType()`

UnsetAccountType ensures that no value is present for AccountType, not even an explicit nil
### GetRecurringOrderId

`func (o *PiFinancialClientFundConnextModelFundOrder) GetRecurringOrderId() string`

GetRecurringOrderId returns the RecurringOrderId field if non-nil, zero value otherwise.

### GetRecurringOrderIdOk

`func (o *PiFinancialClientFundConnextModelFundOrder) GetRecurringOrderIdOk() (*string, bool)`

GetRecurringOrderIdOk returns a tuple with the RecurringOrderId field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetRecurringOrderId

`func (o *PiFinancialClientFundConnextModelFundOrder) SetRecurringOrderId(v string)`

SetRecurringOrderId sets RecurringOrderId field to given value.

### HasRecurringOrderId

`func (o *PiFinancialClientFundConnextModelFundOrder) HasRecurringOrderId() bool`

HasRecurringOrderId returns a boolean if a field has been set.

### SetRecurringOrderIdNil

`func (o *PiFinancialClientFundConnextModelFundOrder) SetRecurringOrderIdNil(b bool)`

 SetRecurringOrderIdNil sets the value for RecurringOrderId to be an explicit nil

### UnsetRecurringOrderId
`func (o *PiFinancialClientFundConnextModelFundOrder) UnsetRecurringOrderId()`

UnsetRecurringOrderId ensures that no value is present for RecurringOrderId, not even an explicit nil
### GetPaymentStatus

`func (o *PiFinancialClientFundConnextModelFundOrder) GetPaymentStatus() string`

GetPaymentStatus returns the PaymentStatus field if non-nil, zero value otherwise.

### GetPaymentStatusOk

`func (o *PiFinancialClientFundConnextModelFundOrder) GetPaymentStatusOk() (*string, bool)`

GetPaymentStatusOk returns a tuple with the PaymentStatus field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetPaymentStatus

`func (o *PiFinancialClientFundConnextModelFundOrder) SetPaymentStatus(v string)`

SetPaymentStatus sets PaymentStatus field to given value.

### HasPaymentStatus

`func (o *PiFinancialClientFundConnextModelFundOrder) HasPaymentStatus() bool`

HasPaymentStatus returns a boolean if a field has been set.

### SetPaymentStatusNil

`func (o *PiFinancialClientFundConnextModelFundOrder) SetPaymentStatusNil(b bool)`

 SetPaymentStatusNil sets the value for PaymentStatus to be an explicit nil

### UnsetPaymentStatus
`func (o *PiFinancialClientFundConnextModelFundOrder) UnsetPaymentStatus()`

UnsetPaymentStatus ensures that no value is present for PaymentStatus, not even an explicit nil
### GetPaymentProcessingType

`func (o *PiFinancialClientFundConnextModelFundOrder) GetPaymentProcessingType() string`

GetPaymentProcessingType returns the PaymentProcessingType field if non-nil, zero value otherwise.

### GetPaymentProcessingTypeOk

`func (o *PiFinancialClientFundConnextModelFundOrder) GetPaymentProcessingTypeOk() (*string, bool)`

GetPaymentProcessingTypeOk returns a tuple with the PaymentProcessingType field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetPaymentProcessingType

`func (o *PiFinancialClientFundConnextModelFundOrder) SetPaymentProcessingType(v string)`

SetPaymentProcessingType sets PaymentProcessingType field to given value.

### HasPaymentProcessingType

`func (o *PiFinancialClientFundConnextModelFundOrder) HasPaymentProcessingType() bool`

HasPaymentProcessingType returns a boolean if a field has been set.

### SetPaymentProcessingTypeNil

`func (o *PiFinancialClientFundConnextModelFundOrder) SetPaymentProcessingTypeNil(b bool)`

 SetPaymentProcessingTypeNil sets the value for PaymentProcessingType to be an explicit nil

### UnsetPaymentProcessingType
`func (o *PiFinancialClientFundConnextModelFundOrder) UnsetPaymentProcessingType()`

UnsetPaymentProcessingType ensures that no value is present for PaymentProcessingType, not even an explicit nil
### GetSaRecurringOrderRefNo

`func (o *PiFinancialClientFundConnextModelFundOrder) GetSaRecurringOrderRefNo() string`

GetSaRecurringOrderRefNo returns the SaRecurringOrderRefNo field if non-nil, zero value otherwise.

### GetSaRecurringOrderRefNoOk

`func (o *PiFinancialClientFundConnextModelFundOrder) GetSaRecurringOrderRefNoOk() (*string, bool)`

GetSaRecurringOrderRefNoOk returns a tuple with the SaRecurringOrderRefNo field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetSaRecurringOrderRefNo

`func (o *PiFinancialClientFundConnextModelFundOrder) SetSaRecurringOrderRefNo(v string)`

SetSaRecurringOrderRefNo sets SaRecurringOrderRefNo field to given value.

### HasSaRecurringOrderRefNo

`func (o *PiFinancialClientFundConnextModelFundOrder) HasSaRecurringOrderRefNo() bool`

HasSaRecurringOrderRefNo returns a boolean if a field has been set.

### SetSaRecurringOrderRefNoNil

`func (o *PiFinancialClientFundConnextModelFundOrder) SetSaRecurringOrderRefNoNil(b bool)`

 SetSaRecurringOrderRefNoNil sets the value for SaRecurringOrderRefNo to be an explicit nil

### UnsetSaRecurringOrderRefNo
`func (o *PiFinancialClientFundConnextModelFundOrder) UnsetSaRecurringOrderRefNo()`

UnsetSaRecurringOrderRefNo ensures that no value is present for SaRecurringOrderRefNo, not even an explicit nil
### GetCrcApprovalCode

`func (o *PiFinancialClientFundConnextModelFundOrder) GetCrcApprovalCode() string`

GetCrcApprovalCode returns the CrcApprovalCode field if non-nil, zero value otherwise.

### GetCrcApprovalCodeOk

`func (o *PiFinancialClientFundConnextModelFundOrder) GetCrcApprovalCodeOk() (*string, bool)`

GetCrcApprovalCodeOk returns a tuple with the CrcApprovalCode field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetCrcApprovalCode

`func (o *PiFinancialClientFundConnextModelFundOrder) SetCrcApprovalCode(v string)`

SetCrcApprovalCode sets CrcApprovalCode field to given value.

### HasCrcApprovalCode

`func (o *PiFinancialClientFundConnextModelFundOrder) HasCrcApprovalCode() bool`

HasCrcApprovalCode returns a boolean if a field has been set.

### SetCrcApprovalCodeNil

`func (o *PiFinancialClientFundConnextModelFundOrder) SetCrcApprovalCodeNil(b bool)`

 SetCrcApprovalCodeNil sets the value for CrcApprovalCode to be an explicit nil

### UnsetCrcApprovalCode
`func (o *PiFinancialClientFundConnextModelFundOrder) UnsetCrcApprovalCode()`

UnsetCrcApprovalCode ensures that no value is present for CrcApprovalCode, not even an explicit nil
### GetPointCode

`func (o *PiFinancialClientFundConnextModelFundOrder) GetPointCode() string`

GetPointCode returns the PointCode field if non-nil, zero value otherwise.

### GetPointCodeOk

`func (o *PiFinancialClientFundConnextModelFundOrder) GetPointCodeOk() (*string, bool)`

GetPointCodeOk returns a tuple with the PointCode field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetPointCode

`func (o *PiFinancialClientFundConnextModelFundOrder) SetPointCode(v string)`

SetPointCode sets PointCode field to given value.

### HasPointCode

`func (o *PiFinancialClientFundConnextModelFundOrder) HasPointCode() bool`

HasPointCode returns a boolean if a field has been set.

### SetPointCodeNil

`func (o *PiFinancialClientFundConnextModelFundOrder) SetPointCodeNil(b bool)`

 SetPointCodeNil sets the value for PointCode to be an explicit nil

### UnsetPointCode
`func (o *PiFinancialClientFundConnextModelFundOrder) UnsetPointCode()`

UnsetPointCode ensures that no value is present for PointCode, not even an explicit nil
### GetCounterUnitholderId

`func (o *PiFinancialClientFundConnextModelFundOrder) GetCounterUnitholderId() string`

GetCounterUnitholderId returns the CounterUnitholderId field if non-nil, zero value otherwise.

### GetCounterUnitholderIdOk

`func (o *PiFinancialClientFundConnextModelFundOrder) GetCounterUnitholderIdOk() (*string, bool)`

GetCounterUnitholderIdOk returns a tuple with the CounterUnitholderId field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetCounterUnitholderId

`func (o *PiFinancialClientFundConnextModelFundOrder) SetCounterUnitholderId(v string)`

SetCounterUnitholderId sets CounterUnitholderId field to given value.

### HasCounterUnitholderId

`func (o *PiFinancialClientFundConnextModelFundOrder) HasCounterUnitholderId() bool`

HasCounterUnitholderId returns a boolean if a field has been set.

### SetCounterUnitholderIdNil

`func (o *PiFinancialClientFundConnextModelFundOrder) SetCounterUnitholderIdNil(b bool)`

 SetCounterUnitholderIdNil sets the value for CounterUnitholderId to be an explicit nil

### UnsetCounterUnitholderId
`func (o *PiFinancialClientFundConnextModelFundOrder) UnsetCounterUnitholderId()`

UnsetCounterUnitholderId ensures that no value is present for CounterUnitholderId, not even an explicit nil
### GetCounterFundCode

`func (o *PiFinancialClientFundConnextModelFundOrder) GetCounterFundCode() string`

GetCounterFundCode returns the CounterFundCode field if non-nil, zero value otherwise.

### GetCounterFundCodeOk

`func (o *PiFinancialClientFundConnextModelFundOrder) GetCounterFundCodeOk() (*string, bool)`

GetCounterFundCodeOk returns a tuple with the CounterFundCode field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetCounterFundCode

`func (o *PiFinancialClientFundConnextModelFundOrder) SetCounterFundCode(v string)`

SetCounterFundCode sets CounterFundCode field to given value.

### HasCounterFundCode

`func (o *PiFinancialClientFundConnextModelFundOrder) HasCounterFundCode() bool`

HasCounterFundCode returns a boolean if a field has been set.

### SetCounterFundCodeNil

`func (o *PiFinancialClientFundConnextModelFundOrder) SetCounterFundCodeNil(b bool)`

 SetCounterFundCodeNil sets the value for CounterFundCode to be an explicit nil

### UnsetCounterFundCode
`func (o *PiFinancialClientFundConnextModelFundOrder) UnsetCounterFundCode()`

UnsetCounterFundCode ensures that no value is present for CounterFundCode, not even an explicit nil
### GetXwtReferenceNo

`func (o *PiFinancialClientFundConnextModelFundOrder) GetXwtReferenceNo() string`

GetXwtReferenceNo returns the XwtReferenceNo field if non-nil, zero value otherwise.

### GetXwtReferenceNoOk

`func (o *PiFinancialClientFundConnextModelFundOrder) GetXwtReferenceNoOk() (*string, bool)`

GetXwtReferenceNoOk returns a tuple with the XwtReferenceNo field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetXwtReferenceNo

`func (o *PiFinancialClientFundConnextModelFundOrder) SetXwtReferenceNo(v string)`

SetXwtReferenceNo sets XwtReferenceNo field to given value.

### HasXwtReferenceNo

`func (o *PiFinancialClientFundConnextModelFundOrder) HasXwtReferenceNo() bool`

HasXwtReferenceNo returns a boolean if a field has been set.

### SetXwtReferenceNoNil

`func (o *PiFinancialClientFundConnextModelFundOrder) SetXwtReferenceNoNil(b bool)`

 SetXwtReferenceNoNil sets the value for XwtReferenceNo to be an explicit nil

### UnsetXwtReferenceNo
`func (o *PiFinancialClientFundConnextModelFundOrder) UnsetXwtReferenceNo()`

UnsetXwtReferenceNo ensures that no value is present for XwtReferenceNo, not even an explicit nil
### GetOriginalTransactionId

`func (o *PiFinancialClientFundConnextModelFundOrder) GetOriginalTransactionId() string`

GetOriginalTransactionId returns the OriginalTransactionId field if non-nil, zero value otherwise.

### GetOriginalTransactionIdOk

`func (o *PiFinancialClientFundConnextModelFundOrder) GetOriginalTransactionIdOk() (*string, bool)`

GetOriginalTransactionIdOk returns a tuple with the OriginalTransactionId field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetOriginalTransactionId

`func (o *PiFinancialClientFundConnextModelFundOrder) SetOriginalTransactionId(v string)`

SetOriginalTransactionId sets OriginalTransactionId field to given value.

### HasOriginalTransactionId

`func (o *PiFinancialClientFundConnextModelFundOrder) HasOriginalTransactionId() bool`

HasOriginalTransactionId returns a boolean if a field has been set.

### SetOriginalTransactionIdNil

`func (o *PiFinancialClientFundConnextModelFundOrder) SetOriginalTransactionIdNil(b bool)`

 SetOriginalTransactionIdNil sets the value for OriginalTransactionId to be an explicit nil

### UnsetOriginalTransactionId
`func (o *PiFinancialClientFundConnextModelFundOrder) UnsetOriginalTransactionId()`

UnsetOriginalTransactionId ensures that no value is present for OriginalTransactionId, not even an explicit nil
### GetLmtsAdlsFee

`func (o *PiFinancialClientFundConnextModelFundOrder) GetLmtsAdlsFee() string`

GetLmtsAdlsFee returns the LmtsAdlsFee field if non-nil, zero value otherwise.

### GetLmtsAdlsFeeOk

`func (o *PiFinancialClientFundConnextModelFundOrder) GetLmtsAdlsFeeOk() (*string, bool)`

GetLmtsAdlsFeeOk returns a tuple with the LmtsAdlsFee field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetLmtsAdlsFee

`func (o *PiFinancialClientFundConnextModelFundOrder) SetLmtsAdlsFee(v string)`

SetLmtsAdlsFee sets LmtsAdlsFee field to given value.

### HasLmtsAdlsFee

`func (o *PiFinancialClientFundConnextModelFundOrder) HasLmtsAdlsFee() bool`

HasLmtsAdlsFee returns a boolean if a field has been set.

### SetLmtsAdlsFeeNil

`func (o *PiFinancialClientFundConnextModelFundOrder) SetLmtsAdlsFeeNil(b bool)`

 SetLmtsAdlsFeeNil sets the value for LmtsAdlsFee to be an explicit nil

### UnsetLmtsAdlsFee
`func (o *PiFinancialClientFundConnextModelFundOrder) UnsetLmtsAdlsFee()`

UnsetLmtsAdlsFee ensures that no value is present for LmtsAdlsFee, not even an explicit nil
### GetLmtsLiquidityFee

`func (o *PiFinancialClientFundConnextModelFundOrder) GetLmtsLiquidityFee() string`

GetLmtsLiquidityFee returns the LmtsLiquidityFee field if non-nil, zero value otherwise.

### GetLmtsLiquidityFeeOk

`func (o *PiFinancialClientFundConnextModelFundOrder) GetLmtsLiquidityFeeOk() (*string, bool)`

GetLmtsLiquidityFeeOk returns a tuple with the LmtsLiquidityFee field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetLmtsLiquidityFee

`func (o *PiFinancialClientFundConnextModelFundOrder) SetLmtsLiquidityFee(v string)`

SetLmtsLiquidityFee sets LmtsLiquidityFee field to given value.

### HasLmtsLiquidityFee

`func (o *PiFinancialClientFundConnextModelFundOrder) HasLmtsLiquidityFee() bool`

HasLmtsLiquidityFee returns a boolean if a field has been set.

### SetLmtsLiquidityFeeNil

`func (o *PiFinancialClientFundConnextModelFundOrder) SetLmtsLiquidityFeeNil(b bool)`

 SetLmtsLiquidityFeeNil sets the value for LmtsLiquidityFee to be an explicit nil

### UnsetLmtsLiquidityFee
`func (o *PiFinancialClientFundConnextModelFundOrder) UnsetLmtsLiquidityFee()`

UnsetLmtsLiquidityFee ensures that no value is present for LmtsLiquidityFee, not even an explicit nil

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


