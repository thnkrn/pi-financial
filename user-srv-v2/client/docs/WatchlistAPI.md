# \WatchlistAPI

All URIs are relative to *http://localhost*

Method | HTTP request | Description
------------- | ------------- | -------------
[**InternalV1WatchlistsGet**](WatchlistAPI.md#InternalV1WatchlistsGet) | **Get** /internal/v1/watchlists | Get user&#39;s watchlist
[**SecureV1WatchlistsPost**](WatchlistAPI.md#SecureV1WatchlistsPost) | **Post** /secure/v1/watchlists | Create or delete watchlist item



## InternalV1WatchlistsGet

> InternalV1WatchlistsGet200Response InternalV1WatchlistsGet(ctx).UserId(userId).Venue(venue).Execute()

Get user's watchlist



### Example

```go
package main

import (
	"context"
	"fmt"
	"os"
	openapiclient "github.com/pi-financial/user-srv-v2/client"
)

func main() {
	userId := "userId_example" // string | User ID
	venue := "venue_example" // string | Get watchlist request (optional)

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	resp, r, err := apiClient.WatchlistAPI.InternalV1WatchlistsGet(context.Background()).UserId(userId).Venue(venue).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `WatchlistAPI.InternalV1WatchlistsGet``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `InternalV1WatchlistsGet`: InternalV1WatchlistsGet200Response
	fmt.Fprintf(os.Stdout, "Response from `WatchlistAPI.InternalV1WatchlistsGet`: %v\n", resp)
}
```

### Path Parameters



### Other Parameters

Other parameters are passed through a pointer to a apiInternalV1WatchlistsGetRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **userId** | **string** | User ID |
 **venue** | **string** | Get watchlist request |

### Return type

[**InternalV1WatchlistsGet200Response**](InternalV1WatchlistsGet200Response.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)


## SecureV1WatchlistsPost

> SecureV1WatchlistsPost200Response SecureV1WatchlistsPost(ctx).UserId(userId).DtoOptWatchlistRequest(dtoOptWatchlistRequest).Execute()

Create or delete watchlist item



### Example

```go
package main

import (
	"context"
	"fmt"
	"os"
	openapiclient "github.com/pi-financial/user-srv-v2/client"
)

func main() {
	userId := "userId_example" // string | User ID
	dtoOptWatchlistRequest := *openapiclient.NewDtoOptWatchlistRequest("Opt_example", "Symbol_example", "Venue_example") // DtoOptWatchlistRequest | Watchlist operation request

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	resp, r, err := apiClient.WatchlistAPI.SecureV1WatchlistsPost(context.Background()).UserId(userId).DtoOptWatchlistRequest(dtoOptWatchlistRequest).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `WatchlistAPI.SecureV1WatchlistsPost``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `SecureV1WatchlistsPost`: SecureV1WatchlistsPost200Response
	fmt.Fprintf(os.Stdout, "Response from `WatchlistAPI.SecureV1WatchlistsPost`: %v\n", resp)
}
```

### Path Parameters



### Other Parameters

Other parameters are passed through a pointer to a apiSecureV1WatchlistsPostRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **userId** | **string** | User ID |
 **dtoOptWatchlistRequest** | [**DtoOptWatchlistRequest**](DtoOptWatchlistRequest.md) | Watchlist operation request |

### Return type

[**SecureV1WatchlistsPost200Response**](SecureV1WatchlistsPost200Response.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: application/json
- **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)
