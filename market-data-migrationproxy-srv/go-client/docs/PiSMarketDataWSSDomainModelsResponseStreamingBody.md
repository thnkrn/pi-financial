# PiSMarketDataWSSDomainModelsResponseStreamingBody

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**Symbol** | Pointer to **NullableString** |  | [optional] 
**Venue** | Pointer to **NullableString** |  | [optional] 
**Price** | Pointer to **NullableString** |  | [optional] 
**AuctionPrice** | Pointer to **NullableString** |  | [optional] 
**AuctionVolume** | Pointer to **NullableString** |  | [optional] 
**IsProjected** | Pointer to **bool** |  | [optional] 
**LastPriceTime** | Pointer to **int64** |  | [optional] 
**Open** | Pointer to **NullableString** |  | [optional] 
**High24H** | Pointer to **NullableString** |  | [optional] 
**Low24H** | Pointer to **NullableString** |  | [optional] 
**PriceChanged** | Pointer to **NullableString** |  | [optional] 
**PriceChangedRate** | Pointer to **NullableString** |  | [optional] 
**Volume** | Pointer to **NullableString** |  | [optional] 
**Amount** | Pointer to **NullableString** |  | [optional] 
**TotalAmount** | Pointer to **NullableString** |  | [optional] 
**TotalAmountK** | Pointer to **NullableString** |  | [optional] 
**TotalVolume** | Pointer to **NullableString** |  | [optional] 
**TotalVolumeK** | Pointer to **NullableString** |  | [optional] 
**Open1** | Pointer to **NullableString** |  | [optional] 
**Open2** | Pointer to **NullableString** |  | [optional] 
**Ceiling** | Pointer to **NullableString** |  | [optional] 
**Floor** | Pointer to **NullableString** |  | [optional] 
**Average** | Pointer to **NullableString** |  | [optional] 
**AverageBuy** | Pointer to **NullableString** |  | [optional] 
**AverageSell** | Pointer to **NullableString** |  | [optional] 
**Aggressor** | Pointer to **NullableString** |  | [optional] 
**PreClose** | Pointer to **NullableString** |  | [optional] 
**Status** | Pointer to **NullableString** |  | [optional] 
**Yield** | Pointer to **NullableString** |  | [optional] 
**PublicTrades** | Pointer to **[][]interface{}** |  | [optional] 
**OrderBook** | Pointer to [**PiSMarketDataWSSDomainModelsResponseStreamingOrderBook**](PiSMarketDataWSSDomainModelsResponseStreamingOrderBook.md) |  | [optional] 
**SecurityType** | Pointer to **NullableString** |  | [optional] 
**InstrumentType** | Pointer to **NullableString** |  | [optional] 
**Market** | Pointer to **NullableString** |  | [optional] 
**LastTrade** | Pointer to **NullableString** |  | [optional] 
**ToLastTrade** | Pointer to **int32** |  | [optional] 
**Moneyness** | Pointer to **NullableString** |  | [optional] 
**MaturityDate** | Pointer to **NullableString** |  | [optional] 
**Multiplier** | Pointer to **NullableString** |  | [optional] 
**ExercisePrice** | Pointer to **NullableString** |  | [optional] 
**IntrinsicValue** | Pointer to **NullableString** |  | [optional] 
**PSettle** | Pointer to **NullableString** |  | [optional] 
**Poi** | Pointer to **NullableString** |  | [optional] 
**Underlying** | Pointer to **NullableString** |  | [optional] 
**Open0** | Pointer to **NullableString** |  | [optional] 
**Basis** | Pointer to **NullableString** |  | [optional] 
**Settle** | Pointer to **NullableString** |  | [optional] 

## Methods

### NewPiSMarketDataWSSDomainModelsResponseStreamingBody

`func NewPiSMarketDataWSSDomainModelsResponseStreamingBody() *PiSMarketDataWSSDomainModelsResponseStreamingBody`

NewPiSMarketDataWSSDomainModelsResponseStreamingBody instantiates a new PiSMarketDataWSSDomainModelsResponseStreamingBody object
This constructor will assign default values to properties that have it defined,
and makes sure properties required by API are set, but the set of arguments
will change when the set of required properties is changed

### NewPiSMarketDataWSSDomainModelsResponseStreamingBodyWithDefaults

`func NewPiSMarketDataWSSDomainModelsResponseStreamingBodyWithDefaults() *PiSMarketDataWSSDomainModelsResponseStreamingBody`

NewPiSMarketDataWSSDomainModelsResponseStreamingBodyWithDefaults instantiates a new PiSMarketDataWSSDomainModelsResponseStreamingBody object
This constructor will only assign default values to properties that have it defined,
but it doesn't guarantee that properties required by API are set

### GetSymbol

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) GetSymbol() string`

GetSymbol returns the Symbol field if non-nil, zero value otherwise.

### GetSymbolOk

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) GetSymbolOk() (*string, bool)`

GetSymbolOk returns a tuple with the Symbol field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetSymbol

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) SetSymbol(v string)`

SetSymbol sets Symbol field to given value.

### HasSymbol

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) HasSymbol() bool`

