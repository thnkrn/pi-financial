import { runtime } from '@libs/lambda';
import type {
  AWS,
  AwsCfArrayInstruction,
  AwsLambdaVpcConfig,
} from '@serverless/typescript';
import * as Functions from 'src/bond-thai-bma-instruments/functions';

function getVpcConfig(env?: string): AwsLambdaVpcConfig {
  switch (env) {
    case 'staging':
      return {
        securityGroupIds: ['sg-080c7fdc8e934d37a'],
        subnetIds: [
          'subnet-09093b66093419b0b',
          'subnet-0350c638f838e89ff',
          'subnet-001a008e75d705c55',
        ],
      };
    case 'production':
      return {
        securityGroupIds: ['sg-0cf9d3fc567aec40c'],
        subnetIds: [
          'subnet-006d8e77661411fcc',
          'subnet-0eba9c62e39b04913',
          'subnet-0938dc796f3fadc91',
        ],
      };
    default:
      throw new Error('Missing "AWS_ENVIRONMENT" in env');
  }
}

function getVpcEndpoints(env?: string): AwsCfArrayInstruction {
  switch (env) {
    case 'staging':
      return ['vpce-0090277c15ce6a287'];
    case 'production':
      return ['vpce-09460603674ada0c3'];
    default:
      throw new Error('Missing "AWS_ENVIRONMENT" in env');
  }
}

export const serviceName = 'bond-thai-bma-instruments';
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
        Action: [
          'ssm:GetParameters',
          's3:PutObject',
          's3:GetObject',
          's3:ListBucket',
          's3:DeleteObject',
        ],
        Resource: '*',
      },
    ],
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
  functions: Object.fromEntries(
    Object.entries(Functions).map(([funcName, funcConfig]) => {
      const updatedFuncConfig =
        process.env.DEBUG_LOCAL === 'true'
          ? {
              ...funcConfig,
              // events: funcConfig.events.filter((event) => !event.schedule),
            }
          : funcConfig;

      return [funcName, updatedFuncConfig];
    })
  ),
  package: {
    individually: true,
    exclude: ['node_modules/puppeteer/.local-chromium/**'],
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
      service: 'node-serverless-scheduler',
      tags: 'project:node-serverless-scheduler',
      forwarderArn: process.env.DATADOG_FORWARDER_ARN,
      enableDDTracing: true,
      enabled: process.env.DEBUG_LOCAL !== 'true',
      enableDDLogs: true,
      captureLambdaPayload: true,
      subscribeToAccessLogs: true,
      subscribeToExecutionLogs: true,
      propagateUpstreamTrace: true,
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
