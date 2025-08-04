# PiUserDomainAggregatesModelExternalAccountAggregateExternalAccount

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**RowVersion** | **string** |  | [readonly] 
**Id** | Pointer to **string** |  | [optional] [readonly] 
**Value** | Pointer to **NullableString** |  | [optional] 
**ProviderId** | Pointer to **int32** |  | [optional] 
**TradeAccountId** | Pointer to **string** |  | [optional] 
**CreatedAt** | Pointer to **time.Time** |  | [optional] 
**UpdatedAt** | Pointer to **NullableTime** |  | [optional] 

## Methods

### NewPiUserDomainAggregatesModelExternalAccountAggregateExternalAccount

`func NewPiUserDomainAggregatesModelExternalAccountAggregateExternalAccount(rowVersion string, ) *PiUserDomainAggregatesModelExternalAccountAggregateExternalAccount`

NewPiUserDomainAggregatesModelExternalAccountAggregateExternalAccount instantiates a new PiUserDomainAggregatesModelExternalAccountAggregateExternalAccount object
This constructor will assign default values to properties that have it defined,
and makes sure properties required by API are set, but the set of arguments
will change when the set of required properties is changed

### NewPiUserDomainAggregatesModelExternalAccountAggregateExternalAccountWithDefaults

`func NewPiUserDomainAggregatesModelExternalAccountAggregateExternalAccountWithDefaults() *PiUserDomainAggregatesModelExternalAccountAggregateExternalAccount`

NewPiUserDomainAggregatesModelExternalAccountAggregateExternalAccountWithDefaults instantiates a new PiUserDomainAggregatesModelExternalAccountAggregateExternalAccount object
This constructor will only assign default values to properties that have it defined,
but it doesn't guarantee that properties required by API are set

### GetRowVersion

`func (o *PiUserDomainAggregatesModelExternalAccountAggregateExternalAccount) GetRowVersion() string`

GetRowVersion returns the RowVersion field if non-nil, zero value otherwise.

### GetRowVersionOk

`func (o *PiUserDomainAggregatesModelExternalAccountAggregateExternalAccount) GetRowVersionOk() (*string, bool)`

GetRowVersionOk returns a tuple with the RowVersion field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetRowVersion

`func (o *PiUserDomainAggregatesModelExternalAccountAggregateExternalAccount) SetRowVersion(v string)`

SetRowVersion sets RowVersion field to given value.


### GetId

`func (o *PiUserDomainAggregatesModelExternalAccountAggregateExternalAccount) GetId() string`

GetId returns the Id field if non-nil, zero value otherwise.

### GetIdOk

`func (o *PiUserDomainAggregatesModelExternalAccountAggregateExternalAccount) GetIdOk() (*string, bool)`

GetIdOk returns a tuple with the Id field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetId

`func (o *PiUserDomainAggregatesModelExternalAccountAggregateExternalAccount) SetId(v string)`

SetId sets Id field to given value.

### HasId

`func (o *PiUserDomainAggregatesModelExternalAccountAggregateExternalAccount) HasId() bool`

HasId returns a boolean if a field has been set.

### GetValue

`func (o *PiUserDomainAggregatesModelExternalAccountAggregateExternalAccount) GetValue() string`

GetValue returns the Value field if non-nil, zero value otherwise.

### GetValueOk

`func (o *PiUserDomainAggregatesModelExternalAccountAggregateExternalAccount) GetValueOk() (*string, bool)`

GetValueOk returns a tuple with the Value field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetValue

`func (o *PiUserDomainAggregatesModelExternalAccountAggregateExternalAccount) SetValue(v string)`

SetValue sets Value field to given value.

### HasValue

`func (o *PiUserDomainAggregatesModelExternalAccountAggregateExternalAccount) HasValue() bool`

HasValue returns a boolean if a field has been set.

### SetValueNil

`func (o *PiUserDomainAggregatesModelExternalAccountAggregateExternalAccount) SetValueNil(b bool)`

 SetValueNil sets the value for Value to be an explicit nil

### UnsetValue
`func (o *PiUserDomainAggregatesModelExternalAccountAggregateExternalAccount) UnsetValue()`

