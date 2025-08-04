# DtoTradeAccountResponse

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**CustomerCode** | Pointer to **string** |  | [optional] 
**TradingAccounts** | Pointer to [**[]DtoTradingAccountResponse**](DtoTradingAccountResponse.md) |  | [optional] 

## Methods

### NewDtoTradeAccountResponse

`func NewDtoTradeAccountResponse() *DtoTradeAccountResponse`

NewDtoTradeAccountResponse instantiates a new DtoTradeAccountResponse object
This constructor will assign default values to properties that have it defined,
and makes sure properties required by API are set, but the set of arguments
will change when the set of required properties is changed

### NewDtoTradeAccountResponseWithDefaults

`func NewDtoTradeAccountResponseWithDefaults() *DtoTradeAccountResponse`

NewDtoTradeAccountResponseWithDefaults instantiates a new DtoTradeAccountResponse object
This constructor will only assign default values to properties that have it defined,
but it doesn't guarantee that properties required by API are set

### GetCustomerCode

`func (o *DtoTradeAccountResponse) GetCustomerCode() string`

GetCustomerCode returns the CustomerCode field if non-nil, zero value otherwise.

### GetCustomerCodeOk

`func (o *DtoTradeAccountResponse) GetCustomerCodeOk() (*string, bool)`

GetCustomerCodeOk returns a tuple with the CustomerCode field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetCustomerCode

`func (o *DtoTradeAccountResponse) SetCustomerCode(v string)`

SetCustomerCode sets CustomerCode field to given value.

### HasCustomerCode

`func (o *DtoTradeAccountResponse) HasCustomerCode() bool`

HasCustomerCode returns a boolean if a field has been set.

### GetTradingAccounts

`func (o *DtoTradeAccountResponse) GetTradingAccounts() []DtoTradingAccountResponse`

GetTradingAccounts returns the TradingAccounts field if non-nil, zero value otherwise.

### GetTradingAccountsOk

`func (o *DtoTradeAccountResponse) GetTradingAccountsOk() (*[]DtoTradingAccountResponse, bool)`

GetTradingAccountsOk returns a tuple with the TradingAccounts field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetTradingAccounts

`func (o *DtoTradeAccountResponse) SetTradingAccounts(v []DtoTradingAccountResponse)`

SetTradingAccounts sets TradingAccounts field to given value.

### HasTradingAccounts

`func (o *DtoTradeAccountResponse) HasTradingAccounts() bool`

HasTradingAccounts returns a boolean if a field has been set.


[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


