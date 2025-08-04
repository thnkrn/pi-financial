# PiCommonDomainAggregatesModelProductAggregateProduct

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**RowVersion** | **string** |  | [readonly] 
**Id** | Pointer to **string** |  | [optional] [readonly] 
**Name** | Pointer to **string** |  | [optional] 
**ExchangeMarketId** | Pointer to **NullableString** |  | [optional] 
**AccountTypeCode** | Pointer to **NullableString** |  | [optional] 
**AccountType** | Pointer to **NullableString** |  | [optional] 
**Suffix** | Pointer to **NullableString** |  | [optional] 
**TransactionType** | Pointer to **NullableString** |  | [optional] 

## Methods

### NewPiCommonDomainAggregatesModelProductAggregateProduct

`func NewPiCommonDomainAggregatesModelProductAggregateProduct(rowVersion string, ) *PiCommonDomainAggregatesModelProductAggregateProduct`

NewPiCommonDomainAggregatesModelProductAggregateProduct instantiates a new PiCommonDomainAggregatesModelProductAggregateProduct object
This constructor will assign default values to properties that have it defined,
and makes sure properties required by API are set, but the set of arguments
will change when the set of required properties is changed

### NewPiCommonDomainAggregatesModelProductAggregateProductWithDefaults

`func NewPiCommonDomainAggregatesModelProductAggregateProductWithDefaults() *PiCommonDomainAggregatesModelProductAggregateProduct`

NewPiCommonDomainAggregatesModelProductAggregateProductWithDefaults instantiates a new PiCommonDomainAggregatesModelProductAggregateProduct object
This constructor will only assign default values to properties that have it defined,
but it doesn't guarantee that properties required by API are set

### GetRowVersion

`func (o *PiCommonDomainAggregatesModelProductAggregateProduct) GetRowVersion() string`

GetRowVersion returns the RowVersion field if non-nil, zero value otherwise.

### GetRowVersionOk

`func (o *PiCommonDomainAggregatesModelProductAggregateProduct) GetRowVersionOk() (*string, bool)`

GetRowVersionOk returns a tuple with the RowVersion field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetRowVersion

`func (o *PiCommonDomainAggregatesModelProductAggregateProduct) SetRowVersion(v string)`

SetRowVersion sets RowVersion field to given value.


### GetId

`func (o *PiCommonDomainAggregatesModelProductAggregateProduct) GetId() string`

GetId returns the Id field if non-nil, zero value otherwise.

### GetIdOk

`func (o *PiCommonDomainAggregatesModelProductAggregateProduct) GetIdOk() (*string, bool)`

GetIdOk returns a tuple with the Id field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetId

`func (o *PiCommonDomainAggregatesModelProductAggregateProduct) SetId(v string)`

SetId sets Id field to given value.

### HasId

`func (o *PiCommonDomainAggregatesModelProductAggregateProduct) HasId() bool`

HasId returns a boolean if a field has been set.

### GetName

`func (o *PiCommonDomainAggregatesModelProductAggregateProduct) GetName() string`

GetName returns the Name field if non-nil, zero value otherwise.

### GetNameOk

`func (o *PiCommonDomainAggregatesModelProductAggregateProduct) GetNameOk() (*string, bool)`

GetNameOk returns a tuple with the Name field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetName

`func (o *PiCommonDomainAggregatesModelProductAggregateProduct) SetName(v string)`

SetName sets Name field to given value.

### HasName

`func (o *PiCommonDomainAggregatesModelProductAggregateProduct) HasName() bool`

HasName returns a boolean if a field has been set.

### GetExchangeMarketId

`func (o *PiCommonDomainAggregatesModelProductAggregateProduct) GetExchangeMarketId() string`

GetExchangeMarketId returns the ExchangeMarketId field if non-nil, zero value otherwise.

### GetExchangeMarketIdOk

`func (o *PiCommonDomainAggregatesModelProductAggregateProduct) GetExchangeMarketIdOk() (*string, bool)`

GetExchangeMarketIdOk returns a tuple with the ExchangeMarketId field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetExchangeMarketId

`func (o *PiCommonDomainAggregatesModelProductAggregateProduct) SetExchangeMarketId(v string)`

SetExchangeMarketId sets ExchangeMarketId field to given value.

### HasExchangeMarketId

`func (o *PiCommonDomainAggregatesModelProductAggregateProduct) HasExchangeMarketId() bool`

HasExchangeMarketId returns a boolean if a field has been set.

### SetExchangeMarketIdNil

`func (o *PiCommonDomainAggregatesModelProductAggregateProduct) SetExchangeMarketIdNil(b bool)`

 SetExchangeMarketIdNil sets the value for ExchangeMarketId to be an explicit nil

### UnsetExchangeMarketId
`func (o *PiCommonDomainAggregatesModelProductAggregateProduct) UnsetExchangeMarketId()`

UnsetExchangeMarketId ensures that no value is present for ExchangeMarketId, not even an explicit nil
### GetAccountTypeCode

