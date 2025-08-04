# InternalAddressProvinceGet200Response

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**Detail** | Pointer to **string** |  | [optional] 
**Status** | Pointer to **int32** |  | [optional] 
**Title** | Pointer to **string** |  | [optional] 
**Data** | Pointer to [**[]AddressProvince**](AddressProvince.md) |  | [optional] 

## Methods

### NewInternalAddressProvinceGet200Response

`func NewInternalAddressProvinceGet200Response() *InternalAddressProvinceGet200Response`

NewInternalAddressProvinceGet200Response instantiates a new InternalAddressProvinceGet200Response object
This constructor will assign default values to properties that have it defined,
and makes sure properties required by API are set, but the set of arguments
will change when the set of required properties is changed

### NewInternalAddressProvinceGet200ResponseWithDefaults

`func NewInternalAddressProvinceGet200ResponseWithDefaults() *InternalAddressProvinceGet200Response`

NewInternalAddressProvinceGet200ResponseWithDefaults instantiates a new InternalAddressProvinceGet200Response object
This constructor will only assign default values to properties that have it defined,
but it doesn't guarantee that properties required by API are set

### GetDetail

`func (o *InternalAddressProvinceGet200Response) GetDetail() string`

GetDetail returns the Detail field if non-nil, zero value otherwise.

### GetDetailOk

`func (o *InternalAddressProvinceGet200Response) GetDetailOk() (*string, bool)`

GetDetailOk returns a tuple with the Detail field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetDetail

`func (o *InternalAddressProvinceGet200Response) SetDetail(v string)`

SetDetail sets Detail field to given value.

### HasDetail

`func (o *InternalAddressProvinceGet200Response) HasDetail() bool`

HasDetail returns a boolean if a field has been set.

### GetStatus

`func (o *InternalAddressProvinceGet200Response) GetStatus() int32`

GetStatus returns the Status field if non-nil, zero value otherwise.

### GetStatusOk

`func (o *InternalAddressProvinceGet200Response) GetStatusOk() (*int32, bool)`

GetStatusOk returns a tuple with the Status field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetStatus

`func (o *InternalAddressProvinceGet200Response) SetStatus(v int32)`

SetStatus sets Status field to given value.

### HasStatus

`func (o *InternalAddressProvinceGet200Response) HasStatus() bool`

HasStatus returns a boolean if a field has been set.

### GetTitle

`func (o *InternalAddressProvinceGet200Response) GetTitle() string`

GetTitle returns the Title field if non-nil, zero value otherwise.

### GetTitleOk

`func (o *InternalAddressProvinceGet200Response) GetTitleOk() (*string, bool)`

GetTitleOk returns a tuple with the Title field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetTitle

`func (o *InternalAddressProvinceGet200Response) SetTitle(v string)`

SetTitle sets Title field to given value.

### HasTitle

`func (o *InternalAddressProvinceGet200Response) HasTitle() bool`

HasTitle returns a boolean if a field has been set.

### GetData

`func (o *InternalAddressProvinceGet200Response) GetData() []AddressProvince`

GetData returns the Data field if non-nil, zero value otherwise.

### GetDataOk

`func (o *InternalAddressProvinceGet200Response) GetDataOk() (*[]AddressProvince, bool)`

GetDataOk returns a tuple with the Data field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetData

`func (o *InternalAddressProvinceGet200Response) SetData(v []AddressProvince)`

SetData sets Data field to given value.

### HasData

`func (o *InternalAddressProvinceGet200Response) HasData() bool`

HasData returns a boolean if a field has been set.


[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


