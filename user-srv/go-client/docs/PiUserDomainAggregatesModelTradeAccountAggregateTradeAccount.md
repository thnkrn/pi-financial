# PiUserDomainAggregatesModelTradeAccountAggregateTradeAccount

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**RowVersion** | **string** |  | [readonly] 
**Id** | Pointer to **string** |  | [optional] [readonly] 
**AccountNumber** | Pointer to **NullableString** |  | [optional] 
**AccountType** | Pointer to **int32** |  | [optional] 
**AccountTypeCode** | Pointer to **NullableString** |  | [optional] 
**ExchangeMarketId** | Pointer to **NullableString** |  | [optional] 
**AccountStatus** | Pointer to **NullableString** |  | [optional] 
**CreditLine** | Pointer to **float64** |  | [optional] 
**CreditLineCurrency** | Pointer to **NullableString** |  | [optional] 
**EffectiveDate** | Pointer to **NullableString** |  | [optional] 
**EndDate** | Pointer to **NullableString** |  | [optional] 
**MarketingId** | Pointer to **NullableString** |  | [optional] 
**SaleLicense** | Pointer to **NullableString** |  | [optional] 
**OpenDate** | Pointer to **NullableString** |  | [optional] 
**UserAccountId** | Pointer to **NullableString** |  | [optional] 
**CreatedAt** | Pointer to **time.Time** |  | [optional] 
**UpdatedAt** | Pointer to **NullableTime** |  | [optional] 
**ExternalAccounts** | Pointer to [**[]PiUserDomainAggregatesModelExternalAccountAggregateExternalAccount**](PiUserDomainAggregatesModelExternalAccountAggregateExternalAccount.md) |  | [optional] [readonly] 
**TradeAccountBankAccounts** | Pointer to [**[]PiUserDomainAggregatesModelTradeAccountBankAccountAggregateTradeAccountBankAccount**](PiUserDomainAggregatesModelTradeAccountBankAccountAggregateTradeAccountBankAccount.md) |  | [optional] [readonly] 

## Methods

### NewPiUserDomainAggregatesModelTradeAccountAggregateTradeAccount

`func NewPiUserDomainAggregatesModelTradeAccountAggregateTradeAccount(rowVersion string, ) *PiUserDomainAggregatesModelTradeAccountAggregateTradeAccount`

NewPiUserDomainAggregatesModelTradeAccountAggregateTradeAccount instantiates a new PiUserDomainAggregatesModelTradeAccountAggregateTradeAccount object
This constructor will assign default values to properties that have it defined,
and makes sure properties required by API are set, but the set of arguments
will change when the set of required properties is changed

### NewPiUserDomainAggregatesModelTradeAccountAggregateTradeAccountWithDefaults

`func NewPiUserDomainAggregatesModelTradeAccountAggregateTradeAccountWithDefaults() *PiUserDomainAggregatesModelTradeAccountAggregateTradeAccount`

NewPiUserDomainAggregatesModelTradeAccountAggregateTradeAccountWithDefaults instantiates a new PiUserDomainAggregatesModelTradeAccountAggregateTradeAccount object
This constructor will only assign default values to properties that have it defined,
but it doesn't guarantee that properties required by API are set

### GetRowVersion

`func (o *PiUserDomainAggregatesModelTradeAccountAggregateTradeAccount) GetRowVersion() string`

GetRowVersion returns the RowVersion field if non-nil, zero value otherwise.

### GetRowVersionOk

`func (o *PiUserDomainAggregatesModelTradeAccountAggregateTradeAccount) GetRowVersionOk() (*string, bool)`

GetRowVersionOk returns a tuple with the RowVersion field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetRowVersion

`func (o *PiUserDomainAggregatesModelTradeAccountAggregateTradeAccount) SetRowVersion(v string)`

SetRowVersion sets RowVersion field to given value.


### GetId

`func (o *PiUserDomainAggregatesModelTradeAccountAggregateTradeAccount) GetId() string`

