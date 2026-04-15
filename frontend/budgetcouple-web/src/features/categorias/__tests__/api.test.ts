import { describe, it, expect } from 'vitest'

describe('categoriasApi validation', () => {
  it('should have api methods', async () => {
    const { categoriasApi } = await import('../api')
    expect(categoriasApi.list).toBeDefined()
    expect(categoriasApi.getById).toBeDefined()
    expect(categoriasApi.create).toBeDefined()
    expect(categoriasApi.update).toBeDefined()
    expect(categoriasApi.delete).toBeDefined()
  })
})
