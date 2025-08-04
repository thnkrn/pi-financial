# OrderResponse

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**Id** | Pointer to **NullableString** |  | [optional] 
**AccountId** | Pointer to **NullableString** |  | [optional] 
**Venue** | Pointer to **NullableString** |  | [optional] 
**Symbol** | Pointer to **NullableString** |  | [optional] 
**SymbolId** | Pointer to **NullableString** |  | [optional] [readonly] 
**OrderType** | Pointer to [**OrderType**](OrderType.md) |  | [optional] 
**Side** | Pointer to [**OrderSide**](OrderSide.md) |  | [optional] 
**Status** | Pointer to [**OrderStatus**](OrderStatus.md) |  | [optional] 
**Currency** | Pointer to [**Currency**](Currency.md) |  | [optional] 
**LimitPrice** | Pointer to **NullableFloat32** |  | [optional] 
**StopPrice** | Pointer to **NullableFloat32** |  | [optional] 
**AverageFillPrice** | Pointer to **NullableFloat32** |  | [optional] 
**Quantity** | Pointer to **float32** |  | [optional] 
**QuantityFilled** | Pointer to **float32** |  | [optional] 
**QuantityCancelled** | Pointer to **float32** |  | [optional] 
**Provider** | Pointer to [**Provider**](Provider.md) |  | [optional] 
**StatusReason** | Pointer to [**OrderReason**](OrderReason.md) |  | [optional] 
**AsOfDate** | Pointer to **time.Time** |  | [optional] [readonly] 
**CreatedAt** | Pointer to **time.Time** |  | [optional] 
**UpdatedAt** | Pointer to **time.Time** |  | [optional] 
**Logo** | Pointer to **NullableString** |  | [optional] [readonly] 
**TransactionInfo** | Pointer to [**OrderTransactionResponse**](OrderTransactionResponse.md) |  | [optional] 

## Methods

### NewOrderResponse

`func NewOrderResponse() *OrderResponse`

NewOrderResponse instantiates a new OrderResponse object
This constructor will assign default values to properties that have it defined,
and makes sure properties required by API are set, but the set of arguments
will change when the set of required properties is changed

### NewOrderResponseWithDefaults

`func NewOrderResponseWithDefaults() *OrderResponse`

NewOrderResponseWithDefaults instantiates a new OrderResponse object
This constructor will only assign default values to properties that have it defined,
but it doesn't guarantee that properties required by API are set

### GetId

`func (o *OrderResponse) GetId() string`

GetId returns the Id field if non-nil, zero value otherwise.

### GetIdOk

`func (o *OrderResponse) GetIdOk() (*string, bool)`

GetIdOk returns a tuple with the Id field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetId

`func (o *OrderResponse) SetId(v string)`

SetId sets Id field to given value.

### HasId

`func (o *OrderResponse) HasId() bool`

HasId returns a boolean if a field has been set.

### SetIdNil

`func (o *OrderResponse) SetIdNil(b bool)`

 SetIdNil sets the value for Id to be an explicit nil

### UnsetId
`func (o *OrderResponse) UnsetId()`

UnsetId ensures that no value is present for Id, not even an explicit nil
### GetAccountId

`func (o *OrderResponse) GetAccountId() string`

GetAccountId returns the AccountId field if non-nil, zero value otherwise.

### GetAccountIdOk

`func (o *OrderResponse) GetAccountIdOk() (*string, bool)`

GetAccountIdOk returns a tuple with the AccountId field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetAccountId

`func (o *OrderResponse) SetAccountId(v string)`

SetAccountId sets AccountId field to given value.

### HasAccountId

`func (o *OrderResponse) HasAccountId() bool`

HasAccountId returns a boolean if a field has been set.

### SetAccountIdNil

`func (o *OrderResponse) SetAccountIdNil(b bool)`

 SetAccountIdNil sets the value for AccountId to be an explicit nil

### UnsetAccountId
`func (o *OrderResponse) UnsetAccountId()`

UnsetAccountId ensures that no value is present for AccountId, not even an explicit nil
### GetVenue

`func (o *OrderResponse) GetVenue() string`

GetVenue returns the Venue field if non-nil, zero value otherwise.

### GetVenueOk

`func (o *OrderResponse) GetVenueOk() (*string, bool)`

GetVenueOk returns a tuple with the Venue field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetVenue

`func (o *OrderResponse) SetVenue(v string)`

SetVenue sets Venue field to given value.

### HasVenue

`func (o *OrderResponse) HasVenue() bool`

HasVenue returns a boolean if a field has been set.

### SetVenueNil

`func (o *OrderResponse) SetVenueNil(b bool)`

 SetVenueNil sets the value for Venue to be an explicit nil

