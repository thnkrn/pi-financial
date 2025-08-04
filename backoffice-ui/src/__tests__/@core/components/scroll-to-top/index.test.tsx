import ScrollToTop from '@/@core/components/scroll-to-top'
import '@testing-library/jest-dom'
import { render } from '@testing-library/react'

describe('ScrollToTop Component', () => {
  it('renders the children', () => {
    const { getByText } = render(<ScrollToTop>Scroll to Top</ScrollToTop>)
    const childrenElement = getByText('Scroll to Top')

    expect(childrenElement).toBeInTheDocument()
  })

  it('does not render when trigger is false', () => {
    jest.mock('@mui/material/useScrollTrigger', () => ({
      __esModule: true,
      default: () => false,
    }))

    const { queryByRole } = render(<ScrollToTop>Scroll to Top</ScrollToTop>)
    const scrollToTopButton = queryByRole('presentation')

    expect(scrollToTopButton).not.toBeInTheDocument()
  })
})
