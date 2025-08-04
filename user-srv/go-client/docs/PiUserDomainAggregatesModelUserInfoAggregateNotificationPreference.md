# PiUserDomainAggregatesModelUserInfoAggregateNotificationPreference

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**RowVersion** | **string** |  | [readonly] 
**Id** | Pointer to **string** |  | [optional] [readonly] 
**Important** | Pointer to **bool** |  | [optional] 
**Order** | Pointer to **bool** |  | [optional] 
**Portfolio** | Pointer to **bool** |  | [optional] 
**Wallet** | Pointer to **bool** |  | [optional] 
**Market** | Pointer to **bool** |  | [optional] 
**DeviceForeignKey** | Pointer to **string** |  | [optional] [readonly] 
**UserInfoId** | Pointer to **NullableString** |  | [optional] [readonly] 
**UserInfo** | Pointer to [**PiUserDomainAggregatesModelUserInfoAggregateUserInfo**](PiUserDomainAggregatesModelUserInfoAggregateUserInfo.md) |  | [optional] 
**CreatedAt** | Pointer to **time.Time** |  | [optional] 
**UpdatedAt** | Pointer to **NullableTime** |  | [optional] 

## Methods

### NewPiUserDomainAggregatesModelUserInfoAggregateNotificationPreference

`func NewPiUserDomainAggregatesModelUserInfoAggregateNotificationPreference(rowVersion string, ) *PiUserDomainAggregatesModelUserInfoAggregateNotificationPreference`

NewPiUserDomainAggregatesModelUserInfoAggregateNotificationPreference instantiates a new PiUserDomainAggregatesModelUserInfoAggregateNotificationPreference object
This constructor will assign default values to properties that have it defined,
and makes sure properties required by API are set, but the set of arguments
will change when the set of required properties is changed

### NewPiUserDomainAggregatesModelUserInfoAggregateNotificationPreferenceWithDefaults

`func NewPiUserDomainAggregatesModelUserInfoAggregateNotificationPreferenceWithDefaults() *PiUserDomainAggregatesModelUserInfoAggregateNotificationPreference`

NewPiUserDomainAggregatesModelUserInfoAggregateNotificationPreferenceWithDefaults instantiates a new PiUserDomainAggregatesModelUserInfoAggregateNotificationPreference object
This constructor will only assign default values to properties that have it defined,
but it doesn't guarantee that properties required by API are set

### GetRowVersion

`func (o *PiUserDomainAggregatesModelUserInfoAggregateNotificationPreference) GetRowVersion() string`

GetRowVersion returns the RowVersion field if non-nil, zero value otherwise.

### GetRowVersionOk

`func (o *PiUserDomainAggregatesModelUserInfoAggregateNotificationPreference) GetRowVersionOk() (*string, bool)`

GetRowVersionOk returns a tuple with the RowVersion field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetRowVersion

`func (o *PiUserDomainAggregatesModelUserInfoAggregateNotificationPreference) SetRowVersion(v string)`

SetRowVersion sets RowVersion field to given value.


### GetId

`func (o *PiUserDomainAggregatesModelUserInfoAggregateNotificationPreference) GetId() string`

GetId returns the Id field if non-nil, zero value otherwise.

### GetIdOk

`func (o *PiUserDomainAggregatesModelUserInfoAggregateNotificationPreference) GetIdOk() (*string, bool)`

GetIdOk returns a tuple with the Id field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetId

`func (o *PiUserDomainAggregatesModelUserInfoAggregateNotificationPreference) SetId(v string)`

SetId sets Id field to given value.

### HasId

`func (o *PiUserDomainAggregatesModelUserInfoAggregateNotificationPreference) HasId() bool`

HasId returns a boolean if a field has been set.

### GetImportant

`func (o *PiUserDomainAggregatesModelUserInfoAggregateNotificationPreference) GetImportant() bool`

GetImportant returns the Important field if non-nil, zero value otherwise.

### GetImportantOk

`func (o *PiUserDomainAggregatesModelUserInfoAggregateNotificationPreference) GetImportantOk() (*bool, bool)`

GetImportantOk returns a tuple with the Important field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetImportant

`func (o *PiUserDomainAggregatesModelUserInfoAggregateNotificationPreference) SetImportant(v bool)`

SetImportant sets Important field to given value.

### HasImportant

`func (o *PiUserDomainAggregatesModelUserInfoAggregateNotificationPreference) HasImportant() bool`

