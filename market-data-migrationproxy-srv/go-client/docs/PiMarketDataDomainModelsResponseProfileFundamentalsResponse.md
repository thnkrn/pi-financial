# PiMarketDataDomainModelsResponseProfileFundamentalsResponse

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**MarketCapitalization** | Pointer to **NullableString** |  | [optional] 
**ShareFreeFloat** | Pointer to **NullableString** |  | [optional] 
**Industry** | Pointer to **NullableString** |  | [optional] 
**Sector** | Pointer to **NullableString** |  | [optional] 
**Country** | Pointer to **NullableString** |  | [optional] 
**PriceToEarningsRatio** | Pointer to **NullableString** |  | [optional] 
**PriceToBookRatio** | Pointer to **NullableString** |  | [optional] 
**PriceToSalesRatio** | Pointer to **NullableString** |  | [optional] 
**DividendYieldPercentage** | Pointer to **NullableString** |  | [optional] 
**ExDividendDate** | Pointer to **NullableString** |  | [optional] 
**Units** | Pointer to **NullableString** |  | [optional] 
**Source** | Pointer to **NullableString** |  | [optional] 
**UnderlyingSymbol** | Pointer to **NullableString** |  | [optional] 
**IsClickable** | Pointer to **bool** |  | [optional] 
**UnderlyingVenue** | Pointer to **NullableString** |  | [optional] 
**UnderlyingInstrumentType** | Pointer to **NullableString** |  | [optional] 
**UnderlyingInstrumentCategory** | Pointer to **NullableString** |  | [optional] 
**UnderlyingLogo** | Pointer to **NullableString** |  | [optional] 
**ExerciseRatio** | Pointer to **NullableString** |  | [optional] 
**ExercisePrice** | Pointer to **NullableString** |  | [optional] 
**ExerciseDate** | Pointer to **NullableString** |  | [optional] 
**DaysToExercise** | Pointer to **NullableString** |  | [optional] 
**Moneyness** | Pointer to **NullableString** |  | [optional] 
**Direction** | Pointer to **NullableString** |  | [optional] 
**Multiplier** | Pointer to **NullableString** |  | [optional] 
**LastTradingDate** | Pointer to **NullableString** |  | [optional] 
**DaysToLastTrade** | Pointer to **NullableString** |  | [optional] 
**MaturityDate** | Pointer to **NullableString** |  | [optional] 
**IssuerSeries** | Pointer to **NullableString** |  | [optional] 
**ForeignCurrency** | Pointer to **NullableString** |  | [optional] 
**ConversionRatio** | Pointer to **NullableString** |  | [optional] 
**UnderlyingPrice** | Pointer to **NullableString** |  | [optional] 
**Basis** | Pointer to **NullableString** |  | [optional] 
**OpenInterest** | Pointer to **NullableString** |  | [optional] 
**ContractSize** | Pointer to **NullableString** |  | [optional] 
**LastNavPerShare** | Pointer to **NullableString** |  | [optional] 
**Objective** | Pointer to **NullableString** |  | [optional] 
**AssetClassFocus** | Pointer to **NullableString** |  | [optional] 
**ExpenseRatioPercentage** | Pointer to **NullableString** |  | [optional] 
**StrikePrice** | Pointer to **NullableString** |  | [optional] 
**TheoreticalPrice** | Pointer to **NullableString** |  | [optional] 
**IntrinsicValue** | Pointer to **NullableString** |  | [optional] 
**ImpliedVolatilityPercentage** | Pointer to **NullableString** |  | [optional] 

## Methods

### NewPiMarketDataDomainModelsResponseProfileFundamentalsResponse

`func NewPiMarketDataDomainModelsResponseProfileFundamentalsResponse() *PiMarketDataDomainModelsResponseProfileFundamentalsResponse`

NewPiMarketDataDomainModelsResponseProfileFundamentalsResponse instantiates a new PiMarketDataDomainModelsResponseProfileFundamentalsResponse object
This constructor will assign default values to properties that have it defined,
and makes sure properties required by API are set, but the set of arguments
will change when the set of required properties is changed

### NewPiMarketDataDomainModelsResponseProfileFundamentalsResponseWithDefaults

`func NewPiMarketDataDomainModelsResponseProfileFundamentalsResponseWithDefaults() *PiMarketDataDomainModelsResponseProfileFundamentalsResponse`

NewPiMarketDataDomainModelsResponseProfileFundamentalsResponseWithDefaults instantiates a new PiMarketDataDomainModelsResponseProfileFundamentalsResponse object
This constructor will only assign default values to properties that have it defined,
but it doesn't guarantee that properties required by API are set

### GetMarketCapitalization

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) GetMarketCapitalization() string`

GetMarketCapitalization returns the MarketCapitalization field if non-nil, zero value otherwise.

### GetMarketCapitalizationOk

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) GetMarketCapitalizationOk() (*string, bool)`

GetMarketCapitalizationOk returns a tuple with the MarketCapitalization field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetMarketCapitalization

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) SetMarketCapitalization(v string)`

SetMarketCapitalization sets MarketCapitalization field to given value.

### HasMarketCapitalization

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) HasMarketCapitalization() bool`

HasMarketCapitalization returns a boolean if a field has been set.

### SetMarketCapitalizationNil

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) SetMarketCapitalizationNil(b bool)`

 SetMarketCapitalizationNil sets the value for MarketCapitalization to be an explicit nil

### UnsetMarketCapitalization
`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) UnsetMarketCapitalization()`

UnsetMarketCapitalization ensures that no value is present for MarketCapitalization, not even an explicit nil
### GetShareFreeFloat

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) GetShareFreeFloat() string`

