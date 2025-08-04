import { Definition } from '../../serverless';

const definition: Definition = {
  StartAt: 'capturePendingTransactions',
  States: {
    capturePendingTransactions: {
      Type: 'Task',
      Next: 'transFormData',
      ResultPath: null,
      Resource: {
        'Fn::GetAtt': ['snapshotPendingDepositTransactions', 'Arn'],
      },
    },
    transFormData: {
      Type: 'Pass',
      Next: 'transactionInsertReportData',
      Parameters: {
        'id.$': 'States.UUID()',
        reportName: 'Pending Transaction Report',
        userName: null,
        'dateFrom.$': '$.time',
        'dateTo.$': '$.time',
      },
      ResultPath: '$.body',
    },
    transactionInsertReportData: {
      Type: 'Task',
      Resource: {
        'Fn::GetAtt': ['transactionInsertReportData', 'Arn'],
      },
      Catch: [
        {
          ErrorEquals: ['States.ALL'],
          ResultPath: '$.InsertReportDataError',
          Next: 'InsertDBError',
        },
      ],
      Next: 'dwPendingfetchAndBuildReport',
    },
    dwPendingfetchAndBuildReport: {
      Type: 'Task',
      Resource: {
        'Fn::GetAtt': ['dwPendingfetchAndBuildReport', 'Arn'],
      },
      Next: 'transactionUpdateReportData',
    },
    transactionUpdateReportData: {
      Type: 'Task',
      Resource: {
        'Fn::GetAtt': ['transactionUpdateReportData', 'Arn'],
      },
      Catch: [
        {
          ErrorEquals: ['States.ALL'],
          ResultPath: '$.UpdateReportDataError',
          Next: 'UpdateDBError',
        },
      ],
      End: true,
    },
    InsertDBError: {
      Type: 'Pass',
      Result: 'Error in inserting initial report data in DB',
      End: true,
    },
    UpdateDBError: {
      Type: 'Pass',
      Result: 'Error while Updating report data in DB',
      End: true,
    },
  },
};

export const dwPendingReconcileStateMachine = {
  events: [
    {
      schedule: {
        rate: 'cron(0 10 * * ? *)', // Run at 5 PM BKK Time
        enabled: true,
      },
    },
  ],
  name: `pi-${process.env.AWS_ENVIRONMENT}-DWPendingReconcileStateMachine`,
  definition,
};
