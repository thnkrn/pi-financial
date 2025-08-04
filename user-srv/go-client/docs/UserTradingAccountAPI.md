# \UserTradingAccountAPI

All URIs are relative to *http://localhost*

Method | HTTP request | Description
------------- | ------------- | -------------
[**GetTradingAccountsByCustomerCode**](UserTradingAccountAPI.md#GetTradingAccountsByCustomerCode) | **Get** /internal/user/trading-accounts | 
[**GetUserTradingAccountInfoByCustomerId**](UserTradingAccountAPI.md#GetUserTradingAccountInfoByCustomerId) | **Get** /secure/trading-accounts/v2 | 
[**GetUserTradingAccountInfoByUserId**](UserTradingAccountAPI.md#GetUserTradingAccountInfoByUserId) | **Get** /internal/trading-accounts/v2 | 



## GetTradingAccountsByCustomerCode

> SystemStringIEnumerableApiResponse GetTradingAccountsByCustomerCode(ctx).CustCode(custCode).Execute()



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
	custCode := "custCode_example" // string | 

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	resp, r, err := apiClient.UserTradingAccountAPI.GetTradingAccountsByCustomerCode(context.Background()).CustCode(custCode).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `UserTradingAccountAPI.GetTradingAccountsByCustomerCode``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `GetTradingAccountsByCustomerCode`: SystemStringIEnumerableApiResponse
	fmt.Fprintf(os.Stdout, "Response from `UserTradingAccountAPI.GetTradingAccountsByCustomerCode`: %v\n", resp)
}
```

### Path Parameters



### Other Parameters

Other parameters are passed through a pointer to a apiGetTradingAccountsByCustomerCodeRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **custCode** | **string** |  | 

### Return type

[**SystemStringIEnumerableApiResponse**](SystemStringIEnumerableApiResponse.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: text/plain, application/json, text/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)


## GetUserTradingAccountInfoByCustomerId

> PiUserApplicationModelsUserTradingAccountInfoApiResponse GetUserTradingAccountInfoByCustomerId(ctx).UserId(userId).CustomerCode(customerCode).Execute()



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
	userId := "38400000-8cf0-11bd-b23e-10b96e4ef00d" // string |  (optional)
	customerCode := "customerCode_example" // string |  (optional)

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	resp, r, err := apiClient.UserTradingAccountAPI.GetUserTradingAccountInfoByCustomerId(context.Background()).UserId(userId).CustomerCode(customerCode).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `UserTradingAccountAPI.GetUserTradingAccountInfoByCustomerId``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `GetUserTradingAccountInfoByCustomerId`: PiUserApplicationModelsUserTradingAccountInfoApiResponse
	fmt.Fprintf(os.Stdout, "Response from `UserTradingAccountAPI.GetUserTradingAccountInfoByCustomerId`: %v\n", resp)
}
```

### Path Parameters



### Other Parameters

Other parameters are passed through a pointer to a apiGetUserTradingAccountInfoByCustomerIdRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **userId** | **string** |  | 
 **customerCode** | **string** |  | 

### Return type

[**PiUserApplicationModelsUserTradingAccountInfoApiResponse**](PiUserApplicationModelsUserTradingAccountInfoApiResponse.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: text/plain, application/json, text/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)


## GetUserTradingAccountInfoByUserId

> PiUserApplicationModelsUserTradingAccountInfoApiResponse GetUserTradingAccountInfoByUserId(ctx).UserId(userId).CustomerCode(customerCode).Execute()



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
	userId := "38400000-8cf0-11bd-b23e-10b96e4ef00d" // string |  (optional)
	customerCode := "customerCode_example" // string |  (optional)

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	resp, r, err := apiClient.UserTradingAccountAPI.GetUserTradingAccountInfoByUserId(context.Background()).UserId(userId).CustomerCode(customerCode).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `UserTradingAccountAPI.GetUserTradingAccountInfoByUserId``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `GetUserTradingAccountInfoByUserId`: PiUserApplicationModelsUserTradingAccountInfoApiResponse
	fmt.Fprintf(os.Stdout, "Response from `UserTradingAccountAPI.GetUserTradingAccountInfoByUserId`: %v\n", resp)
}
```

### Path Parameters



### Other Parameters

Other parameters are passed through a pointer to a apiGetUserTradingAccountInfoByUserIdRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **userId** | **string** |  | 
 **customerCode** | **string** |  | 

### Return type

[**PiUserApplicationModelsUserTradingAccountInfoApiResponse**](PiUserApplicationModelsUserTradingAccountInfoApiResponse.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: text/plain, application/json, text/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)