GetId returns the Id field if non-nil, zero value otherwise.

### GetIdOk

`func (o *PiUserDomainAggregatesModelTradeAccountAggregateTradeAccount) GetIdOk() (*string, bool)`

GetIdOk returns a tuple with the Id field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetId

`func (o *PiUserDomainAggregatesModelTradeAccountAggregateTradeAccount) SetId(v string)`

SetId sets Id field to given value.

### HasId

`func (o *PiUserDomainAggregatesModelTradeAccountAggregateTradeAccount) HasId() bool`

HasId returns a boolean if a field has been set.

### GetAccountNumber

`func (o *PiUserDomainAggregatesModelTradeAccountAggregateTradeAccount) GetAccountNumber() string`

GetAccountNumber returns the AccountNumber field if non-nil, zero value otherwise.

### GetAccountNumberOk

`func (o *PiUserDomainAggregatesModelTradeAccountAggregateTradeAccount) GetAccountNumberOk() (*string, bool)`

GetAccountNumberOk returns a tuple with the AccountNumber field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetAccountNumber

`func (o *PiUserDomainAggregatesModelTradeAccountAggregateTradeAccount) SetAccountNumber(v string)`

SetAccountNumber sets AccountNumber field to given value.

### HasAccountNumber

`func (o *PiUserDomainAggregatesModelTradeAccountAggregateTradeAccount) HasAccountNumber() bool`

HasAccountNumber returns a boolean if a field has been set.

### SetAccountNumberNil

`func (o *PiUserDomainAggregatesModelTradeAccountAggregateTradeAccount) SetAccountNumberNil(b bool)`

 SetAccountNumberNil sets the value for AccountNumber to be an explicit nil

### UnsetAccountNumber
`func (o *PiUserDomainAggregatesModelTradeAccountAggregateTradeAccount) UnsetAccountNumber()`

UnsetAccountNumber ensures that no value is present for AccountNumber, not even an explicit nil
### GetAccountType

`func (o *PiUserDomainAggregatesModelTradeAccountAggregateTradeAccount) GetAccountType() int32`

GetAccountType returns the AccountType field if non-nil, zero value otherwise.

### GetAccountTypeOk

`func (o *PiUserDomainAggregatesModelTradeAccountAggregateTradeAccount) GetAccountTypeOk() (*int32, bool)`

GetAccountTypeOk returns a tuple with the AccountType field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetAccountType

`func (o *PiUserDomainAggregatesModelTradeAccountAggregateTradeAccount) SetAccountType(v int32)`

SetAccountType sets AccountType field to given value.

### HasAccountType

`func (o *PiUserDomainAggregatesModelTradeAccountAggregateTradeAccount) HasAccountType() bool`

HasAccountType returns a boolean if a field has been set.

### GetAccountTypeCode

`func (o *PiUserDomainAggregatesModelTradeAccountAggregateTradeAccount) GetAccountTypeCode() string`

GetAccountTypeCode returns the AccountTypeCode field if non-nil, zero value otherwise.

### GetAccountTypeCodeOk

`func (o *PiUserDomainAggregatesModelTradeAccountAggregateTradeAccount) GetAccountTypeCodeOk() (*string, bool)`

GetAccountTypeCodeOk returns a tuple with the AccountTypeCode field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetAccountTypeCode

`func (o *PiUserDomainAggregatesModelTradeAccountAggregateTradeAccount) SetAccountTypeCode(v string)`

SetAccountTypeCode sets AccountTypeCode field to given value.

### HasAccountTypeCode

`func (o *PiUserDomainAggregatesModelTradeAccountAggregateTradeAccount) HasAccountTypeCode() bool`

HasAccountTypeCode returns a boolean if a field has been set.

### SetAccountTypeCodeNil

`func (o *PiUserDomainAggregatesModelTradeAccountAggregateTradeAccount) SetAccountTypeCodeNil(b bool)`

 SetAccountTypeCodeNil sets the value for AccountTypeCode to be an explicit nil

