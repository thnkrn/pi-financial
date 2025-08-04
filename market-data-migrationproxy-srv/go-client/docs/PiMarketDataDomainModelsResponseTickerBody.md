# PiMarketDataDomainModelsResponseTickerBody

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**Symbol** | Pointer to **NullableString** |  | [optional] 
**Venue** | Pointer to **NullableString** |  | [optional] 
**Price** | Pointer to **NullableString** |  | [optional] 
**Currency** | Pointer to **NullableString** |  | [optional] 
**AuctionPrice** | Pointer to **NullableString** |  | [optional] 
**AuctionVolume** | Pointer to **NullableString** |  | [optional] 
**Open** | Pointer to **NullableString** |  | [optional] 
**High24H** | Pointer to **NullableString** |  | [optional] 
**Low24H** | Pointer to **NullableString** |  | [optional] 
**High52W** | Pointer to **NullableString** |  | [optional] 
**Low52W** | Pointer to **NullableString** |  | [optional] 
**PriceChanged** | Pointer to **NullableString** |  | [optional] 
**PriceChangedRate** | Pointer to **NullableString** |  | [optional] 
**Volume** | Pointer to **NullableString** |  | [optional] 
**Amount** | Pointer to **NullableString** |  | [optional] 
**ChangeAmount** | Pointer to **NullableString** |  | [optional] 
**ChangeVolume** | Pointer to **NullableString** |  | [optional] 
**TurnoverRate** | Pointer to **NullableString** |  | [optional] 
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
**Pe** | Pointer to **NullableString** |  | [optional] 
**Pb** | Pointer to **NullableString** |  | [optional] 
**TotalAmount** | Pointer to **NullableString** |  | [optional] 
**TotalAmountK** | Pointer to **NullableString** |  | [optional] 
**TotalVolume** | Pointer to **NullableString** |  | [optional] 
**TotalVolumeK** | Pointer to **NullableString** |  | [optional] 
**TradableEquity** | Pointer to **NullableString** |  | [optional] 
**TradableAmount** | Pointer to **NullableString** |  | [optional] 
**Eps** | Pointer to **NullableString** |  | [optional] 
**PublicTrades** | Pointer to **[][]interface{}** |  | [optional] 
**OrderBook** | Pointer to [**PiMarketDataDomainModelsResponseTickerOrderBook**](PiMarketDataDomainModelsResponseTickerOrderBook.md) |  | [optional] 
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
**Open1** | Pointer to **NullableString** |  | [optional] 
**Basis** | Pointer to **NullableString** |  | [optional] 
**Settle** | Pointer to **NullableString** |  | [optional] 
**InstrumentCategory** | Pointer to **NullableString** |  | [optional] 
**FriendlyName** | Pointer to **NullableString** |  | [optional] 
**Logo** | Pointer to **NullableString** |  | [optional] 
**ExchangeTimezone** | Pointer to **NullableString** |  | [optional] 
**Country** | Pointer to **NullableString** |  | [optional] 
**OffsetSeconds** | Pointer to **int32** |  | [optional] 
**IsProjected** | Pointer to **bool** |  | [optional] 
**LastPriceTime** | Pointer to **int64** |  | [optional] 

## Methods

### NewPiMarketDataDomainModelsResponseTickerBody

`func NewPiMarketDataDomainModelsResponseTickerBody() *PiMarketDataDomainModelsResponseTickerBody`

NewPiMarketDataDomainModelsResponseTickerBody instantiates a new PiMarketDataDomainModelsResponseTickerBody object
This constructor will assign default values to properties that have it defined,
and makes sure properties required by API are set, but the set of arguments
will change when the set of required properties is changed

### NewPiMarketDataDomainModelsResponseTickerBodyWithDefaults

`func NewPiMarketDataDomainModelsResponseTickerBodyWithDefaults() *PiMarketDataDomainModelsResponseTickerBody`

NewPiMarketDataDomainModelsResponseTickerBodyWithDefaults instantiates a new PiMarketDataDomainModelsResponseTickerBody object
This constructor will only assign default values to properties that have it defined,
but it doesn't guarantee that properties required by API are set

### GetSymbol

`func (o *PiMarketDataDomainModelsResponseTickerBody) GetSymbol() string`

GetSymbol returns the Symbol field if non-nil, zero value otherwise.

### GetSymbolOk

`func (o *PiMarketDataDomainModelsResponseTickerBody) GetSymbolOk() (*string, bool)`

GetSymbolOk returns a tuple with the Symbol field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetSymbol

`func (o *PiMarketDataDomainModelsResponseTickerBody) SetSymbol(v string)`

SetSymbol sets Symbol field to given value.

### HasSymbol

`func (o *PiMarketDataDomainModelsResponseTickerBody) HasSymbol() bool`

HasSymbol returns a boolean if a field has been set.

### SetSymbolNil

`func (o *PiMarketDataDomainModelsResponseTickerBody) SetSymbolNil(b bool)`

 SetSymbolNil sets the value for Symbol to be an explicit nil

### UnsetSymbol
`func (o *PiMarketDataDomainModelsResponseTickerBody) UnsetSymbol()`

UnsetSymbol ensures that no value is present for Symbol, not even an explicit nil
### GetVenue

`func (o *PiMarketDataDomainModelsResponseTickerBody) GetVenue() string`

GetVenue returns the Venue field if non-nil, zero value otherwise.

### GetVenueOk

`func (o *PiMarketDataDomainModelsResponseTickerBody) GetVenueOk() (*string, bool)`

GetVenueOk returns a tuple with the Venue field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetVenue

`func (o *PiMarketDataDomainModelsResponseTickerBody) SetVenue(v string)`

SetVenue sets Venue field to given value.

### HasVenue

`func (o *PiMarketDataDomainModelsResponseTickerBody) HasVenue() bool`

HasVenue returns a boolean if a field has been set.

### SetVenueNil

`func (o *PiMarketDataDomainModelsResponseTickerBody) SetVenueNil(b bool)`

 SetVenueNil sets the value for Venue to be an explicit nil

### UnsetVenue
`func (o *PiMarketDataDomainModelsResponseTickerBody) UnsetVenue()`

UnsetVenue ensures that no value is present for Venue, not even an explicit nil
### GetPrice

`func (o *PiMarketDataDomainModelsResponseTickerBody) GetPrice() string`

GetPrice returns the Price field if non-nil, zero value otherwise.

### GetPriceOk

`func (o *PiMarketDataDomainModelsResponseTickerBody) GetPriceOk() (*string, bool)`

GetPriceOk returns a tuple with the Price field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetPrice

`func (o *PiMarketDataDomainModelsResponseTickerBody) SetPrice(v string)`

SetPrice sets Price field to given value.

### HasPrice

`func (o *PiMarketDataDomainModelsResponseTickerBody) HasPrice() bool`

HasPrice returns a boolean if a field has been set.

### SetPriceNil

`func (o *PiMarketDataDomainModelsResponseTickerBody) SetPriceNil(b bool)`

 SetPriceNil sets the value for Price to be an explicit nil

### UnsetPrice
`func (o *PiMarketDataDomainModelsResponseTickerBody) UnsetPrice()`

UnsetPrice ensures that no value is present for Price, not even an explicit nil
### GetCurrency

`func (o *PiMarketDataDomainModelsResponseTickerBody) GetCurrency() string`

GetCurrency returns the Currency field if non-nil, zero value otherwise.

### GetCurrencyOk

`func (o *PiMarketDataDomainModelsResponseTickerBody) GetCurrencyOk() (*string, bool)`

GetCurrencyOk returns a tuple with the Currency field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetCurrency

`func (o *PiMarketDataDomainModelsResponseTickerBody) SetCurrency(v string)`

SetCurrency sets Currency field to given value.

### HasCurrency

`func (o *PiMarketDataDomainModelsResponseTickerBody) HasCurrency() bool`

HasCurrency returns a boolean if a field has been set.

### SetCurrencyNil

`func (o *PiMarketDataDomainModelsResponseTickerBody) SetCurrencyNil(b bool)`

 SetCurrencyNil sets the value for Currency to be an explicit nil

### UnsetCurrency
`func (o *PiMarketDataDomainModelsResponseTickerBody) UnsetCurrency()`

UnsetCurrency ensures that no value is present for Currency, not even an explicit nil
### GetAuctionPrice

`func (o *PiMarketDataDomainModelsResponseTickerBody) GetAuctionPrice() string`

GetAuctionPrice returns the AuctionPrice field if non-nil, zero value otherwise.

### GetAuctionPriceOk

`func (o *PiMarketDataDomainModelsResponseTickerBody) GetAuctionPriceOk() (*string, bool)`

GetAuctionPriceOk returns a tuple with the AuctionPrice field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetAuctionPrice

`func (o *PiMarketDataDomainModelsResponseTickerBody) SetAuctionPrice(v string)`

SetAuctionPrice sets AuctionPrice field to given value.

### HasAuctionPrice

`func (o *PiMarketDataDomainModelsResponseTickerBody) HasAuctionPrice() bool`

HasAuctionPrice returns a boolean if a field has been set.

### SetAuctionPriceNil

`func (o *PiMarketDataDomainModelsResponseTickerBody) SetAuctionPriceNil(b bool)`

 SetAuctionPriceNil sets the value for AuctionPrice to be an explicit nil

### UnsetAuctionPrice
`func (o *PiMarketDataDomainModelsResponseTickerBody) UnsetAuctionPrice()`

