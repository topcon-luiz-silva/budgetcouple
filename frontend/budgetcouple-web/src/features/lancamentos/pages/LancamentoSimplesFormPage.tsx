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
import {
  useCreateLancamentoSimples,
  useUpdateLancamento,
  useLancamentoById,
} from '../hooks'
import { useContasList } from '@/features/contas/hooks'
import { useCartoesList } from '@/features/cartoes/hooks'
import { useCategoriasList } from '@/features/categorias/hooks'
import {
  lancamentoSimplesFormSchema,
  type LancamentoSimplesFormData,
  type NaturezaLancamento,
} from '../types'
import { AxiosError } from 'axios'

const naturezaLabels: Record<NaturezaLancamento, string> = {
  DESPESA: 'Despesa',
  RECEITA: 'Receita',
  TRANSFERENCIA: 'Transferência',
}

export function LancamentoSimplesFormPage() {
  const { t } = useTranslation()
  const navigate = useNavigate()
  const { id } = useParams<{ id?: string }>()
  const isEditing = !!id

  const { data: lancamento, isPending: isLoadingLancamento } = useLancamentoById(id || '')
  const { data: contas = [] } = useContasList()
  const { data: cartoes = [] } = useCartoesList()
  const { data: categorias = [] } = useCategoriasList()

  const { mutate: createLancamento, isPending: isCreating, error: createError } =
    useCreateLancamentoSimples()
  const { mutate: updateLancamento, isPending: isUpdating, error: updateError } =
    useUpdateLancamento(id || '')

  const isPending = isCreating || isUpdating
  const error = createError || updateError

  const {
    register,
    handleSubmit,
    formState: { errors },
    reset,
    watch,
  } = useForm({
    resolver: zodResolver(lancamentoSimplesFormSchema),
    defaultValues: {
      tags: [],
    },
  })

  const statusPagamento = watch('statusPagamento')

  useEffect(() => {
    if (lancamento && isEditing) {
      reset({
        descricao: lancamento.descricao,
        valor: lancamento.valor,
        dataCompetencia: lancamento.dataCompetencia,
        dataVencimento: lancamento.dataVencimento,
        naturezaLancamento: lancamento.naturezaLancamento as NaturezaLancamento,
        contaId: lancamento.contaId,
        cartaoId: lancamento.cartaoId,
        categoriaId: lancamento.categoriaId,
        subcategoriaId: lancamento.subcategoriaId,
        tags: lancamento.tags,
        observacoes: lancamento.observacoes,
        statusPagamento: lancamento.statusPagamento as 'PREVISTO' | 'REALIZADO',
        dataPagamento: lancamento.dataPagamento,
      })
    }
  }, [lancamento, isEditing, reset])

  const onSubmit = (data: LancamentoSimplesFormData) => {
    // Clean empty strings to undefined so backend doesn't receive invalid GUIDs
    const cleaned = {
      ...data,
      contaId: data.contaId || undefined,
      cartaoId: data.cartaoId || undefined,
      subcategoriaId: data.subcategoriaId || undefined,
      observacoes: data.observacoes || undefined,
      dataVencimento: data.dataVencimento || undefined,
      dataPagamento: data.dataPagamento || undefined,
    }
    if (isEditing) {
      updateLancamento(cleaned, {
        onSuccess: () => navigate('/lancamentos'),
      })
    } else {
      createLancamento(cleaned, {
        onSuccess: () => navigate('/lancamentos'),
      })
    }
  }

  let errorMessage: string | null = null
  if (error instanceof AxiosError) {
    errorMessage = error.response?.data?.error || error.message
  } else if (error) {
    errorMessage = 'Erro ao processar lançamento'
  }

  if (isEditing && isLoadingLancamento) {
    return (
      <div className="flex items-center justify-center min-h-screen">
        <p className="text-slate-600">{t('common.loading')}</p>
      </div>
    )
  }

  return (
    <div className="max-w-2xl mx-auto space-y-6">
      <button
        onClick={() => navigate('/lancamentos')}
        className="flex items-center gap-2 text-slate-600 hover:text-slate-900 transition-colors"
      >
        <ArrowLeft className="h-5 w-5" />
        Voltar
      </button>

      <Card>
        <CardHeader>
          <CardTitle>{isEditing ? 'Editar Lançamento' : t('lancamentos.simples')}</CardTitle>
          <CardDescription>
            {isEditing ? 'Edite os detalhes do lançamento' : 'Crie um novo lançamento simples'}
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

            {/* Descrição */}
            <div className="space-y-2">
              <Label htmlFor="descricao">{t('lancamentos.fields.descricao')}</Label>
              <Input
                id="descricao"
                placeholder="Ex: Compra no supermercado"
                {...register('descricao')}
                aria-invalid={!!errors.descricao}
              />
              {errors.descricao && (
                <p className="text-sm text-red-600">{errors.descricao.message as string}</p>
              )}
            </div>

            {/* Valor */}
            <div className="space-y-2">
              <Label htmlFor="valor">{t('lancamentos.fields.valor')}</Label>
              <Input
                id="valor"
                type="number"
                step="0.01"
                placeholder="0.00"
                {...register('valor')}
                aria-invalid={!!errors.valor}
              />
              {errors.valor && (
                <p className="text-sm text-red-600">{errors.valor.message as string}</p>
              )}
            </div>

            {/* Data de Competência */}
            <div className="space-y-2">
              <Label htmlFor="dataCompetencia">{t('lancamentos.fields.dataCompetencia')}</Label>
              <input
                id="dataCompetencia"
                type="date"
                {...register('dataCompetencia')}
                className="flex w-full rounded-md border border-slate-300 bg-white px-3 py-2 text-base placeholder:text-slate-500 focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-slate-400"
                aria-invalid={!!errors.dataCompetencia}
              />
              {errors.dataCompetencia && (
                <p className="text-sm text-red-600">{errors.dataCompetencia.message as string}</p>
              )}
            </div>

            {/* Data de Vencimento */}
            <div className="space-y-2">
              <Label htmlFor="dataVencimento">Data de Vencimento (opcional)</Label>
              <input
                id="dataVencimento"
                type="date"
                {...register('dataVencimento')}
                className="flex w-full rounded-md border border-slate-300 bg-white px-3 py-2 text-base placeholder:text-slate-500 focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-slate-400"
              />
            </div>

            {/* Natureza */}
            <div className="space-y-2">
              <Label htmlFor="naturezaLancamento">{t('lancamentos.fields.natureza')}</Label>
              <Select
                id="naturezaLancamento"
                {...register('naturezaLancamento')}
                aria-invalid={!!errors.naturezaLancamento}
              >
                <option value="">Selecione</option>
                {Object.entries(naturezaLabels).map(([key, label]) => (
                  <option key={key} value={key}>
                    {label}
                  </option>
                ))}
              </Select>
              {errors.naturezaLancamento && (
                <p className="text-sm text-red-600">{errors.naturezaLancamento.message as string}</p>
              )}
            </div>

            {/* Conta / Cartão */}
            <div className="grid grid-cols-2 gap-4">
              <div className="space-y-2">
                <Label htmlFor="contaId">{t('lancamentos.fields.conta')}</Label>
                <Select id="contaId" {...register('contaId')}>
                  <option value="">Selecione uma conta</option>
                  {contas.map((conta) => (
                    <option key={conta.id} value={conta.id}>
                      {conta.nome}
                    </option>
                  ))}
                </Select>
              </div>

              <div className="space-y-2">
                <Label htmlFor="cartaoId">{t('lancamentos.fields.cartao')}</Label>
                <Select id="cartaoId" {...register('cartaoId')}>
                  <option value="">Selecione um cartão</option>
                  {cartoes.map((cartao) => (
                    <option key={cartao.id} value={cartao.id}>
                      {cartao.nome}
                    </option>
                  ))}
                </Select>
              </div>
            </div>
            {(errors.contaId || errors.cartaoId) && (
              <p className="text-sm text-red-600">Você deve selecionar uma conta ou um cartão</p>
            )}

            {/* Categoria */}
            <div className="space-y-2">
              <Label htmlFor="categoriaId">{t('lancamentos.fields.categoria')}</Label>
              <Select
                id="categoriaId"
                {...register('categoriaId')}
                aria-invalid={!!errors.categoriaId}
              >
                <option value="">Selecione uma categoria</option>
                {categorias.map((categoria) => (
                  <option key={categoria.id} value={categoria.id}>
                    {categoria.nome}
                  </option>
                ))}
              </Select>
              {errors.categoriaId && (
                <p className="text-sm text-red-600">{errors.categoriaId.message as string}</p>
              )}
            </div>

            {/* Status de Pagamento */}
            <div className="space-y-2">
              <Label htmlFor="statusPagamento">Status de Pagamento</Label>
              <Select
                id="statusPagamento"
                {...register('statusPagamento')}
                aria-invalid={!!errors.statusPagamento}
              >
                <option value="PREVISTO">Previsto</option>
                <option value="REALIZADO">Realizado</option>
              </Select>
            </div>

            {/* Data de Pagamento (condicional) */}
            {statusPagamento === 'REALIZADO' && (
              <div className="space-y-2">
                <Label htmlFor="dataPagamento">{t('lancamentos.fields.dataPagamento')}</Label>
                <input
                  id="dataPagamento"
                  type="date"
                  {...register('dataPagamento')}
                  className="flex w-full rounded-md border border-slate-300 bg-white px-3 py-2 text-base placeholder:text-slate-500 focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-slate-400"
                  aria-invalid={!!errors.dataPagamento}
                />
                {errors.dataPagamento && (
                  <p className="text-sm text-red-600">{errors.dataPagamento.message as string}</p>
                )}
              </div>
            )}

            {/* Observações */}
            <div className="space-y-2">
              <Label htmlFor="observacoes">Observações (opcional)</Label>
              <textarea
                id="observacoes"
                placeholder="Adicione observações"
                {...register('observacoes')}
                className="flex min-h-24 w-full rounded-md border border-slate-300 bg-white px-3 py-2 text-base placeholder:text-slate-500 focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-slate-400"
              />
              {errors.observacoes && (
                <p className="text-sm text-red-600">{errors.observacoes.message as string}</p>
              )}
            </div>

            {/* Botões */}
            <div className="flex gap-4 pt-6 border-t border-slate-200">
              <Button
                type="button"
                variant="outline"
                onClick={() => navigate('/lancamentos')}
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