GetShareFreeFloat returns the ShareFreeFloat field if non-nil, zero value otherwise.

### GetShareFreeFloatOk

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) GetShareFreeFloatOk() (*string, bool)`

GetShareFreeFloatOk returns a tuple with the ShareFreeFloat field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetShareFreeFloat

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) SetShareFreeFloat(v string)`

SetShareFreeFloat sets ShareFreeFloat field to given value.

### HasShareFreeFloat

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) HasShareFreeFloat() bool`

HasShareFreeFloat returns a boolean if a field has been set.

### SetShareFreeFloatNil

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) SetShareFreeFloatNil(b bool)`

 SetShareFreeFloatNil sets the value for ShareFreeFloat to be an explicit nil

### UnsetShareFreeFloat
`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) UnsetShareFreeFloat()`

UnsetShareFreeFloat ensures that no value is present for ShareFreeFloat, not even an explicit nil
### GetIndustry

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) GetIndustry() string`

GetIndustry returns the Industry field if non-nil, zero value otherwise.

### GetIndustryOk

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) GetIndustryOk() (*string, bool)`

GetIndustryOk returns a tuple with the Industry field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetIndustry

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) SetIndustry(v string)`

SetIndustry sets Industry field to given value.

### HasIndustry

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) HasIndustry() bool`

HasIndustry returns a boolean if a field has been set.

### SetIndustryNil

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) SetIndustryNil(b bool)`

 SetIndustryNil sets the value for Industry to be an explicit nil

### UnsetIndustry
`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) UnsetIndustry()`

UnsetIndustry ensures that no value is present for Industry, not even an explicit nil
### GetSector

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) GetSector() string`

GetSector returns the Sector field if non-nil, zero value otherwise.

### GetSectorOk

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) GetSectorOk() (*string, bool)`

GetSectorOk returns a tuple with the Sector field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetSector

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) SetSector(v string)`

SetSector sets Sector field to given value.

### HasSector

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) HasSector() bool`

HasSector returns a boolean if a field has been set.

### SetSectorNil

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) SetSectorNil(b bool)`

 SetSectorNil sets the value for Sector to be an explicit nil

### UnsetSector
`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) UnsetSector()`

UnsetSector ensures that no value is present for Sector, not even an explicit nil
### GetCountry

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) GetCountry() string`

GetCountry returns the Country field if non-nil, zero value otherwise.

### GetCountryOk

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) GetCountryOk() (*string, bool)`

GetCountryOk returns a tuple with the Country field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetCountry

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) SetCountry(v string)`

SetCountry sets Country field to given value.

### HasCountry

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) HasCountry() bool`

HasCountry returns a boolean if a field has been set.

### SetCountryNil

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) SetCountryNil(b bool)`

 SetCountryNil sets the value for Country to be an explicit nil

### UnsetCountry
`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) UnsetCountry()`

UnsetCountry ensures that no value is present for Country, not even an explicit nil
### GetPriceToEarningsRatio

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) GetPriceToEarningsRatio() string`

GetPriceToEarningsRatio returns the PriceToEarningsRatio field if non-nil, zero value otherwise.

### GetPriceToEarningsRatioOk

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) GetPriceToEarningsRatioOk() (*string, bool)`

GetPriceToEarningsRatioOk returns a tuple with the PriceToEarningsRatio field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetPriceToEarningsRatio

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) SetPriceToEarningsRatio(v string)`

SetPriceToEarningsRatio sets PriceToEarningsRatio field to given value.

### HasPriceToEarningsRatio

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) HasPriceToEarningsRatio() bool`

HasPriceToEarningsRatio returns a boolean if a field has been set.

### SetPriceToEarningsRatioNil

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) SetPriceToEarningsRatioNil(b bool)`

 SetPriceToEarningsRatioNil sets the value for PriceToEarningsRatio to be an explicit nil

### UnsetPriceToEarningsRatio
`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) UnsetPriceToEarningsRatio()`

UnsetPriceToEarningsRatio ensures that no value is present for PriceToEarningsRatio, not even an explicit nil
### GetPriceToBookRatio

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) GetPriceToBookRatio() string`

GetPriceToBookRatio returns the PriceToBookRatio field if non-nil, zero value otherwise.

### GetPriceToBookRatioOk

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) GetPriceToBookRatioOk() (*string, bool)`

GetPriceToBookRatioOk returns a tuple with the PriceToBookRatio field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetPriceToBookRatio

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) SetPriceToBookRatio(v string)`

SetPriceToBookRatio sets PriceToBookRatio field to given value.

### HasPriceToBookRatio

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) HasPriceToBookRatio() bool`

HasPriceToBookRatio returns a boolean if a field has been set.

### SetPriceToBookRatioNil

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) SetPriceToBookRatioNil(b bool)`

 SetPriceToBookRatioNil sets the value for PriceToBookRatio to be an explicit nil

### UnsetPriceToBookRatio
`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) UnsetPriceToBookRatio()`

UnsetPriceToBookRatio ensures that no value is present for PriceToBookRatio, not even an explicit nil
### GetPriceToSalesRatio

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) GetPriceToSalesRatio() string`

GetPriceToSalesRatio returns the PriceToSalesRatio field if non-nil, zero value otherwise.