HasSymbol returns a boolean if a field has been set.

### SetSymbolNil

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) SetSymbolNil(b bool)`

 SetSymbolNil sets the value for Symbol to be an explicit nil

### UnsetSymbol
`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) UnsetSymbol()`

UnsetSymbol ensures that no value is present for Symbol, not even an explicit nil
### GetVenue

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) GetVenue() string`

GetVenue returns the Venue field if non-nil, zero value otherwise.

### GetVenueOk

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) GetVenueOk() (*string, bool)`

GetVenueOk returns a tuple with the Venue field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetVenue

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) SetVenue(v string)`

SetVenue sets Venue field to given value.

### HasVenue

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) HasVenue() bool`

HasVenue returns a boolean if a field has been set.

### SetVenueNil

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) SetVenueNil(b bool)`

 SetVenueNil sets the value for Venue to be an explicit nil

### UnsetVenue
`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) UnsetVenue()`

UnsetVenue ensures that no value is present for Venue, not even an explicit nil
### GetPrice

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) GetPrice() string`

GetPrice returns the Price field if non-nil, zero value otherwise.

### GetPriceOk

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) GetPriceOk() (*string, bool)`

GetPriceOk returns a tuple with the Price field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetPrice

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) SetPrice(v string)`

SetPrice sets Price field to given value.

### HasPrice

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) HasPrice() bool`

HasPrice returns a boolean if a field has been set.

### SetPriceNil

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) SetPriceNil(b bool)`

 SetPriceNil sets the value for Price to be an explicit nil

### UnsetPrice
`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) UnsetPrice()`

UnsetPrice ensures that no value is present for Price, not even an explicit nil
### GetAuctionPrice

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) GetAuctionPrice() string`

GetAuctionPrice returns the AuctionPrice field if non-nil, zero value otherwise.

### GetAuctionPriceOk

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) GetAuctionPriceOk() (*string, bool)`

GetAuctionPriceOk returns a tuple with the AuctionPrice field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetAuctionPrice

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) SetAuctionPrice(v string)`

SetAuctionPrice sets AuctionPrice field to given value.

### HasAuctionPrice

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) HasAuctionPrice() bool`

HasAuctionPrice returns a boolean if a field has been set.

### SetAuctionPriceNil

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) SetAuctionPriceNil(b bool)`

 SetAuctionPriceNil sets the value for AuctionPrice to be an explicit nil

### UnsetAuctionPrice
`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) UnsetAuctionPrice()`

UnsetAuctionPrice ensures that no value is present for AuctionPrice, not even an explicit nil
### GetAuctionVolume

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) GetAuctionVolume() string`

GetAuctionVolume returns the AuctionVolume field if non-nil, zero value otherwise.

### GetAuctionVolumeOk

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) GetAuctionVolumeOk() (*string, bool)`

GetAuctionVolumeOk returns a tuple with the AuctionVolume field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetAuctionVolume

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) SetAuctionVolume(v string)`

SetAuctionVolume sets AuctionVolume field to given value.

### HasAuctionVolume

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) HasAuctionVolume() bool`

HasAuctionVolume returns a boolean if a field has been set.

### SetAuctionVolumeNil

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) SetAuctionVolumeNil(b bool)`

 SetAuctionVolumeNil sets the value for AuctionVolume to be an explicit nil

### UnsetAuctionVolume
`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) UnsetAuctionVolume()`

UnsetAuctionVolume ensures that no value is present for AuctionVolume, not even an explicit nil
### GetIsProjected

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) GetIsProjected() bool`

GetIsProjected returns the IsProjected field if non-nil, zero value otherwise.

### GetIsProjectedOk

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) GetIsProjectedOk() (*bool, bool)`

GetIsProjectedOk returns a tuple with the IsProjected field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetIsProjected

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) SetIsProjected(v bool)`

SetIsProjected sets IsProjected field to given value.

### HasIsProjected

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) HasIsProjected() bool`

HasIsProjected returns a boolean if a field has been set.

### GetLastPriceTime

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) GetLastPriceTime() int64`

GetLastPriceTime returns the LastPriceTime field if non-nil, zero value otherwise.

### GetLastPriceTimeOk

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) GetLastPriceTimeOk() (*int64, bool)`

GetLastPriceTimeOk returns a tuple with the LastPriceTime field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetLastPriceTime

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) SetLastPriceTime(v int64)`

SetLastPriceTime sets LastPriceTime field to given value.

### HasLastPriceTime

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) HasLastPriceTime() bool`

HasLastPriceTime returns a boolean if a field has been set.

### GetOpen

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) GetOpen() string`

GetOpen returns the Open field if non-nil, zero value otherwise.

### GetOpenOk

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) GetOpenOk() (*string, bool)`

GetOpenOk returns a tuple with the Open field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetOpen

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) SetOpen(v string)`

SetOpen sets Open field to given value.

