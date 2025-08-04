import { Definition } from '../../serverless';
import { ReportStatus } from '../constants/report';

const settings = {
  sourceBucket:
    process.env.AWS_ENVIRONMENT === 'production'
      ? 'pi-exante-reports-prod'
      : 'pi-exante-reports-nonprod',
  reportName: 'Velexa Trades',
  reportPrefix: 'Trades_report_for_QLO7111',
  reportBucket: `backoffice-reports-${process.env.AWS_ENVIRONMENT}`,
  reportDir: 'velexa/trades/',
  timezone: 'america/new_york',
  timeFrom: '17:00:00',
  timeTo: '17:00:00',
};

const definition: Definition = {
  StartAt: 'ProcessDateRange',
  States: {
    ProcessDateRange: {
      Type: 'Task',
      Parameters: {
        body: {
          'startDate.$': '$.startDate',
          'endDate.$': '$.endDate',
        },
      },
      ResultPath: '$.dateRangeResult',
      Resource: { 'Fn::GetAtt': ['processDateRange', 'Arn'] },
      Next: 'IterateOverDates',
      Retry: [
        {
          ErrorEquals: ['Error'],
          IntervalSeconds: 60,
          BackoffRate: 2,
          MaxAttempts: 2,
        },
      ],
    },
    IterateOverDates: {
      Type: 'Map',
      ItemsPath: '$.dateRangeResult.body.dateRange.dates',
      MaxConcurrency: 2,
      Iterator: {
        StartAt: 'FindS3File',
        States: {
          FindS3File: {
            Type: 'Task',
            Parameters: {
              body: {
                reportPrefix: settings.reportPrefix,
                bucket: settings.sourceBucket,
                'date.$': '$.day',
                timezone: settings.timezone,
              },
            },
            ResultPath: '$.s3Result',
            Resource: { 'Fn::GetAtt': ['findS3File', 'Arn'] },
            Next: 'MapFileFromS3',
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
          MapFileFromS3: {
            Type: 'Map',
            Next: 'CopyObject',
            ItemProcessor: {
              ProcessorConfig: {
                Mode: 'DISTRIBUTED',
                ExecutionType: 'EXPRESS',
              },
              StartAt: 'InsertToDb',
              States: {
                InsertToDb: {
                  Type: 'Task',
                  Resource: {
                    'Fn::GetAtt': ['insertVelexaTradesReport', 'Arn'],
                  },
                  End: true,
                },
              },
            },
            ItemReader: {
              Resource: 'arn:aws:states:::s3:getObject',
              Parameters: {
                'Bucket.$': '$.s3Result.body.bucket',
                'Key.$': '$.s3Result.body.key',
              },
              ReaderConfig: {
                InputType: 'CSV',
                CSVHeaderLocation: 'FIRST_ROW',
              },
            },
            ResultPath: null,
            MaxConcurrency: 20,
            Catch: [
              {
                ErrorEquals: ['States.ALL'],
                Next: 'FallbackReportFailed',
                ResultPath: null,
              },
            ],
          },
          CopyObject: {
            Type: 'Task',
            Resource: 'arn:aws:states:::aws-sdk:s3:copyObject',
            Parameters: {
              'CopySource.$': `States.Format('{}/{}', $.s3Result.body.bucket, $.s3Result.body.key)`,
              Bucket: settings.reportBucket,
              'Key.$': `States.Format('${settings.reportDir}{}', $.s3Result.body.key)`,
            },
            ResultPath: null,
            Next: 'FallbackReportSuccess',
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
              'filename.$': `States.Format('${settings.reportDir}{}', $.s3Result.body.key)`,
              'date.$': '$.day',
            },
            ResultPath: '$.payload',
          },
          FallbackReportFailed: {
            Type: 'Pass',
            Next: 'TransformData',
            Parameters: {
              status: ReportStatus.Failed,
              filename: null,
              'date.$': '$.day',
            },
            ResultPath: '$.payload',
          },
          TransformData: {
            Type: 'Task',
            Resource: {
              'Fn::GetAtt': ['transformDataToInsertReportHistory', 'Arn'],
            },
            Parameters: {
              body: {
                bucket: settings.reportBucket,
                reportName: settings.reportName,
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
      },
      End: true,
    },
  },
};

export const velexaReconcileTradesReportDateRange = {
  events: [
    ...(process.env.AWS_ENVIRONMENT !== 'production'
      ? [
          {
            http: {
              path: 'reconcile/ge/trades-date-range',
              method: 'POST',
              request: {
                schemas: {
                  'application/json': {
                    type: 'object',
                    properties: {
                      startDate: { type: 'string' },
                      endDate: { type: 'string' },
                    },
                    required: ['startDate', 'endDate'],
                  },
                },
              },
            },
          },
        ]
      : []),
  ],
  iamRoleStatements: [
    {
      Effect: 'Allow',
      Action: ['s3:PutObject', 's3:GetObject', 's3:ListBucket'],
      Resource: [
        `arn:aws:s3:::${settings.reportBucket}/${settings.reportDir}*`,
        `arn:aws:s3:::${settings.reportBucket}`,
      ],
    },
  ],
  name: `pi-${process.env.AWS_ENVIRONMENT}-velexaReconcileTradesReportDateRange`,
  definition,
};
