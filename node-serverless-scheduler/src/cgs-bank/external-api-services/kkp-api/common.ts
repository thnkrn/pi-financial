import { KKPErrorResponse } from '@cgs-bank/external-api-services/kkp-api/kkp-error';

export interface KKPRequestHeader {
  TransactionID: string;
  TransactionDateTime: string;
}

export interface KKPRequestWrapper<T> {
  Header: KKPRequestHeader;
  Data: T;
}

export interface KKPResponseHeader {
  TransactionID: string;
  TransactionDateTime: string;
  ServiceName: string;
  SystemCode: string;
  ChannelCode: string;
}

export interface KKPResponseWrapper<T> {
  Header: KKPResponseHeader;
  Data: T;
  ResponseStatus: KKPResponseStatus;
  fault?: KKPErrorResponse;
}

export interface KKPResponseStatus {
  ResponseCode: string;
  ResponseMessage: string;
}

export interface KKPTokenResponse {
  access_token: string;
  scope: string;
  token_type: string;
  expires_in: string;
  error_description?: string;
  error?: string;
}
