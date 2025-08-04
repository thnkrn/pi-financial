# \BankAPI

All URIs are relative to *http://localhost*

Method | HTTP request | Description
------------- | ------------- | -------------
[**InternalBankGet**](BankAPI.md#InternalBankGet) | **Get** /internal/bank | Get All Banks
[**SecureBankGet**](BankAPI.md#SecureBankGet) | **Get** /secure/bank | Get All Banks



## InternalBankGet

> InternalBankGet200Response InternalBankGet(ctx).Id(id).ShortName(shortName).Code(code).Execute()

Get All Banks



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
	id := "id_example" // string | Id (optional)
	shortName := "shortName_example" // string | ShortName (optional)
	code := "code_example" // string | Code (optional)

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	resp, r, err := apiClient.BankAPI.InternalBankGet(context.Background()).Id(id).ShortName(shortName).Code(code).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `BankAPI.InternalBankGet``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `InternalBankGet`: InternalBankGet200Response
	fmt.Fprintf(os.Stdout, "Response from `BankAPI.InternalBankGet`: %v\n", resp)
}
```

### Path Parameters



### Other Parameters

Other parameters are passed through a pointer to a apiInternalBankGetRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **id** | **string** | Id | 
 **shortName** | **string** | ShortName | 
 **code** | **string** | Code | 

### Return type

[**InternalBankGet200Response**](InternalBankGet200Response.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)


## SecureBankGet

> InternalBankGet200Response SecureBankGet(ctx).Id(id).ShortName(shortName).Code(code).Execute()

Get All Banks



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
	id := "id_example" // string | Id (optional)
	shortName := "shortName_example" // string | ShortName (optional)
	code := "code_example" // string | Code (optional)

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	resp, r, err := apiClient.BankAPI.SecureBankGet(context.Background()).Id(id).ShortName(shortName).Code(code).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `BankAPI.SecureBankGet``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `SecureBankGet`: InternalBankGet200Response
	fmt.Fprintf(os.Stdout, "Response from `BankAPI.SecureBankGet`: %v\n", resp)
}
```

### Path Parameters



### Other Parameters

Other parameters are passed through a pointer to a apiSecureBankGetRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **id** | **string** | Id | 
 **shortName** | **string** | ShortName | 
 **code** | **string** | Code | 

### Return type

[**InternalBankGet200Response**](InternalBankGet200Response.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)

