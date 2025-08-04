# PiSetServiceAPIModelsSetAccountSummaryResponse

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**TradingAccountNo** | **NullableString** |  | 
**TradingAccountType** | [**PiSetServiceDomainAggregatesModelFinancialAssetAggregateTradingAccountType**](PiSetServiceDomainAggregatesModelFinancialAssetAggregateTradingAccountType.md) |  | 
**CustomerCode** | **NullableString** |  | 
**AccountCode** | **NullableString** |  | 
**AsOfDate** | **NullableString** |  | 
**TotalCost** | **float64** |  | 
**TotalUpnl** | **float64** |  | 
**TotalUpnlPercentage** | **float64** |  | 
**CashBalance** | **float64** |  | 
**SblEnabled** | **bool** |  | 
**TotalValue** | **float64** |  | 
**TotalMarketValue** | **float64** |  | 
**Assets** | [**[]PiSetServiceAPIModelsSetAccountAssetsResponse**](PiSetServiceAPIModelsSetAccountAssetsResponse.md) |  | 

## Methods

### NewPiSetServiceAPIModelsSetAccountSummaryResponse

`func NewPiSetServiceAPIModelsSetAccountSummaryResponse(tradingAccountNo NullableString, tradingAccountType PiSetServiceDomainAggregatesModelFinancialAssetAggregateTradingAccountType, customerCode NullableString, accountCode NullableString, asOfDate NullableString, totalCost float64, totalUpnl float64, totalUpnlPercentage float64, cashBalance float64, sblEnabled bool, totalValue float64, totalMarketValue float64, assets []PiSetServiceAPIModelsSetAccountAssetsResponse, ) *PiSetServiceAPIModelsSetAccountSummaryResponse`

NewPiSetServiceAPIModelsSetAccountSummaryResponse instantiates a new PiSetServiceAPIModelsSetAccountSummaryResponse object
This constructor will assign default values to properties that have it defined,
and makes sure properties required by API are set, but the set of arguments
will change when the set of required properties is changed

### NewPiSetServiceAPIModelsSetAccountSummaryResponseWithDefaults

`func NewPiSetServiceAPIModelsSetAccountSummaryResponseWithDefaults() *PiSetServiceAPIModelsSetAccountSummaryResponse`

NewPiSetServiceAPIModelsSetAccountSummaryResponseWithDefaults instantiates a new PiSetServiceAPIModelsSetAccountSummaryResponse object
This constructor will only assign default values to properties that have it defined,
but it doesn't guarantee that properties required by API are set

### GetTradingAccountNo

`func (o *PiSetServiceAPIModelsSetAccountSummaryResponse) GetTradingAccountNo() string`

GetTradingAccountNo returns the TradingAccountNo field if non-nil, zero value otherwise.

### GetTradingAccountNoOk

`func (o *PiSetServiceAPIModelsSetAccountSummaryResponse) GetTradingAccountNoOk() (*string, bool)`

GetTradingAccountNoOk returns a tuple with the TradingAccountNo field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetTradingAccountNo

`func (o *PiSetServiceAPIModelsSetAccountSummaryResponse) SetTradingAccountNo(v string)`

SetTradingAccountNo sets TradingAccountNo field to given value.


### SetTradingAccountNoNil

`func (o *PiSetServiceAPIModelsSetAccountSummaryResponse) SetTradingAccountNoNil(b bool)`

 SetTradingAccountNoNil sets the value for TradingAccountNo to be an explicit nil

### UnsetTradingAccountNo
`func (o *PiSetServiceAPIModelsSetAccountSummaryResponse) UnsetTradingAccountNo()`

UnsetTradingAccountNo ensures that no value is present for TradingAccountNo, not even an explicit nil
### GetTradingAccountType

`func (o *PiSetServiceAPIModelsSetAccountSummaryResponse) GetTradingAccountType() PiSetServiceDomainAggregatesModelFinancialAssetAggregateTradingAccountType`

GetTradingAccountType returns the TradingAccountType field if non-nil, zero value otherwise.

### GetTradingAccountTypeOk

`func (o *PiSetServiceAPIModelsSetAccountSummaryResponse) GetTradingAccountTypeOk() (*PiSetServiceDomainAggregatesModelFinancialAssetAggregateTradingAccountType, bool)`

GetTradingAccountTypeOk returns a tuple with the TradingAccountType field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetTradingAccountType

`func (o *PiSetServiceAPIModelsSetAccountSummaryResponse) SetTradingAccountType(v PiSetServiceDomainAggregatesModelFinancialAssetAggregateTradingAccountType)`

