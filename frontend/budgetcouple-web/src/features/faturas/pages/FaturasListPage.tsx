import { useParams, Link } from 'react-router-dom'
import { useTranslation } from 'react-i18next'
import { format, parse } from 'date-fns'
import { ptBR } from 'date-fns/locale'
import { ArrowLeft, CreditCard } from 'lucide-react'
import { Button } from '@/components/ui/button'
import { Badge } from '@/components/ui/badge'
import { Alert, AlertDescription, AlertTitle } from '@/components/ui/alert'
import { useCartaoById } from '@/features/cartoes/hooks'
import { useFaturasList } from '../hooks'
import { AxiosError } from 'axios'

const currencyFormatter = new Intl.NumberFormat('pt-BR', {
  style: 'currency',
  currency: 'BRL',
})

function formatarCompetencia(competencia: string): string {
  const data = parse(competencia + '-01', 'yyyy-MM-dd', new Date())
  const formatted = format(data, 'MMMM/yyyy', { locale: ptBR })
  return formatted.charAt(0).toUpperCase() + formatted.slice(1)
}

export function FaturasListPage() {
  const { t } = useTranslation()
  const { cartaoId } = useParams<{ cartaoId: string }>()

  if (!cartaoId) {
    return (
      <div className="text-center py-12">
        <p className="text-slate-600">{t('faturas.noLancamentos')}</p>
      </div>
    )
  }

  const { data: cartao } = useCartaoById(cartaoId)
  const { data: faturas = [], isPending: isLoading, error } = useFaturasList(cartaoId)

  let errorMessage: string | null = null
  if (error instanceof AxiosError) {
    errorMessage = error.response?.data?.error || error.message
  } else if (error) {
    errorMessage = 'Erro ao carregar faturas'
  }

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="flex items-center gap-4">
        <Link to="/cartoes">
          <Button variant="ghost" size="sm">
            <ArrowLeft className="h-4 w-4" />
          </Button>
        </Link>
        <div>
          <h1 className="text-3xl font-bold text-slate-900">
            {t('faturas.title')} - {cartao?.nome}
          </h1>
          <p className="text-slate-600 mt-1">Últimas 12 competências</p>
        </div>
      </div>

      {/* Error Alert */}
      {errorMessage && (
        <Alert variant="destructive">
          <AlertTitle>Erro</AlertTitle>
          <AlertDescription>{errorMessage}</AlertDescription>
        </Alert>
      )}

      {/* Loading State */}
      {isLoading ? (
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
          {[1, 2, 3, 4, 5, 6].map((i) => (
            <div key={i} className="h-32 bg-slate-200 rounded-md animate-pulse" />
          ))}
        </div>
      ) : faturas.length === 0 ? (
        <div className="text-center py-12 bg-white rounded-lg border border-slate-200">
          <CreditCard className="h-12 w-12 text-slate-300 mx-auto mb-4" />
          <p className="text-slate-600 mb-4">{t('faturas.noLancamentos')}</p>
        </div>
      ) : (
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
          {faturas.map((fatura) => (
            <Link
              key={fatura.competencia}
              to={`/cartoes/${cartaoId}/faturas/${fatura.competencia}`}
            >
              <div className="bg-white rounded-lg border border-slate-200 shadow-sm hover:shadow-md transition-shadow p-6 cursor-pointer h-full">
                <div className="space-y-4">
                  {/* Header */}
                  <div className="flex items-start justify-between">
                    <div>
                      <h3 className="text-lg font-semibold text-slate-900">
                        {formatarCompetencia(fatura.competencia)}
                      </h3>
                      <p className="text-sm text-slate-600 mt-1">
                        Vencimento: {format(new Date(fatura.dataVencimento), 'dd/MM/yyyy')}
                      </p>
                    </div>
                    <Badge className={fatura.paga ? 'bg-green-100 text-green-800' : 'bg-yellow-100 text-yellow-800'}>
                      {fatura.paga ? t('faturas.paid') : t('faturas.open')}
                    </Badge>
                  </div>

                  {/* Valor */}
                  <div className="border-t border-slate-200 pt-4">
                    <p className="text-sm text-slate-600">{t('faturas.total')}</p>
                    <p className="text-2xl font-bold text-slate-900">
                      {currencyFormatter.format(fatura.valorTotal)}
                    </p>
                  </div>
                </div>
              </div>
            </Link>
          ))}
        </div>
      )}
    </div>
  )
}
