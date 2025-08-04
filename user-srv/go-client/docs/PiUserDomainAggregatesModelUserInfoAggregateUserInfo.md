# PiUserDomainAggregatesModelUserInfoAggregateUserInfo

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**RowVersion** | **string** |  | [readonly] 
**Id** | Pointer to **string** |  | [optional] [readonly] 
**CustomerId** | Pointer to **NullableString** |  | [optional] 
**CustCodes** | Pointer to [**[]PiUserDomainAggregatesModelUserInfoAggregateCustCode**](PiUserDomainAggregatesModelUserInfoAggregateCustCode.md) |  | [optional] [readonly] 
**TradingAccounts** | Pointer to [**[]PiUserDomainAggregatesModelUserInfoAggregateTradingAccount**](PiUserDomainAggregatesModelUserInfoAggregateTradingAccount.md) |  | [optional] [readonly] 
**Devices** | Pointer to [**[]PiUserDomainAggregatesModelUserInfoAggregateDevice**](PiUserDomainAggregatesModelUserInfoAggregateDevice.md) |  | [optional] [readonly] 
**NotificationPreferences** | Pointer to [**[]PiUserDomainAggregatesModelUserInfoAggregateNotificationPreference**](PiUserDomainAggregatesModelUserInfoAggregateNotificationPreference.md) |  | [optional] [readonly] 
**FirstnameTh** | Pointer to **NullableString** |  | [optional] [readonly] 
**LastnameTh** | Pointer to **NullableString** |  | [optional] [readonly] 
**FirstnameEn** | Pointer to **NullableString** |  | [optional] [readonly] 
**LastnameEn** | Pointer to **NullableString** |  | [optional] [readonly] 
**CitizenId** | Pointer to **NullableString** |  | [optional] [readonly] 
**CitizenIdHash** | Pointer to **NullableString** |  | [optional] 
**PhoneNumber** | Pointer to **NullableString** |  | [optional] [readonly] 
**PhoneNumberHash** | Pointer to **NullableString** |  | [optional] [readonly] 
**Email** | Pointer to **NullableString** |  | [optional] [readonly] 
**EmailHash** | Pointer to **NullableString** |  | [optional] [readonly] 
**GlobalAccount** | Pointer to **NullableString** |  | [optional] 
**PlaceOfBirthCountry** | Pointer to **NullableString** |  | [optional] [readonly] 
**PlaceOfBirthCity** | Pointer to **NullableString** |  | [optional] [readonly] 
**DateOfBirth** | Pointer to **NullableString** |  | [optional] [readonly] 
**BankAccounts** | Pointer to [**[]PiUserDomainAggregatesModelBankAccountAggregateBankAccount**](PiUserDomainAggregatesModelBankAccountAggregateBankAccount.md) |  | [optional] [readonly] 
**Documents** | Pointer to [**[]PiUserDomainAggregatesModelDocumentAggregateDocument**](PiUserDomainAggregatesModelDocumentAggregateDocument.md) |  | [optional] [readonly] 
**Examinations** | Pointer to [**[]PiUserDomainAggregatesModelExamAggregateExamination**](PiUserDomainAggregatesModelExamAggregateExamination.md) |  | [optional] [readonly] 
**CreatedAt** | Pointer to **time.Time** |  | [optional] 
**UpdatedAt** | Pointer to **NullableTime** |  | [optional] 
**UserAccounts** | Pointer to [**[]PiUserDomainAggregatesModelUserAccountAggregateUserAccount**](PiUserDomainAggregatesModelUserAccountAggregateUserAccount.md) |  | [optional] [readonly] 
**Kyc** | Pointer to [**PiUserDomainAggregatesModelKycAggregateKyc**](PiUserDomainAggregatesModelKycAggregateKyc.md) |  | [optional] 
**Address** | Pointer to [**PiUserDomainAggregatesModelAddressAggregateAddress**](PiUserDomainAggregatesModelAddressAggregateAddress.md) |  | [optional] 
**SuitabilityTests** | Pointer to [**[]PiUserDomainAggregatesModelSuitabilityTestAggregateSuitabilityTest**](PiUserDomainAggregatesModelSuitabilityTestAggregateSuitabilityTest.md) |  | [optional] [readonly] 
**BankAccountsV2** | Pointer to [**[]PiUserDomainAggregatesModelBankAccountAggregateBankAccountV2**](PiUserDomainAggregatesModelBankAccountAggregateBankAccountV2.md) |  | [optional] [readonly] 

## Methods

### NewPiUserDomainAggregatesModelUserInfoAggregateUserInfo

`func NewPiUserDomainAggregatesModelUserInfoAggregateUserInfo(rowVersion string, ) *PiUserDomainAggregatesModelUserInfoAggregateUserInfo`

NewPiUserDomainAggregatesModelUserInfoAggregateUserInfo instantiates a new PiUserDomainAggregatesModelUserInfoAggregateUserInfo object
This constructor will assign default values to properties that have it defined,
and makes sure properties required by API are set, but the set of arguments
will change when the set of required properties is changed

### NewPiUserDomainAggregatesModelUserInfoAggregateUserInfoWithDefaults

`func NewPiUserDomainAggregatesModelUserInfoAggregateUserInfoWithDefaults() *PiUserDomainAggregatesModelUserInfoAggregateUserInfo`

NewPiUserDomainAggregatesModelUserInfoAggregateUserInfoWithDefaults instantiates a new PiUserDomainAggregatesModelUserInfoAggregateUserInfo object
This constructor will only assign default values to properties that have it defined,
but it doesn't guarantee that properties required by API are set

### GetRowVersion

`func (o *PiUserDomainAggregatesModelUserInfoAggregateUserInfo) GetRowVersion() string`

GetRowVersion returns the RowVersion field if non-nil, zero value otherwise.

### GetRowVersionOk

