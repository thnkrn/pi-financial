export interface RegistrationRequest {
  registrationRefCode: string;
  citizenId: string;
  redirectUrl: string;
  email?: string; // ONLY KBANK SUPPORTED
  mobileNo?: string; // ONLY KBANK SUPPORTED
  remarks?: string; // ONLY SCB SUPPORTED
}

export const RegistrationRequestScheme = {
  type: 'object',
  properties: {
    registrationRefCode: { type: 'string' },
    citizenId: { type: 'string' },
    email: { type: 'string' },
    mobileNo: { type: 'string' },
    redirectUrl: { type: 'string' },
  },

  required: ['registrationRefCode', 'citizenId', 'redirectUrl'],
};

export interface RegistrationResponse {
  webUrl: string;
}

export interface RegistrationInquiryRequest {
  registrationRefCode: string;
  citizenId: string;
}

export const RegistrationInquiryRequestScheme = {
  type: 'object',
  properties: {
    registrationRefCode: { type: 'string' },
    citizenId: { type: 'string' },
  },

  required: ['registrationRefCode', 'citizenId'],
};

export interface RegistrationCallbackResponse {
  registrationRefCode: string;
  bankAccountNo: string;
}
