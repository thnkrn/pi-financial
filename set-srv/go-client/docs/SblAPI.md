# \SblAPI

All URIs are relative to *http://localhost*

Method | HTTP request | Description
------------- | ------------- | -------------
[**InternalSblInstrumentsGet**](SblAPI.md#InternalSblInstrumentsGet) | **Get** /internal/sbl/instruments | 
[**InternalSblOrdersGet**](SblAPI.md#InternalSblOrdersGet) | **Get** /internal/sbl/orders | 
[**InternalSblOrdersOrderIdPatch**](SblAPI.md#InternalSblOrdersOrderIdPatch) | **Patch** /internal/sbl/orders/{orderId} | 



## InternalSblInstrumentsGet

> PiSetServiceDomainAggregatesModelInstrumentAggregateSblInstrumentIEnumerableApiPaginateResponse InternalSblInstrumentsGet(ctx).Page(page).PageSize(pageSize).OrderBy(orderBy).OrderDir(orderDir).Symbol(symbol).Execute()



### Example

```go
package main

import (
	"context"
	"fmt"
	"os"
	openapiclient "github.com/pi-financial/set-srv/go-client"
)

func main() {
	page := "123" // string |  (optional)
	pageSize := "123" // string |  (optional)
	orderBy := "orderBy_example" // string |  (optional)
	orderDir := openapiclient.PiSetServiceDomainAggregatesModelCommonAggregateOrderDirection("asc") // PiSetServiceDomainAggregatesModelCommonAggregateOrderDirection |  (optional)
	symbol := "symbol_example" // string |  (optional)

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	resp, r, err := apiClient.SblAPI.InternalSblInstrumentsGet(context.Background()).Page(page).PageSize(pageSize).OrderBy(orderBy).OrderDir(orderDir).Symbol(symbol).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `SblAPI.InternalSblInstrumentsGet``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `InternalSblInstrumentsGet`: PiSetServiceDomainAggregatesModelInstrumentAggregateSblInstrumentIEnumerableApiPaginateResponse
	fmt.Fprintf(os.Stdout, "Response from `SblAPI.InternalSblInstrumentsGet`: %v\n", resp)
}
```

### Path Parameters



### Other Parameters

Other parameters are passed through a pointer to a apiInternalSblInstrumentsGetRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **page** | **string** |  | 
 **pageSize** | **string** |  | 
 **orderBy** | **string** |  | 
 **orderDir** | [**PiSetServiceDomainAggregatesModelCommonAggregateOrderDirection**](PiSetServiceDomainAggregatesModelCommonAggregateOrderDirection.md) |  | 
 **symbol** | **string** |  | 

### Return type

[**PiSetServiceDomainAggregatesModelInstrumentAggregateSblInstrumentIEnumerableApiPaginateResponse**](PiSetServiceDomainAggregatesModelInstrumentAggregateSblInstrumentIEnumerableApiPaginateResponse.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: text/plain, application/json, text/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)


## InternalSblOrdersGet

> PiSetServiceDomainAggregatesModelTradingAggregateSblOrderIEnumerableApiPaginateResponse InternalSblOrdersGet(ctx).Page(page).PageSize(pageSize).OrderBy(orderBy).OrderDir(orderDir).TradingAccountNo(tradingAccountNo).Open(open).Symbol(symbol).Statues(statues).Type_(type_).CreatedDateFrom(createdDateFrom).CreatedDateTo(createdDateTo).Execute()



### Example

```go
package main

import (
	"context"
	"fmt"
	"os"
    "time"
	openapiclient "github.com/pi-financial/set-srv/go-client"
)

func main() {
	page := "123" // string |  (optional)
	pageSize := "123" // string |  (optional)
	orderBy := "orderBy_example" // string |  (optional)
	orderDir := openapiclient.PiSetServiceDomainAggregatesModelCommonAggregateOrderDirection("asc") // PiSetServiceDomainAggregatesModelCommonAggregateOrderDirection |  (optional)
	tradingAccountNo := "tradingAccountNo_example" // string |  (optional)
	open := true // bool |  (optional)
	symbol := "symbol_example" // string |  (optional)
	statues := []openapiclient.PiSetServiceDomainAggregatesModelFinancialAssetAggregateSblOrderStatus{openapiclient.PiSetServiceDomainAggregatesModelFinancialAssetAggregateSblOrderStatus("pending")} // []PiSetServiceDomainAggregatesModelFinancialAssetAggregateSblOrderStatus |  (optional)
	type_ := openapiclient.PiSetServiceDomainAggregatesModelFinancialAssetAggregateSblOrderType("borrow") // PiSetServiceDomainAggregatesModelFinancialAssetAggregateSblOrderType |  (optional)
	createdDateFrom := time.Now() // string |  (optional)
	createdDateTo := time.Now() // string |  (optional)

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	resp, r, err := apiClient.SblAPI.InternalSblOrdersGet(context.Background()).Page(page).PageSize(pageSize).OrderBy(orderBy).OrderDir(orderDir).TradingAccountNo(tradingAccountNo).Open(open).Symbol(symbol).Statues(statues).Type_(type_).CreatedDateFrom(createdDateFrom).CreatedDateTo(createdDateTo).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `SblAPI.InternalSblOrdersGet``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `InternalSblOrdersGet`: PiSetServiceDomainAggregatesModelTradingAggregateSblOrderIEnumerableApiPaginateResponse
	fmt.Fprintf(os.Stdout, "Response from `SblAPI.InternalSblOrdersGet`: %v\n", resp)
}
```

### Path Parameters



### Other Parameters

Other parameters are passed through a pointer to a apiInternalSblOrdersGetRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **page** | **string** |  | 
 **pageSize** | **string** |  | 
 **orderBy** | **string** |  | 
 **orderDir** | [**PiSetServiceDomainAggregatesModelCommonAggregateOrderDirection**](PiSetServiceDomainAggregatesModelCommonAggregateOrderDirection.md) |  | 
 **tradingAccountNo** | **string** |  | 
 **open** | **bool** |  | 
 **symbol** | **string** |  | 
 **statues** | [**[]PiSetServiceDomainAggregatesModelFinancialAssetAggregateSblOrderStatus**](PiSetServiceDomainAggregatesModelFinancialAssetAggregateSblOrderStatus.md) |  | 
 **type_** | [**PiSetServiceDomainAggregatesModelFinancialAssetAggregateSblOrderType**](PiSetServiceDomainAggregatesModelFinancialAssetAggregateSblOrderType.md) |  | 
 **createdDateFrom** | **string** |  | 
 **createdDateTo** | **string** |  | 

### Return type

[**PiSetServiceDomainAggregatesModelTradingAggregateSblOrderIEnumerableApiPaginateResponse**](PiSetServiceDomainAggregatesModelTradingAggregateSblOrderIEnumerableApiPaginateResponse.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: text/plain, application/json, text/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)


## InternalSblOrdersOrderIdPatch

> PiSetServiceApplicationCommandsReviewSblOrderResponseApiResponse InternalSblOrdersOrderIdPatch(ctx, orderId).PiSetServiceAPIModelsSblOrderSubmitReviewRequest(piSetServiceAPIModelsSblOrderSubmitReviewRequest).Execute()



### Example

```go
package main

import (
	"context"
	"fmt"
	"os"
	openapiclient "github.com/pi-financial/set-srv/go-client"
)

func main() {
	orderId := "38400000-8cf0-11bd-b23e-10b96e4ef00d" // string | 
	piSetServiceAPIModelsSblOrderSubmitReviewRequest := *openapiclient.NewPiSetServiceAPIModelsSblOrderSubmitReviewRequest(openapiclient.PiSetServiceAPIModelsSblSubmitReviewStatus("approved"), "ReviewerId_example") // PiSetServiceAPIModelsSblOrderSubmitReviewRequest |  (optional)

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	resp, r, err := apiClient.SblAPI.InternalSblOrdersOrderIdPatch(context.Background(), orderId).PiSetServiceAPIModelsSblOrderSubmitReviewRequest(piSetServiceAPIModelsSblOrderSubmitReviewRequest).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `SblAPI.InternalSblOrdersOrderIdPatch``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `InternalSblOrdersOrderIdPatch`: PiSetServiceApplicationCommandsReviewSblOrderResponseApiResponse
	fmt.Fprintf(os.Stdout, "Response from `SblAPI.InternalSblOrdersOrderIdPatch`: %v\n", resp)
}
```

### Path Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
**ctx** | **context.Context** | context for authentication, logging, cancellation, deadlines, tracing, etc.
**orderId** | **string** |  | 

### Other Parameters

Other parameters are passed through a pointer to a apiInternalSblOrdersOrderIdPatchRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------

 **piSetServiceAPIModelsSblOrderSubmitReviewRequest** | [**PiSetServiceAPIModelsSblOrderSubmitReviewRequest**](PiSetServiceAPIModelsSblOrderSubmitReviewRequest.md) |  | 

### Return type

[**PiSetServiceApplicationCommandsReviewSblOrderResponseApiResponse**](PiSetServiceApplicationCommandsReviewSblOrderResponseApiResponse.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: application/json, text/json, application/*+json
- **Accept**: text/plain, application/json, text/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)