### UnsetAccountTypeCode
`func (o *PiUserDomainAggregatesModelTradeAccountAggregateTradeAccount) UnsetAccountTypeCode()`

UnsetAccountTypeCode ensures that no value is present for AccountTypeCode, not even an explicit nil
### GetExchangeMarketId

`func (o *PiUserDomainAggregatesModelTradeAccountAggregateTradeAccount) GetExchangeMarketId() string`

GetExchangeMarketId returns the ExchangeMarketId field if non-nil, zero value otherwise.

### GetExchangeMarketIdOk

`func (o *PiUserDomainAggregatesModelTradeAccountAggregateTradeAccount) GetExchangeMarketIdOk() (*string, bool)`

GetExchangeMarketIdOk returns a tuple with the ExchangeMarketId field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetExchangeMarketId

`func (o *PiUserDomainAggregatesModelTradeAccountAggregateTradeAccount) SetExchangeMarketId(v string)`

SetExchangeMarketId sets ExchangeMarketId field to given value.

### HasExchangeMarketId

`func (o *PiUserDomainAggregatesModelTradeAccountAggregateTradeAccount) HasExchangeMarketId() bool`

HasExchangeMarketId returns a boolean if a field has been set.

### SetExchangeMarketIdNil

`func (o *PiUserDomainAggregatesModelTradeAccountAggregateTradeAccount) SetExchangeMarketIdNil(b bool)`

 SetExchangeMarketIdNil sets the value for ExchangeMarketId to be an explicit nil

### UnsetExchangeMarketId
`func (o *PiUserDomainAggregatesModelTradeAccountAggregateTradeAccount) UnsetExchangeMarketId()`

UnsetExchangeMarketId ensures that no value is present for ExchangeMarketId, not even an explicit nil
### GetAccountStatus

`func (o *PiUserDomainAggregatesModelTradeAccountAggregateTradeAccount) GetAccountStatus() string`

GetAccountStatus returns the AccountStatus field if non-nil, zero value otherwise.

### GetAccountStatusOk

`func (o *PiUserDomainAggregatesModelTradeAccountAggregateTradeAccount) GetAccountStatusOk() (*string, bool)`

GetAccountStatusOk returns a tuple with the AccountStatus field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetAccountStatus

`func (o *PiUserDomainAggregatesModelTradeAccountAggregateTradeAccount) SetAccountStatus(v string)`

SetAccountStatus sets AccountStatus field to given value.

### HasAccountStatus

`func (o *PiUserDomainAggregatesModelTradeAccountAggregateTradeAccount) HasAccountStatus() bool`

HasAccountStatus returns a boolean if a field has been set.

### SetAccountStatusNil

`func (o *PiUserDomainAggregatesModelTradeAccountAggregateTradeAccount) SetAccountStatusNil(b bool)`

 SetAccountStatusNil sets the value for AccountStatus to be an explicit nil

### UnsetAccountStatus
`func (o *PiUserDomainAggregatesModelTradeAccountAggregateTradeAccount) UnsetAccountStatus()`

UnsetAccountStatus ensures that no value is present for AccountStatus, not even an explicit nil
### GetCreditLine

`func (o *PiUserDomainAggregatesModelTradeAccountAggregateTradeAccount) GetCreditLine() float64`

GetCreditLine returns the CreditLine field if non-nil, zero value otherwise.

### GetCreditLineOk

`func (o *PiUserDomainAggregatesModelTradeAccountAggregateTradeAccount) GetCreditLineOk() (*float64, bool)`

GetCreditLineOk returns a tuple with the CreditLine field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetCreditLine

`func (o *PiUserDomainAggregatesModelTradeAccountAggregateTradeAccount) SetCreditLine(v float64)`

SetCreditLine sets CreditLine field to given value.

### HasCreditLine

