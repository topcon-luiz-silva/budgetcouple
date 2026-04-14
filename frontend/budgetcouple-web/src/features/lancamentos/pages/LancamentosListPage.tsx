import { useState, useMemo } from 'react'
import { Link } from 'react-router-dom'
import { useTranslation } from 'react-i18next'
import { Edit2, Trash2, Plus, DollarSign, FileDown } from 'lucide-react'
import { format, startOfMonth, endOfMonth } from 'date-fns'
import { Button } from '@/components/ui/button'
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '@/components/ui/table'
import { Badge } from '@/components/ui/badge'
import { Alert, AlertDescription, AlertTitle } from '@/components/ui/alert'
import { Select } from '@/components/ui/select'
import { Label } from '@/components/ui/label'
import {
  useLancamentosList,
  useDeleteLancamento,
  usePagarLancamento,
} from '../hooks'
import { useExportLancamentosExcel, useExportLancamentosPdf } from '@/features/dashboard/hooks'
import { useContasList } from '@/features/contas/hooks'
import { useCartoesList } from '@/features/cartoes/hooks'
import { useCategoriasList } from '@/features/categorias/hooks'
import { DeleteLancamentoDialog } from '../components/DeleteLancamentoDialog'
import { PagarLancamentoDialog } from '../components/PagarLancamentoDialog'
import type { StatusPagamento } from '../types'
import { AxiosError } from 'axios'

const statusLabels: Record<StatusPagamento, string> = {
  PREVISTO: 'Previsto',
  REALIZADO: 'Realizado',
  ATRASADO: 'Atrasado',
}

const statusColors: Record<StatusPagamento, string> = {
  PREVISTO: 'bg-yellow-100 text-yellow-800',
  REALIZADO: 'bg-green-100 text-green-800',
  ATRASADO: 'bg-red-100 text-red-800',
}

