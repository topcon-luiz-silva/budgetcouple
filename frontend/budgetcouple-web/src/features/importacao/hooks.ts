import { useMutation } from '@tanstack/react-query'
import { importacaoApi } from './api'
import type { ConfirmImportDto } from './types'

export function useImportPreview() {
  return useMutation({
    mutationFn: ({
      file,
      contaId,
      cartaoId,
    }: {
      file: File
      contaId?: string | null
      cartaoId?: string | null
    }) => importacaoApi.preview(file, contaId, cartaoId),
  })
}

export function useConfirmImport() {
  return useMutation({
    mutationFn: (input: ConfirmImportDto) => importacaoApi.confirm(input),
  })
}