`func (o *PiUserDomainAggregatesModelTradeAccountAggregateTradeAccount) HasCreditLine() bool`

HasCreditLine returns a boolean if a field has been set.

### GetCreditLineCurrency

`func (o *PiUserDomainAggregatesModelTradeAccountAggregateTradeAccount) GetCreditLineCurrency() string`

GetCreditLineCurrency returns the CreditLineCurrency field if non-nil, zero value otherwise.

### GetCreditLineCurrencyOk

`func (o *PiUserDomainAggregatesModelTradeAccountAggregateTradeAccount) GetCreditLineCurrencyOk() (*string, bool)`

GetCreditLineCurrencyOk returns a tuple with the CreditLineCurrency field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetCreditLineCurrency

`func (o *PiUserDomainAggregatesModelTradeAccountAggregateTradeAccount) SetCreditLineCurrency(v string)`

SetCreditLineCurrency sets CreditLineCurrency field to given value.

### HasCreditLineCurrency

`func (o *PiUserDomainAggregatesModelTradeAccountAggregateTradeAccount) HasCreditLineCurrency() bool`

HasCreditLineCurrency returns a boolean if a field has been set.

### SetCreditLineCurrencyNil

`func (o *PiUserDomainAggregatesModelTradeAccountAggregateTradeAccount) SetCreditLineCurrencyNil(b bool)`

 SetCreditLineCurrencyNil sets the value for CreditLineCurrency to be an explicit nil

### UnsetCreditLineCurrency
`func (o *PiUserDomainAggregatesModelTradeAccountAggregateTradeAccount) UnsetCreditLineCurrency()`

UnsetCreditLineCurrency ensures that no value is present for CreditLineCurrency, not even an explicit nil
### GetEffectiveDate

`func (o *PiUserDomainAggregatesModelTradeAccountAggregateTradeAccount) GetEffectiveDate() string`

GetEffectiveDate returns the EffectiveDate field if non-nil, zero value otherwise.

### GetEffectiveDateOk

`func (o *PiUserDomainAggregatesModelTradeAccountAggregateTradeAccount) GetEffectiveDateOk() (*string, bool)`

GetEffectiveDateOk returns a tuple with the EffectiveDate field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetEffectiveDate

`func (o *PiUserDomainAggregatesModelTradeAccountAggregateTradeAccount) SetEffectiveDate(v string)`

SetEffectiveDate sets EffectiveDate field to given value.

### HasEffectiveDate

`func (o *PiUserDomainAggregatesModelTradeAccountAggregateTradeAccount) HasEffectiveDate() bool`

HasEffectiveDate returns a boolean if a field has been set.

### SetEffectiveDateNil

`func (o *PiUserDomainAggregatesModelTradeAccountAggregateTradeAccount) SetEffectiveDateNil(b bool)`

 SetEffectiveDateNil sets the value for EffectiveDate to be an explicit nil

### UnsetEffectiveDate
`func (o *PiUserDomainAggregatesModelTradeAccountAggregateTradeAccount) UnsetEffectiveDate()`

UnsetEffectiveDate ensures that no value is present for EffectiveDate, not even an explicit nil
### GetEndDate

`func (o *PiUserDomainAggregatesModelTradeAccountAggregateTradeAccount) GetEndDate() string`

GetEndDate returns the EndDate field if non-nil, zero value otherwise.

### GetEndDateOk

`func (o *PiUserDomainAggregatesModelTradeAccountAggregateTradeAccount) GetEndDateOk() (*string, bool)`

GetEndDateOk returns a tuple with the EndDate field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetEndDate

`func (o *PiUserDomainAggregatesModelTradeAccountAggregateTradeAccount) SetEndDate(v string)`

SetEndDate sets EndDate field to given value.

### HasEndDate

`func (o *PiUserDomainAggregatesModelTradeAccountAggregateTradeAccount) HasEndDate() bool`

HasEndDate returns a boolean if a field has been set.

### SetEndDateNil

