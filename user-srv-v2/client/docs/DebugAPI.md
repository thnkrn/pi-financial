# \DebugAPI

All URIs are relative to *http://localhost*

Method | HTTP request | Description
------------- | ------------- | -------------
[**InternalV1DebugHashPost**](DebugAPI.md#InternalV1DebugHashPost) | **Post** /internal/v1/debug/hash | Get hash of input string
[**InternalV1DebugTryFeatureServiceGet**](DebugAPI.md#InternalV1DebugTryFeatureServiceGet) | **Get** /internal/v1/debug/try-feature-service | Try feature service
[**InternalV1DebugTryFeatureServiceWithHeadersGet**](DebugAPI.md#InternalV1DebugTryFeatureServiceWithHeadersGet) | **Get** /internal/v1/debug/try-feature-service/with-headers | Try feature service with headers



## InternalV1DebugHashPost

> InternalV1DebugHashPost200Response InternalV1DebugHashPost(ctx).DtoHashRequest(dtoHashRequest).Execute()

Get hash of input string



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
	dtoHashRequest := *openapiclient.NewDtoHashRequest("Input_example") // DtoHashRequest | String to hash

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	resp, r, err := apiClient.DebugAPI.InternalV1DebugHashPost(context.Background()).DtoHashRequest(dtoHashRequest).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `DebugAPI.InternalV1DebugHashPost``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `InternalV1DebugHashPost`: InternalV1DebugHashPost200Response
	fmt.Fprintf(os.Stdout, "Response from `DebugAPI.InternalV1DebugHashPost`: %v\n", resp)
}
```

### Path Parameters



### Other Parameters

Other parameters are passed through a pointer to a apiInternalV1DebugHashPostRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **dtoHashRequest** | [**DtoHashRequest**](DtoHashRequest.md) | String to hash |

### Return type

[**InternalV1DebugHashPost200Response**](InternalV1DebugHashPost200Response.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: application/json
- **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)


## InternalV1DebugTryFeatureServiceGet

> InternalV1DebugTryFeatureServiceGet200Response InternalV1DebugTryFeatureServiceGet(ctx).FeatureSwitchName(featureSwitchName).Execute()

Try feature service



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
	featureSwitchName := "featureSwitchName_example" // string | Feature Switch Name

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	resp, r, err := apiClient.DebugAPI.InternalV1DebugTryFeatureServiceGet(context.Background()).FeatureSwitchName(featureSwitchName).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `DebugAPI.InternalV1DebugTryFeatureServiceGet``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `InternalV1DebugTryFeatureServiceGet`: InternalV1DebugTryFeatureServiceGet200Response
	fmt.Fprintf(os.Stdout, "Response from `DebugAPI.InternalV1DebugTryFeatureServiceGet`: %v\n", resp)
}
```

### Path Parameters



### Other Parameters

Other parameters are passed through a pointer to a apiInternalV1DebugTryFeatureServiceGetRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **featureSwitchName** | **string** | Feature Switch Name |

### Return type

[**InternalV1DebugTryFeatureServiceGet200Response**](InternalV1DebugTryFeatureServiceGet200Response.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)


## InternalV1DebugTryFeatureServiceWithHeadersGet

> InternalV1DebugTryFeatureServiceGet200Response InternalV1DebugTryFeatureServiceWithHeadersGet(ctx).FeatureSwitchName(featureSwitchName).UserId(userId).DeviceId(deviceId).Random(random).Execute()

Try feature service with headers



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
	featureSwitchName := "featureSwitchName_example" // string | Feature Switch Name
	userId := "userId_example" // string | User ID (optional)
	deviceId := "deviceId_example" // string | Device ID (optional)
	random := "random_example" // string | Random (optional)

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	resp, r, err := apiClient.DebugAPI.InternalV1DebugTryFeatureServiceWithHeadersGet(context.Background()).FeatureSwitchName(featureSwitchName).UserId(userId).DeviceId(deviceId).Random(random).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `DebugAPI.InternalV1DebugTryFeatureServiceWithHeadersGet``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `InternalV1DebugTryFeatureServiceWithHeadersGet`: InternalV1DebugTryFeatureServiceGet200Response
	fmt.Fprintf(os.Stdout, "Response from `DebugAPI.InternalV1DebugTryFeatureServiceWithHeadersGet`: %v\n", resp)
}
```

### Path Parameters



### Other Parameters

Other parameters are passed through a pointer to a apiInternalV1DebugTryFeatureServiceWithHeadersGetRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **featureSwitchName** | **string** | Feature Switch Name |
 **userId** | **string** | User ID |
 **deviceId** | **string** | Device ID |
 **random** | **string** | Random |

### Return type

[**InternalV1DebugTryFeatureServiceGet200Response**](InternalV1DebugTryFeatureServiceGet200Response.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)
