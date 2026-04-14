import { Routes, Route, Link } from 'react-router-dom'
import { Wallet } from 'lucide-react'

function Home() {
  return (
    <div className="min-h-screen flex flex-col items-center justify-center gap-4 p-8 bg-slate-50">
      <div className="flex items-center gap-3">
        <Wallet className="h-8 w-8 text-slate-900" />
        <h1 className="text-3xl font-bold text-slate-900">BudgetCouple</h1>
      </div>
      <p className="text-slate-600">App de Gestão de Despesas Pessoais Compartilhadas</p>
      <p className="text-sm text-slate-500">Fase 0 concluída · stack pronta</p>
      <Link to="/health" className="text-blue-600 hover:underline text-sm">/health</Link>
    </div>
  )
}

function Health() {
  return <div className="p-8">OK</div>
}

export default function App() {
  return (
    <Routes>
      <Route path="/" element={<Home />} />
      <Route path="/health" element={<Health />} />
    </Routes>
  )
}
