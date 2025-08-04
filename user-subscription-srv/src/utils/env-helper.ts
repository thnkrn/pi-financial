export const isDevelopment = () => {
  return process.env.STAGE_NAME === 'dev';
};

export const isDevelopmentOrStaging = () => {
  return process.env.STAGE_NAME === 'dev' || process.env.STAGE_NAME === 'uat';
};

export enum EnvKeys {
  SWAGGER_ENABLED = 'SWAGGER_ENABLED',
  SWAGGER_WRITE_FILE = 'SWAGGER_WRITE_FILE',
}
