import NextAuth, { NextAuthOptions, User } from 'next-auth'
import KeyCloakProvider from 'next-auth/providers/keycloak'
import { parseAccessToken } from 'src/utils/fmt'
import config from '../config'

declare module 'next-auth' {
  interface Session {
    user: User
    accessToken?: string
    rolesGroup: Array<string>
    roles: Array<string>
    error?: 'RefreshAccessTokenError'
  }
}

declare module 'next-auth/jwt' {
  interface JWT {
    user: User
    accessToken: string
    idToken: string
    expiresAt: number
    refreshToken: string
    rolesGroup?: Array<string>
    roles?: Array<string>
    error?: 'RefreshAccessTokenError'
  }
}

export const authOptions: NextAuthOptions = {
  providers: [
    KeyCloakProvider({
      issuer: `${config.keycloak.authServerUrl}/realms/${config.keycloak.realm}`,
      clientId: config.keycloak.clientId,
      clientSecret: config.keycloak.clientSecret,
    }),
  ],
  pages: {
    signIn: 'auth/signin',
  },
  events: {
    async signOut({ token }) {
      const logOutUrl = new URL(
        `${config.keycloak.authServerUrl}/realms/${config.keycloak.realm}/protocol/openid-connect/logout`
      )
      logOutUrl.searchParams.set('id_token_hint', token.idToken)
      await fetch(logOutUrl)
    },
  },
  callbacks: {
    async jwt({ token, account, user, profile }) {
      if (account) {
        return {
          accessToken: account.access_token!,
          expiresAt: account.expires_at! * 1000,
          refreshToken: account.refresh_token!,
          user: user,
          idToken: account.id_token!,

          //@ts-ignore
          roles: parseAccessToken(account.access_token).roles,

          //@ts-ignore
          rolesGroup: profile?.groups,
          error: token.error,
        }
      } else if (Date.now() < token.expiresAt) {
        return token
      } else {
        try {
          const response = await fetch(
            `${config.keycloak.authServerUrl}/realms/${config.keycloak.realm}/protocol/openid-connect/token`,
            {
              headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
              body: new URLSearchParams([
                ['client_id', config.keycloak.clientId],
                ['client_secret', config.keycloak.clientSecret],
                ['grant_type', 'refresh_token'],
                ['refresh_token', token.refreshToken],
              ]),
              method: 'POST',
            }
          )

          const tokens = await response.json()

          if (!response.ok) throw tokens

          return {
            ...token,
            accessToken: tokens.access_token,

            // Give 10 seconds buffer
            expiresAt: Date.now() + tokens.expires_in * 1000 - 10000,
            refreshToken: tokens.refresh_token,
            idToken: tokens.id_token,
          }
        } catch (error) {
          return { ...token, error: 'RefreshAccessTokenError' as const }
        }
      }
    },
    async session({ session, token }) {
      session.user = token.user
      session.accessToken = token.accessToken
      session.rolesGroup = token.rolesGroup ?? []
      session.roles = token.roles ?? []
      session.error = token.error

      return session
    },
  },
}

export default NextAuth(authOptions)
