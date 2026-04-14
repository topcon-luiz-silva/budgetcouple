import { useRef, useState } from 'react'
import { Download, Upload } from 'lucide-react'
import { useBackup } from '@/hooks/useBackup'
import { Button } from '@/components/ui/button'
import { toast } from 'sonner'

export function ConfiguracoesPage() {
  const fileInputRef = useRef<HTMLInputElement>(null)
  const [importMode, setImportMode] = useState<'merge' | 'replace'>('merge')
  const { exportBackup, isExporting, importBackup, isImporting } = useBackup()

  const handleImportFile = (file: File) => {
    importBackup(
      { file, mode: importMode },
      {
        onSuccess: () => {
          toast.success(`Backup importado com sucesso (modo: ${importMode})`)
          if (fileInputRef.current) {
            fileInputRef.current.value = ''
          }
        },
        onError: (error: any) => {
          toast.error(error.response?.data?.error || 'Erro ao importar backup')
        },
      }
    )
  }

  return (
    <div className="container max-w-2xl mx-auto py-8 px-4">
      <div className="space-y-8">
        <div>
          <h1 className="text-3xl font-bold text-slate-900">Configurações</h1>
          <p className="text-slate-600 mt-2">Gerencie suas preferências e dados</p>
        </div>

        {/* Backup Section */}
        <div className="space-y-6">
          <div className="bg-white rounded-lg border border-slate-200 p-6">
            <h2 className="text-xl font-semibold text-slate-900 mb-4">
              Backup e Restauração
            </h2>
            <p className="text-sm text-slate-600 mb-6">
              Exporte ou importe seus dados do BudgetCouple. Use para fazer backup ou
              transferir dados entre dispositivos.
            </p>

            <div className="space-y-4">
              {/* Export */}
              <div>
                <h3 className="text-sm font-semibold text-slate-900 mb-3">
                  Exportar Backup
                </h3>
                <p className="text-xs text-slate-500 mb-3">
                  Baixe todos os seus dados como um arquivo JSON
                </p>
                <Button
                  onClick={() => exportBackup()}
                  disabled={isExporting}
                  className="w-full sm:w-auto"
                >
                  <Download className="w-4 h-4 mr-2" />
                  {isExporting ? 'Exportando...' : 'Exportar Backup'}
                </Button>
              </div>

              {/* Import */}
              <div className="pt-6 border-t border-slate-200">
                <h3 className="text-sm font-semibold text-slate-900 mb-3">
                  Importar Backup
                </h3>
                <p className="text-xs text-slate-500 mb-3">
                  Restaure seus dados a partir de um arquivo JSON de backup
                </p>

                <input
                  ref={fileInputRef}
                  type="file"
                  accept=".json"
                  onChange={(e) => {
                    if (e.target.files?.length) {
                      const confirmed = window.confirm(
                        `Tem certeza? Isso pode sobrescrever seus dados (modo: ${importMode})`
                      )
                      if (confirmed) {
                        handleImportFile(e.target.files[0])
                      }
                    }
                  }}
                  disabled={isImporting}
                  className="hidden"
                />

                <div className="space-y-3">
                  <div>
                    <label className="text-xs font-medium text-slate-700 block mb-2">
                      Modo de Importação
                    </label>
                    <select
                      value={importMode}
                      onChange={(e) =>
                        setImportMode(e.target.value as 'merge' | 'replace')
                      }
                      disabled={isImporting}
                      className="w-full px-3 py-2 border border-slate-300 rounded-md text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
                    >
                      <option value="merge">
                        Mesclar (adiciona dados novos, mantém existentes)
                      </option>
                      <option value="replace">
                        Substituir (limpa e restaura tudo)
                      </option>
                    </select>
                    <p className="text-xs text-slate-500 mt-2">
                      {importMode === 'merge'
                        ? 'Novos registros serão adicionados. Registros existentes serão mantidos.'
                        : 'Aviso: todos os dados serão substituídos!'}
                    </p>
                  </div>

                  <Button
                    onClick={() => fileInputRef.current?.click()}
                    disabled={isImporting}
                    variant="outline"
                    className="w-full sm:w-auto"
                  >
                    <Upload className="w-4 h-4 mr-2" />
                    {isImporting ? 'Importando...' : 'Selecionar arquivo'}
                  </Button>
                </div>
              </div>
            </div>
          </div>

          {/* Info Section */}
          <div className="bg-blue-50 border border-blue-200 rounded-lg p-4">
            <p className="text-xs text-blue-900">
              <strong>Dica:</strong> Faça backup regularmente de seus dados. O arquivo
              JSON contém todas as suas informações (contas, cartões, categorias,
              lançamentos, etc.).
            </p>
          </div>
        </div>
      </div>
    </div>
  )
}
