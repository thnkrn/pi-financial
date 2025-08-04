import { Visible } from '@/@core/components/auth/Visible'
import '@testing-library/jest-dom'
import { render } from '@testing-library/react'

jest.mock('@/lib/auth/useUserRole', () => ({
  useUserRole: jest.fn(() => true),
}))

describe('Visible Component', () => {
  afterEach(() => {
    jest.resetAllMocks()
  })

  it('renders children when user has allowed roles', () => {
    const { getByText } = render(
      <Visible allowedRoles={['transaction-read', 'ticket-manage']}>
        <div>Visible Content</div>
      </Visible>
    )

    expect(getByText('Visible Content')).toBeInTheDocument()
  })
})
