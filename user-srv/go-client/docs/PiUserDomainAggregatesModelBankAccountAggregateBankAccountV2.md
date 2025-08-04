# PiUserDomainAggregatesModelBankAccountAggregateBankAccountV2

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**RowVersion** | **string** |  | [readonly] 
**Id** | Pointer to **string** |  | [optional] [readonly] 
**AccountNo** | Pointer to **NullableString** |  | [optional] 
**HashedAccountNo** | Pointer to **NullableString** |  | [optional] 
**AccountName** | Pointer to **NullableString** |  | [optional] 
**BankCode** | Pointer to **NullableString** |  | [optional] 
**BranchCode** | Pointer to **NullableString** |  | [optional] 
**PaymentToken** | Pointer to **NullableString** |  | [optional] 
**AtsEffectiveDate** | Pointer to **NullableTime** |  | [optional] 
**Status** | Pointer to **int32** |  | [optional] 
**UserId** | Pointer to **string** |  | [optional] 
**TradeAccountBankAccountId** | Pointer to **string** |  | [optional] 
**TradeAccountBankAccounts** | Pointer to [**[]PiUserDomainAggregatesModelTradeAccountBankAccountAggregateTradeAccountBankAccount**](PiUserDomainAggregatesModelTradeAccountBankAccountAggregateTradeAccountBankAccount.md) |  | [optional] [readonly] 

## Methods

### NewPiUserDomainAggregatesModelBankAccountAggregateBankAccountV2

`func NewPiUserDomainAggregatesModelBankAccountAggregateBankAccountV2(rowVersion string, ) *PiUserDomainAggregatesModelBankAccountAggregateBankAccountV2`

NewPiUserDomainAggregatesModelBankAccountAggregateBankAccountV2 instantiates a new PiUserDomainAggregatesModelBankAccountAggregateBankAccountV2 object
This constructor will assign default values to properties that have it defined,
and makes sure properties required by API are set, but the set of arguments
will change when the set of required properties is changed

### NewPiUserDomainAggregatesModelBankAccountAggregateBankAccountV2WithDefaults

`func NewPiUserDomainAggregatesModelBankAccountAggregateBankAccountV2WithDefaults() *PiUserDomainAggregatesModelBankAccountAggregateBankAccountV2`

NewPiUserDomainAggregatesModelBankAccountAggregateBankAccountV2WithDefaults instantiates a new PiUserDomainAggregatesModelBankAccountAggregateBankAccountV2 object
This constructor will only assign default values to properties that have it defined,
but it doesn't guarantee that properties required by API are set

### GetRowVersion

`func (o *PiUserDomainAggregatesModelBankAccountAggregateBankAccountV2) GetRowVersion() string`

GetRowVersion returns the RowVersion field if non-nil, zero value otherwise.

### GetRowVersionOk

`func (o *PiUserDomainAggregatesModelBankAccountAggregateBankAccountV2) GetRowVersionOk() (*string, bool)`

GetRowVersionOk returns a tuple with the RowVersion field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetRowVersion

`func (o *PiUserDomainAggregatesModelBankAccountAggregateBankAccountV2) SetRowVersion(v string)`

SetRowVersion sets RowVersion field to given value.


### GetId

`func (o *PiUserDomainAggregatesModelBankAccountAggregateBankAccountV2) GetId() string`

GetId returns the Id field if non-nil, zero value otherwise.

### GetIdOk

`func (o *PiUserDomainAggregatesModelBankAccountAggregateBankAccountV2) GetIdOk() (*string, bool)`

GetIdOk returns a tuple with the Id field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetId

`func (o *PiUserDomainAggregatesModelBankAccountAggregateBankAccountV2) SetId(v string)`

SetId sets Id field to given value.

### HasId

`func (o *PiUserDomainAggregatesModelBankAccountAggregateBankAccountV2) HasId() bool`

HasId returns a boolean if a field has been set.

### GetAccountNo

`func (o *PiUserDomainAggregatesModelBankAccountAggregateBankAccountV2) GetAccountNo() string`

GetAccountNo returns the AccountNo field if non-nil, zero value otherwise.

### GetAccountNoOk

`func (o *PiUserDomainAggregatesModelBankAccountAggregateBankAccountV2) GetAccountNoOk() (*string, bool)`

GetAccountNoOk returns a tuple with the AccountNo field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetAccountNo

`func (o *PiUserDomainAggregatesModelBankAccountAggregateBankAccountV2) SetAccountNo(v string)`

SetAccountNo sets AccountNo field to given value.

