# PiUserAPIModelsUpdateBankAccountEffectiveDateRequest

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**UserId** | Pointer to **string** |  | [optional] 
**CustomerCode** | Pointer to **NullableString** |  | [optional] 
**BankAccountNo** | Pointer to **NullableString** |  | [optional] 
**BankCode** | Pointer to **NullableString** |  | [optional] 
**BankBranchCode** | Pointer to **NullableString** |  | [optional] 
**EffectiveDate** | Pointer to **time.Time** |  | [optional] 
**EndDate** | Pointer to **time.Time** |  | [optional] 

## Methods

### NewPiUserAPIModelsUpdateBankAccountEffectiveDateRequest

`func NewPiUserAPIModelsUpdateBankAccountEffectiveDateRequest() *PiUserAPIModelsUpdateBankAccountEffectiveDateRequest`

NewPiUserAPIModelsUpdateBankAccountEffectiveDateRequest instantiates a new PiUserAPIModelsUpdateBankAccountEffectiveDateRequest object
This constructor will assign default values to properties that have it defined,
and makes sure properties required by API are set, but the set of arguments
will change when the set of required properties is changed

### NewPiUserAPIModelsUpdateBankAccountEffectiveDateRequestWithDefaults

`func NewPiUserAPIModelsUpdateBankAccountEffectiveDateRequestWithDefaults() *PiUserAPIModelsUpdateBankAccountEffectiveDateRequest`

NewPiUserAPIModelsUpdateBankAccountEffectiveDateRequestWithDefaults instantiates a new PiUserAPIModelsUpdateBankAccountEffectiveDateRequest object
This constructor will only assign default values to properties that have it defined,
but it doesn't guarantee that properties required by API are set

### GetUserId

`func (o *PiUserAPIModelsUpdateBankAccountEffectiveDateRequest) GetUserId() string`

GetUserId returns the UserId field if non-nil, zero value otherwise.

### GetUserIdOk

`func (o *PiUserAPIModelsUpdateBankAccountEffectiveDateRequest) GetUserIdOk() (*string, bool)`

GetUserIdOk returns a tuple with the UserId field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetUserId

`func (o *PiUserAPIModelsUpdateBankAccountEffectiveDateRequest) SetUserId(v string)`

SetUserId sets UserId field to given value.

### HasUserId

`func (o *PiUserAPIModelsUpdateBankAccountEffectiveDateRequest) HasUserId() bool`

HasUserId returns a boolean if a field has been set.

### GetCustomerCode

`func (o *PiUserAPIModelsUpdateBankAccountEffectiveDateRequest) GetCustomerCode() string`

GetCustomerCode returns the CustomerCode field if non-nil, zero value otherwise.

### GetCustomerCodeOk

`func (o *PiUserAPIModelsUpdateBankAccountEffectiveDateRequest) GetCustomerCodeOk() (*string, bool)`

GetCustomerCodeOk returns a tuple with the CustomerCode field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetCustomerCode

`func (o *PiUserAPIModelsUpdateBankAccountEffectiveDateRequest) SetCustomerCode(v string)`

SetCustomerCode sets CustomerCode field to given value.

### HasCustomerCode

`func (o *PiUserAPIModelsUpdateBankAccountEffectiveDateRequest) HasCustomerCode() bool`

HasCustomerCode returns a boolean if a field has been set.

### SetCustomerCodeNil

`func (o *PiUserAPIModelsUpdateBankAccountEffectiveDateRequest) SetCustomerCodeNil(b bool)`

 SetCustomerCodeNil sets the value for CustomerCode to be an explicit nil

### UnsetCustomerCode
`func (o *PiUserAPIModelsUpdateBankAccountEffectiveDateRequest) UnsetCustomerCode()`

UnsetCustomerCode ensures that no value is present for CustomerCode, not even an explicit nil
### GetBankAccountNo

`func (o *PiUserAPIModelsUpdateBankAccountEffectiveDateRequest) GetBankAccountNo() string`

GetBankAccountNo returns the BankAccountNo field if non-nil, zero value otherwise.

### GetBankAccountNoOk

`func (o *PiUserAPIModelsUpdateBankAccountEffectiveDateRequest) GetBankAccountNoOk() (*string, bool)`

GetBankAccountNoOk returns a tuple with the BankAccountNo field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetBankAccountNo

`func (o *PiUserAPIModelsUpdateBankAccountEffectiveDateRequest) SetBankAccountNo(v string)`

SetBankAccountNo sets BankAccountNo field to given value.

### HasBankAccountNo

`func (o *PiUserAPIModelsUpdateBankAccountEffectiveDateRequest) HasBankAccountNo() bool`

HasBankAccountNo returns a boolean if a field has been set.

### SetBankAccountNoNil

