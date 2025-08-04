# DtoMigrateUserRequestTradeAccountBankAccountsInner

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**CustomerCode** | Pointer to **string** |  | [optional] 
**TradeAccount** | Pointer to [**[]DtoMigrateUserTradeAccount**](DtoMigrateUserTradeAccount.md) |  | [optional] 

## Methods

### NewDtoMigrateUserRequestTradeAccountBankAccountsInner

`func NewDtoMigrateUserRequestTradeAccountBankAccountsInner() *DtoMigrateUserRequestTradeAccountBankAccountsInner`

NewDtoMigrateUserRequestTradeAccountBankAccountsInner instantiates a new DtoMigrateUserRequestTradeAccountBankAccountsInner object
This constructor will assign default values to properties that have it defined,
and makes sure properties required by API are set, but the set of arguments
will change when the set of required properties is changed

### NewDtoMigrateUserRequestTradeAccountBankAccountsInnerWithDefaults

`func NewDtoMigrateUserRequestTradeAccountBankAccountsInnerWithDefaults() *DtoMigrateUserRequestTradeAccountBankAccountsInner`

NewDtoMigrateUserRequestTradeAccountBankAccountsInnerWithDefaults instantiates a new DtoMigrateUserRequestTradeAccountBankAccountsInner object
This constructor will only assign default values to properties that have it defined,
but it doesn't guarantee that properties required by API are set

### GetCustomerCode

`func (o *DtoMigrateUserRequestTradeAccountBankAccountsInner) GetCustomerCode() string`

GetCustomerCode returns the CustomerCode field if non-nil, zero value otherwise.

### GetCustomerCodeOk

`func (o *DtoMigrateUserRequestTradeAccountBankAccountsInner) GetCustomerCodeOk() (*string, bool)`

GetCustomerCodeOk returns a tuple with the CustomerCode field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetCustomerCode

`func (o *DtoMigrateUserRequestTradeAccountBankAccountsInner) SetCustomerCode(v string)`

SetCustomerCode sets CustomerCode field to given value.

### HasCustomerCode

`func (o *DtoMigrateUserRequestTradeAccountBankAccountsInner) HasCustomerCode() bool`

HasCustomerCode returns a boolean if a field has been set.

### GetTradeAccount

`func (o *DtoMigrateUserRequestTradeAccountBankAccountsInner) GetTradeAccount() []DtoMigrateUserTradeAccount`

GetTradeAccount returns the TradeAccount field if non-nil, zero value otherwise.

### GetTradeAccountOk

`func (o *DtoMigrateUserRequestTradeAccountBankAccountsInner) GetTradeAccountOk() (*[]DtoMigrateUserTradeAccount, bool)`

GetTradeAccountOk returns a tuple with the TradeAccount field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetTradeAccount

`func (o *DtoMigrateUserRequestTradeAccountBankAccountsInner) SetTradeAccount(v []DtoMigrateUserTradeAccount)`

SetTradeAccount sets TradeAccount field to given value.

### HasTradeAccount

`func (o *DtoMigrateUserRequestTradeAccountBankAccountsInner) HasTradeAccount() bool`

HasTradeAccount returns a boolean if a field has been set.


[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


