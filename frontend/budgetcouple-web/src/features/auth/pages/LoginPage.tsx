import { useForm } from 'react-hook-form'
import { z } from 'zod'
import { zodResolver } from '@hookform/resolvers/zod'
import { useTranslation } from 'react-i18next'
import { Wallet } from 'lucide-react'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card'
import { Alert, AlertDescription, AlertTitle } from '@/components/ui/alert'
import { useLogin } from '../hooks'
import { AxiosError } from 'axios'

const pinSchema = z.object({
  pin: z
    .string()
    .min(6, 'PIN deve ter no mínimo 6 dígitos')
    .max(8, 'PIN deve ter no máximo 8 dígitos')
    .regex(/^\d+$/, 'PIN deve conter apenas dígitos'),
})

type LoginFormData = z.infer<typeof pinSchema>

export function LoginPage() {
  const { t } = useTranslation()
  const { mutate: login, isPending, error } = useLogin()

  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<LoginFormData>({
    resolver: zodResolver(pinSchema),
  })

  const onSubmit = (data: LoginFormData) => {
    login(data.pin)
  }

  let errorMessage: string | null = null
  if (error instanceof AxiosError) {
    if (error.response?.status === 403) {
      const lockedUntil = error.response?.data?.lockedUntil
      if (lockedUntil) {
        const minutes = Math.ceil(
          (new Date(lockedUntil).getTime() - Date.now()) / 60000
        )
        errorMessage = `Acesso bloqueado. Tente novamente em ${minutes} minuto(s).`
      } else {
        errorMessage = 'PIN incorreto. Acesso bloqueado.'
      }
    } else if (error.response?.status === 401) {
      errorMessage = 'PIN incorreto'
    } else {
      errorMessage = error.message
    }
  }

  return (
    <div className="min-h-screen flex flex-col items-center justify-center gap-4 p-4 bg-slate-50">
      <div className="flex items-center gap-3 mb-8">
        <Wallet className="h-8 w-8 text-slate-900" />
        <h1 className="text-3xl font-bold text-slate-900">BudgetCouple</h1>
      </div>

      <Card className="w-full max-w-md">
        <CardHeader>
          <CardTitle>{t('auth.title.login')}</CardTitle>
        </CardHeader>
        <CardContent>
          <form onSubmit={handleSubmit(onSubmit)} className="space-y-6">
            {errorMessage && (
              <Alert variant="destructive">
                <AlertTitle>Erro</AlertTitle>
                <AlertDescription>{errorMessage}</AlertDescription>
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

            <Button type="submit" className="w-full" disabled={isPending}>
              {isPending ? 'Entrando...' : t('auth.submit.login')}
            </Button>
          </form>
        </CardContent>
      </Card>
    </div>
  )
}
