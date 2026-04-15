import { describe, it, expect, beforeEach, vi } from 'vitest'
import { resolveBaseUrl } from './api'

describe('resolveBaseUrl', () => {
  beforeEach(() => {
    vi.resetModules()
  })

  it('should handle URL with /api/v1 suffix', () => {
    const result = resolveBaseUrl('https://example.com/api/v1')
    expect(result).toBe('https://example.com/api/v1')
  })

  it('should handle URL without /api/v1 suffix and append it', () => {
    const result = resolveBaseUrl('https://example.com')
    expect(result).toBe('https://example.com/api/v1')
  })

  it('should handle URL with trailing slash without /api/v1', () => {
    const result = resolveBaseUrl('https://example.com/')
    expect(result).toBe('https://example.com/api/v1')
  })

  it('should handle empty string', () => {
    const result = resolveBaseUrl('')
    expect(result).toBe('/api/v1')
  })

  it('should handle URL with port number', () => {
    const result = resolveBaseUrl('https://example.com:3000')
    expect(result).toBe('https://example.com:3000/api/v1')
  })

  it('should handle localhost', () => {
    const result = resolveBaseUrl('http://localhost:5000')
    expect(result).toBe('http://localhost:5000/api/v1')
  })

  it('should not duplicate /api/v1', () => {
    const result = resolveBaseUrl('https://example.com/api/v1/extra')
    expect(result).not.toBe('https://example.com/api/v1/api/v1')
  })
})
