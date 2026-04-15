import { useEffect, useState } from 'react'
import { Download, X } from 'lucide-react'
import { Button } from '@/components/ui/button'

export function PWAInstallPrompt() {
  const [installPrompt, setInstallPrompt] = useState<BeforeInstallPromptEvent | null>(null)
  const [showPrompt, setShowPrompt] = useState(false)

  useEffect(() => {
    // Check if already installed
    const isInstalled = window.matchMedia('(display-mode: standalone)').matches

    const handleBeforeInstallPrompt = (e: Event) => {
      e.preventDefault()
      setInstallPrompt(e as BeforeInstallPromptEvent)
      if (!isInstalled) {
        setShowPrompt(true)
      }
    }

    if (!isInstalled) {
      window.addEventListener('beforeinstallprompt', handleBeforeInstallPrompt)
    }

    return () => {
      window.removeEventListener('beforeinstallprompt', handleBeforeInstallPrompt)
    }
  }, [])

  const handleInstall = async () => {
    if (!installPrompt) return

    installPrompt.prompt()
    const { outcome } = await installPrompt.userChoice

    if (outcome === 'accepted') {
      setShowPrompt(false)
      setInstallPrompt(null)
    }
  }

  if (!showPrompt || !installPrompt) return null

  return (
    <div
      className="fixed bottom-4 right-4 bg-white border border-slate-200 rounded-lg shadow-lg p-4 max-w-sm"
      role="complementary"
      aria-label="Instalar aplicativo"
    >
      <div className="flex items-start justify-between mb-3">
        <div className="flex items-start space-x-3 flex-1">
          <Download className="w-5 h-5 text-blue-600 flex-shrink-0 mt-0.5" />
          <div>
            <p className="font-semibold text-sm text-slate-900">
              Instalar BudgetCouple
            </p>
            <p className="text-xs text-slate-600 mt-1">
              Instale nossa aplicação para acesso rápido e funcionalidade offline
            </p>
          </div>
        </div>
        <button
          onClick={() => setShowPrompt(false)}
          className="text-slate-400 hover:text-slate-600 flex-shrink-0"
          aria-label="Fechar"
        >
          <X className="w-4 h-4" />
        </button>
      </div>

      <div className="flex gap-2">
        <Button
          onClick={handleInstall}
          size="sm"
          className="flex-1"
          aria-label="Instalar agora"
        >
          Instalar
        </Button>
        <Button
          onClick={() => setShowPrompt(false)}
          size="sm"
          variant="outline"
          className="flex-1"
          aria-label="Mais tarde"
        >
          Mais tarde
        </Button>
      </div>
    </div>
  )
}

declare global {
  interface Window {
    addEventListener(
      type: 'beforeinstallprompt',
      listener: (e: BeforeInstallPromptEvent) => void
    ): void
  }
}

interface BeforeInstallPromptEvent extends Event {
  prompt: () => Promise<void>
  userChoice: Promise<{ outcome: 'accepted' | 'dismissed' }>
}