SetTradingAccountType sets TradingAccountType field to given value.


### GetCustomerCode

`func (o *PiSetServiceAPIModelsSetAccountSummaryResponse) GetCustomerCode() string`

GetCustomerCode returns the CustomerCode field if non-nil, zero value otherwise.

### GetCustomerCodeOk

`func (o *PiSetServiceAPIModelsSetAccountSummaryResponse) GetCustomerCodeOk() (*string, bool)`

GetCustomerCodeOk returns a tuple with the CustomerCode field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetCustomerCode

`func (o *PiSetServiceAPIModelsSetAccountSummaryResponse) SetCustomerCode(v string)`

SetCustomerCode sets CustomerCode field to given value.


### SetCustomerCodeNil

`func (o *PiSetServiceAPIModelsSetAccountSummaryResponse) SetCustomerCodeNil(b bool)`

 SetCustomerCodeNil sets the value for CustomerCode to be an explicit nil

### UnsetCustomerCode
`func (o *PiSetServiceAPIModelsSetAccountSummaryResponse) UnsetCustomerCode()`

UnsetCustomerCode ensures that no value is present for CustomerCode, not even an explicit nil
### GetAccountCode

`func (o *PiSetServiceAPIModelsSetAccountSummaryResponse) GetAccountCode() string`

GetAccountCode returns the AccountCode field if non-nil, zero value otherwise.

### GetAccountCodeOk

`func (o *PiSetServiceAPIModelsSetAccountSummaryResponse) GetAccountCodeOk() (*string, bool)`

GetAccountCodeOk returns a tuple with the AccountCode field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetAccountCode

`func (o *PiSetServiceAPIModelsSetAccountSummaryResponse) SetAccountCode(v string)`

SetAccountCode sets AccountCode field to given value.


### SetAccountCodeNil

`func (o *PiSetServiceAPIModelsSetAccountSummaryResponse) SetAccountCodeNil(b bool)`

 SetAccountCodeNil sets the value for AccountCode to be an explicit nil

### UnsetAccountCode
`func (o *PiSetServiceAPIModelsSetAccountSummaryResponse) UnsetAccountCode()`

UnsetAccountCode ensures that no value is present for AccountCode, not even an explicit nil
### GetAsOfDate

`func (o *PiSetServiceAPIModelsSetAccountSummaryResponse) GetAsOfDate() string`

GetAsOfDate returns the AsOfDate field if non-nil, zero value otherwise.

### GetAsOfDateOk

`func (o *PiSetServiceAPIModelsSetAccountSummaryResponse) GetAsOfDateOk() (*string, bool)`

GetAsOfDateOk returns a tuple with the AsOfDate field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetAsOfDate

`func (o *PiSetServiceAPIModelsSetAccountSummaryResponse) SetAsOfDate(v string)`

SetAsOfDate sets AsOfDate field to given value.


### SetAsOfDateNil

`func (o *PiSetServiceAPIModelsSetAccountSummaryResponse) SetAsOfDateNil(b bool)`

 SetAsOfDateNil sets the value for AsOfDate to be an explicit nil

### UnsetAsOfDate
`func (o *PiSetServiceAPIModelsSetAccountSummaryResponse) UnsetAsOfDate()`

UnsetAsOfDate ensures that no value is present for AsOfDate, not even an explicit nil
### GetTotalCost

`func (o *PiSetServiceAPIModelsSetAccountSummaryResponse) GetTotalCost() float64`

GetTotalCost returns the TotalCost field if non-nil, zero value otherwise.

### GetTotalCostOk

`func (o *PiSetServiceAPIModelsSetAccountSummaryResponse) GetTotalCostOk() (*float64, bool)`

GetTotalCostOk returns a tuple with the TotalCost field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetTotalCost

`func (o *PiSetServiceAPIModelsSetAccountSummaryResponse) SetTotalCost(v float64)`

SetTotalCost sets TotalCost field to given value.


### GetTotalUpnl

`func (o *PiSetServiceAPIModelsSetAccountSummaryResponse) GetTotalUpnl() float64`

GetTotalUpnl returns the TotalUpnl field if non-nil, zero value otherwise.

### GetTotalUpnlOk

`func (o *PiSetServiceAPIModelsSetAccountSummaryResponse) GetTotalUpnlOk() (*float64, bool)`

GetTotalUpnlOk returns a tuple with the TotalUpnl field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetTotalUpnl

`func (o *PiSetServiceAPIModelsSetAccountSummaryResponse) SetTotalUpnl(v float64)`