`func (o *PiUserDomainAggregatesModelTradeAccountAggregateTradeAccount) SetEndDateNil(b bool)`

 SetEndDateNil sets the value for EndDate to be an explicit nil

### UnsetEndDate
`func (o *PiUserDomainAggregatesModelTradeAccountAggregateTradeAccount) UnsetEndDate()`

UnsetEndDate ensures that no value is present for EndDate, not even an explicit nil
### GetMarketingId

`func (o *PiUserDomainAggregatesModelTradeAccountAggregateTradeAccount) GetMarketingId() string`

GetMarketingId returns the MarketingId field if non-nil, zero value otherwise.

### GetMarketingIdOk

`func (o *PiUserDomainAggregatesModelTradeAccountAggregateTradeAccount) GetMarketingIdOk() (*string, bool)`

GetMarketingIdOk returns a tuple with the MarketingId field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetMarketingId

`func (o *PiUserDomainAggregatesModelTradeAccountAggregateTradeAccount) SetMarketingId(v string)`

SetMarketingId sets MarketingId field to given value.

### HasMarketingId

`func (o *PiUserDomainAggregatesModelTradeAccountAggregateTradeAccount) HasMarketingId() bool`

HasMarketingId returns a boolean if a field has been set.

### SetMarketingIdNil

`func (o *PiUserDomainAggregatesModelTradeAccountAggregateTradeAccount) SetMarketingIdNil(b bool)`

 SetMarketingIdNil sets the value for MarketingId to be an explicit nil

### UnsetMarketingId
`func (o *PiUserDomainAggregatesModelTradeAccountAggregateTradeAccount) UnsetMarketingId()`

UnsetMarketingId ensures that no value is present for MarketingId, not even an explicit nil
### GetSaleLicense

`func (o *PiUserDomainAggregatesModelTradeAccountAggregateTradeAccount) GetSaleLicense() string`

GetSaleLicense returns the SaleLicense field if non-nil, zero value otherwise.

### GetSaleLicenseOk

`func (o *PiUserDomainAggregatesModelTradeAccountAggregateTradeAccount) GetSaleLicenseOk() (*string, bool)`

GetSaleLicenseOk returns a tuple with the SaleLicense field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetSaleLicense

`func (o *PiUserDomainAggregatesModelTradeAccountAggregateTradeAccount) SetSaleLicense(v string)`

SetSaleLicense sets SaleLicense field to given value.

### HasSaleLicense

`func (o *PiUserDomainAggregatesModelTradeAccountAggregateTradeAccount) HasSaleLicense() bool`

HasSaleLicense returns a boolean if a field has been set.

### SetSaleLicenseNil

`func (o *PiUserDomainAggregatesModelTradeAccountAggregateTradeAccount) SetSaleLicenseNil(b bool)`

 SetSaleLicenseNil sets the value for SaleLicense to be an explicit nil

### UnsetSaleLicense
`func (o *PiUserDomainAggregatesModelTradeAccountAggregateTradeAccount) UnsetSaleLicense()`

UnsetSaleLicense ensures that no value is present for SaleLicense, not even an explicit nil
### GetOpenDate

`func (o *PiUserDomainAggregatesModelTradeAccountAggregateTradeAccount) GetOpenDate() string`

GetOpenDate returns the OpenDate field if non-nil, zero value otherwise.

### GetOpenDateOk

`func (o *PiUserDomainAggregatesModelTradeAccountAggregateTradeAccount) GetOpenDateOk() (*string, bool)`

GetOpenDateOk returns a tuple with the OpenDate field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetOpenDate

`func (o *PiUserDomainAggregatesModelTradeAccountAggregateTradeAccount) SetOpenDate(v string)`

SetOpenDate sets OpenDate field to given value.

### HasOpenDate

`func (o *PiUserDomainAggregatesModelTradeAccountAggregateTradeAccount) HasOpenDate() bool`

HasOpenDate returns a boolean if a field has been set.

