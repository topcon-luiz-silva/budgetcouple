import { useEffect } from 'react'
import { useNavigate, useParams } from 'react-router-dom'
import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { useTranslation } from 'react-i18next'
import { ArrowLeft } from 'lucide-react'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { Select } from '@/components/ui/select'
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card'
import { Alert, AlertDescription, AlertTitle } from '@/components/ui/alert'
import { useCategoriaById, useCreateCategoria, useUpdateCategoria, useCategoriasList } from '../hooks'
import type { CategoriaFormData, TipoCategoria } from '../types'
import { categoriaFormSchema } from '../types'
import { AxiosError } from 'axios'

const tipoCategoriaLabels: Record<TipoCategoria, string> = {
  DESPESA: 'Despesa',
  RECEITA: 'Receita',
}

const ÍCONES_DISPONÍVEIS = ['💸', '🛒', '🍔', '🏠', '🚗', '💊', '📚', '🎬', '🎮', '⚽']

export function CategoriaFormPage() {
  const { t } = useTranslation()
  const navigate = useNavigate()
  const { id } = useParams<{ id?: string }>()
  const isEditing = !!id

  const { data: categoriaExistente, isPending: isLoadingCategoria } = useCategoriaById(id || '')
  const { data: categorias = [] } = useCategoriasList()
  const { mutate: createCategoria, isPending: isCreating, error: createError } = useCreateCategoria()
  const { mutate: updateCategoria, isPending: isUpdating, error: updateError } = useUpdateCategoria(id || '')

  const isPending = isCreating || isUpdating
  const error = createError || updateError

  const {
    register,
    handleSubmit,
    formState: { errors },
    reset,
    watch,
  } = useForm<CategoriaFormData>({
    resolver: zodResolver(categoriaFormSchema),
  })

  const corHex = watch('corHex')

  useEffect(() => {
    if (categoriaExistente) {
      reset({
        nome: categoriaExistente.nome,
        tipoCategoria: categoriaExistente.tipoCategoria,
        corHex: categoriaExistente.corHex,
        icone: categoriaExistente.icone,
        parentCategoriaId: categoriaExistente.parentCategoriaId,
      })
    }
  }, [categoriaExistente, reset])

  const onSubmit = (data: CategoriaFormData) => {
    if (isEditing) {
      updateCategoria(data, {
        onSuccess: () => navigate('/categorias'),
      })
    } else {
      createCategoria(data, {
        onSuccess: () => navigate('/categorias'),
      })
    }
  }

  let errorMessage: string | null = null
  if (error instanceof AxiosError) {
    errorMessage = error.response?.data?.error || error.message
  } else if (error) {
    errorMessage = 'Erro ao processar categoria'
  }

  if (isLoadingCategoria) {
    return (
      <div className="flex items-center justify-center min-h-screen">
        <p className="text-slate-600">{t('common.loading')}</p>
      </div>
    )
  }

  return (
    <div className="max-w-2xl mx-auto space-y-6">
      <button
        onClick={() => navigate('/categorias')}
        className="flex items-center gap-2 text-slate-600 hover:text-slate-900 transition-colors"
      >
        <ArrowLeft className="h-5 w-5" />
        Voltar
      </button>

      <Card>
        <CardHeader>
          <CardTitle>{isEditing ? t('categorias.edit') : t('categorias.new')}</CardTitle>
          <CardDescription>
            {isEditing ? 'Edite os detalhes da categoria' : 'Crie uma nova categoria'}
          </CardDescription>
        </CardHeader>

        <CardContent>
          <form onSubmit={handleSubmit(onSubmit)} className="space-y-6">
            {errorMessage && (
              <Alert variant="destructive">
                <AlertTitle>Erro</AlertTitle>
                <AlertDescription>{errorMessage}</AlertDescription>
              </Alert>
            )}

            {/* Nome */}
            <div className="space-y-2">
              <Label htmlFor="nome">Nome</Label>
              <Input
                id="nome"
                placeholder="Ex: Supermercado"
                {...register('nome')}
                aria-invalid={!!errors.nome}
              />
              {errors.nome && (
                <p className="text-sm text-red-600">{errors.nome.message as string}</p>
              )}
            </div>

            {/* Tipo de Categoria */}
            <div className="space-y-2">
              <Label htmlFor="tipoCategoria">Tipo</Label>
              <Select
                id="tipoCategoria"
                {...register('tipoCategoria')}
                aria-invalid={!!errors.tipoCategoria}
              >
                <option value="">Selecione um tipo</option>
                {Object.entries(tipoCategoriaLabels).map(([key, label]) => (
                  <option key={key} value={key}>
                    {label}
                  </option>
                ))}
              </Select>
              {errors.tipoCategoria && (
                <p className="text-sm text-red-600">{errors.tipoCategoria.message as string}</p>
              )}
            </div>

            {/* Categoria Pai (opcional) */}
            <div className="space-y-2">
              <Label htmlFor="parentCategoriaId">Categoria Pai (Opcional)</Label>
              <Select
                id="parentCategoriaId"
                {...register('parentCategoriaId')}
                aria-invalid={!!errors.parentCategoriaId}
              >
                <option value="">Nenhuma (categoria raiz)</option>
                {categorias
                  .filter((cat) => cat.id !== id)
                  .map((cat) => (
                    <option key={cat.id} value={cat.id}>
                      {cat.nome}
                    </option>
                  ))}
              </Select>
              {errors.parentCategoriaId && (
                <p className="text-sm text-red-600">{errors.parentCategoriaId.message as string}</p>
              )}
            </div>

            {/* Cor */}
            <div className="space-y-2">
              <Label htmlFor="corHex">Cor</Label>
              <div className="flex gap-4">
                <Input
                  id="corHex"
                  type="color"
                  {...register('corHex')}
                  aria-invalid={!!errors.corHex}
                  className="h-12 w-20 p-1 cursor-pointer"
                />
                <Input
                  type="text"
                  value={corHex || ''}
                  readOnly
                  className="flex-1 bg-slate-50"
                />
              </div>
              {errors.corHex && (
                <p className="text-sm text-red-600">{errors.corHex.message as string}</p>
              )}
            </div>

            {/* Ícone */}
            <div className="space-y-2">
              <Label htmlFor="icone">Ícone</Label>
              <div className="flex gap-2 flex-wrap">
                {ÍCONES_DISPONÍVEIS.map((icon) => (
                  <button
                    key={icon}
                    type="button"
                    onClick={() => {
                      const event = new Event('change', { bubbles: true })
                      const input = document.getElementById('icone') as HTMLInputElement
                      if (input) {
                        input.value = icon
                        input.dispatchEvent(event)
                      }
                    }}
                    className={`text-3xl p-2 rounded-md border-2 transition-colors ${
                      watch('icone') === icon
                        ? 'border-slate-900 bg-slate-100'
                        : 'border-slate-300 hover:border-slate-400'
                    }`}
                  >
                    {icon}
                  </button>
                ))}
              </div>
              <Input
                id="icone"
                {...register('icone')}
                className="hidden"
                aria-invalid={!!errors.icone}
              />
              {errors.icone && (
                <p className="text-sm text-red-600">{errors.icone.message as string}</p>
              )}
            </div>

            {/* Botões */}
            <div className="flex gap-4 pt-6 border-t border-slate-200">
              <Button
                type="button"
                variant="outline"
                onClick={() => navigate('/categorias')}
                disabled={isPending}
              >
                {t('common.cancel')}
              </Button>
              <Button type="submit" disabled={isPending} className="flex-1">
                {isPending ? 'Salvando...' : t('common.save')}
              </Button>
            </div>
          </form>
        </CardContent>
      </Card>
    </div>
  )
}
