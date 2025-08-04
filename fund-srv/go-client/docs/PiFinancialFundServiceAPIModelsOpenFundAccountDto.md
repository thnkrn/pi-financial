# PiFinancialFundServiceAPIModelsOpenFundAccountDto

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**CustomerCode** | **string** |  | 
**Ndid** | Pointer to **bool** |  | [optional] 
**NdidInfo** | Pointer to [**PiFinancialFundServiceAPIModelsNdidInfo**](PiFinancialFundServiceAPIModelsNdidInfo.md) |  | [optional] 
**IsOpenSegregateAccount** | Pointer to **NullableBool** |  | [optional] 
**OpenAccountRegisterUid** | Pointer to **NullableString** |  | [optional] 
**CustomerId** | Pointer to **NullableInt64** |  | [optional] [readonly] 

## Methods

### NewPiFinancialFundServiceAPIModelsOpenFundAccountDto

`func NewPiFinancialFundServiceAPIModelsOpenFundAccountDto(customerCode string, ) *PiFinancialFundServiceAPIModelsOpenFundAccountDto`

NewPiFinancialFundServiceAPIModelsOpenFundAccountDto instantiates a new PiFinancialFundServiceAPIModelsOpenFundAccountDto object
This constructor will assign default values to properties that have it defined,
and makes sure properties required by API are set, but the set of arguments
will change when the set of required properties is changed

### NewPiFinancialFundServiceAPIModelsOpenFundAccountDtoWithDefaults

`func NewPiFinancialFundServiceAPIModelsOpenFundAccountDtoWithDefaults() *PiFinancialFundServiceAPIModelsOpenFundAccountDto`

NewPiFinancialFundServiceAPIModelsOpenFundAccountDtoWithDefaults instantiates a new PiFinancialFundServiceAPIModelsOpenFundAccountDto object
This constructor will only assign default values to properties that have it defined,
but it doesn't guarantee that properties required by API are set

### GetCustomerCode

`func (o *PiFinancialFundServiceAPIModelsOpenFundAccountDto) GetCustomerCode() string`

GetCustomerCode returns the CustomerCode field if non-nil, zero value otherwise.

### GetCustomerCodeOk

`func (o *PiFinancialFundServiceAPIModelsOpenFundAccountDto) GetCustomerCodeOk() (*string, bool)`

GetCustomerCodeOk returns a tuple with the CustomerCode field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetCustomerCode

`func (o *PiFinancialFundServiceAPIModelsOpenFundAccountDto) SetCustomerCode(v string)`

SetCustomerCode sets CustomerCode field to given value.


### GetNdid

`func (o *PiFinancialFundServiceAPIModelsOpenFundAccountDto) GetNdid() bool`

GetNdid returns the Ndid field if non-nil, zero value otherwise.

### GetNdidOk

`func (o *PiFinancialFundServiceAPIModelsOpenFundAccountDto) GetNdidOk() (*bool, bool)`

GetNdidOk returns a tuple with the Ndid field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetNdid

`func (o *PiFinancialFundServiceAPIModelsOpenFundAccountDto) SetNdid(v bool)`

SetNdid sets Ndid field to given value.

### HasNdid

`func (o *PiFinancialFundServiceAPIModelsOpenFundAccountDto) HasNdid() bool`

HasNdid returns a boolean if a field has been set.

### GetNdidInfo

`func (o *PiFinancialFundServiceAPIModelsOpenFundAccountDto) GetNdidInfo() PiFinancialFundServiceAPIModelsNdidInfo`

GetNdidInfo returns the NdidInfo field if non-nil, zero value otherwise.

### GetNdidInfoOk

`func (o *PiFinancialFundServiceAPIModelsOpenFundAccountDto) GetNdidInfoOk() (*PiFinancialFundServiceAPIModelsNdidInfo, bool)`

GetNdidInfoOk returns a tuple with the NdidInfo field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetNdidInfo

`func (o *PiFinancialFundServiceAPIModelsOpenFundAccountDto) SetNdidInfo(v PiFinancialFundServiceAPIModelsNdidInfo)`

SetNdidInfo sets NdidInfo field to given value.

### HasNdidInfo

`func (o *PiFinancialFundServiceAPIModelsOpenFundAccountDto) HasNdidInfo() bool`

HasNdidInfo returns a boolean if a field has been set.

### GetIsOpenSegregateAccount

`func (o *PiFinancialFundServiceAPIModelsOpenFundAccountDto) GetIsOpenSegregateAccount() bool`