### SetOpenDateNil

`func (o *PiUserDomainAggregatesModelTradeAccountAggregateTradeAccount) SetOpenDateNil(b bool)`

 SetOpenDateNil sets the value for OpenDate to be an explicit nil

### UnsetOpenDate
`func (o *PiUserDomainAggregatesModelTradeAccountAggregateTradeAccount) UnsetOpenDate()`

UnsetOpenDate ensures that no value is present for OpenDate, not even an explicit nil
### GetUserAccountId

`func (o *PiUserDomainAggregatesModelTradeAccountAggregateTradeAccount) GetUserAccountId() string`

GetUserAccountId returns the UserAccountId field if non-nil, zero value otherwise.

### GetUserAccountIdOk

`func (o *PiUserDomainAggregatesModelTradeAccountAggregateTradeAccount) GetUserAccountIdOk() (*string, bool)`

GetUserAccountIdOk returns a tuple with the UserAccountId field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetUserAccountId

`func (o *PiUserDomainAggregatesModelTradeAccountAggregateTradeAccount) SetUserAccountId(v string)`

SetUserAccountId sets UserAccountId field to given value.

### HasUserAccountId

`func (o *PiUserDomainAggregatesModelTradeAccountAggregateTradeAccount) HasUserAccountId() bool`

HasUserAccountId returns a boolean if a field has been set.

### SetUserAccountIdNil

`func (o *PiUserDomainAggregatesModelTradeAccountAggregateTradeAccount) SetUserAccountIdNil(b bool)`

 SetUserAccountIdNil sets the value for UserAccountId to be an explicit nil

### UnsetUserAccountId
`func (o *PiUserDomainAggregatesModelTradeAccountAggregateTradeAccount) UnsetUserAccountId()`

UnsetUserAccountId ensures that no value is present for UserAccountId, not even an explicit nil
### GetCreatedAt

`func (o *PiUserDomainAggregatesModelTradeAccountAggregateTradeAccount) GetCreatedAt() time.Time`

GetCreatedAt returns the CreatedAt field if non-nil, zero value otherwise.

### GetCreatedAtOk

`func (o *PiUserDomainAggregatesModelTradeAccountAggregateTradeAccount) GetCreatedAtOk() (*time.Time, bool)`

GetCreatedAtOk returns a tuple with the CreatedAt field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetCreatedAt

`func (o *PiUserDomainAggregatesModelTradeAccountAggregateTradeAccount) SetCreatedAt(v time.Time)`

SetCreatedAt sets CreatedAt field to given value.

### HasCreatedAt

`func (o *PiUserDomainAggregatesModelTradeAccountAggregateTradeAccount) HasCreatedAt() bool`

HasCreatedAt returns a boolean if a field has been set.

### GetUpdatedAt

`func (o *PiUserDomainAggregatesModelTradeAccountAggregateTradeAccount) GetUpdatedAt() time.Time`

GetUpdatedAt returns the UpdatedAt field if non-nil, zero value otherwise.

### GetUpdatedAtOk

`func (o *PiUserDomainAggregatesModelTradeAccountAggregateTradeAccount) GetUpdatedAtOk() (*time.Time, bool)`

GetUpdatedAtOk returns a tuple with the UpdatedAt field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetUpdatedAt

`func (o *PiUserDomainAggregatesModelTradeAccountAggregateTradeAccount) SetUpdatedAt(v time.Time)`

SetUpdatedAt sets UpdatedAt field to given value.

### HasUpdatedAt

`func (o *PiUserDomainAggregatesModelTradeAccountAggregateTradeAccount) HasUpdatedAt() bool`

HasUpdatedAt returns a boolean if a field has been set.

### SetUpdatedAtNil

`func (o *PiUserDomainAggregatesModelTradeAccountAggregateTradeAccount) SetUpdatedAtNil(b bool)`

 SetUpdatedAtNil sets the value for UpdatedAt to be an explicit nil

