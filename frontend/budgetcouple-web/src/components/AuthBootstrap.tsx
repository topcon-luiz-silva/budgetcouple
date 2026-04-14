import { useNavigate, useLocation } from 'react-router-dom'
import { useEffect } from 'react'
import { useAuthStatus } from '@/features/auth/hooks'
import { useAuthStore } from '@/stores/useAuthStore'
import { Loader2 } from 'lucide-react'

export interface AuthBootstrapProps {
  children: React.ReactNode
}

export function AuthBootstrap({ children }: AuthBootstrapProps) {
  const navigate = useNavigate()
  const location = useLocation()
  const { data: status, isLoading } = useAuthStatus()
  const { isAuthenticated } = useAuthStore()

  useEffect(() => {
    if (isLoading || !status) return

    const publicPaths = ['/setup-pin', '/login', '/health']
    const currentPath = location.pathname

    // If PIN not configured yet, force to setup page
    if (!status.pinConfigured) {
      if (currentPath !== '/setup-pin') {
        navigate('/setup-pin', { replace: true })
      }
      return
    }

    // If PIN configured but not authenticated, force to login
    if (!isAuthenticated()) {
      if (!publicPaths.includes(currentPath)) {
        navigate('/login', { replace: true, state: { from: location } })
      }
      return
    }

    // If authenticated, allow navigation
    if (['/setup-pin', '/login'].includes(currentPath)) {
      navigate('/', { replace: true })
    }
  }, [isLoading, status, isAuthenticated, location, navigate])

  if (isLoading) {
    return (
      <div className="min-h-screen flex items-center justify-center bg-slate-50">
        <Loader2 className="h-8 w-8 text-slate-600 animate-spin" />
      </div>
    )
  }

  return <>{children}</>
}
