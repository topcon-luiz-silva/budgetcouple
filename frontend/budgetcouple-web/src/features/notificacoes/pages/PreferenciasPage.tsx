import React, { useState, useEffect } from 'react';
import { Card, CardContent, CardHeader, CardTitle } from '../../../components/ui/Card';
import { Button } from '../../../components/ui/Button';
import { Checkbox } from '../../../components/ui/Checkbox';
import { Input } from '../../../components/ui/Input';
import { useNotificacoes } from '../hooks/useNotificacoes';
import { NotificationChannel, NotificationPreferences, NotificationHistory } from '../types';
import { format } from 'date-fns';
import { ptBR } from 'date-fns/locale';
import './PreferenciasPage.css';

export const PreferenciasPage: React.FC = () => {
  const {
    preferences,
    historico,
    loading,
    error,
    updatePreferencias,
    sendTest,
    subscribeWebPush,
  } = useNotificacoes();

  const [formData, setFormData] = useState<NotificationPreferences | null>(null);
  const [testLoading, setTestLoading] = useState<string | null>(null);
  const [successMessage, setSuccessMessage] = useState<string | null>(null);

  useEffect(() => {
    if (preferences) {
      setFormData(preferences);
    }
  }, [preferences]);

  const handlePreferencesChange = (
    field: keyof NotificationPreferences,
    value: boolean | string
  ) => {
    if (formData) {
      setFormData({
        ...formData,
        [field]: value,
      });
    }
  };

  const handleSavePreferences = async () => {
    if (formData) {
      const success = await updatePreferencias(formData);
      if (success) {
        setSuccessMessage('Preferencias atualizadas com sucesso!');
        setTimeout(() => setSuccessMessage(null), 3000);
      }
    }
  };

  const handleSendTest = async (canal: NotificationChannel) => {
    setTestLoading(canal);
    try {
      const success = await sendTest(canal);
      if (success) {
        setSuccessMessage(`Notificacao de teste enviada via ${canal}!`);
        setTimeout(() => setSuccessMessage(null), 3000);
      }
    } finally {
      setTestLoading(null);
    }
  };

  const handleSubscribeWebPush = async () => {
    setTestLoading('webpush');
    try {
      const success = await subscribeWebPush();
      if (success) {
        setSuccessMessage('Notificacoes web ativadas com sucesso!');
        setTimeout(() => setSuccessMessage(null), 3000);
        // Update form
        if (formData) {
          setFormData({
            ...formData,
            webPushHabilitado: true,
          });
        }
      }
    } finally {
      setTestLoading(null);
    }
  };

  if (loading && !formData) {
    return <div className="loading">Carregando...</div>;
  }

  if (!formData) {
    return <div className="error">Erro ao carregar preferencias</div>;
  }

  return (
    <div className="preferencias-page">
      <h1>Configuracoes de Notificacoes</h1>

      {error && <div className="error-alert">{error}</div>}
      {successMessage && <div className="success-alert">{successMessage}</div>}

      <div className="content-grid">
        {/* Email Section */}
        <Card>
          <CardHeader>
            <CardTitle>Email</CardTitle>
          </CardHeader>
          <CardContent>
            <div className="form-group">
              <label>
                <Checkbox
                  checked={formData.emailHabilitado}
                  onChange={(e) =>
                    handlePreferencesChange('emailHabilitado', e.target.checked)
                  }
                />
                Ativar notificacoes por email
              </label>
            </div>

            {formData.emailHabilitado && (
              <div className="form-group">
                <label htmlFor="email">Endereco de email:</label>
                <Input
                  id="email"
                  type="email"
                  value={formData.emailEndereco || ''}
                  onChange={(e) =>
                    handlePreferencesChange('emailEndereco', e.target.value)
                  }
                  placeholder="seu@email.com"
                />
              </div>
            )}

            <Button
              onClick={() => handleSendTest(NotificationChannel.Email)}
              disabled={!formData.emailHabilitado || testLoading === 'Email'}
              variant="secondary"
            >
              Enviar Teste
            </Button>
          </CardContent>
        </Card>

        {/* WebPush Section */}
        <Card>
          <CardHeader>
            <CardTitle>Notificacoes Web</CardTitle>
          </CardHeader>
          <CardContent>
            <div className="form-group">
              <label>
                <Checkbox
                  checked={formData.webPushHabilitado}
                  onChange={(e) =>
                    handlePreferencesChange('webPushHabilitado', e.target.checked)
                  }
                />
                Ativar notificacoes web push
              </label>
            </div>

            {!formData.webPushHabilitado && (
              <Button
                onClick={handleSubscribeWebPush}
                disabled={testLoading === 'webpush'}
                variant="primary"
              >
                Ativar Notificacoes Web
              </Button>
            )}

            {formData.webPushHabilitado && (
              <Button
                onClick={() => handleSendTest(NotificationChannel.WebPush)}
                disabled={testLoading === 'WebPush'}
                variant="secondary"
              >
                Enviar Teste
              </Button>
            )}
          </CardContent>
        </Card>

        {/* Telegram Section */}
        <Card>
          <CardHeader>
            <CardTitle>Telegram</CardTitle>
          </CardHeader>
          <CardContent>
            <div className="form-group">
              <label>
                <Checkbox
                  checked={formData.telegramHabilitado}
                  onChange={(e) =>
                    handlePreferencesChange('telegramHabilitado', e.target.checked)
                  }
                />
                Ativar notificacoes Telegram
              </label>
            </div>

            {formData.telegramHabilitado && (
              <>
                <div className="form-group">
                  <label htmlFor="chatId">Chat ID do Telegram:</label>
                  <Input
                    id="chatId"
                    type="text"
                    value={formData.telegramChatId || ''}
                    onChange={(e) =>
                      handlePreferencesChange('telegramChatId', e.target.value)
                    }
                    placeholder="Seu Chat ID"
                  />
                  <small>
                    <a href="https://t.me/userinfobot" target="_blank" rel="noreferrer">
                      Clique aqui para obter seu Chat ID
                    </a>
                  </small>
                </div>
              </>
            )}

            <Button
              onClick={() => handleSendTest(NotificationChannel.Telegram)}
              disabled={!formData.telegramHabilitado || testLoading === 'Telegram'}
              variant="secondary"
            >
              Enviar Teste
            </Button>
          </CardContent>
        </Card>

        {/* Notification Types */}
        <Card>
          <CardHeader>
            <CardTitle>Tipos de Notificacoes</CardTitle>
          </CardHeader>
          <CardContent>
            <div className="form-group">
              <label>
                <Checkbox
                  checked={formData.notificarVencimentos1Dia}
                  onChange={(e) =>
                    handlePreferencesChange('notificarVencimentos1Dia', e.target.checked)
                  }
                />
                Vencimentos em 1 dia
              </label>
            </div>

            <div className="form-group">
              <label>
                <Checkbox
                  checked={formData.notificarVencimentosDia}
                  onChange={(e) =>
                    handlePreferencesChange('notificarVencimentosDia', e.target.checked)
                  }
                />
                Vencimentos no dia
              </label>
            </div>

            <div className="form-group">
              <label>
                <Checkbox
                  checked={formData.notificarAlertasOrcamento}
                  onChange={(e) =>
                    handlePreferencesChange('notificarAlertasOrcamento', e.target.checked)
                  }
                />
                Alertas de orcamento (>=80% gasto)
              </label>
            </div>

            <div className="form-group">
              <label>
                <Checkbox
                  checked={formData.notificarFaturas}
                  onChange={(e) =>
                    handlePreferencesChange('notificarFaturas', e.target.checked)
                  }
                />
                Faturas proximas do fechamento/vencimento
              </label>
            </div>
          </CardContent>
        </Card>
      </div>

      {/* Save Button */}
      <div className="save-section">
        <Button
          onClick={handleSavePreferences}
          disabled={loading}
          variant="primary"
        >
          {loading ? 'Salvando...' : 'Salvar Preferencias'}
        </Button>
      </div>

      {/* Recent History */}
      <Card className="history-card">
        <CardHeader>
          <CardTitle>Historico Recente</CardTitle>
        </CardHeader>
        <CardContent>
          {historico.length === 0 ? (
            <p>Nenhuma notificacao enviada ainda</p>
          ) : (
            <div className="history-list">
              {historico.map((item) => (
                <div key={item.id} className={`history-item status-${item.status.toLowerCase()}`}>
                  <div className="history-header">
                    <strong>{item.titulo}</strong>
                    <span className="channel-badge">{item.canal}</span>
                  </div>
                  <p>{item.corpo}</p>
                  <div className="history-footer">
                    <small>
                      {format(new Date(item.enviadoEm), "dd 'de' MMMM 'as' HH:mm", {
                        locale: ptBR,
                      })}
                    </small>
                    {item.erro && <small className="error">{item.erro}</small>}
                  </div>
                </div>
              ))}
            </div>
          )}
        </CardContent>
      </Card>
    </div>
  );
};

export default PreferenciasPage;
