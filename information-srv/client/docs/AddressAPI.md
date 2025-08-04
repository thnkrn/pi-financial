# \AddressAPI

All URIs are relative to *http://localhost*

Method | HTTP request | Description
------------- | ------------- | -------------
[**InternalAddressGet**](AddressAPI.md#InternalAddressGet) | **Get** /internal/address | Get All Addresses
[**InternalAddressProvinceGet**](AddressAPI.md#InternalAddressProvinceGet) | **Get** /internal/address/province | Get All Provinces
[**InternalAddressProvinceProvinceGet**](AddressAPI.md#InternalAddressProvinceProvinceGet) | **Get** /internal/address/province/{province} | Get Addresses by Province
[**InternalAddressZipCodeZipCodeGet**](AddressAPI.md#InternalAddressZipCodeZipCodeGet) | **Get** /internal/address/zip-code/{zipCode} | Get Addresses by Zip Code
[**SecureAddressGet**](AddressAPI.md#SecureAddressGet) | **Get** /secure/address | Get All Addresses
[**SecureAddressProvinceGet**](AddressAPI.md#SecureAddressProvinceGet) | **Get** /secure/address/province | Get All Provinces
[**SecureAddressProvinceProvinceGet**](AddressAPI.md#SecureAddressProvinceProvinceGet) | **Get** /secure/address/province/{province} | Get Addresses by Province
[**SecureAddressZipCodeZipCodeGet**](AddressAPI.md#SecureAddressZipCodeZipCodeGet) | **Get** /secure/address/zip-code/{zipCode} | Get Addresses by Zip Code



## InternalAddressGet

> InternalAddressGet200Response InternalAddressGet(ctx).Execute()

Get All Addresses



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

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	resp, r, err := apiClient.AddressAPI.InternalAddressGet(context.Background()).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `AddressAPI.InternalAddressGet``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `InternalAddressGet`: InternalAddressGet200Response
	fmt.Fprintf(os.Stdout, "Response from `AddressAPI.InternalAddressGet`: %v\n", resp)
}
```

### Path Parameters

This endpoint does not need any parameter.

### Other Parameters

Other parameters are passed through a pointer to a apiInternalAddressGetRequest struct via the builder pattern


### Return type

[**InternalAddressGet200Response**](InternalAddressGet200Response.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)


## InternalAddressProvinceGet

> InternalAddressProvinceGet200Response InternalAddressProvinceGet(ctx).Execute()

Get All Provinces



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

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	resp, r, err := apiClient.AddressAPI.InternalAddressProvinceGet(context.Background()).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `AddressAPI.InternalAddressProvinceGet``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `InternalAddressProvinceGet`: InternalAddressProvinceGet200Response
	fmt.Fprintf(os.Stdout, "Response from `AddressAPI.InternalAddressProvinceGet`: %v\n", resp)
}
```

### Path Parameters

This endpoint does not need any parameter.

### Other Parameters

Other parameters are passed through a pointer to a apiInternalAddressProvinceGetRequest struct via the builder pattern


### Return type

[**InternalAddressProvinceGet200Response**](InternalAddressProvinceGet200Response.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)


## InternalAddressProvinceProvinceGet

> InternalAddressGet200Response InternalAddressProvinceProvinceGet(ctx, province).Lang(lang).Execute()

Get Addresses by Province



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
	province := "province_example" // string | The Province to retrieve addresses
	lang := "lang_example" // string | Language preference for response data. Omit for Thai (default) or specify 'en' for English (optional)

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	resp, r, err := apiClient.AddressAPI.InternalAddressProvinceProvinceGet(context.Background(), province).Lang(lang).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `AddressAPI.InternalAddressProvinceProvinceGet``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `InternalAddressProvinceProvinceGet`: InternalAddressGet200Response
	fmt.Fprintf(os.Stdout, "Response from `AddressAPI.InternalAddressProvinceProvinceGet`: %v\n", resp)
}
```

### Path Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
**ctx** | **context.Context** | context for authentication, logging, cancellation, deadlines, tracing, etc.
**province** | **string** | The Province to retrieve addresses | 

### Other Parameters

Other parameters are passed through a pointer to a apiInternalAddressProvinceProvinceGetRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------

 **lang** | **string** | Language preference for response data. Omit for Thai (default) or specify &#39;en&#39; for English | 

### Return type

[**InternalAddressGet200Response**](InternalAddressGet200Response.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)


## InternalAddressZipCodeZipCodeGet

> InternalAddressGet200Response InternalAddressZipCodeZipCodeGet(ctx, zipCode).Execute()

Get Addresses by Zip Code



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
	zipCode := int32(56) // int32 | The Zip Code to retrieve addresses

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	resp, r, err := apiClient.AddressAPI.InternalAddressZipCodeZipCodeGet(context.Background(), zipCode).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `AddressAPI.InternalAddressZipCodeZipCodeGet``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `InternalAddressZipCodeZipCodeGet`: InternalAddressGet200Response
	fmt.Fprintf(os.Stdout, "Response from `AddressAPI.InternalAddressZipCodeZipCodeGet`: %v\n", resp)
}
```

### Path Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
**ctx** | **context.Context** | context for authentication, logging, cancellation, deadlines, tracing, etc.
**zipCode** | **int32** | The Zip Code to retrieve addresses | 

### Other Parameters

Other parameters are passed through a pointer to a apiInternalAddressZipCodeZipCodeGetRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------


### Return type

[**InternalAddressGet200Response**](InternalAddressGet200Response.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)


## SecureAddressGet

> InternalAddressGet200Response SecureAddressGet(ctx).Execute()

Get All Addresses



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

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	resp, r, err := apiClient.AddressAPI.SecureAddressGet(context.Background()).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `AddressAPI.SecureAddressGet``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `SecureAddressGet`: InternalAddressGet200Response
	fmt.Fprintf(os.Stdout, "Response from `AddressAPI.SecureAddressGet`: %v\n", resp)
}
```

### Path Parameters

This endpoint does not need any parameter.

### Other Parameters

Other parameters are passed through a pointer to a apiSecureAddressGetRequest struct via the builder pattern


### Return type

[**InternalAddressGet200Response**](InternalAddressGet200Response.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)


## SecureAddressProvinceGet

> InternalAddressProvinceGet200Response SecureAddressProvinceGet(ctx).Execute()

Get All Provinces



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

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	resp, r, err := apiClient.AddressAPI.SecureAddressProvinceGet(context.Background()).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `AddressAPI.SecureAddressProvinceGet``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `SecureAddressProvinceGet`: InternalAddressProvinceGet200Response
	fmt.Fprintf(os.Stdout, "Response from `AddressAPI.SecureAddressProvinceGet`: %v\n", resp)
}
```

### Path Parameters

This endpoint does not need any parameter.

### Other Parameters

Other parameters are passed through a pointer to a apiSecureAddressProvinceGetRequest struct via the builder pattern


### Return type

[**InternalAddressProvinceGet200Response**](InternalAddressProvinceGet200Response.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)


## SecureAddressProvinceProvinceGet

> InternalAddressGet200Response SecureAddressProvinceProvinceGet(ctx, province).Lang(lang).Execute()

Get Addresses by Province



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
	province := "province_example" // string | The Province to retrieve addresses
	lang := "lang_example" // string | Language preference for response data. Omit for Thai (default) or specify 'en' for English (optional)

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	resp, r, err := apiClient.AddressAPI.SecureAddressProvinceProvinceGet(context.Background(), province).Lang(lang).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `AddressAPI.SecureAddressProvinceProvinceGet``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `SecureAddressProvinceProvinceGet`: InternalAddressGet200Response
	fmt.Fprintf(os.Stdout, "Response from `AddressAPI.SecureAddressProvinceProvinceGet`: %v\n", resp)
}
```

### Path Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
**ctx** | **context.Context** | context for authentication, logging, cancellation, deadlines, tracing, etc.
**province** | **string** | The Province to retrieve addresses | 

### Other Parameters

Other parameters are passed through a pointer to a apiSecureAddressProvinceProvinceGetRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------

 **lang** | **string** | Language preference for response data. Omit for Thai (default) or specify &#39;en&#39; for English | 

### Return type

[**InternalAddressGet200Response**](InternalAddressGet200Response.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)


## SecureAddressZipCodeZipCodeGet

> InternalAddressGet200Response SecureAddressZipCodeZipCodeGet(ctx, zipCode).Execute()

Get Addresses by Zip Code



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
	zipCode := int32(56) // int32 | The Zip Code to retrieve addresses

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	resp, r, err := apiClient.AddressAPI.SecureAddressZipCodeZipCodeGet(context.Background(), zipCode).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `AddressAPI.SecureAddressZipCodeZipCodeGet``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `SecureAddressZipCodeZipCodeGet`: InternalAddressGet200Response
	fmt.Fprintf(os.Stdout, "Response from `AddressAPI.SecureAddressZipCodeZipCodeGet`: %v\n", resp)
}
```

### Path Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
**ctx** | **context.Context** | context for authentication, logging, cancellation, deadlines, tracing, etc.
**zipCode** | **int32** | The Zip Code to retrieve addresses | 

### Other Parameters

Other parameters are passed through a pointer to a apiSecureAddressZipCodeZipCodeGetRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------


### Return type

[**InternalAddressGet200Response**](InternalAddressGet200Response.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)

