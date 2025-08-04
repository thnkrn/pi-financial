# DtoMigrateUserRequest

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**Address** | Pointer to [**DtoMigrateUserRequestAddress**](DtoMigrateUserRequestAddress.md) |  | [optional] 
**Kyc** | Pointer to [**DtoMigrateUserKyc**](DtoMigrateUserKyc.md) |  | [optional] 
**SuitabilityTests** | Pointer to [**[]DtoMigrateUserRequestSuitabilityTestsInner**](DtoMigrateUserRequestSuitabilityTestsInner.md) |  | [optional] 
**TradeAccountBankAccounts** | Pointer to [**[]DtoMigrateUserRequestTradeAccountBankAccountsInner**](DtoMigrateUserRequestTradeAccountBankAccountsInner.md) |  | [optional] 
**UserInfo** | Pointer to [**DtoMigrateUserInfo**](DtoMigrateUserInfo.md) |  | [optional] 

## Methods

### NewDtoMigrateUserRequest

`func NewDtoMigrateUserRequest() *DtoMigrateUserRequest`

NewDtoMigrateUserRequest instantiates a new DtoMigrateUserRequest object
This constructor will assign default values to properties that have it defined,
and makes sure properties required by API are set, but the set of arguments
will change when the set of required properties is changed

### NewDtoMigrateUserRequestWithDefaults

`func NewDtoMigrateUserRequestWithDefaults() *DtoMigrateUserRequest`

NewDtoMigrateUserRequestWithDefaults instantiates a new DtoMigrateUserRequest object
This constructor will only assign default values to properties that have it defined,
but it doesn't guarantee that properties required by API are set

### GetAddress

`func (o *DtoMigrateUserRequest) GetAddress() DtoMigrateUserRequestAddress`

GetAddress returns the Address field if non-nil, zero value otherwise.

### GetAddressOk

`func (o *DtoMigrateUserRequest) GetAddressOk() (*DtoMigrateUserRequestAddress, bool)`

GetAddressOk returns a tuple with the Address field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetAddress

`func (o *DtoMigrateUserRequest) SetAddress(v DtoMigrateUserRequestAddress)`

SetAddress sets Address field to given value.

### HasAddress

`func (o *DtoMigrateUserRequest) HasAddress() bool`

HasAddress returns a boolean if a field has been set.

### GetKyc

`func (o *DtoMigrateUserRequest) GetKyc() DtoMigrateUserKyc`

GetKyc returns the Kyc field if non-nil, zero value otherwise.

### GetKycOk

`func (o *DtoMigrateUserRequest) GetKycOk() (*DtoMigrateUserKyc, bool)`

GetKycOk returns a tuple with the Kyc field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetKyc

`func (o *DtoMigrateUserRequest) SetKyc(v DtoMigrateUserKyc)`

SetKyc sets Kyc field to given value.

### HasKyc

`func (o *DtoMigrateUserRequest) HasKyc() bool`

HasKyc returns a boolean if a field has been set.

### GetSuitabilityTests

`func (o *DtoMigrateUserRequest) GetSuitabilityTests() []DtoMigrateUserRequestSuitabilityTestsInner`

GetSuitabilityTests returns the SuitabilityTests field if non-nil, zero value otherwise.

### GetSuitabilityTestsOk

`func (o *DtoMigrateUserRequest) GetSuitabilityTestsOk() (*[]DtoMigrateUserRequestSuitabilityTestsInner, bool)`

GetSuitabilityTestsOk returns a tuple with the SuitabilityTests field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetSuitabilityTests

`func (o *DtoMigrateUserRequest) SetSuitabilityTests(v []DtoMigrateUserRequestSuitabilityTestsInner)`

SetSuitabilityTests sets SuitabilityTests field to given value.

### HasSuitabilityTests

`func (o *DtoMigrateUserRequest) HasSuitabilityTests() bool`

HasSuitabilityTests returns a boolean if a field has been set.

### GetTradeAccountBankAccounts

`func (o *DtoMigrateUserRequest) GetTradeAccountBankAccounts() []DtoMigrateUserRequestTradeAccountBankAccountsInner`

GetTradeAccountBankAccounts returns the TradeAccountBankAccounts field if non-nil, zero value otherwise.

### GetTradeAccountBankAccountsOk

`func (o *DtoMigrateUserRequest) GetTradeAccountBankAccountsOk() (*[]DtoMigrateUserRequestTradeAccountBankAccountsInner, bool)`

GetTradeAccountBankAccountsOk returns a tuple with the TradeAccountBankAccounts field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetTradeAccountBankAccounts

`func (o *DtoMigrateUserRequest) SetTradeAccountBankAccounts(v []DtoMigrateUserRequestTradeAccountBankAccountsInner)`

SetTradeAccountBankAccounts sets TradeAccountBankAccounts field to given value.

### HasTradeAccountBankAccounts

`func (o *DtoMigrateUserRequest) HasTradeAccountBankAccounts() bool`

HasTradeAccountBankAccounts returns a boolean if a field has been set.

### GetUserInfo

`func (o *DtoMigrateUserRequest) GetUserInfo() DtoMigrateUserInfo`

GetUserInfo returns the UserInfo field if non-nil, zero value otherwise.

### GetUserInfoOk

`func (o *DtoMigrateUserRequest) GetUserInfoOk() (*DtoMigrateUserInfo, bool)`

GetUserInfoOk returns a tuple with the UserInfo field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetUserInfo

`func (o *DtoMigrateUserRequest) SetUserInfo(v DtoMigrateUserInfo)`

SetUserInfo sets UserInfo field to given value.

### HasUserInfo

`func (o *DtoMigrateUserRequest) HasUserInfo() bool`

HasUserInfo returns a boolean if a field has been set.


[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