### GetPriceToSalesRatioOk

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) GetPriceToSalesRatioOk() (*string, bool)`

GetPriceToSalesRatioOk returns a tuple with the PriceToSalesRatio field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetPriceToSalesRatio

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) SetPriceToSalesRatio(v string)`

SetPriceToSalesRatio sets PriceToSalesRatio field to given value.

### HasPriceToSalesRatio

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) HasPriceToSalesRatio() bool`

HasPriceToSalesRatio returns a boolean if a field has been set.

### SetPriceToSalesRatioNil

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) SetPriceToSalesRatioNil(b bool)`

 SetPriceToSalesRatioNil sets the value for PriceToSalesRatio to be an explicit nil

### UnsetPriceToSalesRatio
`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) UnsetPriceToSalesRatio()`

UnsetPriceToSalesRatio ensures that no value is present for PriceToSalesRatio, not even an explicit nil
### GetDividendYieldPercentage

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) GetDividendYieldPercentage() string`

GetDividendYieldPercentage returns the DividendYieldPercentage field if non-nil, zero value otherwise.

### GetDividendYieldPercentageOk

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) GetDividendYieldPercentageOk() (*string, bool)`

GetDividendYieldPercentageOk returns a tuple with the DividendYieldPercentage field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetDividendYieldPercentage

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) SetDividendYieldPercentage(v string)`

SetDividendYieldPercentage sets DividendYieldPercentage field to given value.

### HasDividendYieldPercentage

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) HasDividendYieldPercentage() bool`

HasDividendYieldPercentage returns a boolean if a field has been set.

### SetDividendYieldPercentageNil

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) SetDividendYieldPercentageNil(b bool)`

 SetDividendYieldPercentageNil sets the value for DividendYieldPercentage to be an explicit nil

### UnsetDividendYieldPercentage
`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) UnsetDividendYieldPercentage()`

UnsetDividendYieldPercentage ensures that no value is present for DividendYieldPercentage, not even an explicit nil
### GetExDividendDate

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) GetExDividendDate() string`

GetExDividendDate returns the ExDividendDate field if non-nil, zero value otherwise.

### GetExDividendDateOk

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) GetExDividendDateOk() (*string, bool)`

GetExDividendDateOk returns a tuple with the ExDividendDate field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetExDividendDate

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) SetExDividendDate(v string)`

SetExDividendDate sets ExDividendDate field to given value.

### HasExDividendDate

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) HasExDividendDate() bool`

HasExDividendDate returns a boolean if a field has been set.

### SetExDividendDateNil

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) SetExDividendDateNil(b bool)`

 SetExDividendDateNil sets the value for ExDividendDate to be an explicit nil

### UnsetExDividendDate
`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) UnsetExDividendDate()`

UnsetExDividendDate ensures that no value is present for ExDividendDate, not even an explicit nil
### GetUnits

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) GetUnits() string`

GetUnits returns the Units field if non-nil, zero value otherwise.

### GetUnitsOk

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) GetUnitsOk() (*string, bool)`

GetUnitsOk returns a tuple with the Units field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetUnits

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) SetUnits(v string)`

SetUnits sets Units field to given value.

### HasUnits

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) HasUnits() bool`

HasUnits returns a boolean if a field has been set.

### SetUnitsNil

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) SetUnitsNil(b bool)`

 SetUnitsNil sets the value for Units to be an explicit nil

### UnsetUnits
`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) UnsetUnits()`

UnsetUnits ensures that no value is present for Units, not even an explicit nil
### GetSource

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) GetSource() string`

GetSource returns the Source field if non-nil, zero value otherwise.

### GetSourceOk

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) GetSourceOk() (*string, bool)`

GetSourceOk returns a tuple with the Source field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetSource

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) SetSource(v string)`

SetSource sets Source field to given value.

### HasSource

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) HasSource() bool`

HasSource returns a boolean if a field has been set.

### SetSourceNil

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) SetSourceNil(b bool)`

 SetSourceNil sets the value for Source to be an explicit nil

### UnsetSource
`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) UnsetSource()`

UnsetSource ensures that no value is present for Source, not even an explicit nil
### GetUnderlyingSymbol

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) GetUnderlyingSymbol() string`

GetUnderlyingSymbol returns the UnderlyingSymbol field if non-nil, zero value otherwise.

### GetUnderlyingSymbolOk

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) GetUnderlyingSymbolOk() (*string, bool)`

GetUnderlyingSymbolOk returns a tuple with the UnderlyingSymbol field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetUnderlyingSymbol

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) SetUnderlyingSymbol(v string)`

SetUnderlyingSymbol sets UnderlyingSymbol field to given value.

### HasUnderlyingSymbol

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) HasUnderlyingSymbol() bool`

HasUnderlyingSymbol returns a boolean if a field has been set.

### SetUnderlyingSymbolNil

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) SetUnderlyingSymbolNil(b bool)`

 SetUnderlyingSymbolNil sets the value for UnderlyingSymbol to be an explicit nil

### UnsetUnderlyingSymbol
`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) UnsetUnderlyingSymbol()`

UnsetUnderlyingSymbol ensures that no value is present for UnderlyingSymbol, not even an explicit nil
### GetIsClickable

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) GetIsClickable() bool`

GetIsClickable returns the IsClickable field if non-nil, zero value otherwise.

### GetIsClickableOk

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) GetIsClickableOk() (*bool, bool)`

GetIsClickableOk returns a tuple with the IsClickable field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetIsClickable

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) SetIsClickable(v bool)`

