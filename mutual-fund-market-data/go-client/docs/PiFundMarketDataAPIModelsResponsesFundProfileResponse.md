# PiFundMarketDataAPIModelsResponsesFundProfileResponse

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**Isin** | Pointer to **NullableString** |  | [optional] 
**Symbol** | Pointer to **NullableString** |  | [optional] 
**Name** | Pointer to **NullableString** |  | [optional] 
**FundCategory** | Pointer to **NullableString** |  | [optional] 
**FactSheetUrl** | Pointer to **NullableString** |  | [optional] 
**Rating** | Pointer to **NullableInt32** |  | [optional] 
**IsInLegacyMarket** | Pointer to **bool** |  | [optional] 
**AsOfDate** | Pointer to **time.Time** |  | [optional] 
**AmcProfile** | Pointer to [**PiFundMarketDataAPIModelsResponsesAmcProfileResponse**](PiFundMarketDataAPIModelsResponsesAmcProfileResponse.md) |  | [optional] 
**Fundamental** | Pointer to [**PiFundMarketDataAPIModelsResponsesFundamentalResponse**](PiFundMarketDataAPIModelsResponsesFundamentalResponse.md) |  | [optional] 
**AssetValue** | Pointer to [**PiFundMarketDataAPIModelsResponsesAssetValueResponse**](PiFundMarketDataAPIModelsResponsesAssetValueResponse.md) |  | [optional] 
**AssetAllocation** | Pointer to [**PiFundMarketDataAPIModelsResponsesAssetAllocationResponse**](PiFundMarketDataAPIModelsResponsesAssetAllocationResponse.md) |  | [optional] 
**PurchaseDetail** | Pointer to [**PiFundMarketDataAPIModelsResponsesPurchaseDetailResponse**](PiFundMarketDataAPIModelsResponsesPurchaseDetailResponse.md) |  | [optional] 
**FeesAndExpense** | Pointer to [**PiFundMarketDataAPIModelsResponsesFeeResponse**](PiFundMarketDataAPIModelsResponsesFeeResponse.md) |  | [optional] 
**Performance** | Pointer to [**PiFundMarketDataAPIModelsResponsesPerformanceResponse**](PiFundMarketDataAPIModelsResponsesPerformanceResponse.md) |  | [optional] 
**Distribution** | Pointer to [**PiFundMarketDataAPIModelsResponsesDistributionResponse**](PiFundMarketDataAPIModelsResponsesDistributionResponse.md) |  | [optional] 

## Methods

### NewPiFundMarketDataAPIModelsResponsesFundProfileResponse

`func NewPiFundMarketDataAPIModelsResponsesFundProfileResponse() *PiFundMarketDataAPIModelsResponsesFundProfileResponse`

NewPiFundMarketDataAPIModelsResponsesFundProfileResponse instantiates a new PiFundMarketDataAPIModelsResponsesFundProfileResponse object
This constructor will assign default values to properties that have it defined,
and makes sure properties required by API are set, but the set of arguments
will change when the set of required properties is changed

### NewPiFundMarketDataAPIModelsResponsesFundProfileResponseWithDefaults

`func NewPiFundMarketDataAPIModelsResponsesFundProfileResponseWithDefaults() *PiFundMarketDataAPIModelsResponsesFundProfileResponse`

NewPiFundMarketDataAPIModelsResponsesFundProfileResponseWithDefaults instantiates a new PiFundMarketDataAPIModelsResponsesFundProfileResponse object
This constructor will only assign default values to properties that have it defined,
but it doesn't guarantee that properties required by API are set

### GetIsin

`func (o *PiFundMarketDataAPIModelsResponsesFundProfileResponse) GetIsin() string`

GetIsin returns the Isin field if non-nil, zero value otherwise.

### GetIsinOk

`func (o *PiFundMarketDataAPIModelsResponsesFundProfileResponse) GetIsinOk() (*string, bool)`

GetIsinOk returns a tuple with the Isin field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetIsin

`func (o *PiFundMarketDataAPIModelsResponsesFundProfileResponse) SetIsin(v string)`

SetIsin sets Isin field to given value.

### HasIsin

