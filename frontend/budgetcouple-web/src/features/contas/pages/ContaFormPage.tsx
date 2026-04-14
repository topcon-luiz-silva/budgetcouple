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
import { useContaById, useCreateConta, useUpdateConta } from '../hooks'
import type { ContaFormData, TipoConta } from '../types'
import { contaFormSchema } from '../types'
import { AxiosError } from 'axios'

const tipoContaLabels: Record<TipoConta, string> = {
  CORRENTE: 'Corrente',
  POUPANCA: 'Poupança',
  INVESTIMENTO: 'Investimento',
  CARTEIRA: 'Carteira',
  OUTRA: 'Outra',
}

const ÍCONES_DISPONÍVEIS = ['💰', '🏦', '💳', '🪲', '📱', '👛']

export function ContaFormPage() {
  const { t } = useTranslation()
  const navigate = useNavigate()
  const { id } = useParams<{ id?: string }>()
  const isEditing = !!id

  const { data: contaExistente, isPending: isLoadingConta } = useContaById(id || '')
  const { mutate: createConta, isPending: isCreating, error: createError } = useCreateConta()
  const { mutate: updateConta, isPending: isUpdating, error: updateError } = useUpdateConta(id || '')

  const isPending = isCreating || isUpdating
  const error = createError || updateError

  const {
    register,
    handleSubmit,
    formState: { errors },
    reset,
    watch,
  } = useForm<any>({
    resolver: zodResolver(contaFormSchema),
  })

  const corHex = watch('corHex')

  useEffect(() => {
    if (contaExistente) {
      reset({
        nome: contaExistente.nome,
        tipoConta: contaExistente.tipoConta,
        saldoInicial: contaExistente.saldoInicial,
        corHex: contaExistente.corHex,
        icone: contaExistente.icone,
        observacoes: contaExistente.observacoes,
      })
    }
  }, [contaExistente, reset])

  const onSubmit = (data: ContaFormData) => {
    if (isEditing) {
      updateConta(data, {
        onSuccess: () => navigate('/contas'),
      })
    } else {
      createConta(data, {
        onSuccess: () => navigate('/contas'),
      })
    }
  }

  let errorMessage: string | null = null
  if (error instanceof AxiosError) {
    errorMessage = error.response?.data?.error || error.message
  } else if (error) {
    errorMessage = 'Erro ao processar conta'
  }

  if (isLoadingConta) {
    return (
      <div className="flex items-center justify-center min-h-screen">
        <p className="text-slate-600">{t('common.loading')}</p>
      </div>
    )
  }

  return (
    <div className="max-w-2xl mx-auto space-y-6">
      <button
        onClick={() => navigate('/contas')}
        className="flex items-center gap-2 text-slate-600 hover:text-slate-900 transition-colors"
      >
        <ArrowLeft className="h-5 w-5" />
        Voltar
      </button>

      <Card>
        <CardHeader>
          <CardTitle>{isEditing ? t('contas.edit') : t('contas.new')}</CardTitle>
          <CardDescription>
            {isEditing ? 'Edite os detalhes da conta' : 'Crie uma nova conta'}
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
                placeholder="Ex: Conta Corrente"
                {...register('nome')}
                aria-invalid={!!errors.nome}
              />
              {errors.nome && (
                <p className="text-sm text-red-600">{errors.nome.message as string}</p>
              )}
            </div>

            {/* Tipo de Conta */}
            <div className="space-y-2">
              <Label htmlFor="tipoConta">Tipo de Conta</Label>
              <Select
                id="tipoConta"
                {...register('tipoConta')}
                aria-invalid={!!errors.tipoConta}
              >
                <option value="">Selecione um tipo</option>
                {Object.entries(tipoContaLabels).map(([key, label]) => (
                  <option key={key} value={key}>
                    {label}
                  </option>
                ))}
              </Select>
              {errors.tipoConta && (
                <p className="text-sm text-red-600">{errors.tipoConta.message as string}</p>
              )}
            </div>

            {/* Saldo Inicial */}
            <div className="space-y-2">
              <Label htmlFor="saldoInicial">Saldo Inicial (R$)</Label>
              <Input
                id="saldoInicial"
                type="number"
                step="0.01"
                placeholder="0.00"
                {...register('saldoInicial')}
                aria-invalid={!!errors.saldoInicial}
              />
              {errors.saldoInicial && (
                <p className="text-sm text-red-600">{errors.saldoInicial.message as string}</p>
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
              <div className="flex gap-2">
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

            {/* Observações */}
            <div className="space-y-2">
              <Label htmlFor="observacoes">Observações</Label>
              <textarea
                id="observacoes"
                placeholder="Adicione observações sobre esta conta"
                {...register('observacoes')}
                className="flex min-h-24 w-full rounded-md border border-slate-300 bg-white px-3 py-2 text-base placeholder:text-slate-500 focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-slate-400 disabled:cursor-not-allowed disabled:opacity-50"
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
                onClick={() => navigate('/contas')}
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
