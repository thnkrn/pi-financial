# PiFundMarketDataAPIModelsResponsesFundTradingProfileResponse

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**Symbol** | Pointer to **NullableString** |  | [optional] 
**Name** | Pointer to **NullableString** |  | [optional] 
**FundCategory** | Pointer to **NullableString** |  | [optional] 
**Fundamental** | Pointer to [**PiFundMarketDataAPIModelsResponsesFundamentalTradingResponse**](PiFundMarketDataAPIModelsResponsesFundamentalTradingResponse.md) |  | [optional] 
**AmcProfile** | Pointer to [**PiFundMarketDataAPIModelsResponsesAmcProfileTradingResponse**](PiFundMarketDataAPIModelsResponsesAmcProfileTradingResponse.md) |  | [optional] 
**PurchaseDetail** | Pointer to [**PiFundMarketDataAPIModelsResponsesPurchaseDetailTradingResponse**](PiFundMarketDataAPIModelsResponsesPurchaseDetailTradingResponse.md) |  | [optional] 
**Nav** | Pointer to **float32** |  | [optional] 

## Methods

### NewPiFundMarketDataAPIModelsResponsesFundTradingProfileResponse

`func NewPiFundMarketDataAPIModelsResponsesFundTradingProfileResponse() *PiFundMarketDataAPIModelsResponsesFundTradingProfileResponse`

NewPiFundMarketDataAPIModelsResponsesFundTradingProfileResponse instantiates a new PiFundMarketDataAPIModelsResponsesFundTradingProfileResponse object
This constructor will assign default values to properties that have it defined,
and makes sure properties required by API are set, but the set of arguments
will change when the set of required properties is changed

### NewPiFundMarketDataAPIModelsResponsesFundTradingProfileResponseWithDefaults

`func NewPiFundMarketDataAPIModelsResponsesFundTradingProfileResponseWithDefaults() *PiFundMarketDataAPIModelsResponsesFundTradingProfileResponse`

NewPiFundMarketDataAPIModelsResponsesFundTradingProfileResponseWithDefaults instantiates a new PiFundMarketDataAPIModelsResponsesFundTradingProfileResponse object
This constructor will only assign default values to properties that have it defined,
but it doesn't guarantee that properties required by API are set

### GetSymbol

`func (o *PiFundMarketDataAPIModelsResponsesFundTradingProfileResponse) GetSymbol() string`

GetSymbol returns the Symbol field if non-nil, zero value otherwise.

### GetSymbolOk

`func (o *PiFundMarketDataAPIModelsResponsesFundTradingProfileResponse) GetSymbolOk() (*string, bool)`

GetSymbolOk returns a tuple with the Symbol field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetSymbol

`func (o *PiFundMarketDataAPIModelsResponsesFundTradingProfileResponse) SetSymbol(v string)`

SetSymbol sets Symbol field to given value.

### HasSymbol

`func (o *PiFundMarketDataAPIModelsResponsesFundTradingProfileResponse) HasSymbol() bool`

HasSymbol returns a boolean if a field has been set.

### SetSymbolNil

`func (o *PiFundMarketDataAPIModelsResponsesFundTradingProfileResponse) SetSymbolNil(b bool)`

 SetSymbolNil sets the value for Symbol to be an explicit nil

### UnsetSymbol
`func (o *PiFundMarketDataAPIModelsResponsesFundTradingProfileResponse) UnsetSymbol()`

UnsetSymbol ensures that no value is present for Symbol, not even an explicit nil
### GetName

`func (o *PiFundMarketDataAPIModelsResponsesFundTradingProfileResponse) GetName() string`

GetName returns the Name field if non-nil, zero value otherwise.

### GetNameOk

`func (o *PiFundMarketDataAPIModelsResponsesFundTradingProfileResponse) GetNameOk() (*string, bool)`

GetNameOk returns a tuple with the Name field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetName

`func (o *PiFundMarketDataAPIModelsResponsesFundTradingProfileResponse) SetName(v string)`

SetName sets Name field to given value.

### HasName

`func (o *PiFundMarketDataAPIModelsResponsesFundTradingProfileResponse) HasName() bool`

HasName returns a boolean if a field has been set.

### SetNameNil

`func (o *PiFundMarketDataAPIModelsResponsesFundTradingProfileResponse) SetNameNil(b bool)`

 SetNameNil sets the value for Name to be an explicit nil

### UnsetName
`func (o *PiFundMarketDataAPIModelsResponsesFundTradingProfileResponse) UnsetName()`

UnsetName ensures that no value is present for Name, not even an explicit nil
### GetFundCategory

`func (o *PiFundMarketDataAPIModelsResponsesFundTradingProfileResponse) GetFundCategory() string`

GetFundCategory returns the FundCategory field if non-nil, zero value otherwise.

### GetFundCategoryOk

`func (o *PiFundMarketDataAPIModelsResponsesFundTradingProfileResponse) GetFundCategoryOk() (*string, bool)`