### UnsetVenue
`func (o *OrderResponse) UnsetVenue()`

UnsetVenue ensures that no value is present for Venue, not even an explicit nil
### GetSymbol

`func (o *OrderResponse) GetSymbol() string`

GetSymbol returns the Symbol field if non-nil, zero value otherwise.

### GetSymbolOk

`func (o *OrderResponse) GetSymbolOk() (*string, bool)`

GetSymbolOk returns a tuple with the Symbol field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetSymbol

`func (o *OrderResponse) SetSymbol(v string)`

SetSymbol sets Symbol field to given value.

### HasSymbol

`func (o *OrderResponse) HasSymbol() bool`

HasSymbol returns a boolean if a field has been set.

### SetSymbolNil

`func (o *OrderResponse) SetSymbolNil(b bool)`

 SetSymbolNil sets the value for Symbol to be an explicit nil

### UnsetSymbol
`func (o *OrderResponse) UnsetSymbol()`

UnsetSymbol ensures that no value is present for Symbol, not even an explicit nil
### GetSymbolId

`func (o *OrderResponse) GetSymbolId() string`

GetSymbolId returns the SymbolId field if non-nil, zero value otherwise.

### GetSymbolIdOk

`func (o *OrderResponse) GetSymbolIdOk() (*string, bool)`

GetSymbolIdOk returns a tuple with the SymbolId field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetSymbolId

`func (o *OrderResponse) SetSymbolId(v string)`

SetSymbolId sets SymbolId field to given value.

### HasSymbolId

`func (o *OrderResponse) HasSymbolId() bool`

HasSymbolId returns a boolean if a field has been set.

### SetSymbolIdNil

`func (o *OrderResponse) SetSymbolIdNil(b bool)`

 SetSymbolIdNil sets the value for SymbolId to be an explicit nil

### UnsetSymbolId
`func (o *OrderResponse) UnsetSymbolId()`

UnsetSymbolId ensures that no value is present for SymbolId, not even an explicit nil
### GetOrderType

`func (o *OrderResponse) GetOrderType() OrderType`

GetOrderType returns the OrderType field if non-nil, zero value otherwise.

### GetOrderTypeOk

`func (o *OrderResponse) GetOrderTypeOk() (*OrderType, bool)`

GetOrderTypeOk returns a tuple with the OrderType field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetOrderType

`func (o *OrderResponse) SetOrderType(v OrderType)`

SetOrderType sets OrderType field to given value.

### HasOrderType

`func (o *OrderResponse) HasOrderType() bool`

HasOrderType returns a boolean if a field has been set.

### GetSide

`func (o *OrderResponse) GetSide() OrderSide`

GetSide returns the Side field if non-nil, zero value otherwise.

### GetSideOk

`func (o *OrderResponse) GetSideOk() (*OrderSide, bool)`

GetSideOk returns a tuple with the Side field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetSide

`func (o *OrderResponse) SetSide(v OrderSide)`

SetSide sets Side field to given value.

### HasSide

`func (o *OrderResponse) HasSide() bool`

HasSide returns a boolean if a field has been set.

### GetStatus

`func (o *OrderResponse) GetStatus() OrderStatus`

GetStatus returns the Status field if non-nil, zero value otherwise.

### GetStatusOk

`func (o *OrderResponse) GetStatusOk() (*OrderStatus, bool)`

GetStatusOk returns a tuple with the Status field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetStatus

`func (o *OrderResponse) SetStatus(v OrderStatus)`

SetStatus sets Status field to given value.

### HasStatus

`func (o *OrderResponse) HasStatus() bool`

HasStatus returns a boolean if a field has been set.

### GetCurrency

`func (o *OrderResponse) GetCurrency() Currency`

GetCurrency returns the Currency field if non-nil, zero value otherwise.

### GetCurrencyOk

`func (o *OrderResponse) GetCurrencyOk() (*Currency, bool)`

GetCurrencyOk returns a tuple with the Currency field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetCurrency

`func (o *OrderResponse) SetCurrency(v Currency)`

SetCurrency sets Currency field to given value.

### HasCurrency

`func (o *OrderResponse) HasCurrency() bool`

HasCurrency returns a boolean if a field has been set.

### GetLimitPrice

`func (o *OrderResponse) GetLimitPrice() float32`

GetLimitPrice returns the LimitPrice field if non-nil, zero value otherwise.

### GetLimitPriceOk

`func (o *OrderResponse) GetLimitPriceOk() (*float32, bool)`

GetLimitPriceOk returns a tuple with the LimitPrice field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetLimitPrice

`func (o *OrderResponse) SetLimitPrice(v float32)`

