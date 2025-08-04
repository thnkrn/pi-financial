# \OrderAPI

All URIs are relative to *http://localhost*

Method | HTTP request | Description
------------- | ------------- | -------------
[**SecureOrdersActiveGet**](OrderAPI.md#SecureOrdersActiveGet) | **Get** /secure/orders/active | 
[**SecureOrdersGet**](OrderAPI.md#SecureOrdersGet) | **Get** /secure/orders | 
[**TradingOrdersOrderIdDelete**](OrderAPI.md#TradingOrdersOrderIdDelete) | **Delete** /trading/orders/{orderId} | 
[**TradingOrdersOrderIdPut**](OrderAPI.md#TradingOrdersOrderIdPut) | **Put** /trading/orders/{orderId} | 
[**TradingOrdersPost**](OrderAPI.md#TradingOrdersPost) | **Post** /trading/orders | 



## SecureOrdersActiveGet

> OrderResponseArrayApiResponse SecureOrdersActiveGet(ctx).UserId(userId).AccountId(accountId).Execute()



### Example

```go
package main

import (
	"context"
	"fmt"
	"os"
	openapiclient "github.com/pi-financial/global-equities/go-client"
)

func main() {
	userId := "userId_example" // string | 
	accountId := "accountId_example" // string | 

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	resp, r, err := apiClient.OrderAPI.SecureOrdersActiveGet(context.Background()).UserId(userId).AccountId(accountId).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `OrderAPI.SecureOrdersActiveGet``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `SecureOrdersActiveGet`: OrderResponseArrayApiResponse
	fmt.Fprintf(os.Stdout, "Response from `OrderAPI.SecureOrdersActiveGet`: %v\n", resp)
}
```

### Path Parameters



### Other Parameters

Other parameters are passed through a pointer to a apiSecureOrdersActiveGetRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **userId** | **string** |  | 
 **accountId** | **string** |  | 

### Return type

[**OrderResponseArrayApiResponse**](OrderResponseArrayApiResponse.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: text/plain, application/json, text/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)


## SecureOrdersGet

> OrderResponseArrayApiResponse SecureOrdersGet(ctx).UserId(userId).AccountId(accountId).From(from).To(to).Side(side).HasFilled(hasFilled).Execute()



### Example

```go
package main

import (
	"context"
	"fmt"
	"os"
    "time"
	openapiclient "github.com/pi-financial/global-equities/go-client"
)

func main() {
	userId := "userId_example" // string | 
	accountId := "accountId_example" // string | 
	from := time.Now() // time.Time | 
	to := time.Now() // time.Time | 
	side := openapiclient.OrderSide("Buy") // OrderSide |  (optional)
	hasFilled := true // bool |  (optional)

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	resp, r, err := apiClient.OrderAPI.SecureOrdersGet(context.Background()).UserId(userId).AccountId(accountId).From(from).To(to).Side(side).HasFilled(hasFilled).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `OrderAPI.SecureOrdersGet``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `SecureOrdersGet`: OrderResponseArrayApiResponse
	fmt.Fprintf(os.Stdout, "Response from `OrderAPI.SecureOrdersGet`: %v\n", resp)
}
```

### Path Parameters



### Other Parameters

Other parameters are passed through a pointer to a apiSecureOrdersGetRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **userId** | **string** |  | 
 **accountId** | **string** |  | 
 **from** | **time.Time** |  | 
 **to** | **time.Time** |  | 
 **side** | [**OrderSide**](OrderSide.md) |  | 
 **hasFilled** | **bool** |  | 

### Return type

[**OrderResponseArrayApiResponse**](OrderResponseArrayApiResponse.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: text/plain, application/json, text/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)


## TradingOrdersOrderIdDelete

> OrderResponseApiResponse TradingOrdersOrderIdDelete(ctx, orderId).UserId(userId).Execute()



### Example

```go
package main

import (
	"context"
	"fmt"
	"os"
	openapiclient "github.com/pi-financial/global-equities/go-client"
)

func main() {
	userId := "userId_example" // string | 
	orderId := "orderId_example" // string | 

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	resp, r, err := apiClient.OrderAPI.TradingOrdersOrderIdDelete(context.Background(), orderId).UserId(userId).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `OrderAPI.TradingOrdersOrderIdDelete``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `TradingOrdersOrderIdDelete`: OrderResponseApiResponse
	fmt.Fprintf(os.Stdout, "Response from `OrderAPI.TradingOrdersOrderIdDelete`: %v\n", resp)
}
```

### Path Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
**ctx** | **context.Context** | context for authentication, logging, cancellation, deadlines, tracing, etc.
**orderId** | **string** |  | 

### Other Parameters

Other parameters are passed through a pointer to a apiTradingOrdersOrderIdDeleteRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **userId** | **string** |  | 


### Return type

[**OrderResponseApiResponse**](OrderResponseApiResponse.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: text/plain, application/json, text/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)


## TradingOrdersOrderIdPut

> OrderResponseApiResponse TradingOrdersOrderIdPut(ctx, orderId).UserId(userId).UpdateOrderRequest(updateOrderRequest).Execute()



### Example

```go
package main

import (
	"context"
	"fmt"
	"os"
	openapiclient "github.com/pi-financial/global-equities/go-client"
)

func main() {
	userId := "userId_example" // string | 
	orderId := "orderId_example" // string | 
	updateOrderRequest := *openapiclient.NewUpdateOrderRequest() // UpdateOrderRequest |  (optional)

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	resp, r, err := apiClient.OrderAPI.TradingOrdersOrderIdPut(context.Background(), orderId).UserId(userId).UpdateOrderRequest(updateOrderRequest).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `OrderAPI.TradingOrdersOrderIdPut``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `TradingOrdersOrderIdPut`: OrderResponseApiResponse
	fmt.Fprintf(os.Stdout, "Response from `OrderAPI.TradingOrdersOrderIdPut`: %v\n", resp)
}
```

### Path Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
**ctx** | **context.Context** | context for authentication, logging, cancellation, deadlines, tracing, etc.
**orderId** | **string** |  | 

### Other Parameters

Other parameters are passed through a pointer to a apiTradingOrdersOrderIdPutRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **userId** | **string** |  | 

 **updateOrderRequest** | [**UpdateOrderRequest**](UpdateOrderRequest.md) |  | 

### Return type

[**OrderResponseApiResponse**](OrderResponseApiResponse.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: application/json, text/json, application/*+json
- **Accept**: text/plain, application/json, text/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)


## TradingOrdersPost

> OrderResponseApiResponse TradingOrdersPost(ctx).UserId(userId).PlaceOrderRequest(placeOrderRequest).Execute()



### Example

```go
package main

import (
	"context"
	"fmt"
	"os"
	openapiclient "github.com/pi-financial/global-equities/go-client"
)

func main() {
	userId := "userId_example" // string | 
	placeOrderRequest := *openapiclient.NewPlaceOrderRequest("AccountId_example", "Venue_example", "Symbol_example", openapiclient.OrderType("Market"), openapiclient.OrderSide("Buy"), float32(123)) // PlaceOrderRequest |  (optional)

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	resp, r, err := apiClient.OrderAPI.TradingOrdersPost(context.Background()).UserId(userId).PlaceOrderRequest(placeOrderRequest).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `OrderAPI.TradingOrdersPost``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `TradingOrdersPost`: OrderResponseApiResponse
	fmt.Fprintf(os.Stdout, "Response from `OrderAPI.TradingOrdersPost`: %v\n", resp)
}
```

### Path Parameters



### Other Parameters

Other parameters are passed through a pointer to a apiTradingOrdersPostRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **userId** | **string** |  | 
 **placeOrderRequest** | [**PlaceOrderRequest**](PlaceOrderRequest.md) |  | 

### Return type

[**OrderResponseApiResponse**](OrderResponseApiResponse.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: application/json, text/json, application/*+json
- **Accept**: text/plain, application/json, text/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)

