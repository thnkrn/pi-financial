# PiUserAPIModelsUserInfoRequest

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**Devices** | Pointer to [**[]PiUserAPIModelsDeviceRequest**](PiUserAPIModelsDeviceRequest.md) |  | [optional] 
**CustCodes** | Pointer to **[]string** |  | [optional] 
**TradingAccounts** | Pointer to **[]string** |  | [optional] 
**CitizenId** | Pointer to **NullableString** |  | [optional] 
**PhoneNumber** | Pointer to **NullableString** |  | [optional] 
**GlobalAccount** | Pointer to **NullableString** |  | [optional] 

## Methods

### NewPiUserAPIModelsUserInfoRequest

`func NewPiUserAPIModelsUserInfoRequest() *PiUserAPIModelsUserInfoRequest`

NewPiUserAPIModelsUserInfoRequest instantiates a new PiUserAPIModelsUserInfoRequest object
This constructor will assign default values to properties that have it defined,
and makes sure properties required by API are set, but the set of arguments
will change when the set of required properties is changed

### NewPiUserAPIModelsUserInfoRequestWithDefaults

`func NewPiUserAPIModelsUserInfoRequestWithDefaults() *PiUserAPIModelsUserInfoRequest`

NewPiUserAPIModelsUserInfoRequestWithDefaults instantiates a new PiUserAPIModelsUserInfoRequest object
This constructor will only assign default values to properties that have it defined,
but it doesn't guarantee that properties required by API are set

### GetDevices

`func (o *PiUserAPIModelsUserInfoRequest) GetDevices() []PiUserAPIModelsDeviceRequest`

GetDevices returns the Devices field if non-nil, zero value otherwise.

### GetDevicesOk

`func (o *PiUserAPIModelsUserInfoRequest) GetDevicesOk() (*[]PiUserAPIModelsDeviceRequest, bool)`

GetDevicesOk returns a tuple with the Devices field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetDevices

`func (o *PiUserAPIModelsUserInfoRequest) SetDevices(v []PiUserAPIModelsDeviceRequest)`

SetDevices sets Devices field to given value.

### HasDevices

`func (o *PiUserAPIModelsUserInfoRequest) HasDevices() bool`

HasDevices returns a boolean if a field has been set.

### SetDevicesNil

`func (o *PiUserAPIModelsUserInfoRequest) SetDevicesNil(b bool)`

 SetDevicesNil sets the value for Devices to be an explicit nil

### UnsetDevices
`func (o *PiUserAPIModelsUserInfoRequest) UnsetDevices()`

UnsetDevices ensures that no value is present for Devices, not even an explicit nil
### GetCustCodes

`func (o *PiUserAPIModelsUserInfoRequest) GetCustCodes() []string`

GetCustCodes returns the CustCodes field if non-nil, zero value otherwise.

### GetCustCodesOk

`func (o *PiUserAPIModelsUserInfoRequest) GetCustCodesOk() (*[]string, bool)`

GetCustCodesOk returns a tuple with the CustCodes field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetCustCodes

`func (o *PiUserAPIModelsUserInfoRequest) SetCustCodes(v []string)`

SetCustCodes sets CustCodes field to given value.

### HasCustCodes

`func (o *PiUserAPIModelsUserInfoRequest) HasCustCodes() bool`

HasCustCodes returns a boolean if a field has been set.

### SetCustCodesNil

`func (o *PiUserAPIModelsUserInfoRequest) SetCustCodesNil(b bool)`

 SetCustCodesNil sets the value for CustCodes to be an explicit nil

### UnsetCustCodes
`func (o *PiUserAPIModelsUserInfoRequest) UnsetCustCodes()`

UnsetCustCodes ensures that no value is present for CustCodes, not even an explicit nil
### GetTradingAccounts

`func (o *PiUserAPIModelsUserInfoRequest) GetTradingAccounts() []string`

GetTradingAccounts returns the TradingAccounts field if non-nil, zero value otherwise.

### GetTradingAccountsOk

`func (o *PiUserAPIModelsUserInfoRequest) GetTradingAccountsOk() (*[]string, bool)`

GetTradingAccountsOk returns a tuple with the TradingAccounts field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetTradingAccounts

`func (o *PiUserAPIModelsUserInfoRequest) SetTradingAccounts(v []string)`

