import { useState } from 'react'
import { useTranslation } from 'react-i18next'
import { Button } from '@/components/ui/button'
import { Dialog, DialogContent, DialogFooter, DialogHeader, DialogTitle, DialogBody } from '@/components/ui/dialog'
import { Alert, AlertDescription, AlertTitle } from '@/components/ui/alert'
import { AxiosError } from 'axios'

interface DeleteLancamentoDialogProps {
  open: boolean
  onOpenChange: (open: boolean) => void
  onDelete: (escopo: 'one' | 'fromHere' | 'all') => void
  isPending: boolean
  error: Error | null
  isFromRecurrence?: boolean
}

export function DeleteLancamentoDialog({
  open,
  onOpenChange,
  onDelete,
  isPending,
  error,
  isFromRecurrence = false,
}: DeleteLancamentoDialogProps) {
  const { t } = useTranslation()
  const [selectedScope, setSelectedScope] = useState<'one' | 'fromHere' | 'all'>('one')

  let errorMessage: string | null = null
  if (error instanceof AxiosError) {
    errorMessage = error.response?.data?.error || error.message
  } else if (error) {
    errorMessage = 'Erro ao deletar lançamento'
  }

  const handleDelete = () => {
    onDelete(selectedScope)
  }

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent>
        <DialogHeader>
          <DialogTitle>{t('common.confirmDelete')}</DialogTitle>
        </DialogHeader>
        <DialogBody>
          {errorMessage && (
            <Alert variant="destructive" className="mb-4">
              <AlertTitle>Erro</AlertTitle>
              <AlertDescription>{errorMessage}</AlertDescription>
            </Alert>
          )}
          <p className="text-slate-600 mb-4">
            Tem certeza que deseja excluir este lançamento? Esta ação não pode ser desfeita.
          </p>

          {isFromRecurrence && (
            <div className="space-y-3">
              <label className="flex items-center gap-3 p-3 border border-slate-200 rounded-md cursor-pointer hover:bg-slate-50">
                <input
                  type="radio"
                  name="scope"
                  value="one"
                  checked={selectedScope === 'one'}
                  onChange={(e) => setSelectedScope(e.target.value as 'one' | 'fromHere' | 'all')}
                  className="h-4 w-4"
                />
                <div>
                  <p className="font-medium text-slate-900">{t('lancamentos.delete.scope.one')}</p>
                  <p className="text-sm text-slate-600">Apenas este lançamento</p>
                </div>
              </label>

              <label className="flex items-center gap-3 p-3 border border-slate-200 rounded-md cursor-pointer hover:bg-slate-50">
                <input
                  type="radio"
                  name="scope"
                  value="fromHere"
                  checked={selectedScope === 'fromHere'}
                  onChange={(e) => setSelectedScope(e.target.value as 'one' | 'fromHere' | 'all')}
                  className="h-4 w-4"
                />
                <div>
                  <p className="font-medium text-slate-900">{t('lancamentos.delete.scope.fromHere')}</p>
                  <p className="text-sm text-slate-600">Este e os próximos lançamentos</p>
                </div>
              </label>

              <label className="flex items-center gap-3 p-3 border border-slate-200 rounded-md cursor-pointer hover:bg-slate-50">
                <input
                  type="radio"
                  name="scope"
                  value="all"
                  checked={selectedScope === 'all'}
                  onChange={(e) => setSelectedScope(e.target.value as 'one' | 'fromHere' | 'all')}
                  className="h-4 w-4"
                />
                <div>
                  <p className="font-medium text-slate-900">{t('lancamentos.delete.scope.all')}</p>
                  <p className="text-sm text-slate-600">Todos os lançamentos desta recorrência</p>
                </div>
              </label>
            </div>
          )}
        </DialogBody>
        <DialogFooter>
          <Button variant="outline" onClick={() => onOpenChange(false)} disabled={isPending}>
            {t('common.cancel')}
          </Button>
          <Button variant="destructive" onClick={handleDelete} disabled={isPending}>
            {isPending ? 'Excluindo...' : 'Excluir'}
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  )
}