SetTotalUpnl sets TotalUpnl field to given value.


### GetTotalUpnlPercentage

`func (o *PiSetServiceAPIModelsSetAccountSummaryResponse) GetTotalUpnlPercentage() float64`

GetTotalUpnlPercentage returns the TotalUpnlPercentage field if non-nil, zero value otherwise.

### GetTotalUpnlPercentageOk

`func (o *PiSetServiceAPIModelsSetAccountSummaryResponse) GetTotalUpnlPercentageOk() (*float64, bool)`

GetTotalUpnlPercentageOk returns a tuple with the TotalUpnlPercentage field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetTotalUpnlPercentage

`func (o *PiSetServiceAPIModelsSetAccountSummaryResponse) SetTotalUpnlPercentage(v float64)`

SetTotalUpnlPercentage sets TotalUpnlPercentage field to given value.


### GetCashBalance

`func (o *PiSetServiceAPIModelsSetAccountSummaryResponse) GetCashBalance() float64`

GetCashBalance returns the CashBalance field if non-nil, zero value otherwise.

### GetCashBalanceOk

`func (o *PiSetServiceAPIModelsSetAccountSummaryResponse) GetCashBalanceOk() (*float64, bool)`

GetCashBalanceOk returns a tuple with the CashBalance field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetCashBalance

`func (o *PiSetServiceAPIModelsSetAccountSummaryResponse) SetCashBalance(v float64)`

SetCashBalance sets CashBalance field to given value.


### GetSblEnabled

`func (o *PiSetServiceAPIModelsSetAccountSummaryResponse) GetSblEnabled() bool`

GetSblEnabled returns the SblEnabled field if non-nil, zero value otherwise.

### GetSblEnabledOk

`func (o *PiSetServiceAPIModelsSetAccountSummaryResponse) GetSblEnabledOk() (*bool, bool)`

GetSblEnabledOk returns a tuple with the SblEnabled field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetSblEnabled

`func (o *PiSetServiceAPIModelsSetAccountSummaryResponse) SetSblEnabled(v bool)`

SetSblEnabled sets SblEnabled field to given value.


### GetTotalValue

`func (o *PiSetServiceAPIModelsSetAccountSummaryResponse) GetTotalValue() float64`

GetTotalValue returns the TotalValue field if non-nil, zero value otherwise.

### GetTotalValueOk

`func (o *PiSetServiceAPIModelsSetAccountSummaryResponse) GetTotalValueOk() (*float64, bool)`

GetTotalValueOk returns a tuple with the TotalValue field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetTotalValue

`func (o *PiSetServiceAPIModelsSetAccountSummaryResponse) SetTotalValue(v float64)`

SetTotalValue sets TotalValue field to given value.


### GetTotalMarketValue

`func (o *PiSetServiceAPIModelsSetAccountSummaryResponse) GetTotalMarketValue() float64`

GetTotalMarketValue returns the TotalMarketValue field if non-nil, zero value otherwise.

### GetTotalMarketValueOk

`func (o *PiSetServiceAPIModelsSetAccountSummaryResponse) GetTotalMarketValueOk() (*float64, bool)`

GetTotalMarketValueOk returns a tuple with the TotalMarketValue field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetTotalMarketValue

`func (o *PiSetServiceAPIModelsSetAccountSummaryResponse) SetTotalMarketValue(v float64)`

SetTotalMarketValue sets TotalMarketValue field to given value.


### GetAssets

`func (o *PiSetServiceAPIModelsSetAccountSummaryResponse) GetAssets() []PiSetServiceAPIModelsSetAccountAssetsResponse`

GetAssets returns the Assets field if non-nil, zero value otherwise.

### GetAssetsOk

`func (o *PiSetServiceAPIModelsSetAccountSummaryResponse) GetAssetsOk() (*[]PiSetServiceAPIModelsSetAccountAssetsResponse, bool)`

GetAssetsOk returns a tuple with the Assets field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetAssets

`func (o *PiSetServiceAPIModelsSetAccountSummaryResponse) SetAssets(v []PiSetServiceAPIModelsSetAccountAssetsResponse)`

SetAssets sets Assets field to given value.


### SetAssetsNil

`func (o *PiSetServiceAPIModelsSetAccountSummaryResponse) SetAssetsNil(b bool)`

 SetAssetsNil sets the value for Assets to be an explicit nil

### UnsetAssets
`func (o *PiSetServiceAPIModelsSetAccountSummaryResponse) UnsetAssets()`

UnsetAssets ensures that no value is present for Assets, not even an explicit nil

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


