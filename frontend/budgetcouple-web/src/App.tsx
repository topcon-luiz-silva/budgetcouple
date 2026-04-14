import { Routes, Route, Outlet } from 'react-router-dom'
import { Wallet } from 'lucide-react'
import { PinSetupPage } from '@/features/auth/pages/PinSetupPage'
import { LoginPage } from '@/features/auth/pages/LoginPage'
import { ChangePinPage } from '@/features/auth/pages/ChangePinPage'
import { RequireAuth } from '@/components/RequireAuth'
import { AuthBootstrap } from '@/components/AuthBootstrap'
import { AppShell } from '@/components/AppShell'
import { ContasListPage } from '@/features/contas/pages/ContasListPage'
import { ContaFormPage } from '@/features/contas/pages/ContaFormPage'
import { CartoesListPage } from '@/features/cartoes/pages/CartoesListPage'
import { CartaoFormPage } from '@/features/cartoes/pages/CartaoFormPage'
import { CategoriasListPage } from '@/features/categorias/pages/CategoriasListPage'
import { CategoriaFormPage } from '@/features/categorias/pages/CategoriaFormPage'
import { LancamentosListPage } from '@/features/lancamentos/pages/LancamentosListPage'
import { LancamentoSimplesFormPage } from '@/features/lancamentos/pages/LancamentoSimplesFormPage'
import { LancamentoParceladoFormPage } from '@/features/lancamentos/pages/LancamentoParceladoFormPage'
import { LancamentoRecorrenteFormPage } from '@/features/lancamentos/pages/LancamentoRecorrenteFormPage'
import { FaturasListPage } from '@/features/faturas/pages/FaturasListPage'
import { FaturaDetalhePage } from '@/features/faturas/pages/FaturaDetalhePage'
import { Button } from '@/components/ui/button'

function HomePage() {
  return (
    <div className="space-y-8">
      <div>
        <h1 className="text-4xl font-bold text-slate-900 mb-2">Bem-vindo ao BudgetCouple</h1>
        <p className="text-slate-600 text-lg">App de Gestão de Despesas Pessoais Compartilhadas</p>
      </div>

      <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
        <div className="bg-white rounded-lg border border-slate-200 p-6 shadow-sm hover:shadow-md transition-shadow">
          <div className="flex items-center gap-3 mb-4">
            <Wallet className="h-6 w-6 text-slate-900" />
            <h2 className="text-xl font-semibold text-slate-900">Contas</h2>
          </div>
          <p className="text-slate-600 mb-4">Gerencie suas contas bancárias e carteiras</p>
          <a href="/contas">
            <Button variant="outline">Ir para Contas</Button>
          </a>
        </div>

        <div className="bg-white rounded-lg border border-slate-200 p-6 shadow-sm hover:shadow-md transition-shadow">
          <div className="flex items-center gap-3 mb-4">
            <Wallet className="h-6 w-6 text-slate-900" />
            <h2 className="text-xl font-semibold text-slate-900">Cartões</h2>
          </div>
          <p className="text-slate-600 mb-4">Gerencie seus cartões de crédito e débito</p>
          <a href="/cartoes">
            <Button variant="outline">Ir para Cartões</Button>
          </a>
        </div>

        <div className="bg-white rounded-lg border border-slate-200 p-6 shadow-sm hover:shadow-md transition-shadow">
          <div className="flex items-center gap-3 mb-4">
            <Wallet className="h-6 w-6 text-slate-900" />
            <h2 className="text-xl font-semibold text-slate-900">Categorias</h2>
          </div>
          <p className="text-slate-600 mb-4">Organize suas despesas e receitas</p>
          <a href="/categorias">
            <Button variant="outline">Ir para Categorias</Button>
          </a>
        </div>
      </div>
    </div>
  )
}

function HealthPage() {
  return <div className="p-8">OK</div>
}

function AuthenticatedLayout() {
  return (
    <AppShell>
      <Outlet />
    </AppShell>
  )
}

export default function App() {
  return (
    <AuthBootstrap>
      <Routes>
        <Route path="/setup-pin" element={<PinSetupPage />} />
        <Route path="/login" element={<LoginPage />} />
        <Route path="/health" element={<HealthPage />} />

        <Route
          element={
            <RequireAuth>
              <AuthenticatedLayout />
            </RequireAuth>
          }
        >
          <Route path="/" element={<HomePage />} />
          <Route path="/contas" element={<ContasListPage />} />
          <Route path="/contas/novo" element={<ContaFormPage />} />
          <Route path="/contas/:id/editar" element={<ContaFormPage />} />
          <Route path="/cartoes" element={<CartoesListPage />} />
          <Route path="/cartoes/novo" element={<CartaoFormPage />} />
          <Route path="/cartoes/:id/editar" element={<CartaoFormPage />} />
          <Route path="/cartoes/:cartaoId/faturas" element={<FaturasListPage />} />
          <Route path="/cartoes/:cartaoId/faturas/:competencia" element={<FaturaDetalhePage />} />
          <Route path="/categorias" element={<CategoriasListPage />} />
          <Route path="/categorias/novo" element={<CategoriaFormPage />} />
          <Route path="/categorias/:id/editar" element={<CategoriaFormPage />} />
          <Route path="/lancamentos" element={<LancamentosListPage />} />
          <Route path="/lancamentos/novo/simples" element={<LancamentoSimplesFormPage />} />
          <Route path="/lancamentos/novo/parcelado" element={<LancamentoParceladoFormPage />} />
          <Route path="/lancamentos/novo/recorrente" element={<LancamentoRecorrenteFormPage />} />
          <Route path="/lancamentos/:id/editar" element={<LancamentoSimplesFormPage />} />
          <Route path="/settings/pin" element={<ChangePinPage />} />
        </Route>
      </Routes>
    </AuthBootstrap>
  )
}