`func (o *PiFundMarketDataAPIModelsResponsesFundProfileResponse) HasIsin() bool`

HasIsin returns a boolean if a field has been set.

### SetIsinNil

`func (o *PiFundMarketDataAPIModelsResponsesFundProfileResponse) SetIsinNil(b bool)`

 SetIsinNil sets the value for Isin to be an explicit nil

### UnsetIsin
`func (o *PiFundMarketDataAPIModelsResponsesFundProfileResponse) UnsetIsin()`

UnsetIsin ensures that no value is present for Isin, not even an explicit nil
### GetSymbol

`func (o *PiFundMarketDataAPIModelsResponsesFundProfileResponse) GetSymbol() string`

GetSymbol returns the Symbol field if non-nil, zero value otherwise.

### GetSymbolOk

`func (o *PiFundMarketDataAPIModelsResponsesFundProfileResponse) GetSymbolOk() (*string, bool)`

GetSymbolOk returns a tuple with the Symbol field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetSymbol

`func (o *PiFundMarketDataAPIModelsResponsesFundProfileResponse) SetSymbol(v string)`

SetSymbol sets Symbol field to given value.

### HasSymbol

`func (o *PiFundMarketDataAPIModelsResponsesFundProfileResponse) HasSymbol() bool`

HasSymbol returns a boolean if a field has been set.

### SetSymbolNil

`func (o *PiFundMarketDataAPIModelsResponsesFundProfileResponse) SetSymbolNil(b bool)`

 SetSymbolNil sets the value for Symbol to be an explicit nil

### UnsetSymbol
`func (o *PiFundMarketDataAPIModelsResponsesFundProfileResponse) UnsetSymbol()`

UnsetSymbol ensures that no value is present for Symbol, not even an explicit nil
### GetName

`func (o *PiFundMarketDataAPIModelsResponsesFundProfileResponse) GetName() string`

GetName returns the Name field if non-nil, zero value otherwise.

### GetNameOk

`func (o *PiFundMarketDataAPIModelsResponsesFundProfileResponse) GetNameOk() (*string, bool)`

GetNameOk returns a tuple with the Name field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetName

`func (o *PiFundMarketDataAPIModelsResponsesFundProfileResponse) SetName(v string)`

SetName sets Name field to given value.

### HasName

`func (o *PiFundMarketDataAPIModelsResponsesFundProfileResponse) HasName() bool`

HasName returns a boolean if a field has been set.

### SetNameNil

`func (o *PiFundMarketDataAPIModelsResponsesFundProfileResponse) SetNameNil(b bool)`

 SetNameNil sets the value for Name to be an explicit nil

### UnsetName
`func (o *PiFundMarketDataAPIModelsResponsesFundProfileResponse) UnsetName()`

UnsetName ensures that no value is present for Name, not even an explicit nil
### GetFundCategory

`func (o *PiFundMarketDataAPIModelsResponsesFundProfileResponse) GetFundCategory() string`

GetFundCategory returns the FundCategory field if non-nil, zero value otherwise.

### GetFundCategoryOk

`func (o *PiFundMarketDataAPIModelsResponsesFundProfileResponse) GetFundCategoryOk() (*string, bool)`

GetFundCategoryOk returns a tuple with the FundCategory field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetFundCategory

`func (o *PiFundMarketDataAPIModelsResponsesFundProfileResponse) SetFundCategory(v string)`

SetFundCategory sets FundCategory field to given value.

### HasFundCategory

`func (o *PiFundMarketDataAPIModelsResponsesFundProfileResponse) HasFundCategory() bool`

HasFundCategory returns a boolean if a field has been set.

### SetFundCategoryNil

`func (o *PiFundMarketDataAPIModelsResponsesFundProfileResponse) SetFundCategoryNil(b bool)`

 SetFundCategoryNil sets the value for FundCategory to be an explicit nil

### UnsetFundCategory
`func (o *PiFundMarketDataAPIModelsResponsesFundProfileResponse) UnsetFundCategory()`

UnsetFundCategory ensures that no value is present for FundCategory, not even an explicit nil
### GetFactSheetUrl

`func (o *PiFundMarketDataAPIModelsResponsesFundProfileResponse) GetFactSheetUrl() string`

