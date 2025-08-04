import { Definition } from 'src/top-trader-cme-report/serverless';
import { ReportStatus } from '../../constants/report';

const settings = {
  sourceBucket:
    process.env.AWS_ENVIRONMENT === 'production'
      ? 'cme-marketdata-prod'
      : 'cme-marketdata-staging',
  outputPath: 'report',
  internalOutputPath: 'original',
  marginFolder: 'margin',
  marginFileName: 'CME_margin_report.csv',
  finalFilePrefix: 'MT5_margin_symbol_cme',
  fileName: 'CME Margin Symbol',
  backofficeBucket: `backoffice-reports-${process.env.AWS_ENVIRONMENT}`,
  backofficeDir: 'mt5-cme',
  cmeBucket: `cme-report-${process.env.AWS_ENVIRONMENT}`,
};

const definition: Definition = {
  Comment: 'Generate CME Margin Report for Top Trader',
  StartAt: 'GetConfigs',
  States: {
    GetConfigs: {
      Type: 'Map',
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
              'product_code.$': "$['CME product name (Globex)']",
              'clearing_code.$': "$['Clearing Code']",
            },
          },
        },
      },
      Next: 'LoginCME',
      MaxConcurrency: 1000,
      ItemReader: {
        Resource: 'arn:aws:states:::s3:getObject',
        ReaderConfig: {
          InputType: 'CSV',
          CSVHeaderLocation: 'FIRST_ROW',
          CSVDelimiter: 'COMMA',
        },
        Parameters: {
          Bucket: settings.sourceBucket,
          Key: 'config/BU.csv',
        },
      },
      ResultPath: '$.configs',
      Catch: [
        {
          ErrorEquals: ['States.ALL'],
          Next: 'ReportFailed',
          ResultPath: null,
        },
      ],
    },
    LoginCME: {
      Type: 'Task',
      Resource: { 'Fn::GetAtt': ['loginCme', 'Arn'] },
      Parameters: {},
      Retry: [],
      ResultPath: '$.LoginCmeResult',
      Next: 'GetProductDetail',
      Catch: [
        {
          ErrorEquals: ['States.ALL'],
          Next: 'ReportFailed',
          ResultPath: null,
        },
      ],
    },
    GetProductDetail: {
      Type: 'Task',
      Resource: { 'Fn::GetAtt': ['getProductDetail', 'Arn'] },
      Parameters: {
        'access_token.$': '$.LoginCmeResult.access_token',
        'configs.$': '$.configs',
      },
      Retry: [],
      ResultPath: '$.GetProductDetailResult',
      Next: 'GenerateReportWithoutMargin',
      Catch: [
        {
          ErrorEquals: ['States.ALL'],
          Next: 'ReportFailed',
          ResultPath: null,
        },
      ],
    },
    GenerateReportWithoutMargin: {
      Type: 'Task',
      Resource: { 'Fn::GetAtt': ['generateCmeReportWithoutMargin', 'Arn'] },
      Parameters: {
        'token.$': '$.LoginCmeResult.access_token',
        bucket: settings.sourceBucket,
        outputDir: settings.internalOutputPath,
        holidayFileKey: 'config/Holiday.csv',
        'products.$': '$.GetProductDetailResult.products',
        'date.$': '$.time',
      },
      ResultPath: '$.GenerateReportWithoutMarginResult',
      Retry: [],
      Next: 'ProcessingMargin',
      Catch: [
        {
          ErrorEquals: ['States.ALL'],
          Next: 'ReportFailed',
          ResultPath: null,
        },
      ],
    },
    ProcessingMargin: {
      Type: 'Task',
      Resource: { 'Fn::GetAtt': ['processInitialMargin', 'Arn'] },
      Parameters: {
        'configs.$': '$.configs',
        'fileKey.$': '$.GenerateReportWithoutMarginResult.fileKey',
        'date.$': '$.time',
        bucket: settings.sourceBucket,
        reportPrefix: settings.outputPath,
        marginFolder: settings.marginFolder,
        marginFileName: settings.marginFileName,
        finalFilePrefix: settings.finalFilePrefix,
        backofficeBucket: settings.backofficeBucket,
        backofficeDir: settings.backofficeDir,
        cmeBucket: settings.cmeBucket,
      },
      Retry: [],
      ResultPath: '$.ProcessingMarginResult',
      Next: 'ReportSuccess',
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
        'fileKey.$': '$.ProcessingMarginResult.fileKey',
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

export const GenerateTopTraderCmeReport = {
  events: [
    {
      schedule: {
        rate: 'cron(0 21 * * ? *)', // Run 4:00 AM BKK Time Everyday
      },
    },
  ],
  iamRoleStatements: [],
  name: `pi-${process.env.AWS_ENVIRONMENT}-GenerateTopTraderCmeReport`,
  definition,
};
