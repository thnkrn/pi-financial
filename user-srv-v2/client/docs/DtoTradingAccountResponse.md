# DtoTradingAccountResponse

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**AccountStatus** | Pointer to **string** |  | [optional]
**AccountType** | Pointer to **string** |  | [optional]
**AccountTypeCode** | Pointer to **string** |  | [optional]
**BankAccounts** | Pointer to [**[]DtoBankAccountsResponse**](DtoBankAccountsResponse.md) |  | [optional]
**CreditLine** | Pointer to **float32** |  | [optional]
**CreditLineCurrency** | Pointer to **string** |  | [optional]
**EnableBuy** | Pointer to **string** |  | [optional]
**EnableDeposit** | Pointer to **string** |  | [optional]
**EnableSell** | Pointer to **string** |  | [optional]
**EnableWithdraw** | Pointer to **string** |  | [optional]
**ExchangeMarketId** | Pointer to **string** |  | [optional]
**ExternalAccounts** | Pointer to [**[]DtoExternalAccountResponse**](DtoExternalAccountResponse.md) |  | [optional]
**FrontName** | Pointer to **string** |  | [optional]
**Id** | Pointer to **string** |  | [optional]
**ProductName** | Pointer to **string** |  | [optional]
**TradingAccountNo** | Pointer to **string** |  | [optional]

## Methods

### NewDtoTradingAccountResponse

`func NewDtoTradingAccountResponse() *DtoTradingAccountResponse`

NewDtoTradingAccountResponse instantiates a new DtoTradingAccountResponse object
This constructor will assign default values to properties that have it defined,
and makes sure properties required by API are set, but the set of arguments
will change when the set of required properties is changed

### NewDtoTradingAccountResponseWithDefaults

`func NewDtoTradingAccountResponseWithDefaults() *DtoTradingAccountResponse`

NewDtoTradingAccountResponseWithDefaults instantiates a new DtoTradingAccountResponse object
This constructor will only assign default values to properties that have it defined,
but it doesn't guarantee that properties required by API are set

### GetAccountStatus

`func (o *DtoTradingAccountResponse) GetAccountStatus() string`

GetAccountStatus returns the AccountStatus field if non-nil, zero value otherwise.

### GetAccountStatusOk

`func (o *DtoTradingAccountResponse) GetAccountStatusOk() (*string, bool)`

GetAccountStatusOk returns a tuple with the AccountStatus field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetAccountStatus

`func (o *DtoTradingAccountResponse) SetAccountStatus(v string)`

SetAccountStatus sets AccountStatus field to given value.

### HasAccountStatus

`func (o *DtoTradingAccountResponse) HasAccountStatus() bool`

HasAccountStatus returns a boolean if a field has been set.

### GetAccountType

`func (o *DtoTradingAccountResponse) GetAccountType() string`

GetAccountType returns the AccountType field if non-nil, zero value otherwise.

### GetAccountTypeOk

`func (o *DtoTradingAccountResponse) GetAccountTypeOk() (*string, bool)`

GetAccountTypeOk returns a tuple with the AccountType field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetAccountType

`func (o *DtoTradingAccountResponse) SetAccountType(v string)`

SetAccountType sets AccountType field to given value.

### HasAccountType

`func (o *DtoTradingAccountResponse) HasAccountType() bool`

HasAccountType returns a boolean if a field has been set.

### GetAccountTypeCode

`func (o *DtoTradingAccountResponse) GetAccountTypeCode() string`

GetAccountTypeCode returns the AccountTypeCode field if non-nil, zero value otherwise.

### GetAccountTypeCodeOk

`func (o *DtoTradingAccountResponse) GetAccountTypeCodeOk() (*string, bool)`

GetAccountTypeCodeOk returns a tuple with the AccountTypeCode field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetAccountTypeCode

`func (o *DtoTradingAccountResponse) SetAccountTypeCode(v string)`

SetAccountTypeCode sets AccountTypeCode field to given value.

### HasAccountTypeCode

`func (o *DtoTradingAccountResponse) HasAccountTypeCode() bool`

HasAccountTypeCode returns a boolean if a field has been set.

### GetBankAccounts

`func (o *DtoTradingAccountResponse) GetBankAccounts() []DtoBankAccountsResponse`

GetBankAccounts returns the BankAccounts field if non-nil, zero value otherwise.

### GetBankAccountsOk

`func (o *DtoTradingAccountResponse) GetBankAccountsOk() (*[]DtoBankAccountsResponse, bool)`

