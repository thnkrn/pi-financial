# PiFundMarketDataAPIModelsResponsesPurchaseDetailResponse

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**MinInitialBuy** | Pointer to **NullableFloat32** |  | [optional] 
**MinAdditionalBuy** | Pointer to **NullableFloat32** |  | [optional] 
**MinSellUnit** | Pointer to **NullableFloat32** |  | [optional] 
**MinSellAmount** | Pointer to **NullableFloat32** |  | [optional] 
**MinHoldUnit** | Pointer to **NullableFloat32** |  | [optional] 
**MinHoldAmount** | Pointer to **NullableFloat32** |  | [optional] 
**SettlementPeriod** | Pointer to **NullableInt32** |  | [optional] 
**PiBuyCutOffTime** | Pointer to **NullableString** |  | [optional] 
**PiSellCutOffTime** | Pointer to **NullableString** |  | [optional] 
**AsOfDate** | Pointer to **time.Time** |  | [optional] 

## Methods

### NewPiFundMarketDataAPIModelsResponsesPurchaseDetailResponse

`func NewPiFundMarketDataAPIModelsResponsesPurchaseDetailResponse() *PiFundMarketDataAPIModelsResponsesPurchaseDetailResponse`

NewPiFundMarketDataAPIModelsResponsesPurchaseDetailResponse instantiates a new PiFundMarketDataAPIModelsResponsesPurchaseDetailResponse object
This constructor will assign default values to properties that have it defined,
and makes sure properties required by API are set, but the set of arguments
will change when the set of required properties is changed

### NewPiFundMarketDataAPIModelsResponsesPurchaseDetailResponseWithDefaults

`func NewPiFundMarketDataAPIModelsResponsesPurchaseDetailResponseWithDefaults() *PiFundMarketDataAPIModelsResponsesPurchaseDetailResponse`

NewPiFundMarketDataAPIModelsResponsesPurchaseDetailResponseWithDefaults instantiates a new PiFundMarketDataAPIModelsResponsesPurchaseDetailResponse object
This constructor will only assign default values to properties that have it defined,
but it doesn't guarantee that properties required by API are set

### GetMinInitialBuy

`func (o *PiFundMarketDataAPIModelsResponsesPurchaseDetailResponse) GetMinInitialBuy() float32`

GetMinInitialBuy returns the MinInitialBuy field if non-nil, zero value otherwise.

### GetMinInitialBuyOk

`func (o *PiFundMarketDataAPIModelsResponsesPurchaseDetailResponse) GetMinInitialBuyOk() (*float32, bool)`

GetMinInitialBuyOk returns a tuple with the MinInitialBuy field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetMinInitialBuy

`func (o *PiFundMarketDataAPIModelsResponsesPurchaseDetailResponse) SetMinInitialBuy(v float32)`

SetMinInitialBuy sets MinInitialBuy field to given value.

### HasMinInitialBuy

`func (o *PiFundMarketDataAPIModelsResponsesPurchaseDetailResponse) HasMinInitialBuy() bool`

HasMinInitialBuy returns a boolean if a field has been set.

### SetMinInitialBuyNil

`func (o *PiFundMarketDataAPIModelsResponsesPurchaseDetailResponse) SetMinInitialBuyNil(b bool)`

 SetMinInitialBuyNil sets the value for MinInitialBuy to be an explicit nil

### UnsetMinInitialBuy
`func (o *PiFundMarketDataAPIModelsResponsesPurchaseDetailResponse) UnsetMinInitialBuy()`

UnsetMinInitialBuy ensures that no value is present for MinInitialBuy, not even an explicit nil
### GetMinAdditionalBuy

`func (o *PiFundMarketDataAPIModelsResponsesPurchaseDetailResponse) GetMinAdditionalBuy() float32`

GetMinAdditionalBuy returns the MinAdditionalBuy field if non-nil, zero value otherwise.

### GetMinAdditionalBuyOk

`func (o *PiFundMarketDataAPIModelsResponsesPurchaseDetailResponse) GetMinAdditionalBuyOk() (*float32, bool)`

GetMinAdditionalBuyOk returns a tuple with the MinAdditionalBuy field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetMinAdditionalBuy

