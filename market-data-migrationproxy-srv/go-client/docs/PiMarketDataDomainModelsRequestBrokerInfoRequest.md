# PiMarketDataDomainModelsRequestBrokerInfoRequest

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**AsOfDate** | Pointer to **time.Time** |  | [optional] 
**Data** | Pointer to [**[]PiMarketDataDomainModelsRequestBrokerData**](PiMarketDataDomainModelsRequestBrokerData.md) |  | [optional] 

## Methods

### NewPiMarketDataDomainModelsRequestBrokerInfoRequest

`func NewPiMarketDataDomainModelsRequestBrokerInfoRequest() *PiMarketDataDomainModelsRequestBrokerInfoRequest`

NewPiMarketDataDomainModelsRequestBrokerInfoRequest instantiates a new PiMarketDataDomainModelsRequestBrokerInfoRequest object
This constructor will assign default values to properties that have it defined,
and makes sure properties required by API are set, but the set of arguments
will change when the set of required properties is changed

### NewPiMarketDataDomainModelsRequestBrokerInfoRequestWithDefaults

`func NewPiMarketDataDomainModelsRequestBrokerInfoRequestWithDefaults() *PiMarketDataDomainModelsRequestBrokerInfoRequest`

NewPiMarketDataDomainModelsRequestBrokerInfoRequestWithDefaults instantiates a new PiMarketDataDomainModelsRequestBrokerInfoRequest object
This constructor will only assign default values to properties that have it defined,
but it doesn't guarantee that properties required by API are set

### GetAsOfDate

`func (o *PiMarketDataDomainModelsRequestBrokerInfoRequest) GetAsOfDate() time.Time`

GetAsOfDate returns the AsOfDate field if non-nil, zero value otherwise.

### GetAsOfDateOk

`func (o *PiMarketDataDomainModelsRequestBrokerInfoRequest) GetAsOfDateOk() (*time.Time, bool)`

GetAsOfDateOk returns a tuple with the AsOfDate field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetAsOfDate

`func (o *PiMarketDataDomainModelsRequestBrokerInfoRequest) SetAsOfDate(v time.Time)`

SetAsOfDate sets AsOfDate field to given value.

### HasAsOfDate

`func (o *PiMarketDataDomainModelsRequestBrokerInfoRequest) HasAsOfDate() bool`

HasAsOfDate returns a boolean if a field has been set.

### GetData

`func (o *PiMarketDataDomainModelsRequestBrokerInfoRequest) GetData() []PiMarketDataDomainModelsRequestBrokerData`

GetData returns the Data field if non-nil, zero value otherwise.

### GetDataOk

`func (o *PiMarketDataDomainModelsRequestBrokerInfoRequest) GetDataOk() (*[]PiMarketDataDomainModelsRequestBrokerData, bool)`

GetDataOk returns a tuple with the Data field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetData

`func (o *PiMarketDataDomainModelsRequestBrokerInfoRequest) SetData(v []PiMarketDataDomainModelsRequestBrokerData)`

SetData sets Data field to given value.

### HasData

`func (o *PiMarketDataDomainModelsRequestBrokerInfoRequest) HasData() bool`

HasData returns a boolean if a field has been set.

### SetDataNil

`func (o *PiMarketDataDomainModelsRequestBrokerInfoRequest) SetDataNil(b bool)`

 SetDataNil sets the value for Data to be an explicit nil

### UnsetData
`func (o *PiMarketDataDomainModelsRequestBrokerInfoRequest) UnsetData()`

UnsetData ensures that no value is present for Data, not even an explicit nil

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


