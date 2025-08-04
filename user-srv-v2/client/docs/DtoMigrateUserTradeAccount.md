# DtoMigrateUserTradeAccount

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**AccountNumber** | Pointer to **string** |  | [optional]
**AccountStatus** | Pointer to **string** |  | [optional]
**AccountType** | Pointer to **string** |  | [optional]
**AccountTypeCode** | Pointer to **string** |  | [optional]
**CreditLine** | Pointer to **float32** |  | [optional]
**CreditLineCurrency** | Pointer to **string** |  | [optional]
**EffectiveDate** | Pointer to **string** |  | [optional]
**EnableBuy** | Pointer to **string** |  | [optional]
**EnableDeposit** | Pointer to **string** |  | [optional]
**EnableSell** | Pointer to **string** |  | [optional]
**EnableWithdraw** | Pointer to **string** |  | [optional]
**EndDate** | Pointer to **string** |  | [optional]
**ExchangeMarketId** | Pointer to **string** |  | [optional]
**ExternalAccount** | Pointer to [**[]DtoMigrateUserExternalAccount**](DtoMigrateUserExternalAccount.md) |  | [optional]
**FrontName** | Pointer to **string** |  | [optional]
**MarketingId** | Pointer to **string** |  | [optional]
**OpenDate** | Pointer to **string** |  | [optional]
**SaleLicense** | Pointer to **string** |  | [optional]

## Methods

### NewDtoMigrateUserTradeAccount

`func NewDtoMigrateUserTradeAccount() *DtoMigrateUserTradeAccount`

NewDtoMigrateUserTradeAccount instantiates a new DtoMigrateUserTradeAccount object
This constructor will assign default values to properties that have it defined,
and makes sure properties required by API are set, but the set of arguments
will change when the set of required properties is changed

### NewDtoMigrateUserTradeAccountWithDefaults

`func NewDtoMigrateUserTradeAccountWithDefaults() *DtoMigrateUserTradeAccount`

NewDtoMigrateUserTradeAccountWithDefaults instantiates a new DtoMigrateUserTradeAccount object
This constructor will only assign default values to properties that have it defined,
but it doesn't guarantee that properties required by API are set

### GetAccountNumber

`func (o *DtoMigrateUserTradeAccount) GetAccountNumber() string`

GetAccountNumber returns the AccountNumber field if non-nil, zero value otherwise.

### GetAccountNumberOk

`func (o *DtoMigrateUserTradeAccount) GetAccountNumberOk() (*string, bool)`

GetAccountNumberOk returns a tuple with the AccountNumber field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetAccountNumber

`func (o *DtoMigrateUserTradeAccount) SetAccountNumber(v string)`

SetAccountNumber sets AccountNumber field to given value.

### HasAccountNumber

`func (o *DtoMigrateUserTradeAccount) HasAccountNumber() bool`

HasAccountNumber returns a boolean if a field has been set.

### GetAccountStatus

`func (o *DtoMigrateUserTradeAccount) GetAccountStatus() string`

GetAccountStatus returns the AccountStatus field if non-nil, zero value otherwise.

### GetAccountStatusOk

`func (o *DtoMigrateUserTradeAccount) GetAccountStatusOk() (*string, bool)`

GetAccountStatusOk returns a tuple with the AccountStatus field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetAccountStatus

`func (o *DtoMigrateUserTradeAccount) SetAccountStatus(v string)`

SetAccountStatus sets AccountStatus field to given value.

### HasAccountStatus

`func (o *DtoMigrateUserTradeAccount) HasAccountStatus() bool`

HasAccountStatus returns a boolean if a field has been set.

### GetAccountType

`func (o *DtoMigrateUserTradeAccount) GetAccountType() string`

GetAccountType returns the AccountType field if non-nil, zero value otherwise.

### GetAccountTypeOk

`func (o *DtoMigrateUserTradeAccount) GetAccountTypeOk() (*string, bool)`

GetAccountTypeOk returns a tuple with the AccountType field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetAccountType

`func (o *DtoMigrateUserTradeAccount) SetAccountType(v string)`

SetAccountType sets AccountType field to given value.

### HasAccountType

`func (o *DtoMigrateUserTradeAccount) HasAccountType() bool`

HasAccountType returns a boolean if a field has been set.

### GetAccountTypeCode

`func (o *DtoMigrateUserTradeAccount) GetAccountTypeCode() string`

GetAccountTypeCode returns the AccountTypeCode field if non-nil, zero value otherwise.

### GetAccountTypeCodeOk

`func (o *DtoMigrateUserTradeAccount) GetAccountTypeCodeOk() (*string, bool)`

GetAccountTypeCodeOk returns a tuple with the AccountTypeCode field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetAccountTypeCode

`func (o *DtoMigrateUserTradeAccount) SetAccountTypeCode(v string)`

SetAccountTypeCode sets AccountTypeCode field to given value.

### HasAccountTypeCode

`func (o *DtoMigrateUserTradeAccount) HasAccountTypeCode() bool`

HasAccountTypeCode returns a boolean if a field has been set.

### GetCreditLine

