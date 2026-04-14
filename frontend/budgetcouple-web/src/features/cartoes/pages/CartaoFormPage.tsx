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
import { useCartaoById, useCreateCartao, useUpdateCartao } from '../hooks'
import type { CartaoFormData } from '../types'
import { cartaoFormSchema } from '../types'
import { useContasList } from '@/features/contas/hooks'
import { AxiosError } from 'axios'

const ÍCONES_DISPONÍVEIS = ['💳', '🏦', '💰', '🪲', '📱', '👛']

export function CartaoFormPage() {
  const { t } = useTranslation()
  const navigate = useNavigate()
  const { id } = useParams<{ id?: string }>()
  const isEditing = !!id

  const { data: cartaoExistente, isPending: isLoadingCartao } = useCartaoById(id || '')
  const { data: contas = [] } = useContasList()
  const { mutate: createCartao, isPending: isCreating, error: createError } = useCreateCartao()
  const { mutate: updateCartao, isPending: isUpdating, error: updateError } = useUpdateCartao(id || '')

  const isPending = isCreating || isUpdating
  const error = createError || updateError

  const {
    register,
    handleSubmit,
    formState: { errors },
    reset,
    watch,
  } = useForm<any>({
    resolver: zodResolver(cartaoFormSchema),
  })

  const corHex = watch('corHex')

  useEffect(() => {
    if (cartaoExistente) {
      reset({
        nome: cartaoExistente.nome,
        bandeira: cartaoExistente.bandeira,
        ultimosDigitos: cartaoExistente.ultimosDigitos,
        limite: cartaoExistente.limite,
        diaFechamento: cartaoExistente.diaFechamento,
        diaVencimento: cartaoExistente.diaVencimento,
        contaPagamentoId: cartaoExistente.contaPagamentoId,
        corHex: cartaoExistente.corHex,
        icone: cartaoExistente.icone,
      })
    }
  }, [cartaoExistente, reset])

  const onSubmit = (data: CartaoFormData) => {
    if (isEditing) {
      updateCartao(data, {
        onSuccess: () => navigate('/cartoes'),
      })
    } else {
      createCartao(data, {
        onSuccess: () => navigate('/cartoes'),
      })
    }
  }

  let errorMessage: string | null = null
  if (error instanceof AxiosError) {
    errorMessage = error.response?.data?.error || error.message
  } else if (error) {
    errorMessage = 'Erro ao processar cartão'
  }

  if (isLoadingCartao) {
    return (
      <div className="flex items-center justify-center min-h-screen">
        <p className="text-slate-600">{t('common.loading')}</p>
      </div>
    )
  }

  return (
    <div className="max-w-2xl mx-auto space-y-6">
      <button
        onClick={() => navigate('/cartoes')}
        className="flex items-center gap-2 text-slate-600 hover:text-slate-900 transition-colors"
      >
        <ArrowLeft className="h-5 w-5" />
        Voltar
      </button>

      <Card>
        <CardHeader>
          <CardTitle>{isEditing ? t('cartoes.edit') : t('cartoes.new')}</CardTitle>
          <CardDescription>
            {isEditing ? 'Edite os detalhes do cartão' : 'Crie um novo cartão'}
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
              <Label htmlFor="nome">Nome do Cartão</Label>
              <Input
                id="nome"
                placeholder="Ex: Cartão Principal"
                {...register('nome')}
                aria-invalid={!!errors.nome}
              />
              {errors.nome && (
                <p className="text-sm text-red-600">{errors.nome.message as string}</p>
              )}
            </div>

            {/* Bandeira */}
            <div className="space-y-2">
              <Label htmlFor="bandeira">Bandeira</Label>
              <Input
                id="bandeira"
                placeholder="Ex: Visa, Mastercard, Elo"
                {...register('bandeira')}
                aria-invalid={!!errors.bandeira}
              />
              {errors.bandeira && (
                <p className="text-sm text-red-600">{errors.bandeira.message as string}</p>
              )}
            </div>

            {/* Últimos Dígitos */}
            <div className="space-y-2">
              <Label htmlFor="ultimosDigitos">Últimos 4 Dígitos</Label>
              <Input
                id="ultimosDigitos"
                placeholder="1234"
                maxLength={4}
                {...register('ultimosDigitos')}
                aria-invalid={!!errors.ultimosDigitos}
              />
              {errors.ultimosDigitos && (
                <p className="text-sm text-red-600">{errors.ultimosDigitos.message as string}</p>
              )}
            </div>

            {/* Limite */}
            <div className="space-y-2">
              <Label htmlFor="limite">Limite (R$)</Label>
              <Input
                id="limite"
                type="number"
                step="0.01"
                placeholder="0.00"
                {...register('limite')}
                aria-invalid={!!errors.limite}
              />
              {errors.limite && (
                <p className="text-sm text-red-600">{errors.limite.message as string}</p>
              )}
            </div>

            {/* Dia de Fechamento */}
            <div className="space-y-2">
              <Label htmlFor="diaFechamento">Dia de Fechamento</Label>
              <Input
                id="diaFechamento"
                type="number"
                min="1"
                max="31"
                {...register('diaFechamento')}
                aria-invalid={!!errors.diaFechamento}
              />
              {errors.diaFechamento && (
                <p className="text-sm text-red-600">{errors.diaFechamento.message as string}</p>
              )}
            </div>

            {/* Dia de Vencimento */}
            <div className="space-y-2">
              <Label htmlFor="diaVencimento">Dia de Vencimento</Label>
              <Input
                id="diaVencimento"
                type="number"
                min="1"
                max="31"
                {...register('diaVencimento')}
                aria-invalid={!!errors.diaVencimento}
              />
              {errors.diaVencimento && (
                <p className="text-sm text-red-600">{errors.diaVencimento.message as string}</p>
              )}
            </div>

            {/* Conta de Pagamento */}
            <div className="space-y-2">
              <Label htmlFor="contaPagamentoId">Conta de Pagamento</Label>
              <Select
                id="contaPagamentoId"
                {...register('contaPagamentoId')}
                aria-invalid={!!errors.contaPagamentoId}
              >
                <option value="">Selecione uma conta</option>
                {contas.map((conta) => (
                  <option key={conta.id} value={conta.id}>
                    {conta.nome}
                  </option>
                ))}
              </Select>
              {errors.contaPagamentoId && (
                <p className="text-sm text-red-600">{errors.contaPagamentoId.message as string}</p>
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

            {/* Botões */}
            <div className="flex gap-4 pt-6 border-t border-slate-200">
              <Button
                type="button"
                variant="outline"
                onClick={() => navigate('/cartoes')}
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
