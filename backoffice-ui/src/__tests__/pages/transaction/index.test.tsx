import useFetchTransaction from '@/hooks/backoffice/useFetchTransaction'
import Transactions from '@/pages/transaction/[transactionNo]'
import { render } from '@testing-library/react'
import { useSession } from 'next-auth/react'
import { useRouter } from 'next/router'

jest.mock('@/hooks/backoffice/useFetchTransaction')
jest.mock('next-auth/react')
jest.mock('next/router', () => ({
  useRouter: jest.fn(),
}))

const mockTransaction = {
  customerProfile: {
    customerCode: '0054xxx',
    customerAccountName: 'ชื่อ 0054xxx นามสกุล 0054xxx (Name 0054xxx Surname 0054xxx)',
  },
  statusAction: {
    status: 'Pending',
    errorCode: 'Trading Account Deposit Fail',
    callToAction: 'Check Customer Trading Account Balance, before Manual Allocate',
    failedReason: 'Manual allocation in SBA',
    refundSuccessfulAt: null,
    requestedAmount: '2000000',
    paymentReceivedAmount: '2000000',
  },
  transactionDetail: {
    transactionNo: 'DHDPxxxxx',
    senderBankName: 'SCB',
    senderBankAccountNumber: '1112xxxx',
    senderBankAccountName: 'SUPAN RUAN',
    requestedCurrency: 'thb',
    transactionType: 'deposit',
    requestedAmount: '2000000',
    effectiveDate: '2024-04-18T06:04:55.000Z',
    createdAt: '2024-04-18T06:03:46.715Z',
  },
  productDetail: {
    channelName: 'QR',
    customerAccountNumber: '0054xxx',
    accountType: 'TFEX',
  },
  qrDeposit: {
    state: 'QrDepositCompleted',
    paymentReceivedAmount: '2000000',
    paymentReceivedDateTime: '2024-04-18T06:04:55.000Z',
    fee: null,
    failedReason: null,
    createdAt: '2024-04-18T06:03:46.892Z',
  },
  oddDeposit: null,
  atsDeposit: null,
  oddWithdraw: null,
  atsWithdraw: null,
  upback: {
    state: 'UpBackFailedRequireActionSba',
    failedReason: 'Freewill update failed. ResultCode: 906',
    createdAt: '2024-04-18T06:03:46.715Z',
  },
  globalTransfer: null,
  recovery: null,
  refundInfo: null,
  actions: [
    {
      label: 'SBA Allocation Transfer',
      value: 'SbaAllocationTransfer',
    },
  ],
  makerType: [],
  checkerType: [],
}

beforeEach(() => {
  const mockSession = {
    expires: new Date(Date.now() + 2 * 86400).toISOString(),
    user: { username: 'admin' },
  }
  ;(useSession as jest.Mock).mockReturnValueOnce([mockSession, 'authenticated'])

  // @ts-ignore
  useRouter.mockReturnValue({
    push: jest.fn(),
    query: {
      transactionNo: 'TEST0000',
    },
  })
})

describe('Transaction', () => {
  it('should match with snapshot if no error', () => {
    // NOTE: ignore type assertion since we do not want to add mockImplementation into the real implementated useFetchTransaction type
    // @ts-ignore
    useFetchTransaction.mockImplementation(() => ({
      transaction: mockTransaction,
      loading: false,
      error: undefined,
      fetchTransaction: jest.fn(),
    }))

    const { asFragment } = render(<Transactions />)

    expect(asFragment()).toMatchSnapshot()
  })

  it('should match with snapshot if no transaction data', () => {
    // @ts-ignore
    useFetchTransaction.mockImplementation(() => ({
      loading: false,
      error: undefined,
      fetchTransaction: jest.fn(),
    }))

    const { asFragment } = render(<Transactions />)

    expect(asFragment()).toMatchSnapshot()
  })

  it('should match with snapshot if loading transaction', () => {
    // @ts-ignore
    useFetchTransaction.mockImplementation(() => ({
      loading: true,
      error: undefined,
      fetchTransaction: jest.fn(),
    }))

    const { asFragment } = render(<Transactions />)

    expect(asFragment()).toMatchSnapshot()
  })

  it('should redirect to error page if error occured', () => {
    // @ts-ignore
    useFetchTransaction.mockImplementation(() => ({
      loading: false,
      error: { message: 'errr' },
      fetchTransaction: jest.fn(),
    }))

    const router = useRouter()

    render(<Transactions />)

    expect(router.push).toBeCalled()
  })
})