SetTradingAccounts sets TradingAccounts field to given value.

### HasTradingAccounts

`func (o *PiUserAPIModelsUserInfoRequest) HasTradingAccounts() bool`

HasTradingAccounts returns a boolean if a field has been set.

### SetTradingAccountsNil

`func (o *PiUserAPIModelsUserInfoRequest) SetTradingAccountsNil(b bool)`

 SetTradingAccountsNil sets the value for TradingAccounts to be an explicit nil

### UnsetTradingAccounts
`func (o *PiUserAPIModelsUserInfoRequest) UnsetTradingAccounts()`

UnsetTradingAccounts ensures that no value is present for TradingAccounts, not even an explicit nil
### GetCitizenId

`func (o *PiUserAPIModelsUserInfoRequest) GetCitizenId() string`

GetCitizenId returns the CitizenId field if non-nil, zero value otherwise.

### GetCitizenIdOk

`func (o *PiUserAPIModelsUserInfoRequest) GetCitizenIdOk() (*string, bool)`

GetCitizenIdOk returns a tuple with the CitizenId field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetCitizenId

`func (o *PiUserAPIModelsUserInfoRequest) SetCitizenId(v string)`

SetCitizenId sets CitizenId field to given value.

### HasCitizenId

`func (o *PiUserAPIModelsUserInfoRequest) HasCitizenId() bool`

HasCitizenId returns a boolean if a field has been set.

### SetCitizenIdNil

`func (o *PiUserAPIModelsUserInfoRequest) SetCitizenIdNil(b bool)`

 SetCitizenIdNil sets the value for CitizenId to be an explicit nil

### UnsetCitizenId
`func (o *PiUserAPIModelsUserInfoRequest) UnsetCitizenId()`

UnsetCitizenId ensures that no value is present for CitizenId, not even an explicit nil
### GetPhoneNumber

`func (o *PiUserAPIModelsUserInfoRequest) GetPhoneNumber() string`

GetPhoneNumber returns the PhoneNumber field if non-nil, zero value otherwise.

### GetPhoneNumberOk

`func (o *PiUserAPIModelsUserInfoRequest) GetPhoneNumberOk() (*string, bool)`

GetPhoneNumberOk returns a tuple with the PhoneNumber field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetPhoneNumber

`func (o *PiUserAPIModelsUserInfoRequest) SetPhoneNumber(v string)`

SetPhoneNumber sets PhoneNumber field to given value.

### HasPhoneNumber

`func (o *PiUserAPIModelsUserInfoRequest) HasPhoneNumber() bool`

HasPhoneNumber returns a boolean if a field has been set.

### SetPhoneNumberNil

`func (o *PiUserAPIModelsUserInfoRequest) SetPhoneNumberNil(b bool)`

 SetPhoneNumberNil sets the value for PhoneNumber to be an explicit nil

### UnsetPhoneNumber
`func (o *PiUserAPIModelsUserInfoRequest) UnsetPhoneNumber()`

UnsetPhoneNumber ensures that no value is present for PhoneNumber, not even an explicit nil
### GetGlobalAccount

`func (o *PiUserAPIModelsUserInfoRequest) GetGlobalAccount() string`

GetGlobalAccount returns the GlobalAccount field if non-nil, zero value otherwise.

### GetGlobalAccountOk

`func (o *PiUserAPIModelsUserInfoRequest) GetGlobalAccountOk() (*string, bool)`

GetGlobalAccountOk returns a tuple with the GlobalAccount field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetGlobalAccount

`func (o *PiUserAPIModelsUserInfoRequest) SetGlobalAccount(v string)`

SetGlobalAccount sets GlobalAccount field to given value.

### HasGlobalAccount

`func (o *PiUserAPIModelsUserInfoRequest) HasGlobalAccount() bool`

HasGlobalAccount returns a boolean if a field has been set.

### SetGlobalAccountNil

`func (o *PiUserAPIModelsUserInfoRequest) SetGlobalAccountNil(b bool)`

 SetGlobalAccountNil sets the value for GlobalAccount to be an explicit nil

### UnsetGlobalAccount
`func (o *PiUserAPIModelsUserInfoRequest) UnsetGlobalAccount()`

UnsetGlobalAccount ensures that no value is present for GlobalAccount, not even an explicit nil

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


