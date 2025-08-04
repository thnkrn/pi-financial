# PiUserApplicationModelsUser

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**Id** | Pointer to **string** |  | [optional] 
**Devices** | Pointer to [**[]PiUserApplicationModelsDevice**](PiUserApplicationModelsDevice.md) |  | [optional] 
**CustomerCodes** | Pointer to [**[]PiUserApplicationModelsCustomerCode**](PiUserApplicationModelsCustomerCode.md) |  | [optional] 
**TradingAccounts** | Pointer to [**[]PiUserDomainAggregatesModelUserInfoAggregateTradingAccount**](PiUserDomainAggregatesModelUserInfoAggregateTradingAccount.md) |  | [optional] 
**FirstnameTh** | Pointer to **NullableString** |  | [optional] 
**LastnameTh** | Pointer to **NullableString** |  | [optional] 
**FirstnameEn** | Pointer to **NullableString** |  | [optional] 
**LastnameEn** | Pointer to **NullableString** |  | [optional] 
**PhoneNumber** | Pointer to **NullableString** |  | [optional] 
**GlobalAccount** | Pointer to **NullableString** |  | [optional] 
**Email** | Pointer to **NullableString** |  | [optional] 
**CustomerId** | Pointer to **NullableString** |  | [optional] 
**PlaceOfBirthCountry** | Pointer to **NullableString** |  | [optional] 
**PlaceOfBirthCity** | Pointer to **NullableString** |  | [optional] 
**CitizenId** | Pointer to **NullableString** |  | [optional] 
**DateOfBirth** | Pointer to **NullableString** |  | [optional] 

## Methods

### NewPiUserApplicationModelsUser

`func NewPiUserApplicationModelsUser() *PiUserApplicationModelsUser`

NewPiUserApplicationModelsUser instantiates a new PiUserApplicationModelsUser object
This constructor will assign default values to properties that have it defined,
and makes sure properties required by API are set, but the set of arguments
will change when the set of required properties is changed

### NewPiUserApplicationModelsUserWithDefaults

`func NewPiUserApplicationModelsUserWithDefaults() *PiUserApplicationModelsUser`

NewPiUserApplicationModelsUserWithDefaults instantiates a new PiUserApplicationModelsUser object
This constructor will only assign default values to properties that have it defined,
but it doesn't guarantee that properties required by API are set

### GetId

`func (o *PiUserApplicationModelsUser) GetId() string`

GetId returns the Id field if non-nil, zero value otherwise.

### GetIdOk

`func (o *PiUserApplicationModelsUser) GetIdOk() (*string, bool)`

GetIdOk returns a tuple with the Id field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetId

`func (o *PiUserApplicationModelsUser) SetId(v string)`

SetId sets Id field to given value.

### HasId

`func (o *PiUserApplicationModelsUser) HasId() bool`

HasId returns a boolean if a field has been set.

### GetDevices

`func (o *PiUserApplicationModelsUser) GetDevices() []PiUserApplicationModelsDevice`

GetDevices returns the Devices field if non-nil, zero value otherwise.

### GetDevicesOk

`func (o *PiUserApplicationModelsUser) GetDevicesOk() (*[]PiUserApplicationModelsDevice, bool)`

GetDevicesOk returns a tuple with the Devices field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetDevices

`func (o *PiUserApplicationModelsUser) SetDevices(v []PiUserApplicationModelsDevice)`

SetDevices sets Devices field to given value.

### HasDevices

`func (o *PiUserApplicationModelsUser) HasDevices() bool`

HasDevices returns a boolean if a field has been set.

### SetDevicesNil

`func (o *PiUserApplicationModelsUser) SetDevicesNil(b bool)`

 SetDevicesNil sets the value for Devices to be an explicit nil

### UnsetDevices
`func (o *PiUserApplicationModelsUser) UnsetDevices()`

UnsetDevices ensures that no value is present for Devices, not even an explicit nil
### GetCustomerCodes

`func (o *PiUserApplicationModelsUser) GetCustomerCodes() []PiUserApplicationModelsCustomerCode`

