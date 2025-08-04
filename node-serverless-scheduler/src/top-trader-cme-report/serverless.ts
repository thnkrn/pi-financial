import { runtime } from '@libs/lambda';
import type {
  AWS,
  AwsCfArrayInstruction,
  AwsLambdaVpcConfig,
} from '@serverless/typescript';
import * as Functions from 'src/top-trader-cme-report/functions';
import * as StateMachines from 'src/top-trader-cme-report/stateMachines';

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

export const serviceName = 'top-trader-cme-report';
const serverlessConfiguration: ServerlessWithStepFunctions = {
  service: serviceName,
  frameworkVersion: '3',
  plugins: [
    'serverless-better-credentials',
    'serverless-plugin-datadog',
    'serverless-esbuild',
    'serverless-step-functions',
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
          'secretsmanager:GetSecretValue',
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
              events: funcConfig.events.filter((event) => !event.schedule),
            }
          : funcConfig;

      return [funcName, updatedFuncConfig];
    })
  ),
  stepFunctions: {
    stateMachines: {
      ...Object.entries(StateMachines).reduce(
        (res, [machineName, machineConfig]) => ({
          ...res,
          [machineName]: machineConfig,
        }),
        {}
      ),
    },
  },
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
      loader: { '.node': 'file' },
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

export type Definition = {
  Comment?: string;
  StartAt: string;
  States: States;
};

export type States = {
  [state: string]: {
    Catch?: Catcher[];
    Type: 'Map' | 'Task' | 'Choice' | 'Pass' | 'Wait' | 'Fail' | 'Succeed';
    End?: boolean;
    Result?:
      | string
      | {
          [parameter: string]: string | number;
        };
    Retry?: Retry[];
    Next?: string;
    Seconds?: number;
    ItemsPath?: string;
    ResultPath?: string;
    OutputPath?: string;
    Output?: string;
    SecondsPath?: string;
    Resource?: string | { 'Fn::GetAtt': string[] };
    Parameters?: {
      [key: string]:
        | string
        | string[]
        | { [key: string]: string | { [key: string]: string } };
    };
    ResultSelector?: {
      [key: string]: string | { [key: string]: string };
    };
    Iterator?: Definition;
    ItemProcessor?: {
      ProcessorConfig?: {
        Mode: 'DISTRIBUTED' | 'INLINE';
        ExecutionType: string;
      };
      StartAt: string;
      States: States;
    };
    ItemReader?: {
      Resource?: string;
      ReaderConfig: {
        InputType: string;
        CSVHeaderLocation: string;
        CSVDelimiter?: string;
      };
      Parameters?: {
        [parameter: string]: string;
        Bucket?: string;
        Key?: string;
      };
    };
    Credentials?: {
      [key: string]: string | { 'Fn::GetAtt': string[] };
    };
    MaxConcurrency?: number;
    ResultWriter?: {
      Resource?: string;
      Parameters?: {
        [parameter: string]: string;
        Bucket: string;
        Key: string;
      };
    };
    Choices?: {
      Variable: string;
      StringEquals?: string;
      Next: string;
      NumericEquals?: number;
    }[];
    Default?: string;
    Cause?: string;
    Error?: string;
  };
};

type Catcher = {
  ErrorEquals: ErrorName[];
  Next: string;
  ResultPath?: string;
};

type Retry = {
  ErrorEquals: ErrorName[];
  MaxAttempts?: number;
  BackoffRate?: number;
  IntervalSeconds?: number;
  MaxDelaySeconds?: number;
};

type ErrorName =
  | 'States.ALL'
  | 'States.DataLimitExceeded'
  | 'States.Runtime'
  | 'States.Timeout'
  | 'States.TaskFailed'
  | 'States.Permissions'
  | string;

interface ServerlessWithStepFunctions extends AWS {
  stepFunctions: {
    stateMachines: {
      [stateMachine: string]: {
        events: Record<string, unknown>[];
        name: string;
        definition: Definition;
      };
    };
    activities?: string[];
    validate?: boolean;
  };
}

module.exports = serverlessConfiguration;
