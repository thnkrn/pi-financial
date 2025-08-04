const bankCodeList = {
  '002': 'BBL',
  '004': 'KBANK',
  '006': 'KTB',
  '008': 'JPMC',
  '009': 'OCBC',
  '011': 'TTB',
  '014': 'SCB',
  '017': 'CITI',
  '018': 'SMBC',
  '020': 'SCBT',
  '022': 'CIMB',
  '023': 'RHB',
  '024': 'UOBT',
  '025': 'BAY',
  '026': 'MEGA ICBC',
  '027': 'AMERICA',
  '029': 'IOB',
  '030': 'GOV',
  '031': 'HSBC',
  '032': 'DB',
  '033': 'GHB',
  '034': 'AGRI',
  '035': 'EXIM',
  '039': 'MHCB',
  '045': 'BNPP',
  '052': 'BOC',
  '065': 'TBANK',
  '066': 'ISBT',
  '067': 'TISCO',
  '069': 'KKP',
  '070': 'ICBC',
  '071': 'TCRB',
  '073': 'LHBANK',
  '079': 'ANZ',
  '080': 'SMTB',
  '098': 'SMEB',
};

export function getBankNameFromCode(bankCode: string): string {
  const value = bankCodeList[bankCode];
  if (value != null) {
    return value;
  } else {
    return bankCode;
  }
}
