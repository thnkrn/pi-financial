import { Definition } from 'src/mt5-cme/serverless';

const settings = {
  timezone: 'Asia/Bangkok',
  bucket: `cme-report-${process.env.AWS_ENVIRONMENT}`,
  clientAccountFilePrefix: 'MT5_CME_ACC_03',
  commissionGroupFilePrefix: 'MT5_CME_COMGROUP_03',
  tierFilePrefix: 'MT5_CME_TIER_03',
  traderFilePrefix: 'MT5_CME_TRAD_03',
  traderGroupFilePrefix: 'MT5_CME_TRADGRP_03',
  transactionFeeFilePrefix: 'MT5_CME_TRANSACTIONFEE_03',
  marginSymbolFilePrefix: 'MT5_margin_symbol_cme',
};

const definition: Definition = {
  StartAt: 'UploadSFTP',
  States: {
    UploadSFTP: {
      Type: 'Task',
      Parameters: {
        body: {
          'date.$': '$.time',
          timezone: settings.timezone,
          bucket: settings.bucket,
          clientAccountFilePrefix: settings.clientAccountFilePrefix,
          commissionGroupFilePrefix: settings.commissionGroupFilePrefix,
          tierFilePrefix: settings.tierFilePrefix,
          traderFilePrefix: settings.traderFilePrefix,
          traderGroupFilePrefix: settings.traderGroupFilePrefix,
          transactionFeeFilePrefix: settings.transactionFeeFilePrefix,
          marginSymbolFilePrefix: settings.marginSymbolFilePrefix,
        },
      },
      Resource: { 'Fn::GetAtt': ['uploadSFTP', 'Arn'] },
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
};

export const CMESender = {
  events: [
    {
      schedule: {
        rate: 'cron(0 21 * * ? *)', // Run 4:00 AM BKK Time Everyday
      },
    },
  ],
  iamRoleStatements: [],
  name: `pi-${process.env.AWS_ENVIRONMENT}-CMESender`,
  definition,
};
