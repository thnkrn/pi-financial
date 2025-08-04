# AccountOverviewResponse

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**AccountId** | Pointer to **NullableString** |  | [optional] 
**TradingAccountNoDisplay** | Pointer to **NullableString** |  | [optional] 
**TradingAccountNo** | Pointer to **NullableString** | Format as {CustCode}-2 | [optional] 
**Currency** | Pointer to [**Currency**](Currency.md) |  | [optional] 
**ExchangeRate** | Pointer to [**ExchangeRate**](ExchangeRate.md) |  | [optional] 
**NetAssetValue** | Pointer to **float32** |  | [optional] 
**MarketValue** | Pointer to **float32** |  | [optional] 
**Cost** | Pointer to **float32** |  | [optional] 
**Upnl** | Pointer to **float32** |  | [optional] 
**UpnlPercentage** | Pointer to **float32** |  | [optional] 
**AccountLimit** | Pointer to **float32** |  | [optional] 
**CashBalance** | Pointer to **float32** |  | [optional] 
**LineAvailable** | Pointer to **float32** |  | [optional] 

## Methods

### NewAccountOverviewResponse

`func NewAccountOverviewResponse() *AccountOverviewResponse`

NewAccountOverviewResponse instantiates a new AccountOverviewResponse object
This constructor will assign default values to properties that have it defined,
and makes sure properties required by API are set, but the set of arguments
will change when the set of required properties is changed

### NewAccountOverviewResponseWithDefaults

`func NewAccountOverviewResponseWithDefaults() *AccountOverviewResponse`

NewAccountOverviewResponseWithDefaults instantiates a new AccountOverviewResponse object
This constructor will only assign default values to properties that have it defined,
but it doesn't guarantee that properties required by API are set

### GetAccountId

`func (o *AccountOverviewResponse) GetAccountId() string`

GetAccountId returns the AccountId field if non-nil, zero value otherwise.

### GetAccountIdOk

`func (o *AccountOverviewResponse) GetAccountIdOk() (*string, bool)`

GetAccountIdOk returns a tuple with the AccountId field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetAccountId

`func (o *AccountOverviewResponse) SetAccountId(v string)`

SetAccountId sets AccountId field to given value.

### HasAccountId

`func (o *AccountOverviewResponse) HasAccountId() bool`

HasAccountId returns a boolean if a field has been set.

### SetAccountIdNil

`func (o *AccountOverviewResponse) SetAccountIdNil(b bool)`

 SetAccountIdNil sets the value for AccountId to be an explicit nil

### UnsetAccountId
`func (o *AccountOverviewResponse) UnsetAccountId()`

UnsetAccountId ensures that no value is present for AccountId, not even an explicit nil
### GetTradingAccountNoDisplay

`func (o *AccountOverviewResponse) GetTradingAccountNoDisplay() string`

GetTradingAccountNoDisplay returns the TradingAccountNoDisplay field if non-nil, zero value otherwise.

### GetTradingAccountNoDisplayOk

`func (o *AccountOverviewResponse) GetTradingAccountNoDisplayOk() (*string, bool)`

GetTradingAccountNoDisplayOk returns a tuple with the TradingAccountNoDisplay field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetTradingAccountNoDisplay

`func (o *AccountOverviewResponse) SetTradingAccountNoDisplay(v string)`

SetTradingAccountNoDisplay sets TradingAccountNoDisplay field to given value.

### HasTradingAccountNoDisplay

`func (o *AccountOverviewResponse) HasTradingAccountNoDisplay() bool`

HasTradingAccountNoDisplay returns a boolean if a field has been set.

### SetTradingAccountNoDisplayNil

`func (o *AccountOverviewResponse) SetTradingAccountNoDisplayNil(b bool)`

 SetTradingAccountNoDisplayNil sets the value for TradingAccountNoDisplay to be an explicit nil

### UnsetTradingAccountNoDisplay
`func (o *AccountOverviewResponse) UnsetTradingAccountNoDisplay()`