`func (o *PiUserDomainAggregatesModelUserInfoAggregateUserInfo) GetRowVersionOk() (*string, bool)`

GetRowVersionOk returns a tuple with the RowVersion field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetRowVersion

`func (o *PiUserDomainAggregatesModelUserInfoAggregateUserInfo) SetRowVersion(v string)`

SetRowVersion sets RowVersion field to given value.


### GetId

`func (o *PiUserDomainAggregatesModelUserInfoAggregateUserInfo) GetId() string`

GetId returns the Id field if non-nil, zero value otherwise.

### GetIdOk

`func (o *PiUserDomainAggregatesModelUserInfoAggregateUserInfo) GetIdOk() (*string, bool)`

GetIdOk returns a tuple with the Id field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetId

`func (o *PiUserDomainAggregatesModelUserInfoAggregateUserInfo) SetId(v string)`

SetId sets Id field to given value.

### HasId

`func (o *PiUserDomainAggregatesModelUserInfoAggregateUserInfo) HasId() bool`

HasId returns a boolean if a field has been set.

### GetCustomerId

`func (o *PiUserDomainAggregatesModelUserInfoAggregateUserInfo) GetCustomerId() string`

GetCustomerId returns the CustomerId field if non-nil, zero value otherwise.

### GetCustomerIdOk

`func (o *PiUserDomainAggregatesModelUserInfoAggregateUserInfo) GetCustomerIdOk() (*string, bool)`

GetCustomerIdOk returns a tuple with the CustomerId field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetCustomerId

`func (o *PiUserDomainAggregatesModelUserInfoAggregateUserInfo) SetCustomerId(v string)`

SetCustomerId sets CustomerId field to given value.

### HasCustomerId

`func (o *PiUserDomainAggregatesModelUserInfoAggregateUserInfo) HasCustomerId() bool`

HasCustomerId returns a boolean if a field has been set.

### SetCustomerIdNil

`func (o *PiUserDomainAggregatesModelUserInfoAggregateUserInfo) SetCustomerIdNil(b bool)`

 SetCustomerIdNil sets the value for CustomerId to be an explicit nil

### UnsetCustomerId
`func (o *PiUserDomainAggregatesModelUserInfoAggregateUserInfo) UnsetCustomerId()`

UnsetCustomerId ensures that no value is present for CustomerId, not even an explicit nil
### GetCustCodes

`func (o *PiUserDomainAggregatesModelUserInfoAggregateUserInfo) GetCustCodes() []PiUserDomainAggregatesModelUserInfoAggregateCustCode`

GetCustCodes returns the CustCodes field if non-nil, zero value otherwise.

### GetCustCodesOk

`func (o *PiUserDomainAggregatesModelUserInfoAggregateUserInfo) GetCustCodesOk() (*[]PiUserDomainAggregatesModelUserInfoAggregateCustCode, bool)`

GetCustCodesOk returns a tuple with the CustCodes field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetCustCodes

`func (o *PiUserDomainAggregatesModelUserInfoAggregateUserInfo) SetCustCodes(v []PiUserDomainAggregatesModelUserInfoAggregateCustCode)`

SetCustCodes sets CustCodes field to given value.

### HasCustCodes

`func (o *PiUserDomainAggregatesModelUserInfoAggregateUserInfo) HasCustCodes() bool`

HasCustCodes returns a boolean if a field has been set.

### SetCustCodesNil

`func (o *PiUserDomainAggregatesModelUserInfoAggregateUserInfo) SetCustCodesNil(b bool)`

 SetCustCodesNil sets the value for CustCodes to be an explicit nil

### UnsetCustCodes
`func (o *PiUserDomainAggregatesModelUserInfoAggregateUserInfo) UnsetCustCodes()`

UnsetCustCodes ensures that no value is present for CustCodes, not even an explicit nil
### GetTradingAccounts

`func (o *PiUserDomainAggregatesModelUserInfoAggregateUserInfo) GetTradingAccounts() []PiUserDomainAggregatesModelUserInfoAggregateTradingAccount`

GetTradingAccounts returns the TradingAccounts field if non-nil, zero value otherwise.

### GetTradingAccountsOk

`func (o *PiUserDomainAggregatesModelUserInfoAggregateUserInfo) GetTradingAccountsOk() (*[]PiUserDomainAggregatesModelUserInfoAggregateTradingAccount, bool)`

GetTradingAccountsOk returns a tuple with the TradingAccounts field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetTradingAccounts

`func (o *PiUserDomainAggregatesModelUserInfoAggregateUserInfo) SetTradingAccounts(v []PiUserDomainAggregatesModelUserInfoAggregateTradingAccount)`

SetTradingAccounts sets TradingAccounts field to given value.

### HasTradingAccounts

`func (o *PiUserDomainAggregatesModelUserInfoAggregateUserInfo) HasTradingAccounts() bool`

HasTradingAccounts returns a boolean if a field has been set.

### SetTradingAccountsNil

`func (o *PiUserDomainAggregatesModelUserInfoAggregateUserInfo) SetTradingAccountsNil(b bool)`

 SetTradingAccountsNil sets the value for TradingAccounts to be an explicit nil

### UnsetTradingAccounts
`func (o *PiUserDomainAggregatesModelUserInfoAggregateUserInfo) UnsetTradingAccounts()`

UnsetTradingAccounts ensures that no value is present for TradingAccounts, not even an explicit nil
### GetDevices

`func (o *PiUserDomainAggregatesModelUserInfoAggregateUserInfo) GetDevices() []PiUserDomainAggregatesModelUserInfoAggregateDevice`

GetDevices returns the Devices field if non-nil, zero value otherwise.

### GetDevicesOk

`func (o *PiUserDomainAggregatesModelUserInfoAggregateUserInfo) GetDevicesOk() (*[]PiUserDomainAggregatesModelUserInfoAggregateDevice, bool)`

GetDevicesOk returns a tuple with the Devices field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetDevices

`func (o *PiUserDomainAggregatesModelUserInfoAggregateUserInfo) SetDevices(v []PiUserDomainAggregatesModelUserInfoAggregateDevice)`

SetDevices sets Devices field to given value.

### HasDevices

`func (o *PiUserDomainAggregatesModelUserInfoAggregateUserInfo) HasDevices() bool`

HasDevices returns a boolean if a field has been set.

### SetDevicesNil

`func (o *PiUserDomainAggregatesModelUserInfoAggregateUserInfo) SetDevicesNil(b bool)`

 SetDevicesNil sets the value for Devices to be an explicit nil

### UnsetDevices
`func (o *PiUserDomainAggregatesModelUserInfoAggregateUserInfo) UnsetDevices()`

UnsetDevices ensures that no value is present for Devices, not even an explicit nil
### GetNotificationPreferences

`func (o *PiUserDomainAggregatesModelUserInfoAggregateUserInfo) GetNotificationPreferences() []PiUserDomainAggregatesModelUserInfoAggregateNotificationPreference`

GetNotificationPreferences returns the NotificationPreferences field if non-nil, zero value otherwise.

### GetNotificationPreferencesOk

`func (o *PiUserDomainAggregatesModelUserInfoAggregateUserInfo) GetNotificationPreferencesOk() (*[]PiUserDomainAggregatesModelUserInfoAggregateNotificationPreference, bool)`

GetNotificationPreferencesOk returns a tuple with the NotificationPreferences field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetNotificationPreferences

`func (o *PiUserDomainAggregatesModelUserInfoAggregateUserInfo) SetNotificationPreferences(v []PiUserDomainAggregatesModelUserInfoAggregateNotificationPreference)`

SetNotificationPreferences sets NotificationPreferences field to given value.

### HasNotificationPreferences

`func (o *PiUserDomainAggregatesModelUserInfoAggregateUserInfo) HasNotificationPreferences() bool`

HasNotificationPreferences returns a boolean if a field has been set.

### SetNotificationPreferencesNil

`func (o *PiUserDomainAggregatesModelUserInfoAggregateUserInfo) SetNotificationPreferencesNil(b bool)`

 SetNotificationPreferencesNil sets the value for NotificationPreferences to be an explicit nil

### UnsetNotificationPreferences
`func (o *PiUserDomainAggregatesModelUserInfoAggregateUserInfo) UnsetNotificationPreferences()`

UnsetNotificationPreferences ensures that no value is present for NotificationPreferences, not even an explicit nil
### GetFirstnameTh

`func (o *PiUserDomainAggregatesModelUserInfoAggregateUserInfo) GetFirstnameTh() string`

GetFirstnameTh returns the FirstnameTh field if non-nil, zero value otherwise.

### GetFirstnameThOk

`func (o *PiUserDomainAggregatesModelUserInfoAggregateUserInfo) GetFirstnameThOk() (*string, bool)`

GetFirstnameThOk returns a tuple with the FirstnameTh field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetFirstnameTh

`func (o *PiUserDomainAggregatesModelUserInfoAggregateUserInfo) SetFirstnameTh(v string)`

SetFirstnameTh sets FirstnameTh field to given value.

### HasFirstnameTh

`func (o *PiUserDomainAggregatesModelUserInfoAggregateUserInfo) HasFirstnameTh() bool`

HasFirstnameTh returns a boolean if a field has been set.

### SetFirstnameThNil

`func (o *PiUserDomainAggregatesModelUserInfoAggregateUserInfo) SetFirstnameThNil(b bool)`

 SetFirstnameThNil sets the value for FirstnameTh to be an explicit nil

### UnsetFirstnameTh
`func (o *PiUserDomainAggregatesModelUserInfoAggregateUserInfo) UnsetFirstnameTh()`

UnsetFirstnameTh ensures that no value is present for FirstnameTh, not even an explicit nil
### GetLastnameTh

`func (o *PiUserDomainAggregatesModelUserInfoAggregateUserInfo) GetLastnameTh() string`

GetLastnameTh returns the LastnameTh field if non-nil, zero value otherwise.

### GetLastnameThOk

`func (o *PiUserDomainAggregatesModelUserInfoAggregateUserInfo) GetLastnameThOk() (*string, bool)`

GetLastnameThOk returns a tuple with the LastnameTh field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetLastnameTh

`func (o *PiUserDomainAggregatesModelUserInfoAggregateUserInfo) SetLastnameTh(v string)`

SetLastnameTh sets LastnameTh field to given value.

### HasLastnameTh

`func (o *PiUserDomainAggregatesModelUserInfoAggregateUserInfo) HasLastnameTh() bool`

HasLastnameTh returns a boolean if a field has been set.

### SetLastnameThNil

`func (o *PiUserDomainAggregatesModelUserInfoAggregateUserInfo) SetLastnameThNil(b bool)`

 SetLastnameThNil sets the value for LastnameTh to be an explicit nil

### UnsetLastnameTh
`func (o *PiUserDomainAggregatesModelUserInfoAggregateUserInfo) UnsetLastnameTh()`

UnsetLastnameTh ensures that no value is present for LastnameTh, not even an explicit nil
### GetFirstnameEn

`func (o *PiUserDomainAggregatesModelUserInfoAggregateUserInfo) GetFirstnameEn() string`

GetFirstnameEn returns the FirstnameEn field if non-nil, zero value otherwise.

### GetFirstnameEnOk

`func (o *PiUserDomainAggregatesModelUserInfoAggregateUserInfo) GetFirstnameEnOk() (*string, bool)`

GetFirstnameEnOk returns a tuple with the FirstnameEn field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetFirstnameEn

`func (o *PiUserDomainAggregatesModelUserInfoAggregateUserInfo) SetFirstnameEn(v string)`

SetFirstnameEn sets FirstnameEn field to given value.

