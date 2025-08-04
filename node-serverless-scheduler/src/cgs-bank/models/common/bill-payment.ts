interface Header {
  TransactionID: string;
  TransactionDateTime: string;
  ServiceName: string;
  SystemCode: string;
  ChannelID: string;
}

interface ReferenceInfo {
  referenceNo1: string;
  referenceNo2: string;
  referenceNo3: string;
  referenceNo4: string;
}

interface PaymentInfo {
  paymentType: string;
  paymentDate: string;
  paymentAmount: number;
  customerName: string;
  accountNo: string;
  accontBank: string; // Possible typo (should this be `accountBank`?)
  accountBank: string;
  billerID: string;
}

interface CompanyAccountInfo {
  accountNumber: string;
  accountBankCode: string;
  accountBranchCode: string;
}

interface Body {
  referenceInfo: ReferenceInfo;
  paymentInfo: PaymentInfo;
  companyAccountInfo: CompanyAccountInfo;
}

export interface KkpBillPaymentCallback {
  Header: Header;
  body: Body;
}

export interface WalletBillPaymentRequest {
  bankTransactionReference: string;
  requestAmount: number;
  reference1: string;
  reference2: string;
  customerPaymentName: string;
  customerPaymentBankCode: string;
  paymentReceivedDate: string;
  customerPaymentBankAccountNo: string;
  paymentChannel: string;
}
