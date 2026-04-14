import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { api } from '@/lib/api'

export interface Anexo {
  id: string
  nomeArquivo: string
  contentType: string
  tamanhoBytes: number
  enviadoEm: string
}

export function useLancamentoAnexos(lancamentoId: string) {
  const queryClient = useQueryClient()

  const anexosQuery = useQuery({
    queryKey: ['lancamentos', lancamentoId, 'anexos'],
    queryFn: async () => {
      const { data } = await api.get<Anexo[]>(
        `/lancamentos/${lancamentoId}/anexos`
      )
      return data
    },
    enabled: !!lancamentoId,
  })

  const uploadMutation = useMutation({
    mutationFn: async ({ file, lancamentoId }: { file: File; lancamentoId: string }) => {
      const formData = new FormData()
      formData.append('arquivo', file)

      const { data } = await api.post(
        `/lancamentos/${lancamentoId}/anexos`,
        formData,
        {
          headers: {
            'Content-Type': 'multipart/form-data',
          },
        }
      )
      return data
    },
    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: ['lancamentos', lancamentoId, 'anexos'],
      })
    },
  })

  const downloadMutation = useMutation({
    mutationFn: async ({ lancamentoId, anexoId }: { lancamentoId: string; anexoId: string }) => {
      const response = await api.get(
        `/lancamentos/${lancamentoId}/anexos/${anexoId}/download`,
        {
          responseType: 'blob',
        }
      )
      return response
    },
  })

  const deleteMutation = useMutation({
    mutationFn: async ({ lancamentoId, anexoId }: { lancamentoId: string; anexoId: string }) => {
      await api.delete(`/lancamentos/${lancamentoId}/anexos/${anexoId}`)
    },
    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: ['lancamentos', lancamentoId, 'anexos'],
      })
    },
  })

  return {
    anexos: anexosQuery.data ?? [],
    isLoadingAnexos: anexosQuery.isLoading,
    isErrorAnexos: anexosQuery.isError,
    errorAnexos: anexosQuery.error,
    uploadAnexo: uploadMutation.mutate,
    isUploadingAnexo: uploadMutation.isPending,
    downloadAnexo: downloadMutation.mutate,
    isDownloadingAnexo: downloadMutation.isPending,
    deleteAnexo: deleteMutation.mutate,
    isDeletingAnexo: deleteMutation.isPending,
  }
}