### HasAccountNo

`func (o *PiUserDomainAggregatesModelBankAccountAggregateBankAccountV2) HasAccountNo() bool`

HasAccountNo returns a boolean if a field has been set.

### SetAccountNoNil

`func (o *PiUserDomainAggregatesModelBankAccountAggregateBankAccountV2) SetAccountNoNil(b bool)`

 SetAccountNoNil sets the value for AccountNo to be an explicit nil

### UnsetAccountNo
`func (o *PiUserDomainAggregatesModelBankAccountAggregateBankAccountV2) UnsetAccountNo()`

UnsetAccountNo ensures that no value is present for AccountNo, not even an explicit nil
### GetHashedAccountNo

`func (o *PiUserDomainAggregatesModelBankAccountAggregateBankAccountV2) GetHashedAccountNo() string`

GetHashedAccountNo returns the HashedAccountNo field if non-nil, zero value otherwise.

### GetHashedAccountNoOk

`func (o *PiUserDomainAggregatesModelBankAccountAggregateBankAccountV2) GetHashedAccountNoOk() (*string, bool)`

GetHashedAccountNoOk returns a tuple with the HashedAccountNo field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetHashedAccountNo

`func (o *PiUserDomainAggregatesModelBankAccountAggregateBankAccountV2) SetHashedAccountNo(v string)`

SetHashedAccountNo sets HashedAccountNo field to given value.

### HasHashedAccountNo

`func (o *PiUserDomainAggregatesModelBankAccountAggregateBankAccountV2) HasHashedAccountNo() bool`

HasHashedAccountNo returns a boolean if a field has been set.

### SetHashedAccountNoNil

`func (o *PiUserDomainAggregatesModelBankAccountAggregateBankAccountV2) SetHashedAccountNoNil(b bool)`

 SetHashedAccountNoNil sets the value for HashedAccountNo to be an explicit nil

### UnsetHashedAccountNo
`func (o *PiUserDomainAggregatesModelBankAccountAggregateBankAccountV2) UnsetHashedAccountNo()`

UnsetHashedAccountNo ensures that no value is present for HashedAccountNo, not even an explicit nil
### GetAccountName

`func (o *PiUserDomainAggregatesModelBankAccountAggregateBankAccountV2) GetAccountName() string`

GetAccountName returns the AccountName field if non-nil, zero value otherwise.

### GetAccountNameOk

`func (o *PiUserDomainAggregatesModelBankAccountAggregateBankAccountV2) GetAccountNameOk() (*string, bool)`

GetAccountNameOk returns a tuple with the AccountName field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetAccountName

`func (o *PiUserDomainAggregatesModelBankAccountAggregateBankAccountV2) SetAccountName(v string)`

SetAccountName sets AccountName field to given value.

### HasAccountName

`func (o *PiUserDomainAggregatesModelBankAccountAggregateBankAccountV2) HasAccountName() bool`

HasAccountName returns a boolean if a field has been set.

### SetAccountNameNil

`func (o *PiUserDomainAggregatesModelBankAccountAggregateBankAccountV2) SetAccountNameNil(b bool)`

 SetAccountNameNil sets the value for AccountName to be an explicit nil

### UnsetAccountName
`func (o *PiUserDomainAggregatesModelBankAccountAggregateBankAccountV2) UnsetAccountName()`

UnsetAccountName ensures that no value is present for AccountName, not even an explicit nil
### GetBankCode

`func (o *PiUserDomainAggregatesModelBankAccountAggregateBankAccountV2) GetBankCode() string`

GetBankCode returns the BankCode field if non-nil, zero value otherwise.

### GetBankCodeOk

`func (o *PiUserDomainAggregatesModelBankAccountAggregateBankAccountV2) GetBankCodeOk() (*string, bool)`

GetBankCodeOk returns a tuple with the BankCode field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetBankCode

`func (o *PiUserDomainAggregatesModelBankAccountAggregateBankAccountV2) SetBankCode(v string)`

SetBankCode sets BankCode field to given value.

### HasBankCode

`func (o *PiUserDomainAggregatesModelBankAccountAggregateBankAccountV2) HasBankCode() bool`

HasBankCode returns a boolean if a field has been set.

### SetBankCodeNil

`func (o *PiUserDomainAggregatesModelBankAccountAggregateBankAccountV2) SetBankCodeNil(b bool)`

 SetBankCodeNil sets the value for BankCode to be an explicit nil

### UnsetBankCode
`func (o *PiUserDomainAggregatesModelBankAccountAggregateBankAccountV2) UnsetBankCode()`

