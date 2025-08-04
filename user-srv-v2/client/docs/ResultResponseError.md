# ResultResponseError

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**Detail** | Pointer to **string** |  | [optional] 
**Status** | Pointer to **int32** |  | [optional] 
**Title** | Pointer to **string** |  | [optional] 

## Methods

### NewResultResponseError

`func NewResultResponseError() *ResultResponseError`

NewResultResponseError instantiates a new ResultResponseError object
This constructor will assign default values to properties that have it defined,
and makes sure properties required by API are set, but the set of arguments
will change when the set of required properties is changed

### NewResultResponseErrorWithDefaults

`func NewResultResponseErrorWithDefaults() *ResultResponseError`

NewResultResponseErrorWithDefaults instantiates a new ResultResponseError object
This constructor will only assign default values to properties that have it defined,
but it doesn't guarantee that properties required by API are set

### GetDetail

`func (o *ResultResponseError) GetDetail() string`

GetDetail returns the Detail field if non-nil, zero value otherwise.

### GetDetailOk

`func (o *ResultResponseError) GetDetailOk() (*string, bool)`

GetDetailOk returns a tuple with the Detail field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetDetail

`func (o *ResultResponseError) SetDetail(v string)`

SetDetail sets Detail field to given value.

### HasDetail

`func (o *ResultResponseError) HasDetail() bool`

HasDetail returns a boolean if a field has been set.

### GetStatus

`func (o *ResultResponseError) GetStatus() int32`

GetStatus returns the Status field if non-nil, zero value otherwise.

### GetStatusOk

`func (o *ResultResponseError) GetStatusOk() (*int32, bool)`

GetStatusOk returns a tuple with the Status field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetStatus

`func (o *ResultResponseError) SetStatus(v int32)`

SetStatus sets Status field to given value.

### HasStatus

`func (o *ResultResponseError) HasStatus() bool`

HasStatus returns a boolean if a field has been set.

### GetTitle

`func (o *ResultResponseError) GetTitle() string`

GetTitle returns the Title field if non-nil, zero value otherwise.

### GetTitleOk

`func (o *ResultResponseError) GetTitleOk() (*string, bool)`

GetTitleOk returns a tuple with the Title field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetTitle

`func (o *ResultResponseError) SetTitle(v string)`

SetTitle sets Title field to given value.

### HasTitle

`func (o *ResultResponseError) HasTitle() bool`

HasTitle returns a boolean if a field has been set.


[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


