import { getToken } from 'next-auth/jwt'
import { withAuth } from 'next-auth/middleware'
import { NextResponse } from 'next/server'
import { ALL_PATHS_ROLES } from './lib/auth/path'

export default withAuth(
  async function middleware(req) {
    const { pathname } = req.nextUrl

    const protectedPath = ALL_PATHS_ROLES.find(pathRoles => pathname.startsWith(pathRoles.path))

    if (protectedPath) {
      const token = await getToken({ req })
      if (
        !!token &&
        !(protectedPath.roles.length === 0 || protectedPath.roles.some(role => token.roles?.includes(role)))
      ) {
        return NextResponse.redirect(new URL('/error?reason=forbidden', req.url))
      }
    }
  },
  {
    pages: {
      signIn: '/signin',
    },

    callbacks: {
      authorized: ({ token }) => {
        return !!token && token?.error !== 'RefreshAccessTokenError'
      },
    },
  }
)

export const config = {
  matcher: ['/((?!signin|error|api).*)', '/api/sso/:path*'],
}