GetBankAccountsOk returns a tuple with the BankAccounts field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetBankAccounts

`func (o *DtoTradingAccountResponse) SetBankAccounts(v []DtoBankAccountsResponse)`

SetBankAccounts sets BankAccounts field to given value.

### HasBankAccounts

`func (o *DtoTradingAccountResponse) HasBankAccounts() bool`

HasBankAccounts returns a boolean if a field has been set.

### GetCreditLine

`func (o *DtoTradingAccountResponse) GetCreditLine() float32`

GetCreditLine returns the CreditLine field if non-nil, zero value otherwise.

### GetCreditLineOk

`func (o *DtoTradingAccountResponse) GetCreditLineOk() (*float32, bool)`

GetCreditLineOk returns a tuple with the CreditLine field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetCreditLine

`func (o *DtoTradingAccountResponse) SetCreditLine(v float32)`

SetCreditLine sets CreditLine field to given value.

### HasCreditLine

`func (o *DtoTradingAccountResponse) HasCreditLine() bool`

HasCreditLine returns a boolean if a field has been set.

### GetCreditLineCurrency

`func (o *DtoTradingAccountResponse) GetCreditLineCurrency() string`

GetCreditLineCurrency returns the CreditLineCurrency field if non-nil, zero value otherwise.

### GetCreditLineCurrencyOk

`func (o *DtoTradingAccountResponse) GetCreditLineCurrencyOk() (*string, bool)`

GetCreditLineCurrencyOk returns a tuple with the CreditLineCurrency field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetCreditLineCurrency

`func (o *DtoTradingAccountResponse) SetCreditLineCurrency(v string)`

SetCreditLineCurrency sets CreditLineCurrency field to given value.

### HasCreditLineCurrency

`func (o *DtoTradingAccountResponse) HasCreditLineCurrency() bool`

HasCreditLineCurrency returns a boolean if a field has been set.

### GetEnableBuy

`func (o *DtoTradingAccountResponse) GetEnableBuy() string`

GetEnableBuy returns the EnableBuy field if non-nil, zero value otherwise.

### GetEnableBuyOk

`func (o *DtoTradingAccountResponse) GetEnableBuyOk() (*string, bool)`

GetEnableBuyOk returns a tuple with the EnableBuy field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetEnableBuy

`func (o *DtoTradingAccountResponse) SetEnableBuy(v string)`

SetEnableBuy sets EnableBuy field to given value.

### HasEnableBuy

`func (o *DtoTradingAccountResponse) HasEnableBuy() bool`

HasEnableBuy returns a boolean if a field has been set.

### GetEnableDeposit

`func (o *DtoTradingAccountResponse) GetEnableDeposit() string`

GetEnableDeposit returns the EnableDeposit field if non-nil, zero value otherwise.

### GetEnableDepositOk

`func (o *DtoTradingAccountResponse) GetEnableDepositOk() (*string, bool)`

GetEnableDepositOk returns a tuple with the EnableDeposit field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetEnableDeposit

`func (o *DtoTradingAccountResponse) SetEnableDeposit(v string)`

SetEnableDeposit sets EnableDeposit field to given value.

### HasEnableDeposit

`func (o *DtoTradingAccountResponse) HasEnableDeposit() bool`

HasEnableDeposit returns a boolean if a field has been set.

### GetEnableSell

`func (o *DtoTradingAccountResponse) GetEnableSell() string`

GetEnableSell returns the EnableSell field if non-nil, zero value otherwise.

### GetEnableSellOk

`func (o *DtoTradingAccountResponse) GetEnableSellOk() (*string, bool)`

GetEnableSellOk returns a tuple with the EnableSell field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetEnableSell

`func (o *DtoTradingAccountResponse) SetEnableSell(v string)`

SetEnableSell sets EnableSell field to given value.

### HasEnableSell

`func (o *DtoTradingAccountResponse) HasEnableSell() bool`

HasEnableSell returns a boolean if a field has been set.

### GetEnableWithdraw

`func (o *DtoTradingAccountResponse) GetEnableWithdraw() string`

GetEnableWithdraw returns the EnableWithdraw field if non-nil, zero value otherwise.

### GetEnableWithdrawOk

`func (o *DtoTradingAccountResponse) GetEnableWithdrawOk() (*string, bool)`

GetEnableWithdrawOk returns a tuple with the EnableWithdraw field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetEnableWithdraw

`func (o *DtoTradingAccountResponse) SetEnableWithdraw(v string)`

SetEnableWithdraw sets EnableWithdraw field to given value.

### HasEnableWithdraw

`func (o *DtoTradingAccountResponse) HasEnableWithdraw() bool`

HasEnableWithdraw returns a boolean if a field has been set.

### GetExchangeMarketId

`func (o *DtoTradingAccountResponse) GetExchangeMarketId() string`

GetExchangeMarketId returns the ExchangeMarketId field if non-nil, zero value otherwise.

### GetExchangeMarketIdOk

`func (o *DtoTradingAccountResponse) GetExchangeMarketIdOk() (*string, bool)`

GetExchangeMarketIdOk returns a tuple with the ExchangeMarketId field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetExchangeMarketId

`func (o *DtoTradingAccountResponse) SetExchangeMarketId(v string)`

SetExchangeMarketId sets ExchangeMarketId field to given value.

### HasExchangeMarketId

`func (o *DtoTradingAccountResponse) HasExchangeMarketId() bool`

HasExchangeMarketId returns a boolean if a field has been set.

### GetExternalAccounts

`func (o *DtoTradingAccountResponse) GetExternalAccounts() []DtoExternalAccountResponse`

GetExternalAccounts returns the ExternalAccounts field if non-nil, zero value otherwise.

### GetExternalAccountsOk

`func (o *DtoTradingAccountResponse) GetExternalAccountsOk() (*[]DtoExternalAccountResponse, bool)`

GetExternalAccountsOk returns a tuple with the ExternalAccounts field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetExternalAccounts

`func (o *DtoTradingAccountResponse) SetExternalAccounts(v []DtoExternalAccountResponse)`

SetExternalAccounts sets ExternalAccounts field to given value.

### HasExternalAccounts

`func (o *DtoTradingAccountResponse) HasExternalAccounts() bool`

HasExternalAccounts returns a boolean if a field has been set.

### GetFrontName

`func (o *DtoTradingAccountResponse) GetFrontName() string`

GetFrontName returns the FrontName field if non-nil, zero value otherwise.

### GetFrontNameOk

`func (o *DtoTradingAccountResponse) GetFrontNameOk() (*string, bool)`

GetFrontNameOk returns a tuple with the FrontName field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetFrontName

`func (o *DtoTradingAccountResponse) SetFrontName(v string)`

SetFrontName sets FrontName field to given value.

### HasFrontName

`func (o *DtoTradingAccountResponse) HasFrontName() bool`

HasFrontName returns a boolean if a field has been set.

### GetId

`func (o *DtoTradingAccountResponse) GetId() string`

GetId returns the Id field if non-nil, zero value otherwise.

### GetIdOk

`func (o *DtoTradingAccountResponse) GetIdOk() (*string, bool)`

GetIdOk returns a tuple with the Id field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetId

`func (o *DtoTradingAccountResponse) SetId(v string)`

SetId sets Id field to given value.

### HasId

`func (o *DtoTradingAccountResponse) HasId() bool`

HasId returns a boolean if a field has been set.

### GetProductName

`func (o *DtoTradingAccountResponse) GetProductName() string`

GetProductName returns the ProductName field if non-nil, zero value otherwise.

### GetProductNameOk

`func (o *DtoTradingAccountResponse) GetProductNameOk() (*string, bool)`

GetProductNameOk returns a tuple with the ProductName field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetProductName

`func (o *DtoTradingAccountResponse) SetProductName(v string)`

SetProductName sets ProductName field to given value.

### HasProductName

`func (o *DtoTradingAccountResponse) HasProductName() bool`

HasProductName returns a boolean if a field has been set.

### GetTradingAccountNo

`func (o *DtoTradingAccountResponse) GetTradingAccountNo() string`

GetTradingAccountNo returns the TradingAccountNo field if non-nil, zero value otherwise.

### GetTradingAccountNoOk

`func (o *DtoTradingAccountResponse) GetTradingAccountNoOk() (*string, bool)`

GetTradingAccountNoOk returns a tuple with the TradingAccountNo field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetTradingAccountNo

`func (o *DtoTradingAccountResponse) SetTradingAccountNo(v string)`

SetTradingAccountNo sets TradingAccountNo field to given value.

### HasTradingAccountNo

`func (o *DtoTradingAccountResponse) HasTradingAccountNo() bool`

HasTradingAccountNo returns a boolean if a field has been set.


[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)