### HasOpen

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) HasOpen() bool`

HasOpen returns a boolean if a field has been set.

### SetOpenNil

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) SetOpenNil(b bool)`

 SetOpenNil sets the value for Open to be an explicit nil

### UnsetOpen
`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) UnsetOpen()`

UnsetOpen ensures that no value is present for Open, not even an explicit nil
### GetHigh24H

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) GetHigh24H() string`

GetHigh24H returns the High24H field if non-nil, zero value otherwise.

### GetHigh24HOk

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) GetHigh24HOk() (*string, bool)`

GetHigh24HOk returns a tuple with the High24H field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetHigh24H

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) SetHigh24H(v string)`

SetHigh24H sets High24H field to given value.

### HasHigh24H

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) HasHigh24H() bool`

HasHigh24H returns a boolean if a field has been set.

### SetHigh24HNil

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) SetHigh24HNil(b bool)`

 SetHigh24HNil sets the value for High24H to be an explicit nil

### UnsetHigh24H
`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) UnsetHigh24H()`

UnsetHigh24H ensures that no value is present for High24H, not even an explicit nil
### GetLow24H

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) GetLow24H() string`

GetLow24H returns the Low24H field if non-nil, zero value otherwise.

### GetLow24HOk

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) GetLow24HOk() (*string, bool)`

GetLow24HOk returns a tuple with the Low24H field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetLow24H

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) SetLow24H(v string)`

SetLow24H sets Low24H field to given value.

### HasLow24H

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) HasLow24H() bool`

HasLow24H returns a boolean if a field has been set.

### SetLow24HNil

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) SetLow24HNil(b bool)`

 SetLow24HNil sets the value for Low24H to be an explicit nil

### UnsetLow24H
`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) UnsetLow24H()`

UnsetLow24H ensures that no value is present for Low24H, not even an explicit nil
### GetPriceChanged

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) GetPriceChanged() string`

GetPriceChanged returns the PriceChanged field if non-nil, zero value otherwise.

### GetPriceChangedOk

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) GetPriceChangedOk() (*string, bool)`

GetPriceChangedOk returns a tuple with the PriceChanged field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetPriceChanged

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) SetPriceChanged(v string)`

SetPriceChanged sets PriceChanged field to given value.

### HasPriceChanged

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) HasPriceChanged() bool`

HasPriceChanged returns a boolean if a field has been set.

### SetPriceChangedNil

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) SetPriceChangedNil(b bool)`

 SetPriceChangedNil sets the value for PriceChanged to be an explicit nil

### UnsetPriceChanged
`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) UnsetPriceChanged()`

UnsetPriceChanged ensures that no value is present for PriceChanged, not even an explicit nil
### GetPriceChangedRate

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) GetPriceChangedRate() string`

GetPriceChangedRate returns the PriceChangedRate field if non-nil, zero value otherwise.

### GetPriceChangedRateOk

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) GetPriceChangedRateOk() (*string, bool)`

GetPriceChangedRateOk returns a tuple with the PriceChangedRate field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetPriceChangedRate

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) SetPriceChangedRate(v string)`

SetPriceChangedRate sets PriceChangedRate field to given value.

### HasPriceChangedRate

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) HasPriceChangedRate() bool`

HasPriceChangedRate returns a boolean if a field has been set.

### SetPriceChangedRateNil

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) SetPriceChangedRateNil(b bool)`

 SetPriceChangedRateNil sets the value for PriceChangedRate to be an explicit nil

### UnsetPriceChangedRate
`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) UnsetPriceChangedRate()`

UnsetPriceChangedRate ensures that no value is present for PriceChangedRate, not even an explicit nil
### GetVolume

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) GetVolume() string`

GetVolume returns the Volume field if non-nil, zero value otherwise.

### GetVolumeOk

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) GetVolumeOk() (*string, bool)`

GetVolumeOk returns a tuple with the Volume field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetVolume

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) SetVolume(v string)`

SetVolume sets Volume field to given value.

### HasVolume

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) HasVolume() bool`

HasVolume returns a boolean if a field has been set.

### SetVolumeNil

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) SetVolumeNil(b bool)`

 SetVolumeNil sets the value for Volume to be an explicit nil

### UnsetVolume
`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) UnsetVolume()`

UnsetVolume ensures that no value is present for Volume, not even an explicit nil
### GetAmount

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) GetAmount() string`

GetAmount returns the Amount field if non-nil, zero value otherwise.

### GetAmountOk

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) GetAmountOk() (*string, bool)`

GetAmountOk returns a tuple with the Amount field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetAmount

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) SetAmount(v string)`

SetAmount sets Amount field to given value.

### HasAmount

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) HasAmount() bool`

HasAmount returns a boolean if a field has been set.

### SetAmountNil

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) SetAmountNil(b bool)`

 SetAmountNil sets the value for Amount to be an explicit nil

### UnsetAmount
`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) UnsetAmount()`

UnsetAmount ensures that no value is present for Amount, not even an explicit nil
### GetTotalAmount

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) GetTotalAmount() string`

GetTotalAmount returns the TotalAmount field if non-nil, zero value otherwise.

### GetTotalAmountOk

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) GetTotalAmountOk() (*string, bool)`

