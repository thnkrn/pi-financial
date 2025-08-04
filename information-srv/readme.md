# 📝 information-srv

## ✨ Overview

information-srv is a web service built with Golang using the Echo framework. This service is designed to provide structured and reliable information management through a RESTful API.

## 💡 Features

- RESTful API endpoints
- Scalable and lightweight service
- Easy configuration and deployment

## 🏗️ Architecture

The project follows the Hexagonal Architecture design pattern, emphasizing maintainability, scalability, and testability. This approach separates business logic from external systems like databases, APIs, and user interfaces.

## 🔗 External Integration

This service integrates with the Bank of Thailand (BOT) API for fetching exchange rates and public holiday data.

- **API Host:** [https://apigw1.bot.or.th/bot/public](https://apigw1.bot.or.th/bot/public)
- **Services Used:**
  - Exchange Rate Data
  - Holiday Calendar

## 📄 API Documentation

The following API endpoints are defined based on the Swagger specification:

### 📍 Address Endpoints

- **GET** `/internal/address` - Retrieve a list of all addresses.
- **GET** `/internal/address/province` - Retrieve a list of all provinces.
- **GET** `/internal/address/province/{province}` - Retrieve addresses by province.
- **GET** `/internal/address/zip-code/{zipCode}` - Retrieve addresses by zip code.

### 📅 Calendar Endpoints

- **GET** `/internal/calendar/holidays` - Retrieve holidays for the current or specified year.
- **GET** `/internal/calendar/holidays/{year}` - Retrieve holidays for a specific year.
- **GET** `/internal/calendar/is-holiday/{date}` - Check if a specific date is a holiday.
- **GET** `/internal/calendar/next-business-day/{date}` - Retrieve the next business day after a specified date.

### 💸 Exchange Rate Endpoints

- **GET** `/internal/exchange-rate` - Retrieve exchange rates for a specific date range.

#### Request Parameters

| Parameter    | Type   | Description                   | Required | Example       |
|--------------|--------|-------------------------------|----------|----------------|
| `from`      | string | Start date (YYYY-MM-DD)      | Yes      | `2024-01-01` |
| `to`        | string | End date (YYYY-MM-DD)        | Yes      | `2024-01-31` |
| `fromCur`   | string | Source currency code         | No       | `USD`        |
| `toCur`     | string | Destination currency code    | No       | `THB`        |


## ⚙️ Makefile Commands

### Build and Run

- `make build` - Compile the code and build the executable file.
- `make run` - Start the application.

### Testing

- `make test` - Run tests with coverage.
- `make test-coverage` - Generate a test coverage report.

### Code Generation

- `make doc` - Generate Swagger & OpenAPI documentation.
- `make gen-client` - Generate client lib using openapi-generator-cli.


## 🛠️ Environment Variables

| Variable               | Description                     | Default                    |
|------------------------|----------------------------------|----------------------------|
| `BOT_CLIENT_ID`        | BOT API client ID              | `your-client-id`                     |
| `BOT_HOST`             | BOT API host URL               | `https://apigw1.bot.or.th/bot/public` |
| `COMMON_DB_DSN`        | Database connection string     | `user:password@tcp(host:3306)/common_db` |
| `REDIS_ENABLED`        | Enable Redis caching           | `true`                    |
| `REDIS_HOST`           | Redis host and port            | `localhost:6379`          |
| `REDIS_DB`             | Redis database number          | `12`                      |

## ⚡ Redis Caching

The service uses Redis caching to improve performance and reduce database load. Caching is applied to frequently accessed endpoints, including addresses, holidays, and exchange rates.

### Redis Caching Behavior

- **Cache Key:** Request URI and query parameters.
- **Cache Expiration:**
  - Addresses: 7 days
  - Exchange Rates: 4 hours
  - Default: 12 hours
- **Request Parameter:**
  - `cache-control: no-cache` - the response can be cached but must be revalidated by the origin server before reuse.
  - `cache-control: no-store` - any caches should not store this response.

### 🐳 Local Redis Installation Using Docker

To run Redis locally using Docker, use the following command:

```bash
docker run --name redis -p 6379:6379 -d redis:latest
```

This command pulls the latest Redis Docker image and runs it in detached mode, exposing it on port `6379`.

## 🏦 Common Database (MySQL) for Address API

The Address API relies on a MySQL-based `common-db` to store and retrieve address-related data. The database contains essential tables such as provinces, districts, sub-districts, and zip codes.

### 🐋 Local MySQL Setup Using Docker

To run a local MySQL database using Docker, use the following command:

```bash
docker run --name common-db -e MYSQL_ROOT_PASSWORD=yourpassword -p 3306:3306 -d mysql:latest
```

- **Database Host:** `localhost`
- **Database Port:** `3306`
- **Default Database Name:** `common_db`
- **Database User:** `root`
- **Password:** Set as specified in the Docker command.

Ensure the environment variable `COMMON_DB_DSN` in the project points to this database connection string:

```plaintext
user:password@tcp(localhost:3306)/common_db?charset=utf8mb4&parseTime=True&loc=Local
```


## 📦 Development Integration

Client libraries are available for seamless integration:

- **Node.js:** [information-srv-client-axios](https://github.com/orgs/pi-financial/packages/npm/package/information-srv-client-axios)
- **C#:** [Pi.Client.InformationSrv](https://github.com/orgs/pi-financial/packages/nuget/package/Pi.Client.InformationSrv)


## 🚢 Deployment

The service supports deployments for both **Staging** and **Production** environments.


### ⚙️ Deployment Strategy

- **Staging Environment:**
  - Used for testing and quality assurance.
  - Deployment triggered by merging changes to the `main` branch.
  - Accessible at `http://information.nonprod.pi.internal/swagger/index.html`.

- **Production Environment:**
  - Used for live, customer-facing operations.
  - Deployment triggered by releasing a new version.
  - Accessible at `http://information.prod.pi.internal/swagger/index.html`.
