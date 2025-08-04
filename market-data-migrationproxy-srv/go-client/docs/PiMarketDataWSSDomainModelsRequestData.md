# PiMarketDataWSSDomainModelsRequestData

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**Param** | Pointer to [**[]PiMarketDataWSSDomainModelsRequestMarketStreamingParameter**](PiMarketDataWSSDomainModelsRequestMarketStreamingParameter.md) |  | [optional] 
**SubscribeType** | Pointer to **NullableString** |  | [optional] 

## Methods

### NewPiMarketDataWSSDomainModelsRequestData

`func NewPiMarketDataWSSDomainModelsRequestData() *PiMarketDataWSSDomainModelsRequestData`

NewPiMarketDataWSSDomainModelsRequestData instantiates a new PiMarketDataWSSDomainModelsRequestData object
This constructor will assign default values to properties that have it defined,
and makes sure properties required by API are set, but the set of arguments
will change when the set of required properties is changed

### NewPiMarketDataWSSDomainModelsRequestDataWithDefaults

`func NewPiMarketDataWSSDomainModelsRequestDataWithDefaults() *PiMarketDataWSSDomainModelsRequestData`

NewPiMarketDataWSSDomainModelsRequestDataWithDefaults instantiates a new PiMarketDataWSSDomainModelsRequestData object
This constructor will only assign default values to properties that have it defined,
but it doesn't guarantee that properties required by API are set

### GetParam

`func (o *PiMarketDataWSSDomainModelsRequestData) GetParam() []PiMarketDataWSSDomainModelsRequestMarketStreamingParameter`

GetParam returns the Param field if non-nil, zero value otherwise.

### GetParamOk

`func (o *PiMarketDataWSSDomainModelsRequestData) GetParamOk() (*[]PiMarketDataWSSDomainModelsRequestMarketStreamingParameter, bool)`

GetParamOk returns a tuple with the Param field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetParam

`func (o *PiMarketDataWSSDomainModelsRequestData) SetParam(v []PiMarketDataWSSDomainModelsRequestMarketStreamingParameter)`

SetParam sets Param field to given value.

### HasParam

`func (o *PiMarketDataWSSDomainModelsRequestData) HasParam() bool`

HasParam returns a boolean if a field has been set.

### SetParamNil

`func (o *PiMarketDataWSSDomainModelsRequestData) SetParamNil(b bool)`

 SetParamNil sets the value for Param to be an explicit nil

### UnsetParam
`func (o *PiMarketDataWSSDomainModelsRequestData) UnsetParam()`

UnsetParam ensures that no value is present for Param, not even an explicit nil
### GetSubscribeType

`func (o *PiMarketDataWSSDomainModelsRequestData) GetSubscribeType() string`

GetSubscribeType returns the SubscribeType field if non-nil, zero value otherwise.

### GetSubscribeTypeOk

`func (o *PiMarketDataWSSDomainModelsRequestData) GetSubscribeTypeOk() (*string, bool)`

GetSubscribeTypeOk returns a tuple with the SubscribeType field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetSubscribeType

`func (o *PiMarketDataWSSDomainModelsRequestData) SetSubscribeType(v string)`

SetSubscribeType sets SubscribeType field to given value.

### HasSubscribeType

`func (o *PiMarketDataWSSDomainModelsRequestData) HasSubscribeType() bool`

HasSubscribeType returns a boolean if a field has been set.

### SetSubscribeTypeNil

`func (o *PiMarketDataWSSDomainModelsRequestData) SetSubscribeTypeNil(b bool)`

 SetSubscribeTypeNil sets the value for SubscribeType to be an explicit nil

### UnsetSubscribeType
`func (o *PiMarketDataWSSDomainModelsRequestData) UnsetSubscribeType()`

UnsetSubscribeType ensures that no value is present for SubscribeType, not even an explicit nil

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


