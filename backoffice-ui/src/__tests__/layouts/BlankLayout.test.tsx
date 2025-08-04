import BlankLayout from '@/@core/layouts/BlankLayout'
import { render } from '@testing-library/react'

describe('BlankLayout Component', () => {
  it('should render the children', () => {
    const { getByText } = render(
      <BlankLayout>
        <div>Child Component</div>
      </BlankLayout>
    )

    expect(getByText('Child Component')).toBeInTheDocument()
  })
})