`func (o *DtoMigrateUserTradeAccount) GetCreditLine() float32`

GetCreditLine returns the CreditLine field if non-nil, zero value otherwise.

### GetCreditLineOk

`func (o *DtoMigrateUserTradeAccount) GetCreditLineOk() (*float32, bool)`

GetCreditLineOk returns a tuple with the CreditLine field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetCreditLine

`func (o *DtoMigrateUserTradeAccount) SetCreditLine(v float32)`

SetCreditLine sets CreditLine field to given value.

### HasCreditLine

`func (o *DtoMigrateUserTradeAccount) HasCreditLine() bool`

HasCreditLine returns a boolean if a field has been set.

### GetCreditLineCurrency

`func (o *DtoMigrateUserTradeAccount) GetCreditLineCurrency() string`

GetCreditLineCurrency returns the CreditLineCurrency field if non-nil, zero value otherwise.

### GetCreditLineCurrencyOk

`func (o *DtoMigrateUserTradeAccount) GetCreditLineCurrencyOk() (*string, bool)`

GetCreditLineCurrencyOk returns a tuple with the CreditLineCurrency field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetCreditLineCurrency

`func (o *DtoMigrateUserTradeAccount) SetCreditLineCurrency(v string)`

SetCreditLineCurrency sets CreditLineCurrency field to given value.

### HasCreditLineCurrency

`func (o *DtoMigrateUserTradeAccount) HasCreditLineCurrency() bool`

HasCreditLineCurrency returns a boolean if a field has been set.

### GetEffectiveDate

`func (o *DtoMigrateUserTradeAccount) GetEffectiveDate() string`

GetEffectiveDate returns the EffectiveDate field if non-nil, zero value otherwise.

### GetEffectiveDateOk

`func (o *DtoMigrateUserTradeAccount) GetEffectiveDateOk() (*string, bool)`

GetEffectiveDateOk returns a tuple with the EffectiveDate field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetEffectiveDate

`func (o *DtoMigrateUserTradeAccount) SetEffectiveDate(v string)`

SetEffectiveDate sets EffectiveDate field to given value.

### HasEffectiveDate

`func (o *DtoMigrateUserTradeAccount) HasEffectiveDate() bool`

HasEffectiveDate returns a boolean if a field has been set.

### GetEnableBuy

`func (o *DtoMigrateUserTradeAccount) GetEnableBuy() string`

GetEnableBuy returns the EnableBuy field if non-nil, zero value otherwise.

### GetEnableBuyOk

`func (o *DtoMigrateUserTradeAccount) GetEnableBuyOk() (*string, bool)`

GetEnableBuyOk returns a tuple with the EnableBuy field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetEnableBuy

`func (o *DtoMigrateUserTradeAccount) SetEnableBuy(v string)`

SetEnableBuy sets EnableBuy field to given value.

### HasEnableBuy

`func (o *DtoMigrateUserTradeAccount) HasEnableBuy() bool`

HasEnableBuy returns a boolean if a field has been set.

### GetEnableDeposit

`func (o *DtoMigrateUserTradeAccount) GetEnableDeposit() string`

GetEnableDeposit returns the EnableDeposit field if non-nil, zero value otherwise.

### GetEnableDepositOk

`func (o *DtoMigrateUserTradeAccount) GetEnableDepositOk() (*string, bool)`

GetEnableDepositOk returns a tuple with the EnableDeposit field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetEnableDeposit

`func (o *DtoMigrateUserTradeAccount) SetEnableDeposit(v string)`

SetEnableDeposit sets EnableDeposit field to given value.

### HasEnableDeposit

`func (o *DtoMigrateUserTradeAccount) HasEnableDeposit() bool`

HasEnableDeposit returns a boolean if a field has been set.

### GetEnableSell

`func (o *DtoMigrateUserTradeAccount) GetEnableSell() string`

GetEnableSell returns the EnableSell field if non-nil, zero value otherwise.

### GetEnableSellOk

`func (o *DtoMigrateUserTradeAccount) GetEnableSellOk() (*string, bool)`

GetEnableSellOk returns a tuple with the EnableSell field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetEnableSell

`func (o *DtoMigrateUserTradeAccount) SetEnableSell(v string)`

SetEnableSell sets EnableSell field to given value.

### HasEnableSell

`func (o *DtoMigrateUserTradeAccount) HasEnableSell() bool`

HasEnableSell returns a boolean if a field has been set.

### GetEnableWithdraw

`func (o *DtoMigrateUserTradeAccount) GetEnableWithdraw() string`

GetEnableWithdraw returns the EnableWithdraw field if non-nil, zero value otherwise.

### GetEnableWithdrawOk

`func (o *DtoMigrateUserTradeAccount) GetEnableWithdrawOk() (*string, bool)`

GetEnableWithdrawOk returns a tuple with the EnableWithdraw field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetEnableWithdraw

`func (o *DtoMigrateUserTradeAccount) SetEnableWithdraw(v string)`

SetEnableWithdraw sets EnableWithdraw field to given value.

### HasEnableWithdraw

`func (o *DtoMigrateUserTradeAccount) HasEnableWithdraw() bool`

