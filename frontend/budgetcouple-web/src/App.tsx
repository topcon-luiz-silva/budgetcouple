import { Routes, Route } from 'react-router-dom'
import { Wallet } from 'lucide-react'
import { PinSetupPage } from '@/features/auth/pages/PinSetupPage'
import { LoginPage } from '@/features/auth/pages/LoginPage'
import { ChangePinPage } from '@/features/auth/pages/ChangePinPage'
import { RequireAuth } from '@/components/RequireAuth'
import { AuthBootstrap } from '@/components/AuthBootstrap'

function HomePage() {
  return (
    <div className="min-h-screen flex flex-col items-center justify-center gap-4 p-8 bg-slate-50">
      <div className="flex items-center gap-3">
        <Wallet className="h-8 w-8 text-slate-900" />
        <h1 className="text-3xl font-bold text-slate-900">BudgetCouple</h1>
      </div>
      <p className="text-slate-600">App de Gestão de Despesas Pessoais Compartilhadas</p>
      <p className="text-sm text-slate-500">Fase 2 concluída — dashboard em breve</p>
    </div>
  )
}

function HealthPage() {
  return <div className="p-8">OK</div>
}

export default function App() {
  return (
    <AuthBootstrap>
      <Routes>
        <Route path="/setup-pin" element={<PinSetupPage />} />
        <Route path="/login" element={<LoginPage />} />
        <Route
          path="/"
          element={
            <RequireAuth>
              <HomePage />
            </RequireAuth>
          }
        />
        <Route
          path="/settings/pin"
          element={
            <RequireAuth>
              <ChangePinPage />
            </RequireAuth>
          }
        />
        <Route path="/health" element={<HealthPage />} />
      </Routes>
    </AuthBootstrap>
  )
}
