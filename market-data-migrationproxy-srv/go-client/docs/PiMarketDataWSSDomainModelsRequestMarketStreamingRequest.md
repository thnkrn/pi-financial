# PiMarketDataWSSDomainModelsRequestMarketStreamingRequest

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**Data** | Pointer to [**PiMarketDataWSSDomainModelsRequestData**](PiMarketDataWSSDomainModelsRequestData.md) |  | [optional] 
**Op** | Pointer to **NullableString** |  | [optional] 
**SessionId** | Pointer to **NullableString** |  | [optional] 

## Methods

### NewPiMarketDataWSSDomainModelsRequestMarketStreamingRequest

`func NewPiMarketDataWSSDomainModelsRequestMarketStreamingRequest() *PiMarketDataWSSDomainModelsRequestMarketStreamingRequest`

NewPiMarketDataWSSDomainModelsRequestMarketStreamingRequest instantiates a new PiMarketDataWSSDomainModelsRequestMarketStreamingRequest object
This constructor will assign default values to properties that have it defined,
and makes sure properties required by API are set, but the set of arguments
will change when the set of required properties is changed

### NewPiMarketDataWSSDomainModelsRequestMarketStreamingRequestWithDefaults

`func NewPiMarketDataWSSDomainModelsRequestMarketStreamingRequestWithDefaults() *PiMarketDataWSSDomainModelsRequestMarketStreamingRequest`

NewPiMarketDataWSSDomainModelsRequestMarketStreamingRequestWithDefaults instantiates a new PiMarketDataWSSDomainModelsRequestMarketStreamingRequest object
This constructor will only assign default values to properties that have it defined,
but it doesn't guarantee that properties required by API are set

### GetData

`func (o *PiMarketDataWSSDomainModelsRequestMarketStreamingRequest) GetData() PiMarketDataWSSDomainModelsRequestData`

GetData returns the Data field if non-nil, zero value otherwise.

### GetDataOk

`func (o *PiMarketDataWSSDomainModelsRequestMarketStreamingRequest) GetDataOk() (*PiMarketDataWSSDomainModelsRequestData, bool)`

GetDataOk returns a tuple with the Data field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetData

`func (o *PiMarketDataWSSDomainModelsRequestMarketStreamingRequest) SetData(v PiMarketDataWSSDomainModelsRequestData)`

SetData sets Data field to given value.

### HasData

`func (o *PiMarketDataWSSDomainModelsRequestMarketStreamingRequest) HasData() bool`

HasData returns a boolean if a field has been set.

### GetOp

`func (o *PiMarketDataWSSDomainModelsRequestMarketStreamingRequest) GetOp() string`

GetOp returns the Op field if non-nil, zero value otherwise.

### GetOpOk

`func (o *PiMarketDataWSSDomainModelsRequestMarketStreamingRequest) GetOpOk() (*string, bool)`

GetOpOk returns a tuple with the Op field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetOp

`func (o *PiMarketDataWSSDomainModelsRequestMarketStreamingRequest) SetOp(v string)`

SetOp sets Op field to given value.

### HasOp

`func (o *PiMarketDataWSSDomainModelsRequestMarketStreamingRequest) HasOp() bool`

HasOp returns a boolean if a field has been set.

### SetOpNil

`func (o *PiMarketDataWSSDomainModelsRequestMarketStreamingRequest) SetOpNil(b bool)`

 SetOpNil sets the value for Op to be an explicit nil

### UnsetOp
`func (o *PiMarketDataWSSDomainModelsRequestMarketStreamingRequest) UnsetOp()`

UnsetOp ensures that no value is present for Op, not even an explicit nil
### GetSessionId

`func (o *PiMarketDataWSSDomainModelsRequestMarketStreamingRequest) GetSessionId() string`

GetSessionId returns the SessionId field if non-nil, zero value otherwise.

### GetSessionIdOk

`func (o *PiMarketDataWSSDomainModelsRequestMarketStreamingRequest) GetSessionIdOk() (*string, bool)`

GetSessionIdOk returns a tuple with the SessionId field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetSessionId

`func (o *PiMarketDataWSSDomainModelsRequestMarketStreamingRequest) SetSessionId(v string)`

SetSessionId sets SessionId field to given value.

### HasSessionId

`func (o *PiMarketDataWSSDomainModelsRequestMarketStreamingRequest) HasSessionId() bool`

HasSessionId returns a boolean if a field has been set.

### SetSessionIdNil

`func (o *PiMarketDataWSSDomainModelsRequestMarketStreamingRequest) SetSessionIdNil(b bool)`

 SetSessionIdNil sets the value for SessionId to be an explicit nil

### UnsetSessionId
`func (o *PiMarketDataWSSDomainModelsRequestMarketStreamingRequest) UnsetSessionId()`

UnsetSessionId ensures that no value is present for SessionId, not even an explicit nil

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


