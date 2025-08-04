# DtoCreateTradingAccountRequest

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**AccountStatus** | Pointer to [**DomainTradeAccountStatus**](DomainTradeAccountStatus.md) |  | [optional]
**AccountType** | Pointer to **string** |  | [optional]
**AccountTypeCode** | Pointer to **string** |  | [optional]
**CreditLine** | Pointer to **float32** |  | [optional]
**CreditLineCurrency** | Pointer to **string** |  | [optional]
**EffectiveDate** | Pointer to **string** | 2025-12-31 | [optional]
**EnableBuy** | Pointer to **string** |  | [optional]
**EnableDeposit** | Pointer to **string** |  | [optional]
**EnableSell** | Pointer to **string** |  | [optional]
**EnableWithdraw** | Pointer to **string** |  | [optional]
**EndDate** | Pointer to **string** | 2025-12-31 | [optional]
**ExchangeMarketId** | Pointer to **string** |  | [optional]
**FrontName** | Pointer to **string** |  | [optional]
**MarketingId** | Pointer to **string** |  | [optional]
**OpenDate** | Pointer to **string** | 2025-12-31 | [optional]
**SaleLicense** | Pointer to **string** |  | [optional]
**TradingAccountNo** | Pointer to **string** |  | [optional]

## Methods

### NewDtoCreateTradingAccountRequest

`func NewDtoCreateTradingAccountRequest() *DtoCreateTradingAccountRequest`

NewDtoCreateTradingAccountRequest instantiates a new DtoCreateTradingAccountRequest object
This constructor will assign default values to properties that have it defined,
and makes sure properties required by API are set, but the set of arguments
will change when the set of required properties is changed

### NewDtoCreateTradingAccountRequestWithDefaults

`func NewDtoCreateTradingAccountRequestWithDefaults() *DtoCreateTradingAccountRequest`

NewDtoCreateTradingAccountRequestWithDefaults instantiates a new DtoCreateTradingAccountRequest object
This constructor will only assign default values to properties that have it defined,
but it doesn't guarantee that properties required by API are set

### GetAccountStatus

`func (o *DtoCreateTradingAccountRequest) GetAccountStatus() DomainTradeAccountStatus`

GetAccountStatus returns the AccountStatus field if non-nil, zero value otherwise.

### GetAccountStatusOk

`func (o *DtoCreateTradingAccountRequest) GetAccountStatusOk() (*DomainTradeAccountStatus, bool)`

GetAccountStatusOk returns a tuple with the AccountStatus field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetAccountStatus

`func (o *DtoCreateTradingAccountRequest) SetAccountStatus(v DomainTradeAccountStatus)`

SetAccountStatus sets AccountStatus field to given value.

### HasAccountStatus

`func (o *DtoCreateTradingAccountRequest) HasAccountStatus() bool`

HasAccountStatus returns a boolean if a field has been set.

### GetAccountType

`func (o *DtoCreateTradingAccountRequest) GetAccountType() string`

GetAccountType returns the AccountType field if non-nil, zero value otherwise.

### GetAccountTypeOk

`func (o *DtoCreateTradingAccountRequest) GetAccountTypeOk() (*string, bool)`

GetAccountTypeOk returns a tuple with the AccountType field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetAccountType

`func (o *DtoCreateTradingAccountRequest) SetAccountType(v string)`

SetAccountType sets AccountType field to given value.

### HasAccountType

`func (o *DtoCreateTradingAccountRequest) HasAccountType() bool`

HasAccountType returns a boolean if a field has been set.

### GetAccountTypeCode

`func (o *DtoCreateTradingAccountRequest) GetAccountTypeCode() string`

GetAccountTypeCode returns the AccountTypeCode field if non-nil, zero value otherwise.

### GetAccountTypeCodeOk

`func (o *DtoCreateTradingAccountRequest) GetAccountTypeCodeOk() (*string, bool)`

GetAccountTypeCodeOk returns a tuple with the AccountTypeCode field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetAccountTypeCode

`func (o *DtoCreateTradingAccountRequest) SetAccountTypeCode(v string)`

SetAccountTypeCode sets AccountTypeCode field to given value.

### HasAccountTypeCode

`func (o *DtoCreateTradingAccountRequest) HasAccountTypeCode() bool`

HasAccountTypeCode returns a boolean if a field has been set.

### GetCreditLine

`func (o *DtoCreateTradingAccountRequest) GetCreditLine() float32`

GetCreditLine returns the CreditLine field if non-nil, zero value otherwise.

### GetCreditLineOk

`func (o *DtoCreateTradingAccountRequest) GetCreditLineOk() (*float32, bool)`

GetCreditLineOk returns a tuple with the CreditLine field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetCreditLine

`func (o *DtoCreateTradingAccountRequest) SetCreditLine(v float32)`

SetCreditLine sets CreditLine field to given value.

