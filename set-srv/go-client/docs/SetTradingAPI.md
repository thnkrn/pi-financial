# \SetTradingAPI

All URIs are relative to *http://localhost*

Method | HTTP request | Description
------------- | ------------- | -------------
[**InternalAccountsBalanceSummaryGet**](SetTradingAPI.md#InternalAccountsBalanceSummaryGet) | **Get** /internal/accounts/balance/summary | 
[**SecureAccountsAvailableBalancePost**](SetTradingAPI.md#SecureAccountsAvailableBalancePost) | **Post** /secure/accounts/available/balance | 
[**SecureAccountsCashBalanceAssetsGet**](SetTradingAPI.md#SecureAccountsCashBalanceAssetsGet) | **Get** /secure/accounts/cash/balance/assets | 
[**SecureAccountsCashBalanceInfoGet**](SetTradingAPI.md#SecureAccountsCashBalanceInfoGet) | **Get** /secure/accounts/cash/balance/info | 
[**SecureAccountsCashBalanceSummaryGet**](SetTradingAPI.md#SecureAccountsCashBalanceSummaryGet) | **Get** /secure/accounts/cash/balance/summary | 
[**SecureAccountsCreditBalanceAssetsGet**](SetTradingAPI.md#SecureAccountsCreditBalanceAssetsGet) | **Get** /secure/accounts/credit/balance/assets | 
[**SecureAccountsCreditBalanceInfoGet**](SetTradingAPI.md#SecureAccountsCreditBalanceInfoGet) | **Get** /secure/accounts/credit/balance/info | 
[**SecureAccountsCreditBalanceSummaryGet**](SetTradingAPI.md#SecureAccountsCreditBalanceSummaryGet) | **Get** /secure/accounts/credit/balance/summary | 
[**SecureAccountsOpenordersGet**](SetTradingAPI.md#SecureAccountsOpenordersGet) | **Get** /secure/accounts/openorders | 
[**SecureAccountsSblAvailableBalancePost**](SetTradingAPI.md#SecureAccountsSblAvailableBalancePost) | **Post** /secure/accounts/sbl/available/balance | 
[**SecureInstrumentsEquityMarginRateGet**](SetTradingAPI.md#SecureInstrumentsEquityMarginRateGet) | **Get** /secure/instruments/equity/margin-rate | 
[**SecureOrdersCancelPost**](SetTradingAPI.md#SecureOrdersCancelPost) | **Post** /secure/orders/cancel | 
[**SecureOrdersChangePatch**](SetTradingAPI.md#SecureOrdersChangePatch) | **Patch** /secure/orders/change | 
[**SecureOrdersHistoriesConfirmGet**](SetTradingAPI.md#SecureOrdersHistoriesConfirmGet) | **Get** /secure/orders/histories/confirm | 
[**SecureOrdersHistoriesOpenGet**](SetTradingAPI.md#SecureOrdersHistoriesOpenGet) | **Get** /secure/orders/histories/open | 
[**SecureOrdersPost**](SetTradingAPI.md#SecureOrdersPost) | **Post** /secure/orders | 
[**TradingOrdersCancelPost**](SetTradingAPI.md#TradingOrdersCancelPost) | **Post** /trading/orders/cancel | 
[**TradingOrdersChangePatch**](SetTradingAPI.md#TradingOrdersChangePatch) | **Patch** /trading/orders/change | 
[**TradingOrdersPost**](SetTradingAPI.md#TradingOrdersPost) | **Post** /trading/orders | 



## InternalAccountsBalanceSummaryGet

> PiSetServiceAPIModelsSetAccountSummaryResponseListApiResponse InternalAccountsBalanceSummaryGet(ctx).UserId(userId).TradingAccountNo(tradingAccountNo).Execute()



### Example

```go
package main

import (
	"context"
	"fmt"
	"os"
	openapiclient "github.com/pi-financial/set-srv/go-client"
)

func main() {
	userId := "38400000-8cf0-11bd-b23e-10b96e4ef00d" // string | 
	tradingAccountNo := "tradingAccountNo_example" // string |  (optional)

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	resp, r, err := apiClient.SetTradingAPI.InternalAccountsBalanceSummaryGet(context.Background()).UserId(userId).TradingAccountNo(tradingAccountNo).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `SetTradingAPI.InternalAccountsBalanceSummaryGet``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `InternalAccountsBalanceSummaryGet`: PiSetServiceAPIModelsSetAccountSummaryResponseListApiResponse
	fmt.Fprintf(os.Stdout, "Response from `SetTradingAPI.InternalAccountsBalanceSummaryGet`: %v\n", resp)
}
```

### Path Parameters



### Other Parameters

Other parameters are passed through a pointer to a apiInternalAccountsBalanceSummaryGetRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **userId** | **string** |  | 
 **tradingAccountNo** | **string** |  | 

### Return type

[**PiSetServiceAPIModelsSetAccountSummaryResponseListApiResponse**](PiSetServiceAPIModelsSetAccountSummaryResponseListApiResponse.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: text/plain, application/json, text/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)


## SecureAccountsAvailableBalancePost

> PiSetServiceAPIModelsAccountInstrumentAvailableBalanceResponseApiResponse SecureAccountsAvailableBalancePost(ctx).UserId(userId).PiSetServiceAPIModelsAccountInstrumentAvailableBalanceRequest(piSetServiceAPIModelsAccountInstrumentAvailableBalanceRequest).Execute()



### Example

```go
package main

import (
	"context"
	"fmt"
	"os"
	openapiclient "github.com/pi-financial/set-srv/go-client"
)

func main() {
	userId := "38400000-8cf0-11bd-b23e-10b96e4ef00d" // string | 
	piSetServiceAPIModelsAccountInstrumentAvailableBalanceRequest := *openapiclient.NewPiSetServiceAPIModelsAccountInstrumentAvailableBalanceRequest("Symbol_example", "TradingAccountNo_example") // PiSetServiceAPIModelsAccountInstrumentAvailableBalanceRequest | 

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	resp, r, err := apiClient.SetTradingAPI.SecureAccountsAvailableBalancePost(context.Background()).UserId(userId).PiSetServiceAPIModelsAccountInstrumentAvailableBalanceRequest(piSetServiceAPIModelsAccountInstrumentAvailableBalanceRequest).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `SetTradingAPI.SecureAccountsAvailableBalancePost``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `SecureAccountsAvailableBalancePost`: PiSetServiceAPIModelsAccountInstrumentAvailableBalanceResponseApiResponse
	fmt.Fprintf(os.Stdout, "Response from `SetTradingAPI.SecureAccountsAvailableBalancePost`: %v\n", resp)
}
```

### Path Parameters



### Other Parameters

Other parameters are passed through a pointer to a apiSecureAccountsAvailableBalancePostRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **userId** | **string** |  | 
 **piSetServiceAPIModelsAccountInstrumentAvailableBalanceRequest** | [**PiSetServiceAPIModelsAccountInstrumentAvailableBalanceRequest**](PiSetServiceAPIModelsAccountInstrumentAvailableBalanceRequest.md) |  | 

### Return type

[**PiSetServiceAPIModelsAccountInstrumentAvailableBalanceResponseApiResponse**](PiSetServiceAPIModelsAccountInstrumentAvailableBalanceResponseApiResponse.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: application/json, text/json, application/*+json
- **Accept**: text/plain, application/json, text/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)


## SecureAccountsCashBalanceAssetsGet

> PiSetServiceAPIModelsSetAccountAssetsResponseListApiResponse SecureAccountsCashBalanceAssetsGet(ctx).UserId(userId).TradingAccountNo(tradingAccountNo).Execute()



### Example

```go
package main

import (
	"context"
	"fmt"
	"os"
	openapiclient "github.com/pi-financial/set-srv/go-client"
)

func main() {
	userId := "38400000-8cf0-11bd-b23e-10b96e4ef00d" // string | 
	tradingAccountNo := "tradingAccountNo_example" // string | 

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	resp, r, err := apiClient.SetTradingAPI.SecureAccountsCashBalanceAssetsGet(context.Background()).UserId(userId).TradingAccountNo(tradingAccountNo).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `SetTradingAPI.SecureAccountsCashBalanceAssetsGet``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `SecureAccountsCashBalanceAssetsGet`: PiSetServiceAPIModelsSetAccountAssetsResponseListApiResponse
	fmt.Fprintf(os.Stdout, "Response from `SetTradingAPI.SecureAccountsCashBalanceAssetsGet`: %v\n", resp)
}
```

### Path Parameters



### Other Parameters

Other parameters are passed through a pointer to a apiSecureAccountsCashBalanceAssetsGetRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **userId** | **string** |  | 
 **tradingAccountNo** | **string** |  | 

### Return type

[**PiSetServiceAPIModelsSetAccountAssetsResponseListApiResponse**](PiSetServiceAPIModelsSetAccountAssetsResponseListApiResponse.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: text/plain, application/json, text/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)


## SecureAccountsCashBalanceInfoGet

> PiSetServiceAPIModelsSetAccountCashBalanceInfoResponseApiResponse SecureAccountsCashBalanceInfoGet(ctx).UserId(userId).TradingAccountNo(tradingAccountNo).Execute()



### Example

```go
package main

import (
	"context"
	"fmt"
	"os"
	openapiclient "github.com/pi-financial/set-srv/go-client"
)

func main() {
	userId := "38400000-8cf0-11bd-b23e-10b96e4ef00d" // string | 
	tradingAccountNo := "tradingAccountNo_example" // string | 

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	resp, r, err := apiClient.SetTradingAPI.SecureAccountsCashBalanceInfoGet(context.Background()).UserId(userId).TradingAccountNo(tradingAccountNo).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `SetTradingAPI.SecureAccountsCashBalanceInfoGet``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `SecureAccountsCashBalanceInfoGet`: PiSetServiceAPIModelsSetAccountCashBalanceInfoResponseApiResponse
	fmt.Fprintf(os.Stdout, "Response from `SetTradingAPI.SecureAccountsCashBalanceInfoGet`: %v\n", resp)
}
```

### Path Parameters



### Other Parameters

Other parameters are passed through a pointer to a apiSecureAccountsCashBalanceInfoGetRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **userId** | **string** |  | 
 **tradingAccountNo** | **string** |  | 

### Return type

[**PiSetServiceAPIModelsSetAccountCashBalanceInfoResponseApiResponse**](PiSetServiceAPIModelsSetAccountCashBalanceInfoResponseApiResponse.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: text/plain, application/json, text/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)


## SecureAccountsCashBalanceSummaryGet

> PiSetServiceAPIModelsSetAccountSummaryResponseApiResponse SecureAccountsCashBalanceSummaryGet(ctx).UserId(userId).TradingAccountNo(tradingAccountNo).Execute()



### Example

```go
package main

import (
	"context"
	"fmt"
	"os"
	openapiclient "github.com/pi-financial/set-srv/go-client"
)

func main() {
	userId := "38400000-8cf0-11bd-b23e-10b96e4ef00d" // string | 
	tradingAccountNo := "tradingAccountNo_example" // string | 

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	resp, r, err := apiClient.SetTradingAPI.SecureAccountsCashBalanceSummaryGet(context.Background()).UserId(userId).TradingAccountNo(tradingAccountNo).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `SetTradingAPI.SecureAccountsCashBalanceSummaryGet``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `SecureAccountsCashBalanceSummaryGet`: PiSetServiceAPIModelsSetAccountSummaryResponseApiResponse
	fmt.Fprintf(os.Stdout, "Response from `SetTradingAPI.SecureAccountsCashBalanceSummaryGet`: %v\n", resp)
}
```

### Path Parameters



### Other Parameters

Other parameters are passed through a pointer to a apiSecureAccountsCashBalanceSummaryGetRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **userId** | **string** |  | 
 **tradingAccountNo** | **string** |  | 

### Return type

[**PiSetServiceAPIModelsSetAccountSummaryResponseApiResponse**](PiSetServiceAPIModelsSetAccountSummaryResponseApiResponse.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: text/plain, application/json, text/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)


## SecureAccountsCreditBalanceAssetsGet

> PiSetServiceAPIModelsSetAccountAssetsResponseListApiResponse SecureAccountsCreditBalanceAssetsGet(ctx).UserId(userId).TradingAccountNo(tradingAccountNo).Execute()



### Example

```go
package main

import (
	"context"
	"fmt"
	"os"
	openapiclient "github.com/pi-financial/set-srv/go-client"
)

func main() {
	userId := "38400000-8cf0-11bd-b23e-10b96e4ef00d" // string | 
	tradingAccountNo := "tradingAccountNo_example" // string | 

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	resp, r, err := apiClient.SetTradingAPI.SecureAccountsCreditBalanceAssetsGet(context.Background()).UserId(userId).TradingAccountNo(tradingAccountNo).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `SetTradingAPI.SecureAccountsCreditBalanceAssetsGet``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `SecureAccountsCreditBalanceAssetsGet`: PiSetServiceAPIModelsSetAccountAssetsResponseListApiResponse
	fmt.Fprintf(os.Stdout, "Response from `SetTradingAPI.SecureAccountsCreditBalanceAssetsGet`: %v\n", resp)
}
```

### Path Parameters



### Other Parameters

Other parameters are passed through a pointer to a apiSecureAccountsCreditBalanceAssetsGetRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **userId** | **string** |  | 
 **tradingAccountNo** | **string** |  | 

### Return type

[**PiSetServiceAPIModelsSetAccountAssetsResponseListApiResponse**](PiSetServiceAPIModelsSetAccountAssetsResponseListApiResponse.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: text/plain, application/json, text/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)


## SecureAccountsCreditBalanceInfoGet

> PiSetServiceAPIModelsSetAccountCreditBalanceInfoResponseApiResponse SecureAccountsCreditBalanceInfoGet(ctx).UserId(userId).TradingAccountNo(tradingAccountNo).Execute()



### Example

```go
package main

import (
	"context"
	"fmt"
	"os"
	openapiclient "github.com/pi-financial/set-srv/go-client"
)

func main() {
	userId := "38400000-8cf0-11bd-b23e-10b96e4ef00d" // string | 
	tradingAccountNo := "tradingAccountNo_example" // string | 

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	resp, r, err := apiClient.SetTradingAPI.SecureAccountsCreditBalanceInfoGet(context.Background()).UserId(userId).TradingAccountNo(tradingAccountNo).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `SetTradingAPI.SecureAccountsCreditBalanceInfoGet``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `SecureAccountsCreditBalanceInfoGet`: PiSetServiceAPIModelsSetAccountCreditBalanceInfoResponseApiResponse
	fmt.Fprintf(os.Stdout, "Response from `SetTradingAPI.SecureAccountsCreditBalanceInfoGet`: %v\n", resp)
}
```

### Path Parameters



### Other Parameters

Other parameters are passed through a pointer to a apiSecureAccountsCreditBalanceInfoGetRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **userId** | **string** |  | 
 **tradingAccountNo** | **string** |  | 

### Return type

[**PiSetServiceAPIModelsSetAccountCreditBalanceInfoResponseApiResponse**](PiSetServiceAPIModelsSetAccountCreditBalanceInfoResponseApiResponse.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: text/plain, application/json, text/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)


## SecureAccountsCreditBalanceSummaryGet

> PiSetServiceAPIModelsSetCreditBalanceAccountSummaryResponseApiResponse SecureAccountsCreditBalanceSummaryGet(ctx).UserId(userId).TradingAccountNo(tradingAccountNo).Execute()



### Example

```go
package main

import (
	"context"
	"fmt"
	"os"
	openapiclient "github.com/pi-financial/set-srv/go-client"
)

func main() {
	userId := "38400000-8cf0-11bd-b23e-10b96e4ef00d" // string | 
	tradingAccountNo := "tradingAccountNo_example" // string | 

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	resp, r, err := apiClient.SetTradingAPI.SecureAccountsCreditBalanceSummaryGet(context.Background()).UserId(userId).TradingAccountNo(tradingAccountNo).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `SetTradingAPI.SecureAccountsCreditBalanceSummaryGet``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `SecureAccountsCreditBalanceSummaryGet`: PiSetServiceAPIModelsSetCreditBalanceAccountSummaryResponseApiResponse
	fmt.Fprintf(os.Stdout, "Response from `SetTradingAPI.SecureAccountsCreditBalanceSummaryGet`: %v\n", resp)
}
```

### Path Parameters



### Other Parameters

Other parameters are passed through a pointer to a apiSecureAccountsCreditBalanceSummaryGetRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **userId** | **string** |  | 
 **tradingAccountNo** | **string** |  | 

### Return type

[**PiSetServiceAPIModelsSetCreditBalanceAccountSummaryResponseApiResponse**](PiSetServiceAPIModelsSetCreditBalanceAccountSummaryResponseApiResponse.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: text/plain, application/json, text/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)


## SecureAccountsOpenordersGet

> PiSetServiceAPIModelsSetOpenOrderResponseListApiResponse SecureAccountsOpenordersGet(ctx).UserId(userId).TradingAccountNo(tradingAccountNo).Execute()



### Example

```go
package main

import (
	"context"
	"fmt"
	"os"
	openapiclient "github.com/pi-financial/set-srv/go-client"
)

func main() {
	userId := "38400000-8cf0-11bd-b23e-10b96e4ef00d" // string | 
	tradingAccountNo := "tradingAccountNo_example" // string | 

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	resp, r, err := apiClient.SetTradingAPI.SecureAccountsOpenordersGet(context.Background()).UserId(userId).TradingAccountNo(tradingAccountNo).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `SetTradingAPI.SecureAccountsOpenordersGet``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `SecureAccountsOpenordersGet`: PiSetServiceAPIModelsSetOpenOrderResponseListApiResponse
	fmt.Fprintf(os.Stdout, "Response from `SetTradingAPI.SecureAccountsOpenordersGet`: %v\n", resp)
}
```

### Path Parameters



### Other Parameters

Other parameters are passed through a pointer to a apiSecureAccountsOpenordersGetRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **userId** | **string** |  | 
 **tradingAccountNo** | **string** |  | 

### Return type

[**PiSetServiceAPIModelsSetOpenOrderResponseListApiResponse**](PiSetServiceAPIModelsSetOpenOrderResponseListApiResponse.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: text/plain, application/json, text/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)


## SecureAccountsSblAvailableBalancePost

> PiSetServiceAPIModelsAccountSblInstrumentAvailableBalanceResponseApiResponse SecureAccountsSblAvailableBalancePost(ctx).UserId(userId).PiSetServiceAPIModelsAccountInstrumentAvailableBalanceRequest(piSetServiceAPIModelsAccountInstrumentAvailableBalanceRequest).Execute()



### Example

```go
package main

import (
	"context"
	"fmt"
	"os"
	openapiclient "github.com/pi-financial/set-srv/go-client"
)

func main() {
	userId := "38400000-8cf0-11bd-b23e-10b96e4ef00d" // string | 
	piSetServiceAPIModelsAccountInstrumentAvailableBalanceRequest := *openapiclient.NewPiSetServiceAPIModelsAccountInstrumentAvailableBalanceRequest("Symbol_example", "TradingAccountNo_example") // PiSetServiceAPIModelsAccountInstrumentAvailableBalanceRequest | 

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	resp, r, err := apiClient.SetTradingAPI.SecureAccountsSblAvailableBalancePost(context.Background()).UserId(userId).PiSetServiceAPIModelsAccountInstrumentAvailableBalanceRequest(piSetServiceAPIModelsAccountInstrumentAvailableBalanceRequest).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `SetTradingAPI.SecureAccountsSblAvailableBalancePost``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `SecureAccountsSblAvailableBalancePost`: PiSetServiceAPIModelsAccountSblInstrumentAvailableBalanceResponseApiResponse
	fmt.Fprintf(os.Stdout, "Response from `SetTradingAPI.SecureAccountsSblAvailableBalancePost`: %v\n", resp)
}
```

### Path Parameters



### Other Parameters

Other parameters are passed through a pointer to a apiSecureAccountsSblAvailableBalancePostRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **userId** | **string** |  | 
 **piSetServiceAPIModelsAccountInstrumentAvailableBalanceRequest** | [**PiSetServiceAPIModelsAccountInstrumentAvailableBalanceRequest**](PiSetServiceAPIModelsAccountInstrumentAvailableBalanceRequest.md) |  | 

### Return type

[**PiSetServiceAPIModelsAccountSblInstrumentAvailableBalanceResponseApiResponse**](PiSetServiceAPIModelsAccountSblInstrumentAvailableBalanceResponseApiResponse.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: application/json, text/json, application/*+json
- **Accept**: text/plain, application/json, text/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)


## SecureInstrumentsEquityMarginRateGet

> PiSetServiceAPIModelsMarginRateResponseApiResponse SecureInstrumentsEquityMarginRateGet(ctx).Symbol(symbol).Execute()



### Example

```go
package main

import (
	"context"
	"fmt"
	"os"
	openapiclient "github.com/pi-financial/set-srv/go-client"
)

func main() {
	symbol := "symbol_example" // string | 

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	resp, r, err := apiClient.SetTradingAPI.SecureInstrumentsEquityMarginRateGet(context.Background()).Symbol(symbol).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `SetTradingAPI.SecureInstrumentsEquityMarginRateGet``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `SecureInstrumentsEquityMarginRateGet`: PiSetServiceAPIModelsMarginRateResponseApiResponse
	fmt.Fprintf(os.Stdout, "Response from `SetTradingAPI.SecureInstrumentsEquityMarginRateGet`: %v\n", resp)
}
```

### Path Parameters



### Other Parameters

Other parameters are passed through a pointer to a apiSecureInstrumentsEquityMarginRateGetRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **symbol** | **string** |  | 

### Return type

[**PiSetServiceAPIModelsMarginRateResponseApiResponse**](PiSetServiceAPIModelsMarginRateResponseApiResponse.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: text/plain, application/json, text/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)


## SecureOrdersCancelPost

> PiSetServiceAPIModelsSetOrderApiResponse SecureOrdersCancelPost(ctx).UserId(userId).PiSetServiceAPIModelsCancelOrderRequest(piSetServiceAPIModelsCancelOrderRequest).Execute()



### Example

```go
package main

import (
	"context"
	"fmt"
	"os"
	openapiclient "github.com/pi-financial/set-srv/go-client"
)

func main() {
	userId := "38400000-8cf0-11bd-b23e-10b96e4ef00d" // string | 
	piSetServiceAPIModelsCancelOrderRequest := *openapiclient.NewPiSetServiceAPIModelsCancelOrderRequest("TradingAccountNo_example", "OrderId_example") // PiSetServiceAPIModelsCancelOrderRequest |  (optional)

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	resp, r, err := apiClient.SetTradingAPI.SecureOrdersCancelPost(context.Background()).UserId(userId).PiSetServiceAPIModelsCancelOrderRequest(piSetServiceAPIModelsCancelOrderRequest).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `SetTradingAPI.SecureOrdersCancelPost``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `SecureOrdersCancelPost`: PiSetServiceAPIModelsSetOrderApiResponse
	fmt.Fprintf(os.Stdout, "Response from `SetTradingAPI.SecureOrdersCancelPost`: %v\n", resp)
}
```

### Path Parameters



### Other Parameters

Other parameters are passed through a pointer to a apiSecureOrdersCancelPostRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **userId** | **string** |  | 
 **piSetServiceAPIModelsCancelOrderRequest** | [**PiSetServiceAPIModelsCancelOrderRequest**](PiSetServiceAPIModelsCancelOrderRequest.md) |  | 

### Return type

[**PiSetServiceAPIModelsSetOrderApiResponse**](PiSetServiceAPIModelsSetOrderApiResponse.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: application/json, text/json, application/*+json
- **Accept**: text/plain, application/json, text/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)


## SecureOrdersChangePatch

> PiSetServiceAPIModelsSetOrderApiResponse SecureOrdersChangePatch(ctx).UserId(userId).PiSetServiceAPIModelsChangeOrderRequest(piSetServiceAPIModelsChangeOrderRequest).Execute()



### Example

```go
package main

import (
	"context"
	"fmt"
	"os"
	openapiclient "github.com/pi-financial/set-srv/go-client"
)

func main() {
	userId := "38400000-8cf0-11bd-b23e-10b96e4ef00d" // string | 
	piSetServiceAPIModelsChangeOrderRequest := *openapiclient.NewPiSetServiceAPIModelsChangeOrderRequest("TradingAccountNo_example", "OrderId_example", "1112.1314", "123") // PiSetServiceAPIModelsChangeOrderRequest |  (optional)

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	resp, r, err := apiClient.SetTradingAPI.SecureOrdersChangePatch(context.Background()).UserId(userId).PiSetServiceAPIModelsChangeOrderRequest(piSetServiceAPIModelsChangeOrderRequest).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `SetTradingAPI.SecureOrdersChangePatch``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `SecureOrdersChangePatch`: PiSetServiceAPIModelsSetOrderApiResponse
	fmt.Fprintf(os.Stdout, "Response from `SetTradingAPI.SecureOrdersChangePatch`: %v\n", resp)
}
```

### Path Parameters



### Other Parameters

Other parameters are passed through a pointer to a apiSecureOrdersChangePatchRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **userId** | **string** |  | 
 **piSetServiceAPIModelsChangeOrderRequest** | [**PiSetServiceAPIModelsChangeOrderRequest**](PiSetServiceAPIModelsChangeOrderRequest.md) |  | 

### Return type

[**PiSetServiceAPIModelsSetOrderApiResponse**](PiSetServiceAPIModelsSetOrderApiResponse.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: application/json, text/json, application/*+json
- **Accept**: text/plain, application/json, text/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)


## SecureOrdersHistoriesConfirmGet

> PiSetServiceAPIModelsSetTradeHistoryResponseListApiResponse SecureOrdersHistoriesConfirmGet(ctx).UserId(userId).TradingAccountNo(tradingAccountNo).StartDate(startDate).EndDate(endDate).Limit(limit).OffSet(offSet).Execute()



### Example

```go
package main

import (
	"context"
	"fmt"
	"os"
	openapiclient "github.com/pi-financial/set-srv/go-client"
)

func main() {
	userId := "38400000-8cf0-11bd-b23e-10b96e4ef00d" // string | 
	tradingAccountNo := "tradingAccountNo_example" // string | 
	startDate := "startDate_example" // string |  (optional)
	endDate := "endDate_example" // string |  (optional)
	limit := "limit_example" // string |  (optional)
	offSet := "offSet_example" // string |  (optional)

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	resp, r, err := apiClient.SetTradingAPI.SecureOrdersHistoriesConfirmGet(context.Background()).UserId(userId).TradingAccountNo(tradingAccountNo).StartDate(startDate).EndDate(endDate).Limit(limit).OffSet(offSet).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `SetTradingAPI.SecureOrdersHistoriesConfirmGet``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `SecureOrdersHistoriesConfirmGet`: PiSetServiceAPIModelsSetTradeHistoryResponseListApiResponse
	fmt.Fprintf(os.Stdout, "Response from `SetTradingAPI.SecureOrdersHistoriesConfirmGet`: %v\n", resp)
}
```

### Path Parameters



### Other Parameters

Other parameters are passed through a pointer to a apiSecureOrdersHistoriesConfirmGetRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **userId** | **string** |  | 
 **tradingAccountNo** | **string** |  | 
 **startDate** | **string** |  | 
 **endDate** | **string** |  | 
 **limit** | **string** |  | 
 **offSet** | **string** |  | 

### Return type

[**PiSetServiceAPIModelsSetTradeHistoryResponseListApiResponse**](PiSetServiceAPIModelsSetTradeHistoryResponseListApiResponse.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: text/plain, application/json, text/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)


## SecureOrdersHistoriesOpenGet

> PiSetServiceAPIModelsSetOrderHistoryResponseListApiResponse SecureOrdersHistoriesOpenGet(ctx).UserId(userId).TradingAccountNo(tradingAccountNo).StartDate(startDate).EndDate(endDate).Limit(limit).OffSet(offSet).Execute()



### Example

```go
package main

import (
	"context"
	"fmt"
	"os"
	openapiclient "github.com/pi-financial/set-srv/go-client"
)

func main() {
	userId := "38400000-8cf0-11bd-b23e-10b96e4ef00d" // string | 
	tradingAccountNo := "tradingAccountNo_example" // string | 
	startDate := "startDate_example" // string |  (optional)
	endDate := "endDate_example" // string |  (optional)
	limit := "limit_example" // string |  (optional)
	offSet := "offSet_example" // string |  (optional)

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	resp, r, err := apiClient.SetTradingAPI.SecureOrdersHistoriesOpenGet(context.Background()).UserId(userId).TradingAccountNo(tradingAccountNo).StartDate(startDate).EndDate(endDate).Limit(limit).OffSet(offSet).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `SetTradingAPI.SecureOrdersHistoriesOpenGet``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `SecureOrdersHistoriesOpenGet`: PiSetServiceAPIModelsSetOrderHistoryResponseListApiResponse
	fmt.Fprintf(os.Stdout, "Response from `SetTradingAPI.SecureOrdersHistoriesOpenGet`: %v\n", resp)
}
```

### Path Parameters



### Other Parameters

Other parameters are passed through a pointer to a apiSecureOrdersHistoriesOpenGetRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **userId** | **string** |  | 
 **tradingAccountNo** | **string** |  | 
 **startDate** | **string** |  | 
 **endDate** | **string** |  | 
 **limit** | **string** |  | 
 **offSet** | **string** |  | 

### Return type

[**PiSetServiceAPIModelsSetOrderHistoryResponseListApiResponse**](PiSetServiceAPIModelsSetOrderHistoryResponseListApiResponse.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: text/plain, application/json, text/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)


## SecureOrdersPost

> PiSetServiceAPIModelsSetOrderApiResponse SecureOrdersPost(ctx).UserId(userId).PiSetServiceAPIModelsPlaceOrderRequest(piSetServiceAPIModelsPlaceOrderRequest).Execute()



### Example

```go
package main

import (
	"context"
	"fmt"
	"os"
	openapiclient "github.com/pi-financial/set-srv/go-client"
)

func main() {
	userId := "38400000-8cf0-11bd-b23e-10b96e4ef00d" // string | 
	piSetServiceAPIModelsPlaceOrderRequest := *openapiclient.NewPiSetServiceAPIModelsPlaceOrderRequest("TradingAccountNo_example", openapiclient.PiSetServiceDomainAggregatesModelFinancialAssetAggregateConditionPrice("Limit"), openapiclient.PiSetServiceDomainAggregatesModelFinancialAssetAggregateOrderAction("cover"), "Symbol_example", "123") // PiSetServiceAPIModelsPlaceOrderRequest |  (optional)

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	resp, r, err := apiClient.SetTradingAPI.SecureOrdersPost(context.Background()).UserId(userId).PiSetServiceAPIModelsPlaceOrderRequest(piSetServiceAPIModelsPlaceOrderRequest).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `SetTradingAPI.SecureOrdersPost``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `SecureOrdersPost`: PiSetServiceAPIModelsSetOrderApiResponse
	fmt.Fprintf(os.Stdout, "Response from `SetTradingAPI.SecureOrdersPost`: %v\n", resp)
}
```

### Path Parameters



### Other Parameters

Other parameters are passed through a pointer to a apiSecureOrdersPostRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **userId** | **string** |  | 
 **piSetServiceAPIModelsPlaceOrderRequest** | [**PiSetServiceAPIModelsPlaceOrderRequest**](PiSetServiceAPIModelsPlaceOrderRequest.md) |  | 

### Return type

[**PiSetServiceAPIModelsSetOrderApiResponse**](PiSetServiceAPIModelsSetOrderApiResponse.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: application/json, text/json, application/*+json
- **Accept**: text/plain, application/json, text/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)


## TradingOrdersCancelPost

> PiSetServiceAPIModelsSetOrderApiResponse TradingOrdersCancelPost(ctx).UserId(userId).PiSetServiceAPIModelsCancelOrderRequest(piSetServiceAPIModelsCancelOrderRequest).Execute()



### Example

```go
package main

import (
	"context"
	"fmt"
	"os"
	openapiclient "github.com/pi-financial/set-srv/go-client"
)

func main() {
	userId := "38400000-8cf0-11bd-b23e-10b96e4ef00d" // string | 
	piSetServiceAPIModelsCancelOrderRequest := *openapiclient.NewPiSetServiceAPIModelsCancelOrderRequest("TradingAccountNo_example", "OrderId_example") // PiSetServiceAPIModelsCancelOrderRequest |  (optional)

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	resp, r, err := apiClient.SetTradingAPI.TradingOrdersCancelPost(context.Background()).UserId(userId).PiSetServiceAPIModelsCancelOrderRequest(piSetServiceAPIModelsCancelOrderRequest).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `SetTradingAPI.TradingOrdersCancelPost``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `TradingOrdersCancelPost`: PiSetServiceAPIModelsSetOrderApiResponse
	fmt.Fprintf(os.Stdout, "Response from `SetTradingAPI.TradingOrdersCancelPost`: %v\n", resp)
}
```

### Path Parameters



### Other Parameters

Other parameters are passed through a pointer to a apiTradingOrdersCancelPostRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **userId** | **string** |  | 
 **piSetServiceAPIModelsCancelOrderRequest** | [**PiSetServiceAPIModelsCancelOrderRequest**](PiSetServiceAPIModelsCancelOrderRequest.md) |  | 

### Return type

[**PiSetServiceAPIModelsSetOrderApiResponse**](PiSetServiceAPIModelsSetOrderApiResponse.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: application/json, text/json, application/*+json
- **Accept**: text/plain, application/json, text/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)


## TradingOrdersChangePatch

> PiSetServiceAPIModelsSetOrderApiResponse TradingOrdersChangePatch(ctx).UserId(userId).PiSetServiceAPIModelsChangeOrderRequest(piSetServiceAPIModelsChangeOrderRequest).Execute()



### Example

```go
package main

import (
	"context"
	"fmt"
	"os"
	openapiclient "github.com/pi-financial/set-srv/go-client"
)

func main() {
	userId := "38400000-8cf0-11bd-b23e-10b96e4ef00d" // string | 
	piSetServiceAPIModelsChangeOrderRequest := *openapiclient.NewPiSetServiceAPIModelsChangeOrderRequest("TradingAccountNo_example", "OrderId_example", "1112.1314", "123") // PiSetServiceAPIModelsChangeOrderRequest |  (optional)

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	resp, r, err := apiClient.SetTradingAPI.TradingOrdersChangePatch(context.Background()).UserId(userId).PiSetServiceAPIModelsChangeOrderRequest(piSetServiceAPIModelsChangeOrderRequest).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `SetTradingAPI.TradingOrdersChangePatch``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `TradingOrdersChangePatch`: PiSetServiceAPIModelsSetOrderApiResponse
	fmt.Fprintf(os.Stdout, "Response from `SetTradingAPI.TradingOrdersChangePatch`: %v\n", resp)
}
```

### Path Parameters



### Other Parameters

Other parameters are passed through a pointer to a apiTradingOrdersChangePatchRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **userId** | **string** |  | 
 **piSetServiceAPIModelsChangeOrderRequest** | [**PiSetServiceAPIModelsChangeOrderRequest**](PiSetServiceAPIModelsChangeOrderRequest.md) |  | 

### Return type

[**PiSetServiceAPIModelsSetOrderApiResponse**](PiSetServiceAPIModelsSetOrderApiResponse.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: application/json, text/json, application/*+json
- **Accept**: text/plain, application/json, text/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)


## TradingOrdersPost

> PiSetServiceAPIModelsSetOrderApiResponse TradingOrdersPost(ctx).UserId(userId).PiSetServiceAPIModelsPlaceOrderRequest(piSetServiceAPIModelsPlaceOrderRequest).Execute()



### Example

```go
package main

import (
	"context"
	"fmt"
	"os"
	openapiclient "github.com/pi-financial/set-srv/go-client"
)

func main() {
	userId := "38400000-8cf0-11bd-b23e-10b96e4ef00d" // string | 
	piSetServiceAPIModelsPlaceOrderRequest := *openapiclient.NewPiSetServiceAPIModelsPlaceOrderRequest("TradingAccountNo_example", openapiclient.PiSetServiceDomainAggregatesModelFinancialAssetAggregateConditionPrice("Limit"), openapiclient.PiSetServiceDomainAggregatesModelFinancialAssetAggregateOrderAction("cover"), "Symbol_example", "123") // PiSetServiceAPIModelsPlaceOrderRequest |  (optional)

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	resp, r, err := apiClient.SetTradingAPI.TradingOrdersPost(context.Background()).UserId(userId).PiSetServiceAPIModelsPlaceOrderRequest(piSetServiceAPIModelsPlaceOrderRequest).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `SetTradingAPI.TradingOrdersPost``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `TradingOrdersPost`: PiSetServiceAPIModelsSetOrderApiResponse
	fmt.Fprintf(os.Stdout, "Response from `SetTradingAPI.TradingOrdersPost`: %v\n", resp)
}
```

### Path Parameters



### Other Parameters

Other parameters are passed through a pointer to a apiTradingOrdersPostRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **userId** | **string** |  | 
 **piSetServiceAPIModelsPlaceOrderRequest** | [**PiSetServiceAPIModelsPlaceOrderRequest**](PiSetServiceAPIModelsPlaceOrderRequest.md) |  | 

### Return type

[**PiSetServiceAPIModelsSetOrderApiResponse**](PiSetServiceAPIModelsSetOrderApiResponse.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: application/json, text/json, application/*+json
- **Accept**: text/plain, application/json, text/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)

