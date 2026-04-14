import { api } from '@/lib/api'
import type { ImportPreviewDto, ConfirmImportDto, ConfirmImportResultDto } from './types'

export const importacaoApi = {
  preview: async (
    file: File,
    contaId?: string | null,
    cartaoId?: string | null
  ): Promise<ImportPreviewDto> => {
    const formData = new FormData()
    formData.append('file', file)

    const params = new URLSearchParams()
    if (contaId) params.append('contaId', contaId)
    if (cartaoId) params.append('cartaoId', cartaoId)

    const url = `/importacoes/preview${params.toString() ? `?${params}` : ''}`
    const { data } = await api.post<ImportPreviewDto>(url, formData, {
      headers: {
        'Content-Type': 'multipart/form-data',
      },
    })
    return data
  },

  confirm: async (input: ConfirmImportDto): Promise<ConfirmImportResultDto> => {
    const { data } = await api.post<ConfirmImportResultDto>('/importacoes/confirmar', input)
    return data
  },
}
