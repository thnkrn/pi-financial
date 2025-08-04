# \UserBankAccountAPI

All URIs are relative to *http://localhost*

Method | HTTP request | Description
------------- | ------------- | -------------
[**DeleteBankAccountByUserId**](UserBankAccountAPI.md#DeleteBankAccountByUserId) | **Delete** /internal/bank-account/{userId} | 
[**GetBankAccountByUserId**](UserBankAccountAPI.md#GetBankAccountByUserId) | **Get** /internal/bank-account/{userId} | 
[**UpdateBankAccount**](UserBankAccountAPI.md#UpdateBankAccount) | **Post** /secure/bank-account | 
[**UploadBankAccountDocument**](UserBankAccountAPI.md#UploadBankAccountDocument) | **Post** /secure/bank-account/upload-document | 



## DeleteBankAccountByUserId

> DeleteBankAccountByUserId(ctx, userId).Execute()



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

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	r, err := apiClient.UserBankAccountAPI.DeleteBankAccountByUserId(context.Background(), userId).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `UserBankAccountAPI.DeleteBankAccountByUserId``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
}
```

### Path Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
**ctx** | **context.Context** | context for authentication, logging, cancellation, deadlines, tracing, etc.
**userId** | **string** |  | 

### Other Parameters

Other parameters are passed through a pointer to a apiDeleteBankAccountByUserIdRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------


### Return type

 (empty response body)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: text/plain, application/json, text/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)


## GetBankAccountByUserId

> PiUserApplicationModelsBankAccountBankAccountDtoApiResponse GetBankAccountByUserId(ctx, userId).Execute()



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

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	resp, r, err := apiClient.UserBankAccountAPI.GetBankAccountByUserId(context.Background(), userId).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `UserBankAccountAPI.GetBankAccountByUserId``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `GetBankAccountByUserId`: PiUserApplicationModelsBankAccountBankAccountDtoApiResponse
	fmt.Fprintf(os.Stdout, "Response from `UserBankAccountAPI.GetBankAccountByUserId`: %v\n", resp)
}
```

### Path Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
**ctx** | **context.Context** | context for authentication, logging, cancellation, deadlines, tracing, etc.
**userId** | **string** |  | 

### Other Parameters

Other parameters are passed through a pointer to a apiGetBankAccountByUserIdRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------


### Return type

[**PiUserApplicationModelsBankAccountBankAccountDtoApiResponse**](PiUserApplicationModelsBankAccountBankAccountDtoApiResponse.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: text/plain, application/json, text/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)


## UpdateBankAccount

> UpdateBankAccount(ctx).UserId(userId).BankAccountNo(bankAccountNo).BankAccountName(bankAccountName).BankCode(bankCode).BankBranchCode(bankBranchCode).Bookbank(bookbank).Execute()



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
	bankAccountNo := "bankAccountNo_example" // string |  (optional)
	bankAccountName := "bankAccountName_example" // string |  (optional)
	bankCode := "bankCode_example" // string |  (optional)
	bankBranchCode := "bankBranchCode_example" // string |  (optional)
	bookbank := os.NewFile(1234, "some_file") // *os.File |  (optional)

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	r, err := apiClient.UserBankAccountAPI.UpdateBankAccount(context.Background()).UserId(userId).BankAccountNo(bankAccountNo).BankAccountName(bankAccountName).BankCode(bankCode).BankBranchCode(bankBranchCode).Bookbank(bookbank).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `UserBankAccountAPI.UpdateBankAccount``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
}
```

### Path Parameters



### Other Parameters

Other parameters are passed through a pointer to a apiUpdateBankAccountRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **userId** | **string** |  | 
 **bankAccountNo** | **string** |  | 
 **bankAccountName** | **string** |  | 
 **bankCode** | **string** |  | 
 **bankBranchCode** | **string** |  | 
 **bookbank** | ***os.File** |  | 

### Return type

 (empty response body)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: multipart/form-data
- **Accept**: Not defined

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)


## UploadBankAccountDocument

> UploadBankAccountDocument(ctx).UserId(userId).Statements(statements).Execute()



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
	statements := []*os.File{"TODO"} // []*os.File |  (optional)

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	r, err := apiClient.UserBankAccountAPI.UploadBankAccountDocument(context.Background()).UserId(userId).Statements(statements).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `UserBankAccountAPI.UploadBankAccountDocument``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
}
```

### Path Parameters



### Other Parameters

Other parameters are passed through a pointer to a apiUploadBankAccountDocumentRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **userId** | **string** |  | 
 **statements** | **[]*os.File** |  | 

### Return type

 (empty response body)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: multipart/form-data
- **Accept**: Not defined

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)

