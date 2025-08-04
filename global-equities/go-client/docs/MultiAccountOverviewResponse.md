# MultiAccountOverviewResponse

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**AccountsOverview** | Pointer to [**[]AccountOverviewResponse**](AccountOverviewResponse.md) |  | [optional] [readonly] 
**FailedToFetchAccounts** | Pointer to [**[]FailedAccountResponse**](FailedAccountResponse.md) |  | [optional] [readonly] 
**HasFailedAccounts** | Pointer to **bool** |  | [optional] [readonly] 

## Methods

### NewMultiAccountOverviewResponse

`func NewMultiAccountOverviewResponse() *MultiAccountOverviewResponse`

NewMultiAccountOverviewResponse instantiates a new MultiAccountOverviewResponse object
This constructor will assign default values to properties that have it defined,
and makes sure properties required by API are set, but the set of arguments
will change when the set of required properties is changed

### NewMultiAccountOverviewResponseWithDefaults

`func NewMultiAccountOverviewResponseWithDefaults() *MultiAccountOverviewResponse`

NewMultiAccountOverviewResponseWithDefaults instantiates a new MultiAccountOverviewResponse object
This constructor will only assign default values to properties that have it defined,
but it doesn't guarantee that properties required by API are set

### GetAccountsOverview

`func (o *MultiAccountOverviewResponse) GetAccountsOverview() []AccountOverviewResponse`

GetAccountsOverview returns the AccountsOverview field if non-nil, zero value otherwise.

### GetAccountsOverviewOk

`func (o *MultiAccountOverviewResponse) GetAccountsOverviewOk() (*[]AccountOverviewResponse, bool)`

GetAccountsOverviewOk returns a tuple with the AccountsOverview field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetAccountsOverview

`func (o *MultiAccountOverviewResponse) SetAccountsOverview(v []AccountOverviewResponse)`

SetAccountsOverview sets AccountsOverview field to given value.

### HasAccountsOverview

`func (o *MultiAccountOverviewResponse) HasAccountsOverview() bool`

HasAccountsOverview returns a boolean if a field has been set.

### SetAccountsOverviewNil

`func (o *MultiAccountOverviewResponse) SetAccountsOverviewNil(b bool)`

 SetAccountsOverviewNil sets the value for AccountsOverview to be an explicit nil

### UnsetAccountsOverview
`func (o *MultiAccountOverviewResponse) UnsetAccountsOverview()`

UnsetAccountsOverview ensures that no value is present for AccountsOverview, not even an explicit nil
### GetFailedToFetchAccounts

`func (o *MultiAccountOverviewResponse) GetFailedToFetchAccounts() []FailedAccountResponse`

GetFailedToFetchAccounts returns the FailedToFetchAccounts field if non-nil, zero value otherwise.

### GetFailedToFetchAccountsOk

`func (o *MultiAccountOverviewResponse) GetFailedToFetchAccountsOk() (*[]FailedAccountResponse, bool)`

GetFailedToFetchAccountsOk returns a tuple with the FailedToFetchAccounts field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetFailedToFetchAccounts

`func (o *MultiAccountOverviewResponse) SetFailedToFetchAccounts(v []FailedAccountResponse)`

SetFailedToFetchAccounts sets FailedToFetchAccounts field to given value.

### HasFailedToFetchAccounts

`func (o *MultiAccountOverviewResponse) HasFailedToFetchAccounts() bool`

HasFailedToFetchAccounts returns a boolean if a field has been set.

### SetFailedToFetchAccountsNil

`func (o *MultiAccountOverviewResponse) SetFailedToFetchAccountsNil(b bool)`

 SetFailedToFetchAccountsNil sets the value for FailedToFetchAccounts to be an explicit nil

### UnsetFailedToFetchAccounts
`func (o *MultiAccountOverviewResponse) UnsetFailedToFetchAccounts()`

UnsetFailedToFetchAccounts ensures that no value is present for FailedToFetchAccounts, not even an explicit nil
### GetHasFailedAccounts

`func (o *MultiAccountOverviewResponse) GetHasFailedAccounts() bool`

GetHasFailedAccounts returns the HasFailedAccounts field if non-nil, zero value otherwise.

### GetHasFailedAccountsOk

`func (o *MultiAccountOverviewResponse) GetHasFailedAccountsOk() (*bool, bool)`

GetHasFailedAccountsOk returns a tuple with the HasFailedAccounts field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetHasFailedAccounts

`func (o *MultiAccountOverviewResponse) SetHasFailedAccounts(v bool)`

SetHasFailedAccounts sets HasFailedAccounts field to given value.

### HasHasFailedAccounts

`func (o *MultiAccountOverviewResponse) HasHasFailedAccounts() bool`

HasHasFailedAccounts returns a boolean if a field has been set.


[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


