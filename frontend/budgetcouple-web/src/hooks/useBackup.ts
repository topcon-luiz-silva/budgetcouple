import { useMutation } from '@tanstack/react-query'
import { api } from '@/lib/api'

export function useBackup() {
  const exportMutation = useMutation({
    mutationFn: async () => {
      const response = await api.get('/backup/export', {
        responseType: 'blob',
      })
      return response
    },
  })

  const importMutation = useMutation({
    mutationFn: async ({ file, mode }: { file: File; mode: 'merge' | 'replace' }) => {
      const formData = new FormData()
      formData.append('arquivo', file)

      const { data } = await api.post(`/backup/import?modo=${mode}`, formData, {
        headers: {
          'Content-Type': 'multipart/form-data',
        },
      })
      return data
    },
  })

  const handleExport = () => {
    exportMutation.mutate(undefined, {
      onSuccess: (response: any) => {
        const url = window.URL.createObjectURL(new Blob([response.data]))
        const link = document.createElement('a')
        link.href = url
        link.setAttribute(
          'download',
          `budgetcouple-backup-${new Date().toISOString().split('T')[0]}.json`
        )
        document.body.appendChild(link)
        link.click()
        link.parentElement?.removeChild(link)
        window.URL.revokeObjectURL(url)
      },
    })
  }

  return {
    exportBackup: handleExport,
    isExporting: exportMutation.isPending,
    importBackup: importMutation.mutate,
    isImporting: importMutation.isPending,
  }
}