UnsetTradingAccountNoDisplay ensures that no value is present for TradingAccountNoDisplay, not even an explicit nil
### GetTradingAccountNo

`func (o *AccountOverviewResponse) GetTradingAccountNo() string`

GetTradingAccountNo returns the TradingAccountNo field if non-nil, zero value otherwise.

### GetTradingAccountNoOk

`func (o *AccountOverviewResponse) GetTradingAccountNoOk() (*string, bool)`

GetTradingAccountNoOk returns a tuple with the TradingAccountNo field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetTradingAccountNo

`func (o *AccountOverviewResponse) SetTradingAccountNo(v string)`

SetTradingAccountNo sets TradingAccountNo field to given value.

### HasTradingAccountNo

`func (o *AccountOverviewResponse) HasTradingAccountNo() bool`

HasTradingAccountNo returns a boolean if a field has been set.

### SetTradingAccountNoNil

`func (o *AccountOverviewResponse) SetTradingAccountNoNil(b bool)`

 SetTradingAccountNoNil sets the value for TradingAccountNo to be an explicit nil

### UnsetTradingAccountNo
`func (o *AccountOverviewResponse) UnsetTradingAccountNo()`

UnsetTradingAccountNo ensures that no value is present for TradingAccountNo, not even an explicit nil
### GetCurrency

`func (o *AccountOverviewResponse) GetCurrency() Currency`

GetCurrency returns the Currency field if non-nil, zero value otherwise.

### GetCurrencyOk

`func (o *AccountOverviewResponse) GetCurrencyOk() (*Currency, bool)`

GetCurrencyOk returns a tuple with the Currency field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetCurrency

`func (o *AccountOverviewResponse) SetCurrency(v Currency)`

SetCurrency sets Currency field to given value.

### HasCurrency

`func (o *AccountOverviewResponse) HasCurrency() bool`

HasCurrency returns a boolean if a field has been set.

### GetExchangeRate

`func (o *AccountOverviewResponse) GetExchangeRate() ExchangeRate`

GetExchangeRate returns the ExchangeRate field if non-nil, zero value otherwise.

### GetExchangeRateOk

`func (o *AccountOverviewResponse) GetExchangeRateOk() (*ExchangeRate, bool)`

GetExchangeRateOk returns a tuple with the ExchangeRate field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetExchangeRate

`func (o *AccountOverviewResponse) SetExchangeRate(v ExchangeRate)`

SetExchangeRate sets ExchangeRate field to given value.

### HasExchangeRate

`func (o *AccountOverviewResponse) HasExchangeRate() bool`

HasExchangeRate returns a boolean if a field has been set.

### GetNetAssetValue

`func (o *AccountOverviewResponse) GetNetAssetValue() float32`

GetNetAssetValue returns the NetAssetValue field if non-nil, zero value otherwise.

### GetNetAssetValueOk

`func (o *AccountOverviewResponse) GetNetAssetValueOk() (*float32, bool)`

GetNetAssetValueOk returns a tuple with the NetAssetValue field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetNetAssetValue

`func (o *AccountOverviewResponse) SetNetAssetValue(v float32)`

SetNetAssetValue sets NetAssetValue field to given value.

### HasNetAssetValue

`func (o *AccountOverviewResponse) HasNetAssetValue() bool`

HasNetAssetValue returns a boolean if a field has been set.

### GetMarketValue

`func (o *AccountOverviewResponse) GetMarketValue() float32`

GetMarketValue returns the MarketValue field if non-nil, zero value otherwise.

### GetMarketValueOk

`func (o *AccountOverviewResponse) GetMarketValueOk() (*float32, bool)`

GetMarketValueOk returns a tuple with the MarketValue field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetMarketValue

`func (o *AccountOverviewResponse) SetMarketValue(v float32)`

SetMarketValue sets MarketValue field to given value.

### HasMarketValue

`func (o *AccountOverviewResponse) HasMarketValue() bool`

