export interface SCBDDPaymentInquiryRequest {
  refNumber: string;
  refDateTime: string; //Merchant reference date and time per transaction Format: yyyyMMddHHmmss
  customerRef?: string;
}
