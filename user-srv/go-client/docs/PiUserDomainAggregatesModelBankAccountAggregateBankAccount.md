# PiUserDomainAggregatesModelBankAccountAggregateBankAccount

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**RowVersion** | **string** |  | [readonly] 
**Id** | Pointer to **string** |  | [optional] [readonly] 
**UserId** | Pointer to **string** |  | [optional] 
**AccountNo** | Pointer to **NullableString** |  | [optional] 
**AccountNoHash** | Pointer to **NullableString** |  | [optional] 
**AccountName** | Pointer to **NullableString** |  | [optional] 
**AccountNameHash** | Pointer to **NullableString** |  | [optional] 
**BankCode** | Pointer to **NullableString** |  | [optional] 
**BankBranchCode** | Pointer to **NullableString** |  | [optional] 
**User** | Pointer to [**PiUserDomainAggregatesModelUserInfoAggregateUserInfo**](PiUserDomainAggregatesModelUserInfoAggregateUserInfo.md) |  | [optional] 
**CreatedAt** | Pointer to **time.Time** |  | [optional] 
**UpdatedAt** | Pointer to **NullableTime** |  | [optional] 

## Methods

### NewPiUserDomainAggregatesModelBankAccountAggregateBankAccount

`func NewPiUserDomainAggregatesModelBankAccountAggregateBankAccount(rowVersion string, ) *PiUserDomainAggregatesModelBankAccountAggregateBankAccount`

NewPiUserDomainAggregatesModelBankAccountAggregateBankAccount instantiates a new PiUserDomainAggregatesModelBankAccountAggregateBankAccount object
This constructor will assign default values to properties that have it defined,
and makes sure properties required by API are set, but the set of arguments
will change when the set of required properties is changed

### NewPiUserDomainAggregatesModelBankAccountAggregateBankAccountWithDefaults

`func NewPiUserDomainAggregatesModelBankAccountAggregateBankAccountWithDefaults() *PiUserDomainAggregatesModelBankAccountAggregateBankAccount`

NewPiUserDomainAggregatesModelBankAccountAggregateBankAccountWithDefaults instantiates a new PiUserDomainAggregatesModelBankAccountAggregateBankAccount object
This constructor will only assign default values to properties that have it defined,
but it doesn't guarantee that properties required by API are set

### GetRowVersion

`func (o *PiUserDomainAggregatesModelBankAccountAggregateBankAccount) GetRowVersion() string`

GetRowVersion returns the RowVersion field if non-nil, zero value otherwise.

### GetRowVersionOk

`func (o *PiUserDomainAggregatesModelBankAccountAggregateBankAccount) GetRowVersionOk() (*string, bool)`

GetRowVersionOk returns a tuple with the RowVersion field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetRowVersion

`func (o *PiUserDomainAggregatesModelBankAccountAggregateBankAccount) SetRowVersion(v string)`

SetRowVersion sets RowVersion field to given value.


### GetId

`func (o *PiUserDomainAggregatesModelBankAccountAggregateBankAccount) GetId() string`

GetId returns the Id field if non-nil, zero value otherwise.

### GetIdOk

`func (o *PiUserDomainAggregatesModelBankAccountAggregateBankAccount) GetIdOk() (*string, bool)`

GetIdOk returns a tuple with the Id field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetId

`func (o *PiUserDomainAggregatesModelBankAccountAggregateBankAccount) SetId(v string)`

SetId sets Id field to given value.

### HasId

`func (o *PiUserDomainAggregatesModelBankAccountAggregateBankAccount) HasId() bool`

HasId returns a boolean if a field has been set.

### GetUserId

`func (o *PiUserDomainAggregatesModelBankAccountAggregateBankAccount) GetUserId() string`

GetUserId returns the UserId field if non-nil, zero value otherwise.

### GetUserIdOk

`func (o *PiUserDomainAggregatesModelBankAccountAggregateBankAccount) GetUserIdOk() (*string, bool)`

GetUserIdOk returns a tuple with the UserId field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetUserId

`func (o *PiUserDomainAggregatesModelBankAccountAggregateBankAccount) SetUserId(v string)`

SetUserId sets UserId field to given value.

### HasUserId

`func (o *PiUserDomainAggregatesModelBankAccountAggregateBankAccount) HasUserId() bool`

HasUserId returns a boolean if a field has been set.

### GetAccountNo

`func (o *PiUserDomainAggregatesModelBankAccountAggregateBankAccount) GetAccountNo() string`

GetAccountNo returns the AccountNo field if non-nil, zero value otherwise.

### GetAccountNoOk

`func (o *PiUserDomainAggregatesModelBankAccountAggregateBankAccount) GetAccountNoOk() (*string, bool)`

GetAccountNoOk returns a tuple with the AccountNo field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetAccountNo

`func (o *PiUserDomainAggregatesModelBankAccountAggregateBankAccount) SetAccountNo(v string)`

SetAccountNo sets AccountNo field to given value.

### HasAccountNo

`func (o *PiUserDomainAggregatesModelBankAccountAggregateBankAccount) HasAccountNo() bool`

HasAccountNo returns a boolean if a field has been set.

