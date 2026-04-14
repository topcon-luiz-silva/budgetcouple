import { NotificationPreferences, NotificationHistory, NotificationChannel, WebPushSubscription } from './types';
import { apiClient } from '../../lib/apiClient';

const BASE_URL = '/api/v1/notificacoes';

export const notificacoesApi = {
  // Preferencias
  getPreferencias: async (): Promise<NotificationPreferences> => {
    const response = await apiClient.get(`${BASE_URL}/preferencias`);
    return response.data;
  },

  updatePreferencias: async (preferences: NotificationPreferences): Promise<NotificationPreferences> => {
    const response = await apiClient.put(`${BASE_URL}/preferencias`, preferences);
    return response.data;
  },

  // Testes
  sendTestNotification: async (canal: NotificationChannel): Promise<boolean> => {
    const response = await apiClient.post(`${BASE_URL}/test?canal=${canal}`);
    return response.data;
  },

  // WebPush
  subscribeWebPush: async (subscription: WebPushSubscription): Promise<boolean> => {
    const response = await apiClient.post(`${BASE_URL}/webpush/subscribe`, subscription);
    return response.data;
  },

  // Historico
  getHistorico: async (limit: number = 20): Promise<NotificationHistory[]> => {
    const response = await apiClient.get(`${BASE_URL}/historico?limit=${limit}`);
    return response.data;
  },
};
