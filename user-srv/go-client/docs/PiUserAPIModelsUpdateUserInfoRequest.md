# PiUserAPIModelsUpdateUserInfoRequest

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**Devices** | Pointer to [**[]PiUserAPIModelsUpdateDeviceRequest**](PiUserAPIModelsUpdateDeviceRequest.md) |  | [optional] 
**CustCodes** | Pointer to **[]string** |  | [optional] 
**TradingAccounts** | Pointer to **[]string** |  | [optional] 
**CitizenId** | Pointer to **NullableString** |  | [optional] 
**PhoneNumber** | Pointer to **NullableString** |  | [optional] 
**GlobalAccount** | Pointer to **NullableString** |  | [optional] 

## Methods

### NewPiUserAPIModelsUpdateUserInfoRequest

`func NewPiUserAPIModelsUpdateUserInfoRequest() *PiUserAPIModelsUpdateUserInfoRequest`

NewPiUserAPIModelsUpdateUserInfoRequest instantiates a new PiUserAPIModelsUpdateUserInfoRequest object
This constructor will assign default values to properties that have it defined,
and makes sure properties required by API are set, but the set of arguments
will change when the set of required properties is changed

### NewPiUserAPIModelsUpdateUserInfoRequestWithDefaults

`func NewPiUserAPIModelsUpdateUserInfoRequestWithDefaults() *PiUserAPIModelsUpdateUserInfoRequest`

NewPiUserAPIModelsUpdateUserInfoRequestWithDefaults instantiates a new PiUserAPIModelsUpdateUserInfoRequest object
This constructor will only assign default values to properties that have it defined,
but it doesn't guarantee that properties required by API are set

### GetDevices

`func (o *PiUserAPIModelsUpdateUserInfoRequest) GetDevices() []PiUserAPIModelsUpdateDeviceRequest`

GetDevices returns the Devices field if non-nil, zero value otherwise.

### GetDevicesOk

`func (o *PiUserAPIModelsUpdateUserInfoRequest) GetDevicesOk() (*[]PiUserAPIModelsUpdateDeviceRequest, bool)`

GetDevicesOk returns a tuple with the Devices field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetDevices

`func (o *PiUserAPIModelsUpdateUserInfoRequest) SetDevices(v []PiUserAPIModelsUpdateDeviceRequest)`

SetDevices sets Devices field to given value.

### HasDevices

`func (o *PiUserAPIModelsUpdateUserInfoRequest) HasDevices() bool`

HasDevices returns a boolean if a field has been set.

### SetDevicesNil

`func (o *PiUserAPIModelsUpdateUserInfoRequest) SetDevicesNil(b bool)`

 SetDevicesNil sets the value for Devices to be an explicit nil

### UnsetDevices
`func (o *PiUserAPIModelsUpdateUserInfoRequest) UnsetDevices()`

UnsetDevices ensures that no value is present for Devices, not even an explicit nil
### GetCustCodes

`func (o *PiUserAPIModelsUpdateUserInfoRequest) GetCustCodes() []string`

GetCustCodes returns the CustCodes field if non-nil, zero value otherwise.

### GetCustCodesOk

`func (o *PiUserAPIModelsUpdateUserInfoRequest) GetCustCodesOk() (*[]string, bool)`

GetCustCodesOk returns a tuple with the CustCodes field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetCustCodes

`func (o *PiUserAPIModelsUpdateUserInfoRequest) SetCustCodes(v []string)`

SetCustCodes sets CustCodes field to given value.

### HasCustCodes

`func (o *PiUserAPIModelsUpdateUserInfoRequest) HasCustCodes() bool`

HasCustCodes returns a boolean if a field has been set.

### SetCustCodesNil

`func (o *PiUserAPIModelsUpdateUserInfoRequest) SetCustCodesNil(b bool)`

 SetCustCodesNil sets the value for CustCodes to be an explicit nil

### UnsetCustCodes
`func (o *PiUserAPIModelsUpdateUserInfoRequest) UnsetCustCodes()`

UnsetCustCodes ensures that no value is present for CustCodes, not even an explicit nil
### GetTradingAccounts

`func (o *PiUserAPIModelsUpdateUserInfoRequest) GetTradingAccounts() []string`

GetTradingAccounts returns the TradingAccounts field if non-nil, zero value otherwise.

### GetTradingAccountsOk

`func (o *PiUserAPIModelsUpdateUserInfoRequest) GetTradingAccountsOk() (*[]string, bool)`

GetTradingAccountsOk returns a tuple with the TradingAccounts field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetTradingAccounts

