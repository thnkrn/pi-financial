# ExchangeRate

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**From** | Pointer to [**Currency**](Currency.md) |  | [optional] 
**To** | Pointer to [**Currency**](Currency.md) |  | [optional] 
**Rate** | Pointer to **float32** |  | [optional] 

## Methods

### NewExchangeRate

`func NewExchangeRate() *ExchangeRate`

NewExchangeRate instantiates a new ExchangeRate object
This constructor will assign default values to properties that have it defined,
and makes sure properties required by API are set, but the set of arguments
will change when the set of required properties is changed

### NewExchangeRateWithDefaults

`func NewExchangeRateWithDefaults() *ExchangeRate`

NewExchangeRateWithDefaults instantiates a new ExchangeRate object
This constructor will only assign default values to properties that have it defined,
but it doesn't guarantee that properties required by API are set

### GetFrom

`func (o *ExchangeRate) GetFrom() Currency`

GetFrom returns the From field if non-nil, zero value otherwise.

### GetFromOk

`func (o *ExchangeRate) GetFromOk() (*Currency, bool)`

GetFromOk returns a tuple with the From field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetFrom

`func (o *ExchangeRate) SetFrom(v Currency)`

SetFrom sets From field to given value.

### HasFrom

`func (o *ExchangeRate) HasFrom() bool`

HasFrom returns a boolean if a field has been set.

### GetTo

`func (o *ExchangeRate) GetTo() Currency`

GetTo returns the To field if non-nil, zero value otherwise.

### GetToOk

`func (o *ExchangeRate) GetToOk() (*Currency, bool)`

GetToOk returns a tuple with the To field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetTo

`func (o *ExchangeRate) SetTo(v Currency)`

SetTo sets To field to given value.

### HasTo

`func (o *ExchangeRate) HasTo() bool`

HasTo returns a boolean if a field has been set.

### GetRate

`func (o *ExchangeRate) GetRate() float32`

GetRate returns the Rate field if non-nil, zero value otherwise.

### GetRateOk

`func (o *ExchangeRate) GetRateOk() (*float32, bool)`

GetRateOk returns a tuple with the Rate field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetRate

`func (o *ExchangeRate) SetRate(v float32)`

SetRate sets Rate field to given value.

### HasRate

`func (o *ExchangeRate) HasRate() bool`

HasRate returns a boolean if a field has been set.


[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


