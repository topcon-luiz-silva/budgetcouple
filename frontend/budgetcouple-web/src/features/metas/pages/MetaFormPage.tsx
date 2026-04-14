import { useState, useEffect } from 'react'
import { useNavigate, useParams } from 'react-router-dom'
import { useTranslation } from 'react-i18next'
import { useGetMeta, useCreateMeta, useUpdateMeta } from '../hooks'
import { useListCategorias } from '@/features/categorias/hooks'
import { Button } from '@/components/ui/button'
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { Select } from '@/components/ui/select'
import type { CreateMetaInput, UpdateMetaInput } from '../types'

export const MetaFormPage = () => {
  const navigate = useNavigate()
  const { t } = useTranslation()
  const { id } = useParams<{ id?: string }>()

  const isEdit = !!id
  const { data: meta } = useGetMeta(id || '', { enabled: isEdit })
  const { data: categorias = [] } = useListCategorias()
  const createMutation = useCreateMeta()
  const updateMutation = useUpdateMeta(id || '')

  const [formData, setFormData] = useState({
    nome: '',
    descricao: '',
    tipo: 'ECONOMIA',
    valorAlvo: '',
    dataInicio: '',
    dataTermino: '',
    categoriaId: '',
    percentualAlerta: '80',
  } as {
    nome: string
    descricao: string
    tipo: 'ECONOMIA' | 'REDUCAO_CATEGORIA'
    valorAlvo: string
    dataInicio: string
    dataTermino: string
    categoriaId: string
    percentualAlerta: string
  })

  useEffect(() => {
    if (isEdit && meta) {
      setFormData({
        nome: meta.nome || '',
        descricao: meta.descricao || '',
        tipo: meta.tipo || 'ECONOMIA',
        valorAlvo: meta.valorAlvo?.toString() || '',
        dataInicio: meta.dataInicio || '',
        dataTermino: meta.dataTermino || '',
        categoriaId: meta.categoriaId || '',
        percentualAlerta: meta.percentualAlerta?.toString() || '80',
      })
    }
  }, [meta, isEdit])

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault()

    const payload = {
      nome: formData.nome,
      descricao: formData.descricao,
      tipo: formData.tipo,
      valorAlvo: parseFloat(formData.valorAlvo),
      dataInicio: formData.dataInicio,
      dataTermino: formData.dataTermino,
      categoriaId: formData.tipo === 'ECONOMIA' ? undefined : formData.categoriaId || undefined,
      percentualAlerta: parseInt(formData.percentualAlerta),
    }

    try {
      if (isEdit) {
        const updatePayload: UpdateMetaInput = {
          id: id!,
          tipo: payload.tipo,
          nome: payload.nome,
          descricao: payload.descricao,
          valorAlvo: payload.valorAlvo,
          dataInicio: payload.dataInicio,
          dataTermino: payload.dataTermino,
          categoriaId: payload.categoriaId,
          percentualAlerta: payload.percentualAlerta,
        }
        await updateMutation.mutateAsync(updatePayload)
      } else {
        const createPayload: CreateMetaInput = {
          tipo: payload.tipo,
          nome: payload.nome,
          descricao: payload.descricao,
          valorAlvo: payload.valorAlvo,
          dataInicio: payload.dataInicio,
          dataTermino: payload.dataTermino,
          categoriaId: payload.categoriaId,
          percentualAlerta: payload.percentualAlerta,
        }
        await createMutation.mutateAsync(createPayload)
      }
      navigate('/metas')
    } catch (err) {
      console.error('Erro ao salvar meta:', err)
    }
  }

  const isLoading = createMutation.isPending || updateMutation.isPending

  return (
    <div className="max-w-2xl mx-auto">
      <Card>
        <CardHeader>
          <CardTitle>
            {isEdit ? t('metas.edit') : t('metas.new')}
          </CardTitle>
        </CardHeader>

        <CardContent>
          <form onSubmit={handleSubmit} className="space-y-6">
            <div>
              <Label htmlFor="nome">{t('metas.fields.nome')}</Label>
              <Input
                id="nome"
                value={formData.nome}
                onChange={(e) => setFormData({ ...formData, nome: e.target.value })}
                required
                className="mt-2"
              />
            </div>

            <div>
              <Label htmlFor="descricao">{t('metas.fields.descricao')}</Label>
              <Input
                id="descricao"
                value={formData.descricao}
                onChange={(e) => setFormData({ ...formData, descricao: e.target.value })}
                className="mt-2"
              />
            </div>

            <div>
              <Label htmlFor="tipo">{t('metas.fields.tipo')}</Label>
              <Select
                id="tipo"
                value={formData.tipo}
                onChange={(e) => {
                  const value = e.target.value as 'ECONOMIA' | 'REDUCAO_CATEGORIA'
                  setFormData({ ...formData, tipo: value })
                }}
                className="mt-2"
              >
                <option value="ECONOMIA">{t('metas.types.economia')}</option>
                <option value="REDUCAO_CATEGORIA">{t('metas.types.reducaoCategoria')}</option>
              </Select>
            </div>

            {formData.tipo !== 'ECONOMIA' && (
              <div>
                <Label htmlFor="categoria">{t('metas.fields.categoria')}</Label>
                <Select
                  id="categoria"
                  value={formData.categoriaId}
                  onChange={(e) => setFormData({ ...formData, categoriaId: e.target.value })}
                  className="mt-2"
                  required
                >
                  <option value="">{t('common.select') || 'Selecione'}</option>
                  {categorias.map((cat) => (
                    <option key={cat.id} value={cat.id}>
                      {cat.nome}
                    </option>
                  ))}
                </Select>
              </div>
            )}

            <div>
              <Label htmlFor="valorAlvo">{t('metas.fields.alvo')}</Label>
              <Input
                id="valorAlvo"
                type="number"
                step="0.01"
                value={formData.valorAlvo}
                onChange={(e) => setFormData({ ...formData, valorAlvo: e.target.value })}
                required
                className="mt-2"
              />
            </div>

            <div>
              <Label htmlFor="dataInicio">{t('metas.fields.dataInicio')}</Label>
              <Input
                id="dataInicio"
                type="date"
                value={formData.dataInicio}
                onChange={(e) => setFormData({ ...formData, dataInicio: e.target.value })}
                required
                className="mt-2"
              />
            </div>

            <div>
              <Label htmlFor="dataTermino">{t('metas.fields.dataTermino')}</Label>
              <Input
                id="dataTermino"
                type="date"
                value={formData.dataTermino}
                onChange={(e) => setFormData({ ...formData, dataTermino: e.target.value })}
                required
                className="mt-2"
              />
            </div>

            <div>
              <Label htmlFor="percentualAlerta">{t('metas.fields.percentualAlerta')}</Label>
              <Input
                id="percentualAlerta"
                type="number"
                min="0"
                max="100"
                value={formData.percentualAlerta}
                onChange={(e) => setFormData({ ...formData, percentualAlerta: e.target.value })}
                className="mt-2"
              />
            </div>

            <div className="flex gap-2 pt-4">
              <Button
                type="button"
                variant="outline"
                onClick={() => navigate('/metas')}
                disabled={isLoading}
              >
                {t('common.cancel')}
              </Button>
              <Button type="submit" disabled={isLoading}>
                {isLoading ? t('common.loading') : t('common.save')}
              </Button>
            </div>
          </form>
        </CardContent>
      </Card>
    </div>
  )
}
