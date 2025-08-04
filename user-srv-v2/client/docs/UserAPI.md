# \UserAPI

All URIs are relative to *http://localhost*

Method | HTTP request | Description
------------- | ------------- | -------------
[**InternalV1UsersGet**](UserAPI.md#InternalV1UsersGet) | **Get** /internal/v1/users | Get user info by filters
[**InternalV1UsersMigratePost**](UserAPI.md#InternalV1UsersMigratePost) | **Post** /internal/v1/users/migrate | Create new user with info from BPM
[**InternalV1UsersPatch**](UserAPI.md#InternalV1UsersPatch) | **Patch** /internal/v1/users | Update some fields by user-id
[**InternalV1UsersPost**](UserAPI.md#InternalV1UsersPost) | **Post** /internal/v1/users | Create user info
[**InternalV1UsersSyncPost**](UserAPI.md#InternalV1UsersSyncPost) | **Post** /internal/v1/users/sync | Sync user data from it services
[**InternalV1UsersUserIdSubUsersGet**](UserAPI.md#InternalV1UsersUserIdSubUsersGet) | **Get** /internal/v1/users/{user-id}/sub-users | Get sub users
[**InternalV1UsersUserIdSubUsersPost**](UserAPI.md#InternalV1UsersUserIdSubUsersPost) | **Post** /internal/v1/users/{user-id}/sub-users | Add sub user
[**SecureV1UsersGet**](UserAPI.md#SecureV1UsersGet) | **Get** /secure/v1/users | Get user info



## InternalV1UsersGet

> InternalV1UsersGet200Response InternalV1UsersGet(ctx).Ids(ids).AccountId(accountId).CitizenId(citizenId).PhoneNumber(phoneNumber).Email(email).FirstName(firstName).LastName(lastName).Execute()

Get user info by filters



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
	ids := "ids_example" // string | User IDs use comma to separate (optional)
	accountId := "accountId_example" // string | Account ID (optional)
	citizenId := "citizenId_example" // string | Citizen ID (optional)
	phoneNumber := "phoneNumber_example" // string | Phone Number (optional)
	email := "email_example" // string | Email (optional)
	firstName := "firstName_example" // string | First Name (optional)
	lastName := "lastName_example" // string | Last Name (optional)

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	resp, r, err := apiClient.UserAPI.InternalV1UsersGet(context.Background()).Ids(ids).AccountId(accountId).CitizenId(citizenId).PhoneNumber(phoneNumber).Email(email).FirstName(firstName).LastName(lastName).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `UserAPI.InternalV1UsersGet``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `InternalV1UsersGet`: InternalV1UsersGet200Response
	fmt.Fprintf(os.Stdout, "Response from `UserAPI.InternalV1UsersGet`: %v\n", resp)
}
```

### Path Parameters



### Other Parameters

Other parameters are passed through a pointer to a apiInternalV1UsersGetRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **ids** | **string** | User IDs use comma to separate |
 **accountId** | **string** | Account ID |
 **citizenId** | **string** | Citizen ID |
 **phoneNumber** | **string** | Phone Number |
 **email** | **string** | Email |
 **firstName** | **string** | First Name |
 **lastName** | **string** | Last Name |

### Return type

[**InternalV1UsersGet200Response**](InternalV1UsersGet200Response.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)


## InternalV1UsersMigratePost

> InternalV1UsersMigratePost200Response InternalV1UsersMigratePost(ctx).UserId(userId).DtoMigrateUserRequest(dtoMigrateUserRequest).Execute()

Create new user with info from BPM



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
	dtoMigrateUserRequest := *openapiclient.NewDtoMigrateUserRequest() // DtoMigrateUserRequest | Migrate User Request

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	resp, r, err := apiClient.UserAPI.InternalV1UsersMigratePost(context.Background()).UserId(userId).DtoMigrateUserRequest(dtoMigrateUserRequest).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `UserAPI.InternalV1UsersMigratePost``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `InternalV1UsersMigratePost`: InternalV1UsersMigratePost200Response
	fmt.Fprintf(os.Stdout, "Response from `UserAPI.InternalV1UsersMigratePost`: %v\n", resp)
}
```

### Path Parameters



### Other Parameters

Other parameters are passed through a pointer to a apiInternalV1UsersMigratePostRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **userId** | **string** | User ID |
 **dtoMigrateUserRequest** | [**DtoMigrateUserRequest**](DtoMigrateUserRequest.md) | Migrate User Request |

### Return type

[**InternalV1UsersMigratePost200Response**](InternalV1UsersMigratePost200Response.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: application/json
- **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)


## InternalV1UsersPatch

> ResultResponseSuccess InternalV1UsersPatch(ctx).UserId(userId).DtoPatchUserInfoRequest(dtoPatchUserInfoRequest).Execute()

Update some fields by user-id



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
	dtoPatchUserInfoRequest := *openapiclient.NewDtoPatchUserInfoRequest() // DtoPatchUserInfoRequest | Patch User Info Request

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	resp, r, err := apiClient.UserAPI.InternalV1UsersPatch(context.Background()).UserId(userId).DtoPatchUserInfoRequest(dtoPatchUserInfoRequest).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `UserAPI.InternalV1UsersPatch``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `InternalV1UsersPatch`: ResultResponseSuccess
	fmt.Fprintf(os.Stdout, "Response from `UserAPI.InternalV1UsersPatch`: %v\n", resp)
}
```

### Path Parameters



### Other Parameters

Other parameters are passed through a pointer to a apiInternalV1UsersPatchRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **userId** | **string** | User ID |
 **dtoPatchUserInfoRequest** | [**DtoPatchUserInfoRequest**](DtoPatchUserInfoRequest.md) | Patch User Info Request |

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


## InternalV1UsersPost

> InternalV1UsersPost200Response InternalV1UsersPost(ctx).DtoCreateUserInfoRequest(dtoCreateUserInfoRequest).Execute()

Create user info



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
	dtoCreateUserInfoRequest := *openapiclient.NewDtoCreateUserInfoRequest("CitizenId_example", "DateOfBirth_example", "Email_example", "FirstnameEn_example", "FirstnameTh_example", "LastnameEn_example", "LastnameTh_example", "PhoneNumber_example", "WealthType_example") // DtoCreateUserInfoRequest | Create User Info Request

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	resp, r, err := apiClient.UserAPI.InternalV1UsersPost(context.Background()).DtoCreateUserInfoRequest(dtoCreateUserInfoRequest).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `UserAPI.InternalV1UsersPost``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `InternalV1UsersPost`: InternalV1UsersPost200Response
	fmt.Fprintf(os.Stdout, "Response from `UserAPI.InternalV1UsersPost`: %v\n", resp)
}
```

### Path Parameters



### Other Parameters

Other parameters are passed through a pointer to a apiInternalV1UsersPostRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **dtoCreateUserInfoRequest** | [**DtoCreateUserInfoRequest**](DtoCreateUserInfoRequest.md) | Create User Info Request |

### Return type

[**InternalV1UsersPost200Response**](InternalV1UsersPost200Response.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: application/json
- **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)


## InternalV1UsersSyncPost

> ResultResponseSuccess InternalV1UsersSyncPost(ctx).CustomerCode(customerCode).SyncType(syncType).Execute()

Sync user data from it services



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
	customerCode := "customerCode_example" // string | Customer Code
	syncType := "syncType_example" // string | Sync Type (kyc,suitTest,address,tradingAccount,userInfo,all)

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	resp, r, err := apiClient.UserAPI.InternalV1UsersSyncPost(context.Background()).CustomerCode(customerCode).SyncType(syncType).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `UserAPI.InternalV1UsersSyncPost``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `InternalV1UsersSyncPost`: ResultResponseSuccess
	fmt.Fprintf(os.Stdout, "Response from `UserAPI.InternalV1UsersSyncPost`: %v\n", resp)
}
```

### Path Parameters



### Other Parameters

Other parameters are passed through a pointer to a apiInternalV1UsersSyncPostRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **customerCode** | **string** | Customer Code |
 **syncType** | **string** | Sync Type (kyc,suitTest,address,tradingAccount,userInfo,all) |

### Return type

[**ResultResponseSuccess**](ResultResponseSuccess.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)


## InternalV1UsersUserIdSubUsersGet

> InternalV1UsersUserIdSubUsersGet200Response InternalV1UsersUserIdSubUsersGet(ctx, userId).Execute()

Get sub users



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
	resp, r, err := apiClient.UserAPI.InternalV1UsersUserIdSubUsersGet(context.Background(), userId).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `UserAPI.InternalV1UsersUserIdSubUsersGet``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `InternalV1UsersUserIdSubUsersGet`: InternalV1UsersUserIdSubUsersGet200Response
	fmt.Fprintf(os.Stdout, "Response from `UserAPI.InternalV1UsersUserIdSubUsersGet`: %v\n", resp)
}
```

### Path Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
**ctx** | **context.Context** | context for authentication, logging, cancellation, deadlines, tracing, etc.
**userId** | **string** | User ID |

### Other Parameters

Other parameters are passed through a pointer to a apiInternalV1UsersUserIdSubUsersGetRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------


### Return type

[**InternalV1UsersUserIdSubUsersGet200Response**](InternalV1UsersUserIdSubUsersGet200Response.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)


## InternalV1UsersUserIdSubUsersPost

> ResultResponseSuccess InternalV1UsersUserIdSubUsersPost(ctx, userId).RequestBody(requestBody).Execute()

Add sub user



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
	requestBody := []string{"Property_example"} // []string | Map Sub User Request

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	resp, r, err := apiClient.UserAPI.InternalV1UsersUserIdSubUsersPost(context.Background(), userId).RequestBody(requestBody).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `UserAPI.InternalV1UsersUserIdSubUsersPost``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `InternalV1UsersUserIdSubUsersPost`: ResultResponseSuccess
	fmt.Fprintf(os.Stdout, "Response from `UserAPI.InternalV1UsersUserIdSubUsersPost`: %v\n", resp)
}
```

### Path Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
**ctx** | **context.Context** | context for authentication, logging, cancellation, deadlines, tracing, etc.
**userId** | **string** | User ID |

### Other Parameters

Other parameters are passed through a pointer to a apiInternalV1UsersUserIdSubUsersPostRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------

 **requestBody** | **[]string** | Map Sub User Request |

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


## SecureV1UsersGet

> SecureV1UsersGet200Response SecureV1UsersGet(ctx).UserId(userId).Execute()

Get user info



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
	resp, r, err := apiClient.UserAPI.SecureV1UsersGet(context.Background()).UserId(userId).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `UserAPI.SecureV1UsersGet``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `SecureV1UsersGet`: SecureV1UsersGet200Response
	fmt.Fprintf(os.Stdout, "Response from `UserAPI.SecureV1UsersGet`: %v\n", resp)
}
```

### Path Parameters



### Other Parameters

Other parameters are passed through a pointer to a apiSecureV1UsersGetRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **userId** | **string** | User ID |

### Return type

[**SecureV1UsersGet200Response**](SecureV1UsersGet200Response.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)
