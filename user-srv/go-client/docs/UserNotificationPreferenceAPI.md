# \UserNotificationPreferenceAPI

All URIs are relative to *http://localhost*

Method | HTTP request | Description
------------- | ------------- | -------------
[**InternalNotificationPreferenceGet**](UserNotificationPreferenceAPI.md#InternalNotificationPreferenceGet) | **Get** /internal/notification-preference | 
[**SecureNotificationPreferenceGet**](UserNotificationPreferenceAPI.md#SecureNotificationPreferenceGet) | **Get** /secure/notification-preference | 
[**SecureNotificationPreferencePut**](UserNotificationPreferenceAPI.md#SecureNotificationPreferencePut) | **Put** /secure/notification-preference | 



## InternalNotificationPreferenceGet

> PiUserAPIModelsDeviceResponseApiResponse InternalNotificationPreferenceGet(ctx).UserId(userId).DeviceId(deviceId).Execute()



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

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	resp, r, err := apiClient.UserNotificationPreferenceAPI.InternalNotificationPreferenceGet(context.Background()).UserId(userId).DeviceId(deviceId).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `UserNotificationPreferenceAPI.InternalNotificationPreferenceGet``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `InternalNotificationPreferenceGet`: PiUserAPIModelsDeviceResponseApiResponse
	fmt.Fprintf(os.Stdout, "Response from `UserNotificationPreferenceAPI.InternalNotificationPreferenceGet`: %v\n", resp)
}
```

### Path Parameters



### Other Parameters

Other parameters are passed through a pointer to a apiInternalNotificationPreferenceGetRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **userId** | **string** |  | 
 **deviceId** | **string** |  | 

### Return type

[**PiUserAPIModelsDeviceResponseApiResponse**](PiUserAPIModelsDeviceResponseApiResponse.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: text/plain, application/json, text/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)


## SecureNotificationPreferenceGet

> PiUserAPIModelsDeviceResponseApiResponse SecureNotificationPreferenceGet(ctx).UserId(userId).DeviceId(deviceId).Execute()



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

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	resp, r, err := apiClient.UserNotificationPreferenceAPI.SecureNotificationPreferenceGet(context.Background()).UserId(userId).DeviceId(deviceId).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `UserNotificationPreferenceAPI.SecureNotificationPreferenceGet``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `SecureNotificationPreferenceGet`: PiUserAPIModelsDeviceResponseApiResponse
	fmt.Fprintf(os.Stdout, "Response from `UserNotificationPreferenceAPI.SecureNotificationPreferenceGet`: %v\n", resp)
}
```

### Path Parameters



### Other Parameters

Other parameters are passed through a pointer to a apiSecureNotificationPreferenceGetRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **userId** | **string** |  | 
 **deviceId** | **string** |  | 

### Return type

[**PiUserAPIModelsDeviceResponseApiResponse**](PiUserAPIModelsDeviceResponseApiResponse.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: text/plain, application/json, text/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)


## SecureNotificationPreferencePut

> PiUserAPIModelsNotificationPreferenceTicketApiResponse SecureNotificationPreferencePut(ctx).UserId(userId).DeviceId(deviceId).PiUserAPIModelsNotificationPreferenceRequest(piUserAPIModelsNotificationPreferenceRequest).Execute()



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
	piUserAPIModelsNotificationPreferenceRequest := *openapiclient.NewPiUserAPIModelsNotificationPreferenceRequest() // PiUserAPIModelsNotificationPreferenceRequest | 

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	resp, r, err := apiClient.UserNotificationPreferenceAPI.SecureNotificationPreferencePut(context.Background()).UserId(userId).DeviceId(deviceId).PiUserAPIModelsNotificationPreferenceRequest(piUserAPIModelsNotificationPreferenceRequest).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `UserNotificationPreferenceAPI.SecureNotificationPreferencePut``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `SecureNotificationPreferencePut`: PiUserAPIModelsNotificationPreferenceTicketApiResponse
	fmt.Fprintf(os.Stdout, "Response from `UserNotificationPreferenceAPI.SecureNotificationPreferencePut`: %v\n", resp)
}
```

### Path Parameters



### Other Parameters

Other parameters are passed through a pointer to a apiSecureNotificationPreferencePutRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **userId** | **string** |  | 
 **deviceId** | **string** |  | 
 **piUserAPIModelsNotificationPreferenceRequest** | [**PiUserAPIModelsNotificationPreferenceRequest**](PiUserAPIModelsNotificationPreferenceRequest.md) |  | 

### Return type

[**PiUserAPIModelsNotificationPreferenceTicketApiResponse**](PiUserAPIModelsNotificationPreferenceTicketApiResponse.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: application/json, text/json, application/*+json
- **Accept**: text/plain, application/json, text/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)