### HasFirstnameEn

`func (o *PiUserDomainAggregatesModelUserInfoAggregateUserInfo) HasFirstnameEn() bool`

HasFirstnameEn returns a boolean if a field has been set.

### SetFirstnameEnNil

`func (o *PiUserDomainAggregatesModelUserInfoAggregateUserInfo) SetFirstnameEnNil(b bool)`

 SetFirstnameEnNil sets the value for FirstnameEn to be an explicit nil

### UnsetFirstnameEn
`func (o *PiUserDomainAggregatesModelUserInfoAggregateUserInfo) UnsetFirstnameEn()`

UnsetFirstnameEn ensures that no value is present for FirstnameEn, not even an explicit nil
### GetLastnameEn

`func (o *PiUserDomainAggregatesModelUserInfoAggregateUserInfo) GetLastnameEn() string`

GetLastnameEn returns the LastnameEn field if non-nil, zero value otherwise.

### GetLastnameEnOk

`func (o *PiUserDomainAggregatesModelUserInfoAggregateUserInfo) GetLastnameEnOk() (*string, bool)`

GetLastnameEnOk returns a tuple with the LastnameEn field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetLastnameEn

`func (o *PiUserDomainAggregatesModelUserInfoAggregateUserInfo) SetLastnameEn(v string)`

SetLastnameEn sets LastnameEn field to given value.

### HasLastnameEn

`func (o *PiUserDomainAggregatesModelUserInfoAggregateUserInfo) HasLastnameEn() bool`

HasLastnameEn returns a boolean if a field has been set.

### SetLastnameEnNil

`func (o *PiUserDomainAggregatesModelUserInfoAggregateUserInfo) SetLastnameEnNil(b bool)`

 SetLastnameEnNil sets the value for LastnameEn to be an explicit nil

### UnsetLastnameEn
`func (o *PiUserDomainAggregatesModelUserInfoAggregateUserInfo) UnsetLastnameEn()`

UnsetLastnameEn ensures that no value is present for LastnameEn, not even an explicit nil
### GetCitizenId

`func (o *PiUserDomainAggregatesModelUserInfoAggregateUserInfo) GetCitizenId() string`

GetCitizenId returns the CitizenId field if non-nil, zero value otherwise.

### GetCitizenIdOk

`func (o *PiUserDomainAggregatesModelUserInfoAggregateUserInfo) GetCitizenIdOk() (*string, bool)`

GetCitizenIdOk returns a tuple with the CitizenId field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetCitizenId

`func (o *PiUserDomainAggregatesModelUserInfoAggregateUserInfo) SetCitizenId(v string)`

SetCitizenId sets CitizenId field to given value.

### HasCitizenId

`func (o *PiUserDomainAggregatesModelUserInfoAggregateUserInfo) HasCitizenId() bool`

HasCitizenId returns a boolean if a field has been set.

### SetCitizenIdNil

`func (o *PiUserDomainAggregatesModelUserInfoAggregateUserInfo) SetCitizenIdNil(b bool)`

 SetCitizenIdNil sets the value for CitizenId to be an explicit nil

### UnsetCitizenId
`func (o *PiUserDomainAggregatesModelUserInfoAggregateUserInfo) UnsetCitizenId()`

UnsetCitizenId ensures that no value is present for CitizenId, not even an explicit nil
### GetCitizenIdHash

`func (o *PiUserDomainAggregatesModelUserInfoAggregateUserInfo) GetCitizenIdHash() string`

GetCitizenIdHash returns the CitizenIdHash field if non-nil, zero value otherwise.

### GetCitizenIdHashOk

`func (o *PiUserDomainAggregatesModelUserInfoAggregateUserInfo) GetCitizenIdHashOk() (*string, bool)`

GetCitizenIdHashOk returns a tuple with the CitizenIdHash field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetCitizenIdHash

`func (o *PiUserDomainAggregatesModelUserInfoAggregateUserInfo) SetCitizenIdHash(v string)`

SetCitizenIdHash sets CitizenIdHash field to given value.

### HasCitizenIdHash

`func (o *PiUserDomainAggregatesModelUserInfoAggregateUserInfo) HasCitizenIdHash() bool`

HasCitizenIdHash returns a boolean if a field has been set.

### SetCitizenIdHashNil

`func (o *PiUserDomainAggregatesModelUserInfoAggregateUserInfo) SetCitizenIdHashNil(b bool)`

 SetCitizenIdHashNil sets the value for CitizenIdHash to be an explicit nil

### UnsetCitizenIdHash
`func (o *PiUserDomainAggregatesModelUserInfoAggregateUserInfo) UnsetCitizenIdHash()`

UnsetCitizenIdHash ensures that no value is present for CitizenIdHash, not even an explicit nil
### GetPhoneNumber

`func (o *PiUserDomainAggregatesModelUserInfoAggregateUserInfo) GetPhoneNumber() string`

GetPhoneNumber returns the PhoneNumber field if non-nil, zero value otherwise.

### GetPhoneNumberOk

`func (o *PiUserDomainAggregatesModelUserInfoAggregateUserInfo) GetPhoneNumberOk() (*string, bool)`

GetPhoneNumberOk returns a tuple with the PhoneNumber field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetPhoneNumber

`func (o *PiUserDomainAggregatesModelUserInfoAggregateUserInfo) SetPhoneNumber(v string)`

SetPhoneNumber sets PhoneNumber field to given value.

### HasPhoneNumber

`func (o *PiUserDomainAggregatesModelUserInfoAggregateUserInfo) HasPhoneNumber() bool`

HasPhoneNumber returns a boolean if a field has been set.

### SetPhoneNumberNil

`func (o *PiUserDomainAggregatesModelUserInfoAggregateUserInfo) SetPhoneNumberNil(b bool)`

 SetPhoneNumberNil sets the value for PhoneNumber to be an explicit nil

