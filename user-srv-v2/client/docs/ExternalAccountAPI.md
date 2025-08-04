# \ExternalAccountAPI

All URIs are relative to *http://localhost*

Method | HTTP request | Description
------------- | ------------- | -------------
[**InternalV1ExternalAccountPost**](ExternalAccountAPI.md#InternalV1ExternalAccountPost) | **Post** /internal/v1/external-account | Create external account.



## InternalV1ExternalAccountPost

> ResultResponseSuccess InternalV1ExternalAccountPost(ctx).UserId(userId).DtoCreateExternalAccountRequest(dtoCreateExternalAccountRequest).Execute()

Create external account.



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
	dtoCreateExternalAccountRequest := *openapiclient.NewDtoCreateExternalAccountRequest() // DtoCreateExternalAccountRequest | Create External Account Request

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	resp, r, err := apiClient.ExternalAccountAPI.InternalV1ExternalAccountPost(context.Background()).UserId(userId).DtoCreateExternalAccountRequest(dtoCreateExternalAccountRequest).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `ExternalAccountAPI.InternalV1ExternalAccountPost``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `InternalV1ExternalAccountPost`: ResultResponseSuccess
	fmt.Fprintf(os.Stdout, "Response from `ExternalAccountAPI.InternalV1ExternalAccountPost`: %v\n", resp)
}
```

### Path Parameters



### Other Parameters

Other parameters are passed through a pointer to a apiInternalV1ExternalAccountPostRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **userId** | **string** | User ID |
 **dtoCreateExternalAccountRequest** | [**DtoCreateExternalAccountRequest**](DtoCreateExternalAccountRequest.md) | Create External Account Request |

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