`func (o *PiUserAPIModelsUpdateUserInfoRequest) SetTradingAccounts(v []string)`

SetTradingAccounts sets TradingAccounts field to given value.

### HasTradingAccounts

`func (o *PiUserAPIModelsUpdateUserInfoRequest) HasTradingAccounts() bool`

HasTradingAccounts returns a boolean if a field has been set.

### SetTradingAccountsNil

`func (o *PiUserAPIModelsUpdateUserInfoRequest) SetTradingAccountsNil(b bool)`

 SetTradingAccountsNil sets the value for TradingAccounts to be an explicit nil

### UnsetTradingAccounts
`func (o *PiUserAPIModelsUpdateUserInfoRequest) UnsetTradingAccounts()`

UnsetTradingAccounts ensures that no value is present for TradingAccounts, not even an explicit nil
### GetCitizenId

`func (o *PiUserAPIModelsUpdateUserInfoRequest) GetCitizenId() string`

GetCitizenId returns the CitizenId field if non-nil, zero value otherwise.

### GetCitizenIdOk

`func (o *PiUserAPIModelsUpdateUserInfoRequest) GetCitizenIdOk() (*string, bool)`

GetCitizenIdOk returns a tuple with the CitizenId field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetCitizenId

`func (o *PiUserAPIModelsUpdateUserInfoRequest) SetCitizenId(v string)`

SetCitizenId sets CitizenId field to given value.

### HasCitizenId

`func (o *PiUserAPIModelsUpdateUserInfoRequest) HasCitizenId() bool`

HasCitizenId returns a boolean if a field has been set.

### SetCitizenIdNil

`func (o *PiUserAPIModelsUpdateUserInfoRequest) SetCitizenIdNil(b bool)`

 SetCitizenIdNil sets the value for CitizenId to be an explicit nil

### UnsetCitizenId
`func (o *PiUserAPIModelsUpdateUserInfoRequest) UnsetCitizenId()`

UnsetCitizenId ensures that no value is present for CitizenId, not even an explicit nil
### GetPhoneNumber

`func (o *PiUserAPIModelsUpdateUserInfoRequest) GetPhoneNumber() string`

GetPhoneNumber returns the PhoneNumber field if non-nil, zero value otherwise.

### GetPhoneNumberOk

`func (o *PiUserAPIModelsUpdateUserInfoRequest) GetPhoneNumberOk() (*string, bool)`

GetPhoneNumberOk returns a tuple with the PhoneNumber field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetPhoneNumber

`func (o *PiUserAPIModelsUpdateUserInfoRequest) SetPhoneNumber(v string)`

SetPhoneNumber sets PhoneNumber field to given value.

### HasPhoneNumber

`func (o *PiUserAPIModelsUpdateUserInfoRequest) HasPhoneNumber() bool`

HasPhoneNumber returns a boolean if a field has been set.

### SetPhoneNumberNil

`func (o *PiUserAPIModelsUpdateUserInfoRequest) SetPhoneNumberNil(b bool)`

 SetPhoneNumberNil sets the value for PhoneNumber to be an explicit nil

### UnsetPhoneNumber
`func (o *PiUserAPIModelsUpdateUserInfoRequest) UnsetPhoneNumber()`

UnsetPhoneNumber ensures that no value is present for PhoneNumber, not even an explicit nil
### GetGlobalAccount

`func (o *PiUserAPIModelsUpdateUserInfoRequest) GetGlobalAccount() string`

GetGlobalAccount returns the GlobalAccount field if non-nil, zero value otherwise.

### GetGlobalAccountOk

`func (o *PiUserAPIModelsUpdateUserInfoRequest) GetGlobalAccountOk() (*string, bool)`

GetGlobalAccountOk returns a tuple with the GlobalAccount field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetGlobalAccount

`func (o *PiUserAPIModelsUpdateUserInfoRequest) SetGlobalAccount(v string)`

SetGlobalAccount sets GlobalAccount field to given value.

### HasGlobalAccount

`func (o *PiUserAPIModelsUpdateUserInfoRequest) HasGlobalAccount() bool`

HasGlobalAccount returns a boolean if a field has been set.

### SetGlobalAccountNil

`func (o *PiUserAPIModelsUpdateUserInfoRequest) SetGlobalAccountNil(b bool)`

 SetGlobalAccountNil sets the value for GlobalAccount to be an explicit nil

### UnsetGlobalAccount
`func (o *PiUserAPIModelsUpdateUserInfoRequest) UnsetGlobalAccount()`

UnsetGlobalAccount ensures that no value is present for GlobalAccount, not even an explicit nil

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


