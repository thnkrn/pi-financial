import { Session } from 'next-auth'
import React from 'react'

export const SessionContext = React.createContext<Session>(undefined!)

export const SessionProvider = ({ session, children }: { session: Session; children: React.ReactNode }) => {
  return <SessionContext.Provider value={session}>{children}</SessionContext.Provider>
}

export const useSessionContext = () => React.useContext(SessionContext)

export default SessionContext