UnsetBankCode ensures that no value is present for BankCode, not even an explicit nil
### GetBranchCode

`func (o *PiUserDomainAggregatesModelBankAccountAggregateBankAccountV2) GetBranchCode() string`

GetBranchCode returns the BranchCode field if non-nil, zero value otherwise.

### GetBranchCodeOk

`func (o *PiUserDomainAggregatesModelBankAccountAggregateBankAccountV2) GetBranchCodeOk() (*string, bool)`

GetBranchCodeOk returns a tuple with the BranchCode field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetBranchCode

`func (o *PiUserDomainAggregatesModelBankAccountAggregateBankAccountV2) SetBranchCode(v string)`

SetBranchCode sets BranchCode field to given value.

### HasBranchCode

`func (o *PiUserDomainAggregatesModelBankAccountAggregateBankAccountV2) HasBranchCode() bool`

HasBranchCode returns a boolean if a field has been set.

### SetBranchCodeNil

`func (o *PiUserDomainAggregatesModelBankAccountAggregateBankAccountV2) SetBranchCodeNil(b bool)`

 SetBranchCodeNil sets the value for BranchCode to be an explicit nil

### UnsetBranchCode
`func (o *PiUserDomainAggregatesModelBankAccountAggregateBankAccountV2) UnsetBranchCode()`

UnsetBranchCode ensures that no value is present for BranchCode, not even an explicit nil
### GetPaymentToken

`func (o *PiUserDomainAggregatesModelBankAccountAggregateBankAccountV2) GetPaymentToken() string`

GetPaymentToken returns the PaymentToken field if non-nil, zero value otherwise.

### GetPaymentTokenOk

`func (o *PiUserDomainAggregatesModelBankAccountAggregateBankAccountV2) GetPaymentTokenOk() (*string, bool)`

GetPaymentTokenOk returns a tuple with the PaymentToken field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetPaymentToken

`func (o *PiUserDomainAggregatesModelBankAccountAggregateBankAccountV2) SetPaymentToken(v string)`

SetPaymentToken sets PaymentToken field to given value.

### HasPaymentToken

`func (o *PiUserDomainAggregatesModelBankAccountAggregateBankAccountV2) HasPaymentToken() bool`

HasPaymentToken returns a boolean if a field has been set.

### SetPaymentTokenNil

`func (o *PiUserDomainAggregatesModelBankAccountAggregateBankAccountV2) SetPaymentTokenNil(b bool)`

 SetPaymentTokenNil sets the value for PaymentToken to be an explicit nil

### UnsetPaymentToken
`func (o *PiUserDomainAggregatesModelBankAccountAggregateBankAccountV2) UnsetPaymentToken()`

UnsetPaymentToken ensures that no value is present for PaymentToken, not even an explicit nil
### GetAtsEffectiveDate

`func (o *PiUserDomainAggregatesModelBankAccountAggregateBankAccountV2) GetAtsEffectiveDate() time.Time`

GetAtsEffectiveDate returns the AtsEffectiveDate field if non-nil, zero value otherwise.

### GetAtsEffectiveDateOk

`func (o *PiUserDomainAggregatesModelBankAccountAggregateBankAccountV2) GetAtsEffectiveDateOk() (*time.Time, bool)`

GetAtsEffectiveDateOk returns a tuple with the AtsEffectiveDate field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetAtsEffectiveDate

`func (o *PiUserDomainAggregatesModelBankAccountAggregateBankAccountV2) SetAtsEffectiveDate(v time.Time)`

SetAtsEffectiveDate sets AtsEffectiveDate field to given value.

### HasAtsEffectiveDate

`func (o *PiUserDomainAggregatesModelBankAccountAggregateBankAccountV2) HasAtsEffectiveDate() bool`

HasAtsEffectiveDate returns a boolean if a field has been set.

### SetAtsEffectiveDateNil

`func (o *PiUserDomainAggregatesModelBankAccountAggregateBankAccountV2) SetAtsEffectiveDateNil(b bool)`

 SetAtsEffectiveDateNil sets the value for AtsEffectiveDate to be an explicit nil

### UnsetAtsEffectiveDate
`func (o *PiUserDomainAggregatesModelBankAccountAggregateBankAccountV2) UnsetAtsEffectiveDate()`

UnsetAtsEffectiveDate ensures that no value is present for AtsEffectiveDate, not even an explicit nil
### GetStatus

`func (o *PiUserDomainAggregatesModelBankAccountAggregateBankAccountV2) GetStatus() int32`

