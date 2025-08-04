# DtoUserInfo

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**CitizenId** | Pointer to **string** |  | [optional] 
**CustCodes** | Pointer to **[]string** |  | [optional] 
**DateOfBirth** | Pointer to **string** | 2024-12-27 | [optional] 
**Devices** | Pointer to [**[]DtoDevice**](DtoDevice.md) |  | [optional] 
**Email** | Pointer to **string** |  | [optional] 
**FirstnameEn** | Pointer to **string** |  | [optional] 
**FirstnameTh** | Pointer to **string** |  | [optional] 
**Id** | Pointer to **string** |  | [optional] 
**LastnameEn** | Pointer to **string** |  | [optional] 
**LastnameTh** | Pointer to **string** |  | [optional] 
**PhoneNumber** | Pointer to **string** |  | [optional] 
**TradingAccounts** | Pointer to **[]string** |  | [optional] 
**WealthType** | Pointer to **string** |  | [optional] 

## Methods

### NewDtoUserInfo

`func NewDtoUserInfo() *DtoUserInfo`

NewDtoUserInfo instantiates a new DtoUserInfo object
This constructor will assign default values to properties that have it defined,
and makes sure properties required by API are set, but the set of arguments
will change when the set of required properties is changed

### NewDtoUserInfoWithDefaults

`func NewDtoUserInfoWithDefaults() *DtoUserInfo`

NewDtoUserInfoWithDefaults instantiates a new DtoUserInfo object
This constructor will only assign default values to properties that have it defined,
but it doesn't guarantee that properties required by API are set

### GetCitizenId

`func (o *DtoUserInfo) GetCitizenId() string`

GetCitizenId returns the CitizenId field if non-nil, zero value otherwise.

### GetCitizenIdOk

`func (o *DtoUserInfo) GetCitizenIdOk() (*string, bool)`

GetCitizenIdOk returns a tuple with the CitizenId field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetCitizenId

`func (o *DtoUserInfo) SetCitizenId(v string)`

SetCitizenId sets CitizenId field to given value.

### HasCitizenId

`func (o *DtoUserInfo) HasCitizenId() bool`

HasCitizenId returns a boolean if a field has been set.

### GetCustCodes

`func (o *DtoUserInfo) GetCustCodes() []string`

GetCustCodes returns the CustCodes field if non-nil, zero value otherwise.

### GetCustCodesOk

`func (o *DtoUserInfo) GetCustCodesOk() (*[]string, bool)`

GetCustCodesOk returns a tuple with the CustCodes field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetCustCodes

`func (o *DtoUserInfo) SetCustCodes(v []string)`

SetCustCodes sets CustCodes field to given value.

### HasCustCodes

`func (o *DtoUserInfo) HasCustCodes() bool`

HasCustCodes returns a boolean if a field has been set.

### GetDateOfBirth

`func (o *DtoUserInfo) GetDateOfBirth() string`

GetDateOfBirth returns the DateOfBirth field if non-nil, zero value otherwise.

### GetDateOfBirthOk

`func (o *DtoUserInfo) GetDateOfBirthOk() (*string, bool)`

GetDateOfBirthOk returns a tuple with the DateOfBirth field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetDateOfBirth

`func (o *DtoUserInfo) SetDateOfBirth(v string)`

SetDateOfBirth sets DateOfBirth field to given value.

### HasDateOfBirth

`func (o *DtoUserInfo) HasDateOfBirth() bool`

HasDateOfBirth returns a boolean if a field has been set.

### GetDevices

`func (o *DtoUserInfo) GetDevices() []DtoDevice`

GetDevices returns the Devices field if non-nil, zero value otherwise.

### GetDevicesOk

`func (o *DtoUserInfo) GetDevicesOk() (*[]DtoDevice, bool)`

GetDevicesOk returns a tuple with the Devices field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetDevices

`func (o *DtoUserInfo) SetDevices(v []DtoDevice)`

SetDevices sets Devices field to given value.

### HasDevices

`func (o *DtoUserInfo) HasDevices() bool`

HasDevices returns a boolean if a field has been set.

### GetEmail

`func (o *DtoUserInfo) GetEmail() string`

GetEmail returns the Email field if non-nil, zero value otherwise.

### GetEmailOk

`func (o *DtoUserInfo) GetEmailOk() (*string, bool)`

GetEmailOk returns a tuple with the Email field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetEmail

`func (o *DtoUserInfo) SetEmail(v string)`

SetEmail sets Email field to given value.

### HasEmail

`func (o *DtoUserInfo) HasEmail() bool`

HasEmail returns a boolean if a field has been set.

### GetFirstnameEn

`func (o *DtoUserInfo) GetFirstnameEn() string`

GetFirstnameEn returns the FirstnameEn field if non-nil, zero value otherwise.

### GetFirstnameEnOk