export function LancamentosListPage() {
  const { t } = useTranslation()
  const [currentPage, setCurrentPage] = useState(0)
  const [deleteConfirmId, setDeleteConfirmId] = useState<string | null>(null)
  const [deleteConfirmError, setDeleteConfirmError] = useState<string | null>(null)
  const [pagarDialogId, setPagarDialogId] = useState<string | null>(null)
  const [pagarDialogError, setPagarDialogError] = useState<string | null>(null)

  // Filters
  const [filters, setFilters] = useState({
    dataInicio: format(startOfMonth(new Date()), 'yyyy-MM-dd'),
    dataFim: format(endOfMonth(new Date()), 'yyyy-MM-dd'),
    contaId: '',
    cartaoId: '',
    categoriaId: '',
    status: '',
  })

  const pageSize = 10

  const { data: lancamentos = { items: [], total: 0, skip: 0, take: 0 }, isPending: isLoading, error } = useLancamentosList({
    dataInicio: filters.dataInicio,
    dataFim: filters.dataFim,
    contaId: filters.contaId || undefined,
    cartaoId: filters.cartaoId || undefined,
    categoriaId: filters.categoriaId || undefined,
    status: filters.status || undefined,
    skip: currentPage * pageSize,
    take: pageSize,
  })

  const { data: contas = [] } = useContasList()
  const { data: cartoes = [] } = useCartoesList()
  const { data: categorias = [] } = useCategoriasList()

  const { mutate: deleteLancamento, isPending: isDeleting, error: deleteError } = useDeleteLancamento()
  const { mutate: pagarLancamento, isPending: isPagarPending, error: pagarError } = usePagarLancamento()
  const { mutate: exportExcel, isPending: isExportingExcel } = useExportLancamentosExcel()
  const { mutate: exportPdf, isPending: isExportingPdf } = useExportLancamentosPdf()

  const totalPages = Math.ceil(lancamentos.total / pageSize)

  const contaMap = useMemo(() => new Map(contas.map((c) => [c.id, c.nome])), [contas])
  const cartaoMap = useMemo(() => new Map(cartoes.map((c) => [c.id, c.nome])), [cartoes])
  const categoriaMap = useMemo(() => new Map(categorias.map((c) => [c.id, c.nome])), [categorias])

  let errorMessage: string | null = null
  if (error instanceof AxiosError) {
    errorMessage = error.response?.data?.error || error.message
  } else if (error) {
    errorMessage = 'Erro ao carregar lançamentos'
  }

  const handleDelete = (id: string, escopo: 'one' | 'fromHere' | 'all' = 'one') => {
    deleteLancamento(
      { id, escopo },
      {
        onSuccess: () => {
          setDeleteConfirmId(null)
          setDeleteConfirmError(null)
        },
        onError: (err) => {
          if (err instanceof AxiosError) {
            setDeleteConfirmError(err.response?.data?.error || 'Erro ao deletar')
          }
        },
      }
    )
  }

  const handlePagar = (dataPagamento: string, contaDebitoId?: string) => {
    if (!pagarDialogId) return
    pagarLancamento(
      { id: pagarDialogId, dataPagamento, contaDebitoId },
      {
        onSuccess: () => {
          setPagarDialogId(null)
          setPagarDialogError(null)
        },
        onError: (err) => {
          if (err instanceof AxiosError) {
            setPagarDialogError(err.response?.data?.error || 'Erro ao pagar')
          }
        },
      }
    )
  }

  const handleFilterChange = (key: string, value: string) => {
    setFilters((prev) => ({ ...prev, [key]: value }))
    setCurrentPage(0)
  }

  const handleExportExcel = () => {
    exportExcel(
      {
        dataInicio: filters.dataInicio || undefined,
        dataFim: filters.dataFim || undefined,
        contaId: filters.contaId || undefined,
        cartaoId: filters.cartaoId || undefined,
        categoriaId: filters.categoriaId || undefined,
        status: filters.status || undefined,
      },
      {
        onSuccess: (blob) => {
          const url = URL.createObjectURL(blob)
          const a = document.createElement('a')
          a.href = url
          a.download = `lancamentos-${format(new Date(), 'yyyy-MM-dd')}.xlsx`
          a.click()
          URL.revokeObjectURL(url)
        },
      }
    )
  }

  const handleExportPdf = () => {
    exportPdf(
      {
        dataInicio: filters.dataInicio || undefined,
        dataFim: filters.dataFim || undefined,
        contaId: filters.contaId || undefined,
        cartaoId: filters.cartaoId || undefined,
        categoriaId: filters.categoriaId || undefined,
        status: filters.status || undefined,
      },
      {
        onSuccess: (blob) => {
          const url = URL.createObjectURL(blob)
          const a = document.createElement('a')
          a.href = url
          a.download = `lancamentos-${format(new Date(), 'yyyy-MM-dd')}.pdf`
          a.click()
          URL.revokeObjectURL(url)
        },
      }
    )
  }

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-3xl font-bold text-slate-900">{t('lancamentos.title')}</h1>
          <p className="text-slate-600 mt-1">Manage your transactions</p>
        </div>
        <div className="flex gap-2 flex-wrap">
          <Button
            variant="outline"
            size="sm"
            onClick={handleExportExcel}
            disabled={isExportingExcel}
            className="gap-2"
          >
            <FileDown className="h-4 w-4" />
            {t('dashboard.exportExcel')}
          </Button>
          <Button
            variant="outline"
            size="sm"
            onClick={handleExportPdf}
            disabled={isExportingPdf}
            className="gap-2"
          >
            <FileDown className="h-4 w-4" />
            {t('dashboard.exportPdf')}
          </Button>
          <Link to="/lancamentos/novo/simples">
            <Button variant="outline" size="sm">
              {t('lancamentos.simples')}
            </Button>
          </Link>
          <Link to="/lancamentos/novo/parcelado">
            <Button variant="outline" size="sm">
              {t('lancamentos.parcelado')}
            </Button>
          </Link>
          <Link to="/lancamentos/novo/recorrente">
            <Button>
              <Plus className="h-5 w-5 mr-2" />
              {t('lancamentos.recorrente')}
            </Button>
          </Link>
        </div>
      </div>

      {/* Filters */}
      <div className="bg-white rounded-lg border border-slate-200 p-6 shadow-sm">
        <div className="grid grid-cols-2 md:grid-cols-6 gap-4">
          <div className="space-y-2">
            <Label htmlFor="dataInicio">Data Inicial</Label>
            <input
              id="dataInicio"
              type="date"
              value={filters.dataInicio}
              onChange={(e) => handleFilterChange('dataInicio', e.target.value)}
              className="flex w-full rounded-md border border-slate-300 bg-white px-3 py-2 text-base placeholder:text-slate-500 focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-slate-400"
            />
          </div>

          <div className="space-y-2">
            <Label htmlFor="dataFim">Data Final</Label>
            <input
              id="dataFim"
              type="date"
              value={filters.dataFim}
              onChange={(e) => handleFilterChange('dataFim', e.target.value)}
              className="flex w-full rounded-md border border-slate-300 bg-white px-3 py-2 text-base placeholder:text-slate-500 focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-slate-400"
            />
          </div>

          <div className="space-y-2">
            <Label htmlFor="contaId">Conta</Label>
            <Select
              id="contaId"
              value={filters.contaId}
              onChange={(e) => handleFilterChange('contaId', e.target.value)}
            >
              <option value="">Todas</option>
              {contas.map((conta) => (
                <option key={conta.id} value={conta.id}>
                  {conta.nome}
                </option>
              ))}
            </Select>
          </div>

          <div className="space-y-2">
            <Label htmlFor="cartaoId">Cartão</Label>
            <Select
              id="cartaoId"
              value={filters.cartaoId}
              onChange={(e) => handleFilterChange('cartaoId', e.target.value)}
            >
              <option value="">Todos</option>
              {cartoes.map((cartao) => (
                <option key={cartao.id} value={cartao.id}>
                  {cartao.nome}
                </option>
              ))}
            </Select>
          </div>

          <div className="space-y-2">
            <Label htmlFor="categoriaId">Categoria</Label>
            <Select
              id="categoriaId"
              value={filters.categoriaId}
              onChange={(e) => handleFilterChange('categoriaId', e.target.value)}
            >
              <option value="">Todas</option>
              {categorias.map((categoria) => (
                <option key={categoria.id} value={categoria.id}>
                  {categoria.nome}
                </option>
              ))}
            </Select>
          </div>

          <div className="space-y-2">
            <Label htmlFor="status">Status</Label>
            <Select value={filters.status} onChange={(e) => handleFilterChange('status', e.target.value)}>
              <option value="">Todos</option>
              <option value="PREVISTO">Previsto</option>
              <option value="REALIZADO">Realizado</option>
              <option value="ATRASADO">Atrasado</option>
            </Select>
          </div>
        </div>
      </div>

      {/* Error Alert */}
      {errorMessage && (
        <Alert variant="destructive">
          <AlertTitle>Erro</AlertTitle>
          <AlertDescription>{errorMessage}</AlertDescription>
        </Alert>
      )}

      {/* Table */}
      {isLoading ? (
        <div className="space-y-3">
          {[1, 2, 3].map((i) => (
            <div key={i} className="h-12 bg-slate-200 rounded-md animate-pulse" />
          ))}
        </div>
      ) : lancamentos.items.length === 0 ? (
        <div className="text-center py-12 bg-white rounded-lg border border-slate-200">
          <DollarSign className="h-12 w-12 text-slate-300 mx-auto mb-4" />
          <p className="text-slate-600 mb-4">Nenhum lançamento encontrado</p>
          <Link to="/lancamentos/novo/simples">
            <Button>
              <Plus className="h-5 w-5 mr-2" />
              Novo Lançamento
            </Button>
          </Link>
        </div>
      ) : (
        <div className="bg-white rounded-lg border border-slate-200 shadow-sm overflow-hidden">
          <Table>
            <TableHeader>
              <TableRow>
                <TableHead>Data</TableHead>
                <TableHead>Descrição</TableHead>
                <TableHead>Categoria</TableHead>
                <TableHead>Conta/Cartão</TableHead>
                <TableHead className="text-right">Valor</TableHead>
                <TableHead>Status</TableHead>
                <TableHead>Ações</TableHead>
              </TableRow>
            </TableHeader>
            <TableBody>
              {lancamentos.items.map((lancamento) => (
                <TableRow key={lancamento.id}>
                  <TableCell className="font-medium">
                    {format(new Date(lancamento.dataCompetencia), 'dd/MM/yyyy')}
                  </TableCell>
                  <TableCell>{lancamento.descricao}</TableCell>
                  <TableCell>{categoriaMap.get(lancamento.categoriaId) || '-'}</TableCell>
                  <TableCell>
                    <div className="flex items-center gap-2">
                      {lancamento.contaId && contaMap.get(lancamento.contaId)}
                      {lancamento.cartaoId && (
                        <>
                          <Badge className="text-xs">
                            Cartão
                          </Badge>
                          {cartaoMap.get(lancamento.cartaoId)}
                        </>
                      )}
                    </div>
                  </TableCell>
                  <TableCell className="text-right font-medium">
                    <span
                      className={
                        lancamento.naturezaLancamento === 'RECEITA' ? 'text-green-600' : 'text-red-600'
                      }
                    >
                      {lancamento.naturezaLancamento === 'RECEITA' ? '+' : '-'} R${' '}
                      {Math.abs(lancamento.valor).toFixed(2)}
                    </span>
                  </TableCell>
                  <TableCell>
                    <Badge className={statusColors[lancamento.statusPagamento]}>
                      {statusLabels[lancamento.statusPagamento]}
                    </Badge>
                  </TableCell>
                  <TableCell>
                    <div className="flex gap-2">
                      <Link to={`/lancamentos/${lancamento.id}/editar`}>
                        <Button
                          variant="ghost"
                          size="sm"
                          className="text-blue-600 hover:text-blue-800"
                        >
                          <Edit2 className="h-4 w-4" />
                        </Button>
                      </Link>
                      {lancamento.statusPagamento === 'PREVISTO' && (
                        <Button
                          variant="ghost"
                          size="sm"
                          className="text-green-600 hover:text-green-800"
                          onClick={() => setPagarDialogId(lancamento.id)}
                        >
                          <DollarSign className="h-4 w-4" />
                        </Button>
                      )}
                      <Button
                        variant="ghost"
                        size="sm"
                        className="text-red-600 hover:text-red-800"
                        onClick={() => setDeleteConfirmId(lancamento.id)}
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

      {/* Pagination */}
      {totalPages > 1 && (
        <div className="flex items-center justify-between">
          <p className="text-sm text-slate-600">
            Mostrando {currentPage * pageSize + 1} a{' '}
            {Math.min((currentPage + 1) * pageSize, lancamentos.total)} de {lancamentos.total}
          </p>
          <div className="flex gap-2">
            <Button
              disabled={currentPage === 0}
              onClick={() => setCurrentPage(currentPage - 1)}
            >
              Anterior
            </Button>
            {Array.from({ length: totalPages }, (_, i) => (
              <Button
                key={i}
                onClick={() => setCurrentPage(i)}
                size="sm"
                disabled={currentPage === i}
              >
                {i + 1}
              </Button>
            ))}
            <Button
              disabled={currentPage === totalPages - 1}
              onClick={() => setCurrentPage(currentPage + 1)}
            >
              Próximo
            </Button>
          </div>
        </div>
      )}

      {/* Dialogs */}
      <DeleteLancamentoDialog
        open={deleteConfirmId !== null}
        onOpenChange={(open) => !open && setDeleteConfirmId(null)}
        onDelete={(escopo) => {
          if (deleteConfirmId) handleDelete(deleteConfirmId, escopo)
        }}
        isPending={isDeleting}
        error={deleteConfirmError ? new Error(deleteConfirmError) : deleteError}
      />

      <PagarLancamentoDialog
        open={pagarDialogId !== null}
        onOpenChange={(open) => {
          if (!open) {
            setPagarDialogId(null)
            setPagarDialogError(null)
          }
        }}
        onPagar={handlePagar}
        isPending={isPagarPending}
        error={pagarDialogError ? new Error(pagarDialogError) : pagarError}
      />
    </div>
  )
}