UnsetValue ensures that no value is present for Value, not even an explicit nil
### GetProviderId

`func (o *PiUserDomainAggregatesModelExternalAccountAggregateExternalAccount) GetProviderId() int32`

GetProviderId returns the ProviderId field if non-nil, zero value otherwise.

### GetProviderIdOk

`func (o *PiUserDomainAggregatesModelExternalAccountAggregateExternalAccount) GetProviderIdOk() (*int32, bool)`

GetProviderIdOk returns a tuple with the ProviderId field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetProviderId

`func (o *PiUserDomainAggregatesModelExternalAccountAggregateExternalAccount) SetProviderId(v int32)`

SetProviderId sets ProviderId field to given value.

### HasProviderId

`func (o *PiUserDomainAggregatesModelExternalAccountAggregateExternalAccount) HasProviderId() bool`

HasProviderId returns a boolean if a field has been set.

### GetTradeAccountId

`func (o *PiUserDomainAggregatesModelExternalAccountAggregateExternalAccount) GetTradeAccountId() string`

GetTradeAccountId returns the TradeAccountId field if non-nil, zero value otherwise.

### GetTradeAccountIdOk

`func (o *PiUserDomainAggregatesModelExternalAccountAggregateExternalAccount) GetTradeAccountIdOk() (*string, bool)`

GetTradeAccountIdOk returns a tuple with the TradeAccountId field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetTradeAccountId

`func (o *PiUserDomainAggregatesModelExternalAccountAggregateExternalAccount) SetTradeAccountId(v string)`

SetTradeAccountId sets TradeAccountId field to given value.

### HasTradeAccountId

`func (o *PiUserDomainAggregatesModelExternalAccountAggregateExternalAccount) HasTradeAccountId() bool`

HasTradeAccountId returns a boolean if a field has been set.

### GetCreatedAt

`func (o *PiUserDomainAggregatesModelExternalAccountAggregateExternalAccount) GetCreatedAt() time.Time`

GetCreatedAt returns the CreatedAt field if non-nil, zero value otherwise.

### GetCreatedAtOk

`func (o *PiUserDomainAggregatesModelExternalAccountAggregateExternalAccount) GetCreatedAtOk() (*time.Time, bool)`

GetCreatedAtOk returns a tuple with the CreatedAt field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetCreatedAt

`func (o *PiUserDomainAggregatesModelExternalAccountAggregateExternalAccount) SetCreatedAt(v time.Time)`

SetCreatedAt sets CreatedAt field to given value.

### HasCreatedAt

`func (o *PiUserDomainAggregatesModelExternalAccountAggregateExternalAccount) HasCreatedAt() bool`

HasCreatedAt returns a boolean if a field has been set.

### GetUpdatedAt

`func (o *PiUserDomainAggregatesModelExternalAccountAggregateExternalAccount) GetUpdatedAt() time.Time`

GetUpdatedAt returns the UpdatedAt field if non-nil, zero value otherwise.

### GetUpdatedAtOk

`func (o *PiUserDomainAggregatesModelExternalAccountAggregateExternalAccount) GetUpdatedAtOk() (*time.Time, bool)`

GetUpdatedAtOk returns a tuple with the UpdatedAt field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetUpdatedAt

`func (o *PiUserDomainAggregatesModelExternalAccountAggregateExternalAccount) SetUpdatedAt(v time.Time)`

SetUpdatedAt sets UpdatedAt field to given value.

### HasUpdatedAt

`func (o *PiUserDomainAggregatesModelExternalAccountAggregateExternalAccount) HasUpdatedAt() bool`

HasUpdatedAt returns a boolean if a field has been set.

### SetUpdatedAtNil

`func (o *PiUserDomainAggregatesModelExternalAccountAggregateExternalAccount) SetUpdatedAtNil(b bool)`

 SetUpdatedAtNil sets the value for UpdatedAt to be an explicit nil

### UnsetUpdatedAt
`func (o *PiUserDomainAggregatesModelExternalAccountAggregateExternalAccount) UnsetUpdatedAt()`

UnsetUpdatedAt ensures that no value is present for UpdatedAt, not even an explicit nil

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


