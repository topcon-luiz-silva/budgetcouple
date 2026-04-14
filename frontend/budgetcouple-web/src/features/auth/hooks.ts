import { useMutation, useQuery } from '@tanstack/react-query'
import { useNavigate } from 'react-router-dom'
import type { AuthResultDto } from './api'
import { authApi } from './api'
import { useAuthStore } from '@/stores/useAuthStore'

export function useAuthStatus() {
  return useQuery({
    queryKey: ['auth', 'status'],
    queryFn: () => authApi.getStatus(),
    staleTime: 30000,
  })
}

export function useSetupPin() {
  const navigate = useNavigate()
  const { setAuth } = useAuthStore()

  return useMutation({
    mutationFn: (pin: string) => authApi.setupPin(pin),
    onSuccess: (data: AuthResultDto) => {
      setAuth(data.token, data.expiresAt)
      navigate('/')
    },
  })
}

export function useLogin() {
  const navigate = useNavigate()
  const { setAuth } = useAuthStore()

  return useMutation({
    mutationFn: (pin: string) => authApi.login(pin),
    onSuccess: (data: AuthResultDto) => {
      setAuth(data.token, data.expiresAt)
      navigate('/')
    },
  })
}

export function useChangePin() {
  const navigate = useNavigate()

  return useMutation({
    mutationFn: ({ pinAtual, novoPin }: { pinAtual: string; novoPin: string }) =>
      authApi.changePin(pinAtual, novoPin),
    onSuccess: () => {
      navigate('/settings')
    },
  })
}
