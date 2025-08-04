# \ProductAPI

All URIs are relative to *http://localhost*

Method | HTTP request | Description
------------- | ------------- | -------------
[**InternalProductGet**](ProductAPI.md#InternalProductGet) | **Get** /internal/product | Get All Products
[**SecureProductGet**](ProductAPI.md#SecureProductGet) | **Get** /secure/product | Get All Products



## InternalProductGet

> InternalProductGet200Response InternalProductGet(ctx).Id(id).Name(name).AccountTypeCode(accountTypeCode).AccountType(accountType).ExchangeMarketId(exchangeMarketId).Suffix(suffix).TransactionType(transactionType).Execute()

Get All Products



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
	name := "name_example" // string | Name (optional)
	accountTypeCode := "accountTypeCode_example" // string | AccountTypeCode (optional)
	accountType := "accountType_example" // string | AccountType (optional)
	exchangeMarketId := "exchangeMarketId_example" // string | ExchangeMarketId (optional)
	suffix := "suffix_example" // string | Suffix (optional)
	transactionType := "transactionType_example" // string | TransactionType (optional)

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	resp, r, err := apiClient.ProductAPI.InternalProductGet(context.Background()).Id(id).Name(name).AccountTypeCode(accountTypeCode).AccountType(accountType).ExchangeMarketId(exchangeMarketId).Suffix(suffix).TransactionType(transactionType).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `ProductAPI.InternalProductGet``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `InternalProductGet`: InternalProductGet200Response
	fmt.Fprintf(os.Stdout, "Response from `ProductAPI.InternalProductGet`: %v\n", resp)
}
```

### Path Parameters



### Other Parameters

Other parameters are passed through a pointer to a apiInternalProductGetRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **id** | **string** | Id | 
 **name** | **string** | Name | 
 **accountTypeCode** | **string** | AccountTypeCode | 
 **accountType** | **string** | AccountType | 
 **exchangeMarketId** | **string** | ExchangeMarketId | 
 **suffix** | **string** | Suffix | 
 **transactionType** | **string** | TransactionType | 

### Return type

[**InternalProductGet200Response**](InternalProductGet200Response.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)


## SecureProductGet

> InternalProductGet200Response SecureProductGet(ctx).Id(id).Name(name).AccountTypeCode(accountTypeCode).AccountType(accountType).ExchangeMarketId(exchangeMarketId).Suffix(suffix).TransactionType(transactionType).Execute()

Get All Products



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
	name := "name_example" // string | Name (optional)
	accountTypeCode := "accountTypeCode_example" // string | AccountTypeCode (optional)
	accountType := "accountType_example" // string | AccountType (optional)
	exchangeMarketId := "exchangeMarketId_example" // string | ExchangeMarketId (optional)
	suffix := "suffix_example" // string | Suffix (optional)
	transactionType := "transactionType_example" // string | TransactionType (optional)

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	resp, r, err := apiClient.ProductAPI.SecureProductGet(context.Background()).Id(id).Name(name).AccountTypeCode(accountTypeCode).AccountType(accountType).ExchangeMarketId(exchangeMarketId).Suffix(suffix).TransactionType(transactionType).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `ProductAPI.SecureProductGet``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `SecureProductGet`: InternalProductGet200Response
	fmt.Fprintf(os.Stdout, "Response from `ProductAPI.SecureProductGet`: %v\n", resp)
}
```

### Path Parameters



### Other Parameters

Other parameters are passed through a pointer to a apiSecureProductGetRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **id** | **string** | Id | 
 **name** | **string** | Name | 
 **accountTypeCode** | **string** | AccountTypeCode | 
 **accountType** | **string** | AccountType | 
 **exchangeMarketId** | **string** | ExchangeMarketId | 
 **suffix** | **string** | Suffix | 
 **transactionType** | **string** | TransactionType | 

### Return type

[**InternalProductGet200Response**](InternalProductGet200Response.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)

