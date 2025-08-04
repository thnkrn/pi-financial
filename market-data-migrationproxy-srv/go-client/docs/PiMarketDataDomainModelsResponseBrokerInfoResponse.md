# PiMarketDataDomainModelsResponseBrokerInfoResponse

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**Status** | Pointer to **bool** |  | [optional] 
**Data** | Pointer to [**PiMarketDataDomainModelsResponseResponseData**](PiMarketDataDomainModelsResponseResponseData.md) |  | [optional] 
**Error** | Pointer to [**PiMarketDataDomainModelsResponseErrorResponse**](PiMarketDataDomainModelsResponseErrorResponse.md) |  | [optional] 

## Methods

### NewPiMarketDataDomainModelsResponseBrokerInfoResponse

`func NewPiMarketDataDomainModelsResponseBrokerInfoResponse() *PiMarketDataDomainModelsResponseBrokerInfoResponse`

NewPiMarketDataDomainModelsResponseBrokerInfoResponse instantiates a new PiMarketDataDomainModelsResponseBrokerInfoResponse object
This constructor will assign default values to properties that have it defined,
and makes sure properties required by API are set, but the set of arguments
will change when the set of required properties is changed

### NewPiMarketDataDomainModelsResponseBrokerInfoResponseWithDefaults

`func NewPiMarketDataDomainModelsResponseBrokerInfoResponseWithDefaults() *PiMarketDataDomainModelsResponseBrokerInfoResponse`

NewPiMarketDataDomainModelsResponseBrokerInfoResponseWithDefaults instantiates a new PiMarketDataDomainModelsResponseBrokerInfoResponse object
This constructor will only assign default values to properties that have it defined,
but it doesn't guarantee that properties required by API are set

### GetStatus

`func (o *PiMarketDataDomainModelsResponseBrokerInfoResponse) GetStatus() bool`

GetStatus returns the Status field if non-nil, zero value otherwise.

### GetStatusOk

`func (o *PiMarketDataDomainModelsResponseBrokerInfoResponse) GetStatusOk() (*bool, bool)`

GetStatusOk returns a tuple with the Status field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetStatus

`func (o *PiMarketDataDomainModelsResponseBrokerInfoResponse) SetStatus(v bool)`

SetStatus sets Status field to given value.

### HasStatus

`func (o *PiMarketDataDomainModelsResponseBrokerInfoResponse) HasStatus() bool`

HasStatus returns a boolean if a field has been set.

### GetData

`func (o *PiMarketDataDomainModelsResponseBrokerInfoResponse) GetData() PiMarketDataDomainModelsResponseResponseData`

GetData returns the Data field if non-nil, zero value otherwise.

### GetDataOk

`func (o *PiMarketDataDomainModelsResponseBrokerInfoResponse) GetDataOk() (*PiMarketDataDomainModelsResponseResponseData, bool)`

GetDataOk returns a tuple with the Data field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetData

`func (o *PiMarketDataDomainModelsResponseBrokerInfoResponse) SetData(v PiMarketDataDomainModelsResponseResponseData)`

SetData sets Data field to given value.

### HasData

`func (o *PiMarketDataDomainModelsResponseBrokerInfoResponse) HasData() bool`

HasData returns a boolean if a field has been set.

### GetError

`func (o *PiMarketDataDomainModelsResponseBrokerInfoResponse) GetError() PiMarketDataDomainModelsResponseErrorResponse`

GetError returns the Error field if non-nil, zero value otherwise.

### GetErrorOk

`func (o *PiMarketDataDomainModelsResponseBrokerInfoResponse) GetErrorOk() (*PiMarketDataDomainModelsResponseErrorResponse, bool)`

GetErrorOk returns a tuple with the Error field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetError

`func (o *PiMarketDataDomainModelsResponseBrokerInfoResponse) SetError(v PiMarketDataDomainModelsResponseErrorResponse)`

SetError sets Error field to given value.

### HasError

`func (o *PiMarketDataDomainModelsResponseBrokerInfoResponse) HasError() bool`

HasError returns a boolean if a field has been set.


[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