GetStatus returns the Status field if non-nil, zero value otherwise.

### GetStatusOk

`func (o *PiUserDomainAggregatesModelBankAccountAggregateBankAccountV2) GetStatusOk() (*int32, bool)`

GetStatusOk returns a tuple with the Status field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetStatus

`func (o *PiUserDomainAggregatesModelBankAccountAggregateBankAccountV2) SetStatus(v int32)`

SetStatus sets Status field to given value.

### HasStatus

`func (o *PiUserDomainAggregatesModelBankAccountAggregateBankAccountV2) HasStatus() bool`

HasStatus returns a boolean if a field has been set.

### GetUserId

`func (o *PiUserDomainAggregatesModelBankAccountAggregateBankAccountV2) GetUserId() string`

GetUserId returns the UserId field if non-nil, zero value otherwise.

### GetUserIdOk

`func (o *PiUserDomainAggregatesModelBankAccountAggregateBankAccountV2) GetUserIdOk() (*string, bool)`

GetUserIdOk returns a tuple with the UserId field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetUserId

`func (o *PiUserDomainAggregatesModelBankAccountAggregateBankAccountV2) SetUserId(v string)`

SetUserId sets UserId field to given value.

### HasUserId

`func (o *PiUserDomainAggregatesModelBankAccountAggregateBankAccountV2) HasUserId() bool`

HasUserId returns a boolean if a field has been set.

### GetTradeAccountBankAccountId

`func (o *PiUserDomainAggregatesModelBankAccountAggregateBankAccountV2) GetTradeAccountBankAccountId() string`

GetTradeAccountBankAccountId returns the TradeAccountBankAccountId field if non-nil, zero value otherwise.

### GetTradeAccountBankAccountIdOk

`func (o *PiUserDomainAggregatesModelBankAccountAggregateBankAccountV2) GetTradeAccountBankAccountIdOk() (*string, bool)`

GetTradeAccountBankAccountIdOk returns a tuple with the TradeAccountBankAccountId field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetTradeAccountBankAccountId

`func (o *PiUserDomainAggregatesModelBankAccountAggregateBankAccountV2) SetTradeAccountBankAccountId(v string)`

SetTradeAccountBankAccountId sets TradeAccountBankAccountId field to given value.

### HasTradeAccountBankAccountId

`func (o *PiUserDomainAggregatesModelBankAccountAggregateBankAccountV2) HasTradeAccountBankAccountId() bool`

HasTradeAccountBankAccountId returns a boolean if a field has been set.

### GetTradeAccountBankAccounts

`func (o *PiUserDomainAggregatesModelBankAccountAggregateBankAccountV2) GetTradeAccountBankAccounts() []PiUserDomainAggregatesModelTradeAccountBankAccountAggregateTradeAccountBankAccount`

GetTradeAccountBankAccounts returns the TradeAccountBankAccounts field if non-nil, zero value otherwise.

### GetTradeAccountBankAccountsOk

`func (o *PiUserDomainAggregatesModelBankAccountAggregateBankAccountV2) GetTradeAccountBankAccountsOk() (*[]PiUserDomainAggregatesModelTradeAccountBankAccountAggregateTradeAccountBankAccount, bool)`

GetTradeAccountBankAccountsOk returns a tuple with the TradeAccountBankAccounts field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetTradeAccountBankAccounts

`func (o *PiUserDomainAggregatesModelBankAccountAggregateBankAccountV2) SetTradeAccountBankAccounts(v []PiUserDomainAggregatesModelTradeAccountBankAccountAggregateTradeAccountBankAccount)`

SetTradeAccountBankAccounts sets TradeAccountBankAccounts field to given value.

### HasTradeAccountBankAccounts

`func (o *PiUserDomainAggregatesModelBankAccountAggregateBankAccountV2) HasTradeAccountBankAccounts() bool`

HasTradeAccountBankAccounts returns a boolean if a field has been set.

### SetTradeAccountBankAccountsNil

`func (o *PiUserDomainAggregatesModelBankAccountAggregateBankAccountV2) SetTradeAccountBankAccountsNil(b bool)`

 SetTradeAccountBankAccountsNil sets the value for TradeAccountBankAccounts to be an explicit nil

### UnsetTradeAccountBankAccounts
`func (o *PiUserDomainAggregatesModelBankAccountAggregateBankAccountV2) UnsetTradeAccountBankAccounts()`

UnsetTradeAccountBankAccounts ensures that no value is present for TradeAccountBankAccounts, not even an explicit nil

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


