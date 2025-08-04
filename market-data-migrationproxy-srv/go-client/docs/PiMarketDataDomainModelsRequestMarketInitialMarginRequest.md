# PiMarketDataDomainModelsRequestMarketInitialMarginRequest

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**AsOfDate** | Pointer to **time.Time** |  | [optional] 
**Data** | Pointer to [**[]PiMarketDataDomainModelsRequestInitialMarginData**](PiMarketDataDomainModelsRequestInitialMarginData.md) |  | [optional] 

## Methods

### NewPiMarketDataDomainModelsRequestMarketInitialMarginRequest

`func NewPiMarketDataDomainModelsRequestMarketInitialMarginRequest() *PiMarketDataDomainModelsRequestMarketInitialMarginRequest`

NewPiMarketDataDomainModelsRequestMarketInitialMarginRequest instantiates a new PiMarketDataDomainModelsRequestMarketInitialMarginRequest object
This constructor will assign default values to properties that have it defined,
and makes sure properties required by API are set, but the set of arguments
will change when the set of required properties is changed

### NewPiMarketDataDomainModelsRequestMarketInitialMarginRequestWithDefaults

`func NewPiMarketDataDomainModelsRequestMarketInitialMarginRequestWithDefaults() *PiMarketDataDomainModelsRequestMarketInitialMarginRequest`

NewPiMarketDataDomainModelsRequestMarketInitialMarginRequestWithDefaults instantiates a new PiMarketDataDomainModelsRequestMarketInitialMarginRequest object
This constructor will only assign default values to properties that have it defined,
but it doesn't guarantee that properties required by API are set

### GetAsOfDate

`func (o *PiMarketDataDomainModelsRequestMarketInitialMarginRequest) GetAsOfDate() time.Time`

GetAsOfDate returns the AsOfDate field if non-nil, zero value otherwise.

### GetAsOfDateOk

`func (o *PiMarketDataDomainModelsRequestMarketInitialMarginRequest) GetAsOfDateOk() (*time.Time, bool)`

GetAsOfDateOk returns a tuple with the AsOfDate field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetAsOfDate

`func (o *PiMarketDataDomainModelsRequestMarketInitialMarginRequest) SetAsOfDate(v time.Time)`

SetAsOfDate sets AsOfDate field to given value.

### HasAsOfDate

`func (o *PiMarketDataDomainModelsRequestMarketInitialMarginRequest) HasAsOfDate() bool`

HasAsOfDate returns a boolean if a field has been set.

### GetData

`func (o *PiMarketDataDomainModelsRequestMarketInitialMarginRequest) GetData() []PiMarketDataDomainModelsRequestInitialMarginData`

GetData returns the Data field if non-nil, zero value otherwise.

### GetDataOk

`func (o *PiMarketDataDomainModelsRequestMarketInitialMarginRequest) GetDataOk() (*[]PiMarketDataDomainModelsRequestInitialMarginData, bool)`

GetDataOk returns a tuple with the Data field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetData

`func (o *PiMarketDataDomainModelsRequestMarketInitialMarginRequest) SetData(v []PiMarketDataDomainModelsRequestInitialMarginData)`

SetData sets Data field to given value.

### HasData

`func (o *PiMarketDataDomainModelsRequestMarketInitialMarginRequest) HasData() bool`

HasData returns a boolean if a field has been set.

### SetDataNil

`func (o *PiMarketDataDomainModelsRequestMarketInitialMarginRequest) SetDataNil(b bool)`

 SetDataNil sets the value for Data to be an explicit nil

### UnsetData
`func (o *PiMarketDataDomainModelsRequestMarketInitialMarginRequest) UnsetData()`

UnsetData ensures that no value is present for Data, not even an explicit nil

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


