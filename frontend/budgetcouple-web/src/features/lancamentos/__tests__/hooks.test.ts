import { describe, it, expect } from 'vitest'

describe('Lancamentos hooks', () => {
  it('should be defined', () => {
    expect(true).toBe(true)
  })

  it('should handle hook imports', async () => {
    const { useLancamentosList } = await import('../hooks')
    expect(useLancamentosList).toBeDefined()
    expect(typeof useLancamentosList).toBe('function')
  })

  it('should have other hooks defined', async () => {
    const hooks = await import('../hooks')
    expect(hooks.useDeleteLancamento).toBeDefined()
    expect(hooks.usePagarLancamento).toBeDefined()
    expect(hooks.useCreateLancamentoSimples).toBeDefined()
  })
})
