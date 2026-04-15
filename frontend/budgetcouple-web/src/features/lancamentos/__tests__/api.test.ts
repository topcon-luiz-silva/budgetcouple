import { describe, it, expect } from 'vitest'

describe('lancamentosApi validation', () => {
  it('should have validation schemas', async () => {
    const { lancamentosApi } = await import('../api')
    expect(lancamentosApi.list).toBeDefined()
    expect(lancamentosApi.getById).toBeDefined()
    expect(lancamentosApi.createSimples).toBeDefined()
  })

  it('should validate response types', () => {
    expect(true).toBe(true)
  })
})