### UnsetPhoneNumber
`func (o *PiUserDomainAggregatesModelUserInfoAggregateUserInfo) UnsetPhoneNumber()`

UnsetPhoneNumber ensures that no value is present for PhoneNumber, not even an explicit nil
### GetPhoneNumberHash

`func (o *PiUserDomainAggregatesModelUserInfoAggregateUserInfo) GetPhoneNumberHash() string`

GetPhoneNumberHash returns the PhoneNumberHash field if non-nil, zero value otherwise.

### GetPhoneNumberHashOk

`func (o *PiUserDomainAggregatesModelUserInfoAggregateUserInfo) GetPhoneNumberHashOk() (*string, bool)`

GetPhoneNumberHashOk returns a tuple with the PhoneNumberHash field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetPhoneNumberHash

`func (o *PiUserDomainAggregatesModelUserInfoAggregateUserInfo) SetPhoneNumberHash(v string)`

SetPhoneNumberHash sets PhoneNumberHash field to given value.

### HasPhoneNumberHash

`func (o *PiUserDomainAggregatesModelUserInfoAggregateUserInfo) HasPhoneNumberHash() bool`

HasPhoneNumberHash returns a boolean if a field has been set.

### SetPhoneNumberHashNil

`func (o *PiUserDomainAggregatesModelUserInfoAggregateUserInfo) SetPhoneNumberHashNil(b bool)`

 SetPhoneNumberHashNil sets the value for PhoneNumberHash to be an explicit nil

### UnsetPhoneNumberHash
`func (o *PiUserDomainAggregatesModelUserInfoAggregateUserInfo) UnsetPhoneNumberHash()`

UnsetPhoneNumberHash ensures that no value is present for PhoneNumberHash, not even an explicit nil
### GetEmail

`func (o *PiUserDomainAggregatesModelUserInfoAggregateUserInfo) GetEmail() string`

GetEmail returns the Email field if non-nil, zero value otherwise.

### GetEmailOk

`func (o *PiUserDomainAggregatesModelUserInfoAggregateUserInfo) GetEmailOk() (*string, bool)`

GetEmailOk returns a tuple with the Email field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetEmail

`func (o *PiUserDomainAggregatesModelUserInfoAggregateUserInfo) SetEmail(v string)`

SetEmail sets Email field to given value.

### HasEmail

`func (o *PiUserDomainAggregatesModelUserInfoAggregateUserInfo) HasEmail() bool`

HasEmail returns a boolean if a field has been set.

### SetEmailNil

`func (o *PiUserDomainAggregatesModelUserInfoAggregateUserInfo) SetEmailNil(b bool)`

 SetEmailNil sets the value for Email to be an explicit nil

### UnsetEmail
`func (o *PiUserDomainAggregatesModelUserInfoAggregateUserInfo) UnsetEmail()`

UnsetEmail ensures that no value is present for Email, not even an explicit nil
### GetEmailHash

`func (o *PiUserDomainAggregatesModelUserInfoAggregateUserInfo) GetEmailHash() string`

GetEmailHash returns the EmailHash field if non-nil, zero value otherwise.

### GetEmailHashOk

`func (o *PiUserDomainAggregatesModelUserInfoAggregateUserInfo) GetEmailHashOk() (*string, bool)`

GetEmailHashOk returns a tuple with the EmailHash field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetEmailHash

`func (o *PiUserDomainAggregatesModelUserInfoAggregateUserInfo) SetEmailHash(v string)`

SetEmailHash sets EmailHash field to given value.

### HasEmailHash

`func (o *PiUserDomainAggregatesModelUserInfoAggregateUserInfo) HasEmailHash() bool`

HasEmailHash returns a boolean if a field has been set.

### SetEmailHashNil

`func (o *PiUserDomainAggregatesModelUserInfoAggregateUserInfo) SetEmailHashNil(b bool)`

 SetEmailHashNil sets the value for EmailHash to be an explicit nil

### UnsetEmailHash
`func (o *PiUserDomainAggregatesModelUserInfoAggregateUserInfo) UnsetEmailHash()`

UnsetEmailHash ensures that no value is present for EmailHash, not even an explicit nil
### GetGlobalAccount

`func (o *PiUserDomainAggregatesModelUserInfoAggregateUserInfo) GetGlobalAccount() string`

GetGlobalAccount returns the GlobalAccount field if non-nil, zero value otherwise.

### GetGlobalAccountOk

`func (o *PiUserDomainAggregatesModelUserInfoAggregateUserInfo) GetGlobalAccountOk() (*string, bool)`

GetGlobalAccountOk returns a tuple with the GlobalAccount field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetGlobalAccount

`func (o *PiUserDomainAggregatesModelUserInfoAggregateUserInfo) SetGlobalAccount(v string)`

SetGlobalAccount sets GlobalAccount field to given value.

### HasGlobalAccount

`func (o *PiUserDomainAggregatesModelUserInfoAggregateUserInfo) HasGlobalAccount() bool`

HasGlobalAccount returns a boolean if a field has been set.

### SetGlobalAccountNil

`func (o *PiUserDomainAggregatesModelUserInfoAggregateUserInfo) SetGlobalAccountNil(b bool)`

 SetGlobalAccountNil sets the value for GlobalAccount to be an explicit nil

### UnsetGlobalAccount
`func (o *PiUserDomainAggregatesModelUserInfoAggregateUserInfo) UnsetGlobalAccount()`

UnsetGlobalAccount ensures that no value is present for GlobalAccount, not even an explicit nil
### GetPlaceOfBirthCountry

`func (o *PiUserDomainAggregatesModelUserInfoAggregateUserInfo) GetPlaceOfBirthCountry() string`

GetPlaceOfBirthCountry returns the PlaceOfBirthCountry field if non-nil, zero value otherwise.

### GetPlaceOfBirthCountryOk

