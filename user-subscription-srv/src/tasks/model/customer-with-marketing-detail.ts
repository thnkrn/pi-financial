export interface CustomerWithMarketingDetail {
  mktId: string;
  mktName: string;
  teamID: string;
  eName: string;
  custcode: string;
  customerName: string;
  telNo: string;
  email: string;
}

export interface FilteredCustomer {
  mktId: string;
  mktName: string;
  teamID: string;
  eName: string;
  mktEmail: string;
  customers: any[];
}
