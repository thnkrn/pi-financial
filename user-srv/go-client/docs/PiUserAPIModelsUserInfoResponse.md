# PiUserAPIModelsUserInfoResponse

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**Id** | Pointer to **string** |  | [optional] 
**Devices** | Pointer to [**[]PiUserAPIModelsDeviceResponse**](PiUserAPIModelsDeviceResponse.md) |  | [optional] 
**CustCodes** | Pointer to **[]string** |  | [optional] 
**TradingAccounts** | Pointer to **[]string** |  | [optional] 
**FirstnameTh** | Pointer to **NullableString** |  | [optional] 
**LastnameTh** | Pointer to **NullableString** |  | [optional] 
**FirstnameEn** | Pointer to **NullableString** |  | [optional] 
**LastnameEn** | Pointer to **NullableString** |  | [optional] 
**PhoneNumber** | Pointer to **NullableString** |  | [optional] 
**GlobalAccount** | Pointer to **NullableString** |  | [optional] 
**Email** | Pointer to **NullableString** |  | [optional] 
**CustomerId** | Pointer to **NullableString** |  | [optional] 
**CitizenId** | Pointer to **NullableString** |  | [optional] 

## Methods

### NewPiUserAPIModelsUserInfoResponse

`func NewPiUserAPIModelsUserInfoResponse() *PiUserAPIModelsUserInfoResponse`

NewPiUserAPIModelsUserInfoResponse instantiates a new PiUserAPIModelsUserInfoResponse object
This constructor will assign default values to properties that have it defined,
and makes sure properties required by API are set, but the set of arguments
will change when the set of required properties is changed

### NewPiUserAPIModelsUserInfoResponseWithDefaults

`func NewPiUserAPIModelsUserInfoResponseWithDefaults() *PiUserAPIModelsUserInfoResponse`

NewPiUserAPIModelsUserInfoResponseWithDefaults instantiates a new PiUserAPIModelsUserInfoResponse object
This constructor will only assign default values to properties that have it defined,
but it doesn't guarantee that properties required by API are set

### GetId

`func (o *PiUserAPIModelsUserInfoResponse) GetId() string`

GetId returns the Id field if non-nil, zero value otherwise.

### GetIdOk

`func (o *PiUserAPIModelsUserInfoResponse) GetIdOk() (*string, bool)`

GetIdOk returns a tuple with the Id field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetId

`func (o *PiUserAPIModelsUserInfoResponse) SetId(v string)`

SetId sets Id field to given value.

### HasId

`func (o *PiUserAPIModelsUserInfoResponse) HasId() bool`

HasId returns a boolean if a field has been set.

### GetDevices

`func (o *PiUserAPIModelsUserInfoResponse) GetDevices() []PiUserAPIModelsDeviceResponse`

GetDevices returns the Devices field if non-nil, zero value otherwise.

### GetDevicesOk

`func (o *PiUserAPIModelsUserInfoResponse) GetDevicesOk() (*[]PiUserAPIModelsDeviceResponse, bool)`

GetDevicesOk returns a tuple with the Devices field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetDevices

`func (o *PiUserAPIModelsUserInfoResponse) SetDevices(v []PiUserAPIModelsDeviceResponse)`

SetDevices sets Devices field to given value.

### HasDevices

`func (o *PiUserAPIModelsUserInfoResponse) HasDevices() bool`

HasDevices returns a boolean if a field has been set.

### SetDevicesNil

`func (o *PiUserAPIModelsUserInfoResponse) SetDevicesNil(b bool)`

 SetDevicesNil sets the value for Devices to be an explicit nil

### UnsetDevices
`func (o *PiUserAPIModelsUserInfoResponse) UnsetDevices()`

UnsetDevices ensures that no value is present for Devices, not even an explicit nil
### GetCustCodes

`func (o *PiUserAPIModelsUserInfoResponse) GetCustCodes() []string`

GetCustCodes returns the CustCodes field if non-nil, zero value otherwise.

### GetCustCodesOk

`func (o *PiUserAPIModelsUserInfoResponse) GetCustCodesOk() (*[]string, bool)`

GetCustCodesOk returns a tuple with the CustCodes field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetCustCodes

