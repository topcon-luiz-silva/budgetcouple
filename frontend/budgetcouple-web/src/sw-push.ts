// Service Worker Push Handler para notificacoes web

declare global {
  interface ServiceWorkerGlobalScope {
    addEventListener(type: 'push', listener: (this: ServiceWorkerGlobalScope, ev: PushEvent) => any): void;
    addEventListener(type: 'notificationclick', listener: (this: ServiceWorkerGlobalScope, ev: NotificationEvent) => any): void;
  }
}

self.addEventListener('push', (event: PushEvent) => {
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
    self.registration.showNotification(title, options)
  );
});

self.addEventListener('notificationclick', (event: NotificationEvent) => {
  event.notification.close();

  const urlToOpen = event.notification.data.url || '/';

  event.waitUntil(
    clients.matchAll({
      type: 'window',
      includeUncontrolled: true,
    }).then((clientList) => {
      // Check if there is already a window/tab open with the target URL
      for (let i = 0; i < clientList.length; i++) {
        const client = clientList[i];
        if (client.url === urlToOpen && 'focus' in client) {
          return (client as any).focus();
        }
      }
      // If not, open a new window/tab with the target URL
      if (clients.openWindow) {
        return clients.openWindow(urlToOpen);
      }
    })
  );
});
