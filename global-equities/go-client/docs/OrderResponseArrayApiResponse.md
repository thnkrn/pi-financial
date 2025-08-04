# OrderResponseArrayApiResponse

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**Data** | Pointer to [**[]OrderResponse**](OrderResponse.md) |  | [optional] 

## Methods

### NewOrderResponseArrayApiResponse

`func NewOrderResponseArrayApiResponse() *OrderResponseArrayApiResponse`

NewOrderResponseArrayApiResponse instantiates a new OrderResponseArrayApiResponse object
This constructor will assign default values to properties that have it defined,
and makes sure properties required by API are set, but the set of arguments
will change when the set of required properties is changed

### NewOrderResponseArrayApiResponseWithDefaults

`func NewOrderResponseArrayApiResponseWithDefaults() *OrderResponseArrayApiResponse`

NewOrderResponseArrayApiResponseWithDefaults instantiates a new OrderResponseArrayApiResponse object
This constructor will only assign default values to properties that have it defined,
but it doesn't guarantee that properties required by API are set

### GetData

`func (o *OrderResponseArrayApiResponse) GetData() []OrderResponse`

GetData returns the Data field if non-nil, zero value otherwise.

### GetDataOk

`func (o *OrderResponseArrayApiResponse) GetDataOk() (*[]OrderResponse, bool)`

GetDataOk returns a tuple with the Data field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetData

`func (o *OrderResponseArrayApiResponse) SetData(v []OrderResponse)`

SetData sets Data field to given value.

### HasData

`func (o *OrderResponseArrayApiResponse) HasData() bool`

HasData returns a boolean if a field has been set.

### SetDataNil

`func (o *OrderResponseArrayApiResponse) SetDataNil(b bool)`

 SetDataNil sets the value for Data to be an explicit nil

### UnsetData
`func (o *OrderResponseArrayApiResponse) UnsetData()`

UnsetData ensures that no value is present for Data, not even an explicit nil

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