`func (o *PiUserAPIModelsUserInfoResponse) SetCustCodes(v []string)`

SetCustCodes sets CustCodes field to given value.

### HasCustCodes

`func (o *PiUserAPIModelsUserInfoResponse) HasCustCodes() bool`

HasCustCodes returns a boolean if a field has been set.

### SetCustCodesNil

`func (o *PiUserAPIModelsUserInfoResponse) SetCustCodesNil(b bool)`

 SetCustCodesNil sets the value for CustCodes to be an explicit nil

### UnsetCustCodes
`func (o *PiUserAPIModelsUserInfoResponse) UnsetCustCodes()`

UnsetCustCodes ensures that no value is present for CustCodes, not even an explicit nil
### GetTradingAccounts

`func (o *PiUserAPIModelsUserInfoResponse) GetTradingAccounts() []string`

GetTradingAccounts returns the TradingAccounts field if non-nil, zero value otherwise.

### GetTradingAccountsOk

`func (o *PiUserAPIModelsUserInfoResponse) GetTradingAccountsOk() (*[]string, bool)`

GetTradingAccountsOk returns a tuple with the TradingAccounts field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetTradingAccounts

`func (o *PiUserAPIModelsUserInfoResponse) SetTradingAccounts(v []string)`

SetTradingAccounts sets TradingAccounts field to given value.

### HasTradingAccounts

`func (o *PiUserAPIModelsUserInfoResponse) HasTradingAccounts() bool`

HasTradingAccounts returns a boolean if a field has been set.

### SetTradingAccountsNil

`func (o *PiUserAPIModelsUserInfoResponse) SetTradingAccountsNil(b bool)`

 SetTradingAccountsNil sets the value for TradingAccounts to be an explicit nil

### UnsetTradingAccounts
`func (o *PiUserAPIModelsUserInfoResponse) UnsetTradingAccounts()`

UnsetTradingAccounts ensures that no value is present for TradingAccounts, not even an explicit nil
### GetFirstnameTh

`func (o *PiUserAPIModelsUserInfoResponse) GetFirstnameTh() string`

GetFirstnameTh returns the FirstnameTh field if non-nil, zero value otherwise.

### GetFirstnameThOk

`func (o *PiUserAPIModelsUserInfoResponse) GetFirstnameThOk() (*string, bool)`

GetFirstnameThOk returns a tuple with the FirstnameTh field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetFirstnameTh

`func (o *PiUserAPIModelsUserInfoResponse) SetFirstnameTh(v string)`

SetFirstnameTh sets FirstnameTh field to given value.

### HasFirstnameTh

`func (o *PiUserAPIModelsUserInfoResponse) HasFirstnameTh() bool`

HasFirstnameTh returns a boolean if a field has been set.

### SetFirstnameThNil

`func (o *PiUserAPIModelsUserInfoResponse) SetFirstnameThNil(b bool)`

 SetFirstnameThNil sets the value for FirstnameTh to be an explicit nil

### UnsetFirstnameTh
`func (o *PiUserAPIModelsUserInfoResponse) UnsetFirstnameTh()`

UnsetFirstnameTh ensures that no value is present for FirstnameTh, not even an explicit nil
### GetLastnameTh

`func (o *PiUserAPIModelsUserInfoResponse) GetLastnameTh() string`

GetLastnameTh returns the LastnameTh field if non-nil, zero value otherwise.

### GetLastnameThOk

`func (o *PiUserAPIModelsUserInfoResponse) GetLastnameThOk() (*string, bool)`

GetLastnameThOk returns a tuple with the LastnameTh field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetLastnameTh

`func (o *PiUserAPIModelsUserInfoResponse) SetLastnameTh(v string)`

SetLastnameTh sets LastnameTh field to given value.

### HasLastnameTh

`func (o *PiUserAPIModelsUserInfoResponse) HasLastnameTh() bool`

HasLastnameTh returns a boolean if a field has been set.

### SetLastnameThNil

`func (o *PiUserAPIModelsUserInfoResponse) SetLastnameThNil(b bool)`

 SetLastnameThNil sets the value for LastnameTh to be an explicit nil

