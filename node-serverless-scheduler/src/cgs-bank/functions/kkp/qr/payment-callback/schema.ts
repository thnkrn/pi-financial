export default {
  type: 'object',
  properties: {
    Header: {
      type: 'object',
      properties: {
        TransactionID: { type: 'string' },
        TransactionDateTime: { type: 'string' },
        ServiceName: { type: 'string' },
        SystemCode: { type: 'string' },
        ChannelID: { type: 'string' },
      },
    },
    body: {
      type: 'object',
      properties: {
        referenceInfo: {
          type: 'object',
          properties: {
            referenceNo1: { type: 'string' },
            referenceNo2: { type: 'string' },
            referenceNo3: { type: 'string' },
            referenceNo4: { type: 'string' },
          },
        },
        paymentInfo: {
          type: 'object',
          properties: {
            paymentType: { type: 'string' },
            paymentDate: { type: 'string' },
            paymentAmount: { type: 'number' },
            customerName: { type: 'string' },
            accountNo: { type: 'string' },
            accontBank: { type: 'string' }, // TYPO: FROM KKP RESPONSE
            billerID: { type: 'string' },
          },
        },
        companyAccountInfo: {
          type: 'object',
          properties: {
            accountNumber: { type: 'string' },
            accountBankCode: { type: 'string' },
            accountBranchCode: { type: 'string' },
          },
        },
      },
    },
  },
} as const;
