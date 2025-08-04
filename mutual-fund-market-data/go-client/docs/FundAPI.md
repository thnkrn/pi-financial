# \FundAPI

All URIs are relative to *http://localhost*

Method | HTTP request | Description
------------- | ------------- | -------------
[**InternalFundsSearchGet**](FundAPI.md#InternalFundsSearchGet) | **Get** /internal/funds/search | 
[**InternalFundsSymbolProfileGet**](FundAPI.md#InternalFundsSymbolProfileGet) | **Get** /internal/funds/{symbol}/profile | 
[**InternalFundsSymbolTradableDatesGet**](FundAPI.md#InternalFundsSymbolTradableDatesGet) | **Get** /internal/funds/{symbol}/tradable-dates | 
[**InternalFundsTradingProfilesPost**](FundAPI.md#InternalFundsTradingProfilesPost) | **Post** /internal/funds/trading-profiles | 
[**SecureFundsMarketBasketMarketSummariesGet**](FundAPI.md#SecureFundsMarketBasketMarketSummariesGet) | **Get** /secure/funds/{marketBasket}/market-summaries | 
[**SecureFundsMarketBasketMarketSummariesV2Get**](FundAPI.md#SecureFundsMarketBasketMarketSummariesV2Get) | **Get** /secure/funds/{marketBasket}/market-summaries/v2 | 
[**SecureFundsProfilesPost**](FundAPI.md#SecureFundsProfilesPost) | **Post** /secure/funds/profiles | 
[**SecureFundsSearchGet**](FundAPI.md#SecureFundsSearchGet) | **Get** /secure/funds/search | 
[**SecureFundsSymbolHistoricalNavGet**](FundAPI.md#SecureFundsSymbolHistoricalNavGet) | **Get** /secure/funds/{symbol}/historical-nav | 
[**SecureFundsSymbolProfileGet**](FundAPI.md#SecureFundsSymbolProfileGet) | **Get** /secure/funds/{symbol}/profile | 
[**SecureFundsSymbolSwitchingFundsGet**](FundAPI.md#SecureFundsSymbolSwitchingFundsGet) | **Get** /secure/funds/{symbol}/switching-funds | 
[**SecureFundsSymbolTradableDatesGet**](FundAPI.md#SecureFundsSymbolTradableDatesGet) | **Get** /secure/funds/{symbol}/tradable-dates | 
[**SecureLegacyFundsSummariesPost**](FundAPI.md#SecureLegacyFundsSummariesPost) | **Post** /secure/legacy/funds/summaries | 



## InternalFundsSearchGet

> PiFundMarketDataAPIModelsResponsesFundSearchResponseIEnumerableApiResponse InternalFundsSearchGet(ctx).Keyword(keyword).Execute()



### Example

```go
package main

import (
	"context"
	"fmt"
	"os"
	openapiclient "github.com/pi-financial/mutual-fund-market-data/go-client"
)

func main() {
	keyword := "keyword_example" // string |  (optional)

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	resp, r, err := apiClient.FundAPI.InternalFundsSearchGet(context.Background()).Keyword(keyword).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `FundAPI.InternalFundsSearchGet``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `InternalFundsSearchGet`: PiFundMarketDataAPIModelsResponsesFundSearchResponseIEnumerableApiResponse
	fmt.Fprintf(os.Stdout, "Response from `FundAPI.InternalFundsSearchGet`: %v\n", resp)
}
```

### Path Parameters



### Other Parameters

Other parameters are passed through a pointer to a apiInternalFundsSearchGetRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **keyword** | **string** |  | 

### Return type

[**PiFundMarketDataAPIModelsResponsesFundSearchResponseIEnumerableApiResponse**](PiFundMarketDataAPIModelsResponsesFundSearchResponseIEnumerableApiResponse.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: text/plain, application/json, text/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)


## InternalFundsSymbolProfileGet

> PiFundMarketDataAPIModelsResponsesFundProfileResponseApiResponse InternalFundsSymbolProfileGet(ctx, symbol).Execute()



### Example

```go
package main

import (
	"context"
	"fmt"
	"os"
	openapiclient "github.com/pi-financial/mutual-fund-market-data/go-client"
)

func main() {
	symbol := "symbol_example" // string | 

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	resp, r, err := apiClient.FundAPI.InternalFundsSymbolProfileGet(context.Background(), symbol).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `FundAPI.InternalFundsSymbolProfileGet``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `InternalFundsSymbolProfileGet`: PiFundMarketDataAPIModelsResponsesFundProfileResponseApiResponse
	fmt.Fprintf(os.Stdout, "Response from `FundAPI.InternalFundsSymbolProfileGet`: %v\n", resp)
}
```

### Path Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
**ctx** | **context.Context** | context for authentication, logging, cancellation, deadlines, tracing, etc.
**symbol** | **string** |  | 

### Other Parameters

Other parameters are passed through a pointer to a apiInternalFundsSymbolProfileGetRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------


### Return type

[**PiFundMarketDataAPIModelsResponsesFundProfileResponseApiResponse**](PiFundMarketDataAPIModelsResponsesFundProfileResponseApiResponse.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: text/plain, application/json, text/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)


## InternalFundsSymbolTradableDatesGet

> SystemDateTimeIEnumerableApiResponse InternalFundsSymbolTradableDatesGet(ctx, symbol).TradeType(tradeType).SwitchSymbol(switchSymbol).Execute()



### Example

```go
package main

import (
	"context"
	"fmt"
	"os"
	openapiclient "github.com/pi-financial/mutual-fund-market-data/go-client"
)

func main() {
	symbol := "symbol_example" // string | 
	tradeType := openapiclient.PiFundMarketDataConstantsTradeSide("Buy") // PiFundMarketDataConstantsTradeSide | 
	switchSymbol := "switchSymbol_example" // string |  (optional)

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	resp, r, err := apiClient.FundAPI.InternalFundsSymbolTradableDatesGet(context.Background(), symbol).TradeType(tradeType).SwitchSymbol(switchSymbol).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `FundAPI.InternalFundsSymbolTradableDatesGet``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `InternalFundsSymbolTradableDatesGet`: SystemDateTimeIEnumerableApiResponse
	fmt.Fprintf(os.Stdout, "Response from `FundAPI.InternalFundsSymbolTradableDatesGet`: %v\n", resp)
}
```

### Path Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
**ctx** | **context.Context** | context for authentication, logging, cancellation, deadlines, tracing, etc.
**symbol** | **string** |  | 

### Other Parameters

Other parameters are passed through a pointer to a apiInternalFundsSymbolTradableDatesGetRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------

 **tradeType** | [**PiFundMarketDataConstantsTradeSide**](PiFundMarketDataConstantsTradeSide.md) |  | 
 **switchSymbol** | **string** |  | 

### Return type

[**SystemDateTimeIEnumerableApiResponse**](SystemDateTimeIEnumerableApiResponse.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: text/plain, application/json, text/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)


## InternalFundsTradingProfilesPost

> PiFundMarketDataAPIModelsResponsesFundTradingProfileResponseIEnumerableApiResponse InternalFundsTradingProfilesPost(ctx).RequestBody(requestBody).Execute()



### Example

```go
package main

import (
	"context"
	"fmt"
	"os"
	openapiclient "github.com/pi-financial/mutual-fund-market-data/go-client"
)

func main() {
	requestBody := []string{"Property_example"} // []string | 

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	resp, r, err := apiClient.FundAPI.InternalFundsTradingProfilesPost(context.Background()).RequestBody(requestBody).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `FundAPI.InternalFundsTradingProfilesPost``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `InternalFundsTradingProfilesPost`: PiFundMarketDataAPIModelsResponsesFundTradingProfileResponseIEnumerableApiResponse
	fmt.Fprintf(os.Stdout, "Response from `FundAPI.InternalFundsTradingProfilesPost`: %v\n", resp)
}
```

### Path Parameters



### Other Parameters

Other parameters are passed through a pointer to a apiInternalFundsTradingProfilesPostRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **requestBody** | **[]string** |  | 

### Return type

[**PiFundMarketDataAPIModelsResponsesFundTradingProfileResponseIEnumerableApiResponse**](PiFundMarketDataAPIModelsResponsesFundTradingProfileResponseIEnumerableApiResponse.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: application/json, text/json, application/*+json
- **Accept**: text/plain, application/json, text/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)


## SecureFundsMarketBasketMarketSummariesGet

> PiFundMarketDataAPIModelsResponsesFundMarketSummaryResponseIEnumerableApiResponse SecureFundsMarketBasketMarketSummariesGet(ctx, marketBasket).Interval(interval).Execute()



### Example

```go
package main

import (
	"context"
	"fmt"
	"os"
	openapiclient "github.com/pi-financial/mutual-fund-market-data/go-client"
)

func main() {
	marketBasket := openapiclient.PiFundMarketDataConstantsMarketBasket("TopFund") // PiFundMarketDataConstantsMarketBasket | 
	interval := openapiclient.PiFundMarketDataConstantsInterval("Over3Months") // PiFundMarketDataConstantsInterval | 

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	resp, r, err := apiClient.FundAPI.SecureFundsMarketBasketMarketSummariesGet(context.Background(), marketBasket).Interval(interval).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `FundAPI.SecureFundsMarketBasketMarketSummariesGet``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `SecureFundsMarketBasketMarketSummariesGet`: PiFundMarketDataAPIModelsResponsesFundMarketSummaryResponseIEnumerableApiResponse
	fmt.Fprintf(os.Stdout, "Response from `FundAPI.SecureFundsMarketBasketMarketSummariesGet`: %v\n", resp)
}
```

### Path Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
**ctx** | **context.Context** | context for authentication, logging, cancellation, deadlines, tracing, etc.
**marketBasket** | [**PiFundMarketDataConstantsMarketBasket**](.md) |  | 

### Other Parameters

Other parameters are passed through a pointer to a apiSecureFundsMarketBasketMarketSummariesGetRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------

 **interval** | [**PiFundMarketDataConstantsInterval**](PiFundMarketDataConstantsInterval.md) |  | 

### Return type

[**PiFundMarketDataAPIModelsResponsesFundMarketSummaryResponseIEnumerableApiResponse**](PiFundMarketDataAPIModelsResponsesFundMarketSummaryResponseIEnumerableApiResponse.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: text/plain, application/json, text/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)


## SecureFundsMarketBasketMarketSummariesV2Get

> PiFundMarketDataAPIModelsResponsesFundMarketSummaryResponseIEnumerableApiPaginateResponse SecureFundsMarketBasketMarketSummariesV2Get(ctx, marketBasket).Interval(interval).Page(page).PageSize(pageSize).Execute()



### Example

```go
package main

import (
	"context"
	"fmt"
	"os"
	openapiclient "github.com/pi-financial/mutual-fund-market-data/go-client"
)

func main() {
	marketBasket := openapiclient.PiFundMarketDataConstantsMarketBasket("TopFund") // PiFundMarketDataConstantsMarketBasket | 
	interval := openapiclient.PiFundMarketDataConstantsInterval("Over3Months") // PiFundMarketDataConstantsInterval | 
	page := int32(56) // int32 |  (optional) (default to 1)
	pageSize := int32(56) // int32 |  (optional) (default to 20)

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	resp, r, err := apiClient.FundAPI.SecureFundsMarketBasketMarketSummariesV2Get(context.Background(), marketBasket).Interval(interval).Page(page).PageSize(pageSize).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `FundAPI.SecureFundsMarketBasketMarketSummariesV2Get``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `SecureFundsMarketBasketMarketSummariesV2Get`: PiFundMarketDataAPIModelsResponsesFundMarketSummaryResponseIEnumerableApiPaginateResponse
	fmt.Fprintf(os.Stdout, "Response from `FundAPI.SecureFundsMarketBasketMarketSummariesV2Get`: %v\n", resp)
}
```

### Path Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
**ctx** | **context.Context** | context for authentication, logging, cancellation, deadlines, tracing, etc.
**marketBasket** | [**PiFundMarketDataConstantsMarketBasket**](.md) |  | 

### Other Parameters

Other parameters are passed through a pointer to a apiSecureFundsMarketBasketMarketSummariesV2GetRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------

 **interval** | [**PiFundMarketDataConstantsInterval**](PiFundMarketDataConstantsInterval.md) |  | 
 **page** | **int32** |  | [default to 1]
 **pageSize** | **int32** |  | [default to 20]

### Return type

[**PiFundMarketDataAPIModelsResponsesFundMarketSummaryResponseIEnumerableApiPaginateResponse**](PiFundMarketDataAPIModelsResponsesFundMarketSummaryResponseIEnumerableApiPaginateResponse.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: text/plain, application/json, text/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)


## SecureFundsProfilesPost

> PiFundMarketDataAPIModelsResponsesFundProfileResponseIEnumerableApiResponse SecureFundsProfilesPost(ctx).RequestBody(requestBody).Execute()



### Example

```go
package main

import (
	"context"
	"fmt"
	"os"
	openapiclient "github.com/pi-financial/mutual-fund-market-data/go-client"
)

func main() {
	requestBody := []string{"Property_example"} // []string | 

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	resp, r, err := apiClient.FundAPI.SecureFundsProfilesPost(context.Background()).RequestBody(requestBody).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `FundAPI.SecureFundsProfilesPost``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `SecureFundsProfilesPost`: PiFundMarketDataAPIModelsResponsesFundProfileResponseIEnumerableApiResponse
	fmt.Fprintf(os.Stdout, "Response from `FundAPI.SecureFundsProfilesPost`: %v\n", resp)
}
```

### Path Parameters



### Other Parameters

Other parameters are passed through a pointer to a apiSecureFundsProfilesPostRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **requestBody** | **[]string** |  | 

### Return type

[**PiFundMarketDataAPIModelsResponsesFundProfileResponseIEnumerableApiResponse**](PiFundMarketDataAPIModelsResponsesFundProfileResponseIEnumerableApiResponse.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: application/json, text/json, application/*+json
- **Accept**: text/plain, application/json, text/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)


## SecureFundsSearchGet

> PiFundMarketDataAPIModelsResponsesFundSearchResponseIEnumerableApiResponse SecureFundsSearchGet(ctx).Keyword(keyword).Execute()



### Example

```go
package main

import (
	"context"
	"fmt"
	"os"
	openapiclient "github.com/pi-financial/mutual-fund-market-data/go-client"
)

func main() {
	keyword := "keyword_example" // string |  (optional)

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	resp, r, err := apiClient.FundAPI.SecureFundsSearchGet(context.Background()).Keyword(keyword).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `FundAPI.SecureFundsSearchGet``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `SecureFundsSearchGet`: PiFundMarketDataAPIModelsResponsesFundSearchResponseIEnumerableApiResponse
	fmt.Fprintf(os.Stdout, "Response from `FundAPI.SecureFundsSearchGet`: %v\n", resp)
}
```

### Path Parameters



### Other Parameters

Other parameters are passed through a pointer to a apiSecureFundsSearchGetRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **keyword** | **string** |  | 

### Return type

[**PiFundMarketDataAPIModelsResponsesFundSearchResponseIEnumerableApiResponse**](PiFundMarketDataAPIModelsResponsesFundSearchResponseIEnumerableApiResponse.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: text/plain, application/json, text/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)


## SecureFundsSymbolHistoricalNavGet

> PiFundMarketDataAPIModelsResponsesFundHistoricalNavResponseApiResponse SecureFundsSymbolHistoricalNavGet(ctx, symbol).Interval(interval).Execute()



### Example

```go
package main

import (
	"context"
	"fmt"
	"os"
	openapiclient "github.com/pi-financial/mutual-fund-market-data/go-client"
)

func main() {
	symbol := "symbol_example" // string | 
	interval := "interval_example" // string |  (optional)

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	resp, r, err := apiClient.FundAPI.SecureFundsSymbolHistoricalNavGet(context.Background(), symbol).Interval(interval).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `FundAPI.SecureFundsSymbolHistoricalNavGet``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `SecureFundsSymbolHistoricalNavGet`: PiFundMarketDataAPIModelsResponsesFundHistoricalNavResponseApiResponse
	fmt.Fprintf(os.Stdout, "Response from `FundAPI.SecureFundsSymbolHistoricalNavGet`: %v\n", resp)
}
```

### Path Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
**ctx** | **context.Context** | context for authentication, logging, cancellation, deadlines, tracing, etc.
**symbol** | **string** |  | 

### Other Parameters

Other parameters are passed through a pointer to a apiSecureFundsSymbolHistoricalNavGetRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------

 **interval** | **string** |  | 

### Return type

[**PiFundMarketDataAPIModelsResponsesFundHistoricalNavResponseApiResponse**](PiFundMarketDataAPIModelsResponsesFundHistoricalNavResponseApiResponse.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: text/plain, application/json, text/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)


## SecureFundsSymbolProfileGet

> PiFundMarketDataAPIModelsResponsesFundProfileResponseApiResponse SecureFundsSymbolProfileGet(ctx, symbol).Execute()



### Example

```go
package main

import (
	"context"
	"fmt"
	"os"
	openapiclient "github.com/pi-financial/mutual-fund-market-data/go-client"
)

func main() {
	symbol := "symbol_example" // string | 

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	resp, r, err := apiClient.FundAPI.SecureFundsSymbolProfileGet(context.Background(), symbol).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `FundAPI.SecureFundsSymbolProfileGet``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `SecureFundsSymbolProfileGet`: PiFundMarketDataAPIModelsResponsesFundProfileResponseApiResponse
	fmt.Fprintf(os.Stdout, "Response from `FundAPI.SecureFundsSymbolProfileGet`: %v\n", resp)
}
```

### Path Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
**ctx** | **context.Context** | context for authentication, logging, cancellation, deadlines, tracing, etc.
**symbol** | **string** |  | 

### Other Parameters

Other parameters are passed through a pointer to a apiSecureFundsSymbolProfileGetRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------


### Return type

[**PiFundMarketDataAPIModelsResponsesFundProfileResponseApiResponse**](PiFundMarketDataAPIModelsResponsesFundProfileResponseApiResponse.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: text/plain, application/json, text/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)


## SecureFundsSymbolSwitchingFundsGet

> PiFundMarketDataAPIModelsResponsesSwitchingFundResponseApiResponse SecureFundsSymbolSwitchingFundsGet(ctx, symbol).Execute()



### Example

```go
package main

import (
	"context"
	"fmt"
	"os"
	openapiclient "github.com/pi-financial/mutual-fund-market-data/go-client"
)

func main() {
	symbol := "symbol_example" // string | 

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	resp, r, err := apiClient.FundAPI.SecureFundsSymbolSwitchingFundsGet(context.Background(), symbol).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `FundAPI.SecureFundsSymbolSwitchingFundsGet``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `SecureFundsSymbolSwitchingFundsGet`: PiFundMarketDataAPIModelsResponsesSwitchingFundResponseApiResponse
	fmt.Fprintf(os.Stdout, "Response from `FundAPI.SecureFundsSymbolSwitchingFundsGet`: %v\n", resp)
}
```

### Path Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
**ctx** | **context.Context** | context for authentication, logging, cancellation, deadlines, tracing, etc.
**symbol** | **string** |  | 

### Other Parameters

Other parameters are passed through a pointer to a apiSecureFundsSymbolSwitchingFundsGetRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------


### Return type

[**PiFundMarketDataAPIModelsResponsesSwitchingFundResponseApiResponse**](PiFundMarketDataAPIModelsResponsesSwitchingFundResponseApiResponse.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: text/plain, application/json, text/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)


## SecureFundsSymbolTradableDatesGet

> SystemDateTimeIEnumerableApiResponse SecureFundsSymbolTradableDatesGet(ctx, symbol).TradeType(tradeType).SwitchSymbol(switchSymbol).Execute()



### Example

```go
package main

import (
	"context"
	"fmt"
	"os"
	openapiclient "github.com/pi-financial/mutual-fund-market-data/go-client"
)

func main() {
	symbol := "symbol_example" // string | 
	tradeType := openapiclient.PiFundMarketDataConstantsTradeSide("Buy") // PiFundMarketDataConstantsTradeSide | 
	switchSymbol := "switchSymbol_example" // string |  (optional)

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	resp, r, err := apiClient.FundAPI.SecureFundsSymbolTradableDatesGet(context.Background(), symbol).TradeType(tradeType).SwitchSymbol(switchSymbol).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `FundAPI.SecureFundsSymbolTradableDatesGet``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `SecureFundsSymbolTradableDatesGet`: SystemDateTimeIEnumerableApiResponse
	fmt.Fprintf(os.Stdout, "Response from `FundAPI.SecureFundsSymbolTradableDatesGet`: %v\n", resp)
}
```

### Path Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
**ctx** | **context.Context** | context for authentication, logging, cancellation, deadlines, tracing, etc.
**symbol** | **string** |  | 

### Other Parameters

Other parameters are passed through a pointer to a apiSecureFundsSymbolTradableDatesGetRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------

 **tradeType** | [**PiFundMarketDataConstantsTradeSide**](PiFundMarketDataConstantsTradeSide.md) |  | 
 **switchSymbol** | **string** |  | 

### Return type

[**SystemDateTimeIEnumerableApiResponse**](SystemDateTimeIEnumerableApiResponse.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: text/plain, application/json, text/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)


## SecureLegacyFundsSummariesPost

> PiFundMarketDataAPIModelsResponsesLegacyFundMarketSummaryResponseIEnumerableApiResponse SecureLegacyFundsSummariesPost(ctx).PiFundMarketDataAPIModelsRequestsLegacySymbolsRequest(piFundMarketDataAPIModelsRequestsLegacySymbolsRequest).Execute()



### Example

```go
package main

import (
	"context"
	"fmt"
	"os"
	openapiclient "github.com/pi-financial/mutual-fund-market-data/go-client"
)

func main() {
	piFundMarketDataAPIModelsRequestsLegacySymbolsRequest := *openapiclient.NewPiFundMarketDataAPIModelsRequestsLegacySymbolsRequest([]string{"Symbols_example"}) // PiFundMarketDataAPIModelsRequestsLegacySymbolsRequest | 

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	resp, r, err := apiClient.FundAPI.SecureLegacyFundsSummariesPost(context.Background()).PiFundMarketDataAPIModelsRequestsLegacySymbolsRequest(piFundMarketDataAPIModelsRequestsLegacySymbolsRequest).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `FundAPI.SecureLegacyFundsSummariesPost``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `SecureLegacyFundsSummariesPost`: PiFundMarketDataAPIModelsResponsesLegacyFundMarketSummaryResponseIEnumerableApiResponse
	fmt.Fprintf(os.Stdout, "Response from `FundAPI.SecureLegacyFundsSummariesPost`: %v\n", resp)
}
```

### Path Parameters



### Other Parameters

Other parameters are passed through a pointer to a apiSecureLegacyFundsSummariesPostRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **piFundMarketDataAPIModelsRequestsLegacySymbolsRequest** | [**PiFundMarketDataAPIModelsRequestsLegacySymbolsRequest**](PiFundMarketDataAPIModelsRequestsLegacySymbolsRequest.md) |  | 

### Return type

[**PiFundMarketDataAPIModelsResponsesLegacyFundMarketSummaryResponseIEnumerableApiResponse**](PiFundMarketDataAPIModelsResponsesLegacyFundMarketSummaryResponseIEnumerableApiResponse.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: application/json, text/json, application/*+json
- **Accept**: text/plain, application/json, text/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)

