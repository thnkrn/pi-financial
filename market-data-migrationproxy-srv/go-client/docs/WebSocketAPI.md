# \WebSocketAPI

All URIs are relative to *http://localhost*

Method | HTTP request | Description
------------- | ------------- | -------------
[**MarketDataStreamingGet**](WebSocketAPI.md#MarketDataStreamingGet) | **Get** /market-data/streaming | WebSocket Endpoint for Market Data



## MarketDataStreamingGet

> PiSMarketDataWSSDomainModelsResponseMarketStreamingResponse MarketDataStreamingGet(ctx).PiMarketDataWSSDomainModelsRequestMarketStreamingRequest(piMarketDataWSSDomainModelsRequestMarketStreamingRequest).Execute()

WebSocket Endpoint for Market Data



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
	piMarketDataWSSDomainModelsRequestMarketStreamingRequest := *openapiclient.NewPiMarketDataWSSDomainModelsRequestMarketStreamingRequest() // PiMarketDataWSSDomainModelsRequestMarketStreamingRequest |  (optional)

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	resp, r, err := apiClient.WebSocketAPI.MarketDataStreamingGet(context.Background()).PiMarketDataWSSDomainModelsRequestMarketStreamingRequest(piMarketDataWSSDomainModelsRequestMarketStreamingRequest).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `WebSocketAPI.MarketDataStreamingGet``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `MarketDataStreamingGet`: PiSMarketDataWSSDomainModelsResponseMarketStreamingResponse
	fmt.Fprintf(os.Stdout, "Response from `WebSocketAPI.MarketDataStreamingGet`: %v\n", resp)
}
```

### Path Parameters



### Other Parameters

Other parameters are passed through a pointer to a apiMarketDataStreamingGetRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **piMarketDataWSSDomainModelsRequestMarketStreamingRequest** | [**PiMarketDataWSSDomainModelsRequestMarketStreamingRequest**](PiMarketDataWSSDomainModelsRequestMarketStreamingRequest.md) |  | 

### Return type

[**PiSMarketDataWSSDomainModelsResponseMarketStreamingResponse**](PiSMarketDataWSSDomainModelsResponseMarketStreamingResponse.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: application/json, text/json, application/*+json
- **Accept**: text/plain, application/json, text/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)

