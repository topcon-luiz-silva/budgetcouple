import type { ReactNode } from 'react'
import { createContext, useState, useContext } from 'react'
import { cn } from '@/lib/utils'
import { X } from 'lucide-react'

interface DialogContextType {
  open: boolean
  onOpenChange: (open: boolean) => void
}

const DialogContext = createContext<DialogContextType>({ open: false, onOpenChange: () => {} })

interface DialogProps {
  open?: boolean
  onOpenChange?: (open: boolean) => void
  children?: ReactNode
}

export function Dialog({ open, onOpenChange, children }: DialogProps) {
  const [internalOpen, setInternalOpen] = useState(open ?? false)
  const isOpen = open !== undefined ? open : internalOpen

  const handleOpenChange = (newOpen: boolean) => {
    if (onOpenChange) {
      onOpenChange(newOpen)
    } else {
      setInternalOpen(newOpen)
    }
  }

  return (
    <DialogContext.Provider value={{ open: isOpen, onOpenChange: handleOpenChange }}>
      {children}
    </DialogContext.Provider>
  )
}

interface DialogTriggerProps {
  asChild?: boolean
  children: ReactNode
}

export function DialogTrigger({ children }: DialogTriggerProps) {
  const { onOpenChange } = useContext(DialogContext)

  return (
    <div onClick={() => onOpenChange(true)}>
      {children}
    </div>
  )
}

interface DialogContentProps {
  children?: ReactNode
  className?: string
}

export function DialogContent({ children, className }: DialogContentProps) {
  const { open, onOpenChange } = useContext(DialogContext)

  if (!open) return null

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center">
      {/* Backdrop */}
      <div
        className="absolute inset-0 bg-black/50"
        onClick={() => onOpenChange(false)}
      />

      {/* Content */}
      <div
        className={cn(
          'relative bg-white rounded-lg shadow-lg max-w-md w-full mx-4',
          className
        )}
      >
        <button
          onClick={() => onOpenChange(false)}
          className="absolute top-4 right-4 text-slate-500 hover:text-slate-900"
        >
          <X className="h-5 w-5" />
        </button>
        {children}
      </div>
    </div>
  )
}

interface DialogHeaderProps {
  children?: ReactNode
  className?: string
}

export function DialogHeader({ children, className }: DialogHeaderProps) {
  return (
    <div className={cn('flex flex-col space-y-1.5 p-6 border-b border-slate-200', className)}>
      {children}
    </div>
  )
}

interface DialogTitleProps {
  children?: ReactNode
  className?: string
}

export function DialogTitle({ children, className }: DialogTitleProps) {
  return (
    <h2 className={cn('text-lg font-semibold leading-none tracking-tight', className)}>
      {children}
    </h2>
  )
}

interface DialogBodyProps {
  children?: ReactNode
  className?: string
}

export function DialogBody({ children, className }: DialogBodyProps) {
  return <div className={cn('px-6 py-4', className)}>{children}</div>
}

interface DialogFooterProps {
  children?: ReactNode
  className?: string
}

export function DialogFooter({ children, className }: DialogFooterProps) {
  return (
    <div className={cn('flex items-center justify-end gap-3 p-6 border-t border-slate-200', className)}>
      {children}
    </div>
  )
}
