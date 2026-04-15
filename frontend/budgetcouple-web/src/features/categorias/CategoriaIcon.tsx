import * as LucideIcons from 'lucide-react'
import { Tag } from 'lucide-react'
import type { LucideProps } from 'lucide-react'

/**
 * Renders a categoria icon.
 *
 * The `icone` field may contain:
 *  - A lucide-react kebab-case name (e.g. "home", "heart-pulse", "more-horizontal")
 *  - A single emoji character (e.g. "🏠", "💸")
 *  - An empty/invalid value
 *
 * We convert kebab-case to PascalCase and look it up on the lucide-react
 * exports. If it's not a valid lucide icon we render the raw string so
 * emojis still display. Fallback is the generic Tag icon.
 */
function toPascalCase(name: string): string {
  return name
    .split(/[-_\s]/)
    .filter(Boolean)
    .map((part) => part.charAt(0).toUpperCase() + part.slice(1).toLowerCase())
    .join('')
}

interface CategoriaIconProps extends Omit<LucideProps, 'name'> {
  name?: string | null
}

export function CategoriaIcon({ name, size = 20, ...rest }: CategoriaIconProps) {
  if (!name) return <Tag size={size} {...rest} />

  // If it looks like an emoji or any non-ASCII char, render as text
  // Lucide names are always lowercase ASCII + hyphens
  const isLikelyIconName = /^[a-z][a-z0-9-]*$/.test(name)

  if (!isLikelyIconName) {
    return <span style={{ fontSize: size }}>{name}</span>
  }

  const pascal = toPascalCase(name)
  const Icon = (LucideIcons as Record<string, unknown>)[pascal] as
    | React.ComponentType<LucideProps>
    | undefined

  if (!Icon) return <Tag size={size} {...rest} />

  return <Icon size={size} {...rest} />
}