`func (o *PiFundMarketDataAPIModelsResponsesPurchaseDetailResponse) SetMinAdditionalBuy(v float32)`

SetMinAdditionalBuy sets MinAdditionalBuy field to given value.

### HasMinAdditionalBuy

`func (o *PiFundMarketDataAPIModelsResponsesPurchaseDetailResponse) HasMinAdditionalBuy() bool`

HasMinAdditionalBuy returns a boolean if a field has been set.

### SetMinAdditionalBuyNil

`func (o *PiFundMarketDataAPIModelsResponsesPurchaseDetailResponse) SetMinAdditionalBuyNil(b bool)`

 SetMinAdditionalBuyNil sets the value for MinAdditionalBuy to be an explicit nil

### UnsetMinAdditionalBuy
`func (o *PiFundMarketDataAPIModelsResponsesPurchaseDetailResponse) UnsetMinAdditionalBuy()`

UnsetMinAdditionalBuy ensures that no value is present for MinAdditionalBuy, not even an explicit nil
### GetMinSellUnit

`func (o *PiFundMarketDataAPIModelsResponsesPurchaseDetailResponse) GetMinSellUnit() float32`

GetMinSellUnit returns the MinSellUnit field if non-nil, zero value otherwise.

### GetMinSellUnitOk

`func (o *PiFundMarketDataAPIModelsResponsesPurchaseDetailResponse) GetMinSellUnitOk() (*float32, bool)`

GetMinSellUnitOk returns a tuple with the MinSellUnit field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetMinSellUnit

`func (o *PiFundMarketDataAPIModelsResponsesPurchaseDetailResponse) SetMinSellUnit(v float32)`

SetMinSellUnit sets MinSellUnit field to given value.

### HasMinSellUnit

`func (o *PiFundMarketDataAPIModelsResponsesPurchaseDetailResponse) HasMinSellUnit() bool`

HasMinSellUnit returns a boolean if a field has been set.

### SetMinSellUnitNil

`func (o *PiFundMarketDataAPIModelsResponsesPurchaseDetailResponse) SetMinSellUnitNil(b bool)`

 SetMinSellUnitNil sets the value for MinSellUnit to be an explicit nil

### UnsetMinSellUnit
`func (o *PiFundMarketDataAPIModelsResponsesPurchaseDetailResponse) UnsetMinSellUnit()`

UnsetMinSellUnit ensures that no value is present for MinSellUnit, not even an explicit nil
### GetMinSellAmount

`func (o *PiFundMarketDataAPIModelsResponsesPurchaseDetailResponse) GetMinSellAmount() float32`

GetMinSellAmount returns the MinSellAmount field if non-nil, zero value otherwise.

### GetMinSellAmountOk

`func (o *PiFundMarketDataAPIModelsResponsesPurchaseDetailResponse) GetMinSellAmountOk() (*float32, bool)`

GetMinSellAmountOk returns a tuple with the MinSellAmount field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetMinSellAmount

`func (o *PiFundMarketDataAPIModelsResponsesPurchaseDetailResponse) SetMinSellAmount(v float32)`

SetMinSellAmount sets MinSellAmount field to given value.

### HasMinSellAmount

`func (o *PiFundMarketDataAPIModelsResponsesPurchaseDetailResponse) HasMinSellAmount() bool`

HasMinSellAmount returns a boolean if a field has been set.

### SetMinSellAmountNil

`func (o *PiFundMarketDataAPIModelsResponsesPurchaseDetailResponse) SetMinSellAmountNil(b bool)`

 SetMinSellAmountNil sets the value for MinSellAmount to be an explicit nil

### UnsetMinSellAmount
`func (o *PiFundMarketDataAPIModelsResponsesPurchaseDetailResponse) UnsetMinSellAmount()`

UnsetMinSellAmount ensures that no value is present for MinSellAmount, not even an explicit nil
### GetMinHoldUnit

`func (o *PiFundMarketDataAPIModelsResponsesPurchaseDetailResponse) GetMinHoldUnit() float32`

GetMinHoldUnit returns the MinHoldUnit field if non-nil, zero value otherwise.

### GetMinHoldUnitOk

