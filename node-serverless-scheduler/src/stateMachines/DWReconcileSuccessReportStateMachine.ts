import schema from '@functions/report/transaction-insert-report-data/schema';
import { Definition } from '../../serverless';

const definition: Definition = {
  StartAt: 'transFormData',
  States: {
    transFormData: {
      Type: 'Pass',
      Next: 'transactionInsertReportData',
      Parameters: {
        'body.$': '$',
      },
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
      Next: 'dwSuccessfetchAndBuildReport',
    },
    dwSuccessfetchAndBuildReport: {
      Type: 'Task',
      Resource: {
        'Fn::GetAtt': ['dwSuccessfetchAndBuildReport', 'Arn'],
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

export const dwSuccessReconcileStateMachine = {
  events: [
    {
      http: {
        path: 'report/success-dw-reconcile',
        method: 'POST',
        request: {
          schemas: {
            'application/json': schema,
          },
        },
      },
    },
  ],
  name: `pi-${process.env.AWS_ENVIRONMENT}-DWSuccessReconcileStateMachine`,
  definition,
};
