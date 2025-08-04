# \AuthAPI

All URIs are relative to *http://localhost:8080*

Method | HTTP request | Description
------------- | ------------- | -------------
[**InternalAuthCreatePasswordPost**](AuthAPI.md#InternalAuthCreatePasswordPost) | **Post** /internal/auth/create-password | Create or reset user&#39;s password
[**InternalAuthUnlockAccountPost**](AuthAPI.md#InternalAuthUnlockAccountPost) | **Post** /internal/auth/unlock-account | Unlock a locked account
[**InternalAuthVerifyPinPost**](AuthAPI.md#InternalAuthVerifyPinPost) | **Post** /internal/auth/verify-pin | Verify user&#39;s PIN
[**PublicAuthLoginPost**](AuthAPI.md#PublicAuthLoginPost) | **Post** /public/auth/login | Login with username and password
[**PublicAuthRequestResetPasswordPost**](AuthAPI.md#PublicAuthRequestResetPasswordPost) | **Post** /public/auth/request-reset-password | Request Reset Password Link
[**SecureAuthChangePasswordPost**](AuthAPI.md#SecureAuthChangePasswordPost) | **Post** /secure/auth/change-password | Change user&#39;s password
[**SecureAuthChangePinPost**](AuthAPI.md#SecureAuthChangePinPost) | **Post** /secure/auth/change-pin | Change user&#39;s PIN
[**SecureAuthCreatePasswordPost**](AuthAPI.md#SecureAuthCreatePasswordPost) | **Post** /secure/auth/create-password | Create or reset user&#39;s password
[**SecureAuthCreatePinPost**](AuthAPI.md#SecureAuthCreatePinPost) | **Post** /secure/auth/create-pin | Create a new PIN for user
[**SecureAuthRefreshTokenPost**](AuthAPI.md#SecureAuthRefreshTokenPost) | **Post** /secure/auth/refresh-token | Refresh JWT tokens
[**SecureAuthResetPinPost**](AuthAPI.md#SecureAuthResetPinPost) | **Post** /secure/auth/reset-pin | Reset user&#39;s PIN



## InternalAuthCreatePasswordPost

> map[string]string InternalAuthCreatePasswordPost(ctx).Request(request).Execute()

Create or reset user's password



### Example

```go
package main

import (
	"context"
	"fmt"
	"os"
	openapiclient "github.com/pi-financial/pi-sso-v2/client"
)

func main() {
	request := *openapiclient.NewHandlerInternalCreatePasswordRequest("EncryptNewPassword_example", "UserId_example", []string{"Username_example"}) // HandlerInternalCreatePasswordRequest | Create Password Request Body

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	resp, r, err := apiClient.AuthAPI.InternalAuthCreatePasswordPost(context.Background()).Request(request).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `AuthAPI.InternalAuthCreatePasswordPost``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `InternalAuthCreatePasswordPost`: map[string]string
	fmt.Fprintf(os.Stdout, "Response from `AuthAPI.InternalAuthCreatePasswordPost`: %v\n", resp)
}
```

### Path Parameters



### Other Parameters

Other parameters are passed through a pointer to a apiInternalAuthCreatePasswordPostRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **request** | [**HandlerInternalCreatePasswordRequest**](HandlerInternalCreatePasswordRequest.md) | Create Password Request Body | 

### Return type

**map[string]string**

### Authorization

[BearerAuth](../README.md#BearerAuth)

### HTTP request headers

- **Content-Type**: application/json
- **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)


## InternalAuthUnlockAccountPost

> map[string]string InternalAuthUnlockAccountPost(ctx).Request(request).Execute()

Unlock a locked account



### Example

```go
package main

import (
	"context"
	"fmt"
	"os"
	openapiclient "github.com/pi-financial/pi-sso-v2/client"
)

func main() {
	request := *openapiclient.NewHandlerUnlockUserRequest("Username_example") // HandlerUnlockUserRequest | Unlock Account Request Body

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	resp, r, err := apiClient.AuthAPI.InternalAuthUnlockAccountPost(context.Background()).Request(request).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `AuthAPI.InternalAuthUnlockAccountPost``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `InternalAuthUnlockAccountPost`: map[string]string
	fmt.Fprintf(os.Stdout, "Response from `AuthAPI.InternalAuthUnlockAccountPost`: %v\n", resp)
}
```

### Path Parameters



### Other Parameters

Other parameters are passed through a pointer to a apiInternalAuthUnlockAccountPostRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **request** | [**HandlerUnlockUserRequest**](HandlerUnlockUserRequest.md) | Unlock Account Request Body | 

### Return type

**map[string]string**

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: application/json
- **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)


## InternalAuthVerifyPinPost

> map[string]string InternalAuthVerifyPinPost(ctx).Request(request).Execute()

Verify user's PIN



### Example

```go
package main

import (
	"context"
	"fmt"
	"os"
	openapiclient "github.com/pi-financial/pi-sso-v2/client"
)

func main() {
	request := *openapiclient.NewHandlerVerifyPinRequest("Custcode_example", "EncryptPin_example") // HandlerVerifyPinRequest | Verify Pin Request Body

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	resp, r, err := apiClient.AuthAPI.InternalAuthVerifyPinPost(context.Background()).Request(request).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `AuthAPI.InternalAuthVerifyPinPost``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `InternalAuthVerifyPinPost`: map[string]string
	fmt.Fprintf(os.Stdout, "Response from `AuthAPI.InternalAuthVerifyPinPost`: %v\n", resp)
}
```

### Path Parameters



### Other Parameters

Other parameters are passed through a pointer to a apiInternalAuthVerifyPinPostRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **request** | [**HandlerVerifyPinRequest**](HandlerVerifyPinRequest.md) | Verify Pin Request Body | 

### Return type

**map[string]string**

### Authorization

[BearerAuth](../README.md#BearerAuth)

### HTTP request headers

- **Content-Type**: application/json
- **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)


## PublicAuthLoginPost

> map[string]string PublicAuthLoginPost(ctx).Request(request).Execute()

Login with username and password



### Example

```go
package main

import (
	"context"
	"fmt"
	"os"
	openapiclient "github.com/pi-financial/pi-sso-v2/client"
)

func main() {
	request := *openapiclient.NewHandlerLoginRequest("EncryptPassword_example", "Username_example") // HandlerLoginRequest | Login request body

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	resp, r, err := apiClient.AuthAPI.PublicAuthLoginPost(context.Background()).Request(request).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `AuthAPI.PublicAuthLoginPost``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `PublicAuthLoginPost`: map[string]string
	fmt.Fprintf(os.Stdout, "Response from `AuthAPI.PublicAuthLoginPost`: %v\n", resp)
}
```

### Path Parameters



### Other Parameters

Other parameters are passed through a pointer to a apiPublicAuthLoginPostRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **request** | [**HandlerLoginRequest**](HandlerLoginRequest.md) | Login request body | 

### Return type

**map[string]string**

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: application/json
- **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)


## PublicAuthRequestResetPasswordPost

> map[string]string PublicAuthRequestResetPasswordPost(ctx).Request(request).Execute()

Request Reset Password Link



### Example

```go
package main

import (
	"context"
	"fmt"
	"os"
	openapiclient "github.com/pi-financial/pi-sso-v2/client"
)

func main() {
	request := *openapiclient.NewHandlerResetPasswordRequest("Birthday_example", "IdCardNo_example", "Username_example") // HandlerResetPasswordRequest | Reset Password Request Body

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	resp, r, err := apiClient.AuthAPI.PublicAuthRequestResetPasswordPost(context.Background()).Request(request).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `AuthAPI.PublicAuthRequestResetPasswordPost``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `PublicAuthRequestResetPasswordPost`: map[string]string
	fmt.Fprintf(os.Stdout, "Response from `AuthAPI.PublicAuthRequestResetPasswordPost`: %v\n", resp)
}
```

### Path Parameters



### Other Parameters

Other parameters are passed through a pointer to a apiPublicAuthRequestResetPasswordPostRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **request** | [**HandlerResetPasswordRequest**](HandlerResetPasswordRequest.md) | Reset Password Request Body | 

### Return type

**map[string]string**

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: application/json
- **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)


## SecureAuthChangePasswordPost

> map[string]string SecureAuthChangePasswordPost(ctx).Request(request).Execute()

Change user's password



### Example

```go
package main

import (
	"context"
	"fmt"
	"os"
	openapiclient "github.com/pi-financial/pi-sso-v2/client"
)

func main() {
	request := *openapiclient.NewHandlerChangePasswordRequest("EncryptNewPassword_example", "EncryptOldPassword_example", "Username_example") // HandlerChangePasswordRequest | Change Password Request Body

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	resp, r, err := apiClient.AuthAPI.SecureAuthChangePasswordPost(context.Background()).Request(request).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `AuthAPI.SecureAuthChangePasswordPost``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `SecureAuthChangePasswordPost`: map[string]string
	fmt.Fprintf(os.Stdout, "Response from `AuthAPI.SecureAuthChangePasswordPost`: %v\n", resp)
}
```

### Path Parameters



### Other Parameters

Other parameters are passed through a pointer to a apiSecureAuthChangePasswordPostRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **request** | [**HandlerChangePasswordRequest**](HandlerChangePasswordRequest.md) | Change Password Request Body | 

### Return type

**map[string]string**

### Authorization

[BearerAuth](../README.md#BearerAuth)

### HTTP request headers

- **Content-Type**: application/json
- **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)


## SecureAuthChangePinPost

> map[string]string SecureAuthChangePinPost(ctx).Request(request).Execute()

Change user's PIN



### Example

```go
package main

import (
	"context"
	"fmt"
	"os"
	openapiclient "github.com/pi-financial/pi-sso-v2/client"
)

func main() {
	request := *openapiclient.NewHandlerChangePinRequest("Custcode_example", "EncryptNewPin_example", "EncryptOldPin_example") // HandlerChangePinRequest | Change Pin Request Body

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	resp, r, err := apiClient.AuthAPI.SecureAuthChangePinPost(context.Background()).Request(request).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `AuthAPI.SecureAuthChangePinPost``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `SecureAuthChangePinPost`: map[string]string
	fmt.Fprintf(os.Stdout, "Response from `AuthAPI.SecureAuthChangePinPost`: %v\n", resp)
}
```

### Path Parameters



### Other Parameters

Other parameters are passed through a pointer to a apiSecureAuthChangePinPostRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **request** | [**HandlerChangePinRequest**](HandlerChangePinRequest.md) | Change Pin Request Body | 

### Return type

**map[string]string**

### Authorization

[BearerAuth](../README.md#BearerAuth)

### HTTP request headers

- **Content-Type**: application/json
- **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)


## SecureAuthCreatePasswordPost

> map[string]string SecureAuthCreatePasswordPost(ctx).Request(request).Execute()

Create or reset user's password



### Example

```go
package main

import (
	"context"
	"fmt"
	"os"
	openapiclient "github.com/pi-financial/pi-sso-v2/client"
)

func main() {
	request := *openapiclient.NewHandlerCreatePasswordRequest("EncryptNewPassword_example", []string{"Username_example"}) // HandlerCreatePasswordRequest | Create Password Request Body

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	resp, r, err := apiClient.AuthAPI.SecureAuthCreatePasswordPost(context.Background()).Request(request).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `AuthAPI.SecureAuthCreatePasswordPost``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `SecureAuthCreatePasswordPost`: map[string]string
	fmt.Fprintf(os.Stdout, "Response from `AuthAPI.SecureAuthCreatePasswordPost`: %v\n", resp)
}
```

### Path Parameters



### Other Parameters

Other parameters are passed through a pointer to a apiSecureAuthCreatePasswordPostRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **request** | [**HandlerCreatePasswordRequest**](HandlerCreatePasswordRequest.md) | Create Password Request Body | 

### Return type

**map[string]string**

### Authorization

[BearerAuth](../README.md#BearerAuth)

### HTTP request headers

- **Content-Type**: application/json
- **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)


## SecureAuthCreatePinPost

> map[string]string SecureAuthCreatePinPost(ctx).Request(request).Execute()

Create a new PIN for user



### Example

```go
package main

import (
	"context"
	"fmt"
	"os"
	openapiclient "github.com/pi-financial/pi-sso-v2/client"
)

func main() {
	request := *openapiclient.NewHandlerCreatePinRequest([]string{"Custcode_example"}, "EncryptNewPin_example") // HandlerCreatePinRequest | Create Pin Request Body

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	resp, r, err := apiClient.AuthAPI.SecureAuthCreatePinPost(context.Background()).Request(request).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `AuthAPI.SecureAuthCreatePinPost``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `SecureAuthCreatePinPost`: map[string]string
	fmt.Fprintf(os.Stdout, "Response from `AuthAPI.SecureAuthCreatePinPost`: %v\n", resp)
}
```

### Path Parameters



### Other Parameters

Other parameters are passed through a pointer to a apiSecureAuthCreatePinPostRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **request** | [**HandlerCreatePinRequest**](HandlerCreatePinRequest.md) | Create Pin Request Body | 

### Return type

**map[string]string**

### Authorization

[BearerAuth](../README.md#BearerAuth)

### HTTP request headers

- **Content-Type**: application/json
- **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)


## SecureAuthRefreshTokenPost

> map[string]string SecureAuthRefreshTokenPost(ctx).Execute()

Refresh JWT tokens



### Example

```go
package main

import (
	"context"
	"fmt"
	"os"
	openapiclient "github.com/pi-financial/pi-sso-v2/client"
)

func main() {

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	resp, r, err := apiClient.AuthAPI.SecureAuthRefreshTokenPost(context.Background()).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `AuthAPI.SecureAuthRefreshTokenPost``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `SecureAuthRefreshTokenPost`: map[string]string
	fmt.Fprintf(os.Stdout, "Response from `AuthAPI.SecureAuthRefreshTokenPost`: %v\n", resp)
}
```

### Path Parameters

This endpoint does not need any parameter.

### Other Parameters

Other parameters are passed through a pointer to a apiSecureAuthRefreshTokenPostRequest struct via the builder pattern


### Return type

**map[string]string**

### Authorization

[BearerAuth](../README.md#BearerAuth)

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)


## SecureAuthResetPinPost

> map[string]string SecureAuthResetPinPost(ctx).Request(request).Execute()

Reset user's PIN



### Example

```go
package main

import (
	"context"
	"fmt"
	"os"
	openapiclient "github.com/pi-financial/pi-sso-v2/client"
)

func main() {
	request := *openapiclient.NewHandlerResetPinRequest([]string{"Custcode_example"}, "EncryptNewPin_example", "EncryptPassword_example") // HandlerResetPinRequest | Reset Pin Request Body

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	resp, r, err := apiClient.AuthAPI.SecureAuthResetPinPost(context.Background()).Request(request).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `AuthAPI.SecureAuthResetPinPost``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `SecureAuthResetPinPost`: map[string]string
	fmt.Fprintf(os.Stdout, "Response from `AuthAPI.SecureAuthResetPinPost`: %v\n", resp)
}
```

### Path Parameters



### Other Parameters

Other parameters are passed through a pointer to a apiSecureAuthResetPinPostRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **request** | [**HandlerResetPinRequest**](HandlerResetPinRequest.md) | Reset Pin Request Body | 

### Return type

**map[string]string**

### Authorization

[BearerAuth](../README.md#BearerAuth)

### HTTP request headers

- **Content-Type**: application/json
- **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)

