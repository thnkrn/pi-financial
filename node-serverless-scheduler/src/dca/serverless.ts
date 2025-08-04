import { runtime } from '@libs/lambda';
import { getVpcConfig, getVpcEndpoints } from '@libs/vpc-utils';
import type { AWS } from '@serverless/typescript';
import { fundTriggerCronjob } from 'src/dca/functions';

export const serviceName = 'dca-service';

const serverlessConfiguration: AWS = {
  service: serviceName,
  frameworkVersion: '3',
  plugins: [
    'serverless-better-credentials',
    'serverless-plugin-datadog',
    'serverless-esbuild',
    'serverless-offline-lambda',
    'serverless-offline',
    'serverless-ssm-fetch',
  ],
  provider: {
    name: 'aws',
    endpointType: 'PRIVATE',
    vpcEndpointIds: getVpcEndpoints(process.env.AWS_ENVIRONMENT),
    runtime: runtime.node20,
    region: 'ap-southeast-1',
    iamRoleStatements: [
      {
        Effect: 'Allow',
        Action: ['ssm:GetParameters'],
        Resource: `arn:aws:ssm:$\{aws:region}:*:parameter/${process.env.AWS_ENVIRONMENT}/pi/functions/${serviceName}/*`,
      },
      {
        Effect: 'Allow',
        Action: ['secretsmanager:GetSecretValue'],
        Resource: `*`,
      },
    ],
    stackTags: {
      Project: serviceName,
    },
    apiGateway: {
      minimumCompressionSize: 1024,
      shouldStartNameWithService: true,
      resourcePolicy: [
        {
          Effect: 'Allow',
          Principal: '*',
          Action: ['execute-api:Invoke'],
          Resource: 'execute-api:/*/*/*',
        },
      ],
    },
    environment: {
      AWS_NODEJS_CONNECTION_REUSE_ENABLED: '1',
      NODE_OPTIONS: '--enable-source-maps --stack-trace-limit=1000',
      ENVIRONMENT: process.env.AWS_ENVIRONMENT,
      EXPIRES_IN: process.env.AWS_EXPIRES_IN || '86400',
      API_TIMEOUT: process.env.AWS_API_TIMEOUT || '100000',
      DEBUG_LOCAL: process.env.DEBUG_LOCAL || 'false',
    },
    vpc: getVpcConfig(process.env.AWS_ENVIRONMENT),
  },
  functions: {
    fundTriggerCronjob,
  },
  package: {
    individually: true,
  },
  custom: {
    'serverless-offline': {
      host: '127.0.0.1',
    },
    serverlessSsmFetch: {
      ELASTIC_APM_LAMBDA_APM_SERVER: `/${process.env.AWS_ENVIRONMENT}/pi/functions/common/apm-server-url`,
      ELASTIC_APM_SECRET_TOKEN: `/${process.env.AWS_ENVIRONMENT}/pi/functions/common/apm-secret-token~true`,
    },
    datadog: {
      site: process.env.DD_SITE,
      apiKeySecretArn: process.env.DATADOG_API_KEY_SECRET_ARN,
      env: process.env.AWS_ENVIRONMENT,
      service: serviceName,
      tags: `project:${serviceName}`,
      forwarderArn: process.env.DATADOG_FORWARDER_ARN,
      enableDDTracing: true,
      enabled: process.env.DEBUG_LOCAL !== 'true',
      enableDDLogs: true,
      captureLambdaPayload: true,
      subscribeToAccessLogs: true,
      subscribeToExecutionLogs: true,
      mergeStepFunctionAndLambdaTraces: false,
      addLayers: true,
      addExtension: true,
      enableStepFunctionsTracing: false,
      // NOTE: enableXrayTracing is not supported for now https://github.com/pi-financial/node-serverless-scheduler/pull/179
      enableXrayTracing: false,
      enableProfiling: true,
    },
    esbuild: {
      bundle: true,
      minify: false,
      sourcemap: true,
      exclude: ['aws-sdk', 'pg-hstore'],
      define: { 'require.resolve': undefined },
      platform: 'node',
      concurrency: 10,
    },
  },
};

module.exports = serverlessConfiguration;
