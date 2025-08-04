# PiUserDomainAggregatesModelUserAccountAggregateUserAccount

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**RowVersion** | **string** |  | [readonly] 
**Id** | Pointer to **NullableString** |  | [optional] [readonly] 
**UserAccountType** | Pointer to **string** |  | [optional] 
**UserId** | Pointer to **string** |  | [optional] 
**CreatedAt** | Pointer to **time.Time** |  | [optional] 
**UpdatedAt** | Pointer to **NullableTime** |  | [optional] 
**TradeAccounts** | Pointer to [**[]PiUserDomainAggregatesModelTradeAccountAggregateTradeAccount**](PiUserDomainAggregatesModelTradeAccountAggregateTradeAccount.md) |  | [optional] [readonly] 

## Methods

### NewPiUserDomainAggregatesModelUserAccountAggregateUserAccount

`func NewPiUserDomainAggregatesModelUserAccountAggregateUserAccount(rowVersion string, ) *PiUserDomainAggregatesModelUserAccountAggregateUserAccount`

NewPiUserDomainAggregatesModelUserAccountAggregateUserAccount instantiates a new PiUserDomainAggregatesModelUserAccountAggregateUserAccount object
This constructor will assign default values to properties that have it defined,
and makes sure properties required by API are set, but the set of arguments
will change when the set of required properties is changed

### NewPiUserDomainAggregatesModelUserAccountAggregateUserAccountWithDefaults

`func NewPiUserDomainAggregatesModelUserAccountAggregateUserAccountWithDefaults() *PiUserDomainAggregatesModelUserAccountAggregateUserAccount`

NewPiUserDomainAggregatesModelUserAccountAggregateUserAccountWithDefaults instantiates a new PiUserDomainAggregatesModelUserAccountAggregateUserAccount object
This constructor will only assign default values to properties that have it defined,
but it doesn't guarantee that properties required by API are set

### GetRowVersion

`func (o *PiUserDomainAggregatesModelUserAccountAggregateUserAccount) GetRowVersion() string`

GetRowVersion returns the RowVersion field if non-nil, zero value otherwise.

### GetRowVersionOk

`func (o *PiUserDomainAggregatesModelUserAccountAggregateUserAccount) GetRowVersionOk() (*string, bool)`

GetRowVersionOk returns a tuple with the RowVersion field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetRowVersion

`func (o *PiUserDomainAggregatesModelUserAccountAggregateUserAccount) SetRowVersion(v string)`

SetRowVersion sets RowVersion field to given value.


### GetId

`func (o *PiUserDomainAggregatesModelUserAccountAggregateUserAccount) GetId() string`

GetId returns the Id field if non-nil, zero value otherwise.

### GetIdOk

`func (o *PiUserDomainAggregatesModelUserAccountAggregateUserAccount) GetIdOk() (*string, bool)`

GetIdOk returns a tuple with the Id field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetId

`func (o *PiUserDomainAggregatesModelUserAccountAggregateUserAccount) SetId(v string)`

SetId sets Id field to given value.

### HasId

`func (o *PiUserDomainAggregatesModelUserAccountAggregateUserAccount) HasId() bool`

HasId returns a boolean if a field has been set.

### SetIdNil

`func (o *PiUserDomainAggregatesModelUserAccountAggregateUserAccount) SetIdNil(b bool)`

 SetIdNil sets the value for Id to be an explicit nil

### UnsetId
`func (o *PiUserDomainAggregatesModelUserAccountAggregateUserAccount) UnsetId()`

UnsetId ensures that no value is present for Id, not even an explicit nil
### GetUserAccountType

`func (o *PiUserDomainAggregatesModelUserAccountAggregateUserAccount) GetUserAccountType() string`

GetUserAccountType returns the UserAccountType field if non-nil, zero value otherwise.

### GetUserAccountTypeOk

`func (o *PiUserDomainAggregatesModelUserAccountAggregateUserAccount) GetUserAccountTypeOk() (*string, bool)`

GetUserAccountTypeOk returns a tuple with the UserAccountType field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetUserAccountType

`func (o *PiUserDomainAggregatesModelUserAccountAggregateUserAccount) SetUserAccountType(v string)`

SetUserAccountType sets UserAccountType field to given value.

### HasUserAccountType

`func (o *PiUserDomainAggregatesModelUserAccountAggregateUserAccount) HasUserAccountType() bool`

HasUserAccountType returns a boolean if a field has been set.

### GetUserId

`func (o *PiUserDomainAggregatesModelUserAccountAggregateUserAccount) GetUserId() string`

