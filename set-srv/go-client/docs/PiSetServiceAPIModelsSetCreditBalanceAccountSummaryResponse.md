# PiSetServiceAPIModelsSetCreditBalanceAccountSummaryResponse

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
**AccountValue** | **float64** |  | 
**LongMarketValue** | **float64** |  | 
**ShortMarketValue** | **float64** |  | 
**MarginLoan** | **float64** |  | 
**LongCostValue** | **float64** |  | 
**ShortCostValue** | **float64** |  | 
**ExcessEquity** | **float64** |  | 
**Liabilities** | **float64** |  | 

## Methods

### NewPiSetServiceAPIModelsSetCreditBalanceAccountSummaryResponse

`func NewPiSetServiceAPIModelsSetCreditBalanceAccountSummaryResponse(tradingAccountNo NullableString, tradingAccountType PiSetServiceDomainAggregatesModelFinancialAssetAggregateTradingAccountType, customerCode NullableString, accountCode NullableString, asOfDate NullableString, totalCost float64, totalUpnl float64, totalUpnlPercentage float64, cashBalance float64, sblEnabled bool, totalValue float64, totalMarketValue float64, assets []PiSetServiceAPIModelsSetAccountAssetsResponse, accountValue float64, longMarketValue float64, shortMarketValue float64, marginLoan float64, longCostValue float64, shortCostValue float64, excessEquity float64, liabilities float64, ) *PiSetServiceAPIModelsSetCreditBalanceAccountSummaryResponse`

NewPiSetServiceAPIModelsSetCreditBalanceAccountSummaryResponse instantiates a new PiSetServiceAPIModelsSetCreditBalanceAccountSummaryResponse object
This constructor will assign default values to properties that have it defined,
and makes sure properties required by API are set, but the set of arguments
will change when the set of required properties is changed

### NewPiSetServiceAPIModelsSetCreditBalanceAccountSummaryResponseWithDefaults

`func NewPiSetServiceAPIModelsSetCreditBalanceAccountSummaryResponseWithDefaults() *PiSetServiceAPIModelsSetCreditBalanceAccountSummaryResponse`

NewPiSetServiceAPIModelsSetCreditBalanceAccountSummaryResponseWithDefaults instantiates a new PiSetServiceAPIModelsSetCreditBalanceAccountSummaryResponse object
This constructor will only assign default values to properties that have it defined,
but it doesn't guarantee that properties required by API are set

### GetTradingAccountNo

`func (o *PiSetServiceAPIModelsSetCreditBalanceAccountSummaryResponse) GetTradingAccountNo() string`

GetTradingAccountNo returns the TradingAccountNo field if non-nil, zero value otherwise.

### GetTradingAccountNoOk

`func (o *PiSetServiceAPIModelsSetCreditBalanceAccountSummaryResponse) GetTradingAccountNoOk() (*string, bool)`

GetTradingAccountNoOk returns a tuple with the TradingAccountNo field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetTradingAccountNo

`func (o *PiSetServiceAPIModelsSetCreditBalanceAccountSummaryResponse) SetTradingAccountNo(v string)`

SetTradingAccountNo sets TradingAccountNo field to given value.


### SetTradingAccountNoNil

`func (o *PiSetServiceAPIModelsSetCreditBalanceAccountSummaryResponse) SetTradingAccountNoNil(b bool)`

 SetTradingAccountNoNil sets the value for TradingAccountNo to be an explicit nil

### UnsetTradingAccountNo
`func (o *PiSetServiceAPIModelsSetCreditBalanceAccountSummaryResponse) UnsetTradingAccountNo()`

UnsetTradingAccountNo ensures that no value is present for TradingAccountNo, not even an explicit nil
### GetTradingAccountType

`func (o *PiSetServiceAPIModelsSetCreditBalanceAccountSummaryResponse) GetTradingAccountType() PiSetServiceDomainAggregatesModelFinancialAssetAggregateTradingAccountType`

GetTradingAccountType returns the TradingAccountType field if non-nil, zero value otherwise.

