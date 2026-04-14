import { useState, useEffect } from 'react';
import type { NotificationPreferences, NotificationHistory } from '../types';
import { NotificationChannel } from '../types';
import { notificacoesApi } from '../api';

export const useNotificacoes = () => {
  const [preferences, setPreferences] = useState<NotificationPreferences | null>(null);
  const [historico, setHistorico] = useState<NotificationHistory[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  // Fetch preferencias
  const fetchPreferencias = async () => {
    setLoading(true);
    setError(null);
    try {
      const data = await notificacoesApi.getPreferencias();
      setPreferences(data);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Erro ao carregar preferencias');
    } finally {
      setLoading(false);
    }
  };

  // Update preferencias
  const updatePreferencias = async (newPreferences: NotificationPreferences) => {
    setLoading(true);
    setError(null);
    try {
      const data = await notificacoesApi.updatePreferencias(newPreferences);
      setPreferences(data);
      return true;
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Erro ao atualizar preferencias');
      return false;
    } finally {
      setLoading(false);
    }
  };

  // Send test notification
  const sendTest = async (canal: NotificationChannel) => {
    setLoading(true);
    setError(null);
    try {
      const result = await notificacoesApi.sendTestNotification(canal);
      if (result) {
        // Reload historico
        await fetchHistorico();
      }
      return result;
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Erro ao enviar notificacao de teste');
      return false;
    } finally {
      setLoading(false);
    }
  };

  // Fetch historico
  const fetchHistorico = async (limit: number = 20) => {
    setLoading(true);
    setError(null);
    try {
      const data = await notificacoesApi.getHistorico(limit);
      setHistorico(data);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Erro ao carregar historico');
    } finally {
      setLoading(false);
    }
  };

  // Subscribe to WebPush
  const subscribeWebPush = async () => {
    if (!('serviceWorker' in navigator) || !('Notification' in window)) {
      setError('Seu navegador nao suporta notificacoes web');
      return false;
    }

    try {
      const permission = await Notification.requestPermission();
      if (permission !== 'granted') {
        setError('Permissao de notificacao negada');
        return false;
      }

      const registration = await navigator.serviceWorker.ready;
      const subscription = await registration.pushManager.subscribe({
        userVisibleOnly: true,
        applicationServerKey: urlBase64ToUint8Array(
          import.meta.env.VITE_VAPID_PUBLIC_KEY || ''
        ),
      });

      // Send subscription to server
      await notificacoesApi.subscribeWebPush(subscription as any);
      return true;
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Erro ao ativar notificacoes web');
      return false;
    }
  };

  // Initial load
  useEffect(() => {
    fetchPreferencias();
    fetchHistorico();
  }, []);

  return {
    preferences,
    historico,
    loading,
    error,
    fetchPreferencias,
    updatePreferencias,
    fetchHistorico,
    sendTest,
    subscribeWebPush,
  };
};

// Helper function for WebPush
function urlBase64ToUint8Array(base64String: string) {
  const padding = '='.repeat((4 - (base64String.length % 4)) % 4);
  const base64 = (base64String + padding).replace(/\-/g, '+').replace(/_/g, '/');
  const rawData = window.atob(base64);
  const outputArray = new Uint8Array(rawData.length);
  for (let i = 0; i < rawData.length; ++i) {
    outputArray[i] = rawData.charCodeAt(i);
  }
  return outputArray;
}
