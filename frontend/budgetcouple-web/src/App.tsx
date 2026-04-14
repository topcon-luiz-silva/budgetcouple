import { Routes, Route, Outlet } from 'react-router-dom'
import { PinSetupPage } from '@/features/auth/pages/PinSetupPage'
import { LoginPage } from '@/features/auth/pages/LoginPage'
import { ChangePinPage } from '@/features/auth/pages/ChangePinPage'
import { RequireAuth } from '@/components/RequireAuth'
import { AuthBootstrap } from '@/components/AuthBootstrap'
import { AppShell } from '@/components/AppShell'
import { DashboardPage } from '@/features/dashboard/pages/DashboardPage'
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
          <Route path="/" element={<DashboardPage />} />
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