### UnsetLastnameTh
`func (o *PiUserAPIModelsUserInfoResponse) UnsetLastnameTh()`

UnsetLastnameTh ensures that no value is present for LastnameTh, not even an explicit nil
### GetFirstnameEn

`func (o *PiUserAPIModelsUserInfoResponse) GetFirstnameEn() string`

GetFirstnameEn returns the FirstnameEn field if non-nil, zero value otherwise.

### GetFirstnameEnOk

`func (o *PiUserAPIModelsUserInfoResponse) GetFirstnameEnOk() (*string, bool)`

GetFirstnameEnOk returns a tuple with the FirstnameEn field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetFirstnameEn

`func (o *PiUserAPIModelsUserInfoResponse) SetFirstnameEn(v string)`

SetFirstnameEn sets FirstnameEn field to given value.

### HasFirstnameEn

`func (o *PiUserAPIModelsUserInfoResponse) HasFirstnameEn() bool`

HasFirstnameEn returns a boolean if a field has been set.

### SetFirstnameEnNil

`func (o *PiUserAPIModelsUserInfoResponse) SetFirstnameEnNil(b bool)`

 SetFirstnameEnNil sets the value for FirstnameEn to be an explicit nil

### UnsetFirstnameEn
`func (o *PiUserAPIModelsUserInfoResponse) UnsetFirstnameEn()`

UnsetFirstnameEn ensures that no value is present for FirstnameEn, not even an explicit nil
### GetLastnameEn

`func (o *PiUserAPIModelsUserInfoResponse) GetLastnameEn() string`

GetLastnameEn returns the LastnameEn field if non-nil, zero value otherwise.

### GetLastnameEnOk

`func (o *PiUserAPIModelsUserInfoResponse) GetLastnameEnOk() (*string, bool)`

GetLastnameEnOk returns a tuple with the LastnameEn field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetLastnameEn

`func (o *PiUserAPIModelsUserInfoResponse) SetLastnameEn(v string)`

SetLastnameEn sets LastnameEn field to given value.

### HasLastnameEn

`func (o *PiUserAPIModelsUserInfoResponse) HasLastnameEn() bool`

HasLastnameEn returns a boolean if a field has been set.

### SetLastnameEnNil

`func (o *PiUserAPIModelsUserInfoResponse) SetLastnameEnNil(b bool)`

 SetLastnameEnNil sets the value for LastnameEn to be an explicit nil

### UnsetLastnameEn
`func (o *PiUserAPIModelsUserInfoResponse) UnsetLastnameEn()`

UnsetLastnameEn ensures that no value is present for LastnameEn, not even an explicit nil
### GetPhoneNumber

`func (o *PiUserAPIModelsUserInfoResponse) GetPhoneNumber() string`

GetPhoneNumber returns the PhoneNumber field if non-nil, zero value otherwise.

### GetPhoneNumberOk

`func (o *PiUserAPIModelsUserInfoResponse) GetPhoneNumberOk() (*string, bool)`

GetPhoneNumberOk returns a tuple with the PhoneNumber field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetPhoneNumber

`func (o *PiUserAPIModelsUserInfoResponse) SetPhoneNumber(v string)`

SetPhoneNumber sets PhoneNumber field to given value.

### HasPhoneNumber

`func (o *PiUserAPIModelsUserInfoResponse) HasPhoneNumber() bool`

HasPhoneNumber returns a boolean if a field has been set.

### SetPhoneNumberNil

`func (o *PiUserAPIModelsUserInfoResponse) SetPhoneNumberNil(b bool)`

 SetPhoneNumberNil sets the value for PhoneNumber to be an explicit nil

### UnsetPhoneNumber
`func (o *PiUserAPIModelsUserInfoResponse) UnsetPhoneNumber()`

UnsetPhoneNumber ensures that no value is present for PhoneNumber, not even an explicit nil
### GetGlobalAccount

`func (o *PiUserAPIModelsUserInfoResponse) GetGlobalAccount() string`

GetGlobalAccount returns the GlobalAccount field if non-nil, zero value otherwise.

### GetGlobalAccountOk

`func (o *PiUserAPIModelsUserInfoResponse) GetGlobalAccountOk() (*string, bool)`

