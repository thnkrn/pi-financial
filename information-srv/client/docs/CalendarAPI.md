# \CalendarAPI

All URIs are relative to *http://localhost*

Method | HTTP request | Description
------------- | ------------- | -------------
[**InternalCalendarHolidaysGet**](CalendarAPI.md#InternalCalendarHolidaysGet) | **Get** /internal/calendar/holidays | Get Holidays
[**InternalCalendarHolidaysYearGet**](CalendarAPI.md#InternalCalendarHolidaysYearGet) | **Get** /internal/calendar/holidays/{year} | Get Holidays
[**InternalCalendarIsHolidayDateGet**](CalendarAPI.md#InternalCalendarIsHolidayDateGet) | **Get** /internal/calendar/is-holiday/{date} | Check if Date is a Holiday
[**InternalCalendarNextBusinessDayDateGet**](CalendarAPI.md#InternalCalendarNextBusinessDayDateGet) | **Get** /internal/calendar/next-business-day/{date} | Get Next Business Day
[**SecureCalendarHolidaysGet**](CalendarAPI.md#SecureCalendarHolidaysGet) | **Get** /secure/calendar/holidays | Get Holidays
[**SecureCalendarHolidaysYearGet**](CalendarAPI.md#SecureCalendarHolidaysYearGet) | **Get** /secure/calendar/holidays/{year} | Get Holidays
[**SecureCalendarIsHolidayDateGet**](CalendarAPI.md#SecureCalendarIsHolidayDateGet) | **Get** /secure/calendar/is-holiday/{date} | Check if Date is a Holiday
[**SecureCalendarNextBusinessDayDateGet**](CalendarAPI.md#SecureCalendarNextBusinessDayDateGet) | **Get** /secure/calendar/next-business-day/{date} | Get Next Business Day



## InternalCalendarHolidaysGet

> InternalCalendarHolidaysGet200Response InternalCalendarHolidaysGet(ctx).Execute()

Get Holidays



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
	resp, r, err := apiClient.CalendarAPI.InternalCalendarHolidaysGet(context.Background()).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `CalendarAPI.InternalCalendarHolidaysGet``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `InternalCalendarHolidaysGet`: InternalCalendarHolidaysGet200Response
	fmt.Fprintf(os.Stdout, "Response from `CalendarAPI.InternalCalendarHolidaysGet`: %v\n", resp)
}
```

### Path Parameters

This endpoint does not need any parameter.

### Other Parameters

Other parameters are passed through a pointer to a apiInternalCalendarHolidaysGetRequest struct via the builder pattern


### Return type

[**InternalCalendarHolidaysGet200Response**](InternalCalendarHolidaysGet200Response.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)


## InternalCalendarHolidaysYearGet

> InternalCalendarHolidaysGet200Response InternalCalendarHolidaysYearGet(ctx, year).Execute()

Get Holidays



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
	year := int32(56) // int32 | The year to retrieve holidays

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	resp, r, err := apiClient.CalendarAPI.InternalCalendarHolidaysYearGet(context.Background(), year).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `CalendarAPI.InternalCalendarHolidaysYearGet``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `InternalCalendarHolidaysYearGet`: InternalCalendarHolidaysGet200Response
	fmt.Fprintf(os.Stdout, "Response from `CalendarAPI.InternalCalendarHolidaysYearGet`: %v\n", resp)
}
```

### Path Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
**ctx** | **context.Context** | context for authentication, logging, cancellation, deadlines, tracing, etc.
**year** | **int32** | The year to retrieve holidays | 

### Other Parameters

Other parameters are passed through a pointer to a apiInternalCalendarHolidaysYearGetRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------


### Return type

[**InternalCalendarHolidaysGet200Response**](InternalCalendarHolidaysGet200Response.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)


## InternalCalendarIsHolidayDateGet

> InternalCalendarIsHolidayDateGet200Response InternalCalendarIsHolidayDateGet(ctx, date).Execute()

Check if Date is a Holiday



### Example

```go
package main

import (
	"context"
	"fmt"
	"os"
    "time"
	openapiclient "github.com/pi-financial/information-srv/client"
)

func main() {
	date := time.Now() // string | The date to check if it is a holiday (format: YYYY-MM-DD)

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	resp, r, err := apiClient.CalendarAPI.InternalCalendarIsHolidayDateGet(context.Background(), date).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `CalendarAPI.InternalCalendarIsHolidayDateGet``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `InternalCalendarIsHolidayDateGet`: InternalCalendarIsHolidayDateGet200Response
	fmt.Fprintf(os.Stdout, "Response from `CalendarAPI.InternalCalendarIsHolidayDateGet`: %v\n", resp)
}
```

### Path Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
**ctx** | **context.Context** | context for authentication, logging, cancellation, deadlines, tracing, etc.
**date** | **string** | The date to check if it is a holiday (format: YYYY-MM-DD) | 

### Other Parameters

Other parameters are passed through a pointer to a apiInternalCalendarIsHolidayDateGetRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------


### Return type

[**InternalCalendarIsHolidayDateGet200Response**](InternalCalendarIsHolidayDateGet200Response.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)


## InternalCalendarNextBusinessDayDateGet

> InternalCalendarNextBusinessDayDateGet200Response InternalCalendarNextBusinessDayDateGet(ctx, date).Execute()

Get Next Business Day



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
	date := "date_example" // string | The date to start from to find the next business day (format: YYYY-MM-DD)

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	resp, r, err := apiClient.CalendarAPI.InternalCalendarNextBusinessDayDateGet(context.Background(), date).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `CalendarAPI.InternalCalendarNextBusinessDayDateGet``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `InternalCalendarNextBusinessDayDateGet`: InternalCalendarNextBusinessDayDateGet200Response
	fmt.Fprintf(os.Stdout, "Response from `CalendarAPI.InternalCalendarNextBusinessDayDateGet`: %v\n", resp)
}
```

### Path Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
**ctx** | **context.Context** | context for authentication, logging, cancellation, deadlines, tracing, etc.
**date** | **string** | The date to start from to find the next business day (format: YYYY-MM-DD) | 

### Other Parameters

Other parameters are passed through a pointer to a apiInternalCalendarNextBusinessDayDateGetRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------


### Return type

[**InternalCalendarNextBusinessDayDateGet200Response**](InternalCalendarNextBusinessDayDateGet200Response.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)


## SecureCalendarHolidaysGet

> InternalCalendarHolidaysGet200Response SecureCalendarHolidaysGet(ctx).Execute()

Get Holidays



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
	resp, r, err := apiClient.CalendarAPI.SecureCalendarHolidaysGet(context.Background()).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `CalendarAPI.SecureCalendarHolidaysGet``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `SecureCalendarHolidaysGet`: InternalCalendarHolidaysGet200Response
	fmt.Fprintf(os.Stdout, "Response from `CalendarAPI.SecureCalendarHolidaysGet`: %v\n", resp)
}
```

### Path Parameters

This endpoint does not need any parameter.

### Other Parameters

Other parameters are passed through a pointer to a apiSecureCalendarHolidaysGetRequest struct via the builder pattern


### Return type

[**InternalCalendarHolidaysGet200Response**](InternalCalendarHolidaysGet200Response.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)


## SecureCalendarHolidaysYearGet

> InternalCalendarHolidaysGet200Response SecureCalendarHolidaysYearGet(ctx, year).Execute()

Get Holidays



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
	year := int32(56) // int32 | The year to retrieve holidays

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	resp, r, err := apiClient.CalendarAPI.SecureCalendarHolidaysYearGet(context.Background(), year).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `CalendarAPI.SecureCalendarHolidaysYearGet``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `SecureCalendarHolidaysYearGet`: InternalCalendarHolidaysGet200Response
	fmt.Fprintf(os.Stdout, "Response from `CalendarAPI.SecureCalendarHolidaysYearGet`: %v\n", resp)
}
```

### Path Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
**ctx** | **context.Context** | context for authentication, logging, cancellation, deadlines, tracing, etc.
**year** | **int32** | The year to retrieve holidays | 

### Other Parameters

Other parameters are passed through a pointer to a apiSecureCalendarHolidaysYearGetRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------


### Return type

[**InternalCalendarHolidaysGet200Response**](InternalCalendarHolidaysGet200Response.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)


## SecureCalendarIsHolidayDateGet

> InternalCalendarIsHolidayDateGet200Response SecureCalendarIsHolidayDateGet(ctx, date).Execute()

Check if Date is a Holiday



### Example

```go
package main

import (
	"context"
	"fmt"
	"os"
    "time"
	openapiclient "github.com/pi-financial/information-srv/client"
)

func main() {
	date := time.Now() // string | The date to check if it is a holiday (format: YYYY-MM-DD)

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	resp, r, err := apiClient.CalendarAPI.SecureCalendarIsHolidayDateGet(context.Background(), date).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `CalendarAPI.SecureCalendarIsHolidayDateGet``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `SecureCalendarIsHolidayDateGet`: InternalCalendarIsHolidayDateGet200Response
	fmt.Fprintf(os.Stdout, "Response from `CalendarAPI.SecureCalendarIsHolidayDateGet`: %v\n", resp)
}
```

### Path Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
**ctx** | **context.Context** | context for authentication, logging, cancellation, deadlines, tracing, etc.
**date** | **string** | The date to check if it is a holiday (format: YYYY-MM-DD) | 

### Other Parameters

Other parameters are passed through a pointer to a apiSecureCalendarIsHolidayDateGetRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------


### Return type

[**InternalCalendarIsHolidayDateGet200Response**](InternalCalendarIsHolidayDateGet200Response.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)


## SecureCalendarNextBusinessDayDateGet

> InternalCalendarNextBusinessDayDateGet200Response SecureCalendarNextBusinessDayDateGet(ctx, date).Execute()

Get Next Business Day



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
	date := "date_example" // string | The date to start from to find the next business day (format: YYYY-MM-DD)

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	resp, r, err := apiClient.CalendarAPI.SecureCalendarNextBusinessDayDateGet(context.Background(), date).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `CalendarAPI.SecureCalendarNextBusinessDayDateGet``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `SecureCalendarNextBusinessDayDateGet`: InternalCalendarNextBusinessDayDateGet200Response
	fmt.Fprintf(os.Stdout, "Response from `CalendarAPI.SecureCalendarNextBusinessDayDateGet`: %v\n", resp)
}
```

### Path Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
**ctx** | **context.Context** | context for authentication, logging, cancellation, deadlines, tracing, etc.
**date** | **string** | The date to start from to find the next business day (format: YYYY-MM-DD) | 

### Other Parameters

Other parameters are passed through a pointer to a apiSecureCalendarNextBusinessDayDateGetRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------


### Return type

[**InternalCalendarNextBusinessDayDateGet200Response**](InternalCalendarNextBusinessDayDateGet200Response.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)

