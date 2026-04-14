import * as React from "react"
import { cn } from "@/lib/utils"

const buttonVariants = {
  default: "bg-slate-900 text-white hover:bg-slate-800 active:bg-slate-700",
  outline: "border border-slate-300 bg-white text-slate-900 hover:bg-slate-50 active:bg-slate-100",
  ghost: "text-slate-900 hover:bg-slate-100 active:bg-slate-200",
  destructive: "bg-red-600 text-white hover:bg-red-700 active:bg-red-800",
}

const buttonSizes = {
  sm: "px-3 py-1.5 text-sm h-8",
  default: "px-4 py-2 text-base h-10",
  lg: "px-6 py-3 text-lg h-12",
}

export interface ButtonProps
  extends React.ButtonHTMLAttributes<HTMLButtonElement> {
  variant?: keyof typeof buttonVariants
  size?: keyof typeof buttonSizes
}

const Button = React.forwardRef<HTMLButtonElement, ButtonProps>(
  ({ className, variant = "default", size = "default", ...props }, ref) => (
    <button
      className={cn(
        "inline-flex items-center justify-center rounded-md font-medium transition-colors focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-slate-400 disabled:opacity-50 disabled:cursor-not-allowed",
        buttonVariants[variant],
        buttonSizes[size],
        className
      )}
      ref={ref}
      {...props}
    />
  )
)
Button.displayName = "Button"

export { Button, buttonVariants, buttonSizes }