`func (o *DtoUserInfo) GetFirstnameEnOk() (*string, bool)`

GetFirstnameEnOk returns a tuple with the FirstnameEn field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetFirstnameEn

`func (o *DtoUserInfo) SetFirstnameEn(v string)`

SetFirstnameEn sets FirstnameEn field to given value.

### HasFirstnameEn

`func (o *DtoUserInfo) HasFirstnameEn() bool`

HasFirstnameEn returns a boolean if a field has been set.

### GetFirstnameTh

`func (o *DtoUserInfo) GetFirstnameTh() string`

GetFirstnameTh returns the FirstnameTh field if non-nil, zero value otherwise.

### GetFirstnameThOk

`func (o *DtoUserInfo) GetFirstnameThOk() (*string, bool)`

GetFirstnameThOk returns a tuple with the FirstnameTh field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetFirstnameTh

`func (o *DtoUserInfo) SetFirstnameTh(v string)`

SetFirstnameTh sets FirstnameTh field to given value.

### HasFirstnameTh

`func (o *DtoUserInfo) HasFirstnameTh() bool`

HasFirstnameTh returns a boolean if a field has been set.

### GetId

`func (o *DtoUserInfo) GetId() string`

GetId returns the Id field if non-nil, zero value otherwise.

### GetIdOk

`func (o *DtoUserInfo) GetIdOk() (*string, bool)`

GetIdOk returns a tuple with the Id field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetId

`func (o *DtoUserInfo) SetId(v string)`

SetId sets Id field to given value.

### HasId

`func (o *DtoUserInfo) HasId() bool`

HasId returns a boolean if a field has been set.

### GetLastnameEn

`func (o *DtoUserInfo) GetLastnameEn() string`

GetLastnameEn returns the LastnameEn field if non-nil, zero value otherwise.

### GetLastnameEnOk

`func (o *DtoUserInfo) GetLastnameEnOk() (*string, bool)`

GetLastnameEnOk returns a tuple with the LastnameEn field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetLastnameEn

`func (o *DtoUserInfo) SetLastnameEn(v string)`

SetLastnameEn sets LastnameEn field to given value.

### HasLastnameEn

`func (o *DtoUserInfo) HasLastnameEn() bool`

HasLastnameEn returns a boolean if a field has been set.

### GetLastnameTh

`func (o *DtoUserInfo) GetLastnameTh() string`

GetLastnameTh returns the LastnameTh field if non-nil, zero value otherwise.

### GetLastnameThOk

`func (o *DtoUserInfo) GetLastnameThOk() (*string, bool)`

GetLastnameThOk returns a tuple with the LastnameTh field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetLastnameTh

`func (o *DtoUserInfo) SetLastnameTh(v string)`

SetLastnameTh sets LastnameTh field to given value.

### HasLastnameTh

`func (o *DtoUserInfo) HasLastnameTh() bool`

HasLastnameTh returns a boolean if a field has been set.

### GetPhoneNumber

`func (o *DtoUserInfo) GetPhoneNumber() string`

GetPhoneNumber returns the PhoneNumber field if non-nil, zero value otherwise.

### GetPhoneNumberOk

`func (o *DtoUserInfo) GetPhoneNumberOk() (*string, bool)`

GetPhoneNumberOk returns a tuple with the PhoneNumber field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetPhoneNumber

`func (o *DtoUserInfo) SetPhoneNumber(v string)`

SetPhoneNumber sets PhoneNumber field to given value.

### HasPhoneNumber

`func (o *DtoUserInfo) HasPhoneNumber() bool`

HasPhoneNumber returns a boolean if a field has been set.

### GetTradingAccounts

`func (o *DtoUserInfo) GetTradingAccounts() []string`

GetTradingAccounts returns the TradingAccounts field if non-nil, zero value otherwise.

### GetTradingAccountsOk

`func (o *DtoUserInfo) GetTradingAccountsOk() (*[]string, bool)`

GetTradingAccountsOk returns a tuple with the TradingAccounts field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetTradingAccounts

`func (o *DtoUserInfo) SetTradingAccounts(v []string)`

SetTradingAccounts sets TradingAccounts field to given value.

### HasTradingAccounts

`func (o *DtoUserInfo) HasTradingAccounts() bool`

HasTradingAccounts returns a boolean if a field has been set.

### GetWealthType

`func (o *DtoUserInfo) GetWealthType() string`

GetWealthType returns the WealthType field if non-nil, zero value otherwise.

### GetWealthTypeOk

`func (o *DtoUserInfo) GetWealthTypeOk() (*string, bool)`

GetWealthTypeOk returns a tuple with the WealthType field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetWealthType

`func (o *DtoUserInfo) SetWealthType(v string)`

SetWealthType sets WealthType field to given value.

### HasWealthType

`func (o *DtoUserInfo) HasWealthType() bool`

HasWealthType returns a boolean if a field has been set.


[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