`func (o *PiFundMarketDataAPIModelsResponsesPurchaseDetailResponse) GetMinHoldUnitOk() (*float32, bool)`

GetMinHoldUnitOk returns a tuple with the MinHoldUnit field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetMinHoldUnit

`func (o *PiFundMarketDataAPIModelsResponsesPurchaseDetailResponse) SetMinHoldUnit(v float32)`

SetMinHoldUnit sets MinHoldUnit field to given value.

### HasMinHoldUnit

`func (o *PiFundMarketDataAPIModelsResponsesPurchaseDetailResponse) HasMinHoldUnit() bool`

HasMinHoldUnit returns a boolean if a field has been set.

### SetMinHoldUnitNil

`func (o *PiFundMarketDataAPIModelsResponsesPurchaseDetailResponse) SetMinHoldUnitNil(b bool)`

 SetMinHoldUnitNil sets the value for MinHoldUnit to be an explicit nil

### UnsetMinHoldUnit
`func (o *PiFundMarketDataAPIModelsResponsesPurchaseDetailResponse) UnsetMinHoldUnit()`

UnsetMinHoldUnit ensures that no value is present for MinHoldUnit, not even an explicit nil
### GetMinHoldAmount

`func (o *PiFundMarketDataAPIModelsResponsesPurchaseDetailResponse) GetMinHoldAmount() float32`

GetMinHoldAmount returns the MinHoldAmount field if non-nil, zero value otherwise.

### GetMinHoldAmountOk

`func (o *PiFundMarketDataAPIModelsResponsesPurchaseDetailResponse) GetMinHoldAmountOk() (*float32, bool)`

GetMinHoldAmountOk returns a tuple with the MinHoldAmount field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetMinHoldAmount

`func (o *PiFundMarketDataAPIModelsResponsesPurchaseDetailResponse) SetMinHoldAmount(v float32)`

SetMinHoldAmount sets MinHoldAmount field to given value.

### HasMinHoldAmount

`func (o *PiFundMarketDataAPIModelsResponsesPurchaseDetailResponse) HasMinHoldAmount() bool`

HasMinHoldAmount returns a boolean if a field has been set.

### SetMinHoldAmountNil

`func (o *PiFundMarketDataAPIModelsResponsesPurchaseDetailResponse) SetMinHoldAmountNil(b bool)`

 SetMinHoldAmountNil sets the value for MinHoldAmount to be an explicit nil

### UnsetMinHoldAmount
`func (o *PiFundMarketDataAPIModelsResponsesPurchaseDetailResponse) UnsetMinHoldAmount()`

UnsetMinHoldAmount ensures that no value is present for MinHoldAmount, not even an explicit nil
### GetSettlementPeriod

`func (o *PiFundMarketDataAPIModelsResponsesPurchaseDetailResponse) GetSettlementPeriod() int32`

GetSettlementPeriod returns the SettlementPeriod field if non-nil, zero value otherwise.

### GetSettlementPeriodOk

`func (o *PiFundMarketDataAPIModelsResponsesPurchaseDetailResponse) GetSettlementPeriodOk() (*int32, bool)`

GetSettlementPeriodOk returns a tuple with the SettlementPeriod field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetSettlementPeriod

`func (o *PiFundMarketDataAPIModelsResponsesPurchaseDetailResponse) SetSettlementPeriod(v int32)`

SetSettlementPeriod sets SettlementPeriod field to given value.

### HasSettlementPeriod

`func (o *PiFundMarketDataAPIModelsResponsesPurchaseDetailResponse) HasSettlementPeriod() bool`

HasSettlementPeriod returns a boolean if a field has been set.

### SetSettlementPeriodNil

`func (o *PiFundMarketDataAPIModelsResponsesPurchaseDetailResponse) SetSettlementPeriodNil(b bool)`

 SetSettlementPeriodNil sets the value for SettlementPeriod to be an explicit nil

### UnsetSettlementPeriod
`func (o *PiFundMarketDataAPIModelsResponsesPurchaseDetailResponse) UnsetSettlementPeriod()`

UnsetSettlementPeriod ensures that no value is present for SettlementPeriod, not even an explicit nil
### GetPiBuyCutOffTime