UnsetAuctionPrice ensures that no value is present for AuctionPrice, not even an explicit nil
### GetAuctionVolume

`func (o *PiMarketDataDomainModelsResponseTickerBody) GetAuctionVolume() string`

GetAuctionVolume returns the AuctionVolume field if non-nil, zero value otherwise.

### GetAuctionVolumeOk

`func (o *PiMarketDataDomainModelsResponseTickerBody) GetAuctionVolumeOk() (*string, bool)`

GetAuctionVolumeOk returns a tuple with the AuctionVolume field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetAuctionVolume

`func (o *PiMarketDataDomainModelsResponseTickerBody) SetAuctionVolume(v string)`

SetAuctionVolume sets AuctionVolume field to given value.

### HasAuctionVolume

`func (o *PiMarketDataDomainModelsResponseTickerBody) HasAuctionVolume() bool`

HasAuctionVolume returns a boolean if a field has been set.

### SetAuctionVolumeNil

`func (o *PiMarketDataDomainModelsResponseTickerBody) SetAuctionVolumeNil(b bool)`

 SetAuctionVolumeNil sets the value for AuctionVolume to be an explicit nil

### UnsetAuctionVolume
`func (o *PiMarketDataDomainModelsResponseTickerBody) UnsetAuctionVolume()`

UnsetAuctionVolume ensures that no value is present for AuctionVolume, not even an explicit nil
### GetOpen

`func (o *PiMarketDataDomainModelsResponseTickerBody) GetOpen() string`

GetOpen returns the Open field if non-nil, zero value otherwise.

### GetOpenOk

`func (o *PiMarketDataDomainModelsResponseTickerBody) GetOpenOk() (*string, bool)`

GetOpenOk returns a tuple with the Open field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetOpen

`func (o *PiMarketDataDomainModelsResponseTickerBody) SetOpen(v string)`

SetOpen sets Open field to given value.

### HasOpen

`func (o *PiMarketDataDomainModelsResponseTickerBody) HasOpen() bool`

HasOpen returns a boolean if a field has been set.

### SetOpenNil

`func (o *PiMarketDataDomainModelsResponseTickerBody) SetOpenNil(b bool)`

 SetOpenNil sets the value for Open to be an explicit nil

### UnsetOpen
`func (o *PiMarketDataDomainModelsResponseTickerBody) UnsetOpen()`

UnsetOpen ensures that no value is present for Open, not even an explicit nil
### GetHigh24H

`func (o *PiMarketDataDomainModelsResponseTickerBody) GetHigh24H() string`

GetHigh24H returns the High24H field if non-nil, zero value otherwise.

### GetHigh24HOk

`func (o *PiMarketDataDomainModelsResponseTickerBody) GetHigh24HOk() (*string, bool)`

GetHigh24HOk returns a tuple with the High24H field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetHigh24H

`func (o *PiMarketDataDomainModelsResponseTickerBody) SetHigh24H(v string)`

SetHigh24H sets High24H field to given value.

### HasHigh24H

`func (o *PiMarketDataDomainModelsResponseTickerBody) HasHigh24H() bool`

HasHigh24H returns a boolean if a field has been set.

### SetHigh24HNil

`func (o *PiMarketDataDomainModelsResponseTickerBody) SetHigh24HNil(b bool)`

 SetHigh24HNil sets the value for High24H to be an explicit nil

### UnsetHigh24H
`func (o *PiMarketDataDomainModelsResponseTickerBody) UnsetHigh24H()`

UnsetHigh24H ensures that no value is present for High24H, not even an explicit nil
### GetLow24H

`func (o *PiMarketDataDomainModelsResponseTickerBody) GetLow24H() string`

GetLow24H returns the Low24H field if non-nil, zero value otherwise.

### GetLow24HOk

`func (o *PiMarketDataDomainModelsResponseTickerBody) GetLow24HOk() (*string, bool)`

GetLow24HOk returns a tuple with the Low24H field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetLow24H

`func (o *PiMarketDataDomainModelsResponseTickerBody) SetLow24H(v string)`

SetLow24H sets Low24H field to given value.

### HasLow24H

`func (o *PiMarketDataDomainModelsResponseTickerBody) HasLow24H() bool`

HasLow24H returns a boolean if a field has been set.

### SetLow24HNil

`func (o *PiMarketDataDomainModelsResponseTickerBody) SetLow24HNil(b bool)`

 SetLow24HNil sets the value for Low24H to be an explicit nil

### UnsetLow24H
`func (o *PiMarketDataDomainModelsResponseTickerBody) UnsetLow24H()`

UnsetLow24H ensures that no value is present for Low24H, not even an explicit nil
### GetHigh52W

`func (o *PiMarketDataDomainModelsResponseTickerBody) GetHigh52W() string`

GetHigh52W returns the High52W field if non-nil, zero value otherwise.

### GetHigh52WOk

`func (o *PiMarketDataDomainModelsResponseTickerBody) GetHigh52WOk() (*string, bool)`

GetHigh52WOk returns a tuple with the High52W field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetHigh52W

`func (o *PiMarketDataDomainModelsResponseTickerBody) SetHigh52W(v string)`

SetHigh52W sets High52W field to given value.

### HasHigh52W

`func (o *PiMarketDataDomainModelsResponseTickerBody) HasHigh52W() bool`

HasHigh52W returns a boolean if a field has been set.

### SetHigh52WNil

`func (o *PiMarketDataDomainModelsResponseTickerBody) SetHigh52WNil(b bool)`

 SetHigh52WNil sets the value for High52W to be an explicit nil

### UnsetHigh52W
`func (o *PiMarketDataDomainModelsResponseTickerBody) UnsetHigh52W()`

UnsetHigh52W ensures that no value is present for High52W, not even an explicit nil
### GetLow52W

`func (o *PiMarketDataDomainModelsResponseTickerBody) GetLow52W() string`

GetLow52W returns the Low52W field if non-nil, zero value otherwise.

### GetLow52WOk

`func (o *PiMarketDataDomainModelsResponseTickerBody) GetLow52WOk() (*string, bool)`

GetLow52WOk returns a tuple with the Low52W field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetLow52W

`func (o *PiMarketDataDomainModelsResponseTickerBody) SetLow52W(v string)`

SetLow52W sets Low52W field to given value.

### HasLow52W

`func (o *PiMarketDataDomainModelsResponseTickerBody) HasLow52W() bool`

HasLow52W returns a boolean if a field has been set.

### SetLow52WNil

`func (o *PiMarketDataDomainModelsResponseTickerBody) SetLow52WNil(b bool)`

 SetLow52WNil sets the value for Low52W to be an explicit nil

### UnsetLow52W
`func (o *PiMarketDataDomainModelsResponseTickerBody) UnsetLow52W()`

UnsetLow52W ensures that no value is present for Low52W, not even an explicit nil
### GetPriceChanged

`func (o *PiMarketDataDomainModelsResponseTickerBody) GetPriceChanged() string`

GetPriceChanged returns the PriceChanged field if non-nil, zero value otherwise.

### GetPriceChangedOk

`func (o *PiMarketDataDomainModelsResponseTickerBody) GetPriceChangedOk() (*string, bool)`

GetPriceChangedOk returns a tuple with the PriceChanged field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetPriceChanged

`func (o *PiMarketDataDomainModelsResponseTickerBody) SetPriceChanged(v string)`

SetPriceChanged sets PriceChanged field to given value.

### HasPriceChanged

`func (o *PiMarketDataDomainModelsResponseTickerBody) HasPriceChanged() bool`

HasPriceChanged returns a boolean if a field has been set.

### SetPriceChangedNil

`func (o *PiMarketDataDomainModelsResponseTickerBody) SetPriceChangedNil(b bool)`

 SetPriceChangedNil sets the value for PriceChanged to be an explicit nil

### UnsetPriceChanged
`func (o *PiMarketDataDomainModelsResponseTickerBody) UnsetPriceChanged()`

UnsetPriceChanged ensures that no value is present for PriceChanged, not even an explicit nil
### GetPriceChangedRate

`func (o *PiMarketDataDomainModelsResponseTickerBody) GetPriceChangedRate() string`

GetPriceChangedRate returns the PriceChangedRate field if non-nil, zero value otherwise.

### GetPriceChangedRateOk

`func (o *PiMarketDataDomainModelsResponseTickerBody) GetPriceChangedRateOk() (*string, bool)`

GetPriceChangedRateOk returns a tuple with the PriceChangedRate field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetPriceChangedRate

`func (o *PiMarketDataDomainModelsResponseTickerBody) SetPriceChangedRate(v string)`

SetPriceChangedRate sets PriceChangedRate field to given value.

### HasPriceChangedRate

`func (o *PiMarketDataDomainModelsResponseTickerBody) HasPriceChangedRate() bool`

HasPriceChangedRate returns a boolean if a field has been set.

### SetPriceChangedRateNil

`func (o *PiMarketDataDomainModelsResponseTickerBody) SetPriceChangedRateNil(b bool)`

 SetPriceChangedRateNil sets the value for PriceChangedRate to be an explicit nil

### UnsetPriceChangedRate
`func (o *PiMarketDataDomainModelsResponseTickerBody) UnsetPriceChangedRate()`

UnsetPriceChangedRate ensures that no value is present for PriceChangedRate, not even an explicit nil
### GetVolume

