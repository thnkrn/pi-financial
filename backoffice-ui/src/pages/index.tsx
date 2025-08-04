import { useSession } from 'next-auth/react'
import { useRouter } from 'next/router'
import { ReactNode, useEffect } from 'react'
import BlankLayout from 'src/@core/layouts/BlankLayout'

const Home = () => {
  const router = useRouter()

  const session = useSession()

  useEffect(() => {
    if (session.status === 'unauthenticated' || session.data?.error === 'RefreshAccessTokenError') {
      router.push('/signin')
    } else if (session.status === 'authenticated') {
      router.push('/dashboard')
    }
  }, [router, session])

  return <></>
}

Home.getLayout = (page: ReactNode) => <BlankLayout>{page}</BlankLayout>

Home.guestGuard = true

export default Home
