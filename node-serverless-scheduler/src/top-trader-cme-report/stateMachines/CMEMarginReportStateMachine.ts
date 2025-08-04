import { Definition } from '../../../serverless';

const settings = {
  reportBucket: `cme-marketdata-${process.env.AWS_ENVIRONMENT}`,
  marginReportFolderPrefix: 'margin',
  marginReportSpan1Prefix: 'CME_margin_span_1',
  cmeMarginSpan1URL:
    'https://www.cmegroup.com/CmeWS/mvc/Margins/OUTRIGHT.csv?sortField=exchange&sortAsc=true&clearingCode=all&sector=all&exchange=all',
  marginReportSpan2Prefix: 'CME_margin_span_2',
  cmeMarginSpan2URL:
    'https://www.cmegroup.com/services/margins/download/OUTRIGHT?sortField=&sortAsc=true&clearingCode=all&sector=all&exchange=all',
  marginReportPrefix: 'CME_margin_report',
};

const definition: Definition = {
  StartAt: 'downloadInParallel',
  States: {
    downloadInParallel: {
      Type: 'Parallel',
      ResultPath: '$.parallelResults',
      Branches: [
        {
          StartAt: 'downloadMarginSpan1',
          States: {
            downloadMarginSpan1: {
              Type: 'Task',
              ResultPath: '$.marginSpan1Result',
              Parameters: {
                body: {
                  reportPrefix: settings.marginReportSpan1Prefix,
                  reportURL: settings.cmeMarginSpan1URL,
                  bucket: settings.reportBucket,
                  folder: settings.marginReportFolderPrefix,
                  'date.$': '$.time',
                },
              },
              Resource: { 'Fn::GetAtt': ['downloadMarginSpan1', 'Arn'] },
              Retry: [
                {
                  ErrorEquals: ['Error'],
                  IntervalSeconds: 60,
                  BackoffRate: 2,
                  MaxAttempts: 2,
                },
              ],
              End: true,
            },
          },
        },
        {
          StartAt: 'downloadMarginSpan2',
          States: {
            downloadMarginSpan2: {
              Type: 'Task',
              ResultPath: '$.marginSpan2Result',
              Parameters: {
                body: {
                  reportPrefix: settings.marginReportSpan2Prefix,
                  reportURL: settings.cmeMarginSpan2URL,
                  bucket: settings.reportBucket,
                  folder: settings.marginReportFolderPrefix,
                  'date.$': '$.time',
                },
              },
              Resource: { 'Fn::GetAtt': ['downloadMarginSpan2', 'Arn'] },
              Retry: [
                {
                  ErrorEquals: ['Error'],
                  IntervalSeconds: 60,
                  BackoffRate: 2,
                  MaxAttempts: 2,
                },
              ],
              End: true,
            },
          },
        },
      ],
      Catch: [
        {
          ErrorEquals: ['States.ALL'],
          Next: 'Fail',
          ResultPath: '$.error',
        },
      ],
      Next: 'processMarginReport',
    },
    processMarginReport: {
      Type: 'Task',
      Parameters: {
        bucket: settings.reportBucket,
        reportPrefix: settings.marginReportPrefix,
        folder: settings.marginReportFolderPrefix,
        'date.$': '$.time',
        'span1ReportName.$':
          '$.parallelResults[0].marginSpan1Result.body.reportName',
        'span2ReportName.$':
          '$.parallelResults[1].marginSpan2Result.body.reportName',
      },
      Resource: { 'Fn::GetAtt': ['processMarginReport', 'Arn'] },
      Retry: [
        {
          ErrorEquals: ['Error'],
          IntervalSeconds: 30,
          BackoffRate: 2,
          MaxAttempts: 2,
        },
      ],
      Catch: [
        {
          ErrorEquals: ['States.ALL'],
          Next: 'Fail',
          ResultPath: '$.error',
        },
      ],
      Next: 'Success',
    },
    Success: {
      Type: 'Succeed',
    },
    Fail: {
      Type: 'Fail',
    },
  },
};

export const cmeMarginReport = {
  events: [
    {
      schedule: {
        rate: 'cron(0 20 * * ? *)', // Run 3:00 AM BKK Time Everyday
      },
    },
    {
      http: {
        path: 'cme/margin',
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
  ],
  iamRoleStatements: [
    {
      Effect: 'Allow',
      Action: ['s3:PutObject', 's3:GetObject', 's3:ListBucket'],
      Resource: [
        `arn:aws:s3:::${settings.reportBucket}/*`,
        `arn:aws:s3:::${settings.reportBucket}`,
      ],
    },
  ],
  name: `pi-${process.env.AWS_ENVIRONMENT}-CMEMarginReport`,
  definition,
};
