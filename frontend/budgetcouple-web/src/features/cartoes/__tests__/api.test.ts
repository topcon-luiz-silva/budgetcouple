import { describe, it, expect } from 'vitest'

describe('cartoesApi validation', () => {
  it('should have api methods', async () => {
    const { cartoesApi } = await import('../api')
    expect(cartoesApi.list).toBeDefined()
    expect(cartoesApi.getById).toBeDefined()
    expect(cartoesApi.create).toBeDefined()
    expect(cartoesApi.update).toBeDefined()
    expect(cartoesApi.delete).toBeDefined()
  })
})
