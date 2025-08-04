# \UserAccountAPI

All URIs are relative to *http://localhost*

Method | HTTP request | Description
------------- | ------------- | -------------
[**InternalUserAccountPost**](UserAccountAPI.md#InternalUserAccountPost) | **Post** /internal/user-account | 



## InternalUserAccountPost

> InternalUserAccountPost(ctx).PiUserAPIModelsCreateUserAccountRequest(piUserAPIModelsCreateUserAccountRequest).UserId(userId).Execute()



### Example

```go
package main

import (
	"context"
	"fmt"
	"os"
	openapiclient "github.com/pi-financial/user-srv/go-client"
)

func main() {
	piUserAPIModelsCreateUserAccountRequest := *openapiclient.NewPiUserAPIModelsCreateUserAccountRequest() // PiUserAPIModelsCreateUserAccountRequest | 
	userId := "38400000-8cf0-11bd-b23e-10b96e4ef00d" // string |  (optional)

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	r, err := apiClient.UserAccountAPI.InternalUserAccountPost(context.Background()).PiUserAPIModelsCreateUserAccountRequest(piUserAPIModelsCreateUserAccountRequest).UserId(userId).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `UserAccountAPI.InternalUserAccountPost``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
}
```

### Path Parameters



### Other Parameters

Other parameters are passed through a pointer to a apiInternalUserAccountPostRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **piUserAPIModelsCreateUserAccountRequest** | [**PiUserAPIModelsCreateUserAccountRequest**](PiUserAPIModelsCreateUserAccountRequest.md) |  | 
 **userId** | **string** |  | 

### Return type

 (empty response body)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: application/json, text/json, application/*+json
- **Accept**: Not defined

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)

