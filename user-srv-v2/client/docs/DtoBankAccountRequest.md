# DtoBankAccountRequest

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**AccountName** | **string** |  | 
**AccountNo** | **string** |  | 
**AtsEffectiveDate** | Pointer to **string** |  | [optional] 
**BankCode** | **string** |  | 
**BranchCode** | **string** |  | 
**PaymentToken** | Pointer to **string** |  | [optional] 
**Status** | **string** |  | 

## Methods

### NewDtoBankAccountRequest

`func NewDtoBankAccountRequest(accountName string, accountNo string, bankCode string, branchCode string, status string, ) *DtoBankAccountRequest`

NewDtoBankAccountRequest instantiates a new DtoBankAccountRequest object
This constructor will assign default values to properties that have it defined,
and makes sure properties required by API are set, but the set of arguments
will change when the set of required properties is changed

### NewDtoBankAccountRequestWithDefaults

`func NewDtoBankAccountRequestWithDefaults() *DtoBankAccountRequest`

NewDtoBankAccountRequestWithDefaults instantiates a new DtoBankAccountRequest object
This constructor will only assign default values to properties that have it defined,
but it doesn't guarantee that properties required by API are set

### GetAccountName

`func (o *DtoBankAccountRequest) GetAccountName() string`

GetAccountName returns the AccountName field if non-nil, zero value otherwise.

### GetAccountNameOk

`func (o *DtoBankAccountRequest) GetAccountNameOk() (*string, bool)`

GetAccountNameOk returns a tuple with the AccountName field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetAccountName

`func (o *DtoBankAccountRequest) SetAccountName(v string)`

SetAccountName sets AccountName field to given value.


### GetAccountNo

`func (o *DtoBankAccountRequest) GetAccountNo() string`

GetAccountNo returns the AccountNo field if non-nil, zero value otherwise.

### GetAccountNoOk

`func (o *DtoBankAccountRequest) GetAccountNoOk() (*string, bool)`

GetAccountNoOk returns a tuple with the AccountNo field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetAccountNo

`func (o *DtoBankAccountRequest) SetAccountNo(v string)`

SetAccountNo sets AccountNo field to given value.


### GetAtsEffectiveDate

`func (o *DtoBankAccountRequest) GetAtsEffectiveDate() string`

GetAtsEffectiveDate returns the AtsEffectiveDate field if non-nil, zero value otherwise.

### GetAtsEffectiveDateOk

`func (o *DtoBankAccountRequest) GetAtsEffectiveDateOk() (*string, bool)`

GetAtsEffectiveDateOk returns a tuple with the AtsEffectiveDate field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetAtsEffectiveDate

`func (o *DtoBankAccountRequest) SetAtsEffectiveDate(v string)`

SetAtsEffectiveDate sets AtsEffectiveDate field to given value.

### HasAtsEffectiveDate

`func (o *DtoBankAccountRequest) HasAtsEffectiveDate() bool`

HasAtsEffectiveDate returns a boolean if a field has been set.

### GetBankCode

`func (o *DtoBankAccountRequest) GetBankCode() string`

GetBankCode returns the BankCode field if non-nil, zero value otherwise.

### GetBankCodeOk

`func (o *DtoBankAccountRequest) GetBankCodeOk() (*string, bool)`

GetBankCodeOk returns a tuple with the BankCode field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetBankCode

`func (o *DtoBankAccountRequest) SetBankCode(v string)`

SetBankCode sets BankCode field to given value.


### GetBranchCode

`func (o *DtoBankAccountRequest) GetBranchCode() string`

GetBranchCode returns the BranchCode field if non-nil, zero value otherwise.

### GetBranchCodeOk

`func (o *DtoBankAccountRequest) GetBranchCodeOk() (*string, bool)`

GetBranchCodeOk returns a tuple with the BranchCode field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetBranchCode

`func (o *DtoBankAccountRequest) SetBranchCode(v string)`

SetBranchCode sets BranchCode field to given value.


### GetPaymentToken

`func (o *DtoBankAccountRequest) GetPaymentToken() string`

GetPaymentToken returns the PaymentToken field if non-nil, zero value otherwise.

### GetPaymentTokenOk

`func (o *DtoBankAccountRequest) GetPaymentTokenOk() (*string, bool)`

GetPaymentTokenOk returns a tuple with the PaymentToken field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetPaymentToken

`func (o *DtoBankAccountRequest) SetPaymentToken(v string)`

SetPaymentToken sets PaymentToken field to given value.

### HasPaymentToken

`func (o *DtoBankAccountRequest) HasPaymentToken() bool`

HasPaymentToken returns a boolean if a field has been set.

### GetStatus

`func (o *DtoBankAccountRequest) GetStatus() string`

GetStatus returns the Status field if non-nil, zero value otherwise.

### GetStatusOk

`func (o *DtoBankAccountRequest) GetStatusOk() (*string, bool)`

GetStatusOk returns a tuple with the Status field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetStatus

`func (o *DtoBankAccountRequest) SetStatus(v string)`

SetStatus sets Status field to given value.



[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


