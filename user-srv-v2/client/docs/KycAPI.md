# \KycAPI

All URIs are relative to *http://localhost*

Method | HTTP request | Description
------------- | ------------- | -------------
[**InternalV1KycsGet**](KycAPI.md#InternalV1KycsGet) | **Get** /internal/v1/kycs | Get KYC by user ID
[**InternalV1KycsPost**](KycAPI.md#InternalV1KycsPost) | **Post** /internal/v1/kycs | Create KYC



## InternalV1KycsGet

> InternalV1KycsGet200Response InternalV1KycsGet(ctx).UserId(userId).Execute()

Get KYC by user ID



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
	resp, r, err := apiClient.KycAPI.InternalV1KycsGet(context.Background()).UserId(userId).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `KycAPI.InternalV1KycsGet``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `InternalV1KycsGet`: InternalV1KycsGet200Response
	fmt.Fprintf(os.Stdout, "Response from `KycAPI.InternalV1KycsGet`: %v\n", resp)
}
```

### Path Parameters



### Other Parameters

Other parameters are passed through a pointer to a apiInternalV1KycsGetRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **userId** | **string** | User ID |

### Return type

[**InternalV1KycsGet200Response**](InternalV1KycsGet200Response.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)


## InternalV1KycsPost

> InternalV1KycsPost(ctx).UserId(userId).DtoCreateKycRequest(dtoCreateKycRequest).Execute()

Create KYC



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
	dtoCreateKycRequest := *openapiclient.NewDtoCreateKycRequest("ExpiredDate_example", "ReviewDate_example") // DtoCreateKycRequest | KYC request

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	r, err := apiClient.KycAPI.InternalV1KycsPost(context.Background()).UserId(userId).DtoCreateKycRequest(dtoCreateKycRequest).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `KycAPI.InternalV1KycsPost``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
}
```

### Path Parameters



### Other Parameters

Other parameters are passed through a pointer to a apiInternalV1KycsPostRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **userId** | **string** | User ID |
 **dtoCreateKycRequest** | [**DtoCreateKycRequest**](DtoCreateKycRequest.md) | KYC request |

### Return type

 (empty response body)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: application/json
- **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)