`func (o *PiMarketDataDomainModelsResponseTickerBody) GetVolume() string`

GetVolume returns the Volume field if non-nil, zero value otherwise.

### GetVolumeOk

`func (o *PiMarketDataDomainModelsResponseTickerBody) GetVolumeOk() (*string, bool)`

GetVolumeOk returns a tuple with the Volume field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetVolume

`func (o *PiMarketDataDomainModelsResponseTickerBody) SetVolume(v string)`

SetVolume sets Volume field to given value.

### HasVolume

`func (o *PiMarketDataDomainModelsResponseTickerBody) HasVolume() bool`

HasVolume returns a boolean if a field has been set.

### SetVolumeNil

`func (o *PiMarketDataDomainModelsResponseTickerBody) SetVolumeNil(b bool)`

 SetVolumeNil sets the value for Volume to be an explicit nil

### UnsetVolume
`func (o *PiMarketDataDomainModelsResponseTickerBody) UnsetVolume()`

UnsetVolume ensures that no value is present for Volume, not even an explicit nil
### GetAmount

`func (o *PiMarketDataDomainModelsResponseTickerBody) GetAmount() string`

GetAmount returns the Amount field if non-nil, zero value otherwise.

### GetAmountOk

`func (o *PiMarketDataDomainModelsResponseTickerBody) GetAmountOk() (*string, bool)`

GetAmountOk returns a tuple with the Amount field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetAmount

`func (o *PiMarketDataDomainModelsResponseTickerBody) SetAmount(v string)`

SetAmount sets Amount field to given value.

### HasAmount

`func (o *PiMarketDataDomainModelsResponseTickerBody) HasAmount() bool`

HasAmount returns a boolean if a field has been set.

### SetAmountNil

`func (o *PiMarketDataDomainModelsResponseTickerBody) SetAmountNil(b bool)`

 SetAmountNil sets the value for Amount to be an explicit nil

### UnsetAmount
`func (o *PiMarketDataDomainModelsResponseTickerBody) UnsetAmount()`

UnsetAmount ensures that no value is present for Amount, not even an explicit nil
### GetChangeAmount

`func (o *PiMarketDataDomainModelsResponseTickerBody) GetChangeAmount() string`

GetChangeAmount returns the ChangeAmount field if non-nil, zero value otherwise.

### GetChangeAmountOk

`func (o *PiMarketDataDomainModelsResponseTickerBody) GetChangeAmountOk() (*string, bool)`

GetChangeAmountOk returns a tuple with the ChangeAmount field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetChangeAmount

`func (o *PiMarketDataDomainModelsResponseTickerBody) SetChangeAmount(v string)`

SetChangeAmount sets ChangeAmount field to given value.

### HasChangeAmount

`func (o *PiMarketDataDomainModelsResponseTickerBody) HasChangeAmount() bool`

HasChangeAmount returns a boolean if a field has been set.

### SetChangeAmountNil

`func (o *PiMarketDataDomainModelsResponseTickerBody) SetChangeAmountNil(b bool)`

 SetChangeAmountNil sets the value for ChangeAmount to be an explicit nil

### UnsetChangeAmount
`func (o *PiMarketDataDomainModelsResponseTickerBody) UnsetChangeAmount()`

UnsetChangeAmount ensures that no value is present for ChangeAmount, not even an explicit nil
### GetChangeVolume

`func (o *PiMarketDataDomainModelsResponseTickerBody) GetChangeVolume() string`

GetChangeVolume returns the ChangeVolume field if non-nil, zero value otherwise.

### GetChangeVolumeOk

`func (o *PiMarketDataDomainModelsResponseTickerBody) GetChangeVolumeOk() (*string, bool)`

GetChangeVolumeOk returns a tuple with the ChangeVolume field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetChangeVolume

`func (o *PiMarketDataDomainModelsResponseTickerBody) SetChangeVolume(v string)`

SetChangeVolume sets ChangeVolume field to given value.

### HasChangeVolume

`func (o *PiMarketDataDomainModelsResponseTickerBody) HasChangeVolume() bool`

HasChangeVolume returns a boolean if a field has been set.

### SetChangeVolumeNil

`func (o *PiMarketDataDomainModelsResponseTickerBody) SetChangeVolumeNil(b bool)`

 SetChangeVolumeNil sets the value for ChangeVolume to be an explicit nil

### UnsetChangeVolume
`func (o *PiMarketDataDomainModelsResponseTickerBody) UnsetChangeVolume()`

UnsetChangeVolume ensures that no value is present for ChangeVolume, not even an explicit nil
### GetTurnoverRate

`func (o *PiMarketDataDomainModelsResponseTickerBody) GetTurnoverRate() string`

GetTurnoverRate returns the TurnoverRate field if non-nil, zero value otherwise.

### GetTurnoverRateOk

`func (o *PiMarketDataDomainModelsResponseTickerBody) GetTurnoverRateOk() (*string, bool)`

GetTurnoverRateOk returns a tuple with the TurnoverRate field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetTurnoverRate

`func (o *PiMarketDataDomainModelsResponseTickerBody) SetTurnoverRate(v string)`

SetTurnoverRate sets TurnoverRate field to given value.

### HasTurnoverRate

`func (o *PiMarketDataDomainModelsResponseTickerBody) HasTurnoverRate() bool`

HasTurnoverRate returns a boolean if a field has been set.

### SetTurnoverRateNil

`func (o *PiMarketDataDomainModelsResponseTickerBody) SetTurnoverRateNil(b bool)`

 SetTurnoverRateNil sets the value for TurnoverRate to be an explicit nil

### UnsetTurnoverRate
`func (o *PiMarketDataDomainModelsResponseTickerBody) UnsetTurnoverRate()`

UnsetTurnoverRate ensures that no value is present for TurnoverRate, not even an explicit nil
### GetOpen2

`func (o *PiMarketDataDomainModelsResponseTickerBody) GetOpen2() string`

GetOpen2 returns the Open2 field if non-nil, zero value otherwise.

### GetOpen2Ok

`func (o *PiMarketDataDomainModelsResponseTickerBody) GetOpen2Ok() (*string, bool)`

GetOpen2Ok returns a tuple with the Open2 field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetOpen2

`func (o *PiMarketDataDomainModelsResponseTickerBody) SetOpen2(v string)`

SetOpen2 sets Open2 field to given value.

### HasOpen2

`func (o *PiMarketDataDomainModelsResponseTickerBody) HasOpen2() bool`

HasOpen2 returns a boolean if a field has been set.

### SetOpen2Nil

`func (o *PiMarketDataDomainModelsResponseTickerBody) SetOpen2Nil(b bool)`

 SetOpen2Nil sets the value for Open2 to be an explicit nil

### UnsetOpen2
`func (o *PiMarketDataDomainModelsResponseTickerBody) UnsetOpen2()`

UnsetOpen2 ensures that no value is present for Open2, not even an explicit nil
### GetCeiling

`func (o *PiMarketDataDomainModelsResponseTickerBody) GetCeiling() string`

GetCeiling returns the Ceiling field if non-nil, zero value otherwise.

### GetCeilingOk

`func (o *PiMarketDataDomainModelsResponseTickerBody) GetCeilingOk() (*string, bool)`

GetCeilingOk returns a tuple with the Ceiling field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetCeiling

`func (o *PiMarketDataDomainModelsResponseTickerBody) SetCeiling(v string)`

SetCeiling sets Ceiling field to given value.

### HasCeiling

`func (o *PiMarketDataDomainModelsResponseTickerBody) HasCeiling() bool`

HasCeiling returns a boolean if a field has been set.

### SetCeilingNil

`func (o *PiMarketDataDomainModelsResponseTickerBody) SetCeilingNil(b bool)`

 SetCeilingNil sets the value for Ceiling to be an explicit nil

### UnsetCeiling
`func (o *PiMarketDataDomainModelsResponseTickerBody) UnsetCeiling()`

UnsetCeiling ensures that no value is present for Ceiling, not even an explicit nil
### GetFloor

`func (o *PiMarketDataDomainModelsResponseTickerBody) GetFloor() string`

GetFloor returns the Floor field if non-nil, zero value otherwise.

### GetFloorOk

`func (o *PiMarketDataDomainModelsResponseTickerBody) GetFloorOk() (*string, bool)`

GetFloorOk returns a tuple with the Floor field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetFloor

`func (o *PiMarketDataDomainModelsResponseTickerBody) SetFloor(v string)`

SetFloor sets Floor field to given value.

### HasFloor

`func (o *PiMarketDataDomainModelsResponseTickerBody) HasFloor() bool`

HasFloor returns a boolean if a field has been set.

### SetFloorNil

`func (o *PiMarketDataDomainModelsResponseTickerBody) SetFloorNil(b bool)`

 SetFloorNil sets the value for Floor to be an explicit nil

### UnsetFloor
`func (o *PiMarketDataDomainModelsResponseTickerBody) UnsetFloor()`

UnsetFloor ensures that no value is present for Floor, not even an explicit nil
### GetAverage

`func (o *PiMarketDataDomainModelsResponseTickerBody) GetAverage() string`

GetAverage returns the Average field if non-nil, zero value otherwise.

### GetAverageOk

`func (o *PiMarketDataDomainModelsResponseTickerBody) GetAverageOk() (*string, bool)`

GetAverageOk returns a tuple with the Average field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetAverage

`func (o *PiMarketDataDomainModelsResponseTickerBody) SetAverage(v string)`

SetAverage sets Average field to given value.

### HasAverage

`func (o *PiMarketDataDomainModelsResponseTickerBody) HasAverage() bool`

HasAverage returns a boolean if a field has been set.

### SetAverageNil

`func (o *PiMarketDataDomainModelsResponseTickerBody) SetAverageNil(b bool)`

 SetAverageNil sets the value for Average to be an explicit nil

### UnsetAverage
`func (o *PiMarketDataDomainModelsResponseTickerBody) UnsetAverage()`

UnsetAverage ensures that no value is present for Average, not even an explicit nil
### GetAverageBuy

`func (o *PiMarketDataDomainModelsResponseTickerBody) GetAverageBuy() string`

GetAverageBuy returns the AverageBuy field if non-nil, zero value otherwise.

### GetAverageBuyOk

`func (o *PiMarketDataDomainModelsResponseTickerBody) GetAverageBuyOk() (*string, bool)`

GetAverageBuyOk returns a tuple with the AverageBuy field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetAverageBuy

`func (o *PiMarketDataDomainModelsResponseTickerBody) SetAverageBuy(v string)`

SetAverageBuy sets AverageBuy field to given value.

### HasAverageBuy

`func (o *PiMarketDataDomainModelsResponseTickerBody) HasAverageBuy() bool`

HasAverageBuy returns a boolean if a field has been set.

### SetAverageBuyNil

`func (o *PiMarketDataDomainModelsResponseTickerBody) SetAverageBuyNil(b bool)`

 SetAverageBuyNil sets the value for AverageBuy to be an explicit nil

### UnsetAverageBuy
`func (o *PiMarketDataDomainModelsResponseTickerBody) UnsetAverageBuy()`

UnsetAverageBuy ensures that no value is present for AverageBuy, not even an explicit nil
### GetAverageSell

`func (o *PiMarketDataDomainModelsResponseTickerBody) GetAverageSell() string`

GetAverageSell returns the AverageSell field if non-nil, zero value otherwise.

### GetAverageSellOk

`func (o *PiMarketDataDomainModelsResponseTickerBody) GetAverageSellOk() (*string, bool)`

GetAverageSellOk returns a tuple with the AverageSell field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetAverageSell

`func (o *PiMarketDataDomainModelsResponseTickerBody) SetAverageSell(v string)`

SetAverageSell sets AverageSell field to given value.

### HasAverageSell

`func (o *PiMarketDataDomainModelsResponseTickerBody) HasAverageSell() bool`

HasAverageSell returns a boolean if a field has been set.

### SetAverageSellNil

`func (o *PiMarketDataDomainModelsResponseTickerBody) SetAverageSellNil(b bool)`

 SetAverageSellNil sets the value for AverageSell to be an explicit nil

### UnsetAverageSell
`func (o *PiMarketDataDomainModelsResponseTickerBody) UnsetAverageSell()`

UnsetAverageSell ensures that no value is present for AverageSell, not even an explicit nil
### GetAggressor

`func (o *PiMarketDataDomainModelsResponseTickerBody) GetAggressor() string`

GetAggressor returns the Aggressor field if non-nil, zero value otherwise.

### GetAggressorOk

`func (o *PiMarketDataDomainModelsResponseTickerBody) GetAggressorOk() (*string, bool)`

GetAggressorOk returns a tuple with the Aggressor field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetAggressor

`func (o *PiMarketDataDomainModelsResponseTickerBody) SetAggressor(v string)`

SetAggressor sets Aggressor field to given value.

### HasAggressor

`func (o *PiMarketDataDomainModelsResponseTickerBody) HasAggressor() bool`

HasAggressor returns a boolean if a field has been set.

### SetAggressorNil

`func (o *PiMarketDataDomainModelsResponseTickerBody) SetAggressorNil(b bool)`

 SetAggressorNil sets the value for Aggressor to be an explicit nil

### UnsetAggressor
`func (o *PiMarketDataDomainModelsResponseTickerBody) UnsetAggressor()`

UnsetAggressor ensures that no value is present for Aggressor, not even an explicit nil
### GetPreClose

`func (o *PiMarketDataDomainModelsResponseTickerBody) GetPreClose() string`

GetPreClose returns the PreClose field if non-nil, zero value otherwise.

### GetPreCloseOk

`func (o *PiMarketDataDomainModelsResponseTickerBody) GetPreCloseOk() (*string, bool)`

GetPreCloseOk returns a tuple with the PreClose field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetPreClose

`func (o *PiMarketDataDomainModelsResponseTickerBody) SetPreClose(v string)`

SetPreClose sets PreClose field to given value.

### HasPreClose

`func (o *PiMarketDataDomainModelsResponseTickerBody) HasPreClose() bool`

HasPreClose returns a boolean if a field has been set.

### SetPreCloseNil

`func (o *PiMarketDataDomainModelsResponseTickerBody) SetPreCloseNil(b bool)`

 SetPreCloseNil sets the value for PreClose to be an explicit nil

### UnsetPreClose
`func (o *PiMarketDataDomainModelsResponseTickerBody) UnsetPreClose()`

UnsetPreClose ensures that no value is present for PreClose, not even an explicit nil
### GetStatus

`func (o *PiMarketDataDomainModelsResponseTickerBody) GetStatus() string`

GetStatus returns the Status field if non-nil, zero value otherwise.

### GetStatusOk

`func (o *PiMarketDataDomainModelsResponseTickerBody) GetStatusOk() (*string, bool)`

GetStatusOk returns a tuple with the Status field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetStatus

`func (o *PiMarketDataDomainModelsResponseTickerBody) SetStatus(v string)`

SetStatus sets Status field to given value.

### HasStatus

`func (o *PiMarketDataDomainModelsResponseTickerBody) HasStatus() bool`

HasStatus returns a boolean if a field has been set.

### SetStatusNil

`func (o *PiMarketDataDomainModelsResponseTickerBody) SetStatusNil(b bool)`

 SetStatusNil sets the value for Status to be an explicit nil

### UnsetStatus
`func (o *PiMarketDataDomainModelsResponseTickerBody) UnsetStatus()`

UnsetStatus ensures that no value is present for Status, not even an explicit nil
### GetYield

`func (o *PiMarketDataDomainModelsResponseTickerBody) GetYield() string`

GetYield returns the Yield field if non-nil, zero value otherwise.

### GetYieldOk

`func (o *PiMarketDataDomainModelsResponseTickerBody) GetYieldOk() (*string, bool)`

GetYieldOk returns a tuple with the Yield field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetYield

`func (o *PiMarketDataDomainModelsResponseTickerBody) SetYield(v string)`

SetYield sets Yield field to given value.

### HasYield

`func (o *PiMarketDataDomainModelsResponseTickerBody) HasYield() bool`

HasYield returns a boolean if a field has been set.

### SetYieldNil

`func (o *PiMarketDataDomainModelsResponseTickerBody) SetYieldNil(b bool)`

 SetYieldNil sets the value for Yield to be an explicit nil

### UnsetYield
`func (o *PiMarketDataDomainModelsResponseTickerBody) UnsetYield()`

UnsetYield ensures that no value is present for Yield, not even an explicit nil
### GetPe

`func (o *PiMarketDataDomainModelsResponseTickerBody) GetPe() string`

GetPe returns the Pe field if non-nil, zero value otherwise.

### GetPeOk

`func (o *PiMarketDataDomainModelsResponseTickerBody) GetPeOk() (*string, bool)`

GetPeOk returns a tuple with the Pe field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetPe

`func (o *PiMarketDataDomainModelsResponseTickerBody) SetPe(v string)`

SetPe sets Pe field to given value.

### HasPe

`func (o *PiMarketDataDomainModelsResponseTickerBody) HasPe() bool`

HasPe returns a boolean if a field has been set.

### SetPeNil

`func (o *PiMarketDataDomainModelsResponseTickerBody) SetPeNil(b bool)`

 SetPeNil sets the value for Pe to be an explicit nil

### UnsetPe
`func (o *PiMarketDataDomainModelsResponseTickerBody) UnsetPe()`

UnsetPe ensures that no value is present for Pe, not even an explicit nil
### GetPb

`func (o *PiMarketDataDomainModelsResponseTickerBody) GetPb() string`

GetPb returns the Pb field if non-nil, zero value otherwise.

### GetPbOk

`func (o *PiMarketDataDomainModelsResponseTickerBody) GetPbOk() (*string, bool)`

GetPbOk returns a tuple with the Pb field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetPb

`func (o *PiMarketDataDomainModelsResponseTickerBody) SetPb(v string)`

SetPb sets Pb field to given value.

### HasPb

`func (o *PiMarketDataDomainModelsResponseTickerBody) HasPb() bool`

HasPb returns a boolean if a field has been set.

### SetPbNil

`func (o *PiMarketDataDomainModelsResponseTickerBody) SetPbNil(b bool)`

 SetPbNil sets the value for Pb to be an explicit nil

### UnsetPb
`func (o *PiMarketDataDomainModelsResponseTickerBody) UnsetPb()`

UnsetPb ensures that no value is present for Pb, not even an explicit nil
### GetTotalAmount

`func (o *PiMarketDataDomainModelsResponseTickerBody) GetTotalAmount() string`

GetTotalAmount returns the TotalAmount field if non-nil, zero value otherwise.

### GetTotalAmountOk