GetGlobalAccountOk returns a tuple with the GlobalAccount field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetGlobalAccount

`func (o *PiUserAPIModelsUserInfoResponse) SetGlobalAccount(v string)`

SetGlobalAccount sets GlobalAccount field to given value.

### HasGlobalAccount

`func (o *PiUserAPIModelsUserInfoResponse) HasGlobalAccount() bool`

HasGlobalAccount returns a boolean if a field has been set.

### SetGlobalAccountNil

`func (o *PiUserAPIModelsUserInfoResponse) SetGlobalAccountNil(b bool)`

 SetGlobalAccountNil sets the value for GlobalAccount to be an explicit nil

### UnsetGlobalAccount
`func (o *PiUserAPIModelsUserInfoResponse) UnsetGlobalAccount()`

UnsetGlobalAccount ensures that no value is present for GlobalAccount, not even an explicit nil
### GetEmail

`func (o *PiUserAPIModelsUserInfoResponse) GetEmail() string`

GetEmail returns the Email field if non-nil, zero value otherwise.

### GetEmailOk

`func (o *PiUserAPIModelsUserInfoResponse) GetEmailOk() (*string, bool)`

GetEmailOk returns a tuple with the Email field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetEmail

`func (o *PiUserAPIModelsUserInfoResponse) SetEmail(v string)`

SetEmail sets Email field to given value.

### HasEmail

`func (o *PiUserAPIModelsUserInfoResponse) HasEmail() bool`

HasEmail returns a boolean if a field has been set.

### SetEmailNil

`func (o *PiUserAPIModelsUserInfoResponse) SetEmailNil(b bool)`

 SetEmailNil sets the value for Email to be an explicit nil

### UnsetEmail
`func (o *PiUserAPIModelsUserInfoResponse) UnsetEmail()`

UnsetEmail ensures that no value is present for Email, not even an explicit nil
### GetCustomerId

`func (o *PiUserAPIModelsUserInfoResponse) GetCustomerId() string`

GetCustomerId returns the CustomerId field if non-nil, zero value otherwise.

### GetCustomerIdOk

`func (o *PiUserAPIModelsUserInfoResponse) GetCustomerIdOk() (*string, bool)`

GetCustomerIdOk returns a tuple with the CustomerId field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetCustomerId

`func (o *PiUserAPIModelsUserInfoResponse) SetCustomerId(v string)`

SetCustomerId sets CustomerId field to given value.

### HasCustomerId

`func (o *PiUserAPIModelsUserInfoResponse) HasCustomerId() bool`

HasCustomerId returns a boolean if a field has been set.

### SetCustomerIdNil

`func (o *PiUserAPIModelsUserInfoResponse) SetCustomerIdNil(b bool)`

 SetCustomerIdNil sets the value for CustomerId to be an explicit nil

### UnsetCustomerId
`func (o *PiUserAPIModelsUserInfoResponse) UnsetCustomerId()`

UnsetCustomerId ensures that no value is present for CustomerId, not even an explicit nil
### GetCitizenId

`func (o *PiUserAPIModelsUserInfoResponse) GetCitizenId() string`

GetCitizenId returns the CitizenId field if non-nil, zero value otherwise.

### GetCitizenIdOk

`func (o *PiUserAPIModelsUserInfoResponse) GetCitizenIdOk() (*string, bool)`

GetCitizenIdOk returns a tuple with the CitizenId field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetCitizenId

`func (o *PiUserAPIModelsUserInfoResponse) SetCitizenId(v string)`

SetCitizenId sets CitizenId field to given value.

### HasCitizenId

`func (o *PiUserAPIModelsUserInfoResponse) HasCitizenId() bool`

HasCitizenId returns a boolean if a field has been set.

### SetCitizenIdNil

`func (o *PiUserAPIModelsUserInfoResponse) SetCitizenIdNil(b bool)`

 SetCitizenIdNil sets the value for CitizenId to be an explicit nil

### UnsetCitizenId
`func (o *PiUserAPIModelsUserInfoResponse) UnsetCitizenId()`

UnsetCitizenId ensures that no value is present for CitizenId, not even an explicit nil

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


