# PiFundMarketDataAPIModelsResponsesFeeResponse

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**FeeUnit** | Pointer to [**PiFundMarketDataDomainModelsFeeUnit**](PiFundMarketDataDomainModelsFeeUnit.md) |  | [optional] 
**SwitchInFee** | Pointer to **NullableFloat32** |  | [optional] 
**ManagementFee** | Pointer to **NullableFloat64** |  | [optional] 
**FrontendFee** | Pointer to **NullableFloat64** |  | [optional] 
**BackendFee** | Pointer to **NullableFloat64** |  | [optional] 
**TotalExpense** | Pointer to **NullableFloat64** |  | [optional] 
**AsOfDate** | Pointer to **NullableTime** |  | [optional] 

## Methods

### NewPiFundMarketDataAPIModelsResponsesFeeResponse

`func NewPiFundMarketDataAPIModelsResponsesFeeResponse() *PiFundMarketDataAPIModelsResponsesFeeResponse`

NewPiFundMarketDataAPIModelsResponsesFeeResponse instantiates a new PiFundMarketDataAPIModelsResponsesFeeResponse object
This constructor will assign default values to properties that have it defined,
and makes sure properties required by API are set, but the set of arguments
will change when the set of required properties is changed

### NewPiFundMarketDataAPIModelsResponsesFeeResponseWithDefaults

`func NewPiFundMarketDataAPIModelsResponsesFeeResponseWithDefaults() *PiFundMarketDataAPIModelsResponsesFeeResponse`

NewPiFundMarketDataAPIModelsResponsesFeeResponseWithDefaults instantiates a new PiFundMarketDataAPIModelsResponsesFeeResponse object
This constructor will only assign default values to properties that have it defined,
but it doesn't guarantee that properties required by API are set

### GetFeeUnit

`func (o *PiFundMarketDataAPIModelsResponsesFeeResponse) GetFeeUnit() PiFundMarketDataDomainModelsFeeUnit`

GetFeeUnit returns the FeeUnit field if non-nil, zero value otherwise.

### GetFeeUnitOk

`func (o *PiFundMarketDataAPIModelsResponsesFeeResponse) GetFeeUnitOk() (*PiFundMarketDataDomainModelsFeeUnit, bool)`

GetFeeUnitOk returns a tuple with the FeeUnit field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetFeeUnit

`func (o *PiFundMarketDataAPIModelsResponsesFeeResponse) SetFeeUnit(v PiFundMarketDataDomainModelsFeeUnit)`

SetFeeUnit sets FeeUnit field to given value.

### HasFeeUnit

`func (o *PiFundMarketDataAPIModelsResponsesFeeResponse) HasFeeUnit() bool`

HasFeeUnit returns a boolean if a field has been set.

### GetSwitchInFee

`func (o *PiFundMarketDataAPIModelsResponsesFeeResponse) GetSwitchInFee() float32`

GetSwitchInFee returns the SwitchInFee field if non-nil, zero value otherwise.

### GetSwitchInFeeOk

`func (o *PiFundMarketDataAPIModelsResponsesFeeResponse) GetSwitchInFeeOk() (*float32, bool)`

GetSwitchInFeeOk returns a tuple with the SwitchInFee field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetSwitchInFee

`func (o *PiFundMarketDataAPIModelsResponsesFeeResponse) SetSwitchInFee(v float32)`

SetSwitchInFee sets SwitchInFee field to given value.

### HasSwitchInFee

`func (o *PiFundMarketDataAPIModelsResponsesFeeResponse) HasSwitchInFee() bool`

HasSwitchInFee returns a boolean if a field has been set.

### SetSwitchInFeeNil

`func (o *PiFundMarketDataAPIModelsResponsesFeeResponse) SetSwitchInFeeNil(b bool)`

 SetSwitchInFeeNil sets the value for SwitchInFee to be an explicit nil

### UnsetSwitchInFee
`func (o *PiFundMarketDataAPIModelsResponsesFeeResponse) UnsetSwitchInFee()`

UnsetSwitchInFee ensures that no value is present for SwitchInFee, not even an explicit nil
### GetManagementFee

`func (o *PiFundMarketDataAPIModelsResponsesFeeResponse) GetManagementFee() float64`

GetManagementFee returns the ManagementFee field if non-nil, zero value otherwise.

### GetManagementFeeOk

`func (o *PiFundMarketDataAPIModelsResponsesFeeResponse) GetManagementFeeOk() (*float64, bool)`

GetManagementFeeOk returns a tuple with the ManagementFee field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetManagementFee

`func (o *PiFundMarketDataAPIModelsResponsesFeeResponse) SetManagementFee(v float64)`

SetManagementFee sets ManagementFee field to given value.

### HasManagementFee

`func (o *PiFundMarketDataAPIModelsResponsesFeeResponse) HasManagementFee() bool`

HasManagementFee returns a boolean if a field has been set.

### SetManagementFeeNil

`func (o *PiFundMarketDataAPIModelsResponsesFeeResponse) SetManagementFeeNil(b bool)`

 SetManagementFeeNil sets the value for ManagementFee to be an explicit nil

### UnsetManagementFee
`func (o *PiFundMarketDataAPIModelsResponsesFeeResponse) UnsetManagementFee()`

