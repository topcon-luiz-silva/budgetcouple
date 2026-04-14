import { useEffect } from 'react'
import { useTranslation } from 'react-i18next'
import { useNavigate, useParams } from 'react-router-dom'
import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { z } from 'zod'
import { useCreateRegra, useUpdateRegra, useRegraById } from '../hooks'
import { useCategoriasList } from '@/features/categorias/hooks'
import { Button } from '@/components/ui/button'
import { Card } from '@/components/ui/card'
import { AlertCircle } from 'lucide-react'
import type { RegraFormData } from '../types'

const schema = z.object({
  nome: z.string().min(1, 'Nome é obrigatório'),
  padrao: z.string().min(1, 'Padrão é obrigatório'),
  tipoPadrao: z.enum(['CONTEM', 'IGUAL', 'REGEX', 'COMECA_COM', 'TERMINA_COM']),
  categoriaId: z.string().min(1, 'Categoria é obrigatória'),
  subcategoriaId: z.string().optional().nullable(),
  prioridade: z.number().min(0),
  ativa: z.boolean(),
})

export function RegrasFormPage() {
  const { t } = useTranslation()
  const { id } = useParams<{ id: string }>()
  const navigate = useNavigate()

  const {
    register,
    handleSubmit,
    formState: { errors },
    reset,
    watch,
  } = useForm<RegraFormData>({
    resolver: zodResolver(schema),
    defaultValues: {
      nome: '',
      padrao: '',
      tipoPadrao: 'CONTEM',
      categoriaId: '',
      prioridade: 0,
      ativa: true,
    },
  })

  const { data: categorias = [] } = useCategoriasList()
  const { data: regraData, isLoading: isLoadingRegra } = useRegraById(id || '')
  const { mutate: createRegra, isPending: isCreating } = useCreateRegra()
  const { mutate: updateRegra, isPending: isUpdating } = useUpdateRegra(id || '')

  useEffect(() => {
    if (regraData) {
      reset(regraData)
    }
  }, [regraData, reset])

  const onSubmit = (data: RegraFormData) => {
    if (id) {
      updateRegra(data, {
        onSuccess: () => {
          navigate('/regras')
        },
        onError: (error: any) => {
          alert(error.response?.data?.message || t('regras.updateError'))
        },
      })
    } else {
      createRegra(data, {
        onSuccess: () => {
          navigate('/regras')
        },
        onError: (error: any) => {
          alert(error.response?.data?.message || t('regras.createError'))
        },
      })
    }
  }

  const selectedCategory = watch('categoriaId')
  const selectedCategoryData = (categorias as any[]).find((c) => c.id === selectedCategory)
  const subcategorias = selectedCategoryData?.subcategorias || []

  if (id && isLoadingRegra) {
    return <div className="text-center py-8">{t('common.loading')}</div>
  }

  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-3xl font-bold">
          {id ? t('regras.edit') : t('regras.create')}
        </h1>
        <p className="text-gray-600 mt-2">
          {id ? t('regras.editSubtitle') : t('regras.createSubtitle')}
        </p>
      </div>

      <Card className="p-6">
        <form onSubmit={handleSubmit(onSubmit)} className="space-y-6">
          {/* Basic Info */}
          <div className="grid grid-cols-2 gap-4">
            <div>
              <label className="block text-sm font-medium mb-2">{t('regras.ruleName')}</label>
              <input
                {...register('nome')}
                placeholder={t('regras.ruleName')}
                className="w-full px-3 py-2 border border-gray-300 rounded-md"
              />
              {errors.nome && <p className="text-red-600 text-sm mt-1">{errors.nome.message}</p>}
            </div>

            <div>
              <label className="block text-sm font-medium mb-2">{t('regras.priority')}</label>
              <input
                {...register('prioridade', { valueAsNumber: true })}
                type="number"
                placeholder="0"
                className="w-full px-3 py-2 border border-gray-300 rounded-md"
              />
              {errors.prioridade && (
                <p className="text-red-600 text-sm mt-1">{errors.prioridade.message}</p>
              )}
            </div>
          </div>

          {/* Pattern */}
          <div>
            <label className="block text-sm font-medium mb-2">{t('regras.pattern')}</label>
            <input
              {...register('padrao')}
              placeholder={t('regras.patternPlaceholder')}
              className="w-full px-3 py-2 border border-gray-300 rounded-md"
            />
            {errors.padrao && <p className="text-red-600 text-sm mt-1">{errors.padrao.message}</p>}
            <p className="text-xs text-gray-500 mt-1">{t('regras.patternHelp')}</p>
          </div>

          {/* Match Type */}
          <div>
            <label className="block text-sm font-medium mb-2">{t('regras.matchType')}</label>
            <select
              {...register('tipoPadrao')}
              className="w-full px-3 py-2 border border-gray-300 rounded-md"
            >
              <option value="CONTEM">{t('regras.tipos.contem')}</option>
              <option value="IGUAL">{t('regras.tipos.igual')}</option>
              <option value="REGEX">{t('regras.tipos.regex')}</option>
              <option value="COMECA_COM">{t('regras.tipos.comecaCom')}</option>
              <option value="TERMINA_COM">{t('regras.tipos.terminaCom')}</option>
            </select>
            {errors.tipoPadrao && (
              <p className="text-red-600 text-sm mt-1">{errors.tipoPadrao.message}</p>
            )}
          </div>

          {/* Category */}
          <div className="grid grid-cols-2 gap-4">
            <div>
              <label className="block text-sm font-medium mb-2">{t('regras.category')}</label>
              <select
                {...register('categoriaId')}
                className="w-full px-3 py-2 border border-gray-300 rounded-md"
              >
                <option value="">{t('regras.selectCategory')}</option>
                {categorias.map((cat: any) => (
                  <option key={cat.id} value={cat.id}>
                    {cat.nome}
                  </option>
                ))}
              </select>
              {errors.categoriaId && (
                <p className="text-red-600 text-sm mt-1">{errors.categoriaId.message}</p>
              )}
            </div>

            {subcategorias.length > 0 && (
              <div>
                <label className="block text-sm font-medium mb-2">
                  {t('regras.subcategory')}
                </label>
                <select
                  {...register('subcategoriaId')}
                  className="w-full px-3 py-2 border border-gray-300 rounded-md"
                >
                  <option value="">{t('regras.selectSubcategory')}</option>
                  {subcategorias.map((subcat: any) => (
                    <option key={subcat.id} value={subcat.id}>
                      {subcat.nome}
                    </option>
                  ))}
                </select>
              </div>
            )}
          </div>

          {/* Active Status */}
          <label className="flex items-center gap-2">
            <input
              {...register('ativa')}
              type="checkbox"
              className="rounded"
            />
            <span className="text-sm">{t('regras.active')}</span>
          </label>

          {/* Info Message */}
          <Card className="p-4 bg-blue-50">
            <div className="flex gap-2">
              <AlertCircle className="text-blue-600 flex-shrink-0 mt-0.5" />
              <div className="text-sm text-blue-900">
                <p className="font-semibold mb-1">{t('regras.tipInfo')}</p>
                <p>{t('regras.priorityInfo')}</p>
              </div>
            </div>
          </Card>

          {/* Action Buttons */}
          <div className="flex gap-2 pt-4">
            <Button
              type="submit"
              disabled={isCreating || isUpdating}
              className="flex-1"
            >
              {isCreating || isUpdating ? t('common.loading') : t('common.save')}
            </Button>
            <Button
              type="button"
              variant="outline"
              onClick={() => navigate('/regras')}
              className="flex-1"
            >
              {t('common.cancel')}
            </Button>
          </div>
        </form>
      </Card>
    </div>
  )
}
