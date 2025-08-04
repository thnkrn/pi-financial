# \UserAPI

All URIs are relative to *http://localhost*

Method | HTTP request | Description
------------- | ------------- | -------------
[**GetOrCreateUserV2**](UserAPI.md#GetOrCreateUserV2) | **Get** /internal/v2/user | Get or create user v2
[**GetUserByIdOrCustomerCodeV2**](UserAPI.md#GetUserByIdOrCustomerCodeV2) | **Get** /internal/v2/user/{id} | Get user by user id or customer code v2
[**InternalUserBulkGet**](UserAPI.md#InternalUserBulkGet) | **Get** /internal/user/bulk | 
[**InternalUserCitizenidCitizenIdGet**](UserAPI.md#InternalUserCitizenidCitizenIdGet) | **Get** /internal/user/citizenid/{citizenId} | 
[**InternalUserEmailEmailGet**](UserAPI.md#InternalUserEmailEmailGet) | **Get** /internal/user/email/{email} | 
[**InternalUserIdCitizenIdGet**](UserAPI.md#InternalUserIdCitizenIdGet) | **Get** /internal/user/{id}/citizen-id | 
[**InternalUserIdGet**](UserAPI.md#InternalUserIdGet) | **Get** /internal/user/{id} | 
[**InternalUserPost**](UserAPI.md#InternalUserPost) | **Post** /internal/user | 
[**SecureUserGet**](UserAPI.md#SecureUserGet) | **Get** /secure/user | 



## GetOrCreateUserV2

> PiUserApplicationModelsUserApiResponse GetOrCreateUserV2(ctx).CustomerId(customerId).Execute()

Get or create user v2

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
	customerId := "customerId_example" // string |  (optional)

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	resp, r, err := apiClient.UserAPI.GetOrCreateUserV2(context.Background()).CustomerId(customerId).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `UserAPI.GetOrCreateUserV2``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `GetOrCreateUserV2`: PiUserApplicationModelsUserApiResponse
	fmt.Fprintf(os.Stdout, "Response from `UserAPI.GetOrCreateUserV2`: %v\n", resp)
}
```

### Path Parameters



### Other Parameters

Other parameters are passed through a pointer to a apiGetOrCreateUserV2Request struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **customerId** | **string** |  | 

### Return type

[**PiUserApplicationModelsUserApiResponse**](PiUserApplicationModelsUserApiResponse.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: text/plain, application/json, text/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)


## GetUserByIdOrCustomerCodeV2

> PiUserApplicationModelsUserApiResponse GetUserByIdOrCustomerCodeV2(ctx, id).IsCustCode(isCustCode).Execute()

Get user by user id or customer code v2

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
	id := "id_example" // string | 
	isCustCode := true // bool |  (optional) (default to false)

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	resp, r, err := apiClient.UserAPI.GetUserByIdOrCustomerCodeV2(context.Background(), id).IsCustCode(isCustCode).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `UserAPI.GetUserByIdOrCustomerCodeV2``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `GetUserByIdOrCustomerCodeV2`: PiUserApplicationModelsUserApiResponse
	fmt.Fprintf(os.Stdout, "Response from `UserAPI.GetUserByIdOrCustomerCodeV2`: %v\n", resp)
}
```

### Path Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
**ctx** | **context.Context** | context for authentication, logging, cancellation, deadlines, tracing, etc.
**id** | **string** |  | 

### Other Parameters

Other parameters are passed through a pointer to a apiGetUserByIdOrCustomerCodeV2Request struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------

 **isCustCode** | **bool** |  | [default to false]

### Return type

[**PiUserApplicationModelsUserApiResponse**](PiUserApplicationModelsUserApiResponse.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: text/plain, application/json, text/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)


## InternalUserBulkGet

> PiUserAPIModelsUserInfoResponseIEnumerableApiResponse InternalUserBulkGet(ctx).Ids(ids).IsCustCode(isCustCode).Execute()



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
	ids := []string{"Inner_example"} // []string | 
	isCustCode := true // bool |  (optional) (default to false)

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	resp, r, err := apiClient.UserAPI.InternalUserBulkGet(context.Background()).Ids(ids).IsCustCode(isCustCode).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `UserAPI.InternalUserBulkGet``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `InternalUserBulkGet`: PiUserAPIModelsUserInfoResponseIEnumerableApiResponse
	fmt.Fprintf(os.Stdout, "Response from `UserAPI.InternalUserBulkGet`: %v\n", resp)
}
```

### Path Parameters



### Other Parameters

Other parameters are passed through a pointer to a apiInternalUserBulkGetRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **ids** | **[]string** |  | 
 **isCustCode** | **bool** |  | [default to false]

### Return type

[**PiUserAPIModelsUserInfoResponseIEnumerableApiResponse**](PiUserAPIModelsUserInfoResponseIEnumerableApiResponse.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: text/plain, application/json, text/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)


## InternalUserCitizenidCitizenIdGet

> PiUserAPIModelsUserInfoResponseApiResponse InternalUserCitizenidCitizenIdGet(ctx, citizenId).Execute()



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
	citizenId := "citizenId_example" // string | 

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	resp, r, err := apiClient.UserAPI.InternalUserCitizenidCitizenIdGet(context.Background(), citizenId).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `UserAPI.InternalUserCitizenidCitizenIdGet``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `InternalUserCitizenidCitizenIdGet`: PiUserAPIModelsUserInfoResponseApiResponse
	fmt.Fprintf(os.Stdout, "Response from `UserAPI.InternalUserCitizenidCitizenIdGet`: %v\n", resp)
}
```

### Path Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
**ctx** | **context.Context** | context for authentication, logging, cancellation, deadlines, tracing, etc.
**citizenId** | **string** |  | 

### Other Parameters

Other parameters are passed through a pointer to a apiInternalUserCitizenidCitizenIdGetRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------


### Return type

[**PiUserAPIModelsUserInfoResponseApiResponse**](PiUserAPIModelsUserInfoResponseApiResponse.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: text/plain, application/json, text/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)


## InternalUserEmailEmailGet

> PiUserAPIModelsUserInfoResponseApiResponse InternalUserEmailEmailGet(ctx, email).Execute()



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
	email := "email_example" // string | 

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	resp, r, err := apiClient.UserAPI.InternalUserEmailEmailGet(context.Background(), email).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `UserAPI.InternalUserEmailEmailGet``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `InternalUserEmailEmailGet`: PiUserAPIModelsUserInfoResponseApiResponse
	fmt.Fprintf(os.Stdout, "Response from `UserAPI.InternalUserEmailEmailGet`: %v\n", resp)
}
```

### Path Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
**ctx** | **context.Context** | context for authentication, logging, cancellation, deadlines, tracing, etc.
**email** | **string** |  | 

### Other Parameters

Other parameters are passed through a pointer to a apiInternalUserEmailEmailGetRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------


### Return type

[**PiUserAPIModelsUserInfoResponseApiResponse**](PiUserAPIModelsUserInfoResponseApiResponse.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: text/plain, application/json, text/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)


## InternalUserIdCitizenIdGet

> PiUserAPIModelsUserInfoCitizenIdResponseApiResponse InternalUserIdCitizenIdGet(ctx, id).Execute()



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
	id := "id_example" // string | 

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	resp, r, err := apiClient.UserAPI.InternalUserIdCitizenIdGet(context.Background(), id).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `UserAPI.InternalUserIdCitizenIdGet``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `InternalUserIdCitizenIdGet`: PiUserAPIModelsUserInfoCitizenIdResponseApiResponse
	fmt.Fprintf(os.Stdout, "Response from `UserAPI.InternalUserIdCitizenIdGet`: %v\n", resp)
}
```

### Path Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
**ctx** | **context.Context** | context for authentication, logging, cancellation, deadlines, tracing, etc.
**id** | **string** |  | 

### Other Parameters

Other parameters are passed through a pointer to a apiInternalUserIdCitizenIdGetRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------


### Return type

[**PiUserAPIModelsUserInfoCitizenIdResponseApiResponse**](PiUserAPIModelsUserInfoCitizenIdResponseApiResponse.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: text/plain, application/json, text/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)


## InternalUserIdGet

> PiUserAPIModelsUserInfoResponseApiResponse InternalUserIdGet(ctx, id).IsCustCode(isCustCode).Execute()



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
	id := "id_example" // string | 
	isCustCode := true // bool |  (optional) (default to false)

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	resp, r, err := apiClient.UserAPI.InternalUserIdGet(context.Background(), id).IsCustCode(isCustCode).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `UserAPI.InternalUserIdGet``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `InternalUserIdGet`: PiUserAPIModelsUserInfoResponseApiResponse
	fmt.Fprintf(os.Stdout, "Response from `UserAPI.InternalUserIdGet`: %v\n", resp)
}
```

### Path Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
**ctx** | **context.Context** | context for authentication, logging, cancellation, deadlines, tracing, etc.
**id** | **string** |  | 

### Other Parameters

Other parameters are passed through a pointer to a apiInternalUserIdGetRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------

 **isCustCode** | **bool** |  | [default to false]

### Return type

[**PiUserAPIModelsUserInfoResponseApiResponse**](PiUserAPIModelsUserInfoResponseApiResponse.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: text/plain, application/json, text/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)


## InternalUserPost

> PiUserAPIModelsUserTicketIdApiResponse InternalUserPost(ctx).CustomerId(customerId).Device(device).PiUserAPIModelsUserInfoRequest(piUserAPIModelsUserInfoRequest).Execute()



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
	customerId := "customerId_example" // string | 
	device := "device_example" // string | 
	piUserAPIModelsUserInfoRequest := *openapiclient.NewPiUserAPIModelsUserInfoRequest() // PiUserAPIModelsUserInfoRequest | 

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	resp, r, err := apiClient.UserAPI.InternalUserPost(context.Background()).CustomerId(customerId).Device(device).PiUserAPIModelsUserInfoRequest(piUserAPIModelsUserInfoRequest).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `UserAPI.InternalUserPost``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `InternalUserPost`: PiUserAPIModelsUserTicketIdApiResponse
	fmt.Fprintf(os.Stdout, "Response from `UserAPI.InternalUserPost`: %v\n", resp)
}
```

### Path Parameters



### Other Parameters

Other parameters are passed through a pointer to a apiInternalUserPostRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **customerId** | **string** |  | 
 **device** | **string** |  | 
 **piUserAPIModelsUserInfoRequest** | [**PiUserAPIModelsUserInfoRequest**](PiUserAPIModelsUserInfoRequest.md) |  | 

### Return type

[**PiUserAPIModelsUserTicketIdApiResponse**](PiUserAPIModelsUserTicketIdApiResponse.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: application/json, text/json, application/*+json
- **Accept**: text/plain, application/json, text/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)


## SecureUserGet

> PiUserAPIModelsUserInfoResponseApiResponse SecureUserGet(ctx).UserId(userId).Execute()



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
	userId := "userId_example" // string |  (optional)

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	resp, r, err := apiClient.UserAPI.SecureUserGet(context.Background()).UserId(userId).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `UserAPI.SecureUserGet``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `SecureUserGet`: PiUserAPIModelsUserInfoResponseApiResponse
	fmt.Fprintf(os.Stdout, "Response from `UserAPI.SecureUserGet`: %v\n", resp)
}
```

### Path Parameters



### Other Parameters

Other parameters are passed through a pointer to a apiSecureUserGetRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **userId** | **string** |  | 

### Return type

[**PiUserAPIModelsUserInfoResponseApiResponse**](PiUserAPIModelsUserInfoResponseApiResponse.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: text/plain, application/json, text/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)

