export interface AccountCondition {
  exchangeMarketId: string;
  accountType: string;
  accountTypeCode: string;
}

export interface CustomerCondition {
  customerSubType: string;
  customerType: string[];
}

export interface AccountTypeCondition {
  accountCondition: AccountCondition[];
  customerCondition: CustomerCondition[];
  accountName: string;
}

export const OTCOffshoreCondition: AccountTypeCondition = {
  accountCondition: [
    {
      exchangeMarketId: '1',
      accountType: 'H',
      accountTypeCode: 'CH',
    },
  ],
  customerCondition: [
    { customerSubType: '5', customerType: ['1', '2', '5'] },
    { customerSubType: '6', customerType: ['4'] },
  ],
  accountName: 'OTC - Offshore',
};

export const ThaiEquityCashBalanceCondition: AccountTypeCondition = {
  accountCondition: [
    {
      exchangeMarketId: '1',
      accountType: 'H',
      accountTypeCode: 'CH',
    },
    {
      exchangeMarketId: '1',
      accountType: 'H',
      accountTypeCode: 'LH',
    },
  ],
  customerCondition: [
    { customerSubType: '1', customerType: ['1', '2', '3', '4', '5'] },
    { customerSubType: '2', customerType: ['1', '2', '3', '4', '5'] },
    { customerSubType: '3', customerType: ['1', '2', '3', '4', '5'] },
    { customerSubType: '4', customerType: ['1', '2', '3', '4', '5'] },
    { customerSubType: '5', customerType: ['4'] },
    { customerSubType: '6', customerType: ['1', '2'] },
  ],
  accountName: 'Cash Balance',
};

export const ThaiEquityCashCondition: AccountTypeCondition = {
  accountCondition: [
    {
      exchangeMarketId: '1',
      accountType: '1',
      accountTypeCode: 'CC',
    },
    {
      exchangeMarketId: '1',
      accountType: '1',
      accountTypeCode: 'LC',
    },
  ],
  customerCondition: [
    { customerSubType: '1', customerType: ['1', '2', '3', '4', '5'] },
    { customerSubType: '2', customerType: ['1', '2', '3', '4', '5'] },
    { customerSubType: '3', customerType: ['1', '2', '3', '4', '5'] },
    { customerSubType: '4', customerType: ['1', '2', '3', '4', '5'] },
    { customerSubType: '5', customerType: ['4'] },
    { customerSubType: '6', customerType: ['1', '2'] },
  ],
  accountName: 'Cash',
};

export const ThaiEquityCreditBalanceCondition: AccountTypeCondition = {
  accountCondition: [
    {
      exchangeMarketId: '1',
      accountType: '6',
      accountTypeCode: 'CB',
    },
    {
      exchangeMarketId: '1',
      accountType: '6',
      accountTypeCode: 'BB',
    },
  ],
  customerCondition: [
    { customerSubType: '1', customerType: ['1', '2', '3', '4', '5'] },
    { customerSubType: '2', customerType: ['1', '2', '3', '4', '5'] },
    { customerSubType: '3', customerType: ['1', '2', '3', '4', '5'] },
    { customerSubType: '4', customerType: ['1', '2', '3', '4', '5'] },
    { customerSubType: '5', customerType: ['4'] },
    { customerSubType: '6', customerType: ['1', '2'] },
  ],
  accountName: 'Credit Balance',
};

export const MutualFundCondition: AccountTypeCondition = {
  accountCondition: [
    {
      exchangeMarketId: '4',
      accountType: '1',
      accountTypeCode: 'UT',
    },
  ],
  customerCondition: [],
  accountName: 'Mutual Funds',
};

export const MutualFundOffshoreCondition: AccountTypeCondition = {
  accountCondition: [
    {
      exchangeMarketId: '1',
      accountType: 'H',
      accountTypeCode: 'CH',
    },
  ],
  customerCondition: [
    { customerSubType: '5', customerType: ['1', '2', '5'] },
    { customerSubType: '6', customerType: ['4'] },
  ],
  accountName: 'Mutual Funds (Offshore)',
};

export const BondCondition: AccountTypeCondition = {
  accountCondition: [
    {
      exchangeMarketId: '3',
      accountType: '1',
      accountTypeCode: 'DC',
    },
  ],
  customerCondition: [],
  accountName: 'Bonds & Debentures',
};

export const BondOffshoreCondition: AccountTypeCondition = {
  accountCondition: [
    {
      exchangeMarketId: '1',
      accountType: 'H',
      accountTypeCode: 'CH',
    },
  ],
  customerCondition: [
    { customerSubType: '5', customerType: ['1', '2', '5'] },
    { customerSubType: '6', customerType: ['4'] },
  ],
  accountName: 'Bonds & Debentures (Offshore)',
};

export const TfexCondition: AccountTypeCondition = {
  accountCondition: [
    {
      exchangeMarketId: '7',
      accountType: 'F',
      accountTypeCode: 'TF',
    },
  ],
  customerCondition: [],
  accountName: 'TFEX',
};

export const StructuredProductCondition: AccountTypeCondition = {
  accountCondition: [
    {
      exchangeMarketId: '1',
      accountType: 'H',
      accountTypeCode: 'CH',
    },
    {
      exchangeMarketId: '1',
      accountType: 'H',
      accountTypeCode: 'LH',
    },
  ],
  customerCondition: [
    { customerSubType: '5', customerType: ['1', '2', '5'] },
    { customerSubType: '6', customerType: ['4'] },
  ],
  accountName: 'Structured Products',
};

export const StructuredProductOnshoreCondition: AccountTypeCondition = {
  accountCondition: [
    {
      exchangeMarketId: '1',
      accountType: '1',
      accountTypeCode: 'CC',
    },
    {
      exchangeMarketId: '1',
      accountType: '1',
      accountTypeCode: 'LC',
    },
  ],
  customerCondition: [
    { customerSubType: '5', customerType: ['1', '2', '5'] },
    { customerSubType: '6', customerType: ['4'] },
  ],
  accountName: 'Structured Products (Onshore)',
};

export const GlobalEquityOtcCondition: AccountTypeCondition = {
  accountCondition: [
    {
      exchangeMarketId: '1',
      accountType: 'H',
      accountTypeCode: 'CH',
    },
    {
      exchangeMarketId: '1',
      accountType: 'H',
      accountTypeCode: 'LH',
    },
  ],
  customerCondition: [
    { customerSubType: '5', customerType: ['1', '2', '5'] },
    { customerSubType: '6', customerType: ['4'] },
  ],
  accountName: 'Global Equity (OTC)',
};

export const GlobalEquityCondition: AccountTypeCondition = {
  accountCondition: [
    {
      exchangeMarketId: '5',
      accountType: 'U',
      accountTypeCode: 'XU',
    },
  ],
  customerCondition: [],
  accountName: 'Global Equity',
};

export const AllAccountTypeConditions: AccountTypeCondition[] = [
  OTCOffshoreCondition,
  ThaiEquityCashBalanceCondition,
  ThaiEquityCashCondition,
  ThaiEquityCreditBalanceCondition,
  MutualFundCondition,
  MutualFundOffshoreCondition,
  BondCondition,
  TfexCondition,
  StructuredProductCondition,
  StructuredProductOnshoreCondition,
  GlobalEquityCondition,
];
