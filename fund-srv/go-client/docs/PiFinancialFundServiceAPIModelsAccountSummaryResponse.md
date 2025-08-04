# PiFinancialFundServiceAPIModelsAccountSummaryResponse

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**CustomerCode** | **NullableString** |  | 
**TradingAccountNo** | **NullableString** |  | 
**AsOfDate** | **time.Time** |  | 
**TotalMarketValue** | **float64** |  | 
**TotalCostValue** | **float64** |  | 
**TotalUpnl** | **float64** |  | 
**Assets** | [**[]PiFinancialFundServiceAPIModelsSiriusFundAssetResponse**](PiFinancialFundServiceAPIModelsSiriusFundAssetResponse.md) |  | 

## Methods

### NewPiFinancialFundServiceAPIModelsAccountSummaryResponse

`func NewPiFinancialFundServiceAPIModelsAccountSummaryResponse(customerCode NullableString, tradingAccountNo NullableString, asOfDate time.Time, totalMarketValue float64, totalCostValue float64, totalUpnl float64, assets []PiFinancialFundServiceAPIModelsSiriusFundAssetResponse, ) *PiFinancialFundServiceAPIModelsAccountSummaryResponse`

NewPiFinancialFundServiceAPIModelsAccountSummaryResponse instantiates a new PiFinancialFundServiceAPIModelsAccountSummaryResponse object
This constructor will assign default values to properties that have it defined,
and makes sure properties required by API are set, but the set of arguments
will change when the set of required properties is changed

### NewPiFinancialFundServiceAPIModelsAccountSummaryResponseWithDefaults

`func NewPiFinancialFundServiceAPIModelsAccountSummaryResponseWithDefaults() *PiFinancialFundServiceAPIModelsAccountSummaryResponse`

NewPiFinancialFundServiceAPIModelsAccountSummaryResponseWithDefaults instantiates a new PiFinancialFundServiceAPIModelsAccountSummaryResponse object
This constructor will only assign default values to properties that have it defined,
but it doesn't guarantee that properties required by API are set

### GetCustomerCode

`func (o *PiFinancialFundServiceAPIModelsAccountSummaryResponse) GetCustomerCode() string`

GetCustomerCode returns the CustomerCode field if non-nil, zero value otherwise.

### GetCustomerCodeOk

`func (o *PiFinancialFundServiceAPIModelsAccountSummaryResponse) GetCustomerCodeOk() (*string, bool)`

GetCustomerCodeOk returns a tuple with the CustomerCode field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetCustomerCode

`func (o *PiFinancialFundServiceAPIModelsAccountSummaryResponse) SetCustomerCode(v string)`

SetCustomerCode sets CustomerCode field to given value.


### SetCustomerCodeNil

`func (o *PiFinancialFundServiceAPIModelsAccountSummaryResponse) SetCustomerCodeNil(b bool)`

 SetCustomerCodeNil sets the value for CustomerCode to be an explicit nil

### UnsetCustomerCode
`func (o *PiFinancialFundServiceAPIModelsAccountSummaryResponse) UnsetCustomerCode()`

UnsetCustomerCode ensures that no value is present for CustomerCode, not even an explicit nil
### GetTradingAccountNo

`func (o *PiFinancialFundServiceAPIModelsAccountSummaryResponse) GetTradingAccountNo() string`

GetTradingAccountNo returns the TradingAccountNo field if non-nil, zero value otherwise.

### GetTradingAccountNoOk

`func (o *PiFinancialFundServiceAPIModelsAccountSummaryResponse) GetTradingAccountNoOk() (*string, bool)`

GetTradingAccountNoOk returns a tuple with the TradingAccountNo field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetTradingAccountNo

`func (o *PiFinancialFundServiceAPIModelsAccountSummaryResponse) SetTradingAccountNo(v string)`

SetTradingAccountNo sets TradingAccountNo field to given value.


### SetTradingAccountNoNil

`func (o *PiFinancialFundServiceAPIModelsAccountSummaryResponse) SetTradingAccountNoNil(b bool)`

 SetTradingAccountNoNil sets the value for TradingAccountNo to be an explicit nil

