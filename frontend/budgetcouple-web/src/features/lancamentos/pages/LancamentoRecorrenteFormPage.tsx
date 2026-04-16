import { useNavigate } from 'react-router-dom'
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
import { useCreateLancamentoRecorrencia } from '../hooks'
import { useContasList } from '@/features/contas/hooks'
import { useCartoesList } from '@/features/cartoes/hooks'
import { useCategoriasList } from '@/features/categorias/hooks'
import {
  lancamentoRecorrenteFormSchema,
  type NaturezaLancamento,
  type FrequenciaRecorrencia,
} from '../types'
import { AxiosError } from 'axios'

const naturezaLabels: Record<NaturezaLancamento, string> = {
  DESPESA: 'Despesa',
  RECEITA: 'Receita',
  TRANSFERENCIA: 'Transferência',
}

const frequenciaLabels: Record<FrequenciaRecorrencia, string> = {
  DIARIA: 'Diariamente',
  SEMANAL: 'Semanalmente',
  QUINZENAL: 'Quinzenalmente',
  MENSAL: 'Mensalmente',
  TRIMESTRAL: 'Trimestralmente',
  SEMESTRAL: 'Semestralmente',
  ANUAL: 'Anualmente',
}

export function LancamentoRecorrenteFormPage() {
  const { t } = useTranslation()
  const navigate = useNavigate()

  const { data: contas = [] } = useContasList()
  const { data: cartoes = [] } = useCartoesList()
  const { data: categorias = [] } = useCategoriasList()

  const { mutate: createRecorrencia, isPending, error } = useCreateLancamentoRecorrencia()

  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm({
    resolver: zodResolver(lancamentoRecorrenteFormSchema),
    defaultValues: {
      tags: [],
    },
  })

  const onSubmit = (data: any) => {
    // Clean empty strings to undefined so backend doesn't receive invalid GUIDs
    const cleaned = {
      ...data,
      contaId: data.contaId || undefined,
      cartaoId: data.cartaoId || undefined,
      subcategoriaId: data.subcategoriaId || undefined,
      dataFim: data.dataFim || undefined,
      gerarOcorrenciasAte: data.gerarOcorrenciasAte || undefined,
      observacoes: data.observacoes || undefined,
    }
    createRecorrencia(cleaned, {
      onSuccess: () => navigate('/lancamentos'),
    })
  }

  let errorMessage: string | null = null
  if (error instanceof AxiosError) {
    errorMessage = error.response?.data?.error || error.message
  } else if (error) {
    errorMessage = 'Erro ao processar lançamento recorrente'
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
          <CardTitle>{t('lancamentos.recorrente')}</CardTitle>
          <CardDescription>Crie um lançamento que se repete em um intervalo regular</CardDescription>
        </CardHeader>

        <CardContent>
          <form onSubmit={handleSubmit(onSubmit)} className="space-y-6">
            {errorMessage && (
              <Alert variant="destructive">
                <AlertTitle>Erro</AlertTitle>
                <AlertDescription>{errorMessage}</AlertDescription>
              </Alert>
            )}

            {/* Descrição Base */}
            <div className="space-y-2">
              <Label htmlFor="descricaoBase">Descrição</Label>
              <Input
                id="descricaoBase"
                placeholder="Ex: Aluguel"
                {...register('descricaoBase')}
                aria-invalid={!!errors.descricaoBase}
              />
              {errors.descricaoBase && (
                <p className="text-sm text-red-600">{errors.descricaoBase.message as string}</p>
              )}
            </div>

            {/* Valor Base */}
            <div className="space-y-2">
              <Label htmlFor="valorBase">Valor Base (R$)</Label>
              <Input
                id="valorBase"
                type="number"
                step="0.01"
                placeholder="0.00"
                {...register('valorBase')}
                aria-invalid={!!errors.valorBase}
              />
              {errors.valorBase && (
                <p className="text-sm text-red-600">{errors.valorBase.message as string}</p>
              )}
            </div>

            {/* Frequência */}
            <div className="space-y-2">
              <Label htmlFor="frequencia">{t('lancamentos.fields.frequencia')}</Label>
              <Select
                id="frequencia"
                {...register('frequencia')}
                aria-invalid={!!errors.frequencia}
              >
                <option value="">Selecione uma frequência</option>
                {Object.entries(frequenciaLabels).map(([key, label]) => (
                  <option key={key} value={key}>
                    {label}
                  </option>
                ))}
              </Select>
              {errors.frequencia && (
                <p className="text-sm text-red-600">{errors.frequencia.message as string}</p>
              )}
            </div>

            {/* Data de Início */}
            <div className="space-y-2">
              <Label htmlFor="dataInicio">Data de Início</Label>
              <input
                id="dataInicio"
                type="date"
                {...register('dataInicio')}
                className="flex w-full rounded-md border border-slate-300 bg-white px-3 py-2 text-base placeholder:text-slate-500 focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-slate-400"
                aria-invalid={!!errors.dataInicio}
              />
              {errors.dataInicio && (
                <p className="text-sm text-red-600">{errors.dataInicio.message as string}</p>
              )}
            </div>

            {/* Data de Fim (opcional) */}
            <div className="space-y-2">
              <Label htmlFor="dataFim">Data de Fim (opcional)</Label>
              <input
                id="dataFim"
                type="date"
                {...register('dataFim')}
                className="flex w-full rounded-md border border-slate-300 bg-white px-3 py-2 text-base placeholder:text-slate-500 focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-slate-400"
              />
            </div>

            {/* Gerar Ocorrências Até (opcional) */}
            <div className="space-y-2">
              <Label htmlFor="gerarOcorrenciasAte">Gerar Ocorrências Até (opcional)</Label>
              <input
                id="gerarOcorrenciasAte"
                type="date"
                {...register('gerarOcorrenciasAte')}
                className="flex w-full rounded-md border border-slate-300 bg-white px-3 py-2 text-base placeholder:text-slate-500 focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-slate-400"
              />
              <p className="text-xs text-slate-600">
                Deixe em branco para gerar apenas a definição da recorrência
              </p>
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

            {/* Observações */}
            <div className="space-y-2">
              <Label htmlFor="observacoes">Observações (opcional)</Label>
              <textarea
                id="observacoes"
                placeholder="Adicione observações"
                {...register('observacoes')}
                className="flex min-h-24 w-full rounded-md border border-slate-300 bg-white px-3 py-2 text-base placeholder:text-slate-500 focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-slate-400"
              />
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
                {isPending ? 'Criando...' : 'Criar Recorrência'}
              </Button>
            </div>
          </form>
        </CardContent>
      </Card>
    </div>
  )
}