### GetTradingAccountTypeOk

`func (o *PiSetServiceAPIModelsSetCreditBalanceAccountSummaryResponse) GetTradingAccountTypeOk() (*PiSetServiceDomainAggregatesModelFinancialAssetAggregateTradingAccountType, bool)`

GetTradingAccountTypeOk returns a tuple with the TradingAccountType field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetTradingAccountType

`func (o *PiSetServiceAPIModelsSetCreditBalanceAccountSummaryResponse) SetTradingAccountType(v PiSetServiceDomainAggregatesModelFinancialAssetAggregateTradingAccountType)`

SetTradingAccountType sets TradingAccountType field to given value.


### GetCustomerCode

`func (o *PiSetServiceAPIModelsSetCreditBalanceAccountSummaryResponse) GetCustomerCode() string`

GetCustomerCode returns the CustomerCode field if non-nil, zero value otherwise.

### GetCustomerCodeOk

`func (o *PiSetServiceAPIModelsSetCreditBalanceAccountSummaryResponse) GetCustomerCodeOk() (*string, bool)`

GetCustomerCodeOk returns a tuple with the CustomerCode field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetCustomerCode

`func (o *PiSetServiceAPIModelsSetCreditBalanceAccountSummaryResponse) SetCustomerCode(v string)`

SetCustomerCode sets CustomerCode field to given value.


### SetCustomerCodeNil

`func (o *PiSetServiceAPIModelsSetCreditBalanceAccountSummaryResponse) SetCustomerCodeNil(b bool)`

 SetCustomerCodeNil sets the value for CustomerCode to be an explicit nil

### UnsetCustomerCode
`func (o *PiSetServiceAPIModelsSetCreditBalanceAccountSummaryResponse) UnsetCustomerCode()`

UnsetCustomerCode ensures that no value is present for CustomerCode, not even an explicit nil
### GetAccountCode

`func (o *PiSetServiceAPIModelsSetCreditBalanceAccountSummaryResponse) GetAccountCode() string`

GetAccountCode returns the AccountCode field if non-nil, zero value otherwise.

### GetAccountCodeOk

`func (o *PiSetServiceAPIModelsSetCreditBalanceAccountSummaryResponse) GetAccountCodeOk() (*string, bool)`

GetAccountCodeOk returns a tuple with the AccountCode field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetAccountCode

`func (o *PiSetServiceAPIModelsSetCreditBalanceAccountSummaryResponse) SetAccountCode(v string)`

SetAccountCode sets AccountCode field to given value.


### SetAccountCodeNil

`func (o *PiSetServiceAPIModelsSetCreditBalanceAccountSummaryResponse) SetAccountCodeNil(b bool)`

 SetAccountCodeNil sets the value for AccountCode to be an explicit nil

### UnsetAccountCode
`func (o *PiSetServiceAPIModelsSetCreditBalanceAccountSummaryResponse) UnsetAccountCode()`

UnsetAccountCode ensures that no value is present for AccountCode, not even an explicit nil
### GetAsOfDate

`func (o *PiSetServiceAPIModelsSetCreditBalanceAccountSummaryResponse) GetAsOfDate() string`

GetAsOfDate returns the AsOfDate field if non-nil, zero value otherwise.

### GetAsOfDateOk

`func (o *PiSetServiceAPIModelsSetCreditBalanceAccountSummaryResponse) GetAsOfDateOk() (*string, bool)`

GetAsOfDateOk returns a tuple with the AsOfDate field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetAsOfDate

`func (o *PiSetServiceAPIModelsSetCreditBalanceAccountSummaryResponse) SetAsOfDate(v string)`

SetAsOfDate sets AsOfDate field to given value.


### SetAsOfDateNil

`func (o *PiSetServiceAPIModelsSetCreditBalanceAccountSummaryResponse) SetAsOfDateNil(b bool)`

 SetAsOfDateNil sets the value for AsOfDate to be an explicit nil