### UnsetTradingAccountNo
`func (o *PiFinancialFundServiceAPIModelsAccountSummaryResponse) UnsetTradingAccountNo()`

UnsetTradingAccountNo ensures that no value is present for TradingAccountNo, not even an explicit nil
### GetAsOfDate

`func (o *PiFinancialFundServiceAPIModelsAccountSummaryResponse) GetAsOfDate() time.Time`

GetAsOfDate returns the AsOfDate field if non-nil, zero value otherwise.

### GetAsOfDateOk

`func (o *PiFinancialFundServiceAPIModelsAccountSummaryResponse) GetAsOfDateOk() (*time.Time, bool)`

GetAsOfDateOk returns a tuple with the AsOfDate field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetAsOfDate

`func (o *PiFinancialFundServiceAPIModelsAccountSummaryResponse) SetAsOfDate(v time.Time)`

SetAsOfDate sets AsOfDate field to given value.


### GetTotalMarketValue

`func (o *PiFinancialFundServiceAPIModelsAccountSummaryResponse) GetTotalMarketValue() float64`

GetTotalMarketValue returns the TotalMarketValue field if non-nil, zero value otherwise.

### GetTotalMarketValueOk

`func (o *PiFinancialFundServiceAPIModelsAccountSummaryResponse) GetTotalMarketValueOk() (*float64, bool)`

GetTotalMarketValueOk returns a tuple with the TotalMarketValue field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetTotalMarketValue

`func (o *PiFinancialFundServiceAPIModelsAccountSummaryResponse) SetTotalMarketValue(v float64)`

SetTotalMarketValue sets TotalMarketValue field to given value.


### GetTotalCostValue

`func (o *PiFinancialFundServiceAPIModelsAccountSummaryResponse) GetTotalCostValue() float64`

GetTotalCostValue returns the TotalCostValue field if non-nil, zero value otherwise.

### GetTotalCostValueOk

`func (o *PiFinancialFundServiceAPIModelsAccountSummaryResponse) GetTotalCostValueOk() (*float64, bool)`

GetTotalCostValueOk returns a tuple with the TotalCostValue field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetTotalCostValue

`func (o *PiFinancialFundServiceAPIModelsAccountSummaryResponse) SetTotalCostValue(v float64)`

SetTotalCostValue sets TotalCostValue field to given value.


### GetTotalUpnl

`func (o *PiFinancialFundServiceAPIModelsAccountSummaryResponse) GetTotalUpnl() float64`

GetTotalUpnl returns the TotalUpnl field if non-nil, zero value otherwise.

### GetTotalUpnlOk

`func (o *PiFinancialFundServiceAPIModelsAccountSummaryResponse) GetTotalUpnlOk() (*float64, bool)`

GetTotalUpnlOk returns a tuple with the TotalUpnl field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetTotalUpnl

`func (o *PiFinancialFundServiceAPIModelsAccountSummaryResponse) SetTotalUpnl(v float64)`

SetTotalUpnl sets TotalUpnl field to given value.


### GetAssets

`func (o *PiFinancialFundServiceAPIModelsAccountSummaryResponse) GetAssets() []PiFinancialFundServiceAPIModelsSiriusFundAssetResponse`

GetAssets returns the Assets field if non-nil, zero value otherwise.

### GetAssetsOk

`func (o *PiFinancialFundServiceAPIModelsAccountSummaryResponse) GetAssetsOk() (*[]PiFinancialFundServiceAPIModelsSiriusFundAssetResponse, bool)`

GetAssetsOk returns a tuple with the Assets field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetAssets

`func (o *PiFinancialFundServiceAPIModelsAccountSummaryResponse) SetAssets(v []PiFinancialFundServiceAPIModelsSiriusFundAssetResponse)`

SetAssets sets Assets field to given value.


### SetAssetsNil

`func (o *PiFinancialFundServiceAPIModelsAccountSummaryResponse) SetAssetsNil(b bool)`

 SetAssetsNil sets the value for Assets to be an explicit nil

### UnsetAssets
`func (o *PiFinancialFundServiceAPIModelsAccountSummaryResponse) UnsetAssets()`

UnsetAssets ensures that no value is present for Assets, not even an explicit nil

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


