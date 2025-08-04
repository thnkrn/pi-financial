import type { AWS } from '@serverless/typescript';
import jwtAuth from '@cgs-bank/utils/jwtAuth';
import authentication from '@cgs-bank/functions/authentication/index';
import scbFunctions from '@cgs-bank/functions/scb-payment';
import kbankFunctions from '@cgs-bank/functions/kbank-payment';
import kkpFunctions from '@cgs-bank/functions/kkp';
import bayFunctions from '@cgs-bank/functions/bay-payment';
import * as process from 'node:process';

const env = process.env.AWS_ENVIRONMENT == 'staging' ? 'uat' : 'prod';
const serverlessConfiguration: AWS = {
  service: 'cgs-bank',
  frameworkVersion: '3',
  variablesResolutionMode: '20210326',
  package: { individually: true },
  custom: {
    dynamodbApiCacheTableName: 'api-cache-table',
    kkpApiCachedKey: 'kkp-api-cache',
    esbuild: {
      bundle: true,
      minify: true,
      sourcemap: true,
      exclude: ['aws-sdk', 'pg-native'],
      target: 'node18',
      define: { 'require.resolve': undefined },
      platform: 'node',
    },
    prune: {
      automatic: true,
      number: 3,
    },
    serverlessSsmFetch: {
      ELASTIC_APM_LAMBDA_APM_SERVER: `/${process.env.AWS_ENVIRONMENT}/pi/functions/common/apm-server-url`,
      ELASTIC_APM_SECRET_TOKEN: `/${process.env.AWS_ENVIRONMENT}/pi/functions/common/apm-secret-token~true`,
    },
  },
  plugins: [
    'serverless-esbuild',
    'serverless-offline',
    'serverless-iam-roles-per-function',
    'serverless-ssm-fetch',
  ],
  provider: {
    name: 'aws',
    region: 'ap-southeast-1',
    endpointType: 'PRIVATE',
    timeout: 30,
    vpcEndpointIds: [`$\{ssm:/vpc/endpoints/apigateway-vpce/endpoint-id}`],
    iamRoleStatements: [
      {
        Effect: 'Allow',
        Action: ['ssm:GetParameters', 'secretsmanager:GetSecretValue'],
        Resource: '*',
      },
    ],
    vpc: {
      securityGroupIds: [`$\{ssm:/vpc/security-groups/cgs-bank-lambda-sg/id}`],
      subnetIds: [
        `$\{ssm:/vpc/subnets/cgs-workload-app-a}`,
        `$\{ssm:/vpc/subnets/cgs-workload-app-b}`,
        `$\{ssm:/vpc/subnets/cgs-workload-app-c}`,
      ],
    },
    runtime: 'nodejs18.x',
    stackTags: {
      Project: 'cgs-bank',
    },
    apiGateway: {
      apiKeys: [
        {
          internal: [
            {
              name: `$\{sls:stage}-api-key`,
              value: `$\{ssm:/${env}/cgs/bank-services/internal-api-key}`,
              description: 'key for sirius',
              customerId: 'sirius',
            },
          ],
        },
      ],
      usagePlan: [
        {
          internal: {
            quota: { limit: 5000, offset: 2, period: 'MONTH' },
            throttle: { burstLimit: 200, rateLimit: 100 },
          },
        },
      ],
      minimumCompressionSize: 1024,
      shouldStartNameWithService: true,
      resourcePolicy: [
        {
          Effect: 'Allow',
          Principal: '*',
          Action: 'execute-api:Invoke',
          Resource: ['execute-api:/*'],
        },
      ],
    },
    environment: {
      ENVIRONMENT: process.env.AWS_ENVIRONMENT,
      INTERNAL_API_KEY: `$\{ssm:/${env}/cgs/bank-services/internal-api-key}`,
      JOB_QUEUE_NAME: `$\{self:service}-srv-${env}-job-queue`,
      JOB_DEFINITION_WORKER: `$\{self:service}-srv-${env}-job-definition-bank-srv-daily-job`,
      DB_DIALECT: 'postgres',
      DB_NAME: `$\{ssm:/${env}/cgs/bank-services/db-name}`,
      DB_USER: `$\{ssm:/${env}/cgs/bank-services/db-user}`,
      DB_PASSWORD: `$\{ssm:/${env}/cgs/bank-services/db-password}`,
      DB_HOST: `$\{ssm:/${env}/cgs/bank-services/db-host}`,
      DB_PORT: `$\{ssm:/${env}/cgs/bank-services/db-port}`,
      AWS_DYNAMODB_API_CACHE_TABLE_NAME: `$\{self:service}-srv-${env}-$\{self:custom.dynamodbApiCacheTableName}`,
      KKP_API_CACHE_KEY: `$\{self:custom.kkpApiCachedKey}`,
      AWS_NODEJS_CONNECTION_REUSE_ENABLED: '1',
      AES_SECRET_KEY: `$\{ssm:/${env}/cgs/bank-services/aes-secret-key}`,
      JWT_SIGNATURE: `$\{ssm:/${env}/cgs/bank-services/jwt-signature}`,
      SCB_PUBLIC_KEY: `/${env}/cgs/bank-services/scb/public-key`,
      KKP_PARTNER_NAME: 'CGS',
    },
    lambdaHashingVersion: '20201221',
  },
  // import the function via paths
  functions: {
    jwtAuth,
    authentication,
    ...scbFunctions,
    ...kbankFunctions,
    ...kkpFunctions,
    ...bayFunctions,
  },
  resources: {
    Resources: {
      apiCacheTable: {
        Type: 'AWS::DynamoDB::Table',
        Properties: {
          TableName: `$\{self:provider.environment.AWS_DYNAMODB_API_CACHE_TABLE_NAME}`,
          AttributeDefinitions: [{ AttributeName: 'key', AttributeType: 'S' }],
          KeySchema: [{ AttributeName: 'key', KeyType: 'HASH' }],
          TimeToLiveSpecification: {
            AttributeName: 'expireAt',
            Enabled: true,
          },
          BillingMode: 'PAY_PER_REQUEST',
        },
      },
      ComputeEnvironment: {
        Type: 'AWS::Batch::ComputeEnvironment',
        Properties: {
          Type: 'MANAGED',
          ServiceRole: `arn:aws:iam::$\{aws:accountId}:role/AWSBatchServiceRole`,
          ComputeEnvironmentName: `$\{self:service}-srv-$\{sls:stage}-compute-env`,
          ComputeResources: {
            MaxvCpus: '256',
            SecurityGroupIds: [
              `$\{ssm:/vpc/security-groups/$\{self:service}-srv-batch-sg/id}`,
            ],
            Type: 'FARGATE',
            Subnets: [
              `$\{ssm:/vpc/subnets/cgs-workload-app-a}`,
              `$\{ssm:/vpc/subnets/cgs-workload-app-b}`,
              `$\{ssm:/vpc/subnets/cgs-workload-app-c}`,
            ],
          },
          State: 'ENABLED',
        },
      },
      JobQueue: {
        Type: 'AWS::Batch::JobQueue',
        Properties: {
          JobQueueName: `$\{self:provider.environment.JOB_QUEUE_NAME}`,
          ComputeEnvironmentOrder: [
            {
              Order: '1',
              ComputeEnvironment: { Ref: 'ComputeEnvironment' },
            },
          ],
          State: 'ENABLED',
          Priority: '1',
        },
      },
      WorkerJobDefinition: {
        Type: 'AWS::Batch::JobDefinition',
        Properties: {
          Type: 'container',
          JobDefinitionName: `$\{self:provider.environment.JOB_DEFINITION_WORKER}`,
          PlatformCapabilities: ['FARGATE'],
          Timeout: {
            AttemptDurationSeconds: 28800,
          },
          RetryStrategy: {
            Attempts: 3,
          },
          ContainerProperties: {
            Command: ['node', 'reconcile-job.js'],
            Environment: [
              {
                Name: 'DB_DIALECT',
                Value: 'postgres',
              },
              {
                Name: 'KKP_PARTNER_NAME',
                Value: 'CGS',
              },
            ],
            Image: `$\{aws:accountId}.dkr.ecr.$\{self:provider.region}.amazonaws.com/$\{self:service}-srv-daily-job-$\{sls:stage}:latest`,
            FargatePlatformConfiguration: {
              PlatformVersion: 'LATEST',
            },
            JobRoleArn: { 'Fn::GetAtt': ['BankEcsTaskExecutionRole', 'Arn'] },
            ExecutionRoleArn: {
              'Fn::GetAtt': ['BankEcsTaskExecutionRole', 'Arn'],
            },
            ResourceRequirements: [
              {
                Type: 'VCPU',
                Value: '0.5',
              },
              {
                Type: 'MEMORY',
                Value: '1024',
              },
            ],
            Secrets: [
              {
                Name: 'DB_NAME',
                ValueFrom: `arn:aws:ssm:$\{self:provider.region}:$\{aws:accountId}:parameter/${env}/cgs/bank-services/db-name`,
              },
              {
                Name: 'DB_USER',
                ValueFrom: `arn:aws:ssm:$\{self:provider.region}:$\{aws:accountId}:parameter/${env}/cgs/bank-services/db-user`,
              },
              {
                Name: 'DB_PASSWORD',
                ValueFrom: `arn:aws:ssm:$\{self:provider.region}:$\{aws:accountId}:parameter/${env}/cgs/bank-services/db-password`,
              },
              {
                Name: 'DB_HOST',
                ValueFrom: `arn:aws:ssm:$\{self:provider.region}:$\{aws:accountId}:parameter/${env}/cgs/bank-services/db-host`,
              },
              {
                Name: 'DB_PORT',
                ValueFrom: `arn:aws:ssm:$\{self:provider.region}:$\{aws:accountId}:parameter/${env}/cgs/bank-services/db-port`,
              },
            ],
          },
        },
      },
      BankEcsTaskExecutionRole: {
        Type: 'AWS::IAM::Role',
        Properties: {
          RoleName: `$\{self:service}-$\{sls:stage}-ecsTaskExecutionRole`,
          Path: '/',
          AssumeRolePolicyDocument: {
            Version: '2012-10-17',
            Statement: [
              {
                Effect: 'Allow',
                Principal: {
                  Service: ['batch.amazonaws.com', 'ecs-tasks.amazonaws.com'],
                },
                Action: ['sts:AssumeRole'],
              },
            ],
          },
          Policies: [
            {
              PolicyName: 'InlinePolicyForReconcileEcsTaskExecution',
              PolicyDocument: {
                Version: '2012-10-17',
                Statement: [
                  {
                    Effect: 'Allow',
                    Action: [
                      'ecr:GetAuthorizationToken',
                      'ecr:BatchCheckLayerAvailability',
                      'ecr:GetDownloadUrlForLayer',
                      'ecr:BatchGetImage',
                      'logs:CreateLogStream',
                      'logs:PutLogEvents',
                    ],
                    Resource: '*',
                  },
                  {
                    Effect: 'Allow',
                    Action: ['ssm:GetParameter*'],
                    Resource: [
                      `arn:aws:ssm:$\{self:provider.region}:$\{aws:accountId}:parameter/${env}/cgs/bank-services/db-name`,
                      `arn:aws:ssm:$\{self:provider.region}:$\{aws:accountId}:parameter/${env}/cgs/bank-services/db-user`,
                      `arn:aws:ssm:$\{self:provider.region}:$\{aws:accountId}:parameter/${env}/cgs/bank-services/db-password`,
                      `arn:aws:ssm:$\{self:provider.region}:$\{aws:accountId}:parameter/${env}/cgs/bank-services/db-host`,
                      `arn:aws:ssm:$\{self:provider.region}:$\{aws:accountId}:parameter/${env}/cgs/bank-services/db-port`,
                    ],
                  },
                  {
                    Effect: 'Allow',
                    Action: [
                      'kms:Encrypt',
                      'kms:Decrypt',
                      'kms:GenerateDataKey',
                    ],
                    Resource: `$\{ssm:/kms/security/arn}`,
                  },
                ],
              },
            },
          ],
        },
      },
    },
  },
};

module.exports = serverlessConfiguration;
