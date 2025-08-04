import { Definition } from '../../serverless';
import { ReportStatus } from '../constants/report';

const settings = {
  sourceBucket:
    process.env.AWS_ENVIRONMENT === 'production'
      ? 'pi-exante-reports-prod'
      : 'pi-exante-reports-nonprod',
  reportBucket: `backoffice-reports-${process.env.AWS_ENVIRONMENT}`,
  velexaReportPrefix: 'QLO7111_account_summary',
  velexaReportDir: 'velexa/account-summary/',
  bloombergReportPrefix: 'equity_closeprice',
  bloombergReportDir: 'bloomberg/',
  bloombergReportName: 'Bloomberg Equity Closeprice',
  timezone: 'america/new_york',
  timeFrom: '17:00:00',
  timeTo: '17:00:00',
};

const definition: Definition = {
  StartAt: 'FindAccountSummaryFile',
  States: {
    FindAccountSummaryFile: {
      Type: 'Task',
      ResultPath: '$.accountSummaryFileResult',
      Parameters: {
        body: {
          reportPrefix: settings.velexaReportPrefix,
          bucket: settings.sourceBucket,
          'date.$': '$.time',
          timezone: settings.timezone,
        },
      },
      Resource: { 'Fn::GetAtt': ['findS3File', 'Arn'] },
      Next: 'ProcessAccountSummary',
      Retry: [
        {
          ErrorEquals: ['Error'],
          IntervalSeconds: 60,
          BackoffRate: 2,
          MaxAttempts: 2,
        },
      ],
      Catch: [
        {
          ErrorEquals: ['States.ALL'],
          Next: 'FallbackReportFailed',
          ResultPath: null,
        },
      ],
    },
    ProcessAccountSummary: {
      Type: 'Task',
      ResultPath: '$.processAccountSummaryResult',
      Parameters: {
        body: {
          'bucket.$': '$.accountSummaryFileResult.body.bucket',
          'key.$': '$.accountSummaryFileResult.body.key',
        },
      },
      Resource: {
        'Fn::GetAtt': ['processAccountSummary', 'Arn'],
      },
      Next: 'RequestEquityClosePrice',
      Retry: [
        {
          ErrorEquals: ['Error'],
          IntervalSeconds: 60,
          BackoffRate: 2,
          MaxAttempts: 2,
        },
      ],
      Catch: [
        {
          ErrorEquals: ['States.ALL'],
          Next: 'FallbackReportFailed',
          ResultPath: null,
        },
      ],
    },
    RequestEquityClosePrice: {
      Type: 'Task',
      ResultPath: '$.requestEquityClosePriceResult',
      Parameters: {
        body: {
          'securityIdentifiers.$':
            '$.processAccountSummaryResult.body.securityIdentifiers',
        },
      },
      Resource: {
        'Fn::GetAtt': ['requestEquityClosePrice', 'Arn'],
      },
      Next: 'InitializeCounter',
      Retry: [
        {
          ErrorEquals: ['Error'],
          IntervalSeconds: 60,
          BackoffRate: 2,
          MaxAttempts: 2,
        },
      ],
      Catch: [
        {
          ErrorEquals: ['States.ALL'],
          Next: 'FallbackReportFailed',
          ResultPath: null,
        },
      ],
    },
    InitializeCounter: {
      Type: 'Pass',
      Result: {
        counter: 0,
      },
      ResultPath: '$.counter',
      Next: 'Wait5Seconds',
    },
    Wait5Seconds: {
      Type: 'Wait',
      Next: 'GetRequestStatus',
      Seconds: 5,
    },
    GetRequestStatus: {
      Type: 'Task',
      ResultPath: '$.requestStatusResult',
      Parameters: {
        body: {
          'identifier.$': '$.requestEquityClosePriceResult.body.identifier',
        },
      },
      Resource: {
        'Fn::GetAtt': ['getRequestStatus', 'Arn'],
      },
      Next: 'CheckRequestStatus',
      Retry: [
        {
          ErrorEquals: ['Error'],
          IntervalSeconds: 60,
          BackoffRate: 2,
          MaxAttempts: 2,
        },
      ],
      Catch: [
        {
          ErrorEquals: ['States.ALL'],
          Next: 'FallbackReportFailed',
          ResultPath: null,
        },
      ],
    },
    CheckRequestStatus: {
      Type: 'Choice',
      Choices: [
        {
          Variable: '$.requestStatusResult.body.status',
          StringEquals: 'FAILED',
          Next: 'FallbackReportFailed',
        },
        {
          Variable: '$.counter',
          NumericEquals: 20,
          Next: 'FallbackReportFailed',
        },
        {
          Variable: '$.requestStatusResult.body.status',
          StringEquals: 'SUCCEEDED',
          Next: 'ProcessEquityClosePrice',
        },
      ],
      Default: 'IncrementCounter',
    },
    IncrementCounter: {
      Type: 'Pass',
      Result: {
        'counter.$': '$.counter + 1',
      },
      ResultPath: '$.counter',
      Next: 'Wait5Seconds',
    },
    ProcessEquityClosePrice: {
      Type: 'Task',
      ResultPath: '$.processEquityClosePrice',
      Parameters: {
        body: {
          'identifier.$': '$.requestEquityClosePriceResult.body.identifier',
          'date.$': '$.accountSummaryFileResult.body.date',
        },
      },
      Next: 'FindBloombergFile',
      Resource: {
        'Fn::GetAtt': ['processEquityClosePrice', 'Arn'],
      },
      Retry: [
        {
          ErrorEquals: ['Error'],
          IntervalSeconds: 60,
          BackoffRate: 2,
          MaxAttempts: 2,
        },
      ],
      Catch: [
        {
          ErrorEquals: ['States.ALL'],
          Next: 'FallbackReportFailed',
          ResultPath: null,
        },
      ],
    },
    FindBloombergFile: {
      Type: 'Task',
      Parameters: {
        body: {
          reportPrefix: settings.bloombergReportPrefix,
          bucket: settings.sourceBucket,
          'date.$': '$.accountSummaryFileResult.body.date',
        },
      },
      Resource: { 'Fn::GetAtt': ['findS3File', 'Arn'] },
      Next: 'FallbackReportSuccess',
      Retry: [
        {
          ErrorEquals: ['Error'],
          IntervalSeconds: 60,
          BackoffRate: 2,
          MaxAttempts: 2,
        },
      ],
      ResultPath: null,
      Catch: [
        {
          ErrorEquals: ['States.ALL'],
          Next: 'FallbackReportFailed',
          ResultPath: null,
        },
      ],
    },
    FallbackReportSuccess: {
      Type: 'Pass',
      Next: 'TransformData',
      Parameters: {
        status: ReportStatus.Done,
        'filename.$': `States.Format('${settings.bloombergReportDir}${settings.bloombergReportPrefix}_{}.csv', $.accountSummaryFileResult.body.date)`,
        'date.$': '$.time',
      },
      ResultPath: '$.payload',
    },
    FallbackReportFailed: {
      Type: 'Pass',
      Next: 'TransformData',
      Parameters: {
        status: ReportStatus.Failed,
        filename: null,
        'date.$': '$.time',
      },
      ResultPath: '$.payload',
    },
    TransformData: {
      Type: 'Task',
      Resource: { 'Fn::GetAtt': ['transformDataToInsertReportHistory', 'Arn'] },
      Parameters: {
        body: {
          bucket: settings.reportBucket,
          reportName: settings.bloombergReportName,
          timezone: settings.timezone,
          timeFrom: settings.timeFrom,
          timeTo: settings.timeTo,
          'dateFrom.$': '$.payload.date',
          'dateTo.$': '$.payload.date',
          'status.$': '$.payload.status',
          'filename.$': '$.payload.filename',
        },
      },
      Next: 'InsertReportHistory',
    },
    InsertReportHistory: {
      Type: 'Task',
      Resource: { 'Fn::GetAtt': ['transactionInsertReportData', 'Arn'] },
      ResultPath: null,
      Next: 'EndTask',
    },
    EndTask: {
      Type: 'Choice',
      Choices: [
        {
          Variable: '$.body.status',
          StringEquals: 'failed',
          Next: 'Fail',
        },
      ],
      Default: 'Success',
    },
    Success: {
      Type: 'Succeed',
    },
    Fail: {
      Type: 'Fail',
    },
  },
};