### SetAccountNoNil

`func (o *PiUserDomainAggregatesModelBankAccountAggregateBankAccount) SetAccountNoNil(b bool)`

 SetAccountNoNil sets the value for AccountNo to be an explicit nil

### UnsetAccountNo
`func (o *PiUserDomainAggregatesModelBankAccountAggregateBankAccount) UnsetAccountNo()`

UnsetAccountNo ensures that no value is present for AccountNo, not even an explicit nil
### GetAccountNoHash

`func (o *PiUserDomainAggregatesModelBankAccountAggregateBankAccount) GetAccountNoHash() string`

GetAccountNoHash returns the AccountNoHash field if non-nil, zero value otherwise.

### GetAccountNoHashOk

`func (o *PiUserDomainAggregatesModelBankAccountAggregateBankAccount) GetAccountNoHashOk() (*string, bool)`

GetAccountNoHashOk returns a tuple with the AccountNoHash field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetAccountNoHash

`func (o *PiUserDomainAggregatesModelBankAccountAggregateBankAccount) SetAccountNoHash(v string)`

SetAccountNoHash sets AccountNoHash field to given value.

### HasAccountNoHash

`func (o *PiUserDomainAggregatesModelBankAccountAggregateBankAccount) HasAccountNoHash() bool`

HasAccountNoHash returns a boolean if a field has been set.

### SetAccountNoHashNil

`func (o *PiUserDomainAggregatesModelBankAccountAggregateBankAccount) SetAccountNoHashNil(b bool)`

 SetAccountNoHashNil sets the value for AccountNoHash to be an explicit nil

### UnsetAccountNoHash
`func (o *PiUserDomainAggregatesModelBankAccountAggregateBankAccount) UnsetAccountNoHash()`

UnsetAccountNoHash ensures that no value is present for AccountNoHash, not even an explicit nil
### GetAccountName

`func (o *PiUserDomainAggregatesModelBankAccountAggregateBankAccount) GetAccountName() string`

GetAccountName returns the AccountName field if non-nil, zero value otherwise.

### GetAccountNameOk

`func (o *PiUserDomainAggregatesModelBankAccountAggregateBankAccount) GetAccountNameOk() (*string, bool)`

GetAccountNameOk returns a tuple with the AccountName field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetAccountName

`func (o *PiUserDomainAggregatesModelBankAccountAggregateBankAccount) SetAccountName(v string)`

SetAccountName sets AccountName field to given value.

### HasAccountName

`func (o *PiUserDomainAggregatesModelBankAccountAggregateBankAccount) HasAccountName() bool`

HasAccountName returns a boolean if a field has been set.

### SetAccountNameNil

`func (o *PiUserDomainAggregatesModelBankAccountAggregateBankAccount) SetAccountNameNil(b bool)`

 SetAccountNameNil sets the value for AccountName to be an explicit nil

### UnsetAccountName
`func (o *PiUserDomainAggregatesModelBankAccountAggregateBankAccount) UnsetAccountName()`

UnsetAccountName ensures that no value is present for AccountName, not even an explicit nil
### GetAccountNameHash

`func (o *PiUserDomainAggregatesModelBankAccountAggregateBankAccount) GetAccountNameHash() string`

GetAccountNameHash returns the AccountNameHash field if non-nil, zero value otherwise.

### GetAccountNameHashOk

`func (o *PiUserDomainAggregatesModelBankAccountAggregateBankAccount) GetAccountNameHashOk() (*string, bool)`

GetAccountNameHashOk returns a tuple with the AccountNameHash field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetAccountNameHash

`func (o *PiUserDomainAggregatesModelBankAccountAggregateBankAccount) SetAccountNameHash(v string)`

SetAccountNameHash sets AccountNameHash field to given value.

### HasAccountNameHash

`func (o *PiUserDomainAggregatesModelBankAccountAggregateBankAccount) HasAccountNameHash() bool`

HasAccountNameHash returns a boolean if a field has been set.

### SetAccountNameHashNil

`func (o *PiUserDomainAggregatesModelBankAccountAggregateBankAccount) SetAccountNameHashNil(b bool)`

 SetAccountNameHashNil sets the value for AccountNameHash to be an explicit nil

### UnsetAccountNameHash
`func (o *PiUserDomainAggregatesModelBankAccountAggregateBankAccount) UnsetAccountNameHash()`

UnsetAccountNameHash ensures that no value is present for AccountNameHash, not even an explicit nil
### GetBankCode

`func (o *PiUserDomainAggregatesModelBankAccountAggregateBankAccount) GetBankCode() string`

GetBankCode returns the BankCode field if non-nil, zero value otherwise.

### GetBankCodeOk

`func (o *PiUserDomainAggregatesModelBankAccountAggregateBankAccount) GetBankCodeOk() (*string, bool)`

GetBankCodeOk returns a tuple with the BankCode field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetBankCode

`func (o *PiUserDomainAggregatesModelBankAccountAggregateBankAccount) SetBankCode(v string)`

SetBankCode sets BankCode field to given value.

### HasBankCode

