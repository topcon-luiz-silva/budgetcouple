import { useState, useMemo } from 'react'
import { useTranslation } from 'react-i18next'
import { format, addMonths, subMonths } from 'date-fns'
import { ptBR } from 'date-fns/locale'
import { useListAlertas } from '@/features/metas/hooks'
import { AlertasOrcamentoSection } from '@/features/metas/components/AlertasOrcamentoSection'
import {
  ChevronLeft,
  ChevronRight,
  TrendingUp,
  TrendingDown,
  Wallet,
  PiggyBank,
  FileDown,
  AlertCircle,
} from 'lucide-react'
import {
  LineChart,
  Line,
  PieChart,
  Pie,
  Cell,
  XAxis,
  YAxis,
  CartesianGrid,
  Tooltip,
  Legend,
  ResponsiveContainer,
} from 'recharts'
import { Button } from '@/components/ui/button'
import { Alert, AlertDescription, AlertTitle } from '@/components/ui/alert'
import { useDashboard, useExportDashboardPdf } from '../hooks'
import { formatCurrency, formatPercentage } from '@/lib/format'

export function DashboardPage() {
  const { t } = useTranslation()
  const [currentDate, setCurrentDate] = useState(new Date())
  const mes = format(currentDate, 'yyyy-MM')

  const { data: dashboard, isPending, error } = useDashboard(mes)
  const { mutate: exportPdf, isPending: isExporting } = useExportDashboardPdf()
  const { data: alertas = [] } = useListAlertas()

  const handlePreviousMonth = () => {
    setCurrentDate((prev) => subMonths(prev, 1))
  }

  const handleNextMonth = () => {
    setCurrentDate((prev) => addMonths(prev, 1))
  }

  const handleExportPdf = () => {
    exportPdf(mes, {
      onSuccess: (blob) => {
        const url = URL.createObjectURL(blob)
        const a = document.createElement('a')
        a.href = url
        a.download = `dashboard-${mes}.pdf`
        a.click()
        URL.revokeObjectURL(url)
      },
    })
  }

  const monthLabel = format(currentDate, "MMMM'/'yyyy", { locale: ptBR })

  if (error) {
    return (
      <Alert variant="destructive">
        <AlertCircle className="h-4 w-4" />
        <AlertTitle>Erro</AlertTitle>
        <AlertDescription>Não foi possível carregar o dashboard</AlertDescription>
      </Alert>
    )
  }

  if (isPending || !dashboard) {
    return (
      <div className="space-y-6">
        {[1, 2, 3, 4, 5].map((i) => (
          <div key={i} className="h-32 bg-slate-200 rounded-lg animate-pulse" />
        ))}
      </div>
    )
  }

  // Cores para o gráfico de pizza
  const COLORS = [
    '#3b82f6',
    '#ef4444',
    '#10b981',
    '#f59e0b',
    '#8b5cf6',
    '#ec4899',
    '#14b8a6',
    '#f97316',
  ]

  const chartData = useMemo(
    () =>
      dashboard.porCategoria
        .sort((a, b) => b.total - a.total)
        .slice(0, 8)
        .map((item) => ({
          name: item.categoriaNome,
          value: item.total,
          color: item.corHex || '#94a3b8',
        })),
    [dashboard.porCategoria]
  )

  return (
    <div className="space-y-6">
      {/* Header com seletor de mês */}
      <div className="flex items-center justify-between">
        <h1 className="text-3xl font-bold text-slate-900">{t('dashboard.title')}</h1>
        <div className="flex items-center gap-2 bg-white rounded-lg border border-slate-200 p-2">
          <Button
            variant="ghost"
            size="sm"
            onClick={handlePreviousMonth}
            className="text-slate-600 hover:text-slate-900"
          >
            <ChevronLeft className="h-5 w-5" />
          </Button>
          <span className="min-w-40 text-center font-medium capitalize text-slate-900">
            {monthLabel}
          </span>
          <Button
            variant="ghost"
            size="sm"
            onClick={handleNextMonth}
            className="text-slate-600 hover:text-slate-900"
          >
            <ChevronRight className="h-5 w-5" />
          </Button>
        </div>
        <Button
          onClick={handleExportPdf}
          disabled={isExporting}
          className="gap-2"
        >
          <FileDown className="h-4 w-4" />
          {t('dashboard.exportPdf')}
        </Button>
      </div>

      {/* Cards de Resumo */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
        {/* Receitas */}
        <div className="bg-white rounded-lg border border-slate-200 p-6 shadow-sm">
          <div className="flex items-start justify-between mb-4">
            <div>
              <p className="text-sm font-medium text-slate-600">
                {t('dashboard.resumo.receitas')}
              </p>
              <p className="text-2xl font-bold text-green-600 mt-2">
                {formatCurrency(dashboard.resumo.receitas)}
              </p>
            </div>
            <TrendingUp className="h-6 w-6 text-green-600" />
          </div>
          <p className="text-xs text-slate-500">
            {t('dashboard.previsto')}: {formatCurrency(dashboard.resumo.receitasPrevisto)}
          </p>
        </div>

        {/* Despesas */}
        <div className="bg-white rounded-lg border border-slate-200 p-6 shadow-sm">
          <div className="flex items-start justify-between mb-4">
            <div>
              <p className="text-sm font-medium text-slate-600">
                {t('dashboard.resumo.despesas')}
              </p>
              <p className="text-2xl font-bold text-red-600 mt-2">
                {formatCurrency(dashboard.resumo.despesas)}
              </p>
            </div>
            <TrendingDown className="h-6 w-6 text-red-600" />
          </div>
          <p className="text-xs text-slate-500">
            {t('dashboard.previsto')}: {formatCurrency(dashboard.resumo.despesasPrevisto)}
          </p>
        </div>

        {/* Saldo */}
        <div className="bg-white rounded-lg border border-slate-200 p-6 shadow-sm">
          <div className="flex items-start justify-between mb-4">
            <div>
              <p className="text-sm font-medium text-slate-600">
                {t('dashboard.resumo.saldo')}
              </p>
              <p className="text-2xl font-bold text-blue-600 mt-2">
                {formatCurrency(dashboard.resumo.saldo)}
              </p>
            </div>
            <Wallet className="h-6 w-6 text-blue-600" />
          </div>
          <p className="text-xs text-slate-500">
            {t('dashboard.previsto')}: {formatCurrency(dashboard.resumo.saldoPrevisto)}
          </p>
        </div>

        {/* Saldo Consolidado */}
        <div className="bg-white rounded-lg border border-slate-200 p-6 shadow-sm">
          <div className="flex items-start justify-between mb-4">
            <div>
              <p className="text-sm font-medium text-slate-600">
                {t('dashboard.resumo.saldoConsolidado')}
              </p>
              <p className="text-2xl font-bold text-purple-600 mt-2">
                {formatCurrency(dashboard.resumo.saldoConsolidado)}
              </p>
            </div>
            <PiggyBank className="h-6 w-6 text-purple-600" />
          </div>
          <p className="text-xs text-slate-500">&nbsp;</p>
        </div>
      </div>

      {/* Alertas de Orçamento */}
      {alertas && alertas.length > 0 && (
        <AlertasOrcamentoSection alertas={alertas} />
      )}

      {/* Gráfico de Evolução Mensal */}
      <div className="bg-white rounded-lg border border-slate-200 p-6 shadow-sm">
        <h2 className="text-lg font-semibold text-slate-900 mb-4">
          {t('dashboard.evolucao')}
        </h2>
        <ResponsiveContainer width="100%" height={300}>
          <LineChart data={dashboard.evolucaoMensal}>
            <CartesianGrid strokeDasharray="3 3" stroke="#e2e8f0" />
            <XAxis dataKey="mes" stroke="#94a3b8" />
            <YAxis stroke="#94a3b8" />
            <Tooltip
              contentStyle={{ backgroundColor: '#1e293b', border: 'none', borderRadius: '0.5rem' }}
              labelStyle={{ color: '#fff' }}
              formatter={(value) => formatCurrency(value as number)}
            />
            <Legend />
            <Line
              type="monotone"
              dataKey="receitas"
              stroke="#10b981"
              strokeWidth={2}
              dot={{ fill: '#10b981', r: 4 }}
              activeDot={{ r: 6 }}
              name="Receitas"
            />
            <Line
              type="monotone"
              dataKey="despesas"
              stroke="#ef4444"
              strokeWidth={2}
              dot={{ fill: '#ef4444', r: 4 }}
              activeDot={{ r: 6 }}
              name="Despesas"
            />
          </LineChart>
        </ResponsiveContainer>
      </div>

      {/* Gráfico de Despesas por Categoria */}
      {chartData.length > 0 && (
        <div className="bg-white rounded-lg border border-slate-200 p-6 shadow-sm">
          <h2 className="text-lg font-semibold text-slate-900 mb-4">
            {t('dashboard.porCategoria')}
          </h2>
          <ResponsiveContainer width="100%" height={300}>
            <PieChart>
              <Pie
                data={chartData}
                cx="50%"
                cy="50%"
                labelLine={false}
                label={({ name, percent }) => `${name} ${formatPercentage((percent ?? 0) * 100)}`}
                outerRadius={100}
                fill="#8884d8"
                dataKey="value"
              >
                {chartData.map((entry, index) => (
                  <Cell key={`cell-${index}`} fill={entry.color || COLORS[index % COLORS.length]} />
                ))}
              </Pie>
              <Tooltip formatter={(value) => formatCurrency(value as number)} />
            </PieChart>
          </ResponsiveContainer>
          <div className="mt-4 grid grid-cols-2 md:grid-cols-4 gap-2">
            {chartData.map((item, index) => (
              <div key={item.name} className="text-xs">
                <div className="flex items-center gap-2">
                  <div
                    className="w-3 h-3 rounded-full"
                    style={{ backgroundColor: item.color || COLORS[index % COLORS.length] }}
                  />
                  <span className="text-slate-600">{item.name}</span>
                </div>
              </div>
            ))}
          </div>
        </div>
      )}

      {/* Contas */}
      {dashboard.porConta.length > 0 && (
        <div className="bg-white rounded-lg border border-slate-200 p-6 shadow-sm">
          <h2 className="text-lg font-semibold text-slate-900 mb-4">
            {t('dashboard.contas')}
          </h2>
          <div className="overflow-x-auto">
            <table className="w-full text-sm">
              <thead className="border-b border-slate-200">
                <tr>
                  <th className="text-left py-3 px-4 font-medium text-slate-700">Conta</th>
                  <th className="text-right py-3 px-4 font-medium text-slate-700">Saldo Atual</th>
                  <th className="text-right py-3 px-4 font-medium text-slate-700">Entradas</th>
                  <th className="text-right py-3 px-4 font-medium text-slate-700">Saídas</th>
                </tr>
              </thead>
              <tbody>
                {dashboard.porConta.map((conta) => (
                  <tr key={conta.contaId} className="border-b border-slate-100 hover:bg-slate-50">
                    <td className="py-3 px-4 text-slate-900">{conta.contaNome}</td>
                    <td className="text-right py-3 px-4 font-medium text-slate-900">
                      {formatCurrency(conta.saldoAtual)}
                    </td>
                    <td className="text-right py-3 px-4 text-green-600">
                      {formatCurrency(conta.entradas)}
                    </td>
                    <td className="text-right py-3 px-4 text-red-600">
                      {formatCurrency(conta.saidas)}
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </div>
      )}

      {/* Cartões */}
      {dashboard.porCartao.length > 0 && (
        <div className="bg-white rounded-lg border border-slate-200 p-6 shadow-sm">
          <h2 className="text-lg font-semibold text-slate-900 mb-4">
            {t('dashboard.cartoes')}
          </h2>
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
            {dashboard.porCartao.map((cartao) => {
              const percentualUtilizado = (cartao.utilizado / cartao.limite) * 100
              return (
                <div
                  key={cartao.cartaoId}
                  className="border border-slate-200 rounded-lg p-4 hover:shadow-md transition-shadow"
                >
                  <h3 className="font-semibold text-slate-900 mb-2">{cartao.cartaoNome}</h3>
                  <div className="space-y-2 text-sm mb-3">
                    <div>
                      <p className="text-slate-600">Fatura Atual</p>
                      <p className="font-bold text-slate-900">
                        {formatCurrency(cartao.faturaBruta)}
                      </p>
                    </div>
                    <div>
                      <p className="text-slate-600">Limite</p>
                      <p className="font-bold text-slate-900">
                        {formatCurrency(cartao.limite)}
                      </p>
                    </div>
                  </div>
                  <div className="w-full bg-slate-200 rounded-full h-2">
                    <div
                      className={`h-2 rounded-full transition-all ${
                        percentualUtilizado > 80
                          ? 'bg-red-500'
                          : percentualUtilizado > 50
                            ? 'bg-yellow-500'
                            : 'bg-green-500'
                      }`}
                      style={{ width: `${Math.min(percentualUtilizado, 100)}%` }}
                    />
                  </div>
                  <p className="text-xs text-slate-500 mt-2">
                    {formatPercentage(percentualUtilizado)} utilizado
                  </p>
                </div>
              )
            })}
          </div>
        </div>
      )}

      {/* Próximos Vencimentos */}
      {dashboard.proximosVencimentos.length > 0 && (
        <div className="bg-white rounded-lg border border-slate-200 p-6 shadow-sm">
          <h2 className="text-lg font-semibold text-slate-900 mb-4">
            {t('dashboard.proximosVencimentos')}
          </h2>
          <div className="space-y-2">
            {dashboard.proximosVencimentos.slice(0, 5).map((item) => (
              <div
                key={item.id}
                className="flex items-center justify-between p-3 border border-slate-100 rounded-lg hover:bg-slate-50"
              >
                <div className="flex-1">
                  <p className="font-medium text-slate-900">{item.descricao}</p>
                  <p className="text-xs text-slate-500">{item.categoriaNome}</p>
                </div>
                <div className="text-right">
                  <p
                    className={`font-bold ${
                      item.natureza === 'RECEITA' ? 'text-green-600' : 'text-red-600'
                    }`}
                  >
                    {item.natureza === 'RECEITA' ? '+' : '-'} {formatCurrency(Math.abs(item.valor))}
                  </p>
                  <p className="text-xs text-slate-500">
                    {format(new Date(item.dataVencimento), 'dd/MM/yyyy')}
                  </p>
                </div>
              </div>
            ))}
          </div>
        </div>
      )}
    </div>
  )
}
