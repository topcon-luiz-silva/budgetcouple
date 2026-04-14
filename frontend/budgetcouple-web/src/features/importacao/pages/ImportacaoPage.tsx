import { useState } from 'react'
import { useTranslation } from 'react-i18next'
import { useConfirmImport, useImportPreview } from '../hooks'
import { useCategoriasList } from '@/features/categorias/hooks'
import { useContasList } from '@/features/contas/hooks'
import { useCartoesList } from '@/features/cartoes/hooks'
import { Button } from '@/components/ui/button'
import { Card } from '@/components/ui/card'
import { Upload, CheckCircle, AlertTriangle } from 'lucide-react'
import type { ImportItemDto, ConfirmImportItemDto } from '../types'

interface EditableImportItem extends ImportItemDto {
  categoriaId?: string | null
  subcategoriaId?: string | null
}

export function ImportacaoPage() {
  const { t } = useTranslation()
  const [step, setStep] = useState<'upload' | 'preview' | 'confirm'>('upload')
  const [selectedFile, setSelectedFile] = useState<File | null>(null)
  const [selectedAccountId, setSelectedAccountId] = useState<string | null>(null)
  const [selectedCardId, setSelectedCardId] = useState<string | null>(null)
  const [previewItems, setPreviewItems] = useState<EditableImportItem[]>([])

  const { mutate: previewImport, isPending: isPreviewLoading } = useImportPreview()
  const { mutate: confirmImport, isPending: isConfirmLoading } = useConfirmImport()
  const { data: categorias = [] } = useCategoriasList()
  const { data: contas = [] } = useContasList()
  const { data: cartoes = [] } = useCartoesList()

  const handleFileSelect = (e: React.ChangeEvent<HTMLInputElement>) => {
    if (e.target.files?.[0]) {
      setSelectedFile(e.target.files[0])
    }
  }

  const handlePreview = () => {
    if (!selectedFile) {
      alert(t('importacao.selectFile'))
      return
    }

    if (!selectedAccountId && !selectedCardId) {
      alert(t('importacao.selectAccountOrCard'))
      return
    }

    previewImport(
      { file: selectedFile, contaId: selectedAccountId, cartaoId: selectedCardId },
      {
        onSuccess: (data) => {
          setPreviewItems(
            data.lancamentos.map((item) => ({
              ...item,
              categoriaId: item.categoriaSugeridaId || null,
            }))
          )
          setStep('preview')
        },
        onError: (error: any) => {
          alert(error.response?.data?.message || t('importacao.previewError'))
        },
      }
    )
  }

  const handleCategoryChange = (index: number, categoryId: string) => {
    const updated = [...previewItems]
    updated[index].categoriaId = categoryId || null
    setPreviewItems(updated)
  }

  const handleConfirmImport = () => {
    const confirmItems: ConfirmImportItemDto[] = previewItems.map((item) => ({
      descricao: item.descricao,
      valor: item.valor,
      dataCompetencia: item.dataCompetencia,
      categoriaId: item.categoriaId || null,
      subcategoriaId: item.subcategoriaId || null,
      duplicado: item.duplicado,
      hashImportacao: item.hashImportacao,
    }))

    confirmImport(
      {
        contaId: selectedAccountId,
        cartaoId: selectedCardId,
        lancamentos: confirmItems,
      },
      {
        onSuccess: () => {
          setStep('confirm')
        },
        onError: (error: any) => {
          alert(error.response?.data?.message || t('importacao.confirmError'))
        },
      }
    )
  }

  const handleReset = () => {
    setStep('upload')
    setSelectedFile(null)
    setSelectedAccountId(null)
    setSelectedCardId(null)
    setPreviewItems([])
  }

  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-3xl font-bold">{t('importacao.title')}</h1>
        <p className="text-gray-600 mt-2">{t('importacao.subtitle')}</p>
      </div>

      {step === 'upload' && (
        <Card className="p-6">
          <div className="space-y-6">
            {/* Account/Card Selection */}
            <div className="grid grid-cols-2 gap-4">
              <div>
                <label className="block text-sm font-medium mb-2">{t('importacao.account')}</label>
                <select
                  value={selectedAccountId || ''}
                  onChange={(e) => setSelectedAccountId(e.target.value || null)}
                  className="w-full px-3 py-2 border border-gray-300 rounded-md"
                >
                  <option value="">{t('importacao.selectAccount')}</option>
                  {contas.map((conta: any) => (
                    <option key={conta.id} value={conta.id}>
                      {conta.nome}
                    </option>
                  ))}
                </select>
              </div>

              <div>
                <label className="block text-sm font-medium mb-2">{t('importacao.card')}</label>
                <select
                  value={selectedCardId || ''}
                  onChange={(e) => setSelectedCardId(e.target.value || null)}
                  className="w-full px-3 py-2 border border-gray-300 rounded-md"
                >
                  <option value="">{t('importacao.selectCard')}</option>
                  {cartoes.map((cartao: any) => (
                    <option key={cartao.id} value={cartao.id}>
                      {cartao.nome}
                    </option>
                  ))}
                </select>
              </div>
            </div>

            {/* File Upload */}
            <div className="border-2 border-dashed border-gray-300 rounded-lg p-6 text-center cursor-pointer hover:border-blue-400 transition">
              <input
                type="file"
                accept=".csv,.ofx"
                onChange={handleFileSelect}
                className="hidden"
                id="file-input"
              />
              <label htmlFor="file-input" className="cursor-pointer">
                <Upload className="w-12 h-12 mx-auto mb-2 text-gray-400" />
                <p className="text-sm font-medium">{t('importacao.dragDrop')}</p>
                <p className="text-xs text-gray-500">{t('importacao.supportedFormats')}</p>
                {selectedFile && <p className="text-sm text-green-600 mt-2">{selectedFile.name}</p>}
              </label>
            </div>

            <Button onClick={handlePreview} disabled={isPreviewLoading || !selectedFile}>
              {isPreviewLoading ? t('common.loading') : t('importacao.previewBtn')}
            </Button>
          </div>
        </Card>
      )}

      {step === 'preview' && (
        <Card className="p-6">
          <div className="space-y-4">
            <div className="flex items-center justify-between">
              <h2 className="text-xl font-semibold">{t('importacao.preview')}</h2>
              <Button variant="outline" onClick={() => setStep('upload')}>
                {t('common.back')}
              </Button>
            </div>

            {/* Import Summary */}
            <div className="grid grid-cols-3 gap-4 mb-6">
              <Card className="p-4 bg-blue-50">
                <p className="text-sm text-gray-600">{t('importacao.totalItems')}</p>
                <p className="text-2xl font-bold">{previewItems.length}</p>
              </Card>
              <Card className="p-4 bg-yellow-50">
                <p className="text-sm text-gray-600">{t('importacao.duplicates')}</p>
                <p className="text-2xl font-bold">{previewItems.filter((i) => i.duplicado).length}</p>
              </Card>
              <Card className="p-4 bg-green-50">
                <p className="text-sm text-gray-600">{t('importacao.newItems')}</p>
                <p className="text-2xl font-bold">
                  {previewItems.filter((i) => !i.duplicado).length}
                </p>
              </Card>
            </div>

            {/* Items Table */}
            <div className="overflow-x-auto">
              <table className="w-full text-sm">
                <thead>
                  <tr className="border-b">
                    <th className="text-left py-2 px-4">{t('importacao.date')}</th>
                    <th className="text-left py-2 px-4">{t('importacao.description')}</th>
                    <th className="text-right py-2 px-4">{t('importacao.value')}</th>
                    <th className="text-left py-2 px-4">{t('importacao.category')}</th>
                    <th className="text-center py-2 px-4">{t('importacao.status')}</th>
                  </tr>
                </thead>
                <tbody>
                  {previewItems.map((item, idx) => (
                    <tr key={idx} className="border-b hover:bg-gray-50">
                      <td className="py-2 px-4">{new Date(item.dataCompetencia).toLocaleDateString()}</td>
                      <td className="py-2 px-4">{item.descricao}</td>
                      <td className="text-right py-2 px-4 font-medium">
                        {new Intl.NumberFormat('pt-BR', {
                          style: 'currency',
                          currency: 'BRL',
                        }).format(item.valor)}
                      </td>
                      <td className="py-2 px-4">
                        <select
                          value={item.categoriaId || ''}
                          onChange={(e) => handleCategoryChange(idx, e.target.value)}
                          className="px-2 py-1 border border-gray-300 rounded text-sm"
                        >
                          <option value="">{t('importacao.selectCategory')}</option>
                          {categorias.map((cat: any) => (
                            <option key={cat.id} value={cat.id}>
                              {cat.nome}
                            </option>
                          ))}
                        </select>
                      </td>
                      <td className="text-center py-2 px-4">
                        {item.duplicado ? (
                          <div className="flex items-center justify-center gap-1 text-yellow-600">
                            <AlertTriangle className="w-4 h-4" />
                            <span className="text-xs">{t('importacao.duplicate')}</span>
                          </div>
                        ) : (
                          <div className="flex items-center justify-center gap-1 text-green-600">
                            <CheckCircle className="w-4 h-4" />
                            <span className="text-xs">{t('importacao.new')}</span>
                          </div>
                        )}
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>

            {/* Action Buttons */}
            <div className="flex gap-2 pt-4">
              <Button
                onClick={handleConfirmImport}
                disabled={isConfirmLoading}
                className="flex-1"
              >
                {isConfirmLoading ? t('common.loading') : t('importacao.confirmBtn')}
              </Button>
              <Button variant="outline" onClick={() => setStep('upload')} className="flex-1">
                {t('common.cancel')}
              </Button>
            </div>
          </div>
        </Card>
      )}

      {step === 'confirm' && (
        <Card className="p-6">
          <div className="space-y-4 text-center">
            <CheckCircle className="w-16 h-16 mx-auto text-green-600" />
            <div>
              <h2 className="text-2xl font-bold">{t('importacao.importSuccess')}</h2>
              <p className="text-gray-600 mt-2">{t('importacao.importCompleted')}</p>
            </div>

            <Button onClick={handleReset} className="w-full">
              {t('importacao.importNew')}
            </Button>
          </div>
        </Card>
      )}
    </div>
  )
}
