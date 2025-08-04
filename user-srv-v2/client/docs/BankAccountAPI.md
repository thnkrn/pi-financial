# \BankAccountAPI

All URIs are relative to *http://localhost*

Method | HTTP request | Description
------------- | ------------- | -------------
[**InternalV1BankAccountDepositWithdrawGet**](BankAccountAPI.md#InternalV1BankAccountDepositWithdrawGet) | **Get** /internal/v1/bank-account/deposit-withdraw | Get bank account for deposit/withdraw (currently used by the app, but will be deprecated soon in favor of /internal/v2/bank-account/deposit-withdraw)
[**InternalV1BankAccountPost**](BankAccountAPI.md#InternalV1BankAccountPost) | **Post** /internal/v1/bank-account | Create a user&#39;s bank account
[**InternalV1BankAccountsGet**](BankAccountAPI.md#InternalV1BankAccountsGet) | **Get** /internal/v1/bank-accounts | Get user&#39;s bank accounts
[**InternalV2BankAccountDepositWithdrawGet**](BankAccountAPI.md#InternalV2BankAccountDepositWithdrawGet) | **Get** /internal/v2/bank-account/deposit-withdraw | Get bank accounts for deposit/withdraw



## InternalV1BankAccountDepositWithdrawGet

> InternalV1BankAccountDepositWithdrawGet200Response InternalV1BankAccountDepositWithdrawGet(ctx).AccountId(accountId).Purpose(purpose).Product(product).Execute()

Get bank account for deposit/withdraw (currently used by the app, but will be deprecated soon in favor of /internal/v2/bank-account/deposit-withdraw)



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
	accountId := "accountId_example" // string | Account ID
	purpose := "purpose_example" // string | Purpose (deposit/withdrawal)
	product := "product_example" // string | Product (optional)

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	resp, r, err := apiClient.BankAccountAPI.InternalV1BankAccountDepositWithdrawGet(context.Background()).AccountId(accountId).Purpose(purpose).Product(product).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `BankAccountAPI.InternalV1BankAccountDepositWithdrawGet``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `InternalV1BankAccountDepositWithdrawGet`: InternalV1BankAccountDepositWithdrawGet200Response
	fmt.Fprintf(os.Stdout, "Response from `BankAccountAPI.InternalV1BankAccountDepositWithdrawGet`: %v\n", resp)
}
```

### Path Parameters



### Other Parameters

Other parameters are passed through a pointer to a apiInternalV1BankAccountDepositWithdrawGetRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **accountId** | **string** | Account ID |
 **purpose** | **string** | Purpose (deposit/withdrawal) |
 **product** | **string** | Product |

### Return type

[**InternalV1BankAccountDepositWithdrawGet200Response**](InternalV1BankAccountDepositWithdrawGet200Response.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)


## InternalV1BankAccountPost

> ResultResponseSuccess InternalV1BankAccountPost(ctx).UserId(userId).DtoBankAccountRequest(dtoBankAccountRequest).Execute()

Create a user's bank account



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
	dtoBankAccountRequest := *openapiclient.NewDtoBankAccountRequest("AccountName_example", "AccountNo_example", "BankCode_example", "BranchCode_example", "Status_example") // DtoBankAccountRequest | BankAccountRequest request

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	resp, r, err := apiClient.BankAccountAPI.InternalV1BankAccountPost(context.Background()).UserId(userId).DtoBankAccountRequest(dtoBankAccountRequest).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `BankAccountAPI.InternalV1BankAccountPost``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `InternalV1BankAccountPost`: ResultResponseSuccess
	fmt.Fprintf(os.Stdout, "Response from `BankAccountAPI.InternalV1BankAccountPost`: %v\n", resp)
}
```

### Path Parameters



### Other Parameters

Other parameters are passed through a pointer to a apiInternalV1BankAccountPostRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **userId** | **string** | User ID |
 **dtoBankAccountRequest** | [**DtoBankAccountRequest**](DtoBankAccountRequest.md) | BankAccountRequest request |

### Return type

[**ResultResponseSuccess**](ResultResponseSuccess.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: application/json
- **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)


## InternalV1BankAccountsGet

> InternalV1BankAccountsGet200Response InternalV1BankAccountsGet(ctx).UserId(userId).Execute()

Get user's bank accounts



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
	resp, r, err := apiClient.BankAccountAPI.InternalV1BankAccountsGet(context.Background()).UserId(userId).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `BankAccountAPI.InternalV1BankAccountsGet``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `InternalV1BankAccountsGet`: InternalV1BankAccountsGet200Response
	fmt.Fprintf(os.Stdout, "Response from `BankAccountAPI.InternalV1BankAccountsGet`: %v\n", resp)
}
```

### Path Parameters



### Other Parameters

Other parameters are passed through a pointer to a apiInternalV1BankAccountsGetRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **userId** | **string** | User ID |

### Return type

[**InternalV1BankAccountsGet200Response**](InternalV1BankAccountsGet200Response.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)


## InternalV2BankAccountDepositWithdrawGet

> InternalV2BankAccountDepositWithdrawGet200Response InternalV2BankAccountDepositWithdrawGet(ctx).AccountId(accountId).Purpose(purpose).Product(product).Execute()

Get bank accounts for deposit/withdraw



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
	accountId := "accountId_example" // string | Account ID. Must be either customer code (7 digits) or cash wallet id (10 digits).
	purpose := "purpose_example" // string | Purpose (deposit/withdrawal)
	product := "product_example" // string | Product. Optional if accountId is cash wallet id. (optional)

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	resp, r, err := apiClient.BankAccountAPI.InternalV2BankAccountDepositWithdrawGet(context.Background()).AccountId(accountId).Purpose(purpose).Product(product).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `BankAccountAPI.InternalV2BankAccountDepositWithdrawGet``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `InternalV2BankAccountDepositWithdrawGet`: InternalV2BankAccountDepositWithdrawGet200Response
	fmt.Fprintf(os.Stdout, "Response from `BankAccountAPI.InternalV2BankAccountDepositWithdrawGet`: %v\n", resp)
}
```

### Path Parameters



### Other Parameters

Other parameters are passed through a pointer to a apiInternalV2BankAccountDepositWithdrawGetRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **accountId** | **string** | Account ID. Must be either customer code (7 digits) or cash wallet id (10 digits). |
 **purpose** | **string** | Purpose (deposit/withdrawal) |
 **product** | **string** | Product. Optional if accountId is cash wallet id. |

### Return type

[**InternalV2BankAccountDepositWithdrawGet200Response**](InternalV2BankAccountDepositWithdrawGet200Response.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)