`func (o *PiMarketDataDomainModelsResponseTickerBody) GetTotalAmountOk() (*string, bool)`

GetTotalAmountOk returns a tuple with the TotalAmount field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetTotalAmount

`func (o *PiMarketDataDomainModelsResponseTickerBody) SetTotalAmount(v string)`

SetTotalAmount sets TotalAmount field to given value.

### HasTotalAmount

`func (o *PiMarketDataDomainModelsResponseTickerBody) HasTotalAmount() bool`

HasTotalAmount returns a boolean if a field has been set.

### SetTotalAmountNil

`func (o *PiMarketDataDomainModelsResponseTickerBody) SetTotalAmountNil(b bool)`

 SetTotalAmountNil sets the value for TotalAmount to be an explicit nil

### UnsetTotalAmount
`func (o *PiMarketDataDomainModelsResponseTickerBody) UnsetTotalAmount()`

UnsetTotalAmount ensures that no value is present for TotalAmount, not even an explicit nil
### GetTotalAmountK

`func (o *PiMarketDataDomainModelsResponseTickerBody) GetTotalAmountK() string`

GetTotalAmountK returns the TotalAmountK field if non-nil, zero value otherwise.

### GetTotalAmountKOk

`func (o *PiMarketDataDomainModelsResponseTickerBody) GetTotalAmountKOk() (*string, bool)`

GetTotalAmountKOk returns a tuple with the TotalAmountK field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetTotalAmountK

`func (o *PiMarketDataDomainModelsResponseTickerBody) SetTotalAmountK(v string)`

SetTotalAmountK sets TotalAmountK field to given value.

### HasTotalAmountK

`func (o *PiMarketDataDomainModelsResponseTickerBody) HasTotalAmountK() bool`

HasTotalAmountK returns a boolean if a field has been set.

### SetTotalAmountKNil

`func (o *PiMarketDataDomainModelsResponseTickerBody) SetTotalAmountKNil(b bool)`

 SetTotalAmountKNil sets the value for TotalAmountK to be an explicit nil

### UnsetTotalAmountK
`func (o *PiMarketDataDomainModelsResponseTickerBody) UnsetTotalAmountK()`

UnsetTotalAmountK ensures that no value is present for TotalAmountK, not even an explicit nil
### GetTotalVolume

`func (o *PiMarketDataDomainModelsResponseTickerBody) GetTotalVolume() string`

GetTotalVolume returns the TotalVolume field if non-nil, zero value otherwise.

### GetTotalVolumeOk

`func (o *PiMarketDataDomainModelsResponseTickerBody) GetTotalVolumeOk() (*string, bool)`

GetTotalVolumeOk returns a tuple with the TotalVolume field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetTotalVolume

`func (o *PiMarketDataDomainModelsResponseTickerBody) SetTotalVolume(v string)`

SetTotalVolume sets TotalVolume field to given value.

### HasTotalVolume

`func (o *PiMarketDataDomainModelsResponseTickerBody) HasTotalVolume() bool`

HasTotalVolume returns a boolean if a field has been set.

### SetTotalVolumeNil

`func (o *PiMarketDataDomainModelsResponseTickerBody) SetTotalVolumeNil(b bool)`

 SetTotalVolumeNil sets the value for TotalVolume to be an explicit nil

### UnsetTotalVolume
`func (o *PiMarketDataDomainModelsResponseTickerBody) UnsetTotalVolume()`

UnsetTotalVolume ensures that no value is present for TotalVolume, not even an explicit nil
### GetTotalVolumeK

`func (o *PiMarketDataDomainModelsResponseTickerBody) GetTotalVolumeK() string`

GetTotalVolumeK returns the TotalVolumeK field if non-nil, zero value otherwise.

### GetTotalVolumeKOk

`func (o *PiMarketDataDomainModelsResponseTickerBody) GetTotalVolumeKOk() (*string, bool)`

GetTotalVolumeKOk returns a tuple with the TotalVolumeK field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetTotalVolumeK

`func (o *PiMarketDataDomainModelsResponseTickerBody) SetTotalVolumeK(v string)`

SetTotalVolumeK sets TotalVolumeK field to given value.

### HasTotalVolumeK

`func (o *PiMarketDataDomainModelsResponseTickerBody) HasTotalVolumeK() bool`

HasTotalVolumeK returns a boolean if a field has been set.

### SetTotalVolumeKNil

`func (o *PiMarketDataDomainModelsResponseTickerBody) SetTotalVolumeKNil(b bool)`

 SetTotalVolumeKNil sets the value for TotalVolumeK to be an explicit nil

### UnsetTotalVolumeK
`func (o *PiMarketDataDomainModelsResponseTickerBody) UnsetTotalVolumeK()`

UnsetTotalVolumeK ensures that no value is present for TotalVolumeK, not even an explicit nil
### GetTradableEquity

`func (o *PiMarketDataDomainModelsResponseTickerBody) GetTradableEquity() string`

GetTradableEquity returns the TradableEquity field if non-nil, zero value otherwise.

### GetTradableEquityOk

`func (o *PiMarketDataDomainModelsResponseTickerBody) GetTradableEquityOk() (*string, bool)`

GetTradableEquityOk returns a tuple with the TradableEquity field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetTradableEquity

`func (o *PiMarketDataDomainModelsResponseTickerBody) SetTradableEquity(v string)`

SetTradableEquity sets TradableEquity field to given value.

### HasTradableEquity

`func (o *PiMarketDataDomainModelsResponseTickerBody) HasTradableEquity() bool`

HasTradableEquity returns a boolean if a field has been set.

### SetTradableEquityNil

`func (o *PiMarketDataDomainModelsResponseTickerBody) SetTradableEquityNil(b bool)`

 SetTradableEquityNil sets the value for TradableEquity to be an explicit nil

### UnsetTradableEquity
`func (o *PiMarketDataDomainModelsResponseTickerBody) UnsetTradableEquity()`

UnsetTradableEquity ensures that no value is present for TradableEquity, not even an explicit nil
### GetTradableAmount

`func (o *PiMarketDataDomainModelsResponseTickerBody) GetTradableAmount() string`

GetTradableAmount returns the TradableAmount field if non-nil, zero value otherwise.

### GetTradableAmountOk

`func (o *PiMarketDataDomainModelsResponseTickerBody) GetTradableAmountOk() (*string, bool)`

GetTradableAmountOk returns a tuple with the TradableAmount field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetTradableAmount

`func (o *PiMarketDataDomainModelsResponseTickerBody) SetTradableAmount(v string)`

SetTradableAmount sets TradableAmount field to given value.

### HasTradableAmount

`func (o *PiMarketDataDomainModelsResponseTickerBody) HasTradableAmount() bool`

HasTradableAmount returns a boolean if a field has been set.

### SetTradableAmountNil

`func (o *PiMarketDataDomainModelsResponseTickerBody) SetTradableAmountNil(b bool)`

 SetTradableAmountNil sets the value for TradableAmount to be an explicit nil

### UnsetTradableAmount
`func (o *PiMarketDataDomainModelsResponseTickerBody) UnsetTradableAmount()`

UnsetTradableAmount ensures that no value is present for TradableAmount, not even an explicit nil
### GetEps

`func (o *PiMarketDataDomainModelsResponseTickerBody) GetEps() string`

GetEps returns the Eps field if non-nil, zero value otherwise.

### GetEpsOk

`func (o *PiMarketDataDomainModelsResponseTickerBody) GetEpsOk() (*string, bool)`

GetEpsOk returns a tuple with the Eps field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetEps

`func (o *PiMarketDataDomainModelsResponseTickerBody) SetEps(v string)`

SetEps sets Eps field to given value.

### HasEps

`func (o *PiMarketDataDomainModelsResponseTickerBody) HasEps() bool`

HasEps returns a boolean if a field has been set.

### SetEpsNil

`func (o *PiMarketDataDomainModelsResponseTickerBody) SetEpsNil(b bool)`

 SetEpsNil sets the value for Eps to be an explicit nil

### UnsetEps
`func (o *PiMarketDataDomainModelsResponseTickerBody) UnsetEps()`

UnsetEps ensures that no value is present for Eps, not even an explicit nil
### GetPublicTrades

`func (o *PiMarketDataDomainModelsResponseTickerBody) GetPublicTrades() [][]interface{}`

GetPublicTrades returns the PublicTrades field if non-nil, zero value otherwise.

### GetPublicTradesOk

`func (o *PiMarketDataDomainModelsResponseTickerBody) GetPublicTradesOk() (*[][]interface{}, bool)`

GetPublicTradesOk returns a tuple with the PublicTrades field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetPublicTrades

`func (o *PiMarketDataDomainModelsResponseTickerBody) SetPublicTrades(v [][]interface{})`

SetPublicTrades sets PublicTrades field to given value.

### HasPublicTrades

`func (o *PiMarketDataDomainModelsResponseTickerBody) HasPublicTrades() bool`

HasPublicTrades returns a boolean if a field has been set.

### SetPublicTradesNil

`func (o *PiMarketDataDomainModelsResponseTickerBody) SetPublicTradesNil(b bool)`

 SetPublicTradesNil sets the value for PublicTrades to be an explicit nil

### UnsetPublicTrades
`func (o *PiMarketDataDomainModelsResponseTickerBody) UnsetPublicTrades()`

UnsetPublicTrades ensures that no value is present for PublicTrades, not even an explicit nil
### GetOrderBook

