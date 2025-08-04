# AccountSummaryValueResponse

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**Currency** | Pointer to [**Currency**](Currency.md) |  | [optional] 
**NetAssetValue** | Pointer to **float32** |  | [optional] 
**MarketValue** | Pointer to **float32** |  | [optional] 
**Cost** | Pointer to **float32** |  | [optional] 
**Upnl** | Pointer to **float32** |  | [optional] 
**UnusedCash** | Pointer to **float32** |  | [optional] 
**AccountLimit** | Pointer to **float32** |  | [optional] 
**LineAvailable** | Pointer to **float32** |  | [optional] 

## Methods

### NewAccountSummaryValueResponse

`func NewAccountSummaryValueResponse() *AccountSummaryValueResponse`

NewAccountSummaryValueResponse instantiates a new AccountSummaryValueResponse object
This constructor will assign default values to properties that have it defined,
and makes sure properties required by API are set, but the set of arguments
will change when the set of required properties is changed

### NewAccountSummaryValueResponseWithDefaults

`func NewAccountSummaryValueResponseWithDefaults() *AccountSummaryValueResponse`

NewAccountSummaryValueResponseWithDefaults instantiates a new AccountSummaryValueResponse object
This constructor will only assign default values to properties that have it defined,
but it doesn't guarantee that properties required by API are set

### GetCurrency

`func (o *AccountSummaryValueResponse) GetCurrency() Currency`

GetCurrency returns the Currency field if non-nil, zero value otherwise.

### GetCurrencyOk

`func (o *AccountSummaryValueResponse) GetCurrencyOk() (*Currency, bool)`

GetCurrencyOk returns a tuple with the Currency field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetCurrency

`func (o *AccountSummaryValueResponse) SetCurrency(v Currency)`

SetCurrency sets Currency field to given value.

### HasCurrency

`func (o *AccountSummaryValueResponse) HasCurrency() bool`

HasCurrency returns a boolean if a field has been set.

### GetNetAssetValue

`func (o *AccountSummaryValueResponse) GetNetAssetValue() float32`

GetNetAssetValue returns the NetAssetValue field if non-nil, zero value otherwise.

### GetNetAssetValueOk

`func (o *AccountSummaryValueResponse) GetNetAssetValueOk() (*float32, bool)`

GetNetAssetValueOk returns a tuple with the NetAssetValue field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetNetAssetValue

`func (o *AccountSummaryValueResponse) SetNetAssetValue(v float32)`

SetNetAssetValue sets NetAssetValue field to given value.

### HasNetAssetValue

`func (o *AccountSummaryValueResponse) HasNetAssetValue() bool`

HasNetAssetValue returns a boolean if a field has been set.

### GetMarketValue

`func (o *AccountSummaryValueResponse) GetMarketValue() float32`

GetMarketValue returns the MarketValue field if non-nil, zero value otherwise.

### GetMarketValueOk

`func (o *AccountSummaryValueResponse) GetMarketValueOk() (*float32, bool)`

GetMarketValueOk returns a tuple with the MarketValue field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetMarketValue

`func (o *AccountSummaryValueResponse) SetMarketValue(v float32)`

SetMarketValue sets MarketValue field to given value.

### HasMarketValue

`func (o *AccountSummaryValueResponse) HasMarketValue() bool`

HasMarketValue returns a boolean if a field has been set.

### GetCost

`func (o *AccountSummaryValueResponse) GetCost() float32`

GetCost returns the Cost field if non-nil, zero value otherwise.

### GetCostOk

`func (o *AccountSummaryValueResponse) GetCostOk() (*float32, bool)`

GetCostOk returns a tuple with the Cost field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetCost

`func (o *AccountSummaryValueResponse) SetCost(v float32)`

SetCost sets Cost field to given value.

### HasCost

`func (o *AccountSummaryValueResponse) HasCost() bool`

HasCost returns a boolean if a field has been set.

### GetUpnl

`func (o *AccountSummaryValueResponse) GetUpnl() float32`

GetUpnl returns the Upnl field if non-nil, zero value otherwise.

### GetUpnlOk

`func (o *AccountSummaryValueResponse) GetUpnlOk() (*float32, bool)`

GetUpnlOk returns a tuple with the Upnl field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetUpnl

`func (o *AccountSummaryValueResponse) SetUpnl(v float32)`

SetUpnl sets Upnl field to given value.

### HasUpnl

`func (o *AccountSummaryValueResponse) HasUpnl() bool`

HasUpnl returns a boolean if a field has been set.

### GetUnusedCash

`func (o *AccountSummaryValueResponse) GetUnusedCash() float32`

GetUnusedCash returns the UnusedCash field if non-nil, zero value otherwise.

### GetUnusedCashOk

`func (o *AccountSummaryValueResponse) GetUnusedCashOk() (*float32, bool)`

GetUnusedCashOk returns a tuple with the UnusedCash field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetUnusedCash

`func (o *AccountSummaryValueResponse) SetUnusedCash(v float32)`

SetUnusedCash sets UnusedCash field to given value.

### HasUnusedCash

`func (o *AccountSummaryValueResponse) HasUnusedCash() bool`

HasUnusedCash returns a boolean if a field has been set.

### GetAccountLimit

`func (o *AccountSummaryValueResponse) GetAccountLimit() float32`

GetAccountLimit returns the AccountLimit field if non-nil, zero value otherwise.

### GetAccountLimitOk

`func (o *AccountSummaryValueResponse) GetAccountLimitOk() (*float32, bool)`

GetAccountLimitOk returns a tuple with the AccountLimit field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetAccountLimit

`func (o *AccountSummaryValueResponse) SetAccountLimit(v float32)`

SetAccountLimit sets AccountLimit field to given value.

### HasAccountLimit

`func (o *AccountSummaryValueResponse) HasAccountLimit() bool`

HasAccountLimit returns a boolean if a field has been set.

### GetLineAvailable

`func (o *AccountSummaryValueResponse) GetLineAvailable() float32`

GetLineAvailable returns the LineAvailable field if non-nil, zero value otherwise.

### GetLineAvailableOk

`func (o *AccountSummaryValueResponse) GetLineAvailableOk() (*float32, bool)`

GetLineAvailableOk returns a tuple with the LineAvailable field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetLineAvailable

`func (o *AccountSummaryValueResponse) SetLineAvailable(v float32)`

SetLineAvailable sets LineAvailable field to given value.

### HasLineAvailable

`func (o *AccountSummaryValueResponse) HasLineAvailable() bool`

HasLineAvailable returns a boolean if a field has been set.


[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