`func (o *PiUserAPIModelsUpdateBankAccountEffectiveDateRequest) SetBankAccountNoNil(b bool)`

 SetBankAccountNoNil sets the value for BankAccountNo to be an explicit nil

### UnsetBankAccountNo
`func (o *PiUserAPIModelsUpdateBankAccountEffectiveDateRequest) UnsetBankAccountNo()`

UnsetBankAccountNo ensures that no value is present for BankAccountNo, not even an explicit nil
### GetBankCode

`func (o *PiUserAPIModelsUpdateBankAccountEffectiveDateRequest) GetBankCode() string`

GetBankCode returns the BankCode field if non-nil, zero value otherwise.

### GetBankCodeOk

`func (o *PiUserAPIModelsUpdateBankAccountEffectiveDateRequest) GetBankCodeOk() (*string, bool)`

GetBankCodeOk returns a tuple with the BankCode field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetBankCode

`func (o *PiUserAPIModelsUpdateBankAccountEffectiveDateRequest) SetBankCode(v string)`

SetBankCode sets BankCode field to given value.

### HasBankCode

`func (o *PiUserAPIModelsUpdateBankAccountEffectiveDateRequest) HasBankCode() bool`

HasBankCode returns a boolean if a field has been set.

### SetBankCodeNil

`func (o *PiUserAPIModelsUpdateBankAccountEffectiveDateRequest) SetBankCodeNil(b bool)`

 SetBankCodeNil sets the value for BankCode to be an explicit nil

### UnsetBankCode
`func (o *PiUserAPIModelsUpdateBankAccountEffectiveDateRequest) UnsetBankCode()`

UnsetBankCode ensures that no value is present for BankCode, not even an explicit nil
### GetBankBranchCode

`func (o *PiUserAPIModelsUpdateBankAccountEffectiveDateRequest) GetBankBranchCode() string`

GetBankBranchCode returns the BankBranchCode field if non-nil, zero value otherwise.

### GetBankBranchCodeOk

`func (o *PiUserAPIModelsUpdateBankAccountEffectiveDateRequest) GetBankBranchCodeOk() (*string, bool)`

GetBankBranchCodeOk returns a tuple with the BankBranchCode field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetBankBranchCode

`func (o *PiUserAPIModelsUpdateBankAccountEffectiveDateRequest) SetBankBranchCode(v string)`

SetBankBranchCode sets BankBranchCode field to given value.

### HasBankBranchCode

`func (o *PiUserAPIModelsUpdateBankAccountEffectiveDateRequest) HasBankBranchCode() bool`

HasBankBranchCode returns a boolean if a field has been set.

### SetBankBranchCodeNil

`func (o *PiUserAPIModelsUpdateBankAccountEffectiveDateRequest) SetBankBranchCodeNil(b bool)`

 SetBankBranchCodeNil sets the value for BankBranchCode to be an explicit nil

### UnsetBankBranchCode
`func (o *PiUserAPIModelsUpdateBankAccountEffectiveDateRequest) UnsetBankBranchCode()`

UnsetBankBranchCode ensures that no value is present for BankBranchCode, not even an explicit nil
### GetEffectiveDate

`func (o *PiUserAPIModelsUpdateBankAccountEffectiveDateRequest) GetEffectiveDate() time.Time`

GetEffectiveDate returns the EffectiveDate field if non-nil, zero value otherwise.

### GetEffectiveDateOk

`func (o *PiUserAPIModelsUpdateBankAccountEffectiveDateRequest) GetEffectiveDateOk() (*time.Time, bool)`

GetEffectiveDateOk returns a tuple with the EffectiveDate field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetEffectiveDate

`func (o *PiUserAPIModelsUpdateBankAccountEffectiveDateRequest) SetEffectiveDate(v time.Time)`

SetEffectiveDate sets EffectiveDate field to given value.

### HasEffectiveDate

`func (o *PiUserAPIModelsUpdateBankAccountEffectiveDateRequest) HasEffectiveDate() bool`

HasEffectiveDate returns a boolean if a field has been set.

### GetEndDate

`func (o *PiUserAPIModelsUpdateBankAccountEffectiveDateRequest) GetEndDate() time.Time`

GetEndDate returns the EndDate field if non-nil, zero value otherwise.

### GetEndDateOk

`func (o *PiUserAPIModelsUpdateBankAccountEffectiveDateRequest) GetEndDateOk() (*time.Time, bool)`

GetEndDateOk returns a tuple with the EndDate field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetEndDate

`func (o *PiUserAPIModelsUpdateBankAccountEffectiveDateRequest) SetEndDate(v time.Time)`

SetEndDate sets EndDate field to given value.

### HasEndDate

`func (o *PiUserAPIModelsUpdateBankAccountEffectiveDateRequest) HasEndDate() bool`

HasEndDate returns a boolean if a field has been set.


[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


