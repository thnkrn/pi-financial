export interface SCBTokenOutput {
  accessToken: string;
  tokenType: string;
  expiresIn: number;
  expiresAt: number;
  refreshToken: string;
  refreshExpiresIn: number;
  refreshExpiresAt: number;
}

export interface SCBResultStatus {
  code: string;
  description: string;
  details?: string;
}

type SCBValidationMessage = {
  message: string;
  description: string;
};

export interface SCBBaseResponseAttribute {
  merchantId: string;
  subAccountId?: string;
}

export interface SCBResponseWrapper<T> extends SCBBaseResponseAttribute {
  status: SCBResultStatus;
  data?: T;
  validationMessages?: SCBValidationMessage;
}