export const bloombergReport = {
  events: [
    {
      schedule: {
        rate: 'cron(0 1 ? * TUE-SAT *)', // Run 8 AM BKK Time Tuesday to Saturday
        enabled: process.env.AWS_ENVIRONMENT === 'production',
      },
    },
    {
      http: {
        path: 'report/bloomberg',
        method: 'POST',
        request: {
          schemas: {
            'application/json': {
              type: 'object',
              properties: {
                time: { type: 'string' },
              },
              required: ['time'],
            },
          },
        },
      },
    },
  ].filter((q) =>
    process.env.AWS_ENVIRONMENT === 'production' ? q.schedule : true
  ),
  iamRoleStatements: [
    {
      Effect: 'Allow',
      Action: ['s3:PutObject', 's3:GetObject', 's3:ListBucket'],
      Resource: [
        `arn:aws:s3:::${settings.reportBucket}/${settings.velexaReportDir}*`,
        `arn:aws:s3:::${settings.reportBucket}`,
        `arn:aws:s3:::${settings.reportBucket}/${settings.bloombergReportDir}*`,
        `arn:aws:s3:::${settings.sourceBucket}`,
      ],
    },
  ],
  name: `pi-${process.env.AWS_ENVIRONMENT}-BloombergReport`,
  definition,
};