`func (o *PiMarketDataDomainModelsResponseTickerBody) GetOrderBook() PiMarketDataDomainModelsResponseTickerOrderBook`

GetOrderBook returns the OrderBook field if non-nil, zero value otherwise.

### GetOrderBookOk

`func (o *PiMarketDataDomainModelsResponseTickerBody) GetOrderBookOk() (*PiMarketDataDomainModelsResponseTickerOrderBook, bool)`

GetOrderBookOk returns a tuple with the OrderBook field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetOrderBook

`func (o *PiMarketDataDomainModelsResponseTickerBody) SetOrderBook(v PiMarketDataDomainModelsResponseTickerOrderBook)`

SetOrderBook sets OrderBook field to given value.

### HasOrderBook

`func (o *PiMarketDataDomainModelsResponseTickerBody) HasOrderBook() bool`

HasOrderBook returns a boolean if a field has been set.

### GetSecurityType

`func (o *PiMarketDataDomainModelsResponseTickerBody) GetSecurityType() string`

GetSecurityType returns the SecurityType field if non-nil, zero value otherwise.

### GetSecurityTypeOk

`func (o *PiMarketDataDomainModelsResponseTickerBody) GetSecurityTypeOk() (*string, bool)`

GetSecurityTypeOk returns a tuple with the SecurityType field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetSecurityType

`func (o *PiMarketDataDomainModelsResponseTickerBody) SetSecurityType(v string)`

SetSecurityType sets SecurityType field to given value.

### HasSecurityType

`func (o *PiMarketDataDomainModelsResponseTickerBody) HasSecurityType() bool`

HasSecurityType returns a boolean if a field has been set.

### SetSecurityTypeNil

`func (o *PiMarketDataDomainModelsResponseTickerBody) SetSecurityTypeNil(b bool)`

 SetSecurityTypeNil sets the value for SecurityType to be an explicit nil

### UnsetSecurityType
`func (o *PiMarketDataDomainModelsResponseTickerBody) UnsetSecurityType()`

UnsetSecurityType ensures that no value is present for SecurityType, not even an explicit nil
### GetInstrumentType

`func (o *PiMarketDataDomainModelsResponseTickerBody) GetInstrumentType() string`

GetInstrumentType returns the InstrumentType field if non-nil, zero value otherwise.

### GetInstrumentTypeOk

`func (o *PiMarketDataDomainModelsResponseTickerBody) GetInstrumentTypeOk() (*string, bool)`

GetInstrumentTypeOk returns a tuple with the InstrumentType field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetInstrumentType

`func (o *PiMarketDataDomainModelsResponseTickerBody) SetInstrumentType(v string)`

SetInstrumentType sets InstrumentType field to given value.

### HasInstrumentType

`func (o *PiMarketDataDomainModelsResponseTickerBody) HasInstrumentType() bool`

HasInstrumentType returns a boolean if a field has been set.

### SetInstrumentTypeNil

`func (o *PiMarketDataDomainModelsResponseTickerBody) SetInstrumentTypeNil(b bool)`

 SetInstrumentTypeNil sets the value for InstrumentType to be an explicit nil

### UnsetInstrumentType
`func (o *PiMarketDataDomainModelsResponseTickerBody) UnsetInstrumentType()`

UnsetInstrumentType ensures that no value is present for InstrumentType, not even an explicit nil
### GetMarket

`func (o *PiMarketDataDomainModelsResponseTickerBody) GetMarket() string`

GetMarket returns the Market field if non-nil, zero value otherwise.

### GetMarketOk

`func (o *PiMarketDataDomainModelsResponseTickerBody) GetMarketOk() (*string, bool)`

GetMarketOk returns a tuple with the Market field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetMarket

`func (o *PiMarketDataDomainModelsResponseTickerBody) SetMarket(v string)`

SetMarket sets Market field to given value.

### HasMarket

`func (o *PiMarketDataDomainModelsResponseTickerBody) HasMarket() bool`

HasMarket returns a boolean if a field has been set.

### SetMarketNil

`func (o *PiMarketDataDomainModelsResponseTickerBody) SetMarketNil(b bool)`

 SetMarketNil sets the value for Market to be an explicit nil

### UnsetMarket
`func (o *PiMarketDataDomainModelsResponseTickerBody) UnsetMarket()`

UnsetMarket ensures that no value is present for Market, not even an explicit nil
### GetLastTrade

`func (o *PiMarketDataDomainModelsResponseTickerBody) GetLastTrade() string`

GetLastTrade returns the LastTrade field if non-nil, zero value otherwise.

### GetLastTradeOk

`func (o *PiMarketDataDomainModelsResponseTickerBody) GetLastTradeOk() (*string, bool)`

GetLastTradeOk returns a tuple with the LastTrade field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetLastTrade

`func (o *PiMarketDataDomainModelsResponseTickerBody) SetLastTrade(v string)`

SetLastTrade sets LastTrade field to given value.

### HasLastTrade

`func (o *PiMarketDataDomainModelsResponseTickerBody) HasLastTrade() bool`

HasLastTrade returns a boolean if a field has been set.

### SetLastTradeNil

`func (o *PiMarketDataDomainModelsResponseTickerBody) SetLastTradeNil(b bool)`

 SetLastTradeNil sets the value for LastTrade to be an explicit nil

### UnsetLastTrade
`func (o *PiMarketDataDomainModelsResponseTickerBody) UnsetLastTrade()`

UnsetLastTrade ensures that no value is present for LastTrade, not even an explicit nil
### GetToLastTrade

`func (o *PiMarketDataDomainModelsResponseTickerBody) GetToLastTrade() int32`

GetToLastTrade returns the ToLastTrade field if non-nil, zero value otherwise.

### GetToLastTradeOk

`func (o *PiMarketDataDomainModelsResponseTickerBody) GetToLastTradeOk() (*int32, bool)`

GetToLastTradeOk returns a tuple with the ToLastTrade field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetToLastTrade

`func (o *PiMarketDataDomainModelsResponseTickerBody) SetToLastTrade(v int32)`

SetToLastTrade sets ToLastTrade field to given value.

### HasToLastTrade

`func (o *PiMarketDataDomainModelsResponseTickerBody) HasToLastTrade() bool`

HasToLastTrade returns a boolean if a field has been set.

### GetMoneyness

`func (o *PiMarketDataDomainModelsResponseTickerBody) GetMoneyness() string`

GetMoneyness returns the Moneyness field if non-nil, zero value otherwise.

### GetMoneynessOk

`func (o *PiMarketDataDomainModelsResponseTickerBody) GetMoneynessOk() (*string, bool)`

GetMoneynessOk returns a tuple with the Moneyness field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetMoneyness

`func (o *PiMarketDataDomainModelsResponseTickerBody) SetMoneyness(v string)`

SetMoneyness sets Moneyness field to given value.

### HasMoneyness

`func (o *PiMarketDataDomainModelsResponseTickerBody) HasMoneyness() bool`

HasMoneyness returns a boolean if a field has been set.

### SetMoneynessNil

`func (o *PiMarketDataDomainModelsResponseTickerBody) SetMoneynessNil(b bool)`

 SetMoneynessNil sets the value for Moneyness to be an explicit nil

### UnsetMoneyness
`func (o *PiMarketDataDomainModelsResponseTickerBody) UnsetMoneyness()`

UnsetMoneyness ensures that no value is present for Moneyness, not even an explicit nil
### GetMaturityDate

`func (o *PiMarketDataDomainModelsResponseTickerBody) GetMaturityDate() string`

GetMaturityDate returns the MaturityDate field if non-nil, zero value otherwise.

### GetMaturityDateOk

`func (o *PiMarketDataDomainModelsResponseTickerBody) GetMaturityDateOk() (*string, bool)`

GetMaturityDateOk returns a tuple with the MaturityDate field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetMaturityDate

`func (o *PiMarketDataDomainModelsResponseTickerBody) SetMaturityDate(v string)`

SetMaturityDate sets MaturityDate field to given value.

### HasMaturityDate

`func (o *PiMarketDataDomainModelsResponseTickerBody) HasMaturityDate() bool`

HasMaturityDate returns a boolean if a field has been set.

### SetMaturityDateNil

`func (o *PiMarketDataDomainModelsResponseTickerBody) SetMaturityDateNil(b bool)`

 SetMaturityDateNil sets the value for MaturityDate to be an explicit nil

### UnsetMaturityDate
`func (o *PiMarketDataDomainModelsResponseTickerBody) UnsetMaturityDate()`

UnsetMaturityDate ensures that no value is present for MaturityDate, not even an explicit nil
### GetMultiplier

`func (o *PiMarketDataDomainModelsResponseTickerBody) GetMultiplier() string`

GetMultiplier returns the Multiplier field if non-nil, zero value otherwise.

### GetMultiplierOk

`func (o *PiMarketDataDomainModelsResponseTickerBody) GetMultiplierOk() (*string, bool)`

GetMultiplierOk returns a tuple with the Multiplier field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetMultiplier

`func (o *PiMarketDataDomainModelsResponseTickerBody) SetMultiplier(v string)`

SetMultiplier sets Multiplier field to given value.

### HasMultiplier

`func (o *PiMarketDataDomainModelsResponseTickerBody) HasMultiplier() bool`

HasMultiplier returns a boolean if a field has been set.

### SetMultiplierNil

