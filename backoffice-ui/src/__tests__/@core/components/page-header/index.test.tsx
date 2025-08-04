import PageHeader from '@/@core/components/page-header'
import '@testing-library/jest-dom'
import { render } from '@testing-library/react'

describe('PageHeader Component', () => {
  it('renders title', () => {
    const { getByText } = render(<PageHeader title='Page Title' />)
    const titleElement = getByText('Page Title')

    expect(titleElement).toBeInTheDocument()
  })

  it('does not render subtitle when not provided', () => {
    const { queryByText } = render(<PageHeader title='Page Title' />)
    const subtitleElement = queryByText('Subtitle')

    expect(subtitleElement).not.toBeInTheDocument()
  })
})
