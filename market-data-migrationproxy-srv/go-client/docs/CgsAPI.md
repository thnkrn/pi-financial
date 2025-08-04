# \CgsAPI

All URIs are relative to *http://localhost*

Method | HTTP request | Description
------------- | ------------- | -------------
[**HomeInstruments**](CgsAPI.md#HomeInstruments) | **Post** /cgs/v2/home/instruments | 
[**MarketBrokerInfo**](CgsAPI.md#MarketBrokerInfo) | **Post** /cgs/v1/market/brokerinfo | 
[**MarketDerivativeInformation**](CgsAPI.md#MarketDerivativeInformation) | **Post** /cgs/v1/market/derivative/information | 
[**MarketFilterInstruments**](CgsAPI.md#MarketFilterInstruments) | **Post** /cgs/v2/market/filter/instruments | 
[**MarketFilters**](CgsAPI.md#MarketFilters) | **Post** /cgs/v2/market/filters | 
[**MarketGlobalEquityInstrumentInfo**](CgsAPI.md#MarketGlobalEquityInstrumentInfo) | **Post** /cgs/v1/market/global/equity/instrument/info | 
[**MarketIndicator**](CgsAPI.md#MarketIndicator) | **Post** /cgs/v2/market/indicator | 
[**MarketInitialMargin**](CgsAPI.md#MarketInitialMargin) | **Post** /cgs/v1/market/initialmargin | 
[**MarketInstrumentInfo**](CgsAPI.md#MarketInstrumentInfo) | **Post** /cgs/v1/market/instrument/info | 
[**MarketOrderBook**](CgsAPI.md#MarketOrderBook) | **Post** /cgs/v2/market/orderbook | 
[**MarketProfileDescription**](CgsAPI.md#MarketProfileDescription) | **Post** /cgs/v1/market/profile/description | 
[**MarketProfileFinancials**](CgsAPI.md#MarketProfileFinancials) | **Post** /cgs/v1/market/profile/financials | 
[**MarketProfileFundamentals**](CgsAPI.md#MarketProfileFundamentals) | **Post** /cgs/v1/market/profile/fundamentals | 
[**MarketProfileOverview**](CgsAPI.md#MarketProfileOverview) | **Post** /cgs/v1/market/profile/overview | 
[**MarketSchedules**](CgsAPI.md#MarketSchedules) | **Post** /cgs/v2/market/schedules | 
[**MarketStatus**](CgsAPI.md#MarketStatus) | **Post** /cgs/v1/market/marketstatus | 
[**MarketTicker**](CgsAPI.md#MarketTicker) | **Post** /cgs/v2/market/ticker | 
[**MarketTimelineRendered**](CgsAPI.md#MarketTimelineRendered) | **Post** /cgs/v2/market/timeline/rendered | 
[**UserInstrumentFavourite**](CgsAPI.md#UserInstrumentFavourite) | **Post** /cgs/v2/user/instrument/favourite | 



## HomeInstruments

> PiMarketDataDomainModelsResponseHomeInstrumentsResponse HomeInstruments(ctx).PiMarketDataDomainModelsHomeInstrumentPayload(piMarketDataDomainModelsHomeInstrumentPayload).Execute()



### Example

```go
package main

import (
	"context"
	"fmt"
	"os"
	openapiclient "github.com/pi-financial/market-data-migrationproxy-srv/go-client"
)

func main() {
	piMarketDataDomainModelsHomeInstrumentPayload := *openapiclient.NewPiMarketDataDomainModelsHomeInstrumentPayload() // PiMarketDataDomainModelsHomeInstrumentPayload |  (optional)

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	resp, r, err := apiClient.CgsAPI.HomeInstruments(context.Background()).PiMarketDataDomainModelsHomeInstrumentPayload(piMarketDataDomainModelsHomeInstrumentPayload).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `CgsAPI.HomeInstruments``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `HomeInstruments`: PiMarketDataDomainModelsResponseHomeInstrumentsResponse
	fmt.Fprintf(os.Stdout, "Response from `CgsAPI.HomeInstruments`: %v\n", resp)
}
```

### Path Parameters



### Other Parameters

Other parameters are passed through a pointer to a apiHomeInstrumentsRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **piMarketDataDomainModelsHomeInstrumentPayload** | [**PiMarketDataDomainModelsHomeInstrumentPayload**](PiMarketDataDomainModelsHomeInstrumentPayload.md) |  | 

### Return type

[**PiMarketDataDomainModelsResponseHomeInstrumentsResponse**](PiMarketDataDomainModelsResponseHomeInstrumentsResponse.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: application/json, text/json, application/*+json
- **Accept**: text/plain, application/json, text/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)


## MarketBrokerInfo

> PiMarketDataDomainModelsResponseBrokerInfoResponse MarketBrokerInfo(ctx).PiMarketDataDomainModelsRequestBrokerInfoRequest(piMarketDataDomainModelsRequestBrokerInfoRequest).Execute()



### Example

```go
package main

import (
	"context"
	"fmt"
	"os"
	openapiclient "github.com/pi-financial/market-data-migrationproxy-srv/go-client"
)

func main() {
	piMarketDataDomainModelsRequestBrokerInfoRequest := *openapiclient.NewPiMarketDataDomainModelsRequestBrokerInfoRequest() // PiMarketDataDomainModelsRequestBrokerInfoRequest |  (optional)

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	resp, r, err := apiClient.CgsAPI.MarketBrokerInfo(context.Background()).PiMarketDataDomainModelsRequestBrokerInfoRequest(piMarketDataDomainModelsRequestBrokerInfoRequest).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `CgsAPI.MarketBrokerInfo``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `MarketBrokerInfo`: PiMarketDataDomainModelsResponseBrokerInfoResponse
	fmt.Fprintf(os.Stdout, "Response from `CgsAPI.MarketBrokerInfo`: %v\n", resp)
}
```

### Path Parameters



### Other Parameters

Other parameters are passed through a pointer to a apiMarketBrokerInfoRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **piMarketDataDomainModelsRequestBrokerInfoRequest** | [**PiMarketDataDomainModelsRequestBrokerInfoRequest**](PiMarketDataDomainModelsRequestBrokerInfoRequest.md) |  | 

### Return type

[**PiMarketDataDomainModelsResponseBrokerInfoResponse**](PiMarketDataDomainModelsResponseBrokerInfoResponse.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: application/json, text/json, application/*+json
- **Accept**: text/plain, application/json, text/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)


## MarketDerivativeInformation

> PiMarketDataDomainModelsResponseMarketDerivativeInformationResponse MarketDerivativeInformation(ctx).PiMarketDataDomainModelsRequestMarketDerivativeInformationRequest(piMarketDataDomainModelsRequestMarketDerivativeInformationRequest).Execute()



### Example

```go
package main

import (
	"context"
	"fmt"
	"os"
	openapiclient "github.com/pi-financial/market-data-migrationproxy-srv/go-client"
)

func main() {
	piMarketDataDomainModelsRequestMarketDerivativeInformationRequest := *openapiclient.NewPiMarketDataDomainModelsRequestMarketDerivativeInformationRequest() // PiMarketDataDomainModelsRequestMarketDerivativeInformationRequest |  (optional)

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	resp, r, err := apiClient.CgsAPI.MarketDerivativeInformation(context.Background()).PiMarketDataDomainModelsRequestMarketDerivativeInformationRequest(piMarketDataDomainModelsRequestMarketDerivativeInformationRequest).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `CgsAPI.MarketDerivativeInformation``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `MarketDerivativeInformation`: PiMarketDataDomainModelsResponseMarketDerivativeInformationResponse
	fmt.Fprintf(os.Stdout, "Response from `CgsAPI.MarketDerivativeInformation`: %v\n", resp)
}
```

### Path Parameters



### Other Parameters

Other parameters are passed through a pointer to a apiMarketDerivativeInformationRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **piMarketDataDomainModelsRequestMarketDerivativeInformationRequest** | [**PiMarketDataDomainModelsRequestMarketDerivativeInformationRequest**](PiMarketDataDomainModelsRequestMarketDerivativeInformationRequest.md) |  | 

### Return type

[**PiMarketDataDomainModelsResponseMarketDerivativeInformationResponse**](PiMarketDataDomainModelsResponseMarketDerivativeInformationResponse.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: application/json, text/json, application/*+json
- **Accept**: text/plain, application/json, text/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)


## MarketFilterInstruments

> PiMarketDataDomainModelsResponseMarketFilterInstrumentsResponse MarketFilterInstruments(ctx).PiMarketDataDomainModelsFiltersRequestPayload(piMarketDataDomainModelsFiltersRequestPayload).Execute()



### Example

```go
package main

import (
	"context"
	"fmt"
	"os"
	openapiclient "github.com/pi-financial/market-data-migrationproxy-srv/go-client"
)

func main() {
	piMarketDataDomainModelsFiltersRequestPayload := *openapiclient.NewPiMarketDataDomainModelsFiltersRequestPayload() // PiMarketDataDomainModelsFiltersRequestPayload |  (optional)

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	resp, r, err := apiClient.CgsAPI.MarketFilterInstruments(context.Background()).PiMarketDataDomainModelsFiltersRequestPayload(piMarketDataDomainModelsFiltersRequestPayload).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `CgsAPI.MarketFilterInstruments``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `MarketFilterInstruments`: PiMarketDataDomainModelsResponseMarketFilterInstrumentsResponse
	fmt.Fprintf(os.Stdout, "Response from `CgsAPI.MarketFilterInstruments`: %v\n", resp)
}
```

### Path Parameters



### Other Parameters

Other parameters are passed through a pointer to a apiMarketFilterInstrumentsRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **piMarketDataDomainModelsFiltersRequestPayload** | [**PiMarketDataDomainModelsFiltersRequestPayload**](PiMarketDataDomainModelsFiltersRequestPayload.md) |  | 

### Return type

[**PiMarketDataDomainModelsResponseMarketFilterInstrumentsResponse**](PiMarketDataDomainModelsResponseMarketFilterInstrumentsResponse.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: application/json, text/json, application/*+json
- **Accept**: text/plain, application/json, text/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)


## MarketFilters

> PiMarketDataDomainModelsResponseMarketFiltersResponse MarketFilters(ctx).PiMarketDataDomainModelsRequestMarketFiltersRequest(piMarketDataDomainModelsRequestMarketFiltersRequest).Execute()



### Example

```go
package main

import (
	"context"
	"fmt"
	"os"
	openapiclient "github.com/pi-financial/market-data-migrationproxy-srv/go-client"
)

func main() {
	piMarketDataDomainModelsRequestMarketFiltersRequest := *openapiclient.NewPiMarketDataDomainModelsRequestMarketFiltersRequest() // PiMarketDataDomainModelsRequestMarketFiltersRequest |  (optional)

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	resp, r, err := apiClient.CgsAPI.MarketFilters(context.Background()).PiMarketDataDomainModelsRequestMarketFiltersRequest(piMarketDataDomainModelsRequestMarketFiltersRequest).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `CgsAPI.MarketFilters``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `MarketFilters`: PiMarketDataDomainModelsResponseMarketFiltersResponse
	fmt.Fprintf(os.Stdout, "Response from `CgsAPI.MarketFilters`: %v\n", resp)
}
```

### Path Parameters



### Other Parameters

Other parameters are passed through a pointer to a apiMarketFiltersRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **piMarketDataDomainModelsRequestMarketFiltersRequest** | [**PiMarketDataDomainModelsRequestMarketFiltersRequest**](PiMarketDataDomainModelsRequestMarketFiltersRequest.md) |  | 

### Return type

[**PiMarketDataDomainModelsResponseMarketFiltersResponse**](PiMarketDataDomainModelsResponseMarketFiltersResponse.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: application/json, text/json, application/*+json
- **Accept**: text/plain, application/json, text/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)


## MarketGlobalEquityInstrumentInfo

> PiMarketDataDomainModelsResponseGlobalMarketInstrumentInfoResponse MarketGlobalEquityInstrumentInfo(ctx).PiMarketDataDomainModelsCommonPayload(piMarketDataDomainModelsCommonPayload).Execute()



### Example

```go
package main

import (
	"context"
	"fmt"
	"os"
	openapiclient "github.com/pi-financial/market-data-migrationproxy-srv/go-client"
)

func main() {
	piMarketDataDomainModelsCommonPayload := *openapiclient.NewPiMarketDataDomainModelsCommonPayload() // PiMarketDataDomainModelsCommonPayload |  (optional)

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	resp, r, err := apiClient.CgsAPI.MarketGlobalEquityInstrumentInfo(context.Background()).PiMarketDataDomainModelsCommonPayload(piMarketDataDomainModelsCommonPayload).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `CgsAPI.MarketGlobalEquityInstrumentInfo``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `MarketGlobalEquityInstrumentInfo`: PiMarketDataDomainModelsResponseGlobalMarketInstrumentInfoResponse
	fmt.Fprintf(os.Stdout, "Response from `CgsAPI.MarketGlobalEquityInstrumentInfo`: %v\n", resp)
}
```

### Path Parameters



### Other Parameters

Other parameters are passed through a pointer to a apiMarketGlobalEquityInstrumentInfoRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **piMarketDataDomainModelsCommonPayload** | [**PiMarketDataDomainModelsCommonPayload**](PiMarketDataDomainModelsCommonPayload.md) |  | 

### Return type

[**PiMarketDataDomainModelsResponseGlobalMarketInstrumentInfoResponse**](PiMarketDataDomainModelsResponseGlobalMarketInstrumentInfoResponse.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: application/json, text/json, application/*+json
- **Accept**: text/plain, application/json, text/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)


## MarketIndicator

> PiMarketDataDomainModelsResponseMarketIndicatorResponse MarketIndicator(ctx).PiMarketDataDomainModelsRequestsMarketIndicatorRequest(piMarketDataDomainModelsRequestsMarketIndicatorRequest).Execute()



### Example

```go
package main

import (
	"context"
	"fmt"
	"os"
	openapiclient "github.com/pi-financial/market-data-migrationproxy-srv/go-client"
)

func main() {
	piMarketDataDomainModelsRequestsMarketIndicatorRequest := *openapiclient.NewPiMarketDataDomainModelsRequestsMarketIndicatorRequest() // PiMarketDataDomainModelsRequestsMarketIndicatorRequest |  (optional)

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	resp, r, err := apiClient.CgsAPI.MarketIndicator(context.Background()).PiMarketDataDomainModelsRequestsMarketIndicatorRequest(piMarketDataDomainModelsRequestsMarketIndicatorRequest).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `CgsAPI.MarketIndicator``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `MarketIndicator`: PiMarketDataDomainModelsResponseMarketIndicatorResponse
	fmt.Fprintf(os.Stdout, "Response from `CgsAPI.MarketIndicator`: %v\n", resp)
}
```

### Path Parameters



### Other Parameters

Other parameters are passed through a pointer to a apiMarketIndicatorRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **piMarketDataDomainModelsRequestsMarketIndicatorRequest** | [**PiMarketDataDomainModelsRequestsMarketIndicatorRequest**](PiMarketDataDomainModelsRequestsMarketIndicatorRequest.md) |  | 

### Return type

[**PiMarketDataDomainModelsResponseMarketIndicatorResponse**](PiMarketDataDomainModelsResponseMarketIndicatorResponse.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: application/json, text/json, application/*+json
- **Accept**: text/plain, application/json, text/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)


## MarketInitialMargin

> PiMarketDataDomainModelsResponseMarketInitialMarginResponse MarketInitialMargin(ctx).PiMarketDataDomainModelsRequestMarketInitialMarginRequest(piMarketDataDomainModelsRequestMarketInitialMarginRequest).Execute()



### Example

```go
package main

import (
	"context"
	"fmt"
	"os"
	openapiclient "github.com/pi-financial/market-data-migrationproxy-srv/go-client"
)

func main() {
	piMarketDataDomainModelsRequestMarketInitialMarginRequest := *openapiclient.NewPiMarketDataDomainModelsRequestMarketInitialMarginRequest() // PiMarketDataDomainModelsRequestMarketInitialMarginRequest |  (optional)

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	resp, r, err := apiClient.CgsAPI.MarketInitialMargin(context.Background()).PiMarketDataDomainModelsRequestMarketInitialMarginRequest(piMarketDataDomainModelsRequestMarketInitialMarginRequest).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `CgsAPI.MarketInitialMargin``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `MarketInitialMargin`: PiMarketDataDomainModelsResponseMarketInitialMarginResponse
	fmt.Fprintf(os.Stdout, "Response from `CgsAPI.MarketInitialMargin`: %v\n", resp)
}
```

### Path Parameters



### Other Parameters

Other parameters are passed through a pointer to a apiMarketInitialMarginRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **piMarketDataDomainModelsRequestMarketInitialMarginRequest** | [**PiMarketDataDomainModelsRequestMarketInitialMarginRequest**](PiMarketDataDomainModelsRequestMarketInitialMarginRequest.md) |  | 

### Return type

[**PiMarketDataDomainModelsResponseMarketInitialMarginResponse**](PiMarketDataDomainModelsResponseMarketInitialMarginResponse.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: application/json, text/json, application/*+json
- **Accept**: text/plain, application/json, text/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)


## MarketInstrumentInfo

> PiMarketDataDomainModelsResponseMarketInstrumentInfoResponse MarketInstrumentInfo(ctx).PiMarketDataDomainModelsCommonPayload(piMarketDataDomainModelsCommonPayload).Execute()



### Example

```go
package main

import (
	"context"
	"fmt"
	"os"
	openapiclient "github.com/pi-financial/market-data-migrationproxy-srv/go-client"
)

func main() {
	piMarketDataDomainModelsCommonPayload := *openapiclient.NewPiMarketDataDomainModelsCommonPayload() // PiMarketDataDomainModelsCommonPayload |  (optional)

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	resp, r, err := apiClient.CgsAPI.MarketInstrumentInfo(context.Background()).PiMarketDataDomainModelsCommonPayload(piMarketDataDomainModelsCommonPayload).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `CgsAPI.MarketInstrumentInfo``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `MarketInstrumentInfo`: PiMarketDataDomainModelsResponseMarketInstrumentInfoResponse
	fmt.Fprintf(os.Stdout, "Response from `CgsAPI.MarketInstrumentInfo`: %v\n", resp)
}
```

### Path Parameters



### Other Parameters

Other parameters are passed through a pointer to a apiMarketInstrumentInfoRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **piMarketDataDomainModelsCommonPayload** | [**PiMarketDataDomainModelsCommonPayload**](PiMarketDataDomainModelsCommonPayload.md) |  | 

### Return type

[**PiMarketDataDomainModelsResponseMarketInstrumentInfoResponse**](PiMarketDataDomainModelsResponseMarketInstrumentInfoResponse.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: application/json, text/json, application/*+json
- **Accept**: text/plain, application/json, text/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)


## MarketOrderBook

> PiMarketDataDomainModelsResponseMarketOrderBookResponse MarketOrderBook(ctx).PiMarketDataDomainModelsSymbolVenuePayload(piMarketDataDomainModelsSymbolVenuePayload).Execute()



### Example

```go
package main

import (
	"context"
	"fmt"
	"os"
	openapiclient "github.com/pi-financial/market-data-migrationproxy-srv/go-client"
)

func main() {
	piMarketDataDomainModelsSymbolVenuePayload := *openapiclient.NewPiMarketDataDomainModelsSymbolVenuePayload() // PiMarketDataDomainModelsSymbolVenuePayload |  (optional)

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	resp, r, err := apiClient.CgsAPI.MarketOrderBook(context.Background()).PiMarketDataDomainModelsSymbolVenuePayload(piMarketDataDomainModelsSymbolVenuePayload).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `CgsAPI.MarketOrderBook``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `MarketOrderBook`: PiMarketDataDomainModelsResponseMarketOrderBookResponse
	fmt.Fprintf(os.Stdout, "Response from `CgsAPI.MarketOrderBook`: %v\n", resp)
}
```

### Path Parameters



### Other Parameters

Other parameters are passed through a pointer to a apiMarketOrderBookRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **piMarketDataDomainModelsSymbolVenuePayload** | [**PiMarketDataDomainModelsSymbolVenuePayload**](PiMarketDataDomainModelsSymbolVenuePayload.md) |  | 

### Return type

[**PiMarketDataDomainModelsResponseMarketOrderBookResponse**](PiMarketDataDomainModelsResponseMarketOrderBookResponse.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: application/json, text/json, application/*+json
- **Accept**: text/plain, application/json, text/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)


## MarketProfileDescription

> PiMarketDataDomainModelsResponseMarketProfileDescriptionResponse MarketProfileDescription(ctx).PiMarketDataDomainModelsCommonPayload(piMarketDataDomainModelsCommonPayload).Execute()



### Example

```go
package main

import (
	"context"
	"fmt"
	"os"
	openapiclient "github.com/pi-financial/market-data-migrationproxy-srv/go-client"
)

func main() {
	piMarketDataDomainModelsCommonPayload := *openapiclient.NewPiMarketDataDomainModelsCommonPayload() // PiMarketDataDomainModelsCommonPayload |  (optional)

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	resp, r, err := apiClient.CgsAPI.MarketProfileDescription(context.Background()).PiMarketDataDomainModelsCommonPayload(piMarketDataDomainModelsCommonPayload).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `CgsAPI.MarketProfileDescription``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `MarketProfileDescription`: PiMarketDataDomainModelsResponseMarketProfileDescriptionResponse
	fmt.Fprintf(os.Stdout, "Response from `CgsAPI.MarketProfileDescription`: %v\n", resp)
}
```

### Path Parameters



### Other Parameters

Other parameters are passed through a pointer to a apiMarketProfileDescriptionRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **piMarketDataDomainModelsCommonPayload** | [**PiMarketDataDomainModelsCommonPayload**](PiMarketDataDomainModelsCommonPayload.md) |  | 

### Return type

[**PiMarketDataDomainModelsResponseMarketProfileDescriptionResponse**](PiMarketDataDomainModelsResponseMarketProfileDescriptionResponse.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: application/json, text/json, application/*+json
- **Accept**: text/plain, application/json, text/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)


## MarketProfileFinancials

> PiMarketDataDomainModelsResponseMarketProfileFinancialsResponse MarketProfileFinancials(ctx).PiMarketDataDomainModelsCommonPayload(piMarketDataDomainModelsCommonPayload).Execute()



### Example

```go
package main

import (
	"context"
	"fmt"
	"os"
	openapiclient "github.com/pi-financial/market-data-migrationproxy-srv/go-client"
)

func main() {
	piMarketDataDomainModelsCommonPayload := *openapiclient.NewPiMarketDataDomainModelsCommonPayload() // PiMarketDataDomainModelsCommonPayload |  (optional)

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	resp, r, err := apiClient.CgsAPI.MarketProfileFinancials(context.Background()).PiMarketDataDomainModelsCommonPayload(piMarketDataDomainModelsCommonPayload).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `CgsAPI.MarketProfileFinancials``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `MarketProfileFinancials`: PiMarketDataDomainModelsResponseMarketProfileFinancialsResponse
	fmt.Fprintf(os.Stdout, "Response from `CgsAPI.MarketProfileFinancials`: %v\n", resp)
}
```

### Path Parameters



### Other Parameters

Other parameters are passed through a pointer to a apiMarketProfileFinancialsRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **piMarketDataDomainModelsCommonPayload** | [**PiMarketDataDomainModelsCommonPayload**](PiMarketDataDomainModelsCommonPayload.md) |  | 

### Return type

[**PiMarketDataDomainModelsResponseMarketProfileFinancialsResponse**](PiMarketDataDomainModelsResponseMarketProfileFinancialsResponse.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: application/json, text/json, application/*+json
- **Accept**: text/plain, application/json, text/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)


## MarketProfileFundamentals

> PiMarketDataDomainModelsResponseMarketProfileFundamentalsResponse MarketProfileFundamentals(ctx).PiMarketDataDomainModelsCommonPayload(piMarketDataDomainModelsCommonPayload).Execute()



### Example

```go
package main

import (
	"context"
	"fmt"
	"os"
	openapiclient "github.com/pi-financial/market-data-migrationproxy-srv/go-client"
)

func main() {
	piMarketDataDomainModelsCommonPayload := *openapiclient.NewPiMarketDataDomainModelsCommonPayload() // PiMarketDataDomainModelsCommonPayload |  (optional)

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	resp, r, err := apiClient.CgsAPI.MarketProfileFundamentals(context.Background()).PiMarketDataDomainModelsCommonPayload(piMarketDataDomainModelsCommonPayload).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `CgsAPI.MarketProfileFundamentals``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `MarketProfileFundamentals`: PiMarketDataDomainModelsResponseMarketProfileFundamentalsResponse
	fmt.Fprintf(os.Stdout, "Response from `CgsAPI.MarketProfileFundamentals`: %v\n", resp)
}
```

### Path Parameters



### Other Parameters

Other parameters are passed through a pointer to a apiMarketProfileFundamentalsRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **piMarketDataDomainModelsCommonPayload** | [**PiMarketDataDomainModelsCommonPayload**](PiMarketDataDomainModelsCommonPayload.md) |  | 

### Return type

[**PiMarketDataDomainModelsResponseMarketProfileFundamentalsResponse**](PiMarketDataDomainModelsResponseMarketProfileFundamentalsResponse.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: application/json, text/json, application/*+json
- **Accept**: text/plain, application/json, text/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)


## MarketProfileOverview

> PiMarketDataDomainModelsResponseMarketProfileOverviewResponse MarketProfileOverview(ctx).PiMarketDataDomainModelsCommonPayload(piMarketDataDomainModelsCommonPayload).Execute()



### Example

```go
package main

import (
	"context"
	"fmt"
	"os"
	openapiclient "github.com/pi-financial/market-data-migrationproxy-srv/go-client"
)

func main() {
	piMarketDataDomainModelsCommonPayload := *openapiclient.NewPiMarketDataDomainModelsCommonPayload() // PiMarketDataDomainModelsCommonPayload |  (optional)

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	resp, r, err := apiClient.CgsAPI.MarketProfileOverview(context.Background()).PiMarketDataDomainModelsCommonPayload(piMarketDataDomainModelsCommonPayload).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `CgsAPI.MarketProfileOverview``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `MarketProfileOverview`: PiMarketDataDomainModelsResponseMarketProfileOverviewResponse
	fmt.Fprintf(os.Stdout, "Response from `CgsAPI.MarketProfileOverview`: %v\n", resp)
}
```

### Path Parameters



### Other Parameters

Other parameters are passed through a pointer to a apiMarketProfileOverviewRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **piMarketDataDomainModelsCommonPayload** | [**PiMarketDataDomainModelsCommonPayload**](PiMarketDataDomainModelsCommonPayload.md) |  | 

### Return type

[**PiMarketDataDomainModelsResponseMarketProfileOverviewResponse**](PiMarketDataDomainModelsResponseMarketProfileOverviewResponse.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: application/json, text/json, application/*+json
- **Accept**: text/plain, application/json, text/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)


## MarketSchedules

> PiMarketDataDomainModelsResponseMarketSchedulesResponse MarketSchedules(ctx).PiMarketDataDomainModelsCommonPayload(piMarketDataDomainModelsCommonPayload).Execute()



### Example

```go
package main

import (
	"context"
	"fmt"
	"os"
	openapiclient "github.com/pi-financial/market-data-migrationproxy-srv/go-client"
)

func main() {
	piMarketDataDomainModelsCommonPayload := *openapiclient.NewPiMarketDataDomainModelsCommonPayload() // PiMarketDataDomainModelsCommonPayload |  (optional)

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	resp, r, err := apiClient.CgsAPI.MarketSchedules(context.Background()).PiMarketDataDomainModelsCommonPayload(piMarketDataDomainModelsCommonPayload).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `CgsAPI.MarketSchedules``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `MarketSchedules`: PiMarketDataDomainModelsResponseMarketSchedulesResponse
	fmt.Fprintf(os.Stdout, "Response from `CgsAPI.MarketSchedules`: %v\n", resp)
}
```

### Path Parameters



### Other Parameters

Other parameters are passed through a pointer to a apiMarketSchedulesRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **piMarketDataDomainModelsCommonPayload** | [**PiMarketDataDomainModelsCommonPayload**](PiMarketDataDomainModelsCommonPayload.md) |  | 

### Return type

[**PiMarketDataDomainModelsResponseMarketSchedulesResponse**](PiMarketDataDomainModelsResponseMarketSchedulesResponse.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: application/json, text/json, application/*+json
- **Accept**: text/plain, application/json, text/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)


## MarketStatus

> PiMarketDataDomainModelsResponseMarketStatusResponse MarketStatus(ctx).PiMarketDataDomainModelsRequestsMarketStatusRequest(piMarketDataDomainModelsRequestsMarketStatusRequest).Execute()



### Example

```go
package main

import (
	"context"
	"fmt"
	"os"
	openapiclient "github.com/pi-financial/market-data-migrationproxy-srv/go-client"
)

func main() {
	piMarketDataDomainModelsRequestsMarketStatusRequest := *openapiclient.NewPiMarketDataDomainModelsRequestsMarketStatusRequest() // PiMarketDataDomainModelsRequestsMarketStatusRequest |  (optional)

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	resp, r, err := apiClient.CgsAPI.MarketStatus(context.Background()).PiMarketDataDomainModelsRequestsMarketStatusRequest(piMarketDataDomainModelsRequestsMarketStatusRequest).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `CgsAPI.MarketStatus``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `MarketStatus`: PiMarketDataDomainModelsResponseMarketStatusResponse
	fmt.Fprintf(os.Stdout, "Response from `CgsAPI.MarketStatus`: %v\n", resp)
}
```

### Path Parameters



### Other Parameters

Other parameters are passed through a pointer to a apiMarketStatusRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **piMarketDataDomainModelsRequestsMarketStatusRequest** | [**PiMarketDataDomainModelsRequestsMarketStatusRequest**](PiMarketDataDomainModelsRequestsMarketStatusRequest.md) |  | 

### Return type

[**PiMarketDataDomainModelsResponseMarketStatusResponse**](PiMarketDataDomainModelsResponseMarketStatusResponse.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: application/json, text/json, application/*+json
- **Accept**: text/plain, application/json, text/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)


## MarketTicker

> PiMarketDataDomainModelsResponseMarketTickerResponse MarketTicker(ctx).PiMarketDataDomainModelsVenuePayload(piMarketDataDomainModelsVenuePayload).Execute()



### Example

```go
package main

import (
	"context"
	"fmt"
	"os"
	openapiclient "github.com/pi-financial/market-data-migrationproxy-srv/go-client"
)

func main() {
	piMarketDataDomainModelsVenuePayload := *openapiclient.NewPiMarketDataDomainModelsVenuePayload() // PiMarketDataDomainModelsVenuePayload |  (optional)

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	resp, r, err := apiClient.CgsAPI.MarketTicker(context.Background()).PiMarketDataDomainModelsVenuePayload(piMarketDataDomainModelsVenuePayload).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `CgsAPI.MarketTicker``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `MarketTicker`: PiMarketDataDomainModelsResponseMarketTickerResponse
	fmt.Fprintf(os.Stdout, "Response from `CgsAPI.MarketTicker`: %v\n", resp)
}
```

### Path Parameters



### Other Parameters

Other parameters are passed through a pointer to a apiMarketTickerRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **piMarketDataDomainModelsVenuePayload** | [**PiMarketDataDomainModelsVenuePayload**](PiMarketDataDomainModelsVenuePayload.md) |  | 

### Return type

[**PiMarketDataDomainModelsResponseMarketTickerResponse**](PiMarketDataDomainModelsResponseMarketTickerResponse.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: application/json, text/json, application/*+json
- **Accept**: text/plain, application/json, text/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)


## MarketTimelineRendered

> PiMarketDataDomainModelsResponseMarketTimelineRenderedResponse MarketTimelineRendered(ctx).PiMarketDataDomainModelsCommonPayload(piMarketDataDomainModelsCommonPayload).Execute()



### Example

```go
package main

import (
	"context"
	"fmt"
	"os"
	openapiclient "github.com/pi-financial/market-data-migrationproxy-srv/go-client"
)

func main() {
	piMarketDataDomainModelsCommonPayload := *openapiclient.NewPiMarketDataDomainModelsCommonPayload() // PiMarketDataDomainModelsCommonPayload |  (optional)

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	resp, r, err := apiClient.CgsAPI.MarketTimelineRendered(context.Background()).PiMarketDataDomainModelsCommonPayload(piMarketDataDomainModelsCommonPayload).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `CgsAPI.MarketTimelineRendered``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `MarketTimelineRendered`: PiMarketDataDomainModelsResponseMarketTimelineRenderedResponse
	fmt.Fprintf(os.Stdout, "Response from `CgsAPI.MarketTimelineRendered`: %v\n", resp)
}
```

### Path Parameters



### Other Parameters

Other parameters are passed through a pointer to a apiMarketTimelineRenderedRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **piMarketDataDomainModelsCommonPayload** | [**PiMarketDataDomainModelsCommonPayload**](PiMarketDataDomainModelsCommonPayload.md) |  | 

### Return type

[**PiMarketDataDomainModelsResponseMarketTimelineRenderedResponse**](PiMarketDataDomainModelsResponseMarketTimelineRenderedResponse.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: application/json, text/json, application/*+json
- **Accept**: text/plain, application/json, text/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)


## UserInstrumentFavourite

> UserInstrumentFavourite(ctx).UserId(userId).Execute()



### Example

```go
package main

import (
	"context"
	"fmt"
	"os"
	openapiclient "github.com/pi-financial/market-data-migrationproxy-srv/go-client"
)

func main() {
	userId := "38400000-8cf0-11bd-b23e-10b96e4ef00d" // string | 

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	r, err := apiClient.CgsAPI.UserInstrumentFavourite(context.Background()).UserId(userId).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `CgsAPI.UserInstrumentFavourite``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
}
```

### Path Parameters



### Other Parameters

Other parameters are passed through a pointer to a apiUserInstrumentFavouriteRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **userId** | **string** |  | 

### Return type

 (empty response body)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: Not defined

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)

