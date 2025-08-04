import { SsmWrapper } from '@cgs-bank/utils/ssm-wrapper';

export const ssmWrapper = new SsmWrapper(process.env.SCB_PUBLIC_KEY);
