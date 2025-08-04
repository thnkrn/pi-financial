# \FundTradingAPI

All URIs are relative to *http://localhost*

Method | HTTP request | Description
------------- | ------------- | -------------
[**InternalAccountsAssetsGet**](FundTradingAPI.md#InternalAccountsAssetsGet) | **Get** /internal/accounts/assets | 
[**InternalAccountsSummariesGet**](FundTradingAPI.md#InternalAccountsSummariesGet) | **Get** /internal/accounts/summaries | 
[**InternalOrdersBuyPost**](FundTradingAPI.md#InternalOrdersBuyPost) | **Post** /internal/orders/buy | Subscribes to a fund order from internal DCA, which will contain the full error message.
[**InternalOrdersDeleteDelete**](FundTradingAPI.md#InternalOrdersDeleteDelete) | **Delete** /internal/orders/delete | 
[**InternalOrdersHistoriesOrdernoGet**](FundTradingAPI.md#InternalOrdersHistoriesOrdernoGet) | **Get** /internal/orders/histories/orderno | Get orders history by saOrderReferenceNo
[**InternalOrdersRawOrdersGet**](FundTradingAPI.md#InternalOrdersRawOrdersGet) | **Get** /internal/orders/raw-orders | 
[**SecureAccountsAssetsGet**](FundTradingAPI.md#SecureAccountsAssetsGet) | **Get** /secure/accounts/assets | 
[**SecureAccountsOpenordersGet**](FundTradingAPI.md#SecureAccountsOpenordersGet) | **Get** /secure/accounts/openorders | 
[**SecureOrdersBuyPost**](FundTradingAPI.md#SecureOrdersBuyPost) | **Post** /secure/orders/buy | 
[**SecureOrdersHistoriesGet**](FundTradingAPI.md#SecureOrdersHistoriesGet) | **Get** /secure/orders/histories | 
[**SecureOrdersSellPost**](FundTradingAPI.md#SecureOrdersSellPost) | **Post** /secure/orders/sell | 
[**SecureOrdersSwitchInfoGet**](FundTradingAPI.md#SecureOrdersSwitchInfoGet) | **Get** /secure/orders/switch/info | 
[**SecureOrdersSwitchPost**](FundTradingAPI.md#SecureOrdersSwitchPost) | **Post** /secure/orders/switch | 
[**TradingOrdersBuyPost**](FundTradingAPI.md#TradingOrdersBuyPost) | **Post** /trading/orders/buy | 
[**TradingOrdersSellPost**](FundTradingAPI.md#TradingOrdersSellPost) | **Post** /trading/orders/sell | 
[**TradingOrdersSwitchPost**](FundTradingAPI.md#TradingOrdersSwitchPost) | **Post** /trading/orders/switch | 



## InternalAccountsAssetsGet

> PiFinancialFundServiceAPIModelsInternalFundAssetResponseListApiResponse InternalAccountsAssetsGet(ctx).UserId(userId).TradingAccountNo(tradingAccountNo).Execute()



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
	userId := "38400000-8cf0-11bd-b23e-10b96e4ef00d" // string | 
	tradingAccountNo := "tradingAccountNo_example" // string |  (optional)

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	resp, r, err := apiClient.FundTradingAPI.InternalAccountsAssetsGet(context.Background()).UserId(userId).TradingAccountNo(tradingAccountNo).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `FundTradingAPI.InternalAccountsAssetsGet``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `InternalAccountsAssetsGet`: PiFinancialFundServiceAPIModelsInternalFundAssetResponseListApiResponse
	fmt.Fprintf(os.Stdout, "Response from `FundTradingAPI.InternalAccountsAssetsGet`: %v\n", resp)
}
```

### Path Parameters



### Other Parameters

Other parameters are passed through a pointer to a apiInternalAccountsAssetsGetRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **userId** | **string** |  | 
 **tradingAccountNo** | **string** |  | 

### Return type

[**PiFinancialFundServiceAPIModelsInternalFundAssetResponseListApiResponse**](PiFinancialFundServiceAPIModelsInternalFundAssetResponseListApiResponse.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: text/plain, application/json, text/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)


## InternalAccountsSummariesGet

> PiFinancialFundServiceAPIModelsAccountSummaryResponseArrayApiResponse InternalAccountsSummariesGet(ctx).UserId(userId).Execute()



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
	userId := "38400000-8cf0-11bd-b23e-10b96e4ef00d" // string | 

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	resp, r, err := apiClient.FundTradingAPI.InternalAccountsSummariesGet(context.Background()).UserId(userId).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `FundTradingAPI.InternalAccountsSummariesGet``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `InternalAccountsSummariesGet`: PiFinancialFundServiceAPIModelsAccountSummaryResponseArrayApiResponse
	fmt.Fprintf(os.Stdout, "Response from `FundTradingAPI.InternalAccountsSummariesGet`: %v\n", resp)
}
```

### Path Parameters



### Other Parameters

Other parameters are passed through a pointer to a apiInternalAccountsSummariesGetRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **userId** | **string** |  | 

### Return type

[**PiFinancialFundServiceAPIModelsAccountSummaryResponseArrayApiResponse**](PiFinancialFundServiceAPIModelsAccountSummaryResponseArrayApiResponse.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: text/plain, application/json, text/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)


## InternalOrdersBuyPost

> PiFinancialFundServiceAPIModelsFundOrderPlacedApiResponse InternalOrdersBuyPost(ctx).UserId(userId).PiFinancialFundServiceAPIModelsSiriusPlaceBuyOrderRequest(piFinancialFundServiceAPIModelsSiriusPlaceBuyOrderRequest).Execute()

Subscribes to a fund order from internal DCA, which will contain the full error message.

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
	userId := "38400000-8cf0-11bd-b23e-10b96e4ef00d" // string | 
	piFinancialFundServiceAPIModelsSiriusPlaceBuyOrderRequest := *openapiclient.NewPiFinancialFundServiceAPIModelsSiriusPlaceBuyOrderRequest("Symbol_example", "EffectiveDate_example", "TradingAccountNo_example", float64(123), "PaymentMethod_example", "BankAccount_example", "BankCode_example") // PiFinancialFundServiceAPIModelsSiriusPlaceBuyOrderRequest |  (optional)

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	resp, r, err := apiClient.FundTradingAPI.InternalOrdersBuyPost(context.Background()).UserId(userId).PiFinancialFundServiceAPIModelsSiriusPlaceBuyOrderRequest(piFinancialFundServiceAPIModelsSiriusPlaceBuyOrderRequest).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `FundTradingAPI.InternalOrdersBuyPost``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `InternalOrdersBuyPost`: PiFinancialFundServiceAPIModelsFundOrderPlacedApiResponse
	fmt.Fprintf(os.Stdout, "Response from `FundTradingAPI.InternalOrdersBuyPost`: %v\n", resp)
}
```

### Path Parameters



### Other Parameters

Other parameters are passed through a pointer to a apiInternalOrdersBuyPostRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **userId** | **string** |  | 
 **piFinancialFundServiceAPIModelsSiriusPlaceBuyOrderRequest** | [**PiFinancialFundServiceAPIModelsSiriusPlaceBuyOrderRequest**](PiFinancialFundServiceAPIModelsSiriusPlaceBuyOrderRequest.md) |  | 

### Return type

[**PiFinancialFundServiceAPIModelsFundOrderPlacedApiResponse**](PiFinancialFundServiceAPIModelsFundOrderPlacedApiResponse.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: application/json, text/json, application/*+json
- **Accept**: text/plain, application/json, text/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)


## InternalOrdersDeleteDelete

> PiFinancialFundServiceAPIModelsBrokerOrderApiResponse InternalOrdersDeleteDelete(ctx).UserId(userId).PiFinancialFundServiceAPIModelsDeleteOrderRequest(piFinancialFundServiceAPIModelsDeleteOrderRequest).Execute()



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
	userId := "38400000-8cf0-11bd-b23e-10b96e4ef00d" // string | 
	piFinancialFundServiceAPIModelsDeleteOrderRequest := *openapiclient.NewPiFinancialFundServiceAPIModelsDeleteOrderRequest("BrokerOrderId_example", "OrderSide_example", "TradingAccountNo_example") // PiFinancialFundServiceAPIModelsDeleteOrderRequest |  (optional)

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	resp, r, err := apiClient.FundTradingAPI.InternalOrdersDeleteDelete(context.Background()).UserId(userId).PiFinancialFundServiceAPIModelsDeleteOrderRequest(piFinancialFundServiceAPIModelsDeleteOrderRequest).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `FundTradingAPI.InternalOrdersDeleteDelete``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `InternalOrdersDeleteDelete`: PiFinancialFundServiceAPIModelsBrokerOrderApiResponse
	fmt.Fprintf(os.Stdout, "Response from `FundTradingAPI.InternalOrdersDeleteDelete`: %v\n", resp)
}
```

### Path Parameters



### Other Parameters

Other parameters are passed through a pointer to a apiInternalOrdersDeleteDeleteRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **userId** | **string** |  | 
 **piFinancialFundServiceAPIModelsDeleteOrderRequest** | [**PiFinancialFundServiceAPIModelsDeleteOrderRequest**](PiFinancialFundServiceAPIModelsDeleteOrderRequest.md) |  | 

### Return type

[**PiFinancialFundServiceAPIModelsBrokerOrderApiResponse**](PiFinancialFundServiceAPIModelsBrokerOrderApiResponse.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: application/json, text/json, application/*+json
- **Accept**: text/plain, application/json, text/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)


## InternalOrdersHistoriesOrdernoGet

> PiFinancialFundServiceAPIModelsSiriusFundOrderHistoryResponseArrayApiResponse InternalOrdersHistoriesOrdernoGet(ctx).OrderNumbers(orderNumbers).Execute()

Get orders history by saOrderReferenceNo

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
	orderNumbers := "FOSW202411260004,FOSUB202403230022" // string | saOrderReferenceNo

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	resp, r, err := apiClient.FundTradingAPI.InternalOrdersHistoriesOrdernoGet(context.Background()).OrderNumbers(orderNumbers).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `FundTradingAPI.InternalOrdersHistoriesOrdernoGet``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `InternalOrdersHistoriesOrdernoGet`: PiFinancialFundServiceAPIModelsSiriusFundOrderHistoryResponseArrayApiResponse
	fmt.Fprintf(os.Stdout, "Response from `FundTradingAPI.InternalOrdersHistoriesOrdernoGet`: %v\n", resp)
}
```

### Path Parameters



### Other Parameters

Other parameters are passed through a pointer to a apiInternalOrdersHistoriesOrdernoGetRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **orderNumbers** | **string** | saOrderReferenceNo | 

### Return type

[**PiFinancialFundServiceAPIModelsSiriusFundOrderHistoryResponseArrayApiResponse**](PiFinancialFundServiceAPIModelsSiriusFundOrderHistoryResponseArrayApiResponse.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: text/plain, application/json, text/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)


## InternalOrdersRawOrdersGet

> PiFinancialClientFundConnextModelFundOrderArrayApiResponse InternalOrdersRawOrdersGet(ctx).EffectiveDate(effectiveDate).Execute()



### Example

```go
package main

import (
	"context"
	"fmt"
	"os"
    "time"
	openapiclient "github.com/pi-financial/fund-srv/go-client"
)

func main() {
	effectiveDate := time.Now() // string | 

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	resp, r, err := apiClient.FundTradingAPI.InternalOrdersRawOrdersGet(context.Background()).EffectiveDate(effectiveDate).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `FundTradingAPI.InternalOrdersRawOrdersGet``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `InternalOrdersRawOrdersGet`: PiFinancialClientFundConnextModelFundOrderArrayApiResponse
	fmt.Fprintf(os.Stdout, "Response from `FundTradingAPI.InternalOrdersRawOrdersGet`: %v\n", resp)
}
```

### Path Parameters



### Other Parameters

Other parameters are passed through a pointer to a apiInternalOrdersRawOrdersGetRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **effectiveDate** | **string** |  | 

### Return type

[**PiFinancialClientFundConnextModelFundOrderArrayApiResponse**](PiFinancialClientFundConnextModelFundOrderArrayApiResponse.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: text/plain, application/json, text/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)


## SecureAccountsAssetsGet

> PiFinancialFundServiceAPIModelsSiriusFundAssetResponseArrayApiResponse SecureAccountsAssetsGet(ctx).UserId(userId).TradingAccountNo(tradingAccountNo).Execute()



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
	userId := "38400000-8cf0-11bd-b23e-10b96e4ef00d" // string | 
	tradingAccountNo := "tradingAccountNo_example" // string | 

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	resp, r, err := apiClient.FundTradingAPI.SecureAccountsAssetsGet(context.Background()).UserId(userId).TradingAccountNo(tradingAccountNo).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `FundTradingAPI.SecureAccountsAssetsGet``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `SecureAccountsAssetsGet`: PiFinancialFundServiceAPIModelsSiriusFundAssetResponseArrayApiResponse
	fmt.Fprintf(os.Stdout, "Response from `FundTradingAPI.SecureAccountsAssetsGet`: %v\n", resp)
}
```

### Path Parameters



### Other Parameters

Other parameters are passed through a pointer to a apiSecureAccountsAssetsGetRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **userId** | **string** |  | 
 **tradingAccountNo** | **string** |  | 

### Return type

[**PiFinancialFundServiceAPIModelsSiriusFundAssetResponseArrayApiResponse**](PiFinancialFundServiceAPIModelsSiriusFundAssetResponseArrayApiResponse.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: text/plain, application/json, text/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)


## SecureAccountsOpenordersGet

> PiFinancialFundServiceAPIModelsSiriusFundOrderResponseArrayApiResponse SecureAccountsOpenordersGet(ctx).UserId(userId).TradingAccountNo(tradingAccountNo).EffectiveDateFrom(effectiveDateFrom).EffectiveDateTo(effectiveDateTo).Execute()



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
	userId := "38400000-8cf0-11bd-b23e-10b96e4ef00d" // string | 
	tradingAccountNo := "tradingAccountNo_example" // string | 
	effectiveDateFrom := "effectiveDateFrom_example" // string | 
	effectiveDateTo := "effectiveDateTo_example" // string | 

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	resp, r, err := apiClient.FundTradingAPI.SecureAccountsOpenordersGet(context.Background()).UserId(userId).TradingAccountNo(tradingAccountNo).EffectiveDateFrom(effectiveDateFrom).EffectiveDateTo(effectiveDateTo).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `FundTradingAPI.SecureAccountsOpenordersGet``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `SecureAccountsOpenordersGet`: PiFinancialFundServiceAPIModelsSiriusFundOrderResponseArrayApiResponse
	fmt.Fprintf(os.Stdout, "Response from `FundTradingAPI.SecureAccountsOpenordersGet`: %v\n", resp)
}
```

### Path Parameters



### Other Parameters

Other parameters are passed through a pointer to a apiSecureAccountsOpenordersGetRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **userId** | **string** |  | 
 **tradingAccountNo** | **string** |  | 
 **effectiveDateFrom** | **string** |  | 
 **effectiveDateTo** | **string** |  | 

### Return type

[**PiFinancialFundServiceAPIModelsSiriusFundOrderResponseArrayApiResponse**](PiFinancialFundServiceAPIModelsSiriusFundOrderResponseArrayApiResponse.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: text/plain, application/json, text/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)


## SecureOrdersBuyPost

> PiFinancialFundServiceAPIModelsFundOrderPlacedApiResponse SecureOrdersBuyPost(ctx).UserId(userId).PiFinancialFundServiceAPIModelsSiriusPlaceBuyOrderRequest(piFinancialFundServiceAPIModelsSiriusPlaceBuyOrderRequest).Execute()



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
	userId := "38400000-8cf0-11bd-b23e-10b96e4ef00d" // string | 
	piFinancialFundServiceAPIModelsSiriusPlaceBuyOrderRequest := *openapiclient.NewPiFinancialFundServiceAPIModelsSiriusPlaceBuyOrderRequest("Symbol_example", "EffectiveDate_example", "TradingAccountNo_example", float64(123), "PaymentMethod_example", "BankAccount_example", "BankCode_example") // PiFinancialFundServiceAPIModelsSiriusPlaceBuyOrderRequest |  (optional)

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	resp, r, err := apiClient.FundTradingAPI.SecureOrdersBuyPost(context.Background()).UserId(userId).PiFinancialFundServiceAPIModelsSiriusPlaceBuyOrderRequest(piFinancialFundServiceAPIModelsSiriusPlaceBuyOrderRequest).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `FundTradingAPI.SecureOrdersBuyPost``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `SecureOrdersBuyPost`: PiFinancialFundServiceAPIModelsFundOrderPlacedApiResponse
	fmt.Fprintf(os.Stdout, "Response from `FundTradingAPI.SecureOrdersBuyPost`: %v\n", resp)
}
```

### Path Parameters



### Other Parameters

Other parameters are passed through a pointer to a apiSecureOrdersBuyPostRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **userId** | **string** |  | 
 **piFinancialFundServiceAPIModelsSiriusPlaceBuyOrderRequest** | [**PiFinancialFundServiceAPIModelsSiriusPlaceBuyOrderRequest**](PiFinancialFundServiceAPIModelsSiriusPlaceBuyOrderRequest.md) |  | 

### Return type

[**PiFinancialFundServiceAPIModelsFundOrderPlacedApiResponse**](PiFinancialFundServiceAPIModelsFundOrderPlacedApiResponse.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: application/json, text/json, application/*+json
- **Accept**: text/plain, application/json, text/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)


## SecureOrdersHistoriesGet

> PiFinancialFundServiceAPIModelsSiriusFundOrderHistoryResponseArrayApiResponse SecureOrdersHistoriesGet(ctx).UserId(userId).TradingAccountNo(tradingAccountNo).OrderType(orderType).BeginEffectiveDate(beginEffectiveDate).EndEffectiveDate(endEffectiveDate).Execute()



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
	userId := "38400000-8cf0-11bd-b23e-10b96e4ef00d" // string | 
	tradingAccountNo := "tradingAccountNo_example" // string | 
	orderType := "orderType_example" // string | 
	beginEffectiveDate := "beginEffectiveDate_example" // string |  (optional)
	endEffectiveDate := "endEffectiveDate_example" // string |  (optional)

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	resp, r, err := apiClient.FundTradingAPI.SecureOrdersHistoriesGet(context.Background()).UserId(userId).TradingAccountNo(tradingAccountNo).OrderType(orderType).BeginEffectiveDate(beginEffectiveDate).EndEffectiveDate(endEffectiveDate).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `FundTradingAPI.SecureOrdersHistoriesGet``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `SecureOrdersHistoriesGet`: PiFinancialFundServiceAPIModelsSiriusFundOrderHistoryResponseArrayApiResponse
	fmt.Fprintf(os.Stdout, "Response from `FundTradingAPI.SecureOrdersHistoriesGet`: %v\n", resp)
}
```

### Path Parameters



### Other Parameters

Other parameters are passed through a pointer to a apiSecureOrdersHistoriesGetRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **userId** | **string** |  | 
 **tradingAccountNo** | **string** |  | 
 **orderType** | **string** |  | 
 **beginEffectiveDate** | **string** |  | 
 **endEffectiveDate** | **string** |  | 

### Return type

[**PiFinancialFundServiceAPIModelsSiriusFundOrderHistoryResponseArrayApiResponse**](PiFinancialFundServiceAPIModelsSiriusFundOrderHistoryResponseArrayApiResponse.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: text/plain, application/json, text/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)


## SecureOrdersSellPost

> PiFinancialFundServiceAPIModelsFundOrderPlacedApiResponse SecureOrdersSellPost(ctx).UserId(userId).PiFinancialFundServiceAPIModelsSiriusPlaceSellOrderRequest(piFinancialFundServiceAPIModelsSiriusPlaceSellOrderRequest).Execute()



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
	userId := "38400000-8cf0-11bd-b23e-10b96e4ef00d" // string | 
	piFinancialFundServiceAPIModelsSiriusPlaceSellOrderRequest := *openapiclient.NewPiFinancialFundServiceAPIModelsSiriusPlaceSellOrderRequest("Symbol_example", "EffectiveDate_example", "TradingAccountNo_example", float64(123), "UnitType_example", "UnitHolderId_example", "BankAccount_example", "BankCode_example") // PiFinancialFundServiceAPIModelsSiriusPlaceSellOrderRequest |  (optional)

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	resp, r, err := apiClient.FundTradingAPI.SecureOrdersSellPost(context.Background()).UserId(userId).PiFinancialFundServiceAPIModelsSiriusPlaceSellOrderRequest(piFinancialFundServiceAPIModelsSiriusPlaceSellOrderRequest).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `FundTradingAPI.SecureOrdersSellPost``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `SecureOrdersSellPost`: PiFinancialFundServiceAPIModelsFundOrderPlacedApiResponse
	fmt.Fprintf(os.Stdout, "Response from `FundTradingAPI.SecureOrdersSellPost`: %v\n", resp)
}
```

### Path Parameters



### Other Parameters

Other parameters are passed through a pointer to a apiSecureOrdersSellPostRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **userId** | **string** |  | 
 **piFinancialFundServiceAPIModelsSiriusPlaceSellOrderRequest** | [**PiFinancialFundServiceAPIModelsSiriusPlaceSellOrderRequest**](PiFinancialFundServiceAPIModelsSiriusPlaceSellOrderRequest.md) |  | 

### Return type

[**PiFinancialFundServiceAPIModelsFundOrderPlacedApiResponse**](PiFinancialFundServiceAPIModelsFundOrderPlacedApiResponse.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: application/json, text/json, application/*+json
- **Accept**: text/plain, application/json, text/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)


## SecureOrdersSwitchInfoGet

> PiFinancialFundServiceApplicationModelsTradingSwitchInfoApiResponse SecureOrdersSwitchInfoGet(ctx).UserId(userId).TradingAccountNo(tradingAccountNo).Symbol(symbol).TargetSymbol(targetSymbol).Execute()



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
	userId := "38400000-8cf0-11bd-b23e-10b96e4ef00d" // string | 
	tradingAccountNo := "tradingAccountNo_example" // string | 
	symbol := "symbol_example" // string | 
	targetSymbol := "targetSymbol_example" // string | 

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	resp, r, err := apiClient.FundTradingAPI.SecureOrdersSwitchInfoGet(context.Background()).UserId(userId).TradingAccountNo(tradingAccountNo).Symbol(symbol).TargetSymbol(targetSymbol).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `FundTradingAPI.SecureOrdersSwitchInfoGet``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `SecureOrdersSwitchInfoGet`: PiFinancialFundServiceApplicationModelsTradingSwitchInfoApiResponse
	fmt.Fprintf(os.Stdout, "Response from `FundTradingAPI.SecureOrdersSwitchInfoGet`: %v\n", resp)
}
```

### Path Parameters



### Other Parameters

Other parameters are passed through a pointer to a apiSecureOrdersSwitchInfoGetRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **userId** | **string** |  | 
 **tradingAccountNo** | **string** |  | 
 **symbol** | **string** |  | 
 **targetSymbol** | **string** |  | 

### Return type

[**PiFinancialFundServiceApplicationModelsTradingSwitchInfoApiResponse**](PiFinancialFundServiceApplicationModelsTradingSwitchInfoApiResponse.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: text/plain, application/json, text/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)


## SecureOrdersSwitchPost

> PiFinancialFundServiceAPIModelsFundOrderPlacedApiResponse SecureOrdersSwitchPost(ctx).UserId(userId).PiFinancialFundServiceAPIModelsSiriusPlaceSwitchOrderRequest(piFinancialFundServiceAPIModelsSiriusPlaceSwitchOrderRequest).Execute()



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
	userId := "38400000-8cf0-11bd-b23e-10b96e4ef00d" // string | 
	piFinancialFundServiceAPIModelsSiriusPlaceSwitchOrderRequest := *openapiclient.NewPiFinancialFundServiceAPIModelsSiriusPlaceSwitchOrderRequest("Symbol_example", "EffectiveDate_example", "TradingAccountNo_example", float64(123), "TargetSymbol_example", "UnitHolderId_example", "UnitType_example") // PiFinancialFundServiceAPIModelsSiriusPlaceSwitchOrderRequest |  (optional)

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	resp, r, err := apiClient.FundTradingAPI.SecureOrdersSwitchPost(context.Background()).UserId(userId).PiFinancialFundServiceAPIModelsSiriusPlaceSwitchOrderRequest(piFinancialFundServiceAPIModelsSiriusPlaceSwitchOrderRequest).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `FundTradingAPI.SecureOrdersSwitchPost``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `SecureOrdersSwitchPost`: PiFinancialFundServiceAPIModelsFundOrderPlacedApiResponse
	fmt.Fprintf(os.Stdout, "Response from `FundTradingAPI.SecureOrdersSwitchPost`: %v\n", resp)
}
```

### Path Parameters



### Other Parameters

Other parameters are passed through a pointer to a apiSecureOrdersSwitchPostRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **userId** | **string** |  | 
 **piFinancialFundServiceAPIModelsSiriusPlaceSwitchOrderRequest** | [**PiFinancialFundServiceAPIModelsSiriusPlaceSwitchOrderRequest**](PiFinancialFundServiceAPIModelsSiriusPlaceSwitchOrderRequest.md) |  | 

### Return type

[**PiFinancialFundServiceAPIModelsFundOrderPlacedApiResponse**](PiFinancialFundServiceAPIModelsFundOrderPlacedApiResponse.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: application/json, text/json, application/*+json
- **Accept**: text/plain, application/json, text/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)


## TradingOrdersBuyPost

> PiFinancialFundServiceAPIModelsFundOrderPlacedApiResponse TradingOrdersBuyPost(ctx).UserId(userId).PiFinancialFundServiceAPIModelsSiriusPlaceBuyOrderRequest(piFinancialFundServiceAPIModelsSiriusPlaceBuyOrderRequest).Execute()



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
	userId := "38400000-8cf0-11bd-b23e-10b96e4ef00d" // string | 
	piFinancialFundServiceAPIModelsSiriusPlaceBuyOrderRequest := *openapiclient.NewPiFinancialFundServiceAPIModelsSiriusPlaceBuyOrderRequest("Symbol_example", "EffectiveDate_example", "TradingAccountNo_example", float64(123), "PaymentMethod_example", "BankAccount_example", "BankCode_example") // PiFinancialFundServiceAPIModelsSiriusPlaceBuyOrderRequest |  (optional)

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	resp, r, err := apiClient.FundTradingAPI.TradingOrdersBuyPost(context.Background()).UserId(userId).PiFinancialFundServiceAPIModelsSiriusPlaceBuyOrderRequest(piFinancialFundServiceAPIModelsSiriusPlaceBuyOrderRequest).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `FundTradingAPI.TradingOrdersBuyPost``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `TradingOrdersBuyPost`: PiFinancialFundServiceAPIModelsFundOrderPlacedApiResponse
	fmt.Fprintf(os.Stdout, "Response from `FundTradingAPI.TradingOrdersBuyPost`: %v\n", resp)
}
```

### Path Parameters



### Other Parameters

Other parameters are passed through a pointer to a apiTradingOrdersBuyPostRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **userId** | **string** |  | 
 **piFinancialFundServiceAPIModelsSiriusPlaceBuyOrderRequest** | [**PiFinancialFundServiceAPIModelsSiriusPlaceBuyOrderRequest**](PiFinancialFundServiceAPIModelsSiriusPlaceBuyOrderRequest.md) |  | 

### Return type

[**PiFinancialFundServiceAPIModelsFundOrderPlacedApiResponse**](PiFinancialFundServiceAPIModelsFundOrderPlacedApiResponse.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: application/json, text/json, application/*+json
- **Accept**: text/plain, application/json, text/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)


## TradingOrdersSellPost

> PiFinancialFundServiceAPIModelsFundOrderPlacedApiResponse TradingOrdersSellPost(ctx).UserId(userId).PiFinancialFundServiceAPIModelsSiriusPlaceSellOrderRequest(piFinancialFundServiceAPIModelsSiriusPlaceSellOrderRequest).Execute()



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
	userId := "38400000-8cf0-11bd-b23e-10b96e4ef00d" // string | 
	piFinancialFundServiceAPIModelsSiriusPlaceSellOrderRequest := *openapiclient.NewPiFinancialFundServiceAPIModelsSiriusPlaceSellOrderRequest("Symbol_example", "EffectiveDate_example", "TradingAccountNo_example", float64(123), "UnitType_example", "UnitHolderId_example", "BankAccount_example", "BankCode_example") // PiFinancialFundServiceAPIModelsSiriusPlaceSellOrderRequest |  (optional)

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	resp, r, err := apiClient.FundTradingAPI.TradingOrdersSellPost(context.Background()).UserId(userId).PiFinancialFundServiceAPIModelsSiriusPlaceSellOrderRequest(piFinancialFundServiceAPIModelsSiriusPlaceSellOrderRequest).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `FundTradingAPI.TradingOrdersSellPost``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `TradingOrdersSellPost`: PiFinancialFundServiceAPIModelsFundOrderPlacedApiResponse
	fmt.Fprintf(os.Stdout, "Response from `FundTradingAPI.TradingOrdersSellPost`: %v\n", resp)
}
```

### Path Parameters



### Other Parameters

Other parameters are passed through a pointer to a apiTradingOrdersSellPostRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **userId** | **string** |  | 
 **piFinancialFundServiceAPIModelsSiriusPlaceSellOrderRequest** | [**PiFinancialFundServiceAPIModelsSiriusPlaceSellOrderRequest**](PiFinancialFundServiceAPIModelsSiriusPlaceSellOrderRequest.md) |  | 

### Return type

[**PiFinancialFundServiceAPIModelsFundOrderPlacedApiResponse**](PiFinancialFundServiceAPIModelsFundOrderPlacedApiResponse.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: application/json, text/json, application/*+json
- **Accept**: text/plain, application/json, text/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)


## TradingOrdersSwitchPost

> PiFinancialFundServiceAPIModelsFundOrderPlacedApiResponse TradingOrdersSwitchPost(ctx).UserId(userId).PiFinancialFundServiceAPIModelsSiriusPlaceSwitchOrderRequest(piFinancialFundServiceAPIModelsSiriusPlaceSwitchOrderRequest).Execute()



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
	userId := "38400000-8cf0-11bd-b23e-10b96e4ef00d" // string | 
	piFinancialFundServiceAPIModelsSiriusPlaceSwitchOrderRequest := *openapiclient.NewPiFinancialFundServiceAPIModelsSiriusPlaceSwitchOrderRequest("Symbol_example", "EffectiveDate_example", "TradingAccountNo_example", float64(123), "TargetSymbol_example", "UnitHolderId_example", "UnitType_example") // PiFinancialFundServiceAPIModelsSiriusPlaceSwitchOrderRequest |  (optional)

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	resp, r, err := apiClient.FundTradingAPI.TradingOrdersSwitchPost(context.Background()).UserId(userId).PiFinancialFundServiceAPIModelsSiriusPlaceSwitchOrderRequest(piFinancialFundServiceAPIModelsSiriusPlaceSwitchOrderRequest).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `FundTradingAPI.TradingOrdersSwitchPost``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `TradingOrdersSwitchPost`: PiFinancialFundServiceAPIModelsFundOrderPlacedApiResponse
	fmt.Fprintf(os.Stdout, "Response from `FundTradingAPI.TradingOrdersSwitchPost`: %v\n", resp)
}
```

### Path Parameters



### Other Parameters

Other parameters are passed through a pointer to a apiTradingOrdersSwitchPostRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **userId** | **string** |  | 
 **piFinancialFundServiceAPIModelsSiriusPlaceSwitchOrderRequest** | [**PiFinancialFundServiceAPIModelsSiriusPlaceSwitchOrderRequest**](PiFinancialFundServiceAPIModelsSiriusPlaceSwitchOrderRequest.md) |  | 

### Return type

[**PiFinancialFundServiceAPIModelsFundOrderPlacedApiResponse**](PiFinancialFundServiceAPIModelsFundOrderPlacedApiResponse.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: application/json, text/json, application/*+json
- **Accept**: text/plain, application/json, text/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)