HasImportant returns a boolean if a field has been set.

### GetOrder

`func (o *PiUserDomainAggregatesModelUserInfoAggregateNotificationPreference) GetOrder() bool`

GetOrder returns the Order field if non-nil, zero value otherwise.

### GetOrderOk

`func (o *PiUserDomainAggregatesModelUserInfoAggregateNotificationPreference) GetOrderOk() (*bool, bool)`

GetOrderOk returns a tuple with the Order field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetOrder

`func (o *PiUserDomainAggregatesModelUserInfoAggregateNotificationPreference) SetOrder(v bool)`

SetOrder sets Order field to given value.

### HasOrder

`func (o *PiUserDomainAggregatesModelUserInfoAggregateNotificationPreference) HasOrder() bool`

HasOrder returns a boolean if a field has been set.

### GetPortfolio

`func (o *PiUserDomainAggregatesModelUserInfoAggregateNotificationPreference) GetPortfolio() bool`

GetPortfolio returns the Portfolio field if non-nil, zero value otherwise.

### GetPortfolioOk

`func (o *PiUserDomainAggregatesModelUserInfoAggregateNotificationPreference) GetPortfolioOk() (*bool, bool)`

GetPortfolioOk returns a tuple with the Portfolio field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetPortfolio

`func (o *PiUserDomainAggregatesModelUserInfoAggregateNotificationPreference) SetPortfolio(v bool)`

SetPortfolio sets Portfolio field to given value.

### HasPortfolio

`func (o *PiUserDomainAggregatesModelUserInfoAggregateNotificationPreference) HasPortfolio() bool`

HasPortfolio returns a boolean if a field has been set.

### GetWallet

`func (o *PiUserDomainAggregatesModelUserInfoAggregateNotificationPreference) GetWallet() bool`

GetWallet returns the Wallet field if non-nil, zero value otherwise.

### GetWalletOk

`func (o *PiUserDomainAggregatesModelUserInfoAggregateNotificationPreference) GetWalletOk() (*bool, bool)`

GetWalletOk returns a tuple with the Wallet field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetWallet

`func (o *PiUserDomainAggregatesModelUserInfoAggregateNotificationPreference) SetWallet(v bool)`

SetWallet sets Wallet field to given value.

### HasWallet

`func (o *PiUserDomainAggregatesModelUserInfoAggregateNotificationPreference) HasWallet() bool`

HasWallet returns a boolean if a field has been set.

### GetMarket

`func (o *PiUserDomainAggregatesModelUserInfoAggregateNotificationPreference) GetMarket() bool`

GetMarket returns the Market field if non-nil, zero value otherwise.

### GetMarketOk

`func (o *PiUserDomainAggregatesModelUserInfoAggregateNotificationPreference) GetMarketOk() (*bool, bool)`

GetMarketOk returns a tuple with the Market field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetMarket

`func (o *PiUserDomainAggregatesModelUserInfoAggregateNotificationPreference) SetMarket(v bool)`

SetMarket sets Market field to given value.

### HasMarket

`func (o *PiUserDomainAggregatesModelUserInfoAggregateNotificationPreference) HasMarket() bool`

HasMarket returns a boolean if a field has been set.

### GetDeviceForeignKey

`func (o *PiUserDomainAggregatesModelUserInfoAggregateNotificationPreference) GetDeviceForeignKey() string`

GetDeviceForeignKey returns the DeviceForeignKey field if non-nil, zero value otherwise.

### GetDeviceForeignKeyOk

`func (o *PiUserDomainAggregatesModelUserInfoAggregateNotificationPreference) GetDeviceForeignKeyOk() (*string, bool)`

GetDeviceForeignKeyOk returns a tuple with the DeviceForeignKey field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetDeviceForeignKey

`func (o *PiUserDomainAggregatesModelUserInfoAggregateNotificationPreference) SetDeviceForeignKey(v string)`

SetDeviceForeignKey sets DeviceForeignKey field to given value.

### HasDeviceForeignKey

`func (o *PiUserDomainAggregatesModelUserInfoAggregateNotificationPreference) HasDeviceForeignKey() bool`

HasDeviceForeignKey returns a boolean if a field has been set.

### GetUserInfoId

`func (o *PiUserDomainAggregatesModelUserInfoAggregateNotificationPreference) GetUserInfoId() string`

GetUserInfoId returns the UserInfoId field if non-nil, zero value otherwise.

### GetUserInfoIdOk

