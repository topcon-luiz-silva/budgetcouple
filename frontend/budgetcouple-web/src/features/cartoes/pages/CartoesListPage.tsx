import { useState } from 'react'
import { Link } from 'react-router-dom'
import { useTranslation } from 'react-i18next'
import { Edit2, Trash2, Plus, Receipt } from 'lucide-react'
import { Button } from '@/components/ui/button'
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '@/components/ui/table'
import { Dialog, DialogContent, DialogFooter, DialogHeader, DialogTitle, DialogBody } from '@/components/ui/dialog'
import { Alert, AlertDescription, AlertTitle } from '@/components/ui/alert'
import { useCartoesList, useDeleteCartao } from '../hooks'
import { AxiosError } from 'axios'

export function CartoesListPage() {
  const { t } = useTranslation()
  const { data: cartoes = [], isPending: isLoading, error } = useCartoesList()
  const { mutate: deleteCartao, isPending: isDeleting } = useDeleteCartao()

  const [deleteConfirmId, setDeleteConfirmId] = useState<string | null>(null)
  const [deleteError, setDeleteError] = useState<string | null>(null)

  const handleDelete = (id: string) => {
    deleteCartao(id, {
      onSuccess: () => {
        setDeleteConfirmId(null)
        setDeleteError(null)
      },
      onError: (err) => {
        if (err instanceof AxiosError) {
          setDeleteError(err.response?.data?.error || 'Erro ao deletar cartão')
        }
      },
    })
  }

  let errorMessage: string | null = null
  if (error instanceof AxiosError) {
    errorMessage = error.response?.data?.error || error.message
  } else if (error) {
    errorMessage = 'Erro ao carregar cartões'
  }

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-3xl font-bold text-slate-900">{t('cartoes.title')}</h1>
          <p className="text-slate-600 mt-1">{t('cartoes.description')}</p>
        </div>
        <Link to="/cartoes/novo">
          <Button>
            <Plus className="h-5 w-5 mr-2" />
            {t('cartoes.new')}
          </Button>
        </Link>
      </div>

      {errorMessage && (
        <Alert variant="destructive">
          <AlertTitle>Erro</AlertTitle>
          <AlertDescription>{errorMessage}</AlertDescription>
        </Alert>
      )}

      {isLoading ? (
        <div className="space-y-3">
          {[1, 2, 3].map((i) => (
            <div key={i} className="h-12 bg-slate-200 rounded-md animate-pulse" />
          ))}
        </div>
      ) : cartoes.length === 0 ? (
        <div className="text-center py-12">
          <p className="text-slate-600 mb-4">{t('cartoes.empty')}</p>
          <Link to="/cartoes/novo">
            <Button>
              <Plus className="h-5 w-5 mr-2" />
              {t('cartoes.new')}
            </Button>
          </Link>
        </div>
      ) : (
        <div className="bg-white rounded-lg border border-slate-200 shadow-sm overflow-hidden">
          <Table>
            <TableHeader>
              <TableRow>
                <TableHead>Nome</TableHead>
                <TableHead>Bandeira</TableHead>
                <TableHead>Últimos Dígitos</TableHead>
                <TableHead>Limite</TableHead>
                <TableHead>Ações</TableHead>
              </TableRow>
            </TableHeader>
            <TableBody>
              {cartoes.map((cartao) => (
                <TableRow key={cartao.id}>
                  <TableCell className="font-medium flex items-center gap-2">
                    <div
                      className="w-3 h-3 rounded-full"
                      style={{ backgroundColor: cartao.corHex }}
                    />
                    {cartao.nome}
                  </TableCell>
                  <TableCell>{cartao.bandeira}</TableCell>
                  <TableCell>{cartao.ultimosDigitos || '-'}</TableCell>
                  <TableCell>R$ {cartao.limite.toFixed(2)}</TableCell>
                  <TableCell>
                    <div className="flex gap-2">
                      <Link to={`/cartoes/${cartao.id}/faturas`}>
                        <Button variant="ghost" size="sm" className="text-purple-600 hover:text-purple-800">
                          <Receipt className="h-4 w-4" />
                        </Button>
                      </Link>
                      <Link to={`/cartoes/${cartao.id}/editar`}>
                        <Button variant="ghost" size="sm" className="text-blue-600 hover:text-blue-800">
                          <Edit2 className="h-4 w-4" />
                        </Button>
                      </Link>
                      <Button
                        variant="ghost"
                        size="sm"
                        className="text-red-600 hover:text-red-800"
                        onClick={() => setDeleteConfirmId(cartao.id)}
                      >
                        <Trash2 className="h-4 w-4" />
                      </Button>
                    </div>
                  </TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
        </div>
      )}

      {/* Delete Confirmation Dialog */}
      <Dialog open={deleteConfirmId !== null} onOpenChange={(open) => !open && setDeleteConfirmId(null)}>
        <DialogContent>
          <DialogHeader>
            <DialogTitle>{t('common.confirmDelete')}</DialogTitle>
          </DialogHeader>
          <DialogBody>
            {deleteError && (
              <Alert variant="destructive" className="mb-4">
                <AlertTitle>Erro</AlertTitle>
                <AlertDescription>{deleteError}</AlertDescription>
              </Alert>
            )}
            <p className="text-slate-600">
              Tem certeza que deseja excluir este cartão? Esta ação não pode ser desfeita.
            </p>
          </DialogBody>
          <DialogFooter>
            <Button
              variant="outline"
              onClick={() => setDeleteConfirmId(null)}
              disabled={isDeleting}
            >
              {t('common.cancel')}
            </Button>
            <Button
              variant="destructive"
              onClick={() => deleteConfirmId && handleDelete(deleteConfirmId)}
              disabled={isDeleting}
            >
              {isDeleting ? 'Excluindo...' : 'Excluir'}
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>
    </div>
  )
}