GetUserId returns the UserId field if non-nil, zero value otherwise.

### GetUserIdOk

`func (o *PiUserDomainAggregatesModelUserAccountAggregateUserAccount) GetUserIdOk() (*string, bool)`

GetUserIdOk returns a tuple with the UserId field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetUserId

`func (o *PiUserDomainAggregatesModelUserAccountAggregateUserAccount) SetUserId(v string)`

SetUserId sets UserId field to given value.

### HasUserId

`func (o *PiUserDomainAggregatesModelUserAccountAggregateUserAccount) HasUserId() bool`

HasUserId returns a boolean if a field has been set.

### GetCreatedAt

`func (o *PiUserDomainAggregatesModelUserAccountAggregateUserAccount) GetCreatedAt() time.Time`

GetCreatedAt returns the CreatedAt field if non-nil, zero value otherwise.

### GetCreatedAtOk

`func (o *PiUserDomainAggregatesModelUserAccountAggregateUserAccount) GetCreatedAtOk() (*time.Time, bool)`

GetCreatedAtOk returns a tuple with the CreatedAt field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetCreatedAt

`func (o *PiUserDomainAggregatesModelUserAccountAggregateUserAccount) SetCreatedAt(v time.Time)`

SetCreatedAt sets CreatedAt field to given value.

### HasCreatedAt

`func (o *PiUserDomainAggregatesModelUserAccountAggregateUserAccount) HasCreatedAt() bool`

HasCreatedAt returns a boolean if a field has been set.

### GetUpdatedAt

`func (o *PiUserDomainAggregatesModelUserAccountAggregateUserAccount) GetUpdatedAt() time.Time`

GetUpdatedAt returns the UpdatedAt field if non-nil, zero value otherwise.

### GetUpdatedAtOk

`func (o *PiUserDomainAggregatesModelUserAccountAggregateUserAccount) GetUpdatedAtOk() (*time.Time, bool)`

GetUpdatedAtOk returns a tuple with the UpdatedAt field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetUpdatedAt

`func (o *PiUserDomainAggregatesModelUserAccountAggregateUserAccount) SetUpdatedAt(v time.Time)`

SetUpdatedAt sets UpdatedAt field to given value.

### HasUpdatedAt

`func (o *PiUserDomainAggregatesModelUserAccountAggregateUserAccount) HasUpdatedAt() bool`

HasUpdatedAt returns a boolean if a field has been set.

### SetUpdatedAtNil

`func (o *PiUserDomainAggregatesModelUserAccountAggregateUserAccount) SetUpdatedAtNil(b bool)`

 SetUpdatedAtNil sets the value for UpdatedAt to be an explicit nil

### UnsetUpdatedAt
`func (o *PiUserDomainAggregatesModelUserAccountAggregateUserAccount) UnsetUpdatedAt()`

UnsetUpdatedAt ensures that no value is present for UpdatedAt, not even an explicit nil
### GetTradeAccounts

`func (o *PiUserDomainAggregatesModelUserAccountAggregateUserAccount) GetTradeAccounts() []PiUserDomainAggregatesModelTradeAccountAggregateTradeAccount`

GetTradeAccounts returns the TradeAccounts field if non-nil, zero value otherwise.

### GetTradeAccountsOk

`func (o *PiUserDomainAggregatesModelUserAccountAggregateUserAccount) GetTradeAccountsOk() (*[]PiUserDomainAggregatesModelTradeAccountAggregateTradeAccount, bool)`

GetTradeAccountsOk returns a tuple with the TradeAccounts field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetTradeAccounts

`func (o *PiUserDomainAggregatesModelUserAccountAggregateUserAccount) SetTradeAccounts(v []PiUserDomainAggregatesModelTradeAccountAggregateTradeAccount)`

SetTradeAccounts sets TradeAccounts field to given value.

### HasTradeAccounts

`func (o *PiUserDomainAggregatesModelUserAccountAggregateUserAccount) HasTradeAccounts() bool`

HasTradeAccounts returns a boolean if a field has been set.

### SetTradeAccountsNil

`func (o *PiUserDomainAggregatesModelUserAccountAggregateUserAccount) SetTradeAccountsNil(b bool)`

 SetTradeAccountsNil sets the value for TradeAccounts to be an explicit nil

### UnsetTradeAccounts
`func (o *PiUserDomainAggregatesModelUserAccountAggregateUserAccount) UnsetTradeAccounts()`

UnsetTradeAccounts ensures that no value is present for TradeAccounts, not even an explicit nil

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


