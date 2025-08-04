import Repeater from '@/@core/components/repeater'
import '@testing-library/jest-dom'
import { render } from '@testing-library/react'

describe('Repeater Component', () => {
  it('renders the children the specified number of times', () => {
    const { getAllByText } = render(<Repeater count={3}>{i => <div key={i}>Item {i}</div>}</Repeater>)

    const items = getAllByText(/Item/)
    expect(items.length).toBe(3)
  })

  it('renders using a custom tag when provided', () => {
    const { container } = render(
      <Repeater count={2} tag='ul'>
        {i => <li key={i}>Item {i}</li>}
      </Repeater>
    )

    const listElement = container.querySelector('ul')
    expect(listElement).toBeInTheDocument()

    const items = container.querySelectorAll('li')
    expect(items.length).toBe(2)
  })

  it('renders using a default div tag when no tag is provided', () => {
    const { container } = render(<Repeater count={4}>{i => <div key={i}>Item {i}</div>}</Repeater>)

    const divElements = container.querySelectorAll('div')
    expect(divElements.length).toBe(5)
  })
})
