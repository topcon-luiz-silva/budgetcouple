import { describe, it, expect } from 'vitest'
import { resolveBaseUrl } from '../api'

describe('resolveBaseUrl', () => {
  it('should use provided URL with /api/v1 suffix', () => {
    const result = resolveBaseUrl('http://localhost:3000')
    expect(result).toBe('http://localhost:3000/api/v1')
  })

  it('should not duplicate /api/v1 when already present', () => {
    const result = resolveBaseUrl('http://localhost:3000/api/v1')
    expect(result).toBe('http://localhost:3000/api/v1')
  })

  it('should handle trailing slashes', () => {
    const result = resolveBaseUrl('http://localhost:3000/')
    expect(result).toBe('http://localhost:3000/api/v1')
  })

  it('should handle multiple trailing slashes', () => {
    const result = resolveBaseUrl('http://localhost:3000///')
    expect(result).toBe('http://localhost:3000/api/v1')
  })

  it('should handle URL with /api/v1 and trailing slash', () => {
    const result = resolveBaseUrl('http://localhost:3000/api/v1/')
    expect(result).toBe('http://localhost:3000/api/v1')
  })

  it('should handle HTTPS URLs', () => {
    const result = resolveBaseUrl('https://api.example.com')
    expect(result).toBe('https://api.example.com/api/v1')
  })

  it('should handle custom ports', () => {
    const result = resolveBaseUrl('http://localhost:5000')
    expect(result).toBe('http://localhost:5000/api/v1')
  })

  it('should return default when undefined', () => {
    const result = resolveBaseUrl(undefined)
    expect(result).toMatch(/api\/v1$/)
  })

  it('should return with /api/v1 when empty string', () => {
    const result = resolveBaseUrl('')
    expect(result).toMatch(/api\/v1$/)
  })
})