`func (o *PiUserDomainAggregatesModelUserInfoAggregateUserInfo) GetPlaceOfBirthCountryOk() (*string, bool)`

GetPlaceOfBirthCountryOk returns a tuple with the PlaceOfBirthCountry field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetPlaceOfBirthCountry

`func (o *PiUserDomainAggregatesModelUserInfoAggregateUserInfo) SetPlaceOfBirthCountry(v string)`

SetPlaceOfBirthCountry sets PlaceOfBirthCountry field to given value.

### HasPlaceOfBirthCountry

`func (o *PiUserDomainAggregatesModelUserInfoAggregateUserInfo) HasPlaceOfBirthCountry() bool`

HasPlaceOfBirthCountry returns a boolean if a field has been set.

### SetPlaceOfBirthCountryNil

`func (o *PiUserDomainAggregatesModelUserInfoAggregateUserInfo) SetPlaceOfBirthCountryNil(b bool)`

 SetPlaceOfBirthCountryNil sets the value for PlaceOfBirthCountry to be an explicit nil

### UnsetPlaceOfBirthCountry
`func (o *PiUserDomainAggregatesModelUserInfoAggregateUserInfo) UnsetPlaceOfBirthCountry()`

UnsetPlaceOfBirthCountry ensures that no value is present for PlaceOfBirthCountry, not even an explicit nil
### GetPlaceOfBirthCity

`func (o *PiUserDomainAggregatesModelUserInfoAggregateUserInfo) GetPlaceOfBirthCity() string`

GetPlaceOfBirthCity returns the PlaceOfBirthCity field if non-nil, zero value otherwise.

### GetPlaceOfBirthCityOk

`func (o *PiUserDomainAggregatesModelUserInfoAggregateUserInfo) GetPlaceOfBirthCityOk() (*string, bool)`

GetPlaceOfBirthCityOk returns a tuple with the PlaceOfBirthCity field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetPlaceOfBirthCity

`func (o *PiUserDomainAggregatesModelUserInfoAggregateUserInfo) SetPlaceOfBirthCity(v string)`

SetPlaceOfBirthCity sets PlaceOfBirthCity field to given value.

### HasPlaceOfBirthCity

`func (o *PiUserDomainAggregatesModelUserInfoAggregateUserInfo) HasPlaceOfBirthCity() bool`

HasPlaceOfBirthCity returns a boolean if a field has been set.

### SetPlaceOfBirthCityNil

`func (o *PiUserDomainAggregatesModelUserInfoAggregateUserInfo) SetPlaceOfBirthCityNil(b bool)`

 SetPlaceOfBirthCityNil sets the value for PlaceOfBirthCity to be an explicit nil

### UnsetPlaceOfBirthCity
`func (o *PiUserDomainAggregatesModelUserInfoAggregateUserInfo) UnsetPlaceOfBirthCity()`

UnsetPlaceOfBirthCity ensures that no value is present for PlaceOfBirthCity, not even an explicit nil
### GetDateOfBirth

`func (o *PiUserDomainAggregatesModelUserInfoAggregateUserInfo) GetDateOfBirth() string`

GetDateOfBirth returns the DateOfBirth field if non-nil, zero value otherwise.

### GetDateOfBirthOk

`func (o *PiUserDomainAggregatesModelUserInfoAggregateUserInfo) GetDateOfBirthOk() (*string, bool)`

GetDateOfBirthOk returns a tuple with the DateOfBirth field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetDateOfBirth

`func (o *PiUserDomainAggregatesModelUserInfoAggregateUserInfo) SetDateOfBirth(v string)`

SetDateOfBirth sets DateOfBirth field to given value.

### HasDateOfBirth

`func (o *PiUserDomainAggregatesModelUserInfoAggregateUserInfo) HasDateOfBirth() bool`

HasDateOfBirth returns a boolean if a field has been set.

### SetDateOfBirthNil

`func (o *PiUserDomainAggregatesModelUserInfoAggregateUserInfo) SetDateOfBirthNil(b bool)`

 SetDateOfBirthNil sets the value for DateOfBirth to be an explicit nil

### UnsetDateOfBirth
`func (o *PiUserDomainAggregatesModelUserInfoAggregateUserInfo) UnsetDateOfBirth()`

UnsetDateOfBirth ensures that no value is present for DateOfBirth, not even an explicit nil
### GetBankAccounts

`func (o *PiUserDomainAggregatesModelUserInfoAggregateUserInfo) GetBankAccounts() []PiUserDomainAggregatesModelBankAccountAggregateBankAccount`

GetBankAccounts returns the BankAccounts field if non-nil, zero value otherwise.

### GetBankAccountsOk

`func (o *PiUserDomainAggregatesModelUserInfoAggregateUserInfo) GetBankAccountsOk() (*[]PiUserDomainAggregatesModelBankAccountAggregateBankAccount, bool)`

GetBankAccountsOk returns a tuple with the BankAccounts field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetBankAccounts

`func (o *PiUserDomainAggregatesModelUserInfoAggregateUserInfo) SetBankAccounts(v []PiUserDomainAggregatesModelBankAccountAggregateBankAccount)`

SetBankAccounts sets BankAccounts field to given value.

### HasBankAccounts

`func (o *PiUserDomainAggregatesModelUserInfoAggregateUserInfo) HasBankAccounts() bool`

HasBankAccounts returns a boolean if a field has been set.

### SetBankAccountsNil

`func (o *PiUserDomainAggregatesModelUserInfoAggregateUserInfo) SetBankAccountsNil(b bool)`

 SetBankAccountsNil sets the value for BankAccounts to be an explicit nil

### UnsetBankAccounts
`func (o *PiUserDomainAggregatesModelUserInfoAggregateUserInfo) UnsetBankAccounts()`

UnsetBankAccounts ensures that no value is present for BankAccounts, not even an explicit nil
### GetDocuments