GetFactSheetUrl returns the FactSheetUrl field if non-nil, zero value otherwise.

### GetFactSheetUrlOk

`func (o *PiFundMarketDataAPIModelsResponsesFundProfileResponse) GetFactSheetUrlOk() (*string, bool)`

GetFactSheetUrlOk returns a tuple with the FactSheetUrl field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetFactSheetUrl

`func (o *PiFundMarketDataAPIModelsResponsesFundProfileResponse) SetFactSheetUrl(v string)`

SetFactSheetUrl sets FactSheetUrl field to given value.

### HasFactSheetUrl

`func (o *PiFundMarketDataAPIModelsResponsesFundProfileResponse) HasFactSheetUrl() bool`

HasFactSheetUrl returns a boolean if a field has been set.

### SetFactSheetUrlNil

`func (o *PiFundMarketDataAPIModelsResponsesFundProfileResponse) SetFactSheetUrlNil(b bool)`

 SetFactSheetUrlNil sets the value for FactSheetUrl to be an explicit nil

### UnsetFactSheetUrl
`func (o *PiFundMarketDataAPIModelsResponsesFundProfileResponse) UnsetFactSheetUrl()`

UnsetFactSheetUrl ensures that no value is present for FactSheetUrl, not even an explicit nil
### GetRating

`func (o *PiFundMarketDataAPIModelsResponsesFundProfileResponse) GetRating() int32`

GetRating returns the Rating field if non-nil, zero value otherwise.

### GetRatingOk

`func (o *PiFundMarketDataAPIModelsResponsesFundProfileResponse) GetRatingOk() (*int32, bool)`

GetRatingOk returns a tuple with the Rating field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetRating

`func (o *PiFundMarketDataAPIModelsResponsesFundProfileResponse) SetRating(v int32)`

SetRating sets Rating field to given value.

### HasRating

`func (o *PiFundMarketDataAPIModelsResponsesFundProfileResponse) HasRating() bool`

HasRating returns a boolean if a field has been set.

### SetRatingNil

`func (o *PiFundMarketDataAPIModelsResponsesFundProfileResponse) SetRatingNil(b bool)`

 SetRatingNil sets the value for Rating to be an explicit nil

### UnsetRating
`func (o *PiFundMarketDataAPIModelsResponsesFundProfileResponse) UnsetRating()`

UnsetRating ensures that no value is present for Rating, not even an explicit nil
### GetIsInLegacyMarket

`func (o *PiFundMarketDataAPIModelsResponsesFundProfileResponse) GetIsInLegacyMarket() bool`

GetIsInLegacyMarket returns the IsInLegacyMarket field if non-nil, zero value otherwise.

### GetIsInLegacyMarketOk

`func (o *PiFundMarketDataAPIModelsResponsesFundProfileResponse) GetIsInLegacyMarketOk() (*bool, bool)`

GetIsInLegacyMarketOk returns a tuple with the IsInLegacyMarket field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetIsInLegacyMarket

`func (o *PiFundMarketDataAPIModelsResponsesFundProfileResponse) SetIsInLegacyMarket(v bool)`

SetIsInLegacyMarket sets IsInLegacyMarket field to given value.

### HasIsInLegacyMarket

`func (o *PiFundMarketDataAPIModelsResponsesFundProfileResponse) HasIsInLegacyMarket() bool`

HasIsInLegacyMarket returns a boolean if a field has been set.

### GetAsOfDate

`func (o *PiFundMarketDataAPIModelsResponsesFundProfileResponse) GetAsOfDate() time.Time`

GetAsOfDate returns the AsOfDate field if non-nil, zero value otherwise.

### GetAsOfDateOk

`func (o *PiFundMarketDataAPIModelsResponsesFundProfileResponse) GetAsOfDateOk() (*time.Time, bool)`

GetAsOfDateOk returns a tuple with the AsOfDate field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetAsOfDate

`func (o *PiFundMarketDataAPIModelsResponsesFundProfileResponse) SetAsOfDate(v time.Time)`

SetAsOfDate sets AsOfDate field to given value.

### HasAsOfDate

`func (o *PiFundMarketDataAPIModelsResponsesFundProfileResponse) HasAsOfDate() bool`

