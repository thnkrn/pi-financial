# \UserMigrationAPI

All URIs are relative to *http://localhost*

Method | HTTP request | Description
------------- | ------------- | -------------
[**CreateUserV2**](UserMigrationAPI.md#CreateUserV2) | **Post** /internal/v2/user | 
[**GetUserIdByCustCodeForLogin**](UserMigrationAPI.md#GetUserIdByCustCodeForLogin) | **Get** /internal/user/login/{customerCode} | Get user id by customer code for login.
[**InternalGetTradingAccountV2**](UserMigrationAPI.md#InternalGetTradingAccountV2) | **Get** /internal/v2/trading-accounts | Returns list of trading accounts belonging to userId, grouped by customer code.
[**InternalUserCustomerCustomerIdGet**](UserMigrationAPI.md#InternalUserCustomerCustomerIdGet) | **Get** /internal/user/customer/{customerId} | Get user by customer id
[**InternalUserIdByCustomerCodeGet**](UserMigrationAPI.md#InternalUserIdByCustomerCodeGet) | **Get** /internal/user/id-by-customer-code | 
[**InternalUserPhoneByPhoneGet**](UserMigrationAPI.md#InternalUserPhoneByPhoneGet) | **Get** /internal/user/phone/by-phone | Get user by phone number
[**PartialUpdateUserInfo**](UserMigrationAPI.md#PartialUpdateUserInfo) | **Patch** /internal/user | 
[**SecureGetTradingAccountV2**](UserMigrationAPI.md#SecureGetTradingAccountV2) | **Get** /secure/v2/trading-accounts | Returns list of trading accounts belonging to userId, grouped by customer code.
[**SecureUserNameGet**](UserMigrationAPI.md#SecureUserNameGet) | **Get** /secure/user/name | 



## CreateUserV2

> PiUserAPIModelsCreateUserResponseApiResponse CreateUserV2(ctx).Device(device).PiUserAPIModelsCreateUserRequest(piUserAPIModelsCreateUserRequest).Execute()



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
	device := "device_example" // string |  (optional)
	piUserAPIModelsCreateUserRequest := *openapiclient.NewPiUserAPIModelsCreateUserRequest() // PiUserAPIModelsCreateUserRequest |  (optional)

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	resp, r, err := apiClient.UserMigrationAPI.CreateUserV2(context.Background()).Device(device).PiUserAPIModelsCreateUserRequest(piUserAPIModelsCreateUserRequest).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `UserMigrationAPI.CreateUserV2``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `CreateUserV2`: PiUserAPIModelsCreateUserResponseApiResponse
	fmt.Fprintf(os.Stdout, "Response from `UserMigrationAPI.CreateUserV2`: %v\n", resp)
}
```

### Path Parameters



### Other Parameters

Other parameters are passed through a pointer to a apiCreateUserV2Request struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **device** | **string** |  | 
 **piUserAPIModelsCreateUserRequest** | [**PiUserAPIModelsCreateUserRequest**](PiUserAPIModelsCreateUserRequest.md) |  | 

### Return type

[**PiUserAPIModelsCreateUserResponseApiResponse**](PiUserAPIModelsCreateUserResponseApiResponse.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: application/json, text/json, application/*+json
- **Accept**: text/plain, application/json, text/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)


## GetUserIdByCustCodeForLogin

> PiUserAPIModelsUserInfoForLoginResponseApiResponse GetUserIdByCustCodeForLogin(ctx, customerCode).Execute()

Get user id by customer code for login.

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
	customerCode := "customerCode_example" // string | The customer code.

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	resp, r, err := apiClient.UserMigrationAPI.GetUserIdByCustCodeForLogin(context.Background(), customerCode).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `UserMigrationAPI.GetUserIdByCustCodeForLogin``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `GetUserIdByCustCodeForLogin`: PiUserAPIModelsUserInfoForLoginResponseApiResponse
	fmt.Fprintf(os.Stdout, "Response from `UserMigrationAPI.GetUserIdByCustCodeForLogin`: %v\n", resp)
}
```

### Path Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
**ctx** | **context.Context** | context for authentication, logging, cancellation, deadlines, tracing, etc.
**customerCode** | **string** | The customer code. | 

### Other Parameters

Other parameters are passed through a pointer to a apiGetUserIdByCustCodeForLoginRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------


### Return type

[**PiUserAPIModelsUserInfoForLoginResponseApiResponse**](PiUserAPIModelsUserInfoForLoginResponseApiResponse.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: text/plain, application/json, text/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)


## InternalGetTradingAccountV2

> PiUserApplicationModelsUserTradingAccountInfoWithExternalAccountsListApiResponse InternalGetTradingAccountV2(ctx).UserId(userId).Execute()

Returns list of trading accounts belonging to userId, grouped by customer code.

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
	userId := "38400000-8cf0-11bd-b23e-10b96e4ef00d" // string | Guid user id to query trading accounts for.

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	resp, r, err := apiClient.UserMigrationAPI.InternalGetTradingAccountV2(context.Background()).UserId(userId).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `UserMigrationAPI.InternalGetTradingAccountV2``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `InternalGetTradingAccountV2`: PiUserApplicationModelsUserTradingAccountInfoWithExternalAccountsListApiResponse
	fmt.Fprintf(os.Stdout, "Response from `UserMigrationAPI.InternalGetTradingAccountV2`: %v\n", resp)
}
```

### Path Parameters



### Other Parameters

Other parameters are passed through a pointer to a apiInternalGetTradingAccountV2Request struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **userId** | **string** | Guid user id to query trading accounts for. | 

### Return type

[**PiUserApplicationModelsUserTradingAccountInfoWithExternalAccountsListApiResponse**](PiUserApplicationModelsUserTradingAccountInfoWithExternalAccountsListApiResponse.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: text/plain, application/json, text/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)


## InternalUserCustomerCustomerIdGet

> PiUserAPIModelsUserInfoResponseApiResponse InternalUserCustomerCustomerIdGet(ctx, customerId).Execute()

Get user by customer id

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
	customerId := "customerId_example" // string | Customer Id

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	resp, r, err := apiClient.UserMigrationAPI.InternalUserCustomerCustomerIdGet(context.Background(), customerId).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `UserMigrationAPI.InternalUserCustomerCustomerIdGet``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `InternalUserCustomerCustomerIdGet`: PiUserAPIModelsUserInfoResponseApiResponse
	fmt.Fprintf(os.Stdout, "Response from `UserMigrationAPI.InternalUserCustomerCustomerIdGet`: %v\n", resp)
}
```

### Path Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
**ctx** | **context.Context** | context for authentication, logging, cancellation, deadlines, tracing, etc.
**customerId** | **string** | Customer Id | 

### Other Parameters

Other parameters are passed through a pointer to a apiInternalUserCustomerCustomerIdGetRequest struct via the builder pattern


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


## InternalUserIdByCustomerCodeGet

> SystemGuidApiResponse InternalUserIdByCustomerCodeGet(ctx).CustomerCode(customerCode).Execute()



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
	customerCode := "customerCode_example" // string |  (optional)

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	resp, r, err := apiClient.UserMigrationAPI.InternalUserIdByCustomerCodeGet(context.Background()).CustomerCode(customerCode).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `UserMigrationAPI.InternalUserIdByCustomerCodeGet``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `InternalUserIdByCustomerCodeGet`: SystemGuidApiResponse
	fmt.Fprintf(os.Stdout, "Response from `UserMigrationAPI.InternalUserIdByCustomerCodeGet`: %v\n", resp)
}
```

### Path Parameters



### Other Parameters

Other parameters are passed through a pointer to a apiInternalUserIdByCustomerCodeGetRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **customerCode** | **string** |  | 

### Return type

[**SystemGuidApiResponse**](SystemGuidApiResponse.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: text/plain, application/json, text/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)


## InternalUserPhoneByPhoneGet

> PiUserAPIModelsUserInfoResponseApiResponse InternalUserPhoneByPhoneGet(ctx).PhoneNumber(phoneNumber).Execute()

Get user by phone number

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
	phoneNumber := "phoneNumber_example" // string | Phone Number (optional)

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	resp, r, err := apiClient.UserMigrationAPI.InternalUserPhoneByPhoneGet(context.Background()).PhoneNumber(phoneNumber).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `UserMigrationAPI.InternalUserPhoneByPhoneGet``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `InternalUserPhoneByPhoneGet`: PiUserAPIModelsUserInfoResponseApiResponse
	fmt.Fprintf(os.Stdout, "Response from `UserMigrationAPI.InternalUserPhoneByPhoneGet`: %v\n", resp)
}
```

### Path Parameters



### Other Parameters

Other parameters are passed through a pointer to a apiInternalUserPhoneByPhoneGetRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **phoneNumber** | **string** | Phone Number | 

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


## PartialUpdateUserInfo

> PartialUpdateUserInfo(ctx).UserId(userId).PiUserAPIModelsPartialUpdateUserInfoRequest(piUserAPIModelsPartialUpdateUserInfoRequest).Execute()



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
	piUserAPIModelsPartialUpdateUserInfoRequest := *openapiclient.NewPiUserAPIModelsPartialUpdateUserInfoRequest() // PiUserAPIModelsPartialUpdateUserInfoRequest |  (optional)

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	r, err := apiClient.UserMigrationAPI.PartialUpdateUserInfo(context.Background()).UserId(userId).PiUserAPIModelsPartialUpdateUserInfoRequest(piUserAPIModelsPartialUpdateUserInfoRequest).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `UserMigrationAPI.PartialUpdateUserInfo``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
}
```

### Path Parameters



### Other Parameters

Other parameters are passed through a pointer to a apiPartialUpdateUserInfoRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **userId** | **string** |  | 
 **piUserAPIModelsPartialUpdateUserInfoRequest** | [**PiUserAPIModelsPartialUpdateUserInfoRequest**](PiUserAPIModelsPartialUpdateUserInfoRequest.md) |  | 

### Return type

 (empty response body)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: application/json, text/json, application/*+json
- **Accept**: text/plain, application/json, text/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)


## SecureGetTradingAccountV2

> PiUserApplicationModelsUserTradingAccountInfoWithExternalAccountsListApiResponse SecureGetTradingAccountV2(ctx).UserId(userId).Execute()

Returns list of trading accounts belonging to userId, grouped by customer code.

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
	userId := "38400000-8cf0-11bd-b23e-10b96e4ef00d" // string | Guid user id to query trading accounts for.

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	resp, r, err := apiClient.UserMigrationAPI.SecureGetTradingAccountV2(context.Background()).UserId(userId).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `UserMigrationAPI.SecureGetTradingAccountV2``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `SecureGetTradingAccountV2`: PiUserApplicationModelsUserTradingAccountInfoWithExternalAccountsListApiResponse
	fmt.Fprintf(os.Stdout, "Response from `UserMigrationAPI.SecureGetTradingAccountV2`: %v\n", resp)
}
```

### Path Parameters



### Other Parameters

Other parameters are passed through a pointer to a apiSecureGetTradingAccountV2Request struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **userId** | **string** | Guid user id to query trading accounts for. | 

### Return type

[**PiUserApplicationModelsUserTradingAccountInfoWithExternalAccountsListApiResponse**](PiUserApplicationModelsUserTradingAccountInfoWithExternalAccountsListApiResponse.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: text/plain, application/json, text/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)


## SecureUserNameGet

> PiUserAPIModelsUserNameResponseApiResponse SecureUserNameGet(ctx).UserId(userId).Execute()



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
	resp, r, err := apiClient.UserMigrationAPI.SecureUserNameGet(context.Background()).UserId(userId).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `UserMigrationAPI.SecureUserNameGet``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `SecureUserNameGet`: PiUserAPIModelsUserNameResponseApiResponse
	fmt.Fprintf(os.Stdout, "Response from `UserMigrationAPI.SecureUserNameGet`: %v\n", resp)
}
```

### Path Parameters



### Other Parameters

Other parameters are passed through a pointer to a apiSecureUserNameGetRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **userId** | **string** |  | 

### Return type

[**PiUserAPIModelsUserNameResponseApiResponse**](PiUserAPIModelsUserNameResponseApiResponse.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: text/plain, application/json, text/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)