`func (o *PiUserDomainAggregatesModelBankAccountAggregateBankAccount) HasBankCode() bool`

HasBankCode returns a boolean if a field has been set.

### SetBankCodeNil

`func (o *PiUserDomainAggregatesModelBankAccountAggregateBankAccount) SetBankCodeNil(b bool)`

 SetBankCodeNil sets the value for BankCode to be an explicit nil

### UnsetBankCode
`func (o *PiUserDomainAggregatesModelBankAccountAggregateBankAccount) UnsetBankCode()`

UnsetBankCode ensures that no value is present for BankCode, not even an explicit nil
### GetBankBranchCode

`func (o *PiUserDomainAggregatesModelBankAccountAggregateBankAccount) GetBankBranchCode() string`

GetBankBranchCode returns the BankBranchCode field if non-nil, zero value otherwise.

### GetBankBranchCodeOk

`func (o *PiUserDomainAggregatesModelBankAccountAggregateBankAccount) GetBankBranchCodeOk() (*string, bool)`

GetBankBranchCodeOk returns a tuple with the BankBranchCode field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetBankBranchCode

`func (o *PiUserDomainAggregatesModelBankAccountAggregateBankAccount) SetBankBranchCode(v string)`

SetBankBranchCode sets BankBranchCode field to given value.

### HasBankBranchCode

`func (o *PiUserDomainAggregatesModelBankAccountAggregateBankAccount) HasBankBranchCode() bool`

HasBankBranchCode returns a boolean if a field has been set.

### SetBankBranchCodeNil

`func (o *PiUserDomainAggregatesModelBankAccountAggregateBankAccount) SetBankBranchCodeNil(b bool)`

 SetBankBranchCodeNil sets the value for BankBranchCode to be an explicit nil

### UnsetBankBranchCode
`func (o *PiUserDomainAggregatesModelBankAccountAggregateBankAccount) UnsetBankBranchCode()`

UnsetBankBranchCode ensures that no value is present for BankBranchCode, not even an explicit nil
### GetUser

`func (o *PiUserDomainAggregatesModelBankAccountAggregateBankAccount) GetUser() PiUserDomainAggregatesModelUserInfoAggregateUserInfo`

GetUser returns the User field if non-nil, zero value otherwise.

### GetUserOk

`func (o *PiUserDomainAggregatesModelBankAccountAggregateBankAccount) GetUserOk() (*PiUserDomainAggregatesModelUserInfoAggregateUserInfo, bool)`

GetUserOk returns a tuple with the User field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetUser

`func (o *PiUserDomainAggregatesModelBankAccountAggregateBankAccount) SetUser(v PiUserDomainAggregatesModelUserInfoAggregateUserInfo)`

SetUser sets User field to given value.

### HasUser

`func (o *PiUserDomainAggregatesModelBankAccountAggregateBankAccount) HasUser() bool`

HasUser returns a boolean if a field has been set.

### GetCreatedAt

`func (o *PiUserDomainAggregatesModelBankAccountAggregateBankAccount) GetCreatedAt() time.Time`

GetCreatedAt returns the CreatedAt field if non-nil, zero value otherwise.

### GetCreatedAtOk

`func (o *PiUserDomainAggregatesModelBankAccountAggregateBankAccount) GetCreatedAtOk() (*time.Time, bool)`

GetCreatedAtOk returns a tuple with the CreatedAt field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetCreatedAt

`func (o *PiUserDomainAggregatesModelBankAccountAggregateBankAccount) SetCreatedAt(v time.Time)`

SetCreatedAt sets CreatedAt field to given value.

### HasCreatedAt

`func (o *PiUserDomainAggregatesModelBankAccountAggregateBankAccount) HasCreatedAt() bool`

HasCreatedAt returns a boolean if a field has been set.

### GetUpdatedAt

`func (o *PiUserDomainAggregatesModelBankAccountAggregateBankAccount) GetUpdatedAt() time.Time`

GetUpdatedAt returns the UpdatedAt field if non-nil, zero value otherwise.

### GetUpdatedAtOk

`func (o *PiUserDomainAggregatesModelBankAccountAggregateBankAccount) GetUpdatedAtOk() (*time.Time, bool)`

GetUpdatedAtOk returns a tuple with the UpdatedAt field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetUpdatedAt

`func (o *PiUserDomainAggregatesModelBankAccountAggregateBankAccount) SetUpdatedAt(v time.Time)`

SetUpdatedAt sets UpdatedAt field to given value.

### HasUpdatedAt

`func (o *PiUserDomainAggregatesModelBankAccountAggregateBankAccount) HasUpdatedAt() bool`

HasUpdatedAt returns a boolean if a field has been set.

### SetUpdatedAtNil

`func (o *PiUserDomainAggregatesModelBankAccountAggregateBankAccount) SetUpdatedAtNil(b bool)`

 SetUpdatedAtNil sets the value for UpdatedAt to be an explicit nil

### UnsetUpdatedAt
`func (o *PiUserDomainAggregatesModelBankAccountAggregateBankAccount) UnsetUpdatedAt()`

UnsetUpdatedAt ensures that no value is present for UpdatedAt, not even an explicit nil

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


