import { Definition } from 'src/mt5-cme/serverless';
import { ReportStatus } from '../../constants/report';

const settings = {
  fileName: 'CME Tier',
  filePrefix: 'MT5_CME_TIER_03',
  backofficeBucket: `backoffice-reports-${process.env.AWS_ENVIRONMENT}`,
  cmeBucket: `cme-report-${process.env.AWS_ENVIRONMENT}`,
  fileDir: 'mt5-cme',
  timezone: 'Asia/Bangkok',
  timeFrom: '00:00:01',
  timeTo: '23:59:59',
};

const definition: Definition = {
  StartAt: 'ProcessFile',
  States: {
    ProcessFile: {
      Type: 'Task',
      Parameters: {
        body: {
          'date.$': '$.time',
          filePrefix: settings.filePrefix,
          fileDir: settings.fileDir,
          backofficeBucket: settings.backofficeBucket,
          cmeBucket: settings.cmeBucket,
          timezone: settings.timezone,
        },
      },
      ResultPath: '$.payload',
      Resource: { 'Fn::GetAtt': ['processTierFile', 'Arn'] },
      Next: 'ReportSuccess',
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

export const CMETier = {
  events: [
    {
      schedule: {
        rate: 'cron(0 21 * * ? *)', // Run 4:00 AM BKK Time Everyday
      },
    },
  ],
  iamRoleStatements: [],
  name: `pi-${process.env.AWS_ENVIRONMENT}-CMETier`,
  definition,
};
