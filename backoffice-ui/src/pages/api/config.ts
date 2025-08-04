import 'dotenv/config'

export interface Config {
  backOfficeService: {
    baseUrl: string
  }
  blocktradeService: {
    baseUrl: string
  }
  keycloak: {
    authServerUrl: string
    realm: string
    clientId: string
    clientSecret: string
  }
  ssoadminService: {
    baseUrl: string
  }
}

export const config: Config = {
  backOfficeService: {
    baseUrl: process.env.NEXT_PUBLIC_BACK_OFFICE_SERVICE_BASE_URL!,
  },
  ssoadminService: {
    baseUrl: process.env.NEXT_PUBLIC_SSOADMIN_API_BASE_URL!,
  },
  blocktradeService: {
    baseUrl: process.env.NEXT_PUBLIC_BLOCKTRADE_SERVICE_BASE_URL!,
  },
  keycloak: {
    authServerUrl: process.env.KEYCLOAK_AUTH_SERVER_URL!,
    realm: process.env.KEYCLOAK_REALM!,
    clientId: process.env.KEYCLOAK_CLIENT_ID!,
    clientSecret: process.env.KEYCLOAK_CLIENT_SECRET!,
  },
}

export default config
