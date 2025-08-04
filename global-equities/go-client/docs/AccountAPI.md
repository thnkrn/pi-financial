# \AccountAPI

All URIs are relative to *http://localhost*

Method | HTTP request | Description
------------- | ------------- | -------------
[**InternalAccountsOverviewGet**](AccountAPI.md#InternalAccountsOverviewGet) | **Get** /internal/accounts/overview | 
[**SecureAccountsCorporateActionsGet**](AccountAPI.md#SecureAccountsCorporateActionsGet) | **Get** /secure/accounts/corporate-actions | 
[**SecureAccountsOverviewGet**](AccountAPI.md#SecureAccountsOverviewGet) | **Get** /secure/accounts/overview | 
[**SecureAccountsSummaryGet**](AccountAPI.md#SecureAccountsSummaryGet) | **Get** /secure/accounts/summary | 



## InternalAccountsOverviewGet

> MultiAccountOverviewResponseApiResponse InternalAccountsOverviewGet(ctx).UserId(userId).Currency(currency).Execute()



### Example

```go
package main

import (
	"context"
	"fmt"
	"os"
	openapiclient "github.com/pi-financial/global-equities/go-client"
)

func main() {
	userId := "userId_example" // string | 
	currency := openapiclient.Currency("AED") // Currency |  (optional)

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	resp, r, err := apiClient.AccountAPI.InternalAccountsOverviewGet(context.Background()).UserId(userId).Currency(currency).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `AccountAPI.InternalAccountsOverviewGet``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `InternalAccountsOverviewGet`: MultiAccountOverviewResponseApiResponse
	fmt.Fprintf(os.Stdout, "Response from `AccountAPI.InternalAccountsOverviewGet`: %v\n", resp)
}
```

### Path Parameters



### Other Parameters

Other parameters are passed through a pointer to a apiInternalAccountsOverviewGetRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **userId** | **string** |  | 
 **currency** | [**Currency**](Currency.md) |  | 

### Return type

[**MultiAccountOverviewResponseApiResponse**](MultiAccountOverviewResponseApiResponse.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: text/plain, application/json, text/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)


## SecureAccountsCorporateActionsGet

> CorporateActionResponseArrayApiResponse SecureAccountsCorporateActionsGet(ctx).UserId(userId).AccountId(accountId).From(from).To(to).Execute()



### Example

```go
package main

import (
	"context"
	"fmt"
	"os"
    "time"
	openapiclient "github.com/pi-financial/global-equities/go-client"
)

func main() {
	userId := "userId_example" // string | 
	accountId := "accountId_example" // string | 
	from := time.Now() // time.Time | 
	to := time.Now() // time.Time | 

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	resp, r, err := apiClient.AccountAPI.SecureAccountsCorporateActionsGet(context.Background()).UserId(userId).AccountId(accountId).From(from).To(to).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `AccountAPI.SecureAccountsCorporateActionsGet``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `SecureAccountsCorporateActionsGet`: CorporateActionResponseArrayApiResponse
	fmt.Fprintf(os.Stdout, "Response from `AccountAPI.SecureAccountsCorporateActionsGet`: %v\n", resp)
}
```

### Path Parameters



### Other Parameters

Other parameters are passed through a pointer to a apiSecureAccountsCorporateActionsGetRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **userId** | **string** |  | 
 **accountId** | **string** |  | 
 **from** | **time.Time** |  | 
 **to** | **time.Time** |  | 

### Return type

[**CorporateActionResponseArrayApiResponse**](CorporateActionResponseArrayApiResponse.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: text/plain, application/json, text/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)


## SecureAccountsOverviewGet

> MultiAccountOverviewResponseApiResponse SecureAccountsOverviewGet(ctx).UserId(userId).Currency(currency).Execute()



### Example

```go
package main

import (
	"context"
	"fmt"
	"os"
	openapiclient "github.com/pi-financial/global-equities/go-client"
)

func main() {
	userId := "userId_example" // string | 
	currency := openapiclient.Currency("AED") // Currency |  (optional)

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	resp, r, err := apiClient.AccountAPI.SecureAccountsOverviewGet(context.Background()).UserId(userId).Currency(currency).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `AccountAPI.SecureAccountsOverviewGet``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `SecureAccountsOverviewGet`: MultiAccountOverviewResponseApiResponse
	fmt.Fprintf(os.Stdout, "Response from `AccountAPI.SecureAccountsOverviewGet`: %v\n", resp)
}
```

### Path Parameters



### Other Parameters

Other parameters are passed through a pointer to a apiSecureAccountsOverviewGetRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **userId** | **string** |  | 
 **currency** | [**Currency**](Currency.md) |  | 

### Return type

[**MultiAccountOverviewResponseApiResponse**](MultiAccountOverviewResponseApiResponse.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: text/plain, application/json, text/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)


## SecureAccountsSummaryGet

> AccountSummaryResponseApiResponse SecureAccountsSummaryGet(ctx).UserId(userId).AccountId(accountId).Currencies(currencies).Sid(sid).Execute()



### Example

```go
package main

import (
	"context"
	"fmt"
	"os"
	openapiclient "github.com/pi-financial/global-equities/go-client"
)

func main() {
	userId := "userId_example" // string | 
	accountId := "accountId_example" // string | 
	currencies := []openapiclient.Currency{openapiclient.Currency("AED")} // []Currency | 
	sid := "sid_example" // string |  (optional)

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	resp, r, err := apiClient.AccountAPI.SecureAccountsSummaryGet(context.Background()).UserId(userId).AccountId(accountId).Currencies(currencies).Sid(sid).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `AccountAPI.SecureAccountsSummaryGet``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `SecureAccountsSummaryGet`: AccountSummaryResponseApiResponse
	fmt.Fprintf(os.Stdout, "Response from `AccountAPI.SecureAccountsSummaryGet`: %v\n", resp)
}
```

### Path Parameters



### Other Parameters

Other parameters are passed through a pointer to a apiSecureAccountsSummaryGetRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **userId** | **string** |  | 
 **accountId** | **string** |  | 
 **currencies** | [**[]Currency**](Currency.md) |  | 
 **sid** | **string** |  | 

### Return type

[**AccountSummaryResponseApiResponse**](AccountSummaryResponseApiResponse.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: text/plain, application/json, text/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)

