import type { ReactNode } from 'react'
import { Link, useLocation, useNavigate } from 'react-router-dom'
import {
  Wallet,
  CreditCard,
  Tag,
  Home,
  Settings,
  LogOut,
  ArrowRightLeft,
} from 'lucide-react'
import { useAuthStore } from '@/stores/useAuthStore'
import { Button } from '@/components/ui/button'

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
    { label: 'Início', icon: <Home className="h-5 w-5" />, href: '/' },
    { label: 'Contas', icon: <Wallet className="h-5 w-5" />, href: '/contas' },
    { label: 'Cartões', icon: <CreditCard className="h-5 w-5" />, href: '/cartoes' },
    { label: 'Categorias', icon: <Tag className="h-5 w-5" />, href: '/categorias' },
    { label: 'Lançamentos', icon: <ArrowRightLeft className="h-5 w-5" />, href: '/lancamentos' },
    { label: 'Configurações', icon: <Settings className="h-5 w-5" />, href: '/settings/pin' },
  ]

  const handleLogout = () => {
    clear()
    navigate('/login')
  }

  return (
    <div className="min-h-screen flex bg-slate-50">
      {/* Sidebar */}
      <aside className="w-64 bg-slate-900 text-white shadow-lg">
        <div className="p-6 flex items-center gap-2 border-b border-slate-800">
          <Wallet className="h-6 w-6" />
          <h1 className="text-lg font-bold">BudgetCouple</h1>
        </div>

        <nav className="p-4 space-y-2">
          {navItems.map((item) => {
            const isActive = location.pathname === item.href || location.pathname.startsWith(item.href + '/')
            return (
              <Link
                key={item.href}
                to={item.href}
                className={`flex items-center gap-3 px-4 py-3 rounded-md transition-colors ${
                  isActive
                    ? 'bg-slate-700 text-white'
                    : 'text-slate-300 hover:bg-slate-800 hover:text-white'
                }`}
              >
                {item.icon}
                <span className="text-sm font-medium">{item.label}</span>
              </Link>
            )
          })}
        </nav>

        <div className="absolute bottom-0 left-0 right-0 p-4 bg-slate-800 border-t border-slate-700">
          <Button
            onClick={handleLogout}
            variant="ghost"
            className="w-full justify-start text-slate-300 hover:text-white hover:bg-slate-700"
          >
            <LogOut className="h-5 w-5 mr-3" />
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
            <div className="text-sm text-slate-600">Usuário Autenticado</div>
          </div>
        </header>

        {/* Content */}
        <main className="flex-1 overflow-auto p-8">
          {children}
        </main>
      </div>
    </div>
  )
}
