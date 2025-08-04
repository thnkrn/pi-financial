# \FundAccountAPI

All URIs are relative to *http://localhost*

Method | HTTP request | Description
------------- | ------------- | -------------
[**GenerateDocument**](FundAccountAPI.md#GenerateDocument) | **Post** /fund-account/document | 
[**GetAccountOpeningState**](FundAccountAPI.md#GetAccountOpeningState) | **Get** /fund-accounts/opening-state | 
[**InternalGetAccountOpeningStateByCustCode**](FundAccountAPI.md#InternalGetAccountOpeningStateByCustCode) | **Get** /internal/fund-accounts/opening-state/{CustCode} | 
[**IsFundAccountExist**](FundAccountAPI.md#IsFundAccountExist) | **Post** /fund-account/existence | Is Fund Account Exist
[**Ndid**](FundAccountAPI.md#Ndid) | **Get** /ndid | Open Fund Account
[**OpenFundAccount**](FundAccountAPI.md#OpenFundAccount) | **Post** /fund-account | Open Fund Account
[**OpenFundAccounts**](FundAccountAPI.md#OpenFundAccounts) | **Post** /fund-accounts | 
[**UpdateFundCustomer**](FundAccountAPI.md#UpdateFundCustomer) | **Put** /debug/fund-customer/update | 



## GenerateDocument

> []PiFinancialFundServiceAPIModelsGeneratedDocument GenerateDocument(ctx).PiFinancialFundServiceAPIModelsOpenFundAccountDto(piFinancialFundServiceAPIModelsOpenFundAccountDto).Execute()



### Example

```go
package main

import (
	"context"
	"fmt"
	"os"
	openapiclient "github.com/pi-financial/fund-srv/go-client"
)

func main() {
	piFinancialFundServiceAPIModelsOpenFundAccountDto := *openapiclient.NewPiFinancialFundServiceAPIModelsOpenFundAccountDto("CustomerCode_example") // PiFinancialFundServiceAPIModelsOpenFundAccountDto |  (optional)

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	resp, r, err := apiClient.FundAccountAPI.GenerateDocument(context.Background()).PiFinancialFundServiceAPIModelsOpenFundAccountDto(piFinancialFundServiceAPIModelsOpenFundAccountDto).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `FundAccountAPI.GenerateDocument``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `GenerateDocument`: []PiFinancialFundServiceAPIModelsGeneratedDocument
	fmt.Fprintf(os.Stdout, "Response from `FundAccountAPI.GenerateDocument`: %v\n", resp)
}
```

### Path Parameters



### Other Parameters

Other parameters are passed through a pointer to a apiGenerateDocumentRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **piFinancialFundServiceAPIModelsOpenFundAccountDto** | [**PiFinancialFundServiceAPIModelsOpenFundAccountDto**](PiFinancialFundServiceAPIModelsOpenFundAccountDto.md) |  | 

### Return type

[**[]PiFinancialFundServiceAPIModelsGeneratedDocument**](PiFinancialFundServiceAPIModelsGeneratedDocument.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: application/json, text/json, application/*+json
- **Accept**: text/plain, application/json, text/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)


## GetAccountOpeningState

> []PiFinancialFundServiceApplicationQueriesFundAccountOpeningStatus GetAccountOpeningState(ctx).RequestReceivedDate(requestReceivedDate).Ndid(ndid).Execute()



### Example

```go
package main

import (
	"context"
	"fmt"
	"os"
	openapiclient "github.com/pi-financial/fund-srv/go-client"
)

func main() {
	requestReceivedDate := "requestReceivedDate_example" // string |  (optional)
	ndid := true // bool |  (optional)

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	resp, r, err := apiClient.FundAccountAPI.GetAccountOpeningState(context.Background()).RequestReceivedDate(requestReceivedDate).Ndid(ndid).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `FundAccountAPI.GetAccountOpeningState``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `GetAccountOpeningState`: []PiFinancialFundServiceApplicationQueriesFundAccountOpeningStatus
	fmt.Fprintf(os.Stdout, "Response from `FundAccountAPI.GetAccountOpeningState`: %v\n", resp)
}
```

### Path Parameters



### Other Parameters

Other parameters are passed through a pointer to a apiGetAccountOpeningStateRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **requestReceivedDate** | **string** |  | 
 **ndid** | **bool** |  | 

### Return type

[**[]PiFinancialFundServiceApplicationQueriesFundAccountOpeningStatus**](PiFinancialFundServiceApplicationQueriesFundAccountOpeningStatus.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: text/plain, application/json, text/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)


## InternalGetAccountOpeningStateByCustCode

> PiFinancialFundServiceApplicationQueriesFundAccountOpeningStatusIEnumerableApiResponse InternalGetAccountOpeningStateByCustCode(ctx, custCode).Execute()



### Example

```go
package main

import (
	"context"
	"fmt"
	"os"
	openapiclient "github.com/pi-financial/fund-srv/go-client"
)

func main() {
	custCode := "custCode_example" // string | 

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	resp, r, err := apiClient.FundAccountAPI.InternalGetAccountOpeningStateByCustCode(context.Background(), custCode).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `FundAccountAPI.InternalGetAccountOpeningStateByCustCode``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `InternalGetAccountOpeningStateByCustCode`: PiFinancialFundServiceApplicationQueriesFundAccountOpeningStatusIEnumerableApiResponse
	fmt.Fprintf(os.Stdout, "Response from `FundAccountAPI.InternalGetAccountOpeningStateByCustCode`: %v\n", resp)
}
```

### Path Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
**ctx** | **context.Context** | context for authentication, logging, cancellation, deadlines, tracing, etc.
**custCode** | **string** |  | 

### Other Parameters

Other parameters are passed through a pointer to a apiInternalGetAccountOpeningStateByCustCodeRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------


### Return type

[**PiFinancialFundServiceApplicationQueriesFundAccountOpeningStatusIEnumerableApiResponse**](PiFinancialFundServiceApplicationQueriesFundAccountOpeningStatusIEnumerableApiResponse.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: text/plain, application/json, text/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)


## IsFundAccountExist

> SystemBooleanApiResponse IsFundAccountExist(ctx).PiFinancialFundServiceAPIModelsFundAccountExistenceRequest(piFinancialFundServiceAPIModelsFundAccountExistenceRequest).Execute()

Is Fund Account Exist

### Example

```go
package main

import (
	"context"
	"fmt"
	"os"
	openapiclient "github.com/pi-financial/fund-srv/go-client"
)

func main() {
	piFinancialFundServiceAPIModelsFundAccountExistenceRequest := *openapiclient.NewPiFinancialFundServiceAPIModelsFundAccountExistenceRequest("IdentificationCardNo_example") // PiFinancialFundServiceAPIModelsFundAccountExistenceRequest |  (optional)

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	resp, r, err := apiClient.FundAccountAPI.IsFundAccountExist(context.Background()).PiFinancialFundServiceAPIModelsFundAccountExistenceRequest(piFinancialFundServiceAPIModelsFundAccountExistenceRequest).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `FundAccountAPI.IsFundAccountExist``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `IsFundAccountExist`: SystemBooleanApiResponse
	fmt.Fprintf(os.Stdout, "Response from `FundAccountAPI.IsFundAccountExist`: %v\n", resp)
}
```

### Path Parameters



### Other Parameters

Other parameters are passed through a pointer to a apiIsFundAccountExistRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **piFinancialFundServiceAPIModelsFundAccountExistenceRequest** | [**PiFinancialFundServiceAPIModelsFundAccountExistenceRequest**](PiFinancialFundServiceAPIModelsFundAccountExistenceRequest.md) |  | 

### Return type

[**SystemBooleanApiResponse**](SystemBooleanApiResponse.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: application/json, text/json, application/*+json
- **Accept**: text/plain, application/json, text/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)


## Ndid

> Ndid(ctx).CustCode(custCode).Execute()

Open Fund Account

### Example

```go
package main

import (
	"context"
	"fmt"
	"os"
	openapiclient "github.com/pi-financial/fund-srv/go-client"
)

func main() {
	custCode := "custCode_example" // string |  (optional)

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	r, err := apiClient.FundAccountAPI.Ndid(context.Background()).CustCode(custCode).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `FundAccountAPI.Ndid``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
}
```

### Path Parameters



### Other Parameters

Other parameters are passed through a pointer to a apiNdidRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **custCode** | **string** |  | 

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


## OpenFundAccount

> PiFinancialFundServiceAPIModelsFundAccountOpeningTicket OpenFundAccount(ctx).PiFinancialFundServiceAPIModelsOpenFundAccountDto(piFinancialFundServiceAPIModelsOpenFundAccountDto).Execute()

Open Fund Account

### Example

```go
package main

import (
	"context"
	"fmt"
	"os"
	openapiclient "github.com/pi-financial/fund-srv/go-client"
)

func main() {
	piFinancialFundServiceAPIModelsOpenFundAccountDto := *openapiclient.NewPiFinancialFundServiceAPIModelsOpenFundAccountDto("CustomerCode_example") // PiFinancialFundServiceAPIModelsOpenFundAccountDto |  (optional)

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	resp, r, err := apiClient.FundAccountAPI.OpenFundAccount(context.Background()).PiFinancialFundServiceAPIModelsOpenFundAccountDto(piFinancialFundServiceAPIModelsOpenFundAccountDto).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `FundAccountAPI.OpenFundAccount``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `OpenFundAccount`: PiFinancialFundServiceAPIModelsFundAccountOpeningTicket
	fmt.Fprintf(os.Stdout, "Response from `FundAccountAPI.OpenFundAccount`: %v\n", resp)
}
```

### Path Parameters



### Other Parameters

Other parameters are passed through a pointer to a apiOpenFundAccountRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **piFinancialFundServiceAPIModelsOpenFundAccountDto** | [**PiFinancialFundServiceAPIModelsOpenFundAccountDto**](PiFinancialFundServiceAPIModelsOpenFundAccountDto.md) |  | 

### Return type

[**PiFinancialFundServiceAPIModelsFundAccountOpeningTicket**](PiFinancialFundServiceAPIModelsFundAccountOpeningTicket.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: application/json, text/json, application/*+json
- **Accept**: text/plain, application/json, text/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)


## OpenFundAccounts

> []PiFinancialFundServiceAPIModelsFundAccountOpeningTicket OpenFundAccounts(ctx).PiFinancialFundServiceAPIModelsOpenFundAccountDto(piFinancialFundServiceAPIModelsOpenFundAccountDto).Execute()



### Example

```go
package main

import (
	"context"
	"fmt"
	"os"
	openapiclient "github.com/pi-financial/fund-srv/go-client"
)

func main() {
	piFinancialFundServiceAPIModelsOpenFundAccountDto := []openapiclient.PiFinancialFundServiceAPIModelsOpenFundAccountDto{*openapiclient.NewPiFinancialFundServiceAPIModelsOpenFundAccountDto("CustomerCode_example")} // []PiFinancialFundServiceAPIModelsOpenFundAccountDto |  (optional)

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	resp, r, err := apiClient.FundAccountAPI.OpenFundAccounts(context.Background()).PiFinancialFundServiceAPIModelsOpenFundAccountDto(piFinancialFundServiceAPIModelsOpenFundAccountDto).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `FundAccountAPI.OpenFundAccounts``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `OpenFundAccounts`: []PiFinancialFundServiceAPIModelsFundAccountOpeningTicket
	fmt.Fprintf(os.Stdout, "Response from `FundAccountAPI.OpenFundAccounts`: %v\n", resp)
}
```

### Path Parameters



### Other Parameters

Other parameters are passed through a pointer to a apiOpenFundAccountsRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **piFinancialFundServiceAPIModelsOpenFundAccountDto** | [**[]PiFinancialFundServiceAPIModelsOpenFundAccountDto**](PiFinancialFundServiceAPIModelsOpenFundAccountDto.md) |  | 

### Return type

[**[]PiFinancialFundServiceAPIModelsFundAccountOpeningTicket**](PiFinancialFundServiceAPIModelsFundAccountOpeningTicket.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: application/json, text/json, application/*+json
- **Accept**: text/plain, application/json, text/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)


## UpdateFundCustomer

> UpdateFundCustomer(ctx).PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5(piFinancialClientFundConnextModelCustomerAccountCreateRequestV5).Execute()



### Example

```go
package main

import (
	"context"
	"fmt"
	"os"
	openapiclient "github.com/pi-financial/fund-srv/go-client"
)

func main() {
	piFinancialClientFundConnextModelCustomerAccountCreateRequestV5 := *openapiclient.NewPiFinancialClientFundConnextModelCustomerAccountCreateRequestV5() // PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5 | 

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	r, err := apiClient.FundAccountAPI.UpdateFundCustomer(context.Background()).PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5(piFinancialClientFundConnextModelCustomerAccountCreateRequestV5).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `FundAccountAPI.UpdateFundCustomer``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
}
```

### Path Parameters



### Other Parameters

Other parameters are passed through a pointer to a apiUpdateFundCustomerRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **piFinancialClientFundConnextModelCustomerAccountCreateRequestV5** | [**PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5**](PiFinancialClientFundConnextModelCustomerAccountCreateRequestV5.md) |  | 

### Return type

 (empty response body)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: application/json, text/json, application/*+json
- **Accept**: text/plain, application/json, text/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)