`func (o *PiMarketDataDomainModelsResponseTickerBody) SetMultiplierNil(b bool)`

 SetMultiplierNil sets the value for Multiplier to be an explicit nil

### UnsetMultiplier
`func (o *PiMarketDataDomainModelsResponseTickerBody) UnsetMultiplier()`

UnsetMultiplier ensures that no value is present for Multiplier, not even an explicit nil
### GetExercisePrice

`func (o *PiMarketDataDomainModelsResponseTickerBody) GetExercisePrice() string`

GetExercisePrice returns the ExercisePrice field if non-nil, zero value otherwise.

### GetExercisePriceOk

`func (o *PiMarketDataDomainModelsResponseTickerBody) GetExercisePriceOk() (*string, bool)`

GetExercisePriceOk returns a tuple with the ExercisePrice field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetExercisePrice

`func (o *PiMarketDataDomainModelsResponseTickerBody) SetExercisePrice(v string)`

SetExercisePrice sets ExercisePrice field to given value.

### HasExercisePrice

`func (o *PiMarketDataDomainModelsResponseTickerBody) HasExercisePrice() bool`

HasExercisePrice returns a boolean if a field has been set.

### SetExercisePriceNil

`func (o *PiMarketDataDomainModelsResponseTickerBody) SetExercisePriceNil(b bool)`

 SetExercisePriceNil sets the value for ExercisePrice to be an explicit nil

### UnsetExercisePrice
`func (o *PiMarketDataDomainModelsResponseTickerBody) UnsetExercisePrice()`

UnsetExercisePrice ensures that no value is present for ExercisePrice, not even an explicit nil
### GetIntrinsicValue

`func (o *PiMarketDataDomainModelsResponseTickerBody) GetIntrinsicValue() string`

GetIntrinsicValue returns the IntrinsicValue field if non-nil, zero value otherwise.

### GetIntrinsicValueOk

`func (o *PiMarketDataDomainModelsResponseTickerBody) GetIntrinsicValueOk() (*string, bool)`

GetIntrinsicValueOk returns a tuple with the IntrinsicValue field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetIntrinsicValue

`func (o *PiMarketDataDomainModelsResponseTickerBody) SetIntrinsicValue(v string)`

SetIntrinsicValue sets IntrinsicValue field to given value.

### HasIntrinsicValue

`func (o *PiMarketDataDomainModelsResponseTickerBody) HasIntrinsicValue() bool`

HasIntrinsicValue returns a boolean if a field has been set.

### SetIntrinsicValueNil

`func (o *PiMarketDataDomainModelsResponseTickerBody) SetIntrinsicValueNil(b bool)`

 SetIntrinsicValueNil sets the value for IntrinsicValue to be an explicit nil

### UnsetIntrinsicValue
`func (o *PiMarketDataDomainModelsResponseTickerBody) UnsetIntrinsicValue()`

UnsetIntrinsicValue ensures that no value is present for IntrinsicValue, not even an explicit nil
### GetPSettle

`func (o *PiMarketDataDomainModelsResponseTickerBody) GetPSettle() string`

GetPSettle returns the PSettle field if non-nil, zero value otherwise.

### GetPSettleOk

`func (o *PiMarketDataDomainModelsResponseTickerBody) GetPSettleOk() (*string, bool)`

GetPSettleOk returns a tuple with the PSettle field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetPSettle

`func (o *PiMarketDataDomainModelsResponseTickerBody) SetPSettle(v string)`

SetPSettle sets PSettle field to given value.

### HasPSettle

`func (o *PiMarketDataDomainModelsResponseTickerBody) HasPSettle() bool`

HasPSettle returns a boolean if a field has been set.

### SetPSettleNil

`func (o *PiMarketDataDomainModelsResponseTickerBody) SetPSettleNil(b bool)`

 SetPSettleNil sets the value for PSettle to be an explicit nil

### UnsetPSettle
`func (o *PiMarketDataDomainModelsResponseTickerBody) UnsetPSettle()`

UnsetPSettle ensures that no value is present for PSettle, not even an explicit nil
### GetPoi

`func (o *PiMarketDataDomainModelsResponseTickerBody) GetPoi() string`

GetPoi returns the Poi field if non-nil, zero value otherwise.

### GetPoiOk

`func (o *PiMarketDataDomainModelsResponseTickerBody) GetPoiOk() (*string, bool)`

GetPoiOk returns a tuple with the Poi field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetPoi

`func (o *PiMarketDataDomainModelsResponseTickerBody) SetPoi(v string)`

SetPoi sets Poi field to given value.

### HasPoi

`func (o *PiMarketDataDomainModelsResponseTickerBody) HasPoi() bool`

HasPoi returns a boolean if a field has been set.

### SetPoiNil

`func (o *PiMarketDataDomainModelsResponseTickerBody) SetPoiNil(b bool)`

 SetPoiNil sets the value for Poi to be an explicit nil

### UnsetPoi
`func (o *PiMarketDataDomainModelsResponseTickerBody) UnsetPoi()`

UnsetPoi ensures that no value is present for Poi, not even an explicit nil
### GetUnderlying

`func (o *PiMarketDataDomainModelsResponseTickerBody) GetUnderlying() string`

GetUnderlying returns the Underlying field if non-nil, zero value otherwise.

### GetUnderlyingOk

`func (o *PiMarketDataDomainModelsResponseTickerBody) GetUnderlyingOk() (*string, bool)`

GetUnderlyingOk returns a tuple with the Underlying field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetUnderlying

`func (o *PiMarketDataDomainModelsResponseTickerBody) SetUnderlying(v string)`

SetUnderlying sets Underlying field to given value.

### HasUnderlying

`func (o *PiMarketDataDomainModelsResponseTickerBody) HasUnderlying() bool`

HasUnderlying returns a boolean if a field has been set.

### SetUnderlyingNil

`func (o *PiMarketDataDomainModelsResponseTickerBody) SetUnderlyingNil(b bool)`

 SetUnderlyingNil sets the value for Underlying to be an explicit nil

### UnsetUnderlying
`func (o *PiMarketDataDomainModelsResponseTickerBody) UnsetUnderlying()`

UnsetUnderlying ensures that no value is present for Underlying, not even an explicit nil
### GetOpen0

`func (o *PiMarketDataDomainModelsResponseTickerBody) GetOpen0() string`

GetOpen0 returns the Open0 field if non-nil, zero value otherwise.

### GetOpen0Ok

`func (o *PiMarketDataDomainModelsResponseTickerBody) GetOpen0Ok() (*string, bool)`

GetOpen0Ok returns a tuple with the Open0 field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetOpen0

`func (o *PiMarketDataDomainModelsResponseTickerBody) SetOpen0(v string)`

SetOpen0 sets Open0 field to given value.

### HasOpen0

`func (o *PiMarketDataDomainModelsResponseTickerBody) HasOpen0() bool`

HasOpen0 returns a boolean if a field has been set.

### SetOpen0Nil

`func (o *PiMarketDataDomainModelsResponseTickerBody) SetOpen0Nil(b bool)`

 SetOpen0Nil sets the value for Open0 to be an explicit nil

### UnsetOpen0
`func (o *PiMarketDataDomainModelsResponseTickerBody) UnsetOpen0()`

UnsetOpen0 ensures that no value is present for Open0, not even an explicit nil
### GetOpen1

`func (o *PiMarketDataDomainModelsResponseTickerBody) GetOpen1() string`

GetOpen1 returns the Open1 field if non-nil, zero value otherwise.

### GetOpen1Ok

`func (o *PiMarketDataDomainModelsResponseTickerBody) GetOpen1Ok() (*string, bool)`

GetOpen1Ok returns a tuple with the Open1 field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetOpen1

`func (o *PiMarketDataDomainModelsResponseTickerBody) SetOpen1(v string)`

SetOpen1 sets Open1 field to given value.

### HasOpen1

`func (o *PiMarketDataDomainModelsResponseTickerBody) HasOpen1() bool`

HasOpen1 returns a boolean if a field has been set.

### SetOpen1Nil

`func (o *PiMarketDataDomainModelsResponseTickerBody) SetOpen1Nil(b bool)`

 SetOpen1Nil sets the value for Open1 to be an explicit nil

### UnsetOpen1
`func (o *PiMarketDataDomainModelsResponseTickerBody) UnsetOpen1()`

UnsetOpen1 ensures that no value is present for Open1, not even an explicit nil
### GetBasis

`func (o *PiMarketDataDomainModelsResponseTickerBody) GetBasis() string`

GetBasis returns the Basis field if non-nil, zero value otherwise.

### GetBasisOk

`func (o *PiMarketDataDomainModelsResponseTickerBody) GetBasisOk() (*string, bool)`

GetBasisOk returns a tuple with the Basis field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetBasis

`func (o *PiMarketDataDomainModelsResponseTickerBody) SetBasis(v string)`

SetBasis sets Basis field to given value.

### HasBasis

`func (o *PiMarketDataDomainModelsResponseTickerBody) HasBasis() bool`

HasBasis returns a boolean if a field has been set.

### SetBasisNil

`func (o *PiMarketDataDomainModelsResponseTickerBody) SetBasisNil(b bool)`

 SetBasisNil sets the value for Basis to be an explicit nil

### UnsetBasis
`func (o *PiMarketDataDomainModelsResponseTickerBody) UnsetBasis()`

UnsetBasis ensures that no value is present for Basis, not even an explicit nil
### GetSettle