`func (o *PiUserDomainAggregatesModelUserInfoAggregateUserInfo) GetDocuments() []PiUserDomainAggregatesModelDocumentAggregateDocument`

GetDocuments returns the Documents field if non-nil, zero value otherwise.

### GetDocumentsOk

`func (o *PiUserDomainAggregatesModelUserInfoAggregateUserInfo) GetDocumentsOk() (*[]PiUserDomainAggregatesModelDocumentAggregateDocument, bool)`

GetDocumentsOk returns a tuple with the Documents field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetDocuments

`func (o *PiUserDomainAggregatesModelUserInfoAggregateUserInfo) SetDocuments(v []PiUserDomainAggregatesModelDocumentAggregateDocument)`

SetDocuments sets Documents field to given value.

### HasDocuments

`func (o *PiUserDomainAggregatesModelUserInfoAggregateUserInfo) HasDocuments() bool`

HasDocuments returns a boolean if a field has been set.

### SetDocumentsNil

`func (o *PiUserDomainAggregatesModelUserInfoAggregateUserInfo) SetDocumentsNil(b bool)`

 SetDocumentsNil sets the value for Documents to be an explicit nil

### UnsetDocuments
`func (o *PiUserDomainAggregatesModelUserInfoAggregateUserInfo) UnsetDocuments()`

UnsetDocuments ensures that no value is present for Documents, not even an explicit nil
### GetExaminations

`func (o *PiUserDomainAggregatesModelUserInfoAggregateUserInfo) GetExaminations() []PiUserDomainAggregatesModelExamAggregateExamination`

GetExaminations returns the Examinations field if non-nil, zero value otherwise.

### GetExaminationsOk

`func (o *PiUserDomainAggregatesModelUserInfoAggregateUserInfo) GetExaminationsOk() (*[]PiUserDomainAggregatesModelExamAggregateExamination, bool)`

GetExaminationsOk returns a tuple with the Examinations field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetExaminations

`func (o *PiUserDomainAggregatesModelUserInfoAggregateUserInfo) SetExaminations(v []PiUserDomainAggregatesModelExamAggregateExamination)`

SetExaminations sets Examinations field to given value.

### HasExaminations

`func (o *PiUserDomainAggregatesModelUserInfoAggregateUserInfo) HasExaminations() bool`

HasExaminations returns a boolean if a field has been set.

### SetExaminationsNil

`func (o *PiUserDomainAggregatesModelUserInfoAggregateUserInfo) SetExaminationsNil(b bool)`

 SetExaminationsNil sets the value for Examinations to be an explicit nil

### UnsetExaminations
`func (o *PiUserDomainAggregatesModelUserInfoAggregateUserInfo) UnsetExaminations()`

UnsetExaminations ensures that no value is present for Examinations, not even an explicit nil
### GetCreatedAt

`func (o *PiUserDomainAggregatesModelUserInfoAggregateUserInfo) GetCreatedAt() time.Time`

GetCreatedAt returns the CreatedAt field if non-nil, zero value otherwise.

### GetCreatedAtOk

`func (o *PiUserDomainAggregatesModelUserInfoAggregateUserInfo) GetCreatedAtOk() (*time.Time, bool)`

GetCreatedAtOk returns a tuple with the CreatedAt field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetCreatedAt

`func (o *PiUserDomainAggregatesModelUserInfoAggregateUserInfo) SetCreatedAt(v time.Time)`

SetCreatedAt sets CreatedAt field to given value.

### HasCreatedAt

`func (o *PiUserDomainAggregatesModelUserInfoAggregateUserInfo) HasCreatedAt() bool`

HasCreatedAt returns a boolean if a field has been set.

### GetUpdatedAt

`func (o *PiUserDomainAggregatesModelUserInfoAggregateUserInfo) GetUpdatedAt() time.Time`

GetUpdatedAt returns the UpdatedAt field if non-nil, zero value otherwise.

### GetUpdatedAtOk

`func (o *PiUserDomainAggregatesModelUserInfoAggregateUserInfo) GetUpdatedAtOk() (*time.Time, bool)`

GetUpdatedAtOk returns a tuple with the UpdatedAt field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetUpdatedAt

`func (o *PiUserDomainAggregatesModelUserInfoAggregateUserInfo) SetUpdatedAt(v time.Time)`

SetUpdatedAt sets UpdatedAt field to given value.

### HasUpdatedAt

`func (o *PiUserDomainAggregatesModelUserInfoAggregateUserInfo) HasUpdatedAt() bool`

HasUpdatedAt returns a boolean if a field has been set.

### SetUpdatedAtNil

`func (o *PiUserDomainAggregatesModelUserInfoAggregateUserInfo) SetUpdatedAtNil(b bool)`

 SetUpdatedAtNil sets the value for UpdatedAt to be an explicit nil

### UnsetUpdatedAt
`func (o *PiUserDomainAggregatesModelUserInfoAggregateUserInfo) UnsetUpdatedAt()`

UnsetUpdatedAt ensures that no value is present for UpdatedAt, not even an explicit nil
### GetUserAccounts

`func (o *PiUserDomainAggregatesModelUserInfoAggregateUserInfo) GetUserAccounts() []PiUserDomainAggregatesModelUserAccountAggregateUserAccount`

GetUserAccounts returns the UserAccounts field if non-nil, zero value otherwise.

### GetUserAccountsOk

`func (o *PiUserDomainAggregatesModelUserInfoAggregateUserInfo) GetUserAccountsOk() (*[]PiUserDomainAggregatesModelUserAccountAggregateUserAccount, bool)`

GetUserAccountsOk returns a tuple with the UserAccounts field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetUserAccounts

`func (o *PiUserDomainAggregatesModelUserInfoAggregateUserInfo) SetUserAccounts(v []PiUserDomainAggregatesModelUserAccountAggregateUserAccount)`

SetUserAccounts sets UserAccounts field to given value.

