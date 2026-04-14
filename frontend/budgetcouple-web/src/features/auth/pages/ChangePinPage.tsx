import { useForm } from 'react-hook-form'
import { z } from 'zod'
import { zodResolver } from '@hookform/resolvers/zod'
import { useTranslation } from 'react-i18next'
import { ArrowLeft } from 'lucide-react'
import { useNavigate } from 'react-router-dom'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card'
import { Alert, AlertDescription, AlertTitle } from '@/components/ui/alert'
import { useChangePin } from '../hooks'

const changePinSchema = z
  .object({
    pinAtual: z
      .string()
      .min(6, 'PIN atual deve ter no mínimo 6 dígitos')
      .regex(/^\d+$/, 'PIN deve conter apenas dígitos'),
    novoPin: z
      .string()
      .min(6, 'Novo PIN deve ter no mínimo 6 dígitos')
      .max(8, 'Novo PIN deve ter no máximo 8 dígitos')
      .regex(/^\d+$/, 'PIN deve conter apenas dígitos'),
    confirmPin: z.string(),
  })
  .refine((data) => data.novoPin === data.confirmPin, {
    message: 'Os novos PINs não conferem',
    path: ['confirmPin'],
  })

type ChangePinFormData = z.infer<typeof changePinSchema>

export function ChangePinPage() {
  const { t } = useTranslation()
  const navigate = useNavigate()
  const { mutate: changePin, isPending, error } = useChangePin()

  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<ChangePinFormData>({
    resolver: zodResolver(changePinSchema),
  })

  const onSubmit = (data: ChangePinFormData) => {
    changePin({
      pinAtual: data.pinAtual,
      novoPin: data.novoPin,
    })
  }

  const serverError = error instanceof Error ? error.message : null

  return (
    <div className="min-h-screen bg-slate-50 p-4">
      <div className="max-w-md mx-auto pt-6">
        <button
          onClick={() => navigate('/settings')}
          className="flex items-center gap-2 text-slate-600 hover:text-slate-900 mb-6"
        >
          <ArrowLeft className="h-5 w-5" />
          <span>Voltar</span>
        </button>

        <Card>
          <CardHeader>
            <CardTitle>{t('auth.title.changePin')}</CardTitle>
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
                <Label htmlFor="pinAtual">{t('auth.pinCurrent')}</Label>
                <Input
                  id="pinAtual"
                  type="tel"
                  inputMode="numeric"
                  placeholder="000000"
                  {...register('pinAtual')}
                  aria-invalid={!!errors.pinAtual}
                />
                {errors.pinAtual && (
                  <p className="text-sm text-red-600" role="alert">
                    {errors.pinAtual.message}
                  </p>
                )}
              </div>

              <div className="space-y-2">
                <Label htmlFor="novoPin">{t('auth.pinNew')}</Label>
                <Input
                  id="novoPin"
                  type="tel"
                  inputMode="numeric"
                  placeholder="000000"
                  {...register('novoPin')}
                  aria-invalid={!!errors.novoPin}
                />
                {errors.novoPin && (
                  <p className="text-sm text-red-600" role="alert">
                    {errors.novoPin.message}
                  </p>
                )}
              </div>

              <div className="space-y-2">
                <Label htmlFor="confirmPin">{t('auth.pinConfirm')}</Label>
                <Input
                  id="confirmPin"
                  type="tel"
                  inputMode="numeric"
                  placeholder="000000"
                  {...register('confirmPin')}
                  aria-invalid={!!errors.confirmPin}
                />
                {errors.confirmPin && (
                  <p className="text-sm text-red-600" role="alert">
                    {errors.confirmPin.message}
                  </p>
                )}
              </div>

              <div className="flex gap-3">
                <Button
                  type="button"
                  variant="outline"
                  className="flex-1"
                  onClick={() => navigate('/settings')}
                >
                  Cancelar
                </Button>
                <Button type="submit" className="flex-1" disabled={isPending}>
                  {isPending ? 'Alterando...' : t('auth.submit.change')}
                </Button>
              </div>
            </form>
          </CardContent>
        </Card>
      </div>
    </div>
  )
}