### UnsetAsOfDate
`func (o *PiSetServiceAPIModelsSetCreditBalanceAccountSummaryResponse) UnsetAsOfDate()`

UnsetAsOfDate ensures that no value is present for AsOfDate, not even an explicit nil
### GetTotalCost

`func (o *PiSetServiceAPIModelsSetCreditBalanceAccountSummaryResponse) GetTotalCost() float64`

GetTotalCost returns the TotalCost field if non-nil, zero value otherwise.

### GetTotalCostOk

`func (o *PiSetServiceAPIModelsSetCreditBalanceAccountSummaryResponse) GetTotalCostOk() (*float64, bool)`

GetTotalCostOk returns a tuple with the TotalCost field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetTotalCost

`func (o *PiSetServiceAPIModelsSetCreditBalanceAccountSummaryResponse) SetTotalCost(v float64)`

SetTotalCost sets TotalCost field to given value.


### GetTotalUpnl

`func (o *PiSetServiceAPIModelsSetCreditBalanceAccountSummaryResponse) GetTotalUpnl() float64`

GetTotalUpnl returns the TotalUpnl field if non-nil, zero value otherwise.

### GetTotalUpnlOk

`func (o *PiSetServiceAPIModelsSetCreditBalanceAccountSummaryResponse) GetTotalUpnlOk() (*float64, bool)`

GetTotalUpnlOk returns a tuple with the TotalUpnl field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetTotalUpnl

`func (o *PiSetServiceAPIModelsSetCreditBalanceAccountSummaryResponse) SetTotalUpnl(v float64)`

SetTotalUpnl sets TotalUpnl field to given value.


### GetTotalUpnlPercentage

`func (o *PiSetServiceAPIModelsSetCreditBalanceAccountSummaryResponse) GetTotalUpnlPercentage() float64`

GetTotalUpnlPercentage returns the TotalUpnlPercentage field if non-nil, zero value otherwise.

### GetTotalUpnlPercentageOk

`func (o *PiSetServiceAPIModelsSetCreditBalanceAccountSummaryResponse) GetTotalUpnlPercentageOk() (*float64, bool)`

GetTotalUpnlPercentageOk returns a tuple with the TotalUpnlPercentage field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetTotalUpnlPercentage

`func (o *PiSetServiceAPIModelsSetCreditBalanceAccountSummaryResponse) SetTotalUpnlPercentage(v float64)`

SetTotalUpnlPercentage sets TotalUpnlPercentage field to given value.


### GetCashBalance

`func (o *PiSetServiceAPIModelsSetCreditBalanceAccountSummaryResponse) GetCashBalance() float64`

GetCashBalance returns the CashBalance field if non-nil, zero value otherwise.

### GetCashBalanceOk

`func (o *PiSetServiceAPIModelsSetCreditBalanceAccountSummaryResponse) GetCashBalanceOk() (*float64, bool)`

GetCashBalanceOk returns a tuple with the CashBalance field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetCashBalance

`func (o *PiSetServiceAPIModelsSetCreditBalanceAccountSummaryResponse) SetCashBalance(v float64)`

SetCashBalance sets CashBalance field to given value.


### GetSblEnabled

`func (o *PiSetServiceAPIModelsSetCreditBalanceAccountSummaryResponse) GetSblEnabled() bool`

GetSblEnabled returns the SblEnabled field if non-nil, zero value otherwise.

### GetSblEnabledOk

`func (o *PiSetServiceAPIModelsSetCreditBalanceAccountSummaryResponse) GetSblEnabledOk() (*bool, bool)`

GetSblEnabledOk returns a tuple with the SblEnabled field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetSblEnabled

`func (o *PiSetServiceAPIModelsSetCreditBalanceAccountSummaryResponse) SetSblEnabled(v bool)`

SetSblEnabled sets SblEnabled field to given value.


### GetTotalValue

`func (o *PiSetServiceAPIModelsSetCreditBalanceAccountSummaryResponse) GetTotalValue() float64`

GetTotalValue returns the TotalValue field if non-nil, zero value otherwise.

