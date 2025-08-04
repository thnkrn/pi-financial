import FallbackSpinner from '@/@core/components/spinner'
import { render } from '@testing-library/react'

describe('FallbackSpinner Component', () => {
  it('should render the FallbackSpinner component', () => {
    const { container } = render(<FallbackSpinner />)

    expect(container).toBeInTheDocument()
  })

  it('should have the correct styles', () => {
    const { container } = render(<FallbackSpinner />)

    const fallbackSpinner = container.firstChild

    expect(fallbackSpinner).toHaveStyle({
      height: '100vh',
      display: 'flex',
      alignItems: 'center',
      flexDirection: 'column',
      justifyContent: 'center',
    })
  })

  it('should have an SVG element', () => {
    const { container } = render(<FallbackSpinner />)

    const svgElement = container.querySelector('svg')

    expect(svgElement).toBeInTheDocument()
  })

  it('should have a CircularProgress component', () => {
    const { container } = render(<FallbackSpinner />)

    const circularProgress = container.querySelector('svg')

    expect(circularProgress).toBeInTheDocument()
  })
})
