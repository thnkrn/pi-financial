# User Service v2

Manages user data by interfacing with [it api](https://github.com/pi-financial/pi-sso-v2) and [information api](https://github.com/pi-financial/information-srv).

## Prerequisites

- Go 1.23 or later
- Docker
- Make

## Getting Started

Quickly set up and run project:

```bash
# Clone repo
git clone https://github.com/pi-financial/user-srv-v2.git

# Get `.env` from other devs and configure your environment variables

# Start database
docker compose up db

# Install external dependencies
make init
make precommit.rehooks
make ci.lint

# Install package dependencies
go mod download
go mod vendor

# Run with hot reload
make build
make run

# Check that service is deployed
curl http://localhost:8080
```

Will deploy swagger on <http://localhost:8080/swagger/index.html>

## API Documentation

The service uses Swagger for API documentation. Access the Swagger UI at:

Local

```
http://localhost:8080/swagger/index.html
```

Staging

```
http://user-api-v2.nonprod.pi.internal/swagger/index.html
```

## Installing Tools

Tools in this project:

1. [wire](https://github.com/google/wire) - Generate dependency injection
2. [swag](https://github.com/swaggo/swag) - Generate Swagger OpenApi client
3. [reflex](https://github.com/cespare/reflex) - Hot-reload run
4. [dlv](https://github.com/go-delve/delve) - Golang debugger
5. [mockgen](https://github.com/uber-go/mock) - Generate mock interface for dependency injection during testing

Example:

```bash
# Install tool
go install github.com/google/wire/cmd/wire@latest

# Add go bin to path
# Add to .bashrc or .zshrc
export "$(echo ~)/go/bin:$PATH"

# Test run
which wire
wire --help
```

## Installing and Updating Packages

Example: Getting Gorm ORM [package](https://pkg.go.dev/gorm.io/gorm).

Get through command line will automatically add to `go.mod`:

```bash
go get gorm.io/gorm@latest
```

Or directly editting `go.mod`:

```
module github.com/pi-financial/user-srv-v2

go 1.23

require (
 gorm.io/gorm v1.25.12 // Specify specific version
)
```

Then sync dependencies.

```bash
go mod download # Install packages in go.mod
go mod vendor   # Save dependencies in local vendor directory
go mod tidy     # Add missing or remove unused modules in the project
```

## Contributing

1. Create a new branch: `feature/<jira code>` or `fix/<jira code>`
2. Make your changes
3. Add method documentation and update any outdated ones, clearly identifying external resources used
4. Add new feature switches if necessary
5. Generate new dependency injection if necessary: `make wire`
6. Write/update tests
7. Generate new OpenApi doc: `make doc`
8. Generate new OpenApi client: `make gen-client`
9. Tests and lints will run automatically on commit
10. Submit a PR
11. Add `Ready to Test PR` tag in the PR to deploy to PR environment
12. Add new secrets on AWS if necessary
13. Get PR approved
14. Test on PR environment and verify results with QA
15. Merge branch
16. **If domain model have ot be updated, make sure there is a corresponding db migration in user service v1**

## Local Development

### Running

```bash
go mod tidy
make build
make run
```

The service will start on port 8080 by default.

### Debugging (Visual Studio Code)

1. Add a `launch.json` and define debug config.
2. Install golang extension: <https://code.visualstudio.com/docs/languages/go>.
3. Navigate to `Run and Debug` on the `Activity Bar` then click on `Launch and debug package`. Log results will show in `Debug Console`.

```
├── .vscode/
│   └── launch.json
```

```json
{
    "version": "0.2.0",
    "configurations": [
        {
            "name": "Launch and debug package",
            "type": "go",
            "request": "launch",
            "mode": "debug",
            "program": "${workspaceFolder}${pathSeparator}cmd", // Starts the app in cmd directory
            "debugAdapter": "dlv-dap",
            "envFile": "${workspaceFolder}${pathSeparator}.env", // Includes .env file
            "showGlobalVariables": true,
            "showRegisters": true
        }
    ]
}
```

### Running Tests

Test files exist in the same directory as the code being tested.

Running tests in command line:

```bash
make test
```

Or in IDE (Visual Studio Code):

1. Install golang extension: <https://code.visualstudio.com/docs/languages/go>
2. Navigate to `Testing` on the `Activity Bar`, check that all tests are collected successfully, then click on `Run Tests`

## Project Structure

```
├── cmd/                      # Main applications
│   ├── http/                 # HTTP server entrypoint
│   └── main.go               # Main application entry
├── config/                   # Configuration handling
│   └── config.go             # Map .env values to internal config object
├── config/                   # Constants
│   └── errors.go             # Generic reusable custom errors
│   └── feature_switches.go   # Growthbook feature switches
├── internal/                 # Private application code
│   ├── domain/               # Domain models
│   ├── dto/                  # Data transfer objects
│   ├── handler/              # HTTP handlers
│   ├── middleware/           # HTTP middleware
│   ├── repository/           # Data access layer
│   ├── driver/               # External service interface layer
│   └── service/              # Business logic
│   └── utils/                # Util methods
├── docs/                     # (Generated) Swagger doc
└── client/                   # (Generated) OpenApi client
├── migrations/               # (Generated) Database migrations
├── mock/                     # (Generated) Mock objects for testing
└── vendor/                   # (Generated) Vendored dependencies
└── go.mod                    # (Config) Project dependencies
└── Makefile                  # (Config) Make commands
└── .env                      # (Config) Environment variables
└── .golangci.yml             # (Config) Linter settings
└── .pre-commit-config.yaml   # (Config) Actions before committing
└── sonar-project.properties  # (Config) Sonarcloud config
└── atlas.hcl                 # (Config) Database migration config
```

### Key Components

- **Handlers**: HTTP request handlers in `internal/handler/`
- **Services**: Business logic in `internal/service/`
- **Repositories**: Database operations in `internal/repository/`
- **Middleware**: Request/response middleware in `internal/middleware/`

### Development Workflow

1. Define your data models in `internal/domain/`
2. Create DTOs in `internal/dto/`
3. Implement repository interfaces in `internal/repository/`
4. Add business logic in `internal/service/`
5. Create HTTP handlers in `internal/handler/`
6. Register routes in `internal/handler/server.go`
7. Add tests in the same directory as the code with the name `{file}_test.go`

### Example Echo Handler

```go
func (h *UserInfoHandler) GetUserInfo(c echo.Context) error {
    userId, err := utils.GetUserIdFromHeader(c)
    if err != nil {
        return result.ParamErrorResult(c, err)
    }

    userInfo, err := h.UserInfoService.GetUserInfo(c.Request().Context(), userId.String())
    return result.HttpResult(c, userInfo, err)
}
```

## Development

### Internal Packages

Package names follow each component type:

- **domain**: [./internal/domain](./internal/domain)
- **dto**: [./internal/dto](./internal/dto)
- **handler**: [./internal/handler](./internal/handler)
- **service**: [./internal/service](./internal/service)
- **repository**: [./internal/repository](./internal/repository)
- **utils**: [./internal/utils](./internal/utils)
- **client**: [./internal/driver/client](./internal/driver/client)
- **growthbook**: [./internal/driver/growthbook](./internal/driver/growthbook)
- **mysql**: [./internal/driver/mysql](./internal/driver/mysql)

Get approval from the team before creating any new package names.

### Cases

Variable names, struct fields, and json fields must all be in camel case.

### Config

Key environment variables:

```shell
DB_HOST=localhost
DB_USER=user
DB_PASSWORD=password
DB_NAME=database
DB_PORT=5432
```

Register each environment variable in [./config/config.go](./config/config.go) to be able to use them in the code.

### Constants

Define constant variables in [./const](./const).

### Dependency Injection

The project uses Wire for dependency injection. New dependencies will not be usable if a new wire is not generated.

Adding new dependency injection for domain logic:

1. Add new domain logic depenencies (new handler, service, repository, or client)
2. Add constructor method in `UserSet` in [./di/user.go](./di/user.go)
3. Run `make wire` to generate new dependency injection in `wire_gen.go`

### Domain Models

Database domain models are defined in [./internal/domain](./internal/domain/).

- Use Gorm annotation for validation
- Column name must be snake case and object field name must be camel case
- We can use Gorm hooks (<https://gorm.io/docs/hooks.html>) to control object life cycle
- We can add domain logic to the domain model

Example:

```go
type BankAccountV2 struct {
    Id          uuid.UUID   `gorm:"column:id;type:varchar(36);primaryKey" json:"id"`
    UserId      uuid.UUID   `gorm:"column:user_id;type:varchar(36)" json:"userId"`
    AccountNo   string  `gorm:"column:account_no" json:"accountNo"`
}

func (a *BankAccountV2) TableName() string {
    return "bank_account_v2s"
}

// Custom domain logic for this domain model
func (a *BankAccountV2) doSomething(value string) (string, error) {
    ...
}

// Gorm hook method
func (a *BankAccountV2) BeforeCreate(tx *gorm.DB) (err error) {
    // Do something before saving object to the database
    return nil
}
```

### Dto

Dtos are defined in [./internal/dto](./internal/dto/).

Any data coming from external services or read from the database need to be mapped to a dto object for internal consistency.

### Server

Routes are defined in [./internal/handler/server.go](./internal/handler/server.go). Map routes to a specific handler method under a grouping. E.g.

```go
// Add route prefix for all handlers below
secure := e.Group("secure/v1")

// Route: secure/v1/watchlists
secure.POST("/watchlists", handlers.WatchlistHandler.CreateWatchlist)


// Add route prefix for all handlers below
internalV2 := e.Group("internal/v2")

// Route: secure/v1/users
secure.GET("/users", handlers.UserInfoHandler.GetUserInfo)

// Route: internal/v2/bank-account/deposit-withdraw
internalV2.GET("/bank-account/deposit-withdraw", handlers.BankAccountHandler.GetBankAccountsForDepositWithdraw)
```

### Handler

Route handlers are defined in [./internal/handler/](./internal/handler/).

A handler method must do only 3 things:

- Validate requests
- Call a service method to invoke domain logic
- Generic error handling/logging

### Service

Service methods are defined in [./internal/service/](./internal/service/).

- Contains the core domain logic
- An interface must be defined for each public method
- Must have proper error handling

### Repository

Repository methods are defined in [./internal/repository/](./internal/repository/).

- Contains logic that interfaces with the database.
- An interface must be defined for each public method.
- Must have proper error handling.
- *Currently uses gorm orm. There are some query optimization issues, so sometimes a raw query is preferred. Standard pattern for querying is still being determined and a guideline will be updated later.*

### External Service

Methods that interface with external services are defined in [./internal/driver/client/](./internal/driver/client/).

- Contains logic that interfaces with external APIs. One method corresponds to 1 route.
- An interface must be defined for each public method.
- Must have proper error handling.
- Domain logic may need to call 1 or more external service method. The preferred approach is to create a new service method for the usecase then map the result to a dto. This will eliminate internal components' reliance on external service models.

### External Component

Connection to external components such as database or growthbook are defined in [./internal/driver/](./internal/driver/). E.g. [./internal/driver/mysql](./internal/driver/mysql/).

- Contain methods that setup the connection to external components
- Need to be applied to dependency injection

### Middleware

Middleware will hook to request/response each api call (e.g. This auth or logging). You probably won't need to create one. Look at Echo's documentation for more info.

### Utils and Common

Common methods are in Pi Security's [go-common](https://github.com/pi-financial/go-common) package.

Local utils methods are in [./internal/utils/](./internal/utils/). This must contain only utils specific to this service.

You should have the permission to modify both.

**BUT!** only add as needed, and make each method as simple as possible. Cluttering utils/common packages is not acceptable.

### Datetime

- Everything in this service is assumed to be in Gregorian format.
- Unless specified otherwise, date format is YYYY-MM-DD.

### Logging

Use the provided logger interface:

```go
s.Log.Info("message")
s.Log.Error("error message")
```

- **ONLY** log what is necessary: special edge cases that happened or errors that can't be represented by a custom error object.
- Logging error is done implicitly with error handling at the outer level.

### Error Handling

Error handling guideline: create an error chain.

- Add custom error constant
- Add error wrapper to method
- If error results from logic, return custom error constant
- If a method returns an error, return an ad-hoc user-defined error that wraps the error
- At the outer most level, unwrap the error and print out the log

#### Error Constant

Error constants are in [./const/errors.go](./const/errors.go). They must:

- Indicate a root cause of the error
- Be generic enough so they can be re-usable
- Have a unique code
- Detail must have the format:
  - Lower-case
  - Describe the exact context of the error
  - Avoid starting with "problem" or "error" or "failed", EXCEPT when it is due to external service failing

Note: [`errorx`](github.com/pi-financial/go-common/errorx) is our own error type define in go-common that satisfies the `error` interface. This section can be applied to any custom error type.

Example:

```go
// Internal error is generic, reusable, concise, and indicates the exact reason
ErrUserAccountNotFound = errorx.NewErrCodeMsg("USR0001", "user account not found")

// External service error indicates the failed service and the action
ErrUserDbGetProduct = errorx.NewErrCodeMsg("USR0002", "problem getting user from db")
```

#### Add Error Wrapper to Method

Wrap any error returned inside the method inside a user-defined error that gives context.

This basically creates a custom stack trace when the whole error is printed out.

```go
func GetUsersByName(allUsers string, name string) (users []string, err error) {
    defer func() {
        if err != nil {
            err = fmt.Errorf("in GetUsersByName by name %q: %w", name, err)
        }
    }()

    // search and return user
}
```

#### Return Custom Error for Logic Error

```go
func GetUsersByName(allUsers string, name string) (users []string, err error) {
    defer func() {
        if err != nil {
            err = fmt.Errorf("in GetUsersByName by name %q: %w", name, err)
        }
    }()

    users := // some filter logic to search for some users

    if (len(users) == 0) {
        return nil, ErrUserAccountNotFound
    }

    return users, nil
}
```

#### Return User Defined Error for External Error

User defined error gives context to the action being performed, telling us what exactly happened when `err` is returned.

```go
func GetUsersByName(allUsers string, name string) (users []string, err error) {
    defer func() {
        if err != nil {
            err = fmt.Errorf("in GetUsersByName by name %q: %w", name, err)
        }
    }()

    users, err := // some external service method

    if (err != nil) {
        return nil, fmt.Errorf("find in some service: %w", err)
    }

    return users, nil
}
```

Or if this is inside a repository method, add custom error and wrap it with the original error. This tells us the "root cause" is that db operation failed, and `err` gives us the actual details.

```go
func FindByName(name string) (users []string, err error) {
    defer func() {
        if err != nil {
            err = fmt.Errorf("in FindByName by name %q: %w", name, err)
        }
    }()

    users, err := // some db query method

    if (err != nil) {
        return nil, fmt.Errorf("%w: %w", ErrUserDbGetProduct, err)
    }

    return users, nil
}
```

#### Unwrap Error and Log At The Outer Level

Printing `err` that have inner errors that had been wrapped will concat all the errors, giving us a stack trace.

```go
func main(name string) ([]string users, err error) {

    allUsers := []string{"a", "a", "b", "c"}

    foundUsers, err := GetUsersByName(allUsers, name)

    if err != nil {
        // Will unwrap the error and print full stack trace
        Log.Error(err.Error())

        return nil, err
    }

    return foundUsers, nil
}
```

#### Return the Custom Error Constant

Usually we want the "root" cause of the error, which is the custom error that we wrapped in the inner levels. We use `error.As` to extract it.

```go
func main(name string) ([]string users, err error) {

    allUsers := []string{"a", "a", "b", "c"}

    foundUsers, err := GetUsersByName(allUsers, name)

    if err != nil {
        Log.Error(err.Error())

        // Extract the custom error from the wrapped error
        var customErr *errorx.ErrorMsg
        if errors.As(err, &customErr) {
            err = customErr
        }

        return nil, err
    }

    return foundUsers, nil
}
```

#### Bringing It Together

```go
func main(name string) ([]string users, err error) {

    allUsers := []string{"a", "a", "b", "c"}

    foundUsers, err := GetUsersByName(allUsers, name)

    if err != nil {
        Log.Error(err.Error())

        var customErr *errorx.ErrorMsg
        if errors.As(err, &customErr) {
            err = customErr
        }

        return nil, err
    }

    return foundUsers, nil
}

func GetUsersByName(allUsers string, name string) (users []string, err error) {
    defer func() {
        if err != nil {
            err = fmt.Errorf("in GetUsersByName by name %q: %w", name, err)
        }
    }()

    users, err := FindByName(name)

    if (err != nil) {
        return nil, fmt.Errorf("find in db: %w", err)
    }

    return users, nil
}

func FindByName(name string) (users []string, err error) {
    defer func() {
        if err != nil {
            err = fmt.Errorf("in FindByName %q: %w", name, err)
        }
    }()

    users, err := // some db query method returns "Connection timeout" error

    if (err != nil) {
        return nil, fmt.Errorf("%w: %w", ErrUserDbGetProduct, err)
    }

    return users, nil
}
```

Assuming db query method in `FindByName` throws a connection error: `Connection timeout`.

Calling `main("some name")` will print out the following log:

```
in GetUsersByName by name \"some name\": find in db: in FindByName \"some name\": problem getting user from db: Connection timeout
```

And will return:

```
ErrUserDbGetProduct
```

### Api Response

All api response must return as:

```go
result.HttpResult(contextObject, responseData, err)
```

[`result`](github.com/pi-financial/go-common/result) is defined in go-common to help with response return handling.

### Database Migration

Atlas is used for database migration. Config is defined in `atlas.hcl`.

```bash
# Create new migration
make migrate

# Apply migration script to database
make migrate-apply
```

For now, we handle database migration in user-srv using entity framework. When we had fully migrated to user-srv-v2, this section will be updated with more instructions on how to do db migration with atlas.

### Filtering and Mapping

You can either use a lodash-style lib called [lo](https://github.com/samber/lo) for filtering/mapping or use a normal for loop.

Depending on your circumstance, choose the most readable and straightforward version.

Lo is good when the logic is simple:

```go
input := []string{"a", "b", "c"}
output := lo.Map(input, func(item string, index int) string {

    // Do some data transformation

    return item
})


output = lo.Filter(output, func(item string, index int) bool {
    return // some filter criteria boolean
})
```

Common `lo` methods are `Filter`, `Map`, `ForEach`, `FilterMap`.

But sometimes for loop is more straightforward when there are complex calls and error handling:

```go
input := []string{"a", "b", "c"}

output := []string{}
for _, r := range input {

    transformedData, err := // some data transformation on r

    if (err != nil) {
        return err
    }

    output = output.append(output, transformedData)
}
```

Note: Our convention is prefering slice initialization over using `make` for clarity and to avoid indexing issues. As this is not a data-intensive application, performance gained by pre-allocating with `make` is negligible.

### Mapping Struct With Pointer Fields

Sometimes we want to map from one struct, whose fields are pointers, to another struct, whose fields are concrete values. To avoid creating a new placeholder variable for each field, you can either use `lo.FromPtr` or use a wrapper method:

```go
type StructWithPtr struct {
    a   *string
    b   *string
    c   *string
    d   *string
    e   *string
    f   *string
}

type StructWithValue struct {
    a   string
    b   string
    c   string
    d   string
    e   string
    f   string
}

func MapStructWithLo(input StructWithPtr) (output StructWithValue) {
    return StructWithValue{
        a: lo.FromPtr(input.a),
        b: lo.FromPtr(input.b),
        c: lo.FromPtr(input.c),
        d: lo.FromPtr(input.d),
        e: lo.FromPtr(input.e),
        f: lo.FromPtr(input.f)
    }
}
```

The benefit of a wrapper method is that there is more flexibility in how to handle different values:

```go
func MapStruct(input StructWithPtr) (output StructWithValue) {
    defaultToEmpty := func(c *string) string {
        if c == nil {
            return ""
        }
        return *c
    }

    return StructWithValue{
        a: defaultToEmpty(input.a),
        b: defaultToEmpty(input.b),
        c: defaultToEmpty(input.c),
        d: defaultToEmpty(input.d),
        e: defaultToEmpty(input.e),
        f: defaultToEmpty(input.f)
    }
}
```

And sometimes we want to map a literal to a pointer variable directly without having to create an intermediate variable. That can also be done with lo:

```go
a := lo.ToPtr("token123")
```

## Testing

### Test Structure

Create the test in the same director as the file being tested.

Assuming we are going to test `SomeService` object, the following is the test file template:

```go
// Test suite definition
type SomeTestSuite struct {
    suite.Suite
    someService     SomeService
    ctx             context.Context
}

// Setup. Initialize subject to test.
func (s *SomeTestSuite) SetupTest() {
    s.someService = SomeService{}
    s.ctx = context.Background()
}

// Boilerplate
func TestSomeService(t *testing.T) {
    suite.Run(t, new(SomeTestSuite))
}

// Main testing code.
func (s *SomeTestSuite) TestSomeMethod() {
    // Test cases are split into structs
    testCases := []struct {
        name        string
        input       string
        expected    string
        setup       func()
        wantErr     bool
    }{
        {
            name: "should return result when valid input",
            input: "valid input",
            expected: "result",
            setup: func() {}
            wantErr: false,
        },
        {
            name: "should return err when invalid input",
            input: "invalid input",
            expected: "",
            setup: func() {
                // do some setup
            }
            wantErr: false,
        },
        // Add your test cases inside this list
    }

    // Running tests = loop through each struct run the method
    for _, tc := range testCases {
        s.Run(tc.name, func() {
            tc.setup()

            result, err := s.someService.SomeMethod(s.ctx, tc.input)

            if tc.wantErr {
                s.Error(err)
            } else {
                s.NoError(err)

                // Assert result
                s.Equal(tc.expected, result)
            }
        })
    }
}
```

### Writing Tests

- 1 method can be tested with multiple test cases
- Add your test cases inside the test case list
- Each test case name should mention what should happen and when

### Mocking

Use `mockgen` to create mock objects based on interface with the following pattern:

```bash
mockgen -source ./internal/{directory}/interfaces/{file}.go -package mock{package} > mock/{directory}/{file}.go
```

Example:

```bash
# package = repository
# interface = trade_account
# interface location = internal/repository
mockgen -source ./internal/repository/interfaces/trade_account.go -package mockrepository > mock/repository/trade_account_mock.go

# package = client
# interface = information
# interface location = internal/driver/client
mockgen -source ./internal/driver/client/interfaces/information.go -package mockclient > mock/driver/client/information.go
```

Inject mock into the test, assuming that `SomeService` depends on `SomeRepo` and `Logger`:

```go
type SomeTestSuite struct {
    suite.Suite
    mockSomeRepo    *mockrepository.MockSomeRepo
    mockLogger      *mockvendor.MockLogger
    someService     SomeService
    ctx             context.Context
}

func (s *SomeTestSuite) SetupTest() {
    s.someService = SomeService{
        SomeRepo:   s.mockSomeRepo,
        Logger:     s.mockLogger
    }
    s.ctx = context.Background()
}
```

Mock calls inside `setup`:

```go
func (s *SomeTestSuite) TestSomeMethod() {
    testCases := []struct {
        name        string
        input       string
        expected    string
        setup       func()
        wantErr     bool
    }{
        {
            name: "should return result when valid input",
            input: "valid input",
            expected: "result",
            setup: func() {
                s.mockSomeRepo.EXPECT().DbQuery().Return("success", nil)
            }
            wantErr: false,
        },
        {
            name: "should return err when invalid input",
            input: "invalid input",
            expected: "",
            setup: func() {
                // Assuming that a repo method call causes SomeMethod to fail
                // Mock the error return value
                s.mockSomeRepo.EXPECT().DbQuery().Return(nil, errors.New("db error"))
            }
            wantErr: false,
        },
        // Add your test cases inside this list
    }

    // Running tests = loop through each struct run the method
    for _, tc := range testCases {
        s.Run(tc.name, func() {
            tc.setup()

            result, err := s.someService.SomeMethod(s.ctx, tc.input)

            if tc.wantErr {
                s.Error(err)
            } else {
                s.NoError(err)

                // Assert result
                s.Equal(tc.expected, result)
            }
        })
    }
}
```

## Additional Resources

- [Echo Framework Documentation](https://echo.labstack.com/)
- [Go Project Layout](https://github.com/golang-standards/project-layout)
- [Effective Go](https://golang.org/doc/effective_go)
- [Uber Go Style Guide](https://github.com/uber-go/guide/blob/master/style.md)
- [Google Go Style Guide](https://google.github.io/styleguide/go/)
- [Gorm Documentation](https://gorm.io/docs/index.html)
- [Pi IT Api](https://github.com/pi-financial/pi-sso-v2)
- [Pi Information Api](https://github.com/pi-financial/information-srv)