### GetTotalValueOk

`func (o *PiSetServiceAPIModelsSetCreditBalanceAccountSummaryResponse) GetTotalValueOk() (*float64, bool)`

GetTotalValueOk returns a tuple with the TotalValue field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetTotalValue

`func (o *PiSetServiceAPIModelsSetCreditBalanceAccountSummaryResponse) SetTotalValue(v float64)`

SetTotalValue sets TotalValue field to given value.


### GetTotalMarketValue

`func (o *PiSetServiceAPIModelsSetCreditBalanceAccountSummaryResponse) GetTotalMarketValue() float64`

GetTotalMarketValue returns the TotalMarketValue field if non-nil, zero value otherwise.

### GetTotalMarketValueOk

`func (o *PiSetServiceAPIModelsSetCreditBalanceAccountSummaryResponse) GetTotalMarketValueOk() (*float64, bool)`

GetTotalMarketValueOk returns a tuple with the TotalMarketValue field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetTotalMarketValue

`func (o *PiSetServiceAPIModelsSetCreditBalanceAccountSummaryResponse) SetTotalMarketValue(v float64)`

SetTotalMarketValue sets TotalMarketValue field to given value.


### GetAssets

`func (o *PiSetServiceAPIModelsSetCreditBalanceAccountSummaryResponse) GetAssets() []PiSetServiceAPIModelsSetAccountAssetsResponse`

GetAssets returns the Assets field if non-nil, zero value otherwise.

### GetAssetsOk

`func (o *PiSetServiceAPIModelsSetCreditBalanceAccountSummaryResponse) GetAssetsOk() (*[]PiSetServiceAPIModelsSetAccountAssetsResponse, bool)`

GetAssetsOk returns a tuple with the Assets field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetAssets

`func (o *PiSetServiceAPIModelsSetCreditBalanceAccountSummaryResponse) SetAssets(v []PiSetServiceAPIModelsSetAccountAssetsResponse)`

SetAssets sets Assets field to given value.


### SetAssetsNil

`func (o *PiSetServiceAPIModelsSetCreditBalanceAccountSummaryResponse) SetAssetsNil(b bool)`

 SetAssetsNil sets the value for Assets to be an explicit nil

### UnsetAssets
`func (o *PiSetServiceAPIModelsSetCreditBalanceAccountSummaryResponse) UnsetAssets()`

UnsetAssets ensures that no value is present for Assets, not even an explicit nil
### GetAccountValue

`func (o *PiSetServiceAPIModelsSetCreditBalanceAccountSummaryResponse) GetAccountValue() float64`

GetAccountValue returns the AccountValue field if non-nil, zero value otherwise.

### GetAccountValueOk

`func (o *PiSetServiceAPIModelsSetCreditBalanceAccountSummaryResponse) GetAccountValueOk() (*float64, bool)`

GetAccountValueOk returns a tuple with the AccountValue field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetAccountValue

`func (o *PiSetServiceAPIModelsSetCreditBalanceAccountSummaryResponse) SetAccountValue(v float64)`

SetAccountValue sets AccountValue field to given value.


### GetLongMarketValue

`func (o *PiSetServiceAPIModelsSetCreditBalanceAccountSummaryResponse) GetLongMarketValue() float64`

GetLongMarketValue returns the LongMarketValue field if non-nil, zero value otherwise.

### GetLongMarketValueOk

`func (o *PiSetServiceAPIModelsSetCreditBalanceAccountSummaryResponse) GetLongMarketValueOk() (*float64, bool)`

GetLongMarketValueOk returns a tuple with the LongMarketValue field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetLongMarketValue

`func (o *PiSetServiceAPIModelsSetCreditBalanceAccountSummaryResponse) SetLongMarketValue(v float64)`

SetLongMarketValue sets LongMarketValue field to given value.


### GetShortMarketValue

`func (o *PiSetServiceAPIModelsSetCreditBalanceAccountSummaryResponse) GetShortMarketValue() float64`

