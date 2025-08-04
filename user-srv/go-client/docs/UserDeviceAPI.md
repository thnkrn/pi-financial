# \UserDeviceAPI

All URIs are relative to *http://localhost*

Method | HTTP request | Description
------------- | ------------- | -------------
[**InternalUserDeviceDelete**](UserDeviceAPI.md#InternalUserDeviceDelete) | **Delete** /internal/user/device | 
[**InternalUserDevicePost**](UserDeviceAPI.md#InternalUserDevicePost) | **Post** /internal/user/device | Internal Create or update device
[**InternalUserIdDeviceGet**](UserDeviceAPI.md#InternalUserIdDeviceGet) | **Get** /internal/user/{id}/device | 
[**SecureUserDeviceDelete**](UserDeviceAPI.md#SecureUserDeviceDelete) | **Delete** /secure/user/device | 
[**SecureUserDevicePost**](UserDeviceAPI.md#SecureUserDevicePost) | **Post** /secure/user/device | Create or update device



## InternalUserDeviceDelete

> PiUserAPIModelsUpdateUserInfoTicketIdApiResponse InternalUserDeviceDelete(ctx).UserId(userId).DeviceId(deviceId).Execute()



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
	userId := "38400000-8cf0-11bd-b23e-10b96e4ef00d" // string | 
	deviceId := "38400000-8cf0-11bd-b23e-10b96e4ef00d" // string | 

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	resp, r, err := apiClient.UserDeviceAPI.InternalUserDeviceDelete(context.Background()).UserId(userId).DeviceId(deviceId).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `UserDeviceAPI.InternalUserDeviceDelete``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `InternalUserDeviceDelete`: PiUserAPIModelsUpdateUserInfoTicketIdApiResponse
	fmt.Fprintf(os.Stdout, "Response from `UserDeviceAPI.InternalUserDeviceDelete`: %v\n", resp)
}
```

### Path Parameters



### Other Parameters

Other parameters are passed through a pointer to a apiInternalUserDeviceDeleteRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **userId** | **string** |  | 
 **deviceId** | **string** |  | 

### Return type

[**PiUserAPIModelsUpdateUserInfoTicketIdApiResponse**](PiUserAPIModelsUpdateUserInfoTicketIdApiResponse.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: text/plain, application/json, text/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)


## InternalUserDevicePost

> PiUserAPIModelsUpdateUserInfoTicketIdApiResponse InternalUserDevicePost(ctx).UserId(userId).DeviceId(deviceId).Device(device).AcceptLanguage(acceptLanguage).PiUserAPIModelsCreateOrUpdateDeviceRequest(piUserAPIModelsCreateOrUpdateDeviceRequest).Execute()

Internal Create or update device

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
	deviceId := "deviceId_example" // string | 
	device := "device_example" // string | 
	acceptLanguage := "acceptLanguage_example" // string |  (default to "th-TH")
	piUserAPIModelsCreateOrUpdateDeviceRequest := *openapiclient.NewPiUserAPIModelsCreateOrUpdateDeviceRequest() // PiUserAPIModelsCreateOrUpdateDeviceRequest | 

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	resp, r, err := apiClient.UserDeviceAPI.InternalUserDevicePost(context.Background()).UserId(userId).DeviceId(deviceId).Device(device).AcceptLanguage(acceptLanguage).PiUserAPIModelsCreateOrUpdateDeviceRequest(piUserAPIModelsCreateOrUpdateDeviceRequest).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `UserDeviceAPI.InternalUserDevicePost``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `InternalUserDevicePost`: PiUserAPIModelsUpdateUserInfoTicketIdApiResponse
	fmt.Fprintf(os.Stdout, "Response from `UserDeviceAPI.InternalUserDevicePost`: %v\n", resp)
}
```

### Path Parameters



### Other Parameters

Other parameters are passed through a pointer to a apiInternalUserDevicePostRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **userId** | **string** |  | 
 **deviceId** | **string** |  | 
 **device** | **string** |  | 
 **acceptLanguage** | **string** |  | [default to &quot;th-TH&quot;]
 **piUserAPIModelsCreateOrUpdateDeviceRequest** | [**PiUserAPIModelsCreateOrUpdateDeviceRequest**](PiUserAPIModelsCreateOrUpdateDeviceRequest.md) |  | 

### Return type

[**PiUserAPIModelsUpdateUserInfoTicketIdApiResponse**](PiUserAPIModelsUpdateUserInfoTicketIdApiResponse.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: application/json, text/json, application/*+json
- **Accept**: text/plain, application/json, text/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)


## InternalUserIdDeviceGet

> PiUserAPIModelsDeviceResponseListApiResponse InternalUserIdDeviceGet(ctx, id).Execute()



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
	id := "38400000-8cf0-11bd-b23e-10b96e4ef00d" // string | 

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	resp, r, err := apiClient.UserDeviceAPI.InternalUserIdDeviceGet(context.Background(), id).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `UserDeviceAPI.InternalUserIdDeviceGet``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `InternalUserIdDeviceGet`: PiUserAPIModelsDeviceResponseListApiResponse
	fmt.Fprintf(os.Stdout, "Response from `UserDeviceAPI.InternalUserIdDeviceGet`: %v\n", resp)
}
```

### Path Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
**ctx** | **context.Context** | context for authentication, logging, cancellation, deadlines, tracing, etc.
**id** | **string** |  | 

### Other Parameters

Other parameters are passed through a pointer to a apiInternalUserIdDeviceGetRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------


### Return type

[**PiUserAPIModelsDeviceResponseListApiResponse**](PiUserAPIModelsDeviceResponseListApiResponse.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: text/plain, application/json, text/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)


## SecureUserDeviceDelete

> PiUserAPIModelsUpdateUserInfoTicketIdApiResponse SecureUserDeviceDelete(ctx).UserId(userId).DeviceId(deviceId).Execute()



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
	userId := "38400000-8cf0-11bd-b23e-10b96e4ef00d" // string | 
	deviceId := "38400000-8cf0-11bd-b23e-10b96e4ef00d" // string | 

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	resp, r, err := apiClient.UserDeviceAPI.SecureUserDeviceDelete(context.Background()).UserId(userId).DeviceId(deviceId).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `UserDeviceAPI.SecureUserDeviceDelete``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `SecureUserDeviceDelete`: PiUserAPIModelsUpdateUserInfoTicketIdApiResponse
	fmt.Fprintf(os.Stdout, "Response from `UserDeviceAPI.SecureUserDeviceDelete`: %v\n", resp)
}
```

### Path Parameters



### Other Parameters

Other parameters are passed through a pointer to a apiSecureUserDeviceDeleteRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **userId** | **string** |  | 
 **deviceId** | **string** |  | 

### Return type

[**PiUserAPIModelsUpdateUserInfoTicketIdApiResponse**](PiUserAPIModelsUpdateUserInfoTicketIdApiResponse.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: text/plain, application/json, text/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)


## SecureUserDevicePost

> PiUserAPIModelsUpdateUserInfoTicketIdApiResponse SecureUserDevicePost(ctx).UserId(userId).DeviceId(deviceId).Device(device).AcceptLanguage(acceptLanguage).PiUserAPIModelsCreateOrUpdateDeviceRequest(piUserAPIModelsCreateOrUpdateDeviceRequest).Sid(sid).Execute()

Create or update device

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
	deviceId := "deviceId_example" // string | 
	device := "device_example" // string | 
	acceptLanguage := "acceptLanguage_example" // string |  (default to "th-TH")
	piUserAPIModelsCreateOrUpdateDeviceRequest := *openapiclient.NewPiUserAPIModelsCreateOrUpdateDeviceRequest() // PiUserAPIModelsCreateOrUpdateDeviceRequest | 
	sid := "sid_example" // string |  (optional)

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	resp, r, err := apiClient.UserDeviceAPI.SecureUserDevicePost(context.Background()).UserId(userId).DeviceId(deviceId).Device(device).AcceptLanguage(acceptLanguage).PiUserAPIModelsCreateOrUpdateDeviceRequest(piUserAPIModelsCreateOrUpdateDeviceRequest).Sid(sid).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `UserDeviceAPI.SecureUserDevicePost``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `SecureUserDevicePost`: PiUserAPIModelsUpdateUserInfoTicketIdApiResponse
	fmt.Fprintf(os.Stdout, "Response from `UserDeviceAPI.SecureUserDevicePost`: %v\n", resp)
}
```

### Path Parameters



### Other Parameters

Other parameters are passed through a pointer to a apiSecureUserDevicePostRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **userId** | **string** |  | 
 **deviceId** | **string** |  | 
 **device** | **string** |  | 
 **acceptLanguage** | **string** |  | [default to &quot;th-TH&quot;]
 **piUserAPIModelsCreateOrUpdateDeviceRequest** | [**PiUserAPIModelsCreateOrUpdateDeviceRequest**](PiUserAPIModelsCreateOrUpdateDeviceRequest.md) |  | 
 **sid** | **string** |  | 

### Return type

[**PiUserAPIModelsUpdateUserInfoTicketIdApiResponse**](PiUserAPIModelsUpdateUserInfoTicketIdApiResponse.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: application/json, text/json, application/*+json
- **Accept**: text/plain, application/json, text/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)

