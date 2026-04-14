// Service Worker Push Handler para notificacoes web

declare global {
  interface ServiceWorkerGlobalScope {
    addEventListener(type: 'push', listener: (this: ServiceWorkerGlobalScope, ev: any) => void): void;
    addEventListener(type: 'notificationclick', listener: (this: ServiceWorkerGlobalScope, ev: any) => void): void;
  }
}

self.addEventListener('push', (event: any) => {
  const data = event.data?.json() ?? {};
  const title = data.titulo || 'Notificacao';
  const options: NotificationOptions = {
    body: data.corpo || '',
    icon: '/icon-192x192.png',
    badge: '/badge-72x72.png',
    tag: data.tipo || 'notification',
    requireInteraction: false,
    data: {
      url: data.link || '/',
    },
  };

  event.waitUntil(
    (self as any).registration.showNotification(title, options)
  );
});

self.addEventListener('notificationclick', (event: any) => {
  event.notification.close();

  const urlToOpen = event.notification.data.url || '/';

  event.waitUntil(
    (self as any).clients.matchAll({
      type: 'window' as const,
      includeUncontrolled: true,
    }).then((clientList: any[]) => {
      // Check if there is already a window/tab open with the target URL
      for (let i = 0; i < clientList.length; i++) {
        const client = clientList[i];
        if (client.url === urlToOpen && 'focus' in client) {
          return (client as any).focus();
        }
      }
      // If not, open a new window/tab with the target URL
      if ((self as any).clients.openWindow) {
        return (self as any).clients.openWindow(urlToOpen);
      }
    })
  );
});
