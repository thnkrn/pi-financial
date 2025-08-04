# \SyncAPI

All URIs are relative to *http://localhost*

Method | HTTP request | Description
------------- | ------------- | -------------
[**InternalSyncInitialMarginPost**](SyncAPI.md#InternalSyncInitialMarginPost) | **Post** /internal/sync/initial-margin | 
[**InternalSyncSblInstrumentsPost**](SyncAPI.md#InternalSyncSblInstrumentsPost) | **Post** /internal/sync/sbl-instruments | 



## InternalSyncInitialMarginPost

> InternalSyncInitialMarginPost(ctx).PiSetServiceApplicationCommandsSyncInitialMarginRequest(piSetServiceApplicationCommandsSyncInitialMarginRequest).Execute()



### Example

```go
package main

import (
	"context"
	"fmt"
	"os"
	openapiclient "github.com/pi-financial/set-srv/go-client"
)

func main() {
	piSetServiceApplicationCommandsSyncInitialMarginRequest := *openapiclient.NewPiSetServiceApplicationCommandsSyncInitialMarginRequest("BucketName_example", "FileKey_example") // PiSetServiceApplicationCommandsSyncInitialMarginRequest |  (optional)

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	r, err := apiClient.SyncAPI.InternalSyncInitialMarginPost(context.Background()).PiSetServiceApplicationCommandsSyncInitialMarginRequest(piSetServiceApplicationCommandsSyncInitialMarginRequest).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `SyncAPI.InternalSyncInitialMarginPost``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
}
```

### Path Parameters



### Other Parameters

Other parameters are passed through a pointer to a apiInternalSyncInitialMarginPostRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **piSetServiceApplicationCommandsSyncInitialMarginRequest** | [**PiSetServiceApplicationCommandsSyncInitialMarginRequest**](PiSetServiceApplicationCommandsSyncInitialMarginRequest.md) |  | 

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


## InternalSyncSblInstrumentsPost

> PiSetServiceApplicationModelsSyncProcessResultApiResponse InternalSyncSblInstrumentsPost(ctx).PiSetServiceApplicationCommandsSyncSblInstrument(piSetServiceApplicationCommandsSyncSblInstrument).Execute()



### Example

```go
package main

import (
	"context"
	"fmt"
	"os"
	openapiclient "github.com/pi-financial/set-srv/go-client"
)

func main() {
	piSetServiceApplicationCommandsSyncSblInstrument := *openapiclient.NewPiSetServiceApplicationCommandsSyncSblInstrument("BucketName_example", "FileKey_example") // PiSetServiceApplicationCommandsSyncSblInstrument |  (optional)

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	resp, r, err := apiClient.SyncAPI.InternalSyncSblInstrumentsPost(context.Background()).PiSetServiceApplicationCommandsSyncSblInstrument(piSetServiceApplicationCommandsSyncSblInstrument).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `SyncAPI.InternalSyncSblInstrumentsPost``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `InternalSyncSblInstrumentsPost`: PiSetServiceApplicationModelsSyncProcessResultApiResponse
	fmt.Fprintf(os.Stdout, "Response from `SyncAPI.InternalSyncSblInstrumentsPost`: %v\n", resp)
}
```

### Path Parameters



### Other Parameters

Other parameters are passed through a pointer to a apiInternalSyncSblInstrumentsPostRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **piSetServiceApplicationCommandsSyncSblInstrument** | [**PiSetServiceApplicationCommandsSyncSblInstrument**](PiSetServiceApplicationCommandsSyncSblInstrument.md) |  | 

### Return type

[**PiSetServiceApplicationModelsSyncProcessResultApiResponse**](PiSetServiceApplicationModelsSyncProcessResultApiResponse.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: application/json, text/json, application/*+json
- **Accept**: text/plain, application/json, text/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)

