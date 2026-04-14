import { useState } from 'react'
import { useTranslation } from 'react-i18next'
import { Link } from 'react-router-dom'
import { useRegrasList, useDeleteRegra } from '../hooks'
import { Button } from '@/components/ui/button'
import { Card } from '@/components/ui/card'
import { Plus, Edit2, Trash2, AlertCircle } from 'lucide-react'

export function RegrasListPage() {
  const { t } = useTranslation()
  const [showInactive, setShowInactive] = useState(false)

  const { data: regras = [], isLoading, error } = useRegrasList(!showInactive || undefined)
  const { mutate: deleteRegra, isPending: isDeleting } = useDeleteRegra()

  const handleDelete = (id: string) => {
    if (confirm(t('common.confirmDelete'))) {
      deleteRegra(id)
    }
  }

  const tipoLabels: Record<string, string> = {
    CONTEM: t('regras.tipos.contem'),
    IGUAL: t('regras.tipos.igual'),
    REGEX: t('regras.tipos.regex'),
    COMECA_COM: t('regras.tipos.comecaCom'),
    TERMINA_COM: t('regras.tipos.terminaCom'),
  }

  if (isLoading) {
    return <div className="text-center py-8">{t('common.loading')}</div>
  }

  if (error) {
    return (
      <Card className="p-6 bg-red-50">
        <div className="flex gap-2">
          <AlertCircle className="text-red-600 flex-shrink-0" />
          <div>
            <h3 className="font-semibold text-red-900">{t('common.error')}</h3>
            <p className="text-red-700 text-sm mt-1">{t('regras.loadError')}</p>
          </div>
        </div>
      </Card>
    )
  }

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-3xl font-bold">{t('regras.title')}</h1>
          <p className="text-gray-600 mt-2">{t('regras.subtitle')}</p>
        </div>
        <Link to="/regras/novo">
          <Button>
            <Plus className="w-4 h-4 mr-2" />
            {t('regras.new')}
          </Button>
        </Link>
      </div>

      {/* Filter */}
      <Card className="p-4">
        <label className="flex items-center gap-2">
          <input
            type="checkbox"
            checked={showInactive}
            onChange={(e) => setShowInactive(e.target.checked)}
            className="rounded"
          />
          <span className="text-sm">{t('regras.showInactive')}</span>
        </label>
      </Card>

      {/* Rules List */}
      {regras.length === 0 ? (
        <Card className="p-8 text-center">
          <p className="text-gray-500">{t('regras.noRules')}</p>
          <Link to="/regras/novo">
            <Button className="mt-4">{t('regras.createFirst')}</Button>
          </Link>
        </Card>
      ) : (
        <div className="grid gap-4">
          {regras.map((regra: any) => (
            <Card key={regra.id} className="p-6">
              <div className="flex items-start justify-between">
                <div className="flex-1">
                  <div className="flex items-center gap-3 mb-2">
                    <h3 className="text-lg font-semibold">{regra.nome}</h3>
                    {!regra.ativa && (
                      <span className="bg-gray-100 text-gray-700 text-xs px-2 py-1 rounded">
                        {t('common.inactive')}
                      </span>
                    )}
                  </div>

                  <div className="grid grid-cols-2 gap-4 text-sm">
                    <div>
                      <p className="text-gray-500">{t('regras.pattern')}</p>
                      <p className="font-mono text-gray-900">"{regra.padrao}"</p>
                    </div>
                    <div>
                      <p className="text-gray-500">{t('regras.matchType')}</p>
                      <p className="text-gray-900">{tipoLabels[regra.tipoPadrao]}</p>
                    </div>
                    <div>
                      <p className="text-gray-500">{t('regras.category')}</p>
                      <p className="text-gray-900">{regra.categoriaNome}</p>
                    </div>
                    <div>
                      <p className="text-gray-500">{t('regras.priority')}</p>
                      <p className="text-gray-900">{regra.prioridade}</p>
                    </div>
                  </div>
                </div>

                <div className="flex gap-2 ml-4">
                  <Link to={`/regras/${regra.id}/editar`}>
                    <Button variant="outline" size="sm">
                      <Edit2 className="w-4 h-4" />
                    </Button>
                  </Link>
                  <Button
                    variant="outline"
                    size="sm"
                    onClick={() => handleDelete(regra.id)}
                    disabled={isDeleting}
                  >
                    <Trash2 className="w-4 h-4 text-red-600" />
                  </Button>
                </div>
              </div>
            </Card>
          ))}
        </div>
      )}
    </div>
  )
}