GetCustomerCodes returns the CustomerCodes field if non-nil, zero value otherwise.

### GetCustomerCodesOk

`func (o *PiUserApplicationModelsUser) GetCustomerCodesOk() (*[]PiUserApplicationModelsCustomerCode, bool)`

GetCustomerCodesOk returns a tuple with the CustomerCodes field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetCustomerCodes

`func (o *PiUserApplicationModelsUser) SetCustomerCodes(v []PiUserApplicationModelsCustomerCode)`

SetCustomerCodes sets CustomerCodes field to given value.

### HasCustomerCodes

`func (o *PiUserApplicationModelsUser) HasCustomerCodes() bool`

HasCustomerCodes returns a boolean if a field has been set.

### SetCustomerCodesNil

`func (o *PiUserApplicationModelsUser) SetCustomerCodesNil(b bool)`

 SetCustomerCodesNil sets the value for CustomerCodes to be an explicit nil

### UnsetCustomerCodes
`func (o *PiUserApplicationModelsUser) UnsetCustomerCodes()`

UnsetCustomerCodes ensures that no value is present for CustomerCodes, not even an explicit nil
### GetTradingAccounts

`func (o *PiUserApplicationModelsUser) GetTradingAccounts() []PiUserDomainAggregatesModelUserInfoAggregateTradingAccount`

GetTradingAccounts returns the TradingAccounts field if non-nil, zero value otherwise.

### GetTradingAccountsOk

`func (o *PiUserApplicationModelsUser) GetTradingAccountsOk() (*[]PiUserDomainAggregatesModelUserInfoAggregateTradingAccount, bool)`

GetTradingAccountsOk returns a tuple with the TradingAccounts field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetTradingAccounts

`func (o *PiUserApplicationModelsUser) SetTradingAccounts(v []PiUserDomainAggregatesModelUserInfoAggregateTradingAccount)`

SetTradingAccounts sets TradingAccounts field to given value.

### HasTradingAccounts

`func (o *PiUserApplicationModelsUser) HasTradingAccounts() bool`

HasTradingAccounts returns a boolean if a field has been set.

### SetTradingAccountsNil

`func (o *PiUserApplicationModelsUser) SetTradingAccountsNil(b bool)`

 SetTradingAccountsNil sets the value for TradingAccounts to be an explicit nil

### UnsetTradingAccounts
`func (o *PiUserApplicationModelsUser) UnsetTradingAccounts()`

UnsetTradingAccounts ensures that no value is present for TradingAccounts, not even an explicit nil
### GetFirstnameTh

`func (o *PiUserApplicationModelsUser) GetFirstnameTh() string`

GetFirstnameTh returns the FirstnameTh field if non-nil, zero value otherwise.

### GetFirstnameThOk

`func (o *PiUserApplicationModelsUser) GetFirstnameThOk() (*string, bool)`

GetFirstnameThOk returns a tuple with the FirstnameTh field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetFirstnameTh

`func (o *PiUserApplicationModelsUser) SetFirstnameTh(v string)`

SetFirstnameTh sets FirstnameTh field to given value.

### HasFirstnameTh

`func (o *PiUserApplicationModelsUser) HasFirstnameTh() bool`

HasFirstnameTh returns a boolean if a field has been set.

### SetFirstnameThNil

`func (o *PiUserApplicationModelsUser) SetFirstnameThNil(b bool)`

 SetFirstnameThNil sets the value for FirstnameTh to be an explicit nil

### UnsetFirstnameTh
`func (o *PiUserApplicationModelsUser) UnsetFirstnameTh()`

UnsetFirstnameTh ensures that no value is present for FirstnameTh, not even an explicit nil
### GetLastnameTh

`func (o *PiUserApplicationModelsUser) GetLastnameTh() string`

GetLastnameTh returns the LastnameTh field if non-nil, zero value otherwise.

### GetLastnameThOk

`func (o *PiUserApplicationModelsUser) GetLastnameThOk() (*string, bool)`

GetLastnameThOk returns a tuple with the LastnameTh field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetLastnameTh

`func (o *PiUserApplicationModelsUser) SetLastnameTh(v string)`