UnsetManagementFee ensures that no value is present for ManagementFee, not even an explicit nil
### GetFrontendFee

`func (o *PiFundMarketDataAPIModelsResponsesFeeResponse) GetFrontendFee() float64`

GetFrontendFee returns the FrontendFee field if non-nil, zero value otherwise.

### GetFrontendFeeOk

`func (o *PiFundMarketDataAPIModelsResponsesFeeResponse) GetFrontendFeeOk() (*float64, bool)`

GetFrontendFeeOk returns a tuple with the FrontendFee field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetFrontendFee

`func (o *PiFundMarketDataAPIModelsResponsesFeeResponse) SetFrontendFee(v float64)`

SetFrontendFee sets FrontendFee field to given value.

### HasFrontendFee

`func (o *PiFundMarketDataAPIModelsResponsesFeeResponse) HasFrontendFee() bool`

HasFrontendFee returns a boolean if a field has been set.

### SetFrontendFeeNil

`func (o *PiFundMarketDataAPIModelsResponsesFeeResponse) SetFrontendFeeNil(b bool)`

 SetFrontendFeeNil sets the value for FrontendFee to be an explicit nil

### UnsetFrontendFee
`func (o *PiFundMarketDataAPIModelsResponsesFeeResponse) UnsetFrontendFee()`

UnsetFrontendFee ensures that no value is present for FrontendFee, not even an explicit nil
### GetBackendFee

`func (o *PiFundMarketDataAPIModelsResponsesFeeResponse) GetBackendFee() float64`

GetBackendFee returns the BackendFee field if non-nil, zero value otherwise.

### GetBackendFeeOk

`func (o *PiFundMarketDataAPIModelsResponsesFeeResponse) GetBackendFeeOk() (*float64, bool)`

GetBackendFeeOk returns a tuple with the BackendFee field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetBackendFee

`func (o *PiFundMarketDataAPIModelsResponsesFeeResponse) SetBackendFee(v float64)`

SetBackendFee sets BackendFee field to given value.

### HasBackendFee

`func (o *PiFundMarketDataAPIModelsResponsesFeeResponse) HasBackendFee() bool`

HasBackendFee returns a boolean if a field has been set.

### SetBackendFeeNil

`func (o *PiFundMarketDataAPIModelsResponsesFeeResponse) SetBackendFeeNil(b bool)`

 SetBackendFeeNil sets the value for BackendFee to be an explicit nil

### UnsetBackendFee
`func (o *PiFundMarketDataAPIModelsResponsesFeeResponse) UnsetBackendFee()`

UnsetBackendFee ensures that no value is present for BackendFee, not even an explicit nil
### GetTotalExpense

`func (o *PiFundMarketDataAPIModelsResponsesFeeResponse) GetTotalExpense() float64`

GetTotalExpense returns the TotalExpense field if non-nil, zero value otherwise.

### GetTotalExpenseOk

`func (o *PiFundMarketDataAPIModelsResponsesFeeResponse) GetTotalExpenseOk() (*float64, bool)`

GetTotalExpenseOk returns a tuple with the TotalExpense field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetTotalExpense

`func (o *PiFundMarketDataAPIModelsResponsesFeeResponse) SetTotalExpense(v float64)`

SetTotalExpense sets TotalExpense field to given value.

### HasTotalExpense

`func (o *PiFundMarketDataAPIModelsResponsesFeeResponse) HasTotalExpense() bool`

HasTotalExpense returns a boolean if a field has been set.

### SetTotalExpenseNil

`func (o *PiFundMarketDataAPIModelsResponsesFeeResponse) SetTotalExpenseNil(b bool)`

 SetTotalExpenseNil sets the value for TotalExpense to be an explicit nil

### UnsetTotalExpense
`func (o *PiFundMarketDataAPIModelsResponsesFeeResponse) UnsetTotalExpense()`

UnsetTotalExpense ensures that no value is present for TotalExpense, not even an explicit nil
### GetAsOfDate

`func (o *PiFundMarketDataAPIModelsResponsesFeeResponse) GetAsOfDate() time.Time`

GetAsOfDate returns the AsOfDate field if non-nil, zero value otherwise.

### GetAsOfDateOk

`func (o *PiFundMarketDataAPIModelsResponsesFeeResponse) GetAsOfDateOk() (*time.Time, bool)`

GetAsOfDateOk returns a tuple with the AsOfDate field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetAsOfDate

`func (o *PiFundMarketDataAPIModelsResponsesFeeResponse) SetAsOfDate(v time.Time)`

SetAsOfDate sets AsOfDate field to given value.

### HasAsOfDate

`func (o *PiFundMarketDataAPIModelsResponsesFeeResponse) HasAsOfDate() bool`

HasAsOfDate returns a boolean if a field has been set.

### SetAsOfDateNil

`func (o *PiFundMarketDataAPIModelsResponsesFeeResponse) SetAsOfDateNil(b bool)`

 SetAsOfDateNil sets the value for AsOfDate to be an explicit nil

### UnsetAsOfDate
`func (o *PiFundMarketDataAPIModelsResponsesFeeResponse) UnsetAsOfDate()`

UnsetAsOfDate ensures that no value is present for AsOfDate, not even an explicit nil

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


