# InternalAddressGet200Response

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**Detail** | Pointer to **string** |  | [optional] 
**Status** | Pointer to **int32** |  | [optional] 
**Title** | Pointer to **string** |  | [optional] 
**Data** | Pointer to [**[]AddressAddress**](AddressAddress.md) |  | [optional] 

## Methods

### NewInternalAddressGet200Response

`func NewInternalAddressGet200Response() *InternalAddressGet200Response`

NewInternalAddressGet200Response instantiates a new InternalAddressGet200Response object
This constructor will assign default values to properties that have it defined,
and makes sure properties required by API are set, but the set of arguments
will change when the set of required properties is changed

### NewInternalAddressGet200ResponseWithDefaults

`func NewInternalAddressGet200ResponseWithDefaults() *InternalAddressGet200Response`

NewInternalAddressGet200ResponseWithDefaults instantiates a new InternalAddressGet200Response object
This constructor will only assign default values to properties that have it defined,
but it doesn't guarantee that properties required by API are set

### GetDetail

`func (o *InternalAddressGet200Response) GetDetail() string`

GetDetail returns the Detail field if non-nil, zero value otherwise.

### GetDetailOk

`func (o *InternalAddressGet200Response) GetDetailOk() (*string, bool)`

GetDetailOk returns a tuple with the Detail field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetDetail

`func (o *InternalAddressGet200Response) SetDetail(v string)`

SetDetail sets Detail field to given value.

### HasDetail

`func (o *InternalAddressGet200Response) HasDetail() bool`

HasDetail returns a boolean if a field has been set.

### GetStatus

`func (o *InternalAddressGet200Response) GetStatus() int32`

GetStatus returns the Status field if non-nil, zero value otherwise.

### GetStatusOk

`func (o *InternalAddressGet200Response) GetStatusOk() (*int32, bool)`

GetStatusOk returns a tuple with the Status field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetStatus

`func (o *InternalAddressGet200Response) SetStatus(v int32)`

SetStatus sets Status field to given value.

### HasStatus

`func (o *InternalAddressGet200Response) HasStatus() bool`

HasStatus returns a boolean if a field has been set.

### GetTitle

`func (o *InternalAddressGet200Response) GetTitle() string`

GetTitle returns the Title field if non-nil, zero value otherwise.

### GetTitleOk

`func (o *InternalAddressGet200Response) GetTitleOk() (*string, bool)`

GetTitleOk returns a tuple with the Title field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetTitle

`func (o *InternalAddressGet200Response) SetTitle(v string)`

SetTitle sets Title field to given value.

### HasTitle

`func (o *InternalAddressGet200Response) HasTitle() bool`

HasTitle returns a boolean if a field has been set.

### GetData

`func (o *InternalAddressGet200Response) GetData() []AddressAddress`

GetData returns the Data field if non-nil, zero value otherwise.

### GetDataOk

`func (o *InternalAddressGet200Response) GetDataOk() (*[]AddressAddress, bool)`

GetDataOk returns a tuple with the Data field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetData

`func (o *InternalAddressGet200Response) SetData(v []AddressAddress)`

SetData sets Data field to given value.

### HasData

`func (o *InternalAddressGet200Response) HasData() bool`

HasData returns a boolean if a field has been set.


[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


