# \UserAccountAPI

All URIs are relative to *http://localhost*

Method | HTTP request | Description
------------- | ------------- | -------------
[**InternalV1UserAccountPost**](UserAccountAPI.md#InternalV1UserAccountPost) | **Post** /internal/v1/user-account | Linking user account id with user id for a user account type.
[**InternalV1UserAccountsGet**](UserAccountAPI.md#InternalV1UserAccountsGet) | **Get** /internal/v1/user-accounts | Get user accounts by filters.
[**SecureV1UserAccountsGet**](UserAccountAPI.md#SecureV1UserAccountsGet) | **Get** /secure/v1/user-accounts | Get user account by user id



## InternalV1UserAccountPost

> InternalV1TradingAccountsCustomerCodePost200Response InternalV1UserAccountPost(ctx).UserId(userId).DtoLinkUserAccountRequest(dtoLinkUserAccountRequest).Execute()

Linking user account id with user id for a user account type.



### Example

```go
package main

import (
	"context"
	"fmt"
	"os"
	openapiclient "github.com/pi-financial/user-srv-v2/client"
)

func main() {
	userId := "userId_example" // string | User ID
	dtoLinkUserAccountRequest := *openapiclient.NewDtoLinkUserAccountRequest("UserAccountId_example", openapiclient.domain.UserAccountType("CashWallet")) // DtoLinkUserAccountRequest | Link User Account Request

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	resp, r, err := apiClient.UserAccountAPI.InternalV1UserAccountPost(context.Background()).UserId(userId).DtoLinkUserAccountRequest(dtoLinkUserAccountRequest).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `UserAccountAPI.InternalV1UserAccountPost``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `InternalV1UserAccountPost`: InternalV1TradingAccountsCustomerCodePost200Response
	fmt.Fprintf(os.Stdout, "Response from `UserAccountAPI.InternalV1UserAccountPost`: %v\n", resp)
}
```

### Path Parameters



### Other Parameters

Other parameters are passed through a pointer to a apiInternalV1UserAccountPostRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **userId** | **string** | User ID |
 **dtoLinkUserAccountRequest** | [**DtoLinkUserAccountRequest**](DtoLinkUserAccountRequest.md) | Link User Account Request |

### Return type

[**InternalV1TradingAccountsCustomerCodePost200Response**](InternalV1TradingAccountsCustomerCodePost200Response.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: application/json
- **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)


## InternalV1UserAccountsGet

> InternalV1UserAccountsGet200Response InternalV1UserAccountsGet(ctx).UserId(userId).CitizenId(citizenId).Execute()

Get user accounts by filters.



### Example

```go
package main

import (
	"context"
	"fmt"
	"os"
	openapiclient "github.com/pi-financial/user-srv-v2/client"
)

func main() {
	userId := "userId_example" // string | User ID (optional)
	citizenId := "citizenId_example" // string | Citizen ID (optional)

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	resp, r, err := apiClient.UserAccountAPI.InternalV1UserAccountsGet(context.Background()).UserId(userId).CitizenId(citizenId).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `UserAccountAPI.InternalV1UserAccountsGet``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `InternalV1UserAccountsGet`: InternalV1UserAccountsGet200Response
	fmt.Fprintf(os.Stdout, "Response from `UserAccountAPI.InternalV1UserAccountsGet`: %v\n", resp)
}
```

### Path Parameters



### Other Parameters

Other parameters are passed through a pointer to a apiInternalV1UserAccountsGetRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **userId** | **string** | User ID |
 **citizenId** | **string** | Citizen ID |

### Return type

[**InternalV1UserAccountsGet200Response**](InternalV1UserAccountsGet200Response.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)


## SecureV1UserAccountsGet

> InternalV1UserAccountsGet200Response SecureV1UserAccountsGet(ctx).UserId(userId).Execute()

Get user account by user id



### Example

```go
package main

import (
	"context"
	"fmt"
	"os"
	openapiclient "github.com/pi-financial/user-srv-v2/client"
)

func main() {
	userId := "userId_example" // string | User ID

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	resp, r, err := apiClient.UserAccountAPI.SecureV1UserAccountsGet(context.Background()).UserId(userId).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `UserAccountAPI.SecureV1UserAccountsGet``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `SecureV1UserAccountsGet`: InternalV1UserAccountsGet200Response
	fmt.Fprintf(os.Stdout, "Response from `UserAccountAPI.SecureV1UserAccountsGet`: %v\n", resp)
}
```

### Path Parameters



### Other Parameters

Other parameters are passed through a pointer to a apiSecureV1UserAccountsGetRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **userId** | **string** | User ID |

### Return type

[**InternalV1UserAccountsGet200Response**](InternalV1UserAccountsGet200Response.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)