GetTotalAmountOk returns a tuple with the TotalAmount field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetTotalAmount

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) SetTotalAmount(v string)`

SetTotalAmount sets TotalAmount field to given value.

### HasTotalAmount

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) HasTotalAmount() bool`

HasTotalAmount returns a boolean if a field has been set.

### SetTotalAmountNil

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) SetTotalAmountNil(b bool)`

 SetTotalAmountNil sets the value for TotalAmount to be an explicit nil

### UnsetTotalAmount
`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) UnsetTotalAmount()`

UnsetTotalAmount ensures that no value is present for TotalAmount, not even an explicit nil
### GetTotalAmountK

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) GetTotalAmountK() string`

GetTotalAmountK returns the TotalAmountK field if non-nil, zero value otherwise.

### GetTotalAmountKOk

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) GetTotalAmountKOk() (*string, bool)`

GetTotalAmountKOk returns a tuple with the TotalAmountK field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetTotalAmountK

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) SetTotalAmountK(v string)`

SetTotalAmountK sets TotalAmountK field to given value.

### HasTotalAmountK

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) HasTotalAmountK() bool`

HasTotalAmountK returns a boolean if a field has been set.

### SetTotalAmountKNil

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) SetTotalAmountKNil(b bool)`

 SetTotalAmountKNil sets the value for TotalAmountK to be an explicit nil

### UnsetTotalAmountK
`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) UnsetTotalAmountK()`

UnsetTotalAmountK ensures that no value is present for TotalAmountK, not even an explicit nil
### GetTotalVolume

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) GetTotalVolume() string`

GetTotalVolume returns the TotalVolume field if non-nil, zero value otherwise.

### GetTotalVolumeOk

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) GetTotalVolumeOk() (*string, bool)`

GetTotalVolumeOk returns a tuple with the TotalVolume field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetTotalVolume

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) SetTotalVolume(v string)`

SetTotalVolume sets TotalVolume field to given value.

### HasTotalVolume

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) HasTotalVolume() bool`

HasTotalVolume returns a boolean if a field has been set.

### SetTotalVolumeNil

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) SetTotalVolumeNil(b bool)`

 SetTotalVolumeNil sets the value for TotalVolume to be an explicit nil

### UnsetTotalVolume
`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) UnsetTotalVolume()`

UnsetTotalVolume ensures that no value is present for TotalVolume, not even an explicit nil
### GetTotalVolumeK

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) GetTotalVolumeK() string`

GetTotalVolumeK returns the TotalVolumeK field if non-nil, zero value otherwise.

### GetTotalVolumeKOk

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) GetTotalVolumeKOk() (*string, bool)`

GetTotalVolumeKOk returns a tuple with the TotalVolumeK field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetTotalVolumeK

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) SetTotalVolumeK(v string)`

SetTotalVolumeK sets TotalVolumeK field to given value.

### HasTotalVolumeK

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) HasTotalVolumeK() bool`

HasTotalVolumeK returns a boolean if a field has been set.

### SetTotalVolumeKNil

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) SetTotalVolumeKNil(b bool)`

 SetTotalVolumeKNil sets the value for TotalVolumeK to be an explicit nil

### UnsetTotalVolumeK
`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) UnsetTotalVolumeK()`

UnsetTotalVolumeK ensures that no value is present for TotalVolumeK, not even an explicit nil
### GetOpen1

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) GetOpen1() string`

GetOpen1 returns the Open1 field if non-nil, zero value otherwise.

### GetOpen1Ok

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) GetOpen1Ok() (*string, bool)`

GetOpen1Ok returns a tuple with the Open1 field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetOpen1

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) SetOpen1(v string)`

SetOpen1 sets Open1 field to given value.

### HasOpen1

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) HasOpen1() bool`

HasOpen1 returns a boolean if a field has been set.

### SetOpen1Nil

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) SetOpen1Nil(b bool)`

 SetOpen1Nil sets the value for Open1 to be an explicit nil

### UnsetOpen1
`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) UnsetOpen1()`

UnsetOpen1 ensures that no value is present for Open1, not even an explicit nil
### GetOpen2

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) GetOpen2() string`

GetOpen2 returns the Open2 field if non-nil, zero value otherwise.

### GetOpen2Ok

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) GetOpen2Ok() (*string, bool)`

GetOpen2Ok returns a tuple with the Open2 field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetOpen2

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) SetOpen2(v string)`

SetOpen2 sets Open2 field to given value.

### HasOpen2

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) HasOpen2() bool`

HasOpen2 returns a boolean if a field has been set.

### SetOpen2Nil

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) SetOpen2Nil(b bool)`

 SetOpen2Nil sets the value for Open2 to be an explicit nil

### UnsetOpen2
`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) UnsetOpen2()`

UnsetOpen2 ensures that no value is present for Open2, not even an explicit nil
### GetCeiling

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) GetCeiling() string`

GetCeiling returns the Ceiling field if non-nil, zero value otherwise.

### GetCeilingOk

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) GetCeilingOk() (*string, bool)`