HasMarketValue returns a boolean if a field has been set.

### GetCost

`func (o *AccountOverviewResponse) GetCost() float32`

GetCost returns the Cost field if non-nil, zero value otherwise.

### GetCostOk

`func (o *AccountOverviewResponse) GetCostOk() (*float32, bool)`

GetCostOk returns a tuple with the Cost field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetCost

`func (o *AccountOverviewResponse) SetCost(v float32)`

SetCost sets Cost field to given value.

### HasCost

`func (o *AccountOverviewResponse) HasCost() bool`

HasCost returns a boolean if a field has been set.

### GetUpnl

`func (o *AccountOverviewResponse) GetUpnl() float32`

GetUpnl returns the Upnl field if non-nil, zero value otherwise.

### GetUpnlOk

`func (o *AccountOverviewResponse) GetUpnlOk() (*float32, bool)`

GetUpnlOk returns a tuple with the Upnl field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetUpnl

`func (o *AccountOverviewResponse) SetUpnl(v float32)`

SetUpnl sets Upnl field to given value.

### HasUpnl

`func (o *AccountOverviewResponse) HasUpnl() bool`

HasUpnl returns a boolean if a field has been set.

### GetUpnlPercentage

`func (o *AccountOverviewResponse) GetUpnlPercentage() float32`

GetUpnlPercentage returns the UpnlPercentage field if non-nil, zero value otherwise.

### GetUpnlPercentageOk

`func (o *AccountOverviewResponse) GetUpnlPercentageOk() (*float32, bool)`

GetUpnlPercentageOk returns a tuple with the UpnlPercentage field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetUpnlPercentage

`func (o *AccountOverviewResponse) SetUpnlPercentage(v float32)`

SetUpnlPercentage sets UpnlPercentage field to given value.

### HasUpnlPercentage

`func (o *AccountOverviewResponse) HasUpnlPercentage() bool`

HasUpnlPercentage returns a boolean if a field has been set.

### GetAccountLimit

`func (o *AccountOverviewResponse) GetAccountLimit() float32`

GetAccountLimit returns the AccountLimit field if non-nil, zero value otherwise.

### GetAccountLimitOk

`func (o *AccountOverviewResponse) GetAccountLimitOk() (*float32, bool)`

GetAccountLimitOk returns a tuple with the AccountLimit field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetAccountLimit

`func (o *AccountOverviewResponse) SetAccountLimit(v float32)`

SetAccountLimit sets AccountLimit field to given value.

### HasAccountLimit

`func (o *AccountOverviewResponse) HasAccountLimit() bool`

HasAccountLimit returns a boolean if a field has been set.

### GetCashBalance

`func (o *AccountOverviewResponse) GetCashBalance() float32`

GetCashBalance returns the CashBalance field if non-nil, zero value otherwise.

### GetCashBalanceOk

`func (o *AccountOverviewResponse) GetCashBalanceOk() (*float32, bool)`

GetCashBalanceOk returns a tuple with the CashBalance field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetCashBalance

`func (o *AccountOverviewResponse) SetCashBalance(v float32)`

SetCashBalance sets CashBalance field to given value.

### HasCashBalance

`func (o *AccountOverviewResponse) HasCashBalance() bool`

HasCashBalance returns a boolean if a field has been set.

### GetLineAvailable

`func (o *AccountOverviewResponse) GetLineAvailable() float32`

GetLineAvailable returns the LineAvailable field if non-nil, zero value otherwise.

### GetLineAvailableOk

`func (o *AccountOverviewResponse) GetLineAvailableOk() (*float32, bool)`

GetLineAvailableOk returns a tuple with the LineAvailable field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetLineAvailable

`func (o *AccountOverviewResponse) SetLineAvailable(v float32)`

SetLineAvailable sets LineAvailable field to given value.

### HasLineAvailable

`func (o *AccountOverviewResponse) HasLineAvailable() bool`

HasLineAvailable returns a boolean if a field has been set.


[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