HasAsOfDate returns a boolean if a field has been set.

### GetAmcProfile

`func (o *PiFundMarketDataAPIModelsResponsesFundProfileResponse) GetAmcProfile() PiFundMarketDataAPIModelsResponsesAmcProfileResponse`

GetAmcProfile returns the AmcProfile field if non-nil, zero value otherwise.

### GetAmcProfileOk

`func (o *PiFundMarketDataAPIModelsResponsesFundProfileResponse) GetAmcProfileOk() (*PiFundMarketDataAPIModelsResponsesAmcProfileResponse, bool)`

GetAmcProfileOk returns a tuple with the AmcProfile field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetAmcProfile

`func (o *PiFundMarketDataAPIModelsResponsesFundProfileResponse) SetAmcProfile(v PiFundMarketDataAPIModelsResponsesAmcProfileResponse)`

SetAmcProfile sets AmcProfile field to given value.

### HasAmcProfile

`func (o *PiFundMarketDataAPIModelsResponsesFundProfileResponse) HasAmcProfile() bool`

HasAmcProfile returns a boolean if a field has been set.

### GetFundamental

`func (o *PiFundMarketDataAPIModelsResponsesFundProfileResponse) GetFundamental() PiFundMarketDataAPIModelsResponsesFundamentalResponse`

GetFundamental returns the Fundamental field if non-nil, zero value otherwise.

### GetFundamentalOk

`func (o *PiFundMarketDataAPIModelsResponsesFundProfileResponse) GetFundamentalOk() (*PiFundMarketDataAPIModelsResponsesFundamentalResponse, bool)`

GetFundamentalOk returns a tuple with the Fundamental field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetFundamental

`func (o *PiFundMarketDataAPIModelsResponsesFundProfileResponse) SetFundamental(v PiFundMarketDataAPIModelsResponsesFundamentalResponse)`

SetFundamental sets Fundamental field to given value.

### HasFundamental

`func (o *PiFundMarketDataAPIModelsResponsesFundProfileResponse) HasFundamental() bool`

HasFundamental returns a boolean if a field has been set.

### GetAssetValue

`func (o *PiFundMarketDataAPIModelsResponsesFundProfileResponse) GetAssetValue() PiFundMarketDataAPIModelsResponsesAssetValueResponse`

GetAssetValue returns the AssetValue field if non-nil, zero value otherwise.

### GetAssetValueOk

`func (o *PiFundMarketDataAPIModelsResponsesFundProfileResponse) GetAssetValueOk() (*PiFundMarketDataAPIModelsResponsesAssetValueResponse, bool)`

GetAssetValueOk returns a tuple with the AssetValue field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetAssetValue

`func (o *PiFundMarketDataAPIModelsResponsesFundProfileResponse) SetAssetValue(v PiFundMarketDataAPIModelsResponsesAssetValueResponse)`

SetAssetValue sets AssetValue field to given value.

### HasAssetValue

`func (o *PiFundMarketDataAPIModelsResponsesFundProfileResponse) HasAssetValue() bool`

HasAssetValue returns a boolean if a field has been set.

### GetAssetAllocation

`func (o *PiFundMarketDataAPIModelsResponsesFundProfileResponse) GetAssetAllocation() PiFundMarketDataAPIModelsResponsesAssetAllocationResponse`

GetAssetAllocation returns the AssetAllocation field if non-nil, zero value otherwise.

### GetAssetAllocationOk

`func (o *PiFundMarketDataAPIModelsResponsesFundProfileResponse) GetAssetAllocationOk() (*PiFundMarketDataAPIModelsResponsesAssetAllocationResponse, bool)`

GetAssetAllocationOk returns a tuple with the AssetAllocation field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetAssetAllocation

`func (o *PiFundMarketDataAPIModelsResponsesFundProfileResponse) SetAssetAllocation(v PiFundMarketDataAPIModelsResponsesAssetAllocationResponse)`

SetAssetAllocation sets AssetAllocation field to given value.

### HasAssetAllocation

`func (o *PiFundMarketDataAPIModelsResponsesFundProfileResponse) HasAssetAllocation() bool`

HasAssetAllocation returns a boolean if a field has been set.