GetCeilingOk returns a tuple with the Ceiling field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetCeiling

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) SetCeiling(v string)`

SetCeiling sets Ceiling field to given value.

### HasCeiling

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) HasCeiling() bool`

HasCeiling returns a boolean if a field has been set.

### SetCeilingNil

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) SetCeilingNil(b bool)`

 SetCeilingNil sets the value for Ceiling to be an explicit nil

### UnsetCeiling
`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) UnsetCeiling()`

UnsetCeiling ensures that no value is present for Ceiling, not even an explicit nil
### GetFloor

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) GetFloor() string`

GetFloor returns the Floor field if non-nil, zero value otherwise.

### GetFloorOk

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) GetFloorOk() (*string, bool)`

GetFloorOk returns a tuple with the Floor field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetFloor

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) SetFloor(v string)`

SetFloor sets Floor field to given value.

### HasFloor

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) HasFloor() bool`

HasFloor returns a boolean if a field has been set.

### SetFloorNil

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) SetFloorNil(b bool)`

 SetFloorNil sets the value for Floor to be an explicit nil

### UnsetFloor
`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) UnsetFloor()`

UnsetFloor ensures that no value is present for Floor, not even an explicit nil
### GetAverage

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) GetAverage() string`

GetAverage returns the Average field if non-nil, zero value otherwise.

### GetAverageOk

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) GetAverageOk() (*string, bool)`

GetAverageOk returns a tuple with the Average field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetAverage

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) SetAverage(v string)`

SetAverage sets Average field to given value.

### HasAverage

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) HasAverage() bool`

HasAverage returns a boolean if a field has been set.

### SetAverageNil

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) SetAverageNil(b bool)`

 SetAverageNil sets the value for Average to be an explicit nil

### UnsetAverage
`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) UnsetAverage()`

UnsetAverage ensures that no value is present for Average, not even an explicit nil
### GetAverageBuy

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) GetAverageBuy() string`

GetAverageBuy returns the AverageBuy field if non-nil, zero value otherwise.

### GetAverageBuyOk

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) GetAverageBuyOk() (*string, bool)`

GetAverageBuyOk returns a tuple with the AverageBuy field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetAverageBuy

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) SetAverageBuy(v string)`

SetAverageBuy sets AverageBuy field to given value.

### HasAverageBuy

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) HasAverageBuy() bool`

HasAverageBuy returns a boolean if a field has been set.

### SetAverageBuyNil

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) SetAverageBuyNil(b bool)`

 SetAverageBuyNil sets the value for AverageBuy to be an explicit nil

### UnsetAverageBuy
`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) UnsetAverageBuy()`

UnsetAverageBuy ensures that no value is present for AverageBuy, not even an explicit nil
### GetAverageSell

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) GetAverageSell() string`

GetAverageSell returns the AverageSell field if non-nil, zero value otherwise.

### GetAverageSellOk

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) GetAverageSellOk() (*string, bool)`

GetAverageSellOk returns a tuple with the AverageSell field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetAverageSell

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) SetAverageSell(v string)`

SetAverageSell sets AverageSell field to given value.

### HasAverageSell

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) HasAverageSell() bool`

HasAverageSell returns a boolean if a field has been set.

### SetAverageSellNil

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) SetAverageSellNil(b bool)`

 SetAverageSellNil sets the value for AverageSell to be an explicit nil

### UnsetAverageSell
`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) UnsetAverageSell()`

UnsetAverageSell ensures that no value is present for AverageSell, not even an explicit nil
### GetAggressor

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) GetAggressor() string`

GetAggressor returns the Aggressor field if non-nil, zero value otherwise.

### GetAggressorOk

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) GetAggressorOk() (*string, bool)`

GetAggressorOk returns a tuple with the Aggressor field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetAggressor

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) SetAggressor(v string)`

SetAggressor sets Aggressor field to given value.

### HasAggressor

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) HasAggressor() bool`

HasAggressor returns a boolean if a field has been set.

### SetAggressorNil

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) SetAggressorNil(b bool)`

 SetAggressorNil sets the value for Aggressor to be an explicit nil

### UnsetAggressor
`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) UnsetAggressor()`

UnsetAggressor ensures that no value is present for Aggressor, not even an explicit nil
### GetPreClose

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) GetPreClose() string`

GetPreClose returns the PreClose field if non-nil, zero value otherwise.

### GetPreCloseOk

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) GetPreCloseOk() (*string, bool)`

GetPreCloseOk returns a tuple with the PreClose field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetPreClose

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) SetPreClose(v string)`

SetPreClose sets PreClose field to given value.

### HasPreClose

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) HasPreClose() bool`

HasPreClose returns a boolean if a field has been set.

### SetPreCloseNil

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) SetPreCloseNil(b bool)`

 SetPreCloseNil sets the value for PreClose to be an explicit nil

### UnsetPreClose
`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) UnsetPreClose()`

UnsetPreClose ensures that no value is present for PreClose, not even an explicit nil
### GetStatus

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) GetStatus() string`

