export interface GetTokenResponse {
  result: string;
  body: {
    token: string;
    type: string;
    expires_in: number;
  };
}