### GetPurchaseDetail

`func (o *PiFundMarketDataAPIModelsResponsesFundProfileResponse) GetPurchaseDetail() PiFundMarketDataAPIModelsResponsesPurchaseDetailResponse`

GetPurchaseDetail returns the PurchaseDetail field if non-nil, zero value otherwise.

### GetPurchaseDetailOk

`func (o *PiFundMarketDataAPIModelsResponsesFundProfileResponse) GetPurchaseDetailOk() (*PiFundMarketDataAPIModelsResponsesPurchaseDetailResponse, bool)`

GetPurchaseDetailOk returns a tuple with the PurchaseDetail field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetPurchaseDetail

`func (o *PiFundMarketDataAPIModelsResponsesFundProfileResponse) SetPurchaseDetail(v PiFundMarketDataAPIModelsResponsesPurchaseDetailResponse)`

SetPurchaseDetail sets PurchaseDetail field to given value.

### HasPurchaseDetail

`func (o *PiFundMarketDataAPIModelsResponsesFundProfileResponse) HasPurchaseDetail() bool`

HasPurchaseDetail returns a boolean if a field has been set.

### GetFeesAndExpense

`func (o *PiFundMarketDataAPIModelsResponsesFundProfileResponse) GetFeesAndExpense() PiFundMarketDataAPIModelsResponsesFeeResponse`

GetFeesAndExpense returns the FeesAndExpense field if non-nil, zero value otherwise.

### GetFeesAndExpenseOk

`func (o *PiFundMarketDataAPIModelsResponsesFundProfileResponse) GetFeesAndExpenseOk() (*PiFundMarketDataAPIModelsResponsesFeeResponse, bool)`

GetFeesAndExpenseOk returns a tuple with the FeesAndExpense field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetFeesAndExpense

`func (o *PiFundMarketDataAPIModelsResponsesFundProfileResponse) SetFeesAndExpense(v PiFundMarketDataAPIModelsResponsesFeeResponse)`

SetFeesAndExpense sets FeesAndExpense field to given value.

### HasFeesAndExpense

`func (o *PiFundMarketDataAPIModelsResponsesFundProfileResponse) HasFeesAndExpense() bool`

HasFeesAndExpense returns a boolean if a field has been set.

### GetPerformance

`func (o *PiFundMarketDataAPIModelsResponsesFundProfileResponse) GetPerformance() PiFundMarketDataAPIModelsResponsesPerformanceResponse`

GetPerformance returns the Performance field if non-nil, zero value otherwise.

### GetPerformanceOk

`func (o *PiFundMarketDataAPIModelsResponsesFundProfileResponse) GetPerformanceOk() (*PiFundMarketDataAPIModelsResponsesPerformanceResponse, bool)`

GetPerformanceOk returns a tuple with the Performance field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetPerformance

`func (o *PiFundMarketDataAPIModelsResponsesFundProfileResponse) SetPerformance(v PiFundMarketDataAPIModelsResponsesPerformanceResponse)`

SetPerformance sets Performance field to given value.

### HasPerformance

`func (o *PiFundMarketDataAPIModelsResponsesFundProfileResponse) HasPerformance() bool`

HasPerformance returns a boolean if a field has been set.

### GetDistribution

`func (o *PiFundMarketDataAPIModelsResponsesFundProfileResponse) GetDistribution() PiFundMarketDataAPIModelsResponsesDistributionResponse`

GetDistribution returns the Distribution field if non-nil, zero value otherwise.

### GetDistributionOk

`func (o *PiFundMarketDataAPIModelsResponsesFundProfileResponse) GetDistributionOk() (*PiFundMarketDataAPIModelsResponsesDistributionResponse, bool)`

GetDistributionOk returns a tuple with the Distribution field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetDistribution

`func (o *PiFundMarketDataAPIModelsResponsesFundProfileResponse) SetDistribution(v PiFundMarketDataAPIModelsResponsesDistributionResponse)`

SetDistribution sets Distribution field to given value.

### HasDistribution

`func (o *PiFundMarketDataAPIModelsResponsesFundProfileResponse) HasDistribution() bool`

HasDistribution returns a boolean if a field has been set.


[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