SetLastnameTh sets LastnameTh field to given value.

### HasLastnameTh

`func (o *PiUserApplicationModelsUser) HasLastnameTh() bool`

HasLastnameTh returns a boolean if a field has been set.

### SetLastnameThNil

`func (o *PiUserApplicationModelsUser) SetLastnameThNil(b bool)`

 SetLastnameThNil sets the value for LastnameTh to be an explicit nil

### UnsetLastnameTh
`func (o *PiUserApplicationModelsUser) UnsetLastnameTh()`

UnsetLastnameTh ensures that no value is present for LastnameTh, not even an explicit nil
### GetFirstnameEn

`func (o *PiUserApplicationModelsUser) GetFirstnameEn() string`

GetFirstnameEn returns the FirstnameEn field if non-nil, zero value otherwise.

### GetFirstnameEnOk

`func (o *PiUserApplicationModelsUser) GetFirstnameEnOk() (*string, bool)`

GetFirstnameEnOk returns a tuple with the FirstnameEn field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetFirstnameEn

`func (o *PiUserApplicationModelsUser) SetFirstnameEn(v string)`

SetFirstnameEn sets FirstnameEn field to given value.

### HasFirstnameEn

`func (o *PiUserApplicationModelsUser) HasFirstnameEn() bool`

HasFirstnameEn returns a boolean if a field has been set.

### SetFirstnameEnNil

`func (o *PiUserApplicationModelsUser) SetFirstnameEnNil(b bool)`

 SetFirstnameEnNil sets the value for FirstnameEn to be an explicit nil

### UnsetFirstnameEn
`func (o *PiUserApplicationModelsUser) UnsetFirstnameEn()`

UnsetFirstnameEn ensures that no value is present for FirstnameEn, not even an explicit nil
### GetLastnameEn

`func (o *PiUserApplicationModelsUser) GetLastnameEn() string`

GetLastnameEn returns the LastnameEn field if non-nil, zero value otherwise.

### GetLastnameEnOk

`func (o *PiUserApplicationModelsUser) GetLastnameEnOk() (*string, bool)`

GetLastnameEnOk returns a tuple with the LastnameEn field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetLastnameEn

`func (o *PiUserApplicationModelsUser) SetLastnameEn(v string)`

SetLastnameEn sets LastnameEn field to given value.

### HasLastnameEn

`func (o *PiUserApplicationModelsUser) HasLastnameEn() bool`

HasLastnameEn returns a boolean if a field has been set.

### SetLastnameEnNil

`func (o *PiUserApplicationModelsUser) SetLastnameEnNil(b bool)`

 SetLastnameEnNil sets the value for LastnameEn to be an explicit nil

### UnsetLastnameEn
`func (o *PiUserApplicationModelsUser) UnsetLastnameEn()`

UnsetLastnameEn ensures that no value is present for LastnameEn, not even an explicit nil
### GetPhoneNumber

`func (o *PiUserApplicationModelsUser) GetPhoneNumber() string`

GetPhoneNumber returns the PhoneNumber field if non-nil, zero value otherwise.

### GetPhoneNumberOk

`func (o *PiUserApplicationModelsUser) GetPhoneNumberOk() (*string, bool)`

GetPhoneNumberOk returns a tuple with the PhoneNumber field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetPhoneNumber

`func (o *PiUserApplicationModelsUser) SetPhoneNumber(v string)`

SetPhoneNumber sets PhoneNumber field to given value.

### HasPhoneNumber

`func (o *PiUserApplicationModelsUser) HasPhoneNumber() bool`

HasPhoneNumber returns a boolean if a field has been set.

### SetPhoneNumberNil

`func (o *PiUserApplicationModelsUser) SetPhoneNumberNil(b bool)`

 SetPhoneNumberNil sets the value for PhoneNumber to be an explicit nil

### UnsetPhoneNumber
`func (o *PiUserApplicationModelsUser) UnsetPhoneNumber()`

UnsetPhoneNumber ensures that no value is present for PhoneNumber, not even an explicit nil
### GetGlobalAccount

