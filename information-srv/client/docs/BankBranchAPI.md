# \BankBranchAPI

All URIs are relative to *http://localhost*

Method | HTTP request | Description
------------- | ------------- | -------------
[**InternalBankBranchGet**](BankBranchAPI.md#InternalBankBranchGet) | **Get** /internal/bank-branch | Get All BankBranches
[**SecureBankBranchGet**](BankBranchAPI.md#SecureBankBranchGet) | **Get** /secure/bank-branch | Get All BankBranches



## InternalBankBranchGet

> InternalBankBranchGet200Response InternalBankBranchGet(ctx).BankCode(bankCode).BankBranchCode(bankBranchCode).Execute()

Get All BankBranches



### Example

```go
package main

import (
	"context"
	"fmt"
	"os"
	openapiclient "github.com/pi-financial/information-srv/client"
)

func main() {
	bankCode := "bankCode_example" // string | BankCode (optional)
	bankBranchCode := "bankBranchCode_example" // string | BankBranchCode (optional)

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	resp, r, err := apiClient.BankBranchAPI.InternalBankBranchGet(context.Background()).BankCode(bankCode).BankBranchCode(bankBranchCode).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `BankBranchAPI.InternalBankBranchGet``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `InternalBankBranchGet`: InternalBankBranchGet200Response
	fmt.Fprintf(os.Stdout, "Response from `BankBranchAPI.InternalBankBranchGet`: %v\n", resp)
}
```

### Path Parameters



### Other Parameters

Other parameters are passed through a pointer to a apiInternalBankBranchGetRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **bankCode** | **string** | BankCode | 
 **bankBranchCode** | **string** | BankBranchCode | 

### Return type

[**InternalBankBranchGet200Response**](InternalBankBranchGet200Response.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)


## SecureBankBranchGet

> InternalBankBranchGet200Response SecureBankBranchGet(ctx).BankCode(bankCode).BankBranchCode(bankBranchCode).Execute()

Get All BankBranches



### Example

```go
package main

import (
	"context"
	"fmt"
	"os"
	openapiclient "github.com/pi-financial/information-srv/client"
)

func main() {
	bankCode := "bankCode_example" // string | BankCode (optional)
	bankBranchCode := "bankBranchCode_example" // string | BankBranchCode (optional)

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	resp, r, err := apiClient.BankBranchAPI.SecureBankBranchGet(context.Background()).BankCode(bankCode).BankBranchCode(bankBranchCode).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `BankBranchAPI.SecureBankBranchGet``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `SecureBankBranchGet`: InternalBankBranchGet200Response
	fmt.Fprintf(os.Stdout, "Response from `BankBranchAPI.SecureBankBranchGet`: %v\n", resp)
}
```

### Path Parameters



### Other Parameters

Other parameters are passed through a pointer to a apiSecureBankBranchGetRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **bankCode** | **string** | BankCode | 
 **bankBranchCode** | **string** | BankBranchCode | 

### Return type

[**InternalBankBranchGet200Response**](InternalBankBranchGet200Response.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)

