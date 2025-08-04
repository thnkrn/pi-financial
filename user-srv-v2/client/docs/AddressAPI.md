# \AddressAPI

All URIs are relative to *http://localhost*

Method | HTTP request | Description
------------- | ------------- | -------------
[**InternalV1AddressGet**](AddressAPI.md#InternalV1AddressGet) | **Get** /internal/v1/address | Get address
[**InternalV1AddressPost**](AddressAPI.md#InternalV1AddressPost) | **Post** /internal/v1/address | Upsert address



## InternalV1AddressGet

> InternalV1AddressGet200Response InternalV1AddressGet(ctx).UserId(userId).Execute()

Get address



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
	resp, r, err := apiClient.AddressAPI.InternalV1AddressGet(context.Background()).UserId(userId).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `AddressAPI.InternalV1AddressGet``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `InternalV1AddressGet`: InternalV1AddressGet200Response
	fmt.Fprintf(os.Stdout, "Response from `AddressAPI.InternalV1AddressGet`: %v\n", resp)
}
```

### Path Parameters



### Other Parameters

Other parameters are passed through a pointer to a apiInternalV1AddressGetRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **userId** | **string** | User ID |

### Return type

[**InternalV1AddressGet200Response**](InternalV1AddressGet200Response.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)


## InternalV1AddressPost

> ResultResponseSuccess InternalV1AddressPost(ctx).UserId(userId).DtoAddress(dtoAddress).Execute()

Upsert address



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
	dtoAddress := *openapiclient.NewDtoAddress() // DtoAddress | Address request

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	resp, r, err := apiClient.AddressAPI.InternalV1AddressPost(context.Background()).UserId(userId).DtoAddress(dtoAddress).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `AddressAPI.InternalV1AddressPost``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `InternalV1AddressPost`: ResultResponseSuccess
	fmt.Fprintf(os.Stdout, "Response from `AddressAPI.InternalV1AddressPost`: %v\n", resp)
}
```

### Path Parameters



### Other Parameters

Other parameters are passed through a pointer to a apiInternalV1AddressPostRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **userId** | **string** | User ID |
 **dtoAddress** | [**DtoAddress**](DtoAddress.md) | Address request |

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
