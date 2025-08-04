# \SyncAPI

All URIs are relative to *http://localhost*

Method | HTTP request | Description
------------- | ------------- | -------------
[**InternalSyncCustomerDataPost**](SyncAPI.md#InternalSyncCustomerDataPost) | **Post** /internal/sync/customer-data | 
[**InternalSyncFundordersPost**](SyncAPI.md#InternalSyncFundordersPost) | **Post** /internal/sync/fundorders | 
[**InternalSyncUnitholdersPost**](SyncAPI.md#InternalSyncUnitholdersPost) | **Post** /internal/sync/unitholders | 



## InternalSyncCustomerDataPost

> InternalSyncCustomerDataPost(ctx).CustomerCode(customerCode).CorrelationId(correlationId).BankAccountNo(bankAccountNo).Execute()



### Example

```go
package main

import (
	"context"
	"fmt"
	"os"
	openapiclient "github.com/pi-financial/fund-srv/go-client"
)

func main() {
	customerCode := "customerCode_example" // string |  (optional)
	correlationId := "38400000-8cf0-11bd-b23e-10b96e4ef00d" // string |  (optional)
	bankAccountNo := "bankAccountNo_example" // string |  (optional)

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	r, err := apiClient.SyncAPI.InternalSyncCustomerDataPost(context.Background()).CustomerCode(customerCode).CorrelationId(correlationId).BankAccountNo(bankAccountNo).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `SyncAPI.InternalSyncCustomerDataPost``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
}
```

### Path Parameters



### Other Parameters

Other parameters are passed through a pointer to a apiInternalSyncCustomerDataPostRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **customerCode** | **string** |  | 
 **correlationId** | **string** |  | 
 **bankAccountNo** | **string** |  | 

### Return type

 (empty response body)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: Not defined

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)


## InternalSyncFundordersPost

> InternalSyncFundordersPost(ctx).EffectiveDate(effectiveDate).ForceCreateOffline(forceCreateOffline).Execute()



### Example

```go
package main

import (
	"context"
	"fmt"
	"os"
    "time"
	openapiclient "github.com/pi-financial/fund-srv/go-client"
)

func main() {
	effectiveDate := time.Now() // string |  (optional)
	forceCreateOffline := true // bool |  (optional)

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	r, err := apiClient.SyncAPI.InternalSyncFundordersPost(context.Background()).EffectiveDate(effectiveDate).ForceCreateOffline(forceCreateOffline).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `SyncAPI.InternalSyncFundordersPost``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
}
```

### Path Parameters



### Other Parameters

Other parameters are passed through a pointer to a apiInternalSyncFundordersPostRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **effectiveDate** | **string** |  | 
 **forceCreateOffline** | **bool** |  | 

### Return type

 (empty response body)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: Not defined

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)


## InternalSyncUnitholdersPost

> InternalSyncUnitholdersPost(ctx).Execute()



### Example

```go
package main

import (
	"context"
	"fmt"
	"os"
	openapiclient "github.com/pi-financial/fund-srv/go-client"
)

func main() {

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	r, err := apiClient.SyncAPI.InternalSyncUnitholdersPost(context.Background()).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `SyncAPI.InternalSyncUnitholdersPost``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
}
```

### Path Parameters

This endpoint does not need any parameter.

### Other Parameters

Other parameters are passed through a pointer to a apiInternalSyncUnitholdersPostRequest struct via the builder pattern


### Return type

 (empty response body)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: Not defined

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)

