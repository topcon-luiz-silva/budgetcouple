import type { AlertaOrcamentoDashboard } from '../types'
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card'
import { Badge } from '@/components/ui/badge'
import { AlertTriangle } from 'lucide-react'
import { formatCurrency } from '@/lib/format'
import { useTranslation } from 'react-i18next'

interface AlertasOrcamentoSectionProps {
  alertas: AlertaOrcamentoDashboard[]
}

export const AlertasOrcamentoSection: React.FC<AlertasOrcamentoSectionProps> = ({
  alertas,
}) => {
  const { t } = useTranslation()

  if (!alertas || alertas.length === 0) {
    return null
  }

  return (
    <Card className="border-yellow-200 bg-yellow-50">
      <CardHeader>
        <CardTitle className="flex items-center gap-2 text-yellow-900">
          <AlertTriangle className="w-5 h-5" />
          {t('metas.alertas.title')}
        </CardTitle>
      </CardHeader>

      <CardContent>
        <div className="space-y-4">
          {alertas.map((alerta) => (
            <div key={alerta.metaId} className="bg-white p-4 rounded border border-yellow-100">
              <div className="flex justify-between items-start mb-2">
                <div>
                  <p className="font-semibold text-sm">{alerta.nomeMeta}</p>
                  {alerta.categoriaNome && (
                    <Badge className="mt-1 text-xs">
                      {alerta.categoriaNome}
                    </Badge>
                  )}
                </div>
                <span className="text-sm font-bold text-yellow-700">
                  {alerta.percentualUtilizado.toFixed(1)}%
                </span>
              </div>

              <div className="w-full bg-slate-200 rounded-full h-2 mb-2">
                <div
                  className={`h-2 rounded-full transition-all ${
                    alerta.percentualUtilizado > 100
                      ? 'bg-red-500'
                      : alerta.percentualUtilizado > 80
                        ? 'bg-yellow-500'
                        : 'bg-green-500'
                  }`}
                  style={{ width: `${Math.min(alerta.percentualUtilizado, 100)}%` }}
                />
              </div>

              <div className="flex justify-between text-xs text-slate-600">
                <span>{formatCurrency(alerta.valorAtual)} usado</span>
                <span>{formatCurrency(alerta.valorAlvo)} limite</span>
              </div>
            </div>
          ))}
        </div>
      </CardContent>
    </Card>
  )
}
