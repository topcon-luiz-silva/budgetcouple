import { useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { useTranslation } from 'react-i18next'
import { useListMetas, useDeleteMeta, useGetMetaProgresso } from '../hooks'
import { Button } from '@/components/ui/button'
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card'
import { Dialog, DialogContent, DialogFooter, DialogTitle } from '@/components/ui/dialog'
import { Plus, Trash2, Edit2 } from 'lucide-react'

export const MetasListPage = () => {
  const navigate = useNavigate()
  const { t } = useTranslation()
  const [deleteId, setDeleteId] = useState<string | null>(null)

  const { data: metas = [], isLoading, error } = useListMetas()
  const deleteMutation = useDeleteMeta()

  const handleCreate = () => {
    navigate('/metas/novo')
  }

  const handleEdit = (id: string) => {
    navigate(`/metas/${id}/editar`)
  }

  const handleDeleteConfirm = async () => {
    if (!deleteId) return
    try {
      await deleteMutation.mutateAsync(deleteId)
      setDeleteId(null)
    } catch (err) {
      console.error('Erro ao excluir meta:', err)
    }
  }

  if (isLoading) {
    return (
      <div className="space-y-4">
        {[1, 2, 3].map((i) => (
          <div key={i} className="h-40 bg-slate-200 rounded-lg animate-pulse" />
        ))}
      </div>
    )
  }

  if (error) {
    return (
      <Card className="border-red-200 bg-red-50">
        <CardContent className="pt-6">
          <p className="text-sm text-red-800">{t('metas.error')}</p>
        </CardContent>
      </Card>
    )
  }

  return (
    <div className="space-y-6">
      <div className="flex justify-between items-center">
        <div>
          <h1 className="text-3xl font-bold text-slate-900">{t('metas.title')}</h1>
          <p className="text-slate-600 mt-1">{t('metas.description')}</p>
        </div>
        <Button onClick={handleCreate} className="gap-2">
          <Plus className="w-4 h-4" />
          {t('metas.new')}
        </Button>
      </div>

      {metas.length === 0 ? (
        <Card>
          <CardContent className="pt-12 pb-12 text-center">
            <p className="text-slate-600 mb-4">{t('metas.empty')}</p>
            <Button onClick={handleCreate} variant="outline">
              {t('metas.createFirst')}
            </Button>
          </CardContent>
        </Card>
      ) : (
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
          {metas.map((meta) => (
            <MetaCard
              key={meta.id}
              meta={meta}
              onEdit={handleEdit}
              onDelete={(id) => setDeleteId(id)}
            />
          ))}
        </div>
      )}

      <Dialog open={!!deleteId} onOpenChange={(open) => !open && setDeleteId(null)}>
        <DialogContent>
          <DialogTitle>{t('metas.delete.title')}</DialogTitle>
          <p className="text-sm text-slate-600">{t('metas.delete.confirm')}</p>
          <DialogFooter className="gap-2">
            <Button variant="outline" onClick={() => setDeleteId(null)}>
              {t('common.cancel')}
            </Button>
            <Button
              variant="destructive"
              onClick={handleDeleteConfirm}
              disabled={deleteMutation.isPending}
            >
              {t('common.delete')}
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>
    </div>
  )
}

interface MetaCardProps {
  meta: any
  onEdit: (id: string) => void
  onDelete: (id: string) => void
}

const MetaCard = ({ meta, onEdit, onDelete }: MetaCardProps) => {
  const { data: progresso } = useGetMetaProgresso(meta.id)

  const percentual = progresso?.percentualProgresso || 0

  return (
    <Card>
      <CardHeader className="pb-3">
        <div className="flex items-start justify-between">
          <div className="flex-1">
            <CardTitle className="text-lg">{meta.nome}</CardTitle>
            <p className="text-xs text-slate-600 mt-1">
              {meta.tipo === 'ECONOMIA' ? 'Economia' : 'Redução'}
            </p>
          </div>
          <div className="flex gap-2">
            <Button
              variant="ghost"
              size="sm"
              onClick={() => onEdit(meta.id)}
              className="h-8 w-8 p-0"
            >
              <Edit2 className="w-4 h-4" />
            </Button>
            <Button
              variant="ghost"
              size="sm"
              onClick={() => onDelete(meta.id)}
              className="h-8 w-8 p-0 text-red-600 hover:text-red-700"
            >
              <Trash2 className="w-4 h-4" />
            </Button>
          </div>
        </div>
      </CardHeader>

      <CardContent>
        <div className="space-y-3">
          <div>
            <div className="flex justify-between items-center mb-2">
              <span className="text-xs text-slate-600">Progresso</span>
              <span className="text-sm font-bold">{percentual.toFixed(1)}%</span>
            </div>
            <div className="w-full bg-slate-200 rounded-full h-2">
              <div
                className={`h-2 rounded-full transition-all ${
                  percentual > 100
                    ? 'bg-red-500'
                    : percentual > 80
                      ? 'bg-yellow-500'
                      : 'bg-green-500'
                }`}
                style={{ width: `${Math.min(percentual, 100)}%` }}
              />
            </div>
          </div>

          {progresso && (
            <div className="grid grid-cols-2 gap-2 text-xs">
              <div>
                <p className="text-slate-600">Atual</p>
                <p className="font-semibold">R$ {(progresso.valorAtual || 0).toFixed(2)}</p>
              </div>
              <div>
                <p className="text-slate-600">Alvo</p>
                <p className="font-semibold">R$ {(progresso.valorAlvo || 0).toFixed(2)}</p>
              </div>
            </div>
          )}
        </div>
      </CardContent>
    </Card>
  )
}
