# AccountSummaryResponse

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**TradingAccountNo** | Pointer to **NullableString** | CustCode-2 | [optional] 
**TradingAccountNoDisplay** | Pointer to **NullableString** |  | [optional] 
**UpnlPercentage** | Pointer to **float32** |  | [optional] 
**ExchangeRate** | Pointer to [**ExchangeRate**](ExchangeRate.md) |  | [optional] 
**Values** | Pointer to [**[]AccountSummaryValueResponse**](AccountSummaryValueResponse.md) |  | [optional] 
**Positions** | Pointer to [**[]PositionResponse**](PositionResponse.md) |  | [optional] 

## Methods

### NewAccountSummaryResponse

`func NewAccountSummaryResponse() *AccountSummaryResponse`

NewAccountSummaryResponse instantiates a new AccountSummaryResponse object
This constructor will assign default values to properties that have it defined,
and makes sure properties required by API are set, but the set of arguments
will change when the set of required properties is changed

### NewAccountSummaryResponseWithDefaults

`func NewAccountSummaryResponseWithDefaults() *AccountSummaryResponse`

NewAccountSummaryResponseWithDefaults instantiates a new AccountSummaryResponse object
This constructor will only assign default values to properties that have it defined,
but it doesn't guarantee that properties required by API are set

### GetTradingAccountNo

`func (o *AccountSummaryResponse) GetTradingAccountNo() string`

GetTradingAccountNo returns the TradingAccountNo field if non-nil, zero value otherwise.

### GetTradingAccountNoOk

`func (o *AccountSummaryResponse) GetTradingAccountNoOk() (*string, bool)`

GetTradingAccountNoOk returns a tuple with the TradingAccountNo field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetTradingAccountNo

`func (o *AccountSummaryResponse) SetTradingAccountNo(v string)`

SetTradingAccountNo sets TradingAccountNo field to given value.

### HasTradingAccountNo

`func (o *AccountSummaryResponse) HasTradingAccountNo() bool`

HasTradingAccountNo returns a boolean if a field has been set.

### SetTradingAccountNoNil

`func (o *AccountSummaryResponse) SetTradingAccountNoNil(b bool)`

 SetTradingAccountNoNil sets the value for TradingAccountNo to be an explicit nil

### UnsetTradingAccountNo
`func (o *AccountSummaryResponse) UnsetTradingAccountNo()`

UnsetTradingAccountNo ensures that no value is present for TradingAccountNo, not even an explicit nil
### GetTradingAccountNoDisplay

`func (o *AccountSummaryResponse) GetTradingAccountNoDisplay() string`

GetTradingAccountNoDisplay returns the TradingAccountNoDisplay field if non-nil, zero value otherwise.

### GetTradingAccountNoDisplayOk

`func (o *AccountSummaryResponse) GetTradingAccountNoDisplayOk() (*string, bool)`

GetTradingAccountNoDisplayOk returns a tuple with the TradingAccountNoDisplay field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetTradingAccountNoDisplay

`func (o *AccountSummaryResponse) SetTradingAccountNoDisplay(v string)`

SetTradingAccountNoDisplay sets TradingAccountNoDisplay field to given value.

### HasTradingAccountNoDisplay

`func (o *AccountSummaryResponse) HasTradingAccountNoDisplay() bool`

HasTradingAccountNoDisplay returns a boolean if a field has been set.

### SetTradingAccountNoDisplayNil

`func (o *AccountSummaryResponse) SetTradingAccountNoDisplayNil(b bool)`

 SetTradingAccountNoDisplayNil sets the value for TradingAccountNoDisplay to be an explicit nil

### UnsetTradingAccountNoDisplay
`func (o *AccountSummaryResponse) UnsetTradingAccountNoDisplay()`

UnsetTradingAccountNoDisplay ensures that no value is present for TradingAccountNoDisplay, not even an explicit nil
### GetUpnlPercentage

`func (o *AccountSummaryResponse) GetUpnlPercentage() float32`

GetUpnlPercentage returns the UpnlPercentage field if non-nil, zero value otherwise.

### GetUpnlPercentageOk

`func (o *AccountSummaryResponse) GetUpnlPercentageOk() (*float32, bool)`

GetUpnlPercentageOk returns a tuple with the UpnlPercentage field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetUpnlPercentage

`func (o *AccountSummaryResponse) SetUpnlPercentage(v float32)`

SetUpnlPercentage sets UpnlPercentage field to given value.

### HasUpnlPercentage

`func (o *AccountSummaryResponse) HasUpnlPercentage() bool`

HasUpnlPercentage returns a boolean if a field has been set.

### GetExchangeRate

`func (o *AccountSummaryResponse) GetExchangeRate() ExchangeRate`

GetExchangeRate returns the ExchangeRate field if non-nil, zero value otherwise.

### GetExchangeRateOk

`func (o *AccountSummaryResponse) GetExchangeRateOk() (*ExchangeRate, bool)`

GetExchangeRateOk returns a tuple with the ExchangeRate field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetExchangeRate

`func (o *AccountSummaryResponse) SetExchangeRate(v ExchangeRate)`

SetExchangeRate sets ExchangeRate field to given value.

### HasExchangeRate

`func (o *AccountSummaryResponse) HasExchangeRate() bool`

HasExchangeRate returns a boolean if a field has been set.

### GetValues

`func (o *AccountSummaryResponse) GetValues() []AccountSummaryValueResponse`

GetValues returns the Values field if non-nil, zero value otherwise.

### GetValuesOk

`func (o *AccountSummaryResponse) GetValuesOk() (*[]AccountSummaryValueResponse, bool)`

GetValuesOk returns a tuple with the Values field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetValues

`func (o *AccountSummaryResponse) SetValues(v []AccountSummaryValueResponse)`

SetValues sets Values field to given value.

### HasValues

`func (o *AccountSummaryResponse) HasValues() bool`

HasValues returns a boolean if a field has been set.

### SetValuesNil

`func (o *AccountSummaryResponse) SetValuesNil(b bool)`

 SetValuesNil sets the value for Values to be an explicit nil

### UnsetValues
`func (o *AccountSummaryResponse) UnsetValues()`

UnsetValues ensures that no value is present for Values, not even an explicit nil
### GetPositions

`func (o *AccountSummaryResponse) GetPositions() []PositionResponse`

GetPositions returns the Positions field if non-nil, zero value otherwise.

### GetPositionsOk

`func (o *AccountSummaryResponse) GetPositionsOk() (*[]PositionResponse, bool)`

GetPositionsOk returns a tuple with the Positions field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetPositions

`func (o *AccountSummaryResponse) SetPositions(v []PositionResponse)`

SetPositions sets Positions field to given value.

### HasPositions

`func (o *AccountSummaryResponse) HasPositions() bool`

HasPositions returns a boolean if a field has been set.

### SetPositionsNil

`func (o *AccountSummaryResponse) SetPositionsNil(b bool)`

 SetPositionsNil sets the value for Positions to be an explicit nil

### UnsetPositions
`func (o *AccountSummaryResponse) UnsetPositions()`

UnsetPositions ensures that no value is present for Positions, not even an explicit nil

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


