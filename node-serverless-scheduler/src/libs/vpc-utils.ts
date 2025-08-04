import type {
  AwsCfArrayInstruction,
  AwsLambdaVpcConfig,
} from '@serverless/typescript';

export function getVpcConfig(env?: string): AwsLambdaVpcConfig {
  switch (env) {
    case 'staging':
      return {
        securityGroupIds: ['sg-080c7fdc8e934d37a'],
        subnetIds: [
          'subnet-09093b66093419b0b',
          'subnet-0350c638f838e89ff',
          'subnet-001a008e75d705c55',
        ],
      };
    case 'production':
      return {
        securityGroupIds: ['sg-0cf9d3fc567aec40c'],
        subnetIds: [
          'subnet-006d8e77661411fcc',
          'subnet-0eba9c62e39b04913',
          'subnet-0938dc796f3fadc91',
        ],
      };
    default:
      throw new Error('Missing "AWS_ENVIRONMENT" in env');
  }
}

export function getVpcEndpoints(env?: string): AwsCfArrayInstruction {
  switch (env) {
    case 'staging':
      return ['vpce-0090277c15ce6a287'];
    case 'production':
      return ['vpce-09460603674ada0c3'];
    default:
      throw new Error('Missing "AWS_ENVIRONMENT" in env');
  }
}