### HasUserAccounts

`func (o *PiUserDomainAggregatesModelUserInfoAggregateUserInfo) HasUserAccounts() bool`

HasUserAccounts returns a boolean if a field has been set.

### SetUserAccountsNil

`func (o *PiUserDomainAggregatesModelUserInfoAggregateUserInfo) SetUserAccountsNil(b bool)`

 SetUserAccountsNil sets the value for UserAccounts to be an explicit nil

### UnsetUserAccounts
`func (o *PiUserDomainAggregatesModelUserInfoAggregateUserInfo) UnsetUserAccounts()`

UnsetUserAccounts ensures that no value is present for UserAccounts, not even an explicit nil
### GetKyc

`func (o *PiUserDomainAggregatesModelUserInfoAggregateUserInfo) GetKyc() PiUserDomainAggregatesModelKycAggregateKyc`

GetKyc returns the Kyc field if non-nil, zero value otherwise.

### GetKycOk

`func (o *PiUserDomainAggregatesModelUserInfoAggregateUserInfo) GetKycOk() (*PiUserDomainAggregatesModelKycAggregateKyc, bool)`

GetKycOk returns a tuple with the Kyc field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetKyc

`func (o *PiUserDomainAggregatesModelUserInfoAggregateUserInfo) SetKyc(v PiUserDomainAggregatesModelKycAggregateKyc)`

SetKyc sets Kyc field to given value.

### HasKyc

`func (o *PiUserDomainAggregatesModelUserInfoAggregateUserInfo) HasKyc() bool`

HasKyc returns a boolean if a field has been set.

### GetAddress

`func (o *PiUserDomainAggregatesModelUserInfoAggregateUserInfo) GetAddress() PiUserDomainAggregatesModelAddressAggregateAddress`

GetAddress returns the Address field if non-nil, zero value otherwise.

### GetAddressOk

`func (o *PiUserDomainAggregatesModelUserInfoAggregateUserInfo) GetAddressOk() (*PiUserDomainAggregatesModelAddressAggregateAddress, bool)`

GetAddressOk returns a tuple with the Address field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetAddress

`func (o *PiUserDomainAggregatesModelUserInfoAggregateUserInfo) SetAddress(v PiUserDomainAggregatesModelAddressAggregateAddress)`

SetAddress sets Address field to given value.

### HasAddress

`func (o *PiUserDomainAggregatesModelUserInfoAggregateUserInfo) HasAddress() bool`

HasAddress returns a boolean if a field has been set.

### GetSuitabilityTests

`func (o *PiUserDomainAggregatesModelUserInfoAggregateUserInfo) GetSuitabilityTests() []PiUserDomainAggregatesModelSuitabilityTestAggregateSuitabilityTest`

GetSuitabilityTests returns the SuitabilityTests field if non-nil, zero value otherwise.

### GetSuitabilityTestsOk

`func (o *PiUserDomainAggregatesModelUserInfoAggregateUserInfo) GetSuitabilityTestsOk() (*[]PiUserDomainAggregatesModelSuitabilityTestAggregateSuitabilityTest, bool)`

GetSuitabilityTestsOk returns a tuple with the SuitabilityTests field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetSuitabilityTests

`func (o *PiUserDomainAggregatesModelUserInfoAggregateUserInfo) SetSuitabilityTests(v []PiUserDomainAggregatesModelSuitabilityTestAggregateSuitabilityTest)`

SetSuitabilityTests sets SuitabilityTests field to given value.

### HasSuitabilityTests

`func (o *PiUserDomainAggregatesModelUserInfoAggregateUserInfo) HasSuitabilityTests() bool`

HasSuitabilityTests returns a boolean if a field has been set.

### SetSuitabilityTestsNil

`func (o *PiUserDomainAggregatesModelUserInfoAggregateUserInfo) SetSuitabilityTestsNil(b bool)`

 SetSuitabilityTestsNil sets the value for SuitabilityTests to be an explicit nil

### UnsetSuitabilityTests
`func (o *PiUserDomainAggregatesModelUserInfoAggregateUserInfo) UnsetSuitabilityTests()`

UnsetSuitabilityTests ensures that no value is present for SuitabilityTests, not even an explicit nil
### GetBankAccountsV2

`func (o *PiUserDomainAggregatesModelUserInfoAggregateUserInfo) GetBankAccountsV2() []PiUserDomainAggregatesModelBankAccountAggregateBankAccountV2`

GetBankAccountsV2 returns the BankAccountsV2 field if non-nil, zero value otherwise.

### GetBankAccountsV2Ok

`func (o *PiUserDomainAggregatesModelUserInfoAggregateUserInfo) GetBankAccountsV2Ok() (*[]PiUserDomainAggregatesModelBankAccountAggregateBankAccountV2, bool)`

GetBankAccountsV2Ok returns a tuple with the BankAccountsV2 field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetBankAccountsV2

`func (o *PiUserDomainAggregatesModelUserInfoAggregateUserInfo) SetBankAccountsV2(v []PiUserDomainAggregatesModelBankAccountAggregateBankAccountV2)`

SetBankAccountsV2 sets BankAccountsV2 field to given value.

### HasBankAccountsV2

`func (o *PiUserDomainAggregatesModelUserInfoAggregateUserInfo) HasBankAccountsV2() bool`

HasBankAccountsV2 returns a boolean if a field has been set.

### SetBankAccountsV2Nil

`func (o *PiUserDomainAggregatesModelUserInfoAggregateUserInfo) SetBankAccountsV2Nil(b bool)`

 SetBankAccountsV2Nil sets the value for BankAccountsV2 to be an explicit nil

### UnsetBankAccountsV2
`func (o *PiUserDomainAggregatesModelUserInfoAggregateUserInfo) UnsetBankAccountsV2()`

UnsetBankAccountsV2 ensures that no value is present for BankAccountsV2, not even an explicit nil

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


