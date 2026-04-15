import { describe, it, expect } from 'vitest'

describe('contasApi validation', () => {
  it('should have api methods', async () => {
    const { contasApi } = await import('../api')
    expect(contasApi.list).toBeDefined()
    expect(contasApi.getById).toBeDefined()
    expect(contasApi.create).toBeDefined()
    expect(contasApi.update).toBeDefined()
    expect(contasApi.delete).toBeDefined()
  })
})
