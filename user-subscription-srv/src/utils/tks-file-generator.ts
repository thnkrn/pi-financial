export interface TKSFileRecord {
  customerCode: string;
  title: string;
  name: string;
  lastname: string;
  idCardNo: string;
  address: string;
  address2: string;
  address3: string;
  postCode: string;
  location: string;
  marketingName: string;
  marketingTeam: string;
  processDate: string;
  taxInvoiceNumber: string;
  bankCode: string;
  paymentAmount: string;
  rawAmount: string;
  taxAmount: string;
}

export const toTKSFileFormat = (value: TKSFileRecord): string => {
  return [
    value.customerCode,
    value.title,
    value.name,
    value.lastname,
    value.idCardNo,
    value.address,
    value.address2,
    value.address3,
    value.postCode,
    value.location,
    value.marketingName,
    value.marketingTeam,
    value.processDate,
    value.taxInvoiceNumber,
    value.bankCode,
    value.paymentAmount,
    value.rawAmount,
    value.taxAmount,
  ].join('|');
};