SetLimitPrice sets LimitPrice field to given value.

### HasLimitPrice

`func (o *OrderResponse) HasLimitPrice() bool`

HasLimitPrice returns a boolean if a field has been set.

### SetLimitPriceNil

`func (o *OrderResponse) SetLimitPriceNil(b bool)`

 SetLimitPriceNil sets the value for LimitPrice to be an explicit nil

### UnsetLimitPrice
`func (o *OrderResponse) UnsetLimitPrice()`

UnsetLimitPrice ensures that no value is present for LimitPrice, not even an explicit nil
### GetStopPrice

`func (o *OrderResponse) GetStopPrice() float32`

GetStopPrice returns the StopPrice field if non-nil, zero value otherwise.

### GetStopPriceOk

`func (o *OrderResponse) GetStopPriceOk() (*float32, bool)`

GetStopPriceOk returns a tuple with the StopPrice field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetStopPrice

`func (o *OrderResponse) SetStopPrice(v float32)`

SetStopPrice sets StopPrice field to given value.

### HasStopPrice

`func (o *OrderResponse) HasStopPrice() bool`

HasStopPrice returns a boolean if a field has been set.

### SetStopPriceNil

`func (o *OrderResponse) SetStopPriceNil(b bool)`

 SetStopPriceNil sets the value for StopPrice to be an explicit nil

### UnsetStopPrice
`func (o *OrderResponse) UnsetStopPrice()`

UnsetStopPrice ensures that no value is present for StopPrice, not even an explicit nil
### GetAverageFillPrice

`func (o *OrderResponse) GetAverageFillPrice() float32`

GetAverageFillPrice returns the AverageFillPrice field if non-nil, zero value otherwise.

### GetAverageFillPriceOk

`func (o *OrderResponse) GetAverageFillPriceOk() (*float32, bool)`

GetAverageFillPriceOk returns a tuple with the AverageFillPrice field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetAverageFillPrice

`func (o *OrderResponse) SetAverageFillPrice(v float32)`

SetAverageFillPrice sets AverageFillPrice field to given value.

### HasAverageFillPrice

`func (o *OrderResponse) HasAverageFillPrice() bool`

HasAverageFillPrice returns a boolean if a field has been set.

### SetAverageFillPriceNil

`func (o *OrderResponse) SetAverageFillPriceNil(b bool)`

 SetAverageFillPriceNil sets the value for AverageFillPrice to be an explicit nil

### UnsetAverageFillPrice
`func (o *OrderResponse) UnsetAverageFillPrice()`

UnsetAverageFillPrice ensures that no value is present for AverageFillPrice, not even an explicit nil
### GetQuantity

`func (o *OrderResponse) GetQuantity() float32`

GetQuantity returns the Quantity field if non-nil, zero value otherwise.

### GetQuantityOk

`func (o *OrderResponse) GetQuantityOk() (*float32, bool)`

GetQuantityOk returns a tuple with the Quantity field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetQuantity

`func (o *OrderResponse) SetQuantity(v float32)`

SetQuantity sets Quantity field to given value.

### HasQuantity

`func (o *OrderResponse) HasQuantity() bool`

HasQuantity returns a boolean if a field has been set.

### GetQuantityFilled

`func (o *OrderResponse) GetQuantityFilled() float32`

GetQuantityFilled returns the QuantityFilled field if non-nil, zero value otherwise.

### GetQuantityFilledOk

`func (o *OrderResponse) GetQuantityFilledOk() (*float32, bool)`

GetQuantityFilledOk returns a tuple with the QuantityFilled field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetQuantityFilled

`func (o *OrderResponse) SetQuantityFilled(v float32)`

SetQuantityFilled sets QuantityFilled field to given value.

### HasQuantityFilled

`func (o *OrderResponse) HasQuantityFilled() bool`

HasQuantityFilled returns a boolean if a field has been set.

### GetQuantityCancelled

`func (o *OrderResponse) GetQuantityCancelled() float32`

GetQuantityCancelled returns the QuantityCancelled field if non-nil, zero value otherwise.

### GetQuantityCancelledOk

`func (o *OrderResponse) GetQuantityCancelledOk() (*float32, bool)`

GetQuantityCancelledOk returns a tuple with the QuantityCancelled field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetQuantityCancelled

`func (o *OrderResponse) SetQuantityCancelled(v float32)`

SetQuantityCancelled sets QuantityCancelled field to given value.

### HasQuantityCancelled

`func (o *OrderResponse) HasQuantityCancelled() bool`

HasQuantityCancelled returns a boolean if a field has been set.

### GetProvider

`func (o *OrderResponse) GetProvider() Provider`