`func (o *PiUserDomainAggregatesModelUserInfoAggregateNotificationPreference) GetUserInfoIdOk() (*string, bool)`

GetUserInfoIdOk returns a tuple with the UserInfoId field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetUserInfoId

`func (o *PiUserDomainAggregatesModelUserInfoAggregateNotificationPreference) SetUserInfoId(v string)`

SetUserInfoId sets UserInfoId field to given value.

### HasUserInfoId

`func (o *PiUserDomainAggregatesModelUserInfoAggregateNotificationPreference) HasUserInfoId() bool`

HasUserInfoId returns a boolean if a field has been set.

### SetUserInfoIdNil

`func (o *PiUserDomainAggregatesModelUserInfoAggregateNotificationPreference) SetUserInfoIdNil(b bool)`

 SetUserInfoIdNil sets the value for UserInfoId to be an explicit nil

### UnsetUserInfoId
`func (o *PiUserDomainAggregatesModelUserInfoAggregateNotificationPreference) UnsetUserInfoId()`

UnsetUserInfoId ensures that no value is present for UserInfoId, not even an explicit nil
### GetUserInfo

`func (o *PiUserDomainAggregatesModelUserInfoAggregateNotificationPreference) GetUserInfo() PiUserDomainAggregatesModelUserInfoAggregateUserInfo`

GetUserInfo returns the UserInfo field if non-nil, zero value otherwise.

### GetUserInfoOk

`func (o *PiUserDomainAggregatesModelUserInfoAggregateNotificationPreference) GetUserInfoOk() (*PiUserDomainAggregatesModelUserInfoAggregateUserInfo, bool)`

GetUserInfoOk returns a tuple with the UserInfo field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetUserInfo

`func (o *PiUserDomainAggregatesModelUserInfoAggregateNotificationPreference) SetUserInfo(v PiUserDomainAggregatesModelUserInfoAggregateUserInfo)`

SetUserInfo sets UserInfo field to given value.

### HasUserInfo

`func (o *PiUserDomainAggregatesModelUserInfoAggregateNotificationPreference) HasUserInfo() bool`

HasUserInfo returns a boolean if a field has been set.

### GetCreatedAt

`func (o *PiUserDomainAggregatesModelUserInfoAggregateNotificationPreference) GetCreatedAt() time.Time`

GetCreatedAt returns the CreatedAt field if non-nil, zero value otherwise.

### GetCreatedAtOk

`func (o *PiUserDomainAggregatesModelUserInfoAggregateNotificationPreference) GetCreatedAtOk() (*time.Time, bool)`

GetCreatedAtOk returns a tuple with the CreatedAt field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetCreatedAt

`func (o *PiUserDomainAggregatesModelUserInfoAggregateNotificationPreference) SetCreatedAt(v time.Time)`

SetCreatedAt sets CreatedAt field to given value.

### HasCreatedAt

`func (o *PiUserDomainAggregatesModelUserInfoAggregateNotificationPreference) HasCreatedAt() bool`

HasCreatedAt returns a boolean if a field has been set.

### GetUpdatedAt

`func (o *PiUserDomainAggregatesModelUserInfoAggregateNotificationPreference) GetUpdatedAt() time.Time`

GetUpdatedAt returns the UpdatedAt field if non-nil, zero value otherwise.

### GetUpdatedAtOk

`func (o *PiUserDomainAggregatesModelUserInfoAggregateNotificationPreference) GetUpdatedAtOk() (*time.Time, bool)`

GetUpdatedAtOk returns a tuple with the UpdatedAt field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetUpdatedAt

`func (o *PiUserDomainAggregatesModelUserInfoAggregateNotificationPreference) SetUpdatedAt(v time.Time)`

SetUpdatedAt sets UpdatedAt field to given value.

### HasUpdatedAt

`func (o *PiUserDomainAggregatesModelUserInfoAggregateNotificationPreference) HasUpdatedAt() bool`

HasUpdatedAt returns a boolean if a field has been set.

### SetUpdatedAtNil

`func (o *PiUserDomainAggregatesModelUserInfoAggregateNotificationPreference) SetUpdatedAtNil(b bool)`

 SetUpdatedAtNil sets the value for UpdatedAt to be an explicit nil

### UnsetUpdatedAt
`func (o *PiUserDomainAggregatesModelUserInfoAggregateNotificationPreference) UnsetUpdatedAt()`

UnsetUpdatedAt ensures that no value is present for UpdatedAt, not even an explicit nil

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


