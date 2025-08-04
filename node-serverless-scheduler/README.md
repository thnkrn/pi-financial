# Serverless - AWS Node.js Typescript

This project has been generated using the `aws-nodejs-typescript` template from the [Serverless framework](https://www.serverless.com/).

For detailed instructions, please refer to the [documentation](https://www.serverless.com/framework/docs/providers/aws/).

## Installation/deployment instructions

Depending on your preferred package manager, follow the instructions below to deploy your project.

> **Requirements**: NodeJS `lts/fermium (v.14.15.0)`. If you're using [nvm](https://github.com/nvm-sh/nvm), run `nvm use` to ensure you're using the same Node version in local and in your lambda's runtime.

### Using NPM

- Run `npm i` to install the project dependencies
- Run `npx sls deploy` to deploy this stack to AWS

### Using Yarn

- Run `yarn install` to install the project dependencies
- Run `yarn sls deploy` to deploy this stack to AWS

## Test your service

This template contains a single lambda function triggered by an HTTP request made on the provisioned API Gateway REST API `/hello` route with `POST` method. The request body must be provided as `application/json`. The body structure is tested by API Gateway against `src/functions/hello/schema.ts` JSON-Schema definition: it must contain the `name` property.

- requesting any other path than `/hello` with any other method than `POST` will result in API Gateway returning a `403` HTTP error code
- sending a `POST` request to `/hello` with a payload **not** containing a string property named `name` will result in API Gateway returning a `400` HTTP error code
- sending a `POST` request to `/hello` with a payload containing a string property named `name` will result in API Gateway returning a `200` HTTP status code with a message saluting the provided name and the detailed event processed by the lambda

> :warning: As is, this template, once deployed, opens a **public** endpoint within your AWS account resources. Anybody with the URL can actively execute the API Gateway endpoint and the corresponding lambda. You should protect this endpoint with the authentication method of your choice.

### Locally

In order to test the hello function locally, run the following command:

- `npx sls invoke local -f hello --path src/functions/hello/mock.json` if you're using NPM
- `yarn sls invoke local -f hello --path src/functions/hello/mock.json` if you're using Yarn

Check the [sls invoke local command documentation](https://www.serverless.com/framework/docs/providers/aws/cli-reference/invoke-local/) for more information.

### Enabling connection to AWS staging with SSO

Please follow the steps below:

- run `yarn install` to install dependencies.
- [Important] Install JRE version >=6.x. Refer: https://www.java.com/en/download/manual.jsp. (Please ignore if already executed)
- run `rm -rf ~/.aws/` (For fresh installation)
- Open Terminal. Enter the command `aws configure sso`. (Please ignore if already executed)

```
% aws configure sso
SSO session name (Recommended): Pi-Developer-Workload-NonProd
SSO start URL [None]: `https://d-966769ada8.awsapps.com/start#`
SSO region [None]: `ap-southeast-1`
SSO registration scopes [sso:account:access]: `sso:account:access`
```

- Authorize from browser.
- Back to Terminal and choose Workload Non-Prod AWS Account ID: 754259340832.

- There are 5 AWS accounts available to you.

```
Using the account ID 754259340832
The only role available to you is: Pi-Developer-Workload-NonProd
Using the role name "Pi-Developer-Workload-NonProd"
CLI default client Region [None]: ap-southeast-1
CLI default output format [None]: json
CLI profile name [Pi-Developer-Workload-NonProd-754259340832]: Pi-Developer-Workload-NonProd-754259340832
```

- Now, run `cat ~/.aws/config`. Output should be as below:

```
[profile Pi-Developer-Workload-NonProd-754259340832]
sso_session = Pi-Developer-Workload-NonProd
sso_account_id = 754259340832
sso_role_name = Pi-Developer-Workload-NonProd
region = ap-southeast-1
output = json
[sso-session Pi-Developer-Workload-NonProd]
sso_start_url = https://d-966769ada8.awsapps.com/start#
sso_region = ap-southeast-1
sso_registration_scopes = sso:account:access
```

To use this profile, specify the profile name using --profile, as shown:

`aws s3 ls --profile Pi-Developer-Workload-NonProd-754259340832`

### Run Step Function Locally

Open a first Terminal and execute start step function server below

- run `yarn run startLocal` to trigger command on package.json (`yarn run startLocalWithoutLogin` after you've got the SSO session)
- Authorize from browser.
- Step Functions Local will be started at: http://localhost:8083 and server will be ready at http://127.0.0.1:3000.

Open a new Terminal and execute state machine using ARN.

- run `aws stepfunctions start-execution --endpoint-url http://localhost:8083 --state-machine-arn arn:aws:states:ap-southeast-1:101010101010:stateMachine:pi-staging-SampleStateMachine --profile Pi-Developer-Workload-NonProd-754259340832`

- ExecutionArn and StartDate can be seen as the response.

```
% aws stepfunctions start-execution --endpoint-url http://localhost:8083 --state-machine-arn arn:aws:states:ap-southeast-1:101010101010:stateMachine:pi-staging-SampleStateMachine --profile Pi-Developer-Workload-NonProd-754259340832
{
    "executionArn": "arn:aws:states:ap-southeast-1:101010101010:execution:pi-staging-SampleStateMachine:aad40c83-8bee-4873-a4e6-638af0b31575",
    "startDate": "2024-03-29T01:23:01.665000+07:00"
}
```

- Here `101010101010` and `ap-southeast-1` are the fake account ID and region respectively used to execute the step function locally.

You can see output in server console.

### Remotely

Copy and replace your `url` - found in Serverless `deploy` command output - and `name` parameter in the following `curl` command in your terminal or in Postman to test your newly deployed application.

```
curl --location --request POST 'https://myApiEndpoint/dev/hello' \
--header 'Content-Type: application/json' \
--data-raw '{
    "name": "Frederic"
}'
```

## Template features

### Project structure

The project code base is mainly located within the `src` folder. This folder is divided in:

- `functions` - containing code base and configuration for your lambda functions
- `libs` - containing shared code base between your lambdas

```
.
├── src
│   ├── functions               # Lambda configuration and source code folder
│   │   ├── hello
│   │   │   ├── handler.ts      # `Hello` lambda source code
│   │   │   ├── index.ts        # `Hello` lambda Serverless configuration
│   │   │   ├── mock.json       # `Hello` lambda input parameter, if any, for local invocation
│   │   │   └── schema.ts       # `Hello` lambda input event JSON-Schema
│   │   │
│   │   └── index.ts            # Import/export of all lambda configurations
│   │
│   └── libs                    # Lambda shared code
│       └── apiGateway.ts       # API Gateway specific helpers
│       └── handlerResolver.ts  # Sharable library for resolving lambda handlers
│       └── lambda.ts           # Lambda middleware
│
├── package.json
├── serverless.ts               # Serverless service file
├── tsconfig.json               # Typescript compiler configuration
├── tsconfig.paths.json         # Typescript paths
└── webpack.config.js           # Webpack configuration
```

### 3rd party libraries

- [json-schema-to-ts](https://github.com/ThomasAribart/json-schema-to-ts) - uses JSON-Schema definitions used by API Gateway for HTTP request validation to statically generate TypeScript types in your lambda's handler code base
- [middy](https://github.com/middyjs/middy) - middleware engine for Node.Js lambda. This template uses [http-json-body-parser](https://github.com/middyjs/middy/tree/master/packages/http-json-body-parser) to convert API Gateway `event.body` property, originally passed as a stringified JSON, to its corresponding parsed object
- [@serverless/typescript](https://github.com/serverless/typescript) - provides up-to-date TypeScript definitions for your `serverless.ts` service file

### Advanced usage

Any tsconfig.json can be used, but if you do, set the environment variable `TS_NODE_CONFIG` for building the application, eg `TS_NODE_CONFIG=./tsconfig.app.json npx serverless webpack`

### Migrations

To run migration commands, you can look at db/Makefile

e.g.

```bash
make add-report
```

For adding migration we use sequelize. While creating a new migration file, we also need to add its sql script to run in production later by devops. We can generate the SQL script via the following command. Firstly run mysql in docker container and run the following steps :

1. docker-compose up -d // Up your database instance
2. go to `db` dir
3. Update `config.js`
4. Run `make scripts`
5. Script will be generated inside `migrations/scripts/*` dir