GetIsOpenSegregateAccount returns the IsOpenSegregateAccount field if non-nil, zero value otherwise.

### GetIsOpenSegregateAccountOk

`func (o *PiFinancialFundServiceAPIModelsOpenFundAccountDto) GetIsOpenSegregateAccountOk() (*bool, bool)`

GetIsOpenSegregateAccountOk returns a tuple with the IsOpenSegregateAccount field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetIsOpenSegregateAccount

`func (o *PiFinancialFundServiceAPIModelsOpenFundAccountDto) SetIsOpenSegregateAccount(v bool)`

SetIsOpenSegregateAccount sets IsOpenSegregateAccount field to given value.

### HasIsOpenSegregateAccount

`func (o *PiFinancialFundServiceAPIModelsOpenFundAccountDto) HasIsOpenSegregateAccount() bool`

HasIsOpenSegregateAccount returns a boolean if a field has been set.

### SetIsOpenSegregateAccountNil

`func (o *PiFinancialFundServiceAPIModelsOpenFundAccountDto) SetIsOpenSegregateAccountNil(b bool)`

 SetIsOpenSegregateAccountNil sets the value for IsOpenSegregateAccount to be an explicit nil

### UnsetIsOpenSegregateAccount
`func (o *PiFinancialFundServiceAPIModelsOpenFundAccountDto) UnsetIsOpenSegregateAccount()`

UnsetIsOpenSegregateAccount ensures that no value is present for IsOpenSegregateAccount, not even an explicit nil
### GetOpenAccountRegisterUid

`func (o *PiFinancialFundServiceAPIModelsOpenFundAccountDto) GetOpenAccountRegisterUid() string`

GetOpenAccountRegisterUid returns the OpenAccountRegisterUid field if non-nil, zero value otherwise.

### GetOpenAccountRegisterUidOk

`func (o *PiFinancialFundServiceAPIModelsOpenFundAccountDto) GetOpenAccountRegisterUidOk() (*string, bool)`

GetOpenAccountRegisterUidOk returns a tuple with the OpenAccountRegisterUid field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetOpenAccountRegisterUid

`func (o *PiFinancialFundServiceAPIModelsOpenFundAccountDto) SetOpenAccountRegisterUid(v string)`

SetOpenAccountRegisterUid sets OpenAccountRegisterUid field to given value.

### HasOpenAccountRegisterUid

`func (o *PiFinancialFundServiceAPIModelsOpenFundAccountDto) HasOpenAccountRegisterUid() bool`

HasOpenAccountRegisterUid returns a boolean if a field has been set.

### SetOpenAccountRegisterUidNil

`func (o *PiFinancialFundServiceAPIModelsOpenFundAccountDto) SetOpenAccountRegisterUidNil(b bool)`

 SetOpenAccountRegisterUidNil sets the value for OpenAccountRegisterUid to be an explicit nil

### UnsetOpenAccountRegisterUid
`func (o *PiFinancialFundServiceAPIModelsOpenFundAccountDto) UnsetOpenAccountRegisterUid()`

UnsetOpenAccountRegisterUid ensures that no value is present for OpenAccountRegisterUid, not even an explicit nil
### GetCustomerId

`func (o *PiFinancialFundServiceAPIModelsOpenFundAccountDto) GetCustomerId() int64`

GetCustomerId returns the CustomerId field if non-nil, zero value otherwise.

### GetCustomerIdOk

`func (o *PiFinancialFundServiceAPIModelsOpenFundAccountDto) GetCustomerIdOk() (*int64, bool)`

GetCustomerIdOk returns a tuple with the CustomerId field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetCustomerId

`func (o *PiFinancialFundServiceAPIModelsOpenFundAccountDto) SetCustomerId(v int64)`

SetCustomerId sets CustomerId field to given value.

### HasCustomerId

`func (o *PiFinancialFundServiceAPIModelsOpenFundAccountDto) HasCustomerId() bool`

HasCustomerId returns a boolean if a field has been set.

### SetCustomerIdNil

`func (o *PiFinancialFundServiceAPIModelsOpenFundAccountDto) SetCustomerIdNil(b bool)`

 SetCustomerIdNil sets the value for CustomerId to be an explicit nil

### UnsetCustomerId
`func (o *PiFinancialFundServiceAPIModelsOpenFundAccountDto) UnsetCustomerId()`

UnsetCustomerId ensures that no value is present for CustomerId, not even an explicit nil

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