SetIsClickable sets IsClickable field to given value.

### HasIsClickable

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) HasIsClickable() bool`

HasIsClickable returns a boolean if a field has been set.

### GetUnderlyingVenue

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) GetUnderlyingVenue() string`

GetUnderlyingVenue returns the UnderlyingVenue field if non-nil, zero value otherwise.

### GetUnderlyingVenueOk

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) GetUnderlyingVenueOk() (*string, bool)`

GetUnderlyingVenueOk returns a tuple with the UnderlyingVenue field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetUnderlyingVenue

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) SetUnderlyingVenue(v string)`

SetUnderlyingVenue sets UnderlyingVenue field to given value.

### HasUnderlyingVenue

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) HasUnderlyingVenue() bool`

HasUnderlyingVenue returns a boolean if a field has been set.

### SetUnderlyingVenueNil

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) SetUnderlyingVenueNil(b bool)`

 SetUnderlyingVenueNil sets the value for UnderlyingVenue to be an explicit nil

### UnsetUnderlyingVenue
`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) UnsetUnderlyingVenue()`

UnsetUnderlyingVenue ensures that no value is present for UnderlyingVenue, not even an explicit nil
### GetUnderlyingInstrumentType

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) GetUnderlyingInstrumentType() string`

GetUnderlyingInstrumentType returns the UnderlyingInstrumentType field if non-nil, zero value otherwise.

### GetUnderlyingInstrumentTypeOk

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) GetUnderlyingInstrumentTypeOk() (*string, bool)`

GetUnderlyingInstrumentTypeOk returns a tuple with the UnderlyingInstrumentType field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetUnderlyingInstrumentType

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) SetUnderlyingInstrumentType(v string)`

SetUnderlyingInstrumentType sets UnderlyingInstrumentType field to given value.

### HasUnderlyingInstrumentType

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) HasUnderlyingInstrumentType() bool`

HasUnderlyingInstrumentType returns a boolean if a field has been set.

### SetUnderlyingInstrumentTypeNil

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) SetUnderlyingInstrumentTypeNil(b bool)`

 SetUnderlyingInstrumentTypeNil sets the value for UnderlyingInstrumentType to be an explicit nil

### UnsetUnderlyingInstrumentType
`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) UnsetUnderlyingInstrumentType()`

UnsetUnderlyingInstrumentType ensures that no value is present for UnderlyingInstrumentType, not even an explicit nil
### GetUnderlyingInstrumentCategory

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) GetUnderlyingInstrumentCategory() string`

GetUnderlyingInstrumentCategory returns the UnderlyingInstrumentCategory field if non-nil, zero value otherwise.

### GetUnderlyingInstrumentCategoryOk

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) GetUnderlyingInstrumentCategoryOk() (*string, bool)`

GetUnderlyingInstrumentCategoryOk returns a tuple with the UnderlyingInstrumentCategory field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetUnderlyingInstrumentCategory

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) SetUnderlyingInstrumentCategory(v string)`

SetUnderlyingInstrumentCategory sets UnderlyingInstrumentCategory field to given value.

### HasUnderlyingInstrumentCategory

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) HasUnderlyingInstrumentCategory() bool`

HasUnderlyingInstrumentCategory returns a boolean if a field has been set.

### SetUnderlyingInstrumentCategoryNil

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) SetUnderlyingInstrumentCategoryNil(b bool)`

 SetUnderlyingInstrumentCategoryNil sets the value for UnderlyingInstrumentCategory to be an explicit nil

### UnsetUnderlyingInstrumentCategory
`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) UnsetUnderlyingInstrumentCategory()`

UnsetUnderlyingInstrumentCategory ensures that no value is present for UnderlyingInstrumentCategory, not even an explicit nil
### GetUnderlyingLogo

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) GetUnderlyingLogo() string`

GetUnderlyingLogo returns the UnderlyingLogo field if non-nil, zero value otherwise.

### GetUnderlyingLogoOk

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) GetUnderlyingLogoOk() (*string, bool)`

GetUnderlyingLogoOk returns a tuple with the UnderlyingLogo field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetUnderlyingLogo

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) SetUnderlyingLogo(v string)`

SetUnderlyingLogo sets UnderlyingLogo field to given value.

### HasUnderlyingLogo

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) HasUnderlyingLogo() bool`

HasUnderlyingLogo returns a boolean if a field has been set.

### SetUnderlyingLogoNil

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) SetUnderlyingLogoNil(b bool)`

 SetUnderlyingLogoNil sets the value for UnderlyingLogo to be an explicit nil

### UnsetUnderlyingLogo
`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) UnsetUnderlyingLogo()`

UnsetUnderlyingLogo ensures that no value is present for UnderlyingLogo, not even an explicit nil
### GetExerciseRatio

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) GetExerciseRatio() string`

GetExerciseRatio returns the ExerciseRatio field if non-nil, zero value otherwise.

### GetExerciseRatioOk

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) GetExerciseRatioOk() (*string, bool)`

GetExerciseRatioOk returns a tuple with the ExerciseRatio field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetExerciseRatio

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) SetExerciseRatio(v string)`

SetExerciseRatio sets ExerciseRatio field to given value.

### HasExerciseRatio

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) HasExerciseRatio() bool`

HasExerciseRatio returns a boolean if a field has been set.

### SetExerciseRatioNil

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) SetExerciseRatioNil(b bool)`

 SetExerciseRatioNil sets the value for ExerciseRatio to be an explicit nil

