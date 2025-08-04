# \CustomerBankAccountAPI

All URIs are relative to *http://localhost*

Method | HTTP request | Description
------------- | ------------- | -------------
[**GetBankAccountInfo**](CustomerBankAccountAPI.md#GetBankAccountInfo) | **Get** /internal/customer/bank-account | Get bank account info
[**UpdateBankAccountEffectiveDate**](CustomerBankAccountAPI.md#UpdateBankAccountEffectiveDate) | **Post** /internal/customer/bank-account/effective-date | Update bank account effective (RPType &#x3D; R)



## GetBankAccountInfo

> PiUserApplicationServicesLegacyUserInfoBankAccountInfoApiResponse GetBankAccountInfo(ctx).Id(id).Execute()

Get bank account info

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
	id := "id_example" // string |  (optional)

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	resp, r, err := apiClient.CustomerBankAccountAPI.GetBankAccountInfo(context.Background()).Id(id).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `CustomerBankAccountAPI.GetBankAccountInfo``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `GetBankAccountInfo`: PiUserApplicationServicesLegacyUserInfoBankAccountInfoApiResponse
	fmt.Fprintf(os.Stdout, "Response from `CustomerBankAccountAPI.GetBankAccountInfo`: %v\n", resp)
}
```

### Path Parameters



### Other Parameters

Other parameters are passed through a pointer to a apiGetBankAccountInfoRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **id** | **string** |  | 

### Return type

[**PiUserApplicationServicesLegacyUserInfoBankAccountInfoApiResponse**](PiUserApplicationServicesLegacyUserInfoBankAccountInfoApiResponse.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: text/plain, application/json, text/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)


## UpdateBankAccountEffectiveDate

> UpdateBankAccountEffectiveDate(ctx).PiUserAPIModelsUpdateBankAccountEffectiveDateRequest(piUserAPIModelsUpdateBankAccountEffectiveDateRequest).Execute()

Update bank account effective (RPType = R)

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
	piUserAPIModelsUpdateBankAccountEffectiveDateRequest := *openapiclient.NewPiUserAPIModelsUpdateBankAccountEffectiveDateRequest() // PiUserAPIModelsUpdateBankAccountEffectiveDateRequest |  (optional)

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	r, err := apiClient.CustomerBankAccountAPI.UpdateBankAccountEffectiveDate(context.Background()).PiUserAPIModelsUpdateBankAccountEffectiveDateRequest(piUserAPIModelsUpdateBankAccountEffectiveDateRequest).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `CustomerBankAccountAPI.UpdateBankAccountEffectiveDate``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
}
```

### Path Parameters



### Other Parameters

Other parameters are passed through a pointer to a apiUpdateBankAccountEffectiveDateRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **piUserAPIModelsUpdateBankAccountEffectiveDateRequest** | [**PiUserAPIModelsUpdateBankAccountEffectiveDateRequest**](PiUserAPIModelsUpdateBankAccountEffectiveDateRequest.md) |  | 

### Return type

 (empty response body)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: application/json, text/json, application/*+json
- **Accept**: Not defined

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)

