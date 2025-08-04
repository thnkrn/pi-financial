# PiSetServiceAPIModelsSblOrderSubmitReviewRequest

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**Status** | [**PiSetServiceAPIModelsSblSubmitReviewStatus**](PiSetServiceAPIModelsSblSubmitReviewStatus.md) |  | 
**ReviewerId** | **string** |  | 
**RejectedReason** | Pointer to **NullableString** |  | [optional] 

## Methods

### NewPiSetServiceAPIModelsSblOrderSubmitReviewRequest

`func NewPiSetServiceAPIModelsSblOrderSubmitReviewRequest(status PiSetServiceAPIModelsSblSubmitReviewStatus, reviewerId string, ) *PiSetServiceAPIModelsSblOrderSubmitReviewRequest`

NewPiSetServiceAPIModelsSblOrderSubmitReviewRequest instantiates a new PiSetServiceAPIModelsSblOrderSubmitReviewRequest object
This constructor will assign default values to properties that have it defined,
and makes sure properties required by API are set, but the set of arguments
will change when the set of required properties is changed

### NewPiSetServiceAPIModelsSblOrderSubmitReviewRequestWithDefaults

`func NewPiSetServiceAPIModelsSblOrderSubmitReviewRequestWithDefaults() *PiSetServiceAPIModelsSblOrderSubmitReviewRequest`

NewPiSetServiceAPIModelsSblOrderSubmitReviewRequestWithDefaults instantiates a new PiSetServiceAPIModelsSblOrderSubmitReviewRequest object
This constructor will only assign default values to properties that have it defined,
but it doesn't guarantee that properties required by API are set

### GetStatus

`func (o *PiSetServiceAPIModelsSblOrderSubmitReviewRequest) GetStatus() PiSetServiceAPIModelsSblSubmitReviewStatus`

GetStatus returns the Status field if non-nil, zero value otherwise.

### GetStatusOk

`func (o *PiSetServiceAPIModelsSblOrderSubmitReviewRequest) GetStatusOk() (*PiSetServiceAPIModelsSblSubmitReviewStatus, bool)`

GetStatusOk returns a tuple with the Status field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetStatus

`func (o *PiSetServiceAPIModelsSblOrderSubmitReviewRequest) SetStatus(v PiSetServiceAPIModelsSblSubmitReviewStatus)`

SetStatus sets Status field to given value.


### GetReviewerId

`func (o *PiSetServiceAPIModelsSblOrderSubmitReviewRequest) GetReviewerId() string`

GetReviewerId returns the ReviewerId field if non-nil, zero value otherwise.

### GetReviewerIdOk

`func (o *PiSetServiceAPIModelsSblOrderSubmitReviewRequest) GetReviewerIdOk() (*string, bool)`

GetReviewerIdOk returns a tuple with the ReviewerId field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetReviewerId

`func (o *PiSetServiceAPIModelsSblOrderSubmitReviewRequest) SetReviewerId(v string)`

SetReviewerId sets ReviewerId field to given value.


### GetRejectedReason

`func (o *PiSetServiceAPIModelsSblOrderSubmitReviewRequest) GetRejectedReason() string`

GetRejectedReason returns the RejectedReason field if non-nil, zero value otherwise.

### GetRejectedReasonOk

`func (o *PiSetServiceAPIModelsSblOrderSubmitReviewRequest) GetRejectedReasonOk() (*string, bool)`

GetRejectedReasonOk returns a tuple with the RejectedReason field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetRejectedReason

`func (o *PiSetServiceAPIModelsSblOrderSubmitReviewRequest) SetRejectedReason(v string)`

SetRejectedReason sets RejectedReason field to given value.

### HasRejectedReason

`func (o *PiSetServiceAPIModelsSblOrderSubmitReviewRequest) HasRejectedReason() bool`

HasRejectedReason returns a boolean if a field has been set.

### SetRejectedReasonNil

`func (o *PiSetServiceAPIModelsSblOrderSubmitReviewRequest) SetRejectedReasonNil(b bool)`

 SetRejectedReasonNil sets the value for RejectedReason to be an explicit nil

### UnsetRejectedReason
`func (o *PiSetServiceAPIModelsSblOrderSubmitReviewRequest) UnsetRejectedReason()`

UnsetRejectedReason ensures that no value is present for RejectedReason, not even an explicit nil

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