### UnsetExerciseRatio
`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) UnsetExerciseRatio()`

UnsetExerciseRatio ensures that no value is present for ExerciseRatio, not even an explicit nil
### GetExercisePrice

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) GetExercisePrice() string`

GetExercisePrice returns the ExercisePrice field if non-nil, zero value otherwise.

### GetExercisePriceOk

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) GetExercisePriceOk() (*string, bool)`

GetExercisePriceOk returns a tuple with the ExercisePrice field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetExercisePrice

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) SetExercisePrice(v string)`

SetExercisePrice sets ExercisePrice field to given value.

### HasExercisePrice

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) HasExercisePrice() bool`

HasExercisePrice returns a boolean if a field has been set.

### SetExercisePriceNil

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) SetExercisePriceNil(b bool)`

 SetExercisePriceNil sets the value for ExercisePrice to be an explicit nil

### UnsetExercisePrice
`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) UnsetExercisePrice()`

UnsetExercisePrice ensures that no value is present for ExercisePrice, not even an explicit nil
### GetExerciseDate

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) GetExerciseDate() string`

GetExerciseDate returns the ExerciseDate field if non-nil, zero value otherwise.

### GetExerciseDateOk

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) GetExerciseDateOk() (*string, bool)`

GetExerciseDateOk returns a tuple with the ExerciseDate field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetExerciseDate

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) SetExerciseDate(v string)`

SetExerciseDate sets ExerciseDate field to given value.

### HasExerciseDate

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) HasExerciseDate() bool`

HasExerciseDate returns a boolean if a field has been set.

### SetExerciseDateNil

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) SetExerciseDateNil(b bool)`

 SetExerciseDateNil sets the value for ExerciseDate to be an explicit nil

### UnsetExerciseDate
`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) UnsetExerciseDate()`

UnsetExerciseDate ensures that no value is present for ExerciseDate, not even an explicit nil
### GetDaysToExercise

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) GetDaysToExercise() string`

GetDaysToExercise returns the DaysToExercise field if non-nil, zero value otherwise.

### GetDaysToExerciseOk

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) GetDaysToExerciseOk() (*string, bool)`

GetDaysToExerciseOk returns a tuple with the DaysToExercise field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetDaysToExercise

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) SetDaysToExercise(v string)`

SetDaysToExercise sets DaysToExercise field to given value.

### HasDaysToExercise

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) HasDaysToExercise() bool`

HasDaysToExercise returns a boolean if a field has been set.

### SetDaysToExerciseNil

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) SetDaysToExerciseNil(b bool)`

 SetDaysToExerciseNil sets the value for DaysToExercise to be an explicit nil

### UnsetDaysToExercise
`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) UnsetDaysToExercise()`

UnsetDaysToExercise ensures that no value is present for DaysToExercise, not even an explicit nil
### GetMoneyness

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) GetMoneyness() string`

GetMoneyness returns the Moneyness field if non-nil, zero value otherwise.

### GetMoneynessOk

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) GetMoneynessOk() (*string, bool)`

GetMoneynessOk returns a tuple with the Moneyness field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetMoneyness

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) SetMoneyness(v string)`

SetMoneyness sets Moneyness field to given value.

### HasMoneyness

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) HasMoneyness() bool`

HasMoneyness returns a boolean if a field has been set.

### SetMoneynessNil

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) SetMoneynessNil(b bool)`

 SetMoneynessNil sets the value for Moneyness to be an explicit nil

### UnsetMoneyness
`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) UnsetMoneyness()`

UnsetMoneyness ensures that no value is present for Moneyness, not even an explicit nil
### GetDirection

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) GetDirection() string`

GetDirection returns the Direction field if non-nil, zero value otherwise.

### GetDirectionOk

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) GetDirectionOk() (*string, bool)`

GetDirectionOk returns a tuple with the Direction field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetDirection

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) SetDirection(v string)`

SetDirection sets Direction field to given value.

### HasDirection

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) HasDirection() bool`

HasDirection returns a boolean if a field has been set.

### SetDirectionNil

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) SetDirectionNil(b bool)`

 SetDirectionNil sets the value for Direction to be an explicit nil

### UnsetDirection
`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) UnsetDirection()`

UnsetDirection ensures that no value is present for Direction, not even an explicit nil
### GetMultiplier

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) GetMultiplier() string`

GetMultiplier returns the Multiplier field if non-nil, zero value otherwise.

### GetMultiplierOk

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) GetMultiplierOk() (*string, bool)`

GetMultiplierOk returns a tuple with the Multiplier field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetMultiplier

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) SetMultiplier(v string)`

SetMultiplier sets Multiplier field to given value.

### HasMultiplier

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) HasMultiplier() bool`

HasMultiplier returns a boolean if a field has been set.

### SetMultiplierNil

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) SetMultiplierNil(b bool)`

 SetMultiplierNil sets the value for Multiplier to be an explicit nil

### UnsetMultiplier
`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) UnsetMultiplier()`

UnsetMultiplier ensures that no value is present for Multiplier, not even an explicit nil
### GetLastTradingDate

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) GetLastTradingDate() string`

GetLastTradingDate returns the LastTradingDate field if non-nil, zero value otherwise.

### GetLastTradingDateOk

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) GetLastTradingDateOk() (*string, bool)`

