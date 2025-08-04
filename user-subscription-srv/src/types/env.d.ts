declare global {
  namespace NodeJS {
    interface ProcessEnv {
      NODE_ENV: 'development' | 'Production' | 'Staging';
      STAGE_NAME: 'prod' | 'uat' | 'dev';
      APP_NAME: string;
      TRANSACTION_PREFIX: 'MT5';
      DEBUG: 'true' | 'false';
      AWS_REGION: 'ap-southeast-1';
      USER_API_HOST: string;
      SQS_QUEUE_URL: string;

      SMARTSALES_DB_HOST: string;
      SMARTSALES_DB_PORT: '1433';
      SMARTSALES_DB_NAME: 'SMARTSALES';
      SMARTSALES_DB_USER: string;
      SMARTSALES_DB_PASSWORD: string;
      SMARTDSALES_DB_REQUEST_TIMEOUT_MS: string;

      BACKOFFICE_DB_HOST: string;
      BACKOFFICE_DB_PORT: '1433';
      BACKOFFICE_DB_NAME: 'BackOffice';
      BACKOFFICE_DB_USER: string;
      BACKOFFICE_DB_PASSWORD: string;
      BACKOFFICE_DB_REQUEST_TIMEOUT_MS: string;

      BANK_SERVICE_URL: string;
      BANK_SERVICE_API_KEY: string;
      AES_SECRET_KEY: string;

      TKS_FROM_EMAIL: string;
      TKS_DESTINATION_EMAIL: string;

      SFTP_HOST: string;
      SFTP_PORT: '22';
      SFTP_USERNAME: string;
      SFTP_PASSWORD: string;
      SFTP_WORKSPACE: string;
      SFTP_FILENAME: string;

      USER_SUBSCRIPTION_BUCKET_NAME: string;
      ORACLE_REPORT_EMAIL_FROM: string;
      ORACLE_REPORT_EMAIL_TO: string;
      ORACLE_REPORT_EMAIL_CC: string;
      FILE_OUTPUT_PATH: string;
      REMIND_USER_EMAIL_FROM: string;
      REMIND_USER_EMAIL_CC: string;
      MANUAL_UPDATE_CUSTCODE_FROM: string;
      MANUAL_UPDATE_CUSTCODE_TO: string;

      SERVICE_HUB_URL: string;
      DYNAMODB_HOLIDAY_TABLE_NAME: string;

      VAT_PERCENT_RATE: '7';
      SIRIUS_BACKEND_HOST: string;
      REPLY_EMAIL: string;

      DATADOG_ENABLED: 'true' | 'false';
      DATADOG_API_KEY: string;
      DATADOG_SOURCE: string;
      DATADOG_SERVICE_NAME: string;

      INCENTIVE_BUCKET_NAME: string;
      INCENTIVE_SFTP_HOST: string;
      INCENTIVE_SFTP_PORT: '22';
      INCENTIVE_SFTP_USERNAME: string;
      INCENTIVE_SFTP_PASSWORD: string;
    }
  }
}

export {};
