export const NotificationChannel = {
  Email: 'Email' as const,
  WebPush: 'WebPush' as const,
  Telegram: 'Telegram' as const,
} as const;
export type NotificationChannel = typeof NotificationChannel[keyof typeof NotificationChannel];

export const NotificationStatus = {
  Success: 'Success' as const,
  Failed: 'Failed' as const,
  Pending: 'Pending' as const,
} as const;
export type NotificationStatus = typeof NotificationStatus[keyof typeof NotificationStatus];

export const NotificationType = {
  VencimentoProximo: 'VencimentoProximo' as const,
  AlertaOrcamento: 'AlertaOrcamento' as const,
  FaturaProxima: 'FaturaProxima' as const,
} as const;
export type NotificationType = typeof NotificationType[keyof typeof NotificationType];

export interface NotificationPreferences {
  emailHabilitado: boolean;
  emailEndereco?: string;
  webPushHabilitado: boolean;
  telegramHabilitado: boolean;
  telegramChatId?: string;
  notificarVencimentos1Dia: boolean;
  notificarVencimentosDia: boolean;
  notificarAlertasOrcamento: boolean;
  notificarFaturas: boolean;
}

export interface NotificationHistory {
  id: string;
  canal: NotificationChannel;
  tipo: NotificationType;
  titulo: string;
  corpo: string;
  status: NotificationStatus;
  erro?: string;
  enviadoEm: string;
}

export interface WebPushSubscription {
  endpoint: string;
  keys: {
    auth: string;
    p256dh: string;
  };
}
