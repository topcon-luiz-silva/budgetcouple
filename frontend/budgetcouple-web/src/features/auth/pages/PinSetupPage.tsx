import { useForm } from 'react-hook-form'
import { z } from 'zod'
import { zodResolver } from '@hookform/resolvers/zod'
import { useTranslation } from 'react-i18next'
import { Wallet } from 'lucide-react'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card'
import { Alert, AlertDescription, AlertTitle } from '@/components/ui/alert'
import { useSetupPin } from '../hooks'

const pinSchema = z
  .object({
    pin: z
      .string()
      .min(6, 'PIN deve ter no mínimo 6 dígitos')
      .max(8, 'PIN deve ter no máximo 8 dígitos')
      .regex(/^\d+$/, 'PIN deve conter apenas dígitos'),
    pinConfirm: z.string(),
  })
  .refine((data) => data.pin === data.pinConfirm, {
    message: 'Os PINs não conferem',
    path: ['pinConfirm'],
  })

type PinSetupFormData = z.infer<typeof pinSchema>

export function PinSetupPage() {
  const { t } = useTranslation()
  const { mutate: setupPin, isPending, error } = useSetupPin()

  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<PinSetupFormData>({
    resolver: zodResolver(pinSchema),
  })

  const onSubmit = (data: PinSetupFormData) => {
    setupPin(data.pin)
  }

  const serverError = error instanceof Error ? error.message : null

  return (
    <div className="min-h-screen flex flex-col items-center justify-center gap-4 p-4 bg-slate-50">
      <div className="flex items-center gap-3 mb-8">
        <Wallet className="h-8 w-8 text-slate-900" />
        <h1 className="text-3xl font-bold text-slate-900">BudgetCouple</h1>
      </div>

      <Card className="w-full max-w-md">
        <CardHeader>
          <CardTitle>{t('auth.title.setup')}</CardTitle>
          <CardDescription>
            Defina um PIN de 6 a 8 dígitos. Ele será compartilhado pelo casal.
          </CardDescription>
        </CardHeader>
        <CardContent>
          <form onSubmit={handleSubmit(onSubmit)} className="space-y-6">
            {serverError && (
              <Alert variant="destructive">
                <AlertTitle>Erro</AlertTitle>
                <AlertDescription>{serverError}</AlertDescription>
              </Alert>
            )}

            <div className="space-y-2">
              <Label htmlFor="pin">{t('auth.pin')}</Label>
              <Input
                id="pin"
                type="tel"
                inputMode="numeric"
                placeholder="000000"
                {...register('pin')}
                aria-invalid={!!errors.pin}
              />
              {errors.pin && (
                <p className="text-sm text-red-600" role="alert">
                  {errors.pin.message}
                </p>
              )}
            </div>

            <div className="space-y-2">
              <Label htmlFor="pinConfirm">{t('auth.pinConfirm')}</Label>
              <Input
                id="pinConfirm"
                type="tel"
                inputMode="numeric"
                placeholder="000000"
                {...register('pinConfirm')}
                aria-invalid={!!errors.pinConfirm}
              />
              {errors.pinConfirm && (
                <p className="text-sm text-red-600" role="alert">
                  {errors.pinConfirm.message}
                </p>
              )}
            </div>

            <Button type="submit" className="w-full" disabled={isPending}>
              {isPending ? 'Configurando...' : t('auth.submit.setup')}
            </Button>
          </form>
        </CardContent>
      </Card>
    </div>
  )
}