GetStatus returns the Status field if non-nil, zero value otherwise.

### GetStatusOk

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) GetStatusOk() (*string, bool)`

GetStatusOk returns a tuple with the Status field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetStatus

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) SetStatus(v string)`

SetStatus sets Status field to given value.

### HasStatus

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) HasStatus() bool`

HasStatus returns a boolean if a field has been set.

### SetStatusNil

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) SetStatusNil(b bool)`

 SetStatusNil sets the value for Status to be an explicit nil

### UnsetStatus
`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) UnsetStatus()`

UnsetStatus ensures that no value is present for Status, not even an explicit nil
### GetYield

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) GetYield() string`

GetYield returns the Yield field if non-nil, zero value otherwise.

### GetYieldOk

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) GetYieldOk() (*string, bool)`

GetYieldOk returns a tuple with the Yield field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetYield

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) SetYield(v string)`

SetYield sets Yield field to given value.

### HasYield

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) HasYield() bool`

HasYield returns a boolean if a field has been set.

### SetYieldNil

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) SetYieldNil(b bool)`

 SetYieldNil sets the value for Yield to be an explicit nil

### UnsetYield
`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) UnsetYield()`

UnsetYield ensures that no value is present for Yield, not even an explicit nil
### GetPublicTrades

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) GetPublicTrades() [][]interface{}`

GetPublicTrades returns the PublicTrades field if non-nil, zero value otherwise.

### GetPublicTradesOk

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) GetPublicTradesOk() (*[][]interface{}, bool)`

GetPublicTradesOk returns a tuple with the PublicTrades field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetPublicTrades

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) SetPublicTrades(v [][]interface{})`

SetPublicTrades sets PublicTrades field to given value.

### HasPublicTrades

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) HasPublicTrades() bool`

HasPublicTrades returns a boolean if a field has been set.

### SetPublicTradesNil

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) SetPublicTradesNil(b bool)`

 SetPublicTradesNil sets the value for PublicTrades to be an explicit nil

### UnsetPublicTrades
`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) UnsetPublicTrades()`

UnsetPublicTrades ensures that no value is present for PublicTrades, not even an explicit nil
### GetOrderBook

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) GetOrderBook() PiSMarketDataWSSDomainModelsResponseStreamingOrderBook`

GetOrderBook returns the OrderBook field if non-nil, zero value otherwise.

### GetOrderBookOk

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) GetOrderBookOk() (*PiSMarketDataWSSDomainModelsResponseStreamingOrderBook, bool)`

GetOrderBookOk returns a tuple with the OrderBook field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetOrderBook

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) SetOrderBook(v PiSMarketDataWSSDomainModelsResponseStreamingOrderBook)`

SetOrderBook sets OrderBook field to given value.

### HasOrderBook

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) HasOrderBook() bool`

HasOrderBook returns a boolean if a field has been set.

### GetSecurityType

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) GetSecurityType() string`

GetSecurityType returns the SecurityType field if non-nil, zero value otherwise.

### GetSecurityTypeOk

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) GetSecurityTypeOk() (*string, bool)`

GetSecurityTypeOk returns a tuple with the SecurityType field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetSecurityType

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) SetSecurityType(v string)`

SetSecurityType sets SecurityType field to given value.

### HasSecurityType

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) HasSecurityType() bool`

HasSecurityType returns a boolean if a field has been set.

### SetSecurityTypeNil

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) SetSecurityTypeNil(b bool)`

 SetSecurityTypeNil sets the value for SecurityType to be an explicit nil

### UnsetSecurityType
`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) UnsetSecurityType()`

UnsetSecurityType ensures that no value is present for SecurityType, not even an explicit nil
### GetInstrumentType

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) GetInstrumentType() string`

GetInstrumentType returns the InstrumentType field if non-nil, zero value otherwise.

### GetInstrumentTypeOk

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) GetInstrumentTypeOk() (*string, bool)`

GetInstrumentTypeOk returns a tuple with the InstrumentType field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetInstrumentType

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) SetInstrumentType(v string)`

SetInstrumentType sets InstrumentType field to given value.

### HasInstrumentType

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) HasInstrumentType() bool`

HasInstrumentType returns a boolean if a field has been set.

### SetInstrumentTypeNil

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) SetInstrumentTypeNil(b bool)`

 SetInstrumentTypeNil sets the value for InstrumentType to be an explicit nil

### UnsetInstrumentType
`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) UnsetInstrumentType()`

UnsetInstrumentType ensures that no value is present for InstrumentType, not even an explicit nil
### GetMarket

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) GetMarket() string`

GetMarket returns the Market field if non-nil, zero value otherwise.

### GetMarketOk

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) GetMarketOk() (*string, bool)`

GetMarketOk returns a tuple with the Market field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetMarket

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) SetMarket(v string)`

SetMarket sets Market field to given value.

### HasMarket

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) HasMarket() bool`

HasMarket returns a boolean if a field has been set.