`func (o *PiFundMarketDataAPIModelsResponsesPurchaseDetailResponse) GetPiBuyCutOffTime() string`

GetPiBuyCutOffTime returns the PiBuyCutOffTime field if non-nil, zero value otherwise.

### GetPiBuyCutOffTimeOk

`func (o *PiFundMarketDataAPIModelsResponsesPurchaseDetailResponse) GetPiBuyCutOffTimeOk() (*string, bool)`

GetPiBuyCutOffTimeOk returns a tuple with the PiBuyCutOffTime field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetPiBuyCutOffTime

`func (o *PiFundMarketDataAPIModelsResponsesPurchaseDetailResponse) SetPiBuyCutOffTime(v string)`

SetPiBuyCutOffTime sets PiBuyCutOffTime field to given value.

### HasPiBuyCutOffTime

`func (o *PiFundMarketDataAPIModelsResponsesPurchaseDetailResponse) HasPiBuyCutOffTime() bool`

HasPiBuyCutOffTime returns a boolean if a field has been set.

### SetPiBuyCutOffTimeNil

`func (o *PiFundMarketDataAPIModelsResponsesPurchaseDetailResponse) SetPiBuyCutOffTimeNil(b bool)`

 SetPiBuyCutOffTimeNil sets the value for PiBuyCutOffTime to be an explicit nil

### UnsetPiBuyCutOffTime
`func (o *PiFundMarketDataAPIModelsResponsesPurchaseDetailResponse) UnsetPiBuyCutOffTime()`

UnsetPiBuyCutOffTime ensures that no value is present for PiBuyCutOffTime, not even an explicit nil
### GetPiSellCutOffTime

`func (o *PiFundMarketDataAPIModelsResponsesPurchaseDetailResponse) GetPiSellCutOffTime() string`

GetPiSellCutOffTime returns the PiSellCutOffTime field if non-nil, zero value otherwise.

### GetPiSellCutOffTimeOk

`func (o *PiFundMarketDataAPIModelsResponsesPurchaseDetailResponse) GetPiSellCutOffTimeOk() (*string, bool)`

GetPiSellCutOffTimeOk returns a tuple with the PiSellCutOffTime field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetPiSellCutOffTime

`func (o *PiFundMarketDataAPIModelsResponsesPurchaseDetailResponse) SetPiSellCutOffTime(v string)`

SetPiSellCutOffTime sets PiSellCutOffTime field to given value.

### HasPiSellCutOffTime

`func (o *PiFundMarketDataAPIModelsResponsesPurchaseDetailResponse) HasPiSellCutOffTime() bool`

HasPiSellCutOffTime returns a boolean if a field has been set.

### SetPiSellCutOffTimeNil

`func (o *PiFundMarketDataAPIModelsResponsesPurchaseDetailResponse) SetPiSellCutOffTimeNil(b bool)`

 SetPiSellCutOffTimeNil sets the value for PiSellCutOffTime to be an explicit nil

### UnsetPiSellCutOffTime
`func (o *PiFundMarketDataAPIModelsResponsesPurchaseDetailResponse) UnsetPiSellCutOffTime()`

UnsetPiSellCutOffTime ensures that no value is present for PiSellCutOffTime, not even an explicit nil
### GetAsOfDate

`func (o *PiFundMarketDataAPIModelsResponsesPurchaseDetailResponse) GetAsOfDate() time.Time`

GetAsOfDate returns the AsOfDate field if non-nil, zero value otherwise.

### GetAsOfDateOk

`func (o *PiFundMarketDataAPIModelsResponsesPurchaseDetailResponse) GetAsOfDateOk() (*time.Time, bool)`

GetAsOfDateOk returns a tuple with the AsOfDate field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetAsOfDate

`func (o *PiFundMarketDataAPIModelsResponsesPurchaseDetailResponse) SetAsOfDate(v time.Time)`

SetAsOfDate sets AsOfDate field to given value.

### HasAsOfDate

`func (o *PiFundMarketDataAPIModelsResponsesPurchaseDetailResponse) HasAsOfDate() bool`

HasAsOfDate returns a boolean if a field has been set.


[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


