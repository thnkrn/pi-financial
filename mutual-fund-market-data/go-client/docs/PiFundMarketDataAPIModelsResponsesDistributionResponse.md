# PiFundMarketDataAPIModelsResponsesDistributionResponse

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**DividendPolicy** | Pointer to **NullableString** |  | [optional] 
**ExDivDate** | Pointer to **NullableTime** |  | [optional] 
**AsOfDate** | Pointer to **time.Time** |  | [optional] 
**HistoricalDividends** | Pointer to [**[]PiFundMarketDataAPIModelsResponsesHistoricalDividendResponse**](PiFundMarketDataAPIModelsResponsesHistoricalDividendResponse.md) |  | [optional] 

## Methods

### NewPiFundMarketDataAPIModelsResponsesDistributionResponse

`func NewPiFundMarketDataAPIModelsResponsesDistributionResponse() *PiFundMarketDataAPIModelsResponsesDistributionResponse`

NewPiFundMarketDataAPIModelsResponsesDistributionResponse instantiates a new PiFundMarketDataAPIModelsResponsesDistributionResponse object
This constructor will assign default values to properties that have it defined,
and makes sure properties required by API are set, but the set of arguments
will change when the set of required properties is changed

### NewPiFundMarketDataAPIModelsResponsesDistributionResponseWithDefaults

`func NewPiFundMarketDataAPIModelsResponsesDistributionResponseWithDefaults() *PiFundMarketDataAPIModelsResponsesDistributionResponse`

NewPiFundMarketDataAPIModelsResponsesDistributionResponseWithDefaults instantiates a new PiFundMarketDataAPIModelsResponsesDistributionResponse object
This constructor will only assign default values to properties that have it defined,
but it doesn't guarantee that properties required by API are set

### GetDividendPolicy

`func (o *PiFundMarketDataAPIModelsResponsesDistributionResponse) GetDividendPolicy() string`

GetDividendPolicy returns the DividendPolicy field if non-nil, zero value otherwise.

### GetDividendPolicyOk

`func (o *PiFundMarketDataAPIModelsResponsesDistributionResponse) GetDividendPolicyOk() (*string, bool)`

GetDividendPolicyOk returns a tuple with the DividendPolicy field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetDividendPolicy

`func (o *PiFundMarketDataAPIModelsResponsesDistributionResponse) SetDividendPolicy(v string)`

SetDividendPolicy sets DividendPolicy field to given value.

### HasDividendPolicy

`func (o *PiFundMarketDataAPIModelsResponsesDistributionResponse) HasDividendPolicy() bool`

HasDividendPolicy returns a boolean if a field has been set.

### SetDividendPolicyNil

`func (o *PiFundMarketDataAPIModelsResponsesDistributionResponse) SetDividendPolicyNil(b bool)`

 SetDividendPolicyNil sets the value for DividendPolicy to be an explicit nil

### UnsetDividendPolicy
`func (o *PiFundMarketDataAPIModelsResponsesDistributionResponse) UnsetDividendPolicy()`

UnsetDividendPolicy ensures that no value is present for DividendPolicy, not even an explicit nil
### GetExDivDate

`func (o *PiFundMarketDataAPIModelsResponsesDistributionResponse) GetExDivDate() time.Time`

GetExDivDate returns the ExDivDate field if non-nil, zero value otherwise.

### GetExDivDateOk

`func (o *PiFundMarketDataAPIModelsResponsesDistributionResponse) GetExDivDateOk() (*time.Time, bool)`

GetExDivDateOk returns a tuple with the ExDivDate field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetExDivDate

`func (o *PiFundMarketDataAPIModelsResponsesDistributionResponse) SetExDivDate(v time.Time)`

SetExDivDate sets ExDivDate field to given value.

### HasExDivDate

`func (o *PiFundMarketDataAPIModelsResponsesDistributionResponse) HasExDivDate() bool`

HasExDivDate returns a boolean if a field has been set.

### SetExDivDateNil

`func (o *PiFundMarketDataAPIModelsResponsesDistributionResponse) SetExDivDateNil(b bool)`

 SetExDivDateNil sets the value for ExDivDate to be an explicit nil

### UnsetExDivDate
`func (o *PiFundMarketDataAPIModelsResponsesDistributionResponse) UnsetExDivDate()`

UnsetExDivDate ensures that no value is present for ExDivDate, not even an explicit nil
### GetAsOfDate

`func (o *PiFundMarketDataAPIModelsResponsesDistributionResponse) GetAsOfDate() time.Time`

GetAsOfDate returns the AsOfDate field if non-nil, zero value otherwise.

### GetAsOfDateOk

`func (o *PiFundMarketDataAPIModelsResponsesDistributionResponse) GetAsOfDateOk() (*time.Time, bool)`

GetAsOfDateOk returns a tuple with the AsOfDate field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetAsOfDate

`func (o *PiFundMarketDataAPIModelsResponsesDistributionResponse) SetAsOfDate(v time.Time)`

SetAsOfDate sets AsOfDate field to given value.

### HasAsOfDate

`func (o *PiFundMarketDataAPIModelsResponsesDistributionResponse) HasAsOfDate() bool`

HasAsOfDate returns a boolean if a field has been set.

### GetHistoricalDividends

`func (o *PiFundMarketDataAPIModelsResponsesDistributionResponse) GetHistoricalDividends() []PiFundMarketDataAPIModelsResponsesHistoricalDividendResponse`

GetHistoricalDividends returns the HistoricalDividends field if non-nil, zero value otherwise.

### GetHistoricalDividendsOk

`func (o *PiFundMarketDataAPIModelsResponsesDistributionResponse) GetHistoricalDividendsOk() (*[]PiFundMarketDataAPIModelsResponsesHistoricalDividendResponse, bool)`

GetHistoricalDividendsOk returns a tuple with the HistoricalDividends field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetHistoricalDividends

`func (o *PiFundMarketDataAPIModelsResponsesDistributionResponse) SetHistoricalDividends(v []PiFundMarketDataAPIModelsResponsesHistoricalDividendResponse)`

SetHistoricalDividends sets HistoricalDividends field to given value.

### HasHistoricalDividends

`func (o *PiFundMarketDataAPIModelsResponsesDistributionResponse) HasHistoricalDividends() bool`

HasHistoricalDividends returns a boolean if a field has been set.

### SetHistoricalDividendsNil

`func (o *PiFundMarketDataAPIModelsResponsesDistributionResponse) SetHistoricalDividendsNil(b bool)`

 SetHistoricalDividendsNil sets the value for HistoricalDividends to be an explicit nil

### UnsetHistoricalDividends
`func (o *PiFundMarketDataAPIModelsResponsesDistributionResponse) UnsetHistoricalDividends()`

UnsetHistoricalDividends ensures that no value is present for HistoricalDividends, not even an explicit nil

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