GetProvider returns the Provider field if non-nil, zero value otherwise.

### GetProviderOk

`func (o *OrderResponse) GetProviderOk() (*Provider, bool)`

GetProviderOk returns a tuple with the Provider field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetProvider

`func (o *OrderResponse) SetProvider(v Provider)`

SetProvider sets Provider field to given value.

### HasProvider

`func (o *OrderResponse) HasProvider() bool`

HasProvider returns a boolean if a field has been set.

### GetStatusReason

`func (o *OrderResponse) GetStatusReason() OrderReason`

GetStatusReason returns the StatusReason field if non-nil, zero value otherwise.

### GetStatusReasonOk

`func (o *OrderResponse) GetStatusReasonOk() (*OrderReason, bool)`

GetStatusReasonOk returns a tuple with the StatusReason field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetStatusReason

`func (o *OrderResponse) SetStatusReason(v OrderReason)`

SetStatusReason sets StatusReason field to given value.

### HasStatusReason

`func (o *OrderResponse) HasStatusReason() bool`

HasStatusReason returns a boolean if a field has been set.

### GetAsOfDate

`func (o *OrderResponse) GetAsOfDate() time.Time`

GetAsOfDate returns the AsOfDate field if non-nil, zero value otherwise.

### GetAsOfDateOk

`func (o *OrderResponse) GetAsOfDateOk() (*time.Time, bool)`

GetAsOfDateOk returns a tuple with the AsOfDate field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetAsOfDate

`func (o *OrderResponse) SetAsOfDate(v time.Time)`

SetAsOfDate sets AsOfDate field to given value.

### HasAsOfDate

`func (o *OrderResponse) HasAsOfDate() bool`

HasAsOfDate returns a boolean if a field has been set.

### GetCreatedAt

`func (o *OrderResponse) GetCreatedAt() time.Time`

GetCreatedAt returns the CreatedAt field if non-nil, zero value otherwise.

### GetCreatedAtOk

`func (o *OrderResponse) GetCreatedAtOk() (*time.Time, bool)`

GetCreatedAtOk returns a tuple with the CreatedAt field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetCreatedAt

`func (o *OrderResponse) SetCreatedAt(v time.Time)`

SetCreatedAt sets CreatedAt field to given value.

### HasCreatedAt

`func (o *OrderResponse) HasCreatedAt() bool`

HasCreatedAt returns a boolean if a field has been set.

### GetUpdatedAt

`func (o *OrderResponse) GetUpdatedAt() time.Time`

GetUpdatedAt returns the UpdatedAt field if non-nil, zero value otherwise.

### GetUpdatedAtOk

`func (o *OrderResponse) GetUpdatedAtOk() (*time.Time, bool)`

GetUpdatedAtOk returns a tuple with the UpdatedAt field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetUpdatedAt

`func (o *OrderResponse) SetUpdatedAt(v time.Time)`

SetUpdatedAt sets UpdatedAt field to given value.

### HasUpdatedAt

`func (o *OrderResponse) HasUpdatedAt() bool`

HasUpdatedAt returns a boolean if a field has been set.

### GetLogo

`func (o *OrderResponse) GetLogo() string`

GetLogo returns the Logo field if non-nil, zero value otherwise.

### GetLogoOk

`func (o *OrderResponse) GetLogoOk() (*string, bool)`

GetLogoOk returns a tuple with the Logo field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetLogo

`func (o *OrderResponse) SetLogo(v string)`

SetLogo sets Logo field to given value.

### HasLogo

`func (o *OrderResponse) HasLogo() bool`

HasLogo returns a boolean if a field has been set.

### SetLogoNil

`func (o *OrderResponse) SetLogoNil(b bool)`

 SetLogoNil sets the value for Logo to be an explicit nil

### UnsetLogo
`func (o *OrderResponse) UnsetLogo()`

UnsetLogo ensures that no value is present for Logo, not even an explicit nil
### GetTransactionInfo

`func (o *OrderResponse) GetTransactionInfo() OrderTransactionResponse`

GetTransactionInfo returns the TransactionInfo field if non-nil, zero value otherwise.

### GetTransactionInfoOk

`func (o *OrderResponse) GetTransactionInfoOk() (*OrderTransactionResponse, bool)`

GetTransactionInfoOk returns a tuple with the TransactionInfo field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetTransactionInfo

`func (o *OrderResponse) SetTransactionInfo(v OrderTransactionResponse)`

SetTransactionInfo sets TransactionInfo field to given value.

### HasTransactionInfo

`func (o *OrderResponse) HasTransactionInfo() bool`

HasTransactionInfo returns a boolean if a field has been set.


[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