GetShortMarketValue returns the ShortMarketValue field if non-nil, zero value otherwise.

### GetShortMarketValueOk

`func (o *PiSetServiceAPIModelsSetCreditBalanceAccountSummaryResponse) GetShortMarketValueOk() (*float64, bool)`

GetShortMarketValueOk returns a tuple with the ShortMarketValue field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetShortMarketValue

`func (o *PiSetServiceAPIModelsSetCreditBalanceAccountSummaryResponse) SetShortMarketValue(v float64)`

SetShortMarketValue sets ShortMarketValue field to given value.


### GetMarginLoan

`func (o *PiSetServiceAPIModelsSetCreditBalanceAccountSummaryResponse) GetMarginLoan() float64`

GetMarginLoan returns the MarginLoan field if non-nil, zero value otherwise.

### GetMarginLoanOk

`func (o *PiSetServiceAPIModelsSetCreditBalanceAccountSummaryResponse) GetMarginLoanOk() (*float64, bool)`

GetMarginLoanOk returns a tuple with the MarginLoan field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetMarginLoan

`func (o *PiSetServiceAPIModelsSetCreditBalanceAccountSummaryResponse) SetMarginLoan(v float64)`

SetMarginLoan sets MarginLoan field to given value.


### GetLongCostValue

`func (o *PiSetServiceAPIModelsSetCreditBalanceAccountSummaryResponse) GetLongCostValue() float64`

GetLongCostValue returns the LongCostValue field if non-nil, zero value otherwise.

### GetLongCostValueOk

`func (o *PiSetServiceAPIModelsSetCreditBalanceAccountSummaryResponse) GetLongCostValueOk() (*float64, bool)`

GetLongCostValueOk returns a tuple with the LongCostValue field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetLongCostValue

`func (o *PiSetServiceAPIModelsSetCreditBalanceAccountSummaryResponse) SetLongCostValue(v float64)`

SetLongCostValue sets LongCostValue field to given value.


### GetShortCostValue

`func (o *PiSetServiceAPIModelsSetCreditBalanceAccountSummaryResponse) GetShortCostValue() float64`

GetShortCostValue returns the ShortCostValue field if non-nil, zero value otherwise.

### GetShortCostValueOk

`func (o *PiSetServiceAPIModelsSetCreditBalanceAccountSummaryResponse) GetShortCostValueOk() (*float64, bool)`

GetShortCostValueOk returns a tuple with the ShortCostValue field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetShortCostValue

`func (o *PiSetServiceAPIModelsSetCreditBalanceAccountSummaryResponse) SetShortCostValue(v float64)`

SetShortCostValue sets ShortCostValue field to given value.


### GetExcessEquity

`func (o *PiSetServiceAPIModelsSetCreditBalanceAccountSummaryResponse) GetExcessEquity() float64`

GetExcessEquity returns the ExcessEquity field if non-nil, zero value otherwise.

### GetExcessEquityOk

`func (o *PiSetServiceAPIModelsSetCreditBalanceAccountSummaryResponse) GetExcessEquityOk() (*float64, bool)`

GetExcessEquityOk returns a tuple with the ExcessEquity field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetExcessEquity

`func (o *PiSetServiceAPIModelsSetCreditBalanceAccountSummaryResponse) SetExcessEquity(v float64)`

SetExcessEquity sets ExcessEquity field to given value.


### GetLiabilities

`func (o *PiSetServiceAPIModelsSetCreditBalanceAccountSummaryResponse) GetLiabilities() float64`

GetLiabilities returns the Liabilities field if non-nil, zero value otherwise.

### GetLiabilitiesOk

`func (o *PiSetServiceAPIModelsSetCreditBalanceAccountSummaryResponse) GetLiabilitiesOk() (*float64, bool)`

GetLiabilitiesOk returns a tuple with the Liabilities field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetLiabilities

`func (o *PiSetServiceAPIModelsSetCreditBalanceAccountSummaryResponse) SetLiabilities(v float64)`

SetLiabilities sets Liabilities field to given value.



[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


