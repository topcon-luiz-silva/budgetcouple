export enum NotificationChannel {
  Email = 'Email',
  WebPush = 'WebPush',
  Telegram = 'Telegram'
}

export enum NotificationStatus {
  Success = 'Success',
  Failed = 'Failed',
  Pending = 'Pending'
}

export enum NotificationType {
  VencimentoProximo = 'VencimentoProximo',
  AlertaOrcamento = 'AlertaOrcamento',
  FaturaProxima = 'FaturaProxima'
}

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