### UnsetUpdatedAt
`func (o *PiUserDomainAggregatesModelTradeAccountAggregateTradeAccount) UnsetUpdatedAt()`

UnsetUpdatedAt ensures that no value is present for UpdatedAt, not even an explicit nil
### GetExternalAccounts

`func (o *PiUserDomainAggregatesModelTradeAccountAggregateTradeAccount) GetExternalAccounts() []PiUserDomainAggregatesModelExternalAccountAggregateExternalAccount`

GetExternalAccounts returns the ExternalAccounts field if non-nil, zero value otherwise.

### GetExternalAccountsOk

`func (o *PiUserDomainAggregatesModelTradeAccountAggregateTradeAccount) GetExternalAccountsOk() (*[]PiUserDomainAggregatesModelExternalAccountAggregateExternalAccount, bool)`

GetExternalAccountsOk returns a tuple with the ExternalAccounts field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetExternalAccounts

`func (o *PiUserDomainAggregatesModelTradeAccountAggregateTradeAccount) SetExternalAccounts(v []PiUserDomainAggregatesModelExternalAccountAggregateExternalAccount)`

SetExternalAccounts sets ExternalAccounts field to given value.

### HasExternalAccounts

`func (o *PiUserDomainAggregatesModelTradeAccountAggregateTradeAccount) HasExternalAccounts() bool`

HasExternalAccounts returns a boolean if a field has been set.

### SetExternalAccountsNil

`func (o *PiUserDomainAggregatesModelTradeAccountAggregateTradeAccount) SetExternalAccountsNil(b bool)`

 SetExternalAccountsNil sets the value for ExternalAccounts to be an explicit nil

### UnsetExternalAccounts
`func (o *PiUserDomainAggregatesModelTradeAccountAggregateTradeAccount) UnsetExternalAccounts()`

UnsetExternalAccounts ensures that no value is present for ExternalAccounts, not even an explicit nil
### GetTradeAccountBankAccounts

`func (o *PiUserDomainAggregatesModelTradeAccountAggregateTradeAccount) GetTradeAccountBankAccounts() []PiUserDomainAggregatesModelTradeAccountBankAccountAggregateTradeAccountBankAccount`

GetTradeAccountBankAccounts returns the TradeAccountBankAccounts field if non-nil, zero value otherwise.

### GetTradeAccountBankAccountsOk

`func (o *PiUserDomainAggregatesModelTradeAccountAggregateTradeAccount) GetTradeAccountBankAccountsOk() (*[]PiUserDomainAggregatesModelTradeAccountBankAccountAggregateTradeAccountBankAccount, bool)`

GetTradeAccountBankAccountsOk returns a tuple with the TradeAccountBankAccounts field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetTradeAccountBankAccounts

`func (o *PiUserDomainAggregatesModelTradeAccountAggregateTradeAccount) SetTradeAccountBankAccounts(v []PiUserDomainAggregatesModelTradeAccountBankAccountAggregateTradeAccountBankAccount)`

SetTradeAccountBankAccounts sets TradeAccountBankAccounts field to given value.

### HasTradeAccountBankAccounts

`func (o *PiUserDomainAggregatesModelTradeAccountAggregateTradeAccount) HasTradeAccountBankAccounts() bool`

HasTradeAccountBankAccounts returns a boolean if a field has been set.

### SetTradeAccountBankAccountsNil

`func (o *PiUserDomainAggregatesModelTradeAccountAggregateTradeAccount) SetTradeAccountBankAccountsNil(b bool)`

 SetTradeAccountBankAccountsNil sets the value for TradeAccountBankAccounts to be an explicit nil

### UnsetTradeAccountBankAccounts
`func (o *PiUserDomainAggregatesModelTradeAccountAggregateTradeAccount) UnsetTradeAccountBankAccounts()`

UnsetTradeAccountBankAccounts ensures that no value is present for TradeAccountBankAccounts, not even an explicit nil

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


