# \ExchangeRateAPI

All URIs are relative to *http://localhost*

Method | HTTP request | Description
------------- | ------------- | -------------
[**InternalExchangeRateGet**](ExchangeRateAPI.md#InternalExchangeRateGet) | **Get** /internal/exchange-rate | Get Exchange Rates
[**SecureExchangeRateGet**](ExchangeRateAPI.md#SecureExchangeRateGet) | **Get** /secure/exchange-rate | Get Exchange Rates



## InternalExchangeRateGet

> InternalExchangeRateGet200Response InternalExchangeRateGet(ctx).From(from).To(to).FromCur(fromCur).ToCur(toCur).Execute()

Get Exchange Rates



### Example

```go
package main

import (
	"context"
	"fmt"
	"os"
	openapiclient "github.com/pi-financial/information-srv/client"
)

func main() {
	from := "from_example" // string | Start date to retrieve the exchange rate (format: YYYY-MM-DD).
	to := "to_example" // string | End date to retrieve the exchange rate (format: YYYY-MM-DD).
	fromCur := "fromCur_example" // string | Source currency code (optional)
	toCur := "toCur_example" // string | Destination currency code (optional)

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	resp, r, err := apiClient.ExchangeRateAPI.InternalExchangeRateGet(context.Background()).From(from).To(to).FromCur(fromCur).ToCur(toCur).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `ExchangeRateAPI.InternalExchangeRateGet``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `InternalExchangeRateGet`: InternalExchangeRateGet200Response
	fmt.Fprintf(os.Stdout, "Response from `ExchangeRateAPI.InternalExchangeRateGet`: %v\n", resp)
}
```

### Path Parameters



### Other Parameters

Other parameters are passed through a pointer to a apiInternalExchangeRateGetRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **from** | **string** | Start date to retrieve the exchange rate (format: YYYY-MM-DD). | 
 **to** | **string** | End date to retrieve the exchange rate (format: YYYY-MM-DD). | 
 **fromCur** | **string** | Source currency code | 
 **toCur** | **string** | Destination currency code | 

### Return type

[**InternalExchangeRateGet200Response**](InternalExchangeRateGet200Response.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)


## SecureExchangeRateGet

> InternalExchangeRateGet200Response SecureExchangeRateGet(ctx).From(from).To(to).FromCur(fromCur).ToCur(toCur).Execute()

Get Exchange Rates



### Example

```go
package main

import (
	"context"
	"fmt"
	"os"
	openapiclient "github.com/pi-financial/information-srv/client"
)

func main() {
	from := "from_example" // string | Start date to retrieve the exchange rate (format: YYYY-MM-DD).
	to := "to_example" // string | End date to retrieve the exchange rate (format: YYYY-MM-DD).
	fromCur := "fromCur_example" // string | Source currency code (optional)
	toCur := "toCur_example" // string | Destination currency code (optional)

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	resp, r, err := apiClient.ExchangeRateAPI.SecureExchangeRateGet(context.Background()).From(from).To(to).FromCur(fromCur).ToCur(toCur).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `ExchangeRateAPI.SecureExchangeRateGet``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `SecureExchangeRateGet`: InternalExchangeRateGet200Response
	fmt.Fprintf(os.Stdout, "Response from `ExchangeRateAPI.SecureExchangeRateGet`: %v\n", resp)
}
```

### Path Parameters



### Other Parameters

Other parameters are passed through a pointer to a apiSecureExchangeRateGetRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **from** | **string** | Start date to retrieve the exchange rate (format: YYYY-MM-DD). | 
 **to** | **string** | End date to retrieve the exchange rate (format: YYYY-MM-DD). | 
 **fromCur** | **string** | Source currency code | 
 **toCur** | **string** | Destination currency code | 

### Return type

[**InternalExchangeRateGet200Response**](InternalExchangeRateGet200Response.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)