`func (o *PiUserApplicationModelsUser) GetGlobalAccount() string`

GetGlobalAccount returns the GlobalAccount field if non-nil, zero value otherwise.

### GetGlobalAccountOk

`func (o *PiUserApplicationModelsUser) GetGlobalAccountOk() (*string, bool)`

GetGlobalAccountOk returns a tuple with the GlobalAccount field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetGlobalAccount

`func (o *PiUserApplicationModelsUser) SetGlobalAccount(v string)`

SetGlobalAccount sets GlobalAccount field to given value.

### HasGlobalAccount

`func (o *PiUserApplicationModelsUser) HasGlobalAccount() bool`

HasGlobalAccount returns a boolean if a field has been set.

### SetGlobalAccountNil

`func (o *PiUserApplicationModelsUser) SetGlobalAccountNil(b bool)`

 SetGlobalAccountNil sets the value for GlobalAccount to be an explicit nil

### UnsetGlobalAccount
`func (o *PiUserApplicationModelsUser) UnsetGlobalAccount()`

UnsetGlobalAccount ensures that no value is present for GlobalAccount, not even an explicit nil
### GetEmail

`func (o *PiUserApplicationModelsUser) GetEmail() string`

GetEmail returns the Email field if non-nil, zero value otherwise.

### GetEmailOk

`func (o *PiUserApplicationModelsUser) GetEmailOk() (*string, bool)`

GetEmailOk returns a tuple with the Email field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetEmail

`func (o *PiUserApplicationModelsUser) SetEmail(v string)`

SetEmail sets Email field to given value.

### HasEmail

`func (o *PiUserApplicationModelsUser) HasEmail() bool`

HasEmail returns a boolean if a field has been set.

### SetEmailNil

`func (o *PiUserApplicationModelsUser) SetEmailNil(b bool)`

 SetEmailNil sets the value for Email to be an explicit nil

### UnsetEmail
`func (o *PiUserApplicationModelsUser) UnsetEmail()`

UnsetEmail ensures that no value is present for Email, not even an explicit nil
### GetCustomerId

`func (o *PiUserApplicationModelsUser) GetCustomerId() string`

GetCustomerId returns the CustomerId field if non-nil, zero value otherwise.

### GetCustomerIdOk

`func (o *PiUserApplicationModelsUser) GetCustomerIdOk() (*string, bool)`

GetCustomerIdOk returns a tuple with the CustomerId field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetCustomerId

`func (o *PiUserApplicationModelsUser) SetCustomerId(v string)`

SetCustomerId sets CustomerId field to given value.

### HasCustomerId

`func (o *PiUserApplicationModelsUser) HasCustomerId() bool`

HasCustomerId returns a boolean if a field has been set.

### SetCustomerIdNil

`func (o *PiUserApplicationModelsUser) SetCustomerIdNil(b bool)`

 SetCustomerIdNil sets the value for CustomerId to be an explicit nil

### UnsetCustomerId
`func (o *PiUserApplicationModelsUser) UnsetCustomerId()`

UnsetCustomerId ensures that no value is present for CustomerId, not even an explicit nil
### GetPlaceOfBirthCountry

`func (o *PiUserApplicationModelsUser) GetPlaceOfBirthCountry() string`

GetPlaceOfBirthCountry returns the PlaceOfBirthCountry field if non-nil, zero value otherwise.

### GetPlaceOfBirthCountryOk

`func (o *PiUserApplicationModelsUser) GetPlaceOfBirthCountryOk() (*string, bool)`

GetPlaceOfBirthCountryOk returns a tuple with the PlaceOfBirthCountry field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetPlaceOfBirthCountry

`func (o *PiUserApplicationModelsUser) SetPlaceOfBirthCountry(v string)`

SetPlaceOfBirthCountry sets PlaceOfBirthCountry field to given value.

### HasPlaceOfBirthCountry

`func (o *PiUserApplicationModelsUser) HasPlaceOfBirthCountry() bool`

HasPlaceOfBirthCountry returns a boolean if a field has been set.

### SetPlaceOfBirthCountryNil

