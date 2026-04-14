import type { ReactNode } from 'react'
import { Link, useLocation, useNavigate } from 'react-router-dom'
import {
  Wallet,
  CreditCard,
  Tag,
  Settings,
  LogOut,
  ArrowRightLeft,
  LayoutDashboard,
  Download,
  Filter,
} from 'lucide-react'
import { useAuthStore } from '@/stores/useAuthStore'
import { Button } from '@/components/ui/button'
import { PWAInstallPrompt } from '@/components/PWAInstallPrompt'

interface NavItem {
  label: string
  icon: ReactNode
  href: string
}

export function AppShell({ children }: { children: ReactNode }) {
  const location = useLocation()
  const navigate = useNavigate()
  const { clear } = useAuthStore()

  const navItems: NavItem[] = [
    { label: 'Dashboard', icon: <LayoutDashboard className="h-5 w-5" />, href: '/' },
    { label: 'Contas', icon: <Wallet className="h-5 w-5" />, href: '/contas' },
    { label: 'Cartões', icon: <CreditCard className="h-5 w-5" />, href: '/cartoes' },
    { label: 'Categorias', icon: <Tag className="h-5 w-5" />, href: '/categorias' },
    { label: 'Lançamentos', icon: <ArrowRightLeft className="h-5 w-5" />, href: '/lancamentos' },
    { label: 'Importar', icon: <Download className="h-5 w-5" />, href: '/importacao' },
    { label: 'Regras', icon: <Filter className="h-5 w-5" />, href: '/regras' },
    { label: 'Configurações', icon: <Settings className="h-5 w-5" />, href: '/settings/pin' },
  ]

  const handleLogout = () => {
    clear()
    navigate('/login')
  }

  return (
    <div className="min-h-screen flex bg-slate-50">
      <PWAInstallPrompt />

      {/* Skip Link for Accessibility */}
      <a
        href="#main-content"
        className="absolute top-0 left-0 z-50 px-4 py-2 bg-blue-600 text-white rounded-br-md focus:outline-none focus:ring-2 focus:ring-blue-500 -translate-x-full focus:translate-x-0 transition-transform"
      >
        Ir para conteúdo principal
      </a>

      {/* Sidebar */}
      <aside className="w-64 bg-slate-900 text-white shadow-lg" role="navigation" aria-label="Menu de navegação">
        <div className="p-6 flex items-center gap-2 border-b border-slate-800">
          <Wallet className="h-6 w-6" aria-hidden="true" />
          <h1 className="text-lg font-bold">BudgetCouple</h1>
        </div>

        <nav className="p-4 space-y-2">
          {navItems.map((item) => {
            const isActive = location.pathname === item.href || location.pathname.startsWith(item.href + '/')
            return (
              <Link
                key={item.href}
                to={item.href}
                className={`flex items-center gap-3 px-4 py-3 rounded-md transition-colors focus:outline-none focus:ring-2 focus:ring-blue-500 ${
                  isActive
                    ? 'bg-slate-700 text-white'
                    : 'text-slate-300 hover:bg-slate-800 hover:text-white'
                }`}
                aria-current={isActive ? 'page' : undefined}
              >
                <span aria-hidden="true">{item.icon}</span>
                <span className="text-sm font-medium">{item.label}</span>
              </Link>
            )
          })}
        </nav>

        <div className="absolute bottom-0 left-0 right-0 p-4 bg-slate-800 border-t border-slate-700">
          <Button
            onClick={handleLogout}
            variant="ghost"
            className="w-full justify-start text-slate-300 hover:text-white hover:bg-slate-700 focus:outline-none focus:ring-2 focus:ring-blue-500"
            aria-label="Fazer logout"
          >
            <LogOut className="h-5 w-5 mr-3" aria-hidden="true" />
            Sair
          </Button>
        </div>
      </aside>

      {/* Main Content */}
      <div className="flex-1 flex flex-col">
        {/* Header */}
        <header className="bg-white border-b border-slate-200 shadow-sm">
          <div className="h-16 px-8 flex items-center justify-between">
            <h2 className="text-xl font-semibold text-slate-900">BudgetCouple</h2>
            <div className="text-sm text-slate-600" role="status" aria-live="polite">
              Usuário Autenticado
            </div>
          </div>
        </header>

        {/* Content */}
        <main id="main-content" className="flex-1 overflow-auto p-8" role="main">
          {children}
        </main>
      </div>
    </div>
  )
}
