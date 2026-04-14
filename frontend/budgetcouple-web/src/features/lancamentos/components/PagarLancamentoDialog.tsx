import { useTranslation } from 'react-i18next'
import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { z } from 'zod'
import { Button } from '@/components/ui/button'
import { Dialog, DialogContent, DialogFooter, DialogHeader, DialogTitle, DialogBody } from '@/components/ui/dialog'
import { Alert, AlertDescription, AlertTitle } from '@/components/ui/alert'
import { Select } from '@/components/ui/select'
import { Label } from '@/components/ui/label'
import { AxiosError } from 'axios'
import { useContasList } from '@/features/contas/hooks'

interface PagarLancamentoDialogProps {
  open: boolean
  onOpenChange: (open: boolean) => void
  onPagar: (dataPagamento: string, contaDebitoId?: string) => void
  isPending: boolean
  error: Error | null
}

const pagarSchema = z.object({
  dataPagamento: z.string(),
  contaDebitoId: z.string().uuid().optional(),
})

type PagarFormData = z.infer<typeof pagarSchema>

export function PagarLancamentoDialog({
  open,
  onOpenChange,
  onPagar,
  isPending,
  error,
}: PagarLancamentoDialogProps) {
  const { t } = useTranslation()
  const { data: contas = [] } = useContasList()

  const {
    register,
    handleSubmit,
    formState: { errors },
    reset,
  } = useForm<PagarFormData>({
    resolver: zodResolver(pagarSchema),
  })

  let errorMessage: string | null = null
  if (error instanceof AxiosError) {
    errorMessage = error.response?.data?.error || error.message
  } else if (error) {
    errorMessage = 'Erro ao pagar lançamento'
  }

  const handlePagar = (data: PagarFormData) => {
    onPagar(data.dataPagamento, data.contaDebitoId)
    reset()
  }

  const handleOpenChange = (newOpen: boolean) => {
    if (!newOpen) {
      reset()
    }
    onOpenChange(newOpen)
  }

  return (
    <Dialog open={open} onOpenChange={handleOpenChange}>
      <DialogContent>
        <DialogHeader>
          <DialogTitle>{t('lancamentos.pagar.title')}</DialogTitle>
        </DialogHeader>
        <form onSubmit={handleSubmit(handlePagar)}>
          <DialogBody>
            {errorMessage && (
              <Alert variant="destructive" className="mb-4">
                <AlertTitle>Erro</AlertTitle>
                <AlertDescription>{errorMessage}</AlertDescription>
              </Alert>
            )}

            <div className="space-y-4">
              {/* Data de Pagamento */}
              <div className="space-y-2">
                <Label htmlFor="dataPagamento">{t('lancamentos.fields.dataPagamento')}</Label>
                <input
                  id="dataPagamento"
                  type="date"
                  {...register('dataPagamento')}
                  className="flex w-full rounded-md border border-slate-300 bg-white px-3 py-2 text-base placeholder:text-slate-500 focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-slate-400 disabled:cursor-not-allowed disabled:opacity-50"
                />
                {errors.dataPagamento && (
                  <p className="text-sm text-red-600">{errors.dataPagamento.message as string}</p>
                )}
              </div>

              {/* Conta de Débito (opcional) */}
              <div className="space-y-2">
                <Label htmlFor="contaDebitoId">Conta de Débito (opcional)</Label>
                <Select id="contaDebitoId" {...register('contaDebitoId')}>
                  <option value="">Selecione uma conta</option>
                  {contas.map((conta) => (
                    <option key={conta.id} value={conta.id}>
                      {conta.nome}
                    </option>
                  ))}
                </Select>
              </div>
            </div>
          </DialogBody>
          <DialogFooter>
            <Button
              type="button"
              variant="outline"
              onClick={() => handleOpenChange(false)}
              disabled={isPending}
            >
              {t('common.cancel')}
            </Button>
            <Button type="submit" disabled={isPending}>
              {isPending ? 'Processando...' : t('lancamentos.pagar.confirm')}
            </Button>
          </DialogFooter>
        </form>
      </DialogContent>
    </Dialog>
  )
}