`func (o *PiUserApplicationModelsUser) SetPlaceOfBirthCountryNil(b bool)`

 SetPlaceOfBirthCountryNil sets the value for PlaceOfBirthCountry to be an explicit nil

### UnsetPlaceOfBirthCountry
`func (o *PiUserApplicationModelsUser) UnsetPlaceOfBirthCountry()`

UnsetPlaceOfBirthCountry ensures that no value is present for PlaceOfBirthCountry, not even an explicit nil
### GetPlaceOfBirthCity

`func (o *PiUserApplicationModelsUser) GetPlaceOfBirthCity() string`

GetPlaceOfBirthCity returns the PlaceOfBirthCity field if non-nil, zero value otherwise.

### GetPlaceOfBirthCityOk

`func (o *PiUserApplicationModelsUser) GetPlaceOfBirthCityOk() (*string, bool)`

GetPlaceOfBirthCityOk returns a tuple with the PlaceOfBirthCity field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetPlaceOfBirthCity

`func (o *PiUserApplicationModelsUser) SetPlaceOfBirthCity(v string)`

SetPlaceOfBirthCity sets PlaceOfBirthCity field to given value.

### HasPlaceOfBirthCity

`func (o *PiUserApplicationModelsUser) HasPlaceOfBirthCity() bool`

HasPlaceOfBirthCity returns a boolean if a field has been set.

### SetPlaceOfBirthCityNil

`func (o *PiUserApplicationModelsUser) SetPlaceOfBirthCityNil(b bool)`

 SetPlaceOfBirthCityNil sets the value for PlaceOfBirthCity to be an explicit nil

### UnsetPlaceOfBirthCity
`func (o *PiUserApplicationModelsUser) UnsetPlaceOfBirthCity()`

UnsetPlaceOfBirthCity ensures that no value is present for PlaceOfBirthCity, not even an explicit nil
### GetCitizenId

`func (o *PiUserApplicationModelsUser) GetCitizenId() string`

GetCitizenId returns the CitizenId field if non-nil, zero value otherwise.

### GetCitizenIdOk

`func (o *PiUserApplicationModelsUser) GetCitizenIdOk() (*string, bool)`

GetCitizenIdOk returns a tuple with the CitizenId field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetCitizenId

`func (o *PiUserApplicationModelsUser) SetCitizenId(v string)`

SetCitizenId sets CitizenId field to given value.

### HasCitizenId

`func (o *PiUserApplicationModelsUser) HasCitizenId() bool`

HasCitizenId returns a boolean if a field has been set.

### SetCitizenIdNil

`func (o *PiUserApplicationModelsUser) SetCitizenIdNil(b bool)`

 SetCitizenIdNil sets the value for CitizenId to be an explicit nil

### UnsetCitizenId
`func (o *PiUserApplicationModelsUser) UnsetCitizenId()`

UnsetCitizenId ensures that no value is present for CitizenId, not even an explicit nil
### GetDateOfBirth

`func (o *PiUserApplicationModelsUser) GetDateOfBirth() string`

GetDateOfBirth returns the DateOfBirth field if non-nil, zero value otherwise.

### GetDateOfBirthOk

`func (o *PiUserApplicationModelsUser) GetDateOfBirthOk() (*string, bool)`

GetDateOfBirthOk returns a tuple with the DateOfBirth field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetDateOfBirth

`func (o *PiUserApplicationModelsUser) SetDateOfBirth(v string)`

SetDateOfBirth sets DateOfBirth field to given value.

### HasDateOfBirth

`func (o *PiUserApplicationModelsUser) HasDateOfBirth() bool`

HasDateOfBirth returns a boolean if a field has been set.

### SetDateOfBirthNil

`func (o *PiUserApplicationModelsUser) SetDateOfBirthNil(b bool)`

 SetDateOfBirthNil sets the value for DateOfBirth to be an explicit nil

### UnsetDateOfBirth
`func (o *PiUserApplicationModelsUser) UnsetDateOfBirth()`

UnsetDateOfBirth ensures that no value is present for DateOfBirth, not even an explicit nil

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


