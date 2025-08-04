# \SuitabilityTestAPI

All URIs are relative to *http://localhost*

Method | HTTP request | Description
------------- | ------------- | -------------
[**InternalV1SuitabilityTestPost**](SuitabilityTestAPI.md#InternalV1SuitabilityTestPost) | **Post** /internal/v1/suitability-test | Create new suitability test for user
[**InternalV1SuitabilityTestsGet**](SuitabilityTestAPI.md#InternalV1SuitabilityTestsGet) | **Get** /internal/v1/suitability-tests | Get all suitability tests for user



## InternalV1SuitabilityTestPost

> ResultResponseSuccess InternalV1SuitabilityTestPost(ctx).UserId(userId).DtoSuitabilityTestRequest(dtoSuitabilityTestRequest).Execute()

Create new suitability test for user



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
	dtoSuitabilityTestRequest := *openapiclient.NewDtoSuitabilityTestRequest() // DtoSuitabilityTestRequest | Suitability Test Create Request

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	resp, r, err := apiClient.SuitabilityTestAPI.InternalV1SuitabilityTestPost(context.Background()).UserId(userId).DtoSuitabilityTestRequest(dtoSuitabilityTestRequest).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `SuitabilityTestAPI.InternalV1SuitabilityTestPost``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `InternalV1SuitabilityTestPost`: ResultResponseSuccess
	fmt.Fprintf(os.Stdout, "Response from `SuitabilityTestAPI.InternalV1SuitabilityTestPost`: %v\n", resp)
}
```

### Path Parameters



### Other Parameters

Other parameters are passed through a pointer to a apiInternalV1SuitabilityTestPostRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **userId** | **string** | User ID |
 **dtoSuitabilityTestRequest** | [**DtoSuitabilityTestRequest**](DtoSuitabilityTestRequest.md) | Suitability Test Create Request |

### Return type

[**ResultResponseSuccess**](ResultResponseSuccess.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: application/json
- **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)


## InternalV1SuitabilityTestsGet

> InternalV1SuitabilityTestsGet200Response InternalV1SuitabilityTestsGet(ctx).UserId(userId).Execute()

Get all suitability tests for user



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

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	resp, r, err := apiClient.SuitabilityTestAPI.InternalV1SuitabilityTestsGet(context.Background()).UserId(userId).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `SuitabilityTestAPI.InternalV1SuitabilityTestsGet``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `InternalV1SuitabilityTestsGet`: InternalV1SuitabilityTestsGet200Response
	fmt.Fprintf(os.Stdout, "Response from `SuitabilityTestAPI.InternalV1SuitabilityTestsGet`: %v\n", resp)
}
```

### Path Parameters



### Other Parameters

Other parameters are passed through a pointer to a apiInternalV1SuitabilityTestsGetRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **userId** | **string** | User ID |

### Return type

[**InternalV1SuitabilityTestsGet200Response**](InternalV1SuitabilityTestsGet200Response.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)
