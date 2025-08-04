import { Definition } from 'src/mt5-cme/serverless';
import { ReportStatus } from '../../constants/report';

const settings = {
  sourceBucket:
    process.env.AWS_ENVIRONMENT === 'production'
      ? 'cme-marketdata-prod'
      : 'cme-marketdata-staging',
  fileName: 'CME Commission Group',
  filePrefix: 'MT5_CME_COMGROUP_03',
  backofficeBucket: `backoffice-reports-${process.env.AWS_ENVIRONMENT}`,
  cmeBucket: `cme-report-${process.env.AWS_ENVIRONMENT}`,
  fileDir: 'mt5-cme',
  timezone: 'Asia/Bangkok',
  timeFrom: '00:00:01',
  timeTo: '23:59:59',
};

const definition: Definition = {
  StartAt: 'GetConfigs',
  States: {
    GetConfigs: {
      Type: 'Map',
      Next: 'SetExecutionTime',
      ItemReader: {
        Resource: 'arn:aws:states:::s3:getObject',
        ReaderConfig: {
          InputType: 'CSV',
          CSVHeaderLocation: 'FIRST_ROW',
          CSVDelimiter: 'COMMA',
        },
        Parameters: {
          Bucket: settings.sourceBucket,
          Key: 'config/CommissionGroup.csv',
        },
      },
      ItemProcessor: {
        ProcessorConfig: {
          Mode: 'DISTRIBUTED',
          ExecutionType: 'EXPRESS',
        },
        StartAt: 'Pass',
        States: {
          Pass: {
            Type: 'Pass',
            End: true,
            Parameters: {
              'commission_group_code.$': "$['Commission Group Code']",
              'commission_category.$': "$['Commission Category']",
              'commission_amount.$': "$['Commission Amount']",
            },
          },
        },
      },
      MaxConcurrency: 1000,
      ResultPath: '$.configs',
    },
    SetExecutionTime: {
      Type: 'Pass',
      Parameters: {
        'configs.$': '$.configs',
        'time.$': '$$.State.EnteredTime',
      },
      Next: 'ProcessFile',
    },
    ProcessFile: {
      Type: 'Task',
      Next: 'ReportSuccess',
      Parameters: {
        body: {
          'date.$': '$.time',
          'configs.$': '$.configs',
          filePrefix: settings.filePrefix,
          fileDir: settings.fileDir,
          backofficeBucket: settings.backofficeBucket,
          cmeBucket: settings.cmeBucket,
          timezone: settings.timezone,
        },
      },
      ResultPath: '$.payload',
      Resource: { 'Fn::GetAtt': ['processCommissionGroupFile', 'Arn'] },
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
          Next: 'ReportFailed',
          ResultPath: null,
        },
      ],
    },
    ReportSuccess: {
      Type: 'Pass',
      Next: 'TransformData',
      Parameters: {
        status: ReportStatus.Done,
        'fileKey.$': '$.payload.body.fileKey',
        'date.$': '$.time',
      },
      ResultPath: '$.payload',
    },
    ReportFailed: {
      Type: 'Pass',
      Next: 'TransformData',
      Parameters: {
        status: ReportStatus.Failed,
        fileKey: null,
        'date.$': '$.time',
      },
      ResultPath: '$.payload',
    },
    TransformData: {
      Type: 'Task',
      Resource: { 'Fn::GetAtt': ['transformDataToInsertReportHistory', 'Arn'] },
      Parameters: {
        body: {
          fileName: settings.fileName,
          timezone: settings.timezone,
          'date.$': '$.payload.date',
          'status.$': '$.payload.status',
          'fileKey.$': '$.payload.fileKey',
        },
      },
      Next: 'InsertReportHistory',
    },
    InsertReportHistory: {
      Type: 'Task',
      Resource: { 'Fn::GetAtt': ['insertReportHistory', 'Arn'] },
      ResultPath: null,
      End: true,
    },
  },
};

export const CMECommissionGroup = {
  events: [
    {
      schedule: {
        rate: 'cron(0 21 * * ? *)', // Run 4:00 AM BKK Time Everyday
      },
    },
  ],
  iamRoleStatements: [],
  name: `pi-${process.env.AWS_ENVIRONMENT}-CMECommissionGroup`,
  definition,
};