### SetMarketNil

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) SetMarketNil(b bool)`

 SetMarketNil sets the value for Market to be an explicit nil

### UnsetMarket
`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) UnsetMarket()`

UnsetMarket ensures that no value is present for Market, not even an explicit nil
### GetLastTrade

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) GetLastTrade() string`

GetLastTrade returns the LastTrade field if non-nil, zero value otherwise.

### GetLastTradeOk

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) GetLastTradeOk() (*string, bool)`

GetLastTradeOk returns a tuple with the LastTrade field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetLastTrade

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) SetLastTrade(v string)`

SetLastTrade sets LastTrade field to given value.

### HasLastTrade

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) HasLastTrade() bool`

HasLastTrade returns a boolean if a field has been set.

### SetLastTradeNil

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) SetLastTradeNil(b bool)`

 SetLastTradeNil sets the value for LastTrade to be an explicit nil

### UnsetLastTrade
`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) UnsetLastTrade()`

UnsetLastTrade ensures that no value is present for LastTrade, not even an explicit nil
### GetToLastTrade

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) GetToLastTrade() int32`

GetToLastTrade returns the ToLastTrade field if non-nil, zero value otherwise.

### GetToLastTradeOk

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) GetToLastTradeOk() (*int32, bool)`

GetToLastTradeOk returns a tuple with the ToLastTrade field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetToLastTrade

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) SetToLastTrade(v int32)`

SetToLastTrade sets ToLastTrade field to given value.

### HasToLastTrade

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) HasToLastTrade() bool`

HasToLastTrade returns a boolean if a field has been set.

### GetMoneyness

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) GetMoneyness() string`

GetMoneyness returns the Moneyness field if non-nil, zero value otherwise.

### GetMoneynessOk

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) GetMoneynessOk() (*string, bool)`

GetMoneynessOk returns a tuple with the Moneyness field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetMoneyness

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) SetMoneyness(v string)`

SetMoneyness sets Moneyness field to given value.

### HasMoneyness

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) HasMoneyness() bool`

HasMoneyness returns a boolean if a field has been set.

### SetMoneynessNil

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) SetMoneynessNil(b bool)`

 SetMoneynessNil sets the value for Moneyness to be an explicit nil

### UnsetMoneyness
`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) UnsetMoneyness()`

UnsetMoneyness ensures that no value is present for Moneyness, not even an explicit nil
### GetMaturityDate

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) GetMaturityDate() string`

GetMaturityDate returns the MaturityDate field if non-nil, zero value otherwise.

### GetMaturityDateOk

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) GetMaturityDateOk() (*string, bool)`

GetMaturityDateOk returns a tuple with the MaturityDate field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetMaturityDate

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) SetMaturityDate(v string)`

SetMaturityDate sets MaturityDate field to given value.

### HasMaturityDate

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) HasMaturityDate() bool`

HasMaturityDate returns a boolean if a field has been set.

### SetMaturityDateNil

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) SetMaturityDateNil(b bool)`

 SetMaturityDateNil sets the value for MaturityDate to be an explicit nil

### UnsetMaturityDate
`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) UnsetMaturityDate()`

UnsetMaturityDate ensures that no value is present for MaturityDate, not even an explicit nil
### GetMultiplier

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) GetMultiplier() string`

GetMultiplier returns the Multiplier field if non-nil, zero value otherwise.

### GetMultiplierOk

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) GetMultiplierOk() (*string, bool)`

GetMultiplierOk returns a tuple with the Multiplier field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetMultiplier

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) SetMultiplier(v string)`

SetMultiplier sets Multiplier field to given value.

### HasMultiplier

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) HasMultiplier() bool`

HasMultiplier returns a boolean if a field has been set.

### SetMultiplierNil

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) SetMultiplierNil(b bool)`

 SetMultiplierNil sets the value for Multiplier to be an explicit nil

### UnsetMultiplier
`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) UnsetMultiplier()`

UnsetMultiplier ensures that no value is present for Multiplier, not even an explicit nil
### GetExercisePrice

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) GetExercisePrice() string`

GetExercisePrice returns the ExercisePrice field if non-nil, zero value otherwise.

### GetExercisePriceOk

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) GetExercisePriceOk() (*string, bool)`

GetExercisePriceOk returns a tuple with the ExercisePrice field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetExercisePrice

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) SetExercisePrice(v string)`

SetExercisePrice sets ExercisePrice field to given value.

### HasExercisePrice

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) HasExercisePrice() bool`

HasExercisePrice returns a boolean if a field has been set.

### SetExercisePriceNil

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) SetExercisePriceNil(b bool)`

 SetExercisePriceNil sets the value for ExercisePrice to be an explicit nil

### UnsetExercisePrice
`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) UnsetExercisePrice()`

UnsetExercisePrice ensures that no value is present for ExercisePrice, not even an explicit nil
### GetIntrinsicValue

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) GetIntrinsicValue() string`

GetIntrinsicValue returns the IntrinsicValue field if non-nil, zero value otherwise.

### GetIntrinsicValueOk

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) GetIntrinsicValueOk() (*string, bool)`