GetLastTradingDateOk returns a tuple with the LastTradingDate field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetLastTradingDate

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) SetLastTradingDate(v string)`

SetLastTradingDate sets LastTradingDate field to given value.

### HasLastTradingDate

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) HasLastTradingDate() bool`

HasLastTradingDate returns a boolean if a field has been set.

### SetLastTradingDateNil

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) SetLastTradingDateNil(b bool)`

 SetLastTradingDateNil sets the value for LastTradingDate to be an explicit nil

### UnsetLastTradingDate
`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) UnsetLastTradingDate()`

UnsetLastTradingDate ensures that no value is present for LastTradingDate, not even an explicit nil
### GetDaysToLastTrade

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) GetDaysToLastTrade() string`

GetDaysToLastTrade returns the DaysToLastTrade field if non-nil, zero value otherwise.

### GetDaysToLastTradeOk

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) GetDaysToLastTradeOk() (*string, bool)`

GetDaysToLastTradeOk returns a tuple with the DaysToLastTrade field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetDaysToLastTrade

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) SetDaysToLastTrade(v string)`

SetDaysToLastTrade sets DaysToLastTrade field to given value.

### HasDaysToLastTrade

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) HasDaysToLastTrade() bool`

HasDaysToLastTrade returns a boolean if a field has been set.

### SetDaysToLastTradeNil

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) SetDaysToLastTradeNil(b bool)`

 SetDaysToLastTradeNil sets the value for DaysToLastTrade to be an explicit nil

### UnsetDaysToLastTrade
`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) UnsetDaysToLastTrade()`

UnsetDaysToLastTrade ensures that no value is present for DaysToLastTrade, not even an explicit nil
### GetMaturityDate

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) GetMaturityDate() string`

GetMaturityDate returns the MaturityDate field if non-nil, zero value otherwise.

### GetMaturityDateOk

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) GetMaturityDateOk() (*string, bool)`

GetMaturityDateOk returns a tuple with the MaturityDate field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetMaturityDate

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) SetMaturityDate(v string)`

SetMaturityDate sets MaturityDate field to given value.

### HasMaturityDate

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) HasMaturityDate() bool`

HasMaturityDate returns a boolean if a field has been set.

### SetMaturityDateNil

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) SetMaturityDateNil(b bool)`

 SetMaturityDateNil sets the value for MaturityDate to be an explicit nil

### UnsetMaturityDate
`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) UnsetMaturityDate()`

UnsetMaturityDate ensures that no value is present for MaturityDate, not even an explicit nil
### GetIssuerSeries

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) GetIssuerSeries() string`

GetIssuerSeries returns the IssuerSeries field if non-nil, zero value otherwise.

### GetIssuerSeriesOk

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) GetIssuerSeriesOk() (*string, bool)`

GetIssuerSeriesOk returns a tuple with the IssuerSeries field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetIssuerSeries

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) SetIssuerSeries(v string)`

SetIssuerSeries sets IssuerSeries field to given value.

### HasIssuerSeries

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) HasIssuerSeries() bool`

HasIssuerSeries returns a boolean if a field has been set.

### SetIssuerSeriesNil

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) SetIssuerSeriesNil(b bool)`

 SetIssuerSeriesNil sets the value for IssuerSeries to be an explicit nil

### UnsetIssuerSeries
`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) UnsetIssuerSeries()`

UnsetIssuerSeries ensures that no value is present for IssuerSeries, not even an explicit nil
### GetForeignCurrency

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) GetForeignCurrency() string`

GetForeignCurrency returns the ForeignCurrency field if non-nil, zero value otherwise.

### GetForeignCurrencyOk

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) GetForeignCurrencyOk() (*string, bool)`

GetForeignCurrencyOk returns a tuple with the ForeignCurrency field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetForeignCurrency

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) SetForeignCurrency(v string)`

SetForeignCurrency sets ForeignCurrency field to given value.

### HasForeignCurrency

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) HasForeignCurrency() bool`

HasForeignCurrency returns a boolean if a field has been set.

### SetForeignCurrencyNil

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) SetForeignCurrencyNil(b bool)`

 SetForeignCurrencyNil sets the value for ForeignCurrency to be an explicit nil

### UnsetForeignCurrency
`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) UnsetForeignCurrency()`

UnsetForeignCurrency ensures that no value is present for ForeignCurrency, not even an explicit nil
### GetConversionRatio

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) GetConversionRatio() string`

GetConversionRatio returns the ConversionRatio field if non-nil, zero value otherwise.

### GetConversionRatioOk

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) GetConversionRatioOk() (*string, bool)`

GetConversionRatioOk returns a tuple with the ConversionRatio field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetConversionRatio

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) SetConversionRatio(v string)`

SetConversionRatio sets ConversionRatio field to given value.

### HasConversionRatio

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) HasConversionRatio() bool`

HasConversionRatio returns a boolean if a field has been set.

### SetConversionRatioNil

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) SetConversionRatioNil(b bool)`

 SetConversionRatioNil sets the value for ConversionRatio to be an explicit nil

### UnsetConversionRatio
`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) UnsetConversionRatio()`

UnsetConversionRatio ensures that no value is present for ConversionRatio, not even an explicit nil
### GetUnderlyingPrice

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) GetUnderlyingPrice() string`

GetUnderlyingPrice returns the UnderlyingPrice field if non-nil, zero value otherwise.

### GetUnderlyingPriceOk

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) GetUnderlyingPriceOk() (*string, bool)`