GetFundCategoryOk returns a tuple with the FundCategory field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetFundCategory

`func (o *PiFundMarketDataAPIModelsResponsesFundTradingProfileResponse) SetFundCategory(v string)`

SetFundCategory sets FundCategory field to given value.

### HasFundCategory

`func (o *PiFundMarketDataAPIModelsResponsesFundTradingProfileResponse) HasFundCategory() bool`

HasFundCategory returns a boolean if a field has been set.

### SetFundCategoryNil

`func (o *PiFundMarketDataAPIModelsResponsesFundTradingProfileResponse) SetFundCategoryNil(b bool)`

 SetFundCategoryNil sets the value for FundCategory to be an explicit nil

### UnsetFundCategory
`func (o *PiFundMarketDataAPIModelsResponsesFundTradingProfileResponse) UnsetFundCategory()`

UnsetFundCategory ensures that no value is present for FundCategory, not even an explicit nil
### GetFundamental

`func (o *PiFundMarketDataAPIModelsResponsesFundTradingProfileResponse) GetFundamental() PiFundMarketDataAPIModelsResponsesFundamentalTradingResponse`

GetFundamental returns the Fundamental field if non-nil, zero value otherwise.

### GetFundamentalOk

`func (o *PiFundMarketDataAPIModelsResponsesFundTradingProfileResponse) GetFundamentalOk() (*PiFundMarketDataAPIModelsResponsesFundamentalTradingResponse, bool)`

GetFundamentalOk returns a tuple with the Fundamental field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetFundamental

`func (o *PiFundMarketDataAPIModelsResponsesFundTradingProfileResponse) SetFundamental(v PiFundMarketDataAPIModelsResponsesFundamentalTradingResponse)`

SetFundamental sets Fundamental field to given value.

### HasFundamental

`func (o *PiFundMarketDataAPIModelsResponsesFundTradingProfileResponse) HasFundamental() bool`

HasFundamental returns a boolean if a field has been set.

### GetAmcProfile

`func (o *PiFundMarketDataAPIModelsResponsesFundTradingProfileResponse) GetAmcProfile() PiFundMarketDataAPIModelsResponsesAmcProfileTradingResponse`

GetAmcProfile returns the AmcProfile field if non-nil, zero value otherwise.

### GetAmcProfileOk

`func (o *PiFundMarketDataAPIModelsResponsesFundTradingProfileResponse) GetAmcProfileOk() (*PiFundMarketDataAPIModelsResponsesAmcProfileTradingResponse, bool)`

GetAmcProfileOk returns a tuple with the AmcProfile field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetAmcProfile

`func (o *PiFundMarketDataAPIModelsResponsesFundTradingProfileResponse) SetAmcProfile(v PiFundMarketDataAPIModelsResponsesAmcProfileTradingResponse)`

SetAmcProfile sets AmcProfile field to given value.

### HasAmcProfile

`func (o *PiFundMarketDataAPIModelsResponsesFundTradingProfileResponse) HasAmcProfile() bool`

HasAmcProfile returns a boolean if a field has been set.

### GetPurchaseDetail

`func (o *PiFundMarketDataAPIModelsResponsesFundTradingProfileResponse) GetPurchaseDetail() PiFundMarketDataAPIModelsResponsesPurchaseDetailTradingResponse`

GetPurchaseDetail returns the PurchaseDetail field if non-nil, zero value otherwise.

### GetPurchaseDetailOk

`func (o *PiFundMarketDataAPIModelsResponsesFundTradingProfileResponse) GetPurchaseDetailOk() (*PiFundMarketDataAPIModelsResponsesPurchaseDetailTradingResponse, bool)`

GetPurchaseDetailOk returns a tuple with the PurchaseDetail field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetPurchaseDetail

`func (o *PiFundMarketDataAPIModelsResponsesFundTradingProfileResponse) SetPurchaseDetail(v PiFundMarketDataAPIModelsResponsesPurchaseDetailTradingResponse)`

SetPurchaseDetail sets PurchaseDetail field to given value.

### HasPurchaseDetail

`func (o *PiFundMarketDataAPIModelsResponsesFundTradingProfileResponse) HasPurchaseDetail() bool`

HasPurchaseDetail returns a boolean if a field has been set.

### GetNav

`func (o *PiFundMarketDataAPIModelsResponsesFundTradingProfileResponse) GetNav() float32`

GetNav returns the Nav field if non-nil, zero value otherwise.

### GetNavOk

`func (o *PiFundMarketDataAPIModelsResponsesFundTradingProfileResponse) GetNavOk() (*float32, bool)`

GetNavOk returns a tuple with the Nav field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetNav

`func (o *PiFundMarketDataAPIModelsResponsesFundTradingProfileResponse) SetNav(v float32)`

SetNav sets Nav field to given value.

### HasNav

`func (o *PiFundMarketDataAPIModelsResponsesFundTradingProfileResponse) HasNav() bool`

HasNav returns a boolean if a field has been set.


[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