### HasCreditLine

`func (o *DtoCreateTradingAccountRequest) HasCreditLine() bool`

HasCreditLine returns a boolean if a field has been set.

### GetCreditLineCurrency

`func (o *DtoCreateTradingAccountRequest) GetCreditLineCurrency() string`

GetCreditLineCurrency returns the CreditLineCurrency field if non-nil, zero value otherwise.

### GetCreditLineCurrencyOk

`func (o *DtoCreateTradingAccountRequest) GetCreditLineCurrencyOk() (*string, bool)`

GetCreditLineCurrencyOk returns a tuple with the CreditLineCurrency field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetCreditLineCurrency

`func (o *DtoCreateTradingAccountRequest) SetCreditLineCurrency(v string)`

SetCreditLineCurrency sets CreditLineCurrency field to given value.

### HasCreditLineCurrency

`func (o *DtoCreateTradingAccountRequest) HasCreditLineCurrency() bool`

HasCreditLineCurrency returns a boolean if a field has been set.

### GetEffectiveDate

`func (o *DtoCreateTradingAccountRequest) GetEffectiveDate() string`

GetEffectiveDate returns the EffectiveDate field if non-nil, zero value otherwise.

### GetEffectiveDateOk

`func (o *DtoCreateTradingAccountRequest) GetEffectiveDateOk() (*string, bool)`

GetEffectiveDateOk returns a tuple with the EffectiveDate field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetEffectiveDate

`func (o *DtoCreateTradingAccountRequest) SetEffectiveDate(v string)`

SetEffectiveDate sets EffectiveDate field to given value.

### HasEffectiveDate

`func (o *DtoCreateTradingAccountRequest) HasEffectiveDate() bool`

HasEffectiveDate returns a boolean if a field has been set.

### GetEnableBuy

`func (o *DtoCreateTradingAccountRequest) GetEnableBuy() string`

GetEnableBuy returns the EnableBuy field if non-nil, zero value otherwise.

### GetEnableBuyOk

`func (o *DtoCreateTradingAccountRequest) GetEnableBuyOk() (*string, bool)`

GetEnableBuyOk returns a tuple with the EnableBuy field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetEnableBuy

`func (o *DtoCreateTradingAccountRequest) SetEnableBuy(v string)`

SetEnableBuy sets EnableBuy field to given value.

### HasEnableBuy

`func (o *DtoCreateTradingAccountRequest) HasEnableBuy() bool`

HasEnableBuy returns a boolean if a field has been set.

### GetEnableDeposit

`func (o *DtoCreateTradingAccountRequest) GetEnableDeposit() string`

GetEnableDeposit returns the EnableDeposit field if non-nil, zero value otherwise.

### GetEnableDepositOk

`func (o *DtoCreateTradingAccountRequest) GetEnableDepositOk() (*string, bool)`

GetEnableDepositOk returns a tuple with the EnableDeposit field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetEnableDeposit

`func (o *DtoCreateTradingAccountRequest) SetEnableDeposit(v string)`

SetEnableDeposit sets EnableDeposit field to given value.

### HasEnableDeposit

`func (o *DtoCreateTradingAccountRequest) HasEnableDeposit() bool`

HasEnableDeposit returns a boolean if a field has been set.

### GetEnableSell

`func (o *DtoCreateTradingAccountRequest) GetEnableSell() string`

GetEnableSell returns the EnableSell field if non-nil, zero value otherwise.

### GetEnableSellOk

`func (o *DtoCreateTradingAccountRequest) GetEnableSellOk() (*string, bool)`

GetEnableSellOk returns a tuple with the EnableSell field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetEnableSell

`func (o *DtoCreateTradingAccountRequest) SetEnableSell(v string)`

SetEnableSell sets EnableSell field to given value.

### HasEnableSell

`func (o *DtoCreateTradingAccountRequest) HasEnableSell() bool`

HasEnableSell returns a boolean if a field has been set.

### GetEnableWithdraw

`func (o *DtoCreateTradingAccountRequest) GetEnableWithdraw() string`

GetEnableWithdraw returns the EnableWithdraw field if non-nil, zero value otherwise.

### GetEnableWithdrawOk

`func (o *DtoCreateTradingAccountRequest) GetEnableWithdrawOk() (*string, bool)`

GetEnableWithdrawOk returns a tuple with the EnableWithdraw field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetEnableWithdraw

`func (o *DtoCreateTradingAccountRequest) SetEnableWithdraw(v string)`

SetEnableWithdraw sets EnableWithdraw field to given value.

### HasEnableWithdraw

`func (o *DtoCreateTradingAccountRequest) HasEnableWithdraw() bool`

HasEnableWithdraw returns a boolean if a field has been set.

### GetEndDate

`func (o *DtoCreateTradingAccountRequest) GetEndDate() string`

GetEndDate returns the EndDate field if non-nil, zero value otherwise.