HasEnableWithdraw returns a boolean if a field has been set.

### GetEndDate

`func (o *DtoMigrateUserTradeAccount) GetEndDate() string`

GetEndDate returns the EndDate field if non-nil, zero value otherwise.

### GetEndDateOk

`func (o *DtoMigrateUserTradeAccount) GetEndDateOk() (*string, bool)`

GetEndDateOk returns a tuple with the EndDate field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetEndDate

`func (o *DtoMigrateUserTradeAccount) SetEndDate(v string)`

SetEndDate sets EndDate field to given value.

### HasEndDate

`func (o *DtoMigrateUserTradeAccount) HasEndDate() bool`

HasEndDate returns a boolean if a field has been set.

### GetExchangeMarketId

`func (o *DtoMigrateUserTradeAccount) GetExchangeMarketId() string`

GetExchangeMarketId returns the ExchangeMarketId field if non-nil, zero value otherwise.

### GetExchangeMarketIdOk

`func (o *DtoMigrateUserTradeAccount) GetExchangeMarketIdOk() (*string, bool)`

GetExchangeMarketIdOk returns a tuple with the ExchangeMarketId field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetExchangeMarketId

`func (o *DtoMigrateUserTradeAccount) SetExchangeMarketId(v string)`

SetExchangeMarketId sets ExchangeMarketId field to given value.

### HasExchangeMarketId

`func (o *DtoMigrateUserTradeAccount) HasExchangeMarketId() bool`

HasExchangeMarketId returns a boolean if a field has been set.

### GetExternalAccount

`func (o *DtoMigrateUserTradeAccount) GetExternalAccount() []DtoMigrateUserExternalAccount`

GetExternalAccount returns the ExternalAccount field if non-nil, zero value otherwise.

### GetExternalAccountOk

`func (o *DtoMigrateUserTradeAccount) GetExternalAccountOk() (*[]DtoMigrateUserExternalAccount, bool)`

GetExternalAccountOk returns a tuple with the ExternalAccount field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetExternalAccount

`func (o *DtoMigrateUserTradeAccount) SetExternalAccount(v []DtoMigrateUserExternalAccount)`

SetExternalAccount sets ExternalAccount field to given value.

### HasExternalAccount

`func (o *DtoMigrateUserTradeAccount) HasExternalAccount() bool`

HasExternalAccount returns a boolean if a field has been set.

### GetFrontName

`func (o *DtoMigrateUserTradeAccount) GetFrontName() string`

GetFrontName returns the FrontName field if non-nil, zero value otherwise.

### GetFrontNameOk

`func (o *DtoMigrateUserTradeAccount) GetFrontNameOk() (*string, bool)`

GetFrontNameOk returns a tuple with the FrontName field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetFrontName

`func (o *DtoMigrateUserTradeAccount) SetFrontName(v string)`

SetFrontName sets FrontName field to given value.

### HasFrontName

`func (o *DtoMigrateUserTradeAccount) HasFrontName() bool`

HasFrontName returns a boolean if a field has been set.

### GetMarketingId

`func (o *DtoMigrateUserTradeAccount) GetMarketingId() string`

GetMarketingId returns the MarketingId field if non-nil, zero value otherwise.

### GetMarketingIdOk

`func (o *DtoMigrateUserTradeAccount) GetMarketingIdOk() (*string, bool)`

GetMarketingIdOk returns a tuple with the MarketingId field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetMarketingId

`func (o *DtoMigrateUserTradeAccount) SetMarketingId(v string)`

SetMarketingId sets MarketingId field to given value.

### HasMarketingId

`func (o *DtoMigrateUserTradeAccount) HasMarketingId() bool`

HasMarketingId returns a boolean if a field has been set.

### GetOpenDate

`func (o *DtoMigrateUserTradeAccount) GetOpenDate() string`

GetOpenDate returns the OpenDate field if non-nil, zero value otherwise.

### GetOpenDateOk

`func (o *DtoMigrateUserTradeAccount) GetOpenDateOk() (*string, bool)`

GetOpenDateOk returns a tuple with the OpenDate field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetOpenDate

`func (o *DtoMigrateUserTradeAccount) SetOpenDate(v string)`

SetOpenDate sets OpenDate field to given value.

### HasOpenDate

`func (o *DtoMigrateUserTradeAccount) HasOpenDate() bool`

HasOpenDate returns a boolean if a field has been set.

### GetSaleLicense

`func (o *DtoMigrateUserTradeAccount) GetSaleLicense() string`

GetSaleLicense returns the SaleLicense field if non-nil, zero value otherwise.

### GetSaleLicenseOk

`func (o *DtoMigrateUserTradeAccount) GetSaleLicenseOk() (*string, bool)`

GetSaleLicenseOk returns a tuple with the SaleLicense field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetSaleLicense

`func (o *DtoMigrateUserTradeAccount) SetSaleLicense(v string)`

SetSaleLicense sets SaleLicense field to given value.

### HasSaleLicense

`func (o *DtoMigrateUserTradeAccount) HasSaleLicense() bool`

HasSaleLicense returns a boolean if a field has been set.


[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)