GetUnderlyingPriceOk returns a tuple with the UnderlyingPrice field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetUnderlyingPrice

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) SetUnderlyingPrice(v string)`

SetUnderlyingPrice sets UnderlyingPrice field to given value.

### HasUnderlyingPrice

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) HasUnderlyingPrice() bool`

HasUnderlyingPrice returns a boolean if a field has been set.

### SetUnderlyingPriceNil

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) SetUnderlyingPriceNil(b bool)`

 SetUnderlyingPriceNil sets the value for UnderlyingPrice to be an explicit nil

### UnsetUnderlyingPrice
`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) UnsetUnderlyingPrice()`

UnsetUnderlyingPrice ensures that no value is present for UnderlyingPrice, not even an explicit nil
### GetBasis

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) GetBasis() string`

GetBasis returns the Basis field if non-nil, zero value otherwise.

### GetBasisOk

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) GetBasisOk() (*string, bool)`

GetBasisOk returns a tuple with the Basis field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetBasis

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) SetBasis(v string)`

SetBasis sets Basis field to given value.

### HasBasis

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) HasBasis() bool`

HasBasis returns a boolean if a field has been set.

### SetBasisNil

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) SetBasisNil(b bool)`

 SetBasisNil sets the value for Basis to be an explicit nil

### UnsetBasis
`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) UnsetBasis()`

UnsetBasis ensures that no value is present for Basis, not even an explicit nil
### GetOpenInterest

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) GetOpenInterest() string`

GetOpenInterest returns the OpenInterest field if non-nil, zero value otherwise.

### GetOpenInterestOk

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) GetOpenInterestOk() (*string, bool)`

GetOpenInterestOk returns a tuple with the OpenInterest field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetOpenInterest

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) SetOpenInterest(v string)`

SetOpenInterest sets OpenInterest field to given value.

### HasOpenInterest

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) HasOpenInterest() bool`

HasOpenInterest returns a boolean if a field has been set.

### SetOpenInterestNil

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) SetOpenInterestNil(b bool)`

 SetOpenInterestNil sets the value for OpenInterest to be an explicit nil

### UnsetOpenInterest
`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) UnsetOpenInterest()`

UnsetOpenInterest ensures that no value is present for OpenInterest, not even an explicit nil
### GetContractSize

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) GetContractSize() string`

GetContractSize returns the ContractSize field if non-nil, zero value otherwise.

### GetContractSizeOk

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) GetContractSizeOk() (*string, bool)`

GetContractSizeOk returns a tuple with the ContractSize field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetContractSize

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) SetContractSize(v string)`

SetContractSize sets ContractSize field to given value.

### HasContractSize

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) HasContractSize() bool`

HasContractSize returns a boolean if a field has been set.

### SetContractSizeNil

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) SetContractSizeNil(b bool)`

 SetContractSizeNil sets the value for ContractSize to be an explicit nil

### UnsetContractSize
`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) UnsetContractSize()`

UnsetContractSize ensures that no value is present for ContractSize, not even an explicit nil
### GetLastNavPerShare

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) GetLastNavPerShare() string`

GetLastNavPerShare returns the LastNavPerShare field if non-nil, zero value otherwise.

### GetLastNavPerShareOk

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) GetLastNavPerShareOk() (*string, bool)`

GetLastNavPerShareOk returns a tuple with the LastNavPerShare field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetLastNavPerShare

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) SetLastNavPerShare(v string)`

SetLastNavPerShare sets LastNavPerShare field to given value.

### HasLastNavPerShare

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) HasLastNavPerShare() bool`

HasLastNavPerShare returns a boolean if a field has been set.

### SetLastNavPerShareNil

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) SetLastNavPerShareNil(b bool)`

 SetLastNavPerShareNil sets the value for LastNavPerShare to be an explicit nil

### UnsetLastNavPerShare
`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) UnsetLastNavPerShare()`

UnsetLastNavPerShare ensures that no value is present for LastNavPerShare, not even an explicit nil
### GetObjective

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) GetObjective() string`

GetObjective returns the Objective field if non-nil, zero value otherwise.

### GetObjectiveOk

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) GetObjectiveOk() (*string, bool)`

GetObjectiveOk returns a tuple with the Objective field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetObjective

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) SetObjective(v string)`

SetObjective sets Objective field to given value.

### HasObjective

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) HasObjective() bool`

HasObjective returns a boolean if a field has been set.

### SetObjectiveNil

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) SetObjectiveNil(b bool)`

 SetObjectiveNil sets the value for Objective to be an explicit nil

### UnsetObjective
`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) UnsetObjective()`

UnsetObjective ensures that no value is present for Objective, not even an explicit nil
### GetAssetClassFocus

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) GetAssetClassFocus() string`

GetAssetClassFocus returns the AssetClassFocus field if non-nil, zero value otherwise.

### GetAssetClassFocusOk

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) GetAssetClassFocusOk() (*string, bool)`

GetAssetClassFocusOk returns a tuple with the AssetClassFocus field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetAssetClassFocus

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) SetAssetClassFocus(v string)`

SetAssetClassFocus sets AssetClassFocus field to given value.

### HasAssetClassFocus

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) HasAssetClassFocus() bool`

HasAssetClassFocus returns a boolean if a field has been set.

