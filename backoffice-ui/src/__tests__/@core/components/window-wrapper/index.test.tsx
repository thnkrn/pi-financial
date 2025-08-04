import WindowWrapper from '@/@core/components/window-wrapper'
import { render } from '@testing-library/react'

jest.mock('next/router', () => ({
  useRouter: () => ({ route: '/mock-route' }),
}))

describe('WindowWrapper Component', () => {
  it('should render the children when the window is ready', () => {
    const { getByText } = render(
      <WindowWrapper>
        <div>Child Component</div>
      </WindowWrapper>
    )

    expect(getByText('Child Component')).toBeInTheDocument()
  })
})
