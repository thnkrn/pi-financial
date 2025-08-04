import Breadcrumbs, { IContent } from '@/@core/components/breadcrumbs'
import '@testing-library/jest-dom'
import { render } from '@testing-library/react'

const mockContents: IContent[] = [
  { name: 'Home', href: '/' },
  { name: 'Category', href: '/category' },
  { name: 'Product', href: '/category/product' },
]

describe('Breadcrumbs Component', () => {
  it('renders null when contents are empty', () => {
    const { container } = render(<Breadcrumbs contents={[]} />)
    expect(container.firstChild).toBeNull()
  })

  it('renders breadcrumbs with links when contents are provided', () => {
    const { getByText } = render(<Breadcrumbs contents={mockContents} />)

    mockContents.forEach(content => {
      const linkElement = getByText(content.name)
      expect(linkElement).toBeInTheDocument()
      if (content.href) {
        expect(linkElement).toHaveAttribute('href', content.href)
      }
    })
  })
})