### SetAssetClassFocusNil

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) SetAssetClassFocusNil(b bool)`

 SetAssetClassFocusNil sets the value for AssetClassFocus to be an explicit nil

### UnsetAssetClassFocus
`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) UnsetAssetClassFocus()`

UnsetAssetClassFocus ensures that no value is present for AssetClassFocus, not even an explicit nil
### GetExpenseRatioPercentage

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) GetExpenseRatioPercentage() string`

GetExpenseRatioPercentage returns the ExpenseRatioPercentage field if non-nil, zero value otherwise.

### GetExpenseRatioPercentageOk

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) GetExpenseRatioPercentageOk() (*string, bool)`

GetExpenseRatioPercentageOk returns a tuple with the ExpenseRatioPercentage field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetExpenseRatioPercentage

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) SetExpenseRatioPercentage(v string)`

SetExpenseRatioPercentage sets ExpenseRatioPercentage field to given value.

### HasExpenseRatioPercentage

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) HasExpenseRatioPercentage() bool`

HasExpenseRatioPercentage returns a boolean if a field has been set.

### SetExpenseRatioPercentageNil

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) SetExpenseRatioPercentageNil(b bool)`

 SetExpenseRatioPercentageNil sets the value for ExpenseRatioPercentage to be an explicit nil

### UnsetExpenseRatioPercentage
`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) UnsetExpenseRatioPercentage()`

UnsetExpenseRatioPercentage ensures that no value is present for ExpenseRatioPercentage, not even an explicit nil
### GetStrikePrice

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) GetStrikePrice() string`

GetStrikePrice returns the StrikePrice field if non-nil, zero value otherwise.

### GetStrikePriceOk

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) GetStrikePriceOk() (*string, bool)`

GetStrikePriceOk returns a tuple with the StrikePrice field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetStrikePrice

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) SetStrikePrice(v string)`

SetStrikePrice sets StrikePrice field to given value.

### HasStrikePrice

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) HasStrikePrice() bool`

HasStrikePrice returns a boolean if a field has been set.

### SetStrikePriceNil

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) SetStrikePriceNil(b bool)`

 SetStrikePriceNil sets the value for StrikePrice to be an explicit nil

### UnsetStrikePrice
`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) UnsetStrikePrice()`

UnsetStrikePrice ensures that no value is present for StrikePrice, not even an explicit nil
### GetTheoreticalPrice

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) GetTheoreticalPrice() string`

GetTheoreticalPrice returns the TheoreticalPrice field if non-nil, zero value otherwise.

### GetTheoreticalPriceOk

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) GetTheoreticalPriceOk() (*string, bool)`

GetTheoreticalPriceOk returns a tuple with the TheoreticalPrice field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetTheoreticalPrice

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) SetTheoreticalPrice(v string)`

SetTheoreticalPrice sets TheoreticalPrice field to given value.

### HasTheoreticalPrice

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) HasTheoreticalPrice() bool`

HasTheoreticalPrice returns a boolean if a field has been set.

### SetTheoreticalPriceNil

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) SetTheoreticalPriceNil(b bool)`

 SetTheoreticalPriceNil sets the value for TheoreticalPrice to be an explicit nil

### UnsetTheoreticalPrice
`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) UnsetTheoreticalPrice()`

UnsetTheoreticalPrice ensures that no value is present for TheoreticalPrice, not even an explicit nil
### GetIntrinsicValue

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) GetIntrinsicValue() string`

GetIntrinsicValue returns the IntrinsicValue field if non-nil, zero value otherwise.

### GetIntrinsicValueOk

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) GetIntrinsicValueOk() (*string, bool)`

GetIntrinsicValueOk returns a tuple with the IntrinsicValue field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetIntrinsicValue

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) SetIntrinsicValue(v string)`

SetIntrinsicValue sets IntrinsicValue field to given value.

### HasIntrinsicValue

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) HasIntrinsicValue() bool`

HasIntrinsicValue returns a boolean if a field has been set.

### SetIntrinsicValueNil

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) SetIntrinsicValueNil(b bool)`

 SetIntrinsicValueNil sets the value for IntrinsicValue to be an explicit nil

### UnsetIntrinsicValue
`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) UnsetIntrinsicValue()`

UnsetIntrinsicValue ensures that no value is present for IntrinsicValue, not even an explicit nil
### GetImpliedVolatilityPercentage

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) GetImpliedVolatilityPercentage() string`

GetImpliedVolatilityPercentage returns the ImpliedVolatilityPercentage field if non-nil, zero value otherwise.

### GetImpliedVolatilityPercentageOk

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) GetImpliedVolatilityPercentageOk() (*string, bool)`

GetImpliedVolatilityPercentageOk returns a tuple with the ImpliedVolatilityPercentage field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetImpliedVolatilityPercentage

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) SetImpliedVolatilityPercentage(v string)`

SetImpliedVolatilityPercentage sets ImpliedVolatilityPercentage field to given value.

### HasImpliedVolatilityPercentage

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) HasImpliedVolatilityPercentage() bool`

HasImpliedVolatilityPercentage returns a boolean if a field has been set.

### SetImpliedVolatilityPercentageNil

`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) SetImpliedVolatilityPercentageNil(b bool)`

 SetImpliedVolatilityPercentageNil sets the value for ImpliedVolatilityPercentage to be an explicit nil

### UnsetImpliedVolatilityPercentage
`func (o *PiMarketDataDomainModelsResponseProfileFundamentalsResponse) UnsetImpliedVolatilityPercentage()`

UnsetImpliedVolatilityPercentage ensures that no value is present for ImpliedVolatilityPercentage, not even an explicit nil

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