GetIntrinsicValueOk returns a tuple with the IntrinsicValue field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetIntrinsicValue

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) SetIntrinsicValue(v string)`

SetIntrinsicValue sets IntrinsicValue field to given value.

### HasIntrinsicValue

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) HasIntrinsicValue() bool`

HasIntrinsicValue returns a boolean if a field has been set.

### SetIntrinsicValueNil

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) SetIntrinsicValueNil(b bool)`

 SetIntrinsicValueNil sets the value for IntrinsicValue to be an explicit nil

### UnsetIntrinsicValue
`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) UnsetIntrinsicValue()`

UnsetIntrinsicValue ensures that no value is present for IntrinsicValue, not even an explicit nil
### GetPSettle

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) GetPSettle() string`

GetPSettle returns the PSettle field if non-nil, zero value otherwise.

### GetPSettleOk

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) GetPSettleOk() (*string, bool)`

GetPSettleOk returns a tuple with the PSettle field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetPSettle

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) SetPSettle(v string)`

SetPSettle sets PSettle field to given value.

### HasPSettle

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) HasPSettle() bool`

HasPSettle returns a boolean if a field has been set.

### SetPSettleNil

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) SetPSettleNil(b bool)`

 SetPSettleNil sets the value for PSettle to be an explicit nil

### UnsetPSettle
`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) UnsetPSettle()`

UnsetPSettle ensures that no value is present for PSettle, not even an explicit nil
### GetPoi

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) GetPoi() string`

GetPoi returns the Poi field if non-nil, zero value otherwise.

### GetPoiOk

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) GetPoiOk() (*string, bool)`

GetPoiOk returns a tuple with the Poi field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetPoi

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) SetPoi(v string)`

SetPoi sets Poi field to given value.

### HasPoi

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) HasPoi() bool`

HasPoi returns a boolean if a field has been set.

### SetPoiNil

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) SetPoiNil(b bool)`

 SetPoiNil sets the value for Poi to be an explicit nil

### UnsetPoi
`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) UnsetPoi()`

UnsetPoi ensures that no value is present for Poi, not even an explicit nil
### GetUnderlying

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) GetUnderlying() string`

GetUnderlying returns the Underlying field if non-nil, zero value otherwise.

### GetUnderlyingOk

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) GetUnderlyingOk() (*string, bool)`

GetUnderlyingOk returns a tuple with the Underlying field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetUnderlying

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) SetUnderlying(v string)`

SetUnderlying sets Underlying field to given value.

### HasUnderlying

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) HasUnderlying() bool`

HasUnderlying returns a boolean if a field has been set.

### SetUnderlyingNil

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) SetUnderlyingNil(b bool)`

 SetUnderlyingNil sets the value for Underlying to be an explicit nil

### UnsetUnderlying
`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) UnsetUnderlying()`

UnsetUnderlying ensures that no value is present for Underlying, not even an explicit nil
### GetOpen0

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) GetOpen0() string`

GetOpen0 returns the Open0 field if non-nil, zero value otherwise.

### GetOpen0Ok

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) GetOpen0Ok() (*string, bool)`

GetOpen0Ok returns a tuple with the Open0 field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetOpen0

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) SetOpen0(v string)`

SetOpen0 sets Open0 field to given value.

### HasOpen0

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) HasOpen0() bool`

HasOpen0 returns a boolean if a field has been set.

### SetOpen0Nil

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) SetOpen0Nil(b bool)`

 SetOpen0Nil sets the value for Open0 to be an explicit nil

### UnsetOpen0
`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) UnsetOpen0()`

UnsetOpen0 ensures that no value is present for Open0, not even an explicit nil
### GetBasis

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) GetBasis() string`

GetBasis returns the Basis field if non-nil, zero value otherwise.

### GetBasisOk

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) GetBasisOk() (*string, bool)`

GetBasisOk returns a tuple with the Basis field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetBasis

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) SetBasis(v string)`

SetBasis sets Basis field to given value.

### HasBasis

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) HasBasis() bool`

HasBasis returns a boolean if a field has been set.

### SetBasisNil

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) SetBasisNil(b bool)`

 SetBasisNil sets the value for Basis to be an explicit nil

### UnsetBasis
`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) UnsetBasis()`

UnsetBasis ensures that no value is present for Basis, not even an explicit nil
### GetSettle

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) GetSettle() string`

GetSettle returns the Settle field if non-nil, zero value otherwise.

### GetSettleOk

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) GetSettleOk() (*string, bool)`

GetSettleOk returns a tuple with the Settle field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetSettle

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) SetSettle(v string)`

SetSettle sets Settle field to given value.

### HasSettle

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) HasSettle() bool`

HasSettle returns a boolean if a field has been set.

### SetSettleNil

`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) SetSettleNil(b bool)`

 SetSettleNil sets the value for Settle to be an explicit nil

### UnsetSettle
`func (o *PiSMarketDataWSSDomainModelsResponseStreamingBody) UnsetSettle()`

UnsetSettle ensures that no value is present for Settle, not even an explicit nil

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


