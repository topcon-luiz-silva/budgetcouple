export function formatCurrency(value: number | null | undefined): string {
  const n = typeof value === 'number' && Number.isFinite(value) ? value : 0
  return new Intl.NumberFormat('pt-BR', {
    style: 'currency',
    currency: 'BRL',
  }).format(n)
}

export function formatPercentage(value: number | null | undefined, decimals: number = 0): string {
  const n = typeof value === 'number' && Number.isFinite(value) ? value : 0
  return `${n.toFixed(decimals)}%`
}
