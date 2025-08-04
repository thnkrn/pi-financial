# \AccountAPI

All URIs are relative to *http://localhost:8080*

Method | HTTP request | Description
------------- | ------------- | -------------
[**InternalAccountsCheckDuplicatePost**](AccountAPI.md#InternalAccountsCheckDuplicatePost) | **Post** /internal/accounts/check-duplicate | Check Email and PhoneNumber Duplication
[**PublicAccountsGuestRegisterPost**](AccountAPI.md#PublicAccountsGuestRegisterPost) | **Post** /public/accounts/guest/register | Register a new guest account
[**SecureAccountsLinkAccountPost**](AccountAPI.md#SecureAccountsLinkAccountPost) | **Post** /secure/accounts/link-account | Link an account to a member



## InternalAccountsCheckDuplicatePost

> HandlerCheckDuplicateResponse InternalAccountsCheckDuplicatePost(ctx).Request(request).Execute()

Check Email and PhoneNumber Duplication



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
	request := *openapiclient.NewHandlerCheckDuplicateRequest() // HandlerCheckDuplicateRequest | Check Duplicate Request

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	resp, r, err := apiClient.AccountAPI.InternalAccountsCheckDuplicatePost(context.Background()).Request(request).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `AccountAPI.InternalAccountsCheckDuplicatePost``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `InternalAccountsCheckDuplicatePost`: HandlerCheckDuplicateResponse
	fmt.Fprintf(os.Stdout, "Response from `AccountAPI.InternalAccountsCheckDuplicatePost`: %v\n", resp)
}
```

### Path Parameters



### Other Parameters

Other parameters are passed through a pointer to a apiInternalAccountsCheckDuplicatePostRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **request** | [**HandlerCheckDuplicateRequest**](HandlerCheckDuplicateRequest.md) | Check Duplicate Request | 

### Return type

[**HandlerCheckDuplicateResponse**](HandlerCheckDuplicateResponse.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: application/json
- **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)


## PublicAccountsGuestRegisterPost

> HandlerSuccessResponse PublicAccountsGuestRegisterPost(ctx).Request(request).Execute()

Register a new guest account



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
	request := *openapiclient.NewHandlerGuestRegisterRequest("Password_example", "Username_example") // HandlerGuestRegisterRequest | Guest Register Request Body

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	resp, r, err := apiClient.AccountAPI.PublicAccountsGuestRegisterPost(context.Background()).Request(request).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `AccountAPI.PublicAccountsGuestRegisterPost``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `PublicAccountsGuestRegisterPost`: HandlerSuccessResponse
	fmt.Fprintf(os.Stdout, "Response from `AccountAPI.PublicAccountsGuestRegisterPost`: %v\n", resp)
}
```

### Path Parameters



### Other Parameters

Other parameters are passed through a pointer to a apiPublicAccountsGuestRegisterPostRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **request** | [**HandlerGuestRegisterRequest**](HandlerGuestRegisterRequest.md) | Guest Register Request Body | 

### Return type

[**HandlerSuccessResponse**](HandlerSuccessResponse.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: application/json
- **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)


## SecureAccountsLinkAccountPost

> map[string]string SecureAccountsLinkAccountPost(ctx).Request(request).Execute()

Link an account to a member



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
	request := *openapiclient.NewHandlerLinkAccountRequest("Custcode_example") // HandlerLinkAccountRequest | Link Account Request Body

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	resp, r, err := apiClient.AccountAPI.SecureAccountsLinkAccountPost(context.Background()).Request(request).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `AccountAPI.SecureAccountsLinkAccountPost``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `SecureAccountsLinkAccountPost`: map[string]string
	fmt.Fprintf(os.Stdout, "Response from `AccountAPI.SecureAccountsLinkAccountPost`: %v\n", resp)
}
```

### Path Parameters



### Other Parameters

Other parameters are passed through a pointer to a apiSecureAccountsLinkAccountPostRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **request** | [**HandlerLinkAccountRequest**](HandlerLinkAccountRequest.md) | Link Account Request Body | 

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