`func (o *PiCommonDomainAggregatesModelProductAggregateProduct) GetAccountTypeCode() string`

GetAccountTypeCode returns the AccountTypeCode field if non-nil, zero value otherwise.

### GetAccountTypeCodeOk

`func (o *PiCommonDomainAggregatesModelProductAggregateProduct) GetAccountTypeCodeOk() (*string, bool)`

GetAccountTypeCodeOk returns a tuple with the AccountTypeCode field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetAccountTypeCode

`func (o *PiCommonDomainAggregatesModelProductAggregateProduct) SetAccountTypeCode(v string)`

SetAccountTypeCode sets AccountTypeCode field to given value.

### HasAccountTypeCode

`func (o *PiCommonDomainAggregatesModelProductAggregateProduct) HasAccountTypeCode() bool`

HasAccountTypeCode returns a boolean if a field has been set.

### SetAccountTypeCodeNil

`func (o *PiCommonDomainAggregatesModelProductAggregateProduct) SetAccountTypeCodeNil(b bool)`

 SetAccountTypeCodeNil sets the value for AccountTypeCode to be an explicit nil

### UnsetAccountTypeCode
`func (o *PiCommonDomainAggregatesModelProductAggregateProduct) UnsetAccountTypeCode()`

UnsetAccountTypeCode ensures that no value is present for AccountTypeCode, not even an explicit nil
### GetAccountType

`func (o *PiCommonDomainAggregatesModelProductAggregateProduct) GetAccountType() string`

GetAccountType returns the AccountType field if non-nil, zero value otherwise.

### GetAccountTypeOk

`func (o *PiCommonDomainAggregatesModelProductAggregateProduct) GetAccountTypeOk() (*string, bool)`

GetAccountTypeOk returns a tuple with the AccountType field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetAccountType

`func (o *PiCommonDomainAggregatesModelProductAggregateProduct) SetAccountType(v string)`

SetAccountType sets AccountType field to given value.

### HasAccountType

`func (o *PiCommonDomainAggregatesModelProductAggregateProduct) HasAccountType() bool`

HasAccountType returns a boolean if a field has been set.

### SetAccountTypeNil

`func (o *PiCommonDomainAggregatesModelProductAggregateProduct) SetAccountTypeNil(b bool)`

 SetAccountTypeNil sets the value for AccountType to be an explicit nil

### UnsetAccountType
`func (o *PiCommonDomainAggregatesModelProductAggregateProduct) UnsetAccountType()`

UnsetAccountType ensures that no value is present for AccountType, not even an explicit nil
### GetSuffix

`func (o *PiCommonDomainAggregatesModelProductAggregateProduct) GetSuffix() string`

GetSuffix returns the Suffix field if non-nil, zero value otherwise.

### GetSuffixOk

`func (o *PiCommonDomainAggregatesModelProductAggregateProduct) GetSuffixOk() (*string, bool)`

GetSuffixOk returns a tuple with the Suffix field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetSuffix

`func (o *PiCommonDomainAggregatesModelProductAggregateProduct) SetSuffix(v string)`

SetSuffix sets Suffix field to given value.

### HasSuffix

`func (o *PiCommonDomainAggregatesModelProductAggregateProduct) HasSuffix() bool`

HasSuffix returns a boolean if a field has been set.

### SetSuffixNil

`func (o *PiCommonDomainAggregatesModelProductAggregateProduct) SetSuffixNil(b bool)`

 SetSuffixNil sets the value for Suffix to be an explicit nil

### UnsetSuffix
`func (o *PiCommonDomainAggregatesModelProductAggregateProduct) UnsetSuffix()`

UnsetSuffix ensures that no value is present for Suffix, not even an explicit nil
### GetTransactionType

`func (o *PiCommonDomainAggregatesModelProductAggregateProduct) GetTransactionType() string`

GetTransactionType returns the TransactionType field if non-nil, zero value otherwise.

### GetTransactionTypeOk

`func (o *PiCommonDomainAggregatesModelProductAggregateProduct) GetTransactionTypeOk() (*string, bool)`

GetTransactionTypeOk returns a tuple with the TransactionType field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetTransactionType

`func (o *PiCommonDomainAggregatesModelProductAggregateProduct) SetTransactionType(v string)`

SetTransactionType sets TransactionType field to given value.

### HasTransactionType

`func (o *PiCommonDomainAggregatesModelProductAggregateProduct) HasTransactionType() bool`

HasTransactionType returns a boolean if a field has been set.

### SetTransactionTypeNil

`func (o *PiCommonDomainAggregatesModelProductAggregateProduct) SetTransactionTypeNil(b bool)`

 SetTransactionTypeNil sets the value for TransactionType to be an explicit nil

### UnsetTransactionType
`func (o *PiCommonDomainAggregatesModelProductAggregateProduct) UnsetTransactionType()`

UnsetTransactionType ensures that no value is present for TransactionType, not even an explicit nil

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


