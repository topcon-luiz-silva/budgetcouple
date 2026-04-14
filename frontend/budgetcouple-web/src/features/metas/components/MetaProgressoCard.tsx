import type { Meta, MetaProgresso } from '../types'
import { Badge } from '@/components/ui/badge'
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card'
import { formatCurrency } from '@/lib/format'
import { format } from 'date-fns'
import { ptBR } from 'date-fns/locale'
import { Target, TrendingDown } from 'lucide-react'

interface MetaProgressoCardProps {
  meta: Meta;
  progresso: MetaProgresso;
}

export const MetaProgressoCard: React.FC<MetaProgressoCardProps> = ({ meta, progresso }) => {
  const percentual = progresso.percentualProgresso

  return (
    <Card>
      <CardHeader className="pb-3">
        <div className="flex items-start justify-between">
          <div className="flex-1">
            <CardTitle className="text-lg">{meta.nome}</CardTitle>
            <div className="flex items-center gap-2 mt-2">
              <Badge>
                {meta.tipo === 'ECONOMIA' ? 'Economia' : 'Redução'}
              </Badge>
              {meta.dataTermino && (
                <span className="text-xs text-slate-600">
                  Até {format(new Date(meta.dataTermino), 'dd/MM/yyyy', { locale: ptBR })}
                </span>
              )}
            </div>
          </div>
          {meta.tipo === 'ECONOMIA' ? (
            <Target className="w-5 h-5 text-blue-500" />
          ) : (
            <TrendingDown className="w-5 h-5 text-orange-500" />
          )}
        </div>
      </CardHeader>

      <CardContent>
        <div className="space-y-4">
          <div>
            <div className="flex justify-between items-center mb-2">
              <span className="text-sm font-medium">Progresso</span>
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

          <div className="grid grid-cols-2 gap-4">
            <div>
              <p className="text-xs text-slate-600">Valor Atual</p>
              <p className="text-sm font-semibold">{formatCurrency(progresso.valorAtual)}</p>
            </div>
            <div>
              <p className="text-xs text-slate-600">Valor Alvo</p>
              <p className="text-sm font-semibold">{formatCurrency(progresso.valorAlvo)}</p>
            </div>
          </div>

          {meta.tipo === 'ECONOMIA' && progresso.diasRestantes !== undefined && (
            <div className="text-xs text-slate-600">
              {progresso.diasRestantes} dias restantes
            </div>
          )}

          {progresso.atingiuAlerta && !progresso.atingida && (
            <div className="p-2 bg-yellow-50 border border-yellow-200 rounded text-xs text-yellow-800">
              Meta atingiu {meta.percentualAlerta}% do alvo
            </div>
          )}

          {progresso.atingida && (
            <div className="p-2 bg-red-50 border border-red-200 rounded text-xs text-red-800">
              Meta atingida ou ultrapassada
            </div>
          )}

          {!progresso.atingiuAlerta && !progresso.atingida && (
            <div className="p-2 bg-green-50 border border-green-200 rounded text-xs text-green-800">
              Meta no caminho certo
            </div>
          )}
        </div>
      </CardContent>
    </Card>
  )
}
