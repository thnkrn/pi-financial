# \DocumentAPI

All URIs are relative to *http://localhost*

Method | HTTP request | Description
------------- | ------------- | -------------
[**GetDocumentsByUserId**](DocumentAPI.md#GetDocumentsByUserId) | **Get** /internal/document/{userId} | 
[**UploadDocument**](DocumentAPI.md#UploadDocument) | **Post** /secure/document | 



## GetDocumentsByUserId

> PiUserApplicationModelsDocumentDocumentDtoListApiResponse GetDocumentsByUserId(ctx, userId).Execute()



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
	userId := "userId_example" // string | 

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	resp, r, err := apiClient.DocumentAPI.GetDocumentsByUserId(context.Background(), userId).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `DocumentAPI.GetDocumentsByUserId``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `GetDocumentsByUserId`: PiUserApplicationModelsDocumentDocumentDtoListApiResponse
	fmt.Fprintf(os.Stdout, "Response from `DocumentAPI.GetDocumentsByUserId`: %v\n", resp)
}
```

### Path Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
**ctx** | **context.Context** | context for authentication, logging, cancellation, deadlines, tracing, etc.
**userId** | **string** |  | 

### Other Parameters

Other parameters are passed through a pointer to a apiGetDocumentsByUserIdRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------


### Return type

[**PiUserApplicationModelsDocumentDocumentDtoListApiResponse**](PiUserApplicationModelsDocumentDocumentDtoListApiResponse.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: text/plain, application/json, text/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)


## UploadDocument

> UploadDocument(ctx).UserId(userId).PiUserApplicationCommandsUploadDocumentRequest(piUserApplicationCommandsUploadDocumentRequest).Execute()



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
	userId := "userId_example" // string | 
	piUserApplicationCommandsUploadDocumentRequest := *openapiclient.NewPiUserApplicationCommandsUploadDocumentRequest() // PiUserApplicationCommandsUploadDocumentRequest | 

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	r, err := apiClient.DocumentAPI.UploadDocument(context.Background()).UserId(userId).PiUserApplicationCommandsUploadDocumentRequest(piUserApplicationCommandsUploadDocumentRequest).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `DocumentAPI.UploadDocument``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
}
```

### Path Parameters



### Other Parameters

Other parameters are passed through a pointer to a apiUploadDocumentRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **userId** | **string** |  | 
 **piUserApplicationCommandsUploadDocumentRequest** | [**PiUserApplicationCommandsUploadDocumentRequest**](PiUserApplicationCommandsUploadDocumentRequest.md) |  | 

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