### GetEndDateOk

`func (o *DtoCreateTradingAccountRequest) GetEndDateOk() (*string, bool)`

GetEndDateOk returns a tuple with the EndDate field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetEndDate

`func (o *DtoCreateTradingAccountRequest) SetEndDate(v string)`

SetEndDate sets EndDate field to given value.

### HasEndDate

`func (o *DtoCreateTradingAccountRequest) HasEndDate() bool`

HasEndDate returns a boolean if a field has been set.

### GetExchangeMarketId

`func (o *DtoCreateTradingAccountRequest) GetExchangeMarketId() string`

GetExchangeMarketId returns the ExchangeMarketId field if non-nil, zero value otherwise.

### GetExchangeMarketIdOk

`func (o *DtoCreateTradingAccountRequest) GetExchangeMarketIdOk() (*string, bool)`

GetExchangeMarketIdOk returns a tuple with the ExchangeMarketId field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetExchangeMarketId

`func (o *DtoCreateTradingAccountRequest) SetExchangeMarketId(v string)`

SetExchangeMarketId sets ExchangeMarketId field to given value.

### HasExchangeMarketId

`func (o *DtoCreateTradingAccountRequest) HasExchangeMarketId() bool`

HasExchangeMarketId returns a boolean if a field has been set.

### GetFrontName

`func (o *DtoCreateTradingAccountRequest) GetFrontName() string`

GetFrontName returns the FrontName field if non-nil, zero value otherwise.

### GetFrontNameOk

`func (o *DtoCreateTradingAccountRequest) GetFrontNameOk() (*string, bool)`

GetFrontNameOk returns a tuple with the FrontName field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetFrontName

`func (o *DtoCreateTradingAccountRequest) SetFrontName(v string)`

SetFrontName sets FrontName field to given value.

### HasFrontName

`func (o *DtoCreateTradingAccountRequest) HasFrontName() bool`

HasFrontName returns a boolean if a field has been set.

### GetMarketingId

`func (o *DtoCreateTradingAccountRequest) GetMarketingId() string`

GetMarketingId returns the MarketingId field if non-nil, zero value otherwise.

### GetMarketingIdOk

`func (o *DtoCreateTradingAccountRequest) GetMarketingIdOk() (*string, bool)`

GetMarketingIdOk returns a tuple with the MarketingId field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetMarketingId

`func (o *DtoCreateTradingAccountRequest) SetMarketingId(v string)`

SetMarketingId sets MarketingId field to given value.

### HasMarketingId

`func (o *DtoCreateTradingAccountRequest) HasMarketingId() bool`

HasMarketingId returns a boolean if a field has been set.

### GetOpenDate

`func (o *DtoCreateTradingAccountRequest) GetOpenDate() string`

GetOpenDate returns the OpenDate field if non-nil, zero value otherwise.

### GetOpenDateOk

`func (o *DtoCreateTradingAccountRequest) GetOpenDateOk() (*string, bool)`

GetOpenDateOk returns a tuple with the OpenDate field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetOpenDate

`func (o *DtoCreateTradingAccountRequest) SetOpenDate(v string)`

SetOpenDate sets OpenDate field to given value.

### HasOpenDate

`func (o *DtoCreateTradingAccountRequest) HasOpenDate() bool`

HasOpenDate returns a boolean if a field has been set.

### GetSaleLicense

`func (o *DtoCreateTradingAccountRequest) GetSaleLicense() string`

GetSaleLicense returns the SaleLicense field if non-nil, zero value otherwise.

### GetSaleLicenseOk

`func (o *DtoCreateTradingAccountRequest) GetSaleLicenseOk() (*string, bool)`

GetSaleLicenseOk returns a tuple with the SaleLicense field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetSaleLicense

`func (o *DtoCreateTradingAccountRequest) SetSaleLicense(v string)`

SetSaleLicense sets SaleLicense field to given value.

### HasSaleLicense

`func (o *DtoCreateTradingAccountRequest) HasSaleLicense() bool`

HasSaleLicense returns a boolean if a field has been set.

### GetTradingAccountNo

`func (o *DtoCreateTradingAccountRequest) GetTradingAccountNo() string`

GetTradingAccountNo returns the TradingAccountNo field if non-nil, zero value otherwise.

### GetTradingAccountNoOk

`func (o *DtoCreateTradingAccountRequest) GetTradingAccountNoOk() (*string, bool)`

GetTradingAccountNoOk returns a tuple with the TradingAccountNo field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetTradingAccountNo

`func (o *DtoCreateTradingAccountRequest) SetTradingAccountNo(v string)`

SetTradingAccountNo sets TradingAccountNo field to given value.

### HasTradingAccountNo

`func (o *DtoCreateTradingAccountRequest) HasTradingAccountNo() bool`

HasTradingAccountNo returns a boolean if a field has been set.


[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)
