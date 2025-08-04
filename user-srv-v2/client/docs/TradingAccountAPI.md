# \TradingAccountAPI

All URIs are relative to *http://localhost*

Method | HTTP request | Description
------------- | ------------- | -------------
[**InternalV1TradingAccountsCustomerCodePost**](TradingAccountAPI.md#InternalV1TradingAccountsCustomerCodePost) | **Post** /internal/v1/trading-accounts/{customerCode} | Create trading account
[**InternalV1TradingAccountsGet**](TradingAccountAPI.md#InternalV1TradingAccountsGet) | **Get** /internal/v1/trading-accounts | Get user&#39;s trading accounts
[**InternalV1TradingAccountsMarketingInfosGet**](TradingAccountAPI.md#InternalV1TradingAccountsMarketingInfosGet) | **Get** /internal/v1/trading-accounts/marketing-infos | Get customer codes&#39; trading accounts with marketing information
[**SecureV1TradingAccountsDepositWithdrawGet**](TradingAccountAPI.md#SecureV1TradingAccountsDepositWithdrawGet) | **Get** /secure/v1/trading-accounts/deposit-withdraw | Get user&#39;s deposit/withdrawal trading accounts
[**SecureV1TradingAccountsGet**](TradingAccountAPI.md#SecureV1TradingAccountsGet) | **Get** /secure/v1/trading-accounts | Get user&#39;s trading accounts



## InternalV1TradingAccountsCustomerCodePost

> InternalV1TradingAccountsCustomerCodePost200Response InternalV1TradingAccountsCustomerCodePost(ctx, customerCode).DtoCreateTradingAccountRequest(dtoCreateTradingAccountRequest).Execute()

Create trading account



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
	customerCode := "customerCode_example" // string | Customer code
	dtoCreateTradingAccountRequest := []openapiclient.DtoCreateTradingAccountRequest{*openapiclient.NewDtoCreateTradingAccountRequest()} // []DtoCreateTradingAccountRequest | Create trading account request

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	resp, r, err := apiClient.TradingAccountAPI.InternalV1TradingAccountsCustomerCodePost(context.Background(), customerCode).DtoCreateTradingAccountRequest(dtoCreateTradingAccountRequest).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `TradingAccountAPI.InternalV1TradingAccountsCustomerCodePost``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `InternalV1TradingAccountsCustomerCodePost`: InternalV1TradingAccountsCustomerCodePost200Response
	fmt.Fprintf(os.Stdout, "Response from `TradingAccountAPI.InternalV1TradingAccountsCustomerCodePost`: %v\n", resp)
}
```

### Path Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
**ctx** | **context.Context** | context for authentication, logging, cancellation, deadlines, tracing, etc.
**customerCode** | **string** | Customer code |

### Other Parameters

Other parameters are passed through a pointer to a apiInternalV1TradingAccountsCustomerCodePostRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------

 **dtoCreateTradingAccountRequest** | [**[]DtoCreateTradingAccountRequest**](DtoCreateTradingAccountRequest.md) | Create trading account request |

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


## InternalV1TradingAccountsGet

> InternalV1TradingAccountsGet200Response InternalV1TradingAccountsGet(ctx).UserId(userId).Status(status).Execute()

Get user's trading accounts



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
	status := "status_example" // string | N for normal, C for closed (optional)

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	resp, r, err := apiClient.TradingAccountAPI.InternalV1TradingAccountsGet(context.Background()).UserId(userId).Status(status).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `TradingAccountAPI.InternalV1TradingAccountsGet``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `InternalV1TradingAccountsGet`: InternalV1TradingAccountsGet200Response
	fmt.Fprintf(os.Stdout, "Response from `TradingAccountAPI.InternalV1TradingAccountsGet`: %v\n", resp)
}
```

### Path Parameters



### Other Parameters

Other parameters are passed through a pointer to a apiInternalV1TradingAccountsGetRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **userId** | **string** | User ID |
 **status** | **string** | N for normal, C for closed |

### Return type

[**InternalV1TradingAccountsGet200Response**](InternalV1TradingAccountsGet200Response.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)


## InternalV1TradingAccountsMarketingInfosGet

> InternalV1TradingAccountsMarketingInfosGet200Response InternalV1TradingAccountsMarketingInfosGet(ctx).CustomerCodes(customerCodes).Execute()

Get customer codes' trading accounts with marketing information



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
	customerCodes := "customerCodes_example" // string | Customer Codes

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	resp, r, err := apiClient.TradingAccountAPI.InternalV1TradingAccountsMarketingInfosGet(context.Background()).CustomerCodes(customerCodes).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `TradingAccountAPI.InternalV1TradingAccountsMarketingInfosGet``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `InternalV1TradingAccountsMarketingInfosGet`: InternalV1TradingAccountsMarketingInfosGet200Response
	fmt.Fprintf(os.Stdout, "Response from `TradingAccountAPI.InternalV1TradingAccountsMarketingInfosGet`: %v\n", resp)
}
```

### Path Parameters



### Other Parameters

Other parameters are passed through a pointer to a apiInternalV1TradingAccountsMarketingInfosGetRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **customerCodes** | **string** | Customer Codes |

### Return type

[**InternalV1TradingAccountsMarketingInfosGet200Response**](InternalV1TradingAccountsMarketingInfosGet200Response.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)


## SecureV1TradingAccountsDepositWithdrawGet

> SecureV1TradingAccountsDepositWithdrawGet200Response SecureV1TradingAccountsDepositWithdrawGet(ctx).UserId(userId).Execute()

Get user's deposit/withdrawal trading accounts



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
	resp, r, err := apiClient.TradingAccountAPI.SecureV1TradingAccountsDepositWithdrawGet(context.Background()).UserId(userId).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `TradingAccountAPI.SecureV1TradingAccountsDepositWithdrawGet``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `SecureV1TradingAccountsDepositWithdrawGet`: SecureV1TradingAccountsDepositWithdrawGet200Response
	fmt.Fprintf(os.Stdout, "Response from `TradingAccountAPI.SecureV1TradingAccountsDepositWithdrawGet`: %v\n", resp)
}
```

### Path Parameters



### Other Parameters

Other parameters are passed through a pointer to a apiSecureV1TradingAccountsDepositWithdrawGetRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **userId** | **string** | User ID |

### Return type

[**SecureV1TradingAccountsDepositWithdrawGet200Response**](SecureV1TradingAccountsDepositWithdrawGet200Response.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)


## SecureV1TradingAccountsGet

> InternalV1TradingAccountsGet200Response SecureV1TradingAccountsGet(ctx).UserId(userId).Status(status).Execute()

Get user's trading accounts



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
	status := "status_example" // string | N for normal, C for closed (optional)

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	resp, r, err := apiClient.TradingAccountAPI.SecureV1TradingAccountsGet(context.Background()).UserId(userId).Status(status).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `TradingAccountAPI.SecureV1TradingAccountsGet``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `SecureV1TradingAccountsGet`: InternalV1TradingAccountsGet200Response
	fmt.Fprintf(os.Stdout, "Response from `TradingAccountAPI.SecureV1TradingAccountsGet`: %v\n", resp)
}
```

### Path Parameters



### Other Parameters

Other parameters are passed through a pointer to a apiSecureV1TradingAccountsGetRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **userId** | **string** | User ID |
 **status** | **string** | N for normal, C for closed |

### Return type

[**InternalV1TradingAccountsGet200Response**](InternalV1TradingAccountsGet200Response.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)
