import { useState } from 'react'
import { useParams, Link } from 'react-router-dom'
import { useTranslation } from 'react-i18next'
import { format, parse } from 'date-fns'
import { ptBR } from 'date-fns/locale'
import { ArrowLeft } from 'lucide-react'
import { Button } from '@/components/ui/button'
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '@/components/ui/table'
import { Alert, AlertDescription, AlertTitle } from '@/components/ui/alert'
import { useFatura, usePagarFatura } from '../hooks'
import { PagarFaturaDialog } from '../components/PagarFaturaDialog'
import { AxiosError } from 'axios'
import type { PayFaturaData } from '../types'

const currencyFormatter = new Intl.NumberFormat('pt-BR', {
  style: 'currency',
  currency: 'BRL',
})

function formatarCompetencia(competencia: string): string {
  const data = parse(competencia + '-01', 'yyyy-MM-dd', new Date())
  const formatted = format(data, 'MMMM/yyyy', { locale: ptBR })
  return formatted.charAt(0).toUpperCase() + formatted.slice(1)
}

export function FaturaDetalhePage() {
  const { t } = useTranslation()
  const { cartaoId, competencia } = useParams<{ cartaoId: string; competencia: string }>()
  const [pagarDialogOpen, setPagarDialogOpen] = useState(false)
  const [pagarError, setPagarError] = useState<string | null>(null)

  if (!cartaoId || !competencia) {
    return (
      <div className="text-center py-12">
        <p className="text-slate-600">Fatura não encontrada</p>
      </div>
    )
  }

  const { data: fatura, isPending: isLoading, error } = useFatura(cartaoId, competencia)
  const { mutate: pagarFatura, isPending: isPagarPending, error: pagarMutationError } = usePagarFatura(cartaoId)

  let errorMessage: string | null = null
  if (error instanceof AxiosError) {
    errorMessage = error.response?.data?.error || error.message
  } else if (error) {
    errorMessage = 'Erro ao carregar fatura'
  }

  const handlePagar = (data: PayFaturaData) => {
    pagarFatura(
      { competencia, body: data },
      {
        onSuccess: () => {
          setPagarDialogOpen(false)
          setPagarError(null)
        },
        onError: (err) => {
          if (err instanceof AxiosError) {
            setPagarError(err.response?.data?.error || 'Erro ao pagar fatura')
          }
        },
      }
    )
  }

  if (isLoading) {
    return (
      <div className="space-y-3">
        {[1, 2, 3].map((i) => (
          <div key={i} className="h-12 bg-slate-200 rounded-md animate-pulse" />
        ))}
      </div>
    )
  }

  if (!fatura) {
    return (
      <div className="text-center py-12">
        <p className="text-slate-600">Fatura não encontrada</p>
      </div>
    )
  }

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="flex items-center gap-4">
        <Link to={`/cartoes/${cartaoId}/faturas`}>
          <Button variant="ghost" size="sm">
            <ArrowLeft className="h-4 w-4" />
          </Button>
        </Link>
        <div className="flex-1">
          <h1 className="text-3xl font-bold text-slate-900">{formatarCompetencia(fatura.competencia)}</h1>
          <p className="text-slate-600 mt-1">{fatura.cartaoNome}</p>
        </div>
      </div>

      {/* Error Alert */}
      {errorMessage && (
        <Alert variant="destructive">
          <AlertTitle>Erro</AlertTitle>
          <AlertDescription>{errorMessage}</AlertDescription>
        </Alert>
      )}

      {/* Fatura Summary */}
      <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
        {/* Left Column */}
        <div className="bg-white rounded-lg border border-slate-200 shadow-sm p-6 space-y-4">
          <div>
            <p className="text-sm text-slate-600">{t('faturas.closingDate')}</p>
            <p className="text-lg font-semibold text-slate-900">
              {format(new Date(fatura.dataFechamento), 'dd/MM/yyyy')}
            </p>
          </div>
          <div>
            <p className="text-sm text-slate-600">{t('faturas.dueDate')}</p>
            <p className="text-lg font-semibold text-slate-900">
              {format(new Date(fatura.dataVencimento), 'dd/MM/yyyy')}
            </p>
          </div>
        </div>

        {/* Right Column */}
        <div className="bg-white rounded-lg border border-slate-200 shadow-sm p-6 space-y-4">
          <div>
            <p className="text-sm text-slate-600">{t('faturas.total')}</p>
            <p className="text-3xl font-bold text-slate-900">{currencyFormatter.format(fatura.valorTotal)}</p>
          </div>
          <div>
            <p className="text-sm text-slate-600">Status</p>
            <p className="text-lg font-semibold text-slate-900">
              {fatura.paga ? (
                <>
                  <span className="text-green-600">Paga</span>
                  {fatura.dataPagamento && (
                    <p className="text-sm text-slate-600 mt-1">
                      em {format(new Date(fatura.dataPagamento), 'dd/MM/yyyy')}
                    </p>
                  )}
                </>
              ) : (
                <>
                  <span className="text-yellow-600">Aberta</span>
                </>
              )}
            </p>
          </div>
        </div>
      </div>

      {/* Pay Button */}
      {!fatura.paga && (
        <div className="flex justify-end">
          <Button onClick={() => setPagarDialogOpen(true)}>{t('faturas.pay')}</Button>
        </div>
      )}

      {/* Lançamentos */}
      <div className="bg-white rounded-lg border border-slate-200 shadow-sm">
        <div className="p-6 border-b border-slate-200">
          <h2 className="text-xl font-semibold text-slate-900">Lançamentos</h2>
        </div>

        {fatura.lancamentos.length === 0 ? (
          <div className="text-center py-12">
            <p className="text-slate-600">{t('faturas.noLancamentos')}</p>
          </div>
        ) : (
          <div className="overflow-hidden">
            <Table>
              <TableHeader>
                <TableRow>
                  <TableHead>Data</TableHead>
                  <TableHead>Descrição</TableHead>
                  <TableHead>Categoria</TableHead>
                  <TableHead className="text-right">Valor</TableHead>
                </TableRow>
              </TableHeader>
              <TableBody>
                {fatura.lancamentos.map((lancamento) => (
                  <TableRow key={lancamento.id}>
                    <TableCell className="font-medium">
                      {format(new Date(lancamento.dataCompetencia), 'dd/MM/yyyy')}
                    </TableCell>
                    <TableCell>{lancamento.descricao}</TableCell>
                    <TableCell>{lancamento.categoriaNome}</TableCell>
                    <TableCell className="text-right font-medium">
                      {currencyFormatter.format(lancamento.valor)}
                    </TableCell>
                  </TableRow>
                ))}
              </TableBody>
            </Table>
          </div>
        )}
      </div>

      {/* Dialogs */}
      <PagarFaturaDialog
        open={pagarDialogOpen}
        onOpenChange={setPagarDialogOpen}
        onPagar={handlePagar}
        isPending={isPagarPending}
        error={pagarError ? new Error(pagarError) : pagarMutationError}
      />
    </div>
  )
}
