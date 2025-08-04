# \ExaminationAPI

All URIs are relative to *http://localhost*

Method | HTTP request | Description
------------- | ------------- | -------------
[**CreateOrUpdateExamination**](ExaminationAPI.md#CreateOrUpdateExamination) | **Post** /internal/examination | 
[**GetExaminationsByUserId**](ExaminationAPI.md#GetExaminationsByUserId) | **Get** /internal/examination/{userId} | 



## CreateOrUpdateExamination

> PiUserApplicationCommandsSubmitExaminationResponseApiResponse CreateOrUpdateExamination(ctx).PiUserApplicationCommandsSubmitExaminationRequest(piUserApplicationCommandsSubmitExaminationRequest).Execute()



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
	piUserApplicationCommandsSubmitExaminationRequest := *openapiclient.NewPiUserApplicationCommandsSubmitExaminationRequest() // PiUserApplicationCommandsSubmitExaminationRequest |  (optional)

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	resp, r, err := apiClient.ExaminationAPI.CreateOrUpdateExamination(context.Background()).PiUserApplicationCommandsSubmitExaminationRequest(piUserApplicationCommandsSubmitExaminationRequest).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `ExaminationAPI.CreateOrUpdateExamination``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `CreateOrUpdateExamination`: PiUserApplicationCommandsSubmitExaminationResponseApiResponse
	fmt.Fprintf(os.Stdout, "Response from `ExaminationAPI.CreateOrUpdateExamination`: %v\n", resp)
}
```

### Path Parameters



### Other Parameters

Other parameters are passed through a pointer to a apiCreateOrUpdateExaminationRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **piUserApplicationCommandsSubmitExaminationRequest** | [**PiUserApplicationCommandsSubmitExaminationRequest**](PiUserApplicationCommandsSubmitExaminationRequest.md) |  | 

### Return type

[**PiUserApplicationCommandsSubmitExaminationResponseApiResponse**](PiUserApplicationCommandsSubmitExaminationResponseApiResponse.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: application/json, text/json, application/*+json
- **Accept**: text/plain, application/json, text/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)


## GetExaminationsByUserId

> PiUserApplicationModelsExaminationExaminationDtoListApiResponse GetExaminationsByUserId(ctx, userId).ExamName(examName).Execute()



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
	examName := "examName_example" // string |  (optional)

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	resp, r, err := apiClient.ExaminationAPI.GetExaminationsByUserId(context.Background(), userId).ExamName(examName).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `ExaminationAPI.GetExaminationsByUserId``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `GetExaminationsByUserId`: PiUserApplicationModelsExaminationExaminationDtoListApiResponse
	fmt.Fprintf(os.Stdout, "Response from `ExaminationAPI.GetExaminationsByUserId`: %v\n", resp)
}
```

### Path Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
**ctx** | **context.Context** | context for authentication, logging, cancellation, deadlines, tracing, etc.
**userId** | **string** |  | 

### Other Parameters

Other parameters are passed through a pointer to a apiGetExaminationsByUserIdRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------

 **examName** | **string** |  | 

### Return type

[**PiUserApplicationModelsExaminationExaminationDtoListApiResponse**](PiUserApplicationModelsExaminationExaminationDtoListApiResponse.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: text/plain, application/json, text/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)

