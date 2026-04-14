import { api } from '@/lib/api'

export interface AuthStatusDto {
  pinConfigured: boolean
  locked: boolean
  lockedUntil: string | null
}

export interface AuthResultDto {
  token: string
  expiresAt: string
}

export const authApi = {
  getStatus: async (): Promise<AuthStatusDto> => {
    const { data } = await api.get<AuthStatusDto>('/auth/status')
    return data
  },

  setupPin: async (pin: string): Promise<AuthResultDto> => {
    const { data } = await api.post<AuthResultDto>('/auth/setup-pin', { pin })
    return data
  },

  login: async (pin: string): Promise<AuthResultDto> => {
    const { data } = await api.post<AuthResultDto>('/auth/login', { pin })
    return data
  },

  changePin: async (pinAtual: string, novoPin: string): Promise<void> => {
    await api.post('/auth/change-pin', { pinAtual, novoPin })
  },
}