`func (o *PiMarketDataDomainModelsResponseTickerBody) GetSettle() string`

GetSettle returns the Settle field if non-nil, zero value otherwise.

### GetSettleOk

`func (o *PiMarketDataDomainModelsResponseTickerBody) GetSettleOk() (*string, bool)`

GetSettleOk returns a tuple with the Settle field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetSettle

`func (o *PiMarketDataDomainModelsResponseTickerBody) SetSettle(v string)`

SetSettle sets Settle field to given value.

### HasSettle

`func (o *PiMarketDataDomainModelsResponseTickerBody) HasSettle() bool`

HasSettle returns a boolean if a field has been set.

### SetSettleNil

`func (o *PiMarketDataDomainModelsResponseTickerBody) SetSettleNil(b bool)`

 SetSettleNil sets the value for Settle to be an explicit nil

### UnsetSettle
`func (o *PiMarketDataDomainModelsResponseTickerBody) UnsetSettle()`

UnsetSettle ensures that no value is present for Settle, not even an explicit nil
### GetInstrumentCategory

`func (o *PiMarketDataDomainModelsResponseTickerBody) GetInstrumentCategory() string`

GetInstrumentCategory returns the InstrumentCategory field if non-nil, zero value otherwise.

### GetInstrumentCategoryOk

`func (o *PiMarketDataDomainModelsResponseTickerBody) GetInstrumentCategoryOk() (*string, bool)`

GetInstrumentCategoryOk returns a tuple with the InstrumentCategory field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetInstrumentCategory

`func (o *PiMarketDataDomainModelsResponseTickerBody) SetInstrumentCategory(v string)`

SetInstrumentCategory sets InstrumentCategory field to given value.

### HasInstrumentCategory

`func (o *PiMarketDataDomainModelsResponseTickerBody) HasInstrumentCategory() bool`

HasInstrumentCategory returns a boolean if a field has been set.

### SetInstrumentCategoryNil

`func (o *PiMarketDataDomainModelsResponseTickerBody) SetInstrumentCategoryNil(b bool)`

 SetInstrumentCategoryNil sets the value for InstrumentCategory to be an explicit nil

### UnsetInstrumentCategory
`func (o *PiMarketDataDomainModelsResponseTickerBody) UnsetInstrumentCategory()`

UnsetInstrumentCategory ensures that no value is present for InstrumentCategory, not even an explicit nil
### GetFriendlyName

`func (o *PiMarketDataDomainModelsResponseTickerBody) GetFriendlyName() string`

GetFriendlyName returns the FriendlyName field if non-nil, zero value otherwise.

### GetFriendlyNameOk

`func (o *PiMarketDataDomainModelsResponseTickerBody) GetFriendlyNameOk() (*string, bool)`

GetFriendlyNameOk returns a tuple with the FriendlyName field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetFriendlyName

`func (o *PiMarketDataDomainModelsResponseTickerBody) SetFriendlyName(v string)`

SetFriendlyName sets FriendlyName field to given value.

### HasFriendlyName

`func (o *PiMarketDataDomainModelsResponseTickerBody) HasFriendlyName() bool`

HasFriendlyName returns a boolean if a field has been set.

### SetFriendlyNameNil

`func (o *PiMarketDataDomainModelsResponseTickerBody) SetFriendlyNameNil(b bool)`

 SetFriendlyNameNil sets the value for FriendlyName to be an explicit nil

### UnsetFriendlyName
`func (o *PiMarketDataDomainModelsResponseTickerBody) UnsetFriendlyName()`

UnsetFriendlyName ensures that no value is present for FriendlyName, not even an explicit nil
### GetLogo

`func (o *PiMarketDataDomainModelsResponseTickerBody) GetLogo() string`

GetLogo returns the Logo field if non-nil, zero value otherwise.

### GetLogoOk

`func (o *PiMarketDataDomainModelsResponseTickerBody) GetLogoOk() (*string, bool)`

GetLogoOk returns a tuple with the Logo field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetLogo

`func (o *PiMarketDataDomainModelsResponseTickerBody) SetLogo(v string)`

SetLogo sets Logo field to given value.

### HasLogo

`func (o *PiMarketDataDomainModelsResponseTickerBody) HasLogo() bool`

HasLogo returns a boolean if a field has been set.

### SetLogoNil

`func (o *PiMarketDataDomainModelsResponseTickerBody) SetLogoNil(b bool)`

 SetLogoNil sets the value for Logo to be an explicit nil

### UnsetLogo
`func (o *PiMarketDataDomainModelsResponseTickerBody) UnsetLogo()`

UnsetLogo ensures that no value is present for Logo, not even an explicit nil
### GetExchangeTimezone

`func (o *PiMarketDataDomainModelsResponseTickerBody) GetExchangeTimezone() string`

GetExchangeTimezone returns the ExchangeTimezone field if non-nil, zero value otherwise.

### GetExchangeTimezoneOk

`func (o *PiMarketDataDomainModelsResponseTickerBody) GetExchangeTimezoneOk() (*string, bool)`

GetExchangeTimezoneOk returns a tuple with the ExchangeTimezone field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetExchangeTimezone

`func (o *PiMarketDataDomainModelsResponseTickerBody) SetExchangeTimezone(v string)`

SetExchangeTimezone sets ExchangeTimezone field to given value.

### HasExchangeTimezone

`func (o *PiMarketDataDomainModelsResponseTickerBody) HasExchangeTimezone() bool`

HasExchangeTimezone returns a boolean if a field has been set.

### SetExchangeTimezoneNil

`func (o *PiMarketDataDomainModelsResponseTickerBody) SetExchangeTimezoneNil(b bool)`

 SetExchangeTimezoneNil sets the value for ExchangeTimezone to be an explicit nil

### UnsetExchangeTimezone
`func (o *PiMarketDataDomainModelsResponseTickerBody) UnsetExchangeTimezone()`

UnsetExchangeTimezone ensures that no value is present for ExchangeTimezone, not even an explicit nil
### GetCountry

`func (o *PiMarketDataDomainModelsResponseTickerBody) GetCountry() string`

GetCountry returns the Country field if non-nil, zero value otherwise.

### GetCountryOk

`func (o *PiMarketDataDomainModelsResponseTickerBody) GetCountryOk() (*string, bool)`

GetCountryOk returns a tuple with the Country field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetCountry

`func (o *PiMarketDataDomainModelsResponseTickerBody) SetCountry(v string)`

SetCountry sets Country field to given value.

### HasCountry

`func (o *PiMarketDataDomainModelsResponseTickerBody) HasCountry() bool`

HasCountry returns a boolean if a field has been set.

### SetCountryNil

`func (o *PiMarketDataDomainModelsResponseTickerBody) SetCountryNil(b bool)`

 SetCountryNil sets the value for Country to be an explicit nil

### UnsetCountry
`func (o *PiMarketDataDomainModelsResponseTickerBody) UnsetCountry()`

UnsetCountry ensures that no value is present for Country, not even an explicit nil
### GetOffsetSeconds

`func (o *PiMarketDataDomainModelsResponseTickerBody) GetOffsetSeconds() int32`

GetOffsetSeconds returns the OffsetSeconds field if non-nil, zero value otherwise.

### GetOffsetSecondsOk

`func (o *PiMarketDataDomainModelsResponseTickerBody) GetOffsetSecondsOk() (*int32, bool)`

GetOffsetSecondsOk returns a tuple with the OffsetSeconds field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetOffsetSeconds

`func (o *PiMarketDataDomainModelsResponseTickerBody) SetOffsetSeconds(v int32)`

SetOffsetSeconds sets OffsetSeconds field to given value.

### HasOffsetSeconds

`func (o *PiMarketDataDomainModelsResponseTickerBody) HasOffsetSeconds() bool`

HasOffsetSeconds returns a boolean if a field has been set.

### GetIsProjected

`func (o *PiMarketDataDomainModelsResponseTickerBody) GetIsProjected() bool`

GetIsProjected returns the IsProjected field if non-nil, zero value otherwise.

### GetIsProjectedOk

`func (o *PiMarketDataDomainModelsResponseTickerBody) GetIsProjectedOk() (*bool, bool)`

GetIsProjectedOk returns a tuple with the IsProjected field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetIsProjected

`func (o *PiMarketDataDomainModelsResponseTickerBody) SetIsProjected(v bool)`

SetIsProjected sets IsProjected field to given value.

### HasIsProjected

`func (o *PiMarketDataDomainModelsResponseTickerBody) HasIsProjected() bool`

HasIsProjected returns a boolean if a field has been set.

### GetLastPriceTime

`func (o *PiMarketDataDomainModelsResponseTickerBody) GetLastPriceTime() int64`

GetLastPriceTime returns the LastPriceTime field if non-nil, zero value otherwise.

### GetLastPriceTimeOk

`func (o *PiMarketDataDomainModelsResponseTickerBody) GetLastPriceTimeOk() (*int64, bool)`

GetLastPriceTimeOk returns a tuple with the LastPriceTime field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetLastPriceTime

`func (o *PiMarketDataDomainModelsResponseTickerBody) SetLastPriceTime(v int64)`

SetLastPriceTime sets LastPriceTime field to given value.

### HasLastPriceTime

`func (o *PiMarketDataDomainModelsResponseTickerBody) HasLastPriceTime() bool`

HasLastPriceTime returns a boolean if a field has been set.


[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


