import { useState, useRef } from 'react'
import { FileUp, Download, Trash2, FileIcon } from 'lucide-react'
import { useLancamentoAnexos } from '@/hooks/useLancamentoAnexos'
import { Button } from '@/components/ui/button'
import { toast } from 'sonner'

interface LancamentoAnexosSectionProps {
  lancamentoId: string
}

export function LancamentoAnexosSection({ lancamentoId }: LancamentoAnexosSectionProps) {
  const fileInputRef = useRef<HTMLInputElement>(null)
  const [isDragging, setIsDragging] = useState(false)

  const {
    anexos,
    isLoadingAnexos,
    uploadAnexo,
    isUploadingAnexo,
    downloadAnexo,
    isDownloadingAnexo,
    deleteAnexo,
    isDeletingAnexo,
  } = useLancamentoAnexos(lancamentoId)

  const handleFileSelect = (file: File) => {
    const maxSize = 10 * 1024 * 1024 // 10 MB
    const allowedTypes = ['image/jpeg', 'image/png', 'image/gif', 'application/pdf']

    if (file.size > maxSize) {
      toast.error('Arquivo não pode exceder 10 MB')
      return
    }

    if (!allowedTypes.includes(file.type)) {
      toast.error('Tipo de arquivo não permitido. Aceitar: imagens (JPG, PNG, GIF) e PDF.')
      return
    }

    uploadAnexo({ file, lancamentoId }, {
      onSuccess: () => {
        toast.success('Anexo enviado com sucesso')
        if (fileInputRef.current) {
          fileInputRef.current.value = ''
        }
      },
      onError: (_error: any) => {
        toast.error(_error.response?.data?.error || 'Erro ao enviar anexo')
      },
    })
  }

  const handleDragOver = (e: React.DragEvent<HTMLDivElement>) => {
    e.preventDefault()
    setIsDragging(true)
  }

  const handleDragLeave = () => {
    setIsDragging(false)
  }

  const handleDrop = (e: React.DragEvent<HTMLDivElement>) => {
    e.preventDefault()
    setIsDragging(false)
    const files = e.dataTransfer.files
    if (files.length > 0) {
      handleFileSelect(files[0])
    }
  }

  const handleDownload = (anexoId: string, nomeArquivo: string) => {
    downloadAnexo({ lancamentoId, anexoId }, {
      onSuccess: (response: any) => {
        const url = window.URL.createObjectURL(new Blob([response.data]))
        const link = document.createElement('a')
        link.href = url
        link.setAttribute('download', nomeArquivo)
        document.body.appendChild(link)
        link.click()
        link.parentElement?.removeChild(link)
        window.URL.revokeObjectURL(url)
      },
      onError: (_error: any) => {
        toast.error('Erro ao fazer download')
      },
    })
  }

  const handleDelete = (anexoId: string) => {
    if (!window.confirm('Tem certeza que deseja deletar este anexo?')) return

    deleteAnexo({ lancamentoId, anexoId }, {
      onSuccess: () => {
        toast.success('Anexo deletado com sucesso')
      },
      onError: (_error: any) => {
        toast.error('Erro ao deletar anexo')
      },
    })
  }

  const formatFileSize = (bytes: number) => {
    if (bytes === 0) return '0 B'
    const k = 1024
    const sizes = ['B', 'KB', 'MB']
    const i = Math.floor(Math.log(bytes) / Math.log(k))
    return Math.round((bytes / Math.pow(k, i)) * 100) / 100 + ' ' + sizes[i]
  }

  const formatDate = (dateString: string) => {
    const date = new Date(dateString)
    return new Intl.DateTimeFormat('pt-BR', {
      year: 'numeric',
      month: '2-digit',
      day: '2-digit',
      hour: '2-digit',
      minute: '2-digit',
    }).format(date)
  }

  return (
    <div className="space-y-4">
      <h3 className="text-sm font-semibold text-slate-900">Anexos</h3>

      {/* Upload Area */}
      <div
        onDragOver={handleDragOver}
        onDragLeave={handleDragLeave}
        onDrop={handleDrop}
        className={`border-2 border-dashed rounded-lg p-6 text-center transition-colors ${
          isDragging
            ? 'border-blue-500 bg-blue-50'
            : 'border-slate-300 hover:border-slate-400'
        }`}
      >
        <input
          ref={fileInputRef}
          type="file"
          accept="image/*,application/pdf"
          onChange={(e) => {
            if (e.target.files?.length) {
              handleFileSelect(e.target.files[0])
            }
          }}
          className="hidden"
          disabled={isUploadingAnexo}
        />

        <div className="flex flex-col items-center space-y-2">
          <FileUp className="w-8 h-8 text-slate-400" />
          <div>
            <p className="text-sm font-medium text-slate-900">
              Clique ou arraste arquivos aqui
            </p>
            <p className="text-xs text-slate-500">
              Máx. 10 MB - Imagens (JPG, PNG, GIF) e PDF
            </p>
          </div>
        </div>

        <Button
          type="button"
          onClick={() => fileInputRef.current?.click()}
          disabled={isUploadingAnexo}
          className="mt-4"
          variant="outline"
          size="sm"
        >
          {isUploadingAnexo ? 'Enviando...' : 'Selecionar arquivo'}
        </Button>
      </div>

      {/* Attachments List */}
      {isLoadingAnexos ? (
        <div className="text-center py-4 text-sm text-slate-500">
          Carregando anexos...
        </div>
      ) : anexos.length === 0 ? (
        <div className="text-center py-4 text-sm text-slate-500">
          Nenhum anexo adicionado
        </div>
      ) : (
        <div className="space-y-2">
          {anexos.map((anexo) => (
            <div
              key={anexo.id}
              className="flex items-center justify-between p-3 rounded-lg bg-slate-50 border border-slate-200 hover:bg-slate-100 transition-colors"
            >
              <div className="flex items-center space-x-3 flex-1 min-w-0">
                <FileIcon className="w-4 h-4 text-slate-400 flex-shrink-0" />
                <div className="min-w-0 flex-1">
                  <p className="text-sm font-medium text-slate-900 truncate">
                    {anexo.nomeArquivo}
                  </p>
                  <p className="text-xs text-slate-500">
                    {formatFileSize(anexo.tamanhoBytes)} • {formatDate(anexo.enviadoEm)}
                  </p>
                </div>
              </div>

              <div className="flex items-center space-x-2 flex-shrink-0">
                <Button
                  onClick={() => handleDownload(anexo.id, anexo.nomeArquivo)}
                  disabled={isDownloadingAnexo}
                  size="sm"
                  variant="ghost"
                  aria-label={`Baixar ${anexo.nomeArquivo}`}
                >
                  <Download className="w-4 h-4" />
                </Button>
                <Button
                  onClick={() => handleDelete(anexo.id)}
                  disabled={isDeletingAnexo}
                  size="sm"
                  variant="ghost"
                  className="text-red-600 hover:text-red-700"
                  aria-label={`Deletar ${anexo.nomeArquivo}`}
                >
                  <Trash2 className="w-4 h-4" />
                </Button>
              </div>
            </div>
          ))}
        </div>
      )}
    </div>
  )
}
